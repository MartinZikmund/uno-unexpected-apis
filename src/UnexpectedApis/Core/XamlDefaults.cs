using System.Globalization;

namespace UnexpectedApis.Core;

public static class XamlDefaults
{
    /// <summary>Whether writing <paramref name="xaml"/> would just restate the control's own default.</summary>
    public static bool Matches(object? frameworkValue, string xaml) => frameworkValue switch
    {
        null => xaml.Length == 0,
        bool flag => (flag ? "True" : "False") == xaml,
        double number => double.TryParse(xaml, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
            && Math.Abs(number - parsed) < 0.0001,
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture) == xaml,
        _ => frameworkValue.ToString() == xaml,
    };
}
