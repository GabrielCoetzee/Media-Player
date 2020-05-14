using System.ComponentModel;
using System.Runtime.CompilerServices;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.Model.Annotations;

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

        #region Properties

        public ISettingsProvider SettingsProvider { get; set; }

        #endregion

        #region Constructor

        public ViewModelApplicationSettings(ISettingsProvider settingsProvider)
        {
            SettingsProvider = settingsProvider;
        }

        #endregion
    }
}
