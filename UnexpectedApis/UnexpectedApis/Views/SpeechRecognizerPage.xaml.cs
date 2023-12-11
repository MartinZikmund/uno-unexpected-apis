using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnexpectedApisDemo.Shared.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace UnexpectedApisDemo.Shared.Views;

public sealed partial class SpeechRecognizerPage : Page
{
    public SpeechRecognizerPage()
    {
        this.InitializeComponent(); Model = new SpeechRecognizerViewModel(DispatcherQueue);
        DataContext = Model;
        this.Unloaded += SpeechRecognizerPage_Unloaded;
    }

    public SpeechRecognizerViewModel Model { get; }

    private void SpeechRecognizerPage_Unloaded(object sender, RoutedEventArgs e)
    {
        Model.Dispose();
    }
}
