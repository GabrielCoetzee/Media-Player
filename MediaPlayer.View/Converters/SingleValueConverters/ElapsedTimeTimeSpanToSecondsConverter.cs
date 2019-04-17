using System;
using System.Globalization;
using System.Windows.Data;

namespace MediaPlayer.View.Converters
{
    internal class ElapsedTimeTimeSpanToSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                var val = value is TimeSpan ? (TimeSpan) value : new TimeSpan();

                return val.TotalSeconds;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                return TimeSpan.FromSeconds((double)value);
            }

            return null;
        }
    }
}
