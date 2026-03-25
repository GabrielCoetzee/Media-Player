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
        public void SetPlaybackState_WhenCalled_ChangesPlaybackStateToCorrectMediaState(MediaState mediaState)
        {
            _vm.SetPlaybackState(mediaState);

            Assert.That(_vm.MediaState, Is.EqualTo(mediaState));
        }

    }
}
