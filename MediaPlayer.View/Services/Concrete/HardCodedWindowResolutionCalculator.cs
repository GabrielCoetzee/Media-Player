using MediaPlayer.Common.Constants;
using MediaPlayer.View.Services.Abstract;
using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows;

namespace MediaPlayer.View.Services.Concrete
{
    [Export(ServiceNames.HardCodedWindowResolutionCalculator, typeof(IWindowResolutionCalculator))]
    public class HardCodedWindowResolutionCalculator : IWindowResolutionCalculator
    {
        public Rectangle CalculateOptimalMainWindowResolution()
        {
            var defaultResolution = new Rectangle()
            {
                Height = 768,
                Width = 1366
            };

            if (SystemParameters.PrimaryScreenHeight <= 1080)
                return defaultResolution;

            if (SystemParameters.PrimaryScreenHeight <= 1440)
                return new Rectangle() { Width = 1600, Height = 900 };

            if (SystemParameters.PrimaryScreenHeight <= 2160)
                return new Rectangle() { Width = 1920, Height = 1080 };

            return defaultResolution;
        }

        public Rectangle CalculateOptimalSettingsWindowResolution()
        {
            var defaultResolution = new Rectangle()
            {
                Height = 350,
                Width = 500
            };

            if (SystemParameters.PrimaryScreenHeight <= 1440)
                return defaultResolution;

            if (SystemParameters.PrimaryScreenHeight <= 2160)
                return new Rectangle() { Width = 600, Height = 450 };

            return defaultResolution;
        }
    }
}
