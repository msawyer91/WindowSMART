using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.WindowsServerSolutions.Administration.ObjectModel;

using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls;
using Gurock.SmartInspect;
using Microsoft.Win32;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI
{
    [ContainsCustomControl] // Required attribute for custom tabs
    public class HssTopLevelTab : PageProvider
    {
        // Beta expiration
        private DateTime expirationDate = DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.License.GetExpirationDate();
        //private DateTime expirationDate = new DateTime(2012, 1, 20, 15, 21, 45, DateTimeKind.Local); // enable this line for testing

        public HssTopLevelTab()
            : base(new Guid("07e72bbe-e3c9-4348-854e-7688b9f88ab0"),  // Put your fixed, static guid here
                   "HSS 24/7", "Hard Disk monitoring and reporting for Windows Server Solutions.")
        {
            String path = String.Empty;
            String filename = Components.Utilities.Utility.GetLogFileName(Properties.Resources.LogfilePrefix, Properties.Resources.LogfileExtension, out path);
            SiAuto.Si.Connections = "file(filename=" + filename + ")";
            SiAuto.Si.Enabled = DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Utilities.Utility.IsLogEnabled();
            SiAuto.Main.LogMessage("Cleaning up old log files.");
            Components.Debugging.LogPruner.ObliterateOldLogs(path, Properties.Resources.LogfilePrefix, Properties.Resources.LogfileExtension, 14);
        }

        protected override Icon CreateImage()
        {
            // return SystemIcons.Information;
            SiAuto.Main.EnterMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.CreateImage");
            SiAuto.Main.LeaveMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.CreateImage");
            return Properties.Resources.DiskHeartBeat48x48;
        }

        protected override object CreatePages()
        {
            SiAuto.Main.EnterMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.CreatePages");
            // Do we hide BitLocker?
            bool hideBitLocker = false;

            // Try connecting to the Registry.
            try
            {
                SiAuto.Main.LogMessage("Connecting to the Registry to fetch configuration information.");
                RegistryKey registryHklm = Registry.LocalMachine;
                RegistryKey dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);
                RegistryKey configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, false);
                if (dojoNorthSubKey == null || configurationKey == null)
                {
                    SiAuto.Main.LogWarning("Configuration key(s) are NULL; hide BitLocker tab flag is set to false.");
                    hideBitLocker = false;
                }
                else
                {
                    SiAuto.Main.LogMessage("Checking BitLocker tab hide state.");
                    hideBitLocker = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigBitLockerHideTab));
                    SiAuto.Main.LogBool("hideBitLocker", hideBitLocker);
                    configurationKey.Close();
                    dojoNorthSubKey.Close();
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogException(ex);
            }

            // Use this method to instantiate sub-tabs
            try
            {
                #region BETA Section - Comment Out in Production
                //if (DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.License.IsExpired(expirationDate, this, true))
                //{
                //    // do nothing
                //}
                #endregion BETA Section - Comment Out in Production
                SiAuto.Main.LogMessage("Instantiating the HSS sub-tabs.");
                
                if (hideBitLocker)
                {
                    SiAuto.Main.LogMessage("Hide BitLocker is true; creating only the main page.");
                    object pageMainUi = new HssMainUiSubTabPage();
                    object[] pages = new Object[] { pageMainUi };
                    SiAuto.Main.LogMessage("Main page is instantiated and pages object is populated; returning pages object.");
                    SiAuto.Main.LeaveMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.CreatePages");
                    return pages;
                }
                else
                {
                    SiAuto.Main.LogMessage("Instantiating the main page.");
                    object pageMainUi = new HssMainUiSubTabPage();
                    SiAuto.Main.LogMessage("Instantiating the BitLocker page.");
                    object pageBitLocker = new HssBitLockerTabPage();
                    SiAuto.Main.LogMessage("Main and BitLocker pages are instantiating; assigning to pages object.");
                    object[] pages = new Object[] { pageMainUi, pageBitLocker };
                    SiAuto.Main.LogMessage("pages object is populated; returning pages object.");
                    SiAuto.Main.LeaveMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.CreatePages");
                    return pages;
                }
            }
            catch (System.ComponentModel.LicenseException lex)
            {
                SiAuto.Main.LogFatal("Fatal license error. Home Server SMART 24/7 will not be loaded.");
                SiAuto.Main.LogException(lex);
                EpicFail fail = new EpicFail(lex, "Fatal license error. Home Server SMART 24/7 will not be loaded.",
                    "Your beta license has expired. You must download a newer version. Please visit http://www.dojonorthsoftware.net to download a new version.", true);
                fail.ShowDialog();
                object[] pages = new Object[] { };
                SiAuto.Main.LeaveMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.CreatePages");
                return pages;
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogFatal("Exceptions were detected during page instantiation. Home Server SMART 24/7 will not be loaded.");
                SiAuto.Main.LogException(ex);
                EpicFail fail = new EpicFail(ex, "Exceptions were detected instantiating the Home Server SMART UI. Home Server SMART 24/7 will not be loaded.",
                    "Please ensure the Microsoft .NET Framework is correctly installed. There may also be a conflict with another add-in.", true);
                fail.ShowDialog();
                object[] pages = new Object[] { };
                SiAuto.Main.LeaveMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.CreatePages");
                return pages;
            }
        }

        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOW = 5;
        public static string SW_WINDOWS_KEY = "XtlKtnVnWtT/rFfft8WjHmhSXdFrqGT9KFkaBIJqn5Nwdq0dXakM2+NVYQDhPcDGj9qovDUnJujc4fBIrlKIpPl/LADT8Wkl+ZpW2YBMmuHjnJ";
    }
}
