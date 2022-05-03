using MediaPlayer.Model.Objects.Base;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MediaPlayer.View.Converters
{
    internal class MediaItemIsSelectedToVisibilityConverter : IValueConverter
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
