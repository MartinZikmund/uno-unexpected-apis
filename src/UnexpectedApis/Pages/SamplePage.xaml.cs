using Microsoft.UI.Xaml.Navigation;
using UnexpectedApis.Catalog;
using UnexpectedApis.Core;
using UnexpectedApis.Services;
using UnexpectedApis.Views;
using Windows.System;

namespace UnexpectedApis.Pages;

public sealed class KnobTemplateSelector : DataTemplateSelector
{
    public DataTemplate? BoolTemplate { get; set; }

    public DataTemplate? TextTemplate { get; set; }

    public DataTemplate? NumberTemplate { get; set; }

    public DataTemplate? ChoiceTemplate { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item) => item switch
    {
        BoolKnob => BoolTemplate,
        TextKnob => TextTemplate,
        NumberKnob => NumberTemplate,
        ChoiceKnob => ChoiceTemplate,
        _ => null,
    };

    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container) => SelectTemplateCore(item);
}

public sealed partial class SamplePage : Page
{
    private static readonly double[] PreviewWidths = [double.PositiveInfinity, 720, 480, 320];

    private readonly List<(StageView Stage, FrameworkElement? Instance)> _stages = new();
    private Sample? _sample;
    private SampleSession? _session;

    public SamplePage()
    {
        InitializeComponent();
        Unloaded += (_, _) => DetachKnobs();
    }

    /// <summary>The last failure while building or updating the control, if any.</summary>
    public string? LastError { get; private set; }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        _sample = SampleCatalog.Find((string)e.Parameter);
        if (_sample is null)
        {
            return;
        }

        CategoryText.Text = _sample.Category.Title;
        TitleText.Text = _sample.Title;
        SummaryText.Text = _sample.Summary;
        FavoriteButton.IsChecked = AppServices.Library.IsFavorite(_sample.Id);

        ConfigureForKind(_sample);

        TipsList.ItemsSource = _sample.Tips;
        TipsSection.Visibility = _sample.Tips.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        _session = _sample.CreateSession();
        KnobsList.ItemsSource = _session.Knobs;
        var hasKnobs = _session.Knobs.Count > 0;
        NoKnobsText.Visibility = hasKnobs ? Visibility.Collapsed : Visibility.Visible;
        ResetButton.Visibility = hasKnobs ? Visibility.Visible : Visibility.Collapsed;

        foreach (var knob in _session.Knobs)
        {
            knob.Changed += OnKnobChanged;
        }

        BuildStages();
        UpdateSnippet();

        AppServices.Library.Touch(_sample.Id);
        AppServices.SaveLibrary();
    }

    private void ConfigureForKind(Sample sample)
    {
        var live = sample.IsLiveControl;
        StageToolbar.Visibility = live ? Visibility.Visible : Visibility.Collapsed;
        PropertiesPanel.Visibility = live ? Visibility.Visible : Visibility.Collapsed;
        Grid.SetColumnSpan(StageCard, live ? 1 : 2);
        Grid.SetColumnSpan(LowerStack, live ? 1 : 2);

        SnippetLabel.Text = sample.SnippetLanguage;
        Code.CodeLanguage = sample.SnippetLanguage;
        CopyLabel.Text = live ? "Copy" : "Copy code";

        PlatformsText.Visibility = live ? Visibility.Collapsed : Visibility.Visible;
        PlatformsText.Text = $"Works on: {ApiPlatformInfo.Describe(sample.Platforms)}";

        var supported = CurrentPlatform.Supports(sample.Platforms);
        UnsupportedBar.IsOpen = !supported;
        UnsupportedBar.Title = $"Not available on {CurrentPlatform.Name}";
        UnsupportedBar.Message = $"Run the app on {ApiPlatformInfo.Describe(sample.Platforms)} to see this one work. The code still shows how.";
    }

    private void DetachKnobs()
    {
        if (_session is null)
        {
            return;
        }

        foreach (var knob in _session.Knobs)
        {
            knob.Changed -= OnKnobChanged;
        }
    }

    private void OnKnobChanged(object? sender, EventArgs e)
    {
        foreach (var (stage, instance) in _stages)
        {
            if (instance is not null)
            {
                ApplyKnobs(stage, instance);
            }
        }

        UpdateSnippet();
    }

    private void ApplyKnobs(StageView stage, FrameworkElement instance)
    {
        try
        {
            _session!.Apply(instance);
            LastError = null;
        }
        catch (Exception ex)
        {
            ReportFailure(stage, ex);
        }
    }

    private void BuildStages()
    {
        if (_sample is null || _session is null)
        {
            return;
        }

        _stages.Clear();
        StageHost.Children.Clear();
        StageHost.ColumnDefinitions.Clear();

        var mode = (ThemeModes.SelectedItem?.Tag as string) ?? "App";
        ElementTheme[] themes = mode switch
        {
            "Light" => [ElementTheme.Light],
            "Dark" => [ElementTheme.Dark],
            "Both" => [ElementTheme.Light, ElementTheme.Dark],
            _ => [ElementTheme.Default],
        };

        for (var i = 0; i < themes.Length; i++)
        {
            StageHost.ColumnDefinitions.Add(new ColumnDefinition());
            StageView stage = new(_sample.StageHeight, _sample.Fills) { RequestedTheme = themes[i] };
            stage.SetContentMaxWidth(PreviewWidths[Math.Max(WidthBox.SelectedIndex, 0)]);
            stage.SetOuterCorners(left: i == 0, right: i == themes.Length - 1);
            Grid.SetColumn(stage, i);
            StageHost.Children.Add(stage);
            _stages.Add((stage, CreateInstance(stage)));
        }
    }

    private FrameworkElement? CreateInstance(StageView stage)
    {
        try
        {
            var instance = _session!.CreateInstance();
            stage.SetContent(instance);
            LastError = null;
            return instance;
        }
        catch (Exception ex)
        {
            ReportFailure(stage, ex);
            return null;
        }
    }

    private void ReportFailure(StageView stage, Exception ex)
    {
        LastError = $"{ex.GetType().Name}: {ex.Message}";
        stage.ShowError(LastError);
    }

    private void UpdateSnippet()
    {
        if (_session is not null)
        {
            Code.Source = _session.BuildSnippet();
        }
    }

    private void OnThemeModeChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args) => BuildStages();

    private void OnWidthChanged(object sender, SelectionChangedEventArgs e)
    {
        var width = PreviewWidths[Math.Max(WidthBox.SelectedIndex, 0)];
        foreach (var (stage, _) in _stages)
        {
            stage.SetContentMaxWidth(width);
        }
    }

    private void OnFavoriteClick(object sender, RoutedEventArgs e)
    {
        if (_sample is null)
        {
            return;
        }

        var isFavorite = AppServices.Library.ToggleFavorite(_sample.Id);
        AppServices.SaveLibrary();
        FavoriteButton.IsChecked = isFavorite;
        AppServices.ShowToast?.Invoke(isFavorite ? $"{_sample.Title} added to favorites" : $"{_sample.Title} removed from favorites");
    }

    private async void OnDocsClick(object sender, RoutedEventArgs e)
    {
        if (_sample is not null && !await Launcher.LaunchUriAsync(_sample.DocsUri))
        {
            AppServices.ShowToast?.Invoke("Couldn't open the browser");
        }
    }

    private void OnCopyClick(object sender, RoutedEventArgs e)
    {
        if (_session is not null)
        {
            AppServices.Copy(_session.BuildSnippet(), $"{_sample?.SnippetLanguage} copied");
        }
    }

    private void OnResetClick(object sender, RoutedEventArgs e)
    {
        if (_session is null)
        {
            return;
        }

        foreach (var knob in _session.Knobs)
        {
            knob.Reset();
        }
    }
}
