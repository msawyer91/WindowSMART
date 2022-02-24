using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components;
using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Service
{
    partial class HSSmartService : ServiceBase
    {
        private bool running = false;
        private HssServiceHelper helper;
        private DateTime result;
        Guid refGuid = new Guid("{00000000-0000-0000-0000-000000000000}");
        private uint retVal;
        private String xmlText;
        private bool isWorkerProcessRunning;

        public HSSmartService(uint hyperSlab, DateTime babyHead)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Service.HSSmartService");
            SiAuto.Main.LogMessage("Checking beta license.");

            #region BETA Section - Comment Out in Production
            //DateTime expirationDate = Components.License.GetExpirationDate();
            //try
            //{
            //    if (Components.License.IsExpired(expirationDate, this, true))
            //    {
            //        // Do nothing - the true will cause an exception to be thrown and exit.
            //    }
            //}
            //catch (LicenseException lex)
            //{
            //    SiAuto.Main.LogFatal("The beta license is expired. Please download a new version.");
            //    SiAuto.Main.LogException(lex);
            //    throw;
            //}
            #endregion BETA Section - Comment Out in Production

            SiAuto.Main.LogMessage("Setting worker process flag.");
            isWorkerProcessRunning = false;
            
            SiAuto.Main.LogMessage("Initializing the service: InitializeComponent.");

            InitializeComponent();

            // Now we do the real checking.
            String concatenatedString = String.Empty;
            retVal = hyperSlab;
            if (hyperSlab == 0x0)
            {
                // New user
                concatenatedString = Components.LegacyOs.Concatenate(Program.SW_WINDOWS_KEY, Properties.Resources.RegistryConfigHelperServiceKey, true, true, out refGuid);
            }
            else if (hyperSlab == 0x1)
            {
                // Existing user and no errors returned with info
                concatenatedString = Components.LegacyOs.Concatenate(Program.SW_WINDOWS_KEY, Properties.Resources.RegistryConfigHelperServiceKey, false, true, out refGuid);
            }
            else
            {
                // Date was invalid but we still grab the license because it could be valid (if it is we don't care about the date).
                concatenatedString = "0xF," + Components.LegacyOs.Concatenate(Program.SW_WINDOWS_KEY, Properties.Resources.RegistryConfigHelperServiceKey, false, true, out refGuid);
            }
            xmlText = concatenatedString;
            result = babyHead;
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Service.HSSmartService");
        }

        /// <summary>
        /// Runs when the service has been signaled by Service Control Manager to start. Start the service in a worker
        /// thread. This way the OnStart method exits quickly. If things take too long to run in the OnStart method, the
        /// SCM may terminate the service.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Service.OnStart");
            SiAuto.Main.LogMessage("Home Server SMART/WindowSMART v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " startup in progress");
            SiAuto.Main.LogMessage("Build Name: I Prevail (Engineered to Amaze)");
            SiAuto.Main.LogMessage("Copyright © Dojo North Software, LLC 2018");
            Thread t = new Thread(new ThreadStart(ServiceThread));
            t.Name = "HSS_ServiceMain";
            t.Start();
            SiAuto.Main.LogMessage("Home Server SMART/WindowSMART v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " started");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Service.OnStart");
        }

        /// <summary>
        /// Runs when the service has been signaled by Service Control Manager to stop. Set the running flag to false,
        /// which allows the service thread to exit, and thus the service will shut down.
        /// </summary>
        protected override void OnStop()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Service.OnStop");
            SiAuto.Main.LogMessage("[Teardown] Home Server SMART has been ordered to halt.");
            running = false;
            SiAuto.Main.LogBool("running", running);
            SiAuto.Main.LogMessage("[Teardown] running flag has been set to false; worker thread will exit.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Service.OnStop");
        }

        /// <summary>
        /// Runs when the service has been signaled by Service Control Manager to shut down. Set the running flag to false,
        /// which allows the service thread to exit, and thus the service will shut down.
        /// </summary>
        protected override void OnShutdown()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Service.OnShutdown");
            SiAuto.Main.LogMessage("[Teardown] Home Server SMART has been ordered to halt.");
            running = false;
            SiAuto.Main.LogBool("running", running);
            SiAuto.Main.LogMessage("[Teardown] running flag has been set to false; worker thread will exit.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Service.OnShutdown");
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Service.OnPowerEvent");
            SiAuto.Main.LogWarning("A power state change was detected on the Server.");
            switch (powerStatus)
            {
                case PowerBroadcastStatus.BatteryLow:
                    {
                        SiAuto.Main.LogMessage("Battery power is low.");
                        if (helper != null)
                        {
                            try
                            {
                                helper.PostPowerNotificate("Battery power is low on computer " + Environment.MachineName + ". Consider saving your work or plugging in the computer. If the battery " +
                                    "power gets low enough, the computer may shut down unexpectedly and you could lose any unsaved work.", "Battery Low", true, "Power Event", 65530);
                            }
                            catch
                            {
                            }
                        }
                        break;
                    }
                case PowerBroadcastStatus.OemEvent:
                    {
                        SiAuto.Main.LogMessage("OEM power event detected.");
                        break;
                    }
                case PowerBroadcastStatus.PowerStatusChange:
                    {
                        SiAuto.Main.LogMessage("Power status change - battery to A/C or vice versa.");
                        if (helper != null)
                        {
                            try
                            {
                                helper.PostPowerNotificate("Windows reported a power status change on computer " + Environment.MachineName + ". This can occur if a " +
                                    "computer switches from AC to battery by becoming unplugged or if a power failure occurred while plugged into an uninterruptable " +
                                    "power supply. Windows also reports this condition if the battery percentage changes drastically in a short time. If this is " +
                                    "unexpected, it is possible there is a power outage at this computer's location.", "Power Status Change", true, "Power Event", 65531);
                            }
                            catch
                            {
                            }
                        }
                        break;
                    }
                case PowerBroadcastStatus.QuerySuspend:
                    {
                        SiAuto.Main.LogMessage("System is requesting permission to suspend.");
                        break;
                    }
                case PowerBroadcastStatus.QuerySuspendFailed:
                    {
                        SiAuto.Main.LogMessage("System was denied permission to suspend.");
                        break;
                    }
                case PowerBroadcastStatus.ResumeAutomatic:
                    {
                        SiAuto.Main.LogMessage("System has awakened to handle an automatic event.");
                        break;
                    }
                case PowerBroadcastStatus.ResumeCritical:
                    {
                        SiAuto.Main.LogMessage("System has resumed from a failing battery.");
                        if (helper != null)
                        {
                            try
                            {
                                helper.PostPowerNotificate("Windows reported that computer " + Environment.MachineName + " has resumed from a critical or failing battery. This could be " +
                                    "because you were using the computer and ran the battery too low, causing Windows to force suspension or " +
                                    "hibernation. If this was unexpected, the battery may need to be replaced.", "Battery Critical", true, "Power Event", 65532);
                            }
                            catch
                            {
                            }
                        }
                        break;
                    }
                case PowerBroadcastStatus.ResumeSuspend:
                    {
                        SiAuto.Main.LogMessage("Resume suspend broadcast event; apps can resume full user interaction.");
                        break;
                    }
                case PowerBroadcastStatus.Suspend:
                    {
                        SiAuto.Main.LogMessage("System is about to enter a suspended state.");
                        break;
                    }
                default:
                    {
                        SiAuto.Main.LogMessage("Unknown power event.");
                        break;
                    }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Service.OnPowerEvent");
            return true;
        }

        private void ServiceThread()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Service.ServiceThread");
            try
            {
                running = true;
                SiAuto.Main.LogMessage("Instantiate HssServiceHelper in singleton instance.");
                helper = HssServiceHelper.GetInstance(xmlText, result, refGuid, retVal);
                SiAuto.Main.LogObject("HssServiceHelper instantiated.", helper);
                int pollingInterval = helper.ServicePollingInterval;
                SiAuto.Main.LogInt("Polling interval is configured.", pollingInterval);
                SiAuto.Main.LogDouble("Default polling interval.", attributeRefresh.Interval);

                if (attributeRefresh.Interval != pollingInterval)
                {
                    SiAuto.Main.LogMessage("Default polling interval and user-defined interval are different; adjusting attributeRefresh interval value.");
                    attributeRefresh.Interval = pollingInterval;
                }
                SiAuto.Main.LogMessage("Start attributeRefresh.");
                attributeRefresh.Start();
                SiAuto.Main.LogMessage("Timer attributeRefresh is started.");

                SiAuto.Main.LogMessage("Starting updateGanderizer. This checks for updates every 2 hours, although the actual web service call only occurs twice per day.");
                updateGanderizer.Start();
                SiAuto.Main.LogMessage("Timer updateGanderizer is started.");

                SiAuto.Main.LogMessage("Checking for Updates.");
                helper.ServiceCheckForUpdates();
                SiAuto.Main.LogMessage("Update check done.");

                SiAuto.Main.LogMessage("Service initialization is complete. Main service thread entering while(running) loop.");
                while (running)
                {
                    Thread.Sleep(1000);
                    pollingInterval = helper.ServicePollingInterval;
                    helper.RefreshPowerEventNotifyState();

                    if (!running)
                    {
                        SiAuto.Main.LogWarning("Running flag has changed to false; exiting.");
                        break;
                    }

                    if (attributeRefresh.Interval != pollingInterval)
                    {
                        SiAuto.Main.LogDouble("attributeRefresh.Interval", attributeRefresh.Interval);
                        SiAuto.Main.LogDouble("pollingInterval", pollingInterval);
                        SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "User changed polling interval. Worker thread will be restarted.");
                        SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Stopping attributeRefresh worker process.");
                        attributeRefresh.Stop();
                        SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Worker process signaled to stop. Sleeping 5 seconds to allow thread to exit.");
                        if (!running)
                        {
                            SiAuto.Main.LogWarning("Running flag has changed to false; skipping the siesta and exiting.");
                            break;
                        }
                        else
                        {
                            Thread.Sleep(5000);
                        }
                        SiAuto.Main.LogDouble("Old timer interval.", attributeRefresh.Interval);
                        SiAuto.Main.LogInt("New polling interval.", helper.ServicePollingInterval);
                        attributeRefresh.Interval = helper.ServicePollingInterval;
                        SiAuto.Main.LogMessage("Timer interval is reset. Starting attributeRefresh worker process.");
                        Thread.Sleep(1000);
                        attributeRefresh.Start();
                        SiAuto.Main.LogMessage("Workper process is restarted with new polling interval.");
                    }

                    //helper.PerformDriveBenderBitLockerTasks();
                }
                SiAuto.Main.LogMessage("[Teardown] Worker thread detected running flag has been set to false. Beginning demolition.");
                try
                {
                    SiAuto.Main.LogMessage("[Teardown] Signaling the timers to halt.");
                    attributeRefresh.Stop();
                    SiAuto.Main.LogMessage("[Teardown] attributeRefresh has been halted.");
                    updateGanderizer.Stop();
                    SiAuto.Main.LogMessage("[Teardown] updateGanderizer has been halted.");
                }
                catch (Exception timerException)
                {
                    SiAuto.Main.LogError("[Teardown] Exception was thrown signaling the timers to halt: " + timerException);
                    SiAuto.Main.LogException(timerException);
                }
                helper.TearDown();
                SiAuto.Main.LogMessage("[Teardown] The Service Helper has been demolished.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogFatal("Exceptions were detected within the main service thread. The service is halting.");
                SiAuto.Main.LogException(ex);
                SiAuto.Main.LogMessage("[Teardown] Attempting demolition of reserved resources to free memory.");
                try
                {
                    SiAuto.Main.LogMessage("[Teardown] Signaling the timers to halt.");
                    attributeRefresh.Stop();
                    SiAuto.Main.LogMessage("[Teardown] attributeRefresh has been halted.");
                    updateGanderizer.Stop();
                    SiAuto.Main.LogMessage("[Teardown] updateGanderizer has been halted.");
                }
                catch (Exception timerException)
                {
                    SiAuto.Main.LogError("[Teardown] Exception was thrown signaling the timers to halt: " + timerException);
                    SiAuto.Main.LogException(timerException);
                }

                try
                {
                    if (helper == null)
                    {
                        SiAuto.Main.LogWarning("[Teardown] Helper object is null; nothing to demolish.");
                    }
                    else
                    {
                        helper.TearDown();
                        SiAuto.Main.LogMessage("[Teardown] The Service Helper has been demolished.");
                    }
                }
                catch (Exception tearDownException)
                {
                    SiAuto.Main.LogError("[Teardown] Demolition failed: " + tearDownException);
                    SiAuto.Main.LogException(tearDownException);
                }
                WindowsEventLogger.LogError("Exceptions were detected in the service worker thread. " + ex.Message +
                    "\n\nThe service is halting.", 53883, Properties.Resources.EventLogJoshua);
            }
            finally
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Service.ServiceThread");
            }
        }

        /// <summary>
        /// Event fires when the polling interval elapses. Polling interval is every 5 minutes. Re-reads the configuration
        /// and refreshes all of the data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void attributeRefresh_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Service.attributeRefresh_Elapsed");
            helper.ServiceAutoPollDisks();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Service.attributeRefresh_Elapsed");
        }

        private void updateGanderizer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Service.updateGanderizer_Elapsed");
            helper.ServiceCheckForUpdates();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Service.updateGanderizer_Elapsed");
        }

        public static void CheckDotNetFourUpdate()
        {
            try
            {
                if (Components.OperatingSystem.IsWindowsServerSolutionsProduct(String.Empty))
                {
                    Console.WriteLine("The requested operation is not supported on this version of Windows.");
                    Console.WriteLine("Exceptions were detected.");
                    return;
                }
                Guid dotNetFourCurrent = new Guid("{00000000-0000-0000-0000-000000000000}");
                Guid dotNetFourUpdateAvailable = new Guid("{00000000-0000-0000-0000-000000000000}");
                String result = Components.LegacyOs.Concatenate(Program.SW_WINDOWS_KEY, Properties.Resources.RegistryConfigHelperServiceKey, true, true, out dotNetFourCurrent);
                if (dotNetFourCurrent != dotNetFourUpdateAvailable)
                {
                    Console.WriteLine("The WindowSMART license has been invalidated and will revert to trial mode");
                    Console.WriteLine("the next time the computer is rebooted. Please reboot the computer at your");
                    Console.WriteLine("earliest convenience. If you do not wish to reboot, you can complete the");
                    Console.WriteLine("process by restarting the WindowSMART service, and exiting out of and re-");
                    Console.WriteLine("launching any running instances of the WindowSMART console and WindowSMART");
                    Console.WriteLine("tray application.");
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}
