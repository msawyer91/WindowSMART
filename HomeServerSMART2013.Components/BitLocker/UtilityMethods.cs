using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.BitLocker
{
    public sealed class UtilityMethods
    {
        public static string BITLOCKER_METADATA_VERSION = "L1pSmackinFing3r";

        public static bool IsDriveBenderInstalled()
        {
            try
            {
                System.ServiceProcess.ServiceController driveBender = new System.ServiceProcess.ServiceController(Properties.Resources.DiskPoolProviderDriveBender);
                if (driveBender == null || String.IsNullOrEmpty(driveBender.DisplayName))
                {
                    driveBender.Dispose();
                    return false;
                }

                driveBender.Refresh();
                driveBender.Dispose();
                return true;
            }
            catch
            {
                
            }

            try
            {
                System.ServiceProcess.ServiceController driveBender = new System.ServiceProcess.ServiceController(Properties.Resources.DiskPoolProviderDrivePool);
                if (driveBender == null || String.IsNullOrEmpty(driveBender.DisplayName))
                {
                    driveBender.Dispose();
                    return false;
                }

                driveBender.Refresh();
                driveBender.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
