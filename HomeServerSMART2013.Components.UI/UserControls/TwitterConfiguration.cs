using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Gurock.SmartInspect;
using Twitterizer;
using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class TwitterConfiguration : Form
    {
        private bool isTwitterEnabled;
        private String twitterToken;
        private String twitterSecret;
        private Microsoft.Win32.RegistryKey configurationKey;
        private String serverProductId;
        private bool isWindowsServerSolutions;

        private OAuthTokenResponse requestResponse;
        private Uri authorizationUri;
        private OAuthTokenResponse tokenResponse;
        private bool requestInProgress;

        public TwitterConfiguration(Microsoft.Win32.RegistryKey key, bool isWss)
        {
            InitializeComponent();
            isTwitterEnabled = false;
            
            isWindowsServerSolutions = isWss;
            serverProductId = Utilities.Utility.NT_STATUS_SERVER_ID;
            requestInProgress = false;
        }

        private void ProwlConfiguration_Load(object sender, EventArgs e)
        {
            
        }

        public bool TwitterEnabled
        {
            get
            {
                return isTwitterEnabled;
            }
        }

        public String TwitterToken
        {
            get
            {
                return twitterToken;
            }
        }

        public String TwitterSecret
        {
            get
            {
                return twitterSecret;
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
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.UserControls.TwitterConfiguration.SaveConfiguration");
            isTwitterEnabled = cbEnableTwitter.Checked;
            twitterToken = tbToken.Text;
            twitterSecret = tbSecret.Text;
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.UserControls.TwitterConfiguration.SaveConfiguration");
        }

        private void buttonAuthorize_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.UserControls.TwitterConfiguration.buttonAuthorize_Click");
            AuthorizeTwitter();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.UserControls.TwitterConfiguration.buttonAuthorize_Click");
        }

        private void AuthorizeTwitter()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.UserControls.TwitterConfiguration.AuthorizeTwitter");

            try
            {
                requestResponse = OAuthUtility.GetRequestToken(Properties.Resources.TwitterConsumerToken, serverProductId, Properties.Resources.TwitterCallbackUri);
                SiAuto.Main.LogString("token", requestResponse.Token);
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to generate OAuth REQUEST token: " + ex.Message);
                SiAuto.Main.LogException(ex);
                QMessageBox.Show("An error occured while generating an OAuth token request from Twitter: " + ex.Message + "Please try the request again in a little while. If the problem " +
                    "persists, please submit a bug report.", "Request Token Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                authorizationUri = OAuthUtility.BuildAuthorizationUri(requestResponse.Token);
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to generate authorization uri: " + ex.Message);
                SiAuto.Main.LogException(ex);
                QMessageBox.Show("An error occured while generating a URL from Twitter: " + ex.Message + "Please try the request again in a little while. If the problem " +
                    "persists, please submit a bug report.", "Request URI Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            buttonAuthorize.Enabled = false;
            buttonGetToken.Enabled = true;
            requestInProgress = true;

            QMessageBox.Show("You will now be directed to twitter.com in your default web browser to authorize the " + Properties.Resources.ApplicationTitleWindowSmart + " Twitter application to post to your feed. " +
                "Please do NOT close the Twitter Configuration dialogue before completing the authorization process. If you close the dialogue prior to getting your access token, you " +
                "will have to start over.", "Twitter Application Authorization", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Utilities.Utility.LaunchBrowser(authorizationUri.OriginalString);

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.UserControls.TwitterConfiguration.AuthorizeTwitter");
        }

        private void buttonGetToken_Click(object sender, EventArgs e)
        {
            CollectToken();
        }

        private void CollectToken()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.UserControls.TwitterConfiguration.CollectToken");
            try
            {
                tokenResponse = OAuthUtility.GetAccessToken(Properties.Resources.TwitterConsumerToken, serverProductId, requestResponse.Token, requestResponse.VerificationString);
                SiAuto.Main.LogString("accessToken", tokenResponse.Token);
                tbToken.Text = tokenResponse.Token;
                tbSecret.Text = tokenResponse.TokenSecret;
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to collect user access token: " + ex.Message);
                SiAuto.Main.LogException(ex);
                QMessageBox.Show("An error occured while collecting the access token from Twitter: " + ex.Message + "Bear in mind tokens can expire so if a long period of time has elapsed " +
                    "since you completed Step One, you need to start over. Please try the request again in a little while. If the problem " +
                    "persists, please submit a bug report.", "Access Token Collection Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            buttonGetToken.Enabled = false;
            requestInProgress = false;

            if (QMessageBox.Show("Congratulations! Your access token and token secret have been successfully collected. Be sure to save the configuration by clicking OK, or at the very " +
                    "least, make a note of your access token and token secret (you can check the Show Secret checkbox to see the token secret). If you lose your access token and token " +
                    "secret, you can retrieve them from twitter.com by selecting Settings, then selecting Apps.\n\nIf you run " + (isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss :
                    Properties.Resources.ApplicationTitleWindowSmart) + " on multiple computers and wish to set up Twitter notifications on them, you do NOT need to authorize Twitter " +
                    "again. Instead, please copy your access token and token secret and paste it directly into the Twitter Configuration dialogue on those computers. If you perform the " +
                    "authorization steps again, you will invalidate your existing token and secret, and will then need to copy and paste the new token and secret to all affected computers.\n\n" +
                    "Do you want to send a test tweet now?", "Twitter Setup Complete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                TestTwitter();
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.UserControls.TwitterConfiguration.CollectToken");
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            TestTwitter();
        }

        private void TestTwitter()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.UserControls.TwitterConfiguration.TestTwitter");

            isTwitterEnabled = cbEnableTwitter.Checked;
            twitterToken = tbToken.Text;
            twitterSecret = tbSecret.Text;

            SiAuto.Main.LogString("twitterToken", twitterToken);
            SiAuto.Main.LogString("twitterSecret", "Passwords are not logged; logging only whether secret is specified. Secret is " + (String.IsNullOrEmpty(twitterSecret) ? "MISSING" : "specified"));

            if (String.IsNullOrEmpty(tbToken.Text))
            {
                QMessageBox.Show("Access Token is a required field. Please supply your access token and try again.", "Missing Token", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (String.IsNullOrEmpty(tbSecret.Text))
            {
                QMessageBox.Show("Token Secret is a required field. Please supply your token secret and try again.", "Missing Secret", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            
            OAuthTokens tokens = new OAuthTokens();
            tokens.ConsumerKey = Properties.Resources.TwitterConsumerToken;
            tokens.ConsumerSecret = serverProductId;
            tokens.AccessToken = tbToken.Text;
            tokens.AccessTokenSecret = tbSecret.Text;

            //IAsyncResult asyncResult = TwitterStatusAsync.Update(
            //    tokens,                     // The OAuth tokens
            //    message,                    // The text of the tweet
            //    null,                       // Optional parameters (none given here)
            //    new TimeSpan(0, 3, 0),      // The maximum time to let the process run
            //    updateResponse =>           // The callback method
            //    {
            //        // Handle any errors here with: updateResponse.ErrorMessage
            //    });

            String message = (isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart) + " on computer " + Environment.MachineName +
                ", setup complete!";

            try
            {
                TwitterResponse<TwitterStatus> tweetResponse = TwitterStatus.Update(tokens, message);
                if (tweetResponse.Result == RequestResult.Success)
                {
                    SiAuto.Main.LogMessage("Tweet Successful");
                    QMessageBox.Show("Twitter reports the tweet was successfully tweeted to your feed. Please check your feed to verify it was in fact posted.", "Test Tweet Successful",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    SiAuto.Main.LogWarning("Tweet Failed: " + tweetResponse.Result.ToString());
                    QMessageBox.Show("Tweet failed with Twitter status code " + tweetResponse.Result.ToString() + "\n\nPlease verify your access token and token secret are correct and do not " +
                        "contain typos or trailing spaces. If they are correct, please try again later. If the problem persists, please open a bug report.", "Test Tweet Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Tweet Failed with Error: " + ex.Message);
                SiAuto.Main.LogException(ex);
                QMessageBox.Show("Tweet failed: " + ex.Message + "\n\nPlease verify your access token and token secret are correct and do not contain typos or trailing spaces. If they are " +
                    "correct, please try again later. If the problem persists, please open a bug report.", "Test Tweet Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.UserControls.TwitterConfiguration.TestTwitter");
        }

        private void cbEnableTwitter_CheckedChanged(object sender, EventArgs e)
        {
            tbToken.Enabled = cbEnableTwitter.Checked;
            tbSecret.Enabled = cbEnableTwitter.Checked;
            buttonTest.Enabled = cbEnableTwitter.Checked;
            cbShowSecret.Enabled = cbEnableTwitter.Checked;
            if (cbEnableTwitter.Checked && requestInProgress)
            {
                buttonAuthorize.Enabled = false;
                buttonGetToken.Enabled = true;
            }
            else if (cbEnableTwitter.Checked && !requestInProgress)
            {
                buttonAuthorize.Enabled = true;
                buttonGetToken.Enabled = false;
            }
            else
            {
                buttonAuthorize.Enabled = false;
                buttonGetToken.Enabled = false;
            }
        }

        private void cbShowSecret_CheckedChanged(object sender, EventArgs e)
        {
            if (cbShowSecret.Checked)
            {
                tbSecret.PasswordChar = '\0';
            }
            else
            {
                tbSecret.PasswordChar = '●';
            }
        }
    }
}
