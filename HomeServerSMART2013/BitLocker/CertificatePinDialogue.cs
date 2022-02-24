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
    public partial class CertificateFilePinDialogue : Form
    {
        private String passwordKeyId;

        public CertificateFilePinDialogue()
        {
            GPConfig.ReloadConfiguration();
            InitializeComponent();
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            
        }

        public String Pin
        {
            get
            {
                return tbPassword.Text;
            }
        }
    }
}
