using System.Collections.Generic;
using System.Linq;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Metadata.Abstract;

namespace MediaPlayer.Model.Metadata.Concrete
{
    public class MetadataReaderResolver
    {
        private readonly IEnumerable<IMetadataReaderProvider> _metadataReaderProviders;

        public MetadataReaderResolver(IEnumerable<IMetadataReaderProvider> metadataReaderProviders)
        {
            _metadataReaderProviders = metadataReaderProviders;
        }

        public IMetadataReaderProvider Resolve(MetadataReaders selectedMetadataReader)
        {
            return _metadataReaderProviders.SingleOrDefault(x => x.MetadataReader == selectedMetadataReader);
        }
    }
}
