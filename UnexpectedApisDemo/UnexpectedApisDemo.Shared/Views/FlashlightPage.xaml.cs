using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using UnexpectedApisDemo.Shared.ViewModels;
using Windows.Devices.Lights;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UnexpectedApisDemo.Shared.Views
{
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

        public FlashlightViewModel ViewModel => _viewModel ?? (_viewModel = new FlashlightViewModel(Dispatcher));

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
        private readonly CoreDispatcher _dispatcher;
        private bool _noSensor = false;
        private Lamp _lamp = null;
        private string _lampStatus;

        public FlashlightViewModel(CoreDispatcher dispatcher)
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

        public bool FlashlightAvailable => _lamp != null;

        public async Task InitializeAsync()
        {
            _lamp = await Lamp.GetDefaultAsync();
            RaisePropertyChanged(nameof(FlashlightAvailable));
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
                RaisePropertyChanged();
            }
        }
    }
}
