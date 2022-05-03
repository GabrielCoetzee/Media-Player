using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Collections;
using MediaPlayer.Model.Objects.Base;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MediaPlayer.BusinessLogic.State.Abstract
{
    public interface IState : INotifyPropertyChanged
    {
        MediaItem SelectedMediaItem { get; set; }

        MediaItemObservableCollection MediaItems { get; set; }

        bool IsLoadingMediaItems { get; set; }

        TimeSpan MediaElementPosition { get; set; }

        MediaState MediaState { get; set; }

        VolumeLevel MediaVolume { get; set; }

        bool IsUserDraggingSeekbarThumb { get; set; }

        bool IsRepeatEnabled { get; set; }

        bool IsMediaItemsShuffled { get; set; }

        DispatcherTimer CurrentPositionTracker { get; set; }

        bool IsMediaListEmpty();

        void PlayMedia();

        void PauseMedia();

        void StopMedia();

        void SelectMediaItem(int index);

        bool IsPreviousMediaItemAvailable();

        public bool IsNextMediaItemAvailable();

        public int GetPreviousMediaItemIndex();

        public int GetNextMediaItemIndex();

        public int GetFirstMediaItemIndex();

        public int GetLastMediaItemIndex();

        public bool IsFirstMediaItemSelected();

        public bool IsLastMediaItemSelected();

        public void SetAccurateCurrentMediaDuration(TimeSpan duration);

        public bool IsEndOfCurrentMedia(TimeSpan elapsedTime);

        public void PlayPreviousMediaItem();

        public void PlayNextMediaItem();

        public void OrderMediaList();

        public void ShuffleMediaList();
    }
}
