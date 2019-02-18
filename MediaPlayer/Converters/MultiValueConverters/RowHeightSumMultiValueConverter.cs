using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace MediaPlayer.Converters
{
    public class RowHeightSumMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Cast<double>().Sum();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
