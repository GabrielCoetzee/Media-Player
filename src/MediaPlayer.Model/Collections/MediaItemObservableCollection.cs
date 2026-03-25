using System.Collections.Generic;
using System.Linq;
using Generic.Collections;
using MediaPlayer.Common.Exceptions;
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

        public MediaItemObservableCollection(IOrderedEnumerable<MediaItem> orderedMediaItems)
            :base(orderedMediaItems)
        {
        }

        public new MediaItem this[int index]
        {
            get
            {
                if (Count == 0)
                    throw new EmptyMediaListException();

                return base[index];
            }
            set => base[index] = value;
        }

        public new int IndexOf(MediaItem item)
        {
            if (Count == 0)
                throw new EmptyMediaListException();

            return base.IndexOf(item);
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
