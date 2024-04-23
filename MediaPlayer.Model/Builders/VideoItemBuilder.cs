using System;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Concrete;

namespace MediaPlayer.Model.ObjectBuilders
{
    public class VideoItemBuilder
    {
        private readonly VideoItem _videoItem;

        public VideoItemBuilder(string filePath)
        {
            _videoItem = new VideoItem
            {
                FilePath = new Uri(filePath)
            };
        }

        public VideoItemBuilder WithResolution(string resolution)
        {
            _videoItem.InfoLabel = $"Resolution: {resolution}";

            return this;
        }

        public VideoItemBuilder WithTitle(string videoTitle)
        {
            _videoItem.MediaTitle = videoTitle ?? _videoItem.FileName;
            _videoItem.WindowTitle += $"{(!string.IsNullOrEmpty(_videoItem.MediaTitle) ? $"{_videoItem.MediaTitle}" : $"{_videoItem.FileName}")}";

            return this;
        }

        public VideoItemBuilder WithDuration(TimeSpan mediaDuration)
        {
            _videoItem.Duration = mediaDuration;

            return this;
        }

        public VideoItemBuilder AsMediaType(MediaTypes mediaType)
        {
            _videoItem.MediaType = mediaType;

            return this;
        }

        public VideoItem Build()
        {
            return _videoItem;
        }
    }
}
