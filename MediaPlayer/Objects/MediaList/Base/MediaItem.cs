using System;
using System.IO;

namespace MediaPlayer.MVVM.Models.Base_Types
{
    public abstract class MediaItem
    {
        public abstract int Id { get; set; }
        public abstract Uri FilePath{ get; set; }

        public string FileName => Path.GetFileNameWithoutExtension(FilePath.ToString());

        public abstract TimeSpan MediaDuration { get; set; }

        public abstract string WindowTitle { get; set; }

        public abstract string MediaTitle { get; }

        public abstract string MediaTitleTrimmed { get; }
    }
}
