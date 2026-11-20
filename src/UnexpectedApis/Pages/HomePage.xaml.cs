using Microsoft.UI.Xaml.Navigation;
using UnexpectedApis.Catalog;
using UnexpectedApis.Services;

namespace UnexpectedApis.Pages;

public sealed record SampleTile(string Title, string Subtitle, Symbol Icon, string Route)
{
    public static SampleTile From(Sample sample) =>
        new(sample.Title, sample.Summary, sample.Icon, Routes.ForSample(sample.Id));
}

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        InitializeComponent();

        var apiCount = SampleCatalog.All.Count(s => !s.IsLiveControl);
        var controlCount = SampleCatalog.All.Count - apiCount;
        CountsText.Text = $"{controlCount} live controls and {apiCount} platform APIs. Press Ctrl+K to find one fast.";

        BrowseGrid.ItemsSource = SampleCatalog.Categories
            .Where(category => SampleCatalog.All.Any(s => s.Category == category))
            .Select(category => new SampleTile(
                category.Title,
                category.Blurb,
                category.Icon,
                Routes.ForSample(SampleCatalog.All.First(s => s.Category == category).Id)))
            .ToList();

        TokensGrid.ItemsSource = new List<SampleTile>
        {
            new("Theme brushes", "Light and dark, side by side", Symbol.FontColor, Routes.Brushes),
            new("Typography", "The Fluent type ramp", Symbol.Font, Routes.Typography),
            new("Icons", "Every Symbol, ready to paste", Symbol.Emoji2, Routes.Icons),
        };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        var library = AppServices.Library;
        Fill(FavoritesSection, FavoritesGrid, library.Favorites);
        Fill(RecentsSection, RecentsGrid, library.Recents);
    }

    private static void Fill(FrameworkElement section, GridView grid, IEnumerable<string> ids)
    {
        var tiles = ids.Select(SampleCatalog.Find).OfType<Sample>().Select(SampleTile.From).ToList();
        grid.ItemsSource = tiles;
        section.Visibility = tiles.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void OnTileClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is SampleTile tile)
        {
            AppServices.Navigate?.Invoke(tile.Route);
        }
    }

    private void OnStartClick(object sender, RoutedEventArgs e) =>
        AppServices.Navigate?.Invoke(Routes.ForSample(SampleCatalog.All[0].Id));

    private void OnBrushesClick(object sender, RoutedEventArgs e) =>
        AppServices.Navigate?.Invoke(Routes.Brushes);
}
