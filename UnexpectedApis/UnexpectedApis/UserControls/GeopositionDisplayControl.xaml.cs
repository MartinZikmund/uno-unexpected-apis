using Windows.Devices.Geolocation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace UnexpectedApis.UserControls;

public sealed partial class GeopositionDisplayControl : UserControl
{
    public GeopositionDisplayControl()
    {
        this.InitializeComponent();
    }

    public Geoposition Geoposition
    {
        get { return (Geoposition)GetValue(GeopositionProperty); }
        set { SetValue(GeopositionProperty, value); }
    }

    public static DependencyProperty GeopositionProperty { get; } =
        DependencyProperty.Register(nameof(Geoposition), typeof(Geoposition), typeof(GeopositionDisplayControl), new PropertyMetadata(null, GeopositionChanged));


    private static void GeopositionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is GeopositionDisplayControl control && args.NewValue is Geoposition geoposition)
        {
            control.LatitudeRun.Text = geoposition.Coordinate.Point.Position.Longitude.ToString("0.####");
            control.LongitudeRun.Text = geoposition.Coordinate.Point.Position.Latitude.ToString("0.####");
            control.AltitudeRun.Text = geoposition.Coordinate.Point.Position.Altitude.ToString("0.##") + " m";
            control.AccuracyRun.Text = geoposition.Coordinate.Accuracy.ToString("0") + " m";
            control.TimestampRun.Text = geoposition.Coordinate.Timestamp.ToString();
        }
    }
}
