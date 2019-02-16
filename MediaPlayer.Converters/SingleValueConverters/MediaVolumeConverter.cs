using System;
using System.Globalization;
using System.Windows.Data;
using MediaPlayer.BusinessEntities;

namespace MediaPlayer.Converters
{
    public class MediaVolumeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VolumeLevel volumeLevel)
            {
                if (volumeLevel == VolumeLevel.Full)
                    return 100;
                else if (volumeLevel == VolumeLevel.Mute)
                    return 0;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VolumeLevel volumeLevel)
            {
                if (volumeLevel == VolumeLevel.Full)
                    return 100;
                else if (volumeLevel == VolumeLevel.Mute)
                    return 0;
            }

            return null;
        }
    }
}
