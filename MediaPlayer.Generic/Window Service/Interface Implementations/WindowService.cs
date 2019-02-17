using System.Windows;
using MediaPlayer.Generic.Window_Service.Interfaces;

namespace MediaPlayer.Generic.Window_Service.Interface_Implementations
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
