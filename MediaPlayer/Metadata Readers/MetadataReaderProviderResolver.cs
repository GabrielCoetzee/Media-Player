using System.Collections.Generic;
using System.Linq;

namespace MediaPlayer.MetadataReaders.Factory
{
    public class MetadataReaderProviderResolver
    {
        #region Fields

        private readonly IEnumerable<MetadataReaderProvider> _metadataReaderProviders;

        #endregion

        #region Constructor

        public MetadataReaderProviderResolver(IEnumerable<MetadataReaderProvider> metadataReaderProviders)
        {
            this._metadataReaderProviders = metadataReaderProviders;
        }

        #endregion

        #region Methods

        public MetadataReaderProvider Resolve(BusinessEntities.MetadataReaders selectedMetadataReader)
        {
            return this._metadataReaderProviders
                .SingleOrDefault(x => x.MetadataReader == selectedMetadataReader);
        }

        #endregion
    }
}
