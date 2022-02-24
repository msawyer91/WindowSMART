using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UserControls
{
    public sealed class Aero
    {
        public static string USE_WINDOWS_VISTA_AERO = "Of2007Gorgeous";

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern int DwmIsCompositionEnabled(out bool enabled);

        public static bool IsAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return false;
            }

            try
            {
                bool enabled = false;
                int retVal = DwmIsCompositionEnabled(out enabled);
                return enabled;
            }
            catch
            {
                return false;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }
}
