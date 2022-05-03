using MediaPlayer.Model.Objects.Base;
using System.Collections.Generic;

namespace MediaPlayer.BusinessLogic.Services.Abstract
{
    public interface IMediaListService
    {
        void AddRange(IEnumerable<MediaItem> mediaItems);
    }
}
