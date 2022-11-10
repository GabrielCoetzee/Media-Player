using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Generic.PropertyNotify;
using MediaPlayer.Settings.Abstract;
using MediaPlayer.Settings.Config;

namespace MediaPlayer.Settings.Concrete
{
    [Export(typeof(IThemeManager))]
    public class ThemeManager : NotifyPropertyChanged, IThemeManager
    {
        readonly ThemeSettings _themeSettings;

        [ImportingConstructor]
        public ThemeManager(ThemeSettings themeSettings)
        {
            _themeSettings = themeSettings;
        }

        public void ChangeTheme(string baseColor, string accent)
        {
            Application.Current.Resources.MergedDictionaries.Remove(Application.Current.Resources.MergedDictionaries.Last());

            var resourceDictionary = new Uri($"pack://application:,,,/MahApps.Metro;component/Styles/Themes/{baseColor}.{accent}.xaml", UriKind.RelativeOrAbsolute);

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = resourceDictionary });
        }

        public void ChangeOpacity(double opacity)
        {
            Application.Current.MainWindow.Background.Opacity = opacity;
        }

        public string BackgroundColor => BaseColor == "Dark" ? "Black" : "White";
        public string ForegroundColor => BaseColor == "Dark" ? "White" : "Black";

        public string BaseColor
        {
            get => _themeSettings.BaseColor;
            set
            {
                _themeSettings.BaseColor = value;
                OnPropertyChanged(nameof(BaseColor));
                OnPropertyChanged(nameof(BackgroundColor));
                OnPropertyChanged(nameof(ForegroundColor));

                ChangeTheme(value, Accent);
            }
        }

        public string Accent
        {
            get => _themeSettings.Accent;
            set
            {
                _themeSettings.Accent = value;
                OnPropertyChanged(nameof(Accent));

                ChangeTheme(BaseColor, value);
            }
        }

        public decimal Opacity
        {
            get => _themeSettings.Opacity;
            set
            {
                _themeSettings.Opacity = value;
                OnPropertyChanged(nameof(Opacity));

                ChangeOpacity((double)value);
            }
        }

        public void SaveSettings()
        {
            _themeSettings.Save();
        }
    }
}
