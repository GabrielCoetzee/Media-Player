using MediaPlayer.AudioEngine.Enumerations;
using System;

namespace MediaPlayer.AudioEngine.Events
{
    public class PlaybackStateChangedEventArgs : EventArgs
    {
        public PlaybackState State { get; }

        public PlaybackStateChangedEventArgs(PlaybackState state)
        {
            State = state;
        }
    }
}
