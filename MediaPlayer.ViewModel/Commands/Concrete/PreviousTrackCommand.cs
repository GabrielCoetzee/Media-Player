﻿using MediaPlayer.Common.Constants;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.PreviousTrack, typeof(ICommand))]
    public class PreviousTrackCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return false;

            return vm.IsMediaListPopulated && (vm.IsPreviousMediaItemAvailable() || vm.MediaControlsViewModel.IsRepeatEnabled);
        }

        public void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            PlayPreviousMediaItem(vm);
        }

        private void PlayPreviousMediaItem(MainViewModel vm)
        {
            var index = vm.GetPreviousMediaItemIndex();

            if (vm.MediaControlsViewModel.IsRepeatEnabled && vm.IsFirstMediaItemSelected())
                index = vm.GetLastMediaItemIndex();

            vm.SelectMediaItem(index);
            vm.MediaControlsViewModel.SetPlaybackState(MediaState.Play);
        }
    }
}
