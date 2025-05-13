using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace UIApis.UserControls;

public sealed partial class KeyboardKey : Page
{
    public KeyboardKey()
    {
        this.InitializeComponent();
    }

    public string KeyText
    {
        get { return (string)GetValue(KeyTextProperty); }
        set { SetValue(KeyTextProperty, value); }
    }

    public static readonly DependencyProperty KeyTextProperty =
        DependencyProperty.Register(nameof(KeyText), typeof(string), typeof(KeyboardKey), new PropertyMetadata(""));

    public event RoutedEventHandler Click;

    private void ButtonClick(object sender, RoutedEventArgs args)
    {
        Click?.Invoke(this, args);
    }
}
