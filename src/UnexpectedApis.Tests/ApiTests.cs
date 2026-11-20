using UnexpectedApis.Core;
using Xunit;

namespace UnexpectedApis.Tests;

public class CSharpTokenizerTests
{
    private static string Join(IEnumerable<CSharpToken> tokens) => string.Concat(tokens.Select(t => t.Text));

    private static List<(CSharpTokenKind, string)> Significant(string source) =>
        CSharpTokenizer.Tokenize(source)
            .Where(t => t.Kind != CSharpTokenKind.Text)
            .Select(t => (t.Kind, t.Text))
            .ToList();

    [Fact]
    public void Tokens_round_trip_the_source()
    {
        const string source = "var x = await Geolocator.RequestAccessAsync(); // ask first\n/* block */ \"text\" @\"v\\erbatim\" 'c' 42";
        Assert.Equal(source, Join(CSharpTokenizer.Tokenize(source)));
    }

    [Fact]
    public void Keywords_and_capitalized_names_are_told_apart() =>
        Assert.Equal(
            [(CSharpTokenKind.Keyword, "var"), (CSharpTokenKind.Keyword, "new"), (CSharpTokenKind.Type, "Geolocator")],
            Significant("var locator = new Geolocator();"));

    [Fact]
    public void Line_and_block_comments_run_to_their_end() =>
        Assert.Equal(
            [(CSharpTokenKind.Comment, "// new Foo\n"), (CSharpTokenKind.Comment, "/* if */")],
            Significant("// new Foo\n/* if */"));

    [Fact]
    public void Strings_keep_escaped_quotes_inside()
    {
        Assert.Equal([(CSharpTokenKind.String, "\"say \\\"hi\\\"\"")], Significant("\"say \\\"hi\\\"\""));
        Assert.Equal([(CSharpTokenKind.String, "@\"C:\\temp\\\"")], Significant("@\"C:\\temp\\\""));
    }

    [Fact]
    public void Interpolated_strings_are_strings() =>
        Assert.Equal([(CSharpTokenKind.String, "$\"{level:P0}\"")], Significant("$\"{level:P0}\""));

    [Fact]
    public void Unterminated_constructs_do_not_throw() =>
        Assert.Equal("\"open /* never", Join(CSharpTokenizer.Tokenize("\"open /* never")));
}

public class ApiPlatformTests
{
    [Fact]
    public void Each_lists_every_head_once_in_display_order() =>
        Assert.Equal(
            ["Android", "iOS", "tvOS", "Windows", "WebAssembly", "Desktop"],
            ApiPlatformInfo.Each.Select(p => p.Name));

    [Fact]
    public void All_covers_each_head() =>
        Assert.All(ApiPlatformInfo.Each, p => Assert.True(ApiPlatforms.All.HasFlag(p.Platform)));

    [Fact]
    public void Describe_names_the_supported_heads() =>
        Assert.Equal("Android, iOS", ApiPlatformInfo.Describe(ApiPlatforms.Android | ApiPlatforms.iOS));

    [Fact]
    public void Describe_says_everywhere_for_all() =>
        Assert.Equal("All platforms", ApiPlatformInfo.Describe(ApiPlatforms.All));
}
