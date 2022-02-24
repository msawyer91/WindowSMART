using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components;
using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls;
using Gurock.SmartInspect;
using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI
{
    public partial class MainForm : Form
    {
        // Beta expiration
        //private DateTime expirationDate = Components.License.GetExpirationDate();
        
        private DateTime result;
        Guid refGuid = new Guid("{00000000-0000-0000-0000-000000000000}");
        private uint retVal;
        private bool useDefaultSkinning = true;
        private MainUiControl mainUi;
        private String xmlText;
        private Components.Mexi_Sexi.MexiSexi mexiSexi = null;

        public MainForm(uint slobberhead, DateTime hubbaChubba)
        {
            SiAuto.Main.LogMessage("[I Prevail] WinodwSMART 24/7 is initializing. (0x" + slobberhead.ToString("X") + ")");
            InitializeComponent();
            //mainUi = new MainUiControl();
            //mainUi.Location = new Point(0, 24);
            //this.Controls.Add(mainUi);

            #region BETA Section - Comment Out in Production
            //SiAuto.Main.LogWarning("[I Prevail] Checking beta expiration.");
            //if (Components.License.IsExpired(expirationDate, this, true))
            //{
            //    // do nothing
            //}
            //SiAuto.Main.LogError("[I Prevail] The license is okay; load continuing.");
            #endregion BETA Section - Comment Out in Production

            // Now we do the real checking.
            String concatenatedString = String.Empty;
            retVal = slobberhead;
            if (slobberhead == 0x0)
            {
                // New user
                concatenatedString = Components.LegacyOs.Concatenate(Program.SW_WINDOWS_KEY, Properties.Resources.RegistryConfigCriticalFailure, true, true, out refGuid);
            }
            else if (slobberhead == 0x1)
            {
                // Existing user and no errors returned with info
                concatenatedString = Components.LegacyOs.Concatenate(Program.SW_WINDOWS_KEY, Properties.Resources.RegistryConfigCriticalFailure, false, true, out refGuid);
            }
            else
            {
                // Date was invalid but we still grab the license because it could be valid (if it is we don't care about the date).
                concatenatedString = "0xF," + Components.LegacyOs.Concatenate(Program.SW_WINDOWS_KEY, Properties.Resources.RegistryConfigCriticalFailure, false, true, out refGuid);
            }
            xmlText = concatenatedString;
            result = hubbaChubba;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SiAuto.Main.LogMessage("[I Prevail] WindowSMART 24/7 is deinitializing.");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings(mexiSexi);
            settings.ShowDialog();
            mainUi.CheckForBackgroundChange();
        }

        private void aboutWindowSMART2013ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpAbout about = new HelpAbout(useDefaultSkinning, mexiSexi);
            about.ShowDialog();
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "WindowSMART.chm");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Try connecting to the Registry.
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);
                Microsoft.Win32.RegistryKey configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, false);
                useDefaultSkinning = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigUseDefaultSkinning));
            }
            catch
            {
                useDefaultSkinning = true;
            }

            // License
            mexiSexi = new Components.Mexi_Sexi.MexiSexi(xmlText, result, refGuid, retVal);

            // Mexi-Sexi can have three flags - Geezery, Error and MexiSexi
            // Error prevails - that means bad things happened; force expiration.
            // Error = true; expiration forced (irrespective of Geezery). MexiSexi cannot be true.
            // Geezery = true; we're expired. MexiSexi cannot be true; even if it was, Geezery prevails.
            // MexiSexi = false (and Error and Geezery are also false); we're in the trial period.
            // MexiSexi = true; we're golden - run unrestricted.
            Welcome dubya = null;
            if (mexiSexi.IsError)
            {
                dubya = new Welcome(mexiSexi.Checker, true, false, mexiSexi.ReferenceCode);
                this.Text = Properties.Resources.ApplicationTitle + " Professional Trial";
                dubya.ShowDialog();
            }
            else if (mexiSexi.IsGeezery)
            {
                dubya = new Welcome(mexiSexi.Checker, true, false, mexiSexi.ReferenceCode);
                this.Text = Properties.Resources.ApplicationTitle + " Professional Trial";
                dubya.ShowDialog();
            }
            else if (!mexiSexi.IsMexiSexi)
            {
                dubya = new Welcome(mexiSexi.Checker, false, false, mexiSexi.ReferenceCode);
                this.Text = Properties.Resources.ApplicationTitle + " Professional Trial";
                dubya.ShowDialog();
            }
            else
            {
                //mexiSexi.IsMexiSexi
                if (mexiSexi.IsHomeEdition)
                {
                    this.Text = Properties.Resources.ApplicationTitle + " Home";
                }
                else
                {
                    this.Text = Properties.Resources.ApplicationTitle + " Professional";
                }
                enterLicenseKeyToolStripMenuItem.Visible = false;
                registerToolStripMenuItem1.Visible = false;
                registerToolStripMenuItem.Visible = false;
            }

            if (dubya != null && dubya.ExitCode == 0x1)
            {
                this.Close();
                this.Dispose();
                Application.Exit();
                return;
            }
            else if (dubya != null && dubya.ExitCode == 0x0)
            {
                // Re-read the license info.
                bool belch = BelchBack();
                if (belch)
                {
                    if (mexiSexi.IsHomeEdition)
                    {
                        this.Text = Properties.Resources.ApplicationTitle + " Home";
                    }
                    else
                    {
                        this.Text = Properties.Resources.ApplicationTitle + " Professional";
                    }
                    enterLicenseKeyToolStripMenuItem.Visible = false;
                    registerToolStripMenuItem1.Visible = false;
                    registerToolStripMenuItem.Visible = false;
                }
                else
                {
                    QMessageBox.Show("No errors were detected while committing the license. However, a read-back verification operation failed with code 0x" +
                        mexiSexi.ReferenceCode.ToString("X") + ". When you click OK, WindowSMART 24/7 will close. Please restart WindowSMART 24/7. If the " +
                        "trial dialogue reappears with an error code, please contact Dojo North Software.", "Readback Failed", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    this.Close();
                    this.Dispose();
                    Application.Exit();
                }
            }

            mainUi = new MainUiControl(mexiSexi, false);
            mainUi.Location = new Point(0, 24);
            mainUi.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right);
            this.Controls.Add(mainUi);
        }

        private void enterLicenseKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PerformKeyApplication();
            if (mainUi != null)
            {
                try
                {
                    mainUi.SetAppTitle();
                }
                catch
                {
                }
            }
        }

        private bool BelchBack()
        {
            refGuid = new Guid("{00000000-0000-0000-0000-000000000000}");
            String belchBack = Components.LegacyOs.Concatenate(Program.SW_WINDOWS_KEY, Properties.Resources.RegistryConfigCriticalFailure, false, true, out refGuid);
            if (mexiSexi != null)
            {
                mexiSexi.Refresh(belchBack, result, refGuid, 0x0);
            }
            else
            {
                mexiSexi = new Components.Mexi_Sexi.MexiSexi(belchBack, result, refGuid, 0x0);
            }
            return mexiSexi.IsMexiSexi;
        }

        private void reKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (QMessageBox.Show("This is intended for debugging and troubleshooting only. Proceed with re-key operation?",
                "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                PerformKeyApplication();
                if (mainUi != null)
                {
                    try
                    {
                        mainUi.SetAppTitle();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void PerformKeyApplication()
        {
            Components.Licensing.Registration reg = new Components.Licensing.Registration();
            reg.ShowDialog();
            if (reg.CommitCode == 0x0)
            {
                // Re-read the license info.
                bool belch = BelchBack();
                if (belch)
                {
                    if (mexiSexi.IsHomeEdition)
                    {
                        this.Text = Properties.Resources.ApplicationTitle + " Home";
                    }
                    else
                    {
                        this.Text = Properties.Resources.ApplicationTitle + " Professional";
                    }
                    enterLicenseKeyToolStripMenuItem.Visible = false;
                    registerToolStripMenuItem1.Visible = false;
                    registerToolStripMenuItem.Visible = false;
                }
                else
                {
                    QMessageBox.Show("No errors were detected while committing the license. However, a read-back verification operation failed with code 0x" +
                        mexiSexi.ReferenceCode.ToString("X") + ". Please restart WindowSMART 24/7. If the " +
                        "trial dialogue reappears with an error code, please contact Dojo North Software.", "Readback Failed", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
        }

        private void registerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Components.Utilities.Utility.LaunchBrowser(Properties.Resources.PurchaseUrl);
        }
    }
}
