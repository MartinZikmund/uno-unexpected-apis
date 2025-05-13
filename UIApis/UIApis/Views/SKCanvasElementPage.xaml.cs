using UIApis.ViewModels;

namespace UIApis.Views;

public sealed partial class SKCanvasElementPage : SamplePage
{
#if DESKTOP
    public int MaxSampleIndex => SampleSKCanvasElement.SampleCount - 1;
#endif

    public SKCanvasElementPage()
    {
        this.InitializeComponent();            
    }

    public string Code =>
"""
public class SampleSKCanvasElement : SKCanvasElement
{
    protected override void RenderOverride(SKCanvas canvas, Size area)
    {
        // Custom rendering logic goes here
    }
}
""";

    private void NextSample()
    {
#if DESKTOP
        CanvasElement.Sample = (CanvasElement.Sample + 1) % SampleSKCanvasElement.SampleCount;
#endif
    }
}
