using Microsoft.UI.Xaml.Documents;
using UnexpectedApis.Core;

namespace UnexpectedApis.Views;

/// <summary>Read-only, selectable XAML with syntax coloring that follows the theme.</summary>
public sealed partial class CodeView : UserControl
{
    private string _source = string.Empty;

    public CodeView()
    {
        InitializeComponent();
        ActualThemeChanged += (_, _) => Render();
    }

    public string Source
    {
        get => _source;
        set
        {
            _source = value;
            Render();
        }
    }

    /// <summary>Wrap long lines instead of scrolling them, for narrow panels.</summary>
    public bool WrapLines
    {
        get => Output.TextWrapping == TextWrapping.Wrap;
        set
        {
            Output.TextWrapping = value ? TextWrapping.Wrap : TextWrapping.NoWrap;
            Scroller.HorizontalScrollBarVisibility = value ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
        }
    }

    /// <summary>"XAML" or "C#".</summary>
    public string CodeLanguage
    {
        get;
        set
        {
            field = value;
            Render();
        }
    } = "XAML";

    private void Render()
    {
        Output.Inlines.Clear();
        if (CodeLanguage == "C#")
        {
            foreach (var token in CSharpTokenizer.Tokenize(_source))
            {
                AddRun(token.Text, BrushFor(token.Kind));
            }

            return;
        }

        foreach (var token in XamlTokenizer.Tokenize(_source))
        {
            Run run = new() { Text = token.Text };
            if (BrushFor(token.Kind) is { } brush)
            {
                run.Foreground = brush;
            }

            Output.Inlines.Add(run);
        }
    }

    private void AddRun(string text, Microsoft.UI.Xaml.Media.Brush? brush)
    {
        Run run = new() { Text = text };
        if (brush is not null)
        {
            run.Foreground = brush;
        }

        Output.Inlines.Add(run);
    }

    private Microsoft.UI.Xaml.Media.Brush? BrushFor(CSharpTokenKind kind) => kind switch
    {
        CSharpTokenKind.Keyword => TagSwatch.Background,
        CSharpTokenKind.Type => AttributeSwatch.Background,
        CSharpTokenKind.String => ValueSwatch.Background,
        CSharpTokenKind.Comment => CommentSwatch.Background,
        _ => null,
    };

    private Microsoft.UI.Xaml.Media.Brush? BrushFor(XamlTokenKind kind) => kind switch
    {
        XamlTokenKind.Tag => TagSwatch.Background,
        XamlTokenKind.AttributeName => AttributeSwatch.Background,
        XamlTokenKind.AttributeValue => ValueSwatch.Background,
        XamlTokenKind.Punctuation => PunctuationSwatch.Background,
        XamlTokenKind.Comment => CommentSwatch.Background,
        _ => null,
    };
}
