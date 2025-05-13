using Windows.Networking.Connectivity;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace UnexpectedApis.Views;

public sealed partial class NetworkInformationPage : SamplePage
{
    public NetworkInformationPage()
    {
        this.InitializeComponent();
    }

    public string Code =>
"""
NetworkInformation.NetworkStatusChanged += (s,e) =>
{
    var profile = NetworkInformation.GetInternetConnectionProfile();
    var level = profile.GetNetworkConnectivityLevel();
    if (level == NetworkConnectivityLevel.InternetAccess)
    {
        // Connected to the internet
    }
}
""";

    private void CheckConnectivity_Click(object sender, RoutedEventArgs e) => UpdateConnectivity();

    private void UpdateConnectivity()
    {
        var profile = NetworkInformation.GetInternetConnectionProfile();
        if (profile != null)
        {
            var level = profile.GetNetworkConnectivityLevel();
            OnlineImage.Visibility = level == NetworkConnectivityLevel.InternetAccess ? Visibility.Visible : Visibility.Collapsed;
            OfflineImage.Visibility = level != NetworkConnectivityLevel.InternetAccess ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void ObserveConnectivity_Checked(object sender, RoutedEventArgs e)
    {
        NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
    }

    private void ObserveConnectivity_Unchecked(object sender, RoutedEventArgs e)
    {
        NetworkInformation.NetworkStatusChanged -= NetworkInformation_NetworkStatusChanged;
    }

    private void NetworkInformation_NetworkStatusChanged(object sender) => UpdateConnectivity();
}
