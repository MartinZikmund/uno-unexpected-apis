namespace UnexpectedApis.Attributes;

internal static class DisabledPlatformsHelper
{
    private static readonly Lazy<DisabledPlatforms> _currentPlatform = new Lazy<DisabledPlatforms>(GetCurrentPlatform);

    /// <summary>
    /// Returns the current runtime test platform.
    /// </summary>
    public static DisabledPlatforms CurrentPlatform => _currentPlatform.Value;

    private static DisabledPlatforms GetCurrentPlatform()
    {
        var values = Enum.GetValues<DisabledPlatforms>();
        var currentPlatform = default(DisabledPlatforms);
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

    private static bool HasSingleFlag(DisabledPlatforms value)
    {
        var numericValue = Convert.ToInt64(value);

        // Check if exactly one bit is set (i.e., power of two)
        return numericValue != 0 && (numericValue & (numericValue - 1)) == 0;
    }

    private static bool IsCurrentTarget(DisabledPlatforms singlePlatform)
    {
        return singlePlatform switch
        {
            DisabledPlatforms.NativeWinUI => IsWinUI(),
            DisabledPlatforms.NativeWasm => IsNativeWasm(),
            DisabledPlatforms.NativeAndroid => IsNativeAndroid(),
            DisabledPlatforms.NativeIOS => IsNativeIOS(),
            DisabledPlatforms.NativeMacCatalyst => IsNativeMacCatalyst(),
            DisabledPlatforms.NativeTvOS => IsNativetvOS(),
            DisabledPlatforms.SkiaWin32 => IsSkiaWin32(),
            DisabledPlatforms.SkiaX11 => IsSkiaX11(),
            DisabledPlatforms.SkiaMacOS => IsSkiaMacOS(),
            DisabledPlatforms.SkiaIslands => IsSkiaIslands(),
            DisabledPlatforms.SkiaWasm => IsSkia() && OperatingSystem.IsBrowser(),
            DisabledPlatforms.SkiaAndroid => IsSkia() && OperatingSystem.IsAndroid(),
            DisabledPlatforms.SkiaIOS => IsSkia() && OperatingSystem.IsIOS(),
            DisabledPlatforms.SkiaTvOS => IsSkia() && OperatingSystem.IsTvOS(),
            DisabledPlatforms.SkiaMacCatalyst => IsSkia() && OperatingSystem.IsMacCatalyst(),
            _ => throw new ArgumentException(nameof(singlePlatform)),
        };
    }

    private static bool HasSkiaHostAssembly(string name)
    {
        var assembly = typeof(DisabledPlatformsHelper).Assembly;
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
