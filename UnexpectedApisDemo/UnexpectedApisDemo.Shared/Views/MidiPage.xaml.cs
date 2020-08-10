using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnexpectedApisDemo.Shared.Helpers;
using Windows.Devices.Midi;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UnexpectedApisDemo.Shared.Views
{
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

        public ObservableCollection<string> OutputDevices { get; } = new ObservableCollection<string>();

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
            KeyboardKeys.IsEnabled = false;
        }

        private void MidiDeviceOutputTests_Unloaded(object sender, RoutedEventArgs e)
        {
            // Stop the output device watcher
            _midiOutDeviceWatcher.Stop();

            _currentDevice?.Dispose();
        }
    }
}
