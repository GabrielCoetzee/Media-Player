using MediaPlayer.ViewModel;
using MediaPlayer.ViewModel.ConverterObject;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MediaPlayer.View.Converters
{
    internal class MediaElementOpenedMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
                return null;

            return new MediaOpenedConverterModel() { MediaElement = values[0] as MediaElement, MainViewModel = values[1] as MainViewModel };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
