using System.Collections.Generic;
using System.Linq;
using MediaPlayer.MetadataReaders.Abstract;
using Ninject;

namespace MediaPlayer.MetadataReaders
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

        public MetadataReaderProvider Resolve(Common.Enumerations.MetadataReaders selectedMetadataReader)
        {
            return this._metadataReaderProviders
                .SingleOrDefault(x => x.MetadataReader == selectedMetadataReader);
        }

        #endregion
    }
}
