using System.Collections.Generic;
using System.Linq;
using MediaPlayer.BusinessEntities.Enumerations;
using MediaPlayer.Common.Metadata_Readers.Abstract;
using Ninject;

namespace MediaPlayer.Metadata_Readers
{
    public class MetadataReaderProviderResolver
    {
        #region Fields

        private readonly IEnumerable<MetadataReaderProvider> _metadataReaderProviders;

        #endregion

        #region Constructor

        [Inject]
        public MetadataReaderProviderResolver(IEnumerable<MetadataReaderProvider> metadataReaderProviders)
        {
            this._metadataReaderProviders = metadataReaderProviders;
        }

        #endregion

        #region Methods

        public MetadataReaderProvider Resolve(MetadataReaders selectedMetadataReader)
        {
            return this._metadataReaderProviders
                .SingleOrDefault(x => x.MetadataReader == selectedMetadataReader);
        }

        #endregion
    }
}
