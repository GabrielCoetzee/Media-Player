using System;
using System.Globalization;
using System.Windows.Data;
using MediaPlayer.Common.Enumerations;
using Wpf.Ui.Controls;

namespace MediaPlayer.View.Converters
{
    public class DwmBackdropTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                DwmBackdropType.Auto    => WindowBackdropType.Auto,
                DwmBackdropType.None    => WindowBackdropType.None,
                DwmBackdropType.Mica    => WindowBackdropType.Mica,
                DwmBackdropType.Acrylic => WindowBackdropType.Acrylic,
                DwmBackdropType.Tabbed  => WindowBackdropType.Tabbed,
                _                       => WindowBackdropType.Auto
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
