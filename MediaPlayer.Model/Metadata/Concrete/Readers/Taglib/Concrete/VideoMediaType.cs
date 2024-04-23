using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.Metadata.Concrete.Readers.Taglib.Abstract;
using MediaPlayer.Model.ObjectBuilders;
using System.ComponentModel.Composition;
using TagLib;

namespace MediaPlayer.Model.Metadata.Concrete.Readers.Taglib.Concrete
{
    [Export(typeof(ITaglibMediaTypeIdentifiable))]
    public class VideoMediaType : ITaglibMediaTypeIdentifiable
    {
        public bool IsValid(MediaTypes mediaType)
        {
            return mediaType == (MediaTypes.Video | MediaTypes.Audio);
        }
        public MediaItem BuildMediaItem(File reader)
        {
            return new VideoItemBuilder(reader.Name)
                .AsMediaType(Common.Enumerations.MediaTypes.Video | Common.Enumerations.MediaTypes.Audio)
                .WithResolution($"{reader.Properties.VideoWidth} x {reader.Properties.VideoHeight}")
                .WithTitle(reader.Tag.Title)
                .WithDuration(reader.Properties.Duration)
                .Build();
        }
    }
}
