using MediaPlayer.BusinessLogic.Services.Abstract;
using MediaPlayer.BusinessLogic.State.Abstract;
using MediaPlayer.Model.Objects.Base;
using System.Collections.Generic;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Services.Concrete
{
    public class MediaListService : IMediaListService
    {
        readonly IState _state;

        public MediaListService(IState state)
        {
            _state = state;
        }

        public void AddRange(IEnumerable<MediaItem> mediaItems)
        {
            _state.MediaItems.AddRange(mediaItems);

            if (_state.SelectedMediaItem != null || _state.IsMediaListEmpty())
                return;

            _state.SelectMediaItem(_state.GetFirstMediaItemIndex());
            _state.PlayMedia();

            RefreshUIBindings();
        }

        private static void RefreshUIBindings()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
