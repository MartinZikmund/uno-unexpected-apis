using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace UnexpectedApisDemo.Shared.Helpers
{
    internal class MidiDeviceWatcher
    {
        private readonly DeviceWatcher _deviceWatcher = null;
        private readonly ListView _portList = null;
        private readonly ObservableCollection<MidiDeviceInfo> _items;
        private readonly string _midiSelector = string.Empty;
        private readonly CoreDispatcher _coreDispatcher = null;

        private DeviceInformationCollection _deviceInformationCollection = null;
        private bool _enumerationCompleted = false;

        /// <summary>
        /// Constructor: Initialize and hook up Device Watcher events
        /// </summary>
        /// <param name="midiSelectorString">MIDI Device Selector</param>
        /// <param name="dispatcher">CoreDispatcher instance, to update UI thread</param>
        /// <param name="portListBox">The UI element to update with list of devices</param>
        internal MidiDeviceWatcher(string midiSelectorString, CoreDispatcher dispatcher, ListView portListBox, ObservableCollection<MidiDeviceInfo> items)
        {
            _deviceWatcher = DeviceInformation.CreateWatcher(midiSelectorString);
            _portList = portListBox;
            _items = items;
            _midiSelector = midiSelectorString;
            _coreDispatcher = dispatcher;

            _deviceWatcher.Added += DeviceWatcher_Added;
            _deviceWatcher.Removed += DeviceWatcher_Removed;
            _deviceWatcher.Updated += DeviceWatcher_Updated;
            _deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
        }

        /// <summary>
        /// Destructor: Remove Device Watcher events
        /// </summary>
        ~MidiDeviceWatcher()
        {
            _deviceWatcher.Added -= DeviceWatcher_Added;
            _deviceWatcher.Removed -= DeviceWatcher_Removed;
            _deviceWatcher.Updated -= DeviceWatcher_Updated;
            _deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
        }

        /// <summary>
        /// Start the Device Watcher
        /// </summary>
        internal void Start()
        {
            if (_deviceWatcher.Status != DeviceWatcherStatus.Started)
            {
                _deviceWatcher.Start();
            }
        }

        /// <summary>
        /// Stop the Device Watcher
        /// </summary>
        internal void Stop()
        {
            if (_deviceWatcher.Status != DeviceWatcherStatus.Stopped)
            {
                _deviceWatcher.Stop();
            }
        }

        /// <summary>
        /// Get the DeviceInformationCollection
        /// </summary>
        /// <returns></returns>
        internal DeviceInformationCollection GetDeviceInformationCollection()
        {
            return _deviceInformationCollection;
        }

        /// <summary>
        /// Add any connected MIDI devices to the list
        /// </summary>
        private async Task UpdateDevicesAsync()
        {
            // Get a list of all MIDI devices
            _deviceInformationCollection = await DeviceInformation.FindAllAsync(_midiSelector);

            // If no devices are found, update the ListBox
            if ((_deviceInformationCollection == null) || (_deviceInformationCollection.Count == 0))
            {
                // Start with a clean list
                _items.Clear();

                _portList.IsEnabled = false;
            }
            // If devices are found, enumerate them and add them to the list
            else
            {
                // Start with a clean list
                _items.Clear();

                foreach (var device in _deviceInformationCollection)
                {
                    _items.Add(new MidiDeviceInfo() { Name = device.Name, Id = device.Id });
                }

                _portList.IsEnabled = true;
            }
        }

        /// <summary>
        /// Update UI on device added
        /// </summary>
        /// <param name="sender">The active DeviceWatcher instance</param>
        /// <param name="args">Event arguments</param>
        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            // If all devices have been enumerated
            if (_enumerationCompleted)
            {
                await _coreDispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                {
                    // Update the device list
                    await UpdateDevicesAsync();
                });
            }
        }

        /// <summary>
        /// Update UI on device removed
        /// </summary>
        /// <param name="sender">The active DeviceWatcher instance</param>
        /// <param name="args">Event arguments</param>
        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // If all devices have been enumerated
            if (_enumerationCompleted)
            {
                await _coreDispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                {
                    // Update the device list
                    await UpdateDevicesAsync();
                });
            }
        }

        /// <summary>
        /// Update UI on device updated
        /// </summary>
        /// <param name="sender">The active DeviceWatcher instance</param>
        /// <param name="args">Event arguments</param>
        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // If all devices have been enumerated
            if (_enumerationCompleted)
            {
                await _coreDispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                {
                    // Update the device list
                    await UpdateDevicesAsync();
                });
            }
        }

        /// <summary>
        /// Update UI on device enumeration completed.
        /// </summary>
        /// <param name="sender">The active DeviceWatcher instance</param>
        /// <param name="args">Event arguments</param>
        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            _enumerationCompleted = true;
            await _coreDispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                // Update the device list
                await UpdateDevicesAsync();
            });
        }
    }
}
