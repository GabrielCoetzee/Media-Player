using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.ViewModel.Services.Abstract;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;
using MediaPlayer.Common.Constants;
using MediaPlayer.Model.Metadata.Abstract.Updaters;
using Generic.Extensions;
using System;

namespace MediaPlayer.ViewModel.Services.Concrete
{
    [Export(typeof(IMetadataUpdateService))]
    public class MetadataUpdateService : IMetadataUpdateService
    {
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
            var updateAlbumArtTask = UpdateAlbumArtAsync(audioItems, token);
            var updateLyricsTask = UpdateLyricsAsync(audioItems, token);

            await Task.WhenAll(updateAlbumArtTask, updateLyricsTask);
        }

        private async Task UpdateLyricsAsync(IEnumerable<AudioItem> audioItems, CancellationToken token)
        {
            var lyricsDictionary = new ConcurrentDictionary<string, string>();

            await Task.Run(async () => {

                try
                {
                    await Parallel.ForEachAsync(audioItems, token, async (audioItem, token) =>
                    {
                        token.ThrowIfCancellationRequested();

                        if (audioItem.HasLyrics)
                            return;

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

            var updateItems = audioItems.Where(x => !x.HasLyrics).ToList();

            updateItems.ForEach(x => x.Lyrics = lyricsDictionary.GetValueOrDefault(x.FileName));
        }

        private async Task UpdateAlbumArtAsync(IEnumerable<AudioItem> audioItems, CancellationToken token)
        {
            var albumArtDictionary = new ConcurrentDictionary<string, byte[]>();

            await Task.Run(async () => {

                try
                {
                    await Parallel.ForEachAsync(audioItems, token, async (audioItem, token) =>
                    {
                        token.ThrowIfCancellationRequested();

                        if (audioItem.HasAlbumArt)
                            return;

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

            var updateItems = audioItems.Where(x => !x.HasAlbumArt).ToList();

            updateItems.ForEach(x => x.AlbumArt = albumArtDictionary.GetValueOrDefault(x.FileName));
        }
    }
}