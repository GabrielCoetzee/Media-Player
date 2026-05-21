using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.ViewModel.Services.Abstract;
using MediaPlayer.ViewModel.Services.Concrete;
using Moq;
using NUnit.Framework;
using System.Runtime.CompilerServices;

namespace MediaPlayer.ViewModel.Test.ServicesTests
{
    [TestFixture]
    public class MediaLoaderTests
    {
        Mock<IMetadataReaderService> _metadataReaderMock;
        MediaLoader _loader;

        [SetUp]
        public void SetUp()
        {
            _metadataReaderMock = new Mock<IMetadataReaderService>();
            _loader = new MediaLoader(_metadataReaderMock.Object);
        }

        [Test]
        public async Task LoadInBatchesAsync_FewerItemsThanBatchSize_YieldsSingleBatchOnCompletion()
        {
            var items = Enumerable.Range(0, 5).Select(i => (MediaItem)new AudioItem { Id = i }).ToList();

            _metadataReaderMock
                .Setup(x => x.EnumerateMediaItemsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .Returns(ToAsyncEnumerable(items));

            var batches = await CollectBatchesAsync(_loader.LoadInBatchesAsync(new[] { "anywhere" }));

            Assert.Multiple(() =>
            {
                Assert.That(batches, Has.Count.EqualTo(1));
                Assert.That(batches[0], Has.Count.EqualTo(5));
            });
        }

        [Test]
        public async Task LoadInBatchesAsync_ExactlyBatchSizeItems_YieldsSingleBatchAtThreshold()
        {
            var items = Enumerable.Range(0, 25).Select(i => (MediaItem)new AudioItem { Id = i }).ToList();

            _metadataReaderMock
                .Setup(x => x.EnumerateMediaItemsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .Returns(ToAsyncEnumerable(items));

            var batches = await CollectBatchesAsync(_loader.LoadInBatchesAsync(new[] { "anywhere" }));

            Assert.That(batches, Has.Count.EqualTo(1));
            Assert.That(batches[0], Has.Count.EqualTo(25));
        }

        [Test]
        public async Task LoadInBatchesAsync_MoreItemsThanBatchSize_YieldsMultipleBatches()
        {
            var items = Enumerable.Range(0, 60).Select(i => (MediaItem)new AudioItem { Id = i }).ToList();

            _metadataReaderMock
                .Setup(x => x.EnumerateMediaItemsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .Returns(ToAsyncEnumerable(items));

            var batches = await CollectBatchesAsync(_loader.LoadInBatchesAsync(new[] { "anywhere" }));

            Assert.That(batches.SelectMany(b => b).Count(), Is.EqualTo(60));
            Assert.That(batches.Count, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void LoadInBatchesAsync_ReaderThrowsOperationCanceled_PropagatesAndDiscardsPartialBatch()
        {
            _metadataReaderMock
                .Setup(x => x.EnumerateMediaItemsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .Returns(ItemsThenThrow(3));

            var collected = new List<IReadOnlyList<MediaItem>>();

            Assert.That(async () =>
            {
                await foreach (var batch in _loader.LoadInBatchesAsync(new[] { "anywhere" }))
                    collected.Add(batch);
            }, Throws.InstanceOf<OperationCanceledException>());

            Assert.That(collected, Is.Empty, "Partial batch below threshold must be discarded on cancellation.");
        }

        [Test]
        public void Cancel_DuringActiveLoad_CancelsTokenPassedToReader()
        {
            CancellationToken capturedToken = default;

            _metadataReaderMock
                .Setup(x => x.EnumerateMediaItemsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<string> _, CancellationToken ct) =>
                {
                    capturedToken = ct;
                    return BlockedUntilCancelled(ct);
                });

            var enumeration = Task.Run(async () =>
            {
                try
                {
                    await foreach (var _ in _loader.LoadInBatchesAsync(new[] { "anywhere" })) { }
                }
                catch (OperationCanceledException) { }
            });

            SpinWait.SpinUntil(() => capturedToken != default && capturedToken.CanBeCanceled, TimeSpan.FromSeconds(2));

            _loader.Cancel();

            Assert.That(enumeration.Wait(TimeSpan.FromSeconds(2)), Is.True);
            Assert.That(capturedToken.IsCancellationRequested, Is.True);
        }

        [Test]
        public async Task LoadInBatchesAsync_CallAgainAfterCancel_Succeeds()
        {
            _metadataReaderMock
                .Setup(x => x.EnumerateMediaItemsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .Returns(ToAsyncEnumerable(new[] { (MediaItem)new AudioItem { Id = 1 } }));

            _loader.Cancel();

            var batches = await CollectBatchesAsync(_loader.LoadInBatchesAsync(new[] { "anywhere" }));

            Assert.That(batches, Has.Count.EqualTo(1));
        }

        private static async Task<List<IReadOnlyList<MediaItem>>> CollectBatchesAsync(IAsyncEnumerable<IReadOnlyList<MediaItem>> source)
        {
            var batches = new List<IReadOnlyList<MediaItem>>();

            await foreach (var batch in source)
                batches.Add(batch);

            return batches;
        }

        private static async IAsyncEnumerable<MediaItem> ToAsyncEnumerable(IEnumerable<MediaItem> items)
        {
            await Task.CompletedTask;

            foreach (var item in items)
                yield return item;
        }

        private static async IAsyncEnumerable<MediaItem> ItemsThenThrow(int count)
        {
            await Task.CompletedTask;

            for (var i = 0; i < count; i++)
                yield return new AudioItem { Id = i };

            throw new OperationCanceledException();
        }

        private static async IAsyncEnumerable<MediaItem> BlockedUntilCancelled([EnumeratorCancellation] CancellationToken token)
        {
            var tcs = new TaskCompletionSource();

            using var registration = token.Register(() => tcs.TrySetResult());

            await tcs.Task;

            token.ThrowIfCancellationRequested();

            yield break;
        }
    }
}
