using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;

namespace UnexpectedApisDemo.Shared.ViewModels
{
    [Bindable]
    public class AccelerometerViewModel : ViewModelBase, IDisposable
    {
        private CoreDispatcher _dispatcher;
        private readonly Accelerometer _accelerometer = null;
        private bool _readingChangedAttached;        
        private double _accelerationX;
        private double _accelerationY;
        private double _accelerationZ;
        private string _readingTimestamp;
        private string _shakenTimestamp;
        private string _sensorStatus;

        public AccelerometerViewModel(CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _accelerometer = Accelerometer.GetDefault();
            if (_accelerometer != null)
            {
                _accelerometer.ReportInterval = 250;
                SensorStatus = "Accelerometer ready";
            }
            else
            {
                SensorStatus = "Accelerometer not available on this device";
            }
        }

        public Command ObserveCommand => new Command((p) =>
        {
            if (!ReadingChangedAttached)
            {
                _accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            }
            else
            {
                _accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
            }
            ReadingChangedAttached = !ReadingChangedAttached;
        });

        public bool AccelerometerAvailable => _accelerometer != null;

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
            set
            {
                _readingChangedAttached = value;
                RaisePropertyChanged();
            }
        }

        public double AccelerationX
        {
            get => _accelerationX;
            set
            {
                _accelerationX = value;
                RaisePropertyChanged();
            }
        }

        public double AccelerationY
        {
            get => _accelerationY;
            set
            {
                _accelerationY = value;
                RaisePropertyChanged();
            }
        }

        public double AccelerationZ
        {
            get => _accelerationZ;
            set
            {
                _accelerationZ = value;
                RaisePropertyChanged();
            }
        }

        public string ReadingTimestamp
        {
            get => _readingTimestamp;
            set
            {
                _readingTimestamp = value;
                RaisePropertyChanged();
            }
        }

        public string ShakenTimestamp
        {
            get => _shakenTimestamp;
            set
            {
                _shakenTimestamp = value;
                RaisePropertyChanged();
            }
        }

        private async void Accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AccelerationX = args.Reading.AccelerationX;
                AccelerationY = args.Reading.AccelerationY;
                AccelerationZ = args.Reading.AccelerationZ;
                ReadingTimestamp = args.Reading.Timestamp.ToString("R");
            });
        }

        private async void Accelerometer_Shaken(Accelerometer sender, AccelerometerShakenEventArgs args)
        {
            await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () => ShakenTimestamp = args.Timestamp.ToString("R"));
        }

        public void Dispose()
        {
            if (_accelerometer != null)
            {
                _accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
                _accelerometer.Shaken -= Accelerometer_Shaken;
            }
        }
    }
}
