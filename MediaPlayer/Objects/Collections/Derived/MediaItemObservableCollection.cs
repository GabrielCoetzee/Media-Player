using System.Collections.Generic;
using System.Linq;
using MediaPlayer.MVVM.Models.Base_Types;

namespace MediaPlayer.Objects.Collections
{
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

            SetMediaItemIds();
        }

        private void SetMediaItemIds()
        {
            foreach (var mediaItem in Items)
                mediaItem.Id = IndexOf(mediaItem);
        }
    }
}
