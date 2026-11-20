// Only implemented on Apple heads; calls are guarded and the demo says so.
#pragma warning disable Uno0001
#pragma warning disable CS1998

using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
#if __IOS__ || __TVOS__
using UserNotifications;
#endif

namespace UnexpectedApis.Apis;

public sealed partial class BadgeNotificationDemo : UserControl
{
    public BadgeNotificationDemo() => InitializeComponent();

    private async void OnSetClick(object sender, RoutedEventArgs e)
    {
        if (double.IsNaN(BadgeBox.Value))
        {
            StatusText.Text = "Enter a number first.";
            return;
        }

        try
        {
#if __IOS__ || __TVOS__
            await UNUserNotificationCenter.Current.RequestAuthorizationAsync(UNAuthorizationOptions.Badge);
#endif
            XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);
            XmlElement badgeElement = (XmlElement)badgeXml.SelectSingleNode("/badge");
            badgeElement.SetAttribute("value", ((int)BadgeBox.Value).ToString());

            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(new BadgeNotification(badgeXml));
            StatusText.Text = $"Badge set to {(int)BadgeBox.Value}.";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Badges aren't available here: {ex.Message}";
        }
    }

    private void OnClearClick(object sender, RoutedEventArgs e)
    {
        try
        {
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
            StatusText.Text = "Badge cleared.";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Badges aren't available here: {ex.Message}";
        }
    }
}

