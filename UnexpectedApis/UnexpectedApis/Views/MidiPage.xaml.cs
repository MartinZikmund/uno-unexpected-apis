using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnexpectedApis.Helpers;
using UnexpectedApis.UserControls;
using Windows.Devices.Midi;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace UnexpectedApis.Views;

public sealed partial class MidiPage : Page
{
    /// <summary>
    /// Device watcher for MIDI out ports
    /// </summary>
    private readonly MidiDeviceWatcher _midiOutDeviceWatcher;

    private IMidiOutPort _currentDevice;

    public MidiPage()
    {
        this.InitializeComponent();

        // Set up the MIDI output device watcher
        _midiOutDeviceWatcher = new MidiDeviceWatcher(MidiOutPort.GetDeviceSelector(), Dispatcher, OutputDevicesList, OutputDevices);
        OutputDevicesList.ItemsSource = OutputDevices;

        // Start watching for devices
        _midiOutDeviceWatcher.Start();

        Unloaded += MidiDeviceOutputTests_Unloaded;
    }

    public ObservableCollection<MidiDeviceInfo> OutputDevices { get; } = new ObservableCollection<MidiDeviceInfo>();

    public ObservableCollection<string> MessageTypeItems { get; } = new ObservableCollection<string>();

    private async void MidiDeviceSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedDevice = (MidiDeviceInfo)OutputDevicesList.SelectedItem;
        if (selectedDevice != null)
        {
            _currentDevice?.Dispose();
            _currentDevice = await MidiOutPort.FromIdAsync(selectedDevice.Id);
            KeyboardKeys.IsEnabled = true;
        }
    }

    private void MidiDeviceOutputTests_Unloaded(object sender, RoutedEventArgs e)
    {
        // Stop the output device watcher
        _midiOutDeviceWatcher.Stop();

        _currentDevice?.Dispose();
    }

    private async void KeyClicked(object sender, RoutedEventArgs e)
    {
        if (_currentDevice != null)
        {
            var key = (KeyboardKey)sender;
            var note = 60;
            switch (key.KeyText.ToUpperInvariant())
            {
                case "C4":
                    note = 60;
                    break;
                case "D":
                    note = 62;
                    break;
                case "E":
                    note = 64;
                    break;
                case "F":
                    note = 65;
                    break;
                case "G":
                    note = 67;
                    break;
                case "A":
                    note = 69;
                    break;
                case "B":
                    note = 71;
                    break;
                case "C5":
                    note = 72;
                    break;
            }

            await PlayNoteAsync((byte)note);
        }
    }

    private async void HappyBirthday_Click(object sender, RoutedEventArgs args)
    {
        if (_currentDevice != null)
        {
            await PlayHappyBirthday();
        }
    }

    private const byte ENote = 64;
    private const byte DNote = 62;
    private const byte CNote = 60;
    private const byte HighCNote = 72;
    private const byte FNote = 65;
    private const byte GNote = 67;
    private const byte ANote = 69;
    private const byte ASharpNote = 70;
    private const int Skip = 400;

    private async Task PlayHappyBirthday()
    {
        await PlayNoteAsync(CNote, Skip / 2, 127);
        await PlayNoteAsync(CNote, Skip / 2, 127);
        await PlayNoteAsync(DNote);
        await PlayNoteAsync(CNote);
        await PlayNoteAsync(FNote);
        await PlayNoteAsync(ENote, Skip * 2, 127);

        await PlayNoteAsync(CNote, Skip / 2, 127);
        await PlayNoteAsync(CNote, Skip / 2, 127);
        await PlayNoteAsync(DNote);
        await PlayNoteAsync(CNote);
        await PlayNoteAsync(GNote);
        await PlayNoteAsync(FNote, Skip * 2, 127);

        await PlayNoteAsync(CNote, Skip / 2, 127);
        await PlayNoteAsync(CNote, Skip / 2, 127);
        await PlayNoteAsync(HighCNote, Skip * 2, 127);
        await PlayNoteAsync(ANote);
        await PlayNoteAsync(FNote);
        await PlayNoteAsync(ENote);
        await PlayNoteAsync(DNote);

        await PlayNoteAsync(ASharpNote, Skip / 2, 127);
        await PlayNoteAsync(ASharpNote, Skip / 2, 127);
        await PlayNoteAsync(ANote);
        await PlayNoteAsync(FNote);
        await PlayNoteAsync(GNote);
        await PlayNoteAsync(FNote, Skip * 2, 127);
    }

    private async Task PlayNoteAsync(byte noteNumber, int duration = Skip, byte velocity = 127)
    {
        _currentDevice?.SendMessage(new MidiNoteOnMessage(0, noteNumber, velocity));
        await Task.Delay(duration);
        _currentDevice?.SendMessage(new MidiNoteOffMessage(0, noteNumber, velocity));
    }
}
