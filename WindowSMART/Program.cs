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

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI
{
    static class Program
    {
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOW = 5;
        public static string SW_WINDOWS_KEY = "XtlKtnVnWtT/rFfft8WjHmhSXdFrqGT9KFkaBIJqn5Nwdq0dXakM2+NVYQDhPcDGj9qovDUnJujc4fBIrlKIpPl/LADT8Wkl+ZpW2YBMmuHjnJ";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (AdministratorNayNay())
            {
                WSSControls.BelovedComponents.QMessageBox.Show("Sorry, but use of the " + Properties.Resources.ApplicationTitle + " client has been blocked by system policy. " +
                    "Please contact your system administrator or network administrator for assistance.", "Forbidden", MessageBoxButtons.OK, MessageBoxIcon.Hand, FormStartPosition.CenterScreen);
                return;
            }

            Thread.Sleep(3000);
            bool createdNew = true;
            String userName = System.Environment.UserName + "WindowSMART2013Mutex";
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
                    uint theSlab = Components.LegacyOs.IsLegacyOs(out slobberhead, true);
                    // theSlab contains return code; slobberhead = object with date/time installed (or 1/1/1980 if bad things happened)

                    try
                    {
                        SiAuto.Main.LogMessage("Cleaning up old log files.");
                        Components.Debugging.LogPruner.ObliterateOldLogs(path, Properties.Resources.LogfilePrefix, Properties.Resources.LogfileExtension, 14);
                        
                        Application.Run(new MainForm(theSlab, (DateTime)slobberhead));
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
                    Process currentProcess = Process.GetCurrentProcess();

                    string applicationTitle = ((AssemblyTitleAttribute)Attribute
                        .GetCustomAttribute(Assembly.GetExecutingAssembly(),
                        typeof(AssemblyTitleAttribute), false)).Title;

                    var runningProcess = (from process in Process.GetProcesses()
                                          where
                                            process.Id != currentProcess.Id &&
                                            process.ProcessName.Equals(
                                              currentProcess.ProcessName,
                                              StringComparison.Ordinal)
                                          select process).FirstOrDefault();

                    if (runningProcess != null)
                    {
                        ShowWindow(runningProcess.MainWindowHandle, SW_SHOW);
                        SetForegroundWindow(runningProcess.MainWindowHandle);

                        try
                        {
                            Components.WindowsEventLogger.LogInformation("WindowSMART.exe: Too many user sessions. This session will exit. " +
                                "User: " + System.Environment.UserName + ", PID: " + currentProcess.Id.ToString(), 53870);
                        }
                        catch
                        {
                            Components.WindowsEventLogger.LogInformation("WindowSMART.exe: Too many user sessions. This session will exit. " +
                                "User: " + System.Environment.UserName + ", PID: Unavailable", 53870);
                        }
                        return;
                    }
                }
            }
        }

        private static bool AdministratorNayNay()
        {
            bool nayNay = false;
            try
            {
                // Try connecting to the Registry.
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);
                Microsoft.Win32.RegistryKey configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, false);

                if ((int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoBlockUi) == 1)
                {
                    nayNay = true;
                }
                else
                {
                    nayNay = false;
                }
                configurationKey.Close();
                dojoNorthSubKey.Close();
            }
            catch
            {
                
            }
            return nayNay;
        }
    }
}
