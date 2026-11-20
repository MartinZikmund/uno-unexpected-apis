#pragma warning disable Uno0001 // Stubbed on desktop; the demo handles the missing sensor.

using Windows.Devices.Sensors;

namespace UnexpectedApis.Apis;

public sealed partial class AccelerometerDemo : UserControl
{
    private readonly Accelerometer? _sensor;

    public AccelerometerDemo()
    {
        InitializeComponent();
        _sensor = TryGetSensor();

        if (_sensor is null)
        {
            StatusText.Text = "No accelerometer on this device.";
            ReadingPanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            StatusText.Text = "Accelerometer ready. Tilt the device.";
        }

        Unloaded += (_, _) => Stop();
    }

    private static Accelerometer? TryGetSensor()
    {
        try
        {
            var sensor = Accelerometer.GetDefault();
            if (sensor is not null)
            {
                sensor.ReportInterval = Math.Max(sensor.MinimumReportInterval, 100);
            }

            return sensor;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private void OnObserveToggled(object sender, RoutedEventArgs e)
    {
        if (_sensor is null)
        {
            return;
        }

        _sensor.ReadingChanged -= OnReadingChanged;
        if (ObserveSwitch.IsOn)
        {
            _sensor.ReadingChanged += OnReadingChanged;
        }
    }

    private void OnReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
    {
        var reading = args.Reading;
        DispatcherQueue.TryEnqueue(() =>
        {
            XText.Text = reading.AccelerationX.ToString("0.00");
            YText.Text = reading.AccelerationY.ToString("0.00");
            ZText.Text = reading.AccelerationZ.ToString("0.00");
            TimestampText.Text = $"Updated {reading.Timestamp:T}";
        });
    }

    private void Stop()
    {
        if (_sensor is not null)
        {
            _sensor.ReadingChanged -= OnReadingChanged;
        }
    }
}
