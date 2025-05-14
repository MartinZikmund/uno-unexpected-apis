using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using UnexpectedApis.Attributes;

namespace UnexpectedApis.Views;

[Sample("Geolocator", "Geolocator.png", SampleKind.NonUI, TargetPlatforms.All & ~TargetPlatforms.SkiaDesktop)]
public sealed partial class GeolocatorPage : SamplePage
{
    public GeolocatorPage()
    {
        this.InitializeComponent();
    }

    public string Code =>
"""
if (await Geolocator.RequestAccessAsync() == GeolocationAccessStatus.Allowed)
{
    var geolocator = new Geolocator();
    var geoposition = await geolocator.GetGeopositionAsync();
}
""";

    private async void GetGeoposition_Click(object sender, RoutedEventArgs args)
    {
        ProgressRing.IsActive = true;
        ProgressRing.Visibility = Visibility.Visible;

        if (await Geolocator.RequestAccessAsync() == GeolocationAccessStatus.Allowed)
        {
            var geolocator = new Geolocator();
            var geoposition = await geolocator.GetGeopositionAsync();

            GeopositionDisplay.Visibility = Visibility.Visible;
            GeopositionDisplay.Geoposition = geoposition;
        }

        ProgressRing.IsActive = false;
        ProgressRing.Visibility = Visibility.Collapsed;
    }
}
