using MediaPlayer.Model.BusinessEntities.Abstract;
using System.Collections.Generic;

namespace MediaPlayer.ViewModel.Services.Abstract
{
    public interface IMetadataCorrectorService
    {
        void FixMetadata(IEnumerable<MediaItem> mediaItems);
    }
}
