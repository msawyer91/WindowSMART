using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public partial class GroupPolicy : Form
    {
        private bool isWindows7Family;
        
        public GroupPolicy(bool windows7)
        {
            InitializeComponent();

            isWindows7Family = windows7;
        }

        private void GroupPolicy_Load(object sender, EventArgs e)
        {
            GPConfig.ReloadConfiguration();

            if (isWindows7Family)
            {
                pictureBox2.Image = Properties.Resources.status_warning_16;
                labelVista.Text = "You are running a Windows Server Solutions product. The below policies do not apply to you, but are provided for your reference.";
                pictureBox3.Image = Properties.Resources.Information16;
                labelWin7.Text = "You are running a Windows Server Solutions product. The below policies apply to you.";
            }
            else
            {
                pictureBox2.Image = Properties.Resources.Information16;
                labelVista.Text = "You are running Windows Vista/Server 2008. The below policies apply to you.";
                pictureBox3.Image = Properties.Resources.status_warning_16;
                labelWin7.Text = "You are running Windows Vista/Server 2008. The below policies do not apply to you, but are provided for your reference.";
            }

            tbRequireKeyEscrow.Text = GPConfig.VistaRequireKeyEscrow;
            tbKeyEscrow.Text = GPConfig.VistaKeyEscrow;
            tbKeyStorage.Text = GPConfig.VistaKeyStorage;
            tbAllowWithoutTpm.Text = GPConfig.VistaAllowWithoutTpm;
            tbStartupKey.Text = GPConfig.VistaStartupKey;
            tbStartupPin.Text = GPConfig.VistaStartupPin;
            tbRequiredEncryptionMethod.Text = GPConfig.AllRequiredEncryptionMethod;
            tbAllowBek.Text = GPConfig.VistaAllAllowBek;
            tbAllowPassword.Text = GPConfig.VistaAllAllowPassword;
            tbDefaultFolder.Text = GPConfig.AllDefaultFolder;
            tbMemoryOverwrite.Text = GPConfig.AllMemoryOverwrite;

            tbBitLockerIdField.Text = GPConfig.IdentificationFieldString;
            tbSecondaryIdentification.Text = GPConfig.SecondaryIdentificationField;
            tbCertificateOid.Text = GPConfig.CertificateOid;

            // RDV Volume Windows 7/Server 2008 R2
            tbRdvAllowBde.Text = GPConfig.RdvAllowBde;
            tbRdvConfigureBde.Text = GPConfig.RdvConfigureBde;
            tbRdvDisableBde.Text = GPConfig.RdvDisableBde;
            tbRdvAllowUserCert.Text = GPConfig.RdvAllowUserCert;
            tbRdvEnforceUserCert.Text = GPConfig.RdvEnforceUserCert;
            tbRdvDenyWriteAccess.Text = GPConfig.RdvDenyWriteAccess;
            tbRdvDenyCrossOrg.Text = GPConfig.RdvDenyCrossOrg;
            tbRdvDiscoveryVolumeType.Text = GPConfig.RdvDiscoveryVolumeType;
            tbRdvNoBitLockerToGoReader.Text = GPConfig.RdvNoBitLockerToGoReader;
            tbRdvEnforcePassphrase.Text = GPConfig.RdvEnforcePassphrase;
            tbRdvPassphrase.Text = GPConfig.RdvPassphrase;
            tbRdvPassphraseComplexity.Text = GPConfig.RdvPassphraseComplexity;
            tbRdvMinimumPassphraseLength.Text = GPConfig.RdvPassphraseLength;
            tbRdvActiveDirectoryBackup.Text = GPConfig.RdvActiveDirectoryBackup;
            tbRdvActiveDirectoryInfoToStore.Text = GPConfig.RdvActiveDirectoryInfoToStore;
            tbRdvHideRecoveryPage.Text = GPConfig.RdvHideRecoveryPage;
            tbRdvManageDra.Text = GPConfig.RdvManageDra;
            tbRdvRecovery.Text = GPConfig.RdvRecovery;
            tbRdvRecoveryKey.Text = GPConfig.RdvRecoveryKey;
            tbRdvRecoveryPassword.Text = GPConfig.RdvRecoveryPassword;
            tbRdvRequireActiveDirectoryBackup.Text = GPConfig.RdvRequireActiveDirectoryBackup;

            // FDV Volume Windows 7/Server 2008 R2
            tbFdvAllowUserCert.Text = GPConfig.FdvAllowUserCert;
            tbFdvEnforceUserCert.Text = GPConfig.FdvEnforceUserCert;
            tbFdvDenyWriteAccess.Text = GPConfig.FdvDenyWriteAccess;
            tbFdvDiscoveryVolumeType.Text = GPConfig.FdvDiscoveryVolumeType;
            tbFdvNoBitLockerToGoReader.Text = GPConfig.FdvNoBitLockerToGoReader;
            tbFdvEnforcePassphrase.Text = GPConfig.FdvEnforcePassphrase;
            tbFdvPassphrase.Text = GPConfig.FdvPassphrase;
            tbFdvPassphraseComplexity.Text = GPConfig.FdvPassphraseComplexity;
            tbFdvMinimumPassphraseLength.Text = GPConfig.FdvPassphraseLength;
            tbFdvActiveDirectoryBackup.Text = GPConfig.FdvActiveDirectoryBackup;
            tbFdvActiveDirectoryInfoToStore.Text = GPConfig.FdvActiveDirectoryInfoToStore;
            tbFdvHideRecoveryPage.Text = GPConfig.FdvHideRecoveryPage;
            tbFdvManageDra.Text = GPConfig.FdvManageDra;
            tbFdvRecovery.Text = GPConfig.FdvRecovery;
            tbFdvRecoveryKey.Text = GPConfig.FdvRecoveryKey;
            tbFdvRecoveryPassword.Text = GPConfig.FdvRecoveryPassword;
            tbFdvRequireActiveDirectoryBackup.Text = GPConfig.FdvRequireActiveDirectoryBackup;
            
            // OS Volume Windows 7/Server 2008 R2
            tbOsRecovery.Text = GPConfig.OsRecovery;
            tbOsActiveDirectoryBackup.Text = GPConfig.OsActiveDirectoryBackup;
            tbOsActiveDirectoryInfoToStore.Text = GPConfig.OsActiveDirectoryInfoToStore;
            tbOsAllowDra.Text = GPConfig.OsManageDra;
            tbOsHideRecoveryPage.Text = GPConfig.OsHideRecoveryPage;
            tbOsRecoveryKey.Text = GPConfig.OsRecoveryKey;
            tbOsRecoveryPassword.Text = GPConfig.OsRecoveryPassword;
            tbOsRequireActiveDirectoryBackup.Text = GPConfig.OsRequireActiveDirectoryBackup;
            tbUseAdvancedStartup.Text = GPConfig.UseAdvancedStartup;
            tbEnableBdeWithNoTpm.Text = GPConfig.EnableBdeWithNoTpm;
            tbUseTpm.Text = GPConfig.UseTpm;
            tbUseTpmKey.Text = GPConfig.UseTpmKey;
            tbUseTpmPin.Text = GPConfig.UseTpmPin;
            tbUseTpmKeyPin.Text = GPConfig.UseTpmKeyPin;
            tbMinimumPin.Text = GPConfig.MinimumPin;
            tbUseEnhancedPin.Text = GPConfig.UseEnhancedPin;

            DisplayOrHideErrorBulletsPolicy();

            if (GPConfig.IsFipsComplianceMandatory)
            {
                pictureBoxFips.Visible = true;
                labelFips.Visible = true;
            }
        }

        private void DisplayOrHideErrorBulletsPolicy()
        {
            int tpmItemsMandatory = 0;
            int tpmItemsForbidden = 0;

            // VALID conditions:  0 mandatory, 0 forbidden; 1 mandatory, 3 forbidden; 0 mandatory, up to 3 forbidden
            // INVALID conditions:  >1 mandatory; 4 forbidden

            // Check each and increment.
            if (GPConfig.UseTpm == "Mandatory")
            {
                tpmItemsMandatory++;
            }
            else if (GPConfig.UseTpm == "Forbidden")
            {
                tpmItemsForbidden++;
            }
            if (GPConfig.UseTpmKey == "Mandatory")
            {
                tpmItemsMandatory++;
            }
            else if (GPConfig.UseTpmKey == "Forbidden")
            {
                tpmItemsForbidden++;
            }
            if (GPConfig.UseTpmKeyPin == "Mandatory")
            {
                tpmItemsMandatory++;
            }
            else if (GPConfig.UseTpmKeyPin == "Forbidden")
            {
                tpmItemsForbidden++;
            }
            if (GPConfig.UseTpmPin == "Mandatory")
            {
                tpmItemsMandatory++;
            }
            else if (GPConfig.UseTpmPin == "Forbidden")
            {
                tpmItemsForbidden++;
            }

            if ((tpmItemsMandatory == 0 && tpmItemsForbidden == 0) ||
                (tpmItemsMandatory == 1 && tpmItemsForbidden == 3) ||
                (tpmItemsMandatory == 0 && tpmItemsForbidden < 4))
            {
                // configuration valid
                return;
            }

            // Based on what is set, we'll highlight error bullets.
            if (tpmItemsMandatory > 1)
            {
                // All marked "mandatory" will be flagged.
                if (GPConfig.UseTpm == "Mandatory")
                {
                    pbPolErrorTpm.Visible = true;
                    toolTipPolicyErrorTooManyMandatory.SetToolTip(pbPolErrorTpm, "You cannot set more than one TPM option to Mandatory. " +
                        "If one is set to Mandatory, all others must be set to Forbidden. Contact your system administrator.");
                }
                if (GPConfig.UseTpmKey == "Mandatory")
                {
                    pbPolErrorTpmKey.Visible = true;
                    toolTipPolicyErrorTooManyMandatory.SetToolTip(pbPolErrorTpmKey, "You cannot set more than one TPM option to Mandatory. " +
                        "If one is set to Mandatory, all others must be set to Forbidden. Contact your system administrator.");
                }
                if (GPConfig.UseTpmPin == "Mandatory")
                {
                    pbPolErrorTpmPin.Visible = true;
                    toolTipPolicyErrorTooManyMandatory.SetToolTip(pbPolErrorTpmPin, "You cannot set more than one TPM option to Mandatory. " +
                        "If one is set to Mandatory, all others must be set to Forbidden. Contact your system administrator.");
                }
                if (GPConfig.UseTpmKeyPin == "Mandatory")
                {
                    pbPolErrorTpmKeyPin.Visible = true;
                    toolTipPolicyErrorTooManyMandatory.SetToolTip(pbPolErrorTpmKeyPin, "You cannot set more than one TPM option to Mandatory. " +
                        "If one is set to Mandatory, all others must be set to Forbidden. Contact your system administrator.");
                }
            }
            else if (tpmItemsMandatory == 1 && tpmItemsForbidden < 3)
            {
                // All marked "allow user to create or skip" will be flagged. If one is mandatory, all others MUST be forbidden.
                // This won't be a Policy Error, though...we'll call it a warning.
                if (GPConfig.UseTpm == "Allow User to Create or Skip")
                {
                    pbPolErrorTpm.Visible = true;
                    toolTipPolicyNotEnoughForbidden.SetToolTip(pbPolErrorTpm, "The policy is inconsistent. It is not valid to set a TPM " +
                        "option to Mandatory, while leaving others at Allow (they should be set to Forbidden). The Mandatory item will be " +
                        "enforced.");
                }
                if (GPConfig.UseTpmKey == "Allow User to Create or Skip")
                {
                    pbPolErrorTpmKey.Visible = true;
                    toolTipPolicyNotEnoughForbidden.SetToolTip(pbPolErrorTpmKey, "The policy is inconsistent. It is not valid to set a TPM " +
                        "option to Mandatory, while leaving others at Allow (they should be set to Forbidden). The Mandatory item will be " +
                        "enforced.");
                }
                if (GPConfig.UseTpmPin == "Allow User to Create or Skip")
                {
                    pbPolErrorTpmPin.Visible = true;
                    toolTipPolicyNotEnoughForbidden.SetToolTip(pbPolErrorTpmPin, "The policy is inconsistent. It is not valid to set a TPM " +
                        "option to Mandatory, while leaving others at Allow (they should be set to Forbidden). The Mandatory item will be " +
                        "enforced.");
                }
                if (GPConfig.UseTpmKeyPin == "Allow User to Create or Skip")
                {
                    pbPolErrorTpmKeyPin.Visible = true;
                    toolTipPolicyNotEnoughForbidden.SetToolTip(pbPolErrorTpmKeyPin, "The policy is inconsistent. It is not valid to set a TPM " +
                        "option to Mandatory, while leaving others at Allow (they should be set to Forbidden). The Mandatory item will be " +
                        "enforced.");
                }
            }
            else
            {
                // Only other possible is all forbidden, so flag 'em all.
                pbPolErrorTpm.Visible = true;           
                pbPolErrorTpmKey.Visible = true;           
                pbPolErrorTpmPin.Visible = true;
                pbPolErrorTpmKeyPin.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
