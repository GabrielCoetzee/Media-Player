using System.Windows;
using System.Windows.Controls;

namespace MediaPlayer.View.Components
{
    public partial class PlayerShell : UserControl
    {
        public static readonly DependencyProperty IsQueueOpenProperty =
            DependencyProperty.Register(
                nameof(IsQueueOpen),
                typeof(bool),
                typeof(PlayerShell),
                new PropertyMetadata(true));

        public bool IsQueueOpen
        {
            get => (bool)GetValue(IsQueueOpenProperty);
            set => SetValue(IsQueueOpenProperty, value);
        }

        public static readonly DependencyProperty IsLyricsOpenProperty =
            DependencyProperty.Register(
                nameof(IsLyricsOpen),
                typeof(bool),
                typeof(PlayerShell),
                new PropertyMetadata(false));

        public bool IsLyricsOpen
        {
            get => (bool)GetValue(IsLyricsOpenProperty);
            set => SetValue(IsLyricsOpenProperty, value);
        }

        public PlayerShell()
        {
            InitializeComponent();
        }
    }
}
