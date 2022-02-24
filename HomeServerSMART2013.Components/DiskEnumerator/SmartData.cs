using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartData
    {
        DataTable smartDataTable;

        public SmartData()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartData.SmartData");
            CreateDataTable();
            ClearDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartData.SmartData");
        }

        public void CreateDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartData.CreateDataTable");
            SiAuto.Main.LogMessage("Initialize data table.");
            smartDataTable = new DataTable("SMARTData");

            // Header Row (columns)
            SiAuto.Main.LogMessage("Add columns to data table.");
            smartDataTable.Columns.Add("Key", typeof(String));
            smartDataTable.Columns.Add("SmartData", typeof(byte[]));
            smartDataTable.Columns.Add("ThreshData", typeof(byte[]));
            smartDataTable.Columns.Add("FailurePredicted", typeof(bool));
            smartDataTable.Columns.Add("Path", typeof(String));
            smartDataTable.Columns.Add("Model", typeof(String));
            smartDataTable.Columns.Add("SerialNumber", typeof(String));
            smartDataTable.Columns.Add("FirmwareRevision", typeof(String));
            smartDataTable.Columns.Add("Description", typeof(String));
            smartDataTable.Columns.Add("Interface", typeof(String));
            smartDataTable.Columns.Add("MediaType", typeof(String));
            smartDataTable.Columns.Add("Name", typeof(String));
            smartDataTable.Columns.Add("PartitionCount", typeof(String));
            smartDataTable.Columns.Add("Status", typeof(String));
            smartDataTable.Columns.Add("BytesPerSector", typeof(UInt32));
            smartDataTable.Columns.Add("TotalCylinders", typeof(UInt64));
            smartDataTable.Columns.Add("TotalHeads", typeof(UInt32));
            smartDataTable.Columns.Add("TotalSectors", typeof(UInt64));
            smartDataTable.Columns.Add("TotalTracks", typeof(UInt64));
            smartDataTable.Columns.Add("TracksPerCylinder", typeof(UInt32));
            smartDataTable.Columns.Add("TotalBytes", typeof(UInt64));
            smartDataTable.Columns.Add("BaseTenCapacity", typeof(decimal));
            smartDataTable.Columns.Add("RealCapacity", typeof(decimal));
            smartDataTable.Columns.Add("NominalMediaRotationRate", typeof(UInt32));
            smartDataTable.Columns.Add("IsSsd", typeof(bool));
            smartDataTable.Columns.Add("IsTrimSupported", typeof(bool));
            smartDataTable.Columns.Add("PreferredModel", typeof(String));
            smartDataTable.Columns.Add("PreferredSerial", typeof(String));
            smartDataTable.Columns.Add("PreferredFirmware", typeof(String));
            smartDataTable.Columns.Add("PhysicalSectorSize", typeof(UInt64));
            smartDataTable.Columns.Add("LogicalSectorSize", typeof(UInt64));
            SiAuto.Main.LogMessage("Completed adding columns.");

            SiAuto.Main.LogMessage("Commit changes.");
            smartDataTable.AcceptChanges();
            SiAuto.Main.LogMessage("Changes committed.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartData.CreateDataTable");
        }

        public void ClearDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartData.ClearDataTable");
            SiAuto.Main.LogMessage("Clear all rows.");
            smartDataTable.Rows.Clear();
            SiAuto.Main.LogMessage("Rows cleared.");
            SiAuto.Main.LogMessage("Commit changes.");
            smartDataTable.AcceptChanges();
            SiAuto.Main.LogMessage("Changes committed.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartData.ClearDataTable");
        }

        public void AddNewRow(String pnpDeviceId, String devicePath)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartData.AddNewRow");
            SiAuto.Main.LogMessage("Create a new row in data table.");
            SiAuto.Main.LogString("pnpDeviceId", pnpDeviceId);
            SiAuto.Main.LogString("devicePath", devicePath);
            DataRow row = smartDataTable.NewRow();
            row["Key"] = pnpDeviceId;
            row["Path"] = devicePath;
            SiAuto.Main.LogMessage("Row parameters specified and populated.");
            smartDataTable.Rows.Add(row);
            SiAuto.Main.LogMessage("Row added to table.");
            SiAuto.Main.LogMessage("Commit changes.");
            smartDataTable.AcceptChanges();
            SiAuto.Main.LogMessage("Changes committed.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartData.AddNewRow");
        }

        public void AddSmartData(String deviceId, byte[] smartData, byte[] thresholdData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartData.AddSmartData");
            try
            {
                SiAuto.Main.LogMessage("Adding SMART data for " + deviceId + " to data table.");
                foreach (DataRow smartDataRow in smartDataTable.Select())
                {
                    if (String.Compare(deviceId, smartDataRow["Key"].ToString().Replace('\\', '~').Replace(' ', '~')) == 0 ||
                        String.Compare(deviceId, smartDataRow["Key"].ToString(), true) == 0 ||
                        String.Compare(deviceId, smartDataRow["Key"].ToString() + "_0", true) == 0)
                    {
                        SiAuto.Main.LogMessage("Desired row found; adding data.");
                        smartDataRow["SmartData"] = smartData;
                        smartDataRow["ThreshData"] = thresholdData;
                        SiAuto.Main.LogMessage("Data added; committing changes to row.");
                        smartDataRow.AcceptChanges();
                        SiAuto.Main.LogMessage("Row changes committed; committing changes to table.");
                        smartDataTable.AcceptChanges();
                        SiAuto.Main.LogMessage("Changes committed.");
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogWarning("AddSmartData data table enumeration raised an error: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            finally
            {
                
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartData.AddSmartData");
        }

        public void AddIdentityData(String deviceId, DiskIdentityData did)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartData.AddIdentityData");
            try
            {
                SiAuto.Main.LogMessage("Adding identity data for " + deviceId + " to data table.");
                SiAuto.Main.LogInt("did.Rpm", (int)did.Rpm);
                SiAuto.Main.LogBool("did.IsSsd", did.IsSsd);
                SiAuto.Main.LogBool("did.TrimSupported", did.TrimSupported);
                SiAuto.Main.LogMessage("did.Model = <<" + did.Model + ">>");
                SiAuto.Main.LogMessage("did.SerialNumber = <<" + did.SerialNumber + ">>");
                SiAuto.Main.LogMessage("did.FirmwareRevision = <<" + did.FirmwareRevision + ">>");
                foreach (DataRow smartDataRow in smartDataTable.Select())
                {
                    if (String.Compare(deviceId, smartDataRow["Key"].ToString().Replace('\\', '~').Replace(' ', '~')) == 0 ||
                        String.Compare(deviceId, smartDataRow["Key"].ToString(), true) == 0 ||
                        String.Compare(deviceId, smartDataRow["Key"].ToString() + "_0", true) == 0)
                    {
                        SiAuto.Main.LogMessage("Desired row found; adding data.");
                        smartDataRow["NominalMediaRotationRate"] = did.Rpm;
                        smartDataRow["IsSsd"] = did.IsSsd;
                        smartDataRow["IsTrimSupported"] = did.TrimSupported;
                        smartDataRow["PreferredModel"] = String.IsNullOrEmpty(did.Model) ? String.Empty : did.Model;
                        smartDataRow["PreferredSerial"] = String.IsNullOrEmpty(did.SerialNumber) ? String.Empty : did.SerialNumber;
                        smartDataRow["PreferredFirmware"] = String.IsNullOrEmpty(did.FirmwareRevision) ? String.Empty : did.FirmwareRevision;
                        SiAuto.Main.LogMessage("Data added; committing changes to row.");
                        smartDataRow.AcceptChanges();
                        SiAuto.Main.LogMessage("Row changes committed; committing changes to table.");
                        smartDataTable.AcceptChanges();
                        SiAuto.Main.LogMessage("Changes committed.");
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogWarning("AddIdentityData data table enumeration raised an error: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            finally
            {

            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartData.AddIdentityData");
        }

        public void AddDiskInformation(String deviceID, String path, String model, String serialNumber, String firmwareRevision, String description,
            String diskInterface, String mediaType, String name, String partitionCount, String status, bool failurePredicted, UInt32 bytesPerSector,
            UInt64 totalCylinders, UInt32 totalHeads, UInt64 totalSectors, UInt64 totalTracks, UInt32 tracksPerCylinder, UInt64 totalBytes,
            UInt64 physicalSectorSize, UInt64 logicalSectorSize)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartData.AddDiskInformation");
            DataRow[] smartDataRows = smartDataTable.Select();
            bool diskInjected = false;

            SiAuto.Main.LogMessage("Adding disk informational data for " + deviceID + " to data table.");
            SiAuto.Main.LogString("path", path);
            SiAuto.Main.LogString("model", model);
            SiAuto.Main.LogString("serialNumber", serialNumber);
            SiAuto.Main.LogString("firmwareRevision", firmwareRevision);
            SiAuto.Main.LogString("description", description);
            SiAuto.Main.LogString("diskInterface", diskInterface);
            SiAuto.Main.LogString("mediaType", mediaType);
            SiAuto.Main.LogString("name", name);
            SiAuto.Main.LogString("partitionCount", partitionCount);
            SiAuto.Main.LogString("status", status);
            SiAuto.Main.LogBool("failurePredicted", failurePredicted);
            SiAuto.Main.LogInt("bytesPerSector", (int)bytesPerSector);
            SiAuto.Main.LogLong("totalCylinders", (long)totalCylinders);
            SiAuto.Main.LogInt("totalHeads", (int)totalHeads);
            SiAuto.Main.LogLong("totalSectors", (long)totalSectors);
            SiAuto.Main.LogLong("totalTracks", (long)totalTracks);
            SiAuto.Main.LogInt("tracksPerCylinder", (int)tracksPerCylinder);
            SiAuto.Main.LogLong("totalBytes", (long)totalBytes);
            SiAuto.Main.LogLong("physicalSectorSize", (long)physicalSectorSize);
            SiAuto.Main.LogLong("logicalSectorSize", (long)logicalSectorSize);

            foreach (DataRow smartDataRow in smartDataRows)
            {
                if (String.Compare(deviceID , smartDataRow["Key"].ToString(), true) != 0)
                {
                    // Not the disk we want; skip to the next.
                    continue;
                }
                try
                {
                    SiAuto.Main.LogMessage("Desired row found; adding data.");
                    // Populate the data.
                    smartDataRow["Path"] = path;
                    smartDataRow["Model"] = model;
                    smartDataRow["SerialNumber"] = serialNumber;
                    smartDataRow["FirmwareRevision"] = firmwareRevision;
                    smartDataRow["Description"] = description;
                    smartDataRow["Interface"] = diskInterface;
                    smartDataRow["MediaType"] = mediaType;
                    smartDataRow["Name"] = name;
                    smartDataRow["PartitionCount"] = partitionCount;
                    smartDataRow["Status"] = status;
                    smartDataRow["FailurePredicted"] = failurePredicted;
                    smartDataRow["BytesPerSector"] = bytesPerSector;
                    smartDataRow["TotalCylinders"] = totalCylinders;
                    smartDataRow["TotalHeads"] = totalHeads;
                    smartDataRow["TotalSectors"] = totalSectors;
                    smartDataRow["TotalTracks"] = totalTracks;
                    smartDataRow["TracksPerCylinder"] = tracksPerCylinder;
                    smartDataRow["TotalBytes"] = totalBytes;
                    smartDataRow["PhysicalSectorSize"] = physicalSectorSize;
                    smartDataRow["LogicalSectorSize"] = logicalSectorSize;

                    SiAuto.Main.LogMessage("Calculating base-10 and real capacity.");
                    decimal capacityBase10 = 0.00M;
                    decimal capacityReal = 0.00M;
                    capacityBase10 = Decimal.Round(totalBytes / 1000000000.00M, 2);
                    SiAuto.Main.LogDecimal("capacityBase10", capacityBase10);
                    capacityReal = Decimal.Round(totalBytes / 1073741824.00M, 2);
                    SiAuto.Main.LogDecimal("capacityReal", capacityReal);

                    smartDataRow["BaseTenCapacity"] = capacityBase10;
                    smartDataRow["RealCapacity"] = capacityReal;
                    SiAuto.Main.LogMessage("Data added; committing changes to row.");
                    smartDataRow.AcceptChanges();
                    SiAuto.Main.LogMessage("Row changes committed; committing changes to table.");
                    smartDataTable.AcceptChanges();
                    SiAuto.Main.LogMessage("Changes committed.");
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogError("AddDiskInformation: Data injection failed. " + ex.Message);
                    SiAuto.Main.LogException(ex);
                }
                diskInjected = true;

                break; // No need to check anymore disks.
            }
            if (!diskInjected)
            {
                SiAuto.Main.LogWarning("Disk " + deviceID + " was not in the table and could not be updated.");
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartData.AddDiskInformation");
        }

        public void WriteInitialRegistryData()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartData.WriteInitialRegistryData");
            SiAuto.Main.LogMessage("Select data table objects.");
            DataRow[] smartDataRows = smartDataTable.Select();
            SiAuto.Main.LogMessage("Acquire Registry objects.");
            Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey dojoNorthSubKey;

            dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

            Microsoft.Win32.RegistryKey monitoredDiksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);
            SiAuto.Main.LogMessage("Acquired Registry objects.");

            foreach (DataRow smartDataRow in smartDataRows)
            {
                foreach( String diskKeyName in monitoredDiksKey.GetSubKeyNames() )
                {
                    //if (String.Compare(disk.DevicePath.Replace('?', '.'), smartDataRow["Path"].ToString(), true) == 0)
                    String comparisonKey = Utilities.Utility.IsSystemWindows8() ? smartDataRow["Key"].ToString().Replace('\\', '~').Replace(' ', '~') :
                        smartDataRow["Key"].ToString().Replace('\\', '~');
                    if( String.Compare(comparisonKey, diskKeyName, true) == 0)
                    {
                        SiAuto.Main.LogMessage("[Write Registry Data] Processing disk " + diskKeyName + "; acquiring disk Registry object.");
                        Microsoft.Win32.RegistryKey diskKey;
                        //diskKey = monitoredDiksKey.OpenSubKey(disk.SystemName, true);
                        diskKey = monitoredDiksKey.OpenSubKey(diskKeyName, true);
                        if (diskKey == null)
                        {
                            SiAuto.Main.LogMessage("[Write Registry Data] Key did not exist; creating new.");
                            diskKey = monitoredDiksKey.CreateSubKey(diskKeyName);
                        }
                        SiAuto.Main.LogMessage("[Write Registry Data] Registry object acquired.");

                        SiAuto.Main.LogMessage("[Write Registry Data] Writing data table row data to the Registry.");
                        diskKey.SetValue("DevicePath", smartDataRow["Path"].ToString());

                        try
                        {
                            if (smartDataRow["SmartData"] != null)
                            {
                                SiAuto.Main.LogMessage("[Write Registry Data] smartDataRow[SmartData] passes null reference check; saving.");
                                diskKey.SetValue("RawSmartData", (byte[])smartDataRow["SmartData"]);
                                SiAuto.Main.LogMessage("[Write Registry Data] Successfully saved smartDataRow[SmartData].");
                            }
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Write Registry Data] Failed to save smartDataRow[SmartData] to the Registry. " + ex.Message);
                            SiAuto.Main.LogException(ex);
                        }

                        try
                        {
                            if (smartDataRow["ThreshData"] != null)
                            {
                                SiAuto.Main.LogMessage("[Write Registry Data] smartDataRow[ThreshData] passes null reference check; saving.");
                                diskKey.SetValue("RawThresholdData", (byte[])smartDataRow["ThreshData"]);
                                SiAuto.Main.LogMessage("[Write Registry Data] Successfully saved smartDataRow[ThreshData].");
                            }
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Write Registry Data] Failed to save smartDataRow[ThreshData] to the Registry. " + ex.Message);
                            SiAuto.Main.LogException(ex);
                        }

                        try
                        {
                            if (smartDataRow["FailurePredicted"] != null)
                            {
                                SiAuto.Main.LogMessage("[Write Registry Data] smartDataRow[FailurePredicted] passes null reference check; saving.");
                                diskKey.SetValue("FailurePredicted", (bool)smartDataRow["FailurePredicted"]);
                                SiAuto.Main.LogMessage("[Write Registry Data] Successfully saved smartDataRow[FailurePredicted].");
                            }
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogWarning("[Write Registry Data] Failed to save smartDataRow[FailurePredicted] to the Registry. " + ex.Message);
                            SiAuto.Main.LogException(ex);
                        }

                        SiAuto.Main.LogMessage("[Write Registry Data] Save string values to the Registry.");
                        diskKey.SetValue("Model", smartDataRow["Model"].ToString());
                        diskKey.SetValue("Description", smartDataRow["Description"].ToString());
                        diskKey.SetValue("SerialNumber", smartDataRow["SerialNumber"].ToString());
                        diskKey.SetValue("FirmwareRevision", smartDataRow["FirmwareRevision"].ToString());
                        diskKey.SetValue("PreferredModel", smartDataRow["PreferredModel"].ToString());
                        diskKey.SetValue("PreferredSerial", smartDataRow["PreferredSerial"].ToString());
                        diskKey.SetValue("PreferredFirmware", smartDataRow["PreferredFirmware"].ToString());
                        diskKey.SetValue("Interface", smartDataRow["Interface"].ToString());
                        diskKey.SetValue("MediaType", smartDataRow["MediaType"].ToString());
                        diskKey.SetValue("Name", smartDataRow["Name"].ToString());
                        diskKey.SetValue("PartitionCount", smartDataRow["PartitionCount"].ToString());
                        diskKey.SetValue("Status", smartDataRow["Status"].ToString());
                        SiAuto.Main.LogMessage("[Write Registry Data] Successfully saved string values.");

                        SiAuto.Main.LogMessage("[Write Registry Data] Save numeric values to the Registry.");
                        try
                        {
                            diskKey.SetValue("BytesPerSector", (UInt32)smartDataRow["BytesPerSector"]);
                        }
                        catch
                        {
                            diskKey.SetValue("BytesPerSector", (UInt32)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value BytesPerSector was corrupt; set to zero.");
                        }
                        try
                        {
                            diskKey.SetValue("Cylinders", (UInt64)smartDataRow["TotalCylinders"]);
                        }
                        catch
                        {
                            diskKey.SetValue("Cylinders", (UInt64)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value Cylinders was corrupt; set to zero.");
                        }
                        try
                        {
                            diskKey.SetValue("Heads", (UInt32)smartDataRow["TotalHeads"]);
                        }
                        catch
                        {
                            diskKey.SetValue("Heads", (UInt32)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value TotalHeads was corrupt; set to zero.");
                        }
                        try
                        {
                            diskKey.SetValue("TotalSectors", (UInt64)smartDataRow["TotalSectors"]);
                        }
                        catch
                        {
                            diskKey.SetValue("TotalSectors", (UInt64)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value TotalSectors was corrupt; set to zero.");
                        }
                        try
                        {
                            diskKey.SetValue("Tracks", (UInt64)smartDataRow["TotalTracks"]);
                        }
                        catch
                        {
                            diskKey.SetValue("Tracks", (UInt64)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value Tracks was corrupt; set to zero.");
                        }
                        try
                        {
                            diskKey.SetValue("TracksPerCylinder", (UInt32)smartDataRow["TracksPerCylinder"]);
                        }
                        catch
                        {
                            diskKey.SetValue("TracksPerCylinder", (UInt32)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value TracksPerCylinder was corrupt; set to zero.");
                        }
                        try
                        {
                            diskKey.SetValue("TotalBytes", (UInt64)smartDataRow["TotalBytes"]);
                        }
                        catch
                        {
                            diskKey.SetValue("TotalBytes", (UInt64)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value TotalBytes was corrupt; set to zero.");
                        }
                        try
                        {
                            diskKey.SetValue("AdvertisedCapacity", (decimal)smartDataRow["BaseTenCapacity"]);
                        }
                        catch
                        {
                            diskKey.SetValue("AdvertisedCapacity", (decimal)0.00);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value AdvertisedCapacity was corrupt; set to zero.");
                        }
                        try
                        {
                            diskKey.SetValue("RealCapacity", (decimal)smartDataRow["RealCapacity"]);
                        }
                        catch
                        {
                            diskKey.SetValue("RealCapacity", (decimal)0.00);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value RealCapacity was corrupt; set to zero.");
                        }
                        try
                        {
                            diskKey.SetValue("PhysicalSectorSize", (UInt64)smartDataRow["PhysicalSectorSize"]);
                        }
                        catch
                        {
                            diskKey.SetValue("PhysicalSectorSize", (UInt64)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value PhysicalSectorSize was corrupt; set to zero.");
                        }
                        try
                        {
                            diskKey.SetValue("LogicalSectorSize", (UInt64)smartDataRow["LogicalSectorSize"]);
                        }
                        catch
                        {
                            diskKey.SetValue("LogicalSectorSize", (UInt64)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value LogicalSectorSize was corrupt; set to zero.");
                        }
                        SiAuto.Main.LogMessage("[Write Registry Data] Successfully saved numeric values.");

                        // RPM and SSD
                        SiAuto.Main.LogMessage("[Write Registry Data] Save RPM and SSD characteristics to the Registry.");
                        try
                        {
                            diskKey.SetValue("NominalMediaRotationRate", (UInt32)smartDataRow["NominalMediaRotationRate"]);
                        }
                        catch
                        {
                            diskKey.SetValue("NominalMediaRotationRate", (UInt32)65535);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value NominalMediaRotationRate was corrupt; set to 65535.");
                        }

                        try
                        {
                            diskKey.SetValue("IsSsd", (bool)smartDataRow["IsSsd"]);
                        }
                        catch
                        {
                            diskKey.SetValue("IsSsd", false);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value IsSsd was corrupt; set to false.");
                        }

                        try
                        {
                            diskKey.SetValue("IsTrimSupported", (bool)smartDataRow["IsTrimSupported"]);
                        }
                        catch
                        {
                            diskKey.SetValue("IsTrimSupported", false);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value IsTrimSupported was corrupt; set to false.");
                        }
                        SiAuto.Main.LogMessage("[Write Registry Data] Successfully saved RPM and SSD characteristics.");

                        // Now check to see if GUIDs are defined; create if missing.
                        SiAuto.Main.LogMessage("[Write Registry Data] Generate condition GUIDs, if needed, and save to Registry.");
                        Guid g;
                        try
                        {
                            g = new Guid((String)diskKey.GetValue("NotificationGuidCriticalHealth"));
                        }
                        catch
                        {
                            diskKey.SetValue("NotificationGuidCriticalHealth", Guid.NewGuid().ToString());
                        }

                        try
                        {
                            g = new Guid((String)diskKey.GetValue("NotificationGuidWarningHealth"));
                        }
                        catch
                        {
                            diskKey.SetValue("NotificationGuidWarningHealth", Guid.NewGuid().ToString());
                        }

                        try
                        {
                            g = new Guid((String)diskKey.GetValue("NotificationGuidBadSectors"));
                        }
                        catch
                        {
                            diskKey.SetValue("NotificationGuidBadSectors", Guid.NewGuid().ToString());
                        }

                        try
                        {
                            g = new Guid((String)diskKey.GetValue("NotificationGuidEndToEndErrors"));
                        }
                        catch
                        {
                            diskKey.SetValue("NotificationGuidEndToEndErrors", Guid.NewGuid().ToString());
                        }

                        try
                        {
                            g = new Guid((String)diskKey.GetValue("NotificationGuidPendingSectors"));
                        }
                        catch
                        {
                            diskKey.SetValue("NotificationGuidPendingSectors", Guid.NewGuid().ToString());
                        }

                        try
                        {
                            g = new Guid((String)diskKey.GetValue("NotificationGuidUncorrectableSectors"));
                        }
                        catch
                        {
                            diskKey.SetValue("NotificationGuidUncorrectableSectors", Guid.NewGuid().ToString());
                        }

                        try
                        {
                            g = new Guid((String)diskKey.GetValue("NotificationGuidReallocationEvents"));
                        }
                        catch
                        {
                            diskKey.SetValue("NotificationGuidReallocationEvents", Guid.NewGuid().ToString());
                        }

                        try
                        {
                            g = new Guid((String)diskKey.GetValue("NotificationGuidSpinRetries"));
                        }
                        catch
                        {
                            diskKey.SetValue("NotificationGuidSpinRetries", Guid.NewGuid().ToString());
                        }

                        try
                        {
                            g = new Guid((String)diskKey.GetValue("NotificationGuidCrcErrors"));
                        }
                        catch
                        {
                            diskKey.SetValue("NotificationGuidCrcErrors", Guid.NewGuid().ToString());
                        }

                        try
                        {
                            g = new Guid((String)diskKey.GetValue("NotificationGuidTemperature"));
                        }
                        catch
                        {
                            diskKey.SetValue("NotificationGuidTemperature", Guid.NewGuid().ToString());
                        }

                        try
                        {
                            g = new Guid((String)diskKey.GetValue("NotificationGuidAirflowTemperature"));
                        }
                        catch
                        {
                            diskKey.SetValue("NotificationGuidAirflowTemperature", Guid.NewGuid().ToString());
                        }

                        try
                        {
                            g = new Guid((String)diskKey.GetValue("NotificationGuidThreshold"));
                        }
                        catch
                        {
                            diskKey.SetValue("NotificationGuidThreshold", Guid.NewGuid().ToString());
                        }

                        try
                        {
                            g = new Guid((String)diskKey.GetValue("NotificationGuidGeezer"));
                        }
                        catch
                        {
                            diskKey.SetValue("NotificationGuidGeezer", Guid.NewGuid().ToString());
                        }
                        SiAuto.Main.LogMessage("[Write Registry Data] Successfully saved condition GUIDs.");

                        // Now let's check the counts and be sure they're valid.  Otherwise set to zero.
                        long check = 0;

                        SiAuto.Main.LogMessage("[Write Registry Data] Checking counts.");
                        try
                        {
                            check = long.Parse((String)diskKey.GetValue("BadSectorsIgnored"));
                        }
                        catch
                        {
                            diskKey.SetValue("BadSectorsIgnored", (long)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value BadSectorsIgnored was corrupt; set to zero.");
                        }

                        try
                        {
                            check = long.Parse((String)diskKey.GetValue("EndToEndErrorsIgnored"));
                        }
                        catch
                        {
                            diskKey.SetValue("EndToEndErrorsIgnored", (long)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value EndToEndErrosIgnored was corrupt; set to zero.");
                        }

                        try
                        {
                            check = long.Parse((String)diskKey.GetValue("PendingSectorsIgnored"));
                        }
                        catch
                        {
                            diskKey.SetValue("PendingSectorsIgnored", (long)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value PendingSectorsIgnored was corrupt; set to zero.");
                        }

                        try
                        {
                            check = long.Parse((String)diskKey.GetValue("ReallocationEventsIgnored"));
                        }
                        catch
                        {
                            diskKey.SetValue("ReallocationEventsIgnored", (long)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value ReallocationEventsIgnored was corrupt; set to zero.");
                        }

                        try
                        {
                            check = long.Parse((String)diskKey.GetValue("SpinRetriesIgnored"));
                        }
                        catch
                        {
                            diskKey.SetValue("SpinRetriesIgnored", (long)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value SpinRetriesIgnored was corrupt; set to zero.");
                        }

                        try
                        {
                            check = long.Parse((String)diskKey.GetValue("OfflineBadSectorsIgnored"));
                        }
                        catch
                        {
                            diskKey.SetValue("OfflineBadSectorsIgnored", (long)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value OfflineBadSectors was corrupt; set to zero.");
                        }

                        try
                        {
                            check = long.Parse((String)diskKey.GetValue("CrcErrorsIgnored"));
                        }
                        catch
                        {
                            diskKey.SetValue("CrcErrorsIgnored", (long)0);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value CrcErrosIgnored was corrupt; set to zero.");
                        }
                        SiAuto.Main.LogMessage("[Write Registry Data] Checking counts complete.");

                        // Now let's check the disk's flags.
                        SiAuto.Main.LogMessage("[Write Registry Data] Checking flags.");
                        bool flag = false;
                        try
                        {
                            flag = bool.Parse((String)diskKey.GetValue("IsDiskCritical"));
                        }
                        catch
                        {
                            diskKey.SetValue("IsDiskCritical", false);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value IsDiskCritical was corrupt; set to false.");
                        }

                        try
                        {
                            flag = bool.Parse((String)diskKey.GetValue("IsDiskWarning"));
                        }
                        catch
                        {
                            diskKey.SetValue("IsDiskWarning", false);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value IsDiskWarning was corrupt; set to false.");
                        }

                        try
                        {
                            flag = bool.Parse((String)diskKey.GetValue("IsDiskGeriatric"));
                        }
                        catch
                        {
                            diskKey.SetValue("IsDiskGeriatric", false);
                            SiAuto.Main.LogWarning("[Write Registry Data] Value IsDiskGeriatric was corrupt; set to false.");
                        }
                        SiAuto.Main.LogMessage("[Write Registry Data] Checking flags complete.");
                    }
                    else
                    {
                        
                    }
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartData.WriteInitialRegistryData");
        }

        public DataTable SmartDataTable
        {
            get
            {
                return smartDataTable;
            }
        }
    }
}
