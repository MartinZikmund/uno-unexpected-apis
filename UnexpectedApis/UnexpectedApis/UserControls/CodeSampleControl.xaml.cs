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

    public string Content
    {
        get { return (string)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    public static DependencyProperty ContentProperty { get; } =
        DependencyProperty.Register("Content", typeof(string), typeof(CodeSampleControl), new PropertyMetadata(null));

    private void Copy_Click(object sender, RoutedEventArgs e)
    {
        var package = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
        package.SetText(Content);
        Clipboard.SetContent(package);
    }
}
