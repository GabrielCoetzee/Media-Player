using System;
using System.ComponentModel;
using System.Windows.Controls;
using MediaPlayer.BusinessEntities.Collections.Derived;
using MediaPlayer.BusinessEntities.Objects.Abstract;
using MediaPlayer.Common.Enumerations;

namespace MediaPlayer.Model
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

        private MediaItem _selectedMediaItem;
        private bool _isLoadingMediaItems;
        private MediaItemObservableCollection _mediaList;
        private TimeSpan _mediaPosition;
        private MediaState _mediaState;
        private VolumeLevel _mediaVolume;
        private double _elapsedTime;
        private bool _isDraggingSeekbarThumb = false;
        private string _elapsedTimeFormatted;
        private bool _isRepeatMediaListEnabled;
        private bool _isShuffled;

        #endregion

        #region Properties

        public MediaItem SelectedMediaItem
        {
            get => _selectedMediaItem;
            set
            {
                _selectedMediaItem = value;
                OnPropertyChanged(nameof(SelectedMediaItem));
            }
        }

        public bool IsLoadingMediaItems
        {
            get => _isLoadingMediaItems;
            set
            {
                _isLoadingMediaItems = value;
                OnPropertyChanged(nameof(IsLoadingMediaItems));
            }
        }

        public MediaItemObservableCollection MediaList
        {
            get => _mediaList;
            set
            {
                _mediaList = value;
                OnPropertyChanged(nameof(MediaList));
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

        public VolumeLevel MediaVolume
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

        public bool IsRepeatMediaListEnabled
        {
            get => _isRepeatMediaListEnabled;
            set
            {
                _isRepeatMediaListEnabled = value;
                OnPropertyChanged(nameof(IsRepeatMediaListEnabled));
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


        #endregion

    }
}
