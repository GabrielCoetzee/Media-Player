using Generic.PropertyNotify;
using MediaPlayer.AudioEngine.Abstract;
using MediaPlayer.AudioEngine.Enumerations;
using MediaPlayer.AudioEngine.Events;
using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.Events;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.ViewModels
{
    [Export]
    public class MediaControlsViewModel : NotifyPropertyChanged, IPartImportsSatisfiedNotification
    {
        private PlaybackState _playbackState = PlaybackState.Stopped;
        private double _mediaVolume = 1.0;
        private bool _isRepeatEnabled;
        private bool _isShuffled;

        public void Seek(TimeSpan position) => AudioEngine?.SeekTo(position);

        public PlaybackState PlaybackState
        {
            get => _playbackState;
            private set
            {
                if (_playbackState == value)
                    return;

                _playbackState = value;
                OnPropertyChanged(nameof(PlaybackState));
            }
        }

        public double MediaVolume
        {
            get => _mediaVolume;
            set
            {
                _mediaVolume = Math.Clamp(value, 0.0, 1.0);
                OnPropertyChanged(nameof(MediaVolume));
                OnPropertyChanged(nameof(IsMuted));

                AudioEngine?.Volume = _mediaVolume;
            }
        }

        public bool IsMuted => _mediaVolume <= 0.0;

        public bool IsUserDraggingSeekbarThumb { get; set; }

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

        [Import(CommandNames.StartedDragging)]
        public ICommand SeekbarThumbStartedDraggingCommand { get; set; }

        [Import(CommandNames.CompletedDragging)]
        public ICommand SeekbarThumbCompletedDraggingCommand { get; set; }

        [Import(CommandNames.NextTrack)]
        public ICommand NextTrackCommand { get; set; }

        [Import(CommandNames.SeekbarPreviewMouseUp)]
        public ICommand SeekbarPreviewMouseUpCommand { get; set; }

        [Import]
        public IAudioEngine AudioEngine { get; set; }

        [Import]
        public QueueViewModel QueueViewModel { get; set; }

        public void OnImportsSatisfied()
        {
            WireQueueViewModel();
            WireAudioEngine();
        }

        private void WireQueueViewModel()
        {
            if (QueueViewModel == null)
                return;

            QueueViewModel.SelectedMediaItemChanged += QueueViewModel_SelectedMediaItemChanged;
        }

        private void WireAudioEngine()
        {
            if (AudioEngine == null)
                return;

            AudioEngine.Volume = _mediaVolume;
            AudioEngine.StateChanged += AudioEngine_StateChanged;
            AudioEngine.PositionChanged += AudioEngine_PositionChanged;
            AudioEngine.DurationDiscovered += AudioEngine_DurationDiscovered;
            AudioEngine.TrackEnded += AudioEngine_TrackEnded;
        }

        public void TogglePause() => AudioEngine?.TogglePause();

        public void Stop() => AudioEngine?.Stop();

        public void Play(string path)
        {
            if (AudioEngine == null || string.IsNullOrWhiteSpace(path))
                return;

            if (path == AudioEngine.CurrentTrackPath)
                return;

            AudioEngine.Play(path);
        }

        private void QueueViewModel_SelectedMediaItemChanged(object sender, SelectedMediaItemChangedEventArgs e)
        {
            if (e.SelectedMediaItem == null)
            {
                Stop();
                return;
            }

            Play(e.SelectedMediaItem.FilePath?.LocalPath);
        }

        private void AudioEngine_StateChanged(object sender, PlaybackStateChangedEventArgs e)
        {
            PlaybackState = e.State;
        }

        private void AudioEngine_PositionChanged(object sender, PlaybackPositionChangedEventArgs e)
        {
            if (IsUserDraggingSeekbarThumb || (QueueViewModel?.SelectedMediaItem) == null)
                return;

            QueueViewModel.SelectedMediaItem.ElapsedTime = e.Position;
        }

        private void AudioEngine_DurationDiscovered(object sender, DurationDiscoveredEventArgs e)
        {
            var selected = QueueViewModel?.SelectedMediaItem;

            if ((selected?.FilePath?.LocalPath) != e.Path)
                return;

            selected.Duration = e.Duration;
        }

        private void AudioEngine_TrackEnded(object sender, TrackEndedEventArgs e)
        {
            if (NextTrackCommand?.CanExecute(this) == true)
            {
                NextTrackCommand.Execute(this);
                return;
            }

            AudioEngine?.Stop();
        }

    }
}
