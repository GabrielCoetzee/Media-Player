using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.ViewModel;
using MediaPlayer.ViewModel.ConverterObject;
using System;
using System.Globalization;
using System.Windows.Data;

namespace MediaPlayer.View.Converters
{
    internal class RemoveMediaItemMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return null;

            return new RemoveMediaItemConverterModel
            {
                MainViewModel = values[0] as MainViewModel,
                MediaItem = values[1] as MediaItem
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
