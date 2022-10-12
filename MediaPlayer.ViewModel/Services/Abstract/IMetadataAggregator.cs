namespace MediaPlayer.ViewModel.Services.Abstract
{
    public interface IMetadataAggregator
    {
        IMetadataReaderService MetadataReader { get; set; }

        IMetadataWriterService MetadataWriter { get; set; }

        IMetadataUpdateService MetadataUpdater { get; set; }
    }
}
