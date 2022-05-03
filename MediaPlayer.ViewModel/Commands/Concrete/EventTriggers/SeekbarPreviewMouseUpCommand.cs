﻿using MediaPlayer.ViewModel.Commands.Abstract.EventTriggers;
using MediaPlayer.Model;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete.EventTriggers
{
    public class SeekbarPreviewMouseUpCommand : ISeekbarPreviewMouseUpCommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public event EventHandler<SliderPositionEventArgs> ChangeMediaPosition;

        public virtual void OnChangeMediaPosition(SliderPositionEventArgs e)
        {
            ChangeMediaPosition?.Invoke(this, e);
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is not MouseButtonEventArgs e)
                return false;

            var seekbar = e.Source as Slider;

            return seekbar.Value != default;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MouseButtonEventArgs e)
                return;

            var seekbar = e.Source as Slider;

            var pointerLocation = (e.GetPosition(seekbar).X / seekbar.ActualWidth) * (seekbar.Maximum - seekbar.Minimum);

            OnChangeMediaPosition(new SliderPositionEventArgs() { Position = pointerLocation });
        }
    }

    public class SliderPositionEventArgs : EventArgs
    {
        public double Position { get; set; }
    }
}
