using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class LegacyOs
    {
        public static uint Compose(DateTime dt, out String elementA, out int elementB)
        {
            String a = String.Empty; // Month;
            int b = 53; // Day
            int c = -437; // Year

            uint composeVal = 0x102807E0; // Compose method incomplete

            switch (dt.Month)
            {
                case 1:
                    {
                        a = "MHS";
                        break;
                    }
                case 2:
                    {
                        a = "SJS";
                        break;
                    }
                case 3:
                    {
                        a = "LLD";
                        break;
                    }
                case 4:
                    {
                        a = "SRS";
                        break;
                    }
                case 5:
                    {
                        a = "JPS";
                        break;
                    }
                case 6:
                    {
                        a = "JRS";
                        break;
                    }
                case 7:
                    {
                        a = "JLS";
                        break;
                    }
                case 8:
                    {
                        a = "JCS";
                        break;
                    }
                case 9:
                    {
                        a = "TDS";
                        break;
                    }
                case 10:
                    {
                        a = "JCD";
                        break;
                    }
                case 11:
                    {
                        a = "CMS";
                        break;
                    }
                case 12:
                    {
                        a = "STL";
                        break;
                    }
                default:
                    {
                        elementA = "MHS.1543";
                        elementB = 54;
                        composeVal = 0x102807E1; // Month out of range 1-12
                        return composeVal;
                    }
            }

            b += dt.Day;
            c += dt.Year;

            elementA = a + "." + c.ToString();
            elementB = b;
            composeVal = 0x0;
            return composeVal;
        }

        public static uint Decompose(String elementA, int elementB, out DateTime dt)
        {
            uint decomposeVal = 0x102807E2; // Decompose method incomplete

            // String should always be 8 characters.
            if(elementA.Length != 8)
            {
                dt = new DateTime(1980, 1, 1);
                decomposeVal = 0x102807E3; // Decompose string <> 8 characters
                return decomposeVal;
            }
            String a = elementA.Substring(0, 3).ToUpper();
            String b = elementA.Substring(4);
            int c = -1;
            int d = -1;

            switch (a)
            {
                case "MHS":
                    {
                        c = 1;
                        break;
                    }
                case "SJS":
                    {
                        c = 2;
                        break;
                    }
                case "LLD":
                    {
                        c = 3;
                        break;
                    }
                case "SRS":
                    {
                        c = 4;
                        break;
                    }
                case "JPS":
                    {
                        c = 5;
                        break;
                    }
                case "JRS":
                    {
                        c = 6;
                        break;
                    }
                case "JLS":
                    {
                        c = 7;
                        break;
                    }
                case "JCS":
                    {
                        c = 8;
                        break;
                    }
                case "TDS":
                    {
                        c = 9;
                        break;
                    }
                case "JCD":
                    {
                        c = 10;
                        break;
                    }
                case "CMS":
                    {
                        c = 11;
                        break;
                    }
                case "STL":
                    {
                        c = 12;
                        break;
                    }
                default:
                    {
                             dt = new DateTime(1980, 1, 1);
                        decomposeVal = 0x102807E4; // Month decompose string didn't match pattern
                        return decomposeVal;
                    }
            }

            try
            {
                d = Int32.Parse(b);
                d += 437;
            }
            catch
            {
                dt = new DateTime(1980, 1, 1);
                decomposeVal = 0x102807E5; // Failed to parse year element
                return decomposeVal;
            }

            try
            {
                dt = new DateTime(d, c, (elementB - 53));
                decomposeVal = 0x0;
                return decomposeVal;
            }
            catch
            {
                dt = new DateTime(1980, 1, 1);
                decomposeVal = 0x102807E6; // Invalid date/time
                return decomposeVal;
            }
        }

        public static uint IsLegacyOs(out object osFlags, bool isUac)
        {
            // Checks the install date in the Registry. If new install the key won't exist so create it.
            // We also return the date/time of the installation, set to 23:59:59. We don't want to call attention
            // to it as a date/time so we return as an object.

            uint legacyVal = 0x102807E7; // IsLegacyOs method incomplete

            RegistryKey rootKey = Registry.ClassesRoot;
            try
            {
                RegistryKey subKey = rootKey.OpenSubKey(@"CLSID\{8E0FAC57-0D71-4942-A50D-9CB944C0B399}\ProgID", isUac);
                if (subKey == null)
                {
                    // Doesn't exist, but let's first check to see that our hidden WsRebar.xml file exists.
                    // If that file exists but the reg key doesn't, that would indicate the user trying to bypass the trial.
                    bool backupRun, goodBackup;
                    backupRun = goodBackup = false;

                    backupRun = Backup(out goodBackup);
                    if (!backupRun && goodBackup)
                    {
                        // The file exists already. This is a user trying to bypass the trial.
                        legacyVal = 0x102807EA;
                        osFlags = new DateTime(1980, 1, 1);
                        return legacyVal;
                    }
                    else if (!backupRun && !goodBackup)
                    {
                        // File doesn't exist but cannot be created.
                        legacyVal = 0x102807EB;
                        osFlags = new DateTime(1980, 1, 1);
                        return legacyVal;
                    }
                    else if (backupRun && goodBackup)
                    {
                        legacyVal = 0x102807EC;
                        osFlags = new DateTime(1980, 1, 1);
                        return legacyVal;
                    }

                    // If we got to here then we're good -- now we can create the key!
                    if (isUac)
                    {
                        subKey = rootKey.CreateSubKey(@"CLSID\{8E0FAC57-0D71-4942-A50D-9CB944C0B399}\ProgID");
                    }
                    else
                    {
                        subKey = null;
                    }
                    DateTime dt = DateTime.Now;
                    String elementA = String.Empty;
                    int elementB = -1;
                    uint retVal = Compose(dt, out elementA, out elementB);
                    if (retVal == 0x0 && isUac)
                    {
                        subKey.SetValue("Processor", elementA);
                        subKey.SetValue("Stepping", elementB);
                        subKey.Close();
                        osFlags = dt;
                        legacyVal = 0x0;
                        return legacyVal;
                    }
                    else if (retVal == 0x0 && !isUac)
                    {
                        osFlags = dt;
                        legacyVal = 0x0;
                        return legacyVal;
                    }
                    else
                    {
                        // Pass on the original failure code.
                        legacyVal = retVal;
                        osFlags = new DateTime(1980, 1, 1);
                        return legacyVal;
                    }
                }
                else
                {
                    try
                    {
                        // Validate/Create WsRebar.xml file.
                        bool backupRun, goodBackup;
                        backupRun = goodBackup = false;

                        backupRun = Backup(out goodBackup);

                        // Not doing anything now with backupRun or goodBackup. We can code failure conditions here later.

                        // Now read/verify the data.
                        String elementA = (String)subKey.GetValue("Processor");
                        int elementB = (int)subKey.GetValue("Stepping");
                        DateTime dt;
                        uint retVal = Decompose(elementA, elementB, out dt);
                        if (retVal == 0x0)
                        {
                            // We found it and it looks good. We return 0x1 (still good) since 0x0 means brand new.
                            osFlags = dt;
                            legacyVal = 0x1;
                            return legacyVal;
                        }
                        else
                        {
                            osFlags = new DateTime(1980, 1, 1);
                            // Pass on the original failure code.
                            legacyVal = retVal;
                            return legacyVal;
                        }
                    }
                    catch
                    {
                        osFlags = new DateTime(1980, 1, 1);
                        legacyVal = 0x102807E8; // Exception in Decompose try block.
                        return legacyVal;
                    }
                }
            }
            catch(Exception ex)
            {
                SiAuto.Main.LogError("[Severe] " + ex.Message);
                SiAuto.Main.LogException(ex);
                legacyVal = 0x102807E9; // Exception in outer try block.
                osFlags = new DateTime(1980, 1, 1);
                return legacyVal;
            }
        }

        public static String Concatenate(String string1, String string2, bool throwException, bool isUac, out Guid refGuid)
        {
            String string3 = String.Empty;
            string3 += string1;
            string3 += string2;
            string3 += Properties.Resources.SmartWindowStatusGeneral;
            String string4 = string3;

            // If a new user, installs the default license key (the default value of string4 (string1 + string2)). New user
            // means throwException is true. If throwException is false, we grab the existing data and return it. If an
            // exception occurs (corrupt/tampering), we return an empty string and a zeroed-out GUID.

            try
            {
                RegistryKey rootKey = Registry.ClassesRoot;
                RegistryKey subKey = rootKey.OpenSubKey(@"CLSID\{8E0FAC57-0D71-4942-A50D-9CB944C0B399}\ProgID", isUac);
                if (throwException)
                {
                    // New User
                    if (isUac)
                    {
                        subKey.SetValue("Exec", string4);
                        subKey.SetValue("GUID", "{f0f451fa-2164-426b-a9ab-b05367aeb2dc}");
                    }
                    subKey.Close();
                    refGuid = new Guid("{f0f451fa-2164-426b-a9ab-b05367aeb2dc}");
                    return string4;
                }
                else
                {
                    // Existing User
                    string4 = (String)subKey.GetValue("Exec");
                    refGuid = new Guid((String)subKey.GetValue("GUID"));
                    subKey.Close();
                    return string4;
                }
            }
            catch (Exception)
            {
                string4 = String.Empty;
                refGuid = new Guid("{00000000-0000-0000-0000-000000000000}");
                return string4;
            }
        }

        public static bool Inject(String storageElement, String refGuid)
        {
            try
            {
                RegistryKey rootKey = Registry.ClassesRoot;
                RegistryKey subKey = rootKey.CreateSubKey(@"CLSID\{8E0FAC57-0D71-4942-A50D-9CB944C0B399}\ProgID");
                subKey.SetValue("Exec", storageElement);
                subKey.SetValue("GUID", refGuid);
                subKey.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Backup(out bool goodBackup)
        {
            goodBackup = false;

            try
            {
                String backupId = Environment.GetFolderPath(Environment.SpecialFolder.System);
                if (System.IO.File.Exists(backupId + "\\WsRebar.xml"))
                {
                    goodBackup = true;
                    return false;
                }
                else
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(backupId + "\\WsRebar.xml");
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    sb.AppendLine("<runStatus>");
                    sb.AppendLine("\t<exceptionsDetected onInstall=\"0\" onRun=\"0\" />");
                    sb.AppendLine("</runStatus>");
                    writer.Write(sb.ToString());
                    writer.Flush();
                    writer.Close();
                    return true;
                }
            }
            catch(Exception ex)
            {
                SiAuto.Main.LogError("0x102807EB");
                SiAuto.Main.LogException(ex);
                return false;
            }
        }

        public static bool IsWindowsXp()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.LegacyOs.IsWindowsXp");
            SiAuto.Main.LogString("OSInfo.Name", OSInfo.Name);
            if (OSInfo.Name.ToUpper().Contains("WINDOWS XP"))
            {
                SiAuto.Main.LogMessage("Returning true.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.LegacyOs.IsWindowsXp");
                return true;
            }
            else
            {
                SiAuto.Main.LogMessage("Returning false.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.LegacyOs.IsWindowsXp");
                return false;
            }
        }

        public static bool IsXpSha256CryptoSupported()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.LegacyOs.IsXpSha256CryptoSupported");
            try
            {
                SiAuto.Main.LogMessage("Checking for existence of HKLM\\Software\\Microsoft\\Cryptography\\Defaults\\Provider\\Microsoft Enhanced RSA " +
                    "and AES Cryptographic Provider key. This key does not exist in Windows XP, even with SP-3, but WindowSMART depends on it. So if it's not " +
                    "present, we'll create it later.");
                RegistryKey machineKey = Registry.LocalMachine;
                RegistryKey sha256Key = machineKey.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography\Defaults\Provider\Microsoft Enhanced RSA and AES Cryptographic Provider");
                if (sha256Key == null)
                {
                    machineKey.Close();
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.LegacyOs.IsXpSha256CryptoSupported");
                    return false;
                }
                else
                {
                    sha256Key.Close();
                    machineKey.Close();
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.LegacyOs.IsXpSha256CryptoSupported");
                    return true;
                }
            }
            catch(Exception ex)
            {
                SiAuto.Main.LogError("Cannot perform Windows XP SHA 256 Registry check: " + ex.Message);
                SiAuto.Main.LogException(ex);
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.LegacyOs.IsXpSha256CryptoSupported");
                return false;
            }
        }

        public static void EnableXpSha256CryptoSupport()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.LegacyOs.EnableXpSha256CryptoSupport");
            try
            {
                SiAuto.Main.LogMessage("Acquiring Registry objects and creating key.");
                RegistryKey machineKey = Registry.LocalMachine;
                RegistryKey sha256Key = machineKey.CreateSubKey(@"SOFTWARE\Microsoft\Cryptography\Defaults\Provider\Microsoft Enhanced RSA and AES Cryptographic Provider");
                SiAuto.Main.LogMessage("Key created; populating values.");
                SiAuto.Main.LogMessage("Setting Registry value:  Image Path   REG_SZ      rsaenh.dll");
                sha256Key.SetValue("Image Path", "rsaenh.dll");
                SiAuto.Main.LogMessage("Setting Registry value:  Type         REG_DWORD   0x00000018");
                sha256Key.SetValue("Type", 0x00000018);
                SiAuto.Main.LogMessage("Setting Registry value:  SigInFile    REG_DWORD   0x00000000");
                sha256Key.SetValue("SigInFile", 0x00000000);
                SiAuto.Main.LogMessage("Registry updated; closing keys.");
                sha256Key.Close();
                machineKey.Close();
                SiAuto.Main.LogMessage("Registry update successful.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogFatal("[Cataclysmic] Cannot enable support for the SHA256CryptoServiceProvider: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.LegacyOs.EnableXpSha256CryptoSupport");
        }
    }
}
