using System.Collections;
using MediaPlayer.MetadataReaders.Interfaces;

namespace MediaPlayer.Objects.MediaList.Processing
{
    public class MediaItemProcessingArguments
    {
        public IReadMetadata ReadMetadata { get; set; }

        public IEnumerable FilePaths { get; set; }
    }
}
