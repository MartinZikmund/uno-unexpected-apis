using Microsoft.UI.Xaml.Markup;

namespace UnexpectedApis.UserControls;

[ContentProperty(Name = nameof(Content))]
public sealed partial class CodeSampleControl : UserControl
{
    public CodeSampleControl()
    {
        this.InitializeComponent();
    }

    public UIElement Content
    {
        get { return (UIElement)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    public static DependencyProperty ContentProperty { get; } =
        DependencyProperty.Register("Content", typeof(UIElement), typeof(CodeSampleControl), new PropertyMetadata(null));


}
