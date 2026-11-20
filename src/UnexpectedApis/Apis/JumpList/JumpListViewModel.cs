// Guarded at runtime: these members exist only on some heads.
#pragma warning disable Uno0001

using System.Collections.ObjectModel;
using Windows.UI.StartScreen;

namespace UnexpectedApis.Apis;

public sealed partial class JumpListViewModel : ObservableObject
{
    private JumpList? _jumpList;

    public Uri[] Icons { get; } =
    [
        new("ms-appx:///Assets/Apis/JumpList/xp.png"),
        new("ms-appx:///Assets/Apis/JumpList/win10.png"),
        new("ms-appx:///Assets/Apis/JumpList/computer.png"),
    ];

    public bool IsSupported { get; } = CheckSupported();

    public ObservableCollection<JumpListItem> Items { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoaded))]
    public partial bool Loaded { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveItemCommand))]
    public partial object? SelectedItem { get; set; }

    [ObservableProperty]
    public partial string Arguments { get; set; } = "Activated from JumpList";

    [ObservableProperty]
    public partial string DisplayName { get; set; } = "UnoConf 2020";

    [ObservableProperty]
    public partial string Description { get; set; } = "Your apps everywhere.";

    [ObservableProperty]
    public partial Uri Logo { get; set; } = new("ms-appx:///Assets/Apis/JumpList/xp.png");

    [ObservableProperty]
    public partial string Status { get; set; } = string.Empty;

    public bool IsLoaded => Loaded && IsSupported;

    public bool IsUnsupported => !IsSupported;

    [RelayCommand(CanExecute = nameof(IsSupported))]
    private async Task LoadCurrentAsync()
    {
        try
        {
            _jumpList = await JumpList.LoadCurrentAsync();
            Loaded = true;
            Refresh();
            Status = $"Loaded {Items.Count} item(s).";
        }
        catch (Exception ex)
        {
            Status = $"Couldn't load the jump list: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task AddItemAsync()
    {
        if (_jumpList is null)
        {
            return;
        }

        try
        {
            var item = JumpListItem.CreateWithArguments(Arguments, DisplayName);
            item.Description = Description;
            item.Logo = Logo;
            _jumpList.Items.Add(item);
            await _jumpList.SaveAsync();
            Refresh();
            Status = $"Added \"{DisplayName}\".";
        }
        catch (Exception ex)
        {
            Status = $"Couldn't add the item: {ex.Message}";
        }
    }

    [RelayCommand(CanExecute = nameof(CanRemove))]
    private async Task RemoveItemAsync()
    {
        if (_jumpList is null || SelectedItem is null)
        {
            return;
        }

        try
        {
            _jumpList.Items.Remove((JumpListItem)SelectedItem);
            await _jumpList.SaveAsync();
            Refresh();
            Status = "Removed the item.";
        }
        catch (Exception ex)
        {
            Status = $"Couldn't remove the item: {ex.Message}";
        }
    }

    private bool CanRemove() => SelectedItem is not null;

    private void Refresh()
    {
        SelectedItem = null;
        Items.Clear();
        if (_jumpList is not null)
        {
            foreach (var item in _jumpList.Items)
            {
                Items.Add(item);
            }
        }
    }

    private static bool CheckSupported()
    {
        try
        {
            return JumpList.IsSupported();
        }
        catch
        {
            return false;
        }
    }
}
