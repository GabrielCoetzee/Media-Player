using System;
using System.Globalization;
using System.Windows.Data;
using MediaPlayer.Common.Enumerations;

namespace MediaPlayer.View.Converters
{
    internal class MediaVolumeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not VolumeLevel volumeLevel)
                return null;

            return volumeLevel == VolumeLevel.Full ? 100 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
