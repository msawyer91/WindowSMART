using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using RestSharp;
using DojoNorthSoftware.Pushover;
using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class ProwlConfiguration : Form
    {
        private bool isProwlEnabled;
        private bool isPushoverEnabled;
        private String prowlApiKey;
        private String pushoverUserKey;
        private String pushoverClearedSound;
        private String pushoverCriticalSound;
        private String pushoverWarningSound;
        private String pushoverDeviceTarget;
        private Microsoft.Win32.RegistryKey configurationKey;
        private bool isWindowsServerSolutions;
        private int prowlGpoSetting;
        private int pushoverGpoSetting;
        private const String PUSHOVER_DEFAULT_SOUND = "(Device default sound)";

        public ProwlConfiguration(Microsoft.Win32.RegistryKey key, bool isWss, int gpoProwl, int gpoPushover)
        {
            InitializeComponent();
            isProwlEnabled = false;
            isPushoverEnabled = false;
            prowlApiKey = String.Empty;
            pushoverUserKey = String.Empty;
            pushoverClearedSound = PUSHOVER_DEFAULT_SOUND;
            pushoverCriticalSound = PUSHOVER_DEFAULT_SOUND;
            pushoverWarningSound = PUSHOVER_DEFAULT_SOUND;
            pushoverDeviceTarget = String.Empty;

            configurationKey = key;
            isWindowsServerSolutions = isWss;
            prowlGpoSetting = gpoProwl;
            pushoverGpoSetting = gpoPushover;

            comboBoxCleared.SelectedIndex = 0;
            comboBoxCritical.SelectedIndex = 0;
            comboBoxWarning.SelectedIndex = 0;
        }

        private void cbEnableProwl_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnableProwl.Checked)
            {
                tbProwlApiKey.Enabled = true;
                buttonValidate.Enabled = true;
            }
            else
            {
                tbProwlApiKey.Enabled = false;
                buttonValidate.Enabled = false;
            }
        }

        private void ProwlConfiguration_Load(object sender, EventArgs e)
        {
            try
            {
                isProwlEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsProwlEnabled));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsProwlEnabled, isProwlEnabled);
            }

            try
            {
                prowlApiKey = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigProwlApiKeychain);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigProwlApiKeychain, prowlApiKey);
            }

            try
            {
                isPushoverEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsPushoverEnabled));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsPushoverEnabled, isPushoverEnabled);
            }

            try
            {
                pushoverUserKey = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigPushoverKey);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverKey, pushoverUserKey);
            }

            try
            {
                pushoverClearedSound = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigPushoverClearedSound);
                if (String.IsNullOrEmpty(pushoverClearedSound))
                {
                    pushoverClearedSound = PUSHOVER_DEFAULT_SOUND;
                }
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverClearedSound, PUSHOVER_DEFAULT_SOUND);
            }

            try
            {
                pushoverCriticalSound = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigPushoverCriticalSound);
                if (String.IsNullOrEmpty(pushoverCriticalSound))
                {
                    pushoverCriticalSound = PUSHOVER_DEFAULT_SOUND;
                }
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverCriticalSound, PUSHOVER_DEFAULT_SOUND);
            }

            try
            {
                pushoverWarningSound = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigPushoverWarningSound);
                if (String.IsNullOrEmpty(pushoverWarningSound))
                {
                    pushoverWarningSound = PUSHOVER_DEFAULT_SOUND;
                }
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverWarningSound, PUSHOVER_DEFAULT_SOUND);
            }

            try
            {
                pushoverDeviceTarget = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigPushoverDeviceTarget);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverDeviceTarget, pushoverDeviceTarget);
            }

            cbEnableProwl.Checked = isProwlEnabled;
            tbProwlApiKey.Text = prowlApiKey;
            cbEnablePushover.Checked = isPushoverEnabled;
            tbPushoverKey.Text = pushoverUserKey;
            tbPushoverDevice.Text = pushoverDeviceTarget;
            comboBoxCleared.SelectedItem = pushoverClearedSound;
            comboBoxCritical.SelectedItem = pushoverCriticalSound;
            comboBoxWarning.SelectedItem = pushoverWarningSound;

            if (prowlGpoSetting < 2)
            {
                cbEnableProwl.Enabled = false;
                tbProwlApiKey.Enabled = false;
            }
            if (pushoverGpoSetting < 2)
            {
                cbEnablePushover.Enabled = false;
                tbPushoverKey.Enabled = false;
                tbPushoverDevice.Enabled = false;
                comboBoxCleared.Enabled = false;
                comboBoxCritical.Enabled = false;
                comboBoxWarning.Enabled = false;
            }
        }

        public bool ProwlEnabled
        {
            get
            {
                return isProwlEnabled;
            }
        }

        public String ProwlApiKey
        {
            get
            {
                return prowlApiKey;
            }
        }

        public bool PushoverEnabled
        {
            get
            {
                return isPushoverEnabled;
            }
        }

        public String PushoverUserKey
        {
            get
            {
                return pushoverUserKey;
            }
        }

        public String PushoverDeviceTarget
        {
            get
            {
                return pushoverDeviceTarget;
            }
        }

        public String PushoverClearedSound
        {
            get
            {
                return pushoverClearedSound;
            }
        }

        public String PushoverCriticalSound
        {
            get
            {
                return pushoverCriticalSound;
            }
        }

        public String PushoverWarningSound
        {
            get
            {
                return pushoverWarningSound;
            }
        }

        private void buttonValidate_Click(object sender, EventArgs e)
        {
            // Cull spaces
            tbProwlApiKey.Text = tbProwlApiKey.Text.Replace(" ", String.Empty);
            
            Prowl.ProwlClientConfiguration config = new Prowl.ProwlClientConfiguration();
            try
            {
                config.ApiKeychain = tbProwlApiKey.Text;
                config.ProviderKey = isWindowsServerSolutions ? Properties.Resources.ProwlApiKeyHss : Properties.Resources.ProwlApiKeyWs2;
                config.ApplicationName = isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart;
                config.Validate();
            }
            catch (Exception ex)
            {
                QMessageBox.Show("Prowl validation failed. " + ex.Message, "Prowl Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (QMessageBox.Show("Prowl validation successful. Do you want to try sending a test message now?", "Prowl Validation",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Prowl.ProwlNotification notificate = new Prowl.ProwlNotification();
                    notificate.Event = "Prowl Test";
                    notificate.Description = "Congratulations! You have successfully set up Prowl notifications!";
                    notificate.Priority = Prowl.ProwlNotificationPriority.Normal;
                    Prowl.ProwlClient client = new Prowl.ProwlClient(config);
                    client.PostNotification(notificate);
                    QMessageBox.Show("Prowl successfully sent the notification. Please check your device(s) to confirm receipt of the message.",
                        "Prowl Transmitted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                QMessageBox.Show("Prowl messaging failed. " + ex.Message, "Prowl Messaging", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // Cull spaces
            tbProwlApiKey.Text = tbProwlApiKey.Text.Replace(" ", String.Empty);

            SaveConfiguration();
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
                isProwlEnabled = cbEnableProwl.Checked;
                prowlApiKey = tbProwlApiKey.Text;
                isPushoverEnabled = cbEnablePushover.Checked;
                pushoverUserKey = tbPushoverKey.Text;
                pushoverDeviceTarget = tbPushoverDevice.Text;
                pushoverClearedSound = comboBoxCleared.SelectedItem.ToString();
                pushoverCriticalSound = comboBoxCritical.SelectedItem.ToString();
                pushoverWarningSound = comboBoxWarning.SelectedItem.ToString();

                configurationKey.SetValue(Properties.Resources.RegistryConfigIsProwlEnabled, isProwlEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigProwlApiKeychain, prowlApiKey);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsPushoverEnabled, isPushoverEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverKey, pushoverUserKey);
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverClearedSound, pushoverClearedSound);
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverCriticalSound, pushoverCriticalSound);
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverWarningSound, pushoverWarningSound);
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverDeviceTarget, pushoverDeviceTarget);
            }
            catch (Exception ex)
            {
                QMessageBox.Show("Save failed; one or more configured values have been lost. " + ex.Message, "Severe",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbEnablePushover_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnablePushover.Checked)
            {
                tbPushoverKey.Enabled = true;
                tbPushoverDevice.Enabled = true;
                comboBoxCleared.Enabled = true;
                comboBoxCritical.Enabled = true;
                comboBoxWarning.Enabled = true;
                buttonValidatePushover.Enabled = true;
            }
            else
            {
                tbPushoverKey.Enabled = false;
                tbPushoverDevice.Enabled = false;
                comboBoxCleared.Enabled = false;
                comboBoxCritical.Enabled = false;
                comboBoxWarning.Enabled = false;
                buttonValidatePushover.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbPushoverKey.Text))
            {
                QMessageBox.Show("You must specify a Pushover user key.", "Key Missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                if (QMessageBox.Show("Pushover validation successful. Do you want to try sending a test message now? Please note the test message will be sent " +
                    "using the Critical sound.", "Pushover Validation", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Exception except;
                    bool success = false;
                    if (String.IsNullOrEmpty(tbPushoverDevice.Text))
                    {
                        success = Pushover.Pushover.SendNotification(Properties.Resources.PushoverApiToken, tbPushoverKey.Text, "Pushover Notification Setup",
                            "Congratulations, you have successfully set up Pushover notifications!", Pushover.Priority.Normal,
                            Utilities.Utility.ResolveSoundFromString(comboBoxCritical.SelectedItem.ToString()), out except);
                    }
                    else
                    {
                        success = Pushover.Pushover.SendNotification(Properties.Resources.PushoverApiToken, tbPushoverKey.Text, "Pushover Notification Setup",
                            "Congratulations, you have successfully set up Pushover notifications on device " + tbPushoverDevice.Text + "!", Pushover.Priority.Normal,
                            Utilities.Utility.ResolveSoundFromString(comboBoxCritical.SelectedItem.ToString()), tbPushoverDevice.Text, out except);
                    }

                    if (success)
                    {
                        QMessageBox.Show("Pushover notification sent successfully. Please check device(s) to confirm receipt.", "Pushover Notification",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        if (except == null)
                        {
                            QMessageBox.Show("Pushover notification failed. Unfortunately, Pushover did not provide any further error details.", "Pushover Notification",
                                MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                        else
                        {
                            QMessageBox.Show("Pushover notification failed. Pushover returned the following error, which may aid troubleshooting: " + except.Message,
                                "Pushover Notification", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
