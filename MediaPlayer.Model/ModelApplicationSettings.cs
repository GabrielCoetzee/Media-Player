using System.ComponentModel;
using System.Runtime.CompilerServices;
using MediaPlayer.ApplicationSettings.SettingsProvider;
using MediaPlayer.Model.Annotations;

namespace MediaPlayer.Model
{
    public class ModelApplicationSettings : INotifyPropertyChanged
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

        private string _selectedTheme;
        private decimal _opacity;

        #endregion

        #region Properties

        public string[] SupportedFileFormats => this.SettingsProvider.SupportedFileFormats;

        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                OnPropertyChanged(nameof(SelectedTheme));

                this.SettingsProvider.SelectedTheme = value;
            }
        }

        public decimal Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                OnPropertyChanged(nameof(Opacity));

                this.SettingsProvider.Opacity = value;
            }
        }

        #endregion

        #region Properties

        public ISettingsProvider SettingsProvider { get; set; }

        #endregion

        #region Constructor

        public ModelApplicationSettings(ISettingsProvider settingsProvider)
        {
            this.SettingsProvider = settingsProvider;
        }

        #endregion

    }
}
