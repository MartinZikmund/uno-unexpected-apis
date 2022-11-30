using System;
using System.IO;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using UnexpectedApisDemo.Shared.ViewModels;
using Windows.Devices.Sensors;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UnexpectedApisDemo.Shared.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AmbientLightPage : Page
    {
        private AmbientLightSensorViewModel _viewModel;

        public AmbientLightPage()
        {
            this.InitializeComponent();
        }

        public AmbientLightSensorViewModel ViewModel => _viewModel ?? (_viewModel = new AmbientLightSensorViewModel(Dispatcher));

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
        private LightSensor _hinge;
        private bool _readingChangedAttached;
        private string _sensorStatus;
        private string _lux;
        private readonly CoreDispatcher _dispatcher;
        private bool _noSensor = false;
        private Brush _backgroundBrush = new SolidColorBrush(Colors.Transparent);
        
        public AmbientLightSensorViewModel(CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public bool NoSensor
        {
            get => _noSensor;
            set
            {
                _noSensor = value;
                RaisePropertyChanged();
            }
        }

        public void Initialize()
        {
            _hinge = LightSensor.GetDefault();
            RaisePropertyChanged(nameof(AmbientLightSensorAvailable));
            if (_hinge == null)
            {
                NoSensor = true;
                return;
            }

            Disposables.Add(Disposable.Create(() =>
            {
                if (_hinge != null)
                {
                    _hinge.ReadingChanged -= AmbientLightSensor_ReadingChanged;
                }
            }));
            _hinge.ReadingChanged += AmbientLightSensor_ReadingChanged;
        }

        public bool AmbientLightSensorAvailable => _hinge != null;

        public string SensorStatus
        {
            get => _sensorStatus;
            private set
            {
                _sensorStatus = value;
                RaisePropertyChanged();
            }
        }

        public bool ReadingChangedAttached
        {
            get => _readingChangedAttached;
            private set
            {
                _readingChangedAttached = value;
                RaisePropertyChanged();
            }
        }

        public string Lux
        {
            get => _lux;
            private set
            {
                _lux = value;
                RaisePropertyChanged();
            }
        }

        public Brush BackgroundBrush
        {
            get => _backgroundBrush;
            set
            {
                _backgroundBrush = value;
                RaisePropertyChanged();
            }
        }

        private async void AmbientLightSensor_ReadingChanged(LightSensor sender, LightSensorReadingChangedEventArgs args)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Lux = string.Format("{0:0}", Math.Round(args.Reading.IlluminanceInLux));
                var adjustedLux = Math.Max(Math.Min(args.Reading.IlluminanceInLux, 1000), 0);
                var ratio = adjustedLux / 1000;

                BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, (byte)(ratio * 255), (byte)(ratio * 255), (byte)(ratio * 255)));
            });
        }
    }
}
