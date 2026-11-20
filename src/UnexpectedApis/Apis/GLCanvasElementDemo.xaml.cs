#if DESKTOP || WINDOWS || __WASM__
using Uno.WinUI.Graphics3DGL;
#endif

namespace UnexpectedApis.Apis;

public sealed partial class GLCanvasElementDemo : UserControl
{
#if DESKTOP || WINDOWS || __WASM__
    private RotatingCubeGlCanvasElement? _cube;
#endif

    public GLCanvasElementDemo()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
#if DESKTOP || WINDOWS || __WASM__
        try
        {
            if (_cube is null)
            {
                _cube = new RotatingCubeGlCanvasElement();
                _cube.RegisterPropertyChangedCallback(GLCanvasElement.IsGLInitializedProperty, OnGLInitializedChanged);
                Host.Children.Insert(0, _cube);
            }
            else
            {
                _cube.Start();
            }
        }
        catch (Exception ex)
        {
            Show($"OpenGL isn't available here: {ex.Message}");
        }
#else
        Show("GLCanvasElement needs OpenGL, which this platform's head doesn't provide.");
#endif
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
#if DESKTOP || WINDOWS || __WASM__
        _cube?.Stop();
#endif
    }

#if DESKTOP || WINDOWS || __WASM__
    private void OnGLInitializedChanged(DependencyObject sender, DependencyProperty property)
    {
        if (_cube?.IsGLInitialized == false)
        {
            Show("OpenGL couldn't start on this machine, so there's no cube to show.");
        }
    }
#endif

    private void Show(string message)
    {
        MessageText.Text = message;
        MessageText.Visibility = Visibility.Visible;
    }
}
