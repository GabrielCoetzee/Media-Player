using Generic.DependencyInjection;
using Generic.PropertyNotify;
using MediaPlayer.Settings.Config;
using MediaPlayer.Theming.Abstract;
using System.ComponentModel.Composition;

namespace MediaPlayer.Settings.Concrete
{
    [Export(typeof(ISettingsManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SettingsManager : PropertyNotifyBase, ISettingsManager
    {
        readonly ApplicationSettings _applicationSettings;
        readonly IThemeManager _themeManager;

        [ImportingConstructor]
        public SettingsManager(ApplicationSettings applicationSettings,
            IThemeManager themeManager)
        {
            _applicationSettings = applicationSettings;
            _themeManager = themeManager;

            _selectedOpacity = _applicationSettings.Opacity;
            _selectedAccent = _applicationSettings.Accent;
        }

        private decimal _selectedOpacity;
        private string _selectedAccent;

        public string SelectedAccent
        {
            get => _selectedAccent;
            set
            {
                _selectedAccent = value;
                OnPropertyChanged(nameof(SelectedAccent));

                _applicationSettings.Accent = value;
                _themeManager.ChangeAccent(value);
            }
        }

        public decimal SelectedOpacity
        {
            get => _selectedOpacity;
            set
            {
                _selectedOpacity = value;
                OnPropertyChanged(nameof(SelectedOpacity));

                _applicationSettings.Opacity = value;
                _themeManager.ChangeOpacity((double)value);
            }
        }

        public void SaveSettings()
        {
            _applicationSettings.Save();

            //_options.Update(opt => {
            //    opt.Opacity = _selectedOpacity;
            //    opt.Accent = _selectedAccent;
            //});
        }

    }
}
