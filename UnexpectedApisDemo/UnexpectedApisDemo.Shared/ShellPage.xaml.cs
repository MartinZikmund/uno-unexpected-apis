using System;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace UnexpectedApisDemo.Shared
{
    public sealed partial class ShellPage : Page
    {
        public ShellPage()
        {
            this.InitializeComponent();
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
