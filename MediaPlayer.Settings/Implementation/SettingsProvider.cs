using Generic.Configuration.Abstract;
using Generic.PropertyNotify;
using MediaPlayer.ApplicationSettings.Config;
using MediaPlayer.Theming;

namespace MediaPlayer.ApplicationSettings
{
    public class SettingsProvider : PropertyNotifyBase, ISettingsProvider
    {
        readonly IWritableOptions<Settings> _options;
        readonly IThemeSelector _themeSelector;

        public SettingsProvider(IWritableOptions<Settings> options, IThemeSelector themeSelector)
        {
            _options = options;

            _selectedOpacity = _options.Value.Opacity;
            _selectedTheme = _options.Value.Theme;

            _themeSelector = themeSelector;
        }

        #region Fields

        private decimal _selectedOpacity;
        private string _selectedTheme;

        #endregion

        #region Properties

        public string[] SupportedFileFormats => this._options.Value.SupportedFileFormats;

        public string SelectedAccent
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                OnPropertyChanged(nameof(SelectedAccent));

                this._themeSelector.ChangeAccent(value);
            }
        }

        public decimal SelectedOpacity
        {
            get => _selectedOpacity;
            set
            {
                _selectedOpacity = value;
                OnPropertyChanged(nameof(SelectedOpacity));

                this._themeSelector.ChangeOpacity((double)value);
            }
        }

        public void SaveSettings()
        {
            this._options.Update(opt => {
                opt.Opacity = _selectedOpacity;
                opt.Theme = _selectedTheme;
            });
        }

        #endregion

    }
}
