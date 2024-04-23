using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.Metadata.Concrete.Readers.Taglib.Abstract;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using TagLib;

namespace MediaPlayer.Model.Metadata.Concrete.Readers.Taglib
{
    [Export]
    public class TaglibMediaItemBuilderFactory
    {
        readonly IEnumerable<ITaglibMediaTypeIdentifiable> _taglibMediaTypes;

        [ImportingConstructor]
        public TaglibMediaItemBuilderFactory([ImportMany] IEnumerable<ITaglibMediaTypeIdentifiable> taglibMediaTypes)
        {
            _taglibMediaTypes = taglibMediaTypes;
        }

        public MediaItem BuildMediaItem(string path)
        {
            try
            {
                using var reader = File.Create(path);

                var mediaType = _taglibMediaTypes.SingleOrDefault(x => x.IsValid(reader.Properties.MediaTypes));

                return mediaType.BuildMediaItem(reader);
            }
            catch (CorruptFileException)
            {
                return null;
            }
        }
    }
}
