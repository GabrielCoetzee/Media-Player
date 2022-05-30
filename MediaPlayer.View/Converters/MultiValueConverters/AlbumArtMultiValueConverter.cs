using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MediaPlayer.View.Converters
{
    internal class AlbumArtMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var accent = values.Single(x => x?.GetType() == typeof(string));
            var defaultArt = new BitmapImage(new Uri($"../Resources/Default_AlbumArt/{accent}.png", UriKind.Relative));

            var value = values.FirstOrDefault(o => (o != null && o != DependencyProperty.UnsetValue));

            return value switch
            {
                string => defaultArt,
                byte[] => ((byte[])value).Length == 0 ? defaultArt : ToImage((byte[])value),
                _ => null
            };
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
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();

                return image;
            }
        }
    }
}
