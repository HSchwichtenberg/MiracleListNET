using ITVisions;
using System.Globalization;

namespace MiracleList_MAUI.Converters
{
    public class TaskStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                return date.Date < DateTime.Now.Date ? "overdue" : "open";
            }
            return "open";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}