using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32;

using  WSSControls.BelovedComponents;
using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI
{
    public partial class DiskPoolAutoUnlock : Form
    {
        private List<HardDisk> listOfEncryptableVolumes;
        private EncryptableVolume.EncryptableVolumeCollection encryptionVolumes;

        // ImageList for disk statuses.
        private ImageList diskHealthIndicators;

        // Constants
        private const String ENCRYPTION_STATUS_FULLY_DECRYPTED = "Fully Decrypted";
        
        public DiskPoolAutoUnlock(List<HardDisk> listOfDisks)
        {
            InitializeComponent();

            listOfEncryptableVolumes = GetEncryptableVolumes(listOfDisks);

            // Encryption indicators
            diskHealthIndicators = new ImageList();
            diskHealthIndicators.Images.Add("Configured", Properties.Resources.ShieldGreen16);
            diskHealthIndicators.Images.Add("ConfiguredOsOnly", Properties.Resources.ShieldYellow16);
            diskHealthIndicators.Images.Add("NotConfigured", Properties.Resources.ShieldRed16);
            lvEncryptableVolumes.SmallImageList = diskHealthIndicators;

            // EV collection.
            encryptionVolumes = null;
        }

        private List<HardDisk> GetEncryptableVolumes(List<HardDisk> allDisks)
        {
            List<HardDisk> encryptableDisks = new List<HardDisk>();

            // We could get the information from the FancyListView in the parent window, which technically is faster.
            // But checking again and getting the latest information ensures that any changes made by the user not
            // yet reflected in the UI are detected.
            
            try
            {
                encryptionVolumes = EncryptableVolume.GetInstances();
            }
            catch
            {
                return encryptableDisks;
            }

            // Go through all the encryptable volumes till we find a match to our disk.
            foreach (EncryptableVolume encryptVolume in encryptionVolumes)
            {
                foreach (HardDisk hardDisk in allDisks)
                {
                    //if (hardDisk.DriveLetter.ToUpper() == encryptVolume.DriveLetter.ToUpper())
                    if (String.Compare(hardDisk.DeviceID, encryptVolume.DeviceID, true) == 0)
                    {
                        if (hardDisk.HdType == DiskType.DISK_SYSTEM ||
                            hardDisk.EncryptionStatus == "Locked" ||
                            hardDisk.EncryptionStatus == ENCRYPTION_STATUS_FULLY_DECRYPTED)
                        {
                            // We don't need the System volume. If the user gets this far the System volume is unlocked!
                            // We also hide locked disks -- we can't manage them here!
                            break;
                        }
                        bool isProtectedByService = false;
                        encryptableDisks.Add(hardDisk);

                        isProtectedByService = IsDiskAutoUnlockedByService(encryptVolume.PersistentVolumeID);

                        FancyListView.ImageSubItem protectionItem = new FancyListView.ImageSubItem();
                        if (isProtectedByService)
                        {
                            protectionItem.Text = "Managed by Taryn BitLocker Manager";
                            protectionItem.ImageKey = "Configured";
                        }
                        else if (hardDisk.IsAutoUnlockEnabled)
                        {
                            protectionItem.Text = "Windows auto-unlock is enabled";
                            protectionItem.ImageKey = "ConfiguredOsOnly";
                        }
                        else
                        {
                            protectionItem.Text = "Volume is locked at boot time";
                            protectionItem.ImageKey = "NotConfigured";
                        }

                        String driveName = String.Empty;
                        if (String.IsNullOrEmpty(hardDisk.DriveLetter) || hardDisk.DriveLetter.StartsWith("UL"))
                        {
                            if (String.IsNullOrEmpty(hardDisk.Caption))
                            {
                                driveName = hardDisk.DeviceID;
                            }
                            else
                            {
                                driveName = hardDisk.Caption;
                            }
                        }
                        else
                        {
                            driveName = hardDisk.DriveLetter;
                        }

                        // Item doesn't exist, so compose new.
                        ListViewItem lvi = new ListViewItem(new string[] { driveName });
                        lvi.SubItems.Add(encryptVolume.PersistentVolumeID);
                        lvi.SubItems.Add(hardDisk.VolumeLabel);
                        lvi.SubItems.Add(protectionItem);
                        lvEncryptableVolumes.Items.Add(lvi);

                        break;
                    }
                }
            }
            return encryptableDisks;
        }

        private bool IsDiskAutoUnlockedByService(String persistentVolumeID)
        {
            try
            {
                Microsoft.Win32.RegistryKey dojoNorthRootKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, true);
                Microsoft.Win32.RegistryKey configurationKey = dojoNorthRootKey.CreateSubKey(Properties.Resources.RegistryConfigurationPoolHelperKey);

                Microsoft.Win32.RegistryKey volumeKey = configurationKey.OpenSubKey(persistentVolumeID, true);
                if (volumeKey == null || volumeKey.Handle == null)
                {
                    configurationKey.Close();
                    dojoNorthRootKey.Close();
                    return false;
                }
                else
                {
                    byte[] savedBekData = (byte[])volumeKey.GetValue("Data");
                    object obj = volumeKey.GetValue("Guid");

                    volumeKey.Close();
                    configurationKey.Close();
                    dojoNorthRootKey.Close();

                    if (obj == null || savedBekData == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                QMessageBox.Show("The Server failed to read auto-unlock configuration data from the Registry. " + ex.Message, "Severe",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
        }

        private void DiskPoolAutoUnlock_Load(object sender, EventArgs e)
        {
            GetServiceHealthState();
        }

        private void GetServiceHealthState()
        {
            bool isHelperStarted = false;
            try
            {
                if (DoesServiceExist(Properties.Resources.ServiceNameHssFveHelper))
                {
                    ServiceController svcHelper = new ServiceController(Properties.Resources.ServiceNameHssFveHelper);
                    if (svcHelper.Status == ServiceControllerStatus.Running)
                    {
                        isHelperStarted = true;
                    }
                    RegistryKey serviceRoot = Registry.LocalMachine.OpenSubKey(Properties.Resources.RegistryConfigServiceHelperKey, false);
                    int startType = (Int32)serviceRoot.GetValue(Properties.Resources.RegistryServiceStartFlag);
                    switch (startType)
                    {
                        case 2:
                            {
                                if (isHelperStarted)
                                {
                                    pictureBoxServiceStatus.Image = Properties.Resources.Healthy16;
                                    labelServiceStatus.Text = "BitLocker Pool Helper Service is set for automatic startup and is running.";
                                }
                                else
                                {
                                    pictureBoxServiceStatus.Image = Properties.Resources.Critical16;
                                    labelServiceStatus.Text = "BitLocker Pool Helper Service is set for automatic startup but is NOT running.";
                                }
                                break;
                            }
                        case 3:
                            {
                                if (isHelperStarted)
                                {
                                    pictureBoxServiceStatus.Image = Properties.Resources.Healthy16;
                                    labelServiceStatus.Text = "BitLocker Pool Helper Service is set for manual startup and is running.";
                                }
                                else
                                {
                                    pictureBoxServiceStatus.Image = Properties.Resources.Critical16;
                                    labelServiceStatus.Text = "BitLocker Pool Helper Service is set for manual startup but is NOT running.";
                                }
                                break;
                            }
                        case 4:
                            {
                                if (isHelperStarted)
                                {
                                    pictureBoxServiceStatus.Image = Properties.Resources.Warning16;
                                    labelServiceStatus.Text = "BitLocker Pool Helper Service is running, but is set to disabled. The service may not restart on the next reboot.";
                                }
                                else
                                {
                                    pictureBoxServiceStatus.Image = Properties.Resources.Warning16;
                                    labelServiceStatus.Text = "BitLocker Pool Helper Service is disabled.";
                                }
                                break;
                            }
                        default:
                            {
                                pictureBoxServiceStatus.Image = Properties.Resources.Critical16;
                                labelServiceStatus.Text = "BitLocker Pool Helper Service is not configured correctly. Run/start state is unknown.";
                                break;
                            }
                    }
                }
                else
                {
                    pictureBoxServiceStatus.Image = Properties.Resources.Warning16;
                    labelServiceStatus.Text = "BitLocker Pool Helper Service is not installed. You can configure your volumes now, but you must install the service if you want to auto-unlock them.";
                }
            }
            catch (Exception ex)
            {
                pictureBoxServiceStatus.Image = Properties.Resources.Critical16;
                labelServiceStatus.Text = "Service Failure: " + ex.Message;
            }
        }
        
        /// <summary>
        /// method to determine if a Windows Service is available on the local service
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        private bool DoesServiceExist(string service)
        {
            ServiceController[] services = System.ServiceProcess.ServiceController.GetServices(".");
            return services.Any(s => s.ServiceName.ToLower() == service.ToLower());
        }

        private void buttonEnable_Click(object sender, EventArgs e)
        {
            if (lvEncryptableVolumes.SelectedItems != null && lvEncryptableVolumes.SelectedItems.Count == 1)
            {
                buttonEnable.Enabled = false;

                try
                {
                    EncryptableVolume.EncryptableVolumeCollection encryptionVolumes = EncryptableVolume.GetInstances();
                    EncryptableVolume ev = null;
                    ListViewItem lvi = lvEncryptableVolumes.SelectedItems[0];
                    String persistentVolumeID = lvi.SubItems[1].Text;
                    foreach (EncryptableVolume volume in encryptionVolumes)
                    {
                        if (String.Compare(persistentVolumeID, volume.PersistentVolumeID, true) == 0)
                        {
                            ev = volume;
                            break;
                        }
                    }

                    if (ev != null)
                    {
                        Microsoft.Win32.RegistryKey dojoNorthRootKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                            Properties.Resources.RegistryDojoNorthRootKey, true);
                        Microsoft.Win32.RegistryKey configurationKey = dojoNorthRootKey.OpenSubKey(Properties.Resources.RegistryConfigurationPoolHelperKey, true);

                        if (QMessageBox.Show("Enabling automatic unlocking for pools will automatically unlock the volume only if you have installed (and enabled) the " +
                            "Home Server SMART BitLocker Pool Helper Service. This service unlocks the volumes at boot time, before the disk pool attempts to start.\n\n" +
                            "Do you want to enable automatic unlocking on the following volume?\n\n     Volume: " + lvi.SubItems[0].Text + "\n     ID: " + persistentVolumeID,
                            "Enable Automatic Unlocking for Pools", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            buttonEnable.Enabled = true;
                            return;
                        }

                        // Create the Registry key.
                        Microsoft.Win32.RegistryKey volumeKey = configurationKey.CreateSubKey(ev.PersistentVolumeID);

                        String protectorID = String.Empty;
                        BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)ev.ProtectKeyWithExternalKey(null, "HSS Disk Pool Service Helper Key",
                            out protectorID);
                        if (bvms == BitLockerVolumeStatus.S_OK)
                        {
                            byte[] key;
                            bvms = (BitLockerVolumeStatus)ev.GetKeyProtectorExternalKey(protectorID, out key);
                            if (bvms == BitLockerVolumeStatus.S_OK)
                            {
                                // Commit to the Registry.
                                volumeKey.SetValue("Guid", protectorID);
                                volumeKey.SetValue("Data", key);
                                volumeKey.SetValue("Path", ev.DeviceID);

                                // Update the UI.
                                FancyListView.ImageSubItem subItem = new FancyListView.ImageSubItem();
                                subItem.Text = "Managed by Taryn BitLocker Manager";
                                subItem.ImageKey = "Configured";
                                lvi.SubItems[3] = subItem;

                                QMessageBox.Show("Automatic unlocking successfully configured on volume " + persistentVolumeID + ".", "Enable Automatic Unlocking for Pools",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                DisplayErrorMessage(bvms, "A BitLocker key was generated, but could not be read back from the volume.", "Severe");
                            }
                        }
                        else
                        {
                            DisplayErrorMessage(bvms, "A BitLocker key could not be generated or written to the volume.", "Severe");
                        }
                        volumeKey.Close();
                        configurationKey.Close();
                        dojoNorthRootKey.Close();
                    }
                    else
                    {
                        QMessageBox.Show("The disk with BitLocker persistent volume ID " + persistentVolumeID + " no longer appears to be attached to the Server.",
                            "Volume Missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                catch (Exception ex)
                {
                    buttonEnable.Enabled = true;
                    QMessageBox.Show("Exceptions were detected processing the enable request. " + ex.Message, "Severe",
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void buttonDisable_Click(object sender, EventArgs e)
        {
            if (lvEncryptableVolumes.SelectedItems != null && lvEncryptableVolumes.SelectedItems.Count == 1)
            {
                buttonDisable.Enabled = false;
                try
                {
                    // Get a handle to the encryptable volume.
                    EncryptableVolume.EncryptableVolumeCollection encryptionVolumes = EncryptableVolume.GetInstances();
                    EncryptableVolume ev = null;
                    ListViewItem lvi = lvEncryptableVolumes.SelectedItems[0];
                    String persistentVolumeID = lvi.SubItems[1].Text;
                    foreach (EncryptableVolume volume in encryptionVolumes)
                    {
                        if (String.Compare(persistentVolumeID, volume.PersistentVolumeID, true) == 0)
                        {
                            ev = volume;
                            break;
                        }
                    }

                    if (ev != null)
                    {
                        Microsoft.Win32.RegistryKey dojoNorthRootKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                            Properties.Resources.RegistryDojoNorthRootKey, true);
                        Microsoft.Win32.RegistryKey configurationKey = dojoNorthRootKey.OpenSubKey(Properties.Resources.RegistryConfigurationPoolHelperKey, true);

                        if (QMessageBox.Show("Disabling automatic unlocking for pools will prevent the volume from being unlocked automatically at boot time by the " +
                            "Home Server SMART BitLocker Pool Helper Service. If this disk participates in a disk pool, the pool may fail to start up correctly.\n\n" +
                            "Do you want to disable automatic unlocking on the following volume?\n\n     Volume: " + lvi.SubItems[0].Text + "\n     ID: " + persistentVolumeID,
                            "Disable Automatic Unlocking for Pools", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
                        {
                            buttonDisable.Enabled = true;
                            return;
                        }

                        Microsoft.Win32.RegistryKey volumeKey = configurationKey.OpenSubKey(ev.PersistentVolumeID, false);

                        String protectorID = String.Empty;
                        try
                        {
                            protectorID = (String)volumeKey.GetValue("Guid");
                        }
                        catch (Exception ex)
                        {
                            QMessageBox.Show("Unable to read the stored protector ID from the Registry. " + ex.Message, "Severe",
                                MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            configurationKey.Close();
                            dojoNorthRootKey.Close();
                            return;
                        }

                        BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)ev.DeleteKeyProtector(protectorID);

                        if (bvms == BitLockerVolumeStatus.S_OK)
                        {
                            // Successful deletion, so clean up the Registry.
                            try
                            {
                                // Close the key and delete.
                                volumeKey.Close();
                                configurationKey.DeleteSubKey(ev.PersistentVolumeID);

                                // Update the UI.
                                FancyListView.ImageSubItem subItem = new FancyListView.ImageSubItem();
                                subItem.Text = "Volume is locked at boot time";
                                subItem.ImageKey = "NotConfigured";
                                lvi.SubItems[3] = subItem;

                                QMessageBox.Show("Automatic unlocking successfully disabled on volume " + persistentVolumeID + ".", "Disable Automatic Unlocking for Pools",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                QMessageBox.Show("Automatic unlocking was successfully disabled, but the saved key data could not be removed from the Registry. " + ex.Message,
                                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            }
                        }
                        else
                        {
                            buttonDisable.Enabled = true;
                            DisplayErrorMessage(bvms, "BitLocker could not remove the key protector from the volume.", "Severe");
                        }
                        volumeKey.Close();
                        configurationKey.Close();
                        dojoNorthRootKey.Close();
                    }
                    else
                    {
                        QMessageBox.Show("The disk with BitLocker persistent volume ID " + persistentVolumeID + " no longer appears to be attached to the Server.",
                            "Volume Missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                catch (Exception ex)
                {
                    buttonDisable.Enabled = true;
                    QMessageBox.Show("Exceptions were detected processing the disable request. " + ex.Message, "Severe",
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DisplayErrorMessage(BitLockerVolumeStatus bvms, String messageBeginning,
            String messageBoxTitle)
        {
            String errorMessage, errorCode, errorSymbol;
            errorMessage = ErrorInfo.GetErrorMessage(bvms, out errorSymbol, out errorCode);

            QMessageBox.Show(messageBeginning + " " + errorMessage + "\n\n" +
                    "(" + errorCode + " " + errorSymbol + ")", messageBoxTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        private void lvEncryptableVolumes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvEncryptableVolumes.SelectedItems != null && lvEncryptableVolumes.SelectedItems.Count == 1)
            {
                ListViewItem lvi = lvEncryptableVolumes.SelectedItems[0];
                if (lvi.SubItems[3].Text == "Managed by Taryn BitLocker Manager")
                {
                    buttonEnable.Enabled = false;
                    buttonDisable.Enabled = true;
                }
                else
                {
                    buttonEnable.Enabled = true;
                    buttonDisable.Enabled = false;
                }
            }
            else
            {
                buttonEnable.Enabled = false;
                buttonDisable.Enabled = false;
            }
        }
    }
}
