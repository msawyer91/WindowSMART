using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public static class UtilityMethods
    {
        static UtilityMethods()
        {
        }

        public static bool IsSelectedDiskUsb(String driveLetter)
        {
            try
            {
                // Browse all WMI physical disks.
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                  Properties.Resources.WmiQueryStringNonWin8).Get())
                {
                    // Associate physical disks with partitions
                    foreach (ManagementObject partition in new ManagementObjectSearcher(
                      "ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + drive["DeviceID"] +
                        "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition").Get())
                    {
                        // Associate partitions with logical disks (drive letter volumes)
                        foreach (ManagementObject disk in new ManagementObjectSearcher(
                          "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" +
                           partition["DeviceID"] + "'} WHERE AssocClass = Win32_LogicalDiskToPartition").Get())
                        {
                            if (String.Compare(disk["Name"].ToString(), driveLetter, true)
                                == 0)
                            {
                                if (String.Compare(drive["InterfaceType"].ToString(), "USB", true)
                                    == 0)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot determine disk type: " + driveLetter + " " + ex.Message,
                    "Error Checking Interface", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return false;
        }

        public static bool AreDisksSame(String volume, String savePath)
        {
            if (String.Compare(volume, savePath, true) == 0)
            {
                MessageBox.Show("You cannot save the recovery data to volume " + volume + " because " +
                    "volume " + volume + " is the one targeted for encryption. Please select another " +
                    "location.", "Cannot Save to Target Disk", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return true;
            }
            return false;
        }

        public static bool IsComposeTextFileError(String path)
        {
            try
            {
                System.IO.FileStream fs = System.IO.File.Create(path + "\\Tarynblmtemp65535.txt");
                fs.Flush();
                fs.Close();
                System.IO.File.Delete(path + "\\Tarynblmtemp65535.txt");
                return false;
            }
            catch
            {
                MessageBox.Show("Path " + path + " cannot be used. Home Server SMART attempted to write a temporary " +
                    "file to this location (prior to External Key Protector generation) to validate your ability to write " +
                    "a file to this location. That attempt failed. Please choose another location.", "Path Not Available",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return true;
            }
        }

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
