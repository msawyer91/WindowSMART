using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.ServiceHelper
{
    public sealed class ServiceStaticMethods
    {
        public static uint RebootService(bool takeSiesta)
        {
            ServiceController controller;

            // Make sure the service is in a stoppable state.
            try
            {
                controller = new ServiceController(Properties.Resources.ServiceNameHss);
                if (controller.Status != ServiceControllerStatus.Running)
                {
                    Console.WriteLine(String.Empty);
                    Console.WriteLine("Service must be in a Running state to restart it. Current state: " + controller.Status.ToString());
                    Console.WriteLine("Exceptions were detected.");
                    return 0x1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Empty);
                Console.WriteLine("Service bind failed: " + ex.Message);
                Console.WriteLine("Exceptions were detected.");
                return 0x2;
            }

            // Stop the service.
            try
            {
                Console.WriteLine(String.Empty);
                Console.WriteLine("Attempting to stop service...");
                Thread.Sleep(500);
                controller.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Stop command failed: " + ex.Message);
                Console.WriteLine("Exceptions were detected.");
                controller.Close();
                controller.Dispose();
                return 0x3;
            }

            // Give the service 15 seconds to wrap things up.
            int timer = 0;
            controller.Refresh();
            while (timer < 15 && controller.Status != ServiceControllerStatus.Stopped)
            {
                System.Threading.Thread.Sleep(1000);
                controller.Refresh();
                timer++;
            }

            if (controller.Status == ServiceControllerStatus.Stopped)
            {
                Console.WriteLine("Service stopped successfully.");
                Thread.Sleep(500);
            }
            else
            {
                Console.WriteLine("Service did not stop in a reasonable amount of time.");
                Console.WriteLine("Exceptions were detected.");
                controller.Close();
                controller.Dispose();
                return 0x4;
            }
            controller.Refresh();

            timer = 0;
            Console.WriteLine("Attempting to start service...");

            try
            {
                controller.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Start command failed: " + ex.Message);
                Console.WriteLine("Exceptions were detected.");
                controller.Close();
                controller.Dispose();
                return 0x5;
            }

            controller.Refresh();
            while (timer < 15 && controller.Status != ServiceControllerStatus.Running)
            {
                System.Threading.Thread.Sleep(1000);
                controller.Refresh();
                timer++;
            }

            if (controller.Status == ServiceControllerStatus.Running)
            {
                Console.WriteLine("Service started successfully.");
                controller.Close();
                controller.Dispose();
                if (takeSiesta)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("Waiting 5 seconds to allow the service time to initialize and complete");
                    Console.WriteLine("an initial polling. If you see question marks next to disks after this");
                    Console.WriteLine("window closes, just click Fast Refresh.");
                    Thread.Sleep(5000);
                }
                return 0x0;
            }
            else
            {
                Console.WriteLine("Service did not start in a reasonable amount of time.");
                Console.WriteLine("Exceptions were detected.");
                controller.Close();
                controller.Dispose();
                return 0x7;
            }
        }

        public static uint StopService()
        {
            ServiceController controller;

            // Make sure the service is in a stoppable state.
            try
            {
                controller = new ServiceController(Properties.Resources.ServiceNameHss);
                if (controller.Status != ServiceControllerStatus.Running)
                {
                    Console.WriteLine(String.Empty);
                    Console.WriteLine("Service must be in a Running state to stop it. Current state: " + controller.Status.ToString());
                    Console.WriteLine("Exceptions were detected.");
                    controller.Close();
                    controller.Dispose();
                    return 0x1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Empty);
                Console.WriteLine("Service bind failed: " + ex.Message);
                Console.WriteLine("Exceptions were detected.");
                return 0x2;
            }

            // Stop the service.
            try
            {
                Console.WriteLine(String.Empty);
                Console.WriteLine("Attempting to stop service...");
                Thread.Sleep(500);
                controller.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Stop command failed: " + ex.Message);
                Console.WriteLine("Exceptions were detected.");
                controller.Close();
                controller.Dispose();
                return 0x3;
            }

            // Give the service 15 seconds to wrap things up.
            int timer = 0;
            controller.Refresh();
            while (timer < 15 && controller.Status != ServiceControllerStatus.Stopped)
            {
                System.Threading.Thread.Sleep(1000);
                controller.Refresh();
                timer++;
            }

            if (controller.Status == ServiceControllerStatus.Stopped)
            {
                Console.WriteLine("Service stopped successfully.");
                controller.Close();
                controller.Dispose();
                return 0x0;
            }
            else
            {
                Console.WriteLine("Service did not stop in a reasonable amount of time.");
                Console.WriteLine("Exceptions were detected.");
                controller.Close();
                controller.Dispose();
                return 0x4;
            }
        }

        public static uint StartService()
        {
            ServiceController controller;

            // Make sure the service is in a startable state.
            try
            {
                controller = new ServiceController(Properties.Resources.ServiceNameHss);
                if (controller.Status != ServiceControllerStatus.Stopped)
                {
                    Console.WriteLine(String.Empty);
                    Console.WriteLine("Service must be in a Stopped state to start it. Current state: " + controller.Status.ToString());
                    Console.WriteLine("Exceptions were detected.");
                    controller.Close();
                    controller.Dispose();
                    return 0x1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Empty);
                Console.WriteLine("Service bind failed: " + ex.Message);
                Console.WriteLine("Exceptions were detected.");
                return 0x2;
            }

            // Stop the service.
            try
            {
                Console.WriteLine(String.Empty);
                Console.WriteLine("Attempting to start service...");
                Thread.Sleep(500);
                controller.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Stop command failed: " + ex.Message);
                Console.WriteLine("Exceptions were detected.");
                controller.Close();
                controller.Dispose();
                return 0x3;
            }

            // Give the service 15 seconds to start.
            int timer = 0;
            controller.Refresh();
            while (timer < 15 && controller.Status != ServiceControllerStatus.Running)
            {
                System.Threading.Thread.Sleep(1000);
                controller.Refresh();
                timer++;
            }

            if (controller.Status == ServiceControllerStatus.Running)
            {
                Console.WriteLine("Service started successfully.");
                controller.Close();
                controller.Dispose();
                return 0x0;
            }
            else
            {
                Console.WriteLine("Service did not start in a reasonable amount of time.");
                Console.WriteLine("Exceptions were detected.");
                controller.Close();
                controller.Dispose();
                return 0x4;
            }
        }

        public static void GetServiceStatus()
        {
            ServiceController controller;

            try
            {
                controller = new ServiceController(Properties.Resources.ServiceNameHss);

                Console.WriteLine(String.Empty);
                Console.WriteLine("Home Server SMART 24/7 Service Manager v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                Console.WriteLine("Copyright © Dojo North Software, LLC 2018");
                Console.WriteLine(String.Empty);
                Console.WriteLine("     Service Name        : " + controller.ServiceName);
                Console.WriteLine("     Service Display Name: " + controller.DisplayName);
                Console.WriteLine("     Service Status      : " + controller.Status.ToString());
                Console.WriteLine("     Server              : " + System.Environment.MachineName);
                Console.WriteLine("     Execution Flags     : " + (controller.CanPauseAndContinue ? "CAN_PAUSE " : "") +
                    (controller.CanShutdown ? "SHUTDOWN_NOTIFY " : "") + (controller.CanStop ? "CAN_STOP" : ""));
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Empty);
                Console.WriteLine("Service bind failed: " + ex.Message);
                Console.WriteLine("Exceptions were detected.");
                return;
            }
        }

        public static void DisplayServiceHelp()
        {
            Console.WriteLine(String.Empty);
            Console.WriteLine("WindowSMART/Home Server SMART 24/7 Service Manager v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine("Copyright © Dojo North Software, LLC 2018");
            Console.WriteLine(String.Empty);
            Console.WriteLine("Usage:");
            Console.WriteLine("     HomeServerSMART2013.Service.exe command [argument]");
            Console.WriteLine(String.Empty);
            Console.WriteLine("Commands (no arguments):");
            Console.WriteLine("     /restart            Restarts the Home Server SMART service");
            Console.WriteLine("     /reboot             Restarts the Home Server SMART service");
            Console.WriteLine("     /stop               Stops the Home Server SMART service");
            Console.WriteLine("     /start              Starts the Home Server SMART service");
            Console.WriteLine("     /status             Gets the current service execution status");
            Console.WriteLine(String.Empty);
        }
    }
}
