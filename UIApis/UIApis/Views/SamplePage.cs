namespace UIApis.Views;

public partial class SamplePage : Page
{
    public Uri IconUri => new Uri($"ms-appx:///UIApis/Assets/{this.GetType().Name.Substring(0, this.GetType().Name.Length - "Page".Length)}.png");
}
