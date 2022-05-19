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
        [ImportMany(typeof(IMetadataReaderProvider))]
        public IEnumerable<IMetadataReaderProvider> MetadataReaderProviders { get; set; }

        public MetadataReaderResolver()
        {
            MEF.Container?.SatisfyImportsOnce(this);
        }

        public IMetadataReaderProvider Resolve(MetadataReaders selectedMetadataReader)
        {
            return MetadataReaderProviders.SingleOrDefault(x => x.MetadataReader == selectedMetadataReader);
        }
    }
}
