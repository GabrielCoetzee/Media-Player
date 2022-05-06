using Generic.Configuration.Abstract;
using Generic.PropertyNotify;
using MediaPlayer.ApplicationSettings.Config;
using MediaPlayer.Theming;
using MediaPlayer.Theming.Abstract;

namespace MediaPlayer.ApplicationSettings.Concrete
{
    public class SettingsProviderViewModel : PropertyNotifyBase, ISettingsProviderViewModel
    {
        readonly IWritableOptions<Settings> _options;
        readonly IThemeSelector _themeSelector;

        public SettingsProviderViewModel(IWritableOptions<Settings> options, IThemeSelector themeSelector)
        {
            _options = options;

            _selectedOpacity = _options.Value.Opacity;
            _selectedAccent = _options.Value.Accent;

            _themeSelector = themeSelector;
        }

        public string[] SupportedFileFormats => _options.Value.SupportedFileFormats;

        private decimal _selectedOpacity;
        private string _selectedAccent;

        public string SelectedAccent
        {
            get => _selectedAccent;
            set
            {
                _selectedAccent = value;
                OnPropertyChanged(nameof(SelectedAccent));

                _themeSelector.ChangeAccent(value);
            }
        }

        public decimal SelectedOpacity
        {
            get => _selectedOpacity;
            set
            {
                _selectedOpacity = value;
                OnPropertyChanged(nameof(SelectedOpacity));

                _themeSelector.ChangeOpacity((double)value);
            }
        }

        public void SaveSettings()
        {
            _options.Update(opt => {
                opt.Opacity = _selectedOpacity;
                opt.Accent = _selectedAccent;
            });
        }

    }
}
