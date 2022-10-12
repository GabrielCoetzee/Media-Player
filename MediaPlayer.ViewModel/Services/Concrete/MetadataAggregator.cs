using MediaPlayer.ViewModel.Services.Abstract;
using System.ComponentModel.Composition;

namespace MediaPlayer.ViewModel.Services.Concrete
{
    [Export(typeof(IMetadataAggregator))]
    public class MetadataAggregator : IMetadataAggregator
    {
        [Import]
        public IMetadataReaderService MetadataReader { get; set; }

        [Import]
        public IMetadataWriterService MetadataWriter { get; set; }

        [Import]
        public IMetadataUpdateService MetadataUpdater { get; set; }
    }
}
