using UnexpectedApis.Services;

namespace UnexpectedApis.Pages;

public sealed partial class TypeRow : ObservableObject
{
    public TypeRow(string styleKey, string metrics, string text)
    {
        StyleKey = styleKey;
        Metrics = metrics;
        Text = text;
    }

    public string StyleKey { get; }

    public string Metrics { get; }

    public Style TextStyle => (Style)Application.Current.Resources[StyleKey];

    [ObservableProperty]
    public partial string Text { get; set; }
}

public sealed partial class TypographyPage : Page
{
    private readonly List<TypeRow> _rows;

    public TypographyPage()
    {
        InitializeComponent();

        var text = SampleBox.Text;
        _rows =
        [
            new("CaptionTextBlockStyle", "12 / Regular. Labels, timestamps", text),
            new("BodyTextBlockStyle", "14 / Regular. The default", text),
            new("BodyStrongTextBlockStyle", "14 / Semibold. Emphasis", text),
            new("SubtitleTextBlockStyle", "20 / Semibold. Section headers", text),
            new("TitleTextBlockStyle", "28 / Semibold. Page titles", text),
            new("TitleLargeTextBlockStyle", "40 / Semibold. Feature titles", text),
            new("DisplayTextBlockStyle", "68 / Semibold. Hero text", text),
        ];
        RampList.ItemsSource = _rows;
    }

    private void OnSampleChanged(object sender, TextChangedEventArgs e)
    {
        // Fires during InitializeComponent, before the rows exist.
        if (_rows is null)
        {
            return;
        }

        foreach (var row in _rows)
        {
            row.Text = SampleBox.Text;
        }
    }

    private void OnCopyClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string key })
        {
            AppServices.Copy($"Style=\"{{StaticResource {key}}}\"", $"{key} copied");
        }
    }
}
