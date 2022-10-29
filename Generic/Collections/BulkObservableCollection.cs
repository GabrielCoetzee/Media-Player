using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Generic.Collections
{
    /// <inheritdoc />
    /// <summary>
    /// The Point of this class is to be able to use AddRange, and then still have it notify that collection has changed on each individual item add.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BulkObservableCollection<T> : ObservableCollection<T>
    {
        public BulkObservableCollection()
        {
        }

        public BulkObservableCollection(IOrderedEnumerable<T> iOrderedEnumerable)
            :base(iOrderedEnumerable)
        {
        }

        private bool _suppressNotification = false;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_suppressNotification)
                return;

            base.OnCollectionChanged(e);
        }

        public virtual void AddRange(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            _suppressNotification = true;

            foreach (T item in list)
                Add(item);

            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void RemoveRange(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            _suppressNotification = true;

            foreach (T item in list)
                Remove(item);

            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
