using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnexpectedApis.ViewModels;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UnexpectedApis.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[Sample("Clipboard", "Clipboard.png", SampleKind.NonUI)]
public sealed partial class ClipboardPage : SamplePage
{
    public ClipboardPage()
    {
        this.InitializeComponent();
        Model = new ClipboardViewModel();
        DataContext = Model;
        this.Unloaded += ClipboardPage_Unloaded;
    }

    public string Code =>
"""
// Set content
DataPackage dataPackage = new DataPackage();
dataPackage.SetText("Hello world");
Clipboard.SetContent(dataPackage);

// Get content
var content = Clipboard.GetContent();
var text = await content.GetTextAsync();
""";

    public ClipboardViewModel Model { get; private set; }

    private void ClipboardPage_Unloaded(object sender, RoutedEventArgs e)
    {
        Model.Dispose();
    }
}
