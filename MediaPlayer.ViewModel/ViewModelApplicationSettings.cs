using System.ComponentModel;
using System.Runtime.CompilerServices;
using MediaPlayer.ApplicationSettings.Settings_Provider;
using MediaPlayer.Model;
using MediaPlayer.Model.Annotations;
using Ninject;

namespace MediaPlayer.ViewModel
{
    public class ViewModelApplicationSettings : INotifyPropertyChanged
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

        private ModelApplicationSettings _modelApplicationSettings;

        #endregion

        #region Properties

        public ModelApplicationSettings ModelApplicationSettings
        {
            get => _modelApplicationSettings;
            set
            {
                _modelApplicationSettings = value;
                OnPropertyChanged(nameof(ModelApplicationSettings));
            }
        }

        public ISettingsProvider SettingsProvider { get; }

        #endregion

        #region Constructor

        [Inject]
        public ViewModelApplicationSettings(ISettingsProvider settingsProvider)
        {
            this.SettingsProvider = settingsProvider;

            InitializeModelInstance();
        }

        #endregion

        #region Initialization

        private void InitializeModelInstance()
        {
            ModelApplicationSettings = new ModelApplicationSettings(this.SettingsProvider)
            {
                SelectedTheme = this.SettingsProvider.SelectedTheme,
                Opacity = this.SettingsProvider.Opacity
            };
        }

        #endregion
    }
}
