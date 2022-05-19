using MediaPlayer.Common.Constants;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.EventTriggers.Concrete
{
    [Export(CommandNames.TopMostGridDragEnter, typeof(ICommand))]
    public class TopMostGridDragEnterCommand : ICommand
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
