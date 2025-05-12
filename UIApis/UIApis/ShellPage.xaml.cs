namespace UIApis;

public partial class ShellPage : Page
{
    public ShellPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        Menu.SelectedItem = Menu.MenuItems[0];
        RootFrame.Navigate(typeof(MainPage));
    }

    private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        var item = args.InvokedItemContainer as NavigationViewItem;
        var pageName = $"{item?.Tag}Page";
        var pageType = GetType().Assembly.GetTypes().FirstOrDefault(t => t.Name == pageName);
        if (pageType != null)
        {
            RootFrame.Navigate(pageType);
        }
    }
}
