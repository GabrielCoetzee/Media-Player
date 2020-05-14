using Generic.Configuration.Abstract;
using MediaPlayer.ApplicationSettings.Config;
using MediaPlayer.Theming;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MediaPlayer.ApplicationSettings
{
    public class SettingsProvider : ISettingsProvider
    {
        #region Interface Implementations

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        readonly IWritableOptions<Settings> _options;
        readonly IThemeSelector _themeSelector;

        public SettingsProvider(IWritableOptions<Settings> options, IThemeSelector themeSelector)
        {
            _options = options;

            _selectedOpacity = _options.Value.SelectedOpacity;
            _selectedTheme = _options.Value.SelectedTheme;

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
                opt.SelectedOpacity = _selectedOpacity;
                opt.SelectedTheme = _selectedTheme;
            });
        }

        #endregion

    }
}
