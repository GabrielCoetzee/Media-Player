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

            var vm = mediaOpened.MainViewModel;

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
            var vm = mediaOpenedModel.MainViewModel;

            SetAccurateCurrentMediaDuration(vm, mediaElement.NaturalDuration.TimeSpan);

            vm.PositionTracker.Tick += (sender, args) => TrackMediaPosition(mediaOpenedModel);

            vm.PositionTracker.Start();
        }

        private void SetAccurateCurrentMediaDuration(MainViewModel vm, TimeSpan duration)
        {
            vm.SelectedMediaItem.Duration = duration;
        }

        private void TrackMediaPosition(MediaOpenedConverterModel mediaOpenedModel)
        {
            var mediaElement = mediaOpenedModel.MediaElement;
            var vm = mediaOpenedModel.MainViewModel;

            if (!vm.MediaControlsViewModel.IsUserDraggingSeekbarThumb)
                vm.SelectedMediaItem.ElapsedTime = mediaElement.Position;

            if (!vm.IsEndOfCurrentlyPlayingMedia())
                return;

            if (vm.MediaControlsViewModel.NextTrackCommand.CanExecute(vm))
                vm.MediaControlsViewModel.NextTrackCommand.Execute(vm);
        }
    }
}
