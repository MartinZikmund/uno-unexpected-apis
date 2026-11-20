#pragma warning disable Uno0001 // Stubbed on desktop; the demo handles the missing sensor.

using Windows.Devices.Sensors;

namespace UnexpectedApis.Apis;

public sealed partial class CompassDemo : UserControl
{
    private readonly Compass? _sensor;

    public CompassDemo()
    {
        InitializeComponent();
        _sensor = TryGetSensor();

        if (_sensor is null)
        {
            Dial.Visibility = Visibility.Collapsed;
            HeadingText.Visibility = Visibility.Collapsed;
            StatusText.Text = "No compass on this device. In a browser, the magnetometer may be disabled in the site settings.";
        }
        else
        {
            StatusText.Text = "Turn the device around.";
            _sensor.ReadingChanged += OnReadingChanged;
        }

        Unloaded += (_, _) =>
        {
            if (_sensor is not null)
            {
                _sensor.ReadingChanged -= OnReadingChanged;
            }
        };
    }

    private static Compass? TryGetSensor()
    {
        try
        {
            return Compass.GetDefault();
        }
        catch (Exception)
        {
            return null;
        }
    }

    private void OnReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
    {
        var heading = args.Reading.HeadingMagneticNorth;
        DispatcherQueue.TryEnqueue(() =>
        {
            HeadingText.Text = $"{heading:0}°";
            NeedleRotation.Angle = -heading;
        });
    }
}
