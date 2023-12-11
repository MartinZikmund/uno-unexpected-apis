using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace UnexpectedApis.UserControls;

[ContentProperty(Name = nameof(Content))]
public sealed partial class SamplePageControl : UserControl
{
    public SamplePageControl()
    {
        this.InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static DependencyProperty TitleProperty { get; } =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(SamplePageControl), new PropertyMetadata(""));



    public UIElement Content
    {
        get => (UIElement)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static DependencyProperty ContentProperty { get; } =
        DependencyProperty.Register(nameof(Content), typeof(UIElement), typeof(SamplePageControl), new PropertyMetadata(null));



    public string Code
    {
        get => (string)GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    public static DependencyProperty CodeProperty { get; } =
        DependencyProperty.Register(nameof(Code), typeof(string), typeof(SamplePageControl), new PropertyMetadata(""));
}
