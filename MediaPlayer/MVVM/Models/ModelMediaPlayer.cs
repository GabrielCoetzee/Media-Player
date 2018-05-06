using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using MediaPlayer.Annotations;
using MediaPlayer.Objects;

namespace MediaPlayer.MVVM.Models
{
    public class ModelMediaPlayer : INotifyPropertyChanged
    {
        #region Interface Implemenations

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion

        #region Fields

        private Mp3 _currentTrack;
        private ObservableCollection<Mp3> _trackList;
        private TimeSpan _mediaPosition;
        private MediaState _mediaState;
        private Constants.VolumeLevel _mediaVolume;
        private double _elapsedTime;
        private bool _isDraggingSeekbarThumb = false;
        private string _elapsedTimeFormatted;
        private string _windowTitle;
        private bool _isRepeatMediaListEnabled;
        private bool _isShuffleMediaListEnabled;

        #endregion

        #region Properties

        [CanBeNull]
        public Mp3 CurrentTrack
        {
            get => _currentTrack;
            set
            {
                if (_currentTrack != null && (_currentTrack?.Id != value.Id))
                {
                    _currentTrack.IsSelected = false;
                }

                value.IsSelected = true;
                _currentTrack = value;

                SetMainWindowTitle();

                OnPropertyChanged(nameof(CurrentTrack));
            }
        } 

        public ObservableCollection<Mp3> TrackList
        {
            get => _trackList;
            set
            {
                _trackList = value;
                OnPropertyChanged(nameof(TrackList));
            }
        }

        public TimeSpan MediaPosition
        {
            set
            {
                _mediaPosition = value;

                ElapsedTime = _mediaPosition.TotalSeconds;
                ElapsedTimeFormatted = $"{_mediaPosition.Minutes:00}:{_mediaPosition.Seconds:00;00}";

                OnPropertyChanged(nameof(MediaPosition));
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

        public Constants.VolumeLevel MediaVolume
        {
            get => _mediaVolume;
            set
            {
                _mediaVolume = value;
                OnPropertyChanged(nameof(MediaVolume));
            }
        }

        public double ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = value;

                OnPropertyChanged(nameof(ElapsedTime));
                OnPropertyChanged(nameof(ElapsedTimeFormatted));
            }
        }

        public string ElapsedTimeFormatted
        {
            get => _elapsedTimeFormatted;
            set
            {
                _elapsedTimeFormatted = value;
                OnPropertyChanged(nameof(ElapsedTimeFormatted));
            }
        }

        public bool IsDraggingSeekbarThumb
        {
            get => _isDraggingSeekbarThumb;
            set
            {
                _isDraggingSeekbarThumb = value;
                OnPropertyChanged(nameof(IsDraggingSeekbarThumb));
            }
        }

        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                _windowTitle = value;
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        public bool IsRepeatMediaListEnabled
        {
            get => _isRepeatMediaListEnabled;
            set
            {
                _isRepeatMediaListEnabled = value;
                OnPropertyChanged(nameof(IsRepeatMediaListEnabled));
            }
        }

        public bool IsShuffleMediaListEnabled
        {
            get => _isShuffleMediaListEnabled;
            set
            {
                _isShuffleMediaListEnabled = value;
                OnPropertyChanged(nameof(IsShuffleMediaListEnabled));
            }
        }

        #endregion

        #region Private Methods - Logic

        private void SetMainWindowTitle()
        {
            if (CurrentTrack != null)
            {
                if (!string.IsNullOrEmpty(CurrentTrack.Artist) && !string.IsNullOrEmpty(CurrentTrack.TrackTitle))
                {
                    WindowTitle = $"Now Playing : {CurrentTrack.Artist} - {CurrentTrack.TrackTitle}";
                }
                else
                {
                    WindowTitle = $"Now Playing : {CurrentTrack.FileName}";
                }
            }
        }

        #endregion

    }
}
