using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnexpectedApisDemo.Shared.ViewModels;
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
    public sealed partial class SpeechRecognizerPage : Page
    {
        public SpeechRecognizerPage()
        {
            this.InitializeComponent(); Model = new SpeechRecognizerViewModel(Dispatcher);
            DataContext = Model;
            this.Unloaded += SpeechRecognizerPage_Unloaded;
        }

        public SpeechRecognizerViewModel Model { get; }

        private void SpeechRecognizerPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Model.Dispose();
        }
    }
}
