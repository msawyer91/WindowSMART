using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Management;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using DojoNorthSoftware.HomeServer.HomeServerSMARTService.Enumerator;
using Prowl;

namespace TestHarnessUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //WHSInfoClass myClass = new WHSInfoClass();

            //foreach (IDiskInfo2 disk in myClass.GetDiskInfo())
            //{
            //    textBox1.Text += "DevPath: " + disk.DevicePath + "\r\n";
            //    textBox1.Text += "SysName: " + disk.SystemName + "\r\n\r\n";
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RunGeneralTests();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveResults();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RunGeneralTests()
        {
            AppendLine("Home Server SMART Test Harness UI");
            AppendLine("Copyright (C) 2011 Dojo North Software - All Rights Reserved\r\n");

            AppendLine("****** RUNNING TEST 1 of 3 *** Win32_DiskDrive General Query ******\r\n");
            int diskCount = 0;

            try
            {
                AppendLine("WMI query:  \"select * from Win32_DiskDrive\"");
                AppendLine("Executing FOREACH:  foreach (ManagementObject drive in new ManagementObjectSearcher(\"select * from Win32_DiskDrive\").Get())");
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                       "select * from Win32_DiskDrive").Get())
                {
                    AppendLine("\tDetected Disk with DeviceID: " + drive["DeviceID"].ToString());
                    diskCount++;
                }
                AppendLine("Test 1 Result:  SUCCESS");
            }
            catch (Exception ex)
            {
                AppendLine("Test 1 Exception: " + ex.Message);
                AppendLine("Test 1 Stack Trace: " + ex.StackTrace);
                AppendLine("Test 1 Result:  FAILED");
            }
            finally
            {
                AppendLine("I detected " + diskCount.ToString() + " disks in your Server.");
                AppendLine("****** TEST 1 IS COMPLETE ******\r\n");
            }

            AppendLine("****** RUNNING TEST 2 of 3 *** Win32_DiskDrive Per-Disk Size/Model Data Fetch Query ******\r\n");

            try
            {
                AppendLine("WMI query:  \"select * from Win32_DiskDrive\"");
                AppendLine("Executing FOREACH:  foreach (ManagementObject drive in new ManagementObjectSearcher(\"select * from Win32_DiskDrive\").Get())");
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                       "select * from Win32_DiskDrive").Get())
                {
                    AppendLine("\tDetected Disk with DeviceID: " + drive["DeviceID"].ToString());
                    AppendLine("\t\tDisk Model: " + (drive["Model"] == null ? "Model is NULL" : drive["Model"].ToString()));
                    if (drive["Size"] != null)
                    {
                        AppendLine("\t\tDisk Size : " + drive["Size"].ToString() + " bytes");
                        AppendLine("\t\tAttempting conversion to GB...");
                        decimal capacity = 0.00M;
                        decimal giggage = 0.00M;
                        giggage = Decimal.Parse(drive["Size"].ToString());
                        capacity = Decimal.Round(giggage / 1073741824.00M, 2);
                        AppendLine("\t\tDisk Size : " + capacity.ToString() + " GB\r\n");
                    }
                    else
                    {
                        AppendLine("\t\tDisk Size : NULL\r\n");
                    }
                }
                AppendLine("Test 2 Result:  SUCCESS");
            }
            catch (Exception ex)
            {
                AppendLine("Test 2 Exception: " + ex.Message);
                AppendLine("Test 2 Stack Trace: " + ex.StackTrace);
                AppendLine("Test 2 Result:  FAILED");
            }
            finally
            {
                AppendLine("****** TEST 2 IS COMPLETE ******\r\n");
            }

            AppendLine("****** RUNNING TEST 3 of 3 *** SMART Data Fetch using WMI Only ******\r\n");

            try
            {
                ManagementObjectSearcher WMISearch = new ManagementObjectSearcher();
                AppendLine("Set WMI ManagementScope object to \\root\\wmi");
                WMISearch.Scope = new ManagementScope(@"\root\wmi");
                AppendLine("WMI query:  \"Select * from MSStorageDriver_FailurePredictData\"");
                //String query = "Select * from MSStorageDriver_FailurePredictData";
                String query = "Select * from MSStorageDriver_FailurePredictData"; // ATAPISmartData gives all the same stuff as FailurePredictData and more.
                AppendLine("Creating WMI query:  \"WMISearch.Query = new ObjectQuery(query);\"");
                WMISearch.Query = new ObjectQuery(query);
                AppendLine("Invoking WMI query:  \"ManagementObjectCollection FailDataSet = WMISearch.Get();\"");
                ManagementObjectCollection FailDataSet = WMISearch.Get();
                AppendLine("WMI query:  \"Select * from MSStorageDriver_FailurePredictThresholds\"");
                AppendLine("Creating WMI query:  \"WMISearch.Query = new ObjectQuery(query);\"");
                WMISearch.Query = new ObjectQuery("Select * from MSStorageDriver_FailurePredictThresholds");
                AppendLine("Invoking WMI query:  \"ManagementObjectCollection ThresholdDataSet = WMISearch.Get();\"");
                ManagementObjectCollection ThresholdDataSet = WMISearch.Get();
                AppendLine("WMI query:  \"Select * from MSStorageDriver_FailurePredictStatus\"");
                AppendLine("Creating WMI query:  \"WMISearch.Query = new ObjectQuery(query);\"");
                WMISearch.Query = new ObjectQuery("Select * from MSStorageDriver_FailurePredictStatus");
                AppendLine("Invoking WMI query:  \"ManagementObjectCollection FailurePredictSet = WMISearch.Get();\"");
                ManagementObjectCollection FailurePredictSet = WMISearch.Get();

                AppendLine("All WMI queries executed; performing iterations.");

                AppendLine("FailDataSet contains " + FailDataSet.Count + " objects");
                AppendLine("ThresholdDataSet contains " + ThresholdDataSet.Count + " objects");
                AppendLine("FailurePredictSet contains " + FailurePredictSet.Count + " objects");

                // Iterate the SMART attributes - note this does NOT contain thresholds!
                AppendLine("Executing FOREACH:  foreach (ManagementObject FailData in FailDataSet)");
                String currentDisk = String.Empty;

                foreach (ManagementObject FailData in FailDataSet)
                {
                    bool failurePredicted = false;
                    AppendLine("\tforeach iterating; get InstanceName");
                    currentDisk = FailData.Properties["InstanceName"].Value.ToString();
                    AppendLine("\tInstanceName: " + currentDisk);
                    //String desiredDisk = drive["PNPDeviceID"].ToString() + "_0";
                    String desiredDisk = String.Empty;
                    //if (String.Compare(currentDisk, desiredDisk, true) == 0)
                    //{
                    AppendLine("\tGet VendorSpecific fail data");
                    Byte[] smartData = (Byte[])FailData.Properties["VendorSpecific"].Value;
                    AppendLine("\tSuccessfully fetched VendorSpecific fail data");
                    // Now iterate the SMART attribute thresholds.
                    foreach (ManagementObject ThresholdData in ThresholdDataSet)
                    {
                        //currentDisk = ThresholdData.Properties["InstanceName"].Value.ToString();
                        desiredDisk = ThresholdData.Properties["InstanceName"].Value.ToString();

                        if (String.Compare(currentDisk, desiredDisk, true) == 0)
                        {
                            Byte[] smartThreshold = (Byte[])ThresholdData.Properties["VendorSpecific"].Value;

                            // Finally iterate the SMART failure flag status in the storage driver.
                            foreach (ManagementObject FailurePredicted in FailurePredictSet)
                            {
                                desiredDisk = FailurePredicted.Properties["InstanceName"].Value.ToString();
                                if (String.Compare(currentDisk, desiredDisk, true) == 0)
                                {
                                    try
                                    {
                                        failurePredicted = bool.Parse(FailurePredicted.Properties["PredictFailure"].Value.ToString());
                                    }
                                    catch
                                    {
                                        failurePredicted = false;
                                    }
                                    break;
                                }
                            }

                            // Now we have the threshold data, SMART data and instance name.
                            // Let's store them.
                            //smartDataTable.AddSmartData(currentDisk, smartData, smartThreshold, failurePredicted);
                            failurePredicted = false;
                        }
                    }
                    //}
                }

                AppendLine("Test 3 Result:  SUCCESS");
            }
            catch (Exception ex)
            {
                AppendLine("Test 3 Exception: " + ex.Message);
                AppendLine("Test 3 Stack Trace: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    AppendLine("Test 3 Inner Exception: " + ex.InnerException.Message);
                    AppendLine("Test 3 Inner Stack Trace: " + ex.InnerException.StackTrace);
                }
                AppendLine("Test 3 Result:  FAILED");
            }
            finally
            {
                AppendLine("****** TEST 3 IS COMPLETE ******\r\n");
            }
        }

        private void AppendLine(String message)
        {
            textBox1.Text += message + "\r\n";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //BeThereSiI();
            ProwlClientConfiguration config = new ProwlClientConfiguration();
            config.ApplicationName = "Sam Wick";
            config.ProviderKey = "669a448635ef5e2d2bc59f4bed23b3f6ca240e32";
            config.ApiKeychain = "e3478472b59b9209439a2c49c023b1214ebd5d35";
            ProwlClient client = new ProwlClient(config);
            ProwlNotification notificate = new ProwlNotification();
            notificate.Description = "A structural imperfection was detected on Josh's faultlessly curved, flawlessly constructed, perfectly formed, exceedingly handsome (and good looking) baby head.";
            notificate.Priority = ProwlNotificationPriority.Emergency;
            notificate.Event = "Bad Sectors Detected";
            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;
            client.PostNotification(notificate);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void InstantiateWmi()
        {
            
        }

        private void BeThereSiI()
        {
            AppendLine("Home Server SMART Test Harness UI");
            AppendLine("Copyright (C) 2011 Dojo North Software - All Rights Reserved\r\n");

            AppendLine("****** RUNNING TEST 1 of 3 *** Searching for SiI Controllers ******\r\n");

            bool isSiIDetected = false;
            List<string> siIControllers = new List<string>();

            try
            {
                AppendLine("WMI query:  \"select Name, DeviceID from Win32_SCSIController\"");
                AppendLine("Executing FOREACH:  foreach (ManagementObject drive in new ManagementObjectSearcher(\"select Name, DeviceID from Win32_SCSIController\").Get())");
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                       "select Name, DeviceID from Win32_SCSIController").Get())
                {
                    AppendLine("\tDetected Controller with DeviceID: " + drive["DeviceID"].ToString());
                    String controllerName = drive["Name"].ToString();
                    if (controllerName.ToUpper().Contains("SILICON IMAGE"))
                    {
                        siIControllers.Add(drive["DeviceID"].ToString());
                        AppendLine("\t\tSilicon Image SiI Controller detected!");
                        isSiIDetected = true;
                    }
                }
                AppendLine("Test 1 Result:  " + (isSiIDetected ? "At least one SiI controller detected." :
                    "No SiI controllers detected. No further action required."));
            }
            catch (Exception ex)
            {
                AppendLine("Test 1 Exception: " + ex.Message);
                AppendLine("Test 1 Stack Trace: " + ex.StackTrace);
                AppendLine("Test 1 Result:  FAILED (cannot continue further; halting)");
                isSiIDetected = false;
            }
            finally
            {
                AppendLine("****** TEST 1 IS COMPLETE ******\r\n");
            }

            if (!isSiIDetected)
            {
                return;
            }

            AppendLine("****** RUNNING TEST 2 of 3 *** Searching for Disks (Non-Removables) on SiI Controllers ******\r\n");

            isSiIDetected = false;
            List<string> nonRemovables = new List<string>();

            foreach (String controller in siIControllers)
            {
                String testController = controller.Replace("\\", "\\\\");
                try
                {
                    AppendLine("WMI query:  \"ASSOCIATORS OF {Win32_SCSIController.DeviceID=\"" + testController + "\"} WHERE AssocClass = Win32_SCSIControllerDevice\"");
                    AppendLine("Executing FOREACH:  foreach (ManagementObject drive in new ManagementObjectSearcher(\"ASSOCIATORS OF {Win32_SCSIController.DeviceID=\"" + testController + "\"} WHERE AssocClass = Win32_SCSIControllerDevice\").Get())");
                    foreach (ManagementObject drive in new ManagementObjectSearcher(
                           "ASSOCIATORS OF {Win32_SCSIController.DeviceID=\"" + testController + "\"} WHERE AssocClass = Win32_SCSIControllerDevice").Get())
                    {
                        AppendLine("\tDetected Device with DeviceID: " + drive["DeviceID"].ToString());
                        String device = drive["DeviceID"].ToString();
                        if (device.ToUpper().Contains("SCSI\\DISK"))
                        {
                            nonRemovables.Add(drive["DeviceID"].ToString());
                            AppendLine("\t\tDevice is a disk drive.");
                            isSiIDetected = true;
                        }
                        else
                        {
                            AppendLine("\t\tDevice is NOT a disk drive.");
                        }
                    }
                    AppendLine("Test 2 Result:  " + (isSiIDetected ? "At least one non-removable disk detected." :
                        "No non-removable disks detected. No further action required."));
                }
                catch (Exception ex)
                {
                    AppendLine("Test 2 Exception: " + ex.Message);
                    AppendLine("Test 2 Stack Trace: " + ex.StackTrace);
                    AppendLine("Test 2 Result:  FAILED (cannot continue further; halting)");
                    return;
                }
                finally
                {
                    AppendLine("****** TEST 2 IS COMPLETE ******\r\n");
                }
            }

            AppendLine("****** RUNNING TEST 3 of 3 *** Browsing for S.M.A.R.T. Data using WMI Only ******\r\n");

            isSiIDetected = false;
            
            try
            {
                ManagementObjectSearcher WMISearch = new ManagementObjectSearcher();
                AppendLine("Set WMI ManagementScope object to \\root\\wmi");
                WMISearch.Scope = new ManagementScope(@"\root\wmi");

                foreach (String nonRemovable in nonRemovables)
                {
                    String testDisk = nonRemovable.Replace("\\", "\\\\");
                    try
                    {
                        AppendLine("Querying " + testDisk + " for SMART Attributes.");
                        AppendLine("WMI Query: " + "select VendorSpecific from MSStorageDriver_ATAPISmartData where InstanceName=\"" + testDisk + "_0\"");
                        WMISearch.Query = new ObjectQuery("select VendorSpecific from MSStorageDriver_ATAPISmartData where InstanceName=\"" + testDisk + "_0\"");
                        //MSStorageDriver_FailurePredictData
                        ManagementObjectCollection FailDataSet = WMISearch.Get();
                        foreach (ManagementObject attribute in FailDataSet)
                        {
                            Byte[] smartData = (Byte[])attribute.Properties["VendorSpecific"].Value;
                            AppendLine("\tSuccessfully reaped SMART attributes! (First 30/512 displayed.)");
                            for (int i = 0; i < 30; i++)
                            {
                                AppendLine("\t\tVendorSpecific[" + i.ToString() + "] = " + smartData[i].ToString());
                            }
                        }
                        AppendLine("Done reading SMART attributes; if you do not see a list of 30/512 items printed above the query did not get any results.");
                    }
                    catch(Exception ex)
                    {
                        AppendLine("Failed to reap SMART attributes! " + ex.Message);
                    }

                    try
                    {
                        AppendLine("Querying " + testDisk + " for SMART Thresholds.");
                        AppendLine("WMI Query: " + "select VendorSpecific from MSStorageDriver_FailurePredictThresholds where InstanceName=\"" + testDisk + "_0\"");
                        WMISearch.Query = new ObjectQuery("select VendorSpecific from MSStorageDriver_FailurePredictThresholds where InstanceName=\"" + testDisk + "_0\"");
                        //MSStorageDriver_FailurePredictData
                        ManagementObjectCollection FailDataSet = WMISearch.Get();
                        foreach (ManagementObject attribute in FailDataSet)
                        {
                            Byte[] smartData = (Byte[])attribute.Properties["VendorSpecific"].Value;
                            AppendLine("\tSuccessfully reaped SMART thresholds! (First 30/512 displayed.)");
                            for (int i = 0; i < 30; i++)
                            {
                                AppendLine("\t\tVendorSpecific[" + i.ToString() + "] = " + smartData[i].ToString());
                            }
                        }
                        AppendLine("Done reading SMART thresholds; if you do not see a list of 30/512 items printed above the query did not get any results.");
                    }
                    catch (Exception ex)
                    {
                        AppendLine("Failed to reap SMART thresholds! " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLine("Test 3 Exception: " + ex.Message);
                AppendLine("Test 3 Stack Trace: " + ex.StackTrace);
                AppendLine("Test 3 Result:  FAILED");
                return;
            }
            finally
            {
                AppendLine("****** TEST 3 IS COMPLETE ******\r\n");
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            //GetDriveInfo();

            GetUsbInfo();
        }

        private void GetDriveInfo()
        {
            AppendLine("Home Server SMART Test Harness UI");
            AppendLine("Copyright (C) 2011 Dojo North Software - All Rights Reserved\r\n");

            AppendLine("****** RUNNING TEST 1 of 1 *** Query Drives for Information ******\r\n");

            try
            {
                AppendLine("Running code that is executed in DiskEnumerator.GetDriveInfo() to better determine the cause of its failure.\r\n");

                // Browse all WMI physical disks.
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                  "select * from Win32_DiskDrive").Get())
                {
                    UInt32 bytesPerSector = 0;
                    UInt64 totalCylinders = 0;
                    UInt32 totalHeads = 0;
                    UInt64 totalSectors = 0;
                    UInt64 totalTracks = 0;
                    UInt32 tracksPerCylinder = 0;

                    String path = (drive["DeviceID"] == null ? "Undefined in WMI" : drive["DeviceID"].ToString());
                    String deviceInstanceID = (drive["PNPDeviceID"] == null ? "Undefined in WMI" : drive["PNPDeviceID"].ToString());
                    AppendLine("Processing disk at path " + path + " with instance ID " + deviceInstanceID);
                    String model = (drive["Model"] == null ? "Undefined in WMI" : drive["Model"].ToString());
                    String description = (drive["Description"] == null ? "N/A" : drive["Description"].ToString());
                    String interfaceType = (drive["InterfaceType"] == null ? "Undefined in WMI" : drive["InterfaceType"].ToString());
                    String mediaType = (drive["MediaType"] == null ? "Undefined in WMI" : drive["MediaType"].ToString());
                    String name = (drive["Name"] == null ? "Undefined in WMI" : drive["Name"].ToString());
                    String partitionCount = (drive["Partitions"] == null ? "Unknown" : drive["Partitions"].ToString());
                    String status = (drive["Status"] == null ? "Unknown" : drive["Status"].ToString());
                    AppendLine("\tModel: " + model);
                    AppendLine("\tDescription: " + description);
                    AppendLine("\tInterface Type: " + interfaceType);
                    AppendLine("\tMedia Type: " + mediaType);
                    AppendLine("\tName: " + name);
                    AppendLine("\tPartition Count: " + partitionCount);
                    AppendLine("\tStatus: " + status);

                    AppendLine("\r\n\tA ZERO value for any of the below indicates the data was not available (bad).\r\n");
                    try
                    {
                        bytesPerSector = (UInt32)drive["BytesPerSector"];
                    }
                    catch
                    {
                        bytesPerSector = 0;
                    }
                    try
                    {
                        totalCylinders = (UInt64)drive["TotalCylinders"];
                    }
                    catch
                    {
                        totalCylinders = 0;
                    }
                    try
                    {
                        totalHeads = (UInt32)drive["TotalHeads"];
                    }
                    catch
                    {
                        totalHeads = 0;
                    }
                    try
                    {
                        totalSectors = (UInt64)drive["TotalSectors"];
                    }
                    catch
                    {
                        totalSectors = 0;
                    }
                    try
                    {
                        totalTracks = (UInt64)drive["TotalTracks"];
                    }
                    catch
                    {
                        totalTracks = 0;
                    }
                    try
                    {
                        tracksPerCylinder = (UInt32)drive["TracksPerCylinder"];
                    }
                    catch
                    {
                        tracksPerCylinder = 0;
                    }

                    AppendLine("\tBytes Per Sector: " + bytesPerSector.ToString());
                    AppendLine("\tTotal Cylinders: " + totalCylinders.ToString());
                    AppendLine("\tTotal Heads: " + totalHeads.ToString());
                    AppendLine("\tTotal Sectors: " + totalSectors.ToString());
                    AppendLine("\tTotal Tracks: " + totalTracks.ToString());
                    AppendLine("\tTracks Per Cylinder: " + tracksPerCylinder.ToString());
                }
            }
            catch (Exception ex)
            {
                AppendLine("Test 1 Exception: " + ex.Message);
                AppendLine("Test 1 Stack Trace: " + ex.StackTrace);
                AppendLine("Test 1 Result:  FAILED");
                return;
            }
            finally
            {
                AppendLine("****** TEST 1 IS COMPLETE ******\r\n");
            }
        }

        private void GetUsbInfo()
        {
            AppendLine("Home Server SMART Test Harness UI");
            AppendLine("Copyright (C) 2011 Dojo North Software - All Rights Reserved\r\n");

            AppendLine("****** RUNNING TEST 1 of 1 *** Get USB Information ******\r\n");

            System.IO.StreamReader reader = null;
            DataTable vendorTable = new DataTable("Vendors");

            DataColumn[] keys = new DataColumn[1];
            DataColumn idColumn = new DataColumn();
            idColumn.ColumnName = "VID";
            idColumn.DataType = typeof(int);
            keys[0] = idColumn;
            vendorTable.Columns.Add(idColumn);
            vendorTable.Columns.Add("Vendor", typeof(String));
            vendorTable.PrimaryKey = keys;
            vendorTable.AcceptChanges();
            bool lookupAvailable = false;

            try
            {
                if (System.IO.File.Exists("usb.if"))
                {
                    int vendors = 0;
                    int errors = 0;
                    AppendLine("*** Loading usb.org vendor data from usb.if...");
                    reader = new System.IO.StreamReader("usb.if");
                    while (!reader.EndOfStream)
                    {
                        int vendorID = 0;
                        String line = reader.ReadLine();
                        try
                        {
                            //vendorID = Int32.Parse(line.Substring(0, 4));
                            vendorID = Int32.Parse(line.Substring(0, line.IndexOf('|')));
                            DataRow row = vendorTable.NewRow();
                            row["VID"] = vendorID;
                            row["Vendor"] = line.Substring(line.IndexOf('|') + 1);
                            vendorTable.Rows.Add(row);
                        }
                        catch (System.Data.ConstraintException)
                        {
                            AppendLine("\tWARNING: Error in usb.if, line " + vendors.ToString() + ". Duplicate vendor ID " + line.Substring(0, 4) + ". Each VID must be defined only once.");
                            errors++;
                        }
                        catch
                        {
                            AppendLine("\tWARNING: Error in usb.if, line " + vendors.ToString() + ". Invalid vendor ID " + line.Substring(0, 4) + "; cannot parse to integer.");
                            errors++;
                        }
                        vendors++;
                    }
                    vendors = vendors - errors;
                    vendorTable.AcceptChanges();
                    AppendLine("*** Load done, " + vendors.ToString() + " vendors loaded.\r\n");
                    lookupAvailable = true;
                }
                else
                {
                    AppendLine("WARNING: usb.if was not detected; vendor lookups will not be available.");
                    lookupAvailable = false;
                }
            }
            catch (Exception ex)
            {
                AppendLine("WARNING: usb.if failed to load (" + ex.Message + "); vendor lookups will not be available.");
                lookupAvailable = false;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            try
            {
                AppendLine("Getting a list of all USB devices installed in the Server.\r\n");

                AppendLine("\t***************");
                AppendLine("\t* USB Devices *");
                AppendLine("\t***************\r\n");

                int deviceNo = 0;
                foreach (ManagementObject hub in new ManagementObjectSearcher("select * from Win32_USBHub").Get())
                {
                    AppendLine("Device " + deviceNo.ToString());
                    String vidPid = hub["DeviceID"].ToString();
                    AppendLine("\tDevice ID:" + vidPid);
                    AppendLine("\tPNP ID:" + hub["PNPDeviceID"].ToString());
                    if( vidPid.Contains("VID_") && vidPid.Contains("PID_"))
                    {
                        AppendLine("\t\tVID: " + vidPid.Substring(vidPid.IndexOf("VID_") + 4, 4) + "    PID: " + vidPid.Substring(vidPid.IndexOf("PID_") + 4, 4));
                        if (lookupAvailable)
                        {
                            int vid = Int32.Parse(vidPid.Substring(vidPid.IndexOf("VID_") + 4, 4), System.Globalization.NumberStyles.HexNumber);
                            DataRow[] rows = vendorTable.Select("VID='" + vid.ToString() + "'");
                            if (rows != null && rows.Length > 0)
                            {
                                foreach (DataRow row in rows)
                                {
                                    AppendLine("\t\tVendor: " + row["Vendor"].ToString());
                                }
                            }
                            else
                            {
                                AppendLine("\t\tVendor " + vidPid.Substring(vidPid.IndexOf("VID_") + 4, 4) + " (" + vid.ToString() + ") is NOT DEFINED in usb.org definitions file usb.if.");
                            }
                        }
                    }
                    AppendLine("\tDescription:" + (hub["Description"] == null ? "None" : hub["Description"].ToString()));
                    AppendLine("");
                    deviceNo++;
                }
            }
            catch (Exception ex)
            {
                AppendLine("Damn, unable to enumerate USB devices. " + ex.Message);
            }

            try
            {
                AppendLine("Getting a list of all USB storage devices installed in the Server.\r\n");

                AppendLine("\t***********************");
                AppendLine("\t* USB Storage Devices *");
                AppendLine("\t***********************\r\n");

                int deviceNo = 0;

                // Browse all WMI physical disks.
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                  "select * from Win32_DiskDrive").Get())
                {
                    String diskPath = String.Empty;
                    String diskID = String.Empty;

                    diskPath = drive["DeviceID"].ToString();
                    diskID = drive["PNPDeviceID"].ToString();

                    if (diskID.Contains("USBSTOR"))
                    {
                        AppendLine("USB Storage Device " + deviceNo.ToString());
                        AppendLine("\tDevice ID:" + diskPath);
                        AppendLine("\tPNP ID:" + diskID);
                        AppendLine("\tModel: " + (drive["Model"] == null ? "Not Available" : drive["Model"].ToString()));
                    }
                }
                AppendLine("");
            }
            catch (Exception ex)
            {
                AppendLine("Damn, unable to enumerate USB disks. " + ex.Message);
            }
            finally
            {
                AppendLine("****** TEST 1 IS COMPLETE ******\r\n");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            TestAllMethods();
        }

        private void TestAllMethods()
        {
            MessageBox.Show( "For best results, if you have any flash drives (including Magic Jack) attached to your Server, " +
                "please remove them at this time. This is NOT required but will prevent the test code from trying to process " +
                "them. All of the tests will fail for such a device and you'll have extra useless output cluttering up your " +
                "results. Click OK when ready.", "Detach Flash Drives", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            AppendLine("Home Server SMART Test Harness UI");
            AppendLine("Copyright (C) 2011 Dojo North Software - All Rights Reserved\r\n");

            AppendLine("This test will attempt to run ALL of the SMART access tests against your drive. They will be performed");
            AppendLine("as follows (each drive will be run through all tests before moving on to the next drive):");
            AppendLine("\t1. Run as an \"internal drive\" - this should get SMART data for most IDE/SATA drives.");
            AppendLine("\t2. Run as a Silicon Image SiI controller - disks attached to an SiI that don't yield results above should here.");
            AppendLine("\t3. Run as a SCSI disk. Success here is unlikely but we'll try it anyway.");
            AppendLine("\t\tSubtest: Try using WMI SCSITargetID instead of default target 0xA0 (160).");
            AppendLine("\t4. Run as a USB disk. Only USB disks with supported bridge chips should return success.");
            AppendLine("\t5. Test using \"old faithful\" WMI.\r\n");

            int disksProcessed = 0;
            int disksSuccessful = 0;
            int disksFailed = 0;

            AppendLine("Getting a list of the controllers and the disks attached to them (USB/FireWire not included).\r\n");

            AppendLine("\t************************");
            AppendLine("\t* IDE/SATA Controllers *");
            AppendLine("\t************************\r\n");

            try
            {
                List<String> sataControllers = new List<string>();
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                       "select Name, DeviceID from Win32_IDEController").Get())
                {
                    AppendLine("\tDetected Controller with DeviceID: " + drive["DeviceID"].ToString());
                    AppendLine("\t\tController Name: " + (drive["Name"] == null ? "NULL" : drive["Name"].ToString()));
                    sataControllers.Add(drive["DeviceID"].ToString());
                }

                foreach (String controller in sataControllers)
                {
                    AppendLine("\r\n*** FOUND the following device(s) attached to " + controller);
                    String testController = controller.Replace("\\", "\\\\");
                    try
                    {
                        foreach (ManagementObject drive in new ManagementObjectSearcher(
                               "ASSOCIATORS OF {Win32_IDEController.DeviceID=\"" + testController + "\"} WHERE AssocClass = Win32_IDEControllerDevice").Get())
                        {
                            AppendLine("\tID = " + drive["DeviceID"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendLine("*** FAILED to enumerate " + controller + ": " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLine("*** FAILED to enumerate IDE/SATA controllers: " + ex.Message);
            }

            AppendLine("\r\n\t********************");
            AppendLine("\t* SCSI Controllers *");
            AppendLine("\t********************\r\n");

            try
            {
                List<String> scsiControllers = new List<string>();
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                       "select Name, DeviceID from Win32_SCSIController").Get())
                {
                    AppendLine("\tDetected Controller with DeviceID: " + drive["DeviceID"].ToString());
                    AppendLine("\t\tController Name: " + (drive["Name"] == null ? "NULL" : drive["Name"].ToString()));
                    scsiControllers.Add(drive["DeviceID"].ToString());
                }

                foreach (String controller in scsiControllers)
                {
                    AppendLine("\r\n*** FOUND the following device(s) attached to " + controller);
                    String testController = controller.Replace("\\", "\\\\");
                    try
                    {
                        foreach (ManagementObject drive in new ManagementObjectSearcher(
                               "ASSOCIATORS OF {Win32_SCSIController.DeviceID=\"" + testController + "\"} WHERE AssocClass = Win32_SCSIControllerDevice").Get())
                        {
                            AppendLine("\tID = " + drive["DeviceID"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendLine("*** FAILED to enumerate " + controller + ": " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLine("*** FAILED to enumerate SCSI controllers: " + ex.Message);
            }

            // Browse all WMI physical disks.
            foreach (ManagementObject drive in new ManagementObjectSearcher(
              "select * from Win32_DiskDrive").Get())
            {
                String diskPath = String.Empty;
                String diskID = String.Empty;

                byte[] smartData = null;
                byte[] smartThreshold = null;
                bool isFailurePredicted = false;

                bool isDiskSuccessful = false;

                bool resultOK = false;

                disksProcessed++;

                try
                {
                    diskPath = drive["DeviceID"].ToString();
                    diskID = drive["PNPDeviceID"].ToString();
                }
                catch (Exception ex)
                {
                    AppendLine("\r\n*** WARNING! An exception occurred getting critical drive data (WMI attributes DeviceID and PNPDeviceID).");
                    AppendLine("*** Tests not available for this disk!  Exception message: " + ex.Message);
                    AppendLine("Disk Path: " + diskPath);
                    AppendLine("Disk PNP ID: " + diskID);
                    continue;
                }
                finally
                {
                    
                }

                AppendLine("\r\n\t***************");
                AppendLine("\t* NOW TESTING *");
                AppendLine("\t***************");
                AppendLine("\tDisk Path: " + diskPath);
                AppendLine("\tDisk PNP ID: " + diskID + "\r\n");

                try
                {
                    AppendLine("****** RUNNING TEST 1 of 5 *** Fetch SMART Data Using Internal Drive Code ******\r\n");
                    AppendLine("\tP/Invoke GetInternalSmartData as non-SiI disk");
                    resultOK = InteropSmart.GetInternalSmartData(diskPath, GlobalConstants.DEFAULT_TARGET_ID, out smartData,
                        out smartThreshold, out isFailurePredicted);

                    if (resultOK)
                    {
                        AppendLine("\tGetInternalSmartData returned true, so let's have a look at the first 30 values of attributes and thresholds.");
                        for (int i = 0; i < 30; i++)
                        {
                            AppendLine("\t\tsmartData[" + i.ToString() + "] = " + smartData[i].ToString() + "\tsmartThreshold[" + i.ToString() + "] = " +
                                smartThreshold[i].ToString());
                        }
                        isDiskSuccessful = true;
                    }
                    else
                    {
                        AppendLine("\t\tDamn, GetInternalSmartData returned false. The internal call did not work for this disk.");
                    }
                }
                catch (Exception ex)
                {
                    AppendLine("Test 1 Exception: " + ex.Message);
                    AppendLine("Test 1 Stack Trace: " + ex.StackTrace);
                    AppendLine("Test 1 Result:  FAILED");
                }
                finally
                {
                    AppendLine("\r\nResetting flags.");
                    smartData = null;
                    smartThreshold = null;
                    isFailurePredicted = false;
                    resultOK = false;
                    AppendLine("Reset done.");
                    AppendLine("****** TEST 1 IS COMPLETE ******\r\n");
                }

                bool isSsd = false;
                bool isTrimSupported = false;
                UInt32 rpm = 65535;
                bool temp = InteropSmart.GetInternalDriveIdentity(diskPath, GlobalConstants.DEFAULT_TARGET_ID, out isSsd, out isTrimSupported, out rpm);

                AppendLine("\r\nThe disk " + (isSsd ? "IS" : "is NOT") + " an SSD.");
                AppendLine("RPM: " + rpm.ToString() + "\r\n");

                try
                {
                    AppendLine("****** RUNNING TEST 2 of 5 *** Fetch SMART Data Using SiI Drive Code ******\r\n");
                    AppendLine("\tP/Invoke GetInternalSmartData as SiI disk");
                    resultOK = InteropSmart.GetInternalSmartData(diskPath, GlobalConstants.DEFAULT_TARGET_ID, true, diskID,
                        false, out smartData, out smartThreshold, out isFailurePredicted);
                    //resultOK = InteropSmart.GetInternalSmartData(diskPath, GlobalConstants.DEFAULT_TARGET_ID, true, diskID,
                    //    out smartData, out smartThreshold, out isFailurePredicted, false);

                    if (resultOK)
                    {
                        AppendLine("\tGetInternalSmartData returned true, so let's have a look at the first 30 values of attributes and thresholds.");
                        for (int i = 0; i < 30; i++)
                        {
                            AppendLine("\t\tsmartData[" + i.ToString() + "] = " + smartData[i].ToString() + "\tsmartThreshold[" + i.ToString() + "] = " +
                                smartThreshold[i].ToString());
                        }
                        isDiskSuccessful = true;
                    }
                    else
                    {
                        AppendLine("\t\tDamn, GetInternalSmartData returned false. The internal SiI call did not work for this disk.");
                        AppendLine("TRYING alternate SiI method (P/Invoke call without WMI -- gets SMART values only; no thresholds.");
                        //resultOK = InteropSmart.GetInternalSmartData(diskPath, GlobalConstants.DEFAULT_TARGET_ID, true, diskID,
                        //    out smartData, out smartThreshold, out isFailurePredicted, true);
                        resultOK = InteropSmart.GetInternalSmartData(diskPath, GlobalConstants.DEFAULT_TARGET_ID, true, diskID, true,
                            out smartData, out smartThreshold, out isFailurePredicted);

                        if (resultOK)
                        {
                            AppendLine("\tGetInternalSmartData returned true on ALTERNATE call, so let's have a look at the first 30 values of attributes and thresholds.");
                            for (int i = 0; i < 30; i++)
                            {
                                AppendLine("\t\tsmartData[" + i.ToString() + "] = " + smartData[i].ToString() + "\tsmartThreshold[" + i.ToString() + "] = " +
                                    smartThreshold[i].ToString());
                            }
                            isDiskSuccessful = true;
                        }
                        else
                        {
                            AppendLine("\t\tDouble Damn, GetInternalSmartData returned false. The ALTERNATE SiI call did not work for this disk.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppendLine("Test 2 Exception: " + ex.Message);
                    AppendLine("Test 2 Stack Trace: " + ex.StackTrace);
                    AppendLine("Test 2 Result:  FAILED");
                }
                finally
                {
                    AppendLine("\r\nResetting flags.");
                    smartData = null;
                    smartThreshold = null;
                    isFailurePredicted = false;
                    resultOK = false;
                    AppendLine("Reset done.");
                    AppendLine("****** TEST 2 IS COMPLETE ******\r\n");
                }


                try
                {
                    AppendLine("****** RUNNING TEST 3 of 5 *** Fetch SMART Data Using SCSI Drive Code ******\r\n");
                    AppendLine("\tP/Invoke GetInternalSmartData as SCSI disk");

                    int scsiPort = -1;
                    int scsiTarget = -1;

                    try
                    {
                        scsiPort = (UInt16)drive["SCSIPort"];
                    }
                    catch
                    {
                        scsiPort = -1;
                        AppendLine("\tWARNING: Error getting SCSIPort; defaulting to -1.");
                    }
                    try
                    {
                        //scsiTarget = (UInt16)drive["SCSITargetId"];
                        scsiTarget = 0xA0;
                    }
                    catch
                    {
                        scsiTarget = -1;
                        AppendLine("\tWARNING: Error getting SCSITargetId; defaulting to -1.");
                    }

                    if( scsiPort == -1 || scsiTarget == -1)
                    {
                        AppendLine("*** Invalid SCSI port and/or target ID of -1; skipping this disk.");
                    }
                    else
                    {
                        resultOK = InteropSmart.GetScsiSmartData(scsiPort, scsiTarget, out smartData, out smartThreshold, out isFailurePredicted);

                        if (resultOK)
                        {
                            AppendLine("\tGetScsiSmartData returned true, so let's have a look at the first 30 values of attributes and thresholds.");
                            for (int i = 0; i < 30; i++)
                            {
                                AppendLine("\t\tsmartData[" + i.ToString() + "] = " + smartData[i].ToString() + "\tsmartThreshold[" + i.ToString() + "] = " +
                                    smartThreshold[i].ToString());
                            }
                            AppendLine("*** CAUTION: I have no good means of thoroughly testing the SCSI methods; just because data is returned above " +
                                "does not mean it is correct!");
                            isDiskSuccessful = true;
                        }
                        else
                        {
                            AppendLine("\t\tDamn, GetScsiSmartData returned false. The SCSI call did not work for this disk.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppendLine("Test 3 Exception: " + ex.Message);
                    AppendLine("Test 3 Stack Trace: " + ex.StackTrace);
                    AppendLine("Test 3 Result:  FAILED");
                }
                finally
                {
                    AppendLine("\r\nResetting flags.");
                    smartData = null;
                    smartThreshold = null;
                    isFailurePredicted = false;
                    resultOK = false;
                    AppendLine("Reset done.");
                    AppendLine("****** TEST 3 IS COMPLETE ******\r\n");
                }

                try
                {
                    AppendLine("****** RUNNING SUBTEST 3-1 of 5 *** Fetch SMART Data Using SCSI Drive Code (WMI Target) ******\r\n");
                    AppendLine("\tP/Invoke GetInternalSmartData as SCSI disk");

                    int scsiPort = -1;
                    int scsiTarget = -1;

                    try
                    {
                        scsiPort = (UInt16)drive["SCSIPort"];
                    }
                    catch
                    {
                        scsiPort = -1;
                        AppendLine("\tWARNING: Error getting SCSIPort; defaulting to -1.");
                    }
                    try
                    {
                        scsiTarget = (UInt16)drive["SCSITargetId"];
                    }
                    catch
                    {
                        scsiTarget = -1;
                        AppendLine("\tWARNING: Error getting SCSITargetId; defaulting to -1.");
                    }

                    if (scsiPort == -1 || scsiTarget == -1)
                    {
                        AppendLine("*** Invalid SCSI port and/or target ID of -1; skipping this disk.");
                    }
                    else
                    {
                        resultOK = InteropSmart.GetScsiSmartData(scsiPort, scsiTarget, out smartData, out smartThreshold, out isFailurePredicted);

                        if (resultOK)
                        {
                            AppendLine("\tGetScsiSmartData returned true, so let's have a look at the first 30 values of attributes and thresholds.");
                            for (int i = 0; i < 30; i++)
                            {
                                AppendLine("\t\tsmartData[" + i.ToString() + "] = " + smartData[i].ToString() + "\tsmartThreshold[" + i.ToString() + "] = " +
                                    smartThreshold[i].ToString());
                            }
                            AppendLine("*** CAUTION: I have no good means of thoroughly testing the SCSI methods; just because data is returned above " +
                                "does not mean it is correct!");
                            isDiskSuccessful = true;
                        }
                        else
                        {
                            AppendLine("\t\tDamn, GetScsiSmartData returned false. The SCSI call did not work for this disk.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppendLine("Subtest 3-1 Exception: " + ex.Message);
                    AppendLine("Subtest 3-1 Stack Trace: " + ex.StackTrace);
                    AppendLine("Subtest 3-1 Result:  FAILED");
                }
                finally
                {
                    AppendLine("\r\nResetting flags.");
                    smartData = null;
                    smartThreshold = null;
                    isFailurePredicted = false;
                    resultOK = false;
                    AppendLine("Reset done.");
                    AppendLine("****** SUBTEST 3-1 IS COMPLETE ******\r\n");
                }


                isSsd = false;
                isTrimSupported = false;
                rpm = 65535;
                //temp = InteropSmart.GetInternalDriveIdentity(diskPath, GlobalConstants.DEFAULT_TARGET_ID, out isSsd, out isTrimSupported, out rpm);
                temp = InteropSmart.GetUsbDriveIdentity(diskPath, out isSsd, out isTrimSupported, out rpm);
                AppendLine("\r\nThe disk " + (isSsd ? "IS" : "is NOT") + " an SSD.");
                AppendLine("RPM: " + rpm.ToString() + "\r\n");

                try
                {
                    AppendLine("****** RUNNING TEST 4 of 5 *** Fetch SMART Data Using USB Drive Code ******\r\n");
                    if (diskID.ToUpper().Contains("USBSTOR"))
                    {
                        AppendLine("\tP/Invoke GetUsbSmartData");
                        
                        foreach (COMMAND_TYPE cmdType in Enum.GetValues(typeof(COMMAND_TYPE)))
                        {
                            if (cmdType == COMMAND_TYPE.CMD_TYPE_DEBUG || cmdType == COMMAND_TYPE.CMD_TYPE_PHYSICAL_DRIVE ||
                                cmdType == COMMAND_TYPE.CMD_TYPE_PROLIFIC || cmdType == COMMAND_TYPE.CMD_TYPE_SCSI_MINIPORT ||
                                cmdType == COMMAND_TYPE.CMD_TYPE_SILICON_IMAGE)
                            {
                                continue;
                            }
                            String cmdTypeString = cmdType.ToString();
                            AppendLine("\r\nTrying USB query with command type " + cmdTypeString + "...");
                            //resultOK = InteropSmart.GetUsbSmartData(diskPath, out smartData, out smartThreshold, out isFailurePredicted);
                            resultOK = InteropSmart.GetUsbSmartData(diskPath, cmdType, out smartData, out smartThreshold, out isFailurePredicted);

                            if (resultOK)
                            {
                                AppendLine("\tGetUsbSmartData returned true (" + cmdTypeString + "), so let's have a look at the first 30 values of attributes and thresholds.");
                                for (int i = 0; i < 30; i++)
                                {
                                    AppendLine("\t\tsmartData[" + i.ToString() + "] = " + smartData[i].ToString() + "\tsmartThreshold[" + i.ToString() + "] = " +
                                        smartThreshold[i].ToString());
                                }
                                isDiskSuccessful = true;
                            }
                            else
                            {
                                AppendLine("\t\tDamn, GetUsbSmartData returned false. The USB call (" + cmdTypeString + ") did not work for this disk.");
                            }
                        }

                    }
                    else
                    {
                        //AppendLine("\tWARNING: Disk is not USB; skipping.");
                        AppendLine("\tP/Invoke GetUsbSmartData");

                        foreach (COMMAND_TYPE cmdType in Enum.GetValues(typeof(COMMAND_TYPE)))
                        {
                            if (cmdType == COMMAND_TYPE.CMD_TYPE_DEBUG || cmdType == COMMAND_TYPE.CMD_TYPE_PHYSICAL_DRIVE ||
                                cmdType == COMMAND_TYPE.CMD_TYPE_PROLIFIC || cmdType == COMMAND_TYPE.CMD_TYPE_SCSI_MINIPORT ||
                                cmdType == COMMAND_TYPE.CMD_TYPE_SILICON_IMAGE)
                            {
                                continue;
                            }
                            String cmdTypeString = cmdType.ToString();
                            AppendLine("\r\nTrying USB query with command type " + cmdTypeString + "...");
                            //resultOK = InteropSmart.GetUsbSmartData(diskPath, out smartData, out smartThreshold, out isFailurePredicted);
                            resultOK = InteropSmart.GetUsbSmartData(diskPath, cmdType, out smartData, out smartThreshold, out isFailurePredicted);

                            if (resultOK)
                            {
                                AppendLine("\tGetUsbSmartData returned true (" + cmdTypeString + "), so let's have a look at the first 30 values of attributes and thresholds.");
                                for (int i = 0; i < 30; i++)
                                {
                                    AppendLine("\t\tsmartData[" + i.ToString() + "] = " + smartData[i].ToString() + "\tsmartThreshold[" + i.ToString() + "] = " +
                                        smartThreshold[i].ToString());
                                }
                                isDiskSuccessful = true;
                            }
                            else
                            {
                                AppendLine("\t\tDamn, GetUsbSmartData returned false. The USB call (" + cmdTypeString + ") did not work for this disk.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppendLine("Test 4 Exception: " + ex.Message);
                    AppendLine("Test 4 Stack Trace: " + ex.StackTrace);
                    AppendLine("Test 4 Result:  FAILED");
                }
                finally
                {
                    AppendLine("\r\nResetting flags.");
                    smartData = null;
                    smartThreshold = null;
                    isFailurePredicted = false;
                    resultOK = false;
                    AppendLine("Reset done.");
                    AppendLine("****** TEST 4 IS COMPLETE ******\r\n");
                }


                try
                {
                    AppendLine("****** RUNNING TEST 5 of 5 *** Fetch SMART Data Using WMI Code ******\r\n");
                    AppendLine("\tCall GetWmiSmartData");
                    resultOK = InteropSmart.GetWmiSmartData(diskID, out smartData, out smartThreshold, out isFailurePredicted);

                    if (resultOK)
                    {
                        AppendLine("\tGetWmiSmartData returned true, so let's have a look at the first 30 values of attributes and thresholds.");
                        for (int i = 0; i < 30; i++)
                        {
                            AppendLine("\t\tsmartData[" + i.ToString() + "] = " + smartData[i].ToString() + "\tsmartThreshold[" + i.ToString() + "] = " +
                                smartThreshold[i].ToString());
                        }
                        isDiskSuccessful = true;
                    }
                    else
                    {
                        AppendLine("\t\tDamn, GetWmiSmartData returned false. The WMI call did not work for this disk.");
                    }
                }
                catch (Exception ex)
                {
                    AppendLine("Test 5 Exception: " + ex.Message);
                    AppendLine("Test 5 Stack Trace: " + ex.StackTrace);
                    AppendLine("Test 5 Result:  FAILED");
                }
                finally
                {
                    AppendLine("\r\nResetting flags.");
                    smartData = null;
                    smartThreshold = null;
                    isFailurePredicted = false;
                    resultOK = false;
                    AppendLine("Reset done.");
                    AppendLine("****** TEST 5 IS COMPLETE ******\r\n");
                }


                if (isDiskSuccessful)
                {
                    AppendLine("Success! This disk returned SMART data for at least one test. The likelihood of this disk being supported");
                    AppendLine("by Home Server SMART is good *unless* the only test that worked was the SCSI test. I am not sure how reliable");
                    AppendLine("that test is yet.");
                    disksSuccessful++;
                }
                else
                {
                    AppendLine("Damn, this disk did not return any SMART data on any test. This disk is probably not going to be supported");
                    AppendLine("by this iteration of Home Server SMART.");
                    disksFailed++;
                }

                AppendLine("*** DONE PROCESSING " + diskPath + "\r\n\r\n");
            }

            AppendLine("TESTING COMPLETE!\r\n");

            AppendLine("\t****************");
            AppendLine("\t* TEST RESULTS *");
            AppendLine("\t****************\r\n");

            AppendLine("Total Devices Processed: " + disksProcessed.ToString());
            AppendLine("Devices Successfully Queried for SMART Data: " + disksSuccessful.ToString());
            AppendLine("Devices that FAILED to Return ANY SMART Data: " + disksFailed.ToString());

            AppendLine("\r\nNOTE: The SCSI test may not be accurate. However, disks that failed to return *ANY* SMART data");
            AppendLine("probably won't be supported by Home Server SMART unless I find other, better ways to query them.");
            AppendLine("\r\nPlease save these results and post to the forum or email back to me as requested.\r\n");
            AppendLine("Your assistance in testing is very much appreciated.  Danke!");
        }

        private void SaveResults()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            sfd.ShowDialog();

            if (sfd.FileName != String.Empty)
            {
                try
                {
                    System.IO.StreamWriter writer = System.IO.File.CreateText(sfd.FileName);
                    foreach (String line in textBox1.Lines)
                    {
                        writer.WriteLine(line);
                    }
                    writer.Flush();
                    writer.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exceptions were detected. " + ex.Message, "Severe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            IdentifySsd();
        }

        private void IdentifySsd()
        {
            MessageBox.Show("For best results, if you have any flash drives (including Magic Jack) attached to your Server, " +
                "please remove them at this time. This is NOT required but will prevent the test code from trying to process " +
                "them. All of the tests will fail for such a device and you'll have extra useless output cluttering up your " +
                "results. Click OK when ready.", "Detach Flash Drives", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            AppendLine("Home Server SMART Test Harness UI");
            AppendLine("Copyright (C) 2011 Dojo North Software - All Rights Reserved\r\n");

            AppendLine("This test will attempt to run ALL of the SMART access tests against your drive. They will be performed");
            AppendLine("as follows (each drive will be run through all tests before moving on to the next drive):");
            AppendLine("\t1. Run as an \"internal drive\" - this should get SMART data for most IDE/SATA drives.");
            AppendLine("\t2. Run as a Silicon Image SiI controller - disks attached to an SiI that don't yield results above should here.");
            AppendLine("\t3. Run as a SCSI disk. Success here is unlikely but we'll try it anyway.");
            AppendLine("\t\tSubtest: Try using WMI SCSITargetID instead of default target 0xA0 (160).");
            AppendLine("\t4. Run as a USB disk. Only USB disks with supported bridge chips should return success.");
            AppendLine("\t5. Test using \"old faithful\" WMI.\r\n");

            int disksProcessed = 0;
            int disksSuccessful = 0;
            int disksFailed = 0;

            AppendLine("Getting a list of the controllers and the disks attached to them (USB/FireWire not included).\r\n");

            AppendLine("\t************************");
            AppendLine("\t* IDE/SATA Controllers *");
            AppendLine("\t************************\r\n");

            try
            {
                List<String> sataControllers = new List<string>();
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                       "select Name, DeviceID from Win32_IDEController").Get())
                {
                    AppendLine("\tDetected Controller with DeviceID: " + drive["DeviceID"].ToString());
                    AppendLine("\t\tController Name: " + (drive["Name"] == null ? "NULL" : drive["Name"].ToString()));
                    sataControllers.Add(drive["DeviceID"].ToString());
                }

                foreach (String controller in sataControllers)
                {
                    AppendLine("\r\n*** FOUND the following device(s) attached to " + controller);
                    String testController = controller.Replace("\\", "\\\\");
                    try
                    {
                        foreach (ManagementObject drive in new ManagementObjectSearcher(
                               "ASSOCIATORS OF {Win32_IDEController.DeviceID=\"" + testController + "\"} WHERE AssocClass = Win32_IDEControllerDevice").Get())
                        {
                            AppendLine("\tID = " + drive["DeviceID"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendLine("*** FAILED to enumerate " + controller + ": " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLine("*** FAILED to enumerate IDE/SATA controllers: " + ex.Message);
            }

            AppendLine("\r\n\t********************");
            AppendLine("\t* SCSI Controllers *");
            AppendLine("\t********************\r\n");

            try
            {
                List<String> scsiControllers = new List<string>();
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                       "select Name, DeviceID from Win32_SCSIController").Get())
                {
                    AppendLine("\tDetected Controller with DeviceID: " + drive["DeviceID"].ToString());
                    AppendLine("\t\tController Name: " + (drive["Name"] == null ? "NULL" : drive["Name"].ToString()));
                    scsiControllers.Add(drive["DeviceID"].ToString());
                }

                foreach (String controller in scsiControllers)
                {
                    AppendLine("\r\n*** FOUND the following device(s) attached to " + controller);
                    String testController = controller.Replace("\\", "\\\\");
                    try
                    {
                        foreach (ManagementObject drive in new ManagementObjectSearcher(
                               "ASSOCIATORS OF {Win32_SCSIController.DeviceID=\"" + testController + "\"} WHERE AssocClass = Win32_SCSIControllerDevice").Get())
                        {
                            AppendLine("\tID = " + drive["DeviceID"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendLine("*** FAILED to enumerate " + controller + ": " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLine("*** FAILED to enumerate SCSI controllers: " + ex.Message);
            }

            // Browse all WMI physical disks.
            foreach (ManagementObject drive in new ManagementObjectSearcher(
              "select * from Win32_DiskDrive").Get())
            {
                String diskPath = String.Empty;
                String diskID = String.Empty;

                byte[] smartData = null;
                byte[] smartThreshold = null;
                bool isFailurePredicted = false;

                bool isDiskSuccessful = false;

                bool resultOK = false;

                disksProcessed++;

                try
                {
                    diskPath = drive["DeviceID"].ToString();
                    diskID = drive["PNPDeviceID"].ToString();
                }
                catch (Exception ex)
                {
                    AppendLine("\r\n*** WARNING! An exception occurred getting critical drive data (WMI attributes DeviceID and PNPDeviceID).");
                    AppendLine("*** Tests not available for this disk!  Exception message: " + ex.Message);
                    AppendLine("Disk Path: " + diskPath);
                    AppendLine("Disk PNP ID: " + diskID);
                    continue;
                }
                finally
                {

                }

                AppendLine("\r\n\t***************");
                AppendLine("\t* NOW TESTING *");
                AppendLine("\t***************");
                AppendLine("\tDisk Path: " + diskPath);
                AppendLine("\tDisk PNP ID: " + diskID + "\r\n");

                bool isSsd = false;
                bool isTrimSupported = false;
                UInt32 rpm = 65535;
                IDENTIFY_DEVICE_RESULT result = IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_NULL;
                String exceptionMessage = String.Empty;

                try
                {
                    AppendLine("****** RUNNING TEST 1 of 3 *** Fetch Disk Identity Data Using Internal Drive Code ******\r\n");
                    AppendLine("\tP/Invoke GetInternalDriveIdentity as non-SiI disk");
                    resultOK = InteropSmart.GetInternalDriveIdentity(diskPath,
                        GlobalConstants.DEFAULT_TARGET_ID, out isSsd, out isTrimSupported, out rpm, out result, out exceptionMessage);

                    if (resultOK)
                    {
                        AppendLine("\tGetInternalDriveIdentity returned true, so let's check the RPM, SSD and TRIM.");
                        AppendLine("\r\n\t\tThe disk " + (isSsd ? "IS" : "is NOT") + " an SSD.");
                        if (isSsd)
                        {
                            AppendLine("\t\tThe disk " + (isTrimSupported ? "supports" : "does NOT support") + " TRIM.");
                        }
                        AppendLine("\t\tRPM: " + rpm.ToString() + (rpm == 0 ? " WARNING: Zero RPM detecrted (this MAY be an OLDER SSD)\r\n" : "\r\n"));
                        isDiskSuccessful = true;
                    }
                    else
                    {
                        AppendLine("\t\tDamn, GetInternalDriveIdentity returned false. The internal call did not work for this disk.");
                        AppendLine("\t\tControl Code Returned: " + result.ToString());
                        AppendLine("\t\tMessage: " + (String.IsNullOrEmpty(exceptionMessage) ? "No Message Returned" : exceptionMessage));
                    }
                }
                catch (Exception ex)
                {
                    AppendLine("Test 1 Exception: " + ex.Message);
                    AppendLine("Test 1 Stack Trace: " + ex.StackTrace);
                    AppendLine("Test 1 Result:  FAILED");
                }
                finally
                {
                    AppendLine("\r\nResetting flags.");
                    isSsd = false;
                    isTrimSupported = false;
                    rpm = 65535;
                    resultOK = false;
                    result = IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_NULL;
                    exceptionMessage = String.Empty;
                    AppendLine("Reset done.");
                    AppendLine("****** TEST 1 IS COMPLETE ******\r\n");
                }

                try
                {
                    AppendLine("****** RUNNING TEST 2 of 3 *** Fetch Disk Identity Data Using USB Drive Code ******\r\n");
                    if (diskID.ToUpper().Contains("USBSTOR"))
                    {
                        AppendLine("\tP/Invoke GetUsbSmartData");

                        foreach (COMMAND_TYPE cmdType in Enum.GetValues(typeof(COMMAND_TYPE)))
                        {
                            if (cmdType == COMMAND_TYPE.CMD_TYPE_DEBUG || cmdType == COMMAND_TYPE.CMD_TYPE_PHYSICAL_DRIVE ||
                                cmdType == COMMAND_TYPE.CMD_TYPE_PROLIFIC || cmdType == COMMAND_TYPE.CMD_TYPE_SCSI_MINIPORT ||
                                cmdType == COMMAND_TYPE.CMD_TYPE_SILICON_IMAGE)
                            {
                                continue;
                            }
                            String cmdTypeString = cmdType.ToString();
                            AppendLine("\r\nTrying USB query with command type " + cmdTypeString + "...");
                            //resultOK = InteropSmart.GetUsbSmartData(diskPath, out smartData, out smartThreshold, out isFailurePredicted);
                            resultOK = InteropSmart.GetUsbDriveIdentity(diskPath, cmdType, out isSsd, out isTrimSupported, out rpm,
                                out result, out exceptionMessage);

                            if (resultOK)
                            {
                                AppendLine("\tGetInternalDriveIdentity returned true, so let's check the RPM, SSD and TRIM.");
                                AppendLine("\r\n\t\tThe disk " + (isSsd ? "IS" : "is NOT") + " an SSD.");
                                if (isSsd)
                                {
                                    AppendLine("\t\tThe disk " + (isTrimSupported ? "supports" : "does NOT support") + " TRIM.");
                                }
                                AppendLine("\t\tRPM: " + rpm.ToString() + (rpm == 0 ? " WARNING: Zero RPM detecrted (this MAY be an OLDER SSD)\r\n" : "\r\n"));
                                isDiskSuccessful = true;
                            }
                            else
                            {
                                AppendLine("\t\tDamn, GetUsbSmartData returned false. The USB call (" + cmdTypeString + ") did not work for this disk.");
                                AppendLine("\t\tControl Code Returned: " + result.ToString());
                                AppendLine("\t\tMessage: " + (String.IsNullOrEmpty(exceptionMessage) ? "No Message Returned" : exceptionMessage));
                            }
                        }
                    }
                    else
                    {
                        AppendLine("\tWARNING: Disk is not USB; skipping.");
                    }
                }
                catch (Exception ex)
                {
                    AppendLine("Test 2 Exception: " + ex.Message);
                    AppendLine("Test 2 Stack Trace: " + ex.StackTrace);
                    AppendLine("Test 2 Result:  FAILED");
                }
                finally
                {
                    AppendLine("\r\nResetting flags.");
                    isSsd = false;
                    isTrimSupported = false;
                    rpm = 65535;
                    resultOK = false;
                    result = IDENTIFY_DEVICE_RESULT.IDENTIFY_DEVICE_NULL;
                    exceptionMessage = String.Empty;
                    AppendLine("Reset done.");
                    AppendLine("****** TEST 2 IS COMPLETE ******\r\n");
                }
            }
        }
    }
}
