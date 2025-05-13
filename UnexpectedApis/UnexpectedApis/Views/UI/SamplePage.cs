namespace UIApis.Views;

public partial class SamplePage : Page
{
    public Uri IconUri => new Uri($"ms-appx:///Assets/Samples/{this.GetType().Name.Substring(0, this.GetType().Name.Length - "Page".Length)}.png");
}
