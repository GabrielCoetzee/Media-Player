using System;

namespace MediaPlayer.AudioEngine.Events
{
    public class PlaybackPositionChangedEventArgs : EventArgs
    {
        public TimeSpan Position { get; }

        public PlaybackPositionChangedEventArgs(TimeSpan position)
        {
            Position = position;
        }
    }
}
