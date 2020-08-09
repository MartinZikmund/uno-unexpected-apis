using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Display;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DisplayRequestPage : Page
    {
        private DisplayRequest _displayRequest = new DisplayRequest();

        public DisplayRequestPage()
        {
            this.InitializeComponent();
        }

        private void DisplayRequest_Toggled(object sender, RoutedEventArgs args)
        {
            if (DisplayRequestToggle.IsChecked == true)
            {
                _displayRequest.RequestActive();
            }
            else
            {
                _displayRequest.RequestRelease();
            }
        }
    }
}
