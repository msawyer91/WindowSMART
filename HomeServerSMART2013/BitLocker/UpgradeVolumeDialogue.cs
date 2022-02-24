using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using  WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public partial class UpgradeVolumeDialogue : Form
    {
        private HardDisk hardDisk;

        public UpgradeVolumeDialogue(HardDisk hd)
        {
            GPConfig.ReloadConfiguration();
            InitializeComponent();
            hardDisk = hd;
        }

        private void UpgradeVolumeDialogue_Load(object sender, EventArgs e)
        {
            if (hardDisk.BitLockerVersion == MetadataVersion.VERSION_VISTA)
            {
                currentBitLockerVersion.Text = "Windows Vista";
                newBitLockerVersion.Items.Add("Windows Vista");
                newBitLockerVersion.Items.Add("Windows 7");
                newBitLockerVersion.SelectedIndex = 0;
            }
            else if (hardDisk.BitLockerVersion == MetadataVersion.VERSION_WIN7)
            {
                currentBitLockerVersion.Text = "Windows 7";
                newBitLockerVersion.Items.Add("Windows 7");
                newBitLockerVersion.SelectedIndex = 0;
                newBitLockerVersion.Enabled = false;
                buttonUpgrade.Enabled = false;
                QMessageBox.Show("Volume " + hardDisk.DriveLetter + "'s BitLocker metadata is at the highest possible version.  It cannot " +
                    "be upgraded.", "Upgrade BitLocker Version", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }
            else
            {
                currentBitLockerVersion.Text = "Unknown";
                newBitLockerVersion.Items.Add("Not Available");
                newBitLockerVersion.SelectedIndex = 0;
                newBitLockerVersion.Enabled = false;
                buttonUpgrade.Enabled = false;
                MessageBox.Show("Volume " + hardDisk.DriveLetter + "'s BitLocker metadata is invalid or the volume is not encrypted.  It cannot " +
                    "be upgraded.", "Upgrade BitLocker Version", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }
        }

        private void buttonUpgrade_Click(object sender, EventArgs e)
        {
            if (newBitLockerVersion.SelectedItem.ToString() != "Windows 7")
            {
                buttonUpgrade.DialogResult = DialogResult.Cancel;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            if (QMessageBox.Show("Upgrading the BitLocker metadata is irreversible, and you will not be able to unlock the volume with " +
                "Windows Vista or Windows Server 2008 again (only NTFS volumes can be upgraded, and the BitLocker To Go Reader is not " +
                "compatible with NTFS).  If you want to use this volume with Vista or Server 2008 again, you will either need to " +
                "completely decrypt it or reformat it.\n\nAre you sure you want to upgrade the BitLocker metadata on volume " +
                hardDisk.DriveLetter + "?", "Upgrade BitLocker Version", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                == DialogResult.Yes)
            {
                if (QMessageBox.Show("YOU ARE ABOUT TO COMMIT A PERMANENT CHANGE TO YOUR VOLUME.\n\n" +
                    "This operation will commit immediately and cannot be rolled back!  You will render this volume " +
                    "permanently unusable with Windows Vista and Windows Server 2008 unless you decrypt or reformat " +
                    "the volume.\n\n" +
                    "Are you ABSOLUTELY, POSITIVELY sure you want to upgrade the BitLocker version on volume " +
                    hardDisk.DriveLetter + "?", "WARNING!", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2)
                    == DialogResult.Yes)
                {
                    buttonUpgrade.DialogResult = DialogResult.OK;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
}
