#pragma warning disable Uno0001 // Stubbed on desktop; the demo handles the missing sensor.

using Windows.System.Display;

namespace UnexpectedApis.Apis;

public sealed partial class DisplayRequestDemo : UserControl
{
    private DisplayRequest? _request;
    private bool _active;

    public DisplayRequestDemo()
    {
        InitializeComponent();
        Unloaded += (_, _) => Release();
    }

    private void OnKeepOnToggled(object sender, RoutedEventArgs e)
    {
        if (KeepOnSwitch.IsOn == _active)
        {
            return;
        }

        try
        {
            if (KeepOnSwitch.IsOn)
            {
                _request ??= new();
                _request.RequestActive();
                _active = true;
                StatusText.Text = "Screen request is active.";
            }
            else
            {
                Release();
                StatusText.Text = "Screen request released.";
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Couldn't change the screen request: {ex.Message}";
        }
    }

    private void Release()
    {
        if (!_active)
        {
            return;
        }

        _active = false;
        try
        {
            _request?.RequestRelease();
        }
        catch (Exception)
        {
        }
    }
}
