using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Management;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using WSSControls.BelovedComponents;
using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker;
using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls;
using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI
{
    public partial class BitLockerControl : UserControl
    {
        #region Private Member Variables
        private List<HardDisk> listOfDisks;
        private DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls.ListViewColumnSorter lvwColumnSorter;
        private String oldStatus = String.Empty;
        private String newStatus = String.Empty;
        private bool isSystemVolumeEncrypted;

        // Registry
        Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
        Microsoft.Win32.RegistryKey dojoNorthSubKey;
        Microsoft.Win32.RegistryKey configurationKey;

        // Worker threads to perform encrypt, decrypt, pause, resume
        // and key management operations. This allows the UI to remain
        // as responsive as possible.
        private System.Threading.Thread encryptorThread;
        private System.Threading.Thread decryptorThread;
        private System.Threading.Thread lockUnlockThread;
        private System.Threading.Thread pauseResumeThread;
        private System.Threading.Thread keyManagementThread;
        private bool encryptorRunning;
        private bool decryptorRunning;
        private bool lockUnlockRunning;
        private bool pauseResumeRunning;
        private bool keyManagementRunning;
        private bool userWarnedLowSpace;
        private bool isWindows7Family;
        private bool isFipsComplianceMandatory;
        private bool terminateOnLoad;
        private bool isManualRefreshInProgress;

        // BitLocker configuration
        private bool bitLockerShowAllDrives;
        private bool bitLockerPreventLocking;

        // ImageList for disk statuses.
        private ImageList diskHealthIndicators;

        // Constants
        private const String INVALID_SELECTION = "Invalid selection (view was refreshed); please make a new selection.";

        // Timers
        private System.Threading.Timer timerBackground;
        private System.Threading.Timer refreshListView;
        private System.Threading.TimerCallback timerCallback;
        private System.Threading.TimerCallback refreshListViewCallback;

        // Skinning
        private bool useDefaultSkinning;
        private int windowBackground; // 0 = Metal Grate, 1 = Lightning, 2 = Cracked Glass, 3 = None
        private int oldBackground = 0;

        // Delegates for UI updates.
        private delegate void UpdateUIDelegate();
        private delegate void RefreshReloadDelegate();
        private delegate void DisplayMessageBoxDelegate(String message, String caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, bool displayMessage, bool useParent);
        private delegate void DisplayRefreshDelegate();
        private delegate void RefreshListViewDelegate();
        private delegate void EpicFailDelegate(Exception ex, String message, String resolution);
        private delegate void DisplayResultDelegate1(String results);
        private delegate void DisplayResultDelegate2(String results, String titleText);
        private delegate void DisplayResultDelegate3(String results, String path, String driveLetter, bool saveNow);
        private UpdateUIDelegate dlgUpdateUI;
        private RefreshReloadDelegate dlgRefreshReloadDisks;
        private DisplayMessageBoxDelegate dlgDisplayMessage;
        private DisplayRefreshDelegate dlgDisplayRefresh;
        private RefreshListViewDelegate dlgRefreshListView;
        private EpicFailDelegate dlgEpicFail;
        private DisplayResultDelegate1 dlgDisplayResult1;
        private DisplayResultDelegate2 dlgDisplayResult2;
        private DisplayResultDelegate3 dlgDisplayResult3;

        // Constants for items that won't work as resources (due to use in switch statements).
        private const String ENCRYPTION_STATUS_FULLY_DECRYPTED = "Fully Decrypted";
        private const String ENCRYPTION_STATUS_FULLY_ENCRYPTED = "Fully Encrypted";
        private const String ENCRYPTION_STATUS_ENCRYPTING = "Encrypting";
        private const String ENCRYPTION_STATUS_DECRYPTING = "Decrypting";
        private const String ENCRYPTION_STATUS_ENCRYPTION_PAUSED = "Encryption Paused";
        private const String ENCRYPTION_STATUS_DECRYPTION_PAUSED = "Decryption Paused";
        private const String ENCRYPTION_STATUS_LOCKED = "Locked";
        private const String ENCRYPTION_STATUS_NOT_ENCRYPTABLE = "Not Encryptable";
        private const String ENCRYPTION_STATUS_UNKNOWN = "Unknown";

        private const String ENCRYPTION_METHOD_AES128_DIFFUSER = "AES 128/Diffuser";
        private const String ENCRYPTION_METHOD_AES256_DIFFUSER = "AES 256/Diffuser";
        private const String ENCRYPTION_METHOD_AES256 = "AES 256";
        private const String ENCRYPTION_METHOD_AES128 = "AES 128";
        private const String ENCRYPTION_METHOD_UNKNOWN = "Unknown";
        private const String ENCRYPTION_METHOD_NOT_APPLICABLE = "N/A";
        #endregion

        #region Constructor
        public BitLockerControl()
        {
            SiAuto.Main.EnterMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLockerControl (constructor)");
            InitializeComponent();
            terminateOnLoad = false;

            isSystemVolumeEncrypted = false;
            isManualRefreshInProgress = true;

            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            SiAuto.Main.LogMessage("Creating the ListViewColumSorter and assigning it to the ListView.");
            lvwColumnSorter = new DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls.ListViewColumnSorter();
            this.lvEncryptableVolumes.ListViewItemSorter = lvwColumnSorter;

            //listOfDisks = new ArrayList();
            SiAuto.Main.LogMessage("Creating the list of disks.");
            listOfDisks = new List<HardDisk>();

            SiAuto.Main.LogMessage("Setting worker threads to null and run state to false.");
            // Set threads to null and running state of false.
            encryptorThread = null;
            decryptorThread = null;
            lockUnlockThread = null;
            pauseResumeThread = null;
            keyManagementThread = null;
            encryptorRunning = false;
            decryptorRunning = false;
            lockUnlockRunning = false;
            pauseResumeRunning = false;
            keyManagementRunning = false;
            userWarnedLowSpace = false;
            isWindows7Family = true;
            // This statement ensures the GPConfig static class is initialized.
            isFipsComplianceMandatory = GPConfig.IsFipsComplianceMandatory;

            SiAuto.Main.LogMessage("Setting images used in ListView.");
            // Disk statuses.
            diskHealthIndicators = new ImageList();
            // Space indicators
            diskHealthIndicators.Images.Add("SpaceHealthy", Properties.Resources.Healthy16);
            diskHealthIndicators.Images.Add("SpaceLessThan25", Properties.Resources.Warning16);
            diskHealthIndicators.Images.Add("SpaceLow", Properties.Resources.Critical16);
            // Encryption indicators
            diskHealthIndicators.Images.Add("Encrypted", Properties.Resources.ShieldGreen16);
            diskHealthIndicators.Images.Add(ENCRYPTION_STATUS_ENCRYPTING, Properties.Resources.ShieldYellow16);
            diskHealthIndicators.Images.Add("Decrypted", Properties.Resources.ShieldRed16);
            lvEncryptableVolumes.SmallImageList = diskHealthIndicators;

            // Create delegates.
            SiAuto.Main.LogMessage("Creating the delegates.");
            dlgUpdateUI = new UpdateUIDelegate(DUpdateStatus);
            dlgRefreshReloadDisks = new RefreshReloadDelegate(DRefreshReloadDisks);
            dlgDisplayMessage = new DisplayMessageBoxDelegate(DDisplayMessage);
            dlgDisplayRefresh = new DisplayRefreshDelegate(DDisplayRefresh);
            dlgRefreshListView = new RefreshListViewDelegate(DTimerRefreshListView);
            dlgEpicFail = new EpicFailDelegate(DEpicFail);
            dlgDisplayResult1 = new DisplayResultDelegate1(DDisplayResultWindow);
            dlgDisplayResult2 = new DisplayResultDelegate2(DDisplayResultWindow);
            dlgDisplayResult3 = new DisplayResultDelegate3(DDisplayResultWindow);

            SiAuto.Main.LogMessage("Refreshing the list of disks installed in the Server.");
            RefreshDisks();

            oldStatus = String.Empty;
            SiAuto.Main.LogMessage("Setting the welcome message.");
            newStatus = "Welcome to BitLocker Manager for Home Server SMART 24/7!";

            // Initialize timers.
            SiAuto.Main.LogMessage("Initializing the timers.");
            timerCallback = new System.Threading.TimerCallback(TimerUpdateStatus);
            refreshListViewCallback = new System.Threading.TimerCallback(RefreshListView);
            timerBackground = new System.Threading.Timer(timerCallback, null, 5000, 500);
            refreshListView = new System.Threading.Timer(refreshListViewCallback, null, 6000, 15000);

            // Set up the skinning
            // Try connecting to the Registry.
            try
            {
                SiAuto.Main.LogMessage("Loading configuration from the Registry.");
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;
                Microsoft.Win32.RegistryKey configurationKey;
                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);
                configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, false);
                if (dojoNorthSubKey == null || configurationKey == null)
                {
                    SiAuto.Main.LogWarning("Configuration key(s) are null; cannot continue. Returning.");
                    configurationKey.Close();
                    dojoNorthSubKey.Close();
                    windowBackground = 0;
                    SiAuto.Main.LeaveMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLockerControl (constructor)");
                    return;
                }

                try
                {
                    SiAuto.Main.LogMessage("Getting custom background.");
                    windowBackground = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigWindowBackground);
                }
                catch
                {
                    SiAuto.Main.LogWarning("Exception getting custom background; using default of zero.");
                    windowBackground = 0;
                }
                SiAuto.Main.LogMessage("Closing Registry keys.");
                configurationKey.Close();
                dojoNorthSubKey.Close();
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogFatal("Exceptions were detected setting up the BitLocker control.");
                SiAuto.Main.LogException(ex);
                windowBackground = 0;
            }
            SiAuto.Main.LeaveMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLockerControl (constructor)");
        }
        #endregion

        /// <summary>
        /// Runs when the form is first loaded. Populate the ListView with disks so the user may select them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BitLockerControl_Load(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLockerControl_Load");
            isManualRefreshInProgress = true;
            SiAuto.Main.LogMessage("Load list of disks.");
            LoadList();
            isManualRefreshInProgress = false;

            SiAuto.Main.LogMessage("Check for encryption test.");
            CheckForEncryptionTest();

            SiAuto.Main.LogBool("GPConfig.IsFipsComplianceMandatory", GPConfig.IsFipsComplianceMandatory);
            if (GPConfig.IsFipsComplianceMandatory)
            {
                bool conflictVistaDetected = false;
                bool conflictWin7Detected = false;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                System.Text.StringBuilder sbVista = new System.Text.StringBuilder();
                System.Text.StringBuilder sbWin7 = new System.Text.StringBuilder();
                sb.Append("Home Server SMART detected one or more Group Policy conflicts. These policy conflicts " +
                    "can (or will) prevent some BitLocker functions from working correctly.\n\nFederal Information Protection " +
                    "Standard (FIPS) Compliance is enforced on this computer, but one or more BitLocker policy settings is " +
                    "enforcing the use of certain features that FIPS does not allow, hence the conflict.\n\n");

                // Vista conflicts
                SiAuto.Main.LogMessage("Checking for Vista FIPS compliance policy conflicts.");
                sbVista.Append("Conflicing settings that affect Windows Vista/Server 2008:\n");
                if (GPConfig.VistaRequireNumericPw)
                {
                    conflictVistaDetected = true;
                    sbVista.Append(" - Numeric Passwords are enforced; FIPS disallows them\n\n");
                }
                if (GPConfig.VistaRequireKeyEscrow == "Required")
                {
                    conflictVistaDetected = true;
                    sbVista.Append(" - AD Key Escrow is enforced; FIPS disallows numeric passwords\n\n");
                }
                if (GPConfig.VistaAllAllowPassword == "Forbidden" &&
                    GPConfig.VistaAllAllowBek == "Forbidden" && GPConfig.VistaRequireKeyEscrow != "Required")
                {
                    conflictVistaDetected = true;
                    sbVista.Append(" - Recovery options disabled; no recovery key gen possible\n\n");
                }
                if (!conflictVistaDetected)
                {
                    sbVista.Append(" - No FIPS conflicting settings were detected\n\n");
                }

                // Windows 7 conflicts
                SiAuto.Main.LogMessage("Checking for Windows 7 FIPS compliance policy conflicts.");
                sbWin7.Append("Conflicing settings that affect Windows 7/Server 2008 R2:\n");
                if (GPConfig.Win7OsRequireNumericPw)
                {
                    conflictWin7Detected = true;
                    sbWin7.Append(" - Numeric Passwords enforced on OS volumes; FIPS disallows them\n\n");
                }
                if (GPConfig.Win7FdvRequireNumericPw)
                {
                    conflictWin7Detected = true;
                    sbWin7.Append(" - Numeric Passwords enforced on FDV; FIPS disallows them\n\n");
                }
                if (GPConfig.Win7RdvRequireNumericPw)
                {
                    conflictWin7Detected = true;
                    sbWin7.Append(" - Numeric Passwords enforced on RDV; FIPS disallows them\n\n");
                }
                if (GPConfig.Win7FdvRequirePassphrase)
                {
                    conflictWin7Detected = true;
                    sbWin7.Append(" - Passphrases enforced on FDV; FIPS disallows them\n\n");
                }
                if (GPConfig.Win7FdvRequirePassphrase)
                {
                    conflictWin7Detected = true;
                    sbWin7.Append(" - Passphrases enforced on RDV; FIPS disallows them\n\n");
                }
                if (GPConfig.OsRequireActiveDirectoryBackup == "Required")
                {
                    conflictWin7Detected = true;
                    sbWin7.Append(" - AD Key Escrow enforced on OS volume; FIPS disallows numeric passwords\n\n");
                }
                if (GPConfig.FdvRequireActiveDirectoryBackup == "Required")
                {
                    conflictWin7Detected = true;
                    sbWin7.Append(" - AD Key Escrow enforced on FDV; FIPS disallows numeric passwords\n\n");
                }
                if (GPConfig.RdvRequireActiveDirectoryBackup == "Required")
                {
                    conflictWin7Detected = true;
                    sbWin7.Append(" - AD Key Escrow enforced on RDV; FIPS disallows numeric passwords\n\n");
                }
                if (!conflictWin7Detected)
                {
                    sbWin7.Append(" - No FIPS conflicting settings were detected\n\n");
                }

                if (conflictVistaDetected || conflictWin7Detected)
                {
                    QMessageBox.Show(sb.ToString() + sbVista.ToString() + sbWin7.ToString() +
                        "Please contact your system administrator for assistance.", "FIPS Compliance Conflicts Detected",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            if (lvEncryptableVolumes.Items.Count > 0)
            {
                SiAuto.Main.LogMessage("Suspending ListView layout.");
                lvEncryptableVolumes.SuspendLayout();
                try
                {
                    SiAuto.Main.LogMessage("Setting sort order to ascending.");
                    ((DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls.ListViewColumnSorter)this.lvEncryptableVolumes.ListViewItemSorter).SortColumn = 0;
                    ((DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls.ListViewColumnSorter)this.lvEncryptableVolumes.ListViewItemSorter).Order = SortOrder.Ascending;
                    lvEncryptableVolumes.Sorting = SortOrder.Ascending;
                    lvEncryptableVolumes.Sort();
                }
                catch(Exception ex)
                {
                    SiAuto.Main.LogWarning("Failed to set ListView sort order: " + ex.Message);
                }
                SiAuto.Main.LogMessage("Resuming ListView layout.");
                lvEncryptableVolumes.ResumeLayout();
                lvEncryptableVolumes.Items[0].Selected = true;
            }

            // Drive Bender ~ Dan Garland ~
            if (UtilityMethods.IsDriveBenderInstalled())
            {
                SiAuto.Main.LogMessage("Drive Bender/StableBit is installed; setting pool options.");
                menuDriveBender.Visible = true;
                menuDriveBender.Enabled = true;
            }

            // Window Background
            SiAuto.Main.LogMessage("Setting the window background.");
            SetWindowBackground();
            SiAuto.Main.LeaveMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLockerControl_Load");
        }

        #region ListView Management Methods
        /// <summary>
        /// Iterates through the list of known hard disks, fetches their information and loads it into the ListView for
        /// the user to browse and select.
        /// </summary>
        private void LoadList()
        {
            SiAuto.Main.EnterMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.LoadList");
            bool lowDiskDetected = false;
            foreach (ListViewItem lvi in lvEncryptableVolumes.Items)
            {
                lvi.SubItems[1].Tag = ListViewUpdateState.Filthy;
            }

            foreach (HardDisk hd in listOfDisks)
            {
                decimal freePercent = 0.00M;
                try
                {
                    freePercent = hd.FreeSpace / hd.Capacity;
                }
                catch (DivideByZeroException)
                {
                    freePercent = 1.00M;
                }

                FancyListView.ImageSubItem capacityItem = new FancyListView.ImageSubItem();
                if (freePercent < 0.10M)
                {
                    capacityItem.ImageKey = "SpaceLow";
                }
                else if (freePercent < 0.25M)
                {
                    capacityItem.ImageKey = "SpaceLessThan25";
                }
                else
                {
                    capacityItem.ImageKey = "SpaceHealthy";
                }
                capacityItem.Text = hd.Capacity.ToString() + "/" + hd.FreeSpace.ToString();

                FancyListView.ImageSubItem encryptionItem = new FancyListView.ImageSubItem();
                if (hd.EncryptionStatus == ENCRYPTION_STATUS_FULLY_ENCRYPTED)
                {
                    // Secure (unless protectors are suspended)
                    if (hd.ProtectionStatus == "Suspended")
                    {
                        encryptionItem.ImageKey = "Decrypted";
                    }
                    else
                    {
                        encryptionItem.ImageKey = "Encrypted";
                    }
                }
                else if (hd.EncryptionStatus == ENCRYPTION_STATUS_ENCRYPTING || hd.EncryptionStatus == ENCRYPTION_STATUS_ENCRYPTION_PAUSED)
                {
                    // Partially secure
                    encryptionItem.ImageKey = ENCRYPTION_STATUS_ENCRYPTING;
                }
                else if (hd.EncryptionStatus == ENCRYPTION_STATUS_NOT_ENCRYPTABLE)
                {
                    // Cannot be managed with BitLocker - display yellow shield.
                    encryptionItem.ImageKey = ENCRYPTION_STATUS_ENCRYPTING;
                }
                else
                {
                    // Unsecure
                    encryptionItem.ImageKey = "Decrypted";
                }
                encryptionItem.Text = hd.EncryptionStatus;

                int listViewItemIndex = GetVolumeIndex(hd.DeviceID);
                if (listViewItemIndex == -1)
                {
                    // Item doesn't exist, so compose new.
                    ListViewItem lvi = new ListViewItem(new string[] { hd.DriveLetter });
                    lvi.SubItems.Add(hd.VolumeLabel);
                    lvi.SubItems.Add(hd.FileSystem);
                    lvi.SubItems.Add(capacityItem);
                    lvi.SubItems.Add(encryptionItem);
                    lvi.SubItems.Add(hd.LockStatus);
                    lvi.SubItems.Add(hd.ProtectionStatus);
                    lvi.SubItems.Add(hd.EncryptionPercentage);
                    lvi.SubItems.Add(hd.EncryptionMethod);
                    lvi.SubItems.Add(hd.IsAutoUnlockEnabled ? "Enabled" : "Disabled");
                    lvi.SubItems.Add(hd.DeviceID);
                    lvi.SubItems.Add(hd.VolumeType);
                    lvi.SubItems[1].Tag = ListViewUpdateState.Clean;
                    lvEncryptableVolumes.Items.Add(lvi);
                }
                else
                {
                    ListViewItem item = lvEncryptableVolumes.Items[listViewItemIndex];
                    FancyListView.ImageSubItem capacitySubItem = (FancyListView.ImageSubItem)item.SubItems[3];
                    FancyListView.ImageSubItem encryptionSubItem = (FancyListView.ImageSubItem)item.SubItems[4];
                    if (String.Compare(item.SubItems[0].Text, hd.DriveLetter, true) != 0)
                    {
                        item.SubItems[0].Text = hd.DriveLetter;
                    }
                    if (String.Compare(item.SubItems[1].Text, hd.VolumeLabel, true) != 0)
                    {
                        item.SubItems[1].Text = hd.VolumeLabel;
                    }
                    if (String.Compare(item.SubItems[2].Text, hd.FileSystem, true) != 0)
                    {
                        item.SubItems[2].Text = hd.FileSystem;
                    }
                    if (String.Compare(capacitySubItem.Text, capacityItem.Text, true) != 0)
                    {
                        capacitySubItem.Text = capacityItem.Text;
                    }
                    if (String.Compare(capacitySubItem.ImageKey, capacityItem.ImageKey, true) != 0)
                    {
                        capacitySubItem.ImageKey = capacityItem.ImageKey;
                    }
                    if (String.Compare(encryptionSubItem.Text, encryptionItem.Text, true) != 0)
                    {
                        encryptionSubItem.Text = encryptionItem.Text;
                    }
                    if (String.Compare(encryptionSubItem.ImageKey, encryptionItem.ImageKey, true) != 0)
                    {
                        encryptionSubItem.ImageKey = encryptionItem.ImageKey;
                    }
                    if (String.Compare(item.SubItems[5].Text, hd.LockStatus, true) != 0)
                    {
                        item.SubItems[5].Text = hd.LockStatus;
                    }
                    if (String.Compare(item.SubItems[6].Text, hd.ProtectionStatus, true) != 0)
                    {
                        item.SubItems[6].Text = hd.ProtectionStatus;
                    }
                    if (String.Compare(item.SubItems[7].Text, hd.EncryptionPercentage, true) != 0)
                    {
                        item.SubItems[7].Text = hd.EncryptionPercentage;
                    }
                    if (String.Compare(item.SubItems[8].Text, hd.EncryptionMethod, true) != 0)
                    {
                        item.SubItems[8].Text = hd.EncryptionMethod;
                    }
                    if (String.Compare(item.SubItems[9].Text, hd.IsAutoUnlockEnabled ? "Enabled" : "Disabled", true) != 0)
                    {
                        item.SubItems[9].Text = hd.IsAutoUnlockEnabled ? "Enabled" : "Disabled";
                    }
                    if (String.Compare(item.SubItems[10].Text, hd.DeviceID, true) != 0)
                    {
                        item.SubItems[10].Text = hd.DeviceID;
                    }
                    if (String.Compare(item.SubItems[11].Text, hd.VolumeType, true) != 0)
                    {
                        item.SubItems[11].Text = hd.VolumeType;
                    }
                    item.SubItems[1].Tag = ListViewUpdateState.Clean;
                }

                if (freePercent < 0.10M)
                {
                    lowDiskDetected = true;
                }
            }

            if (lowDiskDetected && !userWarnedLowSpace)
            {
                userWarnedLowSpace = true;
                //QMessageBox.Show("At least one of your volumes is low on disk space (less than 10% free).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // Remove the Filthy items.
            List<ListViewItem> tempList = new List<ListViewItem>();
            foreach (ListViewItem lvi in lvEncryptableVolumes.Items)
            {
                if ((ListViewUpdateState)lvi.SubItems[1].Tag == ListViewUpdateState.Filthy)
                {
                    tempList.Add(lvi);
                }
            }

            foreach (ListViewItem lvi in tempList)
            {
                lvEncryptableVolumes.Items.Remove(lvi);
            }
            SiAuto.Main.LeaveMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.LoadList");
        }

        private int GetVolumeIndex(String deviceID)
        {
            int itemCount = lvEncryptableVolumes.Items.Count;
            if (itemCount == 0)
            {
                return -1;
            }

            for (int index = 0; index < itemCount; index++)
            {
                ListViewItem item = lvEncryptableVolumes.Items[index];
                if (String.Compare(deviceID, item.SubItems[10].Text, true) == 0)
                {
                    return index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Event that fires when the user clicks to sort information in the ListView. Allows the user to sort by
        /// any column, ascending or descending.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvEncryptableVolumes_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.lvEncryptableVolumes.Sort();
        }

        /// <summary>
        /// Event that fires when the user changes the selected item in the ListView. Ensures that appropriate options
        /// are always available (i.e. make "encrypt" available for a decrypted disk but don't make "decrypt" available
        /// for a decrypted disk).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvEncryptableVolumes_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUiFromSelectedIndexChange();
        }

        private void UpdateUiFromSelectedIndexChange()
        {
            // We can enable TPM changes, if a TPM is available.
            if (GPConfig.IsTpmUsable)
            {
                //repairTPMKeyToolStripMenuItem.Enabled = true;
            }
            else
            {
                //repairTPMKeyToolStripMenuItem.Enabled = false;
            }

            if (lvEncryptableVolumes.SelectedItems != null)
            {
                ListViewItem lvi = null;

                try
                {
                    // Determine what is selected.
                    lvi = lvEncryptableVolumes.SelectedItems[0];
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Nothing selected or invalid selection - make all options dimmed.
                    buttonEncrypt.Enabled = false;
                    buttonDecrypt.Enabled = false;
                    buttonLock.Enabled = false;
                    buttonUnlock.Enabled = false;
                    buttonProtectors.Enabled = false;
                    buttonAdvanced.Enabled = true;
                    buttonOptions.Enabled = true;
                    return;
                }

                // A valid volume was selected, so let's get the info about it.
                HardDisk disk = GetDiskInfo(lvi.SubItems[10].Text);

                // We can enable rename.

                /* NTFS disks (including flash drives) can be encrypted in both Windows Vista family and Windows 7 family.
                 * Windows 7 family also supports FAT, FAT32 and exFAT.
                 * Now let's decide what options to make
                 * available based on file system (and/or encryption status). A disk that reports "Locked by BitLocker"
                 * is an NTFS volume, even though the GetDiskInfo method doesn't indicate this. Since only NTFS volumes
                 * can be encrypted, if the volume is locked by BitLocker, the file system MUST be NTFS.
                 * 
                 * NOTE:  FIPS compliance, if enabled, takes precedence over all Group Policy settings where FIPS
                 * compliance has an impact. The use of numeric passwords and complex passwords (passphrases) are not
                 * allowed when FIPS compliance is enforced, for example.
                */
                switch (disk.EncryptionStatus)
                {
                    case ENCRYPTION_STATUS_FULLY_DECRYPTED:
                        {
                            // Make options appropriate to a decrypted volume available (i.e. you can encrypt it,
                            // but not decrypt it).
                            buttonEncrypt.Enabled = true;
                            buttonDecrypt.Enabled = false;
                            buttonLock.Enabled = false;
                            buttonUnlock.Enabled = false;
                            buttonProtectors.Enabled = true;
                            buttonAdvanced.Enabled = true;
                            encryptionProgress.Value = 0;
                            volumeStatus.Text = "Volume is fully decrypted. Click Encrypt to encrypt it!";
                            menuAutoUnlock.Enabled = false;
                            menuAutoUnlockEnable.Enabled = false;
                            menuAutoUnlockDisable.Enabled = false;
                            menuBek.Enabled = true;
                            menuBekAdd.Enabled = true;
                            menuBekChange.Enabled = true;
                            menuBekDelete.Enabled = true;
                            menuNumPassword.Enabled = true;
                            menuNumPasswordAdd.Enabled = true;
                            menuNumPasswordDelete.Enabled = true;
                            menuSmartCards.Enabled = true;
                            menuSmartCardsAdd.Enabled = true;
                            menuSmartCardsDelete.Enabled = true;
                            menuPassword.Enabled = true;
                            menuPasswordAdd.Enabled = true;
                            menuPasswordChange.Enabled = true;
                            menuPasswordDelete.Enabled = true;
                            menuViewProtectors.Enabled = true;
                            menuEscrowKeysToAd.Enabled = true;
                            menuUpgrade.Enabled = false;
                            menuPause.Enabled = false;
                            menuResume.Enabled = false;
                            menuSuspend.Enabled = false;
                            menuSuspendOff.Enabled = false;
                            menuSuspendOn.Enabled = false;
                            break;
                        }
                    case ENCRYPTION_STATUS_DECRYPTING:
                        {
                            // Make options appropriate to a decrypting volume available (i.e. you can re-encrypt it,
                            // but not decrypt it; you can also pause).
                            buttonEncrypt.Enabled = true;
                            buttonDecrypt.Enabled = false;
                            buttonLock.Enabled = false;
                            buttonUnlock.Enabled = false;
                            buttonProtectors.Enabled = true;
                            buttonAdvanced.Enabled = true;
                            try
                            {
                                encryptionProgress.Value = Int32.Parse(disk.EncryptionPercentage);
                            }
                            catch
                            {
                                encryptionProgress.Value = 0;
                            }
                            volumeStatus.Text = "Volume is decrypting; " + (100 - encryptionProgress.Value) + "% done.";
                            menuAutoUnlock.Enabled = false;
                            menuAutoUnlockEnable.Enabled = false;
                            menuAutoUnlockDisable.Enabled = false;
                            menuBek.Enabled = true;
                            menuBekAdd.Enabled = true;
                            menuBekChange.Enabled = true;
                            menuBekDelete.Enabled = true;
                            menuNumPassword.Enabled = true;
                            menuNumPasswordAdd.Enabled = true;
                            menuNumPasswordDelete.Enabled = true;
                            menuSmartCards.Enabled = true;
                            menuSmartCardsAdd.Enabled = true;
                            menuSmartCardsDelete.Enabled = true;
                            menuPassword.Enabled = true;
                            menuPasswordAdd.Enabled = true;
                            menuPasswordChange.Enabled = true;
                            menuPasswordDelete.Enabled = true;
                            menuViewProtectors.Enabled = true;
                            menuEscrowKeysToAd.Enabled = false;
                            menuUpgrade.Enabled = false;
                            menuPause.Enabled = true;
                            menuResume.Enabled = false;
                            menuSuspend.Enabled = false;
                            menuSuspendOff.Enabled = false;
                            menuSuspendOn.Enabled = false;
                            break;
                        }
                    case ENCRYPTION_STATUS_FULLY_ENCRYPTED:
                        {
                            // Make options appropriate to an encrypted volume available (i.e. you can decrypt it,
                            // but not encrypt it).
                            buttonEncrypt.Enabled = false;
                            buttonDecrypt.Enabled = true;
                            buttonLock.Enabled = (bitLockerPreventLocking ? false : true);
                            buttonUnlock.Enabled = false;
                            buttonProtectors.Enabled = true;
                            buttonAdvanced.Enabled = true;
                            encryptionProgress.Value = 100;
                            volumeStatus.Text = "Volume is fully encrypted and secured.";
                            menuAutoUnlock.Enabled = true;
                            menuAutoUnlockEnable.Enabled = true;
                            menuAutoUnlockDisable.Enabled = true;
                            menuBek.Enabled = true;
                            menuBekAdd.Enabled = true;
                            menuBekChange.Enabled = true;
                            menuBekDelete.Enabled = true;
                            menuNumPassword.Enabled = true;
                            menuNumPasswordAdd.Enabled = true;
                            menuNumPasswordDelete.Enabled = true;
                            menuSmartCards.Enabled = true;
                            menuSmartCardsAdd.Enabled = true;
                            menuSmartCardsDelete.Enabled = true;
                            menuPassword.Enabled = true;
                            menuPasswordAdd.Enabled = true;
                            menuPasswordChange.Enabled = true;
                            menuPasswordDelete.Enabled = true;
                            menuViewProtectors.Enabled = true;
                            menuEscrowKeysToAd.Enabled = true;
                            menuUpgrade.Enabled = true;
                            menuPause.Enabled = false;
                            menuResume.Enabled = false;
                            menuSuspend.Enabled = true;
                            menuSuspendOff.Enabled = true;
                            menuSuspendOn.Enabled = true;
                            break;
                        }
                    case ENCRYPTION_STATUS_ENCRYPTING:
                        {
                            // Make options appropriate to a encrypting volume available (i.e. you can decrypt it,
                            // but not encrypt it).
                            buttonEncrypt.Enabled = false;
                            buttonDecrypt.Enabled = true;
                            buttonLock.Enabled = (bitLockerPreventLocking ? false : true);
                            buttonUnlock.Enabled = false;
                            buttonProtectors.Enabled = true;
                            buttonAdvanced.Enabled = true;
                            try
                            {
                                encryptionProgress.Value = Int32.Parse(disk.EncryptionPercentage);
                            }
                            catch
                            {
                                encryptionProgress.Value = 0;
                            }
                            volumeStatus.Text = "Volume is encrypting; " + encryptionProgress.Value + "% done.";
                            menuAutoUnlock.Enabled = true;
                            menuAutoUnlockEnable.Enabled = true;
                            menuAutoUnlockDisable.Enabled = true;
                            menuBek.Enabled = true;
                            menuBekAdd.Enabled = true;
                            menuBekChange.Enabled = true;
                            menuBekDelete.Enabled = true;
                            menuNumPassword.Enabled = true;
                            menuNumPasswordAdd.Enabled = true;
                            menuNumPasswordDelete.Enabled = true;
                            menuSmartCards.Enabled = true;
                            menuSmartCardsAdd.Enabled = true;
                            menuSmartCardsDelete.Enabled = true;
                            menuPassword.Enabled = true;
                            menuPasswordAdd.Enabled = true;
                            menuPasswordChange.Enabled = true;
                            menuPasswordDelete.Enabled = true;
                            menuViewProtectors.Enabled = true;
                            menuEscrowKeysToAd.Enabled = true;
                            menuUpgrade.Enabled = true;
                            menuPause.Enabled = true;
                            menuResume.Enabled = false;
                            menuSuspend.Enabled = true;
                            menuSuspendOff.Enabled = true;
                            menuSuspendOn.Enabled = true;
                            break;
                        }
                    case ENCRYPTION_STATUS_ENCRYPTION_PAUSED:
                        {
                            // Make options appropriate to an encryption paused volume available (i.e. you can resume it).
                            buttonEncrypt.Enabled = true;
                            buttonDecrypt.Enabled = true;
                            buttonLock.Enabled = (bitLockerPreventLocking ? false : true);
                            buttonUnlock.Enabled = false;
                            buttonProtectors.Enabled = true;
                            buttonAdvanced.Enabled = true;
                            try
                            {
                                encryptionProgress.Value = Int32.Parse(disk.EncryptionPercentage);
                            }
                            catch
                            {
                                encryptionProgress.Value = 0;
                            }
                            volumeStatus.Text = "Volume encryption has been paused; " + encryptionProgress.Value + "% done.";
                            menuAutoUnlock.Enabled = true;
                            menuAutoUnlockEnable.Enabled = true;
                            menuAutoUnlockDisable.Enabled = true;
                            menuBek.Enabled = true;
                            menuBekAdd.Enabled = true;
                            menuBekChange.Enabled = true;
                            menuBekDelete.Enabled = true;
                            menuNumPassword.Enabled = true;
                            menuNumPasswordAdd.Enabled = true;
                            menuNumPasswordDelete.Enabled = true;
                            menuSmartCards.Enabled = true;
                            menuSmartCardsAdd.Enabled = true;
                            menuSmartCardsDelete.Enabled = true;
                            menuPassword.Enabled = true;
                            menuPasswordAdd.Enabled = true;
                            menuPasswordChange.Enabled = true;
                            menuPasswordDelete.Enabled = true;
                            menuViewProtectors.Enabled = true;
                            menuEscrowKeysToAd.Enabled = true;
                            menuUpgrade.Enabled = false;
                            menuPause.Enabled = false;
                            menuResume.Enabled = true;
                            menuSuspend.Enabled = true;
                            menuSuspendOff.Enabled = true;
                            menuSuspendOn.Enabled = true;
                            break;
                        }
                    case ENCRYPTION_STATUS_DECRYPTION_PAUSED:
                        {
                            // Make options appropriate to a decryption paused volume available (i.e. you can resume it).
                            buttonEncrypt.Enabled = true;
                            buttonDecrypt.Enabled = true;
                            buttonLock.Enabled = (bitLockerPreventLocking ? false : true);
                            buttonUnlock.Enabled = false;
                            buttonProtectors.Enabled = true;
                            buttonAdvanced.Enabled = true;
                            try
                            {
                                encryptionProgress.Value = Int32.Parse(disk.EncryptionPercentage);
                            }
                            catch
                            {
                                encryptionProgress.Value = 0;
                            }
                            volumeStatus.Text = "Volume decryption has been paused; " + (100 - encryptionProgress.Value) + "% done.";
                            menuAutoUnlock.Enabled = false;
                            menuAutoUnlockEnable.Enabled = false;
                            menuAutoUnlockDisable.Enabled = false;
                            menuBek.Enabled = true;
                            menuBekAdd.Enabled = true;
                            menuBekChange.Enabled = true;
                            menuBekDelete.Enabled = true;
                            menuNumPassword.Enabled = true;
                            menuNumPasswordAdd.Enabled = true;
                            menuNumPasswordDelete.Enabled = true;
                            menuSmartCards.Enabled = true;
                            menuSmartCardsAdd.Enabled = true;
                            menuSmartCardsDelete.Enabled = true;
                            menuPassword.Enabled = true;
                            menuPasswordAdd.Enabled = true;
                            menuPasswordChange.Enabled = true;
                            menuPasswordDelete.Enabled = true;
                            menuViewProtectors.Enabled = true;
                            menuEscrowKeysToAd.Enabled = false;
                            menuUpgrade.Enabled = false;
                            menuPause.Enabled = false;
                            menuResume.Enabled = true;
                            menuSuspend.Enabled = false;
                            menuSuspendOff.Enabled = false;
                            menuSuspendOn.Enabled = false;
                            break;
                        }
                    case ENCRYPTION_STATUS_LOCKED:
                        {
                            // Make options appropriate to a locked volume available (i.e. you can't do much other than unlock it).
                            buttonEncrypt.Enabled = false;
                            buttonDecrypt.Enabled = false;
                            buttonLock.Enabled = false;
                            buttonUnlock.Enabled = true;
                            buttonProtectors.Enabled = true;
                            buttonAdvanced.Enabled = true;
                            encryptionProgress.Value = 100;
                            volumeStatus.Text = "Volume is currently locked; unlock it to see its details.";
                            menuAutoUnlock.Enabled = false;
                            menuAutoUnlockEnable.Enabled = false;
                            menuAutoUnlockDisable.Enabled = false;
                            menuBek.Enabled = false;
                            menuBekAdd.Enabled = false;
                            menuBekChange.Enabled = false;
                            menuBekDelete.Enabled = false;
                            menuNumPassword.Enabled = false;
                            menuNumPasswordAdd.Enabled = false;
                            menuNumPasswordDelete.Enabled = false;
                            menuSmartCards.Enabled = false;
                            menuSmartCardsAdd.Enabled = false;
                            menuSmartCardsDelete.Enabled = false;
                            menuPassword.Enabled = false;
                            menuPasswordAdd.Enabled = false;
                            menuPasswordChange.Enabled = false;
                            menuPasswordDelete.Enabled = false;
                            menuViewProtectors.Enabled = true;
                            menuEscrowKeysToAd.Enabled = false;
                            menuUpgrade.Enabled = false;
                            menuPause.Enabled = false;
                            menuResume.Enabled = false;
                            menuSuspend.Enabled = false;
                            menuSuspendOff.Enabled = false;
                            menuSuspendOn.Enabled = false;
                            break;
                        }
                    case ENCRYPTION_STATUS_NOT_ENCRYPTABLE:
                        {
                            // Make options appropriate to an unknown volume available (everything dimmed).
                            buttonEncrypt.Enabled = false;
                            buttonDecrypt.Enabled = false;
                            buttonLock.Enabled = false;
                            buttonUnlock.Enabled = false;
                            buttonProtectors.Enabled = false;
                            buttonAdvanced.Enabled = true;
                            encryptionProgress.Value = 0;
                            volumeStatus.Text = "Volume cannot be managed with BitLocker.";
                            menuAutoUnlock.Enabled = false;
                            menuAutoUnlockEnable.Enabled = false;
                            menuAutoUnlockDisable.Enabled = false;
                            menuBek.Enabled = false;
                            menuBekAdd.Enabled = false;
                            menuBekChange.Enabled = false;
                            menuBekDelete.Enabled = false;
                            menuNumPassword.Enabled = false;
                            menuNumPasswordAdd.Enabled = false;
                            menuNumPasswordDelete.Enabled = false;
                            menuSmartCards.Enabled = false;
                            menuSmartCardsAdd.Enabled = false;
                            menuSmartCardsDelete.Enabled = false;
                            menuPassword.Enabled = false;
                            menuPasswordAdd.Enabled = false;
                            menuPasswordChange.Enabled = false;
                            menuPasswordDelete.Enabled = false;
                            menuViewProtectors.Enabled = false;
                            menuEscrowKeysToAd.Enabled = false;
                            menuUpgrade.Enabled = false;
                            menuPause.Enabled = false;
                            menuResume.Enabled = false;
                            menuSuspend.Enabled = false;
                            menuSuspendOff.Enabled = false;
                            menuSuspendOn.Enabled = false;
                            break;
                        }
                    case ENCRYPTION_STATUS_UNKNOWN:
                    default:
                        {
                            // Make options appropriate to an unknown volume available (everything dimmed).
                            buttonEncrypt.Enabled = false;
                            buttonDecrypt.Enabled = false;
                            buttonLock.Enabled = false;
                            buttonUnlock.Enabled = false;
                            buttonProtectors.Enabled = false;
                            buttonAdvanced.Enabled = true;
                            encryptionProgress.Value = 0;
                            volumeStatus.Text = "Unable to retrieve volume details.";
                            menuAutoUnlock.Enabled = false;
                            menuAutoUnlockEnable.Enabled = false;
                            menuAutoUnlockDisable.Enabled = false;
                            menuBek.Enabled = false;
                            menuBekAdd.Enabled = false;
                            menuBekChange.Enabled = false;
                            menuBekDelete.Enabled = false;
                            menuNumPassword.Enabled = false;
                            menuNumPasswordAdd.Enabled = false;
                            menuNumPasswordDelete.Enabled = false;
                            menuSmartCards.Enabled = false;
                            menuSmartCardsAdd.Enabled = false;
                            menuSmartCardsDelete.Enabled = false;
                            menuPassword.Enabled = false;
                            menuPasswordAdd.Enabled = false;
                            menuPasswordChange.Enabled = false;
                            menuPasswordDelete.Enabled = false;
                            menuViewProtectors.Enabled = false;
                            menuEscrowKeysToAd.Enabled = false;
                            menuUpgrade.Enabled = false;
                            menuPause.Enabled = false;
                            menuResume.Enabled = false;
                            menuSuspend.Enabled = false;
                            menuSuspendOff.Enabled = false;
                            menuSuspendOn.Enabled = false;
                            break;
                        }
                }

                // Is FIPS compliance mandatory? If so, there are some options we need to turn off in all cases.
                // Whether there are conflicting policies or not, FIPS always prevails. We'll just disable stuff;
                // the user would get an error message if they tried anyway, so we might as well just prevent them
                // from trying.
                if (GPConfig.IsFipsComplianceMandatory)
                {
                    // No use of numeric passwords - protectors menu.
                    menuNumPassword.Enabled = false;
                    menuNumPasswordAdd.Enabled = false;
                    menuNumPasswordDelete.Enabled = false;

                    // No use of passphrases - protectors menu.
                    menuPassword.Enabled = false;
                    menuPasswordAdd.Enabled = false;
                    menuPasswordChange.Enabled = false;
                    menuPasswordDelete.Enabled = false;

                    // No key escrow.
                    menuEscrowKeysToAd.Enabled = false;
                }

                // Other policy - disabling of items - OS volume.
                if (!GPConfig.Win7OsAllowBek && disk.HdType == DiskType.DISK_SYSTEM)
                {
                    // No use of external keys - protectors menu.
                    menuBek.Enabled = false;
                    menuBekAdd.Enabled = false;
                    menuBekChange.Enabled = false;
                    menuBekDelete.Enabled = false;
                }
                if (!GPConfig.Win7OsAllowNumericPw && disk.HdType == DiskType.DISK_SYSTEM)
                {
                    // No use of numeric passwords - protectors menu.
                    menuNumPassword.Enabled = false;
                    menuNumPasswordAdd.Enabled = false;
                    menuNumPasswordDelete.Enabled = false;
                    
                    // No key escrow.
                    menuEscrowKeysToAd.Enabled = false;
                }

                // Other policy - disabling of items - FDV.
                if (!GPConfig.Win7FdvAllowBek && disk.HdType == DiskType.DISK_FIXED_DATA)
                {
                    // No use of external keys - protectors menu.
                    menuBek.Enabled = false;
                    menuBekAdd.Enabled = false;
                    menuBekChange.Enabled = false;
                    menuBekDelete.Enabled = false;
                }
                if (!GPConfig.Win7FdvAllowNumericPw && disk.HdType == DiskType.DISK_FIXED_DATA)
                {
                    // No use of numeric passwords - protectors menu.
                    menuNumPassword.Enabled = false;
                    menuNumPasswordAdd.Enabled = false;
                    menuNumPasswordDelete.Enabled = false;

                    // No key escrow.
                    menuEscrowKeysToAd.Enabled = false;
                }

                // Other policy - disabling of items - RDV.
                if (!GPConfig.Win7RdvAllowBek && disk.HdType == DiskType.DISK_ROAMING_DATA)
                {
                    // No use of external keys - protectors menu.
                    menuBek.Enabled = false;
                    menuBekAdd.Enabled = false;
                    menuBekChange.Enabled = false;
                    menuBekDelete.Enabled = false;
                }
                if (!GPConfig.Win7RdvAllowNumericPw && disk.HdType == DiskType.DISK_ROAMING_DATA)
                {
                    // No use of numeric passwords - protectors menu.
                    menuNumPassword.Enabled = false;
                    menuNumPasswordAdd.Enabled = false;
                    menuNumPasswordDelete.Enabled = false;

                    // No key escrow.
                    menuEscrowKeysToAd.Enabled = false;
                }
                if (!GPConfig.Win7RdvAllowSuspend && disk.HdType == DiskType.DISK_ROAMING_DATA)
                {
                    menuSuspendOff.Enabled = false;
                    buttonDecrypt.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Gets the drive letter based on the user's selection.
        /// </summary>
        /// <returns>Drive letter of selection, or String.Empty if selection is invalid.</returns>
        private String GetDeviceIdFromLvi(out String driveLetter, out String volumeType)
        {
            ListViewItem lvi = null;
            try
            {
                lvi = lvEncryptableVolumes.SelectedItems[0];
                driveLetter = lvi.SubItems[0].Text;
                volumeType = lvi.SubItems[11].Text;
                return lvi.SubItems[10].Text;
            }
            catch
            {
                driveLetter = "N/A";
                volumeType = String.Empty;
                return String.Empty;
            }
        }

        private String GetDeviceIdFromLvi(out String driveLetter)
        {
            String discard = String.Empty;
            return GetDeviceIdFromLvi(out driveLetter, out discard);
        }

        private String GetVolumeLabelFromLvi()
        {
            ListViewItem lvi = null;
            try
            {
                lvi = lvEncryptableVolumes.SelectedItems[0];
                return lvi.SubItems[1].Text;
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Gets the encryption status of a volume based on the user's selection.
        /// </summary>
        /// <returns>String with status message, or String.Empty if selection is invalid.</returns>
        private String GetEncryptionStatusFromLvi()
        {
            ListViewItem lvi = null;
            try
            {
                lvi = lvEncryptableVolumes.SelectedItems[0];
                return lvi.SubItems[4].Text;
            }
            catch
            {
                return String.Empty;
            }
        }
        #endregion

        #region Get Disk Information from OS Methods
        /// <summary>
        /// Gets a HardDisk object, which contains various details about a hard disk, based on the specified drive letter.
        /// </summary>
        /// <param name="deviceID">The drive letter of the disk to fetch.</param>
        /// <returns>A HardDisk matching the drive letter; null if none found.</returns>
        private HardDisk GetDiskInfo(string deviceID)
        {
            foreach (HardDisk disk in listOfDisks)
            {
                if (String.Compare(deviceID, disk.DeviceID, true) == 0)
                {
                    // We have a match, so return it.
                    return disk;
                }
            }
            return null;
        }

        /// <summary>
        /// Iterates through the Win32_EncryptableVolumeCollection (WMI) and then updates the list of disks (ArrayList of
        /// HardDisk objects), updating them with their encryption status.
        /// </summary>
        public void RefreshDisks()
        {
            EncryptableVolume.EncryptableVolumeCollection encryptionVolumes = null;
            try
            {
                encryptionVolumes = EncryptableVolume.GetInstances();
            }
            catch (Exception ex)
            {
                QMessageBox.Show("Failed to bind BitLocker WMI: " + ex.Message, "Severe", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);
                configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, false);
                bitLockerShowAllDrives = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigBitLockerShowAllDrives));
                bitLockerPreventLocking = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigBitLockerPreventLocking));
            }
            catch
            {
                // Fail silently.
                bitLockerPreventLocking = false;
                bitLockerShowAllDrives = true;
            }

            listOfDisks.Clear();

            listOfDisks = GetDiskInterfaceInfo();

            for (int i = 0; i < listOfDisks.Count; i++)
            {
                // Set default values for everything.
                HardDisk hardDisk = (HardDisk)listOfDisks[i];
                hardDisk.LockStatus = "Unlocked";
                hardDisk.ProtectionStatus = "None";
                hardDisk.EncryptionStatus = ENCRYPTION_STATUS_NOT_ENCRYPTABLE;
                hardDisk.EncryptionPercentage = "0";
                hardDisk.EncryptionMethod = String.Empty;

                // Set whether it's the OS volume, an FDV or RDV.
                if (IsOsVolume(hardDisk.DriveLetter))
                {
                    hardDisk.VolumeType = "System";
                    hardDisk.HdType = DiskType.DISK_SYSTEM;
                    isSystemVolumeEncrypted = false;
                }
                else
                {
                    switch (hardDisk.DeviceInterface)
                    {
                        case "2":
                            {
                                hardDisk.VolumeType = "Roaming Data";
                                hardDisk.HdType = DiskType.DISK_ROAMING_DATA;
                                break;
                            }
                        case "3":
                            {
                                hardDisk.VolumeType = "Fixed Data";
                                hardDisk.HdType = DiskType.DISK_FIXED_DATA;
                                break;
                            }
                        default:
                            {
                                hardDisk.VolumeType = "Other";
                                hardDisk.HdType = DiskType.DISK_UNKNOWN;
                                break;
                            }
                    }
                }

                // Go through all the encryptable volumes till we find a match to our disk.
                foreach (EncryptableVolume encryptVolume in encryptionVolumes)
                {
                    uint conversionFlag, conversionPercentage;

                    //if (hardDisk.DriveLetter.ToUpper() == encryptVolume.DriveLetter.ToUpper())
                    if (String.Compare(hardDisk.DeviceID, encryptVolume.DeviceID, true) == 0)
                    {
                        //hardDisk.DriveLetter = (encryptVolume.DriveLetter == null ? "N/A" : encryptVolume.DriveLetter);

                        // Get the BitLocker conversion status, which tells us if a disk is encrypted, decrypted, encrypting, decrypting, paused, etc. The
                        // percentage encrypted is also reported.
                        BitLockerVolumeStatus conversionStatus = (BitLockerVolumeStatus)encryptVolume.GetConversionStatus(out conversionFlag, out conversionPercentage);

                        // If the conversion status reports the error FVE_E_LOCKED_VOLUME, the volume is locked and we can't get any more
                        // details until it's unlocked.
                        if (conversionStatus == BitLockerVolumeStatus.FVE_E_LOCKED_VOLUME)
                        {
                            hardDisk.EncryptionPercentage = "N/A";
                            hardDisk.LockStatus = ENCRYPTION_STATUS_LOCKED;
                            hardDisk.EncryptionStatus = ENCRYPTION_STATUS_LOCKED;
                            hardDisk.ProtectionStatus = "Enabled";
                        }
                        else
                        {
                            // The volume is unlocked, so let's get all the attributes--lock state, auto unlock, encryption method, etc.
                            hardDisk.EncryptionStatus = GetEncryptionStatusFromCode(conversionFlag);
                            hardDisk.EncryptionPercentage = conversionPercentage.ToString();

                            if (conversionFlag != 0)
                            {
                                if (hardDisk.VolumeType == "System")
                                {
                                    isSystemVolumeEncrypted = true;
                                }
                                uint lockStatus;
                                BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)encryptVolume.GetLockStatus(out lockStatus);
                                if (bvms == BitLockerVolumeStatus.S_OK)
                                {
                                    // Get the lock status.
                                    if (lockStatus == 1)
                                    {
                                        hardDisk.LockStatus = ENCRYPTION_STATUS_LOCKED;
                                    }
                                }

                                uint protectionStatus;
                                bvms = (BitLockerVolumeStatus)encryptVolume.GetProtectionStatus(out protectionStatus);
                                if (bvms == BitLockerVolumeStatus.S_OK)
                                {
                                    // Get the protection status. If this is disabled, the volume is encrypted but the key protectors are
                                    // disabled. This means any BitLocker-capable computer can read the volume without needing to specify
                                    // a key (the encryption key is "in the clear" on the disk).
                                    if (protectionStatus == 1)
                                    {
                                        hardDisk.ProtectionStatus = "Enabled";
                                    }
                                    else
                                    {
                                        hardDisk.ProtectionStatus = "Suspended";
                                    }
                                }

                                // Get the encryption method.
                                uint encryptionMethod;
                                bvms = (BitLockerVolumeStatus)encryptVolume.GetEncryptionMethod(out encryptionMethod);
                                if (bvms == BitLockerVolumeStatus.S_OK)
                                {
                                    // Get the encryption method (there are 4...AES 128 and AES 256, each with and without Diffuser).
                                    switch (encryptionMethod)
                                    {
                                        case 0:
                                            {
                                                hardDisk.EncryptionMethod = ENCRYPTION_METHOD_NOT_APPLICABLE;
                                                break;
                                            }
                                        case 1:
                                            {
                                                hardDisk.EncryptionMethod = ENCRYPTION_METHOD_AES128_DIFFUSER;
                                                break;
                                            }
                                        case 2:
                                            {
                                                hardDisk.EncryptionMethod = ENCRYPTION_METHOD_AES256_DIFFUSER;
                                                break;
                                            }
                                        case 3:
                                            {
                                                hardDisk.EncryptionMethod = ENCRYPTION_METHOD_AES128;
                                                break;
                                            }
                                        case 4:
                                            {
                                                hardDisk.EncryptionMethod = ENCRYPTION_METHOD_AES256;
                                                break;
                                            }
                                        default:
                                            {
                                                hardDisk.EncryptionMethod = ENCRYPTION_METHOD_UNKNOWN;
                                                break;
                                            }
                                    }
                                }

                                // Get the BitLocker metadata version.  Valid only on Windows 7 and Server 2008 R2.
                                // Invalid in Vista and Server 2008.
                                uint version = System.Convert.ToUInt32(0);
                                if (hardDisk.EncryptionMethod == "N/A")
                                {
                                    // If the disk is not encrypted, there is no point in checking, regardless
                                    // of the OS.
                                    hardDisk.BitLockerVersion = MetadataVersion.NOT_ENCRYPTED;
                                }
                                else
                                {
                                    if (isWindows7Family)
                                    {
                                        // We call the GetVersion method, which Windows 7 and Server 2008 R2 support.
                                        // Reap the version information.  If an error occurs, or we cannot ascertain the
                                        // version, we set to unknown.
                                        bvms = (BitLockerVolumeStatus)encryptVolume.GetVersion(out version);
                                        if (bvms == BitLockerVolumeStatus.S_OK)
                                        {
                                            switch (version)
                                            {
                                                case 0:
                                                    {
                                                        hardDisk.BitLockerVersion = MetadataVersion.VERSION_UNKNOWN;
                                                        break;
                                                    }
                                                case 1:
                                                    {
                                                        hardDisk.BitLockerVersion = MetadataVersion.VERSION_VISTA;
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        hardDisk.BitLockerVersion = MetadataVersion.VERSION_WIN7;
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        hardDisk.BitLockerVersion = MetadataVersion.VERSION_UNKNOWN;
                                                        break;
                                                    }
                                            }
                                        }
                                        else
                                        {
                                            hardDisk.BitLockerVersion = MetadataVersion.VERSION_UNKNOWN;
                                        }
                                    }
                                    else
                                    {
                                        // It is impossible to unlock a BitLocker volume with a metadata version of Windows 7 in Windows Vista
                                        // or Server 2008.  So if we've gotten this far (the disk is encrypted and unlocked), we know it must
                                        // be a Vista version disk.
                                        hardDisk.BitLockerVersion = MetadataVersion.VERSION_VISTA;
                                    }
                                }

                                // Finally, find out if auto-unlock is enabled.
                                bool isAutoUnlockEnabled;
                                String protectorID;
                                bvms = (BitLockerVolumeStatus)encryptVolume.IsAutoUnlockEnabled(out isAutoUnlockEnabled, out protectorID);
                                if (bvms == BitLockerVolumeStatus.S_OK && isAutoUnlockEnabled)
                                {
                                    hardDisk.IsAutoUnlockEnabled = true;
                                }
                                else
                                {
                                    hardDisk.IsAutoUnlockEnabled = false;
                                }
                            }
                        }
                    }
                }

                try
                {
                    // Use WMI to get the Win32_LogicalDisk based on the drive letter (DeviceID).
                    foreach (ManagementObject volume in new ManagementObjectSearcher(
                      "Select * From Win32_Volume Where DeviceID = '" + hardDisk.DeviceID.Replace("\\", "\\\\") + "'").Get())
                    {
                        // Get the file system and volume label.
                        hardDisk.VolumeLabel = volume["Label"] == null ? String.Empty : volume["Label"].ToString();
                        hardDisk.FileSystem = volume["FileSystem"] == null ? "RAW" : volume["FileSystem"].ToString();

                        // Get the capacity and free space. WMI returns these in bytes, so to convert
                        // to proper GB, we need to divide by 1073741824 (1024 ^ 3). Dividing just by 1024
                        // gives KB; dividing by 1048576 gives MB and dividing by .
                        decimal giggage = 0.00M;
                        Decimal.TryParse(volume["Capacity"].ToString(), out giggage);
                        hardDisk.Capacity = Decimal.Round(giggage / 1073741824.00M, 2);
                        Decimal.TryParse(volume["FreeSpace"].ToString(), out giggage);
                        hardDisk.FreeSpace = Decimal.Round(giggage / 1073741824.00M, 2);
                    }
                }
                catch
                {
                    // Couldn't get details; just return the data we have.
                    //QMessageBox.Show(ex.Message, "Severe");
                }

                listOfDisks[i] = hardDisk;
            }
        }

        /// <summary>
        /// Uses WMI to fetch the disk interface information (IDE, USB, FireWire,etc.)
        /// </summary>
        /// <returns></returns>
        public List<HardDisk> GetDiskInterfaceInfo()
        {
            //ArrayList disks = new ArrayList();
            List<HardDisk> disks = new List<HardDisk>();

            int unletteredIndexNo = 0;

            // Browse all WMI volumes.
            foreach (ManagementObject drive in new ManagementObjectSearcher(
                "select * from Win32_Volume").Get())
            {
                // We'll inject all drives (if the flag says to do so) or, only drives that are encryptable.
                if (bitLockerShowAllDrives || (drive["DeviceID"] != null && IsVolumeEncryptable(drive["DeviceID"].ToString())))
                {
                    disks.Add(new HardDisk(drive["DriveLetter"] == null ? "UL" + unletteredIndexNo.ToString() : drive["DriveLetter"].ToString(),
                        drive["DriveType"].ToString(), drive["PNPDeviceID"] == null ? "Invalid" : drive["PNPDeviceID"].ToString(),
                        drive["DeviceID"] == null ? "Invalid" : drive["DeviceID"].ToString(), drive["Caption"] == null ? String.Empty : drive["Caption"].ToString()));
                    if (drive["DriveLetter"] == null)
                    {
                        unletteredIndexNo++;
                    }
                }
                else
                {
                    continue;
                }
            }
            return disks;
        }

        /// <summary>
        /// The BitLocker method GetConversionStatus returns a uinit conversion flag. This method converts
        /// the flag into a text message detailing the encryption status.
        /// </summary>
        /// <param name="conversionFlag">Conversion flag that BitLocker supplies you.</param>
        /// <returns>String detailing the conversion flag.</returns>
        private string GetEncryptionStatusFromCode(uint conversionFlag)
        {
            switch (conversionFlag)
            {
                case 0:
                    {
                        return ENCRYPTION_STATUS_FULLY_DECRYPTED;
                    }
                case 1:
                    {
                        return ENCRYPTION_STATUS_FULLY_ENCRYPTED;
                    }
                case 2:
                    {
                        return ENCRYPTION_STATUS_ENCRYPTING;
                    }
                case 3:
                    {
                        return ENCRYPTION_STATUS_DECRYPTING;
                    }
                case 4:
                    {
                        return ENCRYPTION_STATUS_ENCRYPTION_PAUSED;
                    }
                case 5:
                    {
                        return ENCRYPTION_STATUS_DECRYPTION_PAUSED;
                    }
                default:
                    {
                        return ENCRYPTION_STATUS_UNKNOWN;
                    }
            }
        }

        /// <summary>
        /// Gets the EncryptableVolume from the OS based on the drive letter.
        /// </summary>
        /// <param name="driveLetter">Drive letter of the EncryptableVolume to fetch.</param>
        /// <param name="volume">EncryptableVolume object returned by BitLocker.</param>
        /// <returns>true if EncryptableVolume fetched; false if failed.</returns>
        private bool GetEncryptableVolumeInfoByDeviceID(String deviceID, out EncryptableVolume volume, out String volumeDisplayName)
        {
            try
            {
                String driveLetter = Properties.Resources.DriveLetterNa;
                EncryptableVolume.EncryptableVolumeCollection encryptionVolumes = EncryptableVolume.GetInstances();
                foreach (EncryptableVolume ev in encryptionVolumes)
                {
                    if (String.Compare(deviceID, ev.DeviceID, true) == 0 ||
                        String.Compare(deviceID, ev.DriveLetter, true) == 0)
                    {
                        volume = ev;
                        if (volume.DriveLetter == null)
                        {
                            volumeDisplayName = deviceID;
                        }
                        else
                        {
                            volumeDisplayName = volume.DriveLetter;
                        }
                        return true;
                    }
                }

                volumeDisplayName = (driveLetter.StartsWith("UL") ? deviceID : driveLetter);
                newStatus = "Volume " + volumeDisplayName + " cannot be managed.";
                QMessageBox.Show("Volume " + volumeDisplayName + " cannot be managed by BitLocker. Either the volume does not support it, or the " +
                  "volume is the reserved BitLocker boot partition required by the operating system to boot.", "BitLocker Drive Encryption",
                  MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                volume = null;
                return false;
            }
            catch (Exception ex)
            {
                QMessageBox.Show("Exceptions were detected trying to access the volume. " + ex.Message, "Severe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                volumeDisplayName = Properties.Resources.DriveLetterNa;
                volume = null;
                statusBar.Text = "Failed to access volume.";
                return false;
            }
        }

        /// <summary>
        /// Checks the BitLocker EncryptableVolumeCollection to see if the specified disk can be encrypted (or have any BitLocker
        /// operations performed on it).
        /// </summary>
        /// <param name="driveLetter">Drive letter to check.</param>
        /// <returns>true if volume can be managed with BitLocker; false if it cannot.</returns>
        private bool IsVolumeEncryptable(String deviceID)
        {
            EncryptableVolume.EncryptableVolumeCollection encryptionVolumes = EncryptableVolume.GetInstances();
            foreach (EncryptableVolume ev in encryptionVolumes)
            {
                if (String.Compare(deviceID, ev.DeviceID, true) == 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Encrypt Volume
        /// <summary>
        /// Encrypts a volume. First checks to be sure no encryption operations are running then allows user to encrypt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EncryptVolume(object sender, EventArgs e)
        {
            // Don't start an encryption operation if another is pending.
            if (encryptorRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another encryption request. Please wait a few seconds " +
                  "and try again.", "Encrypt Volume", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            String volumeDisplayName = (driveLetter.StartsWith("UL") ? deviceID : driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            // Warn on Low disk space.
            if (lvEncryptableVolumes.SelectedItems[0].SubItems[3].Text.Contains("LOW"))
            {
                if (QMessageBox.Show("Volume " + volumeDisplayName + " has less than 10% disk space remaining. This may " +
                    "prevent BitLocker from starting encryption on the volume. Do you want to continue?", "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) ==
                    DialogResult.No)
                {
                    return;
                }
            }

            // Make sure the volume can be encrypted. Some NTFS volumes, like the reserved boot partition, cannot be encrypted.
            if (!IsVolumeEncryptable(deviceID))
            {
                newStatus = "Volume " + volumeDisplayName + " cannot be encrypted.";
                QMessageBox.Show("Volume " + volumeDisplayName + " cannot be encrypted by BitLocker. Either the volume does not support it, or the " +
                  "volume is the reserved BitLocker boot partition required by the operating system to boot.", "BitLocker Drive Encryption",
                  MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the current state of encryption.
            String encryptionStatus = GetEncryptionStatusFromLvi();

            // If paused, just resume. If decrypting/decryption paused, reverse and start encrypting.
            if (String.Compare(encryptionStatus, "encryption paused", true) == 0)
            {
                ResumeOperation(sender, e);
                return;
            }
            else if (String.Compare(encryptionStatus, "decryption paused", true) == 0 ||
                 String.Compare(encryptionStatus, "decrypting", true) == 0)
            {
                ReverseDecryption(sender, e, deviceID, volumeDisplayName);
                return;
            }

            if (IsOsVolume(volumeDisplayName))
            {
                SystemVolumeEncryptionRequest request = null;
                newStatus = "Starting Encryption dialogue.";

                // Pull up the EncryptVolumeDialogue to get the user's options for encrypting.
                EncryptSystemVolumeDialogue evd = new EncryptSystemVolumeDialogue(isWindows7Family, deviceID,
                    GetVolumeLabelFromLvi());
                if (evd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                request = new SystemVolumeEncryptionRequest(evd.Volume, evd.VolumeLabel, evd.CreateRecoveryKey,
                    evd.CreateUserNumericPassword, evd.CreateAutoNumericPassword, evd.StartupKeyPath, evd.RecoveryKeyPath,
                    evd.RecoveryPasswordPath, evd.Method, evd.AllowPrintingAndSaving, evd.Pin, evd.TpmType,
                    evd.PerformSystemCheck, evd.RebootImmediately);

                // Now start the encryption worker thread.
                encryptorThread = new Thread(new ParameterizedThreadStart(EncryptOsVolumeWorkerThread));
                encryptorThread.Name = "Encryptor Thread";
                // The thread uses the SaveFileDialog, which raises an exception in a multithreaded apartment, so
                // we set to single-threaded apartment (STA).
                encryptorThread.SetApartmentState(ApartmentState.STA);
                encryptorThread.Start(request);
            }
            else
            {
                DataVolumeEncryptionRequest request = null;

                // Pull up the EncryptVolumeDialogue to get the user's options for encrypting.
                if (isWindows7Family)
                {
                    // Try to determine if there is a Password or Cert protector; if so don't offer them to the user.
                    newStatus = "Checking existing key protectors...";
                    EncryptableVolume volume = null;
                    bool passwordExists, certExists;
                    passwordExists = certExists = false;

                    if (GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
                    {
                        String protId = String.Empty;
                        if (DoesSpecifiedProtectorAlreadyExist(volume, out protId, KeyProtectorTypes.PASSPHRASE))
                        {
                            passwordExists = true;
                        }
                        if (DoesSpecifiedProtectorAlreadyExist(volume, out protId, KeyProtectorTypes.PUBLIC_KEY))
                        {
                            // Matt Maes: Do you really want this check? You *CAN* have multiple PUBLIC_KEY protectors
                            // on a volume, unlike the PASSPHRASE where only one is allowed.
                            certExists = true;
                        }
                        volume.Dispose();
                    }

                    newStatus = "Starting Encryption dialogue.";
                    EncryptDataVolumeWin7Dialogue evd = new EncryptDataVolumeWin7Dialogue(deviceID, GetVolumeLabelFromLvi(),
                        lvEncryptableVolumes.SelectedItems[0].SubItems[2].Text, isSystemVolumeEncrypted,
                        (lvEncryptableVolumes.SelectedItems[0].SubItems[11].Text == "Roaming Data" ? true : false),
                        passwordExists, certExists);
                    if (evd.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    request = new DataVolumeEncryptionRequest(evd.Volume, evd.VolumeLabel, evd.FileSystem,
                        evd.CreateRecoveryKey, evd.CreateUserNumericPassword, evd.CreateAutoNumericPassword,
                        evd.RecoveryKeyLocation, evd.RecoveryPasswordLocation, evd.Method, evd.AllowPrintingAndSaving,
                        evd.CreateAutoNumericPassword, evd.CreateCert, evd.Thumbprint, evd.CreatePassphrase,
                        evd.Passphrase, evd.CreateBl2gReader, evd.SetOrg, evd.OrgString);
                }

                // Now start the encryption worker thread.
                encryptorThread = new Thread(new ParameterizedThreadStart(EncryptDataVolumeWorkerThread));
                encryptorThread.Name = "Encryptor Thread";
                // The thread uses the SaveFileDialog, which raises an exception in a multithreaded apartment, so
                // we set to single-threaded apartment (STA).
                encryptorThread.SetApartmentState(ApartmentState.STA);
                encryptorThread.Start(request);
            }
        }

        private void EncryptDataVolumeWorkerThread(object data)
        {
            // Prevent additional encryption requests from starting.
            encryptorRunning = true;

            // Get the user's encryption preferences.
            DataVolumeEncryptionRequest request = (DataVolumeEncryptionRequest)data;
            String volumeDisplayName = String.Empty;

            // Get a handle to the desired volume.
            EncryptableVolume volume = null;
            if (!GetEncryptableVolumeInfoByDeviceID(request.Volume, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                encryptorRunning = false;
                return;
            }

            int protectorsRemaining = request.ProtectorsToCreate;
            bool errorDetected = false;

            /* Try creating the protectors, and keep track of them. If any fail, attempt to roll back those that
             * have been created.
             * 
             * If a FAT file system, a discovery volume can be created; this must be done first.
             *
             * Create order:
             * 1.  Windows 7 Password
             * 2.  Windows 7 Cert
             * 3.  AU BEK
             * 4.  Recovery Key
             * 5.  Auto or User 48-Digit Password
             * 
             * Optionally set org id.
             * 
             */
            String discoveryVolumeInfo = String.Empty;

            if (isWindows7Family)
            {
                BitLockerVolumeStatus discVolume = BitLockerVolumeStatus.S_OK;
                if (request.CreateBl2gReader)
                {
                    newStatus = "Creating Discovery Volume...";
                    discVolume = (BitLockerVolumeStatus)volume.PrepareVolume("FAT32");
                }

                if (discVolume == BitLockerVolumeStatus.S_OK && request.CreateBl2gReader)
                {
                    discoveryVolumeInfo = "A BitLocker To Go Discovery Volume was created. This volume can be used with earlier Windows versions.\r\n\r\n";
                }
                else if (request.FileSystem == "NTFS")
                {
                    discoveryVolumeInfo = "The file system is NTFS. A BitLocker To Go Discovery Volume was not created (BL2G does not support NTFS).\r\n\r\n";
                }
                else if (!request.CreateBl2gReader)
                {
                    discoveryVolumeInfo = "At your request, a BitLocker To Go (BL2G) Discovery Volume was NOT created.\r\n\r\n";
                }
                else
                {
                    newStatus = "Discovery Volume was not created.";
                    String extraInfo = String.Empty;
                    if (discVolume == BitLockerVolumeStatus.FVE_E_NOT_DECRYPTED)
                    {
                        extraInfo = "The volume is reporting as not fully decrypted. If the volume shows as being " +
                            "fully decrypted, this problem can occur if one or more Key Protectors exist on the volume. " +
                            "Ensure there are ZERO Key Protectors on the volume before attempting to create a " +
                            "Discovery Volume.\n\n";
                    }

                    if (QMessageBox.Show("BitLocker was unable to create a FAT32 Discovery Volume, which enables the BitLocker " +
                        "To Go Reader. Without this Discovery Volume, you will not be able to unlock your volume (as a read-only " +
                        "volume) in Windows XP, Server 2003, Vista or Server 2008.\n\n" + extraInfo + "You can choose to ignore this issue and " +
                        "continue with encryption. If you do, your volume will be created as a native BitLocker volume, and " +
                        "it will be usable only with Windows 7 (Enterprise and Ultimate editions) and Windows Server 2008 R2.\n\n" +
                        "Do you want to continue without a Discovery Volume and encrypt volume " + volumeDisplayName + "?",
                        "Failed to Create Discovery Volume", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                        == DialogResult.No)
                    {
                        // Dispose the volume object and release encryptor so we can start a new one.
                        volume.Dispose();
                        encryptorRunning = false;
                        return;
                    }
                }
            }

            newStatus = "Installing Key Protectors...";

            request.PasswordProtectorId = String.Empty;
            request.CertificateProtectorId = String.Empty;
            request.AutoUnlockProtectorId = String.Empty;
            request.RecoveryKeyProtectorId = String.Empty;
            request.RecoveryPasswordProtectorId = String.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            BitLockerVolumeStatus bvms = BitLockerVolumeStatus.S_OK;

            sb.Append("Taryn BitLocker Manager - Encrypt Volume - " + DateTime.Now.ToString() + "\r\n\r\n");

            sb.Append("Below is important information regarding your Key Protectors on volume " + request.Volume + ".\r\n");
            sb.Append("Store this information in a secure location. Do not lose or forget your passwords, and do not\r\n");
            sb.Append("lose any other keys (external keys, certificates, etc.). If you forget or lose all means of\r\n");
            sb.Append("unlocking your volume, all data on it will be permanently inaccessible.\r\n\r\n");

            sb.Append("Volume/Drive: " + request.Volume + "\r\n");
            sb.Append("Volume Label: " + request.VolumeLabel + "\r\n");
            sb.Append("File System: " + request.FileSystem + "\r\n\r\n");

            sb.Append(discoveryVolumeInfo);

            // 1 - Windows 7 Password
            if (isWindows7Family && request.CreatePassword && !errorDetected)
            {
                newStatus = "Adding Password Key Protector...";
                String protectorId = String.Empty;
                bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithPassphrase(GenerateFriendlyKeyName("PASS"),
                    request.Password, out protectorId);
                if (bvms == BitLockerVolumeStatus.S_OK)
                {
                    sb.Append("Added Key Protector: Password\r\n");
                    sb.Append("Key Protector ID: " + protectorId + "\r\n\r\n");
                    request.PasswordProtectorId = protectorId;
                }
                else
                {
                    newStatus = "Failed to add Password Key Protector.";
                    DisplayErrorMessage(bvms, "BitLocker failed to generate a Password Key Protector on volume " +
                        request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.", "Encryption " +
                        "Failed", true, true);
                    errorDetected = true;
                }
            }

            // 2 - Windows 7 Certificate
            if (isWindows7Family && request.CreateCertKey && !errorDetected)
            {
                newStatus = "Adding Certificate Key Protector...";
                String protectorId = String.Empty;
                bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithCertificateThumbprint(GenerateFriendlyKeyName("CERT"),
                    request.CertificateThumbprint, out protectorId);
                if (bvms == BitLockerVolumeStatus.S_OK)
                {
                    sb.Append("Added Key Protector: Smart Card Certificate\r\n");
                    sb.Append("Key Protector ID: " + protectorId + "\r\n\r\n");
                    request.CertificateProtectorId = protectorId;
                }
                else
                {
                    newStatus = "Failed to add Certificate Key Protector.";
                    DisplayErrorMessage(bvms, "BitLocker failed to generate a Certificate Key Protector on volume " +
                        request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.", "Encryption " +
                        "Failed", true, true);
                    errorDetected = true;
                }
            }

            // 3 - AU Key
            if (request.CreateAutoUnlockKey && !errorDetected)
            {
                newStatus = "Adding Auto-Unlock Key Protector...";
                String protectorId = String.Empty;
                bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithExternalKey(null, "AU Key for " +
                    System.Environment.MachineName, out protectorId);
                if (bvms == BitLockerVolumeStatus.S_OK)
                {
                    sb.Append("Added Key Protector: External Key (Auto-Unlock Key)\r\n");
                    sb.Append("Key Protector ID: " + protectorId + "\r\n\r\n");
                    request.AutoUnlockProtectorId = protectorId;
                }
                else
                {
                    newStatus = "Failed to add Auto-Unlock Key Protector.";
                    DisplayErrorMessage(bvms, "BitLocker failed to generate an Auto-Unlock Key Protector on volume " +
                        request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.", "Encryption " +
                        "Failed", true, true);
                    errorDetected = true;
                }
            }

            // 4 - Recovery Key (BEK)
            if (request.CreateRecoveryKey && !errorDetected)
            {
                newStatus = "Adding Recovery Key Protector...";
                String protectorId = String.Empty;
                bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithExternalKey(null, GenerateFriendlyKeyName("BEK"),
                    out protectorId);
                if (bvms == BitLockerVolumeStatus.S_OK)
                {
                    sb.Append("Added Key Protector: External Key (Recovery Key)\r\n");
                    sb.Append("Key Protector ID: " + protectorId + "\r\n");
                    request.RecoveryKeyProtectorId = protectorId;
                }
                else
                {
                    newStatus = "Failed to add Recovery Key Protector.";
                    DisplayErrorMessage(bvms, "BitLocker failed to generate a Recovery Key Protector on volume " +
                        request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.", "Encryption " +
                        "Failed", true, true);
                    errorDetected = true;
                }

                if (!errorDetected)
                {
                    String fileName = String.Empty;
                    newStatus = "Getting key filename...";
                    bvms = (BitLockerVolumeStatus)volume.GetExternalKeyFileName(request.RecoveryKeyProtectorId,
                        out fileName);

                    if (bvms == BitLockerVolumeStatus.S_OK)
                    {
                        bvms = (BitLockerVolumeStatus)volume.SaveExternalKeyToFile(request.RecoveryKeyPath,
                            request.RecoveryKeyProtectorId);

                        sb.Append("Save Location: " + request.RecoveryKeyPath + fileName + "\r\n\r\n");

                        if (bvms != BitLockerVolumeStatus.S_OK)
                        {
                            DisplayErrorMessage(bvms, "BitLocker failed to save the Recovery Key Protector file " +
                                request.RecoveryKeyPath + fileName + ". Encryption of volume " + request.Volume + " has been cancelled.",
                                "Encryption Failed", true, true);
                            errorDetected = true;
                        }
                    }
                    else
                    {
                        DisplayErrorMessage(bvms, "BitLocker failed to retrieve the Recovery Key Protector filename " +
                            "on volume " + request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.",
                            "Encryption Failed", true, true);
                        errorDetected = true;
                    }
                }
            }

            // 5 - Automatic/User 48-digit Password
            if ((request.CreateRecoveryPassword || request.CreateAutoPassword) && !errorDetected)
            {
                newStatus = "Adding 48-Digit Password Key Protector...";
                String protectorId = String.Empty;
                bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithNumericalPassword(GenerateFriendlyKeyName("48PW"),
                    null, out protectorId);
                if (bvms == BitLockerVolumeStatus.S_OK)
                {
                    String password48 = String.Empty;
                    bvms = (BitLockerVolumeStatus)volume.GetKeyProtectorNumericalPassword(protectorId, out password48);
                    request.RecoveryPassword = password48;
                    if (bvms != BitLockerVolumeStatus.S_OK)
                    {
                        newStatus = "Failed to fetch 48-Digit Password Key Protector.";
                        DisplayErrorMessage(bvms, "BitLocker failed to retrieve the 48-Digit Password Key Protector on volume " +
                            request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.", "Encryption " +
                            "Failed", true, true);
                        errorDetected = true;
                    }
                    else
                    {
                        if (request.CreateRecoveryPassword)
                        {
                            // Only if not an "auto" PW do we allow the user to see it.
                            sb.Append("Added Key Protector: Numeric (48-Digit) Password\r\n");
                            sb.Append("Key Protector ID: " + protectorId + "\r\n");
                            sb.Append("48-Digit Password: " + request.RecoveryPassword + "\r\n\r\n");
                        }
                        request.RecoveryPasswordProtectorId = protectorId;
                    }
                }
                else
                {
                    newStatus = "Failed to add 48-Digit Password Key Protector.";
                    DisplayErrorMessage(bvms, "BitLocker failed to generate a 48-Digit Password Key Protector on volume " +
                        request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.", "Encryption " +
                        "Failed", true, true);
                    errorDetected = true;
                }
            }

            if (isWindows7Family && request.SetOrgId && !errorDetected && !String.IsNullOrEmpty(request.OrgId))
            {
                bvms = (BitLockerVolumeStatus)volume.SetIdentificationField(request.OrgId);
                if (bvms == BitLockerVolumeStatus.S_OK)
                {
                    sb.Append("Successfully set identification string " + request.OrgId + " on the volume.\r\n\r\n");
                }
                else
                {
                    DisplayErrorMessage(bvms, "BitLocker failed to set your organization identification string. This is not " +
                        "a severe error. Encryption of your volume will continue.", "Warning", false, false);
                }
            }

            // Encrypt it!
            if (!errorDetected)
            {
                newStatus = "Encrypting volume " + request.Volume + "...";

                try
                {
                    bvms = (BitLockerVolumeStatus)volume.Encrypt((uint)request.Method);
                }
                catch (Exception ex)
                {
                    bvms = BitLockerVolumeStatus.FVE_E_ACTION_NOT_ALLOWED;
                    //EpicFail fail = new EpicFail(ex, "Not sure what needs to be done did here.");
                    //fail.ShowDialog();
                    this.Invoke(dlgEpicFail, new object[] { ex, String.Empty, "Not sure what needs to be done did here." });
                }

                if (request.CreateAutoUnlockKey && bvms == BitLockerVolumeStatus.S_OK)
                {
                    newStatus = "Enabling automatic unlocking...";
                    bvms = (BitLockerVolumeStatus)volume.EnableAutoUnlock(request.AutoUnlockProtectorId);
                    if (bvms == BitLockerVolumeStatus.S_OK)
                    {
                        sb.Append("Automatic unlocking of the volume on computer " + System.Environment.MachineName + " has been enabled.\r\n");
                        newStatus = "Encryption successfully initiated on volume " + request.Volume + ".";
                        sb.Append("Security is everyone's responsibility. Thank you for encrypting your volume.");
                        DisplayMessage("Encryption of volume " + request.Volume + " was successfully initiated.",
                            "Start Encryption", MessageBoxButtons.OK, MessageBoxIcon.Information, false);
                        if (request.AllowPrintingAndSaving)
                        {
                            DisplayResultWindow(sb.ToString(), request.RecoveryPasswordPath,
                                request.Volume, request.CreateRecoveryPassword);
                        }
                        DisplayRefresh();
                    }
                    else
                    {
                        sb.Append("Automatic unlocking of the volume on computer " + System.Environment.MachineName + " could not be enabled\r\n");
                        String errorCode, errorSymbol;
                        errorCode = errorSymbol = String.Empty;
                        String errorMessage = ErrorInfo.GetErrorMessage(bvms, out errorSymbol, out errorCode);
                        sb.Append("due to error: " + errorMessage + "\r\n\rn");
                        sb.Append("Security is everyone's responsibility. Thank you for encrypting your volume.");
                        DisplayMessage("Encryption of volume " + request.Volume + " was successfully initiated. However, automatic unlocking " +
                            "could not be enabled due to error: " + errorMessage,
                            "Start Encryption", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, false);
                        if (request.AllowPrintingAndSaving)
                        {
                            DisplayResultWindow(sb.ToString(), request.RecoveryPasswordPath,
                                request.Volume, request.CreateRecoveryPassword);
                        }
                        DisplayRefresh();
                    }
                    sb.Append("Encryption of volume " + request.Volume + " is now in progress.\r\n\r\n");
                }
                else if (bvms == BitLockerVolumeStatus.S_OK)
                {
                    sb.Append("Encryption of volume " + request.Volume + " is now in progress.\r\n\r\n");
                    newStatus = "Encryption successfully initiated on volume " + request.Volume + ".";
                    sb.Append("Security is everyone's responsibility. Thank you for encrypting your volume.");
                    DisplayMessage("Encryption of volume " + request.Volume + " was successfully initiated.",
                        "Start Encryption", MessageBoxButtons.OK, MessageBoxIcon.Information, false);
                    if (request.AllowPrintingAndSaving)
                    {
                        DisplayResultWindow(sb.ToString());
                    }
                    DisplayRefresh();
                }
                else // error
                {
                    newStatus = "Encryption failed to start on volume " + request.Volume + ".";
                    DisplayErrorMessage(bvms, "Encryption of volume " + request.Volume + " failed to start.",
                        "Encryption Failed", true, true);
                    errorDetected = true;
                }
            }

            if (errorDetected)
            {
                // Try to remove the key protectors.
                int successfullyRemoved, failedRemoved;
                successfullyRemoved = failedRemoved = 0;

                if (!String.IsNullOrEmpty(request.AutoUnlockProtectorId))
                {
                    bvms = (BitLockerVolumeStatus)volume.DeleteKeyProtector(request.AutoUnlockProtectorId);
                    if (bvms == BitLockerVolumeStatus.S_OK)
                    {
                        successfullyRemoved++;
                    }
                    else
                    {
                        failedRemoved++;
                    }
                }

                if (!String.IsNullOrEmpty(request.RecoveryKeyProtectorId))
                {
                    bvms = (BitLockerVolumeStatus)volume.DeleteKeyProtector(request.RecoveryKeyProtectorId);
                    if (bvms == BitLockerVolumeStatus.S_OK)
                    {
                        successfullyRemoved++;
                    }
                    else
                    {
                        failedRemoved++;
                    }
                }

                if (!String.IsNullOrEmpty(request.RecoveryPasswordProtectorId))
                {
                    bvms = (BitLockerVolumeStatus)volume.DeleteKeyProtector(request.RecoveryPasswordProtectorId);
                    if (bvms == BitLockerVolumeStatus.S_OK)
                    {
                        successfullyRemoved++;
                    }
                    else
                    {
                        failedRemoved++;
                    }
                }

                if (!String.IsNullOrEmpty(request.CertificateProtectorId))
                {
                    bvms = (BitLockerVolumeStatus)volume.DeleteKeyProtector(request.CertificateProtectorId);
                    if (bvms == BitLockerVolumeStatus.S_OK)
                    {
                        successfullyRemoved++;
                    }
                    else
                    {
                        failedRemoved++;
                    }
                }

                if (!String.IsNullOrEmpty(request.PasswordProtectorId))
                {
                    bvms = (BitLockerVolumeStatus)volume.DeleteKeyProtector(request.PasswordProtectorId);
                    if (bvms == BitLockerVolumeStatus.S_OK)
                    {
                        successfullyRemoved++;
                    }
                    else
                    {
                        failedRemoved++;
                    }
                }

                if (failedRemoved == 0)
                {
                    newStatus = "All key protectors have been deleted from volume " + request.Volume + ".";
                }
                else
                {
                    newStatus = failedRemoved.ToString() + " key " + (failedRemoved > 1 ? "protectors" : "protector") + " failed to delete (" +
                        (successfullyRemoved > 1 ? "were" : "was") + " successful).";
                }
            }

            // Dispose the volume object and release encryptor so we can start a new one.
            volume.Dispose();
            encryptorRunning = false;
        }

        private void EncryptOsVolumeWorkerThread(object data)
        {
            // Prevent additional encryption requests from starting.
            encryptorRunning = true;

            // Get the user's encryption preferences.
            SystemVolumeEncryptionRequest request = (SystemVolumeEncryptionRequest)data;
            String volumeDisplayName = String.Empty;

            // Get a handle to the desired volume.
            EncryptableVolume volume = null;
            if (!GetEncryptableVolumeInfoByDeviceID(request.Volume, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                encryptorRunning = false;
                return;
            }

            int protectorsRemaining = request.ProtectorsToCreate;
            bool errorDetected = false;

            /* Try creating the protectors, and keep track of them. If any fail, attempt to roll back those that
             * have been created.
             * 
             * If a FAT file system, a discovery volume can be created; this must be done first.
             *
             * Create order:
             * 1.  TPM/Startup Key
             * 2.  Recovery Key
             * 3.  Auto or User 48-Digit Password
             * 
             */
            String discoveryVolumeInfo = String.Empty;

            newStatus = "Installing Key Protectors...";

            request.StartupKeyProtectorId = String.Empty;
            request.RecoveryKeyProtectorId = String.Empty;
            request.RecoveryPasswordProtectorId = String.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            BitLockerVolumeStatus bvms = BitLockerVolumeStatus.S_OK;

            sb.Append("Taryn BitLocker Manager - Encrypt Volume - " + DateTime.Now.ToString() + "\r\n\r\n");

            sb.Append("Below is important information regarding your Key Protectors on volume " + request.Volume + ".\r\n");
            sb.Append("Store this information in a secure location. Do not lose or forget your passwords, and do not\r\n");
            sb.Append("lose any other keys (external keys, certificates, etc.). If you forget or lose all means of\r\n");
            sb.Append("unlocking your volume, all data on it will be permanently inaccessible.\r\n\r\n");

            sb.Append("Volume/Drive: " + request.Volume + "\r\n");
            sb.Append("Volume Label: " + request.VolumeLabel + "\r\n");

            sb.Append(discoveryVolumeInfo);

            // 1 - TPM/Startup Key (Mandatory)
            newStatus = "Adding TPM/Startup Key Protector...";
            String protectorId = String.Empty;

            switch (request.TpmType)
            {
                // Vista/7
                case "TPM Only":
                    {
                        bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithTPM(System.Environment.MachineName + " TPM Key",
                            null, out protectorId);
                        if (bvms == BitLockerVolumeStatus.S_OK)
                        {
                            sb.Append("Added Key Protector: TPM\r\n");
                            sb.Append("Key Protector ID: " + protectorId + "\r\n\r\n");
                            request.StartupKeyProtectorId = protectorId;
                        }
                        else
                        {
                            newStatus = "Failed to add TPM Key Protector.";
                            DisplayErrorMessage(bvms, "BitLocker failed to generate a TPM Key Protector on volume " +
                                request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.", "Encryption " +
                                "Failed", true, true);
                            errorDetected = true;
                        }
                        break;
                    }
                // Vista/7
                case "TPM + Startup Key":
                    {
                        bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithTPMAndStartupKey(null,
                            System.Environment.MachineName + " TPM + Startup Key", null, out protectorId);
                        if (bvms == BitLockerVolumeStatus.S_OK)
                        {
                            sb.Append("Added Key Protector: TPM + Startup Key\r\n");
                            sb.Append("Key Protector ID: " + protectorId + "\r\n\r\n");
                            request.StartupKeyProtectorId = protectorId;
                        }
                        else
                        {
                            newStatus = "Failed to add TPM + Startup Key Protector.";
                            DisplayErrorMessage(bvms, "BitLocker failed to generate a TPM + Startup Key Protector on volume " +
                                request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.", "Encryption " +
                                "Failed", true, true);
                            errorDetected = true;
                        }
                        break;
                    }
                // Vista/7
                case "TPM + PIN":
                    {
                        bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithTPMAndPIN(
                            System.Environment.MachineName + " TPM + PIN Key", request.Pin, null, out protectorId);
                        if (bvms == BitLockerVolumeStatus.S_OK)
                        {
                            sb.Append("Added Key Protector: TPM + PIN\r\n");
                            sb.Append("Key Protector ID: " + protectorId + "\r\n\r\n");
                            request.StartupKeyProtectorId = protectorId;
                        }
                        else
                        {
                            newStatus = "Failed to add TPM + PIN Key Protector.";
                            DisplayErrorMessage(bvms, "BitLocker failed to generate a TPM + PIN Key Protector on volume " +
                                request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.", "Encryption " +
                                "Failed", true, true);
                            errorDetected = true;
                        }
                        break;
                    }
                // 7 Only
                case "TPM + Startup Key + PIN":
                    {
                        bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithTPMAndPINAndStartupKey(
                            System.Environment.MachineName + " TPM + Startup + PIN Key", null, request.Pin,
                            null, out protectorId);
                        if (bvms == BitLockerVolumeStatus.S_OK)
                        {
                            sb.Append("Added Key Protector: TPM + Startup Key + PIN\r\n");
                            sb.Append("Key Protector ID: " + protectorId + "\r\n\r\n");
                            request.StartupKeyProtectorId = protectorId;
                        }
                        else
                        {
                            newStatus = "Failed to add TPM + Startup + PIN Key Protector.";
                            DisplayErrorMessage(bvms, "BitLocker failed to generate a TPM + Startup + PIN Key Protector on volume " +
                                request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.", "Encryption " +
                                "Failed", true, true);
                            errorDetected = true;
                        }
                        break;
                    }
                // Vista/7
                default:
                    {
                        // No TPM
                        bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithExternalKey(null,
                            System.Environment.MachineName + " Startup Key", out protectorId);
                        if (bvms == BitLockerVolumeStatus.S_OK)
                        {
                            sb.Append("Added Key Protector: Startup Key\r\n");
                            sb.Append("Key Protector ID: " + protectorId + "\r\n");
                            request.StartupKeyProtectorId = protectorId;
                        }
                        else
                        {
                            newStatus = "Failed to add Startup Key Protector.";
                            DisplayErrorMessage(bvms, "BitLocker failed to generate a Startup Key Protector on volume " +
                                request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.", "Encryption " +
                                "Failed", true, true);
                            errorDetected = true;
                        }
                        break;
                    }
            }

            switch (request.TpmType)
            {
                case "TPM + Startup Key":
                case "TPM + Startup Key + PIN":
                case "Startup Key":
                case "TPM Not Installed in Computer":
                    {
                        if (!errorDetected)
                        {
                            String fileName = String.Empty;
                            newStatus = "Getting key filename...";
                            bvms = (BitLockerVolumeStatus)volume.GetExternalKeyFileName(request.StartupKeyProtectorId,
                                out fileName);

                            if (bvms == BitLockerVolumeStatus.S_OK)
                            {
                                bvms = (BitLockerVolumeStatus)volume.SaveExternalKeyToFile(request.StartupKeyPath,
                                    request.StartupKeyProtectorId);

                                sb.Append("Save Location: " + request.StartupKeyPath + fileName + "\r\n\r\n");

                                if (bvms != BitLockerVolumeStatus.S_OK)
                                {
                                    DisplayErrorMessage(bvms, "BitLocker failed to save the Startup Key Protector file " +
                                        request.StartupKeyPath + fileName + ". Encryption of volume " + request.Volume + " has been cancelled.",
                                        "Encryption Failed", true, true);
                                    errorDetected = true;
                                }
                            }
                            else
                            {
                                DisplayErrorMessage(bvms, "BitLocker failed to retrieve the Startup Key Protector filename " +
                                    "on volume " + request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.",
                                    "Encryption Failed", true, true);
                                errorDetected = true;
                            }
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            // 2 - Recovery Key (BEK)
            if (request.CreateRecoveryKey && !errorDetected)
            {
                newStatus = "Adding Recovery Key Protector...";
                protectorId = String.Empty;
                bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithExternalKey(null, GenerateFriendlyKeyName("BEK"),
                    out protectorId);
                if (bvms == BitLockerVolumeStatus.S_OK)
                {
                    sb.Append("Added Key Protector: External Key (Recovery Key)\r\n");
                    sb.Append("Key Protector ID: " + protectorId + "\r\n");
                    request.RecoveryKeyProtectorId = protectorId;
                }
                else
                {
                    newStatus = "Failed to add Recovery Key Protector.";
                    DisplayErrorMessage(bvms, "BitLocker failed to generate a Recovery Key Protector on volume " +
                        request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.", "Encryption " +
                        "Failed", true, true);
                    errorDetected = true;
                }

                if (!errorDetected)
                {
                    String fileName = String.Empty;
                    newStatus = "Getting key filename...";
                    bvms = (BitLockerVolumeStatus)volume.GetExternalKeyFileName(request.RecoveryKeyProtectorId,
                        out fileName);

                    if (bvms == BitLockerVolumeStatus.S_OK)
                    {
                        bvms = (BitLockerVolumeStatus)volume.SaveExternalKeyToFile(request.RecoveryKeyPath,
                            request.RecoveryKeyProtectorId);

                        sb.Append("Save Location: " + request.RecoveryKeyPath + fileName + "\r\n\r\n");

                        if (bvms != BitLockerVolumeStatus.S_OK)
                        {
                            DisplayErrorMessage(bvms, "BitLocker failed to save the Recovery Key Protector file " +
                                request.RecoveryKeyPath + fileName + ". Encryption of volume " + request.Volume + " has been cancelled.",
                                "Encryption Failed", true, true);
                            errorDetected = true;
                        }
                    }
                    else
                    {
                        DisplayErrorMessage(bvms, "BitLocker failed to retrieve the Recovery Key Protector filename " +
                            "on volume " + request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.",
                            "Encryption Failed", true, true);
                        errorDetected = true;
                    }
                }
            }

            // 3 - Automatic/User 48-digit Password
            if ((request.CreateRecoveryPassword || request.CreateAutoPassword) && !errorDetected)
            {
                newStatus = "Adding 48-Digit Password Key Protector...";
                protectorId = String.Empty;
                bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithNumericalPassword(GenerateFriendlyKeyName("48PW"),
                    null, out protectorId);
                if (bvms == BitLockerVolumeStatus.S_OK)
                {
                    String password48 = String.Empty;
                    bvms = (BitLockerVolumeStatus)volume.GetKeyProtectorNumericalPassword(protectorId, out password48);
                    request.RecoveryPassword = password48;
                    if (bvms != BitLockerVolumeStatus.S_OK)
                    {
                        newStatus = "Failed to fetch 48-Digit Password Key Protector.";
                        DisplayErrorMessage(bvms, "BitLocker failed to retrieve the 48-Digit Password Key Protector on volume " +
                            request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.", "Encryption " +
                            "Failed", true, true);
                        errorDetected = true;
                    }
                    else
                    {
                        if (request.CreateRecoveryPassword)
                        {
                            // Only if not an "auto" PW do we allow the user to see it.
                            sb.Append("Added Key Protector: Numeric (48-Digit) Password\r\n");
                            sb.Append("Key Protector ID: " + protectorId + "\r\n");
                            sb.Append("48-Digit Password: " + request.RecoveryPassword + "\r\n\r\n");
                        }
                        request.RecoveryPasswordProtectorId = protectorId;
                    }
                }
                else
                {
                    newStatus = "Failed to add 48-Digit Password Key Protector.";
                    DisplayErrorMessage(bvms, "BitLocker failed to generate a 48-Digit Password Key Protector on volume " +
                        request.Volume + ". Encryption of volume " + request.Volume + " has been cancelled.", "Encryption " +
                        "Failed", true, true);
                    errorDetected = true;
                }
            }

            // Encrypt it!
            if (!errorDetected)
            {
                if (request.PerformHardwareTest)
                {
                    newStatus = "Setting encryption to start after hardware test...";
                    bvms = (BitLockerVolumeStatus)volume.EncryptAfterHardwareTest((uint)request.Method);

                    if (bvms == BitLockerVolumeStatus.S_OK)
                    {
                        sb.Append("Encryption of volume " + request.Volume + " will start after the hardware test.\r\n\r\n");
                        newStatus = "Encryption successfully prepared on volume " + request.Volume + ".";
                        sb.Append("Security is everyone's responsibility. Thank you for encrypting your volume.");
                        DisplayMessage("Encryption of volume " + request.Volume + " was successfully configured. " +
                            "Encryption will start after your computer reboots. A reboot is required to complete " +
                            "the hardware test. If the hardware test fails, encryption will not be started.",
                            "Start Encryption", MessageBoxButtons.OK, MessageBoxIcon.Information, false);
                        if (request.AllowPrintingAndSaving)
                        {
                            DisplayResultWindow(sb.ToString(), request.RecoveryPasswordPath,
                                    request.Volume, request.CreateRecoveryPassword);
                        }

                        // Dispose the volume object and release encryptor so we can start a new one.
                        if (request.RebootImmediately)
                        {
                            volume.Dispose();
                            encryptorRunning = false;
                        }
                        SetHardwareTestAndReboot(request.Volume, request.RebootImmediately);
                    }
                    else // error
                    {
                        newStatus = "Encryption failed to start on volume " + request.Volume + ".";
                        DisplayErrorMessage(bvms, "Encryption of volume " + request.Volume + " failed to start.",
                            "Encryption Failed", true, true);
                        errorDetected = true;
                    }
                }
                else
                {
                    newStatus = "Encrypting volume " + request.Volume + "...";
                    bvms = (BitLockerVolumeStatus)volume.Encrypt((uint)request.Method);

                    if (bvms == BitLockerVolumeStatus.S_OK)
                    {
                        sb.Append("Encryption of volume " + request.Volume + " is now in progress.\r\n\r\n");
                        newStatus = "Encryption successfully initiated on volume " + request.Volume + ".";
                        sb.Append("Security is everyone's responsibility. Thank you for encrypting your volume.");
                        DisplayMessage("Encryption of volume " + request.Volume + " was successfully initiated.",
                            "Start Encryption", MessageBoxButtons.OK, MessageBoxIcon.Information, false);
                        if (request.AllowPrintingAndSaving)
                        {
                            DisplayResultWindow(sb.ToString(), request.RecoveryPasswordPath,
                                    request.Volume, request.CreateRecoveryPassword);
                        }
                        DisplayRefresh();
                    }
                    else // error
                    {
                        newStatus = "Encryption failed to start on volume " + request.Volume + ".";
                        DisplayErrorMessage(bvms, "Encryption of volume " + request.Volume + " failed to start.",
                            "Encryption Failed", true, true);
                        errorDetected = true;
                    }
                }
            }

            if (errorDetected)
            {
                // Try to remove the key protectors.
                int successfullyRemoved, failedRemoved;
                successfullyRemoved = failedRemoved = 0;

                if (!String.IsNullOrEmpty(request.RecoveryKeyProtectorId))
                {
                    bvms = (BitLockerVolumeStatus)volume.DeleteKeyProtector(request.RecoveryKeyProtectorId);
                    if (bvms == BitLockerVolumeStatus.S_OK)
                    {
                        successfullyRemoved++;
                    }
                    else
                    {
                        failedRemoved++;
                    }
                }

                if (!String.IsNullOrEmpty(request.RecoveryPasswordProtectorId))
                {
                    bvms = (BitLockerVolumeStatus)volume.DeleteKeyProtector(request.RecoveryPasswordProtectorId);
                    if (bvms == BitLockerVolumeStatus.S_OK)
                    {
                        successfullyRemoved++;
                    }
                    else
                    {
                        failedRemoved++;
                    }
                }

                if (!String.IsNullOrEmpty(request.StartupKeyProtectorId))
                {
                    bvms = (BitLockerVolumeStatus)volume.DeleteKeyProtector(request.StartupKeyProtectorId);
                    if (bvms == BitLockerVolumeStatus.S_OK)
                    {
                        successfullyRemoved++;
                    }
                    else
                    {
                        failedRemoved++;
                    }
                }

                if (failedRemoved == 0)
                {
                    newStatus = "All key protectors have been deleted from volume " + request.Volume + ".";
                }
                else
                {
                    newStatus = failedRemoved.ToString() + " key " + (failedRemoved > 1 ? "protectors" : "protector") + " failed to delete (" +
                        (successfullyRemoved > 1 ? "were" : "was") + " successful).";
                }
            }

            // Dispose the volume object and release encryptor so we can start a new one.
            volume.Dispose();
            encryptorRunning = false;
        }

        private void ReverseDecryption(object sender, EventArgs e, String deviceID, String volumeDisplayName)
        {
            // Don't start an encrytion operation if one is pending.
            if (encryptorRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another encryption request. Please wait a few seconds " +
                  "and try again.", "Encrypt Volume", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Ask the user if they want to stop a volume currently being decrypted from decrypting and re-encrypt it.
            if (QMessageBox.Show("Do you want to stop decrypting volume " + volumeDisplayName + " and re-encrypt it? " +
              "If you continue, key protectors currently on the volume will remain valid; you will not need to create " +
              "any new key protectors.", "Reverse Decryption", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Start a worker thread to accomplish re-encryption.
                encryptorThread = new Thread(new ParameterizedThreadStart(ReverseDecryptionWorkerThread));
                encryptorThread.Name = "Re-Encryptor Thread";
                encryptorThread.Start(new BitLockerThreadParameters(deviceID));
            }
        }

        private void ReverseDecryptionWorkerThread(object data)
        {
            // Prevent additional encryption requests from starting.
            encryptorRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;

            // Get the volume information.
            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                encryptorRunning = false;
                return;
            }

            // Start encryption by calling Encrypt with argument 0 (use default or whatever method is set on the volume).
            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.Encrypt(0);

            switch (bvms)
            {
                case BitLockerVolumeStatus.S_OK:
                    {
                        // Decryption successfully reversed; now encrypting.
                        newStatus = "Volume " + volumeDisplayName + " is now encrypting.";
                        DisplayMessage("Encryption of volume " + volumeDisplayName + " is now in progress.", "Reverse Decryption",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                case BitLockerVolumeStatus.ERROR_INVALID_OPERATION:
                case BitLockerVolumeStatus.ERROR_INVALID_OPERATION2:
                    {
                        // Cannot start encryption. The decryption operation already completed. User must start over.
                        newStatus = "Re-encryption of volume " + volumeDisplayName + " failed.";
                        DisplayMessage("BitLocker cannot encrypt the volume. Decryption may have already completed. You must re-encrypt " +
                          "the volume with new key protectors.", "Reverse Decryption Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                default:
                    {
                        // Cannot start encryption. Unexpected error.
                        newStatus = "Re-encryption of volume " + volumeDisplayName + " failed.";
                        DisplayErrorMessage(bvms, "BitLocker was unable to reverse the decryption of volume " + volumeDisplayName +
                            " and start encrypting it.", "Reverse Decryption Failed", true, true);
                        break;
                    }
            }

            volume.Dispose();
            encryptorRunning = false;
        }
        #endregion

        #region Decrypt Volume
        private void DecryptVolume(object sender, EventArgs e)
        {
            // Don't start a decryption operation if another is pending.
            if (decryptorRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another encryption request. Please wait a few seconds " +
                  "and try again.", "Decrypt Volume", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            String volumeDisplayName = (driveLetter.StartsWith("UL") ? deviceID : driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            // Get current drive status.
            String encryptionStatus = GetEncryptionStatusFromLvi();

            // If decryption is paused, just resume it.
            if (String.Compare(encryptionStatus, "decryption paused", true) == 0)
            {
                ResumeOperation(sender, e);
                return;
            }

            // Display a scathing dialogue warning the user of all the horrible things about not encrypting, trying to discourage
            // them from decrypting.
            DecryptVolumeDialogue dvd = new DecryptVolumeDialogue(volumeDisplayName, GetVolumeLabelFromLvi());
            if (dvd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // Try to talk the user out of decrypting.
            if (QMessageBox.Show("Decrypting the volume removes all encryption protection! Are you sure " +
              "you want to decrypt volume " + volumeDisplayName + "?", "Decrypt Volume", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
              MessageBoxDefaultButton.Button2)
              == DialogResult.Yes)
            {
                // The user said yes, give it one more shot to stop them...
                if (QMessageBox.Show("Are you ABSOLUTELY, POSITIVELY sure you want to decrypt volume " + volumeDisplayName + "?", "WARNING!",
                  MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    // Okay, the user wants to decrypt anyway, even though horrible things could happen. Start the decryption worker thread.
                    decryptorThread = new Thread(new ParameterizedThreadStart(DecryptVolumeWorkerThread));
                    decryptorThread.Name = "Decryptor Thread";
                    decryptorThread.Start(new BitLockerThreadParameters(deviceID));
                }
            }
        }

        private void DecryptVolumeWorkerThread(object data)
        {
            // Prevent additional decryption operations from starting.
            decryptorRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;

            // Get the EncryptableVoluem details.
            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                decryptorRunning = false;
                return;
            }

            // Start the decryption operation. No key activities required here.
            newStatus = "Decrypting volume " + volumeDisplayName + "...";
            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.Decrypt();
            switch (bvms)
            {
                case BitLockerVolumeStatus.S_OK:
                    {
                        // Decryption started successfully.
                        newStatus = "Volume " + volumeDisplayName + " is now decrypting.";
                        DisplayMessage("Decryption of volume " + volumeDisplayName + " is now in progress.", "Decrypt Volume",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_AUTOUNLOCK_ENABLED:
                    {
                        // Volumes are auto-unlock-enabled in the OS. Continuing will remove (invalidate) all keys in the OS!
                        // Warn the user about this but give them the opportunity to continue. Unlike decryption, which kills
                        // keys at completion, clearing auto-unlock keys from the OS is an IMMEDIATE operation. The keys will be
                        // irrevocably deleted immediately, even if the user reverses the decryption.
                        newStatus = "Automatic unlocking of data volumes detected.";
                        if (QMessageBox.Show("Decryption of volume " + volumeDisplayName + " cannot start because there are " +
                          "auto unlock keys stored in Windows. Decryption of the volume may continue if you clear these " +
                          "keys first. If you clear these keys, they will be immediately and permanently deleted, even " +
                          "if you cancel the decryption process before it completes. All volumes that are currently " +
                          "auto-unlocked will need to be manually unlocked. Do you want to clear all of these auto " +
                          "unlock keys and decrypt the volume?", "Auto Unlock Keys Detected", MessageBoxButtons.YesNo,
                          MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            // Obliterate the auto-unlock keys.
                            bvms = (BitLockerVolumeStatus)volume.ClearAllAutoUnlockKeys();
                            if (bvms == BitLockerVolumeStatus.S_OK)
                            {
                                // Keys obliterated, start decrypting.
                                bvms = (BitLockerVolumeStatus)volume.Decrypt();
                                if (bvms == BitLockerVolumeStatus.S_OK)
                                {
                                    // Decryption successfully started.
                                    newStatus = "Volume " + volumeDisplayName + " is now decrypting.";
                                    DisplayMessage("Decryption of volume " + volumeDisplayName + " is now in progress.", "Decrypt Volume",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    // Decryption could not be started.
                                    newStatus = "Decryption of volume " + volumeDisplayName + " failed.";
                                    DisplayErrorMessage(bvms, "The auto-unlock keys were successfully cleared, but decryption " +
                                        "of volume " + volumeDisplayName + " failed to start.", "Decryption Failed", true, true);
                                }
                            }
                            else
                            {
                                // The auto-unlock keys could not be cleared, so we can't decrypt.
                                newStatus = "Decryption of volume " + volumeDisplayName + " failed.";
                                DisplayErrorMessage(bvms, "The auto-unlock keys could not be cleared from Windows. Decryption " +
                                    "of volume " + volumeDisplayName + " has been cancelled.", "Unable to Clear Keys",
                                    true, true);
                            }
                        }
                        else
                        {
                            newStatus = "User declined clearing auto-unlock keys; decryption of OS volume has been cancelled.";
                        }
                        break;
                    }
                default:
                    {
                        // Cannot start decryption. Unexpected error.
                        newStatus = "Decryption of volume " + volumeDisplayName + " failed.";
                        DisplayErrorMessage(bvms, "BitLocker failed to start decrypting volume " + volumeDisplayName + ".",
                            "Decryption Failed", true, true);
                        break;
                    }
            }

            // Dispose the volume and release the decryptor flag.
            volume.Dispose();
            decryptorRunning = false;
        }
        #endregion

        #region Lock Volume
        /// <summary>
        /// Attempts to lock the selected volume. If the volume is in use, the user is offered the opportunity
        /// to force dismount it.
        /// 
        /// Locking a volume makes it unavailable for use until it is unlocked using one of its key protectors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LockVolume(object sender, EventArgs e)
        {
            // Don't start a lock/unlock operation if another is pending.
            if (lockUnlockRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another lock or unlock request. Please wait a few seconds " +
                  "and try again.", "Lock Volume", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            String volumeDisplayName = (driveLetter.StartsWith("UL") ? deviceID : driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            // Don't allow locking the OS volume! (We stop it here; BitLocker wouldn't allow it anyway.)
            if (IsOsVolume(deviceID))
            {
                QMessageBox.Show("You cannot lock the volume because it contains the running operating system.", "Lock Volume", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Warn the user that locking a volume will make it unavailable until unlocked.
            String questionDialogueText = String.Empty;
            questionDialogueText = UtilityMethods.IsDriveBenderInstalled() ? "Locking a volume will render it unavailable for use. If you lock a volume " +
                "that is actively participating in a disk pool, you may cause the entire pool to be taken offline, the pool could crash or become corrupted, " +
                "or you could blue screen the Server. Disk pooling software was detected on this Server; great care should be taken when choosing to lock a " +
                "volume. " : "Locking a volume will render it unavailable for use. ";
            if (QMessageBox.Show(questionDialogueText + "If you want to use it again, you will need to unlock it.\n\n" +
                "Are you sure you want to lock volume " + volumeDisplayName + "?", "Lock Volume",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Start the worker thread to lock.
                lockUnlockThread = new Thread(new ParameterizedThreadStart(LockVolumeWorkerThread));
                lockUnlockThread.Name = "Lock Volume Thread";
                lockUnlockThread.Start(new BitLockerThreadParameters(deviceID));
            }
        }

        private void LockVolumeWorkerThread(object data)
        {
            // Prevent another lock/unlock operation from being started.
            lockUnlockRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;

            // Get the EncryptableVolume.
            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                lockUnlockRunning = false;
                return;
            }

            // Try locking the volume.
            newStatus = "Locking volume " + volumeDisplayName + "...";
            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.Lock(false);
            switch (bvms)
            {
                case BitLockerVolumeStatus.S_OK:
                    {
                        // Lock successful.
                        newStatus = "Volume " + volumeDisplayName + " is now locked.";
                        DisplayMessage("Volume " + volumeDisplayName + " is now locked.", "Lock Volume", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                case BitLockerVolumeStatus.E_ACCESS_DENIED:
                    {
                        // Lock failed. The volume is in use. (Can be force dismounted - offer this to the user.)
                        // Force dismounting closes all open file handles and may result in data loss!
                        newStatus = "Volume " + volumeDisplayName + " cannot be locked (volume in use).";
                        if (QMessageBox.Show("BitLocker cannot lock volume " + volumeDisplayName + " because Windows reports that " +
                          "it is in use. If you still want to lock this volume, you can force it to dismount. Forcing a " +
                          "volume dismount will close all open file handles to the volume, which may result in data loss. " +
                          "Do you want to force dismount volume " + volumeDisplayName + "?", "Lock Volume (Force Dismount)",
                          MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        {
                            // User chose to force dismount, so try again.
                            bvms = (BitLockerVolumeStatus)volume.Lock(true);
                            if (bvms == BitLockerVolumeStatus.S_OK)
                            {
                                // Lock successful (force dismounted).
                                newStatus = "Volume " + volumeDisplayName + " was force dismounted and is now locked.";
                                DisplayMessage("Volume " + volumeDisplayName + " is now locked.", "Lock Volume", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                // Lock failed.
                                newStatus = "Volume " + volumeDisplayName + " could not be locked.";
                                DisplayErrorMessage(bvms, "BitLocker was unable to lock volume " + volumeDisplayName + ".",
                                    "Lock Failed", true, true);
                                break;
                            }
                        }
                        break;
                    }
                default:
                    {
                        // Lock failed.
                        newStatus = "Volume " + volumeDisplayName + " could not be locked.";
                        DisplayErrorMessage(bvms, "BitLocker was unable to lock volume " + volumeDisplayName + ".",
                            "Lock Failed", true, true);
                        break;
                    }
            }
            volume.Dispose();
            lockUnlockRunning = false;
        }
        #endregion

        #region Pause/Resume Volume
        private void PauseOperation(object sender, EventArgs e)
        {
            // Don't start a pause/resume operation if another is pending.
            if (pauseResumeRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another pause or resume request. Please wait a few seconds " +
                  "and try again.", "Lock Volume", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = String.Empty;
            String encryptionStatus = GetEncryptionStatusFromLvi();
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            // Start the pause worker thread.
            pauseResumeThread = new Thread(new ParameterizedThreadStart(PauseOperationWorkerThread));
            pauseResumeThread.Name = "Pause Thread";
            pauseResumeThread.Start(new BitLockerThreadParameters(deviceID, encryptionStatus));
        }

        private void PauseOperationWorkerThread(object data)
        {
            // Prevent another pause/resume operation from starting.
            pauseResumeRunning = true;

            EncryptableVolume volume = null;
            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;
            String argument = input.Argument;

            // Get the EncryptableVolume.
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                pauseResumeRunning = false;
                return;
            }

            // Pause the operation.
            newStatus = "Pausing...";
            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.PauseConversion();

            switch (bvms)
            {
                case BitLockerVolumeStatus.S_OK:
                    {
                        if (argument.ToLower().Contains("encrypt"))
                        {
                            // Encryption paused.
                            newStatus = "Encryption of volume " + volumeDisplayName + " has been paused.";
                            DisplayMessage("Encryption of volume " + volumeDisplayName + " has been paused.", "Pause",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (argument.ToLower().Contains("decrypt"))
                        {
                            // Decryption paused.
                            newStatus = "Decryption of volume " + volumeDisplayName + " has been paused.";
                            DisplayMessage("Decryption of volume " + volumeDisplayName + " has been paused.", "Pause",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;
                    }
                default:
                    {
                        if (argument.ToLower().Contains("encrypt"))
                        {
                            // Encryption pause failed.
                            newStatus = "Encryption of volume " + volumeDisplayName + " could not be paused.";
                            DisplayErrorMessage(bvms, "Encryption of volume " + volumeDisplayName + " could not be paused.",
                                "Pause Failed", true, true);
                        }
                        else if (argument.ToLower().Contains("decrypt"))
                        {
                            // Decryption pause failed.
                            newStatus = "Decryption of volume " + volumeDisplayName + " could not be paused.";
                            DisplayErrorMessage(bvms, "Decryption of volume " + volumeDisplayName + " could not be paused.",
                                "Pause Failed", true, true);
                        }
                        break;
                    }
            }

            // Dispose the volume and release the pause/resume flag.
            volume.Dispose();
            pauseResumeRunning = false;
        }

        private void ResumeOperation(object sender, EventArgs e)
        {
            // Don't start a pause/resume operation if another is pending.
            if (pauseResumeRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another pause or resume request. Please wait a few seconds " +
                  "and try again.", "Lock Volume", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String encryptionStatus = GetEncryptionStatusFromLvi();
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            // Start the resume thread.
            pauseResumeThread = new Thread(new ParameterizedThreadStart(ResumeOperationWorkerThread));
            pauseResumeThread.Name = "Resume Thread";
            pauseResumeThread.Start(new BitLockerThreadParameters(deviceID, encryptionStatus));
        }

        private void ResumeOperationWorkerThread(object data)
        {
            // Prevent another pause/resume operation from starting.
            pauseResumeRunning = true;

            EncryptableVolume volume = null;
            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;
            String argument = input.Argument;

            // Get the EncryptableVolume.
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                pauseResumeRunning = false;
                return;
            }

            // Resume the operation.
            newStatus = "Resuming...";
            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.ResumeConversion();

            switch (bvms)
            {
                case BitLockerVolumeStatus.S_OK:
                    {
                        if (argument.ToLower().Contains("encrypt"))
                        {
                            // Encryption resumed.
                            newStatus = "Encryption of volume " + volumeDisplayName + " has been resumed.";
                            DisplayMessage("Encryption of volume " + volumeDisplayName + " has been resumed.", "Resume",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (argument.ToLower().Contains("decrypt"))
                        {
                            // Decryption resumed.
                            newStatus = "Decryption of volume " + volumeDisplayName + " has been resumed.";
                            DisplayMessage("Decryption of volume " + volumeDisplayName + " has been resumed.", "Resume",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;
                    }
                default:
                    {
                        if (argument.ToLower().Contains("encrypt"))
                        {
                            // Encryption pause failed.
                            newStatus = "Encryption of volume " + volumeDisplayName + " could not be resumed.";
                            DisplayErrorMessage(bvms, "Encryption of volume " + volumeDisplayName + " could not be resumed.",
                                "Resume Failed", true, true);
                        }
                        else if (argument.ToLower().Contains("decrypt"))
                        {
                            // Decryption pause failed.
                            newStatus = "Decryption of volume " + volumeDisplayName + " could not be resumed.";
                            DisplayErrorMessage(bvms, "Decryption of volume " + volumeDisplayName + " could not be resumed.",
                                "Resume Failed", true, true);
                        }
                        break;
                    }
            }

            // Dispose the volume and and release the pause/resume flag.
            volume.Dispose();
            pauseResumeRunning = false;
        }
        #endregion

        #region Update UI
        /// <summary>
        /// Updates the UI on a timer worker thread if an update is needed.
        /// </summary>
        /// <param name="state"></param>
        private void TimerUpdateStatus(Object state)
        {
            try
            {
                if (terminateOnLoad)
                {
                    return;
                }

                if (String.Compare(oldStatus, newStatus, true) == 0)
                {
                    return;
                }

                oldStatus = newStatus;
                this.Invoke(dlgUpdateUI);
            }
            catch (Exception ex)
            {
                //EpicFail fail = new EpicFail(ex, "The status update timer thread generated an error. No action is required, but if this appears again " +
                //    "please submit a bug report.");
                //fail.ShowDialog();
                this.Invoke(dlgEpicFail, new object[] { ex, String.Empty, "The status update timer thread generated an error. No action is required, but if this appears again " +
                    "please submit a bug report."});
            }
        }

        /// <summary>
        /// Delegate method to update the UI.
        /// </summary>
        private void DUpdateStatus()
        {
            statusBar.Text = newStatus;
        }

        /// <summary>
        /// Delegate method to refresh and reload disk info.
        /// </summary>
        private void DRefreshReloadDisks()
        {
            if (isManualRefreshInProgress)
            {
                return;
            }
            else
            {
                ProcessingDialogue pd = new ProcessingDialogue(this.Location, this.Size);
                pd.TopMost = true;
                pd.Show();
                isManualRefreshInProgress = true;
                RefreshDisks();
                LoadList();
                isManualRefreshInProgress = false;
                pd.Hide();
                pd.Close();
                pd.Dispose();
            }
        }
        #endregion

        #region Display Message Dialogue in Main Thread
        /// <summary>
        /// Causes a message box to be displayed in the main thread. You are encouraged to use this method if you need to display a "final"
        /// message box in a worker thread, which allows the thread to exit rather than sitting around waiting for input.
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <param name="caption">Caption in message box.</param>
        /// <param name="buttons">MessageBoxButtons to display.</param>
        /// <param name="icon">MessageBoxIcon to display.</param>
        private void DisplayMessage(String message, String caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            DisplayMessage(message, caption, buttons, icon, MessageBoxDefaultButton.Button1, true);
        }

        /// <summary>
        /// Causes a message box to be displayed in the main thread. You are encouraged to use this method if you need to display a "final"
        /// message box in a worker thread, which allows the thread to exit rather than sitting around waiting for input.
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <param name="caption">Caption in message box.</param>
        /// <param name="buttons">MessageBoxButtons to display.</param>
        /// <param name="icon">MessageBoxIcon to display.</param>
        /// <param name="displayRefresh">Determines whether or not to display refresh dialogue.</param>
        private void DisplayMessage(String message, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, bool displayRefresh)
        {
            DisplayMessage(message, caption, buttons, icon, MessageBoxDefaultButton.Button1, displayRefresh);
        }

        /// <summary>
        /// Causes a message box to be displayed in the main thread. You are encouraged to use this method if you need to display a "final"
        /// message box in a worker thread, which allows the thread to exit rather than sitting around waiting for input.
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <param name="caption">Caption in message box.</param>
        /// <param name="buttons">MessageBoxButtons to display.</param>
        /// <param name="icon">MessageBoxIcon to display.</param>
        /// <param name="defaultButton">MessageBoxDefaultButton to set which button is selected.</param>
        /// <param name="displayRefresh">Determines whether or not to display refresh dialogue.</param>
        private void DisplayMessage(String message, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, bool displayRefresh)
        {
            if (this.Parent == null)
            {
                this.Invoke(dlgDisplayMessage, new object[] { message, caption, buttons, icon, defaultButton, displayRefresh, false });
            }
            else
            {
                this.Invoke(dlgDisplayMessage, new object[] { message, caption, buttons, icon, defaultButton, displayRefresh, true });
            }
        }

        /// <summary>
        /// Displays the message box requested by worker threads (allows worker threads to exit). Message displayed in main thread.
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <param name="caption">Caption in message box.</param>
        /// <param name="buttons">MessageBoxButtons to display.</param>
        /// <param name="icon">MessageBoxIcon to display.</param>
        /// <param name="defaultButton">MessageBoxDefaultButton to set which button is selected.</param>
        /// <param name="displayRefresh">Determines whether or not to display refresh dialogue.</param>
        private void DDisplayMessage(String message, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, bool displayRefresh,
            bool useParent)
        {
            QMessageBox.Show(message, caption, buttons, icon, defaultButton, useParent ? FormStartPosition.CenterParent : FormStartPosition.CenterScreen);
            if (displayRefresh)
            {
                DDisplayRefresh();
            }
        }

        private void DisplayRefresh()
        {
            this.Invoke(dlgDisplayRefresh, new object[] { });
        }

        /// <summary>
        /// Displays the refreshing dialogue.
        /// </summary>
        private void DDisplayRefresh()
        {
            if (isManualRefreshInProgress)
            {
                return;
            }
            else
            {
                ProcessingDialogue pd = new ProcessingDialogue(this.Location, this.Size);
                pd.TopMost = true;
                pd.Show();
                isManualRefreshInProgress = true;
                RefreshDisks();
                LoadList();
                isManualRefreshInProgress = false;
                pd.Hide();
                pd.Close();
                pd.Dispose();
            }
        }
        #endregion

        #region Enable/Disable Auto-Unlock
        private void EnableAutoUnlock(object sender, EventArgs e)
        {
            // Don't start a key management operation if another is pending.
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. Please wait a few seconds " +
                  "and try again.", "Lock Volume", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            String volumeDisplayName = (driveLetter.StartsWith("UL") ? deviceID : driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            if (IsOsVolume(driveLetter))
            {
                newStatus = "Volume " + volumeDisplayName + " cannot be auto-unlocked because it is the OS volume.";
                QMessageBox.Show("BitLocker cannot enable automatic unlocking on volume " + volumeDisplayName + " because it contains " +
                  "the running operating system. You may enable automatic unlocking only on data volumes.", "Enable Auto-Unlock",
                  MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (lvEncryptableVolumes.SelectedItems[0].SubItems[9].Text == "Enabled")
            {
                newStatus = "Automatic unlocking already enabled on volume " + volumeDisplayName + ".";
                QMessageBox.Show("Automatic unlocking is already enabled on volume " + volumeDisplayName + ".", "Disable Auto-Unlock",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Warn the user about automatic unlocking.
            if (QMessageBox.Show("Enabling automatic unlock on a data volume allows the operating system to automatically mount the volume at boot " +
              "time or, in the case of an external device such as a USB or FireWire drive, when the device is connected. The volume will remain " +
              "encrypted, and the automatic unlocking will apply only to this computer. If the drive is a portable one, it will not automatically " +
              "unlock with another computer unless you explicitly configure it to do so on that computer.\n\nDo you want to enable automatic " +
              "unlocking on volume " + volumeDisplayName + " for use with this computer?", "Enable Automatic Unlocking", MessageBoxButtons.YesNo,
              MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Start the enable auto unlock thread.
                keyManagementThread = new Thread(new ParameterizedThreadStart(EnableAutoUnlockWorkerThread));
                keyManagementThread.Name = "Auto-Unlock Thread";
                keyManagementThread.Start(new BitLockerThreadParameters(deviceID));
            }
        }

        private void EnableAutoUnlockWorkerThread(object data)
        {
            // Prevent another key management operation from being started.
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;

            // Get the EncryptableVolume.
            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            // Generate a BEK to use as the auto-unlock key.
            String volumeKeyProtectorID = String.Empty;
            newStatus = "Generating external key...";
            String pcName = System.Environment.MachineName;
            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithExternalKey(null, "AU Key for " + pcName, out volumeKeyProtectorID);
            if (bvms != BitLockerVolumeStatus.S_OK)
            {
                // Failed to generate the key, so we cannot continue.
                newStatus = "External key generation failed.";
                DisplayErrorMessage(bvms, "BitLocker failed to generate the external key required for automatic unlock.", "Auto-Unlock Key Generation Failed",
                      true, true);
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }
            newStatus = "Enabling automatic unlock on volume " + volumeDisplayName + "...";

            // Key gen successful, so enable automatic unlocking.
            bvms = (BitLockerVolumeStatus)volume.EnableAutoUnlock(volumeKeyProtectorID);
            switch (bvms)
            {
                case BitLockerVolumeStatus.S_OK:
                    {
                        // Auto-unlock successfully enabled.
                        newStatus = "Automatic unlocking of volume " + volumeDisplayName + " is now enabled.";
                        DisplayMessage("Automatic unlocking of volume " + volumeDisplayName + " is now enabled.",
                          "Enable Auto-Unlock", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                default:
                    {
                        // Auto-unlock failed.
                        newStatus = "Volume " + volumeDisplayName + " could not be auto-unlocked.";

                        // Attempt to remove the BEK and capture error details if failed.
                        BitLockerVolumeStatus bvms2 = (BitLockerVolumeStatus)volume.DeleteKeyProtector(volumeKeyProtectorID);
                        String errorMessage2, errorSymbolic2, errorCode2;
                        errorMessage2 = ErrorInfo.GetErrorMessage(bvms2, out errorSymbolic2, out errorCode2);

                        if (bvms2 == BitLockerVolumeStatus.S_OK)
                        {
                            DisplayErrorMessage(bvms, "BitLocker added an External Key Protector to volume " + volumeDisplayName + ", but " +
                                "was unable to turn on automatic unlocking for the volume. The External Key Protector has been " +
                                "removed from the volume.", "Enable Auto-Unlock Failed", true, true);
                        }
                        else
                        {
                            DisplayErrorMessage(bvms, "BitLocker added an External Key Protector to volume " + volumeDisplayName + ", but " +
                                "was unable to turn on automatic unlocking for the volume. The subsequent attempt to remove the exteral " +
                                "key protector failed with message \"" + errorMessage2 + "\" (" + errorCode2 + " " + errorSymbolic2 + ")",
                                "Enable Auto-Unlock Failed", true, true);
                        }
                        break;
                    }
            }

            // Dispose the volume and release the key management flag.
            volume.Dispose();
            keyManagementRunning = false;
        }

        private void DisableAutoUnlock(object sender, EventArgs e)
        {
            // Don't start a key management operation if another is pending.
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. Please wait a few seconds " +
                  "and try again.", "Lock Volume", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            String volumeDisplayName = (driveLetter.StartsWith("UL") ? deviceID : driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            if (IsOsVolume(driveLetter))
            {
                newStatus = "Volume " + volumeDisplayName + " cannot be auto-unlocked because it is the OS volume.";
                QMessageBox.Show("BitLocker cannot disable automatic unlocking on volume " + volumeDisplayName + " because it contains " +
                  "the running operating system. You may disable automatic unlocking only on data volumes.", "Disable Auto-Unlock",
                  MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (lvEncryptableVolumes.SelectedItems[0].SubItems[9].Text == "Disabled")
            {
                newStatus = "Automatic unlocking is not enabled on volume " + volumeDisplayName + ".";
                QMessageBox.Show("Automatic unlocking is already disabled on volume " + volumeDisplayName + ".", "Disable Auto-Unlock",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Warn the user about disabling auto unlock.
            if (QMessageBox.Show("Disabling automatic unlock on a data volume prevents the operating system from automatically mounting the volume at boot " +
              "time or, in the case of an external device such as a USB or FireWire drive, when the device is connected. In order to use the volume, " +
              "you will need to unlock it using one of the other key protectors remaining on your volume. " +
              "Disabling automatic unlocking also removes the automatic unlock key protector from the volume.\n\n" +
              "CAUTION: If you disable automatic unlocking on a volume that contains applications or services that start at boot time, " +
              "those services will fail to start, and may result in an unbootable system.\n\nDo you want to disable automatic " +
              "unlocking on volume " + volumeDisplayName + "?", "Disable Auto-Unlock", MessageBoxButtons.YesNo,
              MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Start the auto-unlock thread.
                keyManagementThread = new Thread(new ParameterizedThreadStart(DisableAutoUnlockWorkerThread));
                keyManagementThread.Name = "Auto-Unlock Thread";
                keyManagementThread.Start(new BitLockerThreadParameters(deviceID));
            }
        }

        private void DisableAutoUnlockWorkerThread(object data)
        {
            // Prevent another key management operation from being started.
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;

            // Get the EncryptableVolume.
            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            // Disable auto-unlock.
            String protectorId = String.Empty;
            bool aaEnabled = false;

            newStatus = "Disabling automatic unlock on volume " + volumeDisplayName + "...";
            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.IsAutoUnlockEnabled(out aaEnabled, out protectorId);
            if (bvms == BitLockerVolumeStatus.S_OK)
            {
                bvms = (BitLockerVolumeStatus)volume.DisableAutoUnlock();
            }
            else
            {
                DisplayErrorMessage(bvms, "The automatic unlocking status of volume " + volumeDisplayName + " coult not be ascertained.",
                    "Auto-Unlock Key Protector", true, true);
            }

            switch (bvms)
            {
                case BitLockerVolumeStatus.S_OK:
                    {
                        // Auto-unlock successfully disabled.
                        bvms = (BitLockerVolumeStatus)volume.DeleteKeyProtector(protectorId);
                        newStatus = "Automatic unlocking of volume " + volumeDisplayName + " is now disabled.";
                        if (bvms == BitLockerVolumeStatus.S_OK)
                        {
                            DisplayMessage("Automatic unlocking of volume " + volumeDisplayName + " is now disabled.",
                                "Disable Auto-Unlock", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            DisplayMessage("Automatic unlocking of volume " + volumeDisplayName + " is now disabled, but " +
                                "an error occurred removing the external key protector. It will need to be deleted manually.",
                                "Disable Auto-Unlock Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }

                        break;
                    }
                default:
                    {
                        // Disable failed. Unexpected error.
                        newStatus = "Automatic unlocking of volume " + volumeDisplayName + " could not be disabled.";
                        DisplayErrorMessage(bvms, "Automatic unlocking of volume " + volumeDisplayName + " could not be disabled.",
                            "Disable Auto-Unlock", true, true);
                        break;
                    }
            }

            // Dispose the volume and release the key management flag.
            volume.Dispose();
            keyManagementRunning = false;
        }
        #endregion

        #region Add/Delete Key Protectors
        private void AddExternalKeyProtector(object sender, EventArgs e)
        {
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. A dialogue may be open. " +
                  "Please wait a few seconds, and also be sure any key management dialogues you have open are closed, " +
                  "and try again.", "Add Numeric Password Protector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            String volumeDisplayName = (driveLetter.StartsWith("UL") ? deviceID : driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            String path = String.Empty;
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            bool waitForUser = true;

            while (waitForUser)
            {
                fbd.Description = "Select folder in which to save external key.";
                fbd.ShowDialog();
                if (fbd.SelectedPath != String.Empty)
                {
                    path = fbd.SelectedPath;

                    try
                    {
                        System.IO.FileStream fs = System.IO.File.Create(path + "\\Tarynblmtemp65535.txt");
                        fs.Flush();
                        fs.Close();
                        System.IO.File.Delete(path + "\\Tarynblmtemp65535.txt");
                        waitForUser = false;
                    }
                    catch
                    {
                        waitForUser = true;
                        QMessageBox.Show("Path " + path + " cannot be used. Home Server SMART attempted to write a temporary " +
                            "file to this location (prior to External Key Protector generation) to validate your ability to write " +
                            "a file to this location. That attempt failed. Please choose another location.", "Path Not Available",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    waitForUser = false;
                    return;
                }
            }

            if (QMessageBox.Show("Do you want to add an External Key (BEK) Protector to volume " + volumeDisplayName + " and save " +
              "the BEK to " + path + "?", "Add External Key Protector",
              MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                keyManagementThread = new Thread(new ParameterizedThreadStart(AddExternalKeyProtectorWorkerThread));
                keyManagementThread.Name = "External Key Thread";
                // The thread uses the SaveFileDialog, which raises an exception in a multithreaded apartment, so
                // we set to single-threaded apartment (STA).
                keyManagementThread.SetApartmentState(ApartmentState.STA);
                keyManagementThread.Start(driveLetter + path);
            }
        }

        private void AddExternalKeyProtectorWorkerThread(object data)
        {
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;
            String path = input.Argument;

            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            newStatus = "Generating external key...";
            String volumeKeyProtectorID = String.Empty;
            String fileName = String.Empty;
            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithExternalKey(null, GenerateFriendlyKeyName("BEK"), out volumeKeyProtectorID);

            if (bvms == BitLockerVolumeStatus.S_OK)
            {
                bvms = (BitLockerVolumeStatus)volume.GetExternalKeyFileName(volumeKeyProtectorID, out fileName);
                newStatus = "Saving external key...";
                bvms = (BitLockerVolumeStatus)volume.SaveExternalKeyToFile(path, volumeKeyProtectorID);

                if (bvms == BitLockerVolumeStatus.S_OK)
                {
                    newStatus = "External Key Protector generation successful for volume " + volumeDisplayName + ".";
                    sb.Append("Added Key Protector: Recovery Key (BEK)");
                    sb.Append("\r\n");
                    sb.Append("Key File Name: " + fileName);
                    sb.Append("\r\n");
                    sb.Append("Save Location: " + path);
                    sb.Append("\r\n");
                    sb.Append("Key File ID : " + volumeKeyProtectorID);
                    sb.Append("\r\n\r\n");
                    sb.Append("Security is everyone's responsibility. Thank you for encrypting your volume.");

                    String myString = sb.ToString();

                    DisplayResultWindow(myString, "External Key Protector was successfully generated. " +
                      "Be sure to save the below information to a safe location.");
                }
                else
                {
                    newStatus = "External Key Protector on volume " + volumeDisplayName + " failed to read back or save.";
                    DisplayErrorMessage(bvms, "BitLocker generated an External Key Protector on volume " + volumeDisplayName + ", but the " +
                        "attempt by BitLocker to read back the key or save it to disk failed. Select View from the Key Protectors menu, and find " +
                        "the EXTERNAL_KEY protector with ID " + volumeKeyProtectorID + ". You will be able to save the BEK file to a " +
                        "location of your choice.", "Warning", false, true);
                }
            }
            else
            {
                newStatus = "External Key Protector generation on volume " + volumeDisplayName + " failed.";
                DisplayErrorMessage(bvms, "BitLocker failed to generate an External Key Protector on volume " + volumeDisplayName + ".", "BEK Generation Failed",
                    true, true);
            }

            volume.Dispose();
            keyManagementRunning = false;
        }

        private void RemoveExternalKeyProtector(object sender, EventArgs e)
        {
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. A dialogue may be open. " +
                  "Please wait a few seconds, and also be sure any key management dialogues you have open are closed, " +
                  "and try again.", "Add Numeric Password Protector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            keyManagementThread = new Thread(new ParameterizedThreadStart(RemoveExternalKeyProtectorWorkerThread));
            keyManagementThread.Name = "External Key Thread";
            keyManagementThread.Start(new BitLockerThreadParameters(deviceID));
        }

        private void RemoveExternalKeyProtectorWorkerThread(object data)
        {
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;

            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            String[] externalKeyProtectors = null;

            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.GetKeyProtectors((uint)KeyProtectorTypes.EXTERNAL_KEY, out externalKeyProtectors);
            if (bvms != BitLockerVolumeStatus.S_OK)
            {
                newStatus = "Failed to access appropriate Key Protectors.";
                DisplayErrorMessage(bvms, "Unable to access External Key Protectors on volume " + volumeDisplayName + ".", "Failed to Get Key Protectors",
                    true, true);
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            if (externalKeyProtectors == null)
            {
                newStatus = "No External Key Protectors were detected.";
                DisplayMessage("Volume " + volumeDisplayName + " does not possess any External Key Protectors.", "Remove External Key Protector",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            ArrayList listOfKeys = new ArrayList();

            newStatus = "Getting External Key Protectors...";
            foreach (String key in externalKeyProtectors)
            {
                String friendlyName = String.Empty;
                bvms = (BitLockerVolumeStatus)volume.GetKeyProtectorFriendlyName(key, out friendlyName);
                KeyProtectorInfo kpi = new KeyProtectorInfo(KeyProtectorTypes.EXTERNAL_KEY, key, friendlyName);
                String fileName = String.Empty;
                bvms = (BitLockerVolumeStatus)volume.GetExternalKeyFileName(key, out fileName);
                kpi.BekFileName = fileName;
                listOfKeys.Add(kpi);
            }
            newStatus = "External Key Protectors harvested.";

            KeyProtectorDialogue kpd = new KeyProtectorDialogue(listOfKeys, "The following External Key Protector(s) are defined on " +
              "volume " + volumeDisplayName + ". Delete the one you no longer want.", "File Name", false, false);
            kpd.ShowDialog();

            if (kpd.DialogResult == DialogResult.OK)
            {
                if (QMessageBox.Show("Deleting an External Key Protector removes it from the volume. The BEK file associated with it, including " +
                  "any copies of the BEK you may have made, will become permanently invalid. You will not be able to unlock the volume using " +
                  "this External Key Protector if you delete it.\n\nAre you sure you want to permanently delete the External Key Protector with " +
                  "ID " + kpd.SelectedKey + "?", "Remove External Key Protector", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    newStatus = "Obliterating External Key Protector " + kpd.SelectedKey + "...";
                    bvms = (BitLockerVolumeStatus)volume.DeleteKeyProtector(kpd.SelectedKey);

                    switch (bvms)
                    {
                        case BitLockerVolumeStatus.S_OK:
                            {
                                newStatus = "External Key Protector " + kpd.SelectedKey + " has been obliterated.";
                                DisplayMessage("External Key Protector " + kpd.SelectedKey + " was deleted successfully. You may safely delete " +
                                  "any copies of BEK files associated with that protector.", "Remove External Key Protector", MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                                break;
                            }
                        default:
                            {
                                newStatus = "Unable to delete External Key Protector.";
                                DisplayErrorMessage(bvms, "BitLocker cannot delete the External Key Protector.", "BEK Deletion Failed",
                                    true, true);
                                break;
                            }
                    }
                }
            }

            volume.Dispose();
            keyManagementRunning = false;
        }

        private void ChangeExternalKeyProtector(object sender, EventArgs e)
        {
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. A dialogue may be open. " +
                  "Please wait a few seconds, and also be sure any key management dialogues you have open are closed, " +
                  "and try again.", "Add Numeric Password Protector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            String path = String.Empty;
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            bool waitForUser = true;

            while (waitForUser)
            {
                fbd.Description = "Select folder in which to save external key.";
                fbd.ShowDialog();
                if (fbd.SelectedPath != String.Empty)
                {
                    path = fbd.SelectedPath;

                    try
                    {
                        System.IO.FileStream fs = System.IO.File.Create(path + "\\Tarynblmtemp65535.txt");
                        fs.Flush();
                        fs.Close();
                        System.IO.File.Delete(path + "\\Tarynblmtemp65535.txt");
                        waitForUser = false;
                    }
                    catch
                    {
                        waitForUser = true;
                        QMessageBox.Show("Path " + path + " cannot be used. Home Server SMART attempted to write a temporary " +
                            "file to this location (prior to External Key Protector generation) to validate your ability to write " +
                            "a file to this location. That attempt failed. Please choose another location.", "Path Not Available",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    waitForUser = false;
                    return;
                }
            }

            keyManagementThread = new Thread(new ParameterizedThreadStart(ChangeExternalKeyProtectorWorkerThread));
            keyManagementThread.Name = "External Key Thread";
            // The thread uses the SaveFileDialog, which raises an exception in a multithreaded apartment, so
            // we set to single-threaded apartment (STA).
            keyManagementThread.SetApartmentState(ApartmentState.STA);
            keyManagementThread.Start(driveLetter + path);
        }

        private void ChangeExternalKeyProtectorWorkerThread(object data)
        {
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;
            String path = input.Argument;
            String newProtectorId = String.Empty;

            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            String[] externalKeyProtectors = null;

            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.GetKeyProtectors((uint)KeyProtectorTypes.EXTERNAL_KEY, out externalKeyProtectors);
            if (bvms != BitLockerVolumeStatus.S_OK)
            {
                newStatus = "Failed to access appropriate Key Protectors.";
                DisplayErrorMessage(bvms, "Unable to access External Key Protectors on volume " + volumeDisplayName + ".", "Failed to Get Key Protectors",
                    true, true);
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            if (externalKeyProtectors == null)
            {
                newStatus = "No External Key Protectors were detected.";
                DisplayMessage("Volume " + volumeDisplayName + " does not possess any External Key Protectors.", "Replace External Key Protector",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            ArrayList listOfKeys = new ArrayList();

            newStatus = "Getting External Key Protectors...";
            foreach (String key in externalKeyProtectors)
            {
                String friendlyName = String.Empty;
                bvms = (BitLockerVolumeStatus)volume.GetKeyProtectorFriendlyName(key, out friendlyName);
                KeyProtectorInfo kpi = new KeyProtectorInfo(KeyProtectorTypes.EXTERNAL_KEY, key, friendlyName);
                String fileName = String.Empty;
                bvms = (BitLockerVolumeStatus)volume.GetExternalKeyFileName(key, out fileName);
                kpi.BekFileName = fileName;
                listOfKeys.Add(kpi);
            }
            newStatus = "External Key Protectors harvested.";

            KeyProtectorDialogue kpd = new KeyProtectorDialogue(listOfKeys, "The following External Key Protector(s) are defined on " +
              "volume " + volumeDisplayName + ". Select the one you want to replace.", "File Name", true, false);
            kpd.ShowDialog();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (kpd.DialogResult == DialogResult.OK)
            {
                if (QMessageBox.Show("Replacing an External Key Protector removes the old one from the volume. The BEK file associated with it, including " +
                  "any copies of the BEK you may have made, will become permanently invalid. You will not be able to unlock the volume using " +
                  "the replaced External Key Protector if you replace it.\n\nAre you sure you want to replace the External Key Protector with " +
                  "ID " + kpd.SelectedKey + "?", "Replace External Key Protector", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    newStatus = "Replacing External Key Protector " + kpd.SelectedKey + "...";

                    bvms = (BitLockerVolumeStatus)volume.ChangeExternalKey(kpd.SelectedKey, null, out newProtectorId);
                    if (bvms == BitLockerVolumeStatus.S_OK)
                    {
                        String fileName = String.Empty;
                        bvms = (BitLockerVolumeStatus)volume.GetExternalKeyFileName(newProtectorId, out fileName);
                        newStatus = "Saving external key...";
                        bvms = (BitLockerVolumeStatus)volume.SaveExternalKeyToFile(path, newProtectorId);

                        if (bvms == BitLockerVolumeStatus.S_OK)
                        {
                            newStatus = "External Key Protector replacement successful for volume " + volumeDisplayName + ".";
                            sb.Append("Added Key Protector: Recovery Key (BEK)");
                            sb.Append("\r\n");
                            sb.Append("Key File Name: " + fileName);
                            sb.Append("\r\n");
                            sb.Append("Save Location: " + path);
                            sb.Append("\r\n");
                            sb.Append("Key File ID  : " + newProtectorId);
                            sb.Append("\r\n\r\n");
                            sb.Append("Replaced (Deleted) Key Protector: Recovery Key (BEK)");
                            sb.Append("\r\n");
                            sb.Append("Key File ID  : " + kpd.SelectedKey);
                            sb.Append("\r\n\r\n");
                            sb.Append("Security is everyone's responsibility. Thank you for encrypting your volume.");

                            String myString = sb.ToString();

                            DisplayResultWindow(myString, "External Key Protector was successfully replaced. " +
                              "Be sure to save the below information to a safe location.");
                        }
                        else
                        {
                            newStatus = "External Key Protector on volume " + volumeDisplayName + " failed to read back or save.";
                            DisplayErrorMessage(bvms, "BitLocker replaced an External Key Protector on volume " + volumeDisplayName + ", but the " +
                                "attempt by BitLocker to read back the new key or save it to disk failed. Select View from the Key Protectors menu, and find " +
                                "the EXTERNAL_KEY protector with ID " + newProtectorId + ". You will be able to save the BEK file to a " +
                                "location of your choice.", "Warning", false, true);
                        }
                    }
                    else
                    {
                        newStatus = "External Key Protector replacement on volume " + volumeDisplayName + " failed.";
                        DisplayErrorMessage(bvms, "BitLocker failed to replace an External Key Protector on volume " + volumeDisplayName + ".", "BEK Replacement Failed",
                            true, true);
                    }
                }
            }

            volume.Dispose();
            keyManagementRunning = false;
        }

        private void AddNumericPasswordProtector(object sender, EventArgs e)
        {
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. A dialogue may be open. " +
                  "Please wait a few seconds, and also be sure any key management dialogues you have open are closed, " +
                  "and try again.", "Add Numeric Password Protector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            String volumeDisplayName = (driveLetter.StartsWith("UL") ? deviceID : driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            if (QMessageBox.Show("Do you want to generate and add a 48-digit Numeric Password Protector to volume " + volumeDisplayName + "?",
              "Add Numeric Password Protector", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                keyManagementThread = new Thread(new ParameterizedThreadStart(AddNumericPasswordProtectorWorkerThread));
                keyManagementThread.Name = "Password Key Thread";
                // The thread uses the SaveFileDialog, which raises an exception in a multithreaded apartment, so
                // we set to single-threaded apartment (STA).
                keyManagementThread.SetApartmentState(ApartmentState.STA);
                keyManagementThread.Start(new BitLockerThreadParameters(deviceID));
            }
        }

        private void AddNumericPasswordProtectorWorkerThread(object data)
        {
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;

            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            newStatus = "Generating numerical password...";
            String volumePasswordProtectorID = String.Empty;
            String password = String.Empty;
            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithNumericalPassword(GenerateFriendlyKeyName("PW"), null, out volumePasswordProtectorID);

            if (bvms != BitLockerVolumeStatus.S_OK)
            {
                newStatus = "Numeric Password Protector generation on volume " + volumeDisplayName + " failed.";
                DisplayErrorMessage(bvms, "BitLocker failed to generate an 48-Digit Numeric Password Protector on volume " + volumeDisplayName + ".",
                    "Numeric Password Generation Failed", true, true);
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            bvms = (BitLockerVolumeStatus)volume.GetKeyProtectorNumericalPassword(volumePasswordProtectorID, out password);

            if (bvms == BitLockerVolumeStatus.S_OK)
            {
                newStatus = "Numeric Password Protector generation successful for volume " + volumeDisplayName + ".";
                sb.Append("Added Key Protector: Numerical Password");
                sb.Append("\r\n");
                sb.Append("Password   : " + password);
                sb.Append("\r\n");
                sb.Append("Password ID : " + volumePasswordProtectorID);
                sb.Append("\r\n\r\n");
                sb.Append("Security is everyone's responsibility. Thank you for encrypting your volume.");

                String myString = sb.ToString();

                DisplayResultWindow(myString, "Numeric Password Protector was successfully generated. " +
                  "Be sure to save the below information to a safe location.");
            }
            else
            {
                newStatus = "Numeric Password Protector on volume " + volumeDisplayName + " failed to read back.";
                DisplayErrorMessage(bvms, "BitLocker generated a Numeric Password Protector on volume " + volumeDisplayName + ", but the " +
                    "attempt by BitLocker to read back the password failed. Select View from the Key Protectors menu, and find " +
                    "the NUMERIC_PASSWORD protector with ID " + volumePasswordProtectorID + ". You will be able to make a note " +
                    "of the password and, if you would like to, save it to a text file.", "Warning", false, true);
            }

            volume.Dispose();
            keyManagementRunning = false;
        }

        private void RemovePasswordProtector(object sender, EventArgs e)
        {
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. A dialogue may be open. " +
                  "Please wait a few seconds, and also be sure any key management dialogues you have open are closed, " +
                  "and try again.", "Add Numeric Password Protector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            keyManagementThread = new Thread(new ParameterizedThreadStart(RemovePasswordProtectorWorkerThread));
            keyManagementThread.Name = "Password Key Thread";
            keyManagementThread.Start(new BitLockerThreadParameters(deviceID));
        }

        private void RemovePasswordProtectorWorkerThread(object data)
        {
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;

            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            String[] passwordProtectors = null;

            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.GetKeyProtectors((uint)KeyProtectorTypes.NUMERIC_PASSWORD, out passwordProtectors);
            if (bvms != BitLockerVolumeStatus.S_OK)
            {
                newStatus = "Failed to access appropriate Key Protectors.";
                DisplayErrorMessage(bvms, "Unable to access Numeric Password Protectors on volume " + volumeDisplayName + ".", "Failed to Get Key Protectors",
                    true, true);
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            if (passwordProtectors == null)
            {
                newStatus = "No Numeric Key Protectors were detected.";
                DisplayMessage("Volume " + volumeDisplayName + " does not possess any Numeric Password Protectors.", "Remove Numeric Password Protector",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            ArrayList listOfKeys = new ArrayList();

            newStatus = "Getting Numeric Password Protectors...";
            foreach (String key in passwordProtectors)
            {
                String friendlyName = String.Empty;
                bvms = (BitLockerVolumeStatus)volume.GetKeyProtectorFriendlyName(key, out friendlyName);
                KeyProtectorInfo kpi = new KeyProtectorInfo(KeyProtectorTypes.EXTERNAL_KEY, key, friendlyName);
                String password = String.Empty;
                bvms = (BitLockerVolumeStatus)volume.GetKeyProtectorNumericalPassword(key, out password);
                kpi.Password = password;
                listOfKeys.Add(kpi);
            }
            newStatus = "Numeric Password Protectors harvested.";

            KeyProtectorDialogue kpd = new KeyProtectorDialogue(listOfKeys, "The following Numeric Password Protector(s) are defined on " +
              "volume " + volumeDisplayName + ". Delete the one you no longer want.", "48-Digit Password", false, false);
            kpd.ShowDialog();

            if (kpd.DialogResult == DialogResult.OK)
            {
                if (QMessageBox.Show("Deleting a Numeric Password Protector removes it from the volume. The 48-digit password associated with it " +
                  "will become permanently invalid. You will not be able to unlock the volume using this Numeric Password Protector " +
                  "if you delete it.\n\nAre you sure you want to permanently delete the Numeric Password Protector with " +
                  "ID " + kpd.SelectedKey + "?", "Remove Numeric Password Protector", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    newStatus = "Obliterating Numeric Password Protector " + kpd.SelectedKey + "...";
                    bvms = (BitLockerVolumeStatus)volume.DeleteKeyProtector(kpd.SelectedKey);

                    switch (bvms)
                    {
                        case BitLockerVolumeStatus.S_OK:
                            {
                                newStatus = "Numeric Password Protector " + kpd.SelectedKey + " has been obliterated.";
                                DisplayMessage("Numeric Password Protector " + kpd.SelectedKey + " was deleted successfully.",
                                  "Remove Numeric Password Protector", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        default:
                            {
                                newStatus = "Unable to delete Numeric Password Protector.";
                                DisplayErrorMessage(bvms, "BitLocker cannot delete the Numeric Password Protector.", "Password Deletion Failed",
                                    true, true);
                                break;
                            }
                    }
                }
            }

            volume.Dispose();
            keyManagementRunning = false;
        }

        private void AddPassphraseProtector(object sender, EventArgs e)
        {
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. A dialogue may be open. " +
                  "Please wait a few seconds, and also be sure any key management dialogues you have open are closed, " +
                  "and try again.", "Add Volume Password", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            String volumeType = String.Empty;
            String driveLetter = String.Empty;
            String deviceID = GetDeviceIdFromLvi(out driveLetter, out volumeType);
            String minimumPassphrase = String.Empty;
            if (volumeType == "System")
            {
                newStatus = "You cannot add a password to the operating system volume.";
                QMessageBox.Show("You cannot add a Volume Password Protector to the operating system volume.", "Add Volume Password",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (volumeType == "Fixed Data")
            {
                minimumPassphrase = GPConfig.Win7FdvMinimumPassphrase.ToString();
            }
            else // Roaming Data
            {
                minimumPassphrase = GPConfig.Win7RdvMinimumPassphrase.ToString();
            }

            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            keyManagementThread = new Thread(new ParameterizedThreadStart(AddPassphraseProtectorWorkerThread));
            keyManagementThread.Name = "Passphrase Key Thread";
            // The thread uses the SaveFileDialog, which raises an exception in a multithreaded apartment, so
            // we set to single-threaded apartment (STA).
            keyManagementThread.SetApartmentState(ApartmentState.STA);
            keyManagementThread.Start(new BitLockerThreadParameters(deviceID, minimumPassphrase));
        }

        private void AddPassphraseProtectorWorkerThread(object data)
        {
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;
            String minimum = input.Argument;
            int minimumPassphrase = 8;

            try
            {
                minimumPassphrase = Int32.Parse(minimum);
            }
            catch
            {
                minimumPassphrase = 8;
            }

            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            String keyProtectorId = String.Empty;
            if (DoesSpecifiedProtectorAlreadyExist(volume, out keyProtectorId, KeyProtectorTypes.PASSPHRASE))
            {
                newStatus = "A Password protector already exists.";
                QMessageBox.Show("You cannot add a Volume Password Protector to volume " + volumeDisplayName +
                    " because a Volume Password Protector already exists. Only one Password is allowed per volume. " +
                    "You can either change the existing Password, or delete it and then add a new one.",
                    "Password Already Exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                keyManagementRunning = false;
                return;
            }
            else
            {
                newStatus = "No existing Password protector detected; okay to proceed.";
            }

            String passphrase = String.Empty;

            AddPassphraseDialogue ap = new AddPassphraseDialogue(minimumPassphrase, false);
            ap.ShowDialog();
            if (ap.DialogResult == DialogResult.OK)
            {
                passphrase = ap.Passphrase;
            }
            else
            {
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            newStatus = "Adding passphrase...";
            String volumePasswordProtectorID = String.Empty;
            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithPassphrase(GenerateFriendlyKeyName("PASS"), passphrase, out volumePasswordProtectorID);

            if (bvms != BitLockerVolumeStatus.S_OK)
            {
                newStatus = "Volume Password Protector generation on volume " + volumeDisplayName + " failed.";
                DisplayErrorMessage(bvms, "BitLocker failed to generate a Volume Password (Passphrase) Protector on volume " + volumeDisplayName + ".",
                    "Passphrase Generation Failed", true, true);
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            newStatus = "Volume Password (Passphrase) Protector generation successful for volume " + volumeDisplayName + ".";
            sb.Append("Added Key Protector: Passphrase");
            sb.Append("\r\n");
            sb.Append("Password   : " + passphrase);
            sb.Append("\r\n");
            sb.Append("Password ID: " + volumePasswordProtectorID);
            sb.Append("\r\n\r\n");
            sb.Append("Security is everyone's responsibility. Thank you for encrypting your volume.");

            String myString = sb.ToString();

            DisplayResultWindow(myString, "Passphrase Protector was successfully generated. " +
              "Be sure to save the below information to a safe location.");

            volume.Dispose();
            keyManagementRunning = false;
        }

        /// <summary>
        /// Reports if a volume possesses a Password (passphrase) Key Protector.  Only one is allowed per volume.
        /// </summary>
        /// <param name="volume">EncryptableVolume to check.</param>
        /// <param name="keyProtectorId">Key protector ID if a desired protector is found.</param>
        /// <param name="protectorType">The protector type to check against.</param>
        /// <returns>true if the volume has a password; false otherwise.</returns>
        private bool DoesSpecifiedProtectorAlreadyExist(EncryptableVolume volume, out String keyProtectorId,
            KeyProtectorTypes protectorType)
        {
            return DoesSpecifiedProtectorAlreadyExist(volume, out keyProtectorId, protectorType, false);
        }

        /// <summary>
        /// Reports if a volume possesses a Password (passphrase) Key Protector.  Only one is allowed per volume.
        /// </summary>
        /// <param name="volume">EncryptableVolume to check.</param>
        /// <param name="keyProtectorId">Key protector ID if a desired protector is found.</param>
        /// <param name="protectorType">The protector type to check against.</param>
        /// <param name="returnPartialKey">Set to true to return only the first part of the protector's ID.</param>
        /// <returns>true if the volume has a password; false otherwise.</returns>
        private bool DoesSpecifiedProtectorAlreadyExist(EncryptableVolume volume, out String keyProtectorId,
            KeyProtectorTypes protectorType, bool returnPartialKey)
        {
            newStatus = "Checking All Key Protectors...";
            String[] allProtectors = null;
            keyProtectorId = String.Empty;

            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.GetKeyProtectors((uint)KeyProtectorTypes.ALL_TYPES, out allProtectors);
            if (bvms != BitLockerVolumeStatus.S_OK)
            {
                // Couldn't get the protectors, so we'll just assume the protector doesn't exist.
                return false;
            }

            if (allProtectors == null)
            {
                // No protectors, so we know it doesn't exist!
                return false;
            }

            foreach (String key in allProtectors)
            {
                String friendlyName = String.Empty;
                uint keyProtectorType = 0;
                bvms = (BitLockerVolumeStatus)volume.GetKeyProtectorType(key, out keyProtectorType);
                KeyProtectorInfo kpi = new KeyProtectorInfo((KeyProtectorTypes)keyProtectorType, key, friendlyName);

                if (kpi.Type == protectorType)
                {
                    keyProtectorId = returnPartialKey ? key.Substring(1, key.IndexOf('-') - 1) : key;
                    return true;
                }
            }
            return false;
        }

        private void RemovePassphraseProtector(object sender, EventArgs e)
        {
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. A dialogue may be open. " +
                  "Please wait a few seconds, and also be sure any key management dialogues you have open are closed, " +
                  "and try again.", "Delete Volume Password", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            keyManagementThread = new Thread(new ParameterizedThreadStart(RemovePassphraseProtectorWorkerThread));
            keyManagementThread.Name = "Passphrase Key Thread";
            keyManagementThread.Start(new BitLockerThreadParameters(deviceID));
        }

        private void RemovePassphraseProtectorWorkerThread(object data)
        {
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;

            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            String passwordKeyId = String.Empty;
            if (!DoesSpecifiedProtectorAlreadyExist(volume, out passwordKeyId, KeyProtectorTypes.PASSPHRASE))
            {
                newStatus = "Volume Password not detected.";
                QMessageBox.Show("Volume " + volumeDisplayName + " does not have a Volume Password Protector.",
                    "Delete Volume Password", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                keyManagementRunning = false;
                return;
            }
            else
            {
                newStatus = "Password protector detected; okay to proceed.";
            }

            if (QMessageBox.Show("Deleting a Password Key Protector removes it from the volume. You will not be able to " +
                  "unlock the volume using this Password if you delete it.\n\nAre you sure you want to permanently delete " +
                  "the Volume Password Protector with ID " + passwordKeyId + "?", "Remove Volume Password Protector",
                  MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                newStatus = "Obliterating Volume Password Protector " + passwordKeyId + "...";
                BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.DeleteKeyProtector(passwordKeyId);

                switch (bvms)
                {
                    case BitLockerVolumeStatus.S_OK:
                        {
                            newStatus = "Volume Password Protector " + passwordKeyId + " has been obliterated.";
                            DisplayMessage("Volume Password Protector " + passwordKeyId + " was deleted successfully.",
                              "Remove Volume Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                    default:
                        {
                            newStatus = "Unable to delete Volume Password Protector.";
                            DisplayErrorMessage(bvms, "BitLocker cannot delete the Volume Password.", "Password Deletion Failed",
                                true, true);
                            break;
                        }
                }
            }

            volume.Dispose();
            keyManagementRunning = false;
        }

        private void ChangePassphraseProtector(object sender, EventArgs e)
        {
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. A dialogue may be open. " +
                  "Please wait a few seconds, and also be sure any key management dialogues you have open are closed, " +
                  "and try again.", "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            String volumeType = String.Empty;
            String driveLetter = String.Empty;
            String deviceID = GetDeviceIdFromLvi(out driveLetter, out volumeType);
            String minimumPassphrase = String.Empty;
            if (volumeType == "System")
            {
                newStatus = "You cannot add a password to the operating system volume.";
                QMessageBox.Show("You cannot add a Volume Password Protector to the operating system volume.", "Change Volume Password",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (volumeType == "Fixed Data")
            {
                minimumPassphrase = GPConfig.Win7FdvMinimumPassphrase.ToString();
            }
            else // Roaming Data
            {
                minimumPassphrase = GPConfig.Win7RdvMinimumPassphrase.ToString();
            }

            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            keyManagementThread = new Thread(new ParameterizedThreadStart(ChangePassphraseProtectorWorkerThread));
            keyManagementThread.Name = "Passphrase Key Thread";
            keyManagementThread.Start(new BitLockerThreadParameters(driveLetter, minimumPassphrase));
        }

        private void ChangePassphraseProtectorWorkerThread(object data)
        {
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;
            String minimum = input.Argument;
            int minimumPassphrase = 8;

            try
            {
                minimumPassphrase = Int32.Parse(minimum);
            }
            catch
            {
                minimumPassphrase = 8;
            }

            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            String passwordKeyId = String.Empty;
            if (!DoesSpecifiedProtectorAlreadyExist(volume, out passwordKeyId, KeyProtectorTypes.PASSPHRASE))
            {
                newStatus = "No Password protector detected.";
                QMessageBox.Show("Volume " + volumeDisplayName + " does not have a Volume Password Protector.",
                    "Change Volume Password", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                keyManagementRunning = false;
                return;
            }

            newStatus = "Password protector detected; okay to proceed.";
            if (QMessageBox.Show("Do you want to change the Volume Password?", "Change Volume Password",
                  MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                newStatus = "Waiting for user...";

                String passphrase = String.Empty;

                AddPassphraseDialogue ap = new AddPassphraseDialogue(minimumPassphrase, true);
                ap.ShowDialog();
                if (ap.DialogResult == DialogResult.OK)
                {
                    passphrase = ap.Passphrase;
                }
                else
                {
                    newStatus = "Password change cancelled by user.";
                    volume.Dispose();
                    keyManagementRunning = false;
                    return;
                }

                String output;
                BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.ChangePassPhrase(passwordKeyId, passphrase, out output);

                switch (bvms)
                {
                    case BitLockerVolumeStatus.S_OK:
                        {
                            newStatus = "Volume Password changed successfully.";
                            DisplayMessage("The Password for volume " + volumeDisplayName + " was changed successfully. A " +
                                "new Key Protector ID " + output + " was assigned.",
                              "Change Volume Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                    default:
                        {
                            newStatus = "Unable to change Volume Password.";
                            DisplayErrorMessage(bvms, "BitLocker cannot change the Volume Password.", "Password Change Failed",
                                true, true);
                            break;
                        }
                }
            }

            volume.Dispose();
            keyManagementRunning = false;
        }

        private String GenerateFriendlyKeyName(String prefix)
        {
            System.Text.StringBuilder friendlyName = new System.Text.StringBuilder();

            friendlyName.Append(prefix);
            friendlyName.Append(" ");

            if (DateTime.Now.Date.Month < 10)
            {
                friendlyName.Append("0" + DateTime.Now.Date.Month.ToString());
            }
            else
            {
                friendlyName.Append(DateTime.Now.Date.Month.ToString());
            }

            if (DateTime.Now.Date.Day < 10)
            {
                friendlyName.Append("0" + DateTime.Now.Date.Day.ToString());
            }
            else
            {
                friendlyName.Append(DateTime.Now.Date.Day.ToString());
            }

            friendlyName.Append(DateTime.Now.Year.ToString());

            if (DateTime.Now.Hour < 10)
            {
                friendlyName.Append("0" + DateTime.Now.Hour.ToString());
            }
            else
            {
                friendlyName.Append(DateTime.Now.Hour.ToString());
            }

            if (DateTime.Now.Minute < 10)
            {
                friendlyName.Append("0" + DateTime.Now.Minute.ToString());
            }
            else
            {
                friendlyName.Append(DateTime.Now.Minute.ToString());
            }

            if (DateTime.Now.Second < 10)
            {
                friendlyName.Append("0" + DateTime.Now.Second.ToString());
            }
            else
            {
                friendlyName.Append(DateTime.Now.Second.ToString());
            }

            return friendlyName.ToString();
        }
        #endregion

        #region View Key Protectors (includes Escrow)
        private void ViewKeys(object sender, EventArgs e)
        {
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. A dialogue may be open. " +
                  "Please wait a few seconds, and also be sure any key management dialogues you have open are closed, " +
                  "and try again.", "Add Numeric Password Protector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            keyManagementThread = new Thread(new ParameterizedThreadStart(ViewKeysWorkerThread));
            keyManagementThread.Name = "Key Ganderer Thread";
            // The thread uses the SaveFileDialog, which raises an exception in a multithreaded apartment, so
            // we set to single-threaded apartment (STA).
            keyManagementThread.SetApartmentState(ApartmentState.STA);
            keyManagementThread.Start(new BitLockerThreadParameters(deviceID));
        }

        private void ViewKeysWorkerThread(object data)
        {
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;

            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            bool isLocked = false;
            String[] allProtectors = null;

            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.GetKeyProtectors((uint)KeyProtectorTypes.ALL_TYPES, out allProtectors);
            if (bvms != BitLockerVolumeStatus.S_OK)
            {
                DisplayMessage("Unable to access Key Protectors on volume " + volumeDisplayName + " " + bvms.ToString(), "Severe",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            if (allProtectors == null)
            {
                DisplayMessage("Volume " + volumeDisplayName + " does not possess any Key Protectors.", "View Key Protectors",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            newStatus = "Checking volume lock status...";
            uint lockStatus;
            bvms = (BitLockerVolumeStatus)volume.GetLockStatus(out lockStatus);
            if (bvms == BitLockerVolumeStatus.S_OK && lockStatus == 1)
            {
                isLocked = true;
            }

            newStatus = "Getting All Key Protectors...";
            ArrayList listOfKeys = new ArrayList();
            foreach (String key in allProtectors)
            {
                String friendlyName = String.Empty;
                uint keyProtectorType = 0;
                bvms = (BitLockerVolumeStatus)volume.GetKeyProtectorFriendlyName(key, out friendlyName);
                bvms = (BitLockerVolumeStatus)volume.GetKeyProtectorType(key, out keyProtectorType);
                KeyProtectorInfo kpi = new KeyProtectorInfo((KeyProtectorTypes)keyProtectorType, key, friendlyName);

                String keyData = String.Empty;
                switch (kpi.Type)
                {
                    case KeyProtectorTypes.EXTERNAL_KEY:
                        {
                            bvms = (BitLockerVolumeStatus)volume.GetExternalKeyFileName(key, out keyData);
                            kpi.Password = keyData;
                            break;
                        }
                    case KeyProtectorTypes.NUMERIC_PASSWORD:
                        {
                            if (isLocked)
                            {
                                kpi.Password = "Cannot display; unlock volume to view.";
                            }
                            else
                            {
                                bvms = (BitLockerVolumeStatus)volume.GetKeyProtectorNumericalPassword(key, out keyData);
                                kpi.Password = keyData;
                            }
                            break;
                        }
                    case KeyProtectorTypes.TPM_AND_PIN:
                        {
                            //bvms = (BitLockerVolumeStatus)volume.G
                            //kpi.Password = keyData;
                            break;
                        }
                    case KeyProtectorTypes.TPM_AND_STARTUP_KEY:
                        {
                            bvms = (BitLockerVolumeStatus)volume.GetExternalKeyFileName(key, out keyData);
                            kpi.Password = keyData;
                            break;
                        }
                    case KeyProtectorTypes.TPM_AND_PIN_AND_STARTUP_KEY:
                        {
                            bvms = (BitLockerVolumeStatus)volume.GetExternalKeyFileName(key, out keyData);
                            kpi.Password = keyData;
                            break;
                        }
                    case KeyProtectorTypes.TRUSTED_PLATFORM_MODULE:
                        {
                            kpi.Password = "No displayable data with this protector.";
                            break;
                        }
                    case KeyProtectorTypes.PUBLIC_KEY:
                        {
                            kpi.Password = "No displayable data with this protector.";
                            break;
                        }
                    case KeyProtectorTypes.PASSPHRASE:
                        {
                            kpi.Password = "No displayable data with this protector.";
                            break;
                        }
                    default:
                        {
                            kpi.Password = "No displayable data with this protector.";
                            break;
                        }
                }

                listOfKeys.Add(kpi);
            }
            newStatus = "Key Protectors harvested.";

            KeyGandererDialogue kgd = new KeyGandererDialogue(listOfKeys, "The following Numeric Password Protector(s) are defined on " +
              "volume " + volumeDisplayName + ". BEKs and Passwords can be saved to a text file.", volume);
            kgd.ShowDialog();

            volume.Dispose();
            keyManagementRunning = false;
        }

        private void EscrowKeysToActiveDirectory(object sender, EventArgs e)
        {
            // Don't start a key management operation if another is pending.
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. Please wait a few seconds " +
                  "and try again.", "Key Escrow", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            // Start the enable auto unlock thread.
            keyManagementThread = new Thread(new ParameterizedThreadStart(EscrowKeysToActiveDirectoryWorkerThread));
            keyManagementThread.Name = "Key Escrow Thread";
            keyManagementThread.Start(new BitLockerThreadParameters(deviceID));
        }

        private void EscrowKeysToActiveDirectoryWorkerThread(object data)
        {
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;

            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            String[] passwordProtectors = null;

            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.GetKeyProtectors((uint)KeyProtectorTypes.NUMERIC_PASSWORD, out passwordProtectors);
            if (bvms != BitLockerVolumeStatus.S_OK)
            {
                newStatus = "Failed to access appropriate Key Protectors.";
                DisplayErrorMessage(bvms, "Unable to access Numeric Password Protectors on volume " + volumeDisplayName + ".", "Failed to Get Key Protectors",
                    true, true);
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            if (passwordProtectors == null || passwordProtectors.Length == 0)
            {
                newStatus = "No Numeric Key Protectors were detected.";
                DisplayMessage("Volume " + volumeDisplayName + " does not possess any Numeric Password Protectors.", "Key Escrow",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            ArrayList listOfKeys = new ArrayList();

            newStatus = "Getting Numeric Password Protectors...";
            foreach (String key in passwordProtectors)
            {
                String friendlyName = String.Empty;
                bvms = (BitLockerVolumeStatus)volume.GetKeyProtectorFriendlyName(key, out friendlyName);
                KeyProtectorInfo kpi = new KeyProtectorInfo(KeyProtectorTypes.EXTERNAL_KEY, key, friendlyName);
                String password = String.Empty;
                bvms = (BitLockerVolumeStatus)volume.GetKeyProtectorNumericalPassword(key, out password);
                kpi.Password = password;
                listOfKeys.Add(kpi);
            }
            newStatus = "Numeric Password Protectors harvested.";

            KeyProtectorDialogue kpd = new KeyProtectorDialogue(listOfKeys, "The following Numeric Password Protector(s) are defined on " +
              "volume " + volumeDisplayName + ". Select the one you want to save in Active Directory.", "48-Digit Password", false, true);
            kpd.ShowDialog();

            if (kpd.DialogResult == DialogResult.OK)
            {
                newStatus = "Escrow Numeric Password Protector " + kpd.SelectedKey + "...";
                bvms = (BitLockerVolumeStatus)volume.BackupRecoveryInformationToActiveDirectory(kpd.SelectedKey);

                switch (bvms)
                {
                    case BitLockerVolumeStatus.S_OK:
                        {
                            newStatus = "Numeric Password Protector " + kpd.SelectedKey + " has been escrowed.";
                            DisplayMessage("Numeric Password Protector " + kpd.SelectedKey + " was escrowed successfully.",
                              "Key Escrow", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                    case BitLockerVolumeStatus.S_FALSE:
                        {
                            newStatus = "Unable to escrow Numeric Password Protector.";
                            DisplayMessage("BitLocker cannot escrow the Numeric Password Protector. Either the group policy " +
                                "settings on this computer prohibit key escrow, or the computer is not a member of an Active " +
                                "Directory domain.\n\n(0x1 S_FALSE)", "Key Escrow Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    default:
                        {
                            newStatus = "Unable to escrow Numeric Password Protector.";
                            DisplayErrorMessage(bvms, "BitLocker cannot escrow the Numeric Password Protector.", "Key Escrow Failed",
                                true, true);
                            break;
                        }
                }
            }

            volume.Dispose();
            keyManagementRunning = false;
        }
        #endregion

        #region Enable/Suspend Key Protectors
        private void EnableKeyProtectors(object sender, EventArgs e)
        {
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. A dialogue may be open. " +
                  "Please wait a few seconds, and also be sure any key management dialogues you have open are closed, " +
                  "and try again.", "Add Numeric Password Protector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            keyManagementThread = new Thread(new ParameterizedThreadStart(EnableKeyProtectorsWorkerThread));
            keyManagementThread.Name = "Enable Key Protectors Thread";
            keyManagementThread.Start(new BitLockerThreadParameters(deviceID));
        }

        private void DisableKeyProtectors(object sender, EventArgs e)
        {
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. A dialogue may be open. " +
                  "Please wait a few seconds, and also be sure any key management dialogues you have open are closed, " +
                  "and try again.", "Add Numeric Password Protector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            String volumeDisplayName = (driveLetter.StartsWith("UL") ? deviceID : driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            if (QMessageBox.Show("Suspending the Key Protectors on the volume will expose the volume's encryption key in the clear on the disk. " +
              "This suspends all protection on the volume, meaning any user with any version of Windows that supports BitLocker Drive Encrytpion " +
              "will be able to access the contents of the volume without needing to provide any key. This is NOT recommended. If you need to " +
              "use this volume in another computer, you should leave the Key Protectors enabled and instead utilize one of those protectors to " +
              "unlock the volume in the other computer. If you must suspend the Key Protectors, you should reenable them as soon as possible. " +
              "Do you want to suspend the key protectors anyway?", "Suspend Key Protectors", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
              MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }
            else if (QMessageBox.Show("Are you absolutely sure you want to suspend the Key Protectors on volume " + volumeDisplayName + "?",
              "WARNING!", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }

            keyManagementThread = new Thread(new ParameterizedThreadStart(DisableKeyProtectorsWorkerThread));
            keyManagementThread.Name = "Enable Key Protectors Thread";
            keyManagementThread.Start(new BitLockerThreadParameters(deviceID));
        }

        private void EnableKeyProtectorsWorkerThread(object data)
        {
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;

            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.EnableKeyProtectors();
            switch (bvms)
            {
                case BitLockerVolumeStatus.S_OK:
                    {
                        // Key Protectors successfully enabled.
                        newStatus = "Key Protectors on volume " + volumeDisplayName + " are now enabled.";
                        DisplayMessage("Key Protectors on volume " + volumeDisplayName + " are now enabled.",
                          "Enable Key Protectors", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                default:
                    {
                        // Enable failed. Unexpected error.
                        newStatus = "Key Protectors on volume " + volumeDisplayName + " could not be enabled.";
                        DisplayErrorMessage(bvms, "Key Protectors on volume " + volumeDisplayName + " could not be enabled.",
                            "Enable Key Protectors Failed", true, true);
                        break;
                    }
            }

            volume.Dispose();
            keyManagementRunning = false;
        }

        private void DisableKeyProtectorsWorkerThread(object data)
        {
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;

            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.DisableKeyProtectors();
            switch (bvms)
            {
                case BitLockerVolumeStatus.S_OK:
                    {
                        // Key Protectors successfully disabled.
                        newStatus = "Key Protectors on volume " + volumeDisplayName + " are now suspended.";
                        DisplayMessage("Key Protectors on volume " + volumeDisplayName + " are now suspended.",
                          "Suspend Key Protectors", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                default:
                    {
                        // Enable failed. Unexpected error.
                        newStatus = "Key Protectors on volume " + volumeDisplayName + " could not be suspended.";
                        DisplayErrorMessage(bvms, "Key Protectors on volume " + volumeDisplayName + " could not be suspended.",
                            "Suspend Key Protectors Failed", true, true);
                        break;
                    }
            }

            volume.Dispose();
            keyManagementRunning = false;
        }
        #endregion

        #region Advanced Windows 7 - Upgrade BitLocker, Prepare (Discovery) Volume
        // Upgrades the BitLocker metadata version from Windows Vista to Windows 7.  Enables advanced Windows 7
        // features at the expense of losing portability of the volume to Windows Vista.
        private void UpgradeBitLocker(object sender, EventArgs e)
        {
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. A dialogue may be open. " +
                  "Please wait a few seconds, and also be sure any key management dialogues you have open are closed, " +
                  "and try again.", "Upgrade BitLocker Version", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            // A valid volume was selected, so let's get the info about it.
            HardDisk disk = GetDiskInfo(deviceID);
            UpgradeVolumeDialogue uvd = new UpgradeVolumeDialogue(disk);
            uvd.ShowDialog();

            if (uvd.DialogResult != DialogResult.OK)
            {
                return;
            }

            keyManagementThread = new Thread(new ParameterizedThreadStart(UpgradeBitLockerWorkerThread));
            keyManagementThread.Name = "Upgrade BitLocker Thread";
            keyManagementThread.Start(new BitLockerThreadParameters(deviceID));
        }

        private void UpgradeBitLockerWorkerThread(object data)
        {
            // Prevent another key management operation from being started.
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;

            // Get the EncryptableVolume.
            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            // Upgrade BitLocker on the volume.
            newStatus = "Upgrading BitLocker...";
            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.UpgradeVolume();
            switch (bvms)
            {
                case BitLockerVolumeStatus.S_OK:
                    {
                        // BitLocker metadata successfully updated.
                        newStatus = "Volume " + volumeDisplayName + " BitLocker metadata updated to Windows 7.";
                        DisplayMessage("BitLocker metadata version on volume " + volumeDisplayName + " was "
                            + "successfully upgraded to Windows 7.", "Upgrade BitLocker Version",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                default:
                    {
                        // Metadata update failed.
                        newStatus = "BitLocker metadata on volume " + volumeDisplayName + " could not be upgraded.";
                        DisplayErrorMessage(bvms, "The BitLocker metadata on volume " + volumeDisplayName + " failed " +
                            "to upgrade.", "BitLocker Upgrade Failed", true, true);
                        break;
                    }
            }

            volume.Dispose();
            keyManagementRunning = false;
        }
        #endregion

        #region Miscellaneous Methods
        private bool IsOsVolume(String driveLetter)
        {
            return String.Compare(driveLetter, System.Environment.SystemDirectory.Substring(0, 2), true) == 0;
        }

        private void DisplayErrorMessage(BitLockerVolumeStatus bvms, String messageBeginning,
            String messageBoxTitle, bool isError, bool delegateNeeded)
        {
            String errorMessage, errorCode, errorSymbol;
            errorMessage = ErrorInfo.GetErrorMessage(bvms, out errorSymbol, out errorCode);

            if (delegateNeeded)
            {
                DisplayMessage(messageBeginning + " " + errorMessage + "\n\n" +
                    "(" + errorCode + " " + errorSymbol + ")", messageBoxTitle,
                    MessageBoxButtons.OK, isError ? MessageBoxIcon.Error : MessageBoxIcon.Exclamation);
            }
            else
            {
                QMessageBox.Show(messageBeginning + " " + errorMessage + "\n\n" +
                    "(" + errorCode + " " + errorSymbol + ")", messageBoxTitle,
                    MessageBoxButtons.OK, isError ? MessageBoxIcon.Error : MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region Hardware Test
        private void CheckForEncryptionTest()
        {
            Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey subKey = registryHklm.CreateSubKey("SOFTWARE\\Dojo North Software\\Taryn BitLocker Manager");
            String driveToCheck = String.Empty;

            try
            {
                driveToCheck = (string)subKey.GetValue("EncryptionCheck");
            }
            catch
            {
                return;
            }

            if (String.IsNullOrEmpty(driveToCheck))
            {
                return;
            }

            // Clear the check flag.
            try
            {
                subKey.SetValue("EncryptionCheck", String.Empty);
            }
            catch
            {
            }

            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(driveToCheck, out volume, out volumeDisplayName))
            {
                QMessageBox.Show("A post-reboot encryption check was set on volume " + driveToCheck + ", " +
                    "but BitLocker failed to get a handle to the volume.", "Hardware Test Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            uint testStatus, errorCode;
            BitLockerVolumeStatus testError = BitLockerVolumeStatus.S_OK;
            testStatus = 0;
            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.GetHardwareTestStatus(
                out errorCode, out testStatus);
            testError = (BitLockerVolumeStatus)errorCode;

            if (testStatus == 0)
            {
                QMessageBox.Show("A hardware test check was set on volume " + driveToCheck + " on the last reboot. " +
                    "BitLocker reports no test failed, and no test is pending. This means that " +
                    "either the test was successful and volume " + driveToCheck + " is now encrypting, or more than " +
                    "one reboot has passed since the hardware check was executed and Home Server SMART was " +
                    "run. The main window will show you the current encryption status of volume " + driveToCheck + ".",
                    "Hardware Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (testStatus == 2)
            {
                QMessageBox.Show("A hardware test is pending on volume " + driveToCheck + ". Please reboot your " +
                    "computer to complete the test.", "Mandatory Reboot", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                // Test failed.
                DisplayErrorMessage(testError, "The system volume BitLocker hardware test failed.",
                    "Hardware Test Failed", true, false);
            }
        }

        private void SetHardwareTestAndReboot(String driveLetter, bool rebootNow)
        {
            Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey subKey = registryHklm.CreateSubKey("SOFTWARE\\Dojo North Software\\Taryn BitLocker Manager");

            try
            {
                subKey.SetValue("EncryptionCheck", driveLetter);
            }
            catch
            {
                return;
            }

            if (rebootNow)
            {
                QMessageBox.Show("A reboot will be initiated. Please ensure you save any open documents or other " +
                    "items that need to be saved.", "Reboot Pending", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                try
                {
                    ManagementClass W32_OS = new ManagementClass("Win32_OperatingSystem");
                    ManagementBaseObject inParams, outParams;
                    int result;
                    W32_OS.Scope.Options.EnablePrivileges = true;

                    foreach (ManagementObject obj in W32_OS.GetInstances())
                    {
                        inParams = obj.GetMethodParameters("Win32Shutdown");
                        inParams["Flags"] = ShutDown.Reboot;
                        inParams["Reserved"] = 0;
                        outParams = obj.InvokeMethod("Win32Shutdown", inParams, null);
                        result = Convert.ToInt32(outParams["returnValue"]);
                        if (result != 0) throw new Win32Exception(result);
                    }
                }
                catch
                {
                    QMessageBox.Show("Unable to trigger reboot. Please reboot manually.",
                        "Reboot Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        #endregion

        private void DEpicFail(Exception ex, String message, String resolution)
        {
            if (String.IsNullOrEmpty(message))
            {
                EpicFail fail = new EpicFail(ex, resolution, true);
                fail.ShowDialog();
            }
            else
            {
                EpicFail fail = new EpicFail(ex, message, resolution, true);
                fail.ShowDialog();
            }
        }

        private void AddThumbprint(object sender, EventArgs e)
        {
            if (keyManagementRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another key management request. A dialogue may be open. " +
                  "Please wait a few seconds, and also be sure any key management dialogues you have open are closed, " +
                  "and try again.", "Add Numeric Password Protector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            String volumeDisplayName = (driveLetter.StartsWith("UL") ? deviceID : driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            EncryptableVolume volume = null;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            String thumbprint = String.Empty;
            String subjectName = String.Empty;
            String protId = String.Empty;

            thumbprint = Certs.GetThumbprintFromStore(this.Handle, out subjectName);

            if (String.IsNullOrEmpty(thumbprint))
            {
                keyManagementRunning = false;
                return;
            }

            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.ProtectKeyWithCertificateThumbprint(subjectName,
                thumbprint, out protId);

            if (bvms == BitLockerVolumeStatus.S_OK)
            {
                DisplayMessage("A Smart Card key protector was successfully added to volume " + volumeDisplayName + ".",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                DisplayErrorMessage(bvms, "Matt Maes: ", "Severe", true, false);
            }
        }

        private void RenameSelectedDisk()
        {
            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            String volumeDisplayName = (driveLetter.StartsWith("UL") ? deviceID : driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            String volumeLabel = GetVolumeLabelFromLvi();
            VolumeLabelChanger vlc = new VolumeLabelChanger(driveLetter, volumeLabel);
            vlc.ShowDialog();

            if (vlc.DialogResult == DialogResult.OK)
            {
                foreach (ManagementObject device in new ManagementObjectSearcher(
                      "Select * From Win32_Volume").Get())
                {
                    if (String.Compare(device["DeviceID"].ToString(), deviceID, true) == 0)
                    {
                        try
                        {
                            newStatus = "Renaming volume " + volumeDisplayName + "...";
                            if (device["FileSystem"] != null && String.Compare(device["FileSystem"].ToString(), "NTFS", true) == 0)
                            {
                                device.SetPropertyValue("Label", vlc.VolumeLabel);
                                device.Put();
                                newStatus = "Renamed volume " + volumeDisplayName + " to " + vlc.VolumeLabel;
                            }
                            else
                            {
                                String nonNtfsTruncatedLabel = vlc.VolumeLabel.Substring(0, 11);
                                device.SetPropertyValue("Label", nonNtfsTruncatedLabel);
                                device.Put();
                                newStatus = "Renamed volume " + volumeDisplayName + " to " + nonNtfsTruncatedLabel + " (label was truncated)";
                            }
                        }
                        catch (Exception ex)
                        {
                            QMessageBox.Show("Volume " + volumeDisplayName + " cannot be renamed. " + ex.Message, "Rename Failed",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            newStatus = "Failed to rename volume " + volumeDisplayName;
                        }
                        RefreshEverything();
                    }
                }
            }
        }

        private void RefreshEverything()
        {
            // Refresh everything to update the view.
            ProcessingDialogue pd = new ProcessingDialogue(this.Location, this.Size);
            pd.TopMost = true;
            pd.Show();
            isManualRefreshInProgress = true;
            RefreshDisks();
            LoadList();
            isManualRefreshInProgress = false;
            pd.Hide();
            pd.Close();
            pd.Dispose();
        }

        private void buttonEncrypt_Click(object sender, EventArgs e)
        {
            // Encrypt the volume.
            EncryptVolume(sender, e);
        }

        private void buttonDecrypt_Click(object sender, EventArgs e)
        {
            // Decrypt the volume.
            DecryptVolume(sender, e);
        }

        private void RefreshListView(object state)
        {
            try
            {
                if (isManualRefreshInProgress)
                {
                    return;
                }
                else
                {
                    isManualRefreshInProgress = true;
                    RefreshDisks();
                    this.Invoke(dlgRefreshListView);
                }
            }
            catch (ManagementException)
            {
                // WMI exception "shutting down" may occur. If it does, we'll fail the operation silently.
                if (isManualRefreshInProgress)
                {
                    // Let's wait 5 seconds for good measure.
                    Thread.Sleep(5000);
                    if (isManualRefreshInProgress)
                    {
                        isManualRefreshInProgress = false;
                    }
                }
            }
            catch (Exception ex)
            {
                //EpicFail fail = new EpicFail(ex, "The refresh list view timer generated an error. No action is required, but if this appears again " +
                //    "please submit a bug report.");
                //fail.ShowDialog();
                this.Invoke(dlgEpicFail, new object[] { ex, String.Empty, "The refresh list view timer generated an error. No action is required, but if this appears again " +
                    "please submit a bug report."});
                isManualRefreshInProgress = false;
            }
        }

        private void DTimerRefreshListView()
        {
            LoadList();
            isManualRefreshInProgress = false;
            UpdateUiFromSelectedIndexChange();
        }

        private void buttonLock_Click(object sender, EventArgs e)
        {
            LockVolume(sender, e);
        }

        private void buttonUnlock_Click(object sender, EventArgs e)
        {
            // Don't start lock/unlock operation if another is pending.
            if (lockUnlockRunning)
            {
                QMessageBox.Show("Home Server SMART is currently processing another lock or unlock request. Please wait a few seconds " +
                  "and try again.", "Lock Volume", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Get the drive letter from user's selection.
            String driveLetter = Properties.Resources.DriveLetterNa;
            String deviceID = GetDeviceIdFromLvi(out driveLetter);
            if (deviceID == String.Empty)
            {
                newStatus = INVALID_SELECTION;
                return;
            }

            // Start a worker thread to prompt the user and unlock.
            lockUnlockThread = new Thread(new ParameterizedThreadStart(UnlockVolume));
            lockUnlockThread.Name = "Unlock Volume Thread";
            // The thread uses the OpenFileDialog, which raises an exception in a multithreaded apartment, so
            // we set to single-threaded apartment (STA).
            lockUnlockThread.SetApartmentState(ApartmentState.STA);
            lockUnlockThread.Start(new BitLockerThreadParameters(deviceID));
        }

        private void UnlockVolume(object data)
        {
            keyManagementRunning = true;

            BitLockerThreadParameters input = (BitLockerThreadParameters)data;
            String deviceID = input.DeviceId;

            EncryptableVolume volume = null;
            String volumeDisplayName = String.Empty;
            if (!GetEncryptableVolumeInfoByDeviceID(deviceID, out volume, out volumeDisplayName))
            {
                newStatus = "Failed to access volume.";
                keyManagementRunning = false;
                return;
            }

            String[] allProtectors = null;

            BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.GetKeyProtectors((uint)KeyProtectorTypes.ALL_TYPES, out allProtectors);
            if (bvms != BitLockerVolumeStatus.S_OK)
            {
                DisplayMessage("Unable to access Key Protectors on volume " + volumeDisplayName + " " + bvms.ToString(), "Severe",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            if (allProtectors == null)
            {
                DisplayMessage("Volume " + volumeDisplayName + " does not possess any Key Protectors.", "View Key Protectors",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            newStatus = "Checking volume lock status...";
            uint lockStatus;
            bvms = (BitLockerVolumeStatus)volume.GetLockStatus(out lockStatus);
            if (bvms == BitLockerVolumeStatus.S_OK && lockStatus == 1)
            {
            }
            else
            {
                volume.Dispose();
                keyManagementRunning = false;
                return;
            }

            newStatus = "Reading All Key Protectors...";
            bool hasKeyBek = false;
            bool hasKeyNumPassword = false;
            bool hasKeySmartCard = false;
            bool hasKeyPassword = false;
            ArrayList listOfKeys = new ArrayList();
            foreach (String key in allProtectors)
            {
                uint keyProtectorType = 0;
                bvms = (BitLockerVolumeStatus)volume.GetKeyProtectorType(key, out keyProtectorType);

                switch ((KeyProtectorTypes)keyProtectorType)
                {
                    case KeyProtectorTypes.EXTERNAL_KEY:
                        {
                            hasKeyBek = true;
                            break;
                        }
                    case KeyProtectorTypes.NUMERIC_PASSWORD:
                        {
                            hasKeyNumPassword = true;
                            break;
                        }
                    case KeyProtectorTypes.TPM_AND_PIN:
                        {
                            break;
                        }
                    case KeyProtectorTypes.TPM_AND_STARTUP_KEY:
                        {
                            break;
                        }
                    case KeyProtectorTypes.TPM_AND_PIN_AND_STARTUP_KEY:
                        {
                            break;
                        }
                    case KeyProtectorTypes.TRUSTED_PLATFORM_MODULE:
                        {
                            break;
                        }
                    case KeyProtectorTypes.PUBLIC_KEY:
                        {
                            hasKeySmartCard = true;
                            break;
                        }
                    case KeyProtectorTypes.PASSPHRASE:
                        {
                            hasKeyPassword = true;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            newStatus = "Key Protectors harvested; displaying unlock options.";

            UnlockVolumeDialogue uvd = new UnlockVolumeDialogue(hasKeyBek, hasKeyNumPassword, hasKeySmartCard, hasKeyPassword);
            uvd.ShowDialog();

            if (uvd.DialogResult == DialogResult.OK)
            {
                if (uvd.UnlockWithBek)
                {
                    byte[] key;

                    newStatus = "Reading external key...";
                    bvms = (BitLockerVolumeStatus)volume.GetExternalKeyFromFile(uvd.KeyFileName, out key);
                    switch (bvms)
                    {
                        case BitLockerVolumeStatus.S_OK:
                            {
                                // S_OK, FVE_E_PROTECTOR_NOT_FOUND, FVE_E_FAILED_AUTHENTICATION.
                                newStatus = "Unlocking volume " + volumeDisplayName + "...";
                                bvms = (BitLockerVolumeStatus)volume.UnlockWithExternalKey(key);

                                switch (bvms)
                                {
                                    case BitLockerVolumeStatus.S_OK:
                                        {
                                            // Unlock successful.
                                            newStatus = "Volume " + volumeDisplayName + " is now unlocked.";
                                            DisplayMessage("Volume " + volumeDisplayName + " was successfully unlocked.", "Unlock Volume", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            break;
                                        }
                                    default:
                                        {
                                            // Unlock failed. Unexpected error.
                                            newStatus = "Volume " + volumeDisplayName + " could not be unlocked.";
                                            DisplayErrorMessage(bvms, "BitLocker failed to unlock volume " + volumeDisplayName + ".", "Unlock Failed",
                                                true, true);
                                            break;
                                        }
                                }
                                break;
                            }
                        default:
                            {
                                // Read from BEK failed. Unexpected error.
                                newStatus = "Volume " + volumeDisplayName + " could not be unlocked.";
                                DisplayErrorMessage(bvms, "BitLocker was unable to open the External Key Protector provided.",
                                  "Unlock Failed", true, true);
                                break;
                            }
                    }
                }
                else if (uvd.UnlockWithNumPassword)
                {
                    // Try unlocking the volume with the password.
                    bvms = BitLockerVolumeStatus.S_OK;
                    try
                    {
                        bvms = (BitLockerVolumeStatus)volume.UnlockWithNumericalPassword(uvd.NumericPassword);
                    }
                    catch (ArgumentException)
                    {
                        // ArgumentException is thrown if password is not in correct format.
                        bvms = BitLockerVolumeStatus.FVE_E_INVALID_PASSWORD_FORMAT;
                    }

                    switch (bvms)
                    {
                        case BitLockerVolumeStatus.S_OK:
                            {
                                // Unlock successful.
                                newStatus = "Volume " + volumeDisplayName + " is now unlocked.";
                                DisplayMessage("Volume " + volumeDisplayName + " was successfully unlocked.", "Unlock Volume", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        default:
                            {
                                // Unlock failed.
                                newStatus = "Volume " + volumeDisplayName + " failed to unlock.";
                                DisplayErrorMessage(bvms, "Volume " + volumeDisplayName + " failed to unlock. ",
                                    "Unlock Failed", true, true);
                                break;
                            }
                    }
                }
                else if (uvd.UnlockWithSmartCard)
                {
                    newStatus = "Getting PIN...";

                    // Prompt the user for the PIN.
                    String pin = String.Empty;
                    CertificateFilePinDialogue cfpd = new CertificateFilePinDialogue();
                    if (cfpd.ShowDialog() == DialogResult.OK)
                    {
                        pin = cfpd.Pin;
                    }
                    else
                    {
                        newStatus = "Cancelled.";
                        lockUnlockRunning = false;
                        return;
                    }

                    newStatus = "Unlocking volume " + volumeDisplayName + "...";

                    bvms = (BitLockerVolumeStatus)volume.UnlockWithCertificateThumbprint(uvd.CertificateThumbprint, pin);
                    switch (bvms)
                    {
                        case BitLockerVolumeStatus.S_OK:
                            {
                                // Unlock successful.
                                newStatus = "Volume " + volumeDisplayName + " is now unlocked.";
                                DisplayMessage("Volume " + volumeDisplayName + " was successfully unlocked.", "Unlock Volume", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        default:
                            {
                                // Unlock failed.
                                newStatus = "Volume " + volumeDisplayName + " could not be unlocked.";
                                DisplayErrorMessage(bvms, "BitLocker failed to unlock volume " + volumeDisplayName + ".", "Unlock Failed",
                                    true, true);
                                break;
                            }
                    }
                }
                else if (uvd.UnlockWithPassword)
                {
                    newStatus = "Unlocking volume " + volumeDisplayName + "...";

                    // Try unlocking the volume with the password.
                    bvms = BitLockerVolumeStatus.S_OK;
                    try
                    {
                        bvms = (BitLockerVolumeStatus)volume.UnlockWithPassphrase(uvd.Password);
                    }
                    catch (ArgumentException)
                    {
                        // ArgumentException is thrown if password is not in correct format.
                        bvms = BitLockerVolumeStatus.FVE_E_INVALID_PASSWORD_FORMAT;
                    }

                    switch (bvms)
                    {
                        case BitLockerVolumeStatus.S_OK:
                            {
                                // Unlock successful.
                                newStatus = "Volume " + volumeDisplayName + " is now unlocked.";
                                DisplayMessage("Volume " + volumeDisplayName + " was successfully unlocked.", "Unlock Volume", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        default:
                            {
                                // Unlock failed.
                                newStatus = "Volume " + volumeDisplayName + " failed to unlock.";
                                DisplayErrorMessage(bvms, "Volume " + volumeDisplayName + " failed to unlock. ",
                                    "Unlock Failed", true, true);
                                break;
                            }
                    }
                }
            }

            volume.Dispose();
            keyManagementRunning = false;
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshEverything();
        }

        private void buttonQuickHelp_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Dojo North Software\\Home Server SMART 2012\\HomeServerSMART.chm");
        }

        private void menuViewGpo_Click(object sender, EventArgs e)
        {
            // Display the Group Policy Configuration dialogue.
            GroupPolicy gpo = new GroupPolicy(isWindows7Family);
            gpo.ShowDialog();
        }

        private void menuRepairTpm_Click(object sender, EventArgs e)
        {
            RepairTpm(sender, e);
        }

        private void menuEscrowKeysToAd_Click(object sender, EventArgs e)
        {
            System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher("SELECT Name FROM Win32_OperatingSystem");
            foreach (System.Management.ManagementObject OSObject in searcher.Get())
            {
                String osName = String.Empty;
                try
                {
                    osName = OSObject.Properties["Name"].Value.ToString();
                }
                catch { }

                if (osName.Contains("Windows Home Server 2011"))
                {
                    QMessageBox.Show("You are running Windows Home Server 2011, which cannot be joined to an Active Directory domain.",
                        "Active Directory Not Available", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            EscrowKeysToActiveDirectory(sender, e);
        }

        private void menuChangeVolumeLabel_Click(object sender, EventArgs e)
        {
            RenameSelectedDisk();
        }

        private void menuAutoUnlockEnable_Click(object sender, EventArgs e)
        {
            // Enable automatic unlocking on a volume.
            EnableAutoUnlock(sender, e);
        }

        private void menuAutoUnlockDisable_Click(object sender, EventArgs e)
        {
            // Disable automatic unlocking on a volume.
            DisableAutoUnlock(sender, e);
        }

        private void menuViewProtectors_Click(object sender, EventArgs e)
        {
            ViewKeys(sender, e);
        }

        private void menuBekAdd_Click(object sender, EventArgs e)
        {
            AddExternalKeyProtector(sender, e);
        }

        private void menuBekDelete_Click(object sender, EventArgs e)
        {
            RemoveExternalKeyProtector(sender, e);
        }

        private void menuNumPasswordAdd_Click(object sender, EventArgs e)
        {
            AddNumericPasswordProtector(sender, e);
        }

        private void menuNumPasswordDelete_Click(object sender, EventArgs e)
        {
            RemovePasswordProtector(sender, e);
        }

        private void menuSmartCardsAdd_Click(object sender, EventArgs e)
        {
            AddThumbprint(sender, e);
        }

        private void menuSmartCardsDelete_Click(object sender, EventArgs e)
        {
            //RemoveT
        }

        private void menuPasswordAdd_Click(object sender, EventArgs e)
        {
            AddPassphraseProtector(sender, e);
        }

        private void menuPasswordChange_Click(object sender, EventArgs e)
        {
            ChangePassphraseProtector(sender, e);
        }

        private void menuPasswordDelete_Click(object sender, EventArgs e)
        {
            RemovePassphraseProtector(sender, e);
        }

        private void menuSuspendOff_Click(object sender, EventArgs e)
        {
            EnableKeyProtectors(sender, e);
        }

        private void menuSuspendOn_Click(object sender, EventArgs e)
        {
            DisableKeyProtectors(sender, e);
        }

        private void RepairTpm(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvEncryptableVolumes.Items)
            {
                if (lvi != null)
                {
                    if (String.Compare("System", lvi.SubItems[11].Text, true) == 0)
                    {
                        if (String.Compare(ENCRYPTION_STATUS_FULLY_ENCRYPTED, lvi.SubItems[4].Text, true) == 0 ||
                            String.Compare(ENCRYPTION_STATUS_ENCRYPTING, lvi.SubItems[4].Text, true) == 0 ||
                            String.Compare(ENCRYPTION_STATUS_ENCRYPTION_PAUSED, lvi.SubItems[4].Text, true) == 0)
                        {
                            ChangeTpmKey ctk = new ChangeTpmKey(isWindows7Family, lvi.SubItems[0].Text, lvi.SubItems[1].Text);
                            ctk.ShowDialog();
                            break;
                        }
                        else if (String.Compare(ENCRYPTION_STATUS_FULLY_DECRYPTED, lvi.SubItems[4].Text, true) == 0)
                        {
                            QMessageBox.Show("The System volume is fully decrypted. Please use the Encrypt function to create the TPM key.",
                                "System Volume Decrypted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;
                        }
                        else
                        {
                            QMessageBox.Show("The encryption state of the System volume is invalid. To change the TPM key, please " +
                                "ensure the encryption state of the System volume is Fully Encrypted, Encrypting or Encryption " +
                                "Paused.", "System Volume State Invalid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;
                        }
                    }
                }
            }
        }

        private void menuBekChange_Click(object sender, EventArgs e)
        {
            ChangeExternalKeyProtector(sender, e);
        }

        private void menuPause_Click(object sender, EventArgs e)
        {
            PauseOperation(sender, e);
        }

        private void menuResume_Click(object sender, EventArgs e)
        {
            ResumeOperation(sender, e);
        }

        private void menuUpgrade_Click(object sender, EventArgs e)
        {
            UpgradeBitLocker(sender, e);
        }

        private void menuQuickHelp_Click(object sender, EventArgs e)
        {
            
        }

        private void menuDriveBender_Click(object sender, EventArgs e)
        {
            if (!UtilityMethods.IsDriveBenderInstalled())
            {
                if (QMessageBox.Show("Disk pool management software was not detected on this Server. Do you want to manage automatic unlocking for disk pools anyway?",
                    "Drive Bender/StableBit DrivePool", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }
            }

            int timer = 0;
            while (isManualRefreshInProgress && timer < 15)
            {
                Thread.Sleep(1000);
                timer++;
            }

            if (timer >= 15)
            {
                QMessageBox.Show("An background operation is taking an excessively long time to execute. This is not uncommon when a major pool operation is in progress, " +
                    "including but not limited to a pool repair, pool service restart, the addition of a disk to the pool or the removal of a disk from the pool. Please " +
                    "try again later, especially if a major pool operation (which is I/O intensive) completes.", "Request Timed Out", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }
            DiskPoolAutoUnlock unlocker = new DiskPoolAutoUnlock(listOfDisks);
            unlocker.ShowDialog();
        }

        private enum ListViewUpdateState : int
        {
            Clean = 0,
            Filthy = 1
        }

        private void DisplayResultWindow(String results)
        {
            this.Invoke(dlgDisplayResult1, new object[] { results });
        }

        private void DisplayResultWindow(String results, String titleText)
        {
            this.Invoke(dlgDisplayResult2, new object[] { results, titleText });
        }

        private void DisplayResultWindow(String results, String path, String driveLetter, bool saveNow)
        {
            this.Invoke(dlgDisplayResult3, new object[] { results, path, driveLetter, saveNow });
        }

        private void DDisplayResultWindow(String results)
        {
            ResultWindow rw = new ResultWindow(results);
            rw.ShowDialog();
        }

        private void DDisplayResultWindow(String results, String titleText)
        {
            ResultWindow rw = new ResultWindow(results, titleText);
            rw.ShowDialog();
        }

        private void DDisplayResultWindow(String results, String path, String driveLetter, bool saveNow)
        {
            ResultWindow rw = new ResultWindow(results, path, driveLetter, saveNow);
            rw.ShowDialog();
        }

        #region Window Background
        private void SetWindowBackground()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.BitLockerControl.SetWindowBackground");
            switch (windowBackground)
            {
                case 3:
                    {
                        this.BackgroundImage = null;
                        this.BackgroundImageLayout = ImageLayout.None;
                        break;
                    }
                case 0:
                default:
                    {
                        this.BackgroundImage = Properties.Resources.MetalGrate;
                        this.BackgroundImageLayout = ImageLayout.Tile;
                        break;
                    }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.BitLockerControl.SetWindowBackground");
        }
        #endregion
    }
}
