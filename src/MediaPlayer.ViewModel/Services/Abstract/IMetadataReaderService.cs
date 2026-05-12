using MediaPlayer.Model.BusinessEntities.Abstract;
using System.Collections.Generic;
using System.Threading;

namespace MediaPlayer.ViewModel.Services.Abstract
{
    public interface IMetadataReaderService
    {
        IAsyncEnumerable<MediaItem> EnumerateMediaItemsAsync(IEnumerable<string> paths, CancellationToken cancellationToken = default);
    }
}
