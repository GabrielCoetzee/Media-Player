using MediaPlayer.ViewModel.ViewModels;
using System.Windows.Controls;

namespace MediaPlayer.ViewModel.ConverterObject
{
    public class SeekConverterModel
    {
        public MediaControlsViewModel MediaControlsViewModel { get; set; }

        public Slider Seekbar { get; set; }
    }
}
