// Guarded at runtime: these members exist only on some heads.
#pragma warning disable Uno0001

using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace UnexpectedApis.Apis;

public sealed partial class WebViewDemo : UserControl
{
    private Microsoft.UI.Xaml.Controls.WebView2? _web;

    public WebViewDemo()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_web is not null)
        {
            return;
        }

        try
        {
            _web = new Microsoft.UI.Xaml.Controls.WebView2();
            Host.Children.Insert(0, _web);
            Navigate();
        }
        catch (Exception ex)
        {
            Show($"WebView2 isn't available here: {ex.Message}");
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_web is null)
        {
            return;
        }

        try
        {
            Host.Children.Remove(_web);
            _web.Close();
        }
        catch
        {
            // Closing is best effort.
        }

        _web = null;
    }

    private void OnGoClick(object sender, RoutedEventArgs e) => Navigate();

    private void OnAddressKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            Navigate();
        }
    }

    private void Navigate()
    {
        if (_web is null)
        {
            return;
        }

        var text = AddressBox.Text.Trim();
        if (!text.Contains("://", StringComparison.Ordinal))
        {
            text = "https://" + text;
        }

        if (Uri.TryCreate(text, UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https")
        {
            try
            {
                _web.Source = uri;
                MessageText.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Show($"Couldn't open the page: {ex.Message}");
            }
        }
        else
        {
            Show("That doesn't look like a web address.");
        }
    }

    private void Show(string message)
    {
        MessageText.Text = message;
        MessageText.Visibility = Visibility.Visible;
    }
}
