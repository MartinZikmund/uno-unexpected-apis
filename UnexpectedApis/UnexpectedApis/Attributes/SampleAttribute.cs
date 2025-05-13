namespace UnexpectedApis.Attributes;

public class SampleAttribute : Attribute
{
    private readonly string _iconFileName;

    public SampleAttribute(string name, string iconFileName, SampleKind kind, DisabledPlatforms disabledPlatforms = DisabledPlatforms.None)
    {
        _iconFileName = iconFileName;
        DisplayName = name;
        Kind = kind;
    }

    public string DisplayName { get; }

    public SampleKind Kind { get; }

    public Uri IconUri => new Uri($"ms-appx:///Assets/Samples/{_iconFileName}");

    public DisabledPlatforms DisabledPlatforms { get; set; } = DisabledPlatforms.None;
}
