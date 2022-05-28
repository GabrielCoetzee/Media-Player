using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.DataAccess.Abstract;
using MediaPlayer.ViewModel.Services.Abstract;
using MediaPlayer.Model.Metadata.Concrete;
using MediaPlayer.Common.Enumerations;

namespace MediaPlayer.ViewModel.Services.Concrete
{
    [Export(typeof(IMetadataRetrievalService))]
    public class MetadataRetrievalService : IMetadataRetrievalService
    {
        readonly ILastFmDataAccess _lastFmDataAccess;
        readonly ILyricsOvhDataAccess _lyricsOvhDataAccess;

        [ImportingConstructor]
        public MetadataRetrievalService(ILastFmDataAccess lastFmDataAccess, 
            ILyricsOvhDataAccess lyricsOvhDataAccess)
        {
            _lastFmDataAccess = lastFmDataAccess;
            _lyricsOvhDataAccess = lyricsOvhDataAccess;
        }

        public async Task GetMetadataAsync(IEnumerable<AudioItem> audioItems)
        {
            foreach (var audioItem in audioItems)
            {
                //var response = await _lastFmDataAccess.GetTrackInfoAsync(audioItem.Artist, audioItem.MediaTitle);
                if (!audioItem.HasLyrics)
                {
                    var response = await _lyricsOvhDataAccess.GetLyricsAsync(audioItem.Artist, audioItem.MediaTitle);
                    audioItem.Lyrics = response?.Lyrics;

                    audioItem.IsDirty = true;
                }
            }
        }
    }
}
