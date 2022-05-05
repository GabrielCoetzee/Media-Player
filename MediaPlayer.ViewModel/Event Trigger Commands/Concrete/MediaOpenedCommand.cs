using MediaPlayer.ViewModel.Commands.Abstract;
using System;
using System.Windows.Input;
using MediaPlayer.ViewModel.ConverterObject;
using MediaPlayer.ViewModel.EventTriggers.Abstract;

namespace MediaPlayer.ViewModel.EventTriggers.Concrete
{
    public class MediaOpenedCommand : IMediaOpenedCommand
    {
        readonly INextTrackCommand _nextTrackCommand;

        public MediaOpenedCommand(INextTrackCommand nextTrackCommand)
        {
            _nextTrackCommand = nextTrackCommand;
        }

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

            return !vm.IsMediaListEmpty() && vm.SelectedMediaItem != null;
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

            if (!vm.IsEndOfCurrentMedia())
                return;

            if (_nextTrackCommand.CanExecute(vm))
                _nextTrackCommand.Execute(vm);

            RefreshUIBindings();
        }

        private static void RefreshUIBindings()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
