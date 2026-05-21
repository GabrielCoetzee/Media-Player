using MediaPlayer.Model.BusinessEntities.Abstract;
using System;

namespace MediaPlayer.ViewModel.Events
{
    public class SelectedMediaItemChangedEventArgs : EventArgs
    {
        public MediaItem SelectedMediaItem { get; }

        public SelectedMediaItemChangedEventArgs(MediaItem selectedMediaItem)
        {
            SelectedMediaItem = selectedMediaItem;
        }
    }
}
