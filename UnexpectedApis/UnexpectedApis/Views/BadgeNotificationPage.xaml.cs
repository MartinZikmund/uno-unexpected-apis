using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UnexpectedApis.Views;

public sealed partial class BadgeNotificationPage : Page
{
    public BadgeNotificationPage()
    {
        this.InitializeComponent();
    }

    public string Code =>
"""
var badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);
XmlElement badgeElement = (XmlElement)badgeXml.SelectSingleNode("/badge");
badgeElement.SetAttribute("value", BadgeNumberBox.Value.ToString());

var badgeNotification = new BadgeNotification(badgeXml);

BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeNotification);
""";

    private void SetBadge_Click(object sender, RoutedEventArgs e)
    {
        var badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);
        XmlElement badgeElement = (XmlElement)badgeXml.SelectSingleNode("/badge");
        badgeElement.SetAttribute("value", BadgeNumberBox.Value.ToString());

        var badgeNotification = new BadgeNotification(badgeXml);

        BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeNotification);
    }

    private void ClearBadge_Click(object sender, RoutedEventArgs e)
    {
        BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
    }
}
