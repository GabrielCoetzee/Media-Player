using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.Metadata.Concrete.Writers;
using MediaPlayer.ViewModel.Services.Abstract;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace MediaPlayer.ViewModel.Services.Concrete
{
    [Export(typeof(IMetadataWriterService))]
    public class MetadataWriterService : IMetadataWriterService
    {
        readonly MetadataWriterFactory _metadataWriterFactory;

        [ImportingConstructor]
        public MetadataWriterService(MetadataWriterFactory metadataWriterFactory)
        {
            _metadataWriterFactory = metadataWriterFactory;
        }

        public async Task WriteChangesToFilesInParallel(IEnumerable<MediaItem> mediaItems)
        {
            if (!mediaItems.Any())
                return;

            await Task.Run(() => {

                if (Parallel.ForEach(mediaItems, (mediaItem) =>
                {
                    var metadataWriter = _metadataWriterFactory.Resolve(MetadataLibraries.Taglib);

                    metadataWriter.Update(mediaItem);

                }).IsCompleted)
                    return;
            });
        }
    }
}
