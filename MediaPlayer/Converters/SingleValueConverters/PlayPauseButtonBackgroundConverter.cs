using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MediaPlayer.Helpers.Custom_Converters
{
    public class PlayPauseButtonBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MediaState mediaState)
            {
                if (mediaState == MediaState.Play)
                    return Application.Current.MainWindow?.Resources["ResourcePauseButtonImage"] as ImageBrush;
                else
                    return Application.Current.MainWindow?.Resources["ResourcePlayButtonImage"] as ImageBrush;

            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MediaState mediaState)
            {
                if (mediaState == MediaState.Pause)
                    return Application.Current.MainWindow?.Resources["ResourcePlayButtonImage"] as ImageBrush;
            }

            return null;
        }
    }
}
