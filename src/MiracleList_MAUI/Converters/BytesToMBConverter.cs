using ITVisions;
using System.Globalization;

namespace MiracleList_MAUI.Converters
{
    public class BytesToMBConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long bytes)
            {
                return MathUtil.MB(bytes);
            }
            return "0 MB";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}