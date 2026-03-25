using System.Collections.Specialized;
using System.Windows.Controls;

namespace Generic.Controls
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
            ScrollIntoView(SelectedItem);

            base.OnItemsChanged(e);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            ScrollIntoView(SelectedItem);

            base.OnSelectionChanged(e);
        }
    }
}
