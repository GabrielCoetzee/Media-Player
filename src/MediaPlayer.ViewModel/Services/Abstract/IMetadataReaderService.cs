using MediaPlayer.Model.BusinessEntities.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaPlayer.ViewModel.Services.Abstract
{
    public interface IMetadataReaderService
    {
        Task<IEnumerable<MediaItem>> ReadFilePathsAsync(IEnumerable<string> filePaths);
    }
}
