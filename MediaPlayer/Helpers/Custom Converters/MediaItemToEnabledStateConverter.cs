using System;
using System.Globalization;
using System.Windows.Data;
using MediaPlayer.MVVM.Models.Base_Types;

namespace MediaPlayer.Helpers.Custom_Converters
{
    public class MediaItemToEnabledStateConverter : IValueConverter
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
