﻿using MediaPlayer.ViewModel.Services.Abstract;
using System.ComponentModel.Composition;

namespace MediaPlayer.ViewModel.Services.Concrete
{
    [Export(typeof(IMetadataServices))]
    public class MetadataServices : IMetadataServices
    {
        [Import]
        public IMetadataReaderService MetadataReader { get; set; }

        [Import]
        public IMetadataWriterService MetadataWriter { get; set; }

        [Import]
        public IMetadataUpdateService MetadataUpdater { get; set; }

        [Import]
        public IMetadataCorrectorService MetadataCorrector { get; set; }
    }
}
