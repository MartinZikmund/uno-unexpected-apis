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

namespace UnexpectedApis.UserControls;

public sealed partial class SamplePageHeader : UserControl
{
    public SamplePageHeader()
    {
        this.InitializeComponent();
    }

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(SamplePageHeader), new PropertyMetadata(""));

    public bool SupportsWindows
    {
        get { return (bool)GetValue(SupportsWindowsProperty); }
        set { SetValue(SupportsWindowsProperty, value); }
    }

    public static readonly DependencyProperty SupportsWindowsProperty =
        DependencyProperty.Register("SupportsWindows", typeof(bool), typeof(SamplePageHeader), new PropertyMetadata(true));

    public bool SupportsMacos
    {
        get { return (bool)GetValue(SupportsMacosProperty); }
        set { SetValue(SupportsMacosProperty, value); }
    }

    public static readonly DependencyProperty SupportsMacosProperty =
        DependencyProperty.Register("SupportsMacos", typeof(bool), typeof(SamplePageHeader), new PropertyMetadata(true));

    public bool SupportsIos
    {
        get { return (bool)GetValue(SupportsIosProperty); }
        set { SetValue(SupportsIosProperty, value); }
    }

    public static readonly DependencyProperty SupportsIosProperty =
        DependencyProperty.Register("SupportsIos", typeof(bool), typeof(SamplePageHeader), new PropertyMetadata(true));

    public bool SupportsWasm
    {
        get { return (bool)GetValue(SupportsWasmProperty); }
        set { SetValue(SupportsWasmProperty, value); }
    }

    public static readonly DependencyProperty SupportsWasmProperty =
        DependencyProperty.Register("SupportsWasm", typeof(bool), typeof(SamplePageHeader), new PropertyMetadata(true));

    public bool SupportsAndroid
    {
        get { return (bool)GetValue(SupportsAndroidProperty); }
        set { SetValue(SupportsAndroidProperty, value); }
    }

    public static readonly DependencyProperty SupportsAndroidProperty =
        DependencyProperty.Register("SupportsAndroid", typeof(bool), typeof(SamplePageHeader), new PropertyMetadata(true));
}
