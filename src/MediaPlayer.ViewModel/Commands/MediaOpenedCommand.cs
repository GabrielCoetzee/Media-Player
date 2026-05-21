using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.ConverterObject;
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
        private MediaOpenedConverterModel _model;

        public MediaOpenedCommand()
        {
            _positionTracker.Tick += OnPositionTrackerTick;
        }

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

            _model = model;
            SetAccurateDuration();

            _positionTracker.Start();
        }

        private void SetAccurateDuration()
        {
            _model.QueueViewModel.SelectedMediaItem.Duration = _model.MediaElement.NaturalDuration.TimeSpan;
        }

        private void OnPositionTrackerTick(object sender, EventArgs e)
        {
            var queue = _model.QueueViewModel;
            var controls = _model.MediaControlsViewModel;

            if (queue.SelectedMediaItem == null)
                return;

            if (!controls.IsUserDraggingSeekbarThumb)
                queue.SelectedMediaItem.ElapsedTime = _model.MediaElement.Position;

            if (queue.SelectedMediaItem.ElapsedTime < queue.SelectedMediaItem.Duration)
                return;

            if (controls.NextTrackCommand.CanExecute(controls))
                controls.NextTrackCommand.Execute(controls);
        }
    }
}
