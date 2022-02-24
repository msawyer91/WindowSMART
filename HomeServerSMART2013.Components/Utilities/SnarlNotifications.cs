using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gurock.SmartInspect;
using Snarl.V42;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Utilities
{
    public sealed class SnarlNotifications
    {
        private SnarlInterface snarl;
        private String applicationPath;

        private String iconGeneral;
        private String iconCleared;
        private String iconHyperfatal;
        private String iconCritical;
        private String iconWarning;
        private String iconAppWarning;

        private bool isWindowsServerSolutions;

        public SnarlNotifications(bool isWss)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.SnarlNotifications");

            isWindowsServerSolutions = isWss;

            SiAuto.Main.LogMessage("Initializing the Snarl interface.");
            snarl = new SnarlInterface();

            SiAuto.Main.LogMessage("Getting the icon path.");
            applicationPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            SiAuto.Main.LogString("applicationPath", applicationPath);

            SiAuto.Main.LogMessage("Getting the icon filenames.");
            iconGeneral = applicationPath + "\\" + Properties.Resources.IconAlertGeneral;
            iconCleared = applicationPath + "\\" + Properties.Resources.IconAlertCleared;
            iconHyperfatal = applicationPath + "\\" + Properties.Resources.IconAlertHyperfatal;
            iconCritical = applicationPath + "\\" + Properties.Resources.IconAlertCritical;
            iconWarning = applicationPath + "\\" + Properties.Resources.IconAlertWarning;
            iconAppWarning = applicationPath + "\\" + Properties.Resources.IconAlertAppWarning;

            SiAuto.Main.LogMessage("Checking to see if Snarl is running. (This does not preclude the use of Snarl because the user could close and reopen it.)");
            if (SnarlInterface.GetSnarlWindow() == IntPtr.Zero)
            {
                SiAuto.Main.LogWarning("Warning, Snarl was not detected.");
            }

            SiAuto.Main.LogMessage("Build the Snarl alert classes.");
            snarl.AddClass("NT_GENERAL", Properties.Resources.SnarlNotificateGeneral, String.Empty, String.Empty, iconGeneral, String.Empty, 15, String.Empty, true);
            snarl.AddClass("NT_CLEARED", Properties.Resources.SnarlNotificateCleared, String.Empty, String.Empty, iconCleared, String.Empty, 15, String.Empty, true);
            snarl.AddClass("NT_HYPERFATAL", Properties.Resources.SnarlNotificateHyperfatal, String.Empty, String.Empty, iconHyperfatal, String.Empty, 30, String.Empty, true);
            snarl.AddClass("NT_CRITICAL", Properties.Resources.SnarlNotificateCritical, String.Empty, String.Empty, iconCritical, String.Empty, 30, String.Empty, true);
            snarl.AddClass("NT_WARNING", Properties.Resources.SnarlNotificateWarning, String.Empty, String.Empty, iconWarning, String.Empty, 30, String.Empty, true);
            snarl.AddClass("NT_APPWARNING", Properties.Resources.SnarlNotificateAppWarning, String.Empty, String.Empty, iconAppWarning, String.Empty, 30, String.Empty, true);

            SiAuto.Main.LogMessage("Snarl initialization is complete. Snarl is now ready to register.");

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.SnarlNotifications");
        }

        public int Register()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.SnarlNotifications.Register");
            SiAuto.Main.LogMessage("Registering with Snarl. This must be performed so that alerts can be sent to Snarl.");
            int result = snarl.Register("application/windowsmart2013", isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart,
                iconGeneral);
            SiAuto.Main.LogInt("result (snarl status code)", result);
            SiAuto.Main.LogMessage("Snarl registration is complete.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.SnarlNotifications.Register");
            return result;
        }

        public void Unregister()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.SnarlNotifications.Unregister");
            SiAuto.Main.LogMessage("Unregistering with Snarl. This is an essential cleanup task.");
            int result = snarl.Unregister();
            SiAuto.Main.LogInt("result (snarl status code)", result);
            SiAuto.Main.LogMessage("Snarl unregistration is complete.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.SnarlNotifications.Unregister");
        }

        public void Close()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.SnarlNotifications.Close");
            try
            {
                snarl.Unregister();
            }
            catch
            {
            }

            try
            {
                snarl.ClearClasses();
                snarl = null;
                System.GC.Collect();
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.SnarlNotifications.Close");
        }

        public void Notify(String title, String message, SnarlMessageClass msgClass)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.SnarlNotifications.Notify");
            int result = 65535;
            SiAuto.Main.LogString("title", title);
            SiAuto.Main.LogString("message", message);
            SiAuto.Main.LogObjectValue("msgClass", msgClass);
            switch (msgClass)
            {
                case SnarlMessageClass.General:
                    {
                        result = snarl.Notify("NT_GENERAL", title, message);
                        break;
                    }
                case SnarlMessageClass.Cleared:
                    {
                        result = snarl.Notify("NT_CLEARED", title, message);
                        break;
                    }
                case SnarlMessageClass.Hyperfatal:
                    {
                        result = snarl.Notify("NT_HYPERFATAL", title, message);
                        break;
                    }
                case SnarlMessageClass.Critical:
                    {
                        result = snarl.Notify("NT_CRITICAL", title, message);
                        break;
                    }
                case SnarlMessageClass.Warning:
                    {
                        result = snarl.Notify("NT_WARNING", title, message);
                        break;
                    }
                case SnarlMessageClass.AppWarning:
                default:
                    {
                        result = snarl.Notify("NT_APPWARNING", title, message);
                        break;
                    }
            }
            SiAuto.Main.LogInt("result", result);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.SnarlNotifications.Notify");
        }
    }

    public enum SnarlMessageClass : int
    {
        General = 0,
        Cleared = 1,
        Hyperfatal = 2,
        Critical = 3,
        Warning = 4,
        AppWarning = 5
    }
}
