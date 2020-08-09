using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.Devices.Notification;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UnexpectedApisDemo.Shared.Views
{
    public sealed partial class VibrationPage : Page
    {
        public VibrationPage()
        {
            this.InitializeComponent();
        }

        private void Vibrate_Tap(object sender, TappedRoutedEventArgs e)
        {
            var vibrationDevice = VibrationDevice.GetDefault();
            if (vibrationDevice != null)
            {
                vibrationDevice.Vibrate(TimeSpan.FromMilliseconds(200));
            }
        }
    }
}
