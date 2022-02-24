using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Gurock.SmartInspect;
using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class SelfTests : Form
    {
        private bool ignoreVirtualDisks;
        private bool advancedSii;
        private bool fallbackToWmi;
        private bool isWorkerThreadRunning;
        private bool isWindows8;

        private Thread refreshThread;

        private delegate void ListViewPopulator();
        private ListViewPopulator dlgPopulateListView;

        public SelfTests(bool fallback, bool sii, bool ignoreVirtual)
        {
            InitializeComponent();
            isWindows8 = Utilities.Utility.IsSystemWindows8();
            fallbackToWmi = fallback;
            advancedSii = sii;
            ignoreVirtualDisks = ignoreVirtual;
            isWorkerThreadRunning = false;
            dlgPopulateListView = new ListViewPopulator(PopulateListView);
        }

        private void SelfTests_Load(object sender, EventArgs e)
        {
            PopulateListView();
        }

        private void ManualRefresh()
        {
            if (isWorkerThreadRunning)
            {
                return;
            }
            isWorkerThreadRunning = true;
            try
            {
                String phantomDiskList = String.Empty;
                DiskEnumerator.RefreshDiskInfo(true, fallbackToWmi, advancedSii, ignoreVirtualDisks, out phantomDiskList);
                this.Invoke(dlgPopulateListView);
            }
            catch
            {
            }
            isWorkerThreadRunning = false;
        }

        private void PopulateListView()
        {
            try
            {
                // Browse all WMI physical disks.
                String wmiQuery = isWindows8 ? Properties.Resources.WmiQueryStringWin8 : Properties.Resources.WmiQueryStringNonWin8;
                String queryScope = isWindows8 ? Properties.Resources.WmiQueryScope : Properties.Resources.WmiQueryScopeDefault;

                foreach (ManagementObject drive in new ManagementObjectSearcher(queryScope, wmiQuery).Get()) 
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
                    String model = String.Empty;
                    String pnpId = String.Empty;

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
                        SiAuto.Main.LogWarning("Could not parse " + diskIDNumber + " to an integer; skipping this disk.");
                        ssDiskNumber = -1;
                    }

                    SiAuto.Main.LogBool("isWindows8", isWindows8);
                    SiAuto.Main.LogMessage("Attempt to get preferred model name.");
                    model = Utilities.Utility.GetDiskPreferredModel(drive, isWindows8);
                    if (isWindows8)
                    {
                        if (String.IsNullOrEmpty(model))
                        {
                            SiAuto.Main.LogMessage("The model name returned was null or empty; trying to concatenate Manufacturer + Model via WMI.");
                            try
                            {
                                model = drive["Manufacturer"].ToString() + drive["Model"].ToString();
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            SiAuto.Main.LogString("Model name was returned.", model);
                        }
                    }

                    try
                    {
                        if (String.IsNullOrEmpty(model))
                        {
                            SiAuto.Main.LogWarning("Model is still null or empty; trying additional methods.");
                            if (String.IsNullOrEmpty(model) && drive["Model"] != null)
                            {
                                SiAuto.Main.LogMessage("Trying non-null Model.");
                                model = drive["Model"].ToString();
                            }
                            else if (String.IsNullOrEmpty(model) && drive["Model"] == null && drive["PNPDeviceID"] != null)
                            {
                                SiAuto.Main.LogMessage("Trying non-null PNPDeviceID.");
                                model = drive["PNPDeviceID"].ToString();
                            }
                            else if (String.IsNullOrEmpty(model))
                            {
                                SiAuto.Main.LogMessage("No satisfactory name exists; using Undefined.");
                                model = "Undefined";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SiAuto.Main.LogWarning(ex.Message);
                        SiAuto.Main.LogException(ex);
                        model = "Unsupported (Possible VD)";
                    }

                    if (isWindows8)
                    {
                        String serialNumber = (drive["SerialNumber"] == null ? "Undefined" : drive["SerialNumber"].ToString());
                        pnpId = Utilities.Utility.BuildWindows8PnpDeviceName(model, serialNumber, (UInt16)drive["BusType"], ssDiskNumber);
                    }
                    else
                    {
                        if (drive["PNPDeviceID"] != null)
                        {
                            pnpId = drive["PNPDeviceID"].ToString().ToUpper();
                        }
                    }

                    if (ignoreVirtualDisks && KnownVirtualDisks.IsDiskOnVirtualDiskList(model))
                    {
                        // It's a virtual disk we're ignoring, so skip it (continue).
                        continue;
                    }

                    bool isIgnored = false;
                    byte[] smartData = Utilities.Utility.GetDiskStatusFromRegistry(drive, isWindows8, out isIgnored);
                    if (isIgnored)
                    {
                        continue;
                    }

                    // Byte 363 of the SMART data holds the test status (7:4 - status code, 3:0 - percent remaining)
                    String testStatus = "0";
                    try
                    {
                        testStatus = Utilities.Utility.GetTestStatus(smartData[363]);
                    }
                    catch
                    {
                        // No test data, status or support information, so skip.
                        SiAuto.Main.LogWarning("Disk PNP " + pnpId + " did not have SMART byte 363.");
                        continue;
                    }

                    if (Utilities.Utility.GetNumberOfSupportedTests(smartData) == 0)
                    {
                        SiAuto.Main.LogWarning("Disk PNP " + pnpId + " did not report any valid tests.");
                        continue;
                    }

                    String deviceID = isWindows8 ? "\\\\.\\PHYSICALDRIVE" + drive["DeviceID"].ToString() : drive["DeviceID"].ToString();
                    decimal columnTag = 0;

                    columnTag = Utilities.Utility.GetDriveIdFromPath(deviceID);
                    if (columnTag == -1)
                    {
                        columnTag = 0;
                    }

                    SiAuto.Main.LogDecimal("columnTag", columnTag);
                    ListViewItem item = new ListViewItem(new String[] { deviceID });
                    item.SubItems.Add(model);
                    item.SubItems[0].Tag = columnTag;
                    if (pnpId.Contains("USB") || pnpId.Contains("1394"))
                    {
                        item.SubItems[1].Tag = true;
                    }
                    else
                    {
                        item.SubItems[1].Tag = false;
                    }
                    item.SubItems.Add(testStatus);
                    item.SubItems[2].Tag = smartData;
                    AddListViewItemToPhysicalDisks(item);
                }

                if (listViewPhysicalDisks.Items.Count > 0)
                {
                    listViewPhysicalDisks.Items[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                QMessageBox.Show(ex.Message, "Severe", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            buttonRefresh.Enabled = true;
        }

        private void AddListViewItemToPhysicalDisks(ListViewItem lvi)
        {
            listViewPhysicalDisks.Items.Add(lvi);
        }

        private void listViewPhysicalDisks_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listViewPhysicalDisks.SuspendLayout();
            ((ListViewColumnSorter)this.listViewPhysicalDisks.ListViewItemSorter).SortColumn = e.Column;

            listViewPhysicalDisks.Sort();
            listViewPhysicalDisks.ResumeLayout();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RunDiskRefresh();
        }

        private void RunDiskRefresh()
        {
            buttonRefresh.Enabled = false;
            buttonShort.Enabled = false;
            buttonExtended.Enabled = false;
            buttonConveyance.Enabled = false;
            buttonCancel.Enabled = false;
            listViewPhysicalDisks.Items.Clear();
            if (isWorkerThreadRunning)
            {
                return;
            }

            refreshThread = new Thread(new ThreadStart(ManualRefresh));
            refreshThread.Name = "Manual Refresh Thread";
            refreshThread.Start();
        }

        private void buttonShort_Click(object sender, EventArgs e)
        {
            RunSelfTest(1);
        }

        private void buttonExtended_Click(object sender, EventArgs e)
        {
            RunSelfTest(2);
        }

        private void buttonConveyance_Click(object sender, EventArgs e)
        {
            RunSelfTest(3);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            RunSelfTest(127);
        }

        private void RunSelfTest(int selfTestCode)
        {
            if (listViewPhysicalDisks.SelectedItems != null && listViewPhysicalDisks.SelectedItems.Count != 0)
            {
                int physicalDiskId = Utilities.Utility.GetDriveIdFromPath(listViewPhysicalDisks.SelectedItems[0].SubItems[0].Text);
                if (physicalDiskId == -1)
                {
                    QMessageBox.Show("Unable to determine the physical drive ID. Please refresh the disks and try again.",
                        "Run Self-Test", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                byte[] smartData = (byte[])listViewPhysicalDisks.SelectedItems[0].SubItems[2].Tag;

                bool isUsb = false;
                try
                {
                    isUsb = (bool)listViewPhysicalDisks.SelectedItems[0].SubItems[1].Tag;
                }
                catch
                {
                    isUsb = false;
                }

                switch (selfTestCode)
                {
                    case 1:
                        {
                            // Short self-test
                            int selfTestLength = smartData[372];
                            if (QMessageBox.Show("The drive manufacturer indicates the Short Self-Test on this drive will " +
                                "take approximately " + selfTestLength.ToString() + " minutes to complete. Do you want to " +
                                "start a Short Self-Test?", "Start Short Self-Test", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                == DialogResult.Yes)
                            {
                                UInt32 result = isUsb ? InteropSmart.TestUsbDrive(physicalDiskId, 1) :
                                    InteropSmart.TestInternalDrive(physicalDiskId, 1);
                                if (result != 0x0)
                                {
                                    QMessageBox.Show("The test request command was sent to the drive. However, the Windows device " +
                                        "I/O controller returned the error code 0x" + Utilities.Utility.NormalizeBinaryFlags(
                                        Convert.ToString(result, 16)) + ".\n\nNot all disks or controllers support all tests. The " +
                                        "test may not have started successfully. You can refresh the disks to see if the test is " +
                                        "actually running.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                                RunDiskRefresh();
                            }
                            break;
                        }
                    case 2:
                        {
                            // Extended self-test
                            int selfTestLength = smartData[373];
                            if (QMessageBox.Show("The drive manufacturer indicates the Extended Self-Test on this drive will " +
                                "take approximately " + selfTestLength.ToString() + " minutes to complete. Do you want to " +
                                "start an Extended Self-Test?", "Start Extended Self-Test", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                == DialogResult.Yes)
                            {
                                UInt32 result = isUsb ? InteropSmart.TestUsbDrive(physicalDiskId, 2) :
                                    InteropSmart.TestInternalDrive(physicalDiskId, 2);
                                if (result != 0x0)
                                {
                                    QMessageBox.Show("The test request command was sent to the drive. However, the Windows device " +
                                        "I/O controller returned the error code 0x" + Utilities.Utility.NormalizeBinaryFlags(
                                        Convert.ToString(result, 16)) + ".\n\nNot all disks or controllers support all tests. The " +
                                        "test may not have started successfully. You can refresh the disks to see if the test is " +
                                        "actually running.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                                RunDiskRefresh();
                            }
                            break;
                        }
                    case 3:
                        {
                            // Conveyance self-test
                            int selfTestLength = smartData[374];
                            if (QMessageBox.Show("The drive manufacturer indicates the Conveyance Self-Test on this drive will " +
                                "take approximately " + selfTestLength.ToString() + " minutes to complete. Do you want to " +
                                "start a Conveyance Self-Test?", "Start Conveyance Self-Test", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                == DialogResult.Yes)
                            {
                                UInt32 result = isUsb ? InteropSmart.TestUsbDrive(physicalDiskId, 3) :
                                    InteropSmart.TestInternalDrive(physicalDiskId, 3);
                                if (result != 0x0)
                                {
                                    QMessageBox.Show("The test request command was sent to the drive. However, the Windows device " +
                                        "I/O controller returned the error code 0x" + Utilities.Utility.NormalizeBinaryFlags(
                                        Convert.ToString(result, 16)) + ".\n\nNot all disks or controllers support all tests. The " +
                                        "test may not have started successfully. You can refresh the disks to see if the test is " +
                                        "actually running.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                                RunDiskRefresh();
                            }
                            break;
                        }
                    case 127:
                        {
                            // Abort the test
                            if (QMessageBox.Show("Do you want to abort the currently running test?",
                                "Abort Self-Test", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                == DialogResult.Yes)
                            {
                                UInt32 result = isUsb ? InteropSmart.TestUsbDrive(physicalDiskId, 127) :
                                    InteropSmart.TestInternalDrive(physicalDiskId, 127);
                                if (result != 0x0)
                                {
                                    QMessageBox.Show("The test abort command was sent to the drive. However, the Windows device " +
                                        "I/O controller returned the error code 0x" + Utilities.Utility.NormalizeBinaryFlags(
                                        Convert.ToString(result, 16)) + ".\n\nNot all disks or controllers support all tests. The " +
                                        "test may not have started successfully. You can refresh the disks to see if the test is " +
                                        "actually stopped.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                                RunDiskRefresh();
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool IsTestInProgress(int smartTestStatusByte)
        {
            String statusString = Utilities.Utility.NormalizeBinaryFlags(Convert.ToString(smartTestStatusByte, 2));
            String selftestStatus = statusString.Substring(0, 4);
            String intRemaining = statusString.Substring(4);

            int statusCode = Convert.ToInt32(selftestStatus, 2);
            int remaining = Convert.ToInt32(intRemaining, 2);

            if (statusCode == 15)
            {
                // Test in progress; return true.
                return true;
            }
            return false;
        }

        private void listViewPhysicalDisks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewPhysicalDisks.SelectedItems != null && listViewPhysicalDisks.SelectedItems.Count != 0)
            {
                int physicalDiskId = Utilities.Utility.GetDriveIdFromPath(listViewPhysicalDisks.SelectedItems[0].SubItems[0].Text);
                if (physicalDiskId == -1)
                {
                    QMessageBox.Show("Unable to determine the physical drive ID. Please refresh the disks and try again.",
                        "Run Self-Test", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                byte[] smartData = (byte[])listViewPhysicalDisks.SelectedItems[0].SubItems[2].Tag;

                // Is test in progress? If so, enable cancel and dim all of the start buttons.
                try
                {
                    if (IsTestInProgress(smartData[363]))
                    {
                        buttonShort.Enabled = false;
                        buttonExtended.Enabled = false;
                        buttonConveyance.Enabled = false;
                        buttonCancel.Enabled = true;
                        return;
                    }
                }
                catch
                {
                    buttonShort.Enabled = false;
                    buttonExtended.Enabled = false;
                    buttonConveyance.Enabled = false;
                    buttonCancel.Enabled = false;
                }

                // Is short supported?
                try
                {
                    int testLength = smartData[372];
                    if (testLength > 0)
                    {
                        buttonShort.Enabled = true;
                    }
                    else
                    {
                        buttonShort.Enabled = false;
                    }
                }
                catch
                {
                    buttonShort.Enabled = false;
                }

                // Is extended supported?
                try
                {
                    int testLength = smartData[373];
                    if (testLength > 0)
                    {
                        buttonExtended.Enabled = true;
                    }
                    else
                    {
                        buttonExtended.Enabled = false;
                    }
                }
                catch
                {
                    buttonExtended.Enabled = false;
                }

                // Is conveyance supported?
                try
                {
                    int testLength = smartData[374];
                    if (testLength > 0)
                    {
                        buttonConveyance.Enabled = true;
                    }
                    else
                    {
                        buttonConveyance.Enabled = false;
                    }
                }
                catch
                {
                    buttonConveyance.Enabled = false;
                }
            }
            else
            {
                buttonShort.Enabled = false;
                buttonExtended.Enabled = false;
                buttonConveyance.Enabled = false;
                buttonCancel.Enabled = false;
            }
        }
    }
}
