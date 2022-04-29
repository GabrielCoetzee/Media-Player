using System.Collections.Generic;
using System.Linq;
using MediaPlayer.BusinessLogic.Abstract;
using MediaPlayer.Common.Enumerations;

namespace MediaPlayer.BusinessLogic
{
    public class MetadataReaderResolver
    {
        #region Fields

        private readonly IEnumerable<IMetadataReaderProvider> _metadataReaderProviders;

        #endregion

        #region Constructor

        public MetadataReaderResolver(IEnumerable<IMetadataReaderProvider> metadataReaderProviders)
        {
            this._metadataReaderProviders = metadataReaderProviders;
        }

        #endregion

        #region Methods

        public IMetadataReaderProvider Resolve(MetadataReaders selectedMetadataReader)
        {
            return this._metadataReaderProviders.SingleOrDefault(x => x.MetadataReader == selectedMetadataReader);
        }

        #endregion
    }
}
