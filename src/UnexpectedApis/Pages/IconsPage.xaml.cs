using UnexpectedApis.Core;
using UnexpectedApis.Services;

namespace UnexpectedApis.Pages;

public sealed record IconToken(string Name, Symbol Symbol)
{
    public string Code => ((int)Symbol).ToString("X4");
}

public sealed partial class IconsPage : Page
{
    private static readonly List<IconToken> Tokens = Enum.GetNames<Symbol>()
        .Select(name => new IconToken(name, Enum.Parse<Symbol>(name)))
        .OrderBy(token => token.Name, StringComparer.OrdinalIgnoreCase)
        .ToList();

    public IconsPage()
    {
        InitializeComponent();
        CountText.Text = $"All {Tokens.Count} values of the Symbol enum, drawn by this app's icon font. Pick one to get its markup.";
        IconGrid.ItemsSource = Tokens;
        IconGrid.SelectedIndex = 0;
        Show(Tokens[0]);
    }

    private void OnFilterChanged(object sender, TextChangedEventArgs e)
    {
        var filter = FilterBox.Text.Trim();
        IconGrid.ItemsSource = filter.Length == 0
            ? Tokens
            : Tokens.Where(t => t.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    private void OnIconSelected(object sender, SelectionChangedEventArgs e)
    {
        if (IconGrid.SelectedItem is IconToken token)
        {
            Show(token);
        }
    }

    private void Show(IconToken token)
    {
        PreviewIcon.Symbol = token.Symbol;
        DetailName.Text = token.Name;
        DetailCode.Text = $"U+{token.Code}";
        SymbolCode.Source = SymbolSnippet(token);
        FontCode.Source = FontSnippet(token);
    }

    private static string SymbolSnippet(IconToken token) =>
        XamlSnippet.Build("SymbolIcon", [new("Symbol", token.Name)]);

    private static string FontSnippet(IconToken token) =>
        XamlSnippet.Build("FontIcon", [new("FontFamily", "{ThemeResource SymbolThemeFontFamily}"), new("Glyph", $"&#x{token.Code};")]);

    private void OnCopySymbol(object sender, RoutedEventArgs e) =>
        AppServices.Copy(SymbolCode.Source, "SymbolIcon copied");

    private void OnCopyFont(object sender, RoutedEventArgs e) =>
        AppServices.Copy(FontCode.Source, "FontIcon copied");
}
