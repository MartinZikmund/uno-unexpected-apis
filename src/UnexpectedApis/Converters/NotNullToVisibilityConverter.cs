using Microsoft.UI.Xaml.Data;

namespace UnexpectedApis.Converters;

public class NotNullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) => 
        value is null ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException("Only one-way conversion is supported.");
}
