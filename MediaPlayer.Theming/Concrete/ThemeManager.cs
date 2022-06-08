using System.ComponentModel.Composition;
using System.Windows;
using MediaPlayer.Theming.Abstract;

namespace MediaPlayer.Theming.Concrete
{
    [Export(typeof(IThemeManager))]
    public class ThemeManager : IThemeManager
    {
        public void ChangeOpacity(double opacity)
        {
            Application.Current.MainWindow.Background.Opacity = opacity;
        }

        public void ChangeAccent(string accent)
        {
            foreach (Window window in Application.Current.Windows)
            {
                ControlzEx.Theming.ThemeManager.Current.ChangeThemeColorScheme(window, accent);
            }
        }
    }
}
