using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Gurock.SmartInspect;
//using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class DiskInfo
    {
        String model;
        String serialNumber;
        String firmwareRev;
        String description;
        String interfaceType;
        String mediaType;
        String name;
        String partitionCount;
        String status;
        String failurePredicted;
        String bytesPerSector;
        String cylinders;
        String heads;
        String totalSectors;
        String tracks;
        String tracksPerCylinder;
        String totalBytes;
        String advertisedCapacity;
        String realCapacity;
        String physicalSectorSize;
        String logicalSectorSize;

        bool isSsd = false;
        bool isTrimSupported = false;
        String ssdControllerManufacturer = String.Empty;

        public DiskInfo()
        {
            model = String.Empty;
            serialNumber = String.Empty;
            firmwareRev = String.Empty;
            description = String.Empty;
            interfaceType = String.Empty;
            mediaType = String.Empty;
            name = String.Empty;
            partitionCount = String.Empty;
            status = String.Empty;
            failurePredicted = String.Empty;
            bytesPerSector = String.Empty;
            cylinders = String.Empty;
            heads = String.Empty;
            totalSectors = String.Empty;
            tracks = String.Empty;
            tracksPerCylinder = String.Empty;
            totalBytes = String.Empty;
            advertisedCapacity = String.Empty;
            realCapacity = String.Empty;
            physicalSectorSize = String.Empty;
            logicalSectorSize = String.Empty;

            isSsd = false;
            isTrimSupported = false;
            ssdControllerManufacturer = String.Empty;
        }

        public bool Populate(String diskToPopulate)
        {
            try
            {
                SiAuto.Main.LogMessage("Acquire Registry objects.");
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryMonitoredDisksKey, false);
                SiAuto.Main.LogMessage("Acquired Registry objects.");

                SiAuto.Main.LogMessage("Searching Registry for desired disk.");
                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, false);
                    if (String.Compare(diskToPopulate, (String)diskKey.GetValue("DevicePath"), true) == 0)
                    {
                        SiAuto.Main.LogMessage("Found disk " + diskKeyName);
                        try
                        {
                            SiAuto.Main.LogMessage("[Stale Disk Check] Checking disk IsActive flag.");
                            bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                            if (!activeFlag)
                            {
                                // Inactive/Stale disk.
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Stale Disk Notificate] Disk is stale; an old disk with the same identifier may have been removed from the Server.");
                                diskKey.Close();
                                continue;
                            }

                            SiAuto.Main.LogMessage("Read disk characteristics.");
                            try
                            {
                                model = (String)diskKey.GetValue("PreferredModel");
                                if (String.IsNullOrEmpty(model))
                                {
                                    model = (String)diskKey.GetValue("Model", "Missing");
                                }
                            }
                            catch
                            {
                                model = (String)diskKey.GetValue("Model", "Missing");
                            }
                            try
                            {
                                serialNumber = (String)diskKey.GetValue("PreferredSerial");
                                if (String.IsNullOrEmpty(serialNumber))
                                {
                                    serialNumber = (String)diskKey.GetValue("SerialNumber", "Missing");
                                }
                            }
                            catch
                            {
                                serialNumber = (String)diskKey.GetValue("SerialNumber", "Missing");
                            }
                            try
                            {
                                firmwareRev = (String)diskKey.GetValue("PreferredFirmware");
                                if (String.IsNullOrEmpty(firmwareRev))
                                {
                                    firmwareRev = (String)diskKey.GetValue("FirmwareRevision", "Missing");
                                }
                            }
                            catch
                            {
                                firmwareRev = (String)diskKey.GetValue("FirmwareRevision", "Missing");
                            }
                            description = (String)diskKey.GetValue("Description", "Missing");
                            interfaceType = (String)diskKey.GetValue("Interface", "Missing");
                            mediaType = (String)diskKey.GetValue("MediaType", "Missing");
                            name = (String)diskKey.GetValue("Name", "Missing");
                            partitionCount = (String)diskKey.GetValue("PartitionCount", "Missing");
                            status = (String)diskKey.GetValue("Status", "Missing");
                            failurePredicted = (String)diskKey.GetValue("FailurePredicted", "Missing");
                            bytesPerSector = (String)diskKey.GetValue("BytesPerSector", "Missing");
                            cylinders = (String)diskKey.GetValue("Cylinders", "Missing");
                            heads = (String)diskKey.GetValue("Heads", "Missing");
                            totalSectors = (String)diskKey.GetValue("TotalSectors", "Missing");
                            tracks = (String)diskKey.GetValue("Tracks", "Missing");
                            tracksPerCylinder = (String)diskKey.GetValue("TracksPerCylinder", "Missing");
                            totalBytes = (String)diskKey.GetValue("TotalBytes", "Missing");
                            advertisedCapacity = (String)diskKey.GetValue("AdvertisedCapacity", "Missing");
                            realCapacity = (String)diskKey.GetValue("RealCapacity", "Missing");
                            physicalSectorSize = (String)diskKey.GetValue("PhysicalSectorSize", "Missing");
                            logicalSectorSize = (String)diskKey.GetValue("LogicalSectorSize", "Missing");
                            isSsd = bool.Parse((String)diskKey.GetValue("IsSsd", "False"));
                            isTrimSupported = bool.Parse((String)diskKey.GetValue("IsTrimSupported", "False"));
                            if (isSsd)
                            {
                                ssdControllerManufacturer = (String)diskKey.GetValue("SsdController", "None");
                            }
                            else
                            {
                                ssdControllerManufacturer = (String)diskKey.GetValue("NominalMediaRotationRate", "0");
                            }
                            SiAuto.Main.LogMessage("Done reading disk characteristics.");
                            SiAuto.Main.LogBool("isSsd", isSsd);
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogWarning(ex.Message);
                            SiAuto.Main.LogException(ex);
                            diskKey.Close();
                            continue;
                        }
                    }
                    diskKey.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to read disk Registry data. " + ex.Message);
                SiAuto.Main.LogException(ex);
                MessageBox.Show("Cannot read saved data for the selected disk from the Registry. " + ex.Message +
                    "\n\nThe disks may not have finished being polled. You can force " +
                    "a manual polling by clicking Refresh Disks.", "Failed to Read from Registry", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return false;
            }
        }

        public String Model
        {
            get
            {
                return model;
            }
        }

        public String SerialNumber
        {
            get
            {
                return serialNumber;
            }
        }

        public String FirmwareRev
        {
            get
            {
                return firmwareRev;
            }
        }

        public String Description
        {
            get
            {
                return description;
            }
        }

        public String InterfaceType
        {
            get
            {
                return interfaceType;
            }
        }

        public String MediaType
        {
            get
            {
                return mediaType;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }

        public String PartitionCount
        {
            get
            {
                return partitionCount;
            }
        }

        public String Status
        {
            get
            {
                return status;
            }
        }

        public String FailurePredicted
        {
            get
            {
                return failurePredicted;
            }
        }

        public String BytesPerSector
        {
            get
            {
                return bytesPerSector;
            }
        }

        public String Cylinders
        {
            get
            {
                return cylinders;
            }
        }

        public String Heads
        {
            get
            {
                return heads;
            }
        }

        public String TotalSectors
        {
            get
            {
                return totalSectors;
            }
        }

        public String Tracks
        {
            get
            {
                return tracks;
            }
        }

        public String TracksPerCylinder
        {
            get
            {
                return tracksPerCylinder;
            }
        }

        public String TotalBytes
        {
            get
            {
                return totalBytes;
            }
        }

        public String AdvertisedCapacity
        {
            get
            {
                return advertisedCapacity;
            }
        }

        public String RealCapacity
        {
            get
            {
                return realCapacity;
            }
        }

        public bool IsSsd
        {
            get
            {
                return isSsd;
            }
        }

        public bool IsTrimSupported
        {
            get
            {
                return isTrimSupported;
            }
        }

        public String SsdControllerManufacturer
        {
            get
            {
                return ssdControllerManufacturer;
            }
        }

        public String PhysicalSectorSize
        {
            get
            {
                return physicalSectorSize;
            }
        }

        public String LogicalSectorSize
        {
            get
            {
                return logicalSectorSize;
            }
        }
    }
}
