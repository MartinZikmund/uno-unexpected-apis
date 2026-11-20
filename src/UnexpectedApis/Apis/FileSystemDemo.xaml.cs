// Not implemented on every head; calls are guarded and the demo says so.
#pragma warning disable Uno0001
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace UnexpectedApis.Apis;

public sealed partial class FileSystemDemo : UserControl
{
    private StorageFolder? _folder;
    private StorageFile? _file;

    public FileSystemDemo() => InitializeComponent();

    public ObservableCollection<StorageFile> Files { get; } = new();

    private async void OnUseAppFolderClick(object sender, RoutedEventArgs e) =>
        await SetFolderAsync(ApplicationData.Current.LocalFolder);

    private async void OnPickFolderClick(object sender, RoutedEventArgs e)
    {
        try
        {
            FolderPicker picker = new()
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                FileTypeFilter = { "*" },
            };
#if WINDOWS
            WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow));
#endif
            StorageFolder? folder = await picker.PickSingleFolderAsync();
            if (folder is not null)
            {
                await SetFolderAsync(folder);
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Folder picking isn't available here: {ex.Message}";
        }
    }

    private async Task SetFolderAsync(StorageFolder folder)
    {
        _folder = folder;
        CreateButton.IsEnabled = true;
        FileNameBox.IsEnabled = true;
        SetFile(null);
        await RefreshFilesAsync();
    }

    private async Task RefreshFilesAsync()
    {
        if (_folder is null)
        {
            return;
        }

        try
        {
            IReadOnlyList<StorageFile> files = await _folder.GetFilesAsync();
            Files.Clear();
            foreach (StorageFile file in files)
            {
                Files.Add(file);
            }

            StatusText.Text = $"{_folder.Name}: {files.Count} file(s).";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Couldn't list the folder: {ex.Message}";
        }
    }

    private async void OnCreateClick(object sender, RoutedEventArgs e)
    {
        string name = FileNameBox.Text.Trim();
        if (_folder is null || name.Length == 0 || name.IndexOfAny(['/', '\\']) >= 0)
        {
            StatusText.Text = "Enter a plain file name (no slashes).";
            return;
        }

        try
        {
            await _folder.CreateFileAsync(name, CreationCollisionOption.OpenIfExists);
            await RefreshFilesAsync();
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Couldn't create the file: {ex.Message}";
        }
    }

    private async void OnWriteClick(object sender, RoutedEventArgs e)
    {
        if (_file is null)
        {
            return;
        }

        try
        {
            await FileIO.WriteTextAsync(_file, ContentBox.Text);
            StatusText.Text = $"Wrote {_file.Name}.";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Couldn't write the file: {ex.Message}";
        }
    }

    private async void OnFileSelected(object sender, SelectionChangedEventArgs e)
    {
        SetFile(FilesList.SelectedItem as StorageFile);
        if (_file is null)
        {
            return;
        }

        try
        {
            ContentBox.Text = await FileIO.ReadTextAsync(_file);
        }
        catch (Exception)
        {
            ContentBox.Text = string.Empty;
        }
    }

    private void SetFile(StorageFile? file)
    {
        _file = file;
        WriteButton.IsEnabled = file is not null;
        ContentBox.IsEnabled = file is not null;
        if (file is null)
        {
            ContentBox.Text = string.Empty;
        }
    }
}

