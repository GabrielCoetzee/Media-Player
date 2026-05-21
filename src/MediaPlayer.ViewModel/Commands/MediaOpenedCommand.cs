using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.ConverterObject;
using MediaPlayer.ViewModel.ViewModels;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using System.Windows.Threading;

namespace MediaPlayer.ViewModel.Commands
{
    [Export(CommandNames.MediaOpened, typeof(ICommand))]
    public class MediaOpenedCommand : ICommand
    {
        private readonly DispatcherTimer _positionTracker = new() { Interval = TimeSpan.FromMilliseconds(200) };
        private EventHandler _currentTickHandler;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is not MediaOpenedConverterModel model)
                return false;

            return model.QueueViewModel != null
                && model.QueueViewModel.IsMediaListPopulated
                && model.QueueViewModel.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MediaOpenedConverterModel model)
                return;

            PollMediaPosition(model);
        }

        private void PollMediaPosition(MediaOpenedConverterModel model)
        {
            SetAccurateCurrentMediaDuration(model.QueueViewModel, model.MediaElement.NaturalDuration.TimeSpan);

            if (_currentTickHandler != null)
                _positionTracker.Tick -= _currentTickHandler;

            _currentTickHandler = (sender, args) => TrackMediaPosition(model);
            _positionTracker.Tick += _currentTickHandler;

            _positionTracker.Start();
        }

        private void SetAccurateCurrentMediaDuration(QueueViewModel queue, TimeSpan duration)
        {
            queue.SelectedMediaItem.Duration = duration;
        }

        private void TrackMediaPosition(MediaOpenedConverterModel model)
        {
            var queue = model.QueueViewModel;
            var controls = model.MediaControlsViewModel;

            if (queue.SelectedMediaItem == null)
                return;

            if (!controls.IsUserDraggingSeekbarThumb)
                queue.SelectedMediaItem.ElapsedTime = model.MediaElement.Position;

            if (queue.SelectedMediaItem.ElapsedTime < queue.SelectedMediaItem.Duration)
                return;

            if (controls.NextTrackCommand.CanExecute(controls))
                controls.NextTrackCommand.Execute(controls);
        }
    }
}
