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

namespace UnexpectedApis.Views;

[Sample("SpeechRecognizer", "SpeechRecognizer.png", SampleKind.NonUI)]
public sealed partial class SpeechRecognizerPage : SamplePage
{
    public SpeechRecognizerPage()
    {
        this.InitializeComponent(); Model = new SpeechRecognizerViewModel(DispatcherQueue);
        DataContext = Model;
        this.Unloaded += SpeechRecognizerPage_Unloaded;
    }

    public string Code =>
"""
var speechRecognizer = new SpeechRecognizer();
await _speechRecognizer.CompileConstraintsAsync();
var result = await _speechRecognizer.RecognizeAsync();
""";

    public SpeechRecognizerViewModel Model { get; }

    private void SpeechRecognizerPage_Unloaded(object sender, RoutedEventArgs e)
    {
        Model.Dispose();
    }
}
