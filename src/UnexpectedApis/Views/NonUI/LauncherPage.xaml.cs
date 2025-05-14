using UnexpectedApis.ViewModels;
using UnexpectedApis.Attributes;
using Windows.System;

namespace UnexpectedApis.Views;

[Sample("Launcher", "Launcher.png", SampleKind.NonUI)]
public sealed partial class LauncherPage : SamplePage
{
    public LauncherPage()
    {
        this.InitializeComponent();
    }

    public string Code =>
"""
await Launcher.LaunchUriAsync(uri);
""";

    private async void LaunchWebsiteClick(object sender, RoutedEventArgs e)
    {
        var uriString = UriTextBox.Text;
        if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
        {
            await Launcher.LaunchUriAsync(uri);
        }
        else
        {
            // Handle invalid URI
            var dialog = new ContentDialog
            {
                Title = "Invalid URI",
                Content = "Please enter a valid URI.",
                CloseButtonText = "Ok",
                XamlRoot = XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}
