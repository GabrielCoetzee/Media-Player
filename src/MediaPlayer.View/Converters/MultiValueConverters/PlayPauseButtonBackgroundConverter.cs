using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MediaPlayer.View.Converters
{
    internal class PlayPauseButtonBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var mediaState = (MediaState) values[0];
            var baseColor = values[1];

            var pauseImage = Application.Current.MainWindow?.Resources[$"ResourcePauseButtonImage_{baseColor}"] as ImageBrush;
            var playImage = Application.Current.MainWindow?.Resources[$"ResourcePlayButtonImage_{baseColor}"] as ImageBrush;

            return mediaState == MediaState.Play ? pauseImage : playImage;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
