﻿using System.ComponentModel;
using System.Linq;
using System.Windows;
using ControlzEx.Theming;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.Theming;
using MediaPlayer.ViewModel;

namespace MediaPlayer.View.Views
{
    /// <inheritdoc cref="" />
    /// <summary>
    /// Interaction logic for ViewApplicationSettings.xaml
    /// </summary>
    public partial class ViewApplicationSettings : MetroWindow
    {
        #region Bindable Properties

        readonly ISettingsProvider SettingsProvider;

        #endregion

        public ViewApplicationSettings(ViewModelApplicationSettings vm, ISettingsProvider settingsProvider)
        {
            InitializeComponent();

            DataContext = vm;

            SettingsProvider = settingsProvider;
        }

        private void ButtonCloseSettings_Click(object sender, RoutedEventArgs e)
        {
            this.SettingsProvider.SaveSettings();

            this.Close();
        }
    }
}
