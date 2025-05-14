using UnexpectedApis.Attributes;
using UnexpectedApis.ViewModels;

namespace UnexpectedApis.Views;

[Sample("TableView", "TableView.png", SampleKind.UI, TargetPlatforms.All & (~TargetPlatforms.Android & ~TargetPlatforms.IOS))]
public sealed partial class TableViewPage : SamplePage
{
    public TableViewPage()
    {
        this.InitializeComponent();            
        Model = new TableViewModel(DispatcherQueue);
        DataContext = Model;
    }

    public string Code =>
"""
xmlns:tv="using:WinUI.TableView"

<tv:TableView ItemsSource="{x:Bind Model.Plants}"
              AutoGenerateColumns="True"
              SelectionMode="Extended"
              ShowExportOptions="True"
              GridLinesVisibility="All"/>
""";

    public TableViewModel Model { get; }
}
