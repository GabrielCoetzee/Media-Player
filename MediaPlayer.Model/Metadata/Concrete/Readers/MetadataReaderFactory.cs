using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Generic.DependencyInjection;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Metadata.Abstract;

namespace MediaPlayer.Model.Metadata.Concrete.Readers
{
    [Export]
    public class MetadataReaderFactory
    {
        [ImportMany(typeof(IMetadataReader))]
        public IEnumerable<IMetadataReader> MetadataReaders { get; set; }

        public MetadataReaderFactory()
        {
            MEF.Container?.SatisfyImportsOnce(this);
        }

        public IMetadataReader Resolve(MetadataLibraries metadataLibrary)
        {
            return MetadataReaders.SingleOrDefault(x => x.MetadataLibrary == metadataLibrary);
        }
    }
}
