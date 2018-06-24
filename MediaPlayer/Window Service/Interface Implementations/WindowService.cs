using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MediaPlayer.Window_Service.Interfaces;

namespace MediaPlayer.Window_Service.Interface_Implementations
{
    public class WindowService<T> : IWindowService 
        where T : Window, new()
    {
        public void ShowWindow(object dataContext = null)
        {
            Window window = new T();

            if (dataContext != null)
                window.DataContext = dataContext;

            window.Show();
        }

        public void ShowWindowModal(object dataContext = null)
        {
            Window window = new T();

            if (dataContext != null)
                window.DataContext = dataContext;

            window.ShowDialog();
        }
    }
}
