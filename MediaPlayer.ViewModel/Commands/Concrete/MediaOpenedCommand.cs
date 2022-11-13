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
            if (parameter is not MediaOpenedConverterModel model)
                return false;

            var vm = model.MainViewModel;

            return vm.IsMediaListPopulated && vm.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MediaOpenedConverterModel model)
                return;

            PollMediaPosition(model);
        }

        private void PollMediaPosition(MediaOpenedConverterModel model)
        {
            var mediaElement = model.MediaElement;
            var vm = model.MainViewModel;

            SetAccurateCurrentMediaDuration(vm, mediaElement.NaturalDuration.TimeSpan);

            vm.PositionTracker.Tick += (sender, args) => TrackMediaPosition(model);

            vm.PositionTracker.Start();
        }

        private void SetAccurateCurrentMediaDuration(MainViewModel vm, TimeSpan duration)
        {
            vm.SelectedMediaItem.Duration = duration;
        }

        private void TrackMediaPosition(MediaOpenedConverterModel model)
        {
            var mediaElement = model.MediaElement;
            var vm = model.MainViewModel;

            if (!vm.MediaControlsViewModel.IsUserDraggingSeekbarThumb)
                vm.SelectedMediaItem.ElapsedTime = mediaElement.Position;

            if (!vm.IsEndOfCurrentlyPlayingMedia())
                return;

            if (vm.MediaControlsViewModel.NextTrackCommand.CanExecute(vm))
                vm.MediaControlsViewModel.NextTrackCommand.Execute(vm);
        }
    }
}
