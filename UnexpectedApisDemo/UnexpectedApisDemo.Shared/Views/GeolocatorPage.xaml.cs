using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UnexpectedApisDemo.Shared.Views
{
    public sealed partial class GeolocatorPage : Page
    {
        public GeolocatorPage()
        {
            this.InitializeComponent();
        }

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
}