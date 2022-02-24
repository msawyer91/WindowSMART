using System;
using System.Collections.Generic;
using System.Data;
using System.Management;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public static class DiskEnumerator
    {
        /// <summary>
        /// Enumerates the disks and refreshes the SMART information of all detected disks.
        /// <param name="isManualRefresh">Set to true to specify this is a manual (invoked from the UI) refresh.</param>
        /// <param name="fallbackToWmi">Set to true to allow fallback to WMI SMART lookup if P/Invoke fails; false to prevent fallback.</param>
        /// <param name="advancedSii">Set to true to enable advanced SiI mode of zero thresholds if primary lookup fails; false to use normal mode.</param>
        /// <param name="ignoreVirtualDisks">Set to true to ignore known virtual disks - prevents them from being enumerated or registered; false allows them.</param>
        /// </summary>
        public static void RefreshDiskInfo(bool isManualRefresh, bool fallbackToWmi, bool advancedSii, bool ignoreVirtualDisks, out String phantomDiskList)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.DiskEnumerator.RefreshDiskInfo");
            SiAuto.Main.LogBool("isManualRefresh", isManualRefresh);
            SiAuto.Main.LogBool("fallbackToWmi", fallbackToWmi);
            SiAuto.Main.LogBool("advancedSii", advancedSii);
            SiAuto.Main.LogBool("ignoreVirtualDisks", ignoreVirtualDisks);
            DateTime pollStart = DateTime.Now;
            SiAuto.Main.LogDateTime("pollStart", pollStart);
            String startMessage = "Starting a SMART disk polling operation in " + (isManualRefresh ? "Manual" : "Automatic") + " mode.";
            SiAuto.Main.LogColored(System.Drawing.Color.Gray, startMessage);
            WindowsEventLogger.LogInformation(startMessage, 53860);
            phantomDiskList = String.Empty;

            SiAuto.Main.LogMessage("Acquire Registry objects.");
            Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey dojoNorthSubKey;

            dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

            Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);
            SiAuto.Main.LogMessage("Acquired Registry objects.");

            // We set the active flag to false for all disks. If the disk is alive and connected,
            // the flag will be reset to true. This helps us detect disks that have been removed
            // while the service is running.
            SiAuto.Main.LogMessage("Reset active state for all disks to false; active disks will be detected during refresh.");
            foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
            {
                Microsoft.Win32.RegistryKey diskKey;
                diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);
                diskKey.SetValue("IsActive", false);
            }

            SiAuto.Main.LogMessage("Create SmartData data table.");
            SmartData smartDataTable = new SmartData();

            // Keep track of any phantom disks. Phantom disks only appear in Windows 8 and Server 2012 Storage Spaces.
            bool arePhantomsDetected = false;
            int phantomCount = 0;
            List<String> listOfPhantoms = new List<String>();

            if (Utilities.Utility.IsSystemWindows8())
            {
                SiAuto.Main.LogWarning("Enumerating Microsoft Storage Spaces devices.");
                // Iterate disks for model, PNP, etc.
                SiAuto.Main.LogMessage("Iterate WMI MSFT_PhysicalDisk objects for PNP and device IDs.");
                foreach (ManagementObject drive in new ManagementObjectSearcher(Properties.Resources.WmiQueryScope, Properties.Resources.WmiQueryStringWin8).Get())
                {
                    // NULL check for Storage Spaces phantom disks.
                    if (drive["DeviceID"] == null)
                    {
                        String friendlyName = String.Empty;
                        arePhantomsDetected = true;
                        try
                        {
                            if (drive["FriendlyName"] == null)
                            {
                                SiAuto.Main.LogWarning("[Storage Spaces] Phantom Disk detected. Name unknown.");
                                phantomCount++;
                                listOfPhantoms.Add("Unknown Disk");
                            }
                            else
                            {
                                friendlyName = drive["FriendlyName"].ToString();
                                SiAuto.Main.LogWarning("[Storage Spaces] Phantom Disk detected: " + friendlyName);
                                phantomCount++;
                                listOfPhantoms.Add(friendlyName);
                            }
                        }
                        catch
                        {
                        }
                        continue;
                    }
                    String diskIDNumber = drive["DeviceID"].ToString();
                    String deviceId = "\\\\.\\PHYSICALDRIVE" + diskIDNumber;
                    int ssDiskNumber = -1;

                    // Get the physical drive number
                    try
                    {
                        SiAuto.Main.LogMessage("Attempting to parse physical drive number from " + deviceId);
                        ssDiskNumber = Utilities.Utility.GetDriveIdFromPath(deviceId);
                        SiAuto.Main.LogInt("Parse successful; ssDiskNumber", ssDiskNumber);
                    }
                    catch (Exception ex)
                    {
                        SiAuto.Main.LogError(ex.Message);
                        SiAuto.Main.LogException(ex);
                        SiAuto.Main.LogWarning("Could not parse " + deviceId + " to an integer; skipping this disk.");
                        ssDiskNumber = -1;
                    }

                    // If the disk number is -1, skip and move to next one.
                    if (ssDiskNumber == -1)
                    {
                        continue;
                    }

                    // Collect serial number and model.
                    String diskManufacturer = String.Empty;
                    String diskModel = String.Empty;
                    String diskSerial = String.Empty;

                    SiAuto.Main.LogMessage("Collecting serial and model.");
                    if (drive["SerialNumber"] != null)
                    {
                        diskSerial = drive["SerialNumber"].ToString();
                    }
                    if (drive["Manufacturer"] != null)
                    {
                        diskManufacturer = drive["Manufacturer"].ToString();
                    }
                    if (drive["Model"] != null)
                    {
                        diskModel = drive["Model"].ToString();
                    }

                    // Build the PNP device name for storing in Registry.
                    SiAuto.Main.LogMessage("Building PNP name.");
                    String pnpDeviceId = Utilities.Utility.BuildWindows8PnpDeviceName(diskModel, diskSerial, (UInt16)drive["BusType"], ssDiskNumber);
                    diskModel = diskManufacturer + diskModel;
                    SiAuto.Main.LogMessage("Building PNP no-slash name.");
                    SiAuto.Main.LogString("pnpDeviceId", pnpDeviceId);
                    String pnpDeviceIdNoBackslash = pnpDeviceId.Replace('\\', '~').Replace(' ', '~');
                    SiAuto.Main.LogString("pnpDeviceIdNoBackslash", pnpDeviceIdNoBackslash);

                    SiAuto.Main.LogColored(System.Drawing.Color.SkyBlue, "[Disk Detected] PNP=" + pnpDeviceIdNoBackslash + ", Device ID=" + deviceId);

                    if (ignoreVirtualDisks)
                    {
                        SiAuto.Main.LogMessage("[Virtual Disk Check] Ignore virtual disks enforced.");

                        if (KnownVirtualDisks.IsDiskOnVirtualDiskList(diskModel))
                        {
                            // It's a virtual disk we're ignoring, so skip it (continue).
                            SiAuto.Main.LogColored(System.Drawing.Color.Goldenrod, "[Virtual Disk Discard] Disk is on known list of VDs; expurgating.");
                            continue;
                        }
                    }

                    // Correlate the data in the SMART data table.
                    smartDataTable.AddNewRow(pnpDeviceId, deviceId);
                    SiAuto.Main.LogMessage("Add disk to data table: PNP=" + pnpDeviceIdNoBackslash + ", Device ID=" + deviceId);

                    // Write it all to the Registry.
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(pnpDeviceIdNoBackslash, true);
                    SiAuto.Main.LogMessage("Open disk subkey " + pnpDeviceIdNoBackslash);
                    if (diskKey == null)
                    {
                        SiAuto.Main.LogMessage("Subkey does not exist; create new.");
                        diskKey = monitoredDisksKey.CreateSubKey(pnpDeviceIdNoBackslash);
                    }
                    SiAuto.Main.LogMessage("Set Registry - PNP, DevicePath and IsActive.");
                    diskKey.SetValue("PNPDeviceID", pnpDeviceId);
                    diskKey.SetValue("DevicePath", deviceId);
                    diskKey.SetValue("IsActive", true);
                    SiAuto.Main.LogMessage("Added PNP, DevicePath and IsActive to Registry. Changes committed.");
                }
            }
            else
            {
                // Iterate disks for model, PNP, etc.
                SiAuto.Main.LogMessage("Iterate WMI Win32_DiskDrive objects for PNP and device IDs.");

                foreach (ManagementObject drive in new ManagementObjectSearcher(
                  Properties.Resources.WmiQueryStringNonWin8).Get())
                {
                    String pnpDeviceId = drive["PNPDeviceID"].ToString();
                    String pnpDeviceIdNoBackslash = pnpDeviceId.Replace('\\', '~');
                    String deviceId = drive["DeviceID"].ToString();

                    SiAuto.Main.LogColored(System.Drawing.Color.SkyBlue, "[Disk Detected] PNP=" + pnpDeviceIdNoBackslash + ", Device ID=" + deviceId);

                    if (ignoreVirtualDisks)
                    {
                        SiAuto.Main.LogMessage("[Virtual Disk Check] Ignore virtual disks enforced.");
                        String model = String.Empty;
                        if (drive["Model"] != null)
                        {
                            model = drive["Model"].ToString();
                        }
                        else if (drive["Model"] == null && drive["PNPDeviceID"] != null)
                        {
                            model = drive["PNPDeviceID"].ToString();
                        }
                        else
                        {
                            model = "Undefined";
                        }

                        if (KnownVirtualDisks.IsDiskOnVirtualDiskList(model))
                        {
                            // It's a virtual disk we're ignoring, so skip it (continue).
                            SiAuto.Main.LogColored(System.Drawing.Color.Goldenrod, "[Virtual Disk Discard] Disk is on known list of VDs; expurgating.");
                            continue;
                        }
                    }

                    // Correlate the data in the SMART data table.
                    smartDataTable.AddNewRow(pnpDeviceId, deviceId);
                    SiAuto.Main.LogMessage("Add disk to data table: PNP=" + pnpDeviceIdNoBackslash + ", Device ID=" + deviceId);

                    // Write it all to the Registry.
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(pnpDeviceIdNoBackslash, true);
                    SiAuto.Main.LogMessage("Open disk subkey " + pnpDeviceIdNoBackslash);
                    if (diskKey == null)
                    {
                        SiAuto.Main.LogMessage("Subkey does not exist; create new.");
                        diskKey = monitoredDisksKey.CreateSubKey(pnpDeviceIdNoBackslash);
                    }
                    SiAuto.Main.LogMessage("Set Registry - PNP, DevicePath and IsActive.");
                    diskKey.SetValue("PNPDeviceID", pnpDeviceId);
                    diskKey.SetValue("DevicePath", deviceId);
                    diskKey.SetValue("IsActive", true);
                    SiAuto.Main.LogMessage("Added PNP, DevicePath and IsActive to Registry. Changes committed.");
                }
            }

            // Alert or clear on phantom disks.
            if (arePhantomsDetected)
            {
                phantomDiskList = String.Empty;
                foreach (String disk in listOfPhantoms)
                {
                    phantomDiskList += disk + ", ";
                }
                phantomDiskList = phantomDiskList.Substring(0, phantomDiskList.Length - 2);
            }
            else
            {
                phantomDiskList = String.Empty;
            }
            
            // Clean up our Registry activity.
            monitoredDisksKey.Close();
            dojoNorthSubKey.Close();
            SiAuto.Main.LogMessage("Closed Registry objects.");

            // Read the SMART data and rotation/SSD status from the non-removable.
            SiAuto.Main.LogMessage("Attempt retreival of SMART data for internal (PATA/SATA/eSATA) disks.");
            SmartMethods.GetInternalSmartInfo(smartDataTable, fallbackToWmi, advancedSii);
            SiAuto.Main.LogMessage("Attempt retreival of SMART data for USB disks.");
            SmartMethods.GetUsbSmartInfo(smartDataTable);
            SiAuto.Main.LogMessage("Attempt retrieval of disk details.");
            GetDiskDetails(smartDataTable, ignoreVirtualDisks);
            SiAuto.Main.LogMessage("Write SMART data table to Registry.");
            smartDataTable.WriteInitialRegistryData();

            // Log the polling end time.
            DateTime pollEnd = DateTime.Now;
            TimeSpan pollSpan = pollEnd - pollStart;
            String spanTime = pollSpan.Days.ToString() + " day(s), " + pollSpan.Hours.ToString() + " hour(s), " + pollSpan.Minutes.ToString() + " minute(s), " +
                pollSpan.Seconds.ToString() + " second(s), " + pollSpan.Milliseconds.ToString() + " millisecond(s)";
            String pollTimeMessage = "Completed " + (isManualRefresh ? "a manual" : "an automatic") + " disk polling operation.\n\n" +
                    "Start: " + pollStart.ToShortDateString() + " " + pollStart.ToLongTimeString() + "\n" +
                    "Complete: " + pollEnd.ToShortDateString() + " " + pollEnd.ToLongTimeString() + "\n" +
                    "Execution Time: " + spanTime;

            SiAuto.Main.LogDateTime("pollEnd", pollEnd);
            SiAuto.Main.LogColored(System.Drawing.Color.Gray, "Ending a SMART disk polling operation in " + (isManualRefresh ? "Manual" : "Automatic") + " mode.");
            SiAuto.Main.LogObjectValue("pollSpan", pollSpan);
            SiAuto.Main.LogString("pollTimeMessage", pollTimeMessage);

            WindowsEventLogger.LogInformation(pollTimeMessage, 53861);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.RefreshDiskInfo");
        }

        public static void RefreshDiskInfo(bool fallbackToWmi, bool advancedSii, bool ignoreVirtualDisks, out String phantomDiskList)
        {
            // Call RefreshDiskInfo overload with false (not a manual refresh).
            phantomDiskList = String.Empty;
            RefreshDiskInfo(false, fallbackToWmi, advancedSii, ignoreVirtualDisks, out phantomDiskList);
        }

        /// <summary>
        /// Clear any "stale" disk events. If a disk has raised alerts but has been removed from the computer (i.e. a USB disk is unplugged),
        /// its alert state should be cleared.
        /// </summary>
        /// <param name="feverishDisks">The list of FeverishDisk objects.</param>
        public static void ClearStaleDiskEvents(FeverishDisk feverishDisks)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.DiskEnumerator.ClearStaleDiskEvents");
            try
            {
                SiAuto.Main.LogMessage("Acquire Registry objects.");
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);
                SiAuto.Main.LogMessage("Acquired Registry objects.");

                // Look for inactive (stale) disks. These are disks that have been disconnected.
                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    SiAuto.Main.LogString("diskKeyName", diskKeyName);
                    SiAuto.Main.LogMessage("Checking " + diskKeyName + " for staleness.");
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);
                    bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                    bool itemsExpurgated = false;

                    SiAuto.Main.LogBool("activeFlag", activeFlag);
                    if (!activeFlag)
                    {
                        // Grab the list of alert GUIDs associated with the disconnected disk.
                        SiAuto.Main.LogColored(System.Drawing.Color.HotPink, diskKeyName + " is stale. Any active alerts associated with it will be expurgated.");
                        List<String> guids = new List<string>();
                        guids.Add((String)diskKey.GetValue("NotificationGuidBadSectors"));
                        guids.Add((String)diskKey.GetValue("NotificationGuidCrcErrors"));
                        guids.Add((String)diskKey.GetValue("NotificationGuidCriticalHealth"));
                        guids.Add((String)diskKey.GetValue("NotificationGuidEndToEndErrors"));
                        guids.Add((String)diskKey.GetValue("NotificationGuidGeezer"));
                        guids.Add((String)diskKey.GetValue("NotificationGuidPendingSectors"));
                        guids.Add((String)diskKey.GetValue("NotificationGuidReallocationEvents"));
                        guids.Add((String)diskKey.GetValue("NotificationGuidSpinRetries"));
                        guids.Add((String)diskKey.GetValue("NotificationGuidTemperature"));
                        guids.Add((String)diskKey.GetValue("NotificationGuidAirflowTemperature"));
                        guids.Add((String)diskKey.GetValue("NotificationGuidThreshold"));
                        guids.Add((String)diskKey.GetValue("NotificationGuidUncorrectableSectors"));
                        guids.Add((String)diskKey.GetValue("NotificationGuidWarningHealth"));

                        foreach (String guid in guids)
                        {
                            // If an alert is active, clear it.
                            if (feverishDisks.ItemExists(guid))
                            {
                                SiAuto.Main.LogMessage("Active event " + guid + " is assigned to stale disk " + diskKeyName + "; expurgating.");
                                feverishDisks.RemoveItem(guid);
                                itemsExpurgated = true;
                            }
                        }
                    }

                    if (itemsExpurgated)
                    {
                        SiAuto.Main.LogMessage("At least one stale disk event was expurgated from the list of active alerts.");
                    }
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Error clearing stale disks. " + ex.Message);
                SiAuto.Main.LogException(ex);
                WindowsEventLogger.LogError("Failed to clear stale disks: " + ex.Message, 53885, Properties.Resources.EventLogJoshua);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.ClearStaleDiskEvents");
        }

        /// <summary>
        /// Get details about the disk -- model, serial number, etc.
        /// </summary>
        /// <param name="smartDataTable">SMART data table.</param>
        /// <param name="ignoreVirtualDisks">If set to true, ignore known virtual disks (don't enumerate them further).</param>
        private static void GetDiskDetails(SmartData smartDataTable, bool ignoreVirtualDisks)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.DiskEnumerator.GetDiskDetails");

            if (Utilities.Utility.IsSystemWindows8())
            {
                foreach (ManagementObject drive in new ManagementObjectSearcher(Properties.Resources.WmiQueryScope,
                    Properties.Resources.WmiQueryStringWin8).Get())
                {
                    // NULL check for Storage Spaces phantom disks.
                    if (drive["DeviceID"] == null)
                    {
                        String friendlyName = String.Empty;
                        try
                        {
                            if (drive["FriendlyName"] == null)
                            {
                                SiAuto.Main.LogWarning("[Storage Spaces] Phantom Disk detected. Name unknown.");
                            }
                            else
                            {
                                friendlyName = drive["FriendlyName"].ToString();
                                SiAuto.Main.LogWarning("[Storage Spaces] Phantom Disk detected: " + friendlyName);
                            }
                        }
                        catch
                        {
                        }
                        continue;
                    }

                    // We want to collect information such as model, serial, firmware, description, sector sizes, capacity, etc.
                    UInt64 physicalSectorSize = 0;
                    UInt64 logicalSectorSize = 0;
                    UInt64 totalBytes = 0;
                    //bool canPool = false;
                    //String operationalStatus;

                    String path = String.Empty;
                    String serialNumber = String.Empty;
                    String firmwareRevision = String.Empty;
                    String deviceInstanceID = String.Empty;
                    String model = String.Empty;
                    String description = String.Empty;
                    String interfaceType = String.Empty;
                    String status = String.Empty;
                    bool failurePredicted = false;
                    String manufacturer = String.Empty; // we don't save this one; we just check it against ignored disks.

                    SiAuto.Main.LogMessage("GetDiskDetails is enumerating MSFT_PhysicalDisk on a Windows 8 platform disk.");

                    try
                    {
                        // Some information may be NULL or not available. If it's not, we'll put text indicating such, rather than leaving it blank.
                        path = (drive["DeviceID"] == null ? "Invalid ID or Undefined" : "\\\\.\\PHYSICALDRIVE" + drive["DeviceID"].ToString());

                        SiAuto.Main.LogMessage("Examining disk " + path);

                        SiAuto.Main.LogMessage("Collecting serial and model.");
                        if (drive["SerialNumber"] != null)
                        {
                            serialNumber = drive["SerialNumber"].ToString();
                        }
                        if (drive["Model"] != null)
                        {
                            model = drive["Model"].ToString();
                        }

                        try
                        {
                            firmwareRevision = (drive["FirmwareVersion"] == null ? "Invalid or Undefined" : drive["FirmwareVersion"].ToString());
                            if (serialNumber.StartsWith("W -D"))
                            {
                                firmwareRevision = Utilities.Utility.ReverseCharacters(firmwareRevision);
                            }
                        }
                        catch
                        {
                            firmwareRevision = "Not Available";
                        }

                        int ssDiskNumber = -1;

                        // Get the physical drive number
                        try
                        {
                            SiAuto.Main.LogMessage("Attempting to parse physical drive number from " + path);
                            ssDiskNumber = Utilities.Utility.GetDriveIdFromPath(path);
                            SiAuto.Main.LogInt("Parse successful; ssDiskNumber", ssDiskNumber);
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogError(ex.Message);
                            SiAuto.Main.LogException(ex);
                            SiAuto.Main.LogWarning("Could not parse " + path + " to an integer; skipping this disk.");
                            ssDiskNumber = -1;
                        }

                        deviceInstanceID = Utilities.Utility.BuildWindows8PnpDeviceName(model, serialNumber, (UInt16)drive["BusType"], ssDiskNumber);
                        if (String.IsNullOrEmpty(model))
                        {
                            model = "Not Available";
                        }
                        if (String.IsNullOrEmpty(serialNumber))
                        {
                            serialNumber = "Not Available";
                        }

                        description = (drive["Description"] == null ? "N/A" : drive["Description"].ToString());

                        interfaceType = Utilities.Utility.GetWindows8DriveInterfaceType((UInt16)drive["BusType"]);

                        manufacturer = (drive["Manufacturer"] == null ? "Unknown" : drive["Manufacturer"].ToString());

                        if (manufacturer != "Unknown")
                        {
                            model = (manufacturer + model).Trim();
                        }

                        if (ignoreVirtualDisks && KnownVirtualDisks.IsDiskOnVirtualDiskList(model != "Undefined in WMI" ? model : deviceInstanceID))
                        {
                            // It's a virtual disk we're ignoring, so skip it (continue).
                            SiAuto.Main.LogColored(System.Drawing.Color.Goldenrod, "[Virtual Disk Discard] Disk is on known list of VDs; expurgating.");
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        SiAuto.Main.LogError("GetDiskDetails failed to collect general disk detail.");
                        SiAuto.Main.LogException(ex);
                    }

                    try
                    {
                        physicalSectorSize = (UInt64)drive["PhysicalSectorSize"];
                        logicalSectorSize = (UInt64)drive["LogicalSectorSize"];
                        totalBytes = (UInt64)drive["Size"];
                        status = Utilities.Utility.GetWindows8DiskOperationalStatus((UInt16[])drive["OperationalStatus"]);
                        if (status.ToLower().Contains("predictive failure"))
                        {
                            failurePredicted = true;
                        }
                        else
                        {
                            failurePredicted = Utilities.Utility.GetWindows8AlternatePredFail(path);
                            if (failurePredicted)
                            {
                                status = "Predictive Failure";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        physicalSectorSize = 0;
                        logicalSectorSize = 0;
                        totalBytes = 0;
                        status = String.Empty;

                        SiAuto.Main.LogWarning("GetDiskDetails failed to collect numeric sector info; disk type may not support it or is an " +
                            "empty media card reader.");
                        SiAuto.Main.LogException(ex);
                    }

                    String partitionCount = "0";
                    UInt32 bytesPerSector = 0;
                    UInt64 totalCylinders = 0;
                    UInt32 totalHeads = 0;
                    UInt64 totalSectors = 0;
                    UInt64 totalTracks = 0;
                    UInt32 tracksPerCylinder = 0;
                    //UInt64 extTotalBytes = 0;
                    UInt64 extPhysicalSectorSize = 0;
                    UInt64 extLogicalSectorSize = 0;

                    if (GetWindows8ExtendedDiskInfo(path, out partitionCount, out bytesPerSector, out totalCylinders, out totalHeads, out totalSectors, out totalTracks,
                        out tracksPerCylinder, out totalBytes, out extPhysicalSectorSize, out extLogicalSectorSize))
                    {
                        SiAuto.Main.LogMessage("Successfully fetched additional disk characteristics using Windows 8 extended call.");
                        smartDataTable.AddDiskInformation(deviceInstanceID, path, model, serialNumber, firmwareRevision, description, interfaceType,
                            String.Empty, path, partitionCount, status, failurePredicted, bytesPerSector, totalCylinders,
                            totalHeads, totalSectors, totalTracks, tracksPerCylinder, totalBytes, (extPhysicalSectorSize == 0 ? physicalSectorSize : extPhysicalSectorSize),
                            (extLogicalSectorSize == 0 ? logicalSectorSize : extLogicalSectorSize));
                    }
                    else
                    {
                        SiAuto.Main.LogWarning("Unable to fetch additional disk characteristics using Windows 8 extended call. This may be a storage space volume.");
                        smartDataTable.AddDiskInformation(deviceInstanceID, path, model, serialNumber, firmwareRevision, description, interfaceType,
                            String.Empty, path, "0", status, failurePredicted, 0, 0, 0, 0, 0, 0, totalBytes, physicalSectorSize, logicalSectorSize);
                    }
                }
            }
            else
            {
                // Browse all WMI physical disks.
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                  Properties.Resources.WmiQueryStringNonWin8).Get())
                {
                    UInt32 bytesPerSector = 0;
                    UInt64 totalCylinders = 0;
                    UInt32 totalHeads = 0;
                    UInt64 totalSectors = 0;
                    UInt64 totalTracks = 0;
                    UInt32 tracksPerCylinder = 0;
                    UInt64 totalBytes = 0;

                    String path = String.Empty;
                    String serialNumber = String.Empty;
                    String firmwareRevision = String.Empty;
                    String deviceInstanceID = String.Empty;
                    String model = String.Empty;
                    String description = String.Empty;
                    String interfaceType = String.Empty;
                    String mediaType = String.Empty;
                    String name = String.Empty;
                    String partitionCount = String.Empty;
                    String status = String.Empty;
                    bool failurePredicted = false;
                    String manufacturer = String.Empty; // we don't save this one; we just check it against ignored disks.

                    SiAuto.Main.LogMessage("GetDiskDetails is enumerating Win32_DiskDrive on a non Windows 8 platform disk.");

                    try
                    {
                        path = (drive["DeviceID"] == null ? "Invalid ID or Undefined" : drive["DeviceID"].ToString());
                        SiAuto.Main.LogMessage("Examining disk " + path);

                        try
                        {
                            serialNumber = (drive["SerialNumber"] == null ? "Invalid or Undefined" : drive["SerialNumber"].ToString().Trim());
                            if (serialNumber.StartsWith("W -D") || serialNumber.StartsWith("CO-Z"))
                            {
                                serialNumber = Utilities.Utility.ReverseCharacters(serialNumber);
                            }
                        }
                        catch
                        {
                            serialNumber = "Not Available";
                        }
                        try
                        {
                            firmwareRevision = (drive["FirmwareRevision"] == null ? "Invalid or Undefined" : drive["FirmwareRevision"].ToString());
                            if (serialNumber.StartsWith("W -D"))
                            {
                                firmwareRevision = Utilities.Utility.ReverseCharacters(firmwareRevision);
                            }
                        }
                        catch
                        {
                            firmwareRevision = "Not Available";
                        }
                        deviceInstanceID = (drive["PNPDeviceID"] == null ? "Invalid ID or Undefined" : drive["PNPDeviceID"].ToString());
                        model = (drive["Model"] == null ? "Undefined in WMI" : drive["Model"].ToString());
                        description = (drive["Description"] == null ? "N/A" : drive["Description"].ToString());
                        interfaceType = (drive["InterfaceType"] == null ? "Unknown" : drive["InterfaceType"].ToString());
                        try
                        {
                            mediaType = (drive["MediaType"] == null ? "Undefined in WMI" : drive["MediaType"].ToString());
                        }
                        catch
                        {
                            mediaType = "Not Available";
                        }
                        name = (drive["Name"] == null ? "Undefined in WMI" : drive["Name"].ToString());
                        partitionCount = (drive["Partitions"] == null ? "Unknown" : drive["Partitions"].ToString());
                        status = (drive["Status"] == null ? "Unknown" : drive["Status"].ToString());
                        if (status.ToLower().Contains("pred fail"))
                        {
                            failurePredicted = true;
                        }
                        else
                        {
                            failurePredicted = false;
                        }
                        manufacturer = (drive["Manufacturer"] == null ? "Unknown" : drive["Manufacturer"].ToString());

                        if (ignoreVirtualDisks && KnownVirtualDisks.IsDiskOnVirtualDiskList(model != "Undefined in WMI" ? model : deviceInstanceID))
                        {
                            // It's a virtual disk we're ignoring, so skip it (continue).
                            SiAuto.Main.LogColored(System.Drawing.Color.Goldenrod, "[Virtual Disk Discard] Disk is on known list of VDs; expurgating.");
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        SiAuto.Main.LogError("GetDiskDetails failed to collect general disk detail.");
                        SiAuto.Main.LogException(ex);
                    }

                    try
                    {
                        bytesPerSector = (UInt32)drive["BytesPerSector"];
                        totalCylinders = (UInt64)drive["TotalCylinders"];
                        totalHeads = (UInt32)drive["TotalHeads"];
                        totalSectors = (UInt64)drive["TotalSectors"];
                        totalTracks = (UInt64)drive["TotalTracks"];
                        tracksPerCylinder = (UInt32)drive["TracksPerCylinder"];
                        totalBytes = bytesPerSector * totalSectors;
                    }
                    catch (Exception ex)
                    {
                        bytesPerSector = 0;
                        totalCylinders = 0;
                        totalHeads = 0;
                        totalSectors = 0;
                        totalTracks = 0;
                        tracksPerCylinder = 0;
                        totalBytes = 0;

                        SiAuto.Main.LogWarning("GetDiskDetails failed to collect numeric sector info; disk type may not support it or is an " +
                            "empty media card reader.");
                        SiAuto.Main.LogException(ex);
                    }

                    SiAuto.Main.LogMessage("Inject disk details into data table.");
                    smartDataTable.AddDiskInformation(deviceInstanceID, path, model, serialNumber, firmwareRevision, description, interfaceType,
                        mediaType, name, partitionCount, status, failurePredicted, bytesPerSector, totalCylinders,
                        totalHeads, totalSectors, totalTracks, tracksPerCylinder, totalBytes, 0, 0);
                }
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.GetDiskDetails");
        }

        public static bool GetWindows8ExtendedDiskInfo(String path, out String partitionCount, out UInt32 bytesPerSector, out UInt64 totalCylinders, out UInt32 totalHeads,
            out UInt64 totalSectors, out UInt64 totalTracks, out UInt32 tracksPerCylinder, out UInt64 totalBytes, out UInt64 physicalSectorSize, out UInt64 logicalSectorSize)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.DiskEnumerator.GetWindows8ExtendedDiskInfo");
            SiAuto.Main.LogMessage("GetDiskDetails is enumerating Win32_DiskDrive on a non Windows 8 platform disk.");

            partitionCount = "0";
            bytesPerSector = 0;
            totalCylinders = 0;
            totalHeads = 0;
            totalSectors = 0;
            totalTracks = 0;
            tracksPerCylinder = 0;
            totalBytes = 0;
            physicalSectorSize = 0;
            logicalSectorSize = 0;

            foreach (ManagementObject drive in new ManagementObjectSearcher(
                  Properties.Resources.WmiQueryStringNonWin8).Get())
            {
                try
                {
                    String testPath = (drive["DeviceID"] == null ? "Invalid ID or Undefined" : drive["DeviceID"].ToString());
                    
                    SiAuto.Main.LogMessage("Comparing disk " + path + " to " + testPath);
                    if (String.Compare(path, testPath, true) != 0)
                    {
                        continue;
                    }

                    partitionCount = (drive["Partitions"] == null ? "Unknown" : drive["Partitions"].ToString());
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogError("GetWindows8ExtendedDiskInfo failed to collect general disk detail.");
                    SiAuto.Main.LogException(ex);
                }

                try
                {
                    bytesPerSector = (UInt32)drive["BytesPerSector"];
                    totalCylinders = (UInt64)drive["TotalCylinders"];
                    totalHeads = (UInt32)drive["TotalHeads"];
                    totalSectors = (UInt64)drive["TotalSectors"];
                    totalTracks = (UInt64)drive["TotalTracks"];
                    tracksPerCylinder = (UInt32)drive["TracksPerCylinder"];
                    totalBytes = bytesPerSector * totalSectors;
                }
                catch (Exception ex)
                {
                    bytesPerSector = 0;
                    totalCylinders = 0;
                    totalHeads = 0;
                    totalSectors = 0;
                    totalTracks = 0;
                    tracksPerCylinder = 0;
                    totalBytes = 0;

                    SiAuto.Main.LogWarning("GetWindows8ExtendedDiskInfo failed to collect numeric sector info; disk type may not support it or is an " +
                        "empty media card reader.");
                    SiAuto.Main.LogException(ex);
                }

                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.GetWindows8ExtendedDiskInfo");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.GetWindows8ExtendedDiskInfo");
            return false;
        }

        public static void CheckDiskHealth(FeverishDisk feverishDisks, int criticalTemperatureThreshold, int overheatedTemperatureThreshold, int hotTemperatureThreshold,
                    int warmTemperatureThreshold, int ssdLifeLeftCritical, int ssdLifeLeftWarning, int ssdRetirementCritical, int ssdRetirementWarning, bool ignoreOverheated,
                    bool ignoreHot, bool ignoreWarm, bool reportWarnings, bool reportGeriatric, bool reportCritical, String temperaturePreference)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.DiskEnumerator.CheckDiskHealth");

            SiAuto.Main.LogMessage("Construct the SMART definitions tables.");
            SmartDefinitions definitions = new SmartDefinitions();
            SiAuto.Main.LogMessage("SMART definitions tables constructed.");

            int absurdTemperatureThreshold = 90;
            
            try
            {
                SiAuto.Main.LogMessage("Acquire Registry objects.");
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;
                Microsoft.Win32.RegistryKey diskKey = null;
                String diskName = String.Empty;
                String diskPath = String.Empty;

                dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);
                SiAuto.Main.LogMessage("Acquired Registry objects.");

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    byte[] smartData = null;
                    byte[] smartThreshold = null;

                    int badSectorCountOrRetiredBlockCount = 0;
                    int pendingSectorCountOrSsdLifeLeft = 0;
                    int endToEndErrorCountOrLifeCurve = 0;
                    int reallocationEventCount = 0;
                    int spinRetryCount = 0;
                    int uncorrectableSectorCount = 0;
                    int ultraAtaCrcErrorCount = 0;
                    int temperature = 0;
                    int fTemperature = 0;
                    int kTemperature = 0;
                    int airflowTemperature = 0;
                    int fAirflowTemperature = 0;
                    int kAirflowTemperature = 0;
                    int powerOnHours = 0;

                    bool isDiskCritical = false;
                    bool isDiskWarning = false;
                    bool isDiskGeriatric = false;
                    bool isWarningTemperature = false;
                    bool isCriticalTemperature = false;
                    bool isThresholdViolated = false;
                    bool isThresholdMet = false;
                    bool isSsd = false;

                    // Flags for backups
                    bool isHotTec = false;
                    bool isDiskTec = false;

                    // Need to set this to hard disk -- if an SSD we'll try to get the manufacturer.
                    SsdManufacturer manufacturer = SsdManufacturer.SSD_MANUFACTURER_HARD_DISK;

                    try
                    {
                        diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);

                        try
                        {
                            diskName = (String)diskKey.GetValue("PreferredModel");
                        }
                        catch
                        {
                        }

                        if (String.IsNullOrEmpty(diskName))
                        {
                            diskName = (String)diskKey.GetValue("Model");
                        }
                        diskPath = (String)diskKey.GetValue("DevicePath");
                        smartData = (byte[])diskKey.GetValue("RawSmartData");
                        smartThreshold = (byte[])diskKey.GetValue("RawThresholdData");

                        bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                        isSsd = (bool.Parse((String)diskKey.GetValue("IsSsd")));

                        SiAuto.Main.LogColored(System.Drawing.Color.Purple, "[Health Check] Begin health check for " + diskPath);
                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Disk Name = " + diskName + ", IsActive = " + activeFlag.ToString() + ", " +
                            "IsSsd = " + isSsd.ToString());

                        if (!activeFlag)
                        {
                            // Inactive/Stale disk.
                            SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Disk is inactive or stale; skipping.");
                            diskKey.Close();
                            continue;
                        }
                        else
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.LimeGreen, "[Health Check] Disk is active and will be assessed.");
                        }

                        if (isSsd)
                        {
                            SiAuto.Main.LogMessage("Identifying SSD controller manufacturer from SMART attributes.");
                            String model = diskName.ToUpper();
                            manufacturer = IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes(smartData, model);
                        }

                        try
                        {
                            String dateIgnored = (String)diskKey.GetValue("DateIgnored", String.Empty);
                            SiAuto.Main.LogString("dateIgnored (date/time parse check)", dateIgnored);
                            DateTime result;
                            if (DateTime.TryParse(dateIgnored, out result))
                            {
                                // Ignored disk.
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] Date ignored is set; dateIgnored = " + dateIgnored + ", " +
                                    "assessment of disk " + diskName + ", path " + diskPath + " is cancelled.");
                                diskKey.Close();
                                continue;
                            }
                        }
                        catch(Exception ex)
                        {
                            SiAuto.Main.LogWarning("Error occurred on dateIgnored value check; parse operation failed. The disk will be treated as active.");
                            SiAuto.Main.LogException(ex);
                        }
                    }
                    catch(Exception ex)
                    {
                        SiAuto.Main.LogError("Disk " + diskName + ", path " + diskPath + ": SMART evaluation failed. Either the " +
                            "data is not available, or an error occurred reading from the Registry.");
                        SiAuto.Main.LogException(ex);
                        if( diskKey != null )
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] Setting IsDiskUnknown to true.");
                            diskKey.SetValue("IsDiskUnknown", true);
                            diskKey.Close();
                        }
                        continue;
                    }

                    // OK we got the data so let's check it out.
                    // Let's harvest the harvestable data.
                    // SMART data is 512 bytes, and a drive can have up to 30 attributes defined.
                    // We skip bytes 0-1; the attributes then are 12 bytes apiece.
                    //
                    // First attribute is bytes 2-13.
                    // Second attribute is bytes 14-25.
                    // And so on...

                    int attributeID = 0;
                    String hexAttributeID = String.Empty;
                    int flags;
                    String binaryFlags = String.Empty;
                    int threshold = 0;
                    int smartValue = 0;
                    int worst = 0;
                    String rawData = String.Empty;
                    String guid = String.Empty;

                    String attributeName = String.Empty;
                    String description = String.Empty;
                    
                    // Raw data is given in the 7th through 12th bytes of each attribute.
                    // Low order is first byte, ascending to the last byte, high order.
                    // For display to the user we have to reverse 'em so that they make sense.
                    // If SMART says 0123456789AB, we need to reverse it to BA9876543210.
                    int rawLow, raw1, raw2, raw3, raw4, rawHigh;
                    rawLow = raw1 = raw2 = raw3 = raw4 = rawHigh = 0;

                    int attributesProcessed = 0;

                    for (int i = 0; i < 30; i++)
                    {
                        int offset = 12 * i;

                        // Attribute ID: 2
                        attributeID = smartData[offset + 2];
                        if (attributeID == 0)
                        {
                            // Done!
                            SiAuto.Main.LogColored(System.Drawing.Color.Chartreuse, "attributeID == 0; we will skip this item"); 
                            continue;
                        }
                        attributesProcessed++;
                        hexAttributeID = (attributeID.ToString("X").Length == 1 ?
                            "0" + attributeID.ToString("X") : attributeID.ToString("X"));

                        flags = smartData[offset + 3];
                        binaryFlags = Utilities.Utility.NormalizeBinaryFlags(Convert.ToString(flags, 2)); // converts flags to the binary string
                        threshold = smartThreshold[offset + 3];
                        smartValue = smartData[offset + 5];
                        worst = smartData[offset + 6];
                        rawLow = smartData[offset + 7];
                        raw1 = smartData[offset + 8];
                        raw2 = smartData[offset + 9];
                        raw3 = smartData[offset + 10];
                        raw4 = smartData[offset + 11];
                        rawHigh = smartData[offset + 12];

                        rawData = ConcatenateRawData(rawHigh, raw4, raw3, raw2, raw1, rawLow);

                        DataRow[] smartRows;

                        SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Health Check] Attribute " + attributeID.ToString() + ", " +
                            "binaryFlags = " + binaryFlags + ", threshold = " + threshold.ToString() + ", value = " + smartValue.ToString() + ", " +
                            "worst = " + worst.ToString() + ", rawData = " + rawData);

                        if (isSsd)
                        {
                            SiAuto.Main.LogMessage("[HealthCheck] Disk is SSD; conducting vendor-specific SSD assessments.");
                            switch (manufacturer)
                            {
                                case SsdManufacturer.SSD_MANUFACTURER_EVEREST:
                                    {
                                        diskKey.SetValue("SsdController", "Indilinx Everest");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing Indilinx Everest");
                                        smartRows = definitions.SsdEverestDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            if (attributeID == 5 || attributeID == 9)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 5: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 233: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        pendingSectorCountOrSsdLifeLeft = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Left", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;
                                                            SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Life Left is Critical");

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;
                                                            SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Life Left is Warning");

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] Indilinx Everest SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_BAREFOOT3:
                                    {
                                        diskKey.SetValue("SsdController", "Indilinx Barefoot 3");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing Indilinx Barefoot 3");
                                        smartRows = definitions.SsdBarefoot3Definitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            if (attributeID == 5 || attributeID == 9)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 5: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 233: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        pendingSectorCountOrSsdLifeLeft = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Left", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;
                                                            SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Life Left is Critical");

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;
                                                            SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Life Left is Warning");

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] Indilinx Everest SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_INDILINX:
                                    {
                                        diskKey.SetValue("SsdController", "Indilinx");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing Indilinx");
                                        smartRows = definitions.SsdIndilinxDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            if (attributeID == 9)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 209: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        pendingSectorCountOrSsdLifeLeft = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] SSD % Life Remaining", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;
                                                            SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Life Left is Critical");

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53849);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;
                                                            SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Life Left is Warning");

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53850);
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] Indilinx SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_INTEL:
                                case SsdManufacturer.SSD_MANUFACTURER_STEC:
                                    {
                                        bool isIntel = false;
                                        if (manufacturer == SsdManufacturer.SSD_MANUFACTURER_STEC)
                                        {
                                            diskKey.SetValue("SsdController", "STEC");
                                            SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing STEC");
                                            smartRows = definitions.SsdStecDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        }
                                        else
                                        {
                                            diskKey.SetValue("SsdController", "Intel");
                                            SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing Intel");
                                            smartRows = definitions.SsdIntelDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                            isIntel = true;
                                        }
                                        attributeName = GetAttributeName(smartRows);
                                        
                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                            // Temperature for SandForce is attribute 194.
                                            if (attributeID == 194)
                                            {
                                                // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                                                // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            if (attributeID == 5 || attributeID == 9 || attributeID == 184 || attributeID == 196 || attributeID == 197)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                    // Attributes 5, 184, 194 are common to Intel and STEC; Intel uses 233 and STEC uses 196, 197
                                                case 5: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 184: // End-to-End Errors
                                                    {
                                                        endToEndErrorCountOrLifeCurve = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] End-to-End Errors", endToEndErrorCountOrLifeCurve);
                                                        break;
                                                    }
                                                case 194:
                                                    {
                                                        if (temperature == 0) // If both are defined we'll read just one.
                                                        {
                                                            String temperatureLabel = " (Reported Temperature: ";
                                                            temperature = (int)counter;
                                                            fTemperature = (temperature * 9 / 5) + 32;
                                                            kTemperature = temperature + 273;
                                                            SiAuto.Main.LogInt("[Health Check] Temperature (Celsius)", temperature);

                                                            switch (temperaturePreference.ToUpper())
                                                            {
                                                                case "F":
                                                                    {
                                                                        temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                                        break;
                                                                    }
                                                                case "K":
                                                                    {
                                                                        temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                                        break;
                                                                    }
                                                                case "C":
                                                                default:
                                                                    {
                                                                        temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                                        break;
                                                                    }
                                                            }

                                                            if (temperature > absurdTemperatureThreshold)
                                                            {
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                            }
                                                            else if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isCriticalTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                                        Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                            }
                                                            else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Overheated");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                                        Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                            }
                                                            else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Hot");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                                        Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                            }
                                                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Warm");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                                        Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                            }
                                                            else
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = false;
                                                                isCriticalTemperature = false;
                                                                if (feverishDisks.ItemExists(guid))
                                                                {
                                                                    feverishDisks.RemoveItem(guid);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case 233: // % Life remaining
                                                    {
                                                        if (isIntel)
                                                        {
                                                            // % of life remaining
                                                            pendingSectorCountOrSsdLifeLeft = smartValue;
                                                            SiAuto.Main.LogInt("[Health Check] SSD % Life Remaining", pendingSectorCountOrSsdLifeLeft);
                                                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                                isDiskCritical = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Life Left is Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                        Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                            }
                                                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                                isDiskWarning = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Life Left is Warning");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                        Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case 196:
                                                    {
                                                        reallocationEventCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Reallocation Events", reallocationEventCount);
                                                        break;
                                                    }
                                                case 197:
                                                    {
                                                        pendingSectorCountOrSsdLifeLeft = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Sectors Pending Retirement", pendingSectorCountOrSsdLifeLeft);
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] Intel SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_TOSHIBA:
                                case SsdManufacturer.SSD_MANUFACTURER_JMICRON:
                                    {
                                        bool isJMicron = false;
                                        if (manufacturer == SsdManufacturer.SSD_MANUFACTURER_JMICRON)
                                        {
                                            diskKey.SetValue("SsdController", "JMicron");
                                            SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing JMicron");
                                            smartRows = definitions.SsdJMicronDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                            isJMicron = true;
                                        }
                                        else
                                        {
                                            diskKey.SetValue("SsdController", "Toshiba");
                                            SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing Toshiba");
                                            smartRows = definitions.SsdToshibaDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        }
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                            // Temperature for JMicron is attribute 194.
                                            if (attributeID == 194)
                                            {
                                                // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                                                // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            if (attributeID == 5 || attributeID == 9 || attributeID == 170 || attributeID == 175 || attributeID == 197)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 5: // Retired Blocks
                                                case 170: // Bad Block Count
                                                    {
                                                        if (badSectorCountOrRetiredBlockCount == 0)
                                                        {
                                                            badSectorCountOrRetiredBlockCount = (int)counter;
                                                        }
                                                        else
                                                        {
                                                            if (badSectorCountOrRetiredBlockCount < (int)counter)
                                                            {
                                                                // Take the worse value of 5/170
                                                                badSectorCountOrRetiredBlockCount = (int)counter;
                                                            }
                                                        }
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 173: // Media Wearout Indicator
                                                    {
                                                        if (!isJMicron)
                                                        {
                                                            // Value starts at 200, so cut in half to get % remaining
                                                            pendingSectorCountOrSsdLifeLeft = smartValue / 2;
                                                            SiAuto.Main.LogInt("[Health Check] SSD Life Remaining (Wear Leveling)", pendingSectorCountOrSsdLifeLeft);
                                                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                                isDiskCritical = true;

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                        Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                            }
                                                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                                isDiskWarning = true;

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                        Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case 175: // Bad Cluster Table Count (ECC Fail Count)
                                                    {
                                                        if (isJMicron)
                                                        {
                                                            reallocationEventCount = ((int)counter) * 2;
                                                            SiAuto.Main.LogInt("[Health Check] Bad Cluster Table Count (ECC Fail Count)", reallocationEventCount);
                                                        }
                                                        break;
                                                    }
                                                case 197:
                                                    {
                                                        pendingSectorCountOrSsdLifeLeft = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Current Pending Sector Count", pendingSectorCountOrSsdLifeLeft);
                                                        break;
                                                    }
                                                case 194:
                                                    {
                                                        if (temperature == 0) // If both are defined we'll read just one.
                                                        {
                                                            String temperatureLabel = " (Reported Temperature: ";
                                                            temperature = (int)counter;
                                                            fTemperature = (temperature * 9 / 5) + 32;
                                                            kTemperature = temperature + 273;
                                                            SiAuto.Main.LogInt("[Health Check] Temperature (Celsius)", temperature);

                                                            switch (temperaturePreference.ToUpper())
                                                            {
                                                                case "F":
                                                                    {
                                                                        temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                                        break;
                                                                    }
                                                                case "K":
                                                                    {
                                                                        temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                                        break;
                                                                    }
                                                                case "C":
                                                                default:
                                                                    {
                                                                        temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                                        break;
                                                                    }
                                                            }

                                                            if (temperature > absurdTemperatureThreshold)
                                                            {
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                            }
                                                            else if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isCriticalTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                                        Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                            }
                                                            else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Overheated");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                                        Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                            }
                                                            else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Hot");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                                        Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                            }
                                                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Warm");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                                        Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                            }
                                                            else
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = false;
                                                                isCriticalTemperature = false;
                                                                if (feverishDisks.ItemExists(guid))
                                                                {
                                                                    feverishDisks.RemoveItem(guid);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] JMicron SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_MICRON:
                                    {
                                        diskKey.SetValue("SsdController", "Micron");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing Micron");
                                        smartRows = definitions.SsdMicronDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            if (attributeID == 170 || attributeID == 9 || attributeID == 196 || attributeID == 173)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            if (attributeID == 194) // Temperature; grab just the last 2 digits of raw data
                                            {
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 170: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 196: // Reallocations
                                                    {
                                                        reallocationEventCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Reallocation Events", reallocationEventCount);
                                                        break;
                                                    }
                                                case 173: // Wear Leveling
                                                    {
                                                        endToEndErrorCountOrLifeCurve = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Wear Leveling Count", endToEndErrorCountOrLifeCurve);
                                                        break;
                                                    }
                                                case 194: // Temperature
                                                    {
                                                        if (temperature == 0) // If both are defined we'll read just one.
                                                        {
                                                            String temperatureLabel = " (Reported Temperature: ";
                                                            temperature = (int)counter;
                                                            fTemperature = (temperature * 9 / 5) + 32;
                                                            kTemperature = temperature + 273;
                                                            SiAuto.Main.LogInt("[Health Check] Temperature (Celsius)", temperature);

                                                            switch (temperaturePreference.ToUpper())
                                                            {
                                                                case "F":
                                                                    {
                                                                        temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                                        break;
                                                                    }
                                                                case "K":
                                                                    {
                                                                        temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                                        break;
                                                                    }
                                                                case "C":
                                                                default:
                                                                    {
                                                                        temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                                        break;
                                                                    }
                                                            }

                                                            if (temperature > absurdTemperatureThreshold)
                                                            {
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                            }
                                                            else if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isCriticalTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                                        Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                            }
                                                            else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Overheated");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                                        Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                            }
                                                            else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Hot");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                                        Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                            }
                                                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Warm");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                                        Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                            }
                                                            else
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = false;
                                                                isCriticalTemperature = false;
                                                                if (feverishDisks.ItemExists(guid))
                                                                {
                                                                    feverishDisks.RemoveItem(guid);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case 202: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        pendingSectorCountOrSsdLifeLeft = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Remaining (Wear Leveling)", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;
                                                            
                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;
                                                            
                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] Micron SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_SAMSUNG:
                                    {
                                        diskKey.SetValue("SsdController", "Samsung");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing Samsung");
                                        smartRows = definitions.SsdSamsungDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                            // Temperature for SandForce is attribute 190.
                                            if (attributeID == 190)
                                            {
                                                // Attributes 190 are temperature. Some manufacturers use a lot of the raw data field
                                                // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            if (attributeID == 5 || attributeID == 9 || attributeID == 179 || attributeID == 183)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 5: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 179: // Reserve Blocks Used
                                                    {
                                                        reallocationEventCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Reserve Blocks Used Count", reallocationEventCount);
                                                        break;
                                                    }
                                                case 183: // Runtime Bad Blocks
                                                    {
                                                        endToEndErrorCountOrLifeCurve = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Runtime Bad Blocks", endToEndErrorCountOrLifeCurve);
                                                        break;
                                                    }
                                                case 173: // New Wear Leveling (% Life remaining)
                                                case 177: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        pendingSectorCountOrSsdLifeLeft = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Remaining (Wear Leveling)", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;
                                                            
                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;
                                                            
                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                case 190:
                                                    {
                                                        if (temperature == 0) // If both are defined we'll read just one.
                                                        {
                                                            String temperatureLabel = " (Reported Temperature: ";
                                                            temperature = (int)counter;
                                                            fTemperature = (temperature * 9 / 5) + 32;
                                                            kTemperature = temperature + 273;
                                                            SiAuto.Main.LogInt("[Health Check] Temperature (Celsius)", temperature);

                                                            switch (temperaturePreference.ToUpper())
                                                            {
                                                                case "F":
                                                                    {
                                                                        temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                                        break;
                                                                    }
                                                                case "K":
                                                                    {
                                                                        temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                                        break;
                                                                    }
                                                                case "C":
                                                                default:
                                                                    {
                                                                        temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                                        break;
                                                                    }
                                                            }

                                                            if (temperature > absurdTemperatureThreshold)
                                                            {
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                            }
                                                            else if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isCriticalTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                                        Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                            }
                                                            else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Overheated");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                                        Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                            }
                                                            else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Hot");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                                        Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                            }
                                                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Warm");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                                        Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                            }
                                                            else
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = false;
                                                                isCriticalTemperature = false;
                                                                if (feverishDisks.ItemExists(guid))
                                                                {
                                                                    feverishDisks.RemoveItem(guid);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] Samsung SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_MARVELL:
                                    {
                                        diskKey.SetValue("SsdController", "Marvell");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing Marvell");
                                        smartRows = definitions.SsdSamsungDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                            // Temperature for SandForce is attribute 190.
                                            if (attributeID == 190)
                                            {
                                                // Attributes 190 are temperature. Some manufacturers use a lot of the raw data field
                                                // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            if (attributeID == 5 || attributeID == 9 || attributeID == 179 || attributeID == 183)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 5: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 178: // Reserve Blocks Used
                                                    {
                                                        reallocationEventCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Reserve Blocks Used Count", reallocationEventCount);
                                                        break;
                                                    }
                                                case 177: // % Life remaining (wear leveling)
                                                    {
                                                        // % of life remaining
                                                        pendingSectorCountOrSsdLifeLeft = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Remaining (Wear Leveling)", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;
                                                            
                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;
                                                            
                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] Marvell SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_SANDFORCE:
                                    {
                                        diskKey.SetValue("SsdController", "SandForce");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing SandForce");
                                        smartRows = definitions.SsdSandForceDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                            // Temperature for SandForce is attribute 194.
                                            if (attributeID == 194)
                                            {
                                                // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                                                // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            if (attributeID == 5 || attributeID == 9 || attributeID == 196)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 5: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 196: // Reallocation Event Count
                                                    {
                                                        reallocationEventCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Reallocation Event Count", reallocationEventCount);
                                                        break;
                                                    }
                                                case 231: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        pendingSectorCountOrSsdLifeLeft = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Remaining", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;
                                                            
                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;
                                                            
                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                case 230: // Life Curve
                                                    {
                                                        // 100 = good; 90 or less = throttled
                                                        endToEndErrorCountOrLifeCurve = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] Life Curve Status", endToEndErrorCountOrLifeCurve);
                                                        if (endToEndErrorCountOrLifeCurve <= 90)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidEndToEndErrors");
                                                            isDiskWarning = true;
                                                            SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] Life Curve Status: Throttled");

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdThrottled,
                                                                    Properties.Resources.FeverishExplanationSsdThrottled,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53848);
                                                        }
                                                        break;
                                                    }
                                                case 194:
                                                    {
                                                        if (temperature == 0) // If both are defined we'll read just one.
                                                        {
                                                            String temperatureLabel = " (Reported Temperature: ";
                                                            temperature = (int)counter;
                                                            fTemperature = (temperature * 9 / 5) + 32;
                                                            kTemperature = temperature + 273;
                                                            SiAuto.Main.LogInt("[Health Check] Temperature (Celsius)", temperature);

                                                            switch (temperaturePreference.ToUpper())
                                                            {
                                                                case "F":
                                                                    {
                                                                        temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                                        break;
                                                                    }
                                                                case "K":
                                                                    {
                                                                        temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                                        break;
                                                                    }
                                                                case "C":
                                                                default:
                                                                    {
                                                                        temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                                        break;
                                                                    }
                                                            }

                                                            if (temperature > absurdTemperatureThreshold)
                                                            {
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                            }
                                                            else if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isCriticalTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                                        Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                            }
                                                            else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Overheated");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                                        Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                            }
                                                            else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Hot");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                                        Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                            }
                                                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Warm");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                                        Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                            }
                                                            else
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = false;
                                                                isCriticalTemperature = false;
                                                                if (feverishDisks.ItemExists(guid))
                                                                {
                                                                    feverishDisks.RemoveItem(guid);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] SandForce SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_LAMD:
                                    {
                                        diskKey.SetValue("SsdController", "LAMD");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing LAMD");
                                        smartRows = definitions.SsdSandForceDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                            // Temperature for SandForce is attribute 194.
                                            if (attributeID == 194)
                                            {
                                                // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                                                // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            if (attributeID == 5 || attributeID == 9)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 5: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 231: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        pendingSectorCountOrSsdLifeLeft = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Remaining", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                case 194:
                                                    {
                                                        if (temperature == 0) // If both are defined we'll read just one.
                                                        {
                                                            String temperatureLabel = " (Reported Temperature: ";
                                                            temperature = (int)counter;
                                                            fTemperature = (temperature * 9 / 5) + 32;
                                                            kTemperature = temperature + 273;
                                                            SiAuto.Main.LogInt("[Health Check] Temperature (Celsius)", temperature);

                                                            switch (temperaturePreference.ToUpper())
                                                            {
                                                                case "F":
                                                                    {
                                                                        temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                                        break;
                                                                    }
                                                                case "K":
                                                                    {
                                                                        temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                                        break;
                                                                    }
                                                                case "C":
                                                                default:
                                                                    {
                                                                        temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                                        break;
                                                                    }
                                                            }

                                                            if (temperature > absurdTemperatureThreshold)
                                                            {
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                            }
                                                            else if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isCriticalTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                                        Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                            }
                                                            else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Overheated");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                                        Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                            }
                                                            else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Hot");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                                        Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                            }
                                                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Warm");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                                        Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                            }
                                                            else
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = false;
                                                                isCriticalTemperature = false;
                                                                if (feverishDisks.ItemExists(guid))
                                                                {
                                                                    feverishDisks.RemoveItem(guid);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] SandForce SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_SMART_MOD:
                                    {
                                        diskKey.SetValue("SsdController", "SMART Modular");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing SMART Modular");
                                        smartRows = definitions.SsdSmartModularDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                            // Temperature for SandForce is attribute 194.
                                            if (attributeID == 194)
                                            {
                                                // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                                                // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            if (attributeID == 9 || attributeID == 252)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 252: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 232: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        pendingSectorCountOrSsdLifeLeft = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Remaining", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;
                                                            
                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;
                                                            
                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                case 194:
                                                    {
                                                        if (temperature == 0) // If both are defined we'll read just one.
                                                        {
                                                            String temperatureLabel = " (Reported Temperature: ";
                                                            temperature = (int)counter;
                                                            fTemperature = (temperature * 9 / 5) + 32;
                                                            kTemperature = temperature + 273;
                                                            SiAuto.Main.LogInt("[Health Check] Temperature (Celsius)", temperature);

                                                            switch (temperaturePreference.ToUpper())
                                                            {
                                                                case "F":
                                                                    {
                                                                        temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                                        break;
                                                                    }
                                                                case "K":
                                                                    {
                                                                        temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                                        break;
                                                                    }
                                                                case "C":
                                                                default:
                                                                    {
                                                                        temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                                        break;
                                                                    }
                                                            }

                                                            if (temperature > absurdTemperatureThreshold)
                                                            {
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                            }
                                                            else if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isCriticalTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                                        Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                            }
                                                            else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Overheated");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                                        Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                            }
                                                            else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Hot");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                                        Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                            }
                                                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Warm");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                                        Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                            }
                                                            else
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = false;
                                                                isCriticalTemperature = false;
                                                                if (feverishDisks.ItemExists(guid))
                                                                {
                                                                    feverishDisks.RemoveItem(guid);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] SMART Modular SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_KINGSTON:
                                    {
                                        diskKey.SetValue("SsdController", "Kingston");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing Kingston");
                                        smartRows = definitions.SsdSandForceDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                            // Temperature for SandForce is attribute 194.
                                            if (attributeID == 194)
                                            {
                                                // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                                                // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            if (attributeID == 5 || attributeID == 9)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 5: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 231: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        pendingSectorCountOrSsdLifeLeft = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Remaining", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                case 194:
                                                    {
                                                        if (temperature == 0) // If both are defined we'll read just one.
                                                        {
                                                            String temperatureLabel = " (Reported Temperature: ";
                                                            temperature = (int)counter;
                                                            fTemperature = (temperature * 9 / 5) + 32;
                                                            kTemperature = temperature + 273;
                                                            SiAuto.Main.LogInt("[Health Check] Temperature (Celsius)", temperature);

                                                            switch (temperaturePreference.ToUpper())
                                                            {
                                                                case "F":
                                                                    {
                                                                        temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                                        break;
                                                                    }
                                                                case "K":
                                                                    {
                                                                        temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                                        break;
                                                                    }
                                                                case "C":
                                                                default:
                                                                    {
                                                                        temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                                        break;
                                                                    }
                                                            }

                                                            if (temperature > absurdTemperatureThreshold)
                                                            {
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                            }
                                                            else if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isCriticalTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                                        Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                            }
                                                            else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Overheated");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                                        Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                            }
                                                            else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Hot");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                                        Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                            }
                                                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Warm");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                                        Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                            }
                                                            else
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = false;
                                                                isCriticalTemperature = false;
                                                                if (feverishDisks.ItemExists(guid))
                                                                {
                                                                    feverishDisks.RemoveItem(guid);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] Kingston SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_TRANSCEND:
                                    {
                                        diskKey.SetValue("SsdController", "Transcend/Silicon Motion");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing Transcend/Silicon Motion");
                                        smartRows = definitions.SsdSandForceDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                            // Temperature for SandForce is attribute 194.
                                            if (attributeID == 194)
                                            {
                                                // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                                                // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            if (attributeID == 5 || attributeID == 9)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 5: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 169: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        pendingSectorCountOrSsdLifeLeft = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Remaining", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                case 194:
                                                    {
                                                        if (temperature == 0) // If both are defined we'll read just one.
                                                        {
                                                            String temperatureLabel = " (Reported Temperature: ";
                                                            temperature = (int)counter;
                                                            fTemperature = (temperature * 9 / 5) + 32;
                                                            kTemperature = temperature + 273;
                                                            SiAuto.Main.LogInt("[Health Check] Temperature (Celsius)", temperature);

                                                            switch (temperaturePreference.ToUpper())
                                                            {
                                                                case "F":
                                                                    {
                                                                        temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                                        break;
                                                                    }
                                                                case "K":
                                                                    {
                                                                        temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                                        break;
                                                                    }
                                                                case "C":
                                                                default:
                                                                    {
                                                                        temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                                        break;
                                                                    }
                                                            }

                                                            if (temperature > absurdTemperatureThreshold)
                                                            {
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                            }
                                                            else if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isCriticalTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                                        Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                            }
                                                            else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Overheated");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                                        Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                            }
                                                            else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Hot");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                                        Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                            }
                                                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Warm");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                                        Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                            }
                                                            else
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = false;
                                                                isCriticalTemperature = false;
                                                                if (feverishDisks.ItemExists(guid))
                                                                {
                                                                    feverishDisks.RemoveItem(guid);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] Transcend/Silicon Motion SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_KINGSPEC:
                                    {
                                        diskKey.SetValue("SsdController", "KingSpec");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing KingSpec");
                                        smartRows = definitions.SsdSandForceDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                            // Temperature for SandForce is attribute 194.
                                            if (attributeID == 194)
                                            {
                                                // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                                                // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            if (attributeID == 5 || attributeID == 9)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 5: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 194:
                                                    {
                                                        if (temperature == 0) // If both are defined we'll read just one.
                                                        {
                                                            String temperatureLabel = " (Reported Temperature: ";
                                                            temperature = (int)counter;
                                                            fTemperature = (temperature * 9 / 5) + 32;
                                                            kTemperature = temperature + 273;
                                                            SiAuto.Main.LogInt("[Health Check] Temperature (Celsius)", temperature);

                                                            switch (temperaturePreference.ToUpper())
                                                            {
                                                                case "F":
                                                                    {
                                                                        temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                                        break;
                                                                    }
                                                                case "K":
                                                                    {
                                                                        temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                                        break;
                                                                    }
                                                                case "C":
                                                                default:
                                                                    {
                                                                        temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                                        break;
                                                                    }
                                                            }

                                                            if (temperature > absurdTemperatureThreshold)
                                                            {
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                            }
                                                            else if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isCriticalTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                                        Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                            }
                                                            else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Overheated");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                                        Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                            }
                                                            else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Hot");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                                        Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                            }
                                                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Warm");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                                        Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                            }
                                                            else
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = false;
                                                                isCriticalTemperature = false;
                                                                if (feverishDisks.ItemExists(guid))
                                                                {
                                                                    feverishDisks.RemoveItem(guid);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] KingSpec SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_PHISON:
                                    {
                                        diskKey.SetValue("SsdController", "Smartbuy/Phison");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing Smartbuy/Phison");
                                        smartRows = definitions.SsdSandForceDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                            // Temperature for SandForce is attribute 194.
                                            if (attributeID == 194)
                                            {
                                                // Attribute 194 is temperature. Some manufacturers use a lot of the raw data field
                                                // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            // No bad sector counter in Smartbuy/Phison; there is no attribute 5. Attribute 9 is power on time.
                                            if (attributeID == 5 || attributeID == 9)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 5: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 231: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        pendingSectorCountOrSsdLifeLeft = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Remaining", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                case 194:
                                                    {
                                                        if (temperature == 0) // If both are defined we'll read just one.
                                                        {
                                                            String temperatureLabel = " (Reported Temperature: ";
                                                            temperature = (int)counter;
                                                            fTemperature = (temperature * 9 / 5) + 32;
                                                            kTemperature = temperature + 273;
                                                            SiAuto.Main.LogInt("[Health Check] Temperature (Celsius)", temperature);

                                                            switch (temperaturePreference.ToUpper())
                                                            {
                                                                case "F":
                                                                    {
                                                                        temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                                        break;
                                                                    }
                                                                case "K":
                                                                    {
                                                                        temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                                        break;
                                                                    }
                                                                case "C":
                                                                default:
                                                                    {
                                                                        temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                                        break;
                                                                    }
                                                            }

                                                            if (temperature > absurdTemperatureThreshold)
                                                            {
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                            }
                                                            else if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isCriticalTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                                        Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                            }
                                                            else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Overheated");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                                        Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                            }
                                                            else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Hot");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                                        Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                            }
                                                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Warm");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                                        Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                            }
                                                            else
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = false;
                                                                isCriticalTemperature = false;
                                                                if (feverishDisks.ItemExists(guid))
                                                                {
                                                                    feverishDisks.RemoveItem(guid);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] Smartbuy/Phison SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_ADATA:
                                    {
                                        diskKey.SetValue("SsdController", "ADATA");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing ADATA");
                                        smartRows = definitions.SsdSandForceDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                            // Temperature for SandForce is attribute 194.
                                            if (attributeID == 194)
                                            {
                                                // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                                                // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            if (attributeID == 5 || attributeID == 9)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 5: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 231: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        pendingSectorCountOrSsdLifeLeft = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Remaining", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                case 194:
                                                    {
                                                        if (temperature == 0) // If both are defined we'll read just one.
                                                        {
                                                            String temperatureLabel = " (Reported Temperature: ";
                                                            temperature = (int)counter;
                                                            fTemperature = (temperature * 9 / 5) + 32;
                                                            kTemperature = temperature + 273;
                                                            SiAuto.Main.LogInt("[Health Check] Temperature (Celsius)", temperature);

                                                            switch (temperaturePreference.ToUpper())
                                                            {
                                                                case "F":
                                                                    {
                                                                        temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                                        break;
                                                                    }
                                                                case "K":
                                                                    {
                                                                        temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                                        break;
                                                                    }
                                                                case "C":
                                                                default:
                                                                    {
                                                                        temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                                        break;
                                                                    }
                                                            }

                                                            if (temperature > absurdTemperatureThreshold)
                                                            {
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                            }
                                                            else if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isCriticalTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                                        Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                            }
                                                            else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Overheated");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                                        Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                            }
                                                            else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Hot");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                                        Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                            }
                                                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Warm");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                                        Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                            }
                                                            else
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = false;
                                                                isCriticalTemperature = false;
                                                                if (feverishDisks.ItemExists(guid))
                                                                {
                                                                    feverishDisks.RemoveItem(guid);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] ADATA SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_SANDISK:
                                    {
                                        diskKey.SetValue("SsdController", "SanDisk");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing SanDisk");
                                        smartRows = definitions.SsdSandForceDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                            // Temperature for SandForce is attribute 194.
                                            if (attributeID == 194)
                                            {
                                                // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                                                // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            if (attributeID == 5 || attributeID == 9)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 5: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 230: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        // SanDisk has 230 and 232 so we take the worse of the two to calculate the health
                                                        int temp = smartValue;
                                                        if (pendingSectorCountOrSsdLifeLeft == 0 || (pendingSectorCountOrSsdLifeLeft > 0 && temp < pendingSectorCountOrSsdLifeLeft))
                                                        {
                                                            pendingSectorCountOrSsdLifeLeft = temp;
                                                        }
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Remaining", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                case 232: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        // SanDisk has 230 and 232 so we take the worse of the two to calculate the health
                                                        int temp = smartValue;
                                                        if (pendingSectorCountOrSsdLifeLeft == 0 || (pendingSectorCountOrSsdLifeLeft > 0 && temp < pendingSectorCountOrSsdLifeLeft))
                                                        {
                                                            pendingSectorCountOrSsdLifeLeft = temp;
                                                        }
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Remaining", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                case 194:
                                                    {
                                                        if (temperature == 0) // If both are defined we'll read just one.
                                                        {
                                                            String temperatureLabel = " (Reported Temperature: ";
                                                            temperature = (int)counter;
                                                            fTemperature = (temperature * 9 / 5) + 32;
                                                            kTemperature = temperature + 273;
                                                            SiAuto.Main.LogInt("[Health Check] Temperature (Celsius)", temperature);

                                                            switch (temperaturePreference.ToUpper())
                                                            {
                                                                case "F":
                                                                    {
                                                                        temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                                        break;
                                                                    }
                                                                case "K":
                                                                    {
                                                                        temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                                        break;
                                                                    }
                                                                case "C":
                                                                default:
                                                                    {
                                                                        temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                                        break;
                                                                    }
                                                            }

                                                            if (temperature > absurdTemperatureThreshold)
                                                            {
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                            }
                                                            else if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isCriticalTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                                        Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                            }
                                                            else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Overheated");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                                        Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                            }
                                                            else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Hot");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                                        Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                            }
                                                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Warm");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                                        Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                            }
                                                            else
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = false;
                                                                isCriticalTemperature = false;
                                                                if (feverishDisks.ItemExists(guid))
                                                                {
                                                                    feverishDisks.RemoveItem(guid);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] SanDisk SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_SKHYNIX:
                                    {
                                        diskKey.SetValue("SsdController", "SK hynix");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing SK hynix");
                                        smartRows = definitions.SsdSandForceDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                            // Temperature for SandForce is attribute 194.
                                            if (attributeID == 194)
                                            {
                                                // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                                                // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            if (attributeID == 5 || attributeID == 9)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 5: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 180: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        pendingSectorCountOrSsdLifeLeft = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Remaining", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;

                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                case 194:
                                                    {
                                                        if (temperature == 0) // If both are defined we'll read just one.
                                                        {
                                                            String temperatureLabel = " (Reported Temperature: ";
                                                            temperature = (int)counter;
                                                            fTemperature = (temperature * 9 / 5) + 32;
                                                            kTemperature = temperature + 273;
                                                            SiAuto.Main.LogInt("[Health Check] Temperature (Celsius)", temperature);

                                                            switch (temperaturePreference.ToUpper())
                                                            {
                                                                case "F":
                                                                    {
                                                                        temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                                        break;
                                                                    }
                                                                case "K":
                                                                    {
                                                                        temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                                        break;
                                                                    }
                                                                case "C":
                                                                default:
                                                                    {
                                                                        temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                                        break;
                                                                    }
                                                            }

                                                            if (temperature > absurdTemperatureThreshold)
                                                            {
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                            }
                                                            else if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isCriticalTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                                        Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                            }
                                                            else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Overheated");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                                        Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                            }
                                                            else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Hot");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                                        Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                            }
                                                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Warm");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                                        Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                            }
                                                            else
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = false;
                                                                isCriticalTemperature = false;
                                                                if (feverishDisks.ItemExists(guid))
                                                                {
                                                                    feverishDisks.RemoveItem(guid);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] SanDisk SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                                case SsdManufacturer.SSD_MANUFACTURER_OTHER:
                                default:
                                    {
                                        diskKey.SetValue("SsdController", "Generic/Other");
                                        SiAuto.Main.LogColored(System.Drawing.Color.Lavender, "[Health Check] Assessing Unknown SSD");
                                        smartRows = definitions.SsdGenericDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                        attributeName = GetAttributeName(smartRows);

                                        // Keep track of certain attributes.
                                        try
                                        {
                                            long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                            // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                            // Temperature for SandForce is attribute 194.
                                            if (attributeID == 194)
                                            {
                                                // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                                                // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                                counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            if (attributeID == 5 || attributeID == 9 || attributeID == 184 || attributeID == 196 || attributeID == 197)
                                            {
                                                counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                            }

                                            switch (attributeID)
                                            {
                                                case 5: // Retired Blocks
                                                    {
                                                        badSectorCountOrRetiredBlockCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Retired Sectors", badSectorCountOrRetiredBlockCount);
                                                        break;
                                                    }
                                                case 9: // Power-On Hours
                                                    {
                                                        powerOnHours = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                                        break;
                                                    }
                                                case 184:
                                                    {
                                                        endToEndErrorCountOrLifeCurve = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] End-to-End Errors", endToEndErrorCountOrLifeCurve);
                                                        break;
                                                    }
                                                case 196: // Reallocation Event Count
                                                    {
                                                        reallocationEventCount = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Reallocation Event Count", reallocationEventCount);
                                                        break;
                                                    }
                                                case 197:
                                                    {
                                                        pendingSectorCountOrSsdLifeLeft = (int)counter;
                                                        SiAuto.Main.LogInt("[Health Check] Sectors Pending Retirement", pendingSectorCountOrSsdLifeLeft);
                                                        break;
                                                    }
                                                case 231: // % Life remaining
                                                    {
                                                        // % of life remaining
                                                        pendingSectorCountOrSsdLifeLeft = smartValue;
                                                        SiAuto.Main.LogInt("[Health Check] SSD Life Remaining", pendingSectorCountOrSsdLifeLeft);
                                                        if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskCritical = true;
                                                            
                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdNoLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdNoLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53920);
                                                        }
                                                        else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                                        {
                                                            guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                                                            isDiskWarning = true;
                                                            
                                                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusSsdLowLifeLeft,
                                                                    Properties.Resources.FeverishExplanationSsdLowLifeLeft,
                                                                    attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53921);
                                                        }
                                                        break;
                                                    }
                                                case 194:
                                                    {
                                                        if (temperature == 0) // If both are defined we'll read just one.
                                                        {
                                                            String temperatureLabel = " (Reported Temperature: ";
                                                            temperature = (int)counter;
                                                            fTemperature = (temperature * 9 / 5) + 32;
                                                            kTemperature = temperature + 273;
                                                            SiAuto.Main.LogInt("[Health Check] Temperature (Celsius)", temperature);

                                                            switch (temperaturePreference.ToUpper())
                                                            {
                                                                case "F":
                                                                    {
                                                                        temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                                        break;
                                                                    }
                                                                case "K":
                                                                    {
                                                                        temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                                        break;
                                                                    }
                                                                case "C":
                                                                default:
                                                                    {
                                                                        temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                                        break;
                                                                    }
                                                            }

                                                            if (temperature > absurdTemperatureThreshold)
                                                            {
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                            }
                                                            else if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isCriticalTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Critical");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                                        Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                            }
                                                            else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] SSD Temperature Overheated");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                                        Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                            }
                                                            else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = true;
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Hot");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                                        Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                            }
                                                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] SSD Temperature Warm");

                                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                                        Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                            }
                                                            else
                                                            {
                                                                guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                                isWarningTemperature = false;
                                                                isCriticalTemperature = false;
                                                                if (feverishDisks.ItemExists(guid))
                                                                {
                                                                    feverishDisks.RemoveItem(guid);
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SiAuto.Main.LogError("[Health Check] Unknown/Generic/Other SSD SMART assessment failed. " + ex.Message);
                                            SiAuto.Main.LogException(ex);
                                            WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                                        }
                                        break;
                                    }
                            }
                            //continue;
                        }
                        else // HDD
                        {
                            // Keep track of certain attributes.
                            smartRows = definitions.HddDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                            attributeName = GetAttributeName(smartRows);

                            try
                            {
                                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                                // Bad sectors use only the lowest 4 bytes; likewise with reallocations, pending, etc.
                                // Including 190 attribute for airflow temperature too.
                                if (attributeID == 194 || attributeID == 231 || attributeID == 190)
                                {
                                    // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                                }

                                if (attributeID == 5 || attributeID == 9 || attributeID == 184 || attributeID == 196 || attributeID == 197 || attributeID == 198)
                                {
                                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                                }

                                switch (attributeID)
                                {
                                    case 5: // Bad Sectors
                                        {
                                            badSectorCountOrRetiredBlockCount = (int)counter;
                                            SiAuto.Main.LogInt("[Health Check] Bad Sectors", badSectorCountOrRetiredBlockCount);
                                            break;
                                        }
                                    case 9: // Power-On Hours
                                        {
                                            powerOnHours = (int)counter;
                                            SiAuto.Main.LogInt("[Health Check] Power-On Time", powerOnHours);
                                            break;
                                        }
                                    case 10: // Spin Retry
                                        {
                                            spinRetryCount = (int)counter;
                                            SiAuto.Main.LogInt("[Health Check] Spin Retries", spinRetryCount);
                                            break;
                                        }
                                    //case 11: // Recalibration Retry
                                    //    {
                                    //        recalibrationRetryCount = (int)counter;
                                    //        break;
                                    //    }
                                    case 184: // End-to-End Error
                                        {
                                            endToEndErrorCountOrLifeCurve = (int)counter;
                                            SiAuto.Main.LogInt("[Health Check] End-to-End Errors", endToEndErrorCountOrLifeCurve);
                                            break;
                                        }
                                    case 196: // Reallocation Event Count
                                        {
                                            reallocationEventCount = (int)counter;
                                            SiAuto.Main.LogInt("[Health Check] Reallocations", reallocationEventCount);
                                            break;
                                        }
                                    case 197: // Pending Sectors
                                        {
                                            pendingSectorCountOrSsdLifeLeft = (int)counter;
                                            SiAuto.Main.LogInt("[Health Check] Current Pending Sectors", pendingSectorCountOrSsdLifeLeft);
                                            break;
                                        }
                                    case 198: // Uncorrectable Sectors
                                        {
                                            uncorrectableSectorCount = (int)counter;
                                            SiAuto.Main.LogInt("[Health Check] Offline Bad Sectors", uncorrectableSectorCount);
                                            break;
                                        }
                                    case 199: // CRC errors (not as serious as the ones above)
                                        {
                                            ultraAtaCrcErrorCount = (int)counter;
                                            SiAuto.Main.LogInt("[Health Check] Ultra ATA CRC Errors", ultraAtaCrcErrorCount);
                                            break;
                                        }
                                    case 190: // airflow temperature
                                        {
                                            airflowTemperature = (int)counter;
                                            fAirflowTemperature = (airflowTemperature * 9 / 5) + 32;
                                            kAirflowTemperature = fAirflowTemperature + 273;

                                            if (airflowTemperature >= criticalTemperatureThreshold && reportCritical)
                                            {
                                                guid = (String)diskKey.GetValue("NotificationGuidAirflowTemperature");
                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                isCriticalTemperature = true;
                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] HDD Airflow Temperature Critical");

                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                        Properties.Resources.FeverishExplanationCriticalTemperature,
                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                            }
                                            else if (airflowTemperature >= overheatedTemperatureThreshold && reportCritical)
                                            {
                                                guid = (String)diskKey.GetValue("NotificationGuidAirflowTemperature");
                                                IncrementDiskConsecutiveOverheatCount(diskKey);
                                                isWarningTemperature = true;
                                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] HDD Airflow Temperature Overheated");

                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                        Properties.Resources.FeverishExplanationOverheatedTempterature,
                                                        attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                            }
                                            else if (airflowTemperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                            {
                                                guid = (String)diskKey.GetValue("NotificationGuidAirflowTemperature");
                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                isWarningTemperature = true;
                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] HDD Airflow Temperature Hot");

                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                        Properties.Resources.FeverishExplanationHotTemperature,
                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                            }
                                            else if (airflowTemperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                            {
                                                guid = (String)diskKey.GetValue("NotificationGuidAirflowTemperature");
                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] HDD Airflow Temperature Warm");

                                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                        Properties.Resources.FeverishExplanationWarmTemperature,
                                                        attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                            }
                                            else
                                            {
                                                guid = (String)diskKey.GetValue("NotificationGuidAirflowTemperature");
                                                ClearDiskConsecutiveOverheatCount(diskKey);
                                                isWarningTemperature = false;
                                                isCriticalTemperature = false;
                                                if (feverishDisks.ItemExists(guid))
                                                {
                                                    feverishDisks.RemoveItem(guid);
                                                }
                                            }

                                            break;
                                        }
                                    case 194:
                                    case 231: // C2 and E7 are both temperature
                                        {
                                            if (temperature == 0) // If both are defined we'll read just one.
                                            {
                                                String temperatureLabel = " (Reported Temperature: ";
                                                temperature = (int)counter;
                                                fTemperature = (temperature * 9 / 5) + 32;
                                                kTemperature = temperature + 273;

                                                switch (temperaturePreference.ToUpper())
                                                {
                                                    case "F":
                                                        {
                                                            temperatureLabel += fTemperature.ToString() + Properties.Resources.LabelTemperatureF + ")";
                                                            break;
                                                        }
                                                    case "K":
                                                        {
                                                            temperatureLabel += kTemperature.ToString() + Properties.Resources.LabelTemperatureK + ")";
                                                            break;
                                                        }
                                                    case "C":
                                                    default:
                                                        {
                                                            temperatureLabel += temperature.ToString() + Properties.Resources.LabelTemperatureC + ")";
                                                            break;
                                                        }
                                                }

                                                if (temperature >= criticalTemperatureThreshold && reportCritical)
                                                {
                                                    guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                    IncrementDiskConsecutiveOverheatCount(diskKey);
                                                    isCriticalTemperature = true;
                                                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] HDD Temperature Critical");

                                                    feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureCritical,
                                                            Properties.Resources.FeverishExplanationCriticalTemperature + temperatureLabel,
                                                            attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53820);
                                                }
                                                else if (temperature >= overheatedTemperatureThreshold && reportCritical)
                                                {
                                                    guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                    IncrementDiskConsecutiveOverheatCount(diskKey);
                                                    isWarningTemperature = true;
                                                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] HDD Temperature Overheated");

                                                    feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureOverheated,
                                                            Properties.Resources.FeverishExplanationOverheatedTempterature + temperatureLabel,
                                                            attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53821);
                                                }
                                                else if (temperature >= hotTemperatureThreshold && !ignoreHot && reportWarnings)
                                                {
                                                    guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                    ClearDiskConsecutiveOverheatCount(diskKey);
                                                    isWarningTemperature = true;
                                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] HDD Temperature Hot");

                                                    feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureHot,
                                                            Properties.Resources.FeverishExplanationHotTemperature + temperatureLabel,
                                                            attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53822);
                                                }
                                                else if (temperature >= warmTemperatureThreshold && !ignoreWarm && reportWarnings)
                                                {
                                                    guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                    ClearDiskConsecutiveOverheatCount(diskKey);
                                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] HDD Temperature Warm");

                                                    feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusTemperatureWarm,
                                                             Properties.Resources.FeverishExplanationWarmTemperature + temperatureLabel,
                                                             attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53823);
                                                }
                                                else
                                                {
                                                    guid = (String)diskKey.GetValue("NotificationGuidTemperature");
                                                    ClearDiskConsecutiveOverheatCount(diskKey);
                                                    isWarningTemperature = false;
                                                    isCriticalTemperature = false;
                                                    if (feverishDisks.ItemExists(guid))
                                                    {
                                                        feverishDisks.RemoveItem(guid);
                                                        SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] HDD Temperature OK; cleared alert.");
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            break;
                                        }
                                }
                            }
                            catch (Exception ex)
                            {
                                WindowsEventLogger.LogError(ex.Message, 53891, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
                            }
                        }

                        // Compare SMART values to thresholds -- these apply to HDDs and SSDs (except Geriatric).
                        if (!isSsd && threshold == 0)
                        {
                            if (smartValue == threshold)
                            {
                                SiAuto.Main.LogColored(System.Drawing.Color.DeepPink, "[Health Check] Disk is Geriatric.");
                                isDiskGeriatric = true;
                            }
                            else
                            {
                                // Healthy and nothing to do.
                            }
                        }
                        else // Threshold greater than zero, so these can "fail" even if advisory.
                        {
                            // Health Item
                            if (smartValue < threshold && reportCritical)
                            {
                                isDiskCritical = true;
                                guid = (String)diskKey.GetValue("NotificationGuidThreshold");
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] Disk is Critical; SMART Threshold Exceeded (TEC).");
                                isThresholdViolated = true;

                                if (attributeID == 190 || attributeID == 194 || attributeID == 231)
                                {
                                    SiAuto.Main.LogError("Disk hit TEC on attribute 190/194/231 - temperature.");
                                    isHotTec = true;
                                }
                                else
                                {
                                    SiAuto.Main.LogFatal("Disk hit TEC on non-temperature.");
                                    isDiskTec = true;
                                }

                                // Add/Update the item -- AddItem will decide if it needs to change.
                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusThresholdExceeded,
                                    Properties.Resources.FeverishExplanationThresholdExceeded, attributeID, attributeName, true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53853);
                            }
                            else if (smartValue == threshold && reportWarnings && !isThresholdViolated && !(isSsd && threshold == 0)) // threshold violation takes precedence
                            {
                                // We don't count a zero threshold met on an SSD.
                                isDiskWarning = true;
                                guid = (String)diskKey.GetValue("NotificationGuidThreshold");
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] Disk is Warning; SMART Threshold Met.");
                                isThresholdMet = true;

                                feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusThresholdMet,
                                        Properties.Resources.FeverishExplanationThresholdMet, attributeID, attributeName, false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53854);
                            }
                            else
                            {

                            }
                        }
                    }

                    // END OF THE FOR LOOP - post processing below

                    // No threshold violations? We can clear them.
                    if (!isThresholdViolated && !isThresholdMet)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidThreshold");
                        if (feverishDisks.ItemExists(guid))
                        {
                            feverishDisks.RemoveItem(guid);
                            SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] Threshold violation has ben cleared.");
                        }
                    }

                    // The array was full of zeros, which indicates an Unknown disk.
                    if (attributesProcessed == 0)
                    {
                        WindowsEventLogger.LogWarning(
                                    "Disk: " + diskName + ", Path: " + diskPath + ", GUID: " + guid +
                                    " did not return any valid SMART data. The disk health is unknown.", 53852);
                        diskKey.SetValue("IsDiskUnknown", true);
                        diskKey.Close();
                        SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] Zero attributes processed (array was all zeros). This is an Unknown disk; setting IsDiskUnknown to true.");
                        continue;
                    }
                    else
                    {
                        diskKey.SetValue("IsDiskUnknown", false);
                    }

                    // Get the Ignored item counts.
                    long ignoredBadSectors = 0;
                    long ignoredEndToEndErrors = 0;
                    long ignoredOfflineBadSectors = 0;
                    long ignoredPendingSectors = 0;
                    long ignoredReallocations = 0;
                    long ignoredSpinRetries = 0;
                    long ignoredCrcErrors = 0;

                    try
                    {
                        ignoredBadSectors = long.Parse((String)diskKey.GetValue("BadSectorsIgnored"));
                    }
                    catch
                    {
                        ignoredBadSectors = 0;
                    }
                    try
                    {
                        ignoredEndToEndErrors = long.Parse((String)diskKey.GetValue("EndToEndErrorsIgnored"));
                    }
                    catch
                    {
                        ignoredEndToEndErrors = 0;
                    }
                    try
                    {
                        ignoredOfflineBadSectors = long.Parse((String)diskKey.GetValue("OfflineBadSectorsIgnored"));
                    }
                    catch
                    {
                        ignoredOfflineBadSectors = 0;
                    }
                    try
                    {
                        ignoredPendingSectors = long.Parse((String)diskKey.GetValue("PendingSectorsIgnored"));
                    }
                    catch
                    {
                        ignoredPendingSectors = 0;
                    }
                    try
                    {
                        ignoredReallocations = long.Parse((String)diskKey.GetValue("ReallocationEventsIgnored"));
                    }
                    catch
                    {
                        ignoredReallocations = 0;
                    }
                    try
                    {
                        ignoredSpinRetries = long.Parse((String)diskKey.GetValue("SpinRetriesIgnored"));
                    }
                    catch
                    {
                        ignoredSpinRetries = 0;
                    }
                    try
                    {
                        ignoredCrcErrors = long.Parse((String)diskKey.GetValue("CrcErrorsIgnored"));
                    }
                    catch
                    {
                        ignoredCrcErrors = 0;
                    }

                    SiAuto.Main.LogLong("ignoredBadSectors", ignoredBadSectors);
                    SiAuto.Main.LogLong("ignoredEndToEndErrors", ignoredEndToEndErrors);
                    SiAuto.Main.LogLong("ignoredOfflineBadSectors", ignoredOfflineBadSectors);
                    SiAuto.Main.LogLong("ignoredPendingSectors", ignoredPendingSectors);
                    SiAuto.Main.LogLong("ignoredReallocations", ignoredReallocations);
                    SiAuto.Main.LogLong("ignoredSpinRetries", ignoredSpinRetries);
                    SiAuto.Main.LogLong("ignoredBadSectors", ignoredCrcErrors);
                    
                    // Set Critical/Warning flag based on values of the Super Critical attributes;
                    // this is irrespective of the thresholds.
                    
                    // Super Criticals that trigger CRITICAL status.
                    if (isSsd)
                    {
                        if (badSectorCountOrRetiredBlockCount >= ssdRetirementCritical && badSectorCountOrRetiredBlockCount > ignoredBadSectors)
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] Setting SSD to Critical. Basis: super criticals, retired sectors.");
                            SiAuto.Main.LogInt("badSectorCountOrRetiredBlockCount", badSectorCountOrRetiredBlockCount);
                            SiAuto.Main.LogInt("ssdRetirementCritical", ssdRetirementCritical);
                            isDiskCritical = true;
                        }
                    }
                    else
                    {
                        if ((badSectorCountOrRetiredBlockCount >= 50 && badSectorCountOrRetiredBlockCount > ignoredBadSectors) ||
                            (endToEndErrorCountOrLifeCurve > 5 && endToEndErrorCountOrLifeCurve > ignoredEndToEndErrors) ||
                            (reallocationEventCount >= 50 && reallocationEventCount > ignoredReallocations) ||
                            (pendingSectorCountOrSsdLifeLeft >= 30 && pendingSectorCountOrSsdLifeLeft > ignoredPendingSectors) ||
                            (uncorrectableSectorCount >= 50 && uncorrectableSectorCount > ignoredOfflineBadSectors) ||
                            (spinRetryCount >= 8 && spinRetryCount > ignoredSpinRetries))
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Health Check] Setting HDD to Critical. Basis: super criticals, critical counts.");
                            SiAuto.Main.LogInt("badSectorCountOrRetiredBlockCount", badSectorCountOrRetiredBlockCount);
                            SiAuto.Main.LogInt("endToEndErrorCountOrLifeCurve", endToEndErrorCountOrLifeCurve);
                            SiAuto.Main.LogInt("reallocationEventCount", reallocationEventCount);
                            SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            SiAuto.Main.LogInt("uncorrectableSectorCount", uncorrectableSectorCount);
                            SiAuto.Main.LogInt("spinRetryCount", spinRetryCount);
                            isDiskCritical = true;
                        }
                    }

                    // Super Criticals that trigger WARNING status.
                    if (isSsd)
                    {
                        if (badSectorCountOrRetiredBlockCount >= ssdRetirementWarning && badSectorCountOrRetiredBlockCount > ignoredBadSectors)
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] Setting SSD to Warning. Basis: super criticals, retired sectors.");
                            SiAuto.Main.LogInt("badSectorCountOrRetiredBlockCount", badSectorCountOrRetiredBlockCount);
                            SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                            isDiskWarning = true;
                        }
                    }
                    else
                    {
                        if ((badSectorCountOrRetiredBlockCount > 0 && badSectorCountOrRetiredBlockCount > ignoredBadSectors) ||
                            (endToEndErrorCountOrLifeCurve > 0 && endToEndErrorCountOrLifeCurve > ignoredEndToEndErrors) ||
                            (reallocationEventCount > 0 && reallocationEventCount > ignoredReallocations) ||
                            (pendingSectorCountOrSsdLifeLeft > 0 && pendingSectorCountOrSsdLifeLeft > ignoredPendingSectors) ||
                            (uncorrectableSectorCount > 0 && uncorrectableSectorCount > ignoredOfflineBadSectors) ||
                            (spinRetryCount >= 3 && spinRetryCount > ignoredSpinRetries))
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] Setting HDD to Warning. Basis: super criticals, warning counts.");
                            SiAuto.Main.LogInt("badSectorCountOrRetiredBlockCount", badSectorCountOrRetiredBlockCount);
                            SiAuto.Main.LogInt("endToEndErrorCountOrLifeCurve", endToEndErrorCountOrLifeCurve);
                            SiAuto.Main.LogInt("reallocationEventCount", reallocationEventCount);
                            SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            SiAuto.Main.LogInt("uncorrectableSectorCount", uncorrectableSectorCount);
                            SiAuto.Main.LogInt("spinRetryCount", spinRetryCount);
                            isDiskWarning = true;
                        }
                    }

                    // This one isn't a super-critical but these can lead to corrruption and thus are
                    // reported.
                    if (isSsd)
                    {
                    }
                    else
                    {
                        if (ultraAtaCrcErrorCount > 50 && ultraAtaCrcErrorCount > ignoredCrcErrors)
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] Setting HDD to Warning. Basis: high CRC errors.");
                            SiAuto.Main.LogInt("ultraAtaCrcErrorCount", ultraAtaCrcErrorCount);
                            isDiskWarning = true;
                        }
                    }

                    // Set an alert of the disk health. We only report one.  If the disk is critical,
                    // warning and geriatric, the Critical flag is the most severe.  There's no point
                    // in alerting to the lesser items.  If the disk is only Geriatric (not critical or
                    // warning), then we can just report the disk as being a geezer.
                    if (isDiskCritical && reportCritical)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidCriticalHealth");
                        diskKey.SetValue("IsDiskCritical", true);
                        
                        SiAuto.Main.LogMessage("[Health Check] Raising Critical alert.");

                        feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusDiskCritical,
                                Properties.Resources.FeverishExplanationHealthCritical,
                                attributeID, "Critical Disk", true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53824);

                        if (isDiskWarning)
                        {
                            SiAuto.Main.LogMessage("[Health Check] Clearing Warning alert since disk is now Critical.");
                            // No need to inform user that the disk has a warning state when the disk is Critical.
                            guid = (String)diskKey.GetValue("NotificationGuidWarningHealth");
                            if (feverishDisks.ItemExists(guid))
                            {
                                feverishDisks.RemoveItem(guid);
                            }
                        }
                    }
                    else
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidCriticalHealth");
                        diskKey.SetValue("IsDiskCritical", false);
                        SiAuto.Main.LogMessage("[Health Check] Disk is not critical; if an active critical alert exists it will be cleared.");
                        if (feverishDisks.ItemExists(guid))
                        {
                            feverishDisks.RemoveItem(guid);
                        }
                    }

                    if (isDiskWarning && !isDiskCritical && reportWarnings)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidWarningHealth");
                        diskKey.SetValue("IsDiskWarning", true);
                        
                        SiAuto.Main.LogMessage("[Health Check] Raising Warning alert.");

                        feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusDiskWarning,
                                 Properties.Resources.FeverishExplanationHealthWarning,
                                 attributeID, "Warning Disk", false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53825);
                    }
                    else
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidWarningHealth");
                        diskKey.SetValue("IsDiskWarning", false);
                        SiAuto.Main.LogMessage("[Health Check] Disk is not in warning state; if an active warning alert exists it will be cleared.");
                        if (feverishDisks.ItemExists(guid))
                        {
                            feverishDisks.RemoveItem(guid);
                        }
                    }

                    // If the disk isn't critical or warning, but the temperatures are, we want to set
                    // the flags so this appears in the UI. We also clear the Geriatric flag (false) because
                    // warning/critical temperatures should take precedence.
                    if (!isDiskCritical && !isDiskWarning && (isCriticalTemperature || isWarningTemperature))
                    {
                        if (isCriticalTemperature)
                        {
                            SiAuto.Main.LogMessage("[Health Check] Disk is not critical, but excessive temperature has resulted in critical state.");
                            diskKey.SetValue("IsDiskCritical", true);
                        }
                        else
                        {
                            SiAuto.Main.LogMessage("[Health Check] Disk is not warning; but excessive temperature has resulted in warning state.");
                            diskKey.SetValue("IsDiskWarning", true);
                        }
                        diskKey.SetValue("IsDiskGeriatric", false);
                    }

                    if (isDiskGeriatric && !isDiskCritical && reportGeriatric)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidGeezer");
                        diskKey.SetValue("IsDiskGeriatric", true);
                        
                        SiAuto.Main.LogMessage("[Health Check] Disk is Geriatric (on one or more values). No prevailing Critical or Warning exists; disk overall health is Geriatric.");

                        feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusDiskGeezer,
                                Properties.Resources.FeverishExplanationHealthGeriatric,
                                attributeID, "Geriatric Disk", false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53826);
                    }
                    else
                    {
                        SiAuto.Main.LogMessage("[Health Check] Disk is not geriatric; if an active geriatric alert exists it will be cleared.");
                        guid = (String)diskKey.GetValue("NotificationGuidGeezer");
                        diskKey.SetValue("IsDiskGeriatric", false);
                        if (feverishDisks.ItemExists(guid))
                        {
                            feverishDisks.RemoveItem(guid);
                        }
                    }

                    // Save the temperature so it can be easily fetched and shown in the UI.
                    SiAuto.Main.LogInt("temperature", temperature);
                    SiAuto.Main.LogInt("airflowTemperature", airflowTemperature);
                    if (temperature == 0 && airflowTemperature == 0)
                    {
                        SiAuto.Main.LogWarning("Both temperature and airflow temperature are reporting zero; logging temperature value as zero.");
                        diskKey.SetValue("CelsiusTemperature", 0);
                    }
                    else if (temperature == 0)
                    {
                        SiAuto.Main.LogMessage("Temperature shows zero but airflow temperature reports valid; logging the airflow temperature.");
                        diskKey.SetValue("CelsiusTemperature", airflowTemperature);
                    }
                    else // (airflowTemperature == 0)
                    {
                        SiAuto.Main.LogMessage("Temperature value is valid so logging.");
                        diskKey.SetValue("CelsiusTemperature", temperature);
                    }

                    // REPORTING PRECEDENCE:
                    // 1. Exceeded health thresholds (FAIL)
                    // 2. Bad Sectors, End-to-End Errors, Pending Sectors, Reallocation Events, Uncorrectable Sectors, Spin Retry
                    // 2a. Same as 2, except at "warning" level
                    // 3. Ultra ATA CRC Errors
                    // 4. Temperature
                    // 5. Geriatrics

                    // The SERIOUS Stuff is evaluated first, with an else/if for the warning.  These need to be done
                    // together in else/if; otherwise an item could get added in the critical and subsequently culled
                    // in the warnings because the warning condition on the ignore is not met.
                    if ((isSsd && badSectorCountOrRetiredBlockCount >= ssdRetirementCritical && badSectorCountOrRetiredBlockCount > ignoredBadSectors && reportCritical) ||
                        (!isSsd && badSectorCountOrRetiredBlockCount >= 50 && badSectorCountOrRetiredBlockCount > ignoredBadSectors && reportCritical))
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidBadSectors");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating critical alert for Bad or Retired Sectors.");

                        if (isSsd)
                        {
                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusCriticalBadSectorsSsd,
                                "There are " + badSectorCountOrRetiredBlockCount.ToString() + " retired blocks in the SSD flash memory. Their contents have " +
                                "been reallocated to the spare area.",
                                5, "Retired Sector Count", true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53830);
                        }
                        else
                        {
                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusCriticalBadSectors,
                                "There are " + badSectorCountOrRetiredBlockCount.ToString() + " bad sectors on the disk surface. Their contents have " +
                                "been reallocated to the spare area.",
                                5, "Reallocated Sector Count", true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53830);
                        }
                    }
                    else if ((isSsd && badSectorCountOrRetiredBlockCount >= ssdRetirementWarning && badSectorCountOrRetiredBlockCount < ssdRetirementCritical && badSectorCountOrRetiredBlockCount > ignoredBadSectors && reportWarnings) ||
                        (!isSsd && badSectorCountOrRetiredBlockCount > 0 && badSectorCountOrRetiredBlockCount < 50 && badSectorCountOrRetiredBlockCount > ignoredBadSectors && reportWarnings))
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidBadSectors");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating warning alert for Bad or Retired Sectors.");

                        if (isSsd)
                        {
                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusWarningBadSectorsSsd,
                                "There are " + badSectorCountOrRetiredBlockCount.ToString() + " retired blocks in the SSD flash memory. Their contents have " +
                                "been reallocated to the spare area.",
                                5, "Retired Sector Count", false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53831);
                        }
                        else
                        {
                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusWarningBadSectors,
                                (badSectorCountOrRetiredBlockCount == 1 ? "There is " + badSectorCountOrRetiredBlockCount.ToString() + " bad sector on " +
                                "the disk surface. Its contents have been reallocated to the spare area." :
                                "There are " + badSectorCountOrRetiredBlockCount.ToString() + " bad sectors on " +
                                "the disk surface. Their contents have been reallocated to the spare area."),
                                5, "Reallocated Sector Count", false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53831);
                        }
                    }
                    else
                    {
                        SiAuto.Main.LogMessage("[Health Check] Clearing alert for Bad or Retired Sectors.");
                        guid = (String)diskKey.GetValue("NotificationGuidBadSectors");
                        if (feverishDisks.ItemExists(guid))
                        {
                            feverishDisks.RemoveItem(guid);
                        }
                    }

                    if ((!isSsd || (isSsd && (manufacturer == SsdManufacturer.SSD_MANUFACTURER_INTEL || manufacturer == SsdManufacturer.SSD_MANUFACTURER_STEC || manufacturer == SsdManufacturer.SSD_MANUFACTURER_OTHER)))
                        && endToEndErrorCountOrLifeCurve >= 5 && endToEndErrorCountOrLifeCurve > ignoredEndToEndErrors && reportCritical)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidEndToEndErrors");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating critical alert for End-to-End Errors.");

                        feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusCriticalEndToEndErrors,
                                Properties.Resources.FeverishExplanationCriticalEndToEndErrors,
                                184, "End-to-End Error", true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53832);
                    }
                    else if ((!isSsd || (isSsd && (manufacturer == SsdManufacturer.SSD_MANUFACTURER_INTEL || manufacturer == SsdManufacturer.SSD_MANUFACTURER_STEC || manufacturer == SsdManufacturer.SSD_MANUFACTURER_OTHER)))
                        && endToEndErrorCountOrLifeCurve > 0 && endToEndErrorCountOrLifeCurve < 5 && endToEndErrorCountOrLifeCurve > ignoredEndToEndErrors && reportWarnings)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidEndToEndErrors");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating warning alert for End-to-End Errors.");

                        feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusWarningEndToEndErrors,
                                Properties.Resources.FeverishExplanationWarningEndToEndErrors,
                                184, "End-to-End Error", false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53833);
                    }
                    else if (isSsd && manufacturer == SsdManufacturer.SSD_MANUFACTURER_SAMSUNG && endToEndErrorCountOrLifeCurve >= ssdRetirementCritical && endToEndErrorCountOrLifeCurve > ignoredEndToEndErrors)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidEndToEndErrors");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating critical alert for Runtime Bad Block.");

                        feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusCriticalRuntimeBadBlock,
                                Properties.Resources.FeverishExplanationCriticalRuntimeBadBlock,
                                183, "Runtime Bad Block", true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53846);
                    }
                    else if (isSsd && manufacturer == SsdManufacturer.SSD_MANUFACTURER_SAMSUNG && endToEndErrorCountOrLifeCurve >= ssdRetirementWarning && endToEndErrorCountOrLifeCurve > ignoredEndToEndErrors && reportWarnings)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidEndToEndErrors");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating warning alert for Runtime Bad Block.");

                        feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusWarningRuntimeBadBlocks,
                                Properties.Resources.FeverishExplanationWarningRuntimeBadBlock,
                                183, "Runtime Bad Block", false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53847);
                    }
                    else if (!isSsd || (isSsd && endToEndErrorCountOrLifeCurve > 90))
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidEndToEndErrors");

                        SiAuto.Main.LogMessage("[Health Check] Clearing alert for End-to-End Errors.");

                        if (feverishDisks.ItemExists(guid))
                        {
                            feverishDisks.RemoveItem(guid);
                        }
                    }

                    if (isSsd && manufacturer == SsdManufacturer.SSD_MANUFACTURER_JMICRON && reallocationEventCount >= ssdRetirementCritical && reallocationEventCount > ignoredReallocations)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidReallocationEvents");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating critical alert for Write Errors.");

                        feverishDisks.AddItem(guid, diskName, diskPath, "High Write Error Count",
                                "There have been " + reallocationEventCount.ToString() +
                                    " write errors detected by the SSD controller.",
                                196, "Bad Cluster Table Count (ECC Fail Count)", true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53844);
                    }
                    else if (isSsd && manufacturer == SsdManufacturer.SSD_MANUFACTURER_JMICRON && reallocationEventCount >= ssdRetirementWarning && reallocationEventCount > ignoredReallocations)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidReallocationEvents");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating warning alert for Write Errors.");

                        feverishDisks.AddItem(guid, diskName, diskPath, "Write Errors Detected",
                                "There have been " + reallocationEventCount.ToString() +
                                    " write errors detected by the SSD controller.",
                                196, "Bad Cluster Table Count (ECC Fail Count)", false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53845);
                    }
                    else if ((isSsd && reallocationEventCount >= ssdRetirementCritical && reallocationEventCount > ignoredReallocations && reportCritical) ||
                        (!isSsd && reallocationEventCount >= 50 && reallocationEventCount > ignoredReallocations && reportCritical))
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidReallocationEvents");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating critical alert for Reallocation Events.");

                        feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusCriticalReallocations,
                                "There have been " + reallocationEventCount.ToString() +
                                    " attempted sector reallocation events on the disk.",
                                196, "Reallocation Event Count", true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53834);
                    }
                    else if ((isSsd && reallocationEventCount >= ssdRetirementWarning && reallocationEventCount < ssdLifeLeftCritical && reallocationEventCount > ignoredReallocations && reportWarnings) ||
                        (!isSsd && reallocationEventCount > 0 && reallocationEventCount < 50 && reallocationEventCount > ignoredReallocations && reportWarnings))
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidReallocationEvents");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating warning alert for Reallocation Events.");

                        feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusWarningReallocations,
                                (reallocationEventCount == 1 ? "There has been " + reallocationEventCount.ToString() +
                                " attempted sector reallocation event on the disk." : "There have been " + reallocationEventCount.ToString() +
                                " attempted sector reallocation events on the disk."),
                                196, "Reallocation Event Count", false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53835);
                    }
                    else
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidReallocationEvents");

                        SiAuto.Main.LogMessage("[Health Check] Clearing alert for Reallocation Events.");

                        if (feverishDisks.ItemExists(guid))
                        {
                            feverishDisks.RemoveItem(guid);
                        }
                    }

                    if ((isSsd && (manufacturer == SsdManufacturer.SSD_MANUFACTURER_JMICRON || manufacturer == SsdManufacturer.SSD_MANUFACTURER_OTHER) && pendingSectorCountOrSsdLifeLeft >= ssdRetirementCritical && pendingSectorCountOrSsdLifeLeft > ignoredPendingSectors && reportCritical) ||
                        (!isSsd && pendingSectorCountOrSsdLifeLeft >= 30 && pendingSectorCountOrSsdLifeLeft > ignoredPendingSectors && reportCritical))
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating critical alert for Pending Sectors.");

                        feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusCriticalPendingSectors,
                                "There are " + pendingSectorCountOrSsdLifeLeft.ToString() + " unstable sectors that may be reallocated.",
                                197, "Current Pending Sector Count", true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53836);
                    }
                    else if ((isSsd && (manufacturer == SsdManufacturer.SSD_MANUFACTURER_JMICRON || manufacturer == SsdManufacturer.SSD_MANUFACTURER_OTHER) && pendingSectorCountOrSsdLifeLeft >= ssdRetirementWarning && pendingSectorCountOrSsdLifeLeft > ignoredPendingSectors && reportWarnings) ||
                        (!isSsd && pendingSectorCountOrSsdLifeLeft > 0 && pendingSectorCountOrSsdLifeLeft < 30 && pendingSectorCountOrSsdLifeLeft > ignoredPendingSectors && reportWarnings))
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating warning alert for Pending Sectors.");

                        feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusWarningPendingSectors,
                                (pendingSectorCountOrSsdLifeLeft == 1 ? "There is " + pendingSectorCountOrSsdLifeLeft.ToString() + " unstable sector " +
                                "that is waiting to be reallocated." : "There are " + pendingSectorCountOrSsdLifeLeft.ToString() + " sectors " +
                                "that are waiting to be reallocated."),
                                197, "Current Pending Sector Count", false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53837);
                    }
                    else if (!isSsd && pendingSectorCountOrSsdLifeLeft == 0)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");

                        SiAuto.Main.LogMessage("[Health Check] Clearing alert for Pending Sectors.");

                        if (feverishDisks.ItemExists(guid))
                        {
                            feverishDisks.RemoveItem(guid);
                        }
                    }
                    else if (!isSsd)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidPendingSectors");

                        SiAuto.Main.LogMessage("[Health Check] Clearing alert for Pending Sectors.");

                        if (feverishDisks.ItemExists(guid))
                        {
                            feverishDisks.RemoveItem(guid);
                        }
                    }

                    if (!isSsd && pendingSectorCountOrSsdLifeLeft < ignoredPendingSectors)
                    {
                        // The pending sector count can go down, so adjust the ignored number.
                        SiAuto.Main.LogMessage("[Health Check] Pending Sectors has gone done; adjusting ignored event counter. Old count = " + ignoredPendingSectors.ToString() + "; new count = " +
                            pendingSectorCountOrSsdLifeLeft.ToString());
                        ignoredPendingSectors = (long)pendingSectorCountOrSsdLifeLeft;
                        diskKey.SetValue("PendingSectorsIgnored", ignoredPendingSectors);
                    }

                    if (!isSsd && uncorrectableSectorCount >= 50 && uncorrectableSectorCount > ignoredOfflineBadSectors && reportCritical)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidUncorrectableSectors");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating critical alert for Offline Bad Sectors.");

                        feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusCriticalUncorrectableSectors,
                                "The disk detected " + uncorrectableSectorCount.ToString() + " bad sectors during its offline scan.",
                                198, "Offline Uncorrectable Sector Count", true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53838);
                    }
                    else if (!isSsd && uncorrectableSectorCount > 0 && uncorrectableSectorCount < 50 && uncorrectableSectorCount > ignoredOfflineBadSectors && reportWarnings)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidUncorrectableSectors");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating warning alert for Offline Bad Sectors.");

                        feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusWarningUncorrectableSectors,
                                (uncorrectableSectorCount == 1 ? "The disk detected " + uncorrectableSectorCount.ToString() +
                                " bad sector during its offline scan." : "The disk detected " + uncorrectableSectorCount.ToString() +
                                " bad sectors during its offline scan."),
                                198, "Offline Uncorrectable Sector Count", false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53839);
                    }
                    else
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidUncorrectableSectors");

                        SiAuto.Main.LogMessage("[Health Check] Clearing alert for Offline Bad Sectors.");

                        if (feverishDisks.ItemExists(guid))
                        {
                            feverishDisks.RemoveItem(guid);
                        }
                    }

                    if ((!isSsd || (isSsd && manufacturer == SsdManufacturer.SSD_MANUFACTURER_JMICRON)) && spinRetryCount >= 8 && spinRetryCount > ignoredSpinRetries && reportCritical)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidSpinRetries");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating critical alert for Spin Retries.");

                        if (isSsd && manufacturer == SsdManufacturer.SSD_MANUFACTURER_JMICRON)
                        {
                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusCriticalSpinRetries,
                                "The Solid State Disk encountered a startup failure " + spinRetryCount.ToString() + " times. While SSDs have " +
                                "no moving parts, this indicates an excessive power draw by the SSD or a failure of the SSD's onboard power supply.",
                                10, "Spin Retry Count", true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53840);
                        }
                        else
                        {
                            feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusCriticalSpinRetries,
                                    "The drive motor has encountered difficulty spinning up " + spinRetryCount.ToString() + " times. " +
                                    "This may indicate a failing drive motor or weak power supply.",
                                    10, "Spin Retry Count", true, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53840);
                        }
                    }
                    else if (!isSsd && spinRetryCount > 3 && spinRetryCount < 8 && spinRetryCount > ignoredSpinRetries && reportWarnings)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidSpinRetries");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating warning alert for Spin Retries.");

                        feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusWarningSpinRetries,
                                "The drive motor has encountered difficulty spinning up " + spinRetryCount.ToString() + " times. " +
                                "This may indicate a failing drive motor or weak power supply.",
                                10, "Spin Retry Count", false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53841);
                    }
                    else
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidSpinRetries");
                        if (feverishDisks.ItemExists(guid))
                        {
                            SiAuto.Main.LogMessage("[Health Check] Clearing alert for Spin Retries.");
                            feverishDisks.RemoveItem(guid);
                        }
                    }

                    // WARNINGS

                    if (!isSsd && ultraAtaCrcErrorCount > 50 && ultraAtaCrcErrorCount > ignoredCrcErrors && reportWarnings)
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidCrcErrors");
                        
                        SiAuto.Main.LogMessage("[Health Check] Generating warning alert for CRC Errors.");

                        feverishDisks.AddItem(guid, diskName, diskPath, Properties.Resources.FeverishSmartStatusWarningCrcErrors,
                                "There have been " + ultraAtaCrcErrorCount.ToString() +
                                " CRC errors detected on the disk. These are usually caused by a bad or oxidized data cable " +
                                "(or connectors), and in some cases may be caused by faulty USB bridge chips in external enclosures. " +
                                "These errors do not usually indicate a drive failure, but they can result in data corruption. Check " +
                                "the cables and connectors, and also verify the bridge chip is supported in this version of Windows.",
                                199, "Ultra DMA CRC Error Count", false, feverishDisks.ItemExists(guid) ? AlertAction.Update : AlertAction.Insert, 53842);
                    }
                    else
                    {
                        guid = (String)diskKey.GetValue("NotificationGuidCrcErrors");

                        SiAuto.Main.LogMessage("[Health Check] Clearing alert for CRC Errors.");

                        if (feverishDisks.ItemExists(guid))
                        {
                            feverishDisks.RemoveItem(guid);
                        }
                    }

                    // TODO: TEC - backup section
                    // If TEC and backup allowed (either all TEC or non-temperature TEC) then check to see if a backup was already invoked by this drive.
                    // If no backup invoked, we set the flag to trigger a backup.
                    try
                    {
                        SiAuto.Main.LogMessage("Setting temperature-related TEC flag to " + isHotTec.ToString().ToUpper() + ".");
                        diskKey.SetValue("HotTec", isHotTec);
                        SiAuto.Main.LogMessage("Set temperature-related TEC flag.");
                    }
                    catch (Exception ex)
                    {
                        SiAuto.Main.LogError("Failed to set temperature-related TEC flag: " + ex.Message);
                        SiAuto.Main.LogException(ex);
                        WindowsEventLogger.LogError("Disk " + diskName + ", path " + diskPath + " - failed to set the hot TEC flag in the Registry to " +
                            isHotTec.ToString().ToUpper() + ". " + ex.Message, 53894, Properties.Resources.EventLogJoshua);
                    }

                    try
                    {
                        SiAuto.Main.LogMessage("Setting general TEC flag to " + isDiskTec.ToString().ToUpper() + ".");
                        diskKey.SetValue("DiskTec", isDiskTec);
                        SiAuto.Main.LogMessage("Set general TEC flag.");
                    }
                    catch (Exception ex)
                    {
                        SiAuto.Main.LogError("Failed to set temperature-related TEC flag: " + ex.Message);
                        SiAuto.Main.LogException(ex);
                        WindowsEventLogger.LogError("Disk " + diskName + ", path " + diskPath + " - failed to set the general TEC flag in the Registry to " +
                            isDiskTec.ToString().ToUpper() + ". " + ex.Message, 53895, Properties.Resources.EventLogJoshua);
                    }
                    diskKey.Close();
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogFatal("[CheckDiskHealth] Operation failed.");
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.CheckDiskHealth");
            return;
        }

        private static void IncrementDiskConsecutiveOverheatCount(Microsoft.Win32.RegistryKey diskKey)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.DiskEnumerator.IncrementDiskConsecutiveOverheatCount");
            try
            {
                int overheatCount = 0;
                overheatCount = (int)diskKey.GetValue("ConsecutiveOverheatCount");
                SiAuto.Main.LogInt("Last overheat count", overheatCount);
                overheatCount++;
                SiAuto.Main.LogInt("New overheat count", overheatCount);
                diskKey.SetValue("ConsecutiveOverheatCount", overheatCount);
                SiAuto.Main.LogMessage("New overheat count committed to the Registry.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogWarning("Last overheat count appears to be corrupt (maybe it was never set before?); resetting to 1 (since disk is overheated at this point). " + ex.Message);
                SiAuto.Main.LogInt("New overheat count", 1);
                try
                {
                    diskKey.SetValue("ConsecutiveOverheatCount", 1);
                    SiAuto.Main.LogMessage("New overheat count committed to the Registry.");

                }
                catch (Exception ex2)
                {
                    SiAuto.Main.LogError("Failed to set the new overheat count: " + ex2.Message);
                    SiAuto.Main.LogException(ex2);
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.IncrementDiskConsecutiveOverheatCount");
        }

        private static void ClearDiskConsecutiveOverheatCount(Microsoft.Win32.RegistryKey diskKey)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.DiskEnumerator.ClearDiskConsecutiveOverheatCount");
            try
            {
                SiAuto.Main.LogInt("New overheat count", 0);
                diskKey.SetValue("ConsecutiveOverheatCount", 0);
                SiAuto.Main.LogMessage("New overheat count committed to the Registry.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to set the new overheat count: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.ClearDiskConsecutiveOverheatCount");
        }

        private static String ConcatenateRawData(int rawHigh, int raw4, int raw3, int raw2, int raw1, int rawLow)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.DiskEnumerator.ConcatenateRawData");
            String rawDataString = (rawHigh.ToString("X").Length == 1 ? "0" + rawHigh.ToString("X") : rawHigh.ToString("X"))
                        + (raw4.ToString("X").Length == 1 ? "0" + raw4.ToString("X") : raw4.ToString("X"))
                        + (raw3.ToString("X").Length == 1 ? "0" + raw3.ToString("X") : raw3.ToString("X"))
                        + (raw2.ToString("X").Length == 1 ? "0" + raw2.ToString("X") : raw2.ToString("X"))
                        + (raw1.ToString("X").Length == 1 ? "0" + raw1.ToString("X") : raw1.ToString("X"))
                        + (rawLow.ToString("X").Length == 1 ? "0" + rawLow.ToString("X") : rawLow.ToString("X"));
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.ConcatenateRawData");
            return rawDataString;
        }

        /// <summary>
        /// Culls the DevicePath information from each disk in the Registry.  This makes each disk "stale."
        /// When the service starts, only connected disks will have this field repopulated, and hence they
        /// are not stale.
        /// </summary>
        public static void TearDown()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.DiskEnumerator.TearDown");
            SiAuto.Main.LogMessage("[Teardown] Teardown signaled; demolition started.");
            try
            {
                SiAuto.Main.LogMessage("[Teardown] Acquire Registry objects.");
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);
                SiAuto.Main.LogMessage("[Teardown] Acquired Registry objects.");
                SiAuto.Main.LogMessage("[Teardown] Demolition in progress.");

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    try
                    {
                        Microsoft.Win32.RegistryKey diskKey;
                        diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);
                        SiAuto.Main.LogMessage("[Teardown] Demolishing " + diskKeyName + " - setting object to stale.");
                        diskKey.SetValue("DevicePath", String.Empty);
                        diskKey.SetValue("Name", String.Empty);
                        SiAuto.Main.LogMessage("[Teardown] Demolishing " + diskKeyName + " - setting consecutive overheat count to zero.");
                        diskKey.SetValue("ConsecutiveOverheatCount", 0);
                        SiAuto.Main.LogMessage("[Teardown] Demolished " + diskKeyName + " - set to stale. The disk will be marked active the next time the service is " +
                            "started if the disk is present in the Server.");
                    }
                    catch (Exception ex)
                    {
                        SiAuto.Main.LogError("[Teardown] Demoliton of " + diskKeyName + " failed. " + ex.Message);
                        SiAuto.Main.LogException(ex);
                        WindowsEventLogger.LogWarning("Demolition of " + diskKeyName + " failed: " + ex.Message, 53867);
                    }
                }
                SiAuto.Main.LogMessage("[Teardown] Demolition complete.");
            }
            catch (Exception ex)
            {
                WindowsEventLogger.LogWarning("Demolition failed: " + ex.Message, 53868);
            }
            SiAuto.Main.LogMessage("[Teardown] Teardown of DiskEnumerator is complete.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.TearDown");
        }

        public static String GetAttributeName(DataRow[] smartRows)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.DiskEnumerator.GetAttributeName");
            if (smartRows != null && smartRows.Length > 0)
            {
                DataRow smartRow = smartRows[0];
                try
                {
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.GetAttributeName");
                    return smartRow["AttributeName"].ToString();
                }
                catch
                {
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.GetAttributeName");
                    return String.Empty;
                }
            }
            else
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.GetAttributeName");
                return String.Empty;
            }
        }

        public static void IgnoreDisk(String disk, String model)
        {
            try
            {
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);
                    bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                    if (!activeFlag)
                    {
                        // Inactive/Stale disk.
                        continue;
                    }

                    if (String.Compare(disk, (String)diskKey.GetValue("DevicePath").ToString(), true) == 0 &&
                        String.Compare(disk, (String)diskKey.GetValue("Name").ToString(), true) == 0 &&
                        (String.Compare(model, (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model, (String)diskKey.GetValue("PreferredModel"), true) == 0 ||
                        String.Compare(model.Trim(), (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model.Trim(), (String)diskKey.GetValue("PreferredModel"), true) == 0 ||
                        (model.Contains("Unrecognized") && model.Contains("GB (Possible VD)"))))
                    {
                        diskKey.SetValue("DateIgnored", DateTime.Now.ToShortDateString());
                        diskKey.Close();
                        return;
                    }
                    else
                    {
                        diskKey.Close();
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Ignores all of the bad sectors associated with the specified disk. This doesn't change the SMART
        /// attribute; it just causes HSS to stop warning the user about them. (If new bad sectors pop up,
        /// the user will be warned again.)
        /// </summary>
        /// <param name="disk"></param>
        public static void IgnoreBadSectors(String disk, String model, long ignoreCount)
        {
            try
            {
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);
                    bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                    if (!activeFlag)
                    {
                        // Inactive/Stale disk.
                        continue;
                    }

                    if (String.Compare(disk, (String)diskKey.GetValue("DevicePath").ToString(), true) == 0 &&
                        String.Compare(disk, (String)diskKey.GetValue("Name").ToString(), true) == 0 &&
                        (String.Compare(model, (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model, (String)diskKey.GetValue("PreferredModel"), true) == 0 ||
                        String.Compare(model.Trim(), (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model.Trim(), (String)diskKey.GetValue("PreferredModel"), true) == 0))
                    {
                        diskKey.SetValue("BadSectorsIgnored", ignoreCount);
                        diskKey.Close();
                        return;
                    }
                    else
                    {
                        diskKey.Close();
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Ignores all of the pending sectors associated with the specified disk. This doesn't change the SMART
        /// attribute; it just causes HSS to stop warning the user about them. (If new pending sectors pop up, or
        /// the count drops, the user will be warned again.)
        /// </summary>
        /// <param name="disk"></param>
        public static void IgnorePendingSectors(String disk, String model, long ignoreCount)
        {
            try
            {
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);
                    bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                    if (!activeFlag)
                    {
                        // Inactive/Stale disk.
                        continue;
                    }

                    if (String.Compare(disk, (String)diskKey.GetValue("DevicePath").ToString(), true) == 0 &&
                        String.Compare(disk, (String)diskKey.GetValue("Name").ToString(), true) == 0 &&
                        (String.Compare(model, (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model, (String)diskKey.GetValue("PreferredModel"), true) == 0 ||
                        String.Compare(model.Trim(), (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model.Trim(), (String)diskKey.GetValue("PreferredModel"), true) == 0))
                    {
                        diskKey.SetValue("PendingSectorsIgnored", ignoreCount);
                        diskKey.Close();
                        return;
                    }
                    else
                    {
                        diskKey.Close();
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Ignores all of the end-to-end errors associated with the specified disk. This doesn't change the SMART
        /// attribute; it just causes HSS to stop warning the user about them. (If new end-to-end errors pop up,
        /// the user will be warned again.)
        /// </summary>
        /// <param name="disk"></param>
        public static void IgnoreEndToEndErrors(String disk, String model, long ignoreCount)
        {
            try
            {
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);
                    bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                    if (!activeFlag)
                    {
                        // Inactive/Stale disk.
                        continue;
                    }

                    if (String.Compare(disk, (String)diskKey.GetValue("DevicePath").ToString(), true) == 0 &&
                        String.Compare(disk, (String)diskKey.GetValue("Name").ToString(), true) == 0 &&
                        (String.Compare(model, (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model, (String)diskKey.GetValue("PreferredModel"), true) == 0 ||
                        String.Compare(model.Trim(), (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model.Trim(), (String)diskKey.GetValue("PreferredModel"), true) == 0))
                    {
                        diskKey.SetValue("EndToEndErrorsIgnored", ignoreCount);
                        diskKey.Close();
                        return;
                    }
                    else
                    {
                        diskKey.Close();
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Ignores all of the spin retries associated with the specified disk. This doesn't change the SMART
        /// attribute; it just causes HSS to stop warning the user about them. (If new spin retries pop up,
        /// the user will be warned again.)
        /// </summary>
        /// <param name="disk"></param>
        public static void IgnoreSpinRetries(String disk, String model, long ignoreCount)
        {
            try
            {
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);
                    bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                    if (!activeFlag)
                    {
                        // Inactive/Stale disk.
                        continue;
                    }

                    if (String.Compare(disk, (String)diskKey.GetValue("DevicePath").ToString(), true) == 0 &&
                        String.Compare(disk, (String)diskKey.GetValue("Name").ToString(), true) == 0 &&
                        (String.Compare(model, (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model, (String)diskKey.GetValue("PreferredModel"), true) == 0 ||
                        String.Compare(model.Trim(), (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model.Trim(), (String)diskKey.GetValue("PreferredModel"), true) == 0))
                    {
                        diskKey.SetValue("SpinRetriesIgnored", ignoreCount);
                        diskKey.Close();
                        return;
                    }
                    else
                    {
                        diskKey.Close();
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Ignores all of the reallocation events associated with the specified disk. This doesn't change the SMART
        /// attribute; it just causes HSS to stop warning the user about them. (If new reallocation events pop up,
        /// the user will be warned again.)
        /// </summary>
        /// <param name="disk"></param>
        public static void IgnoreReallocations(String disk, String model, long ignoreCount)
        {
            try
            {
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);
                    bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                    if (!activeFlag)
                    {
                        // Inactive/Stale disk.
                        continue;
                    }

                    if (String.Compare(disk, (String)diskKey.GetValue("DevicePath").ToString(), true) == 0 &&
                        String.Compare(disk, (String)diskKey.GetValue("Name").ToString(), true) == 0 &&
                        (String.Compare(model, (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model, (String)diskKey.GetValue("PreferredModel"), true) == 0 ||
                        String.Compare(model.Trim(), (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model.Trim(), (String)diskKey.GetValue("PreferredModel"), true) == 0))
                    {
                        diskKey.SetValue("ReallocationEventsIgnored", ignoreCount);
                        diskKey.Close();
                        return;
                    }
                    else
                    {
                        diskKey.Close();
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Ignores all of the offline uncorrectable bad sectors associated with the specified disk.  This doesn't change
        /// the SMART attribute; it just causes HSS to stop warning the user about them. (If new offline bad sectors pop up,
        /// the user will be warned again.)
        /// </summary>
        /// <param name="disk"></param>
        /// <param name="ignoreCount"></param>
        public static void IgnoreOfflineBadSectors(String disk, String model, long ignoreCount)
        {
            try
            {
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);
                    bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                    if (!activeFlag)
                    {
                        // Inactive/Stale disk.
                        continue;
                    }

                    if (String.Compare(disk, (String)diskKey.GetValue("DevicePath").ToString(), true) == 0 &&
                        String.Compare(disk, (String)diskKey.GetValue("Name").ToString(), true) == 0 &&
                        (String.Compare(model, (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model, (String)diskKey.GetValue("PreferredModel"), true) == 0 ||
                        String.Compare(model.Trim(), (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model.Trim(), (String)diskKey.GetValue("PreferredModel"), true) == 0))
                    {
                        diskKey.SetValue("OfflineBadSectorsIgnored", ignoreCount);
                        diskKey.Close();
                        return;
                    }
                    else
                    {
                        diskKey.Close();
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Ignores all of the CRC  associated with the specified disk.  This doesn't change
        /// the SMART attribute; it just causes HSS to stop warning the user about them. (If new CRC errors pop up,
        /// the user will be warned again.)
        /// </summary>
        /// <param name="disk"></param>
        /// <param name="ignoreCount"></param>
        public static void IgnoreCrcErrors(String disk, String model, long ignoreCount)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.DiskEnumerator.IgnoreCrcErrors");
            SiAuto.Main.LogString("disk", disk);
            SiAuto.Main.LogString("model", model);
            SiAuto.Main.LogLong("ignoreCount", ignoreCount);
            try
            {
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    SiAuto.Main.LogMessage("Examining " + diskKeyName);
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);
                    bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                    if (!activeFlag)
                    {
                        // Inactive/Stale disk.
                        SiAuto.Main.LogMessage("Disk is inactive or stale, skipping.");
                        continue;
                    }

                    if (String.Compare(disk, (String)diskKey.GetValue("DevicePath").ToString(), true) == 0 &&
                        String.Compare(disk, (String)diskKey.GetValue("Name").ToString(), true) == 0 &&
                        (String.Compare(model, (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model, (String)diskKey.GetValue("PreferredModel"), true) == 0 ||
                        String.Compare(model.Trim(), (String)diskKey.GetValue("Model"), true) == 0 || String.Compare(model.Trim(), (String)diskKey.GetValue("PreferredModel"), true) == 0))
                    {
                        SiAuto.Main.LogMessage("Found disk to update; performing update.");
                        diskKey.SetValue("CrcErrorsIgnored", ignoreCount);
                        diskKey.Close();
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.IgnoreCrcErrors");
                        return;
                    }
                    else
                    {
                        diskKey.Close();
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError(ex.Message);
                SiAuto.Main.LogException(ex);
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.IgnoreCrcErrors");
                throw ex;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.DiskEnumerator.IgnoreCrcErrors");
        }
    }
}
