using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MediaPlayer.MVVM.Models.Base_Types;

namespace MediaPlayer.Helpers.Custom_Converters
{
    public class MediaItemToVisibilityConverter : IValueConverter
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
