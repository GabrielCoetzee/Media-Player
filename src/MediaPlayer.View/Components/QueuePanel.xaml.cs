using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MediaPlayer.View.Components
{
    public partial class QueuePanel : UserControl
    {
        public QueuePanel()
        {
            InitializeComponent();
        }

        private void OnMoreButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe || fe.ContextMenu is null)
                return;

            fe.ContextMenu.PlacementTarget = fe;
            fe.ContextMenu.Placement = PlacementMode.Bottom;
            fe.ContextMenu.IsOpen = true;
        }
    }
}
