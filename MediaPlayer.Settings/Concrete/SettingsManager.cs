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
        public Configuration Configuration { get; set; }

        [Import]
        public IThemeSelector ThemeSelector { get; set; }

        [ImportingConstructor]
        public SettingsManager()
        {
            MEF.Container?.SatisfyImportsOnce(this);

            _selectedOpacity = Configuration.Opacity;
            _selectedAccent = Configuration.Accent;
        }

        public string[] SupportedFileFormats => Configuration.SupportedFileFormats;

        private decimal _selectedOpacity;
        private string _selectedAccent;

        public string SelectedAccent
        {
            get => _selectedAccent;
            set
            {
                _selectedAccent = value;
                OnPropertyChanged(nameof(SelectedAccent));

                Configuration.Accent = value;
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

                Configuration.Opacity = value;
                ThemeSelector.ChangeOpacity((double)value);
            }
        }

        public void SaveSettings()
        {
            Configuration.Save();

            //_options.Update(opt => {
            //    opt.Opacity = _selectedOpacity;
            //    opt.Accent = _selectedAccent;
            //});
        }

    }
}
