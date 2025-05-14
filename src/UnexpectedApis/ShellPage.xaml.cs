using System;
using System.Linq;
using UnexpectedApis.Views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItemInvokedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using System.Reflection;
using UnexpectedApis.Attributes;

namespace UnexpectedApis;

public sealed partial class ShellPage : Page
{
    public ShellPage()
    {
        this.InitializeComponent();
        FindSamples();
    }

    private void FindSamples()
    {
        var samples = this.GetType().Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(SamplePage)))
            .Where(t => t.GetCustomAttribute<SampleAttribute>() is not null)
            .OrderBy(t => t.GetCustomAttribute<SampleAttribute>()!.DisplayName)
            .ToArray();

        foreach (var sample in samples)
        {
            var sampleAttribute = sample.GetCustomAttribute<SampleAttribute>()!;

            if (sampleAttribute.TargetPlatforms.HasFlag(TargetPlatformsHelper.CurrentPlatform))
            {
                var item = new NavigationViewItem
                {
                    Content = sampleAttribute.DisplayName,
                    Tag = sample.Name,
                    Icon = new BitmapIcon() { ShowAsMonochrome = false, UriSource = sampleAttribute.IconUri },
                };

                if (sampleAttribute.Kind == SampleKind.UI)
                {
                    UIApisItem.MenuItems.Add(item);
                }
                else if (sampleAttribute.Kind == SampleKind.NonUI)
                {
                    NonUIApisItem.MenuItems.Add(item);
                }
                else
                {
                    throw new NotSupportedException($"Sample kind {sampleAttribute.Kind} is not supported.");
                }
            }
        }
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
        var pageName = $"{item?.Tag}";
        if (!pageName.EndsWith("Page"))
        {
            pageName += "Page";
        }
        var pageType = GetType().Assembly.GetTypes().FirstOrDefault(t => t.Name == pageName);
        if (pageType != null)
        {
            RootFrame.Navigate(pageType);
        }
    }
}
