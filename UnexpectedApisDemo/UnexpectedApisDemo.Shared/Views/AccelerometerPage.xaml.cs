using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnexpectedApisDemo.Shared.ViewModels;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class AccelerometerPage : Page
    {
        public AccelerometerPage()
        {
            this.InitializeComponent();            
            Model = new AccelerometerViewModel(Dispatcher);
            DataContext = Model;
            this.Unloaded += AccelerometerPage_Unloaded;
        }

        public AccelerometerViewModel Model { get; }

        private void AccelerometerPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Model.Dispose();
        }
    }
}
