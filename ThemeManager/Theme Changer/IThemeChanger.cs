using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.ApplicationSettings.ThemeChanger
{
    public interface IThemeChanger
    {
        void ChangeTheme(string selectedTheme);

        void ChangeOpacity(double opacity);

    }
}
