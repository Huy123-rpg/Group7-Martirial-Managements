using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF.PresentationLayer.Helpers.Converters;

/// <summary>
/// Trả về Visible nếu value không null / không rỗng; ngược lại Collapsed.
/// </summary>
public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s)
            return string.IsNullOrWhiteSpace(s) ? Visibility.Collapsed : Visibility.Visible;

        return value != null ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
