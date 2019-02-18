using System.Linq;
using System.Windows;
using MahApps.Metro;
using MediaPlayer.ApplicationSettings.Interfaces;
using StringCollection = System.Collections.Specialized.StringCollection;

namespace MediaPlayer.ApplicationSettings.Interface_Implementations
{
    public class ApplicationSettings : IExposeApplicationSettings
    {
        private string _selectedTheme = Settings.Properties.Settings.Default[nameof(SelectedTheme)].ToString();
        private decimal _opacity = (decimal)Settings.Properties.Settings.Default[nameof(Opacity)];

        public string[] SupportedFormats
        {
            get
            {
                var supportedFormats = (StringCollection)Settings.Properties.Settings.Default[nameof(SupportedFormats)];

                return supportedFormats.Cast<string>().ToArray<string>();
            }
        }

        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;

                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(SelectedTheme), ThemeManager.GetAppTheme("BaseDark"));
            }
        }

        public decimal Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                Application.Current.MainWindow.Background.Opacity = (double)Opacity;
            }
        } 

        public void SaveSettings()
        {
            Settings.Properties.Settings.Default[nameof(SelectedTheme)] = _selectedTheme;
            Settings.Properties.Settings.Default[nameof(Opacity)] = _opacity;

            Settings.Properties.Settings.Default.Save();
        }
    }
}
