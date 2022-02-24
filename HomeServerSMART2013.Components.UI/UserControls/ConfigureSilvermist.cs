using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class ConfigureSilvermist : Form
    {
        private Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
        private Microsoft.Win32.RegistryKey dojoNorthSubKey;
        private Microsoft.Win32.RegistryKey configurationKey;

        public ConfigureSilvermist()
        {
            InitializeComponent();

            bool isRegistryAvailable = false;

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

            if (!isRegistryAvailable)
            {
                buttonAddDir.Enabled = false;
                buttonAddExclusion.Enabled = false;
                buttonOK.Enabled = false;
                QMessageBox.Show("Unable to access necessary Registry configuration. You cannot make or save changes.", "Severe",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                LoadConfiguration();
            }
        }

        private void buttonAddDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Choose a folder to back up. The selected folder and its contents will be included automatically.";
            fbd.ShowDialog();

            if (String.IsNullOrEmpty(fbd.SelectedPath))
            {
                return;
            }

            String path = fbd.SelectedPath;

            foreach (String entry in listBoxInclude.Items)
            {
                if (String.Compare(path, entry, true) == 0)
                {
                    QMessageBox.Show("Folder " + path + " is already selected.", "Emergency Backup", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            listBoxInclude.Items.Add(path);
        }

        private void buttonRemDir_Click(object sender, EventArgs e)
        {
            if (listBoxInclude.Items.Count < 1)
            {
                QMessageBox.Show("The include list is empty.", "Emergency Backup", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                EnableDisableDirectoryButtons(false);
                return;
            }

            if (listBoxInclude.SelectedIndex < 0)
            {
                QMessageBox.Show("You haven't selected anything to remove.", "Emergency Backup", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                listBoxInclude.Items.RemoveAt(listBoxInclude.SelectedIndex);
            }
        }

        private void listBoxInclude_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxInclude.Items.Count > 0 && listBoxInclude.SelectedIndex >= 0)
            {
                EnableDisableDirectoryButtons(true);
            }
            else
            {
                EnableDisableDirectoryButtons(false);
            }
        }

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            if (listBoxInclude.Items.Count < 1)
            {
                QMessageBox.Show("The include list is empty.", "Emergency Backup", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                EnableDisableDirectoryButtons(false);
                return;
            }

            if (listBoxInclude.SelectedIndex < 0)
            {
                QMessageBox.Show("You haven't selected anything to move up.", "Emergency Backup", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                if (listBoxInclude.SelectedIndex == 0)
                {
                    // We're as high as we can go.
                    System.Media.SystemSounds.Asterisk.Play();
                }
                else
                {
                    String currentItem = (String)listBoxInclude.Items[listBoxInclude.SelectedIndex];
                    String higherItem = (String)listBoxInclude.Items[listBoxInclude.SelectedIndex - 1];

                    listBoxInclude.Items[listBoxInclude.SelectedIndex] = higherItem;
                    listBoxInclude.Items[listBoxInclude.SelectedIndex - 1] = currentItem;

                    listBoxInclude.SelectedIndex = listBoxInclude.SelectedIndex - 1;
                }
            }
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            if (listBoxInclude.Items.Count < 1)
            {
                QMessageBox.Show("The include list is empty.", "Emergency Backup", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                EnableDisableDirectoryButtons(false);
                return;
            }

            if (listBoxInclude.SelectedIndex < 0)
            {
                QMessageBox.Show("You haven't selected anything to move down.", "Emergency Backup", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                if (listBoxInclude.SelectedIndex >= (listBoxInclude.Items.Count - 1))
                {
                    // We're as low as we can go.
                    System.Media.SystemSounds.Asterisk.Play();
                }
                else
                {
                    String currentItem = (String)listBoxInclude.Items[listBoxInclude.SelectedIndex];
                    String lowerItem = (String)listBoxInclude.Items[listBoxInclude.SelectedIndex + 1];

                    listBoxInclude.Items[listBoxInclude.SelectedIndex] = lowerItem;
                    listBoxInclude.Items[listBoxInclude.SelectedIndex + 1] = currentItem;

                    listBoxInclude.SelectedIndex = listBoxInclude.SelectedIndex + 1;
                }
            }
        }

        private void EnableDisableDirectoryButtons(bool enable)
        {
            buttonRemDir.Enabled = enable;
            buttonMoveUp.Enabled = enable;
            buttonMoveDown.Enabled = enable;
        }

        private void EnableDisableExcludeButtons(bool enable)
        {
            buttonRemoveExclusion.Enabled = enable;
        }

        private void buttonAddExclusion_Click(object sender, EventArgs e)
        {
            DialogBox dialogExclude = new DialogBox("Add Excluded Item",
                "Please enter a filename, filespec, partial or full path, or a fully-qualified path with filename to exclude. " +
                "Do NOT include wildcard characters (* or ?). Eample: \"helloworld\" will exclude any file or folder containing " +
                "that word, whilst \"\\helloworld\" will exclude any folder (and all of its subcontents (files/folders)) that " +
                "starts with that word. Use \".\" (period) before an item to exclude files with certain extensions (i.e. " +
                "\".tmp\" excludes TMP files).");
            dialogExclude.ShowDialog();

            if (dialogExclude.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                String filespec = dialogExclude.DialogText;

                if (String.IsNullOrEmpty(filespec))
                {
                    // Cannot add blank or null; just return.
                    return;
                }

                foreach (String entry in listBoxItemsToExclude.Items)
                {
                    if (String.Compare(filespec, entry, true) == 0)
                    {
                        QMessageBox.Show("The specified item is already being excluded.", "Emergency Backup", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                listBoxItemsToExclude.Items.Add(filespec);
                EnableDisableExcludeButtons(true);
            }
        }

        private void listBoxItemsToExclude_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxItemsToExclude.Items.Count > 0 && listBoxItemsToExclude.SelectedIndex >= 0)
            {
                EnableDisableExcludeButtons(true);
            }
            else
            {
                EnableDisableExcludeButtons(false);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            SaveConfiguration();
            CloseKeys();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            CloseKeys();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void CloseKeys()
        {
            try
            {
                configurationKey.Close();
            }
            catch
            {
            }

            try
            {
                dojoNorthSubKey.Close();
            }
            catch
            {
            }
        }

        private void LoadConfiguration()
        {
            Array arrayIncludes;
            Array arrayExcludes;

            try
            {
                arrayIncludes = (String[])configurationKey.GetValue(Properties.Resources.RegistryConfigIncludeDirectories);
                listBoxInclude.Items.Clear();
                foreach (String item in arrayIncludes)
                {
                    listBoxInclude.Items.Add(item);
                }
            }
            catch
            {
                listBoxInclude.Items.Clear();
            }

            try
            {
                arrayExcludes = (String[])configurationKey.GetValue(Properties.Resources.RegistryConfigExcludeItems);
                listBoxItemsToExclude.Items.Clear();
                foreach (String item in arrayExcludes)
                {
                    listBoxItemsToExclude.Items.Add(item);
                }
            }
            catch
            {
                listBoxItemsToExclude.Items.Clear();
            }
        }

        private void SaveConfiguration()
        {
            List<String> include = new List<String>();
            List<String> exclude = new List<String>();

            foreach (String item in listBoxInclude.Items)
            {
                include.Add(item);
            }

            foreach (String item in listBoxItemsToExclude.Items)
            {
                exclude.Add(item);
            }

            try
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigIncludeDirectories, include.ToArray());
            }
            catch (Exception ex)
            {
                QMessageBox.Show("Failed to save included folders to the Registry. The data has been lost. " + ex.Message, "Severe",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

            try
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigExcludeItems, exclude.ToArray());
            }
            catch (Exception ex)
            {
                QMessageBox.Show("Failed to save excluded items to the Registry. The data has been lost. " + ex.Message, "Severe",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
    }
}
