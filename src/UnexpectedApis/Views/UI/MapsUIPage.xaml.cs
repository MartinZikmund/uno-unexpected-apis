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

        if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Devices.Geolocation.Geolocator"))
        {
            GeolocatorButton.Visibility = Visibility.Visible;
        }
        else
        {
            GeolocatorButton.Visibility = Visibility.Collapsed;
            DisplayMap(14.3893686, 50.0910957, 11);
        }
    }

    public string Code =>
"""
var mapControl = new Mapsui.UI.WinUI.MapControl();
mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
""";

    private async void GetGeoposition_Click(object sender, RoutedEventArgs args)
    {
        if (await Geolocator.RequestAccessAsync() == GeolocationAccessStatus.Allowed)
        {
            var geolocator = new Geolocator();
            var geoposition = await geolocator.GetGeopositionAsync();

            DisplayMap(geoposition.Coordinate.Longitude, geoposition.Coordinate.Latitude, 18);
        }
    }

    private void DisplayMap(double lon, double lat, int zoom)
    {
        var mapControl = new Mapsui.UI.WinUI.MapControl();
        mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        MapControlContainer.Visibility = Visibility.Visible;
        MapControlContainer.Children.Add(mapControl);

        var local = SphericalMercator.FromLonLat(lon, lat);
        mapControl.Map.Navigator.CenterOnAndZoomTo(new MPoint(local), mapControl.Map.Navigator.Resolutions[zoom]);
    }
}
