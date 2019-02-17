using System.Collections.Generic;
using System.Linq;
using MediaPlayer.BusinessEntities.Collections.Abstract;
using MediaPlayer.BusinessEntities.Objects.Abstract;

namespace MediaPlayer.BusinessEntities.Collections.Derived
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

        public new void AddRange(IEnumerable<MediaItem> list)
        {
            base.AddRange(list);

            this.SetMediaItemIds();
        }

        private void SetMediaItemIds()
        {
            foreach (var mediaItem in Items)
            {
                mediaItem.Id = IndexOf(mediaItem);
            }
        }
    }
}
