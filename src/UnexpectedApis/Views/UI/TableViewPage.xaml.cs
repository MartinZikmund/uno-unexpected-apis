using CommunityToolkit.WinUI.UI.Controls;
using UnexpectedApis.Model;
using UnexpectedApis.ViewModels;

namespace UnexpectedApis.Views;

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

    private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        var dg = sender as DataGrid;

        if (Model is not DataGridViewModel viewModel)
            return;

        switch (e.Column.Tag)
        {
            case "PlantName":
                viewModel.SortDataGrid(p => p.PlantName, e.Column);
                break;
            case "PlantsCount":
                viewModel.SortDataGrid(p => p.PlantsCount, e.Column);
                break;
            case "FruitOrVegetable":
                viewModel.SortDataGrid(p => p.FruitOrVegetable, e.Column);
                break;
            case "PlantDate":
                viewModel.SortDataGrid(p => p.PlantDate, e.Column);
                break;
            case "IsWatered":
                viewModel.SortDataGrid(p => p.IsWatered, e.Column);
                break;
        }

        foreach (var dgColumn in dg.Columns)
        {
            if (!dgColumn.Tag.Equals(e.Column.Tag))
            {
                dgColumn.SortDirection = null;
            }
        }
    }

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

    private void DataGrid_LoadingRowGroup(object sender, DataGridRowGroupHeaderEventArgs e)
    {
        if (Model is not DataGridViewModel viewModel)
            return;

        var group = e.RowGroupHeader.CollectionViewGroup;
        var item = group.GroupItems[0] as Plant;

        (sender as DataGrid).RowGroupHeaderPropertyNameAlternative = viewModel.RowGroupHeader;
        if (viewModel.RowGroupHeader == "IsWatered")
        {
            e.RowGroupHeader.PropertyValue = (item?.IsWatered ?? false) ? "Yes" : "No";
        }
        else
        {
            e.RowGroupHeader.PropertyValue = (item?.FruitOrVegetable == Plant.FruitOrVegetableEnum.Fruit) ? "Fruit" : "Vegetable";
        }
    }

    private void DataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
    {
        if (e.EditingElement is CalendarDatePicker calendar)
        {
            calendar.IsCalendarOpen = true;
        }
        else
        if (e.EditingElement is ComboBox comboBox)
        {
            comboBox.IsDropDownOpen = true;
        }
    }
}
