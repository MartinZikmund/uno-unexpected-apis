using UnexpectedApis.Core;

namespace UnexpectedApis.Services;

public static class CurrentPlatform
{
    public static ApiPlatforms Value { get; } = Detect();

    public static string Name => ApiPlatformInfo.Name(Value);

    public static bool Supports(ApiPlatforms platforms) => platforms.HasFlag(Value);

    private static ApiPlatforms Detect()
    {
#if !HAS_UNO
        return ApiPlatforms.Windows;
#else
        if (OperatingSystem.IsAndroid())
        {
            return ApiPlatforms.Android;
        }

        // Check tvOS first: it reports as iOS too.
        if (OperatingSystem.IsTvOS())
        {
            return ApiPlatforms.TvOS;
        }

        if (OperatingSystem.IsIOS())
        {
            return ApiPlatforms.iOS;
        }

        return OperatingSystem.IsBrowser() ? ApiPlatforms.WebAssembly : ApiPlatforms.Desktop;
#endif
    }
}
