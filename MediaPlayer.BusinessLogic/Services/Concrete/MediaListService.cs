using MediaPlayer.BusinessEntities.Objects.Base;
using MediaPlayer.BusinessLogic.Services.Abstract;
using MediaPlayer.Model;
using System.Collections.Generic;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Services.Concrete
{
    public class MediaListService : IMediaListService
    {
        readonly ModelMediaPlayer _modelMediaPlayer;

        public MediaListService(ModelMediaPlayer modelMediaPlayer)
        {
            _modelMediaPlayer = modelMediaPlayer;
        }

        public void AddRange(IEnumerable<MediaItem> mediaItems)
        {
            _modelMediaPlayer.MediaItems.AddRange(mediaItems);

            if (_modelMediaPlayer.SelectedMediaItem != null || _modelMediaPlayer.IsMediaListEmpty())
                return;

            _modelMediaPlayer.SelectMediaItem(_modelMediaPlayer.GetFirstMediaItemIndex());
            _modelMediaPlayer.PlayMedia();

            RefreshUIBindings();
        }

        private static void RefreshUIBindings()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
