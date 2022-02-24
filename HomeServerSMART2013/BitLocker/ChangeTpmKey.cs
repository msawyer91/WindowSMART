using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public partial class ChangeTpmKey : Form
    {
        bool isWindows7Family;

        bool createUserNumericPassword;
        bool createRecoveryKey;
        bool createPin;
        bool createTpmKey;
        bool createStartupKey;
        bool createAutoNumericPassword;
        bool allowAutoNumericPassword;

        private String volume;
        private String volumeLabel;

        public ChangeTpmKey(bool isWin7Family, String drive, String label)
        {
            GPConfig.ReloadConfiguration();
            InitializeComponent();
            isWindows7Family = isWin7Family;
            volume = drive;
            volumeLabel = label;
        }

        private void btnBrowseStartupKey_Click(object sender, EventArgs e)
        {
            String path = String.Empty;
            UsbDiskChooser udc = new UsbDiskChooser();
            bool waitForUser = true;

            while (waitForUser)
            {
                udc.ShowDialog();
                if (udc.SelectedDrive != String.Empty)
                {
                    path = udc.SelectedDrive;

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
                        MessageBox.Show("Path " + path + " cannot be used. Home Server SMART attempted to write a temporary " +
                            "file to this location (prior to External Key Protector generation) to validate your ability to write " +
                            "a file to this location. That attempt failed. Please choose another location.", "Path Not Available",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    // All is well, so update field.
                    tbStartupKeyLocation.Text = path;
                }
                else
                {
                    waitForUser = false;
                    return;
                }
            }
            VerifyFieldsAndSetEncryptButton();
        }

        private void ChangeTpmKey_Load(object sender, EventArgs e)
        {
            labelSysVol.Text = volume;
            labelVolumeLabel.Text = volumeLabel;

            if (isWindows7Family)
            {
                int mandatoryItems = 0;
                int forbiddenItems = 0;
                int selectedIndex = 0;

                if (GPConfig.UseTpm == "Mandatory")
                {
                    selectedIndex = tpmStartupOption.Items.Add("TPM Only");
                    tpmStartupOption.SelectedIndex = selectedIndex;
                    tpmStartupOption.Enabled = false;
                    mandatoryItems++;
                }
                else if (GPConfig.UseTpm != "Forbidden")
                {
                    tpmStartupOption.Items.Add("TPM Only");
                }
                else
                {
                    forbiddenItems++;
                }

                if (GPConfig.UseTpmKey == "Mandatory")
                {
                    selectedIndex = tpmStartupOption.Items.Add("TPM + Startup Key");
                    tpmStartupOption.SelectedIndex = selectedIndex;
                    tpmStartupOption.Enabled = false;
                    mandatoryItems++;
                    createTpmKey = true;
                }
                else if (GPConfig.UseTpmKey != "Forbidden")
                {
                    tpmStartupOption.Items.Add("TPM + Startup Key");
                }
                else
                {
                    forbiddenItems++;
                }

                if (GPConfig.UseTpmPin == "Mandatory")
                {
                    selectedIndex = tpmStartupOption.Items.Add("TPM + PIN");
                    tpmStartupOption.SelectedIndex = selectedIndex;
                    tpmStartupOption.Enabled = false;
                    createPin = true;
                    mandatoryItems++;
                }
                else if (GPConfig.UseTpmPin != "Forbidden")
                {
                    tpmStartupOption.Items.Add("TPM + PIN");
                }
                else
                {
                    forbiddenItems++;
                }

                if (GPConfig.UseTpmKeyPin == "Mandatory")
                {
                    selectedIndex = tpmStartupOption.Items.Add("TPM + Startup Key + PIN");
                    tpmStartupOption.SelectedIndex = selectedIndex;
                    tpmStartupOption.Enabled = false;
                    createPin = true;
                    mandatoryItems++;
                    createTpmKey = true;
                }
                else if (GPConfig.UseTpmKeyPin != "Forbidden")
                {
                    tpmStartupOption.Items.Add("TPM + Startup Key + PIN");
                }
                else
                {
                    forbiddenItems++;
                }

                if (mandatoryItems == 0 && forbiddenItems < 4)
                {
                    tpmStartupOption.SelectedIndex = 0;
                    if (tpmStartupOption.SelectedItem.ToString().Contains("PIN"))
                    {
                        createPin = true;
                        tbPin.ReadOnly = false;
                        tbConfirmPin.ReadOnly = false;
                    }
                    else
                    {
                        createPin = false;
                        tbPin.ReadOnly = true;
                        tbConfirmPin.ReadOnly = true;
                    }
                }
                else if (mandatoryItems > 1 || forbiddenItems == 4)
                {
                    tpmStartupOption.Items.Clear();
                    selectedIndex = tpmStartupOption.Items.Add("ERROR: Policy Conflict");
                    tpmStartupOption.SelectedIndex = selectedIndex;
                    tpmStartupOption.Enabled = false;
                }

                if (tpmStartupOption.SelectedItem.ToString().Contains("Startup Key"))
                {
                    btnBrowseStartupKey.Enabled = true;
                    tbStartupKeyLocation.Enabled = true;
                }

                if (mandatoryItems > 1 && GPConfig.IsTpmUsable)
                {
                    MessageBox.Show("BitLocker cannot modify your volume. There are conflicting Group Policy settings. " +
                        "BitLocker only allows one TPM startup option, but more than one startup option has been defined " +
                        "as Mandatory in Group Policy. Contact your system administrator for assistance.", "Policy Conflict",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DimEverything();
                }
                else if (forbiddenItems == 4 && GPConfig.IsTpmUsable)
                {
                    MessageBox.Show("BitLocker cannot modify your volume. There are conflicting Group Policy settings. " +
                        "BitLocker requires at least one TPM startup option to be available, but all of the startup options " +
                        "have been defined as Forbidden in Group Policy. Contact your system administrator for assistance.", "Policy Conflict",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DimEverything();
                }
                else if (!GPConfig.Win7AllowNonTpm && !GPConfig.IsTpmUsable)
                {
                    MessageBox.Show("BitLocker cannot modify your volume. Group Policy settings on this computer requires " +
                        "a Trusted Platform Module (TPM). A compatible TPM was not detected. Contact your system administrator " +
                        "for assistance.", "TPM Not Detected",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DimEverything();
                }

                if (GPConfig.Win7AllowNonTpm && !GPConfig.IsTpmUsable)
                {
                    //cbNoTpm.Checked = true;
                    //cbNoTpm.Enabled = false;
                    int index = tpmStartupOption.Items.Add("TPM Not Installed in Computer");
                    tpmStartupOption.SelectedIndex = index;
                    tpmStartupOption.Enabled = false;
                    tbPin.ReadOnly = true;
                    tbConfirmPin.ReadOnly = true;
                    btnBrowseStartupKey.Enabled = true;
                    tbStartupKeyLocation.Enabled = true;
                    createPin = false;
                    createStartupKey = true;
                }
                else
                {
                    //cbNoTpm.Checked = false;
                    //cbNoTpm.Enabled = false;
                }
            }
        }

        private void tpmStartupOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tpmStartupOption.SelectedItem.ToString().Contains("Startup Key"))
            {
                btnBrowseStartupKey.Enabled = true;
                tbStartupKeyLocation.Enabled = true;
                createTpmKey = true;
            }
            else
            {
                btnBrowseStartupKey.Enabled = false;
                tbStartupKeyLocation.Enabled = false;
                createTpmKey = false;
            }

            if (tpmStartupOption.SelectedItem.ToString().Contains("PIN"))
            {
                tbPin.ReadOnly = false;
                tbConfirmPin.ReadOnly = false;
                createPin = true;
            }
            else
            {
                tbPin.ReadOnly = true;
                tbConfirmPin.ReadOnly = true;
                createPin = false;
            }

            VerifyFieldsAndSetEncryptButton();
        }

        private void tbPin_TextChanged(object sender, EventArgs e)
        {
            if (tbPin.Text == tbConfirmPin.Text && tbPin.Text.Length >= GPConfig.Win7OsMinimumPin)
            {
                VerifyFieldsAndSetEncryptButton();
            }
            else
            {
                btnEncrypt.Enabled = false;
            }
        }

        private void tbConfirmPin_TextChanged(object sender, EventArgs e)
        {
            if (tbPin.Text == tbConfirmPin.Text && tbPin.Text.Length >= GPConfig.Win7OsMinimumPin)
            {
                VerifyFieldsAndSetEncryptButton();
            }
            else
            {
                btnEncrypt.Enabled = false;
            }
        }

        private void VerifyFieldsAndSetEncryptButton()
        {
            // Don't allow encryption unless fields are all valid.

            if ((createPin) &&
                ((String.IsNullOrEmpty(tbPin.Text) || String.IsNullOrEmpty(tbConfirmPin.Text)) ||
                (tbPin.Text != tbConfirmPin.Text)))
            {
                btnEncrypt.Enabled = false;
                return;
            }
            if ((createStartupKey || createTpmKey)
                && String.IsNullOrEmpty(tbStartupKeyLocation.Text))
            {
                btnEncrypt.Enabled = false;
                return;
            }

            btnEncrypt.Enabled = true;
        }

        private void DimEverything()
        {
            tpmStartupOption.Enabled = false;
            btnBrowseStartupKey.Enabled = false;
            tbPin.ReadOnly = true;
            tbConfirmPin.ReadOnly = true;
            btnBrowseStartupKey.Enabled = false;
        }
    }
}
