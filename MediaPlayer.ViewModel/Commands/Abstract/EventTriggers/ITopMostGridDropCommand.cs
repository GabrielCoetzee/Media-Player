using MediaPlayer.ViewModel.Commands.Concrete.EventTriggers;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Abstract.EventTriggers
{
    public interface ITopMostGridDropCommand : ICommand
    {
        event EventHandler<ProcessDroppedContentEventArgs> ProcessDroppedContent;
    }
}
