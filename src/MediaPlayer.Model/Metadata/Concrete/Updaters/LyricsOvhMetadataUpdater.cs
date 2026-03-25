using Integration.LyricsOVH.Services.Abstract;
using MediaPlayer.Common.Constants;
using MediaPlayer.Model.Metadata.Abstract.Updaters;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace MediaPlayer.Model.Metadata.Concrete.Updaters
{
    [Export(ServiceNames.LyricsOvhMetadataUpdater, typeof(ILyricsMetadataUpdater))]
    public class LyricsOvhMetadataUpdater : ILyricsMetadataUpdater
    {
        readonly ILyricsOvhApi _lyricsOvhApi;

        [ImportingConstructor]
        public LyricsOvhMetadataUpdater(ILyricsOvhApi lyricsOvhApi)
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
