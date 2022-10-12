using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaPlayer.ViewModel.Services.Abstract
{
    public interface IMetadataWriterService
    {
        Task WriteChangesToFilesInParallel(IEnumerable<MediaItem> mediaItems);

        MetadataLibraries MetadataLibrary { get; }
    }
}
