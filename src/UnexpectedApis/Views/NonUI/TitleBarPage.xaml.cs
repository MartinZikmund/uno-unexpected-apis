using System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnexpectedApis.Attributes;

namespace UnexpectedApis.Views;

[Sample("TitleBar", "TitleBar.png", SampleKind.NonUI)]
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
        ((UnexpectedApis.App)Application.Current).MainWindow!.Title = titleText;
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
