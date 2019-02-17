using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MediaPlayer.BusinessEntities.Objects.Abstract;

namespace MediaPlayer.Converters
{
    public class MediaItemSelectedToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MediaItem mediaItem)
            {
                return mediaItem != null ? Visibility.Visible : Visibility.Hidden;
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
