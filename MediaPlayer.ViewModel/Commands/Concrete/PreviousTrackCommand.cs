﻿using MediaPlayer.Model;
using MediaPlayer.ViewModel.Commands.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    public class PreviousTrackCommand : IPreviousTrackCommand
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

            return vm.IsMediaListPopulated && (vm.IsPreviousMediaItemAvailable() || vm.IsRepeatEnabled);
        }

        public void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            PlayPreviousMediaItem(vm);
        }

        private void PlayPreviousMediaItem(MainViewModel vm)
        {
            var index = vm.PreviousMediaItemIndex();

            if (vm.IsRepeatEnabled && vm.IsFirstMediaItemSelected())
                index = vm.LastMediaItemIndex();

            vm.SelectMediaItem(index);
            vm.PlayMedia();
        }
    }
}
