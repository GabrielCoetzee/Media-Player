using System;
using MediaPlayer.BusinessEntities.Objects.Derived;
using MediaPlayer.Common.Enumerations;

namespace MediaPlayer.BusinessEntities.Object_Builders
{
    public class VideoItemBuilder
    {
        #region Fields

        private readonly VideoItem _videoItem;

        #endregion

        #region Constructor

        public VideoItemBuilder(string filePath)
        {
            _videoItem = new VideoItem
            {
                FilePath = new Uri(filePath)
            };
        }

        #endregion

        #region Public Methods

        public VideoItemBuilder WithVideoResolution(string resolution)
        {
            _videoItem.VideoResolution = resolution;

            return this;
        }

        public VideoItemBuilder WithVideoTitle(string videoTitle)
        {
            _videoItem.VideoTitle = videoTitle;
            _videoItem.MediaTitle = GetMediaTitle();
            _videoItem.WindowTitle = GetWindowTitle();

            return this;
        }

        public VideoItemBuilder WithMediaDuration(TimeSpan mediaDuration)
        {
            _videoItem.MediaDuration = mediaDuration;

            return this;
        }

        public VideoItemBuilder AsMediaType(MediaType mediaType)
        {
            _videoItem.MediaType = mediaType;

            return this;
        }

        public VideoItem Build()
        {
            return _videoItem;
        }

        #endregion

        #region Private Methods

        private string GetWindowTitle()
        {
            if (!string.IsNullOrEmpty(_videoItem.MediaTitle))
                return $"Now Playing : {_videoItem.MediaTitle}";
            else
                return $"Now Playing : {_videoItem.FileName}";
        }

        private string GetMediaTitle()
        {
            return _videoItem.VideoTitle ?? _videoItem.FileName;
        }

        #endregion
    }
}
