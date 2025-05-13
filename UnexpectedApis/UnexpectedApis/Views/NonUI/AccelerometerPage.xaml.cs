using UnexpectedApis.ViewModels;
using UnexpectedApis.Attributes;

namespace UnexpectedApis.Views;

[Sample("Accelerometer", "Accelerometer.png", SampleKind.NonUI, DisabledPlatforms.SkiaDesktop)]
public sealed partial class AccelerometerPage : SamplePage
{
    public AccelerometerPage()
    {
        this.InitializeComponent();            
        Model = new AccelerometerViewModel(DispatcherQueue);
        DataContext = Model;
        this.Unloaded += AccelerometerPage_Unloaded;
    }

    public string Code =>
"""
var accelerometer = Accelerometer.GetDefault();
accelerometer.ReadingChanged += (s,e) =>
{
    var x = e.Reading.AccelerationX;
    var y = e.Reading.AccelerationY;
    var z = e.Reading.AccelerationZ;
};
""";

    public AccelerometerViewModel Model { get; }

    private void AccelerometerPage_Unloaded(object sender, RoutedEventArgs e)
    {
        Model.Dispose();
    }
}
