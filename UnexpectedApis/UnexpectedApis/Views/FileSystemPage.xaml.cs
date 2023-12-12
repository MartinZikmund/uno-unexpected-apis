using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using WinRT.Interop;

namespace UnexpectedApis.Views;

public sealed partial class FileSystemPage : SamplePage
{
    private StorageFolder _pickedFolder;
    private StorageFile _selectedFile;

    public FileSystemPage()
    {
        this.InitializeComponent();
    }

    public string Code =>
"""
var picker = new FolderPicker()
{
    SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
    FileTypeFilter = { "*" }
};
var handle = WindowNative.GetWindowHandle(MainWindow);
InitializeWithWindow.Initialize(picker, handle);
var folder = await picker.PickSingleFolderAsync();

var files = await folder.GetFilesAsync();
var newFile = await folder.CreateFileAsync("hello.txt");
await FileIO.WriteTextAsync(newFiled, "Hello world!");
""";

    public ObservableCollection<StorageFile> Files { get; } = new ObservableCollection<StorageFile>();

    public StorageFile SelectedFile
    {
        get => _selectedFile;
        set
        {
            _selectedFile = value;
            WriteToFileButton.IsEnabled = _selectedFile != null;
        }
    }

    public StorageFolder PickedFolder
    {
        get => _pickedFolder;
        set
        {
            _pickedFolder = value;
            GetFileListButton.IsEnabled = _pickedFolder != null;
            CreateFileButton.IsEnabled = _pickedFolder != null;
        }
    }

    private async void PickFolder(object sender, RoutedEventArgs args)
    {
        try
        {
            var picker = new FolderPicker()
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                FileTypeFilter = { "*" }
            };
            var window = WindowNative.GetWindowHandle(App.Instance.MainWindow!);
            InitializeWithWindow.Initialize(picker, window);
            PickedFolder = await picker.PickSingleFolderAsync();
            SelectedFile = null;
            await UpdateFilesAsync();
        }
        catch
        {
            var dialog = new MessageDialog("This browser does not support folder picker", "Not supported");
            await dialog.ShowAsync();
        }
    }

    private async void GetFileList(object sender, RoutedEventArgs args)
    {
        await UpdateFilesAsync();
    }

    private async Task UpdateFilesAsync()
    {
        if (_pickedFolder != null)
        {
            Files.Clear();
            if (_pickedFolder == null)
            {
                return;
            }

            var files = await _pickedFolder.GetFilesAsync();
            foreach (var file in files)
            {
                Files.Add(file);
            }
        }
    }

    private async void CreateFile(object sender, RoutedEventArgs args)
    {
        if (_pickedFolder != null)
        {
            await _pickedFolder.CreateFileAsync(FileNameTextBox.Text);
            await UpdateFilesAsync();
        }
    }

    private async void WriteToFile(object sender, RoutedEventArgs args)
    {
        if (_selectedFile != null)
        {
            await FileIO.WriteTextAsync(_selectedFile, ContentTextBox.Text);
        }
    }

    private void FilesSelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        if (FileListView.SelectedItem is StorageFile file)
        {
            SelectedFile = file;
        }
        else
        {
            SelectedFile = null;
        }
    }
}
