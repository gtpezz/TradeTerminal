using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TradeTerminal.Desktop;

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Если value - bool
        if (value is bool b)
        {
            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        // Если value - число (скидка)
        if (value is decimal discount)
        {
            return discount > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        // Если value - целое число
        if (value is int intValue)
        {
            return intValue > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}