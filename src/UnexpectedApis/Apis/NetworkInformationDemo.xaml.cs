using Windows.Networking.Connectivity;

namespace UnexpectedApis.Apis;

public sealed partial class NetworkInformationDemo : UserControl
{
    private bool _observing;

    public NetworkInformationDemo()
    {
        InitializeComponent();
        Loaded += (_, _) => UpdateConnectivity();
        Unloaded += (_, _) => StopObserving();
    }

    private void OnCheckClick(object sender, RoutedEventArgs e) => UpdateConnectivity();

    private void OnObserveChecked(object sender, RoutedEventArgs e)
    {
        try
        {
            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
            _observing = true;
        }
        catch (Exception ex) when (ex is NotImplementedException or NotSupportedException or UnauthorizedAccessException)
        {
            ObserveToggle.IsChecked = false;
            StatusText.Text = "Watching for changes isn't available on this platform.";
        }
    }

    private void OnObserveUnchecked(object sender, RoutedEventArgs e) => StopObserving();

    private void StopObserving()
    {
        if (_observing)
        {
            NetworkInformation.NetworkStatusChanged -= OnNetworkStatusChanged;
            _observing = false;
        }
    }

    private void OnNetworkStatusChanged(object sender) => DispatcherQueue.TryEnqueue(UpdateConnectivity);

    private void UpdateConnectivity()
    {
        try
        {
            ConnectionProfile? profile = NetworkInformation.GetInternetConnectionProfile();
            NetworkConnectivityLevel level = profile?.GetNetworkConnectivityLevel() ?? NetworkConnectivityLevel.None;
            bool online = level == NetworkConnectivityLevel.InternetAccess;
            OnlineImage.Visibility = online ? Visibility.Visible : Visibility.Collapsed;
            OfflineImage.Visibility = online ? Visibility.Collapsed : Visibility.Visible;
            StatusText.Text = profile is null ? "No connection profile: offline." : $"Connectivity level: {level}";
        }
        catch (Exception ex) when (ex is NotImplementedException or NotSupportedException or UnauthorizedAccessException)
        {
            OnlineImage.Visibility = Visibility.Collapsed;
            OfflineImage.Visibility = Visibility.Collapsed;
            StatusText.Text = "Network information isn't available on this platform.";
        }
    }
}
