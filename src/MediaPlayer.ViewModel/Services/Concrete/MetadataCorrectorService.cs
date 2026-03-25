using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.Metadata.Abstract.Correctors;
using MediaPlayer.ViewModel.Services.Abstract;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace MediaPlayer.ViewModel.Services.Concrete
{
    [Export(typeof(IMetadataCorrectorService))]
    public class MetadataCorrectorService : IMetadataCorrectorService
    {
        readonly IEnumerable<IMetadataCorrector> _metadataCorrectors;

        [ImportingConstructor]
        public MetadataCorrectorService([ImportMany] IEnumerable<IMetadataCorrector> metadataCorrectors)
        {
            _metadataCorrectors = metadataCorrectors;
        }

        public void FixMetadata(IEnumerable<MediaItem> mediaItems)
        {
            foreach (var mediaItem in mediaItems)
                _metadataCorrectors.Where(x => x.IsValid(mediaItem)).ToList().ForEach(x => x.FixMetadata(mediaItem));
        }
    }
}
