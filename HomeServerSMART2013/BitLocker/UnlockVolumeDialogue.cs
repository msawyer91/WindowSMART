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
    public partial class UnlockVolumeDialogue : Form
    {
        private bool hasKeyBek;
        private bool hasKeyNumPassword;
        private bool hasKeySmartCard;
        private bool hasKeyPassword;

        /// <summary>
        /// Initializes a new 
        /// </summary>
        /// <param name="bek"></param>
        /// <param name="numPassword"></param>
        /// <param name="smartCard"></param>
        /// <param name="password"></param>
        public UnlockVolumeDialogue(bool bek, bool numPassword, bool smartCard, bool password)
        {
            InitializeComponent();
            if (this.Parent == null)
            {
                this.StartPosition = FormStartPosition.CenterScreen;
            }
            hasKeyBek = bek;
            hasKeyNumPassword = numPassword;
            hasKeySmartCard = smartCard;
            hasKeyPassword = password;
        }

        private void UnlockVolumeDialogue_Load(object sender, EventArgs e)
        {
            rbBek.Enabled = hasKeyBek;
            rbNumPassword.Enabled = hasKeyNumPassword;
            rbSmartCard.Enabled = hasKeySmartCard;
            rbPassword.Enabled = hasKeyPassword;

            if (hasKeyBek)
            {
                rbBek.Checked = true;
            }
            else if (hasKeyNumPassword)
            {
                rbNumPassword.Checked = true;
            }
            else if (hasKeySmartCard)
            {
                rbSmartCard.Checked = true;
            }
            else if (hasKeyPassword)
            {
                rbPassword.Checked = true;
            }
        }

        private void rbBek_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBek.Checked)
            {
                tbBekFile.Enabled = true;
                btnBrowseBek.Enabled = true;
            }
            else
            {
                tbBekFile.Enabled = false;
                btnBrowseBek.Enabled = false;
            }
            ValidateFields();
        }

        private void rbNumPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNumPassword.Checked)
            {
                tbNumPassword.Enabled = true;
                cbShowNumPassword.Enabled = true;
            }
            else
            {
                tbNumPassword.Enabled = false;
                cbShowNumPassword.Enabled = false;
            }
            ValidateFields();
        }

        private void rbSmartCard_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSmartCard.Checked)
            {
                tbSmartCard.Enabled = true;
                btnBrowseCert.Enabled = true;
            }
            else
            {
                tbSmartCard.Enabled = false;
                btnBrowseCert.Enabled = false;
            }
            ValidateFields();
        }

        private void rbPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPassword.Checked)
            {
                tbPassword.Enabled = true;
                cbShowPassword.Enabled = true;
            }
            else
            {
                tbPassword.Enabled = false;
                cbShowPassword.Enabled = false;
            }
            ValidateFields();
        }

        private void btnBrowseBek_Click(object sender, EventArgs e)
        {
            // Prompt the user for the BEK.
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select BitLocker Encryption Key (note BEK files by default are hidden)";
            ofd.Filter = "BitLocker Encryption Keys (*.bek)|*.bek";
            ofd.ShowDialog();

            if (ofd.FileName != String.Empty)
            {
                tbBekFile.Text = ofd.FileName;
            }
            ValidateFields();
        }

        private void btnBrowseCert_Click(object sender, EventArgs e)
        {
            // Thumbprint and PIN will be needed from the user.
            String thumbprint, subjectName;
            thumbprint = String.Empty;
            subjectName = String.Empty;

            // Let the user choose the cert they want.
            thumbprint = Certs.GetThumbprintFromStore(this.Handle, out subjectName);
            if (String.IsNullOrEmpty(thumbprint))
            {
                return;
            }
            else
            {
                tbSmartCard.Text = thumbprint;
            }
            ValidateFields();
        }

        private void ValidateFields()
        {
            if (rbBek.Checked)
            {
                if (String.IsNullOrEmpty(tbBekFile.Text))
                {
                    btnUnlock.Enabled = false;
                }
                else
                {
                    btnUnlock.Enabled = true;
                }
            }
            else if (rbNumPassword.Checked)
            {
                if (String.IsNullOrEmpty(tbNumPassword.Text))
                {
                    btnUnlock.Enabled = false;
                }
                else
                {
                    btnUnlock.Enabled = true;
                }
            }
            else if (rbSmartCard.Checked)
            {
                if(String.IsNullOrEmpty(tbSmartCard.Text))
                {
                    btnUnlock.Enabled = false;
                }
                else
                {
                    btnUnlock.Enabled = true;
                }
            }
            else if (rbPassword.Checked)
            {
                if(String.IsNullOrEmpty(tbPassword.Text))
                {
                    btnUnlock.Enabled = false;
                }
                else
                {
                    btnUnlock.Enabled = true;
                }
            }
            else
            {
                btnUnlock.Enabled = false;
            }
        }

        private void cbShowNumPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (cbShowNumPassword.Checked)
            {
                tbNumPassword.PasswordChar = '\0';
            }
            else
            {
                tbNumPassword.PasswordChar = '●';
            }
        }

        private void cbShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (cbShowPassword.Checked)
            {
                tbPassword.PasswordChar = '\0';
            }
            else
            {
                tbPassword.PasswordChar = '●';
            }
        }

        /// <summary>
        /// Gets a flag that indicates whether an external key (BEK) is used to unlock the volume.
        /// </summary>
        public bool UnlockWithBek
        {
            get
            {
                return rbBek.Checked;
            }
        }

        /// <summary>
        /// Gets a flag that indicates whether a numeric password is used to unlock the volume.
        /// </summary>
        public bool UnlockWithNumPassword
        {
            get
            {
                return rbNumPassword.Checked;
            }
        }

        /// <summary>
        /// Gets a flag that indicates whether a smart card certificate is used to unlock the volume.
        /// </summary>
        public bool UnlockWithSmartCard
        {
            get
            {
                return rbSmartCard.Checked;
            }
        }

        /// <summary>
        /// Gets a flag that indicates whether an alphanumeric password is used to unlock the volume.
        /// </summary>
        public bool UnlockWithPassword
        {
            get
            {
                return rbPassword.Checked;
            }
        }

        /// <summary>
        /// Gets the full path and filename to the target external key (BEK) file.
        /// </summary>
        public String KeyFileName
        {
            get
            {
                return tbBekFile.Text;
            }
        }

        /// <summary>
        /// Gets the 48-digit password.
        /// </summary>
        public String NumericPassword
        {
            get
            {
                return tbNumPassword.Text;
            }
        }

        /// <summary>
        /// Gets the smart card certificate thumbprint.
        /// </summary>
        public String CertificateThumbprint
        {
            get
            {
                return tbSmartCard.Text;
            }
        }

        /// <summary>
        /// Gets the alphanumeric password.
        /// </summary>
        public String Password
        {
            get
            {
                return tbPassword.Text;
            }
        }

        private void tbNumPassword_TextChanged(object sender, EventArgs e)
        {
            ValidateFields();
        }

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {
            ValidateFields();
        }
    }
}
