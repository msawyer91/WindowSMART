using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.HomeServer.HomeServerSMARTService.Enumerator
{
    public class InteropSmart
    {
        [DllImport("HomeServerSMART.Utility.dll", SetLastError = true)]
        static extern IDENTIFY_DEVICE_RESULT IdentifyInternalDriveRpm(int physicalDriveId, byte target, IntPtr identifyDevice);

        [DllImport("HomeServerSMART.Utility.dll", SetLastError = true)]
        static extern IDENTIFY_DEVICE_RESULT IdentifyUsbDriveRpm(int physicalDriveId, byte target, IntPtr identifyDevice, COMMAND_TYPE cmdType);

        [DllImport("HomeServerSMART.Utility.dll", SetLastError = true)]
        static extern IDENTIFY_DEVICE_RESULT IdentifySiiDriveRpm(int physicalDriveId, int scsiPort, int scsiBus, UInt32 siliconImageType, IntPtr identifyDevice);

        [DllImport("HomeServerSMART.Utility.dll", SetLastError = true)]
        static extern SMART_QUERY_RESULT GetInternalDriveSmartAttributes(int physicalDriveId, byte target, IntPtr smart);

        [DllImport("HomeServerSMART.Utility.dll", SetLastError = true)]
        static extern SMART_QUERY_RESULT GetInternalDriveSmartThresholds(int physicalDriveId, byte target, IntPtr smart);

        [DllImport("HomeServerSMART.Utility.dll", SetLastError = true)]
        static extern SMART_QUERY_RESULT GetUsbDriveSmartAttributes(int physicalDriveId, byte target, COMMAND_TYPE cmdType, IntPtr smart);

        [DllImport("HomeServerSMART.Utility.dll", SetLastError = true)]
        static extern SMART_QUERY_RESULT GetUsbDriveSmartThresholds(int physicalDriveId, byte target, COMMAND_TYPE cmdType, IntPtr smart);

        [DllImport("HomeServerSMART.Utility.dll", SetLastError = true)]
        static extern SMART_QUERY_RESULT GetSiIDriveSmartAttributes(int physicalDriveId, IntPtr smart);

        [DllImport("HomeServerSMART.Utility.dll", SetLastError = true)]
        static extern SMART_QUERY_RESULT GetScsiDriveSmartAttributes(int scsiPort, int scsiTargetId, IntPtr asi, IntPtr smart);

        [DllImport("HomeServerSMART.Utility.dll", SetLastError = true)]
        static extern SMART_QUERY_RESULT GetScsiDriveSmartThresholds(int scsiPort, int scsiTargetId, IntPtr asi, IntPtr smart);

        /// <summary>
        /// Requests an internal disk drive to return its identity information. Collects the SSD, TRIM and RPM information from the disk. Disks
        /// that return 1 for RPM are SSDs. Also determines whether or not TRIM is supported for SSDs.
        /// </summary>
        /// <param name="physicalDrivePath">The physical drive path to test (\\.\PHYSICALDRIVEx).</param>
        /// <param name="target">The disk target to test. If unsure specify 0xA0 (GlobalConstants.DEFAULT_TARGET_ID).</param>
        /// <param name="isSsd">Output parameter is set to true if the disk is an SSD; false for a Winchester.</param>
        /// <param name="isTrimSupported">Output parameter is set to true if an SSD supports TRIM; false for all others.</param>
        /// <param name="rpm">Revolutions per minute of the disk spindle. SSDs return 1.</param>
        /// <returns>true if the disk identity data was successfully collected; false if the operation failed.</returns>
        public static bool GetInternalDriveIdentity(String physicalDrivePath, byte target, out bool isSsd, out bool isTrimSupported, out UInt32 rpm)
        {
            IDENTIFY_DEVICE_RESULT result = IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_PENDING;
            String exceptionMessage = String.Empty;
            return GetInternalDriveIdentity(physicalDrivePath, target, out isSsd, out isTrimSupported, out rpm, out result, out exceptionMessage);
        }

        /// <summary>
        /// Requests a disk drive to return its identity information. Collects the SSD, TRIM and RPM information from the disk. Disks
        /// that return 1 for RPM are SSDs. Also determines whether or not TRIM is supported for SSDs. This overload should be called
        /// only when debugging details need to be collected.
        /// </summary>
        /// <param name="physicalDrivePath">The physical drive path to test (\\.\PHYSICALDRIVEx).</param>
        /// <param name="target">The disk target to test. If unsure specify 0xA0 (GlobalConstants.DEFAULT_TARGET_ID).</param>
        /// <param name="isSsd">Output parameter is set to true if the disk is an SSD; false for a Winchester.</param>
        /// <param name="isTrimSupported">Output parameter is set to true if an SSD supports TRIM; false for all others.</param>
        /// <param name="rpm">Revolutions per minute of the disk spindle. SSDs return 1.</param>
        /// <param name="result">Debug output - gets the IDENTIFY_DEVICE_RESULT control code.</param>
        /// <param name="exceptionMessage">Debug output - gets the exception message text, if any is available.</param>
        /// <returns>true if the disk identity data was successfully collected; false if the operation failed.</returns>
        public static bool GetInternalDriveIdentity(String physicalDrivePath, byte target, out bool isSsd, out bool isTrimSupported, out UInt32 rpm,
            out IDENTIFY_DEVICE_RESULT result, out String exceptionMessage)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetInternalDriveIdentity");
            rpm = 65535;
            isSsd = false;
            isTrimSupported = false;
            IDENTIFY_DEVICE_RESULT ok = IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_PENDING;
            exceptionMessage = String.Empty;

            // Physical drive string is (without quotes) "\\.\PHYSICALDRIVEx" so we want the index.
            // Index is as follows =====================> 012345678901234567 (17)

            int physicalDriveId = 65535;

            try
            {
                String driveId = physicalDrivePath.Substring(17);
                SiAuto.Main.LogMessage("Parse integer drive ID from " + physicalDrivePath + ", substring(17) = " + driveId);
                physicalDriveId = Int32.Parse(driveId);
            }
            catch
            {
                result = ok;
                exceptionMessage = "Cannot map physical drive ID; conversion failed.";
                SiAuto.Main.LogColored(System.Drawing.Color.Red, "Drive ID parse failed.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetInternalDriveIdentity");
                return false;
            }

            try
            {
                IDENTITY_DATA data = new IDENTITY_DATA();
                uint dataSize = (uint)Marshal.SizeOf(typeof(IDENTITY_DATA));
                IntPtr pointer = Marshal.AllocHGlobal((int)dataSize);
                Marshal.StructureToPtr(data, pointer, true);
                SiAuto.Main.LogMessage("[P/Invoke] IdentifyInternalDriveRpm");
                ok = IdentifyInternalDriveRpm(physicalDriveId, target, pointer);
                SiAuto.Main.LogObjectValue("[P/Invoke] Return value", ok);

                if (ok == IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_RESULT_OK)
                {
                    IDENTITY_DATA identityData = (IDENTITY_DATA)Marshal.PtrToStructure(pointer, typeof(IDENTITY_DATA));
                    rpm = identityData.NominalMediaRotationRate;
                    String dataSetManagement = Convert.ToString(identityData.DataSetManagement, 2);
                    
                    // Get model
                    String tempModel = String.Empty;
                    String model = String.Empty;
                    foreach (char c in identityData.Model)
                    {
                        tempModel += c;
                    }
                    tempModel = tempModel.Trim();
                    int leader = 1;
                    int follower = 0;
                    while (leader < tempModel.Length)
                    {
                        model += tempModel.Substring(leader, 1) + tempModel.Substring(follower, 1);
                        leader = leader + 2;
                        follower = follower + 2;
                    }
                    if (leader == tempModel.Length) // odd number in string
                    {
                        model += tempModel.Substring(follower); // pick up the last character
                    }

                    // Get serial
                    String tempSerial = String.Empty;
                    String serialNumber = String.Empty;
                    foreach (char c in identityData.SerialNumber)
                    {
                        tempSerial += c;
                    }
                    tempSerial = tempSerial.Trim();
                    leader = 1;
                    follower = 0;
                    while (leader < tempSerial.Length)
                    {
                        serialNumber += tempSerial.Substring(leader, 1) + tempSerial.Substring(follower, 1);
                        leader = leader + 2;
                        follower = follower + 2;
                    }
                    if (leader == tempSerial.Length) // odd number in string
                    {
                        serialNumber += tempSerial.Substring(follower); // pick up the last character
                    }

                    // Get firmware
                    String tempFirmware = String.Empty;
                    String firmwareRevision = String.Empty;
                    foreach (char c in identityData.FirmwareRev)
                    {
                        tempFirmware += c;
                    }
                    tempFirmware = tempFirmware.Trim();
                    leader = 1;
                    follower = 0;
                    while (leader < tempFirmware.Length)
                    {
                        firmwareRevision += tempFirmware.Substring(leader, 1) + tempFirmware.Substring(follower, 1);
                        leader = leader + 2;
                        follower = follower + 2;
                    }
                    if (leader == tempFirmware.Length) // odd number in string
                    {
                        firmwareRevision += tempFirmware.Substring(follower); // pick up the last character
                    }

                    SiAuto.Main.LogObjectValue("IDENTITY_DATA", identityData);
                    SiAuto.Main.LogMessage("rpm = " + rpm.ToString());
                    SiAuto.Main.LogMessage("dataSetManagement = " + dataSetManagement);
                    SiAuto.Main.LogMessage("model", tempModel);
                    SiAuto.Main.LogMessage("serialNumber", serialNumber);
                    SiAuto.Main.LogMessage("firmwareRevision", firmwareRevision);

                    if (rpm == 1)
                    {
                        SiAuto.Main.LogColored(System.Drawing.Color.HotPink, "[SSD Notificate] Solid State Disk detected.");
                        isSsd = true;
                    }
                    if (dataSetManagement.Substring(dataSetManagement.Length - 1) == "1")
                    {
                        SiAuto.Main.LogColored(System.Drawing.Color.HotPink, "[SSD Notificate] TRIM capable SSD detected.");
                        isTrimSupported = true;
                    }
                }
                result = ok;
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("P/Invoke identity operation failed.");
                SiAuto.Main.LogException(ex);
                ok = IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_IO_CONTROL_FALSE;
                result = ok;
                exceptionMessage = ex.Message;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetInternalDriveIdentity");
            return (ok == IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_RESULT_OK);
        }

        /// <summary>
        /// Requests a USB disk drive to return its identity information. Collects the SSD, TRIM and RPM information from the disk. Disks
        /// that return 1 for RPM are SSDs. Also determines whether or not TRIM is supported for SSDs.
        /// </summary>
        /// <param name="physicalDrivePath">The physical drive path to test (\\.\PHYSICALDRIVEx).</param>
        /// <param name="isSsd">Output parameter is set to true if the disk is an SSD; false for a Winchester.</param>
        /// <param name="isTrimSupported">Output parameter is set to true if an SSD supports TRIM; false for all others.</param>
        /// <param name="rpm">Revolutions per minute of the disk spindle. SSDs return 1.</param>
        /// <returns>true if the disk identity data was successfully collected; false if the operation failed.</returns>
        public static bool GetUsbDriveIdentity(String physicalDrivePath, out bool isSsd, out bool isTrimSupported, out UInt32 rpm)
        {
            IDENTIFY_DEVICE_RESULT result = IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_PENDING;
            String exceptionMessage = String.Empty;
            return GetUsbDriveIdentity(physicalDrivePath, out isSsd, out isTrimSupported, out rpm, out result, out exceptionMessage);
        }

        /// <summary>
        /// Requests a USB disk drive to return its identity information. Collects the SSD, TRIM and RPM information from the disk. Disks
        /// that return 1 for RPM are SSDs. Also determines whether or not TRIM is supported for SSDs. Call this overload when the
        /// COMMAND_TYPE (bridge chip manufacturer) is known, as this overload executes faster, rather than iterating all the options.
        /// </summary>
        /// <param name="physicalDrivePath">The physical drive path to test (\\.\PHYSICALDRIVEx).</param>
        /// <param name="cmdType">The COMMAND_TYPE that specifies the bridge chip to evaluate.</param>
        /// <param name="isSsd">Output parameter is set to true if the disk is an SSD; false for a Winchester.</param>
        /// <param name="isTrimSupported">Output parameter is set to true if an SSD supports TRIM; false for all others.</param>
        /// <param name="rpm">Revolutions per minute of the disk spindle. SSDs return 1.</param>
        /// <returns>true if the disk identity data was successfully collected; false if the operation failed.</returns>
        public static bool GetUsbDriveIdentity(String physicalDrivePath, COMMAND_TYPE cmdType, out bool isSsd, out bool isTrimSupported, out UInt32 rpm)
        {
            IDENTIFY_DEVICE_RESULT result = IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_PENDING;
            String exceptionMessage = String.Empty;
            return GetUsbDriveIdentity(physicalDrivePath, cmdType, out isSsd, out isTrimSupported, out rpm, out result, out exceptionMessage);
        }

        /// <summary>
        /// Requests a USB disk drive to return its identity information. Collects the SSD, TRIM and RPM information from the disk. Disks
        /// that return 1 for RPM are SSDs. Also determines whether or not TRIM is supported for SSDs. This overload should be called only
        /// when debugging details need to be collected.
        /// </summary>
        /// <param name="physicalDrivePath">The physical drive path to test (\\.\PHYSICALDRIVEx).</param>
        /// <param name="isSsd">Output parameter is set to true if the disk is an SSD; false for a Winchester.</param>
        /// <param name="isTrimSupported">Output parameter is set to true if an SSD supports TRIM; false for all others.</param>
        /// <param name="rpm">Revolutions per minute of the disk spindle. SSDs return 1.</param>
        /// <param name="result">Debug output - gets the IDENTIFY_DEVICE_RESULT control code.</param>
        /// <param name="exceptionMessage">Debug output - gets the exception message text, if any is available.</param>
        /// <returns>true if the disk identity data was successfully collected; false if the operation failed.</returns>
        public static bool GetUsbDriveIdentity(String physicalDrivePath, out bool isSsd, out bool isTrimSupported, out UInt32 rpm,
            out IDENTIFY_DEVICE_RESULT result, out String exceptionMessage)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetUsblDriveIdentity");
            rpm = 65535;
            isSsd = false;
            isTrimSupported = false;
            exceptionMessage = String.Empty;

            foreach (COMMAND_TYPE cmdType in Enum.GetValues(typeof(COMMAND_TYPE)))
            {
                if (cmdType == COMMAND_TYPE.CMD_TYPE_DEBUG || cmdType == COMMAND_TYPE.CMD_TYPE_PHYSICAL_DRIVE ||
                    cmdType == COMMAND_TYPE.CMD_TYPE_PROLIFIC || cmdType == COMMAND_TYPE.CMD_TYPE_SCSI_MINIPORT ||
                    cmdType == COMMAND_TYPE.CMD_TYPE_SILICON_IMAGE)
                {
                    continue;
                }

                IDENTIFY_DEVICE_RESULT ok = IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_PENDING;

                // Physical drive string is (without quotes) "\\.\PHYSICALDRIVEx" so we want the index.
                // Index is as follows =====================> 012345678901234567 (17)

                int physicalDriveId = 65535;

                try
                {
                    String driveId = physicalDrivePath.Substring(17);
                    SiAuto.Main.LogMessage("Parse integer drive ID from " + physicalDrivePath + ", substring(17) = " + driveId);
                    physicalDriveId = Int32.Parse(driveId);
                }
                catch
                {
                    result = ok;
                    exceptionMessage = "Cannot map physical drive ID; conversion failed.";
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "Drive ID parse failed.");
                    SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetUsblDriveIdentity");
                    return false;
                }

                try
                {
                    IDENTITY_DATA data = new IDENTITY_DATA();
                    uint dataSize = (uint)Marshal.SizeOf(typeof(IDENTITY_DATA));
                    IntPtr pointer = Marshal.AllocHGlobal((int)dataSize);
                    Marshal.StructureToPtr(data, pointer, true);
                    SiAuto.Main.LogMessage("[P/Invoke] IdentifyInternalDriveRpm");
                    ok = IdentifyUsbDriveRpm(physicalDriveId, GlobalConstants.DEFAULT_TARGET_ID, pointer, cmdType);
                    SiAuto.Main.LogObjectValue("[P/Invoke] Return value", ok);

                    if (ok == IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_RESULT_OK)
                    {
                        IDENTITY_DATA identityData = (IDENTITY_DATA)Marshal.PtrToStructure(pointer, typeof(IDENTITY_DATA));
                        rpm = identityData.NominalMediaRotationRate;
                        String dataSetManagement = Convert.ToString(identityData.DataSetManagement, 2);
                        String model = String.Empty;
                        foreach (char c in identityData.Model)
                        {
                            model += c;
                        }
                        model = model.Trim();
                        String serialNumber = String.Empty;
                        foreach (char c in identityData.SerialNumber)
                        {
                            serialNumber += c;
                        }
                        serialNumber = serialNumber.Trim();
                        String firmwareRevision = String.Empty;
                        foreach (char c in identityData.FirmwareRev)
                        {
                            firmwareRevision += c;
                        }
                        firmwareRevision = firmwareRevision.Trim();

                        SiAuto.Main.LogObjectValue("IDENTITY_DATA", identityData);
                        SiAuto.Main.LogMessage("rpm = " + rpm.ToString());
                        SiAuto.Main.LogMessage("dataSetManagement = " + dataSetManagement);
                        SiAuto.Main.LogMessage("model", model);
                        SiAuto.Main.LogMessage("serialNumber", serialNumber);
                        SiAuto.Main.LogMessage("firmwareRevision", firmwareRevision);

                        if (rpm == 1)
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.HotPink, "[SSD Notificate] Solid State Disk detected.");
                            isSsd = true;
                        }
                        if (dataSetManagement.Substring(dataSetManagement.Length - 1) == "1")
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.HotPink, "[SSD Notificate] TRIM capable SSD detected.");
                            isTrimSupported = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogError("P/Invoke identity operation failed.");
                    SiAuto.Main.LogException(ex);
                    ok = IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_IO_CONTROL_FALSE;
                    result = ok;
                    exceptionMessage = ex.Message;
                }

                if (ok == IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_RESULT_OK)
                {
                    result = ok;
                    SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetUsblDriveIdentity");
                    return true;
                }
            }
            result = IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_NULL;
            exceptionMessage = "No Error - exited foreach without a valid device match.";
            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetUsblDriveIdentity");
            return false;
        }

        /// <summary>
        /// Requests a USB disk drive to return its identity information. Collects the SSD, TRIM and RPM information from the disk. Disks
        /// that return 1 for RPM are SSDs. Also determines whether or not TRIM is supported for SSDs. Call this overload when the
        /// COMMAND_TYPE (bridge chip manufacturer) is known, as this overload executes faster, rather than iterating all the options.
        /// This overload should be called only when debugging details need to be collected.
        /// </summary>
        /// <param name="physicalDrivePath">The physical drive path to test (\\.\PHYSICALDRIVEx).</param>
        /// <param name="cmdType">The COMMAND_TYPE that specifies the bridge chip to evaluate.</param>
        /// <param name="isSsd">Output parameter is set to true if the disk is an SSD; false for a Winchester.</param>
        /// <param name="isTrimSupported">Output parameter is set to true if an SSD supports TRIM; false for all others.</param>
        /// <param name="rpm">Revolutions per minute of the disk spindle. SSDs return 1.</param>
        /// <param name="result">Debug output - gets the IDENTIFY_DEVICE_RESULT control code.</param>
        /// <param name="exceptionMessage">Debug output - gets the exception message text, if any is available.</param>
        /// <returns>true if the disk identity data was successfully collected; false if the operation failed.</returns>
        public static bool GetUsbDriveIdentity(String physicalDrivePath, COMMAND_TYPE cmdType, out bool isSsd, out bool isTrimSupported, out UInt32 rpm,
            out IDENTIFY_DEVICE_RESULT result, out String exceptionMessage)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetUsblDriveIdentity");
            rpm = 65535;
            isSsd = false;
            isTrimSupported = false;
            exceptionMessage = String.Empty;

            IDENTIFY_DEVICE_RESULT ok = IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_PENDING;

            // Physical drive string is (without quotes) "\\.\PHYSICALDRIVEx" so we want the index.
            // Index is as follows =====================> 012345678901234567 (17)

            int physicalDriveId = 65535;

            try
            {
                String driveId = physicalDrivePath.Substring(17);
                SiAuto.Main.LogMessage("Parse integer drive ID from " + physicalDrivePath + ", substring(17) = " + driveId);
                physicalDriveId = Int32.Parse(driveId);
            }
            catch
            {
                result = ok;
                exceptionMessage = "Cannot map physical drive ID; conversion failed.";
                SiAuto.Main.LogColored(System.Drawing.Color.Red, "Drive ID parse failed.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetUsblDriveIdentity");
                return false;
            }

            try
            {
                IDENTITY_DATA data = new IDENTITY_DATA();
                uint dataSize = (uint)Marshal.SizeOf(typeof(IDENTITY_DATA));
                IntPtr pointer = Marshal.AllocHGlobal((int)dataSize);
                Marshal.StructureToPtr(data, pointer, true);
                SiAuto.Main.LogMessage("[P/Invoke] IdentifyUsbDriveRpm");
                ok = IdentifyUsbDriveRpm(physicalDriveId, GlobalConstants.DEFAULT_TARGET_ID, pointer, cmdType);
                SiAuto.Main.LogObjectValue("[P/Invoke] Return value", ok);

                if (ok == IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_RESULT_OK)
                {
                    IDENTITY_DATA identityData = (IDENTITY_DATA)Marshal.PtrToStructure(pointer, typeof(IDENTITY_DATA));
                    rpm = identityData.NominalMediaRotationRate;
                    String dataSetManagement = Convert.ToString(identityData.DataSetManagement, 2);
                    String model = String.Empty;
                    foreach (char c in identityData.Model)
                    {
                        model += c;
                    }
                    model = model.Trim();
                    String serialNumber = String.Empty;
                    foreach (char c in identityData.SerialNumber)
                    {
                        serialNumber += c;
                    }
                    serialNumber = serialNumber.Trim();
                    String firmwareRevision = String.Empty;
                    foreach (char c in identityData.FirmwareRev)
                    {
                        firmwareRevision += c;
                    }
                    firmwareRevision = firmwareRevision.Trim();

                    SiAuto.Main.LogObjectValue("IDENTITY_DATA", identityData);
                    SiAuto.Main.LogMessage("rpm = " + rpm.ToString());
                    SiAuto.Main.LogMessage("dataSetManagement = " + dataSetManagement);
                    SiAuto.Main.LogMessage("model", model);
                    SiAuto.Main.LogMessage("serialNumber", serialNumber);
                    SiAuto.Main.LogMessage("firmwareRevision", firmwareRevision);

                    if (rpm == 1)
                    {
                        SiAuto.Main.LogColored(System.Drawing.Color.HotPink, "[SSD Notificate] Solid State Disk detected.");
                        isSsd = true;
                    }
                    if (dataSetManagement.Substring(dataSetManagement.Length - 1) == "1")
                    {
                        SiAuto.Main.LogColored(System.Drawing.Color.HotPink, "[SSD Notificate] TRIM capable SSD detected.");
                        isTrimSupported = true;
                    }
                }
                result = ok;
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("P/Invoke identity operation failed.");
                SiAuto.Main.LogException(ex);
                ok = IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_IO_CONTROL_FALSE;
                result = ok;
                exceptionMessage = ex.Message;
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetUsblDriveIdentity");
            return (ok == IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_RESULT_OK);
        }

        /// <summary>
        /// Gets the SMART attributes and thresholds (SMART data) from "internal" disks -- these basically being anything that
        /// isn't SCSI, FireWire or USB.  We'll regard eSATA as "internal."
        /// </summary>
        /// <param name="physicalDrivePath">The physical drive string from WMI, which is of the format \\.\PHYSICALDRIVEx</param>
        /// <param name="target">The target ID of the disk. Probably will be 0xA0 (160) -- use GlobalConstants.DEFAULT_TARGET_ID.</param>
        /// <param name="smartData">512-byte array with the SMART attribute data.</param>
        /// <param name="smartThreshold">512-byte array with the SMART threshold data.</param>
        /// <param name="failurePredicted">Flag that indicates whether or not WMI is showing "Failure Predicted."</param>
        /// <returns>true if we successfully got the SMART data; false otherwise.</returns>
        public static bool GetInternalSmartData(String physicalDrivePath, byte target, out byte[] smartData, out byte[] smartThreshold, out bool failurePredicted)
        {
            return GetInternalSmartData(physicalDrivePath, GlobalConstants.DEFAULT_TARGET_ID, false, null, false, out smartData,
                out smartThreshold, out failurePredicted);
        }

        /// <summary>
        /// Gets the SMART attributes and thresholds (SMART data) from "internal" disks -- these basically being anything that
        /// isn't SCSI, FireWire or USB.  We'll regard eSATA as "internal."
        /// </summary>
        /// <param name="physicalDrivePath">The physical drive string from WMI, which is of the format \\.\PHYSICALDRIVEx</param>
        /// <param name="target">The target ID of the disk. Probably will be 0xA0 (160) -- use GlobalConstants.DEFAULT_TARGET_ID.</param>
        /// <param name="isDiskSiI">Flag that indicates if the disk is attached to an SiI controller.</param>
        /// <param name="pnpDeviceId">PNP Device ID of the disk drive.</param>
        /// <param name="advancedSii">Set to true to use WMI if P/Invoke data fetch fails; false does not attempt WMI.</param>
        /// <param name="debugMode">Set to true to enable debug logging.</param>
        /// <param name="advancedSii">Set to true to use an alternate method to get SiI SMART data (no WMI; all thresholds will be zero).
        /// This parameter has no effect if isDiskSiI is false.</param>        
        /// <param name="smartData">512-byte array with the SMART attribute data.</param>
        /// <param name="smartThreshold">512-byte array with the SMART threshold data.</param>
        /// <param name="failurePredicted">Flag that indicates whether or not WMI is showing "Failure Predicted."</param>
        /// <returns>true if we successfully got the SMART data; false otherwise.</returns>
        public static bool GetInternalSmartData(String physicalDrivePath, byte target, bool isDiskSiI, String pnpDeviceId,
            bool advancedSii, out byte[] smartData, out byte[] smartThreshold, out bool failurePredicted)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetInternalSmartData");
            bool ok = false;
            smartData = null;
            smartThreshold = null;
            // TODO: Get the below from WMI!
            failurePredicted = false;

            // Physical drive string is (without quotes) "\\.\PHYSICALDRIVEx" so we want the index.
            // Index is as follows =====================> 012345678901234567 (17)

            int physicalDriveId = -1;

            try
            {
                String driveId = physicalDrivePath.Substring(17);
                SiAuto.Main.LogMessage("Parse integer drive ID from " + physicalDrivePath + ", substring(17) = " + driveId);
                physicalDriveId = Int32.Parse(driveId);
            }
            catch
            {
                SiAuto.Main.LogColored(System.Drawing.Color.Red, "Drive ID parse failed.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetInternalSmartData");
                return false;
            }

            if (isDiskSiI)
            {
                SiAuto.Main.LogColored(System.Drawing.Color.Orange, "Disk is attached to SiI; using SiI SMaRT retrieval.");
                // The disk is attached to a Silicon Image SiI controller so we use the interop calls for the attributes, and WMI
                // for the thresholds.
                if (GetSiliconImageSmartAttributes(physicalDriveId, out smartData))
                {
                    SiAuto.Main.LogMessage("Retrieved SiI SMART attributes.");
                    // Got the SiI attributes, now get the thresholds.
                    ok = GetSiliconImageSmartThresholds(pnpDeviceId, out smartThreshold);

                    if (!ok && advancedSii)
                    {
                        SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Advanced SiI] Failed to get thresholds; using zeros for thresholds.");
                        smartThreshold = new byte[512];
                        for (int i = 0; i < 512; i++)
                        {
                            smartThreshold[i] = 0;
                        }
                        ok = true;
                    }
                }
            }
            else
            {
                // The disk is not attached to an SiI controller so use the standard interop calls to get the attributes and thresholds.
                if (GetInternalSmartAttributes(physicalDriveId, target, out smartData))
                {
                    SiAuto.Main.LogMessage("Retrieved internal SMART attributes.");
                    // Got the SiI attributes, now get the thresholds.
                    ok = GetInternalSmartThresholds(physicalDriveId, target, out smartThreshold);
                }
            }

            SiAuto.Main.LogBool("GetInternalSmartData", ok);
            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetInternalSmartData");
            return ok;
        }

        /// <summary>
        /// Gets the SMART attributes and thresholds (SMART data) from USB disks. If no COMMAND_TYPE is specified, the default of
        /// COMMAND_TYPE.CMD_TYPE_SAT is used.
        /// </summary>
        /// <param name="physicalDrivePath">The physical drive string from WMI, which is of the format \\.\PHYSICALDRIVEx</param>
        /// <param name="smartData">512-byte array with the SMART attribute data.</param>
        /// <param name="smartThreshold">512-byte array with the SMART threshold data.</param>
        /// <param name="failurePredicted">Flag that indicates whether or not WMI is showing "Failure Predicted."</param>
        /// <returns>true if we successfully got the SMART data; false otherwise.</returns>
        public static bool GetUsbSmartData(String physicalDrivePath, out byte[] smartData, out byte[] smartThreshold, out bool failurePredicted)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetUsbSmartData");
            foreach (COMMAND_TYPE cmdType in Enum.GetValues(typeof(COMMAND_TYPE)))
            {
                if (cmdType == COMMAND_TYPE.CMD_TYPE_DEBUG || cmdType == COMMAND_TYPE.CMD_TYPE_PHYSICAL_DRIVE ||
                    cmdType == COMMAND_TYPE.CMD_TYPE_PROLIFIC || cmdType == COMMAND_TYPE.CMD_TYPE_SCSI_MINIPORT ||
                    cmdType == COMMAND_TYPE.CMD_TYPE_SILICON_IMAGE)
                {
                    continue;
                }

                bool ok = GetUsbSmartData(physicalDrivePath, cmdType, out smartData, out smartThreshold, out failurePredicted);
                if (ok)
                {
                    SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetUsbSmartData");
                    return ok;
                }
            }
            smartData = new byte[512];
            smartThreshold = new byte[512];
            failurePredicted = false;
            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetUsbSmartData");
            return false;
        }

        /// <summary>
        /// Gets the SMART attributes and thresholds (SMART data) from USB disks. If no COMMAND_TYPE is specified, the default of
        /// COMMAND_TYPE.CMD_TYPE_SAT is used.
        /// </summary>
        /// <param name="physicalDrivePath">The physical drive string from WMI, which is of the format \\.\PHYSICALDRIVEx</param>
        /// <param name="diskCommandType">The COMMAND_TYPE that specifies what type of USB bridge chip such as Sunplus, JMicron, IO Data, etc. If
        /// you're not sure specify COMMAND_TYPE.CMD_TYPE_SAT. Note that only SAT, SUNPLUS, LOGITEC, IO_DATA, JMICRON and CYPRESS should be used
        /// with USB disks. All others will fail with SMART_QUERY_EXCEPTION being returned.</param>
        /// <param name="smartData">512-byte array with the SMART attribute data.</param>
        /// <param name="smartThreshold">512-byte array with the SMART threshold data.</param>
        /// <param name="failurePredicted">Flag that indicates whether or not WMI is showing "Failure Predicted."</param>
        /// <returns>true if we successfully got the SMART data; false otherwise.</returns>
        public static bool GetUsbSmartData(String physicalDrivePath, COMMAND_TYPE diskCommandType, out byte[] smartData, out byte[] smartThreshold,
            out bool failurePredicted)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetUsbSmartData");
            bool ok = false;
            smartData = null;
            smartThreshold = null;
            // TODO: get below from WMI!
            failurePredicted = false;

            // Physical drive string is (without quotes) "\\.\PHYSICALDRIVEx" so we want the index.
            // Index is as follows =====================> 012345678901234567 (17)

            int physicalDriveId = -1;

            try
            {
                String driveId = physicalDrivePath.Substring(17);
                SiAuto.Main.LogMessage("Parse integer drive ID from " + physicalDrivePath + ", substring(17) = " + driveId);
                physicalDriveId = Int32.Parse(driveId);
            }
            catch
            {
                SiAuto.Main.LogColored(System.Drawing.Color.Red, "Drive ID parse failed.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetUsbSmartData");
                return false;
            }

            if (GetUsbSmartAttributes(physicalDriveId, GlobalConstants.DEFAULT_TARGET_ID, diskCommandType, out smartData))
            {
                SiAuto.Main.LogMessage("Retrieved USB SMART attributes.");
                ok = GetUsbSmartThresholds(physicalDriveId, GlobalConstants.DEFAULT_TARGET_ID, diskCommandType, out smartThreshold);
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetUsbSmartData");
            return ok;
        }

        /// <summary>
        /// Gets the SMART attributes and thresholds (SMART data) from SCSI disks.
        /// </summary>
        /// <param name="scsiPort">SCSI port number.</param>
        /// <param name="scsiTarget">SCSI target ID.</param>
        /// <param name="smartData">512-byte array with the SMART attribute data.</param>
        /// <param name="smartThreshold">512-byte array with the SMART threshold data.</param>
        /// <param name="failurePredicted">Flag that indicates whether or not WMI is showing "Failure Predicted."</param>
        /// <returns>true if we successfully got the SMART data; false otherwise.</returns>
        public static bool GetScsiSmartData(int scsiPort, int scsiTarget, out byte[] smartData, out byte[] smartThreshold, out bool failurePredicted)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetScsiSmartData");
            bool ok = false;
            smartData = null;
            smartThreshold = null;
            // TODO: get below from WMI!
            failurePredicted = false;

            if (GetScsiSmartAttributes(scsiPort, scsiTarget, out smartData))
            {
                SiAuto.Main.LogMessage("Retrieved SCSI SMART attributes.");
                ok = GetScsiSmartThresholds(scsiPort, scsiTarget, out smartThreshold);
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetScsiSmartData");
            return ok;
        }

        /// <summary>
        /// Gets the SMART attributes and thresholds (SMART data) using WMI.
        /// </summary>
        /// <param name="physicalDrivePath">The physical drive string from WMI, which is of the format \\.\PHYSICALDRIVEx</param>
        /// <param name="smartData">512-byte array with the SMART attribute data.</param>
        /// <param name="smartThreshold">512-byte array with the SMART threshold data.</param>
        /// <param name="failurePredicted">Flag that indicates whether or not WMI is showing "Failure Predicted."</param>
        /// <returns>true if we successfully got the SMART data; false otherwise.</returns>
        public static bool GetWmiSmartData(String physicalDrivePath, out byte[] smartData, out byte[] smartThreshold, out bool failurePredicted)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetWmiSmartData");
            bool ok = false;
            smartData = null;
            smartThreshold = null;
            // TODO: get below from WMI!
            failurePredicted = false;

            if (GetWmiSmartAttributes(physicalDrivePath, out smartData))
            {
                SiAuto.Main.LogMessage("Retrieved WMI SMART attributes.");
                ok = GetWmiSmartThresholds(physicalDrivePath, out smartThreshold);
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetWmiSmartData");
            return ok;
        }

        /// <summary>
        /// Performs the P/Invoke call to get SMART attributes from a USB disk.
        /// </summary>
        /// <param name="physicalDriveId">The physical drive number.</param>
        /// <param name="target">The target ID of the disk (probably will be 0xA0).</param>
        /// <param name="dataBuffer">The data buffer (probably will be 512-byte array) that is returned by the P/Invoke call.</param>
        /// <returns>true if we got the SMART attributes; false otherwise.</returns>
        private static bool GetUsbSmartAttributes(int physicalDriveId, byte target, COMMAND_TYPE diskCommandType, out byte[] dataBuffer)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetUsbSmartAttributes");
            SMART_QUERY_RESULT ok = SMART_QUERY_RESULT.SMART_QUERY_PENDING;

            try
            {
                SMART_DATA_UCHAR data = new SMART_DATA_UCHAR();
                uint dataSize = (uint)Marshal.SizeOf(typeof(SMART_DATA_UCHAR));
                IntPtr pointer = Marshal.AllocHGlobal((int)dataSize);
                Marshal.StructureToPtr(data, pointer, true);
                SiAuto.Main.LogMessage("[P/Invoke] GetUsbDriveSmartAttributes physicalDriveId=" + physicalDriveId.ToString() + ", target=" +
                    target.ToString() + ", diskCommandType=" + diskCommandType.ToString());
                ok = GetUsbDriveSmartAttributes(physicalDriveId, target, diskCommandType, pointer);
                SiAuto.Main.LogObjectValue("[P/Invoke] Return value", ok);

                SMART_DATA_UCHAR smartData = (SMART_DATA_UCHAR)Marshal.PtrToStructure(pointer, typeof(SMART_DATA_UCHAR));
                dataBuffer = smartData.Data;
            }
            catch (Exception ex)
            {
                dataBuffer = null;
                SiAuto.Main.LogWarning("[P/Invoke Error] Operation failed");
                SiAuto.Main.LogException(ex);
                ok = SMART_QUERY_RESULT.SMART_QUERY_IO_CONTROL_FALSE;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetUsbSmartAttributes");
            return (ok == SMART_QUERY_RESULT.SMART_QUERY_RESULT_OK);
        }

        /// <summary>
        /// Performs the P/Invoke call to get SMART thresholds from a USB disk.
        /// </summary>
        /// <param name="physicalDriveId">The physical drive number.</param>
        /// <param name="target">The target ID of the disk (probably will be 0xA0).</param>
        /// <param name="dataBuffer">The data buffer (probably will be 512-byte array) that is returned by the P/Invoke call.</param>
        /// <returns>true if we got the SMART thresholds; false otherwise.</returns>
        private static bool GetUsbSmartThresholds(int physicalDriveId, byte target, COMMAND_TYPE diskCommandType, out byte[] dataBuffer)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetUsbSmartThresholds");
            SMART_QUERY_RESULT ok = SMART_QUERY_RESULT.SMART_QUERY_PENDING;

            try
            {
                SMART_DATA_UCHAR data = new SMART_DATA_UCHAR();
                uint dataSize = (uint)Marshal.SizeOf(typeof(SMART_DATA_UCHAR));
                IntPtr pointer = Marshal.AllocHGlobal((int)dataSize);
                Marshal.StructureToPtr(data, pointer, true);
                SiAuto.Main.LogMessage("[P/Invoke] GetUsbDriveSmartThresholds physicalDriveId=" + physicalDriveId.ToString() + ", target=" +
                    target.ToString() + ", diskCommandType=" + diskCommandType.ToString());
                ok = GetUsbDriveSmartThresholds(physicalDriveId, target, diskCommandType, pointer);
                SiAuto.Main.LogObjectValue("[P/Invoke] Return value", ok);

                SMART_DATA_UCHAR smartData = (SMART_DATA_UCHAR)Marshal.PtrToStructure(pointer, typeof(SMART_DATA_UCHAR));
                dataBuffer = smartData.Data;
            }
            catch (Exception ex)
            {
                dataBuffer = null;
                SiAuto.Main.LogWarning("[P/Invoke Error] Operation failed");
                SiAuto.Main.LogException(ex);
                ok = SMART_QUERY_RESULT.SMART_QUERY_IO_CONTROL_FALSE;
            }
            finally
            {

            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetUsbSmartThresholds");
            return (ok == SMART_QUERY_RESULT.SMART_QUERY_RESULT_OK);
        }

        /// <summary>
        /// Performs the P/Invoke call to get SMART attributes from an internal IDE/SATA/eSATA disk.
        /// </summary>
        /// <param name="physicalDriveId">The physical drive number.</param>
        /// <param name="target">The target ID of the disk (probably will be 0xA0).</param>
        /// <param name="dataBuffer">The data buffer (probably will be 512-byte array) that is returned by the P/Invoke call.</param>
        /// <returns>true if we got the SMART attributes; false otherwise.</returns>
        private static bool GetInternalSmartAttributes(int physicalDriveId, byte target, out byte[] dataBuffer)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetInternalSmartAttributes");
            SMART_QUERY_RESULT ok = SMART_QUERY_RESULT.SMART_QUERY_PENDING;

            try
            {
                SMART_DATA data = new SMART_DATA();
                uint dataSize = (uint)Marshal.SizeOf(typeof(SMART_DATA));
                IntPtr pointer = Marshal.AllocHGlobal((int)dataSize);
                Marshal.StructureToPtr(data, pointer, true);
                SiAuto.Main.LogMessage("[P/Invoke] GetInternalDriveSmartAttributes physicalDriveId=" + physicalDriveId.ToString() + ", target=" +
                    target.ToString());
                ok = GetInternalDriveSmartAttributes(physicalDriveId, target, pointer);
                SiAuto.Main.LogObjectValue("[P/Invoke] Return value", ok);

                SMART_DATA smartData = (SMART_DATA)Marshal.PtrToStructure(pointer, typeof(SMART_DATA));
                dataBuffer = smartData.Data;
            }
            catch (Exception ex)
            {
                dataBuffer = null;
                SiAuto.Main.LogWarning("[P/Invoke Error] Operation failed");
                SiAuto.Main.LogException(ex);
                ok = SMART_QUERY_RESULT.SMART_QUERY_IO_CONTROL_FALSE;
            }
            finally
            {

            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetInternalSmartAttributes");
            return (ok == SMART_QUERY_RESULT.SMART_QUERY_RESULT_OK);
        }

        /// <summary>
        /// Performs the P/Invoke call to get SMART thresholds from an internal IDE/SATA/eSATA disk.
        /// </summary>
        /// <param name="physicalDriveId">The physical drive number.</param>
        /// <param name="target">The target ID of the disk (probably will be 0xA0).</param>
        /// <param name="dataBuffer">The data buffer (probably will be 512-byte array) that is returned by the P/Invoke call.</param>
        /// <returns>true if we got the SMART thresholds; false otherwise.</returns>
        private static bool GetInternalSmartThresholds(int physicalDriveId, byte target, out byte[] dataBuffer)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetInternalSmartThresholds");
            SMART_QUERY_RESULT ok = SMART_QUERY_RESULT.SMART_QUERY_PENDING;

            try
            {
                SMART_DATA data = new SMART_DATA();
                uint dataSize = (uint)Marshal.SizeOf(typeof(SMART_DATA));
                IntPtr pointer = Marshal.AllocHGlobal((int)dataSize);
                Marshal.StructureToPtr(data, pointer, true);
                SiAuto.Main.LogMessage("[P/Invoke] GetInternalDriveSmartThresholds physicalDriveId=" + physicalDriveId.ToString() + ", target=" +
                    target.ToString());
                ok = GetInternalDriveSmartThresholds(physicalDriveId, target, pointer);
                SiAuto.Main.LogObjectValue("[P/Invoke] Return value", ok);

                SMART_DATA smartData = (SMART_DATA)Marshal.PtrToStructure(pointer, typeof(SMART_DATA));
                dataBuffer = smartData.Data;
            }
            catch (Exception ex)
            {
                dataBuffer = null;
                SiAuto.Main.LogWarning("[P/Invoke Error] Operation failed");
                SiAuto.Main.LogException(ex);
                ok = SMART_QUERY_RESULT.SMART_QUERY_IO_CONTROL_FALSE;
            }
            finally
            {

            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetInternalSmartThresholds");
            return (ok == SMART_QUERY_RESULT.SMART_QUERY_RESULT_OK);
        }

        /// <summary>
        /// Gets the SMART attributes for a disk attached to a Silicon Image SiI controller.
        /// </summary>
        /// <param name="physicalDriveId">The physical device ID.</param>
        /// <param name="dataBuffer">output byte array containing SMART data.</param>
        /// <returns>true if attributes successfully fetched; false otherwise.</returns>
        private static bool GetSiliconImageSmartAttributes(int physicalDriveId, out byte[] dataBuffer)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetSiliconImageSmartAttributes");
            SMART_QUERY_RESULT ok = SMART_QUERY_RESULT.SMART_QUERY_PENDING;

            try
            {
                SMART_DATA_SII data = new SMART_DATA_SII();
                uint dataSize = (uint)Marshal.SizeOf(typeof(SMART_DATA_SII));
                IntPtr pointer = Marshal.AllocHGlobal((int)dataSize);
                Marshal.StructureToPtr(data, pointer, true);
                SiAuto.Main.LogMessage("[P/Invoke] GetSiIDriveSmartAttributes physicalDriveId=" + physicalDriveId.ToString());
                ok = GetSiIDriveSmartAttributes(physicalDriveId, pointer);
                SiAuto.Main.LogObjectValue("[P/Invoke] Return value", ok);

                SMART_DATA_SII smartData = (SMART_DATA_SII)Marshal.PtrToStructure(pointer, typeof(SMART_DATA_SII));
                dataBuffer = smartData.VendorSpecific;
            }
            catch (Exception ex)
            {
                dataBuffer = null;
                SiAuto.Main.LogWarning("[P/Invoke Error] Operation failed");
                SiAuto.Main.LogException(ex);
                ok = SMART_QUERY_RESULT.SMART_QUERY_IO_CONTROL_FALSE;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetSiliconImageSmartAttributes");
            return (ok == SMART_QUERY_RESULT.SMART_QUERY_RESULT_OK);
        }

        /// <summary>
        /// Gets the SMART thresholds for a disk attached to a Silicon Image SiI controller.  This
        /// needs to be done through WMI.
        /// </summary>
        /// <param name="pnpDeviceId">PNP device ID of the disk to evaluate.</param>
        /// <param name="smartThreshold">output byte array containing SMART thresholds.</param>
        /// <returns>true if thresholds successfully fetched; false otherwise.</returns>
        private static bool GetSiliconImageSmartThresholds(String pnpDeviceId, out byte[] smartThreshold)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetSiliconImageSmartThresholds");
            smartThreshold = null;

            try
            {
                ManagementObjectSearcher WMISearch = new ManagementObjectSearcher();
                //MSStorageDriver is in \root\wmi namespace.
                WMISearch.Scope = new ManagementScope(@"\root\wmi");

                // Must change \\ to \\\\ or WMI call will die horribly.
                String testDisk = pnpDeviceId.Replace("\\", "\\\\");

                // WMI is slow so rather than querying all the MSStorageDriver objects, query for the specific disk.
                WMISearch.Query = new ObjectQuery("select VendorSpecific from MSStorageDriver_FailurePredictThresholds where InstanceName=\"" + testDisk + "_0\"");
                ManagementObjectCollection FailDataSet = WMISearch.Get();
                foreach (ManagementObject attribute in FailDataSet)
                {
                    // If nothing is returned the foreach will have nothing to iterate.
                    smartThreshold = (Byte[])attribute.Properties["VendorSpecific"].Value;
                    SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetSiliconImageSmartThresholds");
                    return true;
                }
            }
            catch (Exception)
            {
                smartThreshold = null;
                SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetSiliconImageSmartThresholds");
                return false;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetSiliconImageSmartThresholds");
            return false;
        }

        /// <summary>
        /// Performs the P/Invoke call to get SMART attributes from a SCSI disk.
        /// </summary>
        /// <param name="scsiPort">The SCSI port number.</param>
        /// <param name="scsiTarget">The SCSI target number.</param>
        /// <param name="dataBuffer">The data buffer (probably will be 512-byte array) that is returned by the P/Invoke call.</param>
        /// <returns>true if we got the SMART attributes; false otherwise.</returns>
        private static bool GetScsiSmartAttributes(int scsiPort, int scsiTarget, out byte[] dataBuffer)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetScsiSmartAttributes");
            SMART_QUERY_RESULT ok = SMART_QUERY_RESULT.SMART_QUERY_PENDING;

            try
            {
                ATA_SMART_INFO asi = new ATA_SMART_INFO();
                SMART_DATA data = new SMART_DATA();
                uint asiSize = (uint)Marshal.SizeOf(typeof(ATA_SMART_INFO));
                uint dataSize = (uint)Marshal.SizeOf(typeof(SMART_DATA));
                IntPtr asiPtr = Marshal.AllocHGlobal((int)asiSize);
                IntPtr pointer = Marshal.AllocHGlobal((int)dataSize);
                Marshal.StructureToPtr(asi, asiPtr, true);
                Marshal.StructureToPtr(data, pointer, true);
                SiAuto.Main.LogMessage("[P/Invoke] GetScsiDriveSmartAttributes scsiPort=" + scsiPort.ToString() + ", scsiTarget=" +
                    scsiTarget.ToString());
                ok = GetScsiDriveSmartAttributes(scsiPort, scsiTarget, asiPtr, pointer);
                SiAuto.Main.LogObjectValue("[P/Invoke] Return value", ok);

                SMART_DATA smartData = (SMART_DATA)Marshal.PtrToStructure(pointer, typeof(SMART_DATA));
                dataBuffer = smartData.Data;
            }
            catch (Exception ex)
            {
                dataBuffer = null;
                SiAuto.Main.LogWarning("[P/Invoke Error] Operation failed");
                SiAuto.Main.LogException(ex);
                ok = SMART_QUERY_RESULT.SMART_QUERY_IO_CONTROL_FALSE;
            }
            finally
            {

            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetScsiSmartAttributes");
            return (ok == SMART_QUERY_RESULT.SMART_QUERY_RESULT_OK);
        }

        /// <summary>
        /// Performs the P/Invoke call to get SMART thresholds from a SCSI disk.
        /// </summary>
        /// <param name="scsiPort">The SCSI port number.</param>
        /// <param name="scsiTarget">The SCSI target number.</param>
        /// <param name="dataBuffer">The data buffer (probably will be 512-byte array) that is returned by the P/Invoke call.</param>
        /// <returns>true if we got the SMART thresholds; false otherwise.</returns>
        private static bool GetScsiSmartThresholds(int scsiPort, int scsiTarget, out byte[] dataBuffer)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetScsiSmartThresholds");
            SMART_QUERY_RESULT ok = SMART_QUERY_RESULT.SMART_QUERY_PENDING;

            try
            {
                ATA_SMART_INFO asi = new ATA_SMART_INFO();
                SMART_DATA data = new SMART_DATA();
                uint asiSize = (uint)Marshal.SizeOf(typeof(ATA_SMART_INFO));
                uint dataSize = (uint)Marshal.SizeOf(typeof(SMART_DATA));
                IntPtr asiPtr = Marshal.AllocHGlobal((int)asiSize);
                IntPtr pointer = Marshal.AllocHGlobal((int)dataSize);
                Marshal.StructureToPtr(asi, asiPtr, true);
                Marshal.StructureToPtr(data, pointer, true);
                SiAuto.Main.LogMessage("[P/Invoke] GetScsiDriveSmartAttributes scsiPort=" + scsiPort.ToString() + ", scsiTarget=" +
                    scsiTarget.ToString());
                ok = GetScsiDriveSmartThresholds(scsiPort, scsiTarget, asiPtr, pointer);
                SiAuto.Main.LogObjectValue("[P/Invoke] Return value", ok);

                SMART_DATA smartData = (SMART_DATA)Marshal.PtrToStructure(pointer, typeof(SMART_DATA));
                dataBuffer = smartData.Data;
            }
            catch (Exception ex)
            {
                dataBuffer = null;
                SiAuto.Main.LogWarning("[P/Invoke Error] Operation failed");
                SiAuto.Main.LogException(ex);
                ok = SMART_QUERY_RESULT.SMART_QUERY_IO_CONTROL_FALSE;
            }
            finally
            {

            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetScsiSmartThresholds");
            return (ok == SMART_QUERY_RESULT.SMART_QUERY_RESULT_OK);
        }

        /// <summary>
        /// Gets the SMART attributes for the specified disk from WMI, if WMI has that data available.
        /// </summary>
        /// <param name="physicalDrivePath">Path to the physical drive.</param>
        /// <param name="dataBuffer">The data buffer (probably will be 512-byte array) that is returned by the WMI call.</param>
        /// <returns>true if the SMART data was successfully retrieved from WMI; false otherwise.</returns>
        private static bool GetWmiSmartAttributes(String physicalDrivePath, out byte[] dataBuffer)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetWmiSmartAttributes");
            SMART_QUERY_RESULT ok = SMART_QUERY_RESULT.SMART_QUERY_PENDING;
            dataBuffer = null;

            try
            {
                ManagementObjectSearcher WMISearch = new ManagementObjectSearcher();
                WMISearch.Scope = new ManagementScope(@"\root\wmi");
                // Need to convert "\\" to "\\\\" or the WMI call will fail.
                String testDisk = physicalDrivePath.Replace("\\", "\\\\");

                // Not sure why but in WMI the instance name field has a _0 tacked on the end of it.
                WMISearch.Query = new ObjectQuery("select VendorSpecific from MSStorageDriver_ATAPISmartData where InstanceName=\"" + testDisk + "_0\"");
                // MSStorageDriver_FailurePredictData can be used in place of ATAPISmartData but the latter provides additional properties.
                ManagementObjectCollection FailDataSet = WMISearch.Get();
                foreach (ManagementObject attribute in FailDataSet)
                {
                    // We got what we needed, so save it, set the OK flag and break out of the foreach.
                    dataBuffer = (byte[])attribute.Properties["VendorSpecific"].Value;
                    ok = SMART_QUERY_RESULT.SMART_QUERY_RESULT_OK;
                    SiAuto.Main.LogMessage("Successfully retrieved SMART threshold WMI value.");
                    break;
                }
            }
            catch (Exception ex)
            {
                // WMI call failed; probably a "not found" or "not supported" which means the requested instance isn't in WMI so there's no data to fetch.
                dataBuffer = null;
                SiAuto.Main.LogWarning("[WMI Error] Operation failed");
                SiAuto.Main.LogException(ex);
                ok = SMART_QUERY_RESULT.SMART_QUERY_EXCEPTION;
            }
            finally
            {

            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetWmiSmartAttributes");
            return (ok == SMART_QUERY_RESULT.SMART_QUERY_RESULT_OK);
        }

        /// <summary>
        /// Gets the SMART thresholds for the specified disk from WMI, if WMI has that data available.
        /// </summary>
        /// <param name="physicalDrivePath">Path to the physical drive.</param>
        /// <param name="dataBuffer">The data buffer (probably will be 512-byte array) that is returned by the WMI call.</param>
        /// <returns>true if the SMART data was successfully retrieved from WMI; false otherwise.</returns>
        private static bool GetWmiSmartThresholds(String physicalDrivePath, out byte[] dataBuffer)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2012.Components.InteropSmart.GetWmiSmartThresholds");
            SMART_QUERY_RESULT ok = SMART_QUERY_RESULT.SMART_QUERY_PENDING;
            dataBuffer = null;

            try
            {
                ManagementObjectSearcher WMISearch = new ManagementObjectSearcher();
                WMISearch.Scope = new ManagementScope(@"\root\wmi");
                // Need to convert "\\" to "\\\\" or the WMI call will fail.
                String testDisk = physicalDrivePath.Replace("\\", "\\\\");

                // Not sure why but in WMI the instance name field has a _0 tacked on the end of it.
                WMISearch.Query = new ObjectQuery("select VendorSpecific from MSStorageDriver_FailurePredictThresholds where InstanceName=\"" + testDisk + "_0\"");
                ManagementObjectCollection FailDataSet = WMISearch.Get();
                foreach (ManagementObject attribute in FailDataSet)
                {
                    // We got what we needed, so save it, set the OK flag and break out of the foreach.
                    dataBuffer = (byte[])attribute.Properties["VendorSpecific"].Value;
                    ok = SMART_QUERY_RESULT.SMART_QUERY_RESULT_OK;
                    SiAuto.Main.LogMessage("Successfully retrieved SMART threshold WMI value.");
                    break;
                }
            }
            catch (Exception ex)
            {
                // WMI call failed; probably a "not found" or "not supported" which means the requested instance isn't in WMI so there's no data to fetch.
                dataBuffer = null;
                SiAuto.Main.LogWarning("[WMI Error] Operation failed");
                SiAuto.Main.LogException(ex);
                ok = SMART_QUERY_RESULT.SMART_QUERY_EXCEPTION;
            }
            finally
            {

            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2012.Components.InteropSmart.GetWmiSmartThresholds");
            return (ok == SMART_QUERY_RESULT.SMART_QUERY_RESULT_OK);
        }
    }
}