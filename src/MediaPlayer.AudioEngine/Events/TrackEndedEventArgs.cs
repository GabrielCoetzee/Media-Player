using System;

namespace MediaPlayer.AudioEngine.Events
{
    public class TrackEndedEventArgs : EventArgs
    {
        public string Path { get; }

        public TrackEndedEventArgs(string path)
        {
            Path = path;
        }
    }
}
