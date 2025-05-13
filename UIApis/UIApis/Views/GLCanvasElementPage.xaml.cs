using UIApis.ViewModels;

namespace UIApis.Views;

public sealed partial class GLCanvasElementPage : SamplePage
{
    public GLCanvasElementPage()
    {
        this.InitializeComponent();            
        Model = new AccelerometerViewModel(DispatcherQueue);
        DataContext = Model;
        this.Unloaded += AccelerometerPage_Unloaded;
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

    public AccelerometerViewModel Model { get; }

    private void AccelerometerPage_Unloaded(object sender, RoutedEventArgs e)
    {
        Model.Dispose();
    }
}
