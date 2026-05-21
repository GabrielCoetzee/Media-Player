using MediaPlayer.Model.BusinessEntities.Abstract;
using System.Collections.Generic;

namespace MediaPlayer.ViewModel.Services.Abstract
{
    public interface IMediaLoader
    {
        IAsyncEnumerable<IReadOnlyList<MediaItem>> LoadInBatchesAsync(IEnumerable<string> paths);

        void Cancel();
    }
}
