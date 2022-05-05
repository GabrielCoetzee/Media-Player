using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MediaPlayer.View.Converters
{
    internal class PlayPauseButtonBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not MediaState mediaState)
                return null;

            var pauseImage = Application.Current.MainWindow?.Resources["ResourcePauseButtonImage"] as ImageBrush;
            var playImage = Application.Current.MainWindow?.Resources["ResourcePlayButtonImage"] as ImageBrush;

            return mediaState == MediaState.Play ? pauseImage : playImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
