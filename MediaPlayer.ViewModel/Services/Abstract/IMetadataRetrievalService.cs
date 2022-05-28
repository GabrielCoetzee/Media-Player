using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaPlayer.ViewModel.Services.Abstract
{
    public interface IMetadataRetrievalService
    {
        Task GetMetadataAsync(IEnumerable<AudioItem> audioItems);
    }
}
