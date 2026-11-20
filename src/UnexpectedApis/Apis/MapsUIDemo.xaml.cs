// Guarded at runtime: these members exist only on some heads.
#pragma warning disable Uno0001

#if WINDOWS
using Mapsui;
using Mapsui.Projections;
using Mapsui.Tiling;
using MapControl = Mapsui.UI.WinUI.MapControl;
using Windows.Devices.Geolocation;
#endif

namespace UnexpectedApis.Apis;

public sealed partial class MapsUIDemo : UserControl
{
#if WINDOWS
    private MapControl? _map;
#endif

    public MapsUIDemo()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
#if WINDOWS
        if (_map is not null)
        {
            return;
        }

        try
        {
            _map = new MapControl();
            _map.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
            MapHost.Children.Add(_map);
            CenterOn(14.3893686, 50.0910957, 11);
            LocateButton.Visibility = Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Devices.Geolocation.Geolocator")
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        catch (Exception ex)
        {
            Show($"The map couldn't start: {ex.Message}");
        }
#else
        Show("Mapsui.Uno.WinUI is built for Uno Platform 6 and fails to load on Uno 7.1, so the map is only available on the Windows head for now.");
#endif
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
#if WINDOWS
        if (_map is null)
        {
            return;
        }

        try
        {
            MapHost.Children.Remove(_map);
            _map.Dispose();
        }
        catch
        {
            // The map is going away anyway.
        }

        _map = null;
#endif
    }

    private async void OnLocateClick(object sender, RoutedEventArgs e)
    {
#if WINDOWS
        try
        {
            if (await Geolocator.RequestAccessAsync() != GeolocationAccessStatus.Allowed)
            {
                Show("Location access wasn't granted.");
                return;
            }

            var position = await new Geolocator().GetGeopositionAsync();
            MessageText.Visibility = Visibility.Collapsed;
            CenterOn(position.Coordinate.Longitude, position.Coordinate.Latitude, 18);
        }
        catch (Exception ex)
        {
            Show($"Couldn't get your location: {ex.Message}");
        }
#else
        await Task.CompletedTask;
#endif
    }

#if WINDOWS
    private void CenterOn(double longitude, double latitude, int zoom)
    {
        if (_map is null)
        {
            return;
        }

        var center = SphericalMercator.FromLonLat(longitude, latitude);
        _map.Map.Navigator.CenterOnAndZoomTo(new MPoint(center), _map.Map.Navigator.Resolutions[zoom]);
    }
#endif

    private void Show(string message)
    {
        MessageText.Text = message;
        MessageText.Visibility = Visibility.Visible;
    }
}
