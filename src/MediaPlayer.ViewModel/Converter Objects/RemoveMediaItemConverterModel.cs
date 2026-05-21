using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.ViewModel.ViewModels;

namespace MediaPlayer.ViewModel.ConverterObject
{
    public class RemoveMediaItemConverterModel
    {
        public QueueViewModel QueueViewModel { get; set; }

        public MediaItem MediaItem { get; set; }
    }
}
