using MediaPlayer.View.Models;
using MediaPlayer.ViewModel;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MediaPlayer.View.Converters
{
    internal class MediaElementOpenedMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var value = values.FirstOrDefault(o => (o != null && o != DependencyProperty.UnsetValue));

            if (value == null)
                return null;

            return new MediaOpenedModel() { MediaElement = values[0] as MediaElement, ViewModelMediaPlayer = values[1] as ViewModelMediaPlayer };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private BitmapImage ToImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}
