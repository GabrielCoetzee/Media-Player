using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Generic;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Metadata.Abstract;

namespace MediaPlayer.Model.Metadata.Concrete
{
    [Export]
    public class MetadataReaderResolver
    {
        [ImportMany(typeof(IMetadataReader))]
        public IEnumerable<IMetadataReader> MetadataReaders { get; set; }

        public MetadataReaderResolver()
        {
            MEF.Container?.SatisfyImportsOnce(this);
        }

        public IMetadataReader Resolve(MetadataReaders metadataReader)
        {
            return MetadataReaders.SingleOrDefault(x => x.MetadataReader == metadataReader);
        }
    }
}
