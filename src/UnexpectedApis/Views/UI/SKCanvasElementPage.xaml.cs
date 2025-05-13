using UnexpectedApis.ViewModels;

namespace UnexpectedApis.Views;

public sealed partial class SKCanvasElementPage : SamplePage
{
#if DESKTOP
    public int MaxSampleIndex => SampleSKCanvasElement.SampleCount - 1;

    private SampleSKCanvasElement _canvasElement;
#endif

    public SKCanvasElementPage()
    {
        this.InitializeComponent();

#if DESKTOP
        SKContainer.Children.Add(_canvasElement = new SampleSKCanvasElement());
#endif
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
        _canvasElement.Sample = (_canvasElement.Sample + 1) % SampleSKCanvasElement.SampleCount;
#endif
    }
}
