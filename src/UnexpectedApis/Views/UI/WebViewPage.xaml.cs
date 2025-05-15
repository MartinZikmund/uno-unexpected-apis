using UnexpectedApis.Attributes;

namespace UnexpectedApis.Views;

[Sample("WebView", "WebView.png", SampleKind.UI)]
public sealed partial class WebViewPage : SamplePage
{
    public WebViewPage()
    {
        this.InitializeComponent();
    }

    public string Code =>
"""
<controls:WebView2 Source="https://unexpectedapis.uno/" />
""";
}
