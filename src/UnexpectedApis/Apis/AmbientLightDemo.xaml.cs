#pragma warning disable Uno0001 // Stubbed on desktop; the demo handles the missing sensor.

using Windows.Devices.Sensors;
using Windows.UI;

namespace UnexpectedApis.Apis;

public sealed partial class AmbientLightDemo : UserControl
{
    private readonly LightSensor? _sensor;

    public AmbientLightDemo()
    {
        InitializeComponent();
        _sensor = TryGetSensor();

        if (_sensor is null)
        {
            LuxText.Visibility = Visibility.Collapsed;
            StatusText.Text = "No ambient light sensor on this device.";
        }
        else
        {
            StatusText.Text = "Cover the sensor or point it at a lamp.";
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

    private static LightSensor? TryGetSensor()
    {
        try
        {
            return LightSensor.GetDefault();
        }
        catch (Exception)
        {
            return null;
        }
    }

    private void OnReadingChanged(LightSensor sender, LightSensorReadingChangedEventArgs args)
    {
        var lux = args.Reading.IlluminanceInLux;
        DispatcherQueue.TryEnqueue(() =>
        {
            LuxText.Text = $"{Math.Round(lux):0} lux";

            // 0 to 1000 lux maps from black to white.
            var level = (byte)(Math.Clamp(lux, 0, 1000) / 1000 * 255);
            Backdrop.Background = new SolidColorBrush(Color.FromArgb(255, level, level, level));
            var text = level > 140 ? Microsoft.UI.Colors.Black : Microsoft.UI.Colors.White;
            LuxText.Foreground = new SolidColorBrush(text);
            StatusText.Foreground = new SolidColorBrush(text);
        });
    }
}
