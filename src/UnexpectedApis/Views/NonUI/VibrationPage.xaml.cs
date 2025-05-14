using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

#if HAS_UNO
using Windows.Phone.Devices.Notification;
#endif

using UnexpectedApis.Attributes;

namespace UnexpectedApis.Views;

[Sample("Vibration", "Vibration.png", SampleKind.NonUI)]
public sealed partial class VibrationPage : Page
{
    public VibrationPage()
    {
        this.InitializeComponent();
    }

    private void Vibrate_Tap(object sender, RoutedEventArgs e)
    {
#if HAS_UNO
        var vibrationDevice = VibrationDevice.GetDefault();
        if (vibrationDevice != null)
        {
            vibrationDevice.Vibrate(TimeSpan.FromMilliseconds(500));
        }
#endif
    }
}
