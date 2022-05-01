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
            this._modelMediaPlayer.MediaItems.AddRange(mediaItems);

            if (_modelMediaPlayer.SelectedMediaItem != null || this._modelMediaPlayer.IsMediaListEmpty())
                return;

            this._modelMediaPlayer.SelectMediaItem(this._modelMediaPlayer.GetFirstMediaItemIndex());
            this._modelMediaPlayer.PlayMedia();

            this.RefreshUIBindings();
        }

        private void RefreshUIBindings()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
