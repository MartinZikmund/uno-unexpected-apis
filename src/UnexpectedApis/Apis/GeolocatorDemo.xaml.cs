#pragma warning disable Uno0001 // Stubbed on desktop; the demo handles the missing sensor.

using Windows.Devices.Geolocation;

namespace UnexpectedApis.Apis;

public sealed partial class GeolocatorDemo : UserControl
{
    private bool _unloaded;

    public GeolocatorDemo()
    {
        InitializeComponent();
        Unloaded += (_, _) => _unloaded = true;
    }

    private async void OnLocateClick(object sender, RoutedEventArgs e)
    {
        LocateButton.IsEnabled = false;
        Progress.IsActive = true;
        Progress.Visibility = Visibility.Visible;
        ResultCard.Visibility = Visibility.Collapsed;
        StatusText.Text = "Asking for your location...";

        try
        {
            if (await Geolocator.RequestAccessAsync() != GeolocationAccessStatus.Allowed)
            {
                Report("Location access wasn't granted.");
                return;
            }

            Geolocator geolocator = new();
            var position = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(15));
            if (_unloaded)
            {
                return;
            }

            var point = position.Coordinate.Point.Position;
            LatitudeText.Text = point.Latitude.ToString("0.0000");
            LongitudeText.Text = point.Longitude.ToString("0.0000");
            DetailsText.Text = $"Accuracy {position.Coordinate.Accuracy:0} m, altitude {point.Altitude:0.#} m, at {position.Coordinate.Timestamp:T}";
            ResultCard.Visibility = Visibility.Visible;
            StatusText.Text = string.Empty;
        }
        catch (Exception ex) when (ex is NotImplementedException or NotSupportedException)
        {
            Report("Geolocation isn't available on this platform.");
        }
        catch (Exception ex)
        {
            Report($"Couldn't get the location: {ex.Message}");
        }
        finally
        {
            if (!_unloaded)
            {
                Progress.IsActive = false;
                Progress.Visibility = Visibility.Collapsed;
                LocateButton.IsEnabled = true;
            }
        }
    }

    private void Report(string message)
    {
        if (!_unloaded)
        {
            StatusText.Text = message;
        }
    }
}
