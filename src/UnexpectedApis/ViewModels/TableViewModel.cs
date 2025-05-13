using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Data;
using UIApis.Model;
using Uno.Extensions;


namespace UIApis.ViewModels;

[Bindable]
public class TableViewModel : ViewModelBase
{
    private DispatcherQueue _dispatcherQueue;

    public int ColumnHeaderHeight { get => GetProperty<int>(); set => SetProperty(value); }
    public int MaxColumnWidth { get => GetProperty<int>(); set => SetProperty(value); }
    public int MinColumnWidth { get => GetProperty<int>(); set => SetProperty(value); }
    public bool AreRowDetailsFrozen { get => GetProperty<bool>(); set => SetProperty(value); }
    public bool AreRowGroupHeadersFrozen { get => GetProperty<bool>(); set => SetProperty(value); }
    public bool CanUserSortColumns { get => GetProperty<bool>(); set => SetProperty(value); }
    public bool CanUserReorderColumns { get => GetProperty<bool>(); set => SetProperty(value); }
    public bool CanUserResizeColumns { get => GetProperty<bool>(); set => SetProperty(value); }
    public bool IsReadOnly { get => GetProperty<bool>(); set => SetProperty(value); }
    public SolidColorBrush AlternatingRowForeground { get => GetProperty<SolidColorBrush>(); set => SetProperty(value); }
    public SolidColorBrush AlternatingRowBackground { get => GetProperty<SolidColorBrush>(); set => SetProperty(value); }
    public DataGridGridLinesVisibility GridLinesVisibility { get => GetProperty<DataGridGridLinesVisibility>(); set => SetProperty(value); }
    public DataGridHeadersVisibility HeadersVisibility { get => GetProperty<DataGridHeadersVisibility>(); set => SetProperty(value); }
    public DataGridSelectionMode SelectionMode { get => GetProperty<DataGridSelectionMode>(); set => SetProperty(value); }
    public List<SolidColorBrush> AvailableColors { get => GetProperty<List<SolidColorBrush>>(); set => SetProperty(value); }
    public List<DataGridGridLinesVisibility> GridLinesVisibilityOptions { get => GetProperty<List<DataGridGridLinesVisibility>>(); set => SetProperty(value); }
    public List<DataGridHeadersVisibility> HeadersVisibilityOptions { get => GetProperty<List<DataGridHeadersVisibility>>(); set => SetProperty(value); }
    public List<DataGridSelectionMode> SelectionModeOptions { get => GetProperty<List<DataGridSelectionMode>>(); set => SetProperty(value); }

    public string RowGroupHeader { get => GetProperty<string>(); set => SetProperty(value); }

    public IEnumerable DataGridItems { get => GetProperty<IEnumerable>(); set => SetProperty(value); }
    public Plant.FruitOrVegetableEnum[] FruitOrVegetableEnumValues;

    // This currently causes a bug on WASM, removed until fixed https://github.com/unoplatform/uno/issues/11184
    //public DataGridRowDetailsVisibilityMode RowDetailsVisibilityMode { get => GetProperty<DataGridRowDetailsVisibilityMode>(); set => SetProperty(value); }
    //public List<DataGridRowDetailsVisibilityMode> RowDetailsVisibilityModeOptions { get => GetProperty<List<DataGridRowDetailsVisibilityMode>>(); set => SetProperty(value); }

    public TableViewModel(DispatcherQueue dispatcherQueue)
    {
        _dispatcherQueue = dispatcherQueue;

        ColumnHeaderHeight = 32;
        MaxColumnWidth = 400;
        MinColumnWidth = 50;
        AreRowDetailsFrozen = false;
        AreRowGroupHeadersFrozen = true;
        CanUserSortColumns = true;
        CanUserReorderColumns = true;
        CanUserResizeColumns = true;
        IsReadOnly = false;
        AlternatingRowForeground = new SolidColorBrush(Colors.Gray);
        AlternatingRowBackground = new SolidColorBrush(Colors.Transparent);
        GridLinesVisibility = DataGridGridLinesVisibility.None;
        HeadersVisibility = DataGridHeadersVisibility.Column;
        SelectionMode = DataGridSelectionMode.Extended;

        AvailableColors = new List<SolidColorBrush>(typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Select(c => new SolidColorBrush((Windows.UI.Color)c.GetValue(null))));

        GridLinesVisibilityOptions = Enum.GetValues(typeof(DataGridGridLinesVisibility)).Cast<DataGridGridLinesVisibility>().ToList();
        HeadersVisibilityOptions = Enum.GetValues(typeof(DataGridHeadersVisibility)).Cast<DataGridHeadersVisibility>().ToList();
        SelectionModeOptions = Enum.GetValues(typeof(DataGridSelectionMode)).Cast<DataGridSelectionMode>().ToList();

        DataGridItems = new PlantCollection().ToObservableCollection();

        FruitOrVegetableEnumValues = (Plant.FruitOrVegetableEnum[])Enum.GetValues(typeof(Plant.FruitOrVegetableEnum));
    }

    public void SortDataGrid(Func<Plant, object> selector, DataGridColumn column)
    {
        if (column.SortDirection == null || column.SortDirection == DataGridSortDirection.Descending)
        {
            DataGridItems = new ObservableCollection<Plant>((DataGridItems as IEnumerable<Plant>).OrderBy(selector));
            column.SortDirection = DataGridSortDirection.Ascending;
        }
        else
        {
            DataGridItems = new ObservableCollection<Plant>((DataGridItems as IEnumerable<Plant>).OrderByDescending(selector));
            column.SortDirection = DataGridSortDirection.Descending;
        }
    }

    public void FilterDataGrid(Func<Plant, bool> selector)
    {
        DataGridItems = new PlantCollection().ToObservableCollection();

        if (selector != null)
        {
            DataGridItems = new ObservableCollection<Plant>((DataGridItems as IEnumerable<Plant>).Where(selector));
        }
    }

    public void GroupByDataGrid(Func<Plant, object> selector)
    {
        if (DataGridItems == null || selector == null)
        {
            DataGridItems = new PlantCollection();
            return;
        }

        ObservableCollection<GroupInfoCollection<Plant>> groups = new ObservableCollection<GroupInfoCollection<Plant>>();
        var items = new List<Plant>(new PlantCollection());

        var query = items.OrderBy(item => item.PlantName)
                                  .GroupBy(selector, item => item)
                                  .Select(g => new { GroupName = g.Key, Items = g });

        foreach (var g in query)
        {
            GroupInfoCollection<Plant> info = new GroupInfoCollection<Plant>()
            {
                Key = g.GroupName
            };

            foreach (var item in g.Items)
            {
                info.Add(item);
            }

            groups.Add(info);
        }

        var groupedItems = new CollectionViewSource
        {
            IsSourceGrouped = true,
            Source = groups
        };

        DataGridItems = groupedItems.View;
    }

    public class GroupInfoCollection<T> : ObservableCollection<T>
    {
        public object Key { get; set; }

        public new IEnumerator<T> GetEnumerator()
        {
            return base.GetEnumerator();
        }
    }
}
