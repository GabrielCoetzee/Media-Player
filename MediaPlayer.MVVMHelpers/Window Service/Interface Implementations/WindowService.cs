using System.Windows;

namespace MediaPlayer.MVVMHelpers
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
