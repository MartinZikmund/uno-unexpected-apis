namespace UnexpectedApis.Core;

/// <summary>The app heads an API sample works on.</summary>
[Flags]
public enum ApiPlatforms
{
    None = 0,
    Android = 1 << 0,
    iOS = 1 << 1,
    TvOS = 1 << 2,
    Windows = 1 << 3,
    WebAssembly = 1 << 4,
    Desktop = 1 << 5,

    Apple = iOS | TvOS,
    Mobile = Android | Apple,
    All = Mobile | Windows | WebAssembly | Desktop,
}

public static class ApiPlatformInfo
{
    public static IReadOnlyList<(ApiPlatforms Platform, string Name)> Each { get; } =
    ((ApiPlatforms, string)[])[
        (ApiPlatforms.Android, "Android"),
        (ApiPlatforms.iOS, "iOS"),
        (ApiPlatforms.TvOS, "tvOS"),
        (ApiPlatforms.Windows, "Windows"),
        (ApiPlatforms.WebAssembly, "WebAssembly"),
        (ApiPlatforms.Desktop, "Desktop"),
    ];

    public static string Name(ApiPlatforms platform) => Each.First(p => p.Platform == platform).Name;

    public static string Describe(ApiPlatforms platforms) =>
        platforms == ApiPlatforms.All
            ? "All platforms"
            : string.Join(", ", Each.Where(p => platforms.HasFlag(p.Platform)).Select(p => p.Name));
}
