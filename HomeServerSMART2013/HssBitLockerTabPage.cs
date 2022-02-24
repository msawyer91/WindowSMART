using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.WindowsServerSolutions.Administration.ObjectModel;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI
{
    public class HssBitLockerTabPage : ControlRendererPage
    {
        public HssBitLockerTabPage()
            : base(new Guid("bb2f3b15-a0ad-47f1-b941-d490fdc130bb"), // Put your fixed, static guid here
                      "BitLocker Drive Encryption",
                      "View and edit BitLocker Drive Encryption settings on your drives.") { }

        protected override ControlRendererPageContent CreateContent()
        {
            SiAuto.Main.EnterMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.CreateContent");
            if (IsBitLockerInstalledOnServer())
            {
                SiAuto.Main.LogMessage("BitLocker appears to be installed on the Server; will configure a new BitLockerControl.");
                SiAuto.Main.LeaveMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.CreateContent");
                return ControlRendererPageContent.Create(new BitLockerControl());
            }
            else
            {
                SiAuto.Main.LogMessage("BitLocker doesn't appear to be installed on the Server; will configure a new NoBitLockerControl.");
                SiAuto.Main.LeaveMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.CreateContent");
                return ControlRendererPageContent.Create(new NoBitLockerControl());
            }
        }

        /// <summary>
        /// Check for the presence of several BitLocker DLLs and EXEs. If at least 7 of the 9 are available, we'll consider
        /// BitLocker to be installed.
        /// </summary>
        /// <returns>true if BitLocker is detected; false otherwise.</returns>
        protected bool IsBitLockerInstalledOnServer()
        {
            SiAuto.Main.EnterMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.IsBitLockerInstalledOnServer");
            int requiredItemsDetected = 0;
            String systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);

            if (System.IO.File.Exists(systemPath + "\\fveapi.dll"))
            {
                requiredItemsDetected++;
            }
            if (System.IO.File.Exists(systemPath + "\\fveapibase.dll"))
            {
                requiredItemsDetected++;
            }
            if (System.IO.File.Exists(systemPath + "\\fvecerts.dll"))
            {
                requiredItemsDetected++;
            }
            if (System.IO.File.Exists(systemPath + "\\fvecpl.dll"))
            {
                requiredItemsDetected++;
            }
            if (System.IO.File.Exists(systemPath + "\\fvenotify.exe"))
            {
                requiredItemsDetected++;
            }
            if (System.IO.File.Exists(systemPath + "\\fveprompt.exe"))
            {
                requiredItemsDetected++;
            }
            if (System.IO.File.Exists(systemPath + "\\fverecover.dll"))
            {
                requiredItemsDetected++;
            }
            if (System.IO.File.Exists(systemPath + "\\fveui.dll"))
            {
                requiredItemsDetected++;
            }
            if (System.IO.File.Exists(systemPath + "\\fvewiz.dll"))
            {
                requiredItemsDetected++;
            }

            SiAuto.Main.LogInt("requiredItemsDetected", requiredItemsDetected);

            if (requiredItemsDetected >= 7)
            {
                SiAuto.Main.LogMessage("Required items >= 7; BitLocker is installed so returning true.");
                SiAuto.Main.LeaveMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.IsBitLockerInstalledOnServer");
                return true;
            }
            else
            {
                SiAuto.Main.LogMessage("Required items < 7; BitLocker is not installed so returning false.");
                SiAuto.Main.LeaveMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.IsBitLockerInstalledOnServer");
                return false;
            }
        }
    }
}
