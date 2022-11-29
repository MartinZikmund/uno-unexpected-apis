using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using UnexpectedApisDemo.Shared.ViewModels;
using Windows.Devices.Lights;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public FlashlightPage()
        {
            this.InitializeComponent();
        }

        public FlashlightViewModel ViewModel { get; } = new FlashlightViewModel();
    }

    public class FlashlightViewModel : ViewModelBase
    {
        private Lamp _lamp = null;
        private string _lampStatus;

        public FlashlightViewModel()
        {
            GetLampAsync();
        }

        public ICommand GetLampCommand => GetOrCreateCommand(GetLampAsync);

        public ICommand ToggleCommand => GetOrCreateCommand(ToggleLamp);

        public string LampStatus
        {
            get => _lampStatus;
            private set
            {
                _lampStatus = value;
                RaisePropertyChanged();
            }
        }

        public bool IsAvailable => _lamp != null;

        public bool IsEnabled
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

        public float BrightnessLevel
        {
            get => _lamp?.BrightnessLevel ?? 0.0f;
            set
            {
                if (_lamp != null)
                {
                    _lamp.BrightnessLevel = value;
                }
            }
        }

        private async void GetLampAsync()
        {
            _lamp = await Lamp.GetDefaultAsync();
            if (_lamp == null)
            {
                LampStatus = "Flashlight is not available";
            }
            else
            {
                LampStatus = "Flashlight is available";
            }
            RaisePropertyChanged(nameof(IsAvailable));
            RaisePropertyChanged(nameof(IsEnabled));
            RaisePropertyChanged(nameof(BrightnessLevel));
        }

        private void ToggleLamp()
        {
            _lamp.IsEnabled = !_lamp.IsEnabled;
            RaisePropertyChanged(nameof(IsEnabled));
        }
    }
}
