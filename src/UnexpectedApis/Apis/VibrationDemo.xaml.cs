#pragma warning disable Uno0001 // Stubbed on desktop; the demo handles the missing sensor.

#if HAS_UNO
using Windows.Phone.Devices.Notification;
#endif

namespace UnexpectedApis.Apis;

public sealed partial class VibrationDemo : UserControl
{
    public VibrationDemo() => InitializeComponent();

    private void OnVibrateClick(object sender, RoutedEventArgs e)
    {
#if HAS_UNO
        try
        {
            var device = VibrationDevice.GetDefault();
            if (device is null)
            {
                StatusText.Text = "No vibration motor on this device.";
                return;
            }

            device.Vibrate(TimeSpan.FromMilliseconds(DurationSlider.Value));
            StatusText.Text = "Bzzzt.";
        }
        catch (Exception ex) when (ex is NotImplementedException or NotSupportedException)
        {
            StatusText.Text = "Vibration isn't available on this platform.";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Couldn't vibrate: {ex.Message}";
        }
#else
        StatusText.Text = "VibrationDevice isn't available in WinUI. Use Windows.Devices.Haptics there.";
#endif
    }
}
