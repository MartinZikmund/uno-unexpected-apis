using UnexpectedApisDemo.Shared.ViewModels;
using Windows.Gaming.Input;

namespace UnexpectedApisDemo.Shared.ViewModels.Items
{
    public class GamepadViewModel : ViewModelBase
    {
        private static int _id = 0;

        public GamepadViewModel(Gamepad gamepad)
        {
            Id = ++_id;
            Gamepad = gamepad;
        }

        public Gamepad Gamepad { get; }

        public int Id { get; }

        public void Update()
        {
            var reading = Gamepad.GetCurrentReading();
            Buttons = reading.Buttons.ToString("g");
            RightThumbstickX = reading.RightThumbstickX.ToString("0.00");
            RightThumbstickY = reading.RightThumbstickY.ToString("0.00");
            LeftThumbstickX = reading.LeftThumbstickX.ToString("0.00");
            LeftThumbstickY = reading.LeftThumbstickY.ToString("0.00");
            LeftTrigger = reading.LeftTrigger.ToString("0.00");
            RightTrigger = reading.RightTrigger.ToString("0.00");

            RaisePropertyChanged("");
        }

        public string Buttons { get; private set; }

        public string RightThumbstickX { get; private set; }

        public string RightThumbstickY { get; private set; }

        public string LeftThumbstickX { get; private set; }

        public string LeftThumbstickY { get; private set; }

        public string LeftTrigger { get; private set; }

        public string RightTrigger { get; private set; }
    }
}
