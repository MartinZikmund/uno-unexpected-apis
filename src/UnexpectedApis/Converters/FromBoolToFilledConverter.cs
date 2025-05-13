using Microsoft.UI;
using Microsoft.UI.Xaml.Data;

namespace UnexpectedApis.Converters;

public class FromBoolToFilledConverter : IValueConverter
{
    public string ResourceName { get; set; }

    public object? Convert(object value, Type targetType, object parameter, string language) => value is bool x
        ? (x ? Application.Current.Resources[ResourceName] : new SolidColorBrush(Colors.Transparent))
        : null;

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException("Only one-way conversion is supported.");
}
