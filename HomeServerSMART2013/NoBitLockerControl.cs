using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Management;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using WSSControls.BelovedComponents;
using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker;
using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UserControls;
using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI
{
    public partial class NoBitLockerControl : UserControl
    {
        // Skinning
        private bool useDefaultSkinning;
        private int windowBackground; // 0 = Metal Grate, 1 = Lightning, 2 = Cracked Glass, 3 = None
        private int oldBackground = 0;

        #region Constructor
        public NoBitLockerControl()
        {
            InitializeComponent();

            // Try connecting to the Registry.
            try
            {
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;
                Microsoft.Win32.RegistryKey configurationKey;
                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);
                configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, false);
                if (dojoNorthSubKey == null || configurationKey == null)
                {
                    configurationKey.Close();
                    dojoNorthSubKey.Close();
                    windowBackground = 0;
                    return;
                }

                try
                {
                    windowBackground = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigWindowBackground);
                }
                catch
                {
                    windowBackground = 0;
                }
                configurationKey.Close();
                dojoNorthSubKey.Close();
            }
            catch (Exception)
            {
                windowBackground = 0;
            }
        }
        #endregion

        /// <summary>
        /// Runs when the form is first loaded. Populate the ListView with disks so the user may select them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BitLockerControl_Load(object sender, EventArgs e)
        {
            SetWindowBackground();
        }

        private void buttonDocumentation_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Dojo North Software\\Home Server SMART 2012\\HomeServerSMART.chm");
        }

        private void buttonInstallNow_Click(object sender, EventArgs e)
        {
            buttonInstallNow.Enabled = false;

            if (QMessageBox.Show("Do you really want to install BitLocker Drive Encryption on this Server?", "Install " +
                "BitLocker Feature", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) ==
                DialogResult.Yes)
            {
                try
                {
                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("ServerManagerCmd.exe", "-install BitLocker");
                    psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                    psi.RedirectStandardError = true;
                    psi.RedirectStandardOutput = true;
                    psi.UseShellExecute = false;
                    System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);

                    // Wait up to 5 minutes.
                    bool exited = process.WaitForExit(300000);
                    if (!exited)
                    {
                        if (QMessageBox.Show("Server Manager hasn't responded in over 5 minutes. Do you want to continue waiting?" +
                            "Click Yes to continue waiting (up to one hour); click No to immediately kill Server Manager.",
                            "Server Manager Unresponsive", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) ==
                            DialogResult.Yes)
                        {
                            exited = process.WaitForExit(3600000);
                            if (!exited)
                            {
                                process.Kill();
                            }
                        }
                        else
                        {
                            exited = false;
                            process.Kill();
                        }
                    }

                    String error = process.StandardError.ReadToEnd();
                    String output = process.StandardOutput.ReadToEnd();
                    String result = String.Empty;
                    if (String.IsNullOrEmpty(error) && String.IsNullOrEmpty(output))
                    {
                        result = String.Empty;
                    }
                    else if (String.IsNullOrEmpty(error))
                    {
                        result = output;
                    }
                    else
                    {
                        result = error;
                    }

                    if (process.ExitCode == 0)
                    {
                        QMessageBox.Show("BitLocker Drive Encryption was successfully installed on the Server. No reboot is required. Please " +
                            "restart the Dashboard to enable BitLocker Drive Encryption in Home Server SMART 24/7.",
                            "Install BitLocker Drive Encryption", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (process.ExitCode == 3010)
                    {
                        QMessageBox.Show("BitLocker Drive Encryption was successfully installed on the Server. A reboot is required. Please " +
                            "reboot the Server at your earliest convenience to enable BitLocker.", "Install BitLocker Drive Encryption",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        QMessageBox.Show("BitLocker Drive Encryption failed to install on the Server. " +
                            (String.IsNullOrEmpty(result) ? "No error details were returned. However, the Server returned error code " +
                            process.ExitCode.ToString() + "." : result + " (error code " + process.ExitCode.ToString() + ")"),
                            "Install BitLocker Drive Encryption", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        buttonInstallNow.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    QMessageBox.Show("Enabling of BitLocker failed: " + ex.Message, "Severe",
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    buttonInstallNow.Enabled = true;
                }
            }
            else
            {
                buttonInstallNow.Enabled = true;
            }
        }

        #region Window Background
        private void SetWindowBackground()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.NoBitLockerControl.SetWindowBackground");
            switch (windowBackground)
            {
                case 3:
                    {
                        this.BackgroundImage = null;
                        this.BackgroundImageLayout = ImageLayout.None;
                        break;
                    }
                case 0:
                default:
                    {
                        this.BackgroundImage = Properties.Resources.MetalGrate;
                        this.BackgroundImageLayout = ImageLayout.Tile;
                        break;
                    }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.NoBitLockerControl.SetWindowBackground");
        }
        #endregion
    }
}
