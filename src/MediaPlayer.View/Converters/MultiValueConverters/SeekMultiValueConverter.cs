using MediaPlayer.ViewModel.ConverterObject;
using MediaPlayer.ViewModel.ViewModels;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MediaPlayer.View.Converters
{
    internal class SeekMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return null;

            return new SeekConverterModel
            {
                MediaControlsViewModel = values[0] as MediaControlsViewModel,
                Seekbar = values[1] as Slider
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
