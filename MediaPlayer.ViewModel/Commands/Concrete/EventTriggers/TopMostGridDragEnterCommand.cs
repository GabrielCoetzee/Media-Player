using MediaPlayer.ViewModel.Commands.Abstract.EventTriggers;
using System;
using System.Windows;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete.EventTriggers
{
    public class TopMostGridDragEnterCommand : ITopMostGridDragEnterCommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is not DragEventArgs e)
                return;

            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Move : DragDropEffects.None;
        }
    }
}
