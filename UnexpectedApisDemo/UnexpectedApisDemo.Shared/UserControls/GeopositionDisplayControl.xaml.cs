using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UnexpectedApisDemo.Shared.UserControls
{
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
		
		public static DependencyProperty GeopositionProperty { get ; } =
			DependencyProperty.Register(nameof(Geoposition), typeof(Geoposition), typeof(GeopositionDisplayControl), new PropertyMetadata(null, GeopositionChanged));		


		private static void GeopositionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
			if (dependencyObject is GeopositionDisplayControl control && args.NewValue is Geoposition geoposition)
			{
				control.LatitudeRun.Text = geoposition.Coordinate.Point.Position.Longitude.ToString("0.##") + " m";
				control.LongitudeRun.Text = geoposition.Coordinate.Point.Position.Latitude.ToString("0.##") + " m";
				control.AltitudeRun.Text = geoposition.Coordinate.Point.Position.Altitude.ToString("0.##") + " m";				
				control.AccuracyRun.Text = geoposition.Coordinate.Accuracy.ToString("0") + " m";								
				control.TimestampRun.Text = geoposition.Coordinate.Timestamp.ToString();
			}
		}
	}
}
