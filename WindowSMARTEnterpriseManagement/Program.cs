using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components;

namespace WindowSMARTEnterpriseManagement
{
    public class Program
    {
        
        public static int Main(string[] args)
        {
            Console.WriteLine(String.Empty);
            Console.WriteLine("WindowSMART 24/7 Enterprise Management Tool v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine("Copyright © 2010-2018 Dojo North Software, LLC");
            Console.WriteLine(String.Empty);

            if (args.Length == 0 || (args.Length == 1 && (args[0] == "/?" || args[0] == "-?")))
            {
                Console.WriteLine("WARNING: This tool is deprecated. Please use our PowerShell management");
                Console.WriteLine("interface instead.");
                Console.WriteLine(String.Empty);
                Console.WriteLine("The WindowSMART 24/7 Enterprise Management Tool provides a command-line");
                Console.WriteLine("interface for system administrators to script and automate the deployment");
                Console.WriteLine("and management of WindowSMART 24/7.");
                Console.WriteLine(String.Empty);
                Console.WriteLine("The following arguments are supported:");
                Console.WriteLine(String.Empty);
                Console.WriteLine("   /key slablicensefile [/autorestart]");
                Console.WriteLine("     Applies the license key in the Slab license file. Changing the license");
                Console.WriteLine("     requires a service restart; specify /autorestart to restart the service");
                Console.WriteLine("     if the key is successfully applied. Note that if the path to the Slab");
                Console.WriteLine("     license file contains spaces, it must be enclosed in quotes.");
                Console.WriteLine(String.Empty);
                Console.WriteLine("   /supportmessage \"message\"");
                Console.WriteLine("     Sets a custom support message for users. If your environment is configured");
                Console.WriteLine("     such that end users can receive WindowSMART alerts, they may be frightened");
                Console.WriteLine("     or unsure of what action to take. Here you can specify a custom support");
                Console.WriteLine("     message with instructions on what they should so, such as, \"Please ");
                Console.WriteLine("     contact the Helpdesk at +1-999-555-1111, option 4 for further assistance.\"");
                Console.WriteLine("     Unless the message is one word, it must be enclosed in quotes.");
                Console.WriteLine("     If your message is longer than 256 characters, it will be truncated.");
                Console.WriteLine(String.Empty);
                Console.WriteLine("   /supportmessage /delete");
                Console.WriteLine("     Deletes the custom support message if one is defined.");
                Console.WriteLine(String.Empty);
                Console.WriteLine("   /refreshpolicy /tattoo");
                Console.WriteLine("     Removes GPO \"tattoo\" from WindowSMART configuration. This should be run");
                Console.WriteLine("     if you change any policy settings to Not Configured, or if you stop using");
                Console.WriteLine("     GPOs for WindowSMART settings.");

                return 0x0;
            }
            else if (args.Length < 2 || args.Length > 3)
            {
                Console.WriteLine("WARNING: This tool is deprecated. Please use our PowerShell management");
                Console.WriteLine("interface instead.");
                Console.WriteLine(String.Empty);
                Console.WriteLine("Invalid number of parameters specified. Use /? for help.");
                Console.WriteLine("Exceptions were detected.");
                return 0x2;
            }
            else if (args.Length == 2)
            {
                switch (args[0].ToLower())
                {
                    case "/key":
                    case "-key":
                    case "--key":
                        {
                            Console.WriteLine("WARNING: This tool is deprecated. Please use our PowerShell management");
                            Console.WriteLine("interface instead. See cmdlet 'Set-WindowSmartLicenseKey'.");
                            Console.WriteLine(String.Empty);
                            return ApplyLicenseKey(args[1], false);
                        }
                    case "/supportmessage":
                    case "-supportmessage":
                    case "--supportmessage":
                        {
                            Console.WriteLine("WARNING: This tool is deprecated. Please use our PowerShell management");
                            Console.WriteLine("interface instead. See cmdlet 'Set-WindowSmartSupportMessage'.");
                            Console.WriteLine(String.Empty);
                            return SetSupportMessage(args[1]);
                        }
                    case "/refreshpolicy":
                    case "-refreshpolicy":
                    case "--refreshpolicy":
                        {
                            Console.WriteLine("WARNING: This tool is deprecated. Please use our PowerShell management");
                            Console.WriteLine("interface instead. See cmdlet 'Clear-WindowSmartPolicy'.");
                            Console.WriteLine(String.Empty);
                            return TattooPolicy(args[1]);
                        }
                    default:
                        {
                            Console.WriteLine("WARNING: This tool is deprecated. Please use our PowerShell management");
                            Console.WriteLine("interface instead.");
                            Console.WriteLine(String.Empty);
                            Console.WriteLine("Invalid parameter(s) specified. Use /? for help.");
                            Console.WriteLine("Exceptions were detected.");
                            return 0x3;
                        }
                }
            }
            else if (args.Length == 3)
            {
                switch (args[0].ToLower())
                {
                    case "/key":
                    case "-key":
                    case "--key":
                        {
                            Console.WriteLine("WARNING: This tool is deprecated. Please use our PowerShell management");
                            Console.WriteLine("interface instead. See cmdlet 'Set-WindowSmartLicenseKey'.");
                            Console.WriteLine(String.Empty);
                            String thirdArg = args[2].ToLower();
                            if (thirdArg != "/autorestart" && thirdArg != "-autorestart" && thirdArg != "--autorestart")
                            {
                                Console.WriteLine("Invalid parameter(s) specified. Use /? for help.");
                                Console.WriteLine("Exceptions were detected.");
                                return 0x4;
                            }

                            return ApplyLicenseKey(args[1], true);
                        }
                    default:
                        {
                            Console.WriteLine("WARNING: This tool is deprecated. Please use our PowerShell management");
                            Console.WriteLine("interface instead.");
                            Console.WriteLine(String.Empty);
                            Console.WriteLine("Invalid parameter(s) specified. Use /? for help.");
                            Console.WriteLine("Exceptions were detected.");
                            return 0x3;
                        }
                }
            }
            else
            {
                Console.WriteLine("WARNING: This tool is deprecated. Please use our PowerShell management");
                Console.WriteLine("interface instead.");
                Console.WriteLine(String.Empty);
                Console.WriteLine("Unknown error or parameter. Use /? for help.");
                Console.WriteLine("Exceptions were detected.");
                return 0x5;
            }
        }

        private static int ApplyLicenseKey(String slabLicenseFile, bool restartService)
        {
            try
            {
                if (!System.IO.File.Exists(slabLicenseFile))
                {
                    Console.WriteLine("Slab License File Not Found: " + slabLicenseFile);
                    Console.WriteLine("Exceptions were detected.");
                    return 0x6;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking for presence of Slab license file: " + ex.Message);
                Console.WriteLine("Exceptions were detected.");
                return 0x7;
            }

            try
            {
                int retVal = DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Mexi_Sexi.Enterprise.DoEnterpriseTransaction(slabLicenseFile);
                if (retVal == 0x0)
                {
                    if((restartService))
                    {
                        if (DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.ServiceHelper.ServiceStaticMethods.RebootService(false) == 0x0)
                        {
                            Console.WriteLine("The command completed successfully.");
                            return 0x0;
                        }
                        else
                        {
                            Console.WriteLine("The license was applied but the service could not be rebooted.");
                            return 0x1;
                        }
                    }
                    else
                    {
                        Console.WriteLine("The command completed successfully.");
                        return 0x0;
                    }
                }
                else
                {
                    Console.WriteLine("Exceptions were detected: " + retVal.ToString());
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exceptions were detected: " + ex.ToString());
                return 0x9;
            }
        }

        private static int TattooPolicy(String command)
        {
            String arg = command.ToLower();
            if (arg != "/tattoo" && arg != "-tattoo" && arg != "--tattoo")
            {
                Console.WriteLine("Invalid parameter(s) specified. Use /? for help.");
                Console.WriteLine("Exceptions were detected.");
                return 0x4;
            }

            try
            {
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);
                Microsoft.Win32.RegistryKey configurationKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryConfigurationKey);

                Console.WriteLine("Removing GPO tattooed settings.");
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoDpa, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoTempCtl, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoVirtualIgnore, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoAllowIgnoredItems, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoEmailNotificate, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoBoxcarNotificate, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoProwlNotificate, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoNmaNotificate, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoWindowsPhoneNotificate, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoGrowlNotificate, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoSnarlNotificate, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoAdvancedSettings, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoDebuggingControl, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoSSD, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoCheckForUpdates, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoUiTheme, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigLogRequirement, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoDeleteStale, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoRestoreDefaults, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoResetEverything, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoAllowServiceControl, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoAllowShutdownReboot, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoUseSupportMessage, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoBlockUi, false);
                configurationKey.DeleteValue(Properties.Resources.RegistryConfigGpoEmergencyBackup, false);
                Console.WriteLine("GPO-tatted settings have been cleared. Please note if a GPO exists that");
                Console.WriteLine("configures these settings, they will be re-tatted by the GPO.");
                Console.WriteLine(String.Empty);
                Console.WriteLine("The command completed successfully.");
                return 0x0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exceptions were detected: " + ex.Message);
                return 0xF;
            }
        }

        private static int SetSupportMessage(String message)
        {
            try
            {
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);
                Microsoft.Win32.RegistryKey configurationKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryConfigurationKey);
                if (message.Length > 256)
                {
                    message = message.Substring(0, 256);
                    Console.WriteLine("Warning - Message exceeds 256 characters; message truncated.");
                }
                else if (message.ToLower() == "/delete" || message.ToLower() == "-delete" || message.ToLower() == "--delete")
                {
                    Console.WriteLine("Custom support message will be removed.");
                    message = String.Empty;
                }
                configurationKey.SetValue(Properties.Resources.RegistryConfigCustomSupportMessage, message);
                configurationKey.Close();
                dojoNorthSubKey.Close();
                registryHklm.Close();
                Console.WriteLine("The command completed successfully.");
                return 0x0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exceptions were detected: " + ex.Message);
                return 0xE;
            }
        }
    }
}
