﻿using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Views;

namespace UnexpectedApisDemo.Droid
{
	[Activity(
		Icon = "@drawable/icon",
		MainLauncher = true,
		ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
		WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
	)]
	public class MainActivity : Windows.UI.Xaml.ApplicationActivity
	{
	}
}

