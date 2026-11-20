using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.UI;

namespace UnexpectedApis.Apis;

public sealed partial class TitleBarDemo : UserControl
{
    private static readonly Random _random = new();
    private string? _originalTitle;

    public TitleBarDemo()
    {
        InitializeComponent();
        Unloaded += (_, _) => Restore();
        StatusText.Text = Supported
            ? "The caption buttons are the part this demo can recolor."
            : "This platform doesn't let the app customize its title bar.";
    }

    private static bool Supported => App.MainWindow is not null && AppWindowTitleBar.IsCustomizationSupported();

    private void OnSetTitleClick(object sender, RoutedEventArgs e)
    {
        if (App.MainWindow is not { } window)
        {
            return;
        }

        _originalTitle ??= window.Title;
        window.Title = string.IsNullOrWhiteSpace(TitleInput.Text) ? _originalTitle : TitleInput.Text;
        StatusText.Text = $"Window.Title is now \"{window.Title}\".";
    }

    private void OnRandomColorClick(object sender, RoutedEventArgs e)
    {
        if (!Supported)
        {
            StatusText.Text = "Title bar customization isn't supported here.";
            return;
        }

        try
        {
            Color color = Color.FromArgb(255, (byte)_random.Next(256), (byte)_random.Next(256), (byte)_random.Next(256));
            var titleBar = App.MainWindow!.AppWindow.TitleBar;
            titleBar.ButtonBackgroundColor = color;
            titleBar.ButtonInactiveBackgroundColor = color;
            StatusText.Text = $"Caption button background: #{color.R:X2}{color.G:X2}{color.B:X2}";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Couldn't recolor the title bar: {ex.Message}";
        }
    }

    private void OnResetClick(object sender, RoutedEventArgs e)
    {
        Restore();
        StatusText.Text = "Back to the defaults.";
    }

    private void Restore()
    {
        try
        {
            if (App.MainWindow is not { } window)
            {
                return;
            }

            if (_originalTitle is not null)
            {
                window.Title = _originalTitle;
                _originalTitle = null;
            }

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                window.AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                window.AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            }
        }
        catch
        {
            // Restoring is best effort; the window may be gone already.
        }
    }
}
