#if !WINDOWS
using Uno.WinUI.Graphics2DSK;
#endif

namespace UnexpectedApis.Apis;

public sealed partial class SKCanvasElementDemo : UserControl
{
#if !WINDOWS
    private SampleSKCanvasElement? _canvas;
#endif

    public SKCanvasElementDemo()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
#if !WINDOWS
        if (_canvas is not null)
        {
            return;
        }

        try
        {
            if (!SKCanvasElement.IsSupportedOnCurrentPlatform())
            {
                Unavailable("SKCanvasElement needs the Skia renderer, and this head doesn't use it.");
                return;
            }

            _canvas = new SampleSKCanvasElement();
            Host.Children.Insert(0, _canvas);
            UpdateIndex();
        }
        catch (Exception ex)
        {
            Unavailable($"SKCanvasElement isn't available here: {ex.Message}");
        }
#else
        Unavailable("The Windows App SDK head renders with WinUI, not Skia, so SKCanvasElement isn't supported there.");
#endif
    }

    private void OnNextClick(object sender, RoutedEventArgs e)
    {
#if !WINDOWS
        if (_canvas is not null)
        {
            _canvas.Sample = (_canvas.Sample + 1) % SampleSKCanvasElement.SampleCount;
            UpdateIndex();
        }
#endif
    }

#if !WINDOWS
    private void UpdateIndex() => IndexText.Text = $"Drawing {_canvas!.Sample + 1} of {SampleSKCanvasElement.SampleCount}";
#endif

    private void Unavailable(string message)
    {
        NextButton.IsEnabled = false;
        MessageText.Text = message;
        MessageText.Visibility = Visibility.Visible;
    }
}
