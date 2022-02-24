using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public partial class AddPassphraseDialogue : Form
    {
        private String passphrase;
        private int minimumLength;

        public AddPassphraseDialogue(int minLength, bool isChange)
        {
            GPConfig.ReloadConfiguration();
            InitializeComponent();
            passphrase = String.Empty;
            minimumLength = minLength;
            labelPW.Text = "Enter Password (" + minimumLength.ToString() + " characters minimum)";

            if (isChange)
            {
                this.Text = "Change Password";
                labelTitle.Text = "Change a Volume Password";
                labelDescription.Text = labelDescription.Text.Replace("add a", "change the");
            }
        }

        public String Passphrase
        {
            get
            {
                return passphrase;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (textBoxPassword.Text.Length < minimumLength)
            {
                MessageBox.Show("Password must be at least " + minimumLength.ToString() + " characters long.", "Password",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (textBoxPassword.Text != textBoxConfirm.Text)
            {
                MessageBox.Show("Password entered do not match.", "Password",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxConfirm.Text = String.Empty;
                textBoxPassword.Text = String.Empty;
                return;
            }

            passphrase = textBoxPassword.Text;
            btnAdd.DialogResult = DialogResult.OK;
            this.DialogResult = DialogResult.OK;
        }
    }
}
