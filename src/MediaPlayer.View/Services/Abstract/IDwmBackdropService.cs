using System.Windows;
using MediaPlayer.Common.Enumerations;

namespace MediaPlayer.View.Services.Abstract
{
    public interface IDwmBackdropService
    {
        bool IsSupported { get; }
        void ApplyBackdrop(Window window, DwmBackdropType backdropType);
        void RemoveBackdrop(Window window);
    }
}
