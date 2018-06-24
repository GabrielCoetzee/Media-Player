using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MediaPlayer.Interfaces;

namespace MediaPlayer.MVVM.Views
{
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
