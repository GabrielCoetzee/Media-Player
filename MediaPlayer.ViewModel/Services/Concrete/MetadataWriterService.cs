using MediaPlayer.Common.Constants;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.Metadata.Abstract.Writers;
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
        readonly IMetadataWriter _metadataWriter;

        [ImportingConstructor]
        public MetadataWriterService([Import(ServiceNames.TaglibMetadataWriter)] IMetadataWriter metadataWriter)
        {
            _metadataWriter = metadataWriter;
        }

        public async Task WriteChangesToFilesInParallel(IEnumerable<MediaItem> mediaItems)
        {
            if (!mediaItems.Any())
                return;

            await Task.Run(async () => {

                await Parallel.ForEachAsync(mediaItems, new CancellationTokenSource().Token, (mediaItem, token) =>
                {
                    _metadataWriter.SaveMediaItem(mediaItem);

                    return new ValueTask();
                });
            });
        }
    }
}
