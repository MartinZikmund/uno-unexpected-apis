using Microsoft.UI.Xaml.Markup;

namespace UnexpectedApis.Views;

/// <summary>
/// Paints a theme brush by key. Goes through XAML because only a real {ThemeResource}
/// resolves against the subtree's RequestedTheme; a lookup from code would use the app's theme.
/// </summary>
public sealed partial class BrushSwatch : ContentControl
{
    public static readonly DependencyProperty BrushKeyProperty = DependencyProperty.Register(
        nameof(BrushKey),
        typeof(string),
        typeof(BrushSwatch),
        new PropertyMetadata(null, (d, e) => ((BrushSwatch)d).Load((string?)e.NewValue)));

    public BrushSwatch()
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;
        IsTabStop = false;
    }

    public string? BrushKey
    {
        get => (string?)GetValue(BrushKeyProperty);
        set => SetValue(BrushKeyProperty, value);
    }

    private void Load(string? key)
    {
        if (string.IsNullOrEmpty(key))
        {
            Content = null;
            return;
        }

        try
        {
            Content = XamlReader.Load(
                "<Border xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                $"Background='{{ThemeResource {key}}}' />");
        }
        catch (Exception ex) when (ex is XamlParseException or InvalidOperationException or ArgumentException)
        {
            Content = new TextBlock
            {
                Text = "missing",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
        }
    }
}
