using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public partial class NumericPasswordDialogue : Form
    {
        private String passwordKeyId;

        public NumericPasswordDialogue(String id)
        {
            GPConfig.ReloadConfiguration();
            InitializeComponent();
            passwordKeyId = id;
            labelID.Text = "Your password can be identified by the ID " + passwordKeyId + ".";
        }

        private void checkBoxMask_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMask.Checked)
            {
                tbPassword.PasswordChar = '●';
            }
            else
            {
                tbPassword.PasswordChar = '\0';
            }
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            
        }

        public String Password
        {
            get
            {
                return tbPassword.Text;
            }
        }
    }
}
