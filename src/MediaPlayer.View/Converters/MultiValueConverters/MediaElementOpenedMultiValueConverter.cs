using MediaPlayer.ViewModel.ConverterObject;
using MediaPlayer.ViewModel.ViewModels;
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
            if (values == null || values.Length < 3)
                return null;

            return new MediaOpenedConverterModel
            {
                MediaElement = values[0] as MediaElement,
                QueueViewModel = values[1] as QueueViewModel,
                MediaControlsViewModel = values[2] as MediaControlsViewModel
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
