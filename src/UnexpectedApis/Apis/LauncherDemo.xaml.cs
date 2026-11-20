using Windows.System;

namespace UnexpectedApis.Apis;

public sealed partial class LauncherDemo : UserControl
{
    public LauncherDemo() => InitializeComponent();

    private async void OnLaunchClick(object sender, RoutedEventArgs e)
    {
        if (!Uri.TryCreate(UriBox.Text.Trim(), UriKind.Absolute, out Uri? uri))
        {
            StatusText.Text = "That isn't a valid absolute URI.";
            return;
        }

        try
        {
            bool launched = await Launcher.LaunchUriAsync(uri);
            StatusText.Text = launched ? "Launched." : "Nothing on this device handles that URI.";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Couldn't launch: {ex.Message}";
        }
    }
}
