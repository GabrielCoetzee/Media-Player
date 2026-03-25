using MediaPlayer.ViewModel.Commands.Concrete;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Abstract
{
    public interface ISeekbarPreviewMouseUpCommand : ICommand
    {
        event EventHandler<SliderPositionEventArgs> ChangeMediaPosition;
    }
}
