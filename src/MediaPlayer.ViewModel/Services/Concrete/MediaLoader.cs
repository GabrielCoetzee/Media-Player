using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.ViewModel.Services.Abstract;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading;

namespace MediaPlayer.ViewModel.Services.Concrete
{
    [Export(typeof(IMediaLoader))]
    public class MediaLoader : IMediaLoader
    {
        private const int FlushBatchSize = 25;
        private const int FlushIntervalMs = 150;

        private readonly IMetadataReaderService _metadataReader;
        private readonly List<CancellationTokenSource> _tokenSources = [];

        [ImportingConstructor]
        public MediaLoader(IMetadataReaderService metadataReader)
        {
            _metadataReader = metadataReader;
        }

        public async IAsyncEnumerable<IReadOnlyList<MediaItem>> LoadInBatchesAsync(IEnumerable<string> paths)
        {
            var cts = new CancellationTokenSource();
            _tokenSources.Add(cts);

            var pendingItems = new List<MediaItem>();
            var sinceLastFlush = Stopwatch.StartNew();

            try
            {
                await foreach (var mediaItem in _metadataReader.EnumerateMediaItemsAsync(paths, cts.Token))
                {
                    pendingItems.Add(mediaItem);

                    if (pendingItems.Count < FlushBatchSize && sinceLastFlush.ElapsedMilliseconds < FlushIntervalMs)
                        continue;

                    yield return pendingItems.ToArray();

                    pendingItems.Clear();
                    sinceLastFlush.Restart();
                }

                if (pendingItems.Count > 0)
                    yield return pendingItems.ToArray();
            }
            finally
            {
                _tokenSources.Remove(cts);
                cts.Dispose();
            }
        }

        public void Cancel() => _tokenSources.ForEach(x => x.Cancel());
    }
}
