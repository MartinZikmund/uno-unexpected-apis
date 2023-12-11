using Microsoft.UI.Dispatching;
using UnexpectedApisDemo.Shared.ViewModels;
using Uno.Disposables;
using Windows.Devices.Sensors;
using Windows.UI.Core;

namespace UnexpectedApisDemo.Shared.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class HingeAnglePage : Page
{
    private HingeAngleSensorViewModel _viewModel;

    public HingeAnglePage()
    {
        this.InitializeComponent();
    }

    public HingeAngleSensorViewModel ViewModel => _viewModel ?? (_viewModel = new HingeAngleSensorViewModel(DispatcherQueue));

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        await ViewModel.InitializeAsync();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);

        ViewModel.Disposables.Dispose();
    }
}

public class HingeAngleSensorViewModel : ViewModelBase
{
    private HingeAngleSensor _hinge;
    private bool _readingChangedAttached;
    private string _sensorStatus;
    private string _angle;
    private readonly DispatcherQueue _dispatcher;
    private bool _noSensor = false;

    public HingeAngleSensorViewModel(DispatcherQueue dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public bool NoSensor
    {
        get => _noSensor;
        set
        {
            _noSensor = value;
            OnPropertyChanged();
        }
    }

    public async Task InitializeAsync()
    {
        _hinge = await HingeAngleSensor.GetDefaultAsync();
        OnPropertyChanged(nameof(HingeAngleSensorAvailable));
        if (_hinge == null)
        {
            NoSensor = true;
            return;
        }
        
        Disposables.Add(Disposable.Create(() =>
        {
            if (_hinge != null)
            {
                _hinge.ReadingChanged -= HingeAngleSensor_ReadingChanged;
            }
        }));
        _hinge.ReadingChanged += HingeAngleSensor_ReadingChanged;
    }

    public bool HingeAngleSensorAvailable => _hinge != null;

    public string SensorStatus
    {
        get => _sensorStatus;
        private set
        {
            _sensorStatus = value;
            OnPropertyChanged();
        }
    }

    public bool ReadingChangedAttached
    {
        get => _readingChangedAttached;
        private set
        {
            _readingChangedAttached = value;
            OnPropertyChanged();
        }
    }

    public string Angle
    {
        get => _angle;
        private set
        {
            _angle = value;
            OnPropertyChanged();
        }
    }

    private void HingeAngleSensor_ReadingChanged(HingeAngleSensor sender, HingeAngleSensorReadingChangedEventArgs args)
    {
        _dispatcher.TryEnqueue(DispatcherQueuePriority.Normal, () =>
        {
            Angle = string.Format("{0:0}", Math.Round(args.Reading.AngleInDegrees));
        });
    }
}
