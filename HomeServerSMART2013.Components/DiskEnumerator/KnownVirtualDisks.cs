using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    /// <summary>
    /// Provides a means of managing a list of known virtual disks. Virtual disks provide no SMART details so there's
    /// no need to consider them.
    /// </summary>
    public static class KnownVirtualDisks
    {
        // KVD_DISK_ID is used by the licensing system. This value is placed here to help prevent cracking if someone were reverse
        // engineer the software.
        public static string KVD_DISK_ID = "Joshua04Finger28";

        /// <summary>
        /// Returns a list of known virtual disks.
        /// </summary>
        /// <returns>List of known virtual disks.</returns>
        public static List<String> ListOfKnownVirtualDisks()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.KnownVirtualDisks.ListOfKnownVirtualDisks");
            List<String> listOfDisks = new List<String>();
            listOfDisks.Add("storlib virtual");
            listOfDisks.Add("storlibvirtual");
            listOfDisks.Add("msft virtual disk");
            listOfDisks.Add("virtualstorage");
            listOfDisks.Add("vbox");
            listOfDisks.Add("covecube");
            listOfDisks.Add("dataram");
            listOfDisks.Add("ramdisk");
            listOfDisks.Add("storage space");
            listOfDisks.Add("virtual hd");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.KnownVirtualDisks.IsDiskOnVirtualDiskList");
            return listOfDisks;
        }

        /// <summary>
        /// Returns whether or not the specified disk is on the list of known virtual disks.
        /// </summary>
        /// <param name="model">String containing the disk model.</param>
        /// <returns>true if model is on the list of known disk; false otherwise.</returns>
        public static bool IsDiskOnVirtualDiskList(String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.KnownVirtualDisks.IsDiskOnVirtualDiskList");
            SiAuto.Main.LogString("model", model);
            foreach (String disk in KnownVirtualDisks.ListOfKnownVirtualDisks())
            {
                if (model.ToLower().Contains(disk.ToLower()))
                {
                    // It's a virtual disk we're ignoring, so skip it (continue).
                    SiAuto.Main.LogMessage("[Virtual Disk Check] Pattern match on " + disk.ToLower() + "; returning true.");
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.KnownVirtualDisks.IsDiskOnVirtualDiskList");
                    return true;
                }
            }
            SiAuto.Main.LogMessage("[Virtual Disk Check] Not on virtual disk list; returning false.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.KnownVirtualDisks.IsDiskOnVirtualDiskList");
            return false;
        }
    }
}
