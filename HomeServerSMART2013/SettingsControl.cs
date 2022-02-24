using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Gurock.SmartInspect;
using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls;
using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI
{
    public partial class SettingsControl : UserControl
    {
        Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
        Microsoft.Win32.RegistryKey dojoNorthSubKey;
        Microsoft.Win32.RegistryKey configurationKey;

        // Skinning
        private bool useDefaultSkinning;
        private int windowBackground; // 0 = Metal Grate, 1 = Lightning, 2 = Cracked Glass, 3 = None
        private int oldBackground = 0;

        private bool isWindowsServerSolutions = true;
        private bool isRegistryAvailable;

        public SettingsControl()
        {
            InitializeComponent();

            // Is WSS?
            isWindowsServerSolutions = DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.OperatingSystem.IsWindowsServerSolutionsProduct(this);

            // Try connecting to the Registry.
            try
            {
                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);
                configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, false);
                if (dojoNorthSubKey == null || configurationKey == null)
                {
                    configurationKey.Close();
                    dojoNorthSubKey.Close();
                    isRegistryAvailable = false;
                }
                else
                {
                    dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, true);
                    configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, true);
                    isRegistryAvailable = true;
                }
            }
            catch (Exception)
            {
                isRegistryAvailable = false;
            }

            // Set the defaults -- we'll load from the Registry if available.
            SetDefaults();
        }

        private void SetDefaults()
        {
            // Skinning
            useDefaultSkinning = true;
            windowBackground = 0;
            oldBackground = 0;

            if (!isRegistryAvailable)
            {
                try
                {
                    dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                    configurationKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryConfigurationKey);
                    isRegistryAvailable = true;
                }
                catch (Exception ex)
                {
                    isRegistryAvailable = false;
                    QMessageBox.Show(Properties.Resources.ErrorMessageRegistryWriteFailed + ex.Message, Properties.Resources.ErrorMessageTitleSevere,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadDataFromRegistry()
        {
            bool exceptionsDetected = false;
            String errors = String.Empty;


            // Skinning
            try
            {
                useDefaultSkinning = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigUseDefaultSkinning));
            }
            catch
            {
                SiAuto.Main.LogWarning("Use Default Skinning was undefined or defined value was corrupt; it has been reset to default.");
                useDefaultSkinning = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigUseDefaultSkinning, useDefaultSkinning);
                exceptionsDetected = true;
            }

            try
            {
                windowBackground = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigWindowBackground);
            }
            catch
            {
                SiAuto.Main.LogWarning("Window Background was undefined or defined value was corrupt; it has been reset to default (metal grate).");
                windowBackground = 0;
                configurationKey.SetValue(Properties.Resources.RegistryConfigWindowBackground, 0);
                exceptionsDetected = true;
            }

            if (exceptionsDetected)
            {
                QMessageBox.Show(Properties.Resources.ErrorMessageRegistryReadsFailedWarn, Properties.Resources.WindowTitleWarning,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RenderWindowBackground()
        {
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
        }

        private void SettingsControl_Load(object sender, EventArgs e)
        {
            RenderWindowBackground();
        }

        private void buttonInstallNow_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings(null);
            settings.ShowDialog();
        }
    }
}
