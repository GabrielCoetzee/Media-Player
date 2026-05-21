using MediaPlayer.AudioEngine.Enumerations;
using MediaPlayer.AudioEngine.Events;
using System;
using VlcMediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace MediaPlayer.AudioEngine.Abstract
{
    public interface IAudioEngine
    {
        TimeSpan Position { get; }
        TimeSpan Duration { get; }
        double Volume { get; set; }
        PlaybackState PlaybackState { get; }
        string CurrentTrackPath { get; }

        VlcMediaPlayer NativePlayer { get; }

        event EventHandler<PlaybackPositionChangedEventArgs> PositionChanged;
        event EventHandler<PlaybackStateChangedEventArgs> StateChanged;
        event EventHandler<DurationDiscoveredEventArgs> DurationDiscovered;
        event EventHandler<TrackEndedEventArgs> TrackEnded;

        void Play(string path);
        void TogglePause();
        void Stop();
        void SeekTo(TimeSpan position);
    }
}
