#pragma warning disable Uno0001 // Stubbed on desktop; the demo handles the missing sensor.

using Microsoft.UI.Dispatching;
using Windows.System.Power;

namespace UnexpectedApis.Apis;

public sealed partial class PowerDemo : UserControl
{
    private DispatcherQueueTimer? _timer;
    private bool _subscribed;
    private bool _unloaded;

    public PowerDemo()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;

        try
        {
#if HAS_UNO
            // WebAssembly needs this call before any other PowerManager member.
            if (!await PowerManager.InitializeAsync())
            {
                ShowUnavailable("Battery information isn't available in this browser.");
                return;
            }
#else
            await Task.CompletedTask;
#endif
            if (_unloaded)
            {
                return;
            }

            Refresh();

            PowerManager.RemainingChargePercentChanged += OnPowerChanged;
            PowerManager.RemainingDischargeTimeChanged += OnPowerChanged;
            PowerManager.BatteryStatusChanged += OnPowerChanged;
            PowerManager.EnergySaverStatusChanged += OnPowerChanged;
            _subscribed = true;

            // Android only raises the level event on Low/Ok, so poll as well.
            _timer = DispatcherQueue.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }
        catch (Exception ex) when (ex is NotImplementedException or NotSupportedException)
        {
            ShowUnavailable("Battery information isn't available on this platform.");
        }
        catch (Exception ex)
        {
            ShowUnavailable($"Couldn't read the battery: {ex.Message}");
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _unloaded = true;
        if (_timer is not null)
        {
            _timer.Stop();
            _timer.Tick -= OnTimerTick;
            _timer = null;
        }

        if (_subscribed)
        {
            _subscribed = false;
            PowerManager.RemainingChargePercentChanged -= OnPowerChanged;
            PowerManager.RemainingDischargeTimeChanged -= OnPowerChanged;
            PowerManager.BatteryStatusChanged -= OnPowerChanged;
            PowerManager.EnergySaverStatusChanged -= OnPowerChanged;
        }
    }

    private void OnPowerChanged(object? sender, object e) => DispatcherQueue.TryEnqueue(Refresh);

    private void OnTimerTick(DispatcherQueueTimer sender, object args) => Refresh();

    private void ShowUnavailable(string message)
    {
        InfoCard.Visibility = Visibility.Collapsed;
        StatusText.Text = message;
    }

    private void Refresh()
    {
        try
        {
            var percent = PowerManager.RemainingChargePercent;
            LevelText.Text = $"{percent}%";
            LevelBar.Value = percent;

            var status = PowerManager.BatteryStatus;
            if (status == BatteryStatus.NotPresent)
            {
                ShowUnavailable("No battery on this device.");
                return;
            }

            ChargingText.Text = status == BatteryStatus.Charging ? "Yes" : "No";
            SaverText.Text = PowerManager.EnergySaverStatus == EnergySaverStatus.On ? "On" : "Off";
            DischargeText.Text = PowerManager.RemainingDischargeTime.ToString(@"hh\:mm\:ss");
            StatusText.Text = string.Empty;
        }
        catch (Exception ex) when (ex is NotImplementedException or NotSupportedException)
        {
            ShowUnavailable("Battery information isn't available on this platform.");
        }
    }
}
