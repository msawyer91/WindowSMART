using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public partial class UsbDiskChooser : Form
    {
        string driveLetter;

        public UsbDiskChooser()
        {
            InitializeComponent();
            driveLetter = String.Empty;
        }

        private void UsbDiskChooser_Load(object sender, EventArgs e)
        {
            DriveInfo[] ListDrives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in ListDrives)
            {
                if (drive != null && drive.IsReady && drive.DriveType == DriveType.Removable)
                {
                    if (String.IsNullOrEmpty(drive.VolumeLabel))
                    {
                        comboBoxDisks.Items.Add(drive.Name.Substring(0, 2) + " {" + drive.DriveFormat + "}");
                    }
                    else
                    {
                        comboBoxDisks.Items.Add(drive.Name.Substring(0, 2) + " {" + drive.VolumeLabel + ", " + drive.DriveFormat + "}");
                    }
                }
            }

            if (comboBoxDisks.Items.Count > 0)
            {
                comboBoxDisks.SelectedIndex = 0;

                if (comboBoxDisks.Items.Count == 1)
                {
                }
            }
        }

        private void UsbDiskChooser_Shown(object sender, EventArgs e)
        {
            if (comboBoxDisks.Items.Count == 0)
            {
                MessageBox.Show("No USB disks were detected. Startup and Recovery Keys must be saved " +
                    "to a USB disk. Please attach a USB disk, preferably a flash drive, " +
                    "and try again.", "No USB Disks", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                driveLetter = String.Empty;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            driveLetter = comboBoxDisks.SelectedItem.ToString().Substring(0, 2) + "\\";
            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            driveLetter = String.Empty;
            this.DialogResult = DialogResult.Cancel;
        }

        public String SelectedDrive
        {
            get
            {
                return driveLetter;
            }
        }
    }
}
