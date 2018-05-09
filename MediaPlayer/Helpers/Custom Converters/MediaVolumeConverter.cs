using System;
using System.Globalization;
using System.Windows.Data;

namespace MediaPlayer.Helpers.Custom_Converters
{
    public class MediaVolumeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CustomTypes.VolumeLevel volumeLevel)
            {
                if (volumeLevel == CustomTypes.VolumeLevel.FullVolume)
                    return 100;
                else if (volumeLevel == CustomTypes.VolumeLevel.Mute)
                    return 0;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CustomTypes.VolumeLevel volumeLevel)
            {
                if (volumeLevel == CustomTypes.VolumeLevel.FullVolume)
                    return 100;
                else if (volumeLevel == CustomTypes.VolumeLevel.Mute)
                    return 0;
            }

            return null;
        }
    }
}
