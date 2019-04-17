using System.Collections.Specialized;
using System.Windows.Controls;

namespace MediaPlayer.Generic.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// The Point of this class is to automatically scroll to the selected item in the listView, every time items in the listview changes. This is useful when list is shuffled and ordered.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutoScrollListView : ListView
    {
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            this.ScrollIntoView(this.SelectedItem);

            base.OnItemsChanged(e);
        }
    }
}
