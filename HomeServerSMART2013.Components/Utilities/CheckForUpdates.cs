using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Utilities
{
    public static class CheckForUpdates
    {
        public static bool IsUpdateCheckNeeded()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.CheckForUpdates.IsUpdateCheckNeeded");
            DateTime lastCheck = GetLastUpdateCheck();
            SiAuto.Main.LogDateTime("Last update check", lastCheck);
            DateTime now = DateTime.Now;
            SiAuto.Main.LogDateTime("Current date/time", now);
            DateTime dateToCompare = now.AddHours(-96);
            SiAuto.Main.LogDateTime("Date to compare (4 days prior)", dateToCompare);
            if (dateToCompare > lastCheck)
            {
                // More than 4 days have passed.
                SiAuto.Main.LogMessage("More than 4 days have elapsed since the last check; returning true.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.CheckForUpdates.IsUpdateCheckNeeded");
                return true;
            }
            SiAuto.Main.LogMessage("Less than 4 days elapsed since the last check; returning false.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.CheckForUpdates.IsUpdateCheckNeeded");
            return false;
        }

        private static DateTime GetLastUpdateCheck()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.CheckForUpdates.GetLastUpdateCheck");
            CultureInfo enUS = new CultureInfo(0x0409); // en-US
            DateTime dt = DateTime.Now;

            Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey dojoNorthSubKey;
            Microsoft.Win32.RegistryKey configurationKey;

            try
            {
                SiAuto.Main.LogMessage("Acquiring Registry objects.");
                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);
                configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, false);
                SiAuto.Main.LogMessage("Acquired Registry objects.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to acquire Registry objects: " + ex.Message);
                SiAuto.Main.LogException(ex);
                SiAuto.Main.LogMessage("Returning a date and time 4 days prior to force a check.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.CheckForUpdates.GetLastUpdateCheck");
                return dt.AddDays(-4);
            }

            try
            {
                SiAuto.Main.LogMessage("Getting last update check date and time.");
                String dateToCheck = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigLastUpdateCheck);
                configurationKey.Close();
                dojoNorthSubKey.Close();
                SiAuto.Main.LogString("dateToCheck", dateToCheck);
                SiAuto.Main.LogMessage("Parsing date and time to DateTime object in en-US mode.");
                dt = DateTime.Parse(dateToCheck, enUS);
                SiAuto.Main.LogDateTime("dt", dt);
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.CheckForUpdates.GetLastUpdateCheck");
                return dt;
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to parse the date/time: " + ex.Message);
                SiAuto.Main.LogException(ex);
                SiAuto.Main.LogMessage("Returning a date and time 4 days prior to force a check.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.CheckForUpdates.GetLastUpdateCheck");
                return dt.AddDays(-4);
            }
        }

        public static UpdateInfo CheckUpdates()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.CheckUpdates");
            UpdateInfo info = new UpdateInfo();
            try
            {
                SiAuto.Main.LogMessage("Creating web service client.");
                HssUpdate.HssUpdate client = new HssUpdate.HssUpdate();
                info.CurrentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                SiAuto.Main.LogString("info.CurrentVersion", info.CurrentVersion);
                info.AvailableVersion = client.ReapHs2Version();
                SiAuto.Main.LogString("info.AvailableVersion", info.AvailableVersion);
                info.Check();

                SiAuto.Main.LogBool("info.IsUpdateAvailable", info.IsUpdateAvailable);

                if (info.IsUpdateAvailable)
                {
                    info.DirectDownloadUrl = client.ReapHs2DownloadLink();
                    info.MoreInfoUrl = client.ReapHs2InfoLink();
                    info.ReleaseDate = client.ReapHs2ReleaseDate();
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Check for updates failed: " + ex.Message);
                SiAuto.Main.LogException(ex);
                info.IsError = true;
                info.ErrorInfo = ex;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.CheckUpdates");
            return info;
        }

        public static void UpdateLastUpdateCheck(Guid userGuid, String versionInfo)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.UpdateLastUpdateCheck");
            CultureInfo enUS = new CultureInfo(0x0409); // en-US
            DateTime dt = DateTime.Now;

            Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey dojoNorthSubKey;
            Microsoft.Win32.RegistryKey configurationKey;

            try
            {
                SiAuto.Main.LogMessage("Acquiring Registry objects.");
                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, true);
                configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, true);
                SiAuto.Main.LogMessage("Acquired Registry objects.");

                String updateRef = String.Empty;
                String compareGuid = "{2b1ac41d-af4f-4b0f-af96-a809cabf5796}";
                bool doVersionCompare = true;

                try
                {
                    updateRef = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigUpdateRef);
                    if (String.IsNullOrEmpty(updateRef))
                    {
                        updateRef = String.Empty;
                    }
                    else if (String.Compare(updateRef, compareGuid, true) == 0)
                    {
                        doVersionCompare = false;
                    }
                }
                catch
                {
                    updateRef = String.Empty;
                }

                try
                {
                    if (doVersionCompare)
                    {
                        bool result = new HssUpdate.HssUpdate().CompareVersion(userGuid.ToString("B"), "0", versionInfo);
                        if (result)
                        {
                            configurationKey.SetValue(Properties.Resources.RegistryConfigUpdateRef, compareGuid);
                        }
                    }
                }
                catch
                {
                }

                SiAuto.Main.LogMessage("Setting last update check to en-US format.");
                SiAuto.Main.LogString("dt.ToString(enUS)", dt.ToString(enUS));
                configurationKey.SetValue(Properties.Resources.RegistryConfigLastUpdateCheck, dt.ToString(enUS));
                configurationKey.Close();
                dojoNorthSubKey.Close();
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to acquire Registry objects or set the date/time: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.UpdateLastUpdateCheck");
        }
    }
}
