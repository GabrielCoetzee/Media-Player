using Generic.PropertyNotify;
using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.ViewModels;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace MediaPlayer.ViewModel
{
    [Export]
    public class PlayerShellViewModel : NotifyPropertyChanged
    {
        private bool _isLyricsOpen;
        private bool _isQueueOpen = true;
        private bool _isSettingsOpen;

        public bool IsLyricsOpen
        {
            get => _isLyricsOpen;
            set
            {
                _isLyricsOpen = value;
                OnPropertyChanged(nameof(IsLyricsOpen));
            }
        }

        public bool IsQueueOpen
        {
            get => _isQueueOpen;
            set
            {
                _isQueueOpen = value;
                OnPropertyChanged(nameof(IsQueueOpen));
            }
        }

        public bool IsSettingsOpen
        {
            get => _isSettingsOpen;
            set
            {
                _isSettingsOpen = value;
                OnPropertyChanged(nameof(IsSettingsOpen));
            }
        }

        [Import(CommandNames.TopMostGridDragEnter)]
        public ICommand TopMostGridDragEnterCommand { get; set; }

        [Import(CommandNames.TopMostGridDrop)]
        public ICommand TopMostGridDropCommand { get; set; }

        [Import(CommandNames.MainWindowClosing)]
        public ICommand MainWindowClosingCommand { get; set; }

        [Import(CommandNames.ToggleLyrics)]
        public ICommand ToggleLyricsCommand { get; set; }

        [Import(CommandNames.ToggleQueue)]
        public ICommand ToggleQueueCommand { get; set; }

        [Import(CommandNames.Escape)]
        public ICommand EscapeCommand { get; set; }

        [Import]
        public SettingsViewModel SettingsViewModel { get; set; }

        [Import]
        public BusyViewModel BusyViewModel { get; set; }

        [Import]
        public MediaControlsViewModel MediaControlsViewModel { get; set; }

        [Import]
        public QueueViewModel QueueViewModel { get; set; }
    }
}
