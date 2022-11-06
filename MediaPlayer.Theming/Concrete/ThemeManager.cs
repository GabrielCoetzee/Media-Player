using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using MediaPlayer.Theming.Abstract;

namespace MediaPlayer.Theming.Concrete
{
    [Export(typeof(IThemeManager))]
    public class ThemeManager : IThemeManager
    {
        public void ChangeTheme(string baseColor, string accent)
        {
            Application.Current.Resources.MergedDictionaries.Remove(Application.Current.Resources.MergedDictionaries[2]);

            var resourceDictionary = new Uri($"pack://application:,,,/MahApps.Metro;component/Styles/Themes/{baseColor}.{accent}.xaml", UriKind.RelativeOrAbsolute);

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = resourceDictionary });
        }

        public void ChangeOpacity(double opacity)
        {
            Application.Current.MainWindow.Background.Opacity = opacity;
        }
    }
}
