using System.Drawing;

namespace MediaPlayer.View.Services.Abstract
{
    public interface IWindowResolutionCalculator
    {
        Rectangle CalculateOptimalMainWindowResolution();
        Rectangle CalculateOptimalSettingsWindowResolution();
    }
}
