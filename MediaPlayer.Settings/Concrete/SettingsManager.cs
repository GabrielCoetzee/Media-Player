using Generic;
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
        [Import]
        public ApplicationSettings ApplicationSettings { get; set; }

        [Import]
        public IThemeSelector ThemeSelector { get; set; }

        [ImportingConstructor]
        public SettingsManager()
        {
            MEF.Container?.SatisfyImportsOnce(this);

            _selectedOpacity = ApplicationSettings.Opacity;
            _selectedAccent = ApplicationSettings.Accent;
        }

        public string[] SupportedFileFormats => ApplicationSettings.SupportedFileFormats;

        private decimal _selectedOpacity;
        private string _selectedAccent;

        public string SelectedAccent
        {
            get => _selectedAccent;
            set
            {
                _selectedAccent = value;
                OnPropertyChanged(nameof(SelectedAccent));

                ApplicationSettings.Accent = value;
                ThemeSelector.ChangeAccent(value);
            }
        }

        public decimal SelectedOpacity
        {
            get => _selectedOpacity;
            set
            {
                _selectedOpacity = value;
                OnPropertyChanged(nameof(SelectedOpacity));

                ApplicationSettings.Opacity = value;
                ThemeSelector.ChangeOpacity((double)value);
            }
        }

        public void SaveSettings()
        {
            ApplicationSettings.Save();

            //_options.Update(opt => {
            //    opt.Opacity = _selectedOpacity;
            //    opt.Accent = _selectedAccent;
            //});
        }

    }
}
