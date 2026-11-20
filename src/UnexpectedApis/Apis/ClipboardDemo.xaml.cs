using Windows.ApplicationModel.DataTransfer;

namespace UnexpectedApis.Apis;

public sealed partial class ClipboardDemo : UserControl
{
    public ClipboardDemo()
    {
        InitializeComponent();
        Unloaded += (_, _) => Clipboard.ContentChanged -= OnContentChanged;
    }

    private void OnCopyClick(object sender, RoutedEventArgs e)
    {
        DataPackage package = new();
        package.SetText(Input.Text);
        Clipboard.SetContent(package);
        StatusText.Text = "Copied.";
    }

    private async void OnPasteClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var content = Clipboard.GetContent();
            if (content.Contains(StandardDataFormats.Text))
            {
                Input.Text = await content.GetTextAsync();
                StatusText.Text = "Pasted.";
            }
            else
            {
                StatusText.Text = "The clipboard holds no text.";
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Couldn't read the clipboard: {ex.Message}";
        }
    }

    private void OnClearClick(object sender, RoutedEventArgs e)
    {
        Clipboard.Clear();
        StatusText.Text = "Cleared.";
    }

    private void OnObserveToggled(object sender, RoutedEventArgs e)
    {
        Clipboard.ContentChanged -= OnContentChanged;
        if (ObserveSwitch.IsOn)
        {
            Clipboard.ContentChanged += OnContentChanged;
        }
    }

    private void OnContentChanged(object? sender, object e) =>
        DispatcherQueue.TryEnqueue(() => StatusText.Text = $"Clipboard changed at {DateTime.Now:T}");
}
