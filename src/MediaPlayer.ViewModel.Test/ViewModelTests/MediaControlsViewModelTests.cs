using MediaPlayer.AudioEngine.Abstract;
using MediaPlayer.AudioEngine.Enumerations;
using MediaPlayer.AudioEngine.Events;
using MediaPlayer.ViewModel.ViewModels;
using Moq;
using NUnit.Framework;

namespace MediaPlayer.ViewModel.Test.ViewModelTests
{
    [TestFixture]
    public class MediaControlsViewModelTests
    {
        MediaControlsViewModel _vm;
        Mock<IAudioEngine> _audioEngineMock;

        [SetUp]
        public void SetUp()
        {
            _audioEngineMock = new Mock<IAudioEngine>();
            _vm = new MediaControlsViewModel
            {
                AudioEngine = _audioEngineMock.Object
            };
            _vm.OnImportsSatisfied();
        }

        [TestCase(PlaybackState.Playing)]
        [TestCase(PlaybackState.Paused)]
        [TestCase(PlaybackState.Stopped)]
        public void EngineStateChanged_MirrorsStateOntoViewModel(PlaybackState state)
        {
            _audioEngineMock.Raise(x => x.StateChanged += null, this, new PlaybackStateChangedEventArgs(state));

            Assert.That(_vm.PlaybackState, Is.EqualTo(state));
        }

        [Test]
        public void MediaVolume_WhenSet_UpdatesEngineVolume()
        {
            _vm.MediaVolume = 0.6;

            _audioEngineMock.VerifySet(x => x.Volume = 0.6, Times.Once);
        }

        [Test]
        public void Seek_ForwardsPositionToEngine()
        {
            var position = System.TimeSpan.FromSeconds(42);

            _vm.Seek(position);

            _audioEngineMock.Verify(x => x.SeekTo(position), Times.Once);
        }

        [Test]
        public void TogglePause_ForwardsToEngine()
        {
            _vm.TogglePause();

            _audioEngineMock.Verify(x => x.TogglePause(), Times.Once);
        }

        [Test]
        public void Stop_ForwardsToEngine()
        {
            _vm.Stop();

            _audioEngineMock.Verify(x => x.Stop(), Times.Once);
        }

        [Test]
        public void Play_DifferentPath_ForwardsToEngine()
        {
            _audioEngineMock.SetupGet(x => x.CurrentTrackPath).Returns("a.mp3");

            _vm.Play("b.mp3");

            _audioEngineMock.Verify(x => x.Play("b.mp3"), Times.Once);
        }

        [Test]
        public void Play_SamePathAsCurrent_NoOps()
        {
            _audioEngineMock.SetupGet(x => x.CurrentTrackPath).Returns("a.mp3");

            _vm.Play("a.mp3");

            _audioEngineMock.Verify(x => x.Play(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Play_WhitespacePath_NoOps()
        {
            _vm.Play("   ");

            _audioEngineMock.Verify(x => x.Play(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void EngineTrackEnded_NoNextTrackCommand_StopsEngine()
        {
            _audioEngineMock.Raise(x => x.TrackEnded += null, this, new TrackEndedEventArgs("track.mp3"));

            _audioEngineMock.Verify(x => x.Stop(), Times.Once);
        }
    }
}
