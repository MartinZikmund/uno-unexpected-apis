using Microsoft.UI.Dispatching;
using UnexpectedApis.ViewModels;
using Uno.Disposables;
using Windows.Devices.Sensors;
using Windows.UI;
using Windows.UI.Core;
using UnexpectedApis.Attributes;

namespace UnexpectedApis.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[Sample("AmbientLight", "AmbientLight.png", SampleKind.NonUI)]
public sealed partial class AmbientLightPage : SamplePage
{
    private AmbientLightSensorViewModel _viewModel;

    public AmbientLightPage()
    {
        this.InitializeComponent();
    }

    public string Code =>
""""
var lightSensor = LightSensor.GetDefault();
lightSensor.ReadingChanged += (s,e) =>
{
    var lux = e.Reading.IlluminanceInLux;
};
"""";

    public AmbientLightSensorViewModel ViewModel => _viewModel ?? (_viewModel = new AmbientLightSensorViewModel(DispatcherQueue));

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        ViewModel.Initialize();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);

        ViewModel.Disposables.Dispose();
    }
}

public class AmbientLightSensorViewModel : ViewModelBase
{
    private LightSensor _lightSensor;
    private bool _readingChangedAttached;
    private string _sensorStatus;
    private string _lux;
    private readonly DispatcherQueue _dispatcher;
    private bool _noSensor = false;
    private Brush _backgroundBrush = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
    
    public AmbientLightSensorViewModel(DispatcherQueue dispatcher)
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

    public void Initialize()
    {
        _lightSensor = LightSensor.GetDefault();
        OnPropertyChanged(nameof(AmbientLightSensorAvailable));
        if (_lightSensor == null)
        {
            NoSensor = true;
            return;
        }

        Disposables.Add(Disposable.Create(() =>
        {
            if (_lightSensor != null)
            {
                _lightSensor.ReadingChanged -= AmbientLightSensor_ReadingChanged;
            }
        }));
        _lightSensor.ReadingChanged += AmbientLightSensor_ReadingChanged;
    }

    public bool AmbientLightSensorAvailable => _lightSensor != null;

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

    public string Lux
    {
        get => _lux;
        private set
        {
            _lux = value;
            OnPropertyChanged();
        }
    }

    public Brush BackgroundBrush
    {
        get => _backgroundBrush;
        set
        {
            _backgroundBrush = value;
            OnPropertyChanged();
        }
    }

    private void AmbientLightSensor_ReadingChanged(LightSensor sender, LightSensorReadingChangedEventArgs args)
    {
        _dispatcher.TryEnqueue(DispatcherQueuePriority.Normal, () =>
        {
            Lux = string.Format("{0:0}", Math.Round(args.Reading.IlluminanceInLux));
            var adjustedLux = Math.Max(Math.Min(args.Reading.IlluminanceInLux, 1000), 0);
            var ratio = adjustedLux / 1000;

            BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, (byte)(ratio * 255), (byte)(ratio * 255), (byte)(ratio * 255)));
        });
    }
}
