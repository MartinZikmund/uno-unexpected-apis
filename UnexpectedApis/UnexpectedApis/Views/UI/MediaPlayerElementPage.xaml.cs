using UnexpectedApis.ViewModels;

namespace UnexpectedApis.Views;

public sealed partial class MediaPlayerElementPage : SamplePage
{
    public MediaPlayerElementPage()
    {
        this.InitializeComponent();
    }

    public string Code =>
"""
<MediaPlayerElement
    Source="ms-appx:///Assets/Media/sample.mp4"
    AreTransportControlsEnabled="True" />
""";
}
