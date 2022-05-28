using System;
using System.Windows.Input;
using MediaPlayer.ViewModel.ConverterObject;
using System.ComponentModel.Composition;
using Generic;
using MediaPlayer.Common.Constants;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.MediaOpened, typeof(ICommand))]
    public class MediaOpenedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is not MediaOpenedConverterModel mediaOpened)
                return false;

            var vm = mediaOpened.ViewModelMediaPlayer;

            return vm.IsMediaListPopulated && vm.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MediaOpenedConverterModel mediaOpened)
                return;

            PollMediaPosition(mediaOpened);
        }

        private void PollMediaPosition(MediaOpenedConverterModel mediaOpenedModel)
        {
            var mediaElement = mediaOpenedModel.MediaElement;
            var vm = mediaOpenedModel.ViewModelMediaPlayer;

            SetAccurateCurrentMediaDuration(mediaOpenedModel.ViewModelMediaPlayer, mediaElement.NaturalDuration.TimeSpan);

            vm.CurrentPositionTracker.Tick += (sender, args) => TrackMediaPosition(mediaOpenedModel);

            vm.CurrentPositionTracker.Start();
        }

        private void SetAccurateCurrentMediaDuration(MainViewModel vm, TimeSpan duration)
        {
            vm.SelectedMediaItem.Duration = duration;
        }

        private void TrackMediaPosition(MediaOpenedConverterModel mediaOpenedModel)
        {
            var mediaElement = mediaOpenedModel.MediaElement;
            var vm = mediaOpenedModel.ViewModelMediaPlayer;

            if (!vm.IsUserDraggingSeekbarThumb)
                vm.SelectedMediaItem.ElapsedTime = mediaElement.Position;

            if (!vm.IsEndOfCurrentlyPlayingMedia())
                return;

            if (vm.NextTrackCommand.CanExecute(vm))
                vm.NextTrackCommand.Execute(vm);
        }
    }
}
