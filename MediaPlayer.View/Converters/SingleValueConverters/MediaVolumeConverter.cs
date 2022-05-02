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
            if (value is VolumeLevel volumeLevel)
            {
                if (volumeLevel == VolumeLevel.Full)
                    return 100;

                return 0;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
