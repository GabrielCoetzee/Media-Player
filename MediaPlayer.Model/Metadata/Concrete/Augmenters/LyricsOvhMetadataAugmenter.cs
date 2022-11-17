using Integration.LyricsOVH.Services.Abstract;
using MediaPlayer.Common.Constants;
using MediaPlayer.Model.Metadata.Abstract.Augmenters;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace MediaPlayer.Model.Metadata.Concrete.Augmenters
{
    [Export(ServiceNames.LyricsOvhMetadataAugmenter, typeof(ILyricsMetadataAugmenter))]
    public class LyricsOvhMetadataAugmenter : ILyricsMetadataAugmenter
    {
        readonly ILyricsOvhApi _lyricsOvhApi;

        [ImportingConstructor]
        public LyricsOvhMetadataAugmenter(ILyricsOvhApi lyricsOvhApi)
        {
            _lyricsOvhApi = lyricsOvhApi;
        }

        public async Task<string> GetLyricsAsync(string artist, string track)
        {
            var response = await _lyricsOvhApi.GetLyricsAsync(artist, track);

            return response?.Lyrics;
        }
    }
}
