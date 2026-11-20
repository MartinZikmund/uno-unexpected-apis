using Microsoft.UI.Xaml.Controls.Primitives;

namespace UnexpectedApis.Apis;

public sealed partial class AcrylicBrushDemo : UserControl
{
    public AcrylicBrushDemo() => InitializeComponent();

    private void OnTintOpacityChanged(object sender, RangeBaseValueChangedEventArgs e) =>
        Acrylic.TintOpacity = e.NewValue;

    private void OnLuminosityChanged(object sender, RangeBaseValueChangedEventArgs e) =>
        Acrylic.TintLuminosityOpacity = e.NewValue;

    private void OnTintColorChanged(ColorPicker sender, ColorChangedEventArgs args) =>
        Acrylic.TintColor = args.NewColor;
}
