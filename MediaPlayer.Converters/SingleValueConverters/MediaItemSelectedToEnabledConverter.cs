using System;
using System.Globalization;
using System.Windows.Data;
using MediaPlayer.Common;

namespace MediaPlayer.Converters
{
    public class MediaItemSelectedToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MediaItem mediaItem)
            {
                return mediaItem != null;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
