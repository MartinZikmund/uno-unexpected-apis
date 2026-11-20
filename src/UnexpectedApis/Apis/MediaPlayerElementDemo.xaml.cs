using Windows.Media.Core;

namespace UnexpectedApis.Apis;

public sealed partial class MediaPlayerElementDemo : UserControl
{
    private static readonly Uri _source = new("ms-appx:///Assets/Apis/MediaPlayerElement/sample.mp4");

    public MediaPlayerElementDemo()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Player.Source = MediaSource.CreateFromUri(_source);
        }
        catch (Exception ex)
        {
            MessageText.Text = $"Video playback isn't available here: {ex.Message}";
            MessageText.Visibility = Visibility.Visible;
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Player.MediaPlayer?.Pause();
            Player.Source = null;
        }
        catch
        {
            // Nothing left to stop.
        }
    }
}
