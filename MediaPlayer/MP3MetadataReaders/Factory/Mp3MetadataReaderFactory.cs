using MediaPlayer.MetadataReaders.Interfaces;
using MediaPlayer.MetadataReaders.Interface_Implementations;
using MediaPlayer.MetadataReaders.Types;

namespace MediaPlayer.MetadataReaders.Factory
{
    public class Mp3MetadataReaderFactory
    {
        #region Constructor
        public Mp3MetadataReaderFactory()
        {
            
        }

        #endregion

        #region Properties

        private IReadMp3Metadata _mp3MetadataReader;

        private static Mp3MetadataReaderFactory _instance;
        public static Mp3MetadataReaderFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Mp3MetadataReaderFactory();
                }
                return _instance;
            }
        }

        #endregion

        #region Factory

        public IReadMp3Metadata GetMp3MetadataReader(Mp3MetadataReaderTypes.Mp3MetadataReaders selectedMp3MetadataReader)
        {
            if (_mp3MetadataReader == null)
            {
                switch (selectedMp3MetadataReader)
                {
                    case Mp3MetadataReaderTypes.Mp3MetadataReaders.Taglib:
                        _mp3MetadataReader = new TaglibMp3MetadataReaderWrapper();
                        break;
                }
            }

            return _mp3MetadataReader;
        }

        #endregion
    }
}
