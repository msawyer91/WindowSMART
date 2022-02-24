using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;

using DojoNorthSoftware.Pushover;
using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Utilities
{
    public sealed class Utility
    {
        public static string LOGFILE_ERROR_DUMP_LOCATION = "LickinGoodL@@king";
        public static string NT_STATUS_SERVER_ID = "Jczagdf9YWJWEBghkaD2qQLE4vV9z6T4SzdXKz9xU";
        public static string CHILKAT_DOTNET_4 = "HyperDiaperizer";

        public static String GetLogFileName(String prefix, String extension, out String path)
        {
            //String defaultPath = @"C:\ProgramData\Dojo North Software\Logs";
            String defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Dojo North Software\\Logs";
            String returnPath = String.Empty;
            String dateComponent = (DateTime.Now.Date.Month.ToString().Length == 1 ? "0" + DateTime.Now.Date.Month.ToString() : DateTime.Now.Date.Month.ToString()) + "_" +
                (DateTime.Now.Date.Day.ToString().Length == 1 ? "0" + DateTime.Now.Date.Day.ToString() : DateTime.Now.Date.Day.ToString()) + "_" +
                DateTime.Now.Date.Year.ToString() + "_";
            String timeComponent = (DateTime.Now.Hour.ToString().Length == 1 ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + "_" +
                (DateTime.Now.Minute.ToString().Length == 1 ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString()) + "_" +
                (DateTime.Now.Second.ToString().Length == 1 ? "0" + DateTime.Now.Second.ToString() : DateTime.Now.Second.ToString());
            String suffix = extension;
            path = defaultPath;

            try
            {
                Microsoft.Win32.RegistryKey machineKey = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey = machineKey.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);
                Microsoft.Win32.RegistryKey configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, false);
                path = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigLogLocation);
                if (String.Compare(path, "%DEFAULT%", true) == 0)
                {
                    path = defaultPath;
                }
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                returnPath = path + "\\" + prefix + dateComponent + timeComponent + suffix;
            }
            catch (Exception ex)
            {
                // Failed on the user-defined path; let's try the default.
                WindowsEventLogger.LogWarning("Exceptions were detected validating the logfile path " + path +
                    ". " + ex.Message + " The Server will attempt to use the default logging location.", 53862, Properties.Resources.EventLogTaryn);

                try
                {
                    if (!Directory.Exists(defaultPath))
                    {
                        // Oops, doesn't exist so let's create it.
                        Directory.CreateDirectory(defaultPath);
                    }
                }
                catch(Exception ex2)
                {
                    // Damn. We can't create it. :(
                    WindowsEventLogger.LogError("Exceptions were detected configuring the default logfile path " + defaultPath + ". SmartInspect logging will NOT " +
                        "be available. " + ex2.Message, 53863, Properties.Resources.EventLogTaryn);
                }
                path = defaultPath;
                returnPath = defaultPath + "\\" + prefix + dateComponent + timeComponent + suffix;
            }

            try
            {
                System.IO.FileStream fs = System.IO.File.Create(path + "\\Tarynblmtemp65535.txt");
                fs.Flush();
                fs.Close();
                System.IO.File.Delete(path + "\\Tarynblmtemp65535.txt");
                return returnPath;
            }
            catch(Exception ex)
            {
                WindowsEventLogger.LogError("Exceptions were detected accessing path " + path + " for logging. It cannot be used. " +
                    "HSS/WS2 attempted to write a temporary ile to this location (prior to logfile generation) to validate the Server's " +
                    "ability to write a file to this location. That attempt failed. Please check that the folder exists and also check " +
                    "the NTFS permissions on the folder to verify that the service (for service errors) or your user ID (for UI errors) " +
                    "has sufficient perms to write to this location. For service-related errors, add the SYSTEM account with Modify " +
                    "or Full Control perms. For UI-related errors, add your user ID with Modify or Full Control perms.\n\nIf this error " +
                    "is immediately preceded by error 53863 or warning 53862, try creating the desired logfile locations manually and " +
                    "assigning appropriate NTFS perms, and then restart the service.\n\n" + ex.Message, 53864, Properties.Resources.EventLogTaryn);
                return returnPath;
            }
        }

        public static void LaunchBrowser(String url)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(url);
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Unable to launch web browser. " + ex.Message + "\n\nURL: " + url,
                    "Browser Launch Failed", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            }
        }

        public static bool IsLogEnabled()
        {
            bool flag = false;
            try
            {
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;
                Microsoft.Win32.RegistryKey configurationKey;

                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);
                configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, false);

                // We check GPO first.
                int gpoDebuggingControl = 2;
                try
                {
                    // 0 = policy disabled; 1 = policy enabled; 2 = policy undefined
                    gpoDebuggingControl = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoDebuggingControl);
                }
                catch
                {
                    // policy undefined
                    gpoDebuggingControl = 2;
                }

                if (gpoDebuggingControl == 0)
                {
                    // Policy disabled so return false.
                    configurationKey.Close();
                    dojoNorthSubKey.Close();
                    return false;
                }

                String logRequirement = String.Empty;
                try
                {
                    logRequirement = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigLogRequirement);
                }
                catch
                {
                    logRequirement = String.Empty;
                }

                if(String.Compare(logRequirement, "Mandatory", true) == 0 && gpoDebuggingControl == 1)
                {
                    // Logging is forced on.
                    configurationKey.Close();
                    dojoNorthSubKey.Close();
                    return true;
                }
                else if (String.Compare(logRequirement, "Forbidden", true) == 0 && gpoDebuggingControl == 1)
                {
                    // Logging is forbidden.
                    configurationKey.Close();
                    dojoNorthSubKey.Close();
                    return false;
                }

                // Logging policy is either undefined, or is enabled but set to allow.

                flag = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigEnableDebugLogging));
                configurationKey.Close();
                dojoNorthSubKey.Close();
                return flag;
            }
            catch
            {
                return flag;
            }
        }

        public static String NormalizeBinaryFlags(String binaryFlags)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utility.NormalizeBinaryFlags");
            // Normalize the binary string so it's 8 characters long.
            SiAuto.Main.LogString("binaryFlags", binaryFlags);
            SiAuto.Main.LogInt("binaryFlags.Length", binaryFlags.Length);
            switch (binaryFlags.Length)
            {
                case 8:
                    {
                        break;
                    }
                case 7:
                    {
                        binaryFlags = "0" + binaryFlags;
                        break;
                    }
                case 6:
                    {
                        binaryFlags = "00" + binaryFlags;
                        break;
                    }
                case 5:
                    {
                        binaryFlags = "000" + binaryFlags;
                        break;
                    }
                case 4:
                    {
                        binaryFlags = "0000" + binaryFlags;
                        break;
                    }
                case 3:
                    {
                        binaryFlags = "00000" + binaryFlags;
                        break;
                    }
                case 2:
                    {
                        binaryFlags = "000000" + binaryFlags;
                        break;
                    }
                case 1:
                    {
                        binaryFlags = "0000000" + binaryFlags;
                        break;
                    }
                default:
                    {
                        binaryFlags = "00000000";
                        break;
                    }
            }
            SiAuto.Main.LogString("binaryFlags", binaryFlags);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utility.NormalizeBinaryFlags");
            return binaryFlags;
        }

        public static int GetDriveIdFromPath(String drivePath)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utility.GetDriveIdFromPath");
            // Physical drive string is (without quotes) "\\.\PHYSICALDRIVEx" so we want the index.
            // Index is as follows =====================> 012345678901234567 (17)

            int physicalDriveId = -1;

            try
            {
                String driveId = drivePath.Substring(17);
                SiAuto.Main.LogMessage("Parse integer drive ID from " + drivePath + ", substring(17) = " + driveId);
                physicalDriveId = Int32.Parse(driveId);
            }
            catch
            {
                SiAuto.Main.LogColored(System.Drawing.Color.Red, "Drive ID parse failed.");
                physicalDriveId = -1;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utility.GetDriveIdFromPath");
            return physicalDriveId;
        }

        public static String ReverseCharacters(String input)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utility.ReverseCharacters");
            SiAuto.Main.LogString("input", input);
            String retVal = String.Empty;
            int leader = 1;
            int follower = 0;
            while (leader < input.Length)
            {
                retVal += input.Substring(leader, 1) + input.Substring(follower, 1);
                leader = leader + 2;
                follower = follower + 2;
            }
            if (leader == input.Length) // odd number in string
            {
                retVal += input.Substring(follower); // pick up the last character
            }

            SiAuto.Main.LogString("retVal", retVal);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utility.ReverseCharacters");
            return retVal;
        }

        /// <summary>
        /// Returns whether or not the email address specified matches the Regex for an email address. ~ Jake ~
        /// </summary>
        /// <param name="emailAddress">Email address to test.</param>
        /// <returns>true if the specified address matches the Regex (is valid); false otherwise.</returns>
        public static bool IsEmailAddressValid(String emailAddress)
        {
            emailAddress = emailAddress.ToLower();

            if (emailAddress == null)
            {
                return false;
            }
            else if (emailAddress.Contains("..") || emailAddress.Contains(".@") || emailAddress.Contains("@."))
            {
                // The below regex doesn't catch the invalid .., .@ or @. so we weed them out here.
                return false;
            }
            else if (emailAddress.EndsWith("."))
            {
                // The ending . also slips through.
                return false;
            }
            else
            {
                return System.Text.RegularExpressions.Regex.IsMatch(emailAddress,
                    @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[a-z]{2}|com|org|net|edu|gov|mil|biz|info|mobi|name|aero|asia|jobs|museum)\b");
                //                return System.Text.RegularExpressions.Regex.IsMatch(emailAddress, @"
                //^
                //[-a-zA-Z0-9][-.a-zA-Z0-9]*
                //@
                //[-.a-zA-Z0-9]+
                //(\.[-.a-zA-Z0-9]+)*
                //\.
                //(
                //com|edu|info|gov|int|mil|net|org|biz|
                //name|museum|coop|aero|pro|mobi
                //|
                //[a-zA-Z]{2}
                //)
                //$",
                //                System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);
            }
        }

        public static String GetTestStatus(int smartTestStatusByte)
        {
            String statusString = Utilities.Utility.NormalizeBinaryFlags(Convert.ToString(smartTestStatusByte, 2));
            String selftestStatus = statusString.Substring(0, 4);
            String intRemaining = statusString.Substring(4);

            int statusCode = Convert.ToInt32(selftestStatus, 2);
            int remaining = Convert.ToInt32(intRemaining, 2);

            if (statusCode == 15)
            {
                // Test in progress; get the percentage to build the message
                int percentLeft = 100 - (remaining * 10);
                return "Self-test in progress; " + percentLeft.ToString() + "% complete";
            }
            else if (statusCode > 8 && statusCode < 15)
            {
                // Status codes 9-14 are reserved per ATA specifications
                return "" + statusCode.ToString();
            }
            else // (codes 0-8)
            {
                switch (statusCode)
                {
                    case 8:
                        {
                            return "A test element failed that suggests handling damage";
                        }
                    case 7:
                        {
                            return "Self-test failed: Read Element";
                        }
                    case 6:
                        {
                            return "Self-test failed: Servo and/or Seek Element";
                        }
                    case 5:
                        {
                            return "Self-test failed: Electrical Element";
                        }
                    case 4:
                        {
                            return "Self-test failed: Unknown Element";
                        }
                    case 3:
                        {
                            return "Self-test did not complete due to a fatal drive error";
                        }
                    case 2:
                        {
                            return "Test interrupted by hardware/software reset from host";
                        }
                    case 1:
                        {
                            return "Test aborted by user or host";
                        }
                    case 0:
                    default:
                        {
                            return "Successfully completed (or no test ever run)";
                        }
                }
            }
        }

        public static int GetNumberOfSupportedTests(byte[] smartData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.Utility.GetNumberOfSupportedTests");
            int testsSupported = 0;
            try
            {
                int shortTestTime = smartData[372];
                SiAuto.Main.LogInt("shortTestTime", shortTestTime);
                if (shortTestTime > 0)
                {
                    testsSupported++;
                }
                int longTestTime = smartData[373];
                SiAuto.Main.LogInt("longTestTime", longTestTime);
                if (longTestTime > 0)
                {
                    testsSupported++;
                }
                int conveyanceTestTime = smartData[374];
                SiAuto.Main.LogInt("conveyanceTestTime", conveyanceTestTime);
                if (conveyanceTestTime > 0)
                {
                    testsSupported++;
                }
            }
            catch(Exception ex)
            {
                SiAuto.Main.LogWarning("An exception occurred getting the number of tests. Is the SMART data corrupt or missing? " + ex.Message);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.Utility.GetNumberOfSupportedTests");
            return testsSupported;
        }

        /// <summary>
        /// Gets a disk's health status from the Registry.
        /// </summary>
        /// <param name="drive">The drive to check.</param>
        /// <param name="isIgnored">Output flag that specifies whether a disk is ignored.</param>
        /// <returns>The DiskStatus object that reflects the health condition of the disk.</returns>
        public static byte[] GetDiskStatusFromRegistry(System.Management.ManagementObject drive, bool isWindows8, out bool isIgnored)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.Utility.GetDiskStatusFromRegistry");
            isIgnored = false;
            byte[] rawSmartData = new byte[512];

            try
            {
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryMonitoredDisksKey, false);

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, false);
                    String diskTestPath = String.Empty;
                    if (isWindows8)
                    {
                        diskTestPath = "\\\\.\\PHYSICALDRIVE" + drive["DeviceID"].ToString();
                    }
                    else
                    {
                        diskTestPath = drive["DeviceID"].ToString();
                    }

                    if (String.Compare(diskTestPath, (String)diskKey.GetValue("DevicePath"), true) == 0)
                    {
                        SiAuto.Main.LogMessage("Found a matching disk.");
                        bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                        if (!activeFlag)
                        {
                            // Inactive/Stale disk.
                            diskKey.Close();
                            continue;
                        }

                        try
                        {
                            rawSmartData = (byte[])diskKey.GetValue("RawSmartData");
                        }
                        catch
                        {
                        }

                        try
                        {
                            String dateIgnored = (String)diskKey.GetValue("DateIgnored", String.Empty);
                            diskKey.Close();
                            DateTime result;
                            if (DateTime.TryParse(dateIgnored, out result))
                            {
                                isIgnored = true;
                            }
                        }
                        catch
                        {
                        }
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.Utility.GetDiskStatusFromRegistry");
                        return rawSmartData;
                    }
                }
            }
            catch (Exception)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.Utility.GetDiskStatusFromRegistry");
                return new byte[512];
            }
            SiAuto.Main.LogWarning("Hmm...we reached the end of the method without a matching disk. Is it AWOL?");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.Utility.GetDiskStatusFromRegistry");
            return new byte[512];
        }

        /// <summary>
        /// Returns whether or not the system is running on the Windows 8 platform, including Windows 8 and Windows Server 2012
        /// (including Essentials). We use the new MSFT_PhysicalDisk class to interact with Storage Spaces.
        /// </summary>
        /// <returns>true if Windows 8 platform; false in all other cases.</returns>
        public static bool IsSystemWindows8()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.Utility.IsSystemWindows8");
            String osName = OSInfo.Name;
            SiAuto.Main.LogString("osName", osName);
            if (String.Compare(osName, "Windows 8", true) != 0 && String.Compare(osName, "Windows Server 2012", true) != 0)
            {
                if (String.Compare(osName, "Windows 8.1", true) != 0 && String.Compare(osName, "Windows Server 2012 R2", true) != 0)
                {
                    if (String.Compare(osName, "Windows 10", true) != 0 && String.Compare(osName, "Windows Server 2016", true) != 0)
                    {
                        SiAuto.Main.LogMessage("OS is not Windows 8/10/2012 platform; use of Storage Spaces is not possible. Returning false.");
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.Utility.IsSystemWindows8");
                        return false;
                    }
                }
            }

            SiAuto.Main.LogMessage("OS is Windows 8/10/2012 platform; returning true.");

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.Utility.IsSystemWindows8");
            return true;
        }

        /// <summary>
        /// Gets the Windows 8 drive interface type as a string, based on the device interface number.
        /// </summary>
        /// <param name="devType"></param>
        /// <returns>String representation of the interface (bus) type.</returns>
        public static String GetWindows8DriveInterfaceType(UInt16 devType)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.Utility.GetWindows8DriveInterfaceType");
            SiAuto.Main.LogInt("devType", devType);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.Utility.GetWindows8DriveInterfaceType");
            switch (devType)
            {
                case 1:
                    {
                        return "SCSI";
                    }
                case 2:
                    {
                        return "ATAPI";
                    }
                case 3:
                    {
                        return "ATA";
                    }
                case 4:
                    {
                        return "1394";
                    }
                case 5:
                    {
                        return "SSA";
                    }
                case 6:
                    {
                        return "Fibre Channel";
                    }
                case 7:
                    {
                        return "USB";
                    }
                case 8:
                    {
                        return "RAID";
                    }
                case 9:
                    {
                        return "iSCSI";
                    }
                case 10:
                    {
                        return "SAS";
                    }
                case 11:
                    {
                        return "SATA";
                    }
                case 12:
                    {
                        return "SD";
                    }
                case 13:
                    {
                        return "MMC";
                    }
                case 14:
                    {
                        return "MAX";
                    }
                case 15:
                    {
                        return "File Backed Virtual";
                    }
                case 16:
                    {
                        return "Storage Spaces";
                    }
                case 17:
                    {
                        return "Microsoft Reserved";
                    }
                case 0:
                default:
                    {
                        return "Unknown";
                    }
            }
        }

        public static String GetWindows8DiskOperationalStatus(UInt16[] statusCodes)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.Utility.GetWindows8DiskOperationalStatus");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (UInt16 statusCode in statusCodes)
            {
                switch(statusCode)
                {
                    case 0:
                        {
                            sb.Append("Unknown");
                            break;
                        }
                    case 2:
                        {
                            sb.Append("OK");
                            break;
                        }
                    case 3:
                        {
                            sb.Append("Degraded");
                            break;
                        }
                    case 4:
                        {
                            sb.Append("Stressed");
                            break;
                        }
                    case 5:
                        {
                            sb.Append("Predictive Failure");
                            break;
                        }
                    case 6:
                        {
                            sb.Append("Error");
                            break;
                        }
                    case 7:
                        {
                            sb.Append("Non-Recoverable Error");
                            break;
                        }
                    case 8:
                        {
                            sb.Append("Starting");
                            break;
                        }
                    case 9:
                        {
                            sb.Append("Stopping");
                            break;
                        }
                    case 10:
                        {
                            sb.Append("Stopped");
                            break;
                        }
                    case 11:
                        {
                            sb.Append("In Service");
                            break;
                        }
                    case 12:
                        {
                            sb.Append("No Contact");
                            break;
                        }
                    case 13:
                        {
                            sb.Append("Lost Communication");
                            break;
                        }
                    case 15:
                        {
                            sb.Append("Dormant");
                            break;
                        }
                    case 18:
                        {
                            sb.Append("Power Mode");
                            break;
                        }
                    case 0x8004:
                        {
                            sb.Append("Failed Media");
                            break;
                        }
                    case 0x8005:
                        {
                            sb.Append("Split");
                            break;
                        }
                    case 0x8006:
                        {
                            sb.Append("Stale Metadata");
                            break;
                        }
                    case 0x8007:
                        {
                            sb.Append("I/O Error");
                            break;
                        }
                    case 0x8008:
                        {
                            sb.Append("Corrupt Metadata");
                            break;
                        }
                    case 0x8009:
                        {
                            sb.Append("Microsoft Reserved");
                            break;
                        }
                    default:
                        {
                            sb.Append("Status Code " + statusCode.ToString());
                            break;
                        }
                }

                sb.Append(", ");
            }

            String statusString = sb.ToString();
            try
            {
                statusString = statusString.Substring(0, statusString.Length - 2);
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.Utility.GetWindows8DiskOperationalStatus");
            return statusString;
        }

        public static String BuildWindows8PnpDeviceName(String model, String serial, UInt16 interfaceCode, int diskIdNumber)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.Utility.BuildWindows8PnpDeviceName");
            SiAuto.Main.LogString("model", model);
            SiAuto.Main.LogString("serial", serial);
            SiAuto.Main.LogInt("interfaceCode", (int)interfaceCode);
            
            String storage = "WINDOWS8STORAGEDEVICE\\";
            String interfaceType = GetWindows8DriveInterfaceType(interfaceCode) + "\\";
            String serialModel = serial + "\\" + model;
            String combined = storage + interfaceType + serialModel;
            SiAuto.Main.LogString("combined", combined);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.Utility.BuildWindows8PnpDeviceName");
            return combined;
        }

        public static bool IsGenericSerial(String serial, int physicalDriveId)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.Utility.IsGenericSerial");
            bool isGenericSerial = false;

            // Check to see if serial is DB1234567895AA
            if (String.Compare(serial.Trim(), "DB1234567895AA", true) == 0)
            {
                SiAuto.Main.LogWarning("Windows reports disk serial as DB1234567895AA which is not a valid SN; will need to append physical drive ID " + physicalDriveId.ToString() + " to end.");
                isGenericSerial = true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.Utility.CheckSerialValidity");
            return isGenericSerial;
        }

        public static String GetWindows8PnpDiskId(String physicalDrivePath)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.Utility.GetWindows8PnpDiskId");
            // Browse all WMI physical disks.
            String pnpDeviceID = String.Empty;
            foreach (ManagementObject drive in new ManagementObjectSearcher(
              Properties.Resources.WmiQueryStringNonWin8).Get())
            {
                try
                {
                    if (String.Compare(drive["DeviceID"].ToString(), physicalDrivePath, true) == 0)
                    {
                        pnpDeviceID = drive["PNPDeviceID"].ToString();
                        SiAuto.Main.LogMessage("Matching device detected: " + pnpDeviceID);
                        break;
                    }
                }
                catch
                {
                }
            }

            if (String.IsNullOrEmpty(pnpDeviceID))
            {
                SiAuto.Main.LogWarning("No matching devices detected in Win32_DiskDrive.");
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.Utility.GetWindows8PnpDiskId");
            return pnpDeviceID;
        }

        public static String GetDiskPreferredModel(ManagementObject drive, bool isWindows8)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.GetWindows8DiskPreferredModel");
            String preferredModel = String.Empty;

            try
            {
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryMonitoredDisksKey, false);

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, false);
                    String diskTestPath = String.Empty;
                    if (isWindows8)
                    {
                        diskTestPath = "\\\\.\\PHYSICALDRIVE" + drive["DeviceID"].ToString();
                    }
                    else
                    {
                        diskTestPath = drive["DeviceID"].ToString();
                    }

                    if (String.Compare(diskTestPath, (String)diskKey.GetValue("DevicePath"), true) == 0)
                    {
                        bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                        if (!activeFlag)
                        {
                            // Inactive/Stale disk.
                            diskKey.Close();
                            continue;
                        }

                        try
                        {
                            String dateIgnored = (String)diskKey.GetValue("DateIgnored", String.Empty);
                            DateTime result;
                            if (DateTime.TryParse(dateIgnored, out result))
                            {
                                // Make sure the disk isn't ignored.
                                diskKey.Close();
                                continue;
                            }
                        }
                        catch
                        {

                        }

                        // This is the one we want!  It's active and not ignored.
                        preferredModel = (String)diskKey.GetValue("PreferredModel");
                        diskKey.Close();
                        break;
                    }
                    diskKey.Close();
                }
            }
            catch (Exception)
            {
                preferredModel = String.Empty;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetWindows8DiskPreferredModel");
            return preferredModel;
        }

        public static List<String> GetDriveLettersFromDriveNumbers(List<int> failingDiskList)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.GetDriveLettersFromDriveNumbers");
            List<String> failingDriveLetters = new List<String>();
            try
            {
                SiAuto.Main.LogMessage("Attempting to get partition and drive letter/mount point data.");
                foreach (int failingDisk in failingDiskList)
                {
                    foreach (ManagementObject drive in new ManagementObjectSearcher(
                              Properties.Resources.WmiQueryStringNonWin8).Get())
                    {
                        if (String.Compare("\\\\.\\PHYSICALDRIVE" + failingDisk.ToString(), drive["DeviceID"].ToString(), true) != 0)
                        {
                            continue;
                        }
                        foreach (ManagementObject partition in new ManagementObjectSearcher(
                            "ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + drive["DeviceID"]
                            + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition").Get())
                        {
                            // associate partitions with logical disks (drive letter volumes)
                            SiAuto.Main.LogMessage("Associating partitions on " + drive["DeviceID"]);

                            foreach (ManagementObject disk in new ManagementObjectSearcher(
                                "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='"
                                  + partition["DeviceID"]
                                  + "'} WHERE AssocClass = Win32_LogicalDiskToPartition").Get())
                            {
                                SiAuto.Main.LogMessage("Associating locical drives on " + partition["DeviceID"]);
                                String mountPoint = disk["Name"].ToString();
                                if (!mountPoint.Contains('\\'))
                                {
                                    SiAuto.Main.LogMessage("Adding drive letter " + mountPoint + " to the fail list.");
                                    failingDriveLetters.Add(mountPoint);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to get one or more drive letters: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetDriveLettersFromDriveNumbers");
            return failingDriveLetters;
        }

        public static bool GetWindows8AlternatePredFail(String path)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.GetWindows8AlternatePredFail");
            try
            {
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                          Properties.Resources.WmiQueryStringNonWin8).Get())
                {
                    if (String.Compare(path, drive["DeviceID"].ToString(), true) != 0)
                    {
                        continue;
                    }

                    String status = (drive["Status"] == null ? "Unknown" : drive["Status"].ToString());
                    if (status.ToLower().Contains("pred fail"))
                    {
                        SiAuto.Main.LogWarning("Win32_DiskDrive reports \"pred fail\". Setting to true.");
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetWindows8AlternatePredFail");
                        return true;
                    }
                    else
                    {
                        SiAuto.Main.LogMessage("Win32_DiskDrive reports OK so setting to false.");
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetWindows8AlternatePredFail");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to get WMI fail status via alternate method: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetWindows8AlternatePredFail");
            return false;
        }

        public static void GetWindowsScsiPortInfo(String path, out int scsiPort, out int scsiTarget, out int scsiBus)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.GetWindows8ScsiInfo");
            scsiPort = -1;
            scsiTarget = -1;
            scsiBus = -1;
            try
            {
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                          Properties.Resources.WmiQueryStringNonWin8).Get())
                {
                    if (String.Compare(path, drive["DeviceID"].ToString(), true) != 0)
                    {
                        continue;
                    }

                    Int32.TryParse(drive["SCSIPort"].ToString(), out scsiPort);
                    Int32.TryParse(drive["SCSITargetId"].ToString(), out scsiTarget);
                    Int32.TryParse(drive["SCSIBus"].ToString(), out scsiBus);
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to get WMI fail status via alternate method: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            finally
            {
                SiAuto.Main.LogInt("scsiPort", scsiPort);
                SiAuto.Main.LogInt("scsiTarget", scsiTarget);
                SiAuto.Main.LogInt("scsiBus", scsiBus);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetWindows8ScsiInfo");
        }

        public static String GetVolumeLabelFsFromLetter(String driveLetter)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.GetVolumeLabelFsFromLetter");
            String labelFS = String.Empty;
            String label = String.Empty;
            String fileSystem = String.Empty;

            try
            {
                SiAuto.Main.LogString("driveLetter", driveLetter);
                String driveLetterOnly = driveLetter.Substring(0, 1);
                SiAuto.Main.LogString("driveLetterOnly", driveLetterOnly);
                DriveInfo di = new DriveInfo(driveLetterOnly);

                if (!String.IsNullOrEmpty(di.VolumeLabel))
                {
                    label = di.VolumeLabel;
                }
                if (!String.IsNullOrEmpty(di.DriveFormat))
                {
                    fileSystem = di.DriveFormat;
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogWarning("Exceptions were detected fetching drive label/filesystem: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }

            if (String.IsNullOrEmpty(label) && String.IsNullOrEmpty(fileSystem))
            {
                labelFS = String.Empty;
            }
            else if (String.IsNullOrEmpty(fileSystem))
            {
                labelFS = "{" + label + "}";
            }
            else if (String.IsNullOrEmpty(label))
            {
                labelFS = "{" + fileSystem + "}";
            }
            else
            {
                labelFS = "{" + label + ", " + fileSystem + "}";
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetVolumeLabelFsFromLetter");
            return labelFS;
        }

        public static double GetTotalDiskSizeByDriveLetter(String driveName, out double freeSpace)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.GetTotalDiskSizeByDriveLetter");
            freeSpace = -1;
            driveName += "\\";
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                SiAuto.Main.LogMessage("Examining " + drive.Name + " and comparing to " + driveName);
                if (drive.IsReady && String.Compare(drive.Name, driveName, true) == 0)
                {
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetTotalDiskSizeByDriveLetter");
                    freeSpace = (drive.TotalFreeSpace / 1073741824);
                    return (drive.TotalSize / 1073741824);
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetTotalDiskSizeByDriveLetter");
            return -1;
        }

        public static String CreateDiskCapacityFreeString(double capacity, double freeSpace)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.CreateDiskCapacityFreeString");
            String cap = capacity.ToString("F");
            String free = freeSpace.ToString("F");
            double percentFree = (freeSpace / capacity) * 100;
            String pf = percentFree.ToString("F");

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.CreateDiskCapacityFreeString");
            return pf + "% (" + free + " GB) free of " + cap + " GB";
        }

        public static String GetProductRestrictions(String productClass)
        {
            switch (productClass)
            {
                case "FAML":
                case "SOHO":
                case "SBZ1":
                case "SBZ2":
                case "MBZ1":
                case "MBZ2":
                case "LBIZ":
                case "EBIZ":
                case "NPRF":
                case "EDUC":
                case "GOVT":
                case "PROF":
                    {
                        return "Professional Edition";
                    }
                case "FRFA":
                    {
                        return "Professional Edition (Friends and Family)";
                    }
                case "HOME":
                    {
                        return "Home Edition";
                    }
                case "SNGL":
                    {
                        return "Home Single PC Edition";
                    }
                case "INDF":
                    {
                        return "Professional Indefinite Evaluation";
                    }
                default:
                    {
                        return "Invalid";
                    }
            }
        }

        public static PushoverSound ResolveSoundFromString(String soundName)
        {
            switch (soundName)
            {
                case "(Device default)":
                    {
                        return PushoverSound.DeviceDefault;
                    }
                case "Bike":
                    {
                        return PushoverSound.Bike;
                    }
                case "Bugle":
                    {
                        return PushoverSound.Bugle;
                    }
                case "Cash Register":
                    {
                        return PushoverSound.CashRegister;
                    }
                case "Classical":
                    {
                        return PushoverSound.Classical;
                    }
                case "Cosmic":
                    {
                        return PushoverSound.Cosmic;
                    }
                case "Falling":
                    {
                        return PushoverSound.Falling;
                    }
                case "Gamelan":
                    {
                        return PushoverSound.Gamelan;
                    }
                case "Incoming":
                    {
                        return PushoverSound.Incoming;
                    }
                case "Intermission":
                    {
                        return PushoverSound.Intermission;
                    }
                case "Magic":
                    {
                        return PushoverSound.Magic;
                    }
                case "Mechanical":
                    {
                        return PushoverSound.Mechanical;
                    }
                case "Piano Bar":
                    {
                        return PushoverSound.PianoBar;
                    }
                case "Siren":
                    {
                        return PushoverSound.Siren;
                    }
                case "Space Alarm":
                    {
                        return PushoverSound.SpaceAlarm;
                    }
                case "Tug Boat":
                    {
                        return PushoverSound.TugBoat;
                    }
                case "Persistent (long)":
                    {
                        return PushoverSound.Persistent;
                    }
                case "Alien Alarm (long)":
                    {
                        return PushoverSound.Alien;
                    }
                case "Climb (long)":
                    {
                        return PushoverSound.Climb;
                    }
                case "Pushover Echo (long)":
                    {
                        return PushoverSound.Echo;
                    }
                case "Up Down (long)":
                    {
                        return PushoverSound.UpDown;
                    }
                case "None (silent)":
                    {
                        return PushoverSound.None;
                    }
                case "Pushover (default)":
                default:
                    {
                        return PushoverSound.Pushover;
                    }
            }
        }
    }
}
