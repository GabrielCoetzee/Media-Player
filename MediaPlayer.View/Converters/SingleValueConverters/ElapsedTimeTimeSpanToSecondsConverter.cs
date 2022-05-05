using System;
using System.Globalization;
using System.Windows.Data;

namespace MediaPlayer.View.Converters
{
    internal class ElapsedTimeTimeSpanToSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not TimeSpan val)
                return null;

            return val.TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double val)
                return null;

            return TimeSpan.FromSeconds((double)value);
        }
    }
}
