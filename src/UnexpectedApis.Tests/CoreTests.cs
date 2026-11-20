using UnexpectedApis.Core;
using Xunit;

namespace UnexpectedApis.Tests;

public class XamlSnippetTests
{
    [Fact]
    public void No_attributes_self_closes() =>
        Assert.Equal("<Button />", XamlSnippet.Build("Button", []));

    [Fact]
    public void Two_short_attributes_stay_on_one_line() =>
        Assert.Equal(
            "<Button Content=\"Save\" IsEnabled=\"False\" />",
            XamlSnippet.Build("Button", [new("Content", "Save"), new("IsEnabled", "False")]));

    [Fact]
    public void Three_attributes_wrap_one_per_line() =>
        Assert.Equal(
            "<Slider Header=\"Volume\"\n        Minimum=\"10\"\n        Maximum=\"20\" />",
            XamlSnippet.Build("Slider", [new("Header", "Volume"), new("Minimum", "10"), new("Maximum", "20")]));

    [Fact]
    public void Body_is_indented_between_open_and_close_tags() =>
        Assert.Equal(
            "<ComboBox Header=\"Size\">\n    <x:String>Small</x:String>\n</ComboBox>",
            XamlSnippet.Build("ComboBox", [new("Header", "Size")], ["<x:String>Small</x:String>"]));

    [Fact]
    public void Values_are_escaped_but_markup_extensions_and_entities_survive()
    {
        Assert.Equal("<A B=\"x &amp; &quot;y&quot; &lt;z\" />", XamlSnippet.Build("A", [new("B", "x & \"y\" <z")]));
        Assert.Equal("<A B=\"{StaticResource Foo}\" />", XamlSnippet.Build("A", [new("B", "{StaticResource Foo}")]));
        Assert.Equal("<A B=\"&#xE8FB;\" />", XamlSnippet.Build("A", [new("B", "&#xE8FB;")]));
    }
}

public class XamlDefaultsTests
{
    [Theory]
    [InlineData(true, "True", true)]
    [InlineData(false, "True", false)]
    [InlineData(0d, "0", true)]
    [InlineData(40d, "0", false)]
    [InlineData(2.5d, "2.5", true)]
    [InlineData(5, "5", true)]
    [InlineData(DayOfWeek.Monday, "Monday", true)]
    [InlineData(DayOfWeek.Monday, "Tuesday", false)]
    [InlineData("Save", "Save", true)]
    public void Compares_the_framework_value_with_the_xaml_text(object frameworkValue, string xaml, bool expected) =>
        Assert.Equal(expected, XamlDefaults.Matches(frameworkValue, xaml));

    [Fact]
    public void Unset_values_match_only_empty_text()
    {
        Assert.True(XamlDefaults.Matches(null, ""));
        Assert.False(XamlDefaults.Matches(null, "Volume"));
    }

    [Fact]
    public void Unset_size_never_matches_a_number() => Assert.False(XamlDefaults.Matches(double.NaN, "96"));
}

public class CatalogSearchTests
{
    private static readonly SearchEntry<string>[] Items =
    [
        new("NumberBox", "NumberBox", ["numeric", "spinner"]),
        new("TextBox", "TextBox", ["input"]),
        new("ToggleSwitch", "ToggleSwitch", ["boolean", "on off"]),
        new("ToggleButton", "ToggleButton", ["checked"]),
        new("Button", "Button", ["click"]),
    ];

    private static List<string> Rank(string q) => CatalogSearch.Rank(q, Items).ToList();

    [Fact]
    public void Empty_query_returns_nothing() => Assert.Empty(Rank("  "));

    [Fact]
    public void Exact_title_beats_substring()
    {
        var results = Rank("button");
        Assert.Equal("Button", results[0]);
        Assert.Contains("ToggleButton", results);
    }

    [Fact]
    public void Capital_initials_match() => Assert.Equal("ToggleSwitch", Rank("ts")[0]);

    [Fact]
    public void Keywords_match_after_titles() => Assert.Equal(["NumberBox"], Rank("spinner"));

    [Fact]
    public void Every_term_must_match()
    {
        Assert.Equal(["ToggleSwitch"], Rank("toggle boolean"));
        Assert.Empty(Rank("toggle spinner"));
    }

    [Fact]
    public void Scattered_letters_still_find_a_title() => Assert.Equal("NumberBox", Rank("nmbx")[0]);
}

public class XamlTokenizerTests
{
    [Fact]
    public void Tokens_round_trip_to_the_source()
    {
        const string source = "<Button Content=\"Save\">\n    <x:String>Hi</x:String>\n</Button>";
        Assert.Equal(source, string.Concat(XamlTokenizer.Tokenize(source).Select(t => t.Text)));
    }

    [Fact]
    public void Classifies_tag_attribute_and_value()
    {
        var tokens = XamlTokenizer.Tokenize("<Button Content=\"Save\" />").Where(t => !string.IsNullOrWhiteSpace(t.Text)).ToList();
        Assert.Contains(tokens, t => t is { Kind: XamlTokenKind.Tag, Text: "Button" });
        Assert.Contains(tokens, t => t is { Kind: XamlTokenKind.AttributeName, Text: "Content" });
        Assert.Contains(tokens, t => t is { Kind: XamlTokenKind.AttributeValue, Text: "\"Save\"" });
    }

    [Fact]
    public void Text_between_tags_is_plain()
    {
        var tokens = XamlTokenizer.Tokenize("<a>Hello</a>");
        Assert.Contains(tokens, t => t is { Kind: XamlTokenKind.Text, Text: "Hello" });
    }

    [Fact]
    public void Comments_are_one_token() =>
        Assert.Equal(XamlTokenKind.Comment, Assert.Single(XamlTokenizer.Tokenize("<!-- <a b=\"c\"> -->")).Kind);
}

public class UserLibraryTests
{
    [Fact]
    public void Toggle_adds_then_removes_a_favorite()
    {
        UserLibrary library = new();
        Assert.True(library.ToggleFavorite("button"));
        Assert.Contains("button", library.Favorites);
        Assert.False(library.ToggleFavorite("button"));
        Assert.Empty(library.Favorites);
    }

    [Fact]
    public void Recents_move_to_front_without_duplicates_and_are_capped()
    {
        UserLibrary library = new();
        for (var i = 0; i < 12; i++)
        {
            library.Touch($"s{i}");
        }

        library.Touch("s5");

        Assert.Equal("s5", library.Recents[0]);
        Assert.Equal(UserLibrary.MaxRecents, library.Recents.Count);
        Assert.Single(library.Recents, r => r == "s5");
    }

    [Fact]
    public void Survives_a_json_round_trip()
    {
        UserLibrary library = new() { Theme = "Dark" };
        library.ToggleFavorite("slider");
        library.Touch("button");

        var restored = UserLibrary.FromJson(library.ToJson());

        Assert.Equal(["slider"], restored.Favorites);
        Assert.Equal(["button"], restored.Recents);
        Assert.Equal("Dark", restored.Theme);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not json")]
    [InlineData("null")]
    public void Corrupt_json_gives_an_empty_library(string json)
    {
        var restored = UserLibrary.FromJson(json);
        Assert.Empty(restored.Favorites);
        Assert.Empty(restored.Recents);
    }
}
