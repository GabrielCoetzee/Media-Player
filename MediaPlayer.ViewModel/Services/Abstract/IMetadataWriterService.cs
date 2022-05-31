using MediaPlayer.Model.BusinessEntities.Abstract;
using System.Collections.Generic;

namespace MediaPlayer.ViewModel.Services.Abstract
{
    public interface IMetadataWriterService
    {
        void WriteChangesToFilesInParallel(IEnumerable<MediaItem> mediaItems);
    }
}
