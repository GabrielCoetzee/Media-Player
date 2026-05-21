using MediaPlayer.ViewModel.ViewModels;
using NUnit.Framework;
using System.Windows.Controls;

namespace MediaPlayer.ViewModel.Test.ViewModelTests
{
    [TestFixture]
    public class MediaControlsViewModelTests
    {
        MediaControlsViewModel _vm;

        [SetUp]
        public void SetUp()
        {
            _vm = new MediaControlsViewModel();
        }

        [TestCase(MediaState.Play)]
        [TestCase(MediaState.Pause)]
        [TestCase(MediaState.Stop)]
        public void MediaState_WhenSet_RaisesPropertyChanged(MediaState mediaState)
        {
            _vm.MediaState = mediaState;

            Assert.That(_vm.MediaState, Is.EqualTo(mediaState));
        }

        [TestCase(0.0, true)]
        [TestCase(0.5, false)]
        [TestCase(1.0, false)]
        public void IsMuted_TracksMediaVolume(double volume, bool expectedIsMuted)
        {
            _vm.MediaVolume = volume;

            Assert.That(_vm.IsMuted, Is.EqualTo(expectedIsMuted));
        }

        [Test]
        public void MediaVolume_IsClampedBetweenZeroAndOne()
        {
            _vm.MediaVolume = 1.5;
            Assert.That(_vm.MediaVolume, Is.EqualTo(1.0));

            _vm.MediaVolume = -0.5;
            Assert.That(_vm.MediaVolume, Is.EqualTo(0.0));
        }
    }
}
