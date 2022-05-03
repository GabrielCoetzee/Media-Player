using MediaPlayer.Model.Objects.Base;
using System.Collections.Generic;

namespace MediaPlayer.BusinessLogic.Services.Abstract
{
    public interface IMediaService
    {
        void AddMediaItems(IEnumerable<MediaItem> mediaItems);
    }
}
