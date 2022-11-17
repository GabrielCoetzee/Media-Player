using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediaPlayer.ViewModel.Services.Abstract
{
    public interface IMetadataAugmenterService
    {
        Task UpdateMetadataAsync(IEnumerable<AudioItem> audioItems, CancellationToken token);
    }
}
