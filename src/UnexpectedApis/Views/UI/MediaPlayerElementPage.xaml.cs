using UnexpectedApis.ViewModels;
using UnexpectedApis.Attributes;

namespace UnexpectedApis.Views;

[Sample("MediaPlayerElement", "MediaPlayerElement.png", SampleKind.UI)]
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
