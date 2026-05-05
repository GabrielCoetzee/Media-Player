using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.View.Components
{
    public partial class QueuePanel : UserControl
    {
        public QueuePanel()
        {
            InitializeComponent();
        }

        private void OnRemoveItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void OnRemoveItemClick(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe || fe.DataContext is not MediaItem item)
                return;

            if (DataContext is MainViewModel vm)
                vm.MediaItems.Remove(item);

            e.Handled = true;
        }
    }
}
