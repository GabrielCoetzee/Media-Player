using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Generic.DependencyInjection;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Metadata.Abstract;

namespace MediaPlayer.Model.Metadata.Concrete
{
    [Export]
    public class MetadataWriterFactory
    {
        [ImportMany(typeof(IMetadataWriter))]
        public IEnumerable<IMetadataWriter> MetadataWriters { get; set; }

        public MetadataWriterFactory()
        {
            MEF.Container?.SatisfyImportsOnce(this);
        }

        public IMetadataWriter Resolve(MetadataLibraries metadataLibrary)
        {
            return MetadataWriters.SingleOrDefault(x => x.MetadataLibrary == metadataLibrary);
        }
    }
}
