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

        private readonly Random _randomIdGenerator = new();

        private MediaItem _selectedMediaItem;
        private MediaItemObservableCollection _mediaItems = new();
        private bool _isLoadingMediaItems;
        private TimeSpan _currentPosition;
        private TimeSpan _mediaElementPosition;
        private MediaState _mediaState = MediaState.Pause;
        private VolumeLevel _mediaVolume = VolumeLevel.Full;
        private bool _isUserDraggingSeekbarThumb;
        private bool _isRepeatEnabled;
        private bool _isMediaItemsShuffled;

        public readonly DispatcherTimer CurrentPositionTracker = new();

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

        /// <summary>
        /// Custom DispatcherTimer keeps track of media output and this binds to seekbar etc.
        /// </summary>
        public TimeSpan CurrentPosition
        {
            get => _currentPosition;
            set
            {
                _currentPosition = value;

                this.SelectedMediaItem.ElapsedTime = value;

                OnPropertyChanged(nameof(CurrentPosition));
            }
        }

        /// <summary>
        /// The Media element 'Position' dependency property cannot be bound to by default, so I created a custom dependency property.
        /// Unfortunately it is finnicky and two-way binding doesn't work as expected, the position doesn't update by itself from the media element, 
        /// even with two-way binding set up.
        /// Once MediaElementPosition is updated, the media element updates to where user dragged the slider,
        /// this cannot be done using CurrentPosition property due to that property not being bound to media element position and is being tracked manually by
        /// a DispatcherTimer.
        /// Trade-off of having both CurrentPosition and MediaElementPosition properties is necessary right now, though it would've been nice to have just one of the two.
        /// This might be best for user experience after all, the way it is now means while the user is dragging the seekbar, the song will continue playing until they
        /// release the mouse button where,
        /// if we had everything bound to 'CurrentPosition' and bound as expected, dragging the seekbar would've sounded like scrambling.
        /// </summary>
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
