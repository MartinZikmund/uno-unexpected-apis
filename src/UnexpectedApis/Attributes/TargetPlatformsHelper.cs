namespace UnexpectedApis.Attributes;

internal static class TargetPlatformsHelper
{
    private static readonly Lazy<TargetPlatforms> _currentPlatform = new Lazy<TargetPlatforms>(GetCurrentPlatform);

    /// <summary>
    /// Returns the current runtime test platform.
    /// </summary>
    public static TargetPlatforms CurrentPlatform => _currentPlatform.Value;

    private static TargetPlatforms GetCurrentPlatform()
    {
        var values = Enum.GetValues<TargetPlatforms>();
        var currentPlatform = default(TargetPlatforms);
        var counter = 0;
        foreach (var value in values.Where(HasSingleFlag))
        {
            if (IsCurrentTarget(value))
            {
                currentPlatform |= value;
                counter++;
            }
        }

        if (counter == 0)
        {
            throw new InvalidOperationException("Unrecognized runtime platform.");
        }

        if (counter > 1)
        {
            throw new InvalidOperationException($"Multiple runtime platforms detected ({currentPlatform:g})");
        }

        return currentPlatform;
    }

    private static bool HasSingleFlag(TargetPlatforms value)
    {
        var numericValue = Convert.ToInt64(value);

        // Check if exactly one bit is set (i.e., power of two)
        return numericValue != 0 && (numericValue & (numericValue - 1)) == 0;
    }

    private static bool IsCurrentTarget(TargetPlatforms singlePlatform)
    {
        return singlePlatform switch
        {
            TargetPlatforms.NativeWinUI => IsWinUI(),
            TargetPlatforms.NativeWasm => IsNativeWasm(),
            TargetPlatforms.NativeAndroid => IsNativeAndroid(),
            TargetPlatforms.NativeIOS => IsNativeIOS(),
            TargetPlatforms.NativeMacCatalyst => IsNativeMacCatalyst(),
            TargetPlatforms.NativeTvOS => IsNativetvOS(),
            TargetPlatforms.SkiaWin32 => IsSkiaWin32(),
            TargetPlatforms.SkiaX11 => IsSkiaX11(),
            TargetPlatforms.SkiaMacOS => IsSkiaMacOS(),
            TargetPlatforms.SkiaIslands => IsSkiaIslands(),
            TargetPlatforms.SkiaWasm => IsSkia() && OperatingSystem.IsBrowser(),
            TargetPlatforms.SkiaAndroid => IsSkia() && OperatingSystem.IsAndroid(),
            TargetPlatforms.SkiaIOS => IsSkia() && OperatingSystem.IsIOS(),
            TargetPlatforms.SkiaTvOS => IsSkia() && OperatingSystem.IsTvOS(),
            TargetPlatforms.SkiaMacCatalyst => IsSkia() && OperatingSystem.IsMacCatalyst(),
            _ => throw new ArgumentException(nameof(singlePlatform)),
        };
    }

    private static bool HasSkiaHostAssembly(string name)
    {
        var assembly = typeof(TargetPlatformsHelper).Assembly;
        var referencedAssemblies = assembly.GetReferencedAssemblies();
        var hostAssembly = referencedAssemblies
            .FirstOrDefault(a => a.Name == name);
        
        return hostAssembly != null;
    }

    private static bool IsSkia() =>
#if __UNO_SKIA__
        true;
#else
        false;
#endif

    private static bool IsWinUI() =>
#if WINAPPSDK
		true;
#else
        false;
#endif

    private static bool IsSkiaDesktop()
        => HasSkiaHostAssembly("Uno.UI.Runtime.Skia.Wpf") ||
           HasSkiaHostAssembly("Uno.UI.Runtime.Skia.Win32") ||
           HasSkiaHostAssembly("Uno.UI.Runtime.Skia.X11") ||
           HasSkiaHostAssembly("Uno.UI.Runtime.Skia.MacOS") ||
           HasSkiaHostAssembly("Uno.UI.Runtime.Skia.Islands");

    private static bool IsSkiaWin32() => IsSkia() && !IsWinUI() && OperatingSystem.IsWindows();

    private static bool IsSkiaX11() => IsSkia() && OperatingSystem.IsLinux();

    private static bool IsSkiaMacOS() => IsSkia() && OperatingSystem.IsMacOS();

    private static bool IsSkiaIslands()
#if __SKIA__
		=> Microsoft.UI.Xaml.Application.Current.Host is null;
#else
        => false;
#endif

    private static bool IsNativeWasm() => !IsSkia() && OperatingSystem.IsBrowser();

    private static bool IsNativeAndroid() => !IsSkia() && OperatingSystem.IsAndroid();

    private static bool IsNativeIOS() => !IsSkia() && OperatingSystem.IsIOS();

    private static bool IsNativetvOS() => !IsSkia() && OperatingSystem.IsTvOS();

    private static bool IsNativeMacCatalyst() => !IsSkia() && OperatingSystem.IsMacCatalyst();
}
