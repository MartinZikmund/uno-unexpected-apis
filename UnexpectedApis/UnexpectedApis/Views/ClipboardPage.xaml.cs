﻿using System;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UnexpectedApisDemo.Shared.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ClipboardPage : Page
{
    public ClipboardPage()
    {
        this.InitializeComponent();
        Model = new ClipboardViewModel();
        DataContext = Model;
        this.Unloaded += ClipboardPage_Unloaded;
    }

    public ClipboardViewModel Model { get; private set; }

    private void ClipboardPage_Unloaded(object sender, RoutedEventArgs e)
    {
        Model.Dispose();
    }
}
