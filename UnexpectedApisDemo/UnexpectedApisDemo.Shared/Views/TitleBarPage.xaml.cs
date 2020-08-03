using System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UnexpectedApisDemo.Shared.Views
{
    public sealed partial class TitleBarPage : Page
    {
        private Random _random = new Random();

        public TitleBarPage()
        {
            this.InitializeComponent();
        }

        private void SetTitle_Click(object sender, RoutedEventArgs e)
        {
            var titleText = TitleTextBox.Text;
            ApplicationView.GetForCurrentView().Title = titleText;
        }

        private void SetColor_Click(object sender, RoutedEventArgs e)
        {
            var r = (byte)_random.Next(0, 256);
            var g = (byte)_random.Next(0, 256);
            var b = (byte)_random.Next(0, 256);
            var color = Color.FromArgb(255, r, g, b);
            ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = color;
        }
    }
}
