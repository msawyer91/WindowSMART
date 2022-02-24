using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class Welcome : Form
    {
        private DateTime expirationDate;
        private bool isExpired;
        private bool isBeta;
        private uint errorCode;
        private uint exitCode = 0xFFFFFFFF;
        private static int WM_QUERYENDSESSION = 0x11;
        private static int WM_ENDSESSION = 0x16;

        public Welcome(DateTime dt, bool expired, bool beta, uint code)
        {
            InitializeComponent();
            expirationDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, DateTimeKind.Local);
            isExpired = expired;
            isBeta = beta;
            errorCode = code;
        }

        private void Welcome_Load(object sender, EventArgs e)
        {
            if (isExpired)
            {
                pictureBox1.Image = Properties.Resources.Coins128;
                if (expirationDate.Year == 1980 && expirationDate.Month == 1 && expirationDate.Day == 1)
                {
                    buttonContinue.Enabled = isBeta;
                    labelTrialExpiration.Text = "The licensing system detected tampering. (0x" + errorCode.ToString("X") + ")";
                }
                else if (errorCode != 0x0 && errorCode != 0x1)
                {
                    labelTrialExpiration.Text = "The licensing system detected tampering. (0x" + errorCode.ToString("X") + ")";
                    buttonContinue.Enabled = isBeta;
                }
                else
                {
                    buttonContinue.Enabled = isBeta;
                    labelTrialExpiration.Text = "Your trial expired on " + expirationDate.ToShortDateString() + " at " + expirationDate.ToShortTimeString() + ".";
                }
            }
            else
            {
                buttonContinue.Enabled = true;
                labelTrialExpiration.Text = "Your trial will expire on " + expirationDate.ToShortDateString() + " at " + expirationDate.ToShortTimeString() + ".";
            }
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            exitCode = 0x2;
            this.Close();
        }

        private void buttonEnterLicense_Click(object sender, EventArgs e)
        {
            Licensing.Registration reg = new Licensing.Registration();
            reg.ShowDialog();
            if (reg.CommitCode == 0x0)
            {
                exitCode = 0x0;
                this.Close();
            }
        }

        private void buttonPurchase_Click(object sender, EventArgs e)
        {
            Utilities.Utility.LaunchBrowser(Properties.Resources.PurchaseUrl);
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            exitCode = 0x1;
            this.Close();
        }

        public uint ExitCode
        {
            get
            {
                return exitCode;
            }
        }

        private void Welcome_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                SiAuto.Main.LogWarning("Windows has issued a stern shutdown order - WindowsShutDown message received.");
                exitCode = 0x1;
                e.Cancel = false;
            }
            else if (exitCode == 0xFFFFFFFF)
            {
                e.Cancel = true;
            }
        }
    }
}
