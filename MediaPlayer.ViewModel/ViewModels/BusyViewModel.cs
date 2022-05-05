using Generic.PropertyNotify;

namespace MediaPlayer.ViewModel.ViewModels
{
    public class BusyViewModel : PropertyNotifyBase
    {
        private bool? _isLoadingMediaItems;
        private string _mediaListTitle = string.Empty;

        public bool? IsLoadingMediaItems
        {
            get => _isLoadingMediaItems;
            set
            {
                _mediaListTitle = value == true ? "Media List Loading..." : "Media List";

                _isLoadingMediaItems = value;
                OnPropertyChanged(nameof(IsLoadingMediaItems));
                OnPropertyChanged(nameof(MediaListTitle));
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
