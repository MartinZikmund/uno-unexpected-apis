using Mapsui;
using Mapsui.Projections;
using Mapsui.Tiling;
using UnexpectedApis.Attributes;
using Windows.Devices.Geolocation;

namespace UnexpectedApis.Views;

[Sample("MapsUI", "Geolocator.png", SampleKind.UI, TargetPlatforms.All & ~TargetPlatforms.SkiaWasm)]
public sealed partial class MapsUIPage : SamplePage
{
    public MapsUIPage()
    {
        this.InitializeComponent();

        MyMap.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        MyMap.Visibility = Visibility.Visible;

        if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Devices.Geolocation.Geolocator"))
        {
            GeolocatorButton.Visibility = Visibility.Visible;
        }
        else
        {
            GeolocatorButton.Visibility = Visibility.Collapsed;
            DisplayMap(14.3893686, 50.0910957);
        }
    }

    public string Code =>
"""

""";

    private async void GetGeoposition_Click(object sender, RoutedEventArgs args)
    {
        if (await Geolocator.RequestAccessAsync() == GeolocationAccessStatus.Allowed)
        {
            var geolocator = new Geolocator();
            var geoposition = await geolocator.GetGeopositionAsync();

            DisplayMap(geoposition.Coordinate.Longitude, geoposition.Coordinate.Latitude);
        }
    }

    private void DisplayMap(double lon, double lat)
    {
        MyMap.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        MyMap.Visibility = Visibility.Visible;

        var local = SphericalMercator.FromLonLat(lon, lat);
        MyMap.Map.Navigator.CenterOnAndZoomTo(new MPoint(local), MyMap.Map.Navigator.Resolutions[11]);
    }
}
