using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MediaPlayer.View.Converters
{
    internal class ButtonBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var button = values[0];
            var baseColor = values[1];

            var image = Application.Current.MainWindow?.Resources[$"Resource{button}ButtonImage_{baseColor}"] as ImageBrush;

            return image;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
