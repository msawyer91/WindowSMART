using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Growl.Connector;
using Gurock.SmartInspect;
using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class GrowlConfiguration : Form
    {
        private bool isGrowlEnabled;
        private bool isGrowlRemoteEnabled;
        private String growlRemoteTarget;
        private int growlPort;
        private String growlPassword;
        private Microsoft.Win32.RegistryKey configurationKey;
        private bool isWindowsServerSolutions;
        private DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Cryptography.DoubleEncryptor crypto;

        private GrowlConnector growl;
        private NotificationType notificationType;

        public GrowlConfiguration(Microsoft.Win32.RegistryKey key, bool isWss, DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Cryptography.DoubleEncryptor crypt)
        {
            InitializeComponent();
            isGrowlEnabled = false;
            isGrowlRemoteEnabled = false;
            growlRemoteTarget = String.Empty;
            growlPort = 23053;
            growlPassword = String.Empty;
            configurationKey = key;
            isWindowsServerSolutions = isWss;
            if (isWindowsServerSolutions)
            {
                label3.Text = label3.Text.Replace("WindowSMART", "Home Server SMART");
            }
            crypto = crypt;
        }

        private void cbEnableProwl_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnableNma.Checked)
            {
                rbDisplayLocal.Enabled = true;
                rbDeliverRemote.Enabled = true;
                cbCustomPort.Enabled = rbDeliverRemote.Checked;
                customPort.Enabled = cbCustomPort.Checked;
                tbGrowlTarget.Enabled = rbDeliverRemote.Checked;
                tbGrowlPassword.Enabled = true;
                buttonValidate.Enabled = true;
            }
            else
            {
                rbDisplayLocal.Enabled = false;
                rbDeliverRemote.Enabled = false;
                cbCustomPort.Enabled = false;
                customPort.Enabled = false;
                tbGrowlTarget.Enabled = false;
                tbGrowlPassword.Enabled = false;
                buttonValidate.Enabled = false;
            }
        }

        private void ProwlConfiguration_Load(object sender, EventArgs e)
        {
            try
            {
                isGrowlEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsGrowlEnabled));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsGrowlEnabled, isGrowlEnabled);
            }

            try
            {
                isGrowlRemoteEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsGrowlRemoteEnabled));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsGrowlRemoteEnabled, isGrowlRemoteEnabled);
            }

            try
            {
                growlRemoteTarget = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigGrowlRemoteMachine);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigGrowlRemoteMachine, growlRemoteTarget);
            }

            try
            {
                growlPort = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGrowlRemotePort);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigGrowlRemotePort, growlPort);
            }

            try
            {
                growlPassword = crypto.Decrypt((String)configurationKey.GetValue(Properties.Resources.RegistryConfigGrowlPassword));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigGrowlPassword, crypto.Encrypt(growlPassword));
            }

            cbEnableNma.Checked = isGrowlEnabled;
            if (isGrowlRemoteEnabled)
            {
                rbDeliverRemote.Checked = true;
            }
            else
            {
                rbDisplayLocal.Checked = true;
            }
            tbGrowlTarget.Text = growlRemoteTarget;
            customPort.Value = growlPort;
            if (growlPort != 23053)
            {
                cbCustomPort.Checked = true;
            }
            if (growlPassword == "N@yNayDefined")
            {
                tbGrowlPassword.Text = String.Empty;
            }
            else
            {
                tbGrowlPassword.Text = growlPassword;
            }
        }

        public bool GrowlEnabled
        {
            get
            {
                return isGrowlEnabled;
            }
        }

        public bool GrowlRemoteEnabled
        {
            get
            {
                return isGrowlRemoteEnabled;
            }
        }

        public String GrowlRemoteTarget
        {
            get
            {
                return growlRemoteTarget;
            }
        }

        public int GrowlRemotePort
        {
            get
            {
                return growlPort;
            }
        }

        public String GrowlPassword
        {
            get
            {
                return growlPassword;
            }
        }

        private void buttonValidate_Click(object sender, EventArgs e)
        {
            isGrowlEnabled = cbEnableNma.Checked;
            isGrowlRemoteEnabled = rbDeliverRemote.Checked;
            growlRemoteTarget = tbGrowlTarget.Text;
            growlPort = (int)customPort.Value;
            growlPassword = String.IsNullOrEmpty(tbGrowlPassword.Text) ? "N@yNayDefined" : tbGrowlPassword.Text;

            try
            {
                if (!cbEnableNma.Checked)
                {
                    QMessageBox.Show("You must enable Growl notifications. Please check the \"Enable Growl Notifications\" checkbox and try again.",
                        "Register with Growl", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else if (isGrowlRemoteEnabled)
                {
                    SiAuto.Main.LogString("growlPassword", growlPassword);
                    SiAuto.Main.LogString("growlRemoteTarget", growlRemoteTarget);
                    SiAuto.Main.LogInt("growlPort", growlPort);
                    growl = new GrowlConnector(growlPassword, growlRemoteTarget, growlPort);
                }
                else
                {
                    growl = new GrowlConnector(tbGrowlPassword.Text);
                }

                growl.NotificationCallback += new GrowlConnector.CallbackEventHandler(growl_NotificationCallback);
                growl.EncryptionAlgorithm = Growl.Connector.Cryptography.SymmetricAlgorithmType.PlainText;

                Utilities.GrowlNotificationTypes gnt = new Utilities.GrowlNotificationTypes(isWindowsServerSolutions);

                growl.Register(gnt.GrowlApplication, new NotificationType[] { gnt.Critical, gnt.AppWarning, gnt.Cleared, gnt.General, gnt.Hyperfatal, gnt.Warning });

                Notification notificate = new Notification(gnt.GrowlApplication.Name, gnt.Hyperfatal.Name, DateTime.Now.Ticks.ToString(),
                    "Test Notification", "Congratulations! You have successfully registered and configured Growl notifications!", gnt.Hyperfatal.Icon,
                    false, Priority.Moderate, String.Empty);
                growl.Notify(notificate);

                QMessageBox.Show("A Growl registration request, and a test Growl alert, was sent to Growl. Please open your Growl client and verify that an application " +
                    "named " + (isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart) + " has " +
                    "registered itself. If it is registered, you must save the changes in the Growl Configuration dialogue.\n\nYou do not need to re-" +
                    "register with Growl unless you delete the registration from Growl, or if you change the target computer, port or password.", "Register with Growl",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                QMessageBox.Show("An error was reported by the Growl API: " + ex.Message, "Growl Registration Failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            SaveConfiguration();
            if (!isWindowsServerSolutions && isGrowlEnabled && !isGrowlRemoteEnabled)
            {
                QMessageBox.Show("Since Growl notifications have been enabled on this computer, the WindowSMART tray application will no longer " +
                    "display notifications. If you disable Growl alerts in the future, the WindowSMART tray application will resume notifications.",
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
                isGrowlEnabled = cbEnableNma.Checked;
                isGrowlRemoteEnabled = rbDeliverRemote.Checked;
                growlRemoteTarget = tbGrowlTarget.Text;
                growlPort = (int)customPort.Value;

                if (String.IsNullOrEmpty(tbGrowlPassword.Text))
                {
                    growlPassword = "N@yNayDefined";
                }
                else
                {
                    growlPassword = tbGrowlPassword.Text;
                }

                configurationKey.SetValue(Properties.Resources.RegistryConfigIsGrowlEnabled, isGrowlEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsGrowlRemoteEnabled, isGrowlRemoteEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigGrowlRemoteMachine, growlRemoteTarget);
                configurationKey.SetValue(Properties.Resources.RegistryConfigGrowlRemotePort, growlPort);
                configurationKey.SetValue(Properties.Resources.RegistryConfigGrowlPassword, crypto.Encrypt(growlPassword));
            }
            catch (Exception ex)
            {
                QMessageBox.Show("Save failed; one or more configured values have been lost. " + ex.Message, "Severe",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rbDeliverRemote_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDeliverRemote.Checked)
            {
                tbGrowlTarget.Enabled = true;
                cbCustomPort.Enabled = true;
                customPort.Enabled = cbCustomPort.Checked;
            }
            else
            {
                tbGrowlTarget.Enabled = false;
                cbCustomPort.Enabled = false;
                customPort.Enabled = false;
            }
        }

        private void cbCustomPort_CheckedChanged(object sender, EventArgs e)
        {
            customPort.Enabled = cbCustomPort.Checked;
        }

        private void growl_NotificationCallback(Response response, CallbackData callbackData, object state)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.UserControls.GrowlConfiguration.growl_NotificationCallback");
            string text = String.Format("Response Type: {0}\r\nNotification ID: {1}\r\nCallback Data: {2}\r\nCallback Data Type: {3}\r\n", callbackData.Result, callbackData.NotificationID, callbackData.Data, callbackData.Type);
            SiAuto.Main.LogMessage("Growl Callback Received - " + text);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.UserControls.GrowlConfiguration.growl_NotificationCallback");
        }
    }
}
