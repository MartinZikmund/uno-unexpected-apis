// Not implemented on every head; calls are guarded and the demo says so.
#pragma warning disable Uno0001

using System.Collections.ObjectModel;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;

namespace UnexpectedApis.Apis;

public sealed partial class MidiDemo : UserControl
{
    private const int Beat = 400;

    // (note, duration in beats x2) for Happy Birthday
    private static readonly (byte Note, int Duration)[] Birthday =
    [
        (60, 1), (60, 1), (62, 2), (60, 2), (65, 2), (64, 4),
        (60, 1), (60, 1), (62, 2), (60, 2), (67, 2), (65, 4),
        (60, 1), (60, 1), (72, 4), (69, 2), (65, 2), (64, 2), (62, 2),
        (70, 1), (70, 1), (69, 2), (65, 2), (67, 2), (65, 4),
    ];

    private readonly ObservableCollection<MidiDevice> _devices = new();
    private DeviceWatcher? _watcher;
    private IMidiOutPort? _port;
    private CancellationTokenSource? _playing;

    public MidiDemo()
    {
        InitializeComponent();
        DevicesList.ItemsSource = _devices;
        Loaded += (_, _) => StartWatching();
        Unloaded += (_, _) => StopWatching();
    }

    private void StartWatching()
    {
        if (_watcher is not null)
        {
            return;
        }

        try
        {
            _watcher = DeviceInformation.CreateWatcher(MidiOutPort.GetDeviceSelector());
            _watcher.Added += OnDevicesChanged;
            _watcher.Removed += OnDevicesChanged;
            _watcher.Updated += OnDevicesChanged;
            _watcher.EnumerationCompleted += OnDevicesChanged;
            _watcher.Start();
            StatusText.Text = "Looking for MIDI output devices...";
        }
        catch (Exception ex) when (ex is NotImplementedException or NotSupportedException or UnauthorizedAccessException)
        {
            _watcher = null;
            StatusText.Text = "MIDI isn't available on this platform.";
        }
    }

    private void StopWatching()
    {
        _playing?.Cancel();
        _playing = null;

        if (_watcher is not null)
        {
            try
            {
                _watcher.Added -= OnDevicesChanged;
                _watcher.Removed -= OnDevicesChanged;
                _watcher.Updated -= OnDevicesChanged;
                _watcher.EnumerationCompleted -= OnDevicesChanged;
                _watcher.Stop();
            }
            catch (Exception ex) when (ex is NotImplementedException or NotSupportedException or InvalidOperationException)
            {
            }

            _watcher = null;
        }

        _port?.Dispose();
        _port = null;
    }

    private void OnDevicesChanged(DeviceWatcher sender, object args) =>
        DispatcherQueue.TryEnqueue(async () => await RefreshDevicesAsync());

    private async Task RefreshDevicesAsync()
    {
        try
        {
            var found = await DeviceInformation.FindAllAsync(MidiOutPort.GetDeviceSelector());
            _devices.Clear();
            foreach (DeviceInformation device in found)
            {
                _devices.Add(new MidiDevice(device.Name, device.Id));
            }

            StatusText.Text = _devices.Count == 0
                ? "No MIDI output devices found. Connect one and it will show up here."
                : "Pick a device, then play.";
        }
        catch (Exception ex) when (ex is NotImplementedException or NotSupportedException or UnauthorizedAccessException)
        {
            StatusText.Text = "MIDI isn't available on this platform.";
        }
    }

    private async void OnDeviceSelected(object sender, SelectionChangedEventArgs e)
    {
        if (DevicesList.SelectedItem is not MidiDevice device)
        {
            return;
        }

        try
        {
            _port?.Dispose();
            _port = await MidiOutPort.FromIdAsync(device.Id);
            KeyboardPanel.IsEnabled = _port is not null;
            StatusText.Text = _port is null ? $"Couldn't open {device.Name}." : $"Connected to {device.Name}.";
        }
        catch (Exception ex) when (ex is NotImplementedException or NotSupportedException or UnauthorizedAccessException)
        {
            KeyboardPanel.IsEnabled = false;
            StatusText.Text = $"Couldn't open {device.Name}: {ex.Message}";
        }
    }

    private async void OnKeyClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string tag })
        {
            await PlayNoteAsync(byte.Parse(tag), Beat, CancellationToken.None);
        }
    }

    private async void OnBirthdayClick(object sender, RoutedEventArgs e)
    {
        _playing?.Cancel();
        CancellationTokenSource cts = new();
        _playing = cts;
        try
        {
            foreach (var (note, duration) in Birthday)
            {
                await PlayNoteAsync(note, Beat * duration / 2, cts.Token);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task PlayNoteAsync(byte note, int duration, CancellationToken token)
    {
        try
        {
            _port?.SendMessage(new MidiNoteOnMessage(0, note, 127));
            await Task.Delay(duration, token);
        }
        finally
        {
            _port?.SendMessage(new MidiNoteOffMessage(0, note, 127));
        }
    }

    private sealed record MidiDevice(string Name, string Id);
}


