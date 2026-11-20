namespace UnexpectedApis.Views;

/// <summary>A themed surface that hosts one live instance of a sample.</summary>
public sealed partial class StageView : UserControl
{
    private readonly bool _fills;

    public StageView(double height, bool fills)
    {
        InitializeComponent();
        _fills = fills;
        Surface.MinHeight = height;
        if (fills)
        {
            Host.Height = height;
        }
    }

    public void SetContent(FrameworkElement content)
    {
        ErrorBar.IsOpen = false;
        Host.Children.Clear();

        if (_fills)
        {
            content.HorizontalAlignment = HorizontalAlignment.Stretch;
            content.VerticalAlignment = VerticalAlignment.Stretch;
        }
        else
        {
            content.HorizontalAlignment = HorizontalAlignment.Center;
            content.VerticalAlignment = VerticalAlignment.Center;
        }

        Host.Children.Add(content);
    }

    public void SetContentMaxWidth(double width) => Host.MaxWidth = width;

    /// <summary>Only the corners on the card's outer edge round, so split stages meet cleanly.</summary>
    public void SetOuterCorners(bool left, bool right)
    {
        const double Radius = 8;
        Surface.CornerRadius = new CornerRadius(0, 0, right ? Radius : 0, left ? Radius : 0);
    }

    public void ShowError(string message)
    {
        ErrorBar.Message = message;
        ErrorBar.IsOpen = true;
    }
}
