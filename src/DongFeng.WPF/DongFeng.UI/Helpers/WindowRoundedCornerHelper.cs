using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace DongFeng.UI.Helpers
{
    public static class WindowRoundedCornerHelper
    {
        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }

        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void DwmSetWindowAttribute(IntPtr hwnd,
                                                         DWMWINDOWATTRIBUTE attribute,
                                                         ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
                                                         uint cbAttribute);

        public static void EnableRoundedCorners(Window window)
        {
            if (window == null) return;

            // Only supported on Windows 11 (build 22000+)
            if (Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= 22000)
            {
                window.SourceInitialized += (s, e) =>
                {
                    IntPtr handle = new WindowInteropHelper(window).Handle;
                    var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
                    try
                    {
                        DwmSetWindowAttribute(handle,
                                              DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
                                              ref preference,
                                              sizeof(uint));
                    }
                    catch
                    {
                        // Fallback gracefully if DWM call fails
                    }
                };
            }
        }
    }
}

