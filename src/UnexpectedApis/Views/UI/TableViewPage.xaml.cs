using CommunityToolkit.WinUI.UI.Controls;
using UnexpectedApis.Attributes;
using UnexpectedApis.Model;
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
xmlns:wct="using:CommunityToolkit.WinUI.UI.Controls"

<wct:DataGrid ItemsSource="{x:Bind Model.DataGridItems, Mode=TwoWay}"
              AutoGenerateColumns="True"
              HeadersVisibility="All"
              SelectionMode="Single"
              IsReadOnly="False" />
""";

    public TableViewModel Model { get; }

    //private void FilterFlyoutItem_Click(object sender, RoutedEventArgs e)
    //{
    //    if (Model is not DataGridViewModel viewModel)
    //        return;

    //    switch ((sender as Control).Tag)
    //    {
    //        case "TenMore":
    //            viewModel.FilterDataGrid(p => p.PlantsCount > 10);
    //            break;
    //        case "TenLess":
    //            viewModel.FilterDataGrid(p => p.PlantsCount < 10);
    //            break;
    //        case "IsWatered":
    //            viewModel.FilterDataGrid(p => p.IsWatered);
    //            break;
    //        case "Fruit":
    //            viewModel.FilterDataGrid(p => p.FruitOrVegetable == Plant.FruitOrVegetableEnum.Fruit);
    //            break;
    //        case "Vegetable":
    //            viewModel.FilterDataGrid(p => p.FruitOrVegetable == Plant.FruitOrVegetableEnum.Vegetable);
    //            break;
    //        default:
    //            viewModel.FilterDataGrid(null);
    //            break;
    //    }
    //}

    //private void GroupByFlyoutItem_Click(object sender, RoutedEventArgs e)
    //{
    //    if (Model is not DataGridViewModel viewModel)
    //        return;

    //    var tag = (sender as Control).Tag;

    //    switch (tag)
    //    {
    //        case "FruitOrVegetable":
    //            viewModel.RowGroupHeader = tag.ToString();

    //            viewModel.GroupByDataGrid(p => p.FruitOrVegetable);
    //            break;
    //        case "IsWatered":
    //            viewModel.RowGroupHeader = tag.ToString();

    //            viewModel.GroupByDataGrid(p => p.IsWatered);
    //            break;
    //        default:
    //            viewModel.GroupByDataGrid(null);
    //            break;
    //    }
    //}

}
