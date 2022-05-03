using MediaPlayer.Model.Objects.Base;
using MediaPlayer.Common.Enumerations;
using System;

namespace MediaPlayer.BusinessLogic.Abstract
{
    public interface IMetadataReaderProvider
    {
        MetadataReaders MetadataReader { get; }

        MediaItem GetFileMetadata(string path);
    }
}
