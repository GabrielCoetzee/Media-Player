using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.DataAccess.Abstract;
using MediaPlayer.ViewModel.Services.Abstract;
using MediaPlayer.Model.Metadata.Concrete;
using MediaPlayer.Common.Enumerations;
using System.Linq;

namespace MediaPlayer.ViewModel.Services.Concrete
{
    [Export(typeof(IMetadataUpdateService))]
    public class MetadataUpdateService : IMetadataUpdateService
    {
        readonly ILastFmDataAccess _lastFmDataAccess;
        readonly ILyricsOvhDataAccess _lyricsOvhDataAccess;
        readonly MetadataWriterFactory _metadataWriterFactory;

        [ImportingConstructor]
        public MetadataUpdateService(ILastFmDataAccess lastFmDataAccess,
            ILyricsOvhDataAccess lyricsOvhDataAccess,
            MetadataWriterFactory metadataWriterFactory)
        {
            _lastFmDataAccess = lastFmDataAccess;
            _lyricsOvhDataAccess = lyricsOvhDataAccess;
            _metadataWriterFactory = metadataWriterFactory;
        }

        public async Task UpdateMetadataAsync(IEnumerable<AudioItem> audioItems)
        {
            foreach (var audioItem in audioItems)
            {
                if (audioItem.HasLyrics)
                    continue;

                var response = await _lyricsOvhDataAccess.GetLyricsAsync(audioItem.Artist, audioItem.MediaTitle);
                audioItem.Lyrics = response?.Lyrics;

                audioItem.IsDirty = true;
            }

            await Task.Run(() =>
            {
                Parallel.ForEach(audioItems.Where(x => x.IsDirty), (x) => x.Update(_metadataWriterFactory.Resolve(MetadataLibraries.Taglib)));
            });
        }
    }
}
