using System.Collections.Generic;
using System.Linq;
using MediaPlayer.BusinessLogic.Abstract;
using MediaPlayer.Common.Enumerations;

namespace MediaPlayer.BusinessLogic
{
    public class MetadataReaderResolver
    {
        private readonly IEnumerable<IMetadataReaderProvider> _metadataReaderProviders;

        public MetadataReaderResolver(IEnumerable<IMetadataReaderProvider> metadataReaderProviders)
        {
            this._metadataReaderProviders = metadataReaderProviders;
        }

        public IMetadataReaderProvider Resolve(MetadataReaders selectedMetadataReader)
        {
            return this._metadataReaderProviders.SingleOrDefault(x => x.MetadataReader == selectedMetadataReader);
        }
    }
}
