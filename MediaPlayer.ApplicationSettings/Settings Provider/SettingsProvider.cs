using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using MahApps.Metro;
using MediaPlayer.ApplicationSettings.Annotations;

namespace MediaPlayer.ApplicationSettings.Settings_Provider
{
    public class SettingsProvider : ISettingsProvider
    {
        #region Interface Implementations

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Fields

        private string _selectedTheme = Properties.Settings.Default[nameof(SelectedTheme)].ToString();
        private decimal _opacity = (decimal)Properties.Settings.Default[nameof(Opacity)];

        #endregion

        #region Properties
        public string[] SupportedFileFormats => ((StringCollection)Properties.Settings.Default[nameof(SupportedFileFormats)]).Cast<string>().ToArray();

        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                OnPropertyChanged(nameof(SelectedTheme));

                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(value), ThemeManager.GetAppTheme("BaseDark"));
            }
        }

        public decimal Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                OnPropertyChanged(nameof(Opacity));

                Application.Current.MainWindow.Background.Opacity = (double)value;
            }
        }

        #endregion

        #region Public Methods

        public void SaveSettings()
        {
            Properties.Settings.Default[nameof(SelectedTheme)] = _selectedTheme;
            Properties.Settings.Default[nameof(Opacity)] = _opacity;

            Properties.Settings.Default.Save();
        }

        #endregion
    }
}
