using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UnexpectedApisDemo.Shared.UserControls
{
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
}
