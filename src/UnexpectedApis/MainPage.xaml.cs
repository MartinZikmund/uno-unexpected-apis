using Microsoft.UI;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using UnexpectedApis.Catalog;
using UnexpectedApis.Core;
using UnexpectedApis.Pages;
using UnexpectedApis.Services;
using Windows.Graphics;

namespace UnexpectedApis;

public sealed record SearchResult(string Title, string Subtitle, Symbol Icon, string Route);

public sealed partial class MainPage : Page
{
    private const int InitialWidth = 1400;
    private const int InitialHeight = 900;
    private const int MaxSearchResults = 8;
    private static readonly string[] ThemeCycle = ["System", "Light", "Dark"];

    private readonly DispatcherTimer _toastTimer = new() { Interval = TimeSpan.FromSeconds(2) };
    private readonly List<NavigationViewItem> _navItems = new();
    private readonly List<SearchEntry<SearchResult>> _searchEntries = BuildSearchEntries();
    private bool _syncingSelection;

    public MainPage()
    {
        InitializeComponent();

        if (CurrentPlatform.Value is not (ApiPlatforms.Desktop or ApiPlatforms.Windows))
        {
            // Only desktop windows have caption buttons to make room for.
            AppTitleBar.Padding = new Thickness(16, 0, 16, 0);
        }
#if DEBUG
        else
        {
            // Clear the Uno dev toolbar, which sits over the top-left corner in debug sessions.
            AppTitleBar.Padding = new Thickness(96, 0, 152, 0);
        }
#endif

        BuildNavigation();
        ApplyTheme(AppServices.Library.Theme ?? "System");

        AppServices.Navigate = Go;
        AppServices.ShowToast = ShowToast;
        _toastTimer.Tick += (_, _) =>
        {
            _toastTimer.Stop();
            Toast.IsOpen = false;
        };

        Loaded += OnLoaded;
        ActualThemeChanged += (_, _) => UpdateCaptionButtons();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;

        if (App.MainWindow is { } window && CurrentPlatform.Value is ApiPlatforms.Desktop or ApiPlatforms.Windows)
        {
            window.SetTitleBar(AppTitleBar);
            UpdateCaptionButtons();
            var scale = XamlRoot?.RasterizationScale ?? 1;
            window.AppWindow.Resize(new SizeInt32((int)(InitialWidth * scale), (int)(InitialHeight * scale)));
        }

        Go(Routes.Home);

#if DEBUG
        SmokeRunner.StartIfRequested(ContentFrame);
#endif
    }

    private void BuildNavigation()
    {
        Nav.MenuItems.Add(CreateItem("Home", Symbol.Home, Routes.Home));
        AddCategories("Controls", SampleCatalog.ControlCategories);
        AddCategories("Platform APIs", SampleCatalog.ApiCategories);

        Nav.MenuItems.Add(new NavigationViewItemHeader { Content = "Design tokens" });
        Nav.MenuItems.Add(CreateItem("Theme brushes", Symbol.FontColor, Routes.Brushes));
        Nav.MenuItems.Add(CreateItem("Typography", Symbol.Font, Routes.Typography));
        Nav.MenuItems.Add(CreateItem("Icons", Symbol.Emoji2, Routes.Icons));
    }

    private void AddCategories(string header, IEnumerable<Category> categories)
    {
        Nav.MenuItems.Add(new NavigationViewItemHeader { Content = header });
        foreach (var category in categories)
        {
            NavigationViewItem group = new()
            {
                Content = category.Title,
                Icon = new SymbolIcon(category.Icon),
                SelectsOnInvoked = false,
            };
            foreach (var sample in SampleCatalog.All.Where(s => s.Category == category))
            {
                group.MenuItems.Add(CreateItem(sample.Title, sample.Icon, Routes.ForSample(sample.Id)));
            }

            Nav.MenuItems.Add(group);
        }
    }

    private NavigationViewItem CreateItem(string title, Symbol icon, string route)
    {
        NavigationViewItem item = new() { Content = title, Icon = new SymbolIcon(icon), Tag = route };
        AutomationProperties.SetAutomationId(item, "Nav_" + route.Replace(':', '_'));
        _navItems.Add(item);
        return item;
    }

    private static List<SearchEntry<SearchResult>> BuildSearchEntries()
    {
        List<SearchEntry<SearchResult>> entries = SampleCatalog.All
            .Select(s => new SearchEntry<SearchResult>(
                new(s.Title, s.Category.Title, s.Icon, Routes.ForSample(s.Id)),
                s.Title,
                s.Keywords))
            .ToList();

        entries.Add(new(new("Theme brushes", "Design tokens", Symbol.FontColor, Routes.Brushes), "Theme brushes", ["color", "resource", "palette", "stroke", "fill"]));
        entries.Add(new(new("Typography", "Design tokens", Symbol.Font, Routes.Typography), "Typography", ["font", "text style", "type ramp"]));
        entries.Add(new(new("Icons", "Design tokens", Symbol.Emoji2, Routes.Icons), "Icons", ["symbol", "glyph", "fonticon"]));
        return entries;
    }

    private void Go(string route)
    {
        if (route == Routes.Home)
        {
            ContentFrame.Navigate(typeof(HomePage));
        }
        else if (route == Routes.Brushes)
        {
            ContentFrame.Navigate(typeof(BrushesPage));
        }
        else if (route == Routes.Typography)
        {
            ContentFrame.Navigate(typeof(TypographyPage));
        }
        else if (route == Routes.Icons)
        {
            ContentFrame.Navigate(typeof(IconsPage));
        }
        else if (route.StartsWith(Routes.SamplePrefix, StringComparison.Ordinal))
        {
            ContentFrame.Navigate(typeof(SamplePage), route[Routes.SamplePrefix.Length..]);
        }
    }

    private void OnNavSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (!_syncingSelection && args.SelectedItem is NavigationViewItem { Tag: string route })
        {
            Go(route);
        }
    }

    private void OnNavBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (ContentFrame.CanGoBack)
        {
            ContentFrame.GoBack();
        }
    }

    private void OnFrameNavigated(object sender, NavigationEventArgs e)
    {
        Nav.IsBackEnabled = ContentFrame.CanGoBack;

        var route = e.SourcePageType == typeof(SamplePage) ? Routes.ForSample((string)e.Parameter)
            : e.SourcePageType == typeof(BrushesPage) ? Routes.Brushes
            : e.SourcePageType == typeof(TypographyPage) ? Routes.Typography
            : e.SourcePageType == typeof(IconsPage) ? Routes.Icons
            : Routes.Home;

        var item = _navItems.FirstOrDefault(i => (string)i.Tag == route);
        if (item is null || ReferenceEquals(Nav.SelectedItem, item))
        {
            return;
        }

        _syncingSelection = true;
        foreach (var group in Nav.MenuItems.OfType<NavigationViewItem>())
        {
            if (group.MenuItems.Contains(item))
            {
                group.IsExpanded = true;
            }
        }

        Nav.SelectedItem = item;
        _syncingSelection = false;
    }

    private void OnFindAccelerator(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        SearchBox.Focus(FocusState.Programmatic);
        args.Handled = true;
    }

    private void OnSearchTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            sender.ItemsSource = CatalogSearch.Rank(sender.Text, _searchEntries).Take(MaxSearchResults).ToList();
        }
    }

    private void OnSearchSubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var result = args.ChosenSuggestion as SearchResult
            ?? CatalogSearch.Rank(args.QueryText, _searchEntries).FirstOrDefault();

        if (result is not null)
        {
            sender.Text = string.Empty;
            sender.ItemsSource = null;
            Go(result.Route);
        }
    }

    private void OnThemeClick(object sender, RoutedEventArgs e)
    {
        var next = ThemeCycle[(Array.IndexOf(ThemeCycle, AppServices.Library.Theme ?? "System") + 1) % ThemeCycle.Length];
        ApplyTheme(next);
        AppServices.Library.Theme = next;
        AppServices.SaveLibrary();
    }

    private void ApplyTheme(string theme)
    {
        RequestedTheme = theme switch
        {
            "Light" => ElementTheme.Light,
            "Dark" => ElementTheme.Dark,
            _ => ElementTheme.Default,
        };
        ThemeLabel.Text = theme;
    }

    /// <summary>Caption buttons follow the OS theme by default, which vanishes when the app overrides it.</summary>
    private void UpdateCaptionButtons()
    {
        if (App.MainWindow?.AppWindow.TitleBar is { } titleBar)
        {
            var foreground = ActualTheme == ElementTheme.Light ? Colors.Black : Colors.White;
            titleBar.ButtonForegroundColor = foreground;
            titleBar.ButtonHoverForegroundColor = foreground;
            titleBar.ButtonPressedForegroundColor = foreground;
        }
    }

    private void ShowToast(string message)
    {
        Toast.Message = message;
        Toast.IsOpen = true;
        _toastTimer.Stop();
        _toastTimer.Start();
    }
}
