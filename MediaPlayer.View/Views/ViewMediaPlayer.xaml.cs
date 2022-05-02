using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.BusinessEntities.Objects.Base;
using MediaPlayer.BusinessLogic;
using MediaPlayer.BusinessLogic.Services.Abstract;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Theming;
using MediaPlayer.ViewModel;

namespace MediaPlayer.View.Views
{
    /// <inheritdoc cref="" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ViewMediaPlayer : MetroWindow
    {
        public ViewMediaPlayer(ViewModelMediaPlayer vm)
        {
            InitializeComponent();

            DataContext = vm;

            AllowsTransparency = true;
        }
    }
}
