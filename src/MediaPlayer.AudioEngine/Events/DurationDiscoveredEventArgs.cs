using System;

namespace MediaPlayer.AudioEngine.Events
{
    public class DurationDiscoveredEventArgs : EventArgs
    {
        public string Path { get; }

        public TimeSpan Duration { get; }

        public DurationDiscoveredEventArgs(string path, TimeSpan duration)
        {
            Path = path;
            Duration = duration;
        }
    }
}
