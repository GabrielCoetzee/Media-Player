using MediaPlayer.MetadataReaders.Interfaces;
using MediaPlayer.MetadataReaders.Interface_Implementations;
using MediaPlayer.MetadataReaders.Types;

namespace MediaPlayer.MetadataReaders.Factory
{
    public class MetadataReaderFactory
    {
        #region Constructor
        private MetadataReaderFactory()
        {
        }

        #endregion

        #region Properties

        private IReadMetadata _metadataReader;

        private static MetadataReaderFactory _instance;
        public static MetadataReaderFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MetadataReaderFactory();
                }
                return _instance;
            }
        }

        #endregion

        #region Factory

        public IReadMetadata GetMetadataReader(MetadataReaderTypes.MetadataReaders selectedMetadataReader)
        {
            if (_metadataReader == null)
            {
                switch (selectedMetadataReader)
                {
                    case MetadataReaderTypes.MetadataReaders.Taglib:
                        _metadataReader = new TaglibMetadataReaderWrapper();
                        break;
                }
            }

            return _metadataReader;
        }

        #endregion
    }
}
