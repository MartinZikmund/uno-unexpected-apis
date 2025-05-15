using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using UnexpectedApis.Attributes;

namespace UnexpectedApis.Views;

[Sample("Sharing", "Sharing.png", SampleKind.NonUI, TargetPlatforms.All & ~TargetPlatforms.SkiaDesktop)]
public sealed partial class SharingPage : SamplePage
{
    public SharingPage()
    {
        this.InitializeComponent();
    }

    public string Code =>
"""
DataTransferManager.GetForCurrentView().DataRequested += (s,e) =>
{
    args.Request.Data.Properties.Title = "Unexpected APIs in Uno Platform";
    args.Request.Data.Properties.Description = "Link to the app";
    args.Request.Data.SetWebLink(new Uri("https://aka.platform.uno/demo-unexpected-apis"));
};

DataTransferManager.ShowShareUI();
""";

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        DataTransferManager.GetForCurrentView().DataRequested += SharingPage_DataRequested;
    }

    private void Share_Click(object sender, RoutedEventArgs e)
    {
        DataTransferManager.ShowShareUI();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        DataTransferManager.GetForCurrentView().DataRequested -= SharingPage_DataRequested;
    }

    private void SharingPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
    {
        var deferral = args.Request.GetDeferral();

        args.Request.Data.ShareCompleted += Data_ShareCompleted;
        args.Request.Data.ShareCanceled += Data_ShareCanceled;
        args.Request.Data.Properties.Title = "Unexpected APIs in Uno Platform";
        args.Request.Data.Properties.Description = "Link to the app";
        args.Request.Data.SetWebLink(new Uri("https://unexpectedapis.uno"));

        deferral.Complete();
    }

    private void Data_ShareCanceled(DataPackage sender, object args)
    {
        sender.ShareCanceled -= Data_ShareCanceled;
    }

    private void Data_ShareCompleted(DataPackage sender, ShareCompletedEventArgs args)
    {
        sender.ShareCompleted -= Data_ShareCompleted;
    }
}
