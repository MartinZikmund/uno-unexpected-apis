using System.Diagnostics.CodeAnalysis;
using UnexpectedApis.Core;

namespace UnexpectedApis.Catalog;

public sealed record Category(string Id, string Title, Symbol Icon, string Blurb);

public abstract class Sample
{
    protected Sample(string id, string title, Category category, Symbol icon, string summary)
    {
        Id = id;
        Title = title;
        Category = category;
        Icon = icon;
        Summary = summary;
    }

    public string Id { get; }

    public string Title { get; }

    public Category Category { get; }

    public Symbol Icon { get; }

    public string Summary { get; }

    public List<string> Keywords { get; } = new();

    public List<string> Tips { get; } = new();

    /// <summary>Stretch the control across the stage instead of centering it.</summary>
    public bool Fills { get; protected set; }

    public double StageHeight { get; protected set; } = 300;

    public abstract Uri DocsUri { get; }

    /// <summary>False for API demos: no theme modes, width check or properties panel.</summary>
    public virtual bool IsLiveControl => true;

    public virtual string SnippetLanguage => "XAML";

    public virtual ApiPlatforms Platforms => ApiPlatforms.All;

    public abstract SampleSession CreateSession();
}

/// <summary>The knobs of one open sample page. Every live instance on the stage follows them.</summary>
public abstract class SampleSession
{
    public abstract IReadOnlyList<Knob> Knobs { get; }

    public abstract FrameworkElement CreateInstance();

    public abstract void Apply(FrameworkElement instance);

    public abstract string BuildSnippet();
}

public sealed class Sample<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : Sample
    where T : FrameworkElement
{
    private readonly Func<T> _create;
    private readonly string _tag;
    private readonly List<KnobSpec> _specs = new();
    private readonly List<XamlAttribute> _fixedAttributes = new();
    private readonly List<string> _body = new();
    private string? _rawSnippet;

    public Sample(string id, string title, Category category, Symbol icon, string summary, Func<T> create, string? tag = null)
        : base(id, title, category, icon, summary)
    {
        _create = create;
        _tag = tag ?? typeof(T).Name;
    }

    public override Uri DocsUri =>
        new($"https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/{typeof(T).FullName!.ToLowerInvariant()}");

    public Sample<T> Flag(string name, bool value, Action<T, bool> apply) =>
        Add(() => new BoolKnob(name, value), (c, k) => apply(c, ((BoolKnob)k).Value));

    public Sample<T> Text(string name, string value, Action<T, string> apply, bool alwaysEmit = true) =>
        Add(() => new TextKnob(name, value), (c, k) => apply(c, ((TextKnob)k).Value), alwaysEmit);

    public Sample<T> Number(string name, double value, double minimum, double maximum, double step, Action<T, double> apply) =>
        Add(() => new NumberKnob(name, value, minimum, maximum, step), (c, k) => apply(c, ((NumberKnob)k).Value));

    public Sample<T> Choice(string name, string value, string[] options, Action<T, string> apply, Func<string, string>? toXaml = null) =>
        Add(() => new ChoiceKnob(name, value, options, toXaml), (c, k) => apply(c, ((ChoiceKnob)k).Value));

    public Sample<T> Enum<TEnum>(string name, TEnum value, Action<T, TEnum> apply)
        where TEnum : struct, Enum =>
        Choice(name, value.ToString(), System.Enum.GetNames<TEnum>(), (c, v) => apply(c, System.Enum.Parse<TEnum>(v)));

    /// <summary>An attribute the snippet always shows, for values the sample sets up itself.</summary>
    public Sample<T> Attr(string name, string value)
    {
        _fixedAttributes.Add(new(name, value));
        return this;
    }

    public Sample<T> Body(params string[] lines)
    {
        _body.AddRange(lines);
        return this;
    }

    /// <summary>Replaces the generated snippet when the control is not expressible as one tag.</summary>
    public Sample<T> Snippet(string xaml)
    {
        _rawSnippet = xaml;
        return this;
    }

    public Sample<T> Tip(string tip)
    {
        Tips.Add(tip);
        return this;
    }

    public Sample<T> Find(params string[] keywords)
    {
        Keywords.AddRange(keywords);
        return this;
    }

    public Sample<T> Fill(double height = 360)
    {
        Fills = true;
        StageHeight = height;
        return this;
    }

    public override SampleSession CreateSession() => new Session(this);

    private Sample<T> Add(Func<Knob> create, Action<T, Knob> apply, bool alwaysEmit = false)
    {
        _specs.Add(new(create, apply, alwaysEmit));
        return this;
    }

    private sealed record KnobSpec(Func<Knob> Create, Action<T, Knob> Apply, bool AlwaysEmit);

    private sealed class Session : SampleSession
    {
        private readonly Sample<T> _owner;
        private readonly Knob[] _knobs;
        private T? _pristine;

        public Session(Sample<T> owner)
        {
            _owner = owner;
            _knobs = owner._specs.Select(spec => spec.Create()).ToArray();
        }

        public override IReadOnlyList<Knob> Knobs => _knobs;

        public override FrameworkElement CreateInstance()
        {
            var instance = _owner._create();
            Apply(instance);
            return instance;
        }

        public override void Apply(FrameworkElement instance)
        {
            for (var i = 0; i < _knobs.Length; i++)
            {
                _owner._specs[i].Apply((T)instance, _knobs[i]);
            }
        }

        public override string BuildSnippet()
        {
            if (_owner._rawSnippet is { } raw)
            {
                return raw;
            }

            List<XamlAttribute> attributes = new(_owner._fixedAttributes);
            for (var i = 0; i < _knobs.Length; i++)
            {
                if (_owner._specs[i].AlwaysEmit || !RestatesDefault(_knobs[i]))
                {
                    attributes.Add(new(_knobs[i].Name, _knobs[i].XamlValue));
                }
            }

            return XamlSnippet.Build(_owner._tag, attributes, _owner._body);
        }

        /// <summary>
        /// Asks a pristine control rather than the knob's starting value: samples start from
        /// interesting values, and a snippet must still reproduce them when pasted.
        /// </summary>
        private bool RestatesDefault(Knob knob)
        {
            if (knob is ChoiceKnob { HasXamlMapping: true } || typeof(T).GetProperty(knob.Name) is not { } property)
            {
                return knob.IsDefault;
            }

            _pristine ??= Activator.CreateInstance<T>();
            return XamlDefaults.Matches(property.GetValue(_pristine), knob.XamlValue);
        }
    }
}
