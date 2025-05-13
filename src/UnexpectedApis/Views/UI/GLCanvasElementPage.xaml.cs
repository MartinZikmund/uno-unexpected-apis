using UnexpectedApis.ViewModels;

namespace UnexpectedApis.Views;

public sealed partial class GLCanvasElementPage : SamplePage
{
    public GLCanvasElementPage()
    {
        this.InitializeComponent();
#if !__ANDROID__ && !__IOS__ && !WINDOWS && !__WASM__
        GLContainer.Children.Add(new RotatingCubeGlCanvasElement());
#endif
    }

    public string Code =>
"""
public class MyGlCanvasElement() : GLCanvasElement(() => ((App)Application.Current).MainWindow)
{
    protected override unsafe void RenderOverride(GL Gl)
    {
        // Custom rendering logic goes here
    }
}
""";
}
