using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;

using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class WindowsPhoneConfiguration : Form
    {
        private bool isWindowsPhoneEnabled;
        private String wp7Guids;
        private Microsoft.Win32.RegistryKey configurationKey;
        private bool isWindowsServerSolutions;

        public WindowsPhoneConfiguration(Microsoft.Win32.RegistryKey key, bool isWss)
        {
            InitializeComponent();
            isWindowsPhoneEnabled = false;
            wp7Guids = String.Empty;
            configurationKey = key;
            isWindowsServerSolutions = isWss;
        }

        private void cbEnableProwl_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnableWindowsPhone.Checked)
            {
                tbWp7Guids.Enabled = true;
                buttonValidate.Enabled = true;
            }
            else
            {
                tbWp7Guids.Enabled = false;
                buttonValidate.Enabled = false;
            }
        }

        private void ProwlConfiguration_Load(object sender, EventArgs e)
        {
            try
            {
                isWindowsPhoneEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsToastyEnabled));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsToastyEnabled, isWindowsPhoneEnabled);
            }

            try
            {
                wp7Guids = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigWindowsPhoneGuids);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigWindowsPhoneGuids, wp7Guids);
            }

            cbEnableWindowsPhone.Checked = isWindowsPhoneEnabled;
            tbWp7Guids.Text = wp7Guids;
        }

        public bool NmaEnabled
        {
            get
            {
                return isWindowsPhoneEnabled;
            }
        }

        public String NmaApiKey
        {
            get
            {
                return wp7Guids;
            }
        }

        private void buttonValidate_Click(object sender, EventArgs e)
        {
            tbWp7Guids.Text = tbWp7Guids.Text.Replace(" ", String.Empty);
            String[] guids = tbWp7Guids.Text.Split(',');
            List<String> rancidGuids = new List<String>();

            foreach (String guid in guids)
            {
                Guid testGuid;

                if (Guid.TryParse(guid, out testGuid) == false)
                {
                    rancidGuids.Add(guid);
                }
            }

            if (rancidGuids.Count > 0)
            {
                String rancidGuidString = String.Empty;
                foreach (String guid in rancidGuids)
                {
                    rancidGuidString += guid + ", ";
                }

                // Lop off the last ", "                    
                rancidGuidString = rancidGuidString.Substring(0, rancidGuidString.Length - 2);

                QMessageBox.Show("At least one email address failed validation. Below " + (rancidGuids.Count == 1 ? "is the key" : "are the keys") +
                    "that failed validation: " + rancidGuidString + " Toasty keys are GUIDs and look something like 573bd91c-cc77-47ba-906f-8c4e56c4dc70. " +
                    "Please make appropriate corrections and try again.", "Toasty Key Syntax Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (QMessageBox.Show("Toasty validation successful. Do you want to try sending a test message now?", "Toasty Validation",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                foreach (String guid in guids)
                {
                    try
                    {
                        // Construct the URI for calling the service.
                        String deviceID = guid;
                        String title = (isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart) + " Toasty Test";
                        String image = Properties.Resources.RemoteNotifyIconGeneral;
                        String text = "Congratulations! You have successfully set up Toasty notifications!";

                        String parameters = deviceID + Properties.Resources.ToastyParamDeviceID + deviceID + Properties.Resources.ToastyParamTitle + HttpUtility.UrlEncode(title) +
                            Properties.Resources.ToastyParamText + HttpUtility.UrlEncode(text) + Properties.Resources.ToastyParamImage + HttpUtility.UrlEncode(image) + Properties.Resources.ToastyParamSender;

                        String uri = Properties.Resources.ToastyApiUri + parameters;

                        Gurock.SmartInspect.SiAuto.Main.LogFatal("parameters = " + parameters);
                        Gurock.SmartInspect.SiAuto.Main.LogFatal("uri = " + uri);

                        // Create the HTTP GET request
                        WebRequest req = WebRequest.Create(uri);
                        HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                        System.IO.StreamReader reader = new System.IO.StreamReader(resp.GetResponseStream());
                        String response = reader.ReadToEnd();
                        reader.Close();
                        resp.Close();

                        if (response.Contains("OK") && response.Contains("Received"))
                        {
                            if (response.Contains("TempDisconnected") || response.Contains("Disconnected"))
                            {
                                QMessageBox.Show("Toasty indicates that it successfully received the notification for device ID " + deviceID + ". However, Toasty reports that the device is " +
                                    "temporarily disconnected. Your phone may be powered off, in standby or not connected to a Wi-Fi or cellular network. When you turn your phone back on or " +
                                    "reconnect to a network, the message should be delivered automatically.", "Toasty Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else if (response.Contains("Connected"))
                            {
                                QMessageBox.Show("Toasty indicates that it successfully received the notification for device ID " + deviceID + ", and the message was successfully delivered. Check " +
                                    "your phone to confirm successful delivery.", "Toasty Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                QMessageBox.Show("Toasty indicates that it successfully received the notification for device ID " + deviceID + ". However, Toasty reported that there may have been a " +
                                    "problem deliverying the message. The Toasty web service returned a status message \"" + response + "\". Check your phone to confirm successful delivery.",
                                    "Toasty Validation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        else
                        {
                            if (response.Contains("PreconditionFailed") && response.Contains("Active") && response.Contains("Inactive"))
                            {
                                QMessageBox.Show("The Toasty notification test was not immediately successful. The Toasty web service is experiencing problems and " +
                                    "returned the status code " + response + ". This status code indicates the web service most likely received the message, but is " +
                                    "unable to deliver it at this time. Please do NOT try testing again right away. Wait a little while and check your phone to see " +
                                    "if the message did finally come through. In most cases the message will arrive. If after an hour or two the message has not " +
                                    "arrived, try the test again.", "Toasty Validation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            else
                            {
                                QMessageBox.Show("The Toasty notification test was not successful. No error occurred, however the following status message was returned: " +
                                    response + "\n\nThe Toasty web service may be temporarily unavailable.", "Toasty Validation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("404") || ex.Message.Contains("Not Found"))
                        {
                            QMessageBox.Show("Toasty returned the error code \"404 Not Found.\" This typically indicates that Toasty was not able to validate that the device ID you specified is a " +
                                "valid one. If you just set up Toasty on your phone, or if you just changed your device ID within Toasty, it can take up to 60 minutes for the Toasty web service " +
                                "to reflect the new or changed ID. Carefully double-check the ID for typos and try again. The Toasty app has a mail button on the Device ID settings page so that you " +
                                "can email a copy of the device ID to yourself. Try emailing yourself the device ID, which will reduce the likelihood of a typo. The device ID that failed validation is " +
                                guid + ".", "Toasty Validation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                        {
                            QMessageBox.Show("The Toasty web service returned an error: " + ex.Message + " If you just set up Toasty on your phone, or if you just changed your device ID within Toasty, " +
                                "it can take up to 60 minutes for the Toasty web service to reflect the new or changed ID. Carefully double-check the ID for typos and try again. The toasty app has a mail " +
                                "button on the Device ID settings page so that you can email a copy of the device ID to yourself. Try emailing yourself the device ID, which will reduce the likelihood of " +
                                "a typo. The device ID that failed validation is " + guid + ".", "Toasty Validation", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                }
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
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
                isWindowsPhoneEnabled = cbEnableWindowsPhone.Checked;
                wp7Guids = tbWp7Guids.Text;

                configurationKey.SetValue(Properties.Resources.RegistryConfigIsToastyEnabled, isWindowsPhoneEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigWindowsPhoneGuids, wp7Guids);
            }
            catch (Exception ex)
            {
                QMessageBox.Show("Save failed; one or more configured values have been lost. " + ex.Message, "Severe",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
