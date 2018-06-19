using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPlayer.MVVM.Models.Base_Types;

namespace MediaPlayer.Objects.Collections
{
    public class MediaListObservableCollection : BulkObservableCollection<MediaItem>
    {
        public MediaListObservableCollection()
        {
        }

        public MediaListObservableCollection(IOrderedEnumerable<MediaItem> iOrderedEnumerable)
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
