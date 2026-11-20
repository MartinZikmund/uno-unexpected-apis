using System.Globalization;

namespace UnexpectedApis.Catalog;

/// <summary>One editable property of a live sample, named exactly as it is in XAML.</summary>
public abstract class Knob : ObservableObject
{
    protected Knob(string name) => Name = name;

    public string Name { get; }

    public event EventHandler? Changed;

    public abstract bool IsDefault { get; }

    /// <summary>The value as it would be written in XAML.</summary>
    public abstract string XamlValue { get; }

    public abstract void Reset();

    protected void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);
}

public sealed partial class BoolKnob : Knob
{
    private readonly bool _default;

    public BoolKnob(string name, bool value) : base(name)
    {
        _default = value;
        Value = value;
    }

    [ObservableProperty]
    public partial bool Value { get; set; }

    public override bool IsDefault => Value == _default;

    public override string XamlValue => Value ? "True" : "False";

    public override void Reset() => Value = _default;

    partial void OnValueChanged(bool value) => RaiseChanged();
}

public sealed partial class TextKnob : Knob
{
    private readonly string _default;

    public TextKnob(string name, string value) : base(name)
    {
        _default = value;
        Value = value;
    }

    [ObservableProperty]
    public partial string Value { get; set; }

    public override bool IsDefault => Value == _default;

    public override string XamlValue => Value;

    public override void Reset() => Value = _default;

    partial void OnValueChanged(string value) => RaiseChanged();
}

public sealed partial class NumberKnob : Knob
{
    private readonly double _default;

    public NumberKnob(string name, double value, double minimum, double maximum, double step) : base(name)
    {
        _default = value;
        Minimum = minimum;
        Maximum = maximum;
        Step = step;
        Value = value;
    }

    public double Minimum { get; }

    public double Maximum { get; }

    public double Step { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(XamlValue))]
    public partial double Value { get; set; }

    public override bool IsDefault => Math.Abs(Value - _default) < 0.0001;

    public override string XamlValue => Value.ToString("0.##", CultureInfo.InvariantCulture);

    public override void Reset() => Value = _default;

    partial void OnValueChanged(double value) => RaiseChanged();
}

public sealed partial class ChoiceKnob : Knob
{
    private readonly string _default;
    private readonly Func<string, string>? _toXaml;

    public ChoiceKnob(string name, string value, IReadOnlyList<string> options, Func<string, string>? toXaml = null) : base(name)
    {
        _default = value;
        _toXaml = toXaml;
        Options = options;
        Value = value;
    }

    public IReadOnlyList<string> Options { get; }

    public bool HasXamlMapping => _toXaml is not null;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedOption))]
    public partial string Value { get; set; }

    /// <summary>ComboBox.SelectedItem is object, and clears to null while the list rebuilds.</summary>
    public object? SelectedOption
    {
        get => Value;
        set
        {
            if (value is string option)
            {
                Value = option;
            }
        }
    }

    public override bool IsDefault => Value == _default;

    public override string XamlValue => _toXaml?.Invoke(Value) ?? Value;

    public override void Reset() => Value = _default;

    partial void OnValueChanged(string value) => RaiseChanged();
}
