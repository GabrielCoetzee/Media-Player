using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Generic.DependencyInjection;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Metadata.Abstract.Writers;

namespace MediaPlayer.Model.Metadata.Concrete.Writers
{
    [Export]
    public class MetadataWriterFactory
    {
        [ImportMany(typeof(IMetadataWriter))]
        public IEnumerable<IMetadataWriter> MetadataWriters { get; set; }

        public IMetadataWriter Resolve(MetadataLibraries metadataLibrary)
        {
            return MetadataWriters.SingleOrDefault(x => x.MetadataLibrary == metadataLibrary);
        }
    }
}
