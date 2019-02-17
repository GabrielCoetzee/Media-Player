using System.ComponentModel;
using System.Windows;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MediaPlayer.ApplicationSettings.Interfaces;

namespace MediaPlayer.MVVM.Views
{
    /// <inheritdoc cref="" />
    /// <summary>
    /// Interaction logic for ViewApplicationSettings.xaml
    /// </summary>
    public partial class ViewApplicationSettings : MetroWindow
    {
        public ViewApplicationSettings()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var accent in ThemeManager.Accents)
            {
                ComboBoxAccents.Items.Add(accent.Name);
            }
        }

        private void ButtonCloseSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!(DataContext is IExposeApplicationSettings settings))
                return;

            settings.SaveSettings();

            base.OnClosing(e);
        }
    }
}
