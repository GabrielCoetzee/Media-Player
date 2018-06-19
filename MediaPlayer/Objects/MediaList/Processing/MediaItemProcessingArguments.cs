using System.Collections;
using MediaPlayer.MetadataReaders.Interfaces;

namespace MediaPlayer.Objects.MediaList.Processing
{
    public class MediaItemProcessingArguments
    {
        public IReadMp3Metadata IReadMp3Metadata { get; set; }

        public IEnumerable FilePaths { get; set; }
    }
}
