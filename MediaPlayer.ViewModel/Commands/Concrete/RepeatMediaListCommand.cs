﻿using MediaPlayer.Model;
using MediaPlayer.ViewModel.Commands.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    public class RepeatMediaListCommand : IRepeatMediaListCommand
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

            return vm.IsMediaListPopulated && vm.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            vm.IsRepeatEnabled = !vm.IsRepeatEnabled;
        }
    }
}