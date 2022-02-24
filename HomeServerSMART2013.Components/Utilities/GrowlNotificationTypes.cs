using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Growl.Connector;
using Growl.CoreLibrary;
using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Utilities
{
    public sealed class GrowlNotificationTypes
    {
        private NotificationType ntGeneral;
        private NotificationType ntWarning;
        private NotificationType ntCritical;
        private NotificationType ntCleared;
        private NotificationType ntHyperfatal;
        private NotificationType ntAppWarning;
        private String applicationPath;
        private BinaryData applicationIcon;
        private Application application;

        public GrowlNotificationTypes(bool isWindowsServerSolutions)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Utilities.GrowlNotificationTypes");
            applicationPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            try
            {
                SiAuto.Main.LogMessage("Creating NotificationType object for general alerts.");
                byte[] iconData = System.IO.File.ReadAllBytes(applicationPath + "\\" + Properties.Resources.IconAlertGeneral);
                Growl.CoreLibrary.Resource res = new Growl.CoreLibrary.BinaryData(iconData);
                ntGeneral = new NotificationType("NT_GENERAL", Properties.Resources.GrowlNotificateGeneral, res, true);
                SiAuto.Main.LogMessage("Created successfully.");
            }
            catch(Exception ex)
            {
                SiAuto.Main.LogError("Failed to create the NotificationType object: " + ex.Message);
                SiAuto.Main.LogException(ex);
                ntGeneral = new NotificationType("NT_GENERAL", Properties.Resources.GrowlNotificateGeneral);
            }

            try
            {
                SiAuto.Main.LogMessage("Creating NotificationType object for warning alerts.");
                byte[] iconData = System.IO.File.ReadAllBytes(applicationPath + "\\" + Properties.Resources.IconAlertWarning);
                Growl.CoreLibrary.Resource res = new Growl.CoreLibrary.BinaryData(iconData);
                ntWarning = new NotificationType("NT_WARNING", Properties.Resources.GrowlNotificateWarning, res, true);
                SiAuto.Main.LogMessage("Created successfully.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to create the NotificationType object: " + ex.Message);
                SiAuto.Main.LogException(ex);
                ntWarning = new NotificationType("NT_WARNING", Properties.Resources.GrowlNotificateWarning);
            }

            try
            {
                SiAuto.Main.LogMessage("Creating NotificationType object for critical alerts.");
                byte[] iconData = System.IO.File.ReadAllBytes(applicationPath + "\\" + Properties.Resources.IconAlertCritical);
                Growl.CoreLibrary.Resource res = new Growl.CoreLibrary.BinaryData(iconData);
                ntCritical = new NotificationType("NT_CRITICAL", Properties.Resources.GrowlNotificateCritical, res, true);
                SiAuto.Main.LogMessage("Created successfully.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to create the NotificationType object: " + ex.Message);
                SiAuto.Main.LogException(ex);
                ntCritical = new NotificationType("NT_CRITICAL", Properties.Resources.GrowlNotificateCritical);
            }

            try
            {
                SiAuto.Main.LogMessage("Creating NotificationType object for cleared alerts.");
                byte[] iconData = System.IO.File.ReadAllBytes(applicationPath + "\\" + Properties.Resources.IconAlertCleared);
                Growl.CoreLibrary.Resource res = new Growl.CoreLibrary.BinaryData(iconData);
                ntCleared = new NotificationType("NT_CLEARED", Properties.Resources.GrowlNotificateCleared, res, true);
                SiAuto.Main.LogMessage("Created successfully.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to create the NotificationType object: " + ex.Message);
                SiAuto.Main.LogException(ex);
                ntCleared = new NotificationType("NT_CLEARED", Properties.Resources.GrowlNotificateCleared);
            }

            try
            {
                SiAuto.Main.LogMessage("Creating NotificationType object for hyperfatal alerts.");
                byte[] iconData = System.IO.File.ReadAllBytes(applicationPath + "\\" + Properties.Resources.IconAlertHyperfatal);
                Growl.CoreLibrary.Resource res = new Growl.CoreLibrary.BinaryData(iconData);
                ntHyperfatal = new NotificationType("NT_HYPERFATAL", Properties.Resources.GrowlNotificateHyperfatal, res, true);
                SiAuto.Main.LogMessage("Created successfully.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to create the NotificationType object: " + ex.Message);
                SiAuto.Main.LogException(ex);
                ntHyperfatal = new NotificationType("NT_HYPERFATAL", Properties.Resources.GrowlNotificateHyperfatal);
            }

            try
            {
                SiAuto.Main.LogMessage("Creating NotificationType object for application warning alerts.");
                byte[] iconData = System.IO.File.ReadAllBytes(applicationPath + "\\" + Properties.Resources.IconAlertAppWarning);
                Growl.CoreLibrary.Resource res = new Growl.CoreLibrary.BinaryData(iconData);
                ntAppWarning = new NotificationType("NT_APPWARNING", Properties.Resources.GrowlNotificateAppWarning, res, true);
                SiAuto.Main.LogMessage("Created successfully.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to create the NotificationType object: " + ex.Message);
                SiAuto.Main.LogException(ex);
                ntAppWarning = new NotificationType("NT_APPWARNING", Properties.Resources.GrowlNotificateAppWarning);
            }

            try
            {
                SiAuto.Main.LogMessage("Creating application icon.");
                byte[] iconData = System.IO.File.ReadAllBytes(applicationPath + "\\" + Properties.Resources.IconAlertGeneral);
                applicationIcon = new BinaryData(iconData);
                SiAuto.Main.LogMessage("Created successfully.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to create the application icon: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }

            application = new Application(isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart);
            if (applicationIcon != null)
            {
                application.Icon = applicationIcon;
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Utilities.GrowlNotificationTypes");
        }

        public Application GrowlApplication
        {
            get
            {
                return application;
            }
        }

        public BinaryData ApplicationIcon
        {
            get
            {
                return applicationIcon;
            }
        }

        public NotificationType General
        {
            get
            {
                return ntGeneral;
            }
        }

        public NotificationType Warning
        {
            get
            {
                return ntWarning;
            }
        }

        public NotificationType Critical
        {
            get
            {
                return ntCritical;
            }
        }

        public NotificationType Cleared
        {
            get
            {
                return ntCleared;
            }
        }

        public NotificationType Hyperfatal
        {
            get
            {
                return ntHyperfatal;
            }
        }

        public NotificationType AppWarning
        {
            get
            {
                return ntAppWarning;
            }
        }
    }
}
