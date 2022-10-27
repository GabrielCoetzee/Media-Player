using System.Collections.Generic;
using System.Linq;
using Generic.Collections;
using MediaPlayer.Model.BusinessEntities.Abstract;

namespace MediaPlayer.Model.Collections
{
    /// <inheritdoc />
    /// <summary>
    /// This sets each item in the list`s ID to that of it`s index after AddRange. This is important as it is used after shuffle to revert to original position.
    /// </summary>
    public class MediaItemObservableCollection : BulkObservableCollection<MediaItem>
    {
        public MediaItemObservableCollection()
        {
        }

        public MediaItemObservableCollection(IOrderedEnumerable<MediaItem> iOrderedEnumerable)
            :base(iOrderedEnumerable)
        {
        }

        public override void AddRange(IEnumerable<MediaItem> list)
        {
            base.AddRange(list);

            SetMediaItemIds(Items);
        }

        private void SetMediaItemIds(IEnumerable<MediaItem> mediaItems)
        {
            if (mediaItems.All(x => x.Id != null))
                return;

            mediaItems.Where(x => x.Id == null).ToList().ForEach(x => x.Id = IndexOf(x));
        }
    }
}
