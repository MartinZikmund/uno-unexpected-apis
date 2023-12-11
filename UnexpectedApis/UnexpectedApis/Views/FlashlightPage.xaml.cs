using Microsoft.UI.Dispatching;
using UnexpectedApisDemo.Shared.ViewModels;
using Windows.Devices.Lights;
using Windows.UI.Core;

namespace UnexpectedApisDemo.Shared.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class FlashlightPage : Page
{
    private FlashlightViewModel _viewModel;

    public FlashlightPage()
    {
        this.InitializeComponent();
    }

    public FlashlightViewModel ViewModel => _viewModel ?? (_viewModel = new FlashlightViewModel(DispatcherQueue));

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

public class FlashlightViewModel : ViewModelBase
{
    private bool _readingChangedAttached;
    private string _sensorStatus;
    private string _angle;
    private readonly DispatcherQueue _dispatcher;
    private bool _noSensor = false;
    private Lamp _lamp = null;
    private string _lampStatus;

    public FlashlightViewModel(DispatcherQueue dispatcher)
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

    public bool FlashlightAvailable => _lamp != null;

    public async Task InitializeAsync()
    {
        _lamp = await Lamp.GetDefaultAsync();
        OnPropertyChanged(nameof(FlashlightAvailable));
        if (_lamp == null)
        {
            NoSensor = true;
            return;
        }

        Disposables.Add(_lamp);
        _lamp.BrightnessLevel = 1;
    }
    
    public bool IsAvailable => _lamp != null;

    public bool FlashlightOn
    {
        get
        {
            return _lamp?.IsEnabled ?? false;
        }
        set
        {
            if (_lamp != null)
            {
                _lamp.IsEnabled = value;
            }
            OnPropertyChanged();
        }
    }
}
