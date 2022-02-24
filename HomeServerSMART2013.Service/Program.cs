using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Service
{
    static class Program
    {
        public static string SW_WINDOWS_KEY = "XtlKtnVnWtT/rFfft8WjHmhSXdFrqGT9KFkaBIJqn5Nwdq0dXakM2+NVYQDhPcDGj9qovDUnJujc4fBIrlKIpPl/LADT8Wkl+ZpW2YBMmuHjnJQtuueAPfySA3unmcnl";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(String[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            try
            {
                if (IsLocalLicensePresent())
                {
                    ApplyLocalLicense();
                }
                StartService(args);
            }
            catch (Exception ex)
            {
                Components.WindowsEventLogger.LogError("Main service initialization failed: " + ex.Message +
                    "\n\n" + ex.StackTrace, 53882, Properties.Resources.EventLogJoshua);
            }
        }

        private static void StartService(String[] args)
        {
            if (args == null || args.Length == 0)
            {
                String path = String.Empty;
                String filename = Components.Utilities.Utility.GetLogFileName(Properties.Resources.LogfilePrefix, Properties.Resources.LogfileExtension, out path);
                SiAuto.Si.Connections = "file(filename=" + filename + ")";
                SiAuto.Si.Enabled = DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Utilities.Utility.IsLogEnabled();

                // License Test
                object slobberhead;
                uint theSlab = Components.LegacyOs.IsLegacyOs(out slobberhead, true);
                // theSlab contains return code; slobberhead = object with date/time installed (or 1/1/1980 if bad things happened)

                SiAuto.Main.LogColored(System.Drawing.Color.DarkGreen, "[Main] Home Server SMART 24/7 Main is invoked. If called from SCM the service will start.");
                SiAuto.Main.LogMessage("Instantiating a new HsSmartService object.");
                HSSmartService smartService = new HSSmartService(theSlab, (DateTime)slobberhead);
                SiAuto.Main.LogMessage("HSSmartService service instantiated.");
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { smartService };
                SiAuto.Main.LogMessage("Added smartService to ServiceBase[] array.");
                SiAuto.Main.LogMessage("Cleaning up old log files.");
                Components.Debugging.LogPruner.ObliterateOldLogs(path, Properties.Resources.LogfilePrefix, Properties.Resources.LogfileExtension, 14);
                SiAuto.Main.LogMessage("Invoking ServiceBase.Run()");
                ServiceBase.Run(ServicesToRun);
                SiAuto.Main.LogMessage("Invoked ServiceBase.Run()");
            }
            else
            {
                if (args.Length > 2)
                {
                    Console.WriteLine(String.Empty);
                    Console.WriteLine("Invalid number of parameters specified or invalid command specified.");
                    Console.WriteLine("Cannot execute command: " + args[0] + " " + args[1] + " " + args[2] + " " + (args.Length > 3 ? args[3] + " " : "") + (args.Length > 4 ? args[4] + " " : "") +
                        (args.Length > 5 ? args[5] + " " : "") + (args.Length > 5 ? args[5] + " " : "") + (args.Length > 6 ? args[6] + " " : "") + (args.Length > 7 ? args[7] + " " : ""));
                    Console.WriteLine("Exceptions were detected.");
                    return;
                }

                if (args.Length == 1)
                {
                    switch (args[0].ToLower())
                    {
                        case "/reboot":
                        case "/restart":
                        case "-reboot":
                        case "-restart":
                        case "--reboot":
                        case "--restart":
                            {
                                Components.ServiceHelper.ServiceStaticMethods.RebootService(false);
                                break;
                            }
                        case "/stop":
                        case "-stop":
                        case "--stop":
                            {
                                Components.ServiceHelper.ServiceStaticMethods.StopService();
                                break;
                            }
                        case "/start":
                        case "-start":
                        case "--start":
                            {
                                Components.ServiceHelper.ServiceStaticMethods.StartService();
                                break;
                            }
                        case "/status":
                        case "-status":
                        case "--status":
                            {
                                Components.ServiceHelper.ServiceStaticMethods.GetServiceStatus();
                                break;
                            }
                        case "/?":
                        case "-?":
                        case "--?":
                        case "/help":
                        case "-help":
                        case "--help":
                            {
                                Components.ServiceHelper.ServiceStaticMethods.DisplayServiceHelp();
                                break;
                            }
                        case "/voidlicense":
                        case "-voidlicense":
                        case "--voidlicense":
                            {
                                HSSmartService.CheckDotNetFourUpdate();
                                break;
                            }
                        default:
                            {
                                Console.WriteLine(String.Empty);
                                Console.WriteLine("Exceptions were detected.");
                                break;
                            }
                    }
                }

                if (args.Length == 2)
                {
                    switch (args[0].ToLower())
                    {
                        case "/reboot":
                        case "/restart":
                        case "-reboot":
                        case "-restart":
                        case "--reboot":
                        case "--restart":
                            {
                                switch (args[1].ToLower())
                                {
                                    case "/siesta":
                                    case "-siesta":
                                    case "--siesta":
                                        {

                                            Components.ServiceHelper.ServiceStaticMethods.RebootService(true);
                                            break;
                                        }
                                    default:
                                        {
                                            Console.WriteLine(String.Empty);
                                            Console.WriteLine("Exceptions were detected.");
                                            break;
                                        }
                                }
                                break;
                            }
                        default:
                            {
                                Console.WriteLine(String.Empty);
                                Console.WriteLine("Exceptions were detected.");
                                break;
                            }
                    }
                }
            }
        }

        private static void CurrentDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            if (e != null && e.ExceptionObject != null)
            {
                Exception ex = (Exception)e.ExceptionObject;
                Components.WindowsEventLogger.LogError("An unhandled exception was detected, and the service has crashed.\n\n" +
                    "Exception message: " + ex.Message + "\n\n" + "Stack trace:\n" + ex.StackTrace, 53897, Properties.Resources.EventLogJoshua);
            }
        }

        private static bool IsLocalLicensePresent()
        {
            try
            {
                String path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                String localLicense = path + "\\license.slab";
                return System.IO.File.Exists(localLicense);
            }
            catch
            {
            }
            return false;
        }

        private static void ApplyLocalLicense()
        {
            int retVal = -1;
            String errorMessage = String.Empty;
            String slabLicenseFile = String.Empty;
            try
            {
                String path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                slabLicenseFile = path + "\\license.slab";
                retVal = DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Mexi_Sexi.Enterprise.DoEnterpriseTransaction(slabLicenseFile);
            }
            catch(Exception ex)
            {
                retVal = 0xFFFF;
                errorMessage = ex.ToString();
            }

            try
            {
                System.IO.File.Delete(slabLicenseFile);
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                retVal = 0x1;
            }

            if (retVal == 0x0 || retVal == 0x1)
            {
                String message = "Successfully consumed and applied the deployed license file.";
                if (retVal == 0x1)
                {
                    message += " An error occurred deleting the license file. (" + errorMessage + ") Please delete it manually.";
                }
                message += "\n\nStatus Code: 0x" + retVal.ToString("X");
                Components.WindowsEventLogger.LogInformation(message, 53872);
            }
            else
            {
                String message = "An error occurred consuming and/or applying the deployed license file. The status code is shown below, along with " +
                    "an error message (if one was given).\n\nError Message: " + errorMessage + "\n\nStatus Code: 0x" + retVal.ToString();
                Components.WindowsEventLogger.LogWarning(message, 53873);
            }
        }
    }
}
