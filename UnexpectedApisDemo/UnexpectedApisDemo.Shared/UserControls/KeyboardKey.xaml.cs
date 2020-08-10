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
}
