using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using Generic.PropertyNotify;
using MediaPlayer.BusinessEntities.Collections;
using MediaPlayer.BusinessEntities.Objects.Base;
using MediaPlayer.Common.Enumerations;

namespace MediaPlayer.Model
{
    public class ModelMediaPlayer : PropertyNotifyBase
    {
        #region Fields

        private readonly Random _randomIdGenerator = new Random();

        private MediaItem _selectedMediaItem;
        private MediaItemObservableCollection _mediaItems = new();
        private bool _isLoadingMediaItems;
        private TimeSpan _mediaPosition;
        private MediaState _mediaState = MediaState.Pause;
        private VolumeLevel _mediaVolume = VolumeLevel.Full;
        private bool _isUserDraggingSeekbarThumb;
        private bool _isRepeatEnabled;
        private bool _isMediaItemsShuffled;

        public readonly DispatcherTimer MediaPositionTracker = new();

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

        public MediaItemObservableCollection MediaItems
        {
            get => _mediaItems;
            set
            {
                _mediaItems = value;
                OnPropertyChanged(nameof(MediaItems));
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

        public TimeSpan MediaPosition
        {
            set
            {
                _mediaPosition = value;

                this.SelectedMediaItem.ElapsedTime = _mediaPosition;

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

        public bool IsMediaItemsShuffled
        {
            get => _isMediaItemsShuffled;
            set
            {
                _isMediaItemsShuffled = value;
                OnPropertyChanged(nameof(IsMediaItemsShuffled));
            }
        }

        #endregion

        #region Model Business Logic

        public bool IsMediaListEmpty()
        {
            return this.MediaItems.Count <= 0;
        }

        public void PlayMedia()
        {
            this.MediaState = MediaState.Play;
        }

        public void PauseMedia()
        {
            this.MediaState = MediaState.Pause;
        }

        public void StopMedia()
        {
            this.MediaState = MediaState.Stop;
        }

        public void SelectMediaItem(int index)
        {
            this.SelectedMediaItem = this.MediaItems[index];
        }

        public bool IsPreviousMediaItemAvailable()
        {
            return (!this.IsMediaListEmpty()) && this.MediaItems.Any(x => this.MediaItems.IndexOf(x) == this.MediaItems.IndexOf(this.SelectedMediaItem) - 1);
        }
        public bool IsNextMediaItemAvailable()
        {
            return (!this.IsMediaListEmpty()) && this.MediaItems.Any(x => this.MediaItems.IndexOf(x) == this.MediaItems.IndexOf(this.SelectedMediaItem) + 1);
        }

        public int GetPreviousMediaItemIndex()
        {
            return this.MediaItems.IndexOf(this.SelectedMediaItem) - 1;
        }

        public int GetNextMediaItemIndex()
        {
            return this.MediaItems.IndexOf(this.SelectedMediaItem) + 1;
        }

        public int GetFirstMediaItemIndex()
        {
            return this.MediaItems.IndexOf(this.MediaItems.First());
        }

        public int GetLastMediaItemIndex()
        {
            return this.MediaItems.IndexOf(this.MediaItems.Last());
        }

        public bool IsFirstMediaItemSelected()
        {
            return this.MediaItems.IndexOf(this.SelectedMediaItem) == this.MediaItems.IndexOf(this.MediaItems.First());
        }

        public bool IsLastMediaItemSelected()
        {
            return this.MediaItems.IndexOf(this.SelectedMediaItem) == this.MediaItems.IndexOf(this.MediaItems.Last());
        }

        public void SetAccurateCurrentMediaDuration(TimeSpan mediaDuration)
        {
            this.SelectedMediaItem.MediaDuration = mediaDuration;
        }

        public bool IsEndOfCurrentMedia(TimeSpan elapsedTime)
        {
            return elapsedTime == this.SelectedMediaItem.MediaDuration;
        }

        //public bool IsCurrentMediaDurationAccurate(TimeSpan mediaElementNaturalDuration)
        //{
        //    return this.SelectedMediaItem?.MediaDuration == mediaElementNaturalDuration;
        //}

        public void PlayPreviousMediaItem()
        {
            if (this.IsRepeatEnabled && this.IsFirstMediaItemSelected())
                this.SelectMediaItem(this.GetLastMediaItemIndex());
            else
                this.SelectMediaItem(this.GetPreviousMediaItemIndex());

            this.PlayMedia();
        }

        public void PlayNextMediaItem()
        {
            if (this.IsRepeatEnabled && this.IsLastMediaItemSelected())
                this.SelectMediaItem(this.GetFirstMediaItemIndex());
            else
                this.SelectMediaItem(this.GetNextMediaItemIndex());

            this.PlayMedia();
        }

        public void OrderMediaList()
        {
            this.MediaItems = new MediaItemObservableCollection(this.MediaItems.OrderBy(x => x.Id));
        }

        public void ShuffleMediaList()
        {
            this.MediaItems = new MediaItemObservableCollection(this.MediaItems
                .OrderBy(x => x != this.SelectedMediaItem)
                .ThenBy(x => _randomIdGenerator.Next()));
        }

        #endregion
    }
}
