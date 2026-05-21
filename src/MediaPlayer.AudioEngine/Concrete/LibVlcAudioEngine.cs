using LibVLCSharp.Shared;
using MediaPlayer.AudioEngine.Abstract;
using MediaPlayer.AudioEngine.Enumerations;
using MediaPlayer.AudioEngine.Events;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using VlcMediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace MediaPlayer.AudioEngine.Concrete
{
    [Export(typeof(IAudioEngine))]
    public class LibVlcAudioEngine : IAudioEngine, INotifyPropertyChanged, IDisposable
    {
        private readonly Dispatcher _dispatcher;
        private readonly object _initLock = new object();

        private LibVLC _libVlc;
        private VlcMediaPlayer _mediaPlayer;
        private bool _initialized;

        private double _volume = 1.0;
        private PlaybackState _state = PlaybackState.Stopped;
        private TimeSpan _duration;
        private TimeSpan _position;
        private string _currentTrackPath;
        private bool _disposed;

        public LibVlcAudioEngine()
        {
            _dispatcher = Application.Current?.Dispatcher;
        }

        public TimeSpan Position => _position;

        public TimeSpan Duration => _duration;

        public double Volume
        {
            get => _volume;
            set
            {
                var clamped = Math.Clamp(value, 0.0, 1.0);

                if (Math.Abs(clamped - _volume) < double.Epsilon)
                    return;

                _volume = clamped;

                if (_mediaPlayer != null)
                    _mediaPlayer.Volume = (int)Math.Round(_volume * 100);
            }
        }

        public PlaybackState PlaybackState => _state;

        public string CurrentTrackPath => _currentTrackPath;

        public VlcMediaPlayer NativePlayer => _mediaPlayer;

        public event EventHandler<PlaybackPositionChangedEventArgs> PositionChanged;
        public event EventHandler<PlaybackStateChangedEventArgs> StateChanged;
        public event EventHandler<DurationDiscoveredEventArgs> DurationDiscovered;
        public event EventHandler<TrackEndedEventArgs> TrackEnded;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Play(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            RunOffUiThread(() => PlayCore(path));
        }

        public void TogglePause() => RunOffUiThread(TogglePauseCore);

        public void Stop() => RunOffUiThread(StopCore);

        public void SeekTo(TimeSpan position) => RunOffUiThread(() => SeekToCore(position));

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (!_initialized)
                return;

            UnsubscribeFromVlcEvents();

            try { _mediaPlayer.Stop(); } catch { /* shutdown */ }

            var existingMedia = _mediaPlayer.Media;
            _mediaPlayer.Media = null;
            existingMedia?.Dispose();

            _mediaPlayer.Dispose();
            _libVlc.Dispose();
        }

        private void EnsureInitialized()
        {
            if (_initialized)
                return;

            lock (_initLock)
            {
                if (_initialized)
                    return;

                _libVlc = new LibVLC();
                _mediaPlayer = new VlcMediaPlayer(_libVlc)
                {
                    EnableHardwareDecoding = true,
                    Volume = (int)Math.Round(_volume * 100)
                };

                SubscribeToVlcEvents();

                _initialized = true;
            }

            RunOnUiThread(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NativePlayer))));
        }

        private void SubscribeToVlcEvents()
        {
            _mediaPlayer.Playing += OnVlcPlaying;
            _mediaPlayer.Paused += OnVlcPaused;
            _mediaPlayer.Stopped += OnVlcStopped;
            _mediaPlayer.TimeChanged += OnVlcTimeChanged;
            _mediaPlayer.LengthChanged += OnVlcLengthChanged;
            _mediaPlayer.EndReached += OnVlcEndReached;
            _mediaPlayer.EncounteredError += OnVlcEncounteredError;
        }

        private void UnsubscribeFromVlcEvents()
        {
            _mediaPlayer.Playing -= OnVlcPlaying;
            _mediaPlayer.Paused -= OnVlcPaused;
            _mediaPlayer.Stopped -= OnVlcStopped;
            _mediaPlayer.TimeChanged -= OnVlcTimeChanged;
            _mediaPlayer.LengthChanged -= OnVlcLengthChanged;
            _mediaPlayer.EndReached -= OnVlcEndReached;
            _mediaPlayer.EncounteredError -= OnVlcEncounteredError;
        }

        private void PlayCore(string path)
        {
            if (!File.Exists(path))
                return;

            _currentTrackPath = path;
            _duration = TimeSpan.Zero;
            _position = TimeSpan.Zero;

            var previousMedia = _mediaPlayer.Media;

            var media = new Media(_libVlc, new Uri(path));

            _mediaPlayer.Media = media;

            previousMedia?.Dispose();

            _mediaPlayer.Play();
        }

        private void TogglePauseCore()
        {
            if (_state == PlaybackState.Playing)
            {
                _mediaPlayer.SetPause(true);
                return;
            }

            _mediaPlayer.Play();
        }

        private void StopCore()
        {
            try { _mediaPlayer.Stop(); } catch { /* engine idle */ }
        }

        private void SeekToCore(TimeSpan position) => _mediaPlayer.SeekTo(position);

        private void OnVlcPlaying(object sender, EventArgs e) => UpdateState(PlaybackState.Playing);

        private void OnVlcPaused(object sender, EventArgs e) => UpdateState(PlaybackState.Paused);

        private void OnVlcStopped(object sender, EventArgs e) => UpdateState(PlaybackState.Stopped);

        private void OnVlcEncounteredError(object sender, EventArgs e) => UpdateState(PlaybackState.Stopped);

        private void OnVlcTimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            var newPosition = TimeSpan.FromMilliseconds(e.Time);
            _position = newPosition;

            RunOnUiThread(() => PositionChanged?.Invoke(this, new PlaybackPositionChangedEventArgs(newPosition)));
        }

        private void OnVlcLengthChanged(object sender, MediaPlayerLengthChangedEventArgs e)
        {
            if (e.Length <= 0)
                return;

            var duration = TimeSpan.FromMilliseconds(e.Length);
            _duration = duration;

            if (!TryGetCurrentTrackPath(out var path))
                return;

            RunOnUiThread(() => DurationDiscovered?.Invoke(this, new DurationDiscoveredEventArgs(path, duration)));
        }

        private void OnVlcEndReached(object sender, EventArgs e)
        {
            if (!TryGetCurrentTrackPath(out var path))
                return;

            RunOnUiThread(() => TrackEnded?.Invoke(this, new TrackEndedEventArgs(path)));
        }

        private void UpdateState(PlaybackState newState)
        {
            if (_state == newState)
                return;

            _state = newState;
            RunOnUiThread(() => StateChanged?.Invoke(this, new PlaybackStateChangedEventArgs(newState)));
        }

        private bool TryGetCurrentTrackPath(out string path)
        {
            path = _currentTrackPath;

            return !string.IsNullOrEmpty(path);
        }

        private void RunOffUiThread(Action action) => ThreadPool.QueueUserWorkItem(_ =>
        {
            EnsureInitialized();
            action();
        });

        private void RunOnUiThread(Action action)
        {
            if (_dispatcher == null || _dispatcher.CheckAccess())
            {
                action();
                return;
            }

            _dispatcher.BeginInvoke(action);
        }
    }
}
