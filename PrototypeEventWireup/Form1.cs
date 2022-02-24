using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PrototypeEventWireup
{
    public partial class Form1 : Form
    {
        private bool isSourceRegistered = false;
        private String eventSource = String.Empty;
        private String severity = String.Empty;
        private String eventMessage = String.Empty;
        private String eventTitle = String.Empty;
        private int eventID = 0;

        public Form1()
        {
            InitializeComponent();

            try
            {
                isSourceRegistered = WindowsEventLogger.IsEventSourceRegistered(Properties.Resources.EventLogTaryn) &&
                    WindowsEventLogger.IsEventSourceRegistered(Properties.Resources.EventLogJoshua);
                if (isSourceRegistered)
                {
                    buttonRegister.Enabled = false;
                    buttonUnregister.Enabled = true;
                }
                else
                {
                    buttonRegister.Enabled = true;
                    buttonUnregister.Enabled = false;
                }
            }
            catch
            {
                buttonRegister.Enabled = true;
                buttonUnregister.Enabled = false;
            }

            cbEvent.SelectedIndex = 8;
            cbDiskModel.SelectedIndex = 0;
            eventSource = Properties.Resources.EventLogTaryn;
            severity = "Error";
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            bool taryn = false;
            bool joshua = false;

            try
            {
                taryn = WindowsEventLogger.IsEventSourceRegistered(Properties.Resources.EventLogTaryn);
                if (!taryn)
                {
                    WindowsEventLogger.RegisterEventSource(Properties.Resources.EventLogTaryn);
                    taryn = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to register event source WindowSMART-W: " + ex.Message, "Severe",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

            try
            {
                joshua = WindowsEventLogger.IsEventSourceRegistered(Properties.Resources.EventLogJoshua);
                if (!joshua)
                {
                    WindowsEventLogger.RegisterEventSource(Properties.Resources.EventLogJoshua);
                    joshua = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to register event source WindowSMART-E: " + ex.Message, "Severe",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

            if (taryn && joshua)
            {
                buttonRegister.Enabled = false;
                buttonUnregister.Enabled = true;
                MessageBox.Show("Event sources were successfully registered. You may now create sample events.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("At least one event source failed to register. You can try to create events, but they may not get logged properly.", "Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void buttonUnregister_Click(object sender, EventArgs e)
        {
            bool taryn = true;
            bool joshua = true;

            try
            {
                taryn = WindowsEventLogger.IsEventSourceRegistered(Properties.Resources.EventLogTaryn);
                if (taryn)
                {
                    WindowsEventLogger.UnregisterEventSource(Properties.Resources.EventLogTaryn);
                    taryn = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to register event source WindowSMART-W: " + ex.Message, "Severe",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                taryn = true;
            }

            try
            {
                joshua = WindowsEventLogger.IsEventSourceRegistered(Properties.Resources.EventLogJoshua);
                if (joshua)
                {
                    WindowsEventLogger.UnregisterEventSource(Properties.Resources.EventLogJoshua);
                    joshua = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to register event source WindowSMART-E: " + ex.Message, "Severe",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                joshua = true;
            }

            if (!taryn && !joshua)
            {
                buttonRegister.Enabled = true;
                buttonUnregister.Enabled = false;
                MessageBox.Show("Event sources were successfully unregistered.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("At least one event source failed to unregister.", "Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoWireup();
        }

        private void DoWireup()
        {
            String eventString = cbEvent.SelectedItem.ToString();
            if (String.Compare(eventString.Substring(0, 5), "-----", true) == 0)
            {
                label10.Text = "Invalid event. Please select a proper event.";
                tbEvent.Text = "Invalid event. Please select a proper event.";
                buttonCreate.Enabled = false;
            }
            else
            {
                WireUpEvent(eventString);
                buttonCreate.Enabled = true;
            }
        }

        private void WireUpEvent(String eventString)
        {
            Guid myGuid = Guid.NewGuid();
            String guidString = myGuid.ToString("B");
            String alertText = String.Empty;

            int eventCount = (int)numericProblemCount.Value;
            int temperature = (int)numericTemperature.Value;
            String diskPath = @"\\.\PHYSICALDRIVE" + numericDiskNumber.Value.ToString();
            String model = cbDiskModel.SelectedItem == null ? "Unknown" : cbDiskModel.SelectedItem.ToString();
            bool isIssd = IsDiskSsd(model);
            String attributeID = "0";
            String attributeName = String.Empty;
            bool displayDiskInfo = true;

            switch (eventString.Substring(0, 5))
            {
                case "53820":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        eventTitle = isIssd ? "SSD Temperature Critical" : "HDD Temperature Critical";
                        attributeID = "194";
                        attributeName = "Temperature";
                        alertText = "Your disk temperature is critical. Check fans, enclosures and airflow, and get the disk cooled NOW. Continued operation at such an " +
                            "extreme temperature can destroy the disk, even if it's only for a short period. Temperature = " + temperature.ToString() + "C.";
                        break;
                    }
                case "53821":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        eventTitle = isIssd ? "SSD Temperature Overheated" : "HDD Temperature Overheated";
                        attributeID = "194";
                        attributeName = "Temperature";
                        alertText = "Disk temperature is overheated. Cool the disk immediately. Long-term operation of an overheated disk will cause permanent damage. " +
                            "Temperature = " + temperature.ToString() + "C.";
                        break;
                    }
                case "53822":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = isIssd ? "SSD Temperature Hot" : "HDD Temperature Hot";
                        attributeID = "194";
                        attributeName = "Temperature";
                        alertText = "Disk temperature is hot. Long-term operation of a hot disk may cause permanent damage. Check fans and airflow for obstructions and proper operation." +
                            "Temperature = " + temperature.ToString() + "C.";
                        break;
                    }
                case "53823":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = isIssd ? "SSD Temperature Warm" : "HDD Temperature Warm";
                        attributeID = "194";
                        attributeName = "Temperature";
                        alertText = "Disk may be getting hot. Keep a close eye on it. Check fans and airflow for obstructions and proper operation." +
                            "Temperature = " + temperature.ToString() + "C.";
                        break;
                    }
                case "53824":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        eventTitle = "Disk Health Critical";
                        attributeID = "0";
                        attributeName = "Critical Disk";
                        alertText = "The health of this disk is Critical. It is recommended that you replace it as soon as possible, or " +
                            "if the problem(s) are correctable (such as temperature), correct the problems immediately.";
                        break;
                    }
                case "53825":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "Disk Health Warning";
                        attributeID = "0";
                        attributeName = "Warning Disk";
                        alertText = "The health of this disk is questionable. It is recommended that you keep a close eye on it. If its health " +
                            "degrades further, you should replace it. If the problem(s) are correctable (such as temperature), correct the problems as soon as possible.";
                        break;
                    }
                case "53826":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "Disk Health Geriatric";
                        attributeID = "0";
                        attributeName = "Geriatric Disk";
                        alertText = "At least one S.M.A.R.T. attribute value has reached zero. The disk may be reaching the end of its life. " +
                            "If no other problems are reported, a geriatric disk is not usually a problem.";
                        break;
                    }
                case "53830":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        eventTitle = "High Bad Sector Count";
                        attributeID = "5";
                        attributeName = "Reallocated Sector Count";
                        if (isIssd)
                        {
                            alertText = "There are " + eventCount.ToString() + " retired blocks in the SSD flash memory. Their contents have " +
                                "been reallocated to the spare area.";
                        }
                        else
                        {
                            alertText = "There are " + eventCount.ToString() + " bad sectors on the disk surface. Their contents have " +
                                "been reallocated to the spare area.";
                        }
                        break;
                    }
                case "53831":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "Bad Sectors Detected";
                        attributeID = "5";
                        attributeName = "Reallocated Sector Count";
                        if (isIssd)
                        {
                            alertText = "There are " + eventCount.ToString() + " retired blocks in the SSD flash memory. Their contents have " +
                                "been reallocated to the spare area.";
                        }
                        else
                        {
                            alertText = "There " + (eventCount == 1 ? "is 1 bad sector" : "are " + eventCount.ToString() + " bad sectors") +
                                " on the disk surface. Their contents have been reallocated to the spare area.";
                        }
                        break;
                    }
                case "53832":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        eventTitle = "End-to-End Errors Detected";
                        attributeID = "184";
                        attributeName = "End-to-End Error";
                        alertText = "Multiple end-to-end errors have been detected on the disk. End-to-end errors indicate data corruption " +
                            "in the disk's cache RAM. These errors are exceedingly rare, but are severe if they appear.";
                        break;
                    }
                case "53833":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "End-to-End Errors Detected";
                        attributeID = "184";
                        attributeName = "End-to-End Error";
                        alertText = "At least one end-to-end error has been detected on the disk. End-to-end errors indicate data corruption " +
                            " in the disk's cache RAM. These errors are exceedingly rare, but are severe if they appear.";
                        break;
                    }
                case "53834":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        eventTitle = "High Reallocation Event Count";
                        attributeID = "196";
                        attributeName = "Reallocation Event Count";
                        alertText = "There have been " + eventCount.ToString() +
                            " attempted sector reallocation events on the disk.";
                        break;
                    }
                case "53835":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "Reallocation Events Detected";
                        attributeID = "196";
                        attributeName = "Reallocation Event Count";
                        alertText = (eventCount == 1 ? "There has been " + eventCount.ToString() +
                                " attempted sector reallocation event on the disk." : "There have been " + eventCount.ToString() +
                                " attempted sector reallocation events on the disk.");
                        break;
                    }
                case "53836":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        eventTitle = "High Pending Sector Count";
                        attributeID = "197";
                        attributeName = "Current Pending Sector Count";
                        alertText = "There are " + eventCount.ToString() + " unstable sectors that may be reallocated.";
                        break;
                    }
                case "53837":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "Pending Sectors Detected";
                        attributeID = "197";
                        attributeName = "Current Pending Sector Count";
                        alertText = (eventCount == 1 ? "There is " + eventCount.ToString() + " unstable sector " +
                                "that is waiting to be reallocated." : "There are " + eventCount.ToString() + " sectors " +
                                "that are waiting to be reallocated.");
                        break;
                    }
                case "53838":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        eventTitle = "High Uncorrectable Sector Count";
                        attributeID = "198";
                        attributeName = "Offline Uncorrectable Sector Count";
                        alertText = "The disk detected " + eventCount.ToString() + " bad sectors during its offline scan.";
                        break;
                    }
                case "53839":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "Uncorrectable Sectors Detected";
                        attributeID = "198";
                        attributeName = "Offline Uncorrectable Sector Count";
                        alertText = (eventCount == 1 ? "The disk detected " + eventCount.ToString() +
                                " bad sector during its offline scan." : "The disk detected " + eventCount.ToString() +
                                " bad sectors during its offline scan.");
                        break;
                    }
                case "53840":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        eventTitle = "High Spin Retry Count";
                        attributeID = "10";
                        attributeName = "Spin Retry Count";
                        alertText = "The drive motor has encountered difficulty spinning up " + eventCount.ToString() + " times. " +
                            "This may indicate a failing drive motor or weak power supply.";
                        break;
                    }
                case "53841":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "Spin Retries Detected";
                        attributeID = "10";
                        attributeName = "Spin Retry Count";
                        alertText = "The drive motor has encountered difficulty spinning up " + eventCount.ToString() + " times. " +
                                "This may indicate a failing drive motor or weak power supply.";
                        break;
                    }
                case "53842":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "High CRC Error Count";
                        attributeID = "199";
                        attributeName = "Ultra DMA CRC Error Count";
                        alertText = "There have been " + eventCount.ToString() +
                                " CRC errors detected on the disk. These are usually caused by a bad or oxidized data cable " +
                                "(or connectors), and in some cases may be caused by faulty USB bridge chips in external enclosures. " +
                                "These errors do not usually indicate a drive failure, but they can result in data corruption. Check " +
                                "the cables and connectors, and also verify the bridge chip is supported in this version of Windows.";
                        break;
                    }
                case "53844":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        eventTitle = "High Write Error Count";
                        attributeID = "196";
                        attributeName = "Bad Cluster Table Count (ECC Fail Count)";
                        alertText = "There have been " + eventCount.ToString() +
                                    " write errors detected by the SSD controller.";
                        break;
                    }
                case "53845":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "Write Errors Detected";
                        attributeID = "196";
                        attributeName = "Bad Cluster Table Count (ECC Fail Count)";
                        alertText = "There have been " + eventCount.ToString() +
                                    " write errors detected by the SSD controller.";
                        break;
                    }
                case "53846":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        eventTitle = "Runtime Bad Blocks Detected";
                        attributeID = "183";
                        attributeName = "Runtime Bad Block";
                        alertText = "A high number of bad blocks have been detected on the SSD during runtime. This could indicate a problem writing to the SSD media, or the SSD media is wearing out.";
                        break;
                    }
                case "53847":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "Runtime Bad Blocks Detected";
                        attributeID = "183";
                        attributeName = "Runtime Bad Block";
                        alertText = "Bad blocks have been detected on the SSD during runtime. This could indicate a problem writing to the SSD media, or the SSD media is wearing out.";
                        break;
                    }
                case "53848":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "SSD Life Curve Status - Throttled";
                        attributeID = "230";
                        attributeName = "Life Curve Status";
                        alertText = "The SSD life curve status of the disk has been set to throttled. This is done by the SSD when the life curve suggests the writing to the disk will violate the warranty terms.";
                        break;
                    }
                case "53849":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        eventTitle = "SSD Life Left - 0-10%";
                        attributeID = "209";
                        attributeName = "SSD Life Left";
                        alertText = "The estimated SSD life left is 10% or less. The SSD may no longer allow writing in order to preserve the integrity of the data on the disk. You should make a backup of the data on the disk, and replace the disk as soon as possible.";
                        break;
                    }
                case "53850":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "SSD Life Left - 10-30%";
                        attributeID = "209";
                        attributeName = "SSD Life Left";
                        alertText = "The estimated SSD life left is 30% or less, but greater than 10%. Depending on how heavily you write to this disk, you may want to consider replacing it soon.";
                        break;
                    }
                case "53851":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Information";
                        eventTitle = "";
                        attributeID = "";
                        attributeName = "";
                        alertText = "An alert item is being removed. This may be because the alert is no longer active, or an existing alert " +
                            "has changed. Alert correlation ID: " + Guid.NewGuid().ToString("B");
                        displayDiskInfo = false;
                        break;
                    }
                case "53852":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "";
                        attributeID = "";
                        attributeName = "";
                        alertText = "Disk: " + model + ", Path: " + diskPath + ", GUID: " + Guid.NewGuid().ToString("B") +
                                    " did not return any valid SMART data. The disk health is unknown.";
                        displayDiskInfo = false;
                        break;
                    }
                case "53853":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        eventTitle = "SMART Threshold Exceeded";
                        attributeID = "184";
                        attributeName = "End-to-End Error";
                        alertText = "SMART Threshold Exceeded Condition (TEC) has occurred on one or more attributes. This indicates imminent failure (may be less than 24 hours). Back up the data on the disk and replace the disk as soon as possible. If the failing attribute is temperature-related, cooling the disk may correct this problem.";
                        break;
                    }
                case "53854":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "SMART Threshold Met";
                        attributeID = "184";
                        attributeName = "End-to-End Error";
                        alertText = "SMART value is equal to the non-zero threshold for one or more values. If the value drops any further, a Threshold Exceeded Condition (TEC) will exist, which indicates imminent failure. You should keep a very close eye on this disk; you may want to consider replacing it now. If the failing attribute is temperature-related, cooling the disk may correct this problem.";
                        break;
                    }
                case "53855":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        eventTitle = "";
                        attributeID = "";
                        attributeName = "";
                        alertText = "At least one disk has remained in an overheated or critically hot state for 3 or more consecutive disk polling intervals. " +
                                "Per the configured option \"perform thermal shutdown,\" this computer will be shut down in 1 minute.";
                        displayDiskInfo = false;
                        break;
                    }
                case "53856":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        eventTitle = "";
                        attributeID = "";
                        attributeName = "";
                        alertText = "At least one disk has remained in an overheated or critically hot state for 3 or more consecutive disk polling intervals.";
                        displayDiskInfo = false;
                        break;
                    }
                case "53860":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Information";
                        alertText = "Starting a SMART disk polling operation in Automatic mode.";
                        displayDiskInfo = false;
                        break;
                    }
                case "53861":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Information";
                        alertText = "Completed an automatic disk polling operation.\n\nStart: 12/8/2012 4:09:58 PM\nComplete: 12/8/2012 4:10:00 PM\nExecution Time: 0 day(s), 0 hour(s), 0 minute(s), 2 second(s), 420 millisecond(s)";
                        displayDiskInfo = false;
                        break;
                    }
                case "53862":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        alertText = "Exceptions were detected validating the logfile path G:\\MyLogfilePath" +
                            ". The system cannot find the path specified. The Server will attempt to use the default logging location.";
                        displayDiskInfo = false;
                        break;
                    }
                case "53863":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        alertText = "Exceptions were detected configuring the default logfile path C:\\ProgramData\\Dojo North Software. SmartInspect logging will NOT " +
                            "be available. Access is denied.";
                        displayDiskInfo = false;
                        break;
                    }
                case "53864":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        alertText = "Exceptions were detected accessing path C:\\ProgramData\\Dojo North Software for logging. It cannot be used. " +
                            "HSS/WS2 attempted to write a temporary ile to this location (prior to logfile generation) to validate the Server's " +
                            "ability to write a file to this location. That attempt failed. Please check that the folder exists and also check " +
                            "the NTFS permissions on the folder to verify that the service (for service errors) or your user ID (for UI errors) " +
                            "has sufficient perms to write to this location. For service-related errors, add the SYSTEM account with Modify " +
                            "or Full Control perms. For UI-related errors, add your user ID with Modify or Full Control perms.\n\nIf this error " +
                            "is immediately preceded by error 53863 or warning 53862, try creating the desired logfile locations manually and " +
                            "assigning appropriate NTFS perms, and then restart the service.\n\nAccess is denied.";
                        displayDiskInfo = false;
                        break;
                    }
                case "53865":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Information";
                        alertText = "WindowSMART 24/7 application update is available! You're running version 3.0.12.3; the available version is 2.2.10.3, released on 10/3/2012. For more details, please visit http://www.dojonorthsoftware.net/TBM/WindowSMART.aspx. To go directly to the download page, please visit http://www.dojonorthsoftware.net/Downloads.aspx.";
                        displayDiskInfo = false;
                        break;
                    }
                case "53866":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        alertText = "A thermal shutdown has been ordered.";
                        displayDiskInfo = false;
                        break;
                    }
                case "53867":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        alertText = "Demolition of " + model + " failed: An overlapped I/O operation was in progress.";
                        displayDiskInfo = false;
                        break;
                    }
                case "53868":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        alertText = "Demolition failed: Thread was being aborted.";
                        displayDiskInfo = false;
                        break;
                    }
                case "53869":
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Error";
                        alertText = "The backup target is on D:, which is a drive that indicates it " +
                            "will fail. Emergency Backup cannot proceed.";
                        displayDiskInfo = false;
                        break;
                    }
                case "53880":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Error";
                        String stackTrace = String.Empty;
                        String exceptionMessage = String.Empty;
                        ThrowSampleException(out exceptionMessage, out stackTrace);
                        alertText = "ServiceAutoPollDisks failed: " + exceptionMessage +
                            "\n\n" + stackTrace;
                        displayDiskInfo = false;
                        break;
                    }
                case "53881":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Error";
                        String stackTrace = String.Empty;
                        String exceptionMessage = String.Empty;
                        ThrowSampleException(out exceptionMessage, out stackTrace);
                        alertText = exceptionMessage + "\r\n\r\n" + stackTrace;
                        displayDiskInfo = false;
                        break;
                    }
                case "53882":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Error";
                        String stackTrace = String.Empty;
                        String exceptionMessage = String.Empty;
                        ThrowSampleException(out exceptionMessage, out stackTrace);
                        alertText = "HssServiceHelper constructor failed: " + exceptionMessage +
                            "\n\n" + stackTrace;
                        displayDiskInfo = false;
                        break;
                    }
                case "53883":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Error";
                        String stackTrace = String.Empty;
                        String exceptionMessage = String.Empty;
                        ThrowSampleException(out exceptionMessage, out stackTrace);
                        alertText = "Exceptions were detected in the service worker thread. " + exceptionMessage +
                            "\n\nThe service is halting.";
                        displayDiskInfo = false;
                        break;
                    }
                case "53884":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Error";
                        String stackTrace = String.Empty;
                        String exceptionMessage = String.Empty;
                        ThrowSampleException(out exceptionMessage, out stackTrace);
                        alertText = "CheckForFeverishDisks failed: " +
                            exceptionMessage + "\n\n" + stackTrace;
                        displayDiskInfo = false;
                        break;
                    }
                case "53885":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Error";
                        String stackTrace = String.Empty;
                        String exceptionMessage = String.Empty;
                        ThrowSampleException(out exceptionMessage, out stackTrace);
                        alertText = "Failed to clear stale disks: " + exceptionMessage;
                        displayDiskInfo = false;
                        break;
                    }
                case "53886":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Warning";
                        String stackTrace = String.Empty;
                        String exceptionMessage = String.Empty;
                        ThrowSampleException(out exceptionMessage, out stackTrace);
                        alertText = "Exceptions were detected checking for automatic updates: " + exceptionMessage + "\n\n" + stackTrace;
                        displayDiskInfo = false;
                        break;
                    }
                case "53887":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Warning";
                        alertText = "Failed to map G: to UNC path \\\\mcdonalds\\bigmac: The referenced account is currently locked out and may not be logged on to.";
                        displayDiskInfo = false;
                        break;
                    }
                case "53888":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Error";
                        alertText = "Failed to map any drive to UNC path \\\\mcdonalds\\bigmac";
                        displayDiskInfo = false;
                        break;
                    }
                case "53889":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Error";
                        alertText = "A thermal shutdown was ordered, but an error occurred processing the shutdown request. Access is denied.";
                        displayDiskInfo = false;
                        break;
                    }
                case "53890":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Error";
                        String stackTrace = String.Empty;
                        String exceptionMessage = String.Empty;
                        ThrowSampleException(out exceptionMessage, out stackTrace);
                        alertText = "Exception was thrown during the thermal shutdown check. " + exceptionMessage;
                        displayDiskInfo = false;
                        break;
                    }
                case "53891":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Error";
                        String stackTrace = String.Empty;
                        String exceptionMessage = String.Empty;
                        ThrowSampleException(out exceptionMessage, out stackTrace);
                        alertText = exceptionMessage;
                        displayDiskInfo = false;
                        break;
                    }
                case "53892":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Error";
                        String stackTrace = String.Empty;
                        String exceptionMessage = String.Empty;
                        ThrowSampleException(out exceptionMessage, out stackTrace);
                        alertText = "Unable to bind Windows Service \"dnhsSmart\" (WindowSMART Service) in the service controller. Some " +
                            "management transactions will not be possible. " + exceptionMessage;
                        displayDiskInfo = false;
                        break;
                    }
                case "53893":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Error";
                        String stackTrace = String.Empty;
                        String exceptionMessage = String.Empty;
                        ThrowSampleException(out exceptionMessage, out stackTrace);
                        alertText = "Emergency Backup cannot run - Unable to read the desired backup source names from the Registry. " + exceptionMessage;
                        displayDiskInfo = false;
                        break;
                    }
                case "53894":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Error";
                        String stackTrace = String.Empty;
                        String exceptionMessage = String.Empty;
                        ThrowSampleException(out exceptionMessage, out stackTrace);
                        alertText = "Disk " + model + ", path " + diskPath + " - failed to set the hot TEC flag in the Registry to TRUE. " + exceptionMessage;
                        displayDiskInfo = false;
                        break;
                    }
                case "53895":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Error";
                        String stackTrace = String.Empty;
                        String exceptionMessage = String.Empty;
                        ThrowSampleException(out exceptionMessage, out stackTrace);
                        alertText = "Disk " + model + ", path " + diskPath + " - failed to set the general TEC flag in the Registry to TRUE. " + exceptionMessage;
                        displayDiskInfo = false;
                        break;
                    }
                case "53896":
                    {
                        eventSource = Properties.Resources.EventLogJoshua;
                        severity = "Error";
                        String stackTrace = String.Empty;
                        String exceptionMessage = String.Empty;
                        ThrowSampleException(out exceptionMessage, out stackTrace);
                        alertText = "Exception was thrown during the emergency backup TEC check. " + exceptionMessage;
                        displayDiskInfo = false;
                        break;
                    }
                default:
                    {
                        eventSource = Properties.Resources.EventLogTaryn;
                        severity = "Warning";
                        eventTitle = "Default Bucket";
                        attributeID = "65535";
                        attributeName = "Default bucket - " + eventString;
                        break;
                    }
            }

            tbSource.Text = eventSource;

            if (severity == "Error")
            {
                tbSeverity.Text = "Error";
                if (displayDiskInfo)
                {
                    eventMessage = "A Critical disk event (" + eventTitle + ") was detected: " + alertText +
                        "\n\nDisk: " + model + ", Path: " + diskPath + ", Attribute ID: " + attributeID + ", Attribute: " + attributeName +
                        ", Correlation ID: " + myGuid;
                }
                else
                {
                    eventMessage = alertText;
                }
            }
            else if (severity == "Warning")
            {
                tbSeverity.Text = "Warning";
                if (displayDiskInfo)
                {
                    eventMessage = "A Warning disk event (" + eventTitle + ") was detected: " + alertText +
                        "\n\nDisk: " + model + ", Path: " + diskPath + ", Attribute ID: " + attributeID + ", Attribute: " + attributeName +
                        ", Correlation ID: " + myGuid;
                }
                else
                {
                    eventMessage = alertText;
                }
            }
            else
            {
                tbSeverity.Text = "Information";
                if (displayDiskInfo)
                {
                    eventMessage = alertText;
                }
                else
                {
                    eventMessage = alertText;
                }
            }
            label10.Text = "The following " + severity + " event will be logged with event source WindowSMART-W with the event ID " + eventString.Substring(0, 5);
            eventID = Int32.Parse(eventString.Substring(0, 5));
            tbEvent.Text = eventMessage.Replace("\n", "\r\n");
        }

        private bool IsDiskSsd(String model)
        {
            String modelToLower = model.ToLower();
            if (modelToLower.Contains("ocz") || modelToLower.Contains("intel") || modelToLower.Contains("samsung"))
            {
                return true;
            }
            return false;
        }

        private void numericProblemCount_ValueChanged(object sender, EventArgs e)
        {
            DoWireup();
        }

        private void numericTemperature_ValueChanged(object sender, EventArgs e)
        {
            DoWireup();
        }

        private void numericDiskNumber_ValueChanged(object sender, EventArgs e)
        {
            DoWireup();
        }

        private void cbDiskModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoWireup();
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (severity == "Error")
                {
                    WindowsEventLogger.LogError(eventMessage, eventID);
                }
                else if (severity == "Warning")
                {
                    WindowsEventLogger.LogWarning(eventMessage, eventID);
                }
                else
                {
                    WindowsEventLogger.LogInformation(eventMessage, eventID);
                }

                MessageBox.Show("Successfully wrote event to the Application event log.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to write event to the Application event log: " + ex.Message, "Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ThrowSampleException(out String exceptionMessage, out String stackTrace)
        {
            exceptionMessage = String.Empty;
            stackTrace = String.Empty;
            try
            {
                throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                stackTrace = ex.StackTrace;
            }
        }
    }
}
