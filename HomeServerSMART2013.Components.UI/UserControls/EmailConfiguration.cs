using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Chilkat;
using Gurock.SmartInspect;
using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class EmailConfiguration : Form
    {
        // Email Configuration ~ Jake Scherer ~
        private String mailServer;
        private int serverPort;
        private String senderFriendlyName;
        private String senderEmailAddress;
        private String recipientFriendlyName;
        private String recipientEmailAddress;
        private String recipientEmail2;
        private String recipientEmail3;
        private String mailUser;
        private String mailPassword;
        private bool isMailConfigValid;
        private bool authenticationEnabled;
        private bool mailAlertsEnabled;
        private bool useSsl;
        private bool sendDailySummary;
        private bool sendPlaintext;

        private Microsoft.Win32.RegistryKey configurationKey;
        private DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Cryptography.DoubleEncryptor crypto;
        
        public EmailConfiguration(Microsoft.Win32.RegistryKey key, DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Cryptography.DoubleEncryptor crypt)
        {
            InitializeComponent();
            configurationKey = key;
            crypto = crypt;
        }

        private void checkBoxEnableAuthentication_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEnableAuthentication.Checked)
            {
                textBoxUsername.Enabled = true;
                textBoxPassword.Enabled = true;
            }
            else
            {
                textBoxUsername.Enabled = false;
                textBoxPassword.Enabled = false;
            }
        }

        #region Email Settings Validation
        /// <summary>
        /// Returns whether or not the email configuration is valid, as well if the configuration is partially defined
        /// (some, but not all, fields are populated). Even one invalid value will render the entire configuration invalid. ~ Jake ~
        /// </summary>
        /// <param name="isPartiallyDefined">true if at least one field is defined; false if all are blank.</param>
        /// <returns>true if the entire configuration is valid; false otherwise.</returns>
        private bool IsEmailConfigurationValid(out bool isPartiallyDefined, out bool isMailEnabled)
        {
            isPartiallyDefined = false;
            isMailEnabled = false;
            bool isError = false;

            // Check that mail is enabled first; we won't display errors if it isn't.
            isMailEnabled = checkBoxEnableEmail.Checked;

            // Mail Server
            if (String.IsNullOrEmpty(textBoxHost.Text))
            {
                mailServer = String.Empty;
                if (isMailEnabled)
                {
                    QMessageBox.Show("Server Hostname is a required field.", Properties.Resources.ErrorMessageTitleInvalidEmail,
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    isError = true;
                }
            }
            else
            {
                mailServer = textBoxHost.Text;
                isPartiallyDefined = true;
            }

            // Sender Name
            if (String.IsNullOrEmpty(textBoxSenderFriendly.Text))
            {
                senderFriendlyName = String.Empty;
                if (isMailEnabled && !isError) // Don't annoy the user with excessive message prompts.
                {
                    QMessageBox.Show("Sender Name is a required field.", Properties.Resources.ErrorMessageTitleInvalidEmail,
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    isError = true;
                }
            }
            else
            {
                senderFriendlyName = textBoxSenderFriendly.Text;
                isPartiallyDefined = true;
            }

            // Sender Email
            if (String.IsNullOrEmpty(textBoxSenderEmailAddy.Text))
            {
                senderEmailAddress = String.Empty;
                if (isMailEnabled && !isError) // Don't annoy the user with excessive message prompts.
                {
                    QMessageBox.Show("Sender Email Address is a required field.", Properties.Resources.ErrorMessageTitleInvalidEmail,
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    isError = true;
                }
            }
            else
            {
                senderEmailAddress = textBoxSenderEmailAddy.Text;
                if (!Utilities.Utility.IsEmailAddressValid(senderEmailAddress) && !isError)
                {
                    QMessageBox.Show("The email address specified for the Sender is not valid.", Properties.Resources.ErrorMessageTitleInvalidEmail,
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    isError = true;
                }
                isPartiallyDefined = true;
            }

            if (String.IsNullOrEmpty(textBoxRecipientName.Text))
            {
                recipientFriendlyName = String.Empty;
                if (isMailEnabled && !isError) // Don't annoy the user with excessive message prompts.
                {
                    QMessageBox.Show("Recipient Name is a required field.", Properties.Resources.ErrorMessageTitleInvalidEmail,
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    isError = true;
                }
            }
            else
            {
                recipientFriendlyName = textBoxRecipientName.Text;
                isPartiallyDefined = true;
            }

            if (String.IsNullOrEmpty(textBoxRecipientEmail.Text))
            {
                recipientEmailAddress = String.Empty;
                if (isMailEnabled && !isError) // Don't annoy the user with excessive message prompts.
                {
                    QMessageBox.Show("Recipient Email Address is a required field.", Properties.Resources.ErrorMessageTitleInvalidEmail,
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    isError = true;
                }
            }
            else
            {
                recipientEmailAddress = textBoxRecipientEmail.Text;
                if (!Utilities.Utility.IsEmailAddressValid(recipientEmailAddress) && !isError)
                {
                    QMessageBox.Show("The email address specified for the Recipient is not valid.", Properties.Resources.ErrorMessageTitleInvalidEmail,
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    isError = true;
                }
                isPartiallyDefined = true;
            }

            if (String.IsNullOrEmpty(textBoxEmailAddy2.Text))
            {
                recipientEmail2 = String.Empty;
            }
            else
            {
                recipientEmail2 = textBoxEmailAddy2.Text;
                if (!Utilities.Utility.IsEmailAddressValid(recipientEmail2))
                {
                    QMessageBox.Show("The email address specified for Recipient 2 is not valid. Correct the email address, or remove it completely.",
                        Properties.Resources.ErrorMessageTitleInvalidEmail, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    isError = true;
                }
            }

            if (String.IsNullOrEmpty(textBoxEmailAddy3.Text))
            {
                recipientEmail3 = String.Empty;
            }
            else
            {
                recipientEmail3 = textBoxEmailAddy3.Text;
                if (!Utilities.Utility.IsEmailAddressValid(recipientEmail3))
                {
                    QMessageBox.Show("The email address specified for Recipient 3 is not valid. Correct the email address, or remove it completely.",
                        Properties.Resources.ErrorMessageTitleInvalidEmail, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    isError = true;
                }
            }

            authenticationEnabled = checkBoxEnableAuthentication.Checked;
            if (authenticationEnabled)
            {
                if (String.IsNullOrEmpty(textBoxUsername.Text))
                {
                    mailUser = "Undefined";
                    QMessageBox.Show("Username must be specified when email authentication is being used.", "Invalid Credentials",
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    isError = true;
                }
                else
                {
                    mailUser = textBoxUsername.Text;
                }

                if (String.IsNullOrEmpty(textBoxPassword.Text))
                {
                    mailPassword = "N0t_Sp3c1fied";
                    QMessageBox.Show("Password must be specified when email authentication is being used.", "Invalid Credentials",
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    isError = true;
                }
                else
                {
                    mailPassword = textBoxPassword.Text;
                }
            }
            else
            {
                if (String.IsNullOrEmpty(textBoxUsername.Text))
                {
                    mailUser = "Undefined";
                }
                else
                {
                    mailUser = textBoxUsername.Text;
                }

                if (String.IsNullOrEmpty(textBoxPassword.Text))
                {
                    mailPassword = "N0t_Sp3c1fied";
                }
                else
                {
                    mailPassword = textBoxPassword.Text;
                }
            }

            useSsl = checkBoxSsl.Checked;
            serverPort = (int)numericUpDownPort.Value;

            sendDailySummary = checkBoxSendSummary.Checked;
            sendPlaintext = checkBoxSendPlaintext.Checked;

            if (useSsl || isMailEnabled)
            {
                isPartiallyDefined = true;
            }

            if ((isPartiallyDefined && isError) || !isPartiallyDefined)
            {
                return false;
            }

            return true;
        }
        #endregion

        private void buttonSendTest_Click(object sender, EventArgs e)
        {
            bool isPartiallyDefined = false;
            isMailConfigValid = IsEmailConfigurationValid(out isPartiallyDefined, out mailAlertsEnabled);

            if (!isMailConfigValid)
            {
                return;
            }

            try
            {
                // Create a new MailMan object, and unlock it.

                MailMan mailman = new MailMan();
                bool success = mailman.UnlockComponent("DOJONO.CB1012021_bKaXHW7WockC");

                if (!success)
                {
                    throw new UnauthorizedAccessException("Cannot activate Chilkat.MailMan email module. The licensing module encountered an exception.");
                }

                // Set the SMTP Server
                mailman.SmtpHost = mailServer;
                mailman.SmtpPort = serverPort;
                mailman.SmtpSsl = useSsl;

                // Creds
                if (authenticationEnabled)
                {
                    mailman.SmtpUsername = mailUser;
                    mailman.SmtpPassword = mailPassword;
                }

                // Create a new email object.
                Email msg = new Email();
                msg.From = senderFriendlyName + " <" + senderEmailAddress + ">";
                msg.AddTo(recipientFriendlyName, recipientEmailAddress);
                if (!String.IsNullOrEmpty(recipientEmail2))
                {
                    msg.AddCC(String.Empty, recipientEmail2);
                }
                if (!String.IsNullOrEmpty(recipientEmail3))
                {
                    msg.AddCC(String.Empty, recipientEmail3);
                }

                msg.Subject = "Home Server SMART/WindowSMART Mail Send Test";
                if (checkBoxSendPlaintext.Checked)
                {
                    msg.Body = "Home Server SMART/WindowSMART Mail Test\r\n\r\nThis message confirms you have configured Home Server SMART/WindowSMART email alerting correctly.";
                }
                else
                {
                    msg.SetHtmlBody("<html><body><h2>Home Server SMART/WindowSMART Mail Test</h2><hr/>This message confirms you have configured Home Server SMART/WindowSMART email alerting correctly.</body></html>");
                }
                msg.AddHeaderField("X-Priority", "1");

                success = mailman.SendEmail(msg);

                if (success)
                {
                    QMessageBox.Show("Test message was sent successfully. Please check your mailbox to confirm receipt.", "Test Message",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    SiAuto.Main.LogFatal(mailman.LastErrorText);
                    if (QMessageBox.Show("Unable to send the test message. Please verify the email address(es) for correctness. If using authentication, please ensure the username and password. Passwords " +
                        "are usually case-sensitive. If using SSL, the port number is most likely 465 or 587. If not using SSL, the port number is most likely 25. Please check with your email " +
                        "provider.\n\nIf you are a Gmail user, even if your email address does not end in @gmail.com, and you have set up two-stage authentication, you must log in to your Gmail account " +
                        "on the Gmail website and set up an application password.\n\nWould you like to view the technical error details?", "Test Message Failed", MessageBoxButtons.YesNo, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        new ErrorGanderizer(mailman.LastErrorText).ShowDialog();
                    }
                }
                mailman.CloseSmtpConnection();
            }
            catch (Exception ex)
            {
                QMessageBox.Show("Unable to configure the test message for sending. " + ex.Message + (ex.InnerException == null ? " (No additional details " +
                        "were provided.)" : " (" + ex.InnerException.Message + ")"), "Compose Message Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //try
            //{
            //    System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            //    msg.From = new System.Net.Mail.MailAddress(senderEmailAddress, senderFriendlyName);
            //    System.Net.Mail.MailAddress recipient = new System.Net.Mail.MailAddress(recipientEmailAddress, recipientFriendlyName);
            //    msg.To.Add(recipient);
            //    if (!String.IsNullOrEmpty(recipientEmail2))
            //    {
            //        msg.CC.Add(new System.Net.Mail.MailAddress(recipientEmail2));
            //    }
            //    if (!String.IsNullOrEmpty(recipientEmail3))
            //    {
            //        msg.CC.Add(new System.Net.Mail.MailAddress(recipientEmail3));
            //    }
            //    msg.Subject = "Home Server SMART/WindowSMART Mail Send Test";
            //    msg.IsBodyHtml = true;
            //    msg.Body = "<h2>Home Server SMART/WindowSMART Mail Test</h2><hr/>This message confirms you have configured Home Server SMART/WindowSMART email alerting correctly.";
            //    msg.Priority = System.Net.Mail.MailPriority.High;

            //    try
            //    {
            //        System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(mailServer, serverPort);
            //        client.UseDefaultCredentials = false;
            //        client.EnableSsl = useSsl;

            //        if (authenticationEnabled)
            //        {
            //            client.Credentials = new System.Net.NetworkCredential(mailUser, mailPassword);
            //        }
            //        else
            //        {
            //            client.Credentials = null;
            //        }
            //        client.Send(msg);
            //        QMessageBox.Show("Test message was sent successfully. Please check your mailbox to confirm receipt.", "Test Message",
            //            MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    }
            //    catch (Exception ex)
            //    {
            //        QMessageBox.Show("Test message failed to send. " + ex.Message + (ex.InnerException == null ? " (No additional details " +
            //            "were provided.)" : " (" + ex.InnerException.Message + ")"), "Test Message Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    QMessageBox.Show("Unable to configure the test message for sending. " + ex.Message + (ex.InnerException == null ? " (No additional details " +
            //            "were provided.)" : " (" + ex.InnerException.Message + ")"), "Compose Message Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        public bool IsMailConfigurationValid
        {
            get
            {
                return isMailConfigValid;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            bool isPartiallyDefined = false;
            bool isMailEnabled = false;
            isMailConfigValid = IsEmailConfigurationValid(out isPartiallyDefined, out isMailEnabled);
            if (!isMailEnabled || isMailConfigValid)
            {
                SaveEmailConfig();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else if (isPartiallyDefined)
            {
                QMessageBox.Show("The email configuration is in an invalid state. You've set email to enabled, but are missing values in one or more " +
                    "required fields. This configuration cannot be saved. Please fill in the remaining values, or disable email configuration.",
                    "Invalid Email Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                QMessageBox.Show("The email configuration is not valid. You have email enabled, but have not specified any values.", "Invalid Email Configuration",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void SaveEmailConfig()
        {
            try
            {
                // Email Configuration ~ Jake Scherer ~
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailServer, mailServer);
                configurationKey.SetValue(Properties.Resources.RegistryConfigServerPort, serverPort);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSenderFriendly, senderFriendlyName);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSenderEmail, senderEmailAddress);
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientFriendly, recipientFriendlyName);
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail, recipientEmailAddress);
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail2, recipientEmail2);
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail3, recipientEmail3);
                configurationKey.SetValue(Properties.Resources.RegistryConfigAuthenticationEnabled, authenticationEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailUser, crypto.Encrypt(mailUser));
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailPassword, crypto.Encrypt(mailPassword));
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailAlertsEnabled, mailAlertsEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigUseSSL, useSsl);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, isMailConfigValid);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSendDailySummary, sendDailySummary);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSendPlaintext, sendPlaintext);
            }
            catch (Exception ex)
            {
                QMessageBox.Show("Save failed; one or more configured values have been lost. " + ex.Message, "Severe",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EmailConfiguration_Load(object sender, EventArgs e)
        {
            // Read Email Configuration from Registry
            // Email Configuration ~ Jake Scherer ~
            try
            {
                isMailConfigValid = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsEmailConfigValid));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                mailServer = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailServer);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailServer, mailServer);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                serverPort = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigServerPort);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigServerPort, 25);
                serverPort = 25;
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                senderFriendlyName = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigSenderFriendly);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigSenderFriendly, senderFriendlyName);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                senderEmailAddress = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigSenderEmail);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigSenderEmail, senderEmailAddress);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                recipientFriendlyName = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientFriendly);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientFriendly, recipientFriendlyName);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                recipientEmailAddress = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientEmail);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail, recipientEmailAddress);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                recipientEmail2 = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientEmail2);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail, recipientEmail2);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                recipientEmail3 = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientEmail3);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail, recipientEmail3);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                authenticationEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigAuthenticationEnabled));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigAuthenticationEnabled, authenticationEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                mailUser = crypto.Decrypt((String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailUser));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailUser, crypto.Encrypt(mailUser));
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                mailPassword = crypto.Decrypt((String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailPassword));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailPassword, crypto.Encrypt(mailPassword));
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                mailAlertsEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailAlertsEnabled));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailAlertsEnabled, mailAlertsEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                useSsl = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigUseSSL));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigUseSSL, useSsl);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                sendDailySummary = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigSendDailySummary));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigSendDailySummary, true);
            }

            try
            {
                sendPlaintext = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigSendPlaintext));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigSendPlaintext, false);
            }

            // Email ~ Jake ~

            // Populate the fields
            textBoxHost.Text = mailServer;
            numericUpDownPort.Value = serverPort;
            checkBoxSsl.Checked = useSsl;
            textBoxSenderFriendly.Text = senderFriendlyName;
            textBoxSenderEmailAddy.Text = senderEmailAddress;
            checkBoxEnableAuthentication.Checked = authenticationEnabled;
            if (mailUser == "Undefined")
            {
                textBoxUsername.Text = String.Empty;
            }
            else
            {
                textBoxUsername.Text = mailUser;
            }
            if (mailPassword == "N0t_Sp3c1fied")
            {
                textBoxPassword.Text = String.Empty;
            }
            else
            {
                textBoxPassword.Text = mailPassword;
            }
            textBoxRecipientName.Text = recipientFriendlyName;
            textBoxRecipientEmail.Text = recipientEmailAddress;
            checkBoxEnableEmail.Checked = mailAlertsEnabled;
            textBoxEmailAddy2.Text = recipientEmail2;
            textBoxEmailAddy3.Text = recipientEmail3;
            checkBoxSendSummary.Checked = sendDailySummary;
            checkBoxSendPlaintext.Checked = sendPlaintext;
        }

        private void checkBoxEnableEmail_CheckedChanged(object sender, EventArgs e)
        {
            mailAlertsEnabled = checkBoxEnableEmail.Checked;
        }
    }
}
