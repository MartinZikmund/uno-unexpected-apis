using Microsoft.UI.Xaml.Markup;
using Windows.ApplicationModel.DataTransfer;

namespace UnexpectedApis.UserControls;

[ContentProperty(Name = nameof(Content))]
public sealed partial class CodeSampleControl : UserControl
{
    public CodeSampleControl()
    {
        this.InitializeComponent();
    }

    public string Code
    {
        get => (string)GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    public static DependencyProperty CodeProperty { get; } =
        DependencyProperty.Register(nameof(Code), typeof(string), typeof(CodeSampleControl), new PropertyMetadata(""));

    private void Copy_Click(object sender, RoutedEventArgs e)
    {
        var package = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
        package.SetText(Code);
        Clipboard.SetContent(package);
    }
}
