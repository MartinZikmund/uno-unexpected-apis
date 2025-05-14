using UnexpectedApis.Attributes;

namespace UnexpectedApis.Views;

[Sample("WebView", "DataGrid.png", SampleKind.UI)]
public sealed partial class WebViewPage : SamplePage
{
    public WebViewPage()
    {
        this.InitializeComponent();            
        //Model = new DataGridViewModel(DispatcherQueue);
        //DataContext = Model;
    }

    public string Code =>
"""

""";

    //public DataGridViewModel Model { get; }
}
