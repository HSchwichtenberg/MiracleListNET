using Microsoft.UI.Xaml.Data;
using System;

namespace MiracleList_WinUI.Converters
{
    public class DecimalToDoubleConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            return value is null ? double.NaN : (double)(decimal)value;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is double.NaN ? null : (decimal)(double)value;

        }
    }
}
