using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls;
using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.WindowSMARTTray
{
    static class Program
    {
        public static string SW_WINDOWS_KEY = "XtlKtnVnWtT/rFfft8WjHmhSXdFrqGT9KFkaBIJqn5Nwdq0dXakM2+NVYQDhPcDGj9qovDUnJujc4fBIrlKIpPl/LADT8Wkl+ZpW2YBMmuHjnJ";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            bool runConsole = args != null && args.Length == 1 && String.Compare(args[0], "/console", true) == 0;
            if (runConsole)
            {
                try
                {
                    System.Diagnostics.ProcessStartInfo psi;
                    psi = new System.Diagnostics.ProcessStartInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\WindowSMART.exe");
                    psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                    System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);
                }
                catch (Exception)
                {
                    // Fail silently.
                    Application.Exit();
                }
            }
            else
            {
                Thread.Sleep(3000);
                bool createdNew = true;
                String userName = System.Environment.UserName + "WindowSMART2013TrayMutex";
                using (Mutex mutex = new Mutex(true, userName, out createdNew))
                {
                    if (createdNew)
                    {
                        String path = String.Empty;
                        SiAuto.Si.Connections = "file(filename=" + Components.Utilities.Utility.GetLogFileName(
                            Properties.Resources.LogfilePrefix, Properties.Resources.LogfileExtension, out path) + ")";
                        SiAuto.Si.Enabled = DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Utilities.Utility.IsLogEnabled();
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        // License Test
                        object slobberhead;
                        uint theSlab = Components.LegacyOs.IsLegacyOs(out slobberhead, false);
                        // theSlab contains return code; slobberhead = object with date/time installed (or 1/1/1980 if bad things happened)

                        try
                        {
                            SiAuto.Main.LogMessage("Cleaning up old log files.");
                            Components.Debugging.LogPruner.ObliterateOldLogs(path, Properties.Resources.LogfilePrefix, Properties.Resources.LogfileExtension, 14);

                            Application.Run(new TrayForm(theSlab, (DateTime)slobberhead));
                        }
                        catch (System.ComponentModel.LicenseException lex)
                        {
                            // License is expired.
                            SiAuto.Main.LogError("[I Prevail] The beta product license is expired. Please download a newer version.");
                            SiAuto.Main.LogException(lex);
                            EpicFail fail = new EpicFail(lex, "Fatal license error. The application will now close.",
                                "Your beta license has expired. You must download a newer version. Please visit http://www.dojonorthsoftware.net to download a new version.", false);
                            fail.ShowDialog();
                            SiAuto.Main.LogMessage("[I Prevail] Shutting down due to error.");
                            Application.Exit();
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogFatal("[I Prevail] Fatal Error: " + ex.Message);
                            SiAuto.Main.LogException(ex);
                            EpicFail fail = new EpicFail(ex, ex.Message, "Try restarting WindowSMART. If the problem persists, please tender a bug report.", false);
                            fail.ShowDialog();
                            Application.Exit();
                        }
                    }
                    else
                    {
                        try
                        {
                            Components.WindowsEventLogger.LogInformation("WindowSMARTTray.exe: Too many user sessions. This session will exit. " +
                                "User: " + System.Environment.UserName + ", PID: " + Process.GetCurrentProcess().Id.ToString(), 53870);
                        }
                        catch
                        {
                            Components.WindowsEventLogger.LogInformation("WindowSMARTTray.exe: Too many user sessions. This session will exit. " +
                                "User: " + System.Environment.UserName + ", PID: Unavailable", 53870);
                        }
                        Application.Exit();
                    }
                }
            }
        }
    }
}
