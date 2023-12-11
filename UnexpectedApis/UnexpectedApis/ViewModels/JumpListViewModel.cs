using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.UI.StartScreen;

namespace UnexpectedApis.ViewModels;

public class JumpListViewModel : ViewModelBase
{
    private JumpList _jumpList;
    private JumpListItem _selectedItem;
    private ObservableCollection<JumpListItem> _items = new ObservableCollection<JumpListItem>();
    private NewJumpListItem _newItem = new NewJumpListItem()
    {
        Arguments = "Activated from JumpList",
        DisplayName = "UnoConf 2020",
        Description = "Your apps everywhere.",
        Logo = new Uri("ms-appx:///UnexpectedApis/Assets/JumpList/xp.png")
    };

    public JumpListViewModel()
    {
        IsSupported = JumpList.IsSupported();
    }

    public Uri[] Icons { get; } = new Uri[]
    {
        new Uri("ms-appx:///UnexpectedApis/Assets/JumpList/xp.png"),
        new Uri("ms-appx:///UnexpectedApis/Assets/JumpList/win10.png"),
        new Uri("ms-appx:///UnexpectedApis/Assets/JumpList/computer.png"),
    };

    public bool IsSupported { get; }

    public bool IsLoaded => _jumpList != null;

    public bool ItemSelected => SelectedItem != null;

    public ObservableCollection<JumpListItem> Items
    {
        get => _items;
        private set
        {
            _items = value;
            OnPropertyChanged();
        }
    }

    public JumpListItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            OnPropertyChanged(nameof(ItemSelected));
        }
    }

    public NewJumpListItem NewItem
    {
        get => _newItem;
        set
        {
            _newItem = value;
            OnPropertyChanged(nameof(NewItem));
        }
    }

    public ICommand AddItemCommand => GetOrCreateCommand(AddItemAsync);

    private async void AddItemAsync()
    {
        var item = JumpListItem.CreateWithArguments(NewItem.Arguments, NewItem.DisplayName);
        item.Description = NewItem.Description;
        item.Logo = NewItem.Logo;
        _jumpList.Items.Add(item);
        NewItem = new NewJumpListItem();
        await _jumpList.SaveAsync();
        RefreshItems();
    }

    public ICommand RemoveItemCommand => GetOrCreateCommand(RemoveItemAsync);

    private async void RemoveItemAsync()
    {
        _jumpList.Items.Remove(SelectedItem);
        SelectedItem = null;
        await _jumpList.SaveAsync();
        RefreshItems();
    }

    public ICommand LoadCurrentCommand => GetOrCreateCommand(LoadCurrentAsync);

    private async void LoadCurrentAsync()
    {
        _jumpList = await JumpList.LoadCurrentAsync();
        RefreshItems();
        OnPropertyChanged(nameof(IsLoaded));
    }

    private void RefreshItems()
    {
        Items = new ObservableCollection<JumpListItem>(_jumpList.Items);
    }

    public class NewJumpListItem : ViewModelBase
    {
        public string Arguments { get; set; } = "";

        public string DisplayName { get; set; } = "";

        public string Description { get; set; } = "";

        public Uri Logo { get; set; }
    }
}
