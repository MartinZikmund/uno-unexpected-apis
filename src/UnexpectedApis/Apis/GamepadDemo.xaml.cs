// Not implemented on every head; calls are guarded and the demo says so.
#pragma warning disable Uno0001

using Windows.Gaming.Input;

namespace UnexpectedApis.Apis;

public sealed partial class GamepadDemo : UserControl
{
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(100) };
    private bool _subscribed;

    public GamepadDemo()
    {
        InitializeComponent();
        _timer.Tick += OnTick;
        Loaded += (_, _) => Start();
        Unloaded += (_, _) => Stop();
    }

    private void OnObserveToggled(object sender, RoutedEventArgs e)
    {
        if (ObserveSwitch.IsOn)
        {
            Start();
        }
        else
        {
            Stop();
            StatusText.Text = "Not observing.";
        }
    }

    private void Start()
    {
        if (_subscribed || !ObserveSwitch.IsOn)
        {
            return;
        }

        try
        {
            Gamepad.GamepadAdded += OnGamepadsChanged;
            Gamepad.GamepadRemoved += OnGamepadsChanged;
            _subscribed = true;
            _timer.Start();
            Refresh();
        }
        catch (Exception ex) when (ex is NotImplementedException or NotSupportedException or UnauthorizedAccessException)
        {
            _subscribed = false;
            StatusText.Text = "Gamepads aren't available on this platform.";
        }
    }

    private void Stop()
    {
        _timer.Stop();
        if (_subscribed)
        {
            try
            {
                Gamepad.GamepadAdded -= OnGamepadsChanged;
                Gamepad.GamepadRemoved -= OnGamepadsChanged;
            }
            catch (Exception ex) when (ex is NotImplementedException or NotSupportedException)
            {
            }

            _subscribed = false;
        }

        GamepadsPanel.Children.Clear();
    }

    private void OnGamepadsChanged(object? sender, Gamepad gamepad) =>
        DispatcherQueue.TryEnqueue(Refresh);

    private void OnTick(object? sender, object e) => Refresh();

    private void Refresh()
    {
        try
        {
            IReadOnlyList<Gamepad> gamepads = Gamepad.Gamepads;
            var views = GamepadsPanel.Children.OfType<GamepadView>().ToList();

            foreach (GamepadView view in views.Where(v => !gamepads.Contains(v.Gamepad)))
            {
                GamepadsPanel.Children.Remove(view);
            }

            foreach (Gamepad gamepad in gamepads.Where(g => !views.Any(v => v.Gamepad == g)))
            {
                GamepadsPanel.Children.Add(new GamepadView(gamepad));
            }

            int position = 1;
            foreach (GamepadView view in GamepadsPanel.Children.OfType<GamepadView>())
            {
                view.Update(position++);
            }

            StatusText.Text = gamepads.Count == 0
                ? "No gamepads connected. Plug one in or press a button on a paired one."
                : $"{gamepads.Count} gamepad(s) connected.";
        }
        catch (Exception ex) when (ex is NotImplementedException or NotSupportedException or UnauthorizedAccessException)
        {
            _timer.Stop();
            StatusText.Text = "Gamepads aren't available on this platform.";
        }
    }
}

