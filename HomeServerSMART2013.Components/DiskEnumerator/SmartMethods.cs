using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public static class SmartMethods
    {
        public static string SMART_DISK_IDENTIFY_DEVICE = "Lickin20Good10Looking";

        /// <summary>
        /// Try to add IDE and SCSI disks.  No guarantees, so we try/catch each disk and if we can add it, great.  Otherwise,
        /// don't sweat it.  This is in BETA!!!  Note that IDE and SCSI can both encompass SATA.
        /// </summary>
        public static void GetInternalSmartInfo(SmartData smartDataTable, bool fallbackToWmi, bool advancedSii)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartMethods.GetInternalSmartInfo");
            // Get a list of all Silicon Image (SiI) controllers that have disks attached to them.
            // We don't care about SiI controllers without any attached non-removables.
            List<SiIDisk> siIDisks = GetSiIDisks();
            SiAuto.Main.LogObjectValue("List of SiI disks (siIDisks)", siIDisks);

            bool isWindows8 = Utilities.Utility.IsSystemWindows8();

            if (isWindows8)
            {
                // Browse all WMI physical disks.
                foreach (ManagementObject drive in new ManagementObjectSearcher(Properties.Resources.WmiQueryScope,
                  Properties.Resources.WmiQueryStringWin8).Get())
                {
                    SiAuto.Main.LogMessage("Enumerating device using Win8 methods.");
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
                    String interfaceType = Utilities.Utility.GetWindows8DriveInterfaceType((UInt16)drive["BusType"]);
                    if (interfaceType == "SCSI" || interfaceType == "ATAPI" || interfaceType == "ATA" || interfaceType == "SSA" || interfaceType == "Fibre Channel" ||
                        interfaceType == "RAID" || interfaceType == "SAS" || interfaceType == "SATA")
                    {
                        byte[] smartData;
                        byte[] smartThreshold;
                        bool failurePredicted = false;
                        byte target = 0;
                        target = GlobalConstants.DEFAULT_TARGET_ID;
                        String serialNumber = String.Empty;
                        String model = String.Empty;
                        String pnpDeviceID = String.Empty;
                        String deviceID = "\\\\.\\PHYSICALDRIVE" + drive["DeviceID"].ToString();

                        SiAuto.Main.LogMessage("Collecting serial and model.");
                        if (drive["SerialNumber"] != null)
                        {
                            serialNumber = drive["SerialNumber"].ToString();
                        }
                        if (drive["Model"] != null)
                        {
                            model = drive["Model"].ToString();
                        }

                        int ssDiskNumber = -1;

                        // Get the physical drive number
                        try
                        {
                            SiAuto.Main.LogMessage("Attempting to parse physical drive number from " + deviceID);
                            ssDiskNumber = Utilities.Utility.GetDriveIdFromPath(deviceID);
                            SiAuto.Main.LogInt("Parse successful; ssDiskNumber", ssDiskNumber);
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogError(ex.Message);
                            SiAuto.Main.LogException(ex);
                            SiAuto.Main.LogWarning("Could not parse " + deviceID + " to an integer; skipping this disk.");
                            ssDiskNumber = -1;
                        }

                        pnpDeviceID = Utilities.Utility.BuildWindows8PnpDeviceName(model, serialNumber, (UInt16)drive["BusType"], ssDiskNumber);

                        SiAuto.Main.LogMessage("Make sure the disk isn't ignored by the user.");
                        if (IsDiskIgnored(pnpDeviceID))
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Goldenrod, "[Ignored Disk Discard] The disk " + pnpDeviceID + " has the DateIgnored" +
                                "flag set and thus will not be checked.");
                            continue;
                        }

                        SiAuto.Main.LogMessage("Get identity and SMART data from internal disk, PNP=" + pnpDeviceID);

                        // Get identity data (spindle speed/SSD) from the non-removable.
                        try
                        {
                            DiskIdentityData did;
                            String siliconImageType = String.Empty;
                            String alternatePnp = String.Empty;
                            UInt16 scsiPort = 0;

                            try
                            {
                                SiAuto.Main.LogMessage("Determining if there's a usable alternatePnp and port we can use.");
                                alternatePnp = drive["UniqueID"].ToString();
                                SiAuto.Main.LogString("drive[\"UniqueID\"] alternatePnp", alternatePnp);
                                String portString = alternatePnp.Substring(alternatePnp.LastIndexOf('&'));
                                SiAuto.Main.LogString("portString", portString);
                                // Fifth position? (i.e. &000300 --> port 3)?
                                scsiPort = UInt16.Parse(portString.Substring(4, 1));
                                SiAuto.Main.LogMessage("Attempt to get SCSI port returned value " + scsiPort.ToString());
                            }
                            catch
                            {
                                if (String.IsNullOrEmpty(alternatePnp))
                                {
                                    SiAuto.Main.LogMessage("The disk did not have a PNP string in the UniqueID field.");
                                }
                                else
                                {
                                    SiAuto.Main.LogMessage("The disk had a unique PNP ID, but a port couldn't be parsed. Value will be ignored.");
                                    alternatePnp = String.Empty;
                                }
                            }

                            int win8ScsiPort = -1;
                            int win8ScsiTarget = -1;
                            int win8ScsiBus = -1;
                            Utilities.Utility.GetWindowsScsiPortInfo(deviceID, out win8ScsiPort, out win8ScsiTarget, out win8ScsiBus);

                            if (siIDisks.Count > 0 && IsPhysicalDriveSiIDisk((!String.IsNullOrEmpty(alternatePnp) ? alternatePnp : pnpDeviceID), siIDisks, isWindows8, out siliconImageType))
                            {
                                uint siiType = 0;
                                bool code = uint.TryParse(siliconImageType, out siiType);
                                if (!code)
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Could not parse SiI disk type; using default 3132.");
                                    siiType = 3132;
                                }
                                SiAuto.Main.LogInt("siiType", (int)siiType);

                                // TODO - change to appropriate SiI code!
                                //if (InteropSmart.GetSiIDriveIdentity(drive["DeviceID"].ToString(), (UInt16)drive["SCSIPort"], (UInt32)drive["SCSIBus"],
                                //    (UInt16)drive["SCSITargetId"], siiType, out did))
                                //if (InteropSmart.GetSiIDriveIdentity(deviceID, (UInt16)0, (UInt32)0,
                                //    scsiPort, siiType, out did))
                                if (InteropSmart.GetSiIDriveIdentity(deviceID, (UInt16)1, (UInt32)1,
                                    0, siiType, out did))
                                {
                                    SiAuto.Main.LogMessage("Successfully fetched internal disk identity data with SiI code.");
                                    SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                    SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                    SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                    smartDataTable.AddIdentityData(pnpDeviceID, did);
                                }
                                else if (InteropSmart.GetInternalDriveIdentity(deviceID, target, out did))
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Successfully fetched internal disk identity data using non-SiI mode, so some data MAY not be accurate.");
                                    SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                    SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                    SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                    smartDataTable.AddIdentityData(pnpDeviceID, did);
                                }
                                else if(InteropSmart.GetCsmiDriveIdentity(win8ScsiPort, win8ScsiTarget, win8ScsiBus, out did))
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Successfully fetched internal disk identity data using CSMI mode, so some data MAY not be accurate.");
                                    SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                    SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                    SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                    smartDataTable.AddIdentityData(pnpDeviceID, did);
                                }
                                else
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Failed to fetch internal disk identity data.");
                                    SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                    SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                    SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                    smartDataTable.AddIdentityData(pnpDeviceID, did);
                                }
                            }
                            else
                            {
                                if (InteropSmart.GetInternalDriveIdentity(deviceID, target, out did))
                                {
                                    SiAuto.Main.LogMessage("Successfully fetched internal disk identity data.");
                                    SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                    SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                    SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                    smartDataTable.AddIdentityData(pnpDeviceID, did);
                                }
                                else if (InteropSmart.GetCsmiDriveIdentity(win8ScsiPort, win8ScsiTarget, win8ScsiBus, out did))
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Successfully fetched internal disk identity data using CSMI mode.");
                                    SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                    SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                    SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                    smartDataTable.AddIdentityData(pnpDeviceID, did);
                                }
                                else
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Failed to fetch internal disk identity data.");
                                    SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                    SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                    SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                    smartDataTable.AddIdentityData(pnpDeviceID, did);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogError("GetInternalDriveIdentity threw unexpected exception during identity enumeration oepration.");
                            SiAuto.Main.LogException(ex);
                        }

                        // Get SMART data from the non-removable.
                        try
                        {
                            String siliconImageType = String.Empty;
                            String alternatePnp = String.Empty;
                            UInt16 scsiPort = 0;

                            int win8ScsiPort = -1;
                            int win8ScsiTarget = -1;
                            int win8ScsiBus = -1;

                            Utilities.Utility.GetWindowsScsiPortInfo(deviceID, out win8ScsiPort, out win8ScsiTarget, out win8ScsiBus);

                            try
                            {
                                SiAuto.Main.LogMessage("Determining if there's a usable alternatePnp and port we can use.");
                                alternatePnp = drive["UniqueID"].ToString();
                                SiAuto.Main.LogString("drive[\"UniqueID\"] alternatePnp", alternatePnp);
                                String portString = alternatePnp.Substring(alternatePnp.LastIndexOf('&'));
                                SiAuto.Main.LogString("portString", portString);
                                // Fifth position? (i.e. &000300 --> port 3)?
                                scsiPort = UInt16.Parse(portString.Substring(4, 1));
                                SiAuto.Main.LogMessage("Attempt to get SCSI port returned value " + scsiPort.ToString());
                            }
                            catch
                            {
                                if (String.IsNullOrEmpty(alternatePnp))
                                {
                                    SiAuto.Main.LogMessage("The disk did not have a PNP string in the UniqueID field.");
                                }
                                else
                                {
                                    SiAuto.Main.LogMessage("The disk had a unique PNP ID, but a port couldn't be parsed. Value will be ignored.");
                                    alternatePnp = String.Empty;
                                }
                            }
                            
                            if (siIDisks.Count > 0 && IsPhysicalDriveSiIDisk((!String.IsNullOrEmpty(alternatePnp) ? alternatePnp : pnpDeviceID), siIDisks, isWindows8, out siliconImageType))
                            {
                                uint siiType = 0;
                                bool code = uint.TryParse(siliconImageType, out siiType);
                                if (!code)
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Could not parse SiI disk type; using default 3132.");
                                    siiType = 3132;
                                }
                                SiAuto.Main.LogInt("siiType", (int)siiType);

                                SiAuto.Main.LogMessage("Attempt to get SMART data (SiI).");
                                if (InteropSmart.GetInternalSmartData(deviceID, target, true, (!String.IsNullOrEmpty(alternatePnp) ? alternatePnp : pnpDeviceID),
                                    advancedSii, out smartData, out smartThreshold, out failurePredicted))
                                {
                                    SiAuto.Main.LogMessage("Successfully got data; add to table.");
                                    smartDataTable.AddSmartData(pnpDeviceID, smartData, smartThreshold);
                                }
                                else if (InteropSmart.GetSiISmartAttributesAdvanced((UInt16)0, (UInt32)0,
                                    scsiPort, siiType, pnpDeviceID, advancedSii,
                                    out smartData, out smartThreshold, out failurePredicted))
                                {
                                    SiAuto.Main.LogMessage("Successfully got data via SiI alternate method; add to table.");
                                    smartDataTable.AddSmartData(pnpDeviceID, smartData, smartThreshold);
                                }
                                else if(InteropSmart.GetCsmiSmartData(win8ScsiPort, win8ScsiTarget, win8ScsiBus, out smartData, out smartThreshold, out failurePredicted))
                                {
                                    SiAuto.Main.LogMessage("Successfully got data via SiI CSMI method; add to table.");
                                    smartDataTable.AddSmartData(pnpDeviceID, smartData, smartThreshold);
                                }
                                else if (InteropSmart.GetInternalSmartData(deviceID, target, false, (!String.IsNullOrEmpty(alternatePnp) ? alternatePnp : pnpDeviceID),
                                    advancedSii, out smartData, out smartThreshold, out failurePredicted))
                                {
                                    SiAuto.Main.LogMessage("Successfully got data via internal drive code (last chance saloon); add to table.");
                                    smartDataTable.AddSmartData(pnpDeviceID, smartData, smartThreshold);
                                }
                                else
                                {
                                    if (fallbackToWmi)
                                    {
                                        SiAuto.Main.LogWarning("Failed to get SMART data via P/Invoke; falling back to WMI.");
                                        String win8PnpDeviceID = Utilities.Utility.GetWindows8PnpDiskId(deviceID);
                                        if (InteropSmart.GetWmiSmartData(win8PnpDeviceID, out smartData, out smartThreshold, out failurePredicted))
                                        {
                                            SiAuto.Main.LogMessage("Successfully got data; add to table.");
                                            smartDataTable.AddSmartData(pnpDeviceID, smartData, smartThreshold);
                                        }
                                        else
                                        {
                                            SiAuto.Main.LogColored(System.Drawing.Color.Orange, "Failed to get SMART data via WMI fallback. Data will not be available.");
                                            smartData = new byte[512];
                                            smartThreshold = new byte[512];
                                            smartDataTable.AddSmartData(pnpDeviceID, smartData, smartThreshold);
                                        }
                                    }
                                    else
                                    {
                                        SiAuto.Main.LogColored(System.Drawing.Color.Orange, "Failed to get SMART data, and WMI fallback was disabled. Data will not be available.");
                                        smartData = new byte[512];
                                        smartThreshold = new byte[512];
                                        smartDataTable.AddSmartData(pnpDeviceID, smartData, smartThreshold);
                                    }
                                }
                            }
                            else
                            {
                                SiAuto.Main.LogMessage("Attempt to get SMART data (internal).");
                                if (InteropSmart.GetInternalSmartData(deviceID, target, out smartData, out smartThreshold,
                                    out failurePredicted))
                                {
                                    SiAuto.Main.LogMessage("Successfully got data; add to table.");
                                    smartDataTable.AddSmartData(pnpDeviceID, smartData, smartThreshold);
                                }
                                else if(InteropSmart.GetCsmiSmartData(win8ScsiPort, win8ScsiTarget, win8ScsiBus, out smartData, out smartThreshold, out failurePredicted))
                                {
                                    SiAuto.Main.LogMessage("Successfully got data via CSMI method; add to table.");
                                    smartDataTable.AddSmartData(pnpDeviceID, smartData, smartThreshold);
                                }
                                else
                                {
                                    if (fallbackToWmi)
                                    {
                                        SiAuto.Main.LogWarning("Failed to get SMART data via P/Invoke; falling back to WMI.");
                                        String win8PnpDeviceID = Utilities.Utility.GetWindows8PnpDiskId(deviceID);
                                        if (InteropSmart.GetWmiSmartData(win8PnpDeviceID, out smartData, out smartThreshold, out failurePredicted))
                                        {
                                            SiAuto.Main.LogMessage("Successfully got data; add to table.");
                                            smartDataTable.AddSmartData(pnpDeviceID, smartData, smartThreshold);
                                        }
                                        else
                                        {
                                            SiAuto.Main.LogColored(System.Drawing.Color.Orange, "Failed to get SMART data via WMI fallback. Data will not be available.");
                                            smartData = new byte[512];
                                            smartThreshold = new byte[512];
                                            smartDataTable.AddSmartData(pnpDeviceID, smartData, smartThreshold);
                                        }
                                    }
                                    else
                                    {
                                        SiAuto.Main.LogColored(System.Drawing.Color.Orange, "Failed to get SMART data, and WMI fallback was disabled. Data will not be available.");
                                        smartData = new byte[512];
                                        smartThreshold = new byte[512];
                                        smartDataTable.AddSmartData(pnpDeviceID, smartData, smartThreshold);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogError("GetInternalSmartInfo threw unexpected exception during SMART enumeration oepration.");
                            SiAuto.Main.LogException(ex);
                        }
                    }
                }
            }
            else // Windows 7 and earlier
            {
                // Browse all WMI physical disks.
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                  Properties.Resources.WmiQueryStringNonWin8).Get())
                {
                    SiAuto.Main.LogMessage("Enumerating device using pre-Win8 methods.");
                    if (drive["InterfaceType"] != null &&
                        (String.Compare(drive["InterfaceType"].ToString(), "IDE", true) == 0 ||
                        String.Compare(drive["InterfaceType"].ToString(), "SCSI", true) == 0))
                    {
                        byte[] smartData;
                        byte[] smartThreshold;
                        bool failurePredicted = false;
                        byte target = 0;
                        target = GlobalConstants.DEFAULT_TARGET_ID;

                        SiAuto.Main.LogMessage("Make sure the disk isn't ignored by the user.");
                        if (IsDiskIgnored(drive["PNPDeviceID"].ToString()))
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Goldenrod, "[Ignored Disk Discard] The disk " + drive["PNPDeviceID"].ToString() + " has the DateIgnored" +
                                "flag set and thus will not be checked.");
                            continue;
                        }

                        SiAuto.Main.LogMessage("Get identity and SMART data from internal disk, PNP=" + drive["PNPDeviceId"].ToString());

                        // Get identity data (spindle speed/SSD) from the non-removable.
                        try
                        {
                            DiskIdentityData did;
                            String siliconImageType = String.Empty;

                            int win8ScsiPort = -1;
                            int win8ScsiTarget = -1;
                            int win8ScsiBus = -1;
                            Utilities.Utility.GetWindowsScsiPortInfo(drive["DeviceID"].ToString(), out win8ScsiPort, out win8ScsiTarget, out win8ScsiBus);

                            if (siIDisks.Count > 0 && IsPhysicalDriveSiIDisk(drive["PNPDeviceId"].ToString(), siIDisks, isWindows8, out siliconImageType))
                            {
                                uint siiType = 0;
                                bool code = uint.TryParse(siliconImageType, out siiType);
                                if (!code)
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Could not parse SiI disk type; using default 3132.");
                                    siiType = 3132;
                                }
                                SiAuto.Main.LogInt("siiType", (int)siiType);

                                // TODO - change to appropriate SiI code!
                                //if (InteropSmart.GetInternalDriveIdentity(drive["DeviceID"].ToString(), target, out did))
                                if (InteropSmart.GetSiIDriveIdentity(drive["DeviceID"].ToString(), (UInt16)drive["SCSIPort"], (UInt32)drive["SCSIBus"],
                                    (UInt16)drive["SCSITargetId"], siiType, out did))
                                {
                                    SiAuto.Main.LogMessage("Successfully fetched internal disk identity data with SiI mode.");
                                    SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                    SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                    SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                    smartDataTable.AddIdentityData(drive["PNPDeviceID"].ToString(), did);
                                }
                                else if (InteropSmart.GetInternalDriveIdentity(drive["DeviceID"].ToString(), target, out did))
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Successfully fetched internal disk identity data using non-SiI mode, so some data MAY not be accurate.");
                                    SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                    SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                    SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                    smartDataTable.AddIdentityData(drive["PNPDeviceID"].ToString(), did);
                                }
                                else if (InteropSmart.GetCsmiDriveIdentity(win8ScsiPort, win8ScsiTarget, win8ScsiBus, out did))
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Successfully fetched internal disk identity data using CSMI mode, so some data MAY not be accurate.");
                                    SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                    SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                    SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                    smartDataTable.AddIdentityData(drive["PNPDeviceID"].ToString(), did);
                                }
                                else
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Failed to fetch internal disk identity data.");
                                    SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                    SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                    SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                    smartDataTable.AddIdentityData(drive["PNPDeviceID"].ToString(), did);
                                }
                            }
                            else
                            {
                                if (InteropSmart.GetInternalDriveIdentity(drive["DeviceID"].ToString(), target, out did))
                                {
                                    SiAuto.Main.LogMessage("Successfully fetched internal disk identity data.");
                                    SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                    SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                    SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                    smartDataTable.AddIdentityData(drive["PNPDeviceID"].ToString(), did);
                                }
                                else if (InteropSmart.GetCsmiDriveIdentity(win8ScsiPort, win8ScsiTarget, win8ScsiBus, out did))
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Successfully fetched internal disk identity data using CSMI mode.");
                                    SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                    SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                    SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                    smartDataTable.AddIdentityData(drive["PNPDeviceID"].ToString(), did);
                                }
                                else
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Failed to fetch internal disk identity data.");
                                    SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                    SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                    SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                    smartDataTable.AddIdentityData(drive["PNPDeviceID"].ToString(), did);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogError("GetInternalDriveIdentity threw unexpected exception during identity enumeration oepration.");
                            SiAuto.Main.LogException(ex);
                        }

                        // Get SMART data from the non-removable.
                        try
                        {
                            String siliconImageType = String.Empty;
                            int win8ScsiPort = -1;
                            int win8ScsiTarget = -1;
                            int win8ScsiBus = -1;

                            Utilities.Utility.GetWindowsScsiPortInfo(drive["DeviceID"].ToString(), out win8ScsiPort, out win8ScsiTarget, out win8ScsiBus);

                            if (siIDisks.Count > 0 && IsPhysicalDriveSiIDisk(drive["PNPDeviceId"].ToString(), siIDisks, isWindows8, out siliconImageType))
                            {
                                uint siiType = 0;
                                bool code = uint.TryParse(siliconImageType, out siiType);
                                if (!code)
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Could not parse SiI disk type; using default 3132.");
                                    siiType = 3132;
                                }
                                SiAuto.Main.LogInt("siiType", (int)siiType);

                                SiAuto.Main.LogMessage("Attempt to get SMART data (SiI).");
                                if (InteropSmart.GetInternalSmartData(drive["DeviceID"].ToString(), target, true, drive["PNPDeviceId"].ToString(),
                                    advancedSii, out smartData, out smartThreshold, out failurePredicted))
                                {
                                    SiAuto.Main.LogMessage("Successfully got data; add to table.");
                                    smartDataTable.AddSmartData(drive["PNPDeviceID"].ToString(), smartData, smartThreshold);
                                }
                                else if (InteropSmart.GetSiISmartAttributesAdvanced((UInt16)drive["SCSIPort"], (UInt32)drive["SCSIBus"],
                                    (UInt16)drive["SCSITargetId"], siiType, drive["PNPDeviceID"].ToString(), advancedSii,
                                    out smartData, out smartThreshold, out failurePredicted))
                                {
                                    SiAuto.Main.LogMessage("Successfully got data via SiI alternate method; add to table.");
                                    smartDataTable.AddSmartData(drive["PNPDeviceID"].ToString(), smartData, smartThreshold);
                                }
                                else if (InteropSmart.GetCsmiSmartData(win8ScsiPort, win8ScsiTarget, win8ScsiBus, out smartData, out smartThreshold, out failurePredicted))
                                {
                                    SiAuto.Main.LogMessage("Successfully got data via SiI CSMI method; add to table.");
                                    smartDataTable.AddSmartData(drive["PNPDeviceID"].ToString(), smartData, smartThreshold);
                                }
                                else if (InteropSmart.GetInternalSmartData(drive["DeviceID"].ToString(), target, false, drive["PNPDeviceId"].ToString(),
                                    advancedSii, out smartData, out smartThreshold, out failurePredicted))
                                {
                                    SiAuto.Main.LogMessage("Successfully got data via internal drive code (last chance saloon); add to table.");
                                    smartDataTable.AddSmartData(drive["PNPDeviceID"].ToString(), smartData, smartThreshold);
                                }
                                else
                                {
                                    if (fallbackToWmi)
                                    {
                                        SiAuto.Main.LogWarning("Failed to get SMART data via P/Invoke; falling back to WMI.");
                                        if (InteropSmart.GetWmiSmartData(drive["PNPDeviceID"].ToString(), out smartData, out smartThreshold, out failurePredicted))
                                        {
                                            SiAuto.Main.LogMessage("Successfully got data; add to table.");
                                            smartDataTable.AddSmartData(drive["PNPDeviceID"].ToString(), smartData, smartThreshold);
                                        }
                                        else
                                        {
                                            SiAuto.Main.LogColored(System.Drawing.Color.Orange, "Failed to get SMART data via WMI fallback. Data will not be available.");
                                            smartData = new byte[512];
                                            smartThreshold = new byte[512];
                                            smartDataTable.AddSmartData(drive["PNPDeviceID"].ToString(), smartData, smartThreshold);
                                        }
                                    }
                                    else
                                    {
                                        SiAuto.Main.LogColored(System.Drawing.Color.Orange, "Failed to get SMART data, and WMI fallback was disabled. Data will not be available.");
                                        smartData = new byte[512];
                                        smartThreshold = new byte[512];
                                        smartDataTable.AddSmartData(drive["PNPDeviceID"].ToString(), smartData, smartThreshold);
                                    }
                                }
                            }
                            else
                            {
                                SiAuto.Main.LogMessage("Attempt to get SMART data (internal).");
                                if (InteropSmart.GetInternalSmartData(drive["DeviceID"].ToString(), target, out smartData, out smartThreshold,
                                    out failurePredicted))
                                {
                                    SiAuto.Main.LogMessage("Successfully got data; add to table.");
                                    smartDataTable.AddSmartData(drive["PNPDeviceID"].ToString(), smartData, smartThreshold);
                                }
                                else if (InteropSmart.GetCsmiSmartData(win8ScsiPort, win8ScsiTarget, win8ScsiBus, out smartData, out smartThreshold, out failurePredicted))
                                {
                                    SiAuto.Main.LogMessage("Successfully got data via SiI CSMI method; add to table.");
                                    smartDataTable.AddSmartData(drive["PNPDeviceID"].ToString(), smartData, smartThreshold);
                                }
                                else
                                {
                                    if (fallbackToWmi)
                                    {
                                        SiAuto.Main.LogWarning("Failed to get SMART data via P/Invoke; falling back to WMI.");
                                        if (InteropSmart.GetWmiSmartData(drive["PNPDeviceID"].ToString(), out smartData, out smartThreshold, out failurePredicted))
                                        {
                                            SiAuto.Main.LogMessage("Successfully got data; add to table.");
                                            smartDataTable.AddSmartData(drive["PNPDeviceID"].ToString(), smartData, smartThreshold);
                                        }
                                        else
                                        {
                                            SiAuto.Main.LogColored(System.Drawing.Color.Orange, "Failed to get SMART data via WMI fallback. Data will not be available.");
                                            smartData = new byte[512];
                                            smartThreshold = new byte[512];
                                            smartDataTable.AddSmartData(drive["PNPDeviceID"].ToString(), smartData, smartThreshold);
                                        }
                                    }
                                    else
                                    {
                                        SiAuto.Main.LogColored(System.Drawing.Color.Orange, "Failed to get SMART data, and WMI fallback was disabled. Data will not be available.");
                                        smartData = new byte[512];
                                        smartThreshold = new byte[512];
                                        smartDataTable.AddSmartData(drive["PNPDeviceID"].ToString(), smartData, smartThreshold);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogError("GetInternalSmartInfo threw unexpected exception during SMART enumeration oepration.");
                            SiAuto.Main.LogException(ex);
                        }
                    }
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartMethods.GetInternalSmartInfo");
        }

        /// <summary>
        /// Try to add USB disks.  No guarantees, so we try/catch each disk and if we can add it, great.  Otherwise,
        /// don't sweat it.  This is in BETA!!!
        /// </summary>
        public static void GetUsbSmartInfo(SmartData smartDataTable)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartMethods.GetUsbSmartInfo");

            if (Utilities.Utility.IsSystemWindows8())
            {
                // Browse all WMI physical disks.
                foreach (ManagementObject drive in new ManagementObjectSearcher(Properties.Resources.WmiQueryScope,
                  Properties.Resources.WmiQueryStringWin8).Get())
                {
                    SiAuto.Main.LogMessage("Enumerating device using Win8 methods.");
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
                    String interfaceType = Utilities.Utility.GetWindows8DriveInterfaceType((UInt16)drive["BusType"]);
                    if (interfaceType == "USB" || interfaceType == "1394")
                    {
                        byte[] smartData;
                        byte[] smartThreshold;
                        bool failurePredicted = false;

                        String serialNumber = String.Empty;
                        String model = String.Empty;
                        String pnpDeviceID = String.Empty;
                        String deviceID = "\\\\.\\PHYSICALDRIVE" + drive["DeviceID"].ToString();

                        SiAuto.Main.LogMessage("Collecting serial and model.");
                        if (drive["SerialNumber"] != null)
                        {
                            serialNumber = drive["SerialNumber"].ToString();
                        }
                        if (drive["Model"] != null)
                        {
                            model = drive["Model"].ToString();
                        }

                        int ssDiskNumber = -1;

                        // Get the physical drive number
                        try
                        {
                            SiAuto.Main.LogMessage("Attempting to parse physical drive number from " + deviceID);
                            ssDiskNumber = Utilities.Utility.GetDriveIdFromPath(deviceID);
                            SiAuto.Main.LogInt("Parse successful; ssDiskNumber", ssDiskNumber);
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogError(ex.Message);
                            SiAuto.Main.LogException(ex);
                            SiAuto.Main.LogWarning("Could not parse " + deviceID + " to an integer; skipping this disk.");
                            ssDiskNumber = -1;
                        }

                        pnpDeviceID = Utilities.Utility.BuildWindows8PnpDeviceName(model, serialNumber, (UInt16)drive["BusType"], ssDiskNumber);

                        SiAuto.Main.LogMessage("Make sure the disk isn't ignored by the user.");
                        if (IsDiskIgnored(pnpDeviceID))
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Goldenrod, "[Ignored Disk Discard] The disk " + pnpDeviceID + " has the DateIgnored" +
                                "flag set and thus will not be checked.");
                            continue;
                        }

                        SiAuto.Main.LogColored(System.Drawing.Color.DarkGreen, "Get identity and SMART data from USB disk, PNP=" + pnpDeviceID);

                        // Get identity data (spindle speed/SSD) from the non-removable.
                        try
                        {
                            DiskIdentityData did;

                            if (InteropSmart.GetUsbDriveIdentity(deviceID, out did))
                            {
                                SiAuto.Main.LogMessage("Successfully fetched internal disk identity data.");
                                SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                smartDataTable.AddIdentityData(pnpDeviceID, did);
                            }
                            else
                            {
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Failed to fetch internal disk identity data.");
                                SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                smartDataTable.AddIdentityData(pnpDeviceID, did);
                            }
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogError("GetUsbSmartInfo threw unexpected exception during identity enumeration oepration.");
                            SiAuto.Main.LogException(ex);
                        }

                        // Get SMART data from the non-removable.
                        try
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.LimeGreen, "Attempt to get SMART data (USB).");
                            if (InteropSmart.GetUsbSmartData(deviceID, out smartData, out smartThreshold, out failurePredicted))
                            {
                                SiAuto.Main.LogMessage("Successfully got data; add to table.");
                                SiAuto.Main.LogObjectValue("smartData", smartData);
                                SiAuto.Main.LogObjectValue("smartThreshold", smartThreshold);
                                smartDataTable.AddSmartData(pnpDeviceID, smartData, smartThreshold);
                            }
                            else
                            {
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange, "Failed to get SMART data, and WMI fallback is not available for USB. Data will not be available.");
                                smartData = new byte[512];
                                smartThreshold = new byte[512];
                                smartDataTable.AddSmartData(pnpDeviceID, smartData, smartThreshold);
                            }
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogError("GetUsbSmartInfo threw unexpected exception during SMART enumeration oepration.");
                            SiAuto.Main.LogException(ex);
                        }
                    }
                }
            }
            else
            {
                // Browse all WMI physical disks.
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                  Properties.Resources.WmiQueryStringNonWin8).Get())
                {
                    SiAuto.Main.LogMessage("Enumerating device using pre-Win8 methods.");
                    if (drive["InterfaceType"] != null && String.Compare(drive["InterfaceType"].ToString(), "USB", true) == 0)
                    {
                        byte[] smartData;
                        byte[] smartThreshold;
                        bool failurePredicted = false;

                        SiAuto.Main.LogMessage("Make sure the disk isn't ignored by the user.");
                        if (IsDiskIgnored(drive["PNPDeviceID"].ToString()))
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Goldenrod, "[Ignored Disk Discard] The disk " + drive["PNPDeviceID"].ToString() + " has the DateIgnored" +
                                "flag set and thus will not be checked.");
                            continue;
                        }

                        SiAuto.Main.LogColored(System.Drawing.Color.DarkGreen, "Get identity and SMART data from USB disk, PNP=" + drive["PNPDeviceId"].ToString());

                        // Get identity data (spindle speed/SSD) from the non-removable.
                        try
                        {
                            DiskIdentityData did;

                            if (InteropSmart.GetUsbDriveIdentity(drive["DeviceID"].ToString(), out did))
                            {
                                SiAuto.Main.LogMessage("Successfully fetched internal disk identity data.");
                                SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                smartDataTable.AddIdentityData(drive["PNPDeviceID"].ToString(), did);
                            }
                            else
                            {
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Failed to fetch internal disk identity data.");
                                SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                                SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                                SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                                smartDataTable.AddIdentityData(drive["PNPDeviceID"].ToString(), did);
                            }
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogError("GetUsbSmartInfo threw unexpected exception during identity enumeration oepration.");
                            SiAuto.Main.LogException(ex);
                        }

                        // Get SMART data from the non-removable.
                        try
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.LimeGreen, "Attempt to get SMART data (USB).");
                            if (InteropSmart.GetUsbSmartData(drive["DeviceID"].ToString(), out smartData, out smartThreshold, out failurePredicted))
                            {
                                SiAuto.Main.LogMessage("Successfully got data; add to table.");
                                SiAuto.Main.LogObjectValue("smartData", smartData);
                                SiAuto.Main.LogObjectValue("smartThreshold", smartThreshold);
                                smartDataTable.AddSmartData(drive["PNPDeviceID"].ToString(), smartData, smartThreshold);
                            }
                            else
                            {
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange, "Failed to get SMART data, and WMI fallback is not available for USB. Data will not be available.");
                                smartData = new byte[512];
                                smartThreshold = new byte[512];
                                smartDataTable.AddSmartData(drive["PNPDeviceID"].ToString(), smartData, smartThreshold);
                            }
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogError("GetUsbSmartInfo threw unexpected exception during SMART enumeration oepration.");
                            SiAuto.Main.LogException(ex);
                        }
                    }
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartMethods.GetUsbSmartInfo");
        }

        /// <summary>
        /// Enumerates the Win32_SCSIController WMI class looking for Silicon Image SiI controllers. If any SiI controllers are detected,
        /// evaluate each one (ASSOCIATORS OF) for Win32_SCSIControllerDevice, looking for objects with a DeviceID field
        /// that contains SCSI\DISK (a hard drive). Each detected hard drive attached to an SiI controller gets added to a generic String
        /// list.
        /// </summary>
        /// <returns>Generic String list containing the Win32_SCSIControllerDevice.DeviceID WMI property of each disk
        /// attached to an SiI controller.  If none are detected, or an exception occurs, the returned list will be empty.</returns>
        public static List<SiIDisk> GetSiIDisks()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartMethods.GetSiIDisks");
            List<String> siIControllers = new List<String>();
            List<SiIDisk> siIDisks = new List<SiIDisk>();
            bool isSiIDetected = false;

            // Find the SiI SCSI controllers.
            try
            {
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                       "select Name, DeviceID from Win32_SCSIController").Get())
                {
                    String controllerName = drive["Name"].ToString();
                    if (controllerName.ToUpper().Contains("SILICON IMAGE"))
                    {
                        SiAuto.Main.LogMessage("[SiI Controller] Detected SiI controller " + drive["DeviceID"].ToString());
                        siIControllers.Add(drive["DeviceID"].ToString());
                        isSiIDetected = true;
                    }
                }
            }
            catch // Catch everything.
            {
                SiAuto.Main.LogWarning("Unable to enumerate disk controllers.");
                isSiIDetected = false;
            }

            if (!isSiIDetected)
            {
                // We either didn't find any or an error occurred.  Make sure the list is empty
                // and return it.
                siIDisks.Clear();
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartMethods.GetSiIDisks");
                return siIDisks;
            }

            // We found at least one SiI so now we need to look for disks.
            foreach (String controller in siIControllers)
            {
                // Change \\ to \\\\ or next WMI call will die horribly.
                String testController = controller.Replace("\\", "\\\\");
                try
                {
                    foreach (ManagementObject drive in new ManagementObjectSearcher(
                           "ASSOCIATORS OF {Win32_SCSIController.DeviceID=\"" + testController + "\"} WHERE AssocClass = Win32_SCSIControllerDevice").Get())
                    {
                        String device = drive["DeviceID"].ToString();
                        String siliconImageType = String.Empty;
                        try
                        {
                            siliconImageType = testController.Substring(testController.ToLower().IndexOf("dev_") + 4, 4);
                            SiAuto.Main.LogMessage("Detected SiI type " + siliconImageType);
                        }
                        catch
                        {
                            SiAuto.Main.LogMessage("Error getting SiI type; using default SiI type 3132.");
                            siliconImageType = "3132";
                        }
                        if (device.ToUpper().Contains("SCSI\\DISK"))
                        {
                            // We only care about disks. Some SiI controllers have pseudo processor devices
                            // and we don't care about them.
                            siIDisks.Add(new SiIDisk(drive["DeviceID"].ToString(), siliconImageType, 0, 0));
                            SiAuto.Main.LogMessage("[SiI Controller] Detected SiI disk " + drive["DeviceID"].ToString());
                            SiAuto.Main.LogString("drive[Caption]", drive["Caption"].ToString());
                            SiAuto.Main.LogString("drive[Name]", drive["Name"].ToString());
                            SiAuto.Main.LogString("drive[PNPDeviceID]", drive["PNPDeviceID"].ToString());
                            SiAuto.Main.LogString("drive[SystemName]", drive["SystemName"].ToString());
                        }
                        else
                        {
                            // Not a disk - nothing to do here (at least not now).
                        }
                    }
                }
                catch // Catch everything.
                {
                    siIDisks.Clear();
                    SiAuto.Main.LogWarning("Failed to enumerate SiI associators.");
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartMethods.GetSiIDisks");
                    return siIDisks;
                }
            }
            SiAuto.Main.LogObjectValue("siIDisks", siIDisks);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartMethods.GetSiIDisks");
            return siIDisks;
        }

        /// <summary>
        /// Determines whether the disk specified is connected to a Silicon Image SiI controller. Note that you MUST
        /// call GetSiIDisks() before calling this method. If you don't, this method will always return false.
        /// </summary>
        /// <param name="diskPnpId">String PNPDeviceID from a Win32_DiskDrive object.</param>
        /// <param name="siiDisks">The generic String list of disks returned by GetSiIDisks().</param>
        /// <returns>true if the PNPDeviceID is a member of siiDisks; false otherwise.</returns>
        public static bool IsPhysicalDriveSiIDisk(String diskPnpId, List<SiIDisk> siiDisks, bool isWindows8, out String siliconImageType)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartMethods.IsPhysicalDriveSiIDisk");
            SiAuto.Main.LogString("diskPnpId", diskPnpId);
            SiAuto.Main.LogBool("isWindows8", isWindows8);
            siliconImageType = String.Empty;
            foreach (SiIDisk siiDisk in siiDisks)
            {
                SiAuto.Main.LogString("Comparing against siiDisk", siiDisk.DeviceID.ToUpper());
                if (isWindows8)
                {
                    if (diskPnpId.ToUpper().Contains(siiDisk.DeviceID.ToUpper()))
                    {
                        SiAuto.Main.LogColored(System.Drawing.Color.Purple, "Detected Windows 8/2012 based SiI disk; match on " + diskPnpId + " to SiI " + siiDisk.DeviceID);
                        siliconImageType = siiDisk.SiliconImageType;
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartMethods.IsPhysicalDriveSiIDisk");
                        return true;
                    }
                    try
                    {
                        String endPnp = diskPnpId.Substring(diskPnpId.LastIndexOf("\\") + 1);
                        SiAuto.Main.LogString("endPnp", endPnp);
                        if (siiDisk.DeviceID.ToUpper().Contains(endPnp.ToUpper()))
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Purple, "Detected Windows 8/2012 based SiI disk; match on alternate PNP check.");
                            siliconImageType = siiDisk.SiliconImageType;
                            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartMethods.IsPhysicalDriveSiIDisk");
                            return true;
                        }
                    }
                    catch
                    {
                        SiAuto.Main.LogMessage("Couldn't parse out an identifier string for alternate PNP comparison.");
                    }
                }
                else
                {
                    if (String.Compare(diskPnpId, siiDisk.DeviceID, true) == 0 ||
                        String.Compare(diskPnpId, siiDisk.DeviceID + "_0", true) == 0)
                    {
                        siliconImageType = siiDisk.SiliconImageType;
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartMethods.IsPhysicalDriveSiIDisk");
                        return true;
                    }
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartMethods.IsPhysicalDriveSiIDisk");
            return false;
        }

        private static bool IsDiskIgnored(String pnpPath)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartMethods.IsDiskIgnored");
            try
            {
                SiAuto.Main.LogMessage("Acquire Registry objects.");
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;
                Microsoft.Win32.RegistryKey diskKey = null;
                
                dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);
                SiAuto.Main.LogMessage("Acquired Registry objects.");

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    try
                    {
                        diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);
                        bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                        String dateIgnored = (String)diskKey.GetValue("DateIgnored", String.Empty);
                        String pnpDeviceID = (String)diskKey.GetValue("PNPDeviceID", String.Empty);

                        if (activeFlag && String.Compare(pnpDeviceID, pnpPath, true) == 0)
                        {
                            SiAuto.Main.LogMessage("pnpDeviceID and pnpPath match and disk is active, so performing ignored date check.");
                            SiAuto.Main.LogString("dateIgnored (date/time parse check)", dateIgnored);
                            DateTime result;
                            if (DateTime.TryParse(dateIgnored, out result))
                            {
                                // Ignored disk.
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Health Check] Date ignored is set; dateIgnored = " + dateIgnored + ", " +
                                    "assessment of disk " + pnpPath + " is cancelled.");
                                diskKey.Close();
                                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartMethods.IsDiskIgnored");
                                return true;
                            }
                            else
                            {
                                SiAuto.Main.LogMessage("Disk is NOT ignored.");
                                diskKey.Close();
                                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartMethods.IsDiskIgnored");
                                return false;
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        SiAuto.Main.LogWarning("Error occurred on dateIgnored value check; parse operation failed. The disk will be treated as active.");
                        SiAuto.Main.LogException(ex2);
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartMethods.IsDiskIgnored");
                        return false;
                    }
                }
                SiAuto.Main.LogMessage("Iterated all disks without a suitable match; assuming the disk is active.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartMethods.IsDiskIgnored");
                return false;
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogWarning("Error occurred on reading from the Registry to determine dateIgnored value check. The disk will be treated as active.");
                SiAuto.Main.LogException(ex);
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartMethods.IsDiskIgnored");
                return false;
            }
        }
    }
}
