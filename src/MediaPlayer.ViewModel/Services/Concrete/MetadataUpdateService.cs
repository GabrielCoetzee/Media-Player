using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.ViewModel.Services.Abstract;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Metadata.Abstract.Updaters;
using Generic.Extensions;
using Generic.Mediator;
using System;

namespace MediaPlayer.ViewModel.Services.Concrete
{
    [Export(typeof(IMetadataUpdateService))]
    public class MetadataUpdateService : IMetadataUpdateService
    {
        const int MaxConcurrentRequests = 8;

        readonly IAlbumArtMetadataUpdater _albumArtMetadataUpdater;
        readonly ILyricsMetadataUpdater _lyricsMetadataUpdater;

        [ImportingConstructor]
        public MetadataUpdateService([Import(ServiceNames.LastFmAlbumArtMetadataUpdater)] IAlbumArtMetadataUpdater albumArtMetadataUpdater,
            [Import(ServiceNames.LyricsOvhMetadataUpdater)] ILyricsMetadataUpdater lyricsMetadataUpdater)
        {
            _albumArtMetadataUpdater = albumArtMetadataUpdater;
            _lyricsMetadataUpdater = lyricsMetadataUpdater;
        }

        public async Task UpdateMetadataAsync(IEnumerable<AudioItem> audioItems, CancellationToken token)
        {
            var updateAlbumArtTask = UpdateAlbumArtAsync(audioItems.Where(x => !x.HasAlbumArt), token);
            var updateLyricsTask = UpdateLyricsAsync(audioItems.Where(x => !x.HasLyrics), token);

            await Task.WhenAll(updateAlbumArtTask, updateLyricsTask);
        }

        private async Task UpdateLyricsAsync(IEnumerable<AudioItem> audioItems, CancellationToken token)
        {
            var updateItems = audioItems.ToList();
            var lyricsDictionary = new ConcurrentDictionary<string, string>();

            await Task.Run(async () =>
            {

                try
                {
                    var parallelOptions = new ParallelOptions 
                    { 
                        MaxDegreeOfParallelism = MaxConcurrentRequests, 
                        CancellationToken = token 
                    };

                    await Parallel.ForEachAsync(updateItems, parallelOptions, async (audioItem, token) =>
                    {
                        token.ThrowIfCancellationRequested();

                        var lyrics = await _lyricsMetadataUpdater.GetLyricsAsync(audioItem.Artist, audioItem.MediaTitle);

                        if (string.IsNullOrEmpty(lyrics))
                            return;

                        lyricsDictionary[audioItem.FileName] = lyrics;
                    });
                }
                catch (TaskCanceledException)
                {
                    //If a task is canceled that's okay, user probably cleared media list and that's a valid case
                }
                catch (OperationCanceledException)
                {
                    //If a task is canceled that's okay, user probably cleared media list and that's a valid case
                }

            }, token);

            updateItems.ForEach(x => x.EnrichLyrics(lyricsDictionary.GetValueOrDefault(x.FileName)));
        }

        private async Task UpdateAlbumArtAsync(IEnumerable<AudioItem> audioItems, CancellationToken token)
        {
            var updateItems = audioItems.ToList();
            var albumArtDictionary = new ConcurrentDictionary<string, byte[]>();

            await Task.Run(async () =>
            {
                try
                {
                    var parallelOptions = new ParallelOptions 
                    { 
                        MaxDegreeOfParallelism = MaxConcurrentRequests, 
                        CancellationToken = token 
                    };

                    await Parallel.ForEachAsync(updateItems, parallelOptions, async (audioItem, token) =>
                    {
                        token.ThrowIfCancellationRequested();

                        var albumArt = await _albumArtMetadataUpdater.GetAlbumArtAsync(audioItem.Artist, audioItem.MediaTitle);

                        if (albumArt.IsNullOrEmpty())
                            return;

                        albumArtDictionary[audioItem.FileName] = albumArt;
                    });
                }
                catch (TaskCanceledException)
                {
                    //If a task is canceled that's okay, user probably cleared media list and that's a valid case
                }
                catch (OperationCanceledException)
                {
                    //If a task is canceled that's okay, user probably cleared media list and that's a valid case
                }

            }, token);

            updateItems.ForEach(x => x.EnrichAlbumArt(albumArtDictionary.GetValueOrDefault(x.FileName)));

            Messenger<MessengerMessages>.Send(MessengerMessages.AutoAdjustAccent);
        }
    }
}