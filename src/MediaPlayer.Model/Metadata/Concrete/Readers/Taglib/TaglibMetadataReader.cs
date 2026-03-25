using MediaPlayer.Model.ObjectBuilders;
using MediaPlayer.Model.BusinessEntities.Abstract;
using TagLib;
using System.ComponentModel.Composition;
using System.Linq;
using MediaPlayer.Model.Metadata.Abstract.Readers;
using MediaPlayer.Common.Constants;

namespace MediaPlayer.Model.Metadata.Concrete.Readers.Taglib
{
    [Export(ServiceNames.TaglibMetadataReader, typeof(IMetadataReader))]
    public class TaglibMetadataReader : IMetadataReader
    {
        readonly TaglibMediaItemBuilderFactory _taglibMediaItemBuilderFactory;

        [ImportingConstructor]
        public TaglibMetadataReader(TaglibMediaItemBuilderFactory taglibMediaItemBuilderFactory)
        {
            _taglibMediaItemBuilderFactory = taglibMediaItemBuilderFactory;
        }

        public MediaItem BuildMediaItem(string path)
        {
            return _taglibMediaItemBuilderFactory.BuildMediaItem(path);
        }
    }
}
