using Microsoft.UI.Dispatching;
using UnexpectedApis.ViewModels;
using Windows.System.Power;
using UnexpectedApis.Attributes;

namespace UnexpectedApis.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[Sample("Power", "Power.png", SampleKind.NonUI)]
public sealed partial class PowerPage : Page
{
    private PowerViewModel? _viewModel;

    public PowerPage()
    {
        InitializeComponent();
    }

    public PowerViewModel ViewModel => _viewModel ??= new PowerViewModel(DispatcherQueue);

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        ViewModel.Initialize();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);

        ViewModel.Dispose();
    }
}

public class PowerViewModel(DispatcherQueue dispatcher) : ViewModelBase
{
    private readonly DispatcherQueue _dispatcher = dispatcher;
    private int? _remainingChargePercent;
    private string? _remainingDischargeTimeString;
    private bool _isDeviceCharging;
    private bool _isEnergySaving;

    public int? RemainingChargePercent
    {
        set
        {
            _remainingChargePercent = value;
            OnPropertyChanged();
        }
        get => _remainingChargePercent;
    }

    public string? RemainingDischargeTimeString
    {
        set
        {
            _remainingDischargeTimeString = value;
            OnPropertyChanged();
        }
        get => _remainingDischargeTimeString;
    }

    public bool IsDeviceCharging
    {
        set
        {
            _isDeviceCharging = value;
            OnPropertyChanged();
        }
        get => _isDeviceCharging;
    }

    public bool IsEnergySaving
    {
        set
        {
            _isEnergySaving = value;
            OnPropertyChanged();
        }
        get => _isEnergySaving;
    }

    public void Initialize()
    {
#if __HAS_UNO__
        var isAvailable = await PowerManager.InitializeAsync();
#endif

        RemainingChargePercent = PowerManager.RemainingChargePercent;
        RemainingDischargeTimeString = PowerManager.RemainingDischargeTime.ToString(@"hh\:mm\:ss");

        IsDeviceCharging = PowerManager.BatteryStatus == BatteryStatus.Charging;
        IsEnergySaving = PowerManager.EnergySaverStatus == EnergySaverStatus.On;


        PowerManager.RemainingChargePercentChanged += PowerManager_RemainingChargePercentChanged;
        PowerManager.RemainingDischargeTimeChanged += PowerManager_RemainingDischargeTimeChanged;

        PowerManager.BatteryStatusChanged += PowerManager_BatteryStatusChanged;
        PowerManager.EnergySaverStatusChanged += PowerManager_EnergySaverStatusChanged;
    }

    private void PowerManager_RemainingChargePercentChanged(object? sender, object e)
    {
        RemainingChargePercent = PowerManager.RemainingChargePercent;
    }

    private void PowerManager_RemainingDischargeTimeChanged(object? sender, object e)
    {
        RemainingDischargeTimeString = PowerManager.RemainingDischargeTime.ToString(@"hh\:mm\:ss");
    }

    private void PowerManager_BatteryStatusChanged(object? sender, object e)
    {
        IsDeviceCharging = PowerManager.BatteryStatus == BatteryStatus.Charging;
    }

    private void PowerManager_EnergySaverStatusChanged(object? sender, object e)
    {
        IsEnergySaving = PowerManager.EnergySaverStatus == EnergySaverStatus.On;
    }

    public void Dispose()
    {
        PowerManager.RemainingChargePercentChanged -= PowerManager_RemainingChargePercentChanged;
        PowerManager.RemainingDischargeTimeChanged -= PowerManager_RemainingDischargeTimeChanged;

        PowerManager.BatteryStatusChanged -= PowerManager_BatteryStatusChanged;
        PowerManager.EnergySaverStatusChanged -= PowerManager_EnergySaverStatusChanged;
    }
}
