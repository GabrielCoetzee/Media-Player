using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.Metadata.Concrete.Writers;
using MediaPlayer.ViewModel.Services.Abstract;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
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

        public MetadataLibraries MetadataLibrary => MetadataLibraries.Taglib;

        public async Task WriteChangesToFilesInParallel(IEnumerable<MediaItem> mediaItems)
        {
            if (!mediaItems.Any())
                return;

            await Task.Run(async () => {

                await Parallel.ForEachAsync(mediaItems, new CancellationTokenSource().Token, (mediaItem, token) =>
                {
                    var metadataWriter = _metadataWriterFactory.Resolve(MetadataLibrary);

                    metadataWriter.Save(mediaItem);

                    return new ValueTask();
                });
            });
        }
    }
}
