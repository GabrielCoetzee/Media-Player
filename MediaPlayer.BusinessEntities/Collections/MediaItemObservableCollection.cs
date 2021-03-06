﻿using System.Collections.Generic;
using System.Linq;
using Generic.Collections;
using MediaPlayer.BusinessEntities.Objects.Base;

namespace MediaPlayer.BusinessEntities.Collections
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

            this.SetMediaItemIds(this.Items);
        }

        private void SetMediaItemIds(IEnumerable<MediaItem> mediaItems)
        {
            foreach (var mediaItem in mediaItems)
            {
                mediaItem.Id = this.IndexOf(mediaItem);
            }
        }
    }
}
