using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class OperatingSystem
    {
        public static string WINDOWS_OS_LEGACY = "FlawlessBabyHead";
        public static string WINDOWS_8_METRO = "FlawlesslyCurved";

        public static bool IsWindowsServerSolutionsProduct(object componentToLicense)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.License.IsWindowsServerSolutionsProduct");
            uint windowsProductId = 0x65535;

            try
            {
                GetProductInfo(Environment.OSVersion.Version.Major, Environment.OSVersion.Version.Minor, 0, 0, out windowsProductId);
            }
            catch (Exception)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.License.IsWindowsServerSolutionsProduct");
                return false;
            }

            switch (windowsProductId)
            {
                case 34: // WHS 2011 (0x22)
                case 50: // SBS 2011 Essentials (0x32)
                case 19: // SS 2008 R2 Essentials (0x13)
                    {
                        SiAuto.Main.LogInt("windowsProductId", (int)windowsProductId);
                        SiAuto.Main.LogMessage("Running in Windows Server Solutions family OS.");
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.License.IsWindowsServerSolutionsProduct");
                        return true;
                    }
                case 2882382797: // PRODUCT_UNLICENSED (0xABCDABCD)
                    {
                        SiAuto.Main.LogInt("windowsProductId", (int)windowsProductId);
                        SiAuto.Main.LogFatal("[Cataclysmic] Windows is not Genuine - fatal 0xABCDABCD product ID.");
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.License.IsWindowsServerSolutionsProduct");
                        throw new System.ComponentModel.LicenseException(typeof(License), componentToLicense, "The copy of Microsoft Windows is not Genuine. It is an egregious, " +
                            "heinous violation of the Dojo North Software, LLC license agreement to run Home Server SMART 24/7, WindowSMART 24/7 or Taryn BitLocker Manager 2013 on a non-Genuine " +
                            "Windows installation. You may have committed a Federal felony.");
                    }
                default:
                    {
                        SiAuto.Main.LogInt("windowsProductId", (int)windowsProductId);
                        SiAuto.Main.LogMessage("Running in non WSS family OS.");
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.License.IsWindowsServerSolutionsProduct");
                        return false;
                    }
            }
        }

        [DllImport("kernel32.dll", SetLastError = false)]
        static extern bool GetProductInfo(
             int dwOSMajorVersion,
             int dwOSMinorVersion,
             int dwSpMajorVersion,
             int dwSpMinorVersion,
             out uint pdwReturnedProductType);
    }
}
