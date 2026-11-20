using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.UI.Controls;

namespace UnexpectedApis.Apis;

public sealed partial class DataGridDemo : UserControl
{
    private static readonly Dictionary<string, Func<Plant, object>> _sorters = new()
    {
        ["PlantName"] = p => p.PlantName,
        ["PlantsCount"] = p => p.PlantsCount,
        ["Kind"] = p => p.Kind,
        ["PlantDate"] = p => p.PlantDate,
        ["IsWatered"] = p => p.IsWatered,
    };

    public DataGridDemo()
    {
        InitializeComponent();
        PlantGrid.PreparingCellForEdit += OnPreparingCellForEdit;
        Unloaded += (_, _) => PlantGrid.PreparingCellForEdit -= OnPreparingCellForEdit;
    }

    public ObservableCollection<Plant> Plants { get; private set; } = new(Plant.CreateAll());

    private void OnSorting(object? sender, DataGridColumnEventArgs e)
    {
        if (e.Column.Tag is not string tag || !_sorters.TryGetValue(tag, out var selector))
        {
            return;
        }

        var ascending = e.Column.SortDirection is null or DataGridSortDirection.Descending;
        Plants = new(ascending ? Plants.OrderBy(selector) : Plants.OrderByDescending(selector));
        PlantGrid.ItemsSource = Plants;
        e.Column.SortDirection = ascending ? DataGridSortDirection.Ascending : DataGridSortDirection.Descending;

        foreach (var other in PlantGrid.Columns.Where(c => !ReferenceEquals(c, e.Column)))
        {
            other.SortDirection = null;
        }
    }

    private void OnReadOnlyToggled(object sender, RoutedEventArgs e) =>
        PlantGrid.IsReadOnly = ((ToggleSwitch)sender).IsOn;

    private void OnGridLinesToggled(object sender, RoutedEventArgs e) =>
        PlantGrid.GridLinesVisibility = ((ToggleSwitch)sender).IsOn ? DataGridGridLinesVisibility.All : DataGridGridLinesVisibility.None;

    private void OnPreparingCellForEdit(object? sender, DataGridPreparingCellForEditEventArgs e)
    {
        switch (e.EditingElement)
        {
            case CalendarDatePicker calendar:
                calendar.IsCalendarOpen = true;
                break;
            case ComboBox combo:
                combo.IsDropDownOpen = true;
                break;
        }
    }
}
