using System.Linq;
using System.Windows;
using MahApps.Metro;
using StringCollection = System.Collections.Specialized.StringCollection;

namespace MediaPlayer.Settings
{
    public class ApplicationSettings : IExposeApplicationSettings
    {
        private ApplicationSettings()
        {
        }

        private static readonly object padlock = new object();
        private static ApplicationSettings _instance;

        public static ApplicationSettings Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                        _instance = new ApplicationSettings();

                    return _instance;
                }
            }
        }

        private string _selectedTheme = Properties.Settings.Default[nameof(SelectedTheme)].ToString();
        private decimal _opacity = (decimal)Properties.Settings.Default[nameof(Opacity)];

        public string[] SupportedFormats
        {
            get
            {
                var supportedFormats = (StringCollection)Properties.Settings.Default[nameof(SupportedFormats)];

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
            Properties.Settings.Default[nameof(SelectedTheme)] = _selectedTheme;
            Properties.Settings.Default[nameof(Opacity)] = _opacity;

            Properties.Settings.Default.Save();
        }
    }
}
