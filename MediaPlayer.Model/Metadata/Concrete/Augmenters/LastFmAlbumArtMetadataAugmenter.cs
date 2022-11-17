using MediaPlayer.Common.Constants;
using MediaPlayer.DataAccess.Abstract;
using MediaPlayer.Model.Metadata.Abstract.Augmenters;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace MediaPlayer.Model.Metadata.Concrete.Augmenters
{
    [Export(ServiceNames.LastFmAlbumArtMetadataAugmenter, typeof(IAlbumArtMetadataAugmenter))]
    public class LastFmAlbumArtMetadataAugmenter : IAlbumArtMetadataAugmenter
    {
        readonly ILastFMApi _lastFmApi;

        [ImportingConstructor]
        public LastFmAlbumArtMetadataAugmenter(ILastFMApi lastFmApi)
        {
            _lastFmApi = lastFmApi;
        }

        public async Task<string> GetAlbumArtAsync(string artist, string track)
        {
            var response = await _lastFmApi.GetTrackInfoAsync(artist, track);

            return response?.Track?.Album?.Image?.LastOrDefault()?.Url;
        }
    }
}
