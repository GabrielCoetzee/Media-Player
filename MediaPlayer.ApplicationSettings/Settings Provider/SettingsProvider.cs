using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MediaPlayer.ApplicationSettings.Annotations;
using MediaPlayer.ApplicationSettings.ThemeChanger;
using Ninject;

namespace MediaPlayer.ApplicationSettings.SettingsProvider
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

        [Inject]
        public IThemeChanger ThemeChanger { get; set; }

        public string[] SupportedFileFormats => ((StringCollection)Properties.Settings.Default[nameof(SupportedFileFormats)]).Cast<string>().ToArray();

        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                OnPropertyChanged(nameof(SelectedTheme));

                Properties.Settings.Default[nameof(SelectedTheme)] = _selectedTheme;

                this.ThemeChanger.ChangeTheme(value);
            }
        }

        public decimal Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                OnPropertyChanged(nameof(Opacity));

                Properties.Settings.Default[nameof(Opacity)] = _opacity;

                this.ThemeChanger.ChangeOpacity((double)value);
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
    }
        #endregion
}
