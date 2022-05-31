using Generic.PropertyNotify;
using System.ComponentModel.Composition;

namespace MediaPlayer.ViewModel.ViewModels
{
    [Export]
    public class BusyViewModel : PropertyNotifyBase
    {
        private bool _isLoading;
        private string _mediaListTitle;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public string MediaListTitle 
        {
            get => _mediaListTitle;
            set
            {
                _mediaListTitle = value;
                OnPropertyChanged(nameof(MediaListTitle));
            }
        }
    }
}
