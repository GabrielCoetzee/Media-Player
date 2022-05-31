using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.Metadata.Concrete;
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

        public void WriteChangesToFilesInParallel(IEnumerable<MediaItem> mediaItems)
        {
            if (!mediaItems.Any())
                return;

            Parallel.ForEach(mediaItems, (x) => _metadataWriterFactory.Resolve(MetadataLibraries.Taglib).Update(x));
        }
    }
}
