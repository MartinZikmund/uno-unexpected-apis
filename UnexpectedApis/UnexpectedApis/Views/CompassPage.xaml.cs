using Microsoft.UI.Dispatching;
using UnexpectedApis.ViewModels;
using Windows.Devices.Sensors;

namespace UnexpectedApis.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CompassPage : Page
{
    private CompassViewModel _viewModel;

    public CompassPage()
    {
        this.InitializeComponent();
    }

    public CompassViewModel ViewModel => _viewModel ??= new CompassViewModel(DispatcherQueue);

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

public class CompassViewModel(DispatcherQueue dispatcher) : ViewModelBase
{
    private readonly DispatcherQueue _dispatcher = dispatcher;
    private bool _noSensor = false;
    private Compass? _compass = null;
    private double? _headingMagneticNorth;

    public IRelayCommand OpenDocs => new RelayCommand(async () => await Windows.System.Launcher.LaunchUriAsync(new Uri("https://developer.mozilla.org/en-US/docs/Web/API/Magnetometer")));

    public bool NoSensor
    {
        get => _noSensor;
        set
        {
            _noSensor = value;
            OnPropertyChanged();
        }
    }

    public bool CompassAvailable => _compass != null;

    public double? HeadingMagneticNorth
    {
        set
        {
            _headingMagneticNorth = value;
            OnPropertyChanged();
        }
        get => _headingMagneticNorth;
    }

    public void Initialize()
    {
        _compass = Compass.GetDefault();
        OnPropertyChanged(nameof(CompassAvailable));
        if (_compass == null)
        {
            NoSensor = true;
            return;
        }

        _compass.ReadingChanged += Compass_ReadingChanged;
    }

    private void Compass_ReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
    {
        HeadingMagneticNorth = args.Reading.HeadingMagneticNorth;
    }

    public void Dispose()
    {
        if (_compass != null)
        {
            _compass.ReadingChanged -= Compass_ReadingChanged;
        }
    }
}
