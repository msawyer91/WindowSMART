using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Snarl.V42;
using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class SnarlConfiguration : Form
    {
        private bool isSnarlEnabled;
        private Microsoft.Win32.RegistryKey configurationKey;
        private bool isWindowsServerSolutions;

        public SnarlConfiguration(Microsoft.Win32.RegistryKey key, bool isWss)
        {
            InitializeComponent();
            isSnarlEnabled = false;
            configurationKey = key;
            isWindowsServerSolutions = isWss;
        }

        private void SnarlConfiguration_Load(object sender, EventArgs e)
        {
            try
            {
                isSnarlEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsSnarlEnabled));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsSnarlEnabled, isSnarlEnabled);
            }

            cbEnableSnarl.Checked = isSnarlEnabled;
        }

        public bool SnarlEnabled
        {
            get
            {
                return isSnarlEnabled;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            SaveConfiguration();
            if (!isWindowsServerSolutions && isSnarlEnabled)
            {
                QMessageBox.Show("Since Snarl notifications have been enabled on this computer, the WindowSMART tray application will no longer " +
                    "display notifications. If you disable Snarl alerts in the future, the WindowSMART tray application will resume notifications.",
                    "Tray App Disabled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void SaveConfiguration()
        {
            try
            {
                isSnarlEnabled = cbEnableSnarl.Checked;

                configurationKey.SetValue(Properties.Resources.RegistryConfigIsSnarlEnabled, isSnarlEnabled);
            }
            catch (Exception ex)
            {
                QMessageBox.Show("Save failed; one or more configured values have been lost. " + ex.Message, "Severe",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonValidate_Click(object sender, EventArgs e)
        {
            if (SnarlInterface.GetSnarlWindow() == IntPtr.Zero)
            {
                QMessageBox.Show("Snarl was not detected. Either Snarl is not installed, or the Snarl client is not running. Please start Snarl and try again.",
                    "Validate Snarl Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            int result = 0;
            String applicationPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            String snarlIcon = applicationPath + "\\" + Properties.Resources.IconAlertGeneral;
            String appTitle = isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart;

            SnarlInterface si = new SnarlInterface();
            result = SnarlInterface.GetVersion();

            result = si.Register("application/windowsmart2015", appTitle + " Test", snarlIcon);

            if (result < (int)SnarlInterface.SnarlStatus.Success)
            {
                QMessageBox.Show("Failed to register with Snarl. The Snarl API returned the error code " + ((SnarlInterface.SnarlStatus)(Math.Abs(result))).ToString(),
                    "Validate Snarl Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            si.AddClass("General Events", appTitle + " Test", String.Empty, String.Empty, applicationPath + "\\" + Properties.Resources.IconAlertHyperfatal,
                String.Empty, 15, String.Empty, true);

            si.Notify("General Events", "Testing", "Congratulations! You have successfully set up Snarl notifications!", 30);

            System.Threading.Thread.Sleep(2000);

            si.Unregister();

            QMessageBox.Show("Three requests were sent to Snarl: register, notify and unregister. Each of these messages should have generated a separate Snarl " +
                "notification. Please check your Snarl client and verify these messages were received.", "Validate Snarl Configuration", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void cbEnableSnarl_CheckedChanged(object sender, EventArgs e)
        {
            buttonValidate.Enabled = cbEnableSnarl.Checked;
        }
    }
}
