using Generic.DependencyInjection;
using Generic.PropertyNotify;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.ViewModel.Commands.Abstract;
using MediaPlayer.ViewModel.Commands.Concrete;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.ViewModels
{
    [Export]
    public class MediaControlsViewModel : PropertyNotifyBase
    {
        private TimeSpan _mediaElementPosition;
        private MediaState _mediaState = MediaState.Pause;
        private VolumeLevel _mediaVolume = VolumeLevel.Full;
        private bool _isUserDraggingSeekbarThumb;
        private bool _isRepeatEnabled;
        private bool _isShuffled;

        public TimeSpan MediaElementPosition
        {
            get => _mediaElementPosition;
            set
            {
                _mediaElementPosition = value;

                OnPropertyChanged(nameof(MediaElementPosition));
            }
        }

        public MediaState MediaState
        {
            get => _mediaState;
            set
            {
                _mediaState = value;
                OnPropertyChanged(nameof(MediaState));
            }
        }

        public VolumeLevel MediaVolume
        {
            get => _mediaVolume;
            set
            {
                _mediaVolume = value;
                OnPropertyChanged(nameof(MediaVolume));
            }
        }

        public bool IsUserDraggingSeekbarThumb
        {
            get => _isUserDraggingSeekbarThumb;
            set
            {
                _isUserDraggingSeekbarThumb = value;
                OnPropertyChanged(nameof(IsUserDraggingSeekbarThumb));
            }
        }

        public bool IsRepeatEnabled
        {
            get => _isRepeatEnabled;
            set
            {
                _isRepeatEnabled = value;
                OnPropertyChanged(nameof(IsRepeatEnabled));
            }
        }

        public bool IsShuffled
        {
            get => _isShuffled;
            set
            {
                _isShuffled = value;
                OnPropertyChanged(nameof(IsShuffled));
            }
        }

        [Import(CommandNames.Shuffle)]
        public ICommand ShuffleCommand { get; set; }

        [Import(CommandNames.PlayPause)]
        public ICommand PlayPauseCommand { get; set; }

        [Import(CommandNames.Mute)]
        public ICommand MuteCommand { get; set; }

        [Import(CommandNames.PreviousTrack)]
        public ICommand PreviousTrackCommand { get; set; }

        [Import(CommandNames.Stop)]
        public ICommand StopCommand { get; set; }

        [Import(CommandNames.Repeat)]
        public ICommand RepeatMediaListCommand { get; set; }

        [Import(CommandNames.ClearList)]
        public ICommand ClearMediaListCommand { get; set; }

        [Import(CommandNames.StartedDragging)]
        public ICommand SeekbarThumbStartedDraggingCommand { get; set; }

        [Import(CommandNames.CompletedDragging)]
        public ICommand SeekbarThumbCompletedDraggingCommand { get; set; }

        [Import(CommandNames.NextTrack)]
        public ICommand NextTrackCommand { get; set; }

        [Import(CommandNames.AddMedia)]
        public ICommand AddMediaCommand { get; set; }

        [Import]
        public ISeekbarPreviewMouseUpCommand SeekbarPreviewMouseUpCommand { get; set; }

        public MediaControlsViewModel()
        {
            MEF.Container?.SatisfyImportsOnce(this);

            SeekbarPreviewMouseUpCommand.ChangeMediaPosition += SeekbarPreviewMouseUpCommand_ChangeMediaPosition;
        }

        private void SeekbarPreviewMouseUpCommand_ChangeMediaPosition(object sender, SliderPositionEventArgs e)
        {
            MediaElementPosition = TimeSpan.FromSeconds(e.Position);
        }

        public void PlayMedia()
        {
            MediaState = MediaState.Play;
        }

        public void PauseMedia()
        {
            MediaState = MediaState.Pause;
        }

        public void StopMedia()
        {
            MediaState = MediaState.Stop;
        }
    }
}
