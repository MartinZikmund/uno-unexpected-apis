#pragma warning disable Uno0001 // Stubbed on desktop; the demo handles the missing sensor.

using Windows.Devices.Sensors;

namespace UnexpectedApis.Apis;

public sealed partial class HingeAngleDemo : UserControl
{
    private HingeAngleSensor? _sensor;
    private bool _unloaded;

    public HingeAngleDemo()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;

        HingeAngleSensor? sensor = null;
        try
        {
            sensor = await HingeAngleSensor.GetDefaultAsync();
        }
        catch (Exception)
        {
        }

        if (_unloaded)
        {
            return;
        }

        if (sensor is null)
        {
            AngleText.Visibility = Visibility.Collapsed;
            StatusText.Text = "No hinge sensor on this device.";
            return;
        }

        _sensor = sensor;
        StatusText.Text = "Open and close the device.";
        sensor.ReadingChanged += OnReadingChanged;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _unloaded = true;
        if (_sensor is not null)
        {
            _sensor.ReadingChanged -= OnReadingChanged;
        }
    }

    private void OnReadingChanged(HingeAngleSensor sender, HingeAngleSensorReadingChangedEventArgs args)
    {
        var angle = args.Reading.AngleInDegrees;
        DispatcherQueue.TryEnqueue(() => AngleText.Text = $"{Math.Round(angle):0}°");
    }
}
