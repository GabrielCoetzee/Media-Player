namespace MediaPlayer.ViewModel.Services.Abstract
{
    /// <summary>
    /// Contains Metadata Services (Read/Write/Update) - Service Aggregator Pattern
    /// </summary>
    public interface IMetadataServices
    {
        IMetadataReaderService MetadataReader { get; set; }

        IMetadataWriterService MetadataWriter { get; set; }

        IMetadataUpdateService MetadataUpdater { get; set; }

        IMetadataCorrectorService MetadataCorrector { get; set; }
    }
}
