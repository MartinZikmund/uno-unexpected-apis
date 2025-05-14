using CommunityToolkit.WinUI.UI.Controls;
using UnexpectedApis.Attributes;
using UnexpectedApis.Model;
using UnexpectedApis.ViewModels;

namespace UnexpectedApis.Views;

[Sample("AcrylicBrush", "AcrylicBrush.png", SampleKind.UI, TargetPlatforms.Skia)]
public sealed partial class AcrylicBrushPage : SamplePage
{
    public AcrylicBrushPage()
    {
        this.InitializeComponent();
    }

    public string Code =>
"""
<AcrylicBrush TintOpacity="0.8" TintLuminosityOpacity="0.3" TintColor="Red" />
""";

    private void TintSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        acrylicBrush.TintOpacity = e.NewValue;
    }

    private void TintLuminositySlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        acrylicBrush.TintLuminosityOpacity = e.NewValue;
    }

    private async void PickColorClick(object sender, RoutedEventArgs e)
    {
        ColorPicker cp = new ColorPicker();
        cp.XamlRoot = this.XamlRoot;
        cp.Color = acrylicBrush.TintColor;
        var dialog = new ContentDialog()
        {
            Title = "Pick a color",
            Content = cp,
            XamlRoot = this.XamlRoot,
            PrimaryButtonText = "OK",
            CloseButtonText = "Cancel"
        };
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            acrylicBrush.TintColor = cp.Color;
        }
    }
}
