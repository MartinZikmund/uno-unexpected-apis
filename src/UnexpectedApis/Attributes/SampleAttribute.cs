namespace UnexpectedApis.Attributes;

public class SampleAttribute : Attribute
{
    private readonly string _iconFileName;

    public SampleAttribute(string name, string iconFileName, SampleKind kind, TargetPlatforms targetPlatforms = TargetPlatforms.All)
    {
        _iconFileName = iconFileName;
        DisplayName = name;
        Kind = kind;
        TargetPlatforms = targetPlatforms;
    }

    public string DisplayName { get; }

    public SampleKind Kind { get; }

    public Uri IconUri => new Uri($"ms-appx:///Assets/Samples/{_iconFileName}");

    public TargetPlatforms TargetPlatforms { get; }
}
