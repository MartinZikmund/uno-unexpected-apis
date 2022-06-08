using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnexpectedApisDemo.Shared.ViewModels.Items;
using Windows.Gaming.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UnexpectedApisDemo.Shared.Views
{
    public sealed partial class GamepadPage : Page
    {
        private DispatcherTimer _timer = new DispatcherTimer();
        
        public GamepadPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            Gamepad.GamepadAdded += GamepadsChanged;
            Gamepad.GamepadRemoved += GamepadsChanged;

            _timer.Tick += OnGamepadReadingUpdate;
            _timer.Start();

            UpdateGamepads();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Gamepad.GamepadAdded -= GamepadsChanged;
            Gamepad.GamepadRemoved -= GamepadsChanged;

            _timer.Stop();
            _timer.Tick -= OnGamepadReadingUpdate;
        }

        private void OnGamepadReadingUpdate(object sender, object e)
        {
            UpdateGamepads();
            foreach (var gamepad in Gamepads)
            {
                gamepad.Update();
            }
        }

        public ObservableCollection<GamepadViewModel> Gamepads { get; } = new ObservableCollection<GamepadViewModel>();

        private void GamepadsChanged(object sender, Gamepad e)
        {
            UpdateGamepads();
        }

        private async void UpdateGamepads()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var gamepads = Gamepad.Gamepads.ToArray();

                var existingGamepads = new HashSet<Gamepad>(Gamepads.Select(g => g.Gamepad));

                var toRemove = existingGamepads.Except(gamepads).ToArray();
                var toAdd = gamepads.Except(existingGamepads).ToArray();

                foreach (var gamepad in toRemove)
                {
                    var vmToRemove = Gamepads.FirstOrDefault(g => g.Gamepad == gamepad);
                    Gamepads.Remove(vmToRemove);
                }

                foreach (var gamepad in toAdd)
                {
                    var vmToAdd = new GamepadViewModel(gamepad);
                    Gamepads.Add(vmToAdd);
                }
            });
        }
    }
}
