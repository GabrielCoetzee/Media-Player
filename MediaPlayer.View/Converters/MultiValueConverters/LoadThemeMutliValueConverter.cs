using MediaPlayer.Settings.ViewModels;
using MediaPlayer.ViewModel;
using MediaPlayer.ViewModel.ConverterObject;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MediaPlayer.View.Converters
{
    internal class LoadThemeMutliValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
                return null;

            return new LoadThemeConverterModel() { ComboBox = values[0] as ComboBox, ThemeViewModel = values[1] as ThemeViewModel };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
