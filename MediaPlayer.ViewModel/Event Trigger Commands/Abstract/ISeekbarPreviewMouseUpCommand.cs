using MediaPlayer.ViewModel.EventTriggers.Concrete;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.EventTriggers.Abstract
{
    public interface ISeekbarPreviewMouseUpCommand : ICommand
    {
        event EventHandler<SliderPositionEventArgs> ChangeMediaPosition;
    }
}
