// Not implemented on every head; calls are guarded and the demo says so.
#pragma warning disable Uno0001

using Microsoft.UI.Xaml.Shapes;
using Windows.Gaming.Input;

namespace UnexpectedApis.Apis;

public sealed partial class GamepadView : UserControl
{
    private Brush? _lit;

    public GamepadView(Gamepad gamepad)
    {
        Gamepad = gamepad;
        InitializeComponent();
    }

    public Gamepad Gamepad { get; }

    public void Update(int position)
    {
        _lit ??= (Brush)Application.Current.Resources["AccentFillColorDefaultBrush"];
        GamepadReading reading = Gamepad.GetCurrentReading();

        Light(ButtonA, reading.Buttons.HasFlag(GamepadButtons.A));
        Light(ButtonX, reading.Buttons.HasFlag(GamepadButtons.X));
        Light(ButtonY, reading.Buttons.HasFlag(GamepadButtons.Y));
        Light(ButtonB, reading.Buttons.HasFlag(GamepadButtons.B));
        Light(DPadDown, reading.Buttons.HasFlag(GamepadButtons.DPadDown));
        Light(DPadLeft, reading.Buttons.HasFlag(GamepadButtons.DPadLeft));
        Light(DPadUp, reading.Buttons.HasFlag(GamepadButtons.DPadUp));
        Light(DPadRight, reading.Buttons.HasFlag(GamepadButtons.DPadRight));
        Light(ButtonView, reading.Buttons.HasFlag(GamepadButtons.View));
        Light(ButtonMenu, reading.Buttons.HasFlag(GamepadButtons.Menu));
        Light(Paddle1, reading.Buttons.HasFlag(GamepadButtons.Paddle1));
        Light(Paddle2, reading.Buttons.HasFlag(GamepadButtons.Paddle2));
        Light(Paddle3, reading.Buttons.HasFlag(GamepadButtons.Paddle3));
        Light(Paddle4, reading.Buttons.HasFlag(GamepadButtons.Paddle4));
        Light(LeftShoulder, reading.Buttons.HasFlag(GamepadButtons.LeftShoulder));
        Light(RightShoulder, reading.Buttons.HasFlag(GamepadButtons.RightShoulder));
        Light(LeftStick, reading.Buttons.HasFlag(GamepadButtons.LeftThumbstick));
        Light(RightStick, reading.Buttons.HasFlag(GamepadButtons.RightThumbstick));

        Canvas.SetLeft(LeftStick, reading.LeftThumbstickX * 6 + 75);
        Canvas.SetTop(LeftStick, reading.LeftThumbstickY * -6 + 49);
        Canvas.SetLeft(RightStick, reading.RightThumbstickX * 6 + 141);
        Canvas.SetTop(RightStick, reading.RightThumbstickY * -6 + 80);
        LeftTriggerFill.Opacity = reading.LeftTrigger;
        RightTriggerFill.Opacity = reading.RightTrigger;

        TitleText.Text = $"Gamepad {position}";
        ButtonsText.Text = reading.Buttons == GamepadButtons.None ? "No buttons pressed" : reading.Buttons.ToString();
        LeftStickText.Text = $"Left stick  X {reading.LeftThumbstickX:0.00}  Y {reading.LeftThumbstickY:0.00}";
        RightStickText.Text = $"Right stick  X {reading.RightThumbstickX:0.00}  Y {reading.RightThumbstickY:0.00}";
        TriggersText.Text = $"Triggers  L {reading.LeftTrigger:0.00}  R {reading.RightTrigger:0.00}";
    }

    private void Light(Shape shape, bool pressed) => shape.Fill = pressed ? _lit : null;
}


