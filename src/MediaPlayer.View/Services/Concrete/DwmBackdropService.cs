using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.View.Services.Abstract;

namespace MediaPlayer.View.Services.Concrete
{
    [Export(ServiceNames.DwmBackdropService, typeof(IDwmBackdropService))]
    public class DwmBackdropService : IDwmBackdropService
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        [StructLayout(LayoutKind.Sequential)]
        private struct MARGINS { public int Left, Right, Top, Bottom; }

        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;

        public bool IsSupported
        {
            get
            {
                var version = Environment.OSVersion.Version;

                return version.Major >= 10 && version.Build >= 22621;
            }
        }

        public void ApplyBackdrop(Window window, DwmBackdropType backdropType)
        {
            var hwnd = new WindowInteropHelper(window).Handle;

            if (hwnd == IntPtr.Zero || !IsSupported)
                return;

            var margins = new MARGINS { Left = -1, Right = -1, Top = -1, Bottom = -1 };
            DwmExtendFrameIntoClientArea(hwnd, ref margins);

            var value = (int)backdropType;
            DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref value, sizeof(int));
        }

        public void RemoveBackdrop(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;

            if (hwnd == IntPtr.Zero)
                return;

            var value = (int)DwmBackdropType.None;
            DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref value, sizeof(int));

            var margins = new MARGINS { Left = 0, Right = 0, Top = 0, Bottom = 0 };
            DwmExtendFrameIntoClientArea(hwnd, ref margins);
        }
    }
}
