#pragma warning disable Uno0001 // Stubbed on desktop; the demo handles the missing sensor.

using Windows.Devices.Lights;

namespace UnexpectedApis.Apis;

public sealed partial class FlashlightDemo : UserControl
{
    private Lamp? _lamp;
    private bool _unloaded;

    public FlashlightDemo()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;

        Lamp? lamp = null;
        try
        {
            lamp = await Lamp.GetDefaultAsync();
        }
        catch (Exception)
        {
        }

        if (_unloaded)
        {
            lamp?.Dispose();
            return;
        }

        if (lamp is null)
        {
            LampToggle.Visibility = Visibility.Collapsed;
            StatusText.Text = "No flashlight on this device.";
            return;
        }

        _lamp = lamp;
        lamp.BrightnessLevel = 1;
        LampToggle.IsEnabled = true;
        StatusText.Text = "Tap to toggle the flashlight.";
    }

    private void OnLampToggled(object sender, RoutedEventArgs e)
    {
        if (_lamp is null)
        {
            return;
        }

        try
        {
            _lamp.IsEnabled = LampToggle.IsChecked == true;
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Couldn't switch the flashlight: {ex.Message}";
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _unloaded = true;
        if (_lamp is null)
        {
            return;
        }

        try
        {
            _lamp.IsEnabled = false;
            _lamp.Dispose();
        }
        catch (Exception)
        {
        }

        _lamp = null;
    }
}
