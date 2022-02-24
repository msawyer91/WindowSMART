using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

using  WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public partial class EncryptSystemVolumeDialogue : Form
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

        /// <summary>
        /// Initializes a new instance of the EncryptSystemVolumeDialogue.
        /// </summary>
        /// <param name="isWin7Family"></param>
        /// <param name="drive"></param>
        public EncryptSystemVolumeDialogue(bool isWin7Family, String drive, String label)
        {
            GPConfig.ReloadConfiguration();
            InitializeComponent();
            isWindows7Family = isWin7Family;
            volume = drive;
            volumeLabel = label;
        }

        private void EncryptSystemVolumeDialogue_Load(object sender, EventArgs e)
        {
            // Enable/dim options based on Group Policy and OS version.
            // Encryption Method
            if (GPConfig.Method == EncryptionMethod.DEFAULT_METHOD)
            {
                encryptionMethod.SelectedIndex = 1;
                encryptionMethod.Enabled = true;
            }
            else
            {
                switch (GPConfig.Method)
                {
                    case EncryptionMethod.AES_128:
                        {
                            encryptionMethod.SelectedIndex = 2;
                            encryptionMethod.Enabled = false;
                            break;
                        }
                    case EncryptionMethod.AES_128_DIFFUSER:
                        {
                            encryptionMethod.SelectedIndex = 0;
                            encryptionMethod.Enabled = false;
                            break;
                        }
                    case EncryptionMethod.AES_256:
                        {
                            encryptionMethod.SelectedIndex = 3;
                            encryptionMethod.Enabled = false;
                            break;
                        }
                    case EncryptionMethod.AES_256_DIFFUSER:
                        {
                            encryptionMethod.SelectedIndex = 1;
                            encryptionMethod.Enabled = false;
                            break;
                        }
                    default:
                        {
                            encryptionMethod.SelectedIndex = 1;
                            encryptionMethod.Enabled = true;
                            break;
                        }
                }
            }

            createStartupKey = false;
            createTpmKey = false;

            // 48-digit recovery password (affected by FIPS)
            if (GPConfig.IsFipsComplianceMandatory)
            {
                cbRecoveryPw.Checked = false;
                cbRecoveryPw.Enabled = false;
                btnBrowseRecoveryPassword.Enabled = false;
                createAutoNumericPassword = false;
                createUserNumericPassword = false;
                allowAutoNumericPassword = false;
            }
            else if ((isWindows7Family && GPConfig.Win7OsRequireNumericPw) ||
                (!isWindows7Family && GPConfig.VistaRequireNumericPw))
            {
                cbRecoveryPw.Checked = true;
                cbRecoveryPw.Enabled = false;
                btnBrowseRecoveryPassword.Enabled = true;
                createUserNumericPassword = true;
                createAutoNumericPassword = false;
                allowAutoNumericPassword = false;
            }
            else if ((isWindows7Family && !GPConfig.Win7OsAllowNumericPw) ||
                (!isWindows7Family && !GPConfig.VistaAllowNumericPw))
            {
                cbRecoveryPw.Checked = false;
                cbRecoveryPw.Enabled = false;
                btnBrowseRecoveryPassword.Enabled = false;
                createUserNumericPassword = false;
                createAutoNumericPassword = true;
                allowAutoNumericPassword = true;
            }
            else
            {
                cbRecoveryPw.Checked = true;
                cbRecoveryPw.Enabled = true;
                btnBrowseRecoveryPassword.Enabled = true;
                createUserNumericPassword = true;
                createAutoNumericPassword = false;
                allowAutoNumericPassword = true;
            }

            // Recovery Key
            if ((isWindows7Family && GPConfig.Win7OsRequireBek) ||
                (!isWindows7Family && GPConfig.VistaRequireBek))
            {
                cbRecoveryKey.Checked = true;
                cbRecoveryKey.Enabled = false;
                btnBrowseRecoveryKey.Enabled = true;
            }
            else if ((isWindows7Family && !GPConfig.Win7OsAllowBek) ||
                (!isWindows7Family && !GPConfig.VistaAllowBek))
            {
                cbRecoveryKey.Checked = false;
                cbRecoveryKey.Enabled = false;
                btnBrowseRecoveryKey.Enabled = false;
            }
            else
            {
                cbRecoveryKey.Checked = true;
                cbRecoveryKey.Enabled = true;
                btnBrowseRecoveryKey.Enabled = true;
            }

            // TPM
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
                    QMessageBox.Show("BitLocker cannot encrypt your volume. There are conflicting Group Policy settings. " +
                        "BitLocker only allows one TPM startup option, but more than one startup option has been defined " +
                        "as Mandatory in Group Policy. Contact your system administrator for assistance.", "Policy Conflict",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DimEverything();
                    this.Close();
                    return;
                }
                else if (forbiddenItems == 4 && GPConfig.IsTpmUsable)
                {
                    QMessageBox.Show("BitLocker cannot encrypt your volume. There are conflicting Group Policy settings. " +
                        "BitLocker requires at least one TPM startup option to be available, but all of the startup options " +
                        "have been defined as Forbidden in Group Policy. Contact your system administrator for assistance.", "Policy Conflict",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DimEverything();
                    this.Close();
                    return;
                }
                else if (!GPConfig.Win7AllowNonTpm && !GPConfig.IsTpmUsable)
                {
                    QMessageBox.Show("BitLocker cannot encrypt your volume. Group Policy settings on this computer requires " +
                        "a Trusted Platform Module (TPM). A compatible TPM was not detected. Contact your system administrator " +
                        "for assistance.", "TPM Not Detected",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DimEverything();
                    this.Close();
                    return;
                }
                else if (GPConfig.Win7OsRequireNumericPw && GPConfig.IsFipsComplianceMandatory)
                {
                    QMessageBox.Show("BitLocker cannot encrypt your volume. FIPS compliance is enforced on this computer, " +
                    "and Recovery Passwords are enforced by policy. FIPS compliance prohibits the use of Recovery Passwords. " +
                    "Encryption of the volume is impossible until these conflicting settings are resolved. Contact your " +
                    "system administrator for assistance.", "Policy Conflict", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DimEverything();
                    this.Close();
                    return;
                }
                else if (!GPConfig.Win7OsAllowBek && GPConfig.IsFipsComplianceMandatory)
                {
                    QMessageBox.Show("BitLocker cannot encrypt your volume. FIPS compliance is enforced on this computer, " +
                    "and Recovery Keys are disabled by policy. Therefore no recovery mechanism is available for this " +
                    "drive (FIPS prevents Recovery Passwords and Key Escrow to Active Directory). Encryption of the " +
                    "volume is impossible until these conflicting settings are resolved. Contact your system administrator " +
                    "for assistance.", "Policy Conflict", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DimEverything();
                    this.Close();
                    return;
                }
                else if (!GPConfig.Win7OsAllowBek && !GPConfig.Win7OsAllowNumericPw &&
                    GPConfig.OsActiveDirectoryBackup.Contains("No"))
                {
                    QMessageBox.Show("BitLocker cannot encrypt your volume. There are conflicting Group Policy settings. " +
                    "All recovery mechanisms (Recovery Key, Recovery Password and Active Directory Key Escrow) are disabled. Encryption of the " +
                    "volume is impossible until these conflicting settings are resolved. Contact your system administrator " +
                    "for assistance.", "Policy Conflict", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DimEverything();
                    this.Close();
                    return;
                }

                if (GPConfig.Win7AllowNonTpm && !GPConfig.IsTpmUsable)
                {
                    cbNoTpm.Checked = true;
                    cbNoTpm.Enabled = false;
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
                    cbNoTpm.Checked = false;
                    cbNoTpm.Enabled = false;
                }
            }

            if (GPConfig.IsTpmUsable)
            {
                tpmStartupOption.Enabled = true;
            }

            // Enhanced PIN?
            if (isWindows7Family && GPConfig.Win7OsAllowEnhancedPin)
            {
                labelPin.Text = "PIN (min " + GPConfig.Win7OsMinimumPin.ToString() + " characters)";
            }
            else if (isWindows7Family)
            {
                labelPin.Text = "PIN (min " + GPConfig.Win7OsMinimumPin.ToString() + " numerals)";
            }
            else
            {
                labelPin.Text = "PIN (min " + GPConfig.MinimumPin.ToString() + " numerals)";
            }

            // Active Directory Info
            if (isWindows7Family)
            {
                if (GPConfig.OsRequireActiveDirectoryBackup == "Required"
                    && DomainInfo.IsInDomain())
                {
                    labelAD.Text = "Your system administrator requires that BitLocker key information " +
                        "be escrowed to Active Directory. If you are not connected to your company's " +
                        "domain, the encryption will fail to start. Ensure you are connected to the " +
                        "domain via the network, VPN or dial-up.";
                }
                else if (GPConfig.OsRequireActiveDirectoryBackup == "Required")
                {
                    labelAD.Text = "Your system administrator requires that BitLocker key information " +
                        "be escrowed to Active Directory. However, your computer is not joined to an Active " +
                        "Directory domain. Encryption will fail to start. Contact your system administrator " +
                        "to fix this invalid policy setting.";
                }
                else
                {
                    labelAD.Text = "There are no Active Directory enforced requirements. You may begin " +
                        "encryption regardless of your domain connectivity.";
                }
            }
            else
            {
                if (GPConfig.VistaRequireKeyEscrow == "Required"
                    && DomainInfo.IsInDomain())
                {
                    labelAD.Text = "Your system administrator requires that BitLocker key information " +
                        "be escrowed to Active Directory. If you are not connected to your company's " +
                        "domain, the encryption will fail to start. Ensure you are connected to the " +
                        "domain via the network, VPN or dial-up.";
                }
                else if (GPConfig.OsRequireActiveDirectoryBackup == "Required")
                {
                    labelAD.Text = "Your system administrator requires that BitLocker key information " +
                        "be escrowed to Active Directory. However, your computer is not joined to an Active " +
                        "Directory domain. Encryption will fail to start. Contact your system administrator " +
                        "to fix this invalid policy setting.";
                }
                else
                {
                    labelAD.Text = "There are no Active Directory enforced requirements. You may begin " +
                        "encryption regardless of your domain connectivity.";
                }
            }
        }

        private void DimEverything()
        {
            encryptionMethod.Enabled = false;
            tpmStartupOption.Enabled = false;
            cbRecoveryPw.Checked = false;
            cbRecoveryPw.Enabled = false;
            cbRecoveryKey.Checked = false;
            cbRecoveryKey.Enabled = false;
            btnBrowseRecoveryKey.Enabled = false;
            btnBrowseRecoveryPassword.Enabled = false;
            btnBrowseStartupKey.Enabled = false;
            tbPin.ReadOnly = true;
            tbConfirmPin.ReadOnly = true;
            btnBrowseStartupKey.Enabled = false;
            cbPerformCheck.Enabled = false;
            cbReboot.Enabled = false;
            cbSkipCheck.Enabled = false;
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
                        QMessageBox.Show("Path " + path + " cannot be used. Home Server SMART attempted to write a temporary " +
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

        private void btnEncrypt_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cbPerformCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPerformCheck.Checked)
            {
                cbReboot.Enabled = true;
                cbSkipCheck.Checked = false;
                cbSkipCheck.Enabled = false;
            }
            else
            {
                cbReboot.Checked = false;
                cbReboot.Enabled = false;
                cbSkipCheck.Enabled = true;
            }
            VerifyFieldsAndSetEncryptButton();
        }

        private void btnBrowseRecoveryPassword_Click(object sender, EventArgs e)
        {
            String path = String.Empty;
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            bool waitForUser = true;
            String oldValue = tbRecoveryPwLocation.Text;

            while (waitForUser)
            {
                fbd.Description = "Select folder in which to save password.";
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && fbd.SelectedPath != String.Empty)
                {
                    path = fbd.SelectedPath;
                    waitForUser = false;

                    if (!waitForUser)
                    {
                        waitForUser = UtilityMethods.AreDisksSame(volume, path.Substring(0, 2));
                    }

                    if (!waitForUser)
                    {
                        waitForUser = UtilityMethods.IsComposeTextFileError(path);
                    }

                    if (!waitForUser)
                    {
                        // All is well, so update field.
                        tbRecoveryPwLocation.Text = path;
                        VerifyFieldsAndSetEncryptButton();
                    }
                    else
                    {
                        tbRecoveryPwLocation.Text = oldValue;
                        VerifyFieldsAndSetEncryptButton();
                    }
                }
                else
                {
                    waitForUser = false;
                    tbRecoveryPwLocation.Text = oldValue;
                    VerifyFieldsAndSetEncryptButton();
                    return;
                }
            }
        }

        private void btnBrowseRecoveryKey_Click(object sender, EventArgs e)
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

                    waitForUser = UtilityMethods.AreDisksSame(volume, path.Substring(0, 2));

                    if (!waitForUser)
                    {
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
                }
                else
                {
                    waitForUser = false;
                    return;
                }
            }
            // All is well, so update field.
            tbRecoveryKeyLocation.Text = path;
            VerifyFieldsAndSetEncryptButton();
        }

        private void cbRecoveryPw_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRecoveryPw.Checked)
            {
                // If this box is checked, we need the save option available.
                btnBrowseRecoveryPassword.Enabled = true;
                tbRecoveryPwLocation.Enabled = true;
                createUserNumericPassword = true;
                createAutoNumericPassword = false;
            }
            else
            {
                btnBrowseRecoveryPassword.Enabled = false;
                tbRecoveryPwLocation.Enabled = false;
                createUserNumericPassword = false;
                if (allowAutoNumericPassword)
                {
                    createAutoNumericPassword = true;
                }
            }

            VerifyFieldsAndSetEncryptButton();
        }

        private void cbRecoveryKey_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRecoveryKey.Checked)
            {
                // If this box is checked, we need the save option available.
                btnBrowseRecoveryKey.Enabled = true;
                tbRecoveryKeyLocation.Enabled = true;
                createRecoveryKey = true;
            }
            else
            {
                btnBrowseRecoveryKey.Enabled = false;
                tbRecoveryKeyLocation.Enabled = false;
                createRecoveryKey = false;
            }

            VerifyFieldsAndSetEncryptButton();
        }

        private void VerifyFieldsAndSetEncryptButton()
        {
            // Don't allow encryption unless fields are all valid.

            if ((cbRecoveryPw.Checked) &&
                String.IsNullOrEmpty(tbRecoveryPwLocation.Text))
            {
                btnEncrypt.Enabled = false;
                return;
            }
            if ((cbRecoveryKey.Checked) &&
                String.IsNullOrEmpty(tbRecoveryKeyLocation.Text))
            {
                btnEncrypt.Enabled = false;
                return;
            }
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
            if (!cbPerformCheck.Checked && !cbSkipCheck.Checked)
            {
                btnEncrypt.Enabled = false;
                return;
            }

            createRecoveryKey = cbRecoveryKey.Checked;
            createUserNumericPassword = cbRecoveryPw.Checked;

            btnEncrypt.Enabled = true;
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

        private void tbPin_Leave(object sender, EventArgs e)
        {
            CheckPasswordFields();
            VerifyFieldsAndSetEncryptButton();
        }

        private void tbConfirmPin_Leave(object sender, EventArgs e)
        {
            CheckPasswordFields();
            VerifyFieldsAndSetEncryptButton();
        }

        private void CheckPasswordFields()
        {
            if (tbPin.ReadOnly || tbConfirmPin.ReadOnly)
            {
                tbPin.Text = String.Empty;
                tbConfirmPin.Text = String.Empty;
                return;
            }

            // If both fields have text in them, compare them to be sure they match.
            if (!String.IsNullOrEmpty(tbPin.Text) &&
                !String.IsNullOrEmpty(tbConfirmPin.Text) &&
                tbPin.Text != tbConfirmPin.Text)
            {
                QMessageBox.Show("The values for PIN and Confirm PIN do not match.",
                    "PIN Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                tbPin.Text = String.Empty;
                tbConfirmPin.Text = String.Empty;
                tbPin.Focus();
                return;
            }

            if (!GPConfig.Win7OsAllowEnhancedPin)
            {
                System.Text.RegularExpressions.Regex objNaturalPattern =
                    new System.Text.RegularExpressions.Regex(@"^[0-9]+$");
                if (!String.IsNullOrEmpty(tbPin.Text) && !String.IsNullOrEmpty(tbConfirmPin.Text) &&
                    !objNaturalPattern.IsMatch(tbPin.Text))
                {
                    QMessageBox.Show("Only numerals are allowed in the PIN.", "Numeric PIN",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tbPin.Text = String.Empty;
                    tbConfirmPin.Text = String.Empty;
                    tbPin.Focus();
                    return;
                }
            }

            if (!String.IsNullOrEmpty(tbPin.Text) &&
                !String.IsNullOrEmpty(tbConfirmPin.Text) &&
                tbPin.Text.Length < GPConfig.Win7OsMinimumPin)
            {
                QMessageBox.Show("Your PIN does not meet the minimum length of " +
                    GPConfig.Win7OsMinimumPin.ToString() + " characters.",
                    "PIN Length", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                tbPin.Text = String.Empty;
                tbConfirmPin.Text = String.Empty;
                tbPin.Focus();
                return;
            }
        }

        public bool CreateUserNumericPassword
        {
            get
            {
                return createUserNumericPassword;
            }
        }

        public bool CreateAutoNumericPassword
        {
            get
            {
                return createAutoNumericPassword;
            }
        }

        public bool CreateRecoveryKey
        {
            get
            {
                return createRecoveryKey;
            }
        }

        public bool CreatePin
        {
            get
            {
                return createPin;
            }
        }

        public bool CreateStartupKey
        {
            get
            {
                if (createTpmKey || createStartupKey)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public String TpmType
        {
            get
            {
                return tpmStartupOption.SelectedItem.ToString();
            }
        }

        public bool PerformSystemCheck
        {
            get
            {
                return cbPerformCheck.Checked;
            }
        }

        public bool RebootImmediately
        {
            get
            {
                return cbReboot.Checked;
            }
        }

        public String StartupKeyPath
        {
            get
            {
                return tbStartupKeyLocation.Text;
            }
        }

        public String RecoveryKeyPath
        {
            get
            {
                return tbRecoveryKeyLocation.Text;
            }
        }

        public String RecoveryPasswordPath
        {
            get
            {
                return tbRecoveryPwLocation.Text;
            }
        }

        public String Volume
        {
            get
            {
                return volume;
            }
        }

        public String VolumeLabel
        {
            get
            {
                return volumeLabel;
            }
        }

        public String Pin
        {
            get
            {
                return tbPin.Text;
            }
        }

        public bool AllowPrintingAndSaving
        {
            get
            {
                return true;
            }
        }

        public EncryptionMethod Method
        {
            get
            {
                switch (encryptionMethod.SelectedItem.ToString())
                {
                    case "AES-128 with Diffuser":
                        {
                            return EncryptionMethod.AES_128_DIFFUSER;
                        }
                    case "AES-128":
                        {
                            return EncryptionMethod.AES_128;
                        }
                    case "AES-256":
                        {
                            return EncryptionMethod.AES_256;
                        }
                    case "AES-256 with Diffuser":
                        {
                            return EncryptionMethod.AES_256_DIFFUSER;
                        }
                    default:
                        {
                            return EncryptionMethod.DEFAULT_METHOD;
                        }
                }
            }
        }

        private void cbSkipCheck_CheckedChanged(object sender, EventArgs e)
        {
            VerifyFieldsAndSetEncryptButton();
        }
    }
}
