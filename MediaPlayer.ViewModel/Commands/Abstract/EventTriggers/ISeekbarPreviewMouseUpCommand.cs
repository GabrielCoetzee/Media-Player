using MediaPlayer.ViewModel.Commands.Concrete.EventTriggers;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Abstract.EventTriggers
{
    public interface ISeekbarPreviewMouseUpCommand : ICommand
    {
        event EventHandler<SliderPositionEventArgs> ChangeMediaPosition;
    }
}
