using Generic.PropertyNotify;
using System.ComponentModel.Composition;

namespace MediaPlayer.ViewModel.ViewModels
{
    [Export]
    public class BusyViewModel : NotifyPropertyChanged
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

        public void InitialStartupState()
        {
            IsLoading = false;
            MediaListTitle = string.Empty;
        }

        public void MediaListLoading()
        {
            IsLoading = true;
            MediaListTitle = "Media List Loading...";
        }

        public void MediaListPopulated()
        {
            IsLoading = false;
            MediaListTitle = "Media List";
        }

        public void UpdatingMetadata()
        {
            IsLoading = true;
            MediaListTitle = "Updating Metadata...";
        }

        public void SavingChanges()
        {
            IsLoading = true;
            MediaListTitle = "Saving Changes...";
        }
    }
}
