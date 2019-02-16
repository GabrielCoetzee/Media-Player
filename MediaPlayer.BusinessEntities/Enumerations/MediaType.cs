using System;

namespace MediaPlayer.BusinessEntities
{
    [Flags]
    public enum MediaType
    {
        None = 0,
        Audio = 1,
        Video = 2
    }
}

