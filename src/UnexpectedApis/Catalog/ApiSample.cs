using UnexpectedApis.Core;

namespace UnexpectedApis.Catalog;

/// <summary>
/// A platform API demo: a self-contained control on the stage plus the C# that matters.
/// The demo must not throw where the API is missing; it shows that it is unavailable instead.
/// </summary>
public sealed class ApiSample : Sample
{
    private readonly Func<FrameworkElement> _create;
    private readonly Uri _docs;
    private readonly ApiPlatforms _platforms;
    private string _code = string.Empty;
    private string _language = "C#";

    public ApiSample(
        string id,
        string title,
        Category category,
        Symbol icon,
        string summary,
        ApiPlatforms platforms,
        string docsUrl,
        Func<FrameworkElement> create)
        : base(id, title, category, icon, summary)
    {
        _platforms = platforms;
        _docs = new(docsUrl);
        _create = create;
        Fills = true;
        StageHeight = 420;
    }

    public override Uri DocsUri => _docs;

    public override bool IsLiveControl => false;

    public override string SnippetLanguage => _language;

    public override ApiPlatforms Platforms => _platforms;

    public ApiSample Code(string csharp) => Snippet(csharp, "C#");

    /// <summary>For samples whose essence is markup rather than code.</summary>
    public ApiSample Xaml(string xaml) => Snippet(xaml, "XAML");

    private ApiSample Snippet(string code, string language)
    {
        _code = code.Trim();
        _language = language;
        return this;
    }

    public ApiSample Tip(string tip)
    {
        Tips.Add(tip);
        return this;
    }

    public ApiSample Find(params string[] keywords)
    {
        Keywords.AddRange(keywords);
        return this;
    }

    public ApiSample Height(double height)
    {
        StageHeight = height;
        return this;
    }

    public override SampleSession CreateSession() => new Session(this);

    private sealed class Session(ApiSample owner) : SampleSession
    {
        public override IReadOnlyList<Knob> Knobs => [];

        public override FrameworkElement CreateInstance() => owner._create();

        public override void Apply(FrameworkElement instance)
        {
        }

        public override string BuildSnippet() => owner._code;
    }
}
