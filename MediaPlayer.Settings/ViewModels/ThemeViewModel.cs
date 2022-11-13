using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Generic.PropertyNotify;
using MediaPlayer.Settings.Config;

namespace MediaPlayer.Settings.ViewModels
{
    [Export]
    public class ThemeViewModel : NotifyPropertyChanged
    {
        readonly ThemeSettings _themeSettings;

        [ImportingConstructor]
        public ThemeViewModel(ThemeSettings themeSettings)
        {
            _themeSettings = themeSettings;
        }

        public void ChangeTheme()
        {
            Application.Current.Resources.MergedDictionaries.Remove(Application.Current.Resources.MergedDictionaries.Last());

            var resourceDictionary = new Uri($"pack://application:,,,/MahApps.Metro;component/Styles/Themes/{BaseColor}.{Accent}.xaml", UriKind.RelativeOrAbsolute);

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = resourceDictionary });
        }

        public void ChangeOpacity()
        {
            Application.Current.MainWindow.Background.Opacity = (double)Opacity;
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

                ChangeTheme();
            }
        }

        public string Accent
        {
            get => _themeSettings.Accent;
            set
            {
                _themeSettings.Accent = value;
                OnPropertyChanged(nameof(Accent));

                ChangeTheme();
            }
        }

        public decimal Opacity
        {
            get => _themeSettings.Opacity;
            set
            {
                _themeSettings.Opacity = value;
                OnPropertyChanged(nameof(Opacity));

                ChangeOpacity();
            }
        }

        public void SaveSettings()
        {
            _themeSettings.Save();
        }
    }
}
