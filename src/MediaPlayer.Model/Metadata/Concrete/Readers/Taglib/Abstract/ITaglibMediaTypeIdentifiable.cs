using MediaPlayer.Model.BusinessEntities.Abstract;
using TagLib;

namespace MediaPlayer.Model.Metadata.Concrete.Readers.Taglib.Abstract
{
    public interface ITaglibMediaTypeIdentifiable
    {
        public bool IsValid(MediaTypes mediaType);
        public MediaItem BuildMediaItem(File reader);
    }
}
