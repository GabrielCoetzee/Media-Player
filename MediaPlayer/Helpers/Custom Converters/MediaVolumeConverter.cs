using System;
using System.Globalization;
using System.Windows.Data;

namespace MediaPlayer.Helpers.Custom_Converters
{
    public class MediaVolumeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CustomEnums.VolumeLevel volumeLevel)
            {
                if (volumeLevel == CustomEnums.VolumeLevel.FullVolume)
                    return 100;
                else if (volumeLevel == CustomEnums.VolumeLevel.Mute)
                    return 0;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CustomEnums.VolumeLevel volumeLevel)
            {
                if (volumeLevel == CustomEnums.VolumeLevel.FullVolume)
                    return 100;
                else if (volumeLevel == CustomEnums.VolumeLevel.Mute)
                    return 0;
            }

            return null;
        }
    }
}
