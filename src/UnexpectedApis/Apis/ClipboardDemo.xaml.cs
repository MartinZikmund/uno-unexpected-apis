// Not implemented on every head; calls are guarded and the demo says so.
#pragma warning disable Uno0001
using Windows.ApplicationModel.DataTransfer;

namespace UnexpectedApis.Apis;

public sealed partial class ClipboardDemo : UserControl
{
    public ClipboardDemo()
    {
        InitializeComponent();
        Unloaded += (_, _) => Try(() => Clipboard.ContentChanged -= OnContentChanged);
    }

    private void OnCopyClick(object sender, RoutedEventArgs e) => Try(() =>
    {
        DataPackage package = new();
        package.SetText(Input.Text);
        Clipboard.SetContent(package);
        StatusText.Text = "Copied.";
    });

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

    private void OnClearClick(object sender, RoutedEventArgs e) => Try(() =>
    {
        Clipboard.Clear();
        StatusText.Text = "Cleared.";
    });

    private void OnObserveToggled(object sender, RoutedEventArgs e) => Try(() =>
    {
        Clipboard.ContentChanged -= OnContentChanged;
        if (ObserveSwitch.IsOn)
        {
            Clipboard.ContentChanged += OnContentChanged;
        }
    });

    private void Try(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            StatusText.Text = $"The clipboard isn't available here: {ex.Message}";
        }
    }

    private void OnContentChanged(object? sender, object e) =>
        DispatcherQueue.TryEnqueue(() => StatusText.Text = $"Clipboard changed at {DateTime.Now:T}");
}
