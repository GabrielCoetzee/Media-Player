using System.Collections.Generic;
using System.Linq;
using MediaPlayer.BusinessLogic.Abstract;
using MediaPlayer.Common.Enumerations;

namespace MediaPlayer.BusinessLogic
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

        public MetadataReaderProvider Resolve(MetadataReaders selectedMetadataReader)
        {
            return this._metadataReaderProviders
                .SingleOrDefault(x => x.MetadataReader == selectedMetadataReader);
        }

        #endregion
    }
}
