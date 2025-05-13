using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Dispatching;

namespace UnexpectedApis.ViewModels;

[Bindable]
public class AccelerometerViewModel : ViewModelBase, IDisposable
{
    private DispatcherQueue _dispatcherQueue;
    private readonly Accelerometer? _accelerometer = null;
    private bool _readingChangedAttached;        
    private string _accelerationX;
    private string _accelerationY;
    private string _accelerationZ;
    private string _readingTimestamp;
    private string _shakenTimestamp;
    private string _sensorStatus;

    public AccelerometerViewModel(DispatcherQueue dispatcherQueue)
    {
        _dispatcherQueue = dispatcherQueue;
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

    public ICommand ObserveCommand => GetOrCreateCommand(() =>
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
            OnPropertyChanged();
        }
    }

    public bool ReadingChangedAttached
    {
        get => _readingChangedAttached;
        set
        {
            _readingChangedAttached = value;
            OnPropertyChanged();
        }
    }

    public string AccelerationX
    {
        get => _accelerationX;
        set
        {
            _accelerationX = value;
            OnPropertyChanged();
        }
    }

    public string AccelerationY
    {
        get => _accelerationY;
        set
        {
            _accelerationY = value;
            OnPropertyChanged();
        }
    }

    public string AccelerationZ
    {
        get => _accelerationZ;
        set
        {
            _accelerationZ = value;
            OnPropertyChanged();
        }
    }

    public string ReadingTimestamp
    {
        get => _readingTimestamp;
        set
        {
            _readingTimestamp = value;
            OnPropertyChanged();
        }
    }

    public string ShakenTimestamp
    {
        get => _shakenTimestamp;
        set
        {
            _shakenTimestamp = value;
            OnPropertyChanged();
        }
    }

    private async void Accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
    {
        _dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () =>
        {
            AccelerationX = args.Reading.AccelerationX.ToString("0.00");
            AccelerationY = args.Reading.AccelerationY.ToString("0.00");
            AccelerationZ = args.Reading.AccelerationZ.ToString("0.00");
            ReadingTimestamp = args.Reading.Timestamp.ToString("R");
        });
    }

    private async void Accelerometer_Shaken(Accelerometer sender, AccelerometerShakenEventArgs args)
    {
        _dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal,
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
