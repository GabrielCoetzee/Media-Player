using System;
using System.Diagnostics;
using System.Windows.Forms;
using MediaPlayer.MetadataReaders.Interfaces;
using MediaPlayer.MVVM.Models.Base_Types;
using MediaPlayer.Objects;
using MediaPlayer.Objects.MediaList.Derived;
using TagLib;
using File = TagLib.File;


namespace MediaPlayer.MetadataReaders.Interface_Implementations
{
    public class TaglibMetadataReaderWrapper : IReadMetadata
    {
        #region Properties

        private File _taglibMetadataReader;

        #endregion

        public MediaItem GetFileMetadata(string path)
        {
            try
            {
                _taglibMetadataReader = File.Create(path);

                switch (_taglibMetadataReader.Properties.MediaTypes)
                {
                    case MediaTypes.Audio:
                        var audioItem = new AudioItem();

                        audioItem.AlbumArt = _taglibMetadataReader.Tag.Pictures.Length >= 1 ? _taglibMetadataReader.Tag.Pictures[0].Data.Data : audioItem.GetAlbumArtFromDirectory(path);
                        audioItem.Album = _taglibMetadataReader.Tag.Album;
                        audioItem.Artist = _taglibMetadataReader.Tag.FirstPerformer;
                        audioItem.Genre = _taglibMetadataReader.Tag.FirstGenre;
                        audioItem.Comments = _taglibMetadataReader.Tag.Comment;
                        audioItem.MediaListNumber = _taglibMetadataReader.Tag.Track;
                        audioItem.Year = _taglibMetadataReader.Tag.Year;
                        audioItem.Lyrics = _taglibMetadataReader.Tag.Lyrics;
                        audioItem.Composer = _taglibMetadataReader.Tag.FirstComposer;
                        audioItem.SongTitle = _taglibMetadataReader.Tag.Title;
                        audioItem.MediaDuration = _taglibMetadataReader.Properties.Duration;
                        audioItem.Bitrate = _taglibMetadataReader.Properties.AudioBitrate;

                        audioItem.HasLyrics = !string.IsNullOrEmpty(audioItem.Lyrics);
                        audioItem.FilePath = new Uri(path);

                        audioItem.MediaTypes.Add(MediaTypes.Audio);

                        Dispose();

                        return audioItem;
                    case MediaTypes.Video | MediaTypes.Audio:
                        var videoItem = new VideoItem();

                        videoItem.VideoHeight = _taglibMetadataReader.Properties.VideoHeight;
                        videoItem.VideoWidth = _taglibMetadataReader.Properties.VideoWidth;
                        videoItem.VideoTitle = _taglibMetadataReader.Tag.Title;
                        videoItem.FilePath = new Uri(path);
                        videoItem.MediaDuration = _taglibMetadataReader.Properties.Duration;

                        videoItem.MediaTypes.AddRange(new []{ MediaTypes.Video | MediaTypes.Audio });

                        Dispose();

                        return videoItem;
                }

                return null;
            }
            catch (CorruptFileException)
            {
                var audioItem = new AudioItem {FilePath = new Uri(path)};

                return audioItem;
            }
        }

        public void Dispose()
        {
            _taglibMetadataReader.Dispose();
        }


    }
}
