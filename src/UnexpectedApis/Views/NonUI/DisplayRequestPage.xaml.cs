using Windows.System.Display;
using UnexpectedApis.Attributes;

namespace UnexpectedApis.Views;

[Sample("DisplayRequest", "DisplayRequest.png", SampleKind.NonUI)]
public sealed partial class DisplayRequestPage : SamplePage
{
    private DisplayRequest _displayRequest = new();

    public DisplayRequestPage()
    {
        this.InitializeComponent();
    }

    public string Code =>
"""
var displayRequest = new DisplayRequest();
displayRequest.RequestActive();
...
displayRequest.RequestRelease();
""";

    private void DisplayRequest_Toggled(object sender, RoutedEventArgs args)
    {
        if (DisplayRequestToggle.IsOn == true)
        {
            _displayRequest.RequestActive();
        }
        else
        {
            _displayRequest.RequestRelease();
        }
    }
}
