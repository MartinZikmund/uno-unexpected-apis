// Not implemented on every head; calls are guarded and the demo says so.
#pragma warning disable Uno0001
using Windows.ApplicationModel.DataTransfer;

namespace UnexpectedApis.Apis;

public sealed partial class SharingDemo : UserControl
{
    private DataTransferManager? _manager;

    public SharingDemo()
    {
        InitializeComponent();
        Unloaded += (_, _) => Detach();
    }

    private void OnShareClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_manager is null)
            {
                _manager = DataTransferManager.GetForCurrentView();
                _manager.DataRequested += OnDataRequested;
            }

            DataTransferManager.ShowShareUI();
        }
        catch (Exception)
        {
            Detach();
            StatusText.Text = "Sharing isn't available on this platform.";
        }
    }

    private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
    {
        DataRequestDeferral deferral = args.Request.GetDeferral();
        args.Request.Data.Properties.Title = "Unexpected APIs in Uno Platform";
        args.Request.Data.Properties.Description = "Link to the app";
        args.Request.Data.SetWebLink(new Uri("https://aka.platform.uno/demo-unexpected-apis"));
        args.Request.Data.ShareCompleted += OnShareCompleted;
        args.Request.Data.ShareCanceled += OnShareCanceled;
        deferral.Complete();
    }

    private void OnShareCompleted(DataPackage sender, ShareCompletedEventArgs args)
    {
        Unhook(sender);
        DispatcherQueue.TryEnqueue(() => StatusText.Text = "Shared.");
    }

    private void OnShareCanceled(DataPackage sender, object? args)
    {
        Unhook(sender);
        DispatcherQueue.TryEnqueue(() => StatusText.Text = "Share canceled.");
    }

    private void Unhook(DataPackage package)
    {
        package.ShareCompleted -= OnShareCompleted;
        package.ShareCanceled -= OnShareCanceled;
    }

    private void Detach()
    {
        if (_manager is not null)
        {
            _manager.DataRequested -= OnDataRequested;
            _manager = null;
        }
    }
}


