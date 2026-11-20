namespace UnexpectedApis.Apis;

public sealed partial class JumpListDemo : UserControl
{
    public JumpListDemo() => InitializeComponent();

    public JumpListViewModel Model { get; } = new();
}
