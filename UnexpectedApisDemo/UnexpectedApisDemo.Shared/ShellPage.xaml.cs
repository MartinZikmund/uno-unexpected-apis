using System;
using System.Linq;
using UnexpectedApisDemo.Shared.Views;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItemInvokedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;

namespace UnexpectedApisDemo.Shared
{
    public sealed partial class ShellPage : Page
    {
        public ShellPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Menu.SelectedItem = Menu.MenuItems[0];
            RootFrame.Navigate(typeof(HelloPage));
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
}
