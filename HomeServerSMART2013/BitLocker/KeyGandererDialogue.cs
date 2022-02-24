using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public partial class KeyGandererDialogue : Form
    {
        private EncryptableVolume volume;

        public KeyGandererDialogue(ArrayList listOfKeys, String caption, EncryptableVolume vol)
        {
            InitializeComponent();

            label1.Text = caption;

            foreach (KeyProtectorInfo kpi in listOfKeys)
            {
                if (kpi.BekFileName == null || kpi.BekFileName == String.Empty)
                {
                    listView1.Items.Add(new ListViewItem(new String[] { kpi.ID, kpi.Type.ToString(), kpi.FriendlyName, kpi.Password }));
                }
                else
                {
                    listView1.Items.Add(new ListViewItem(new String[] { kpi.ID, kpi.Type.ToString(), kpi.FriendlyName, kpi.BekFileName }));
                }
            }

            volume = vol;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            String keyID = String.Empty;
            String keyType = String.Empty;
            String password = String.Empty;

            try
            {
                keyID = listView1.SelectedItems[0].SubItems[0].Text;
                keyType = listView1.SelectedItems[0].SubItems[1].Text;
                password = listView1.SelectedItems[0].SubItems[3].Text;
            }
            catch
            {
                return;
            }

            switch (keyType)
            {
                case "EXTERNAL_KEY":
                case "TPM_AND_STARTUP_KEY":
                case "TPM_AND_PIN_AND_STARTUP_KEY":
                    {
                        String path = String.Empty;
                        FolderBrowserDialog fbd = new FolderBrowserDialog();
                        fbd.Description = "Select a save location for the BEK file " + password + ".  File will be saved with System, Read-Only " +
                            "and Hidden attributes.";
                        fbd.ShowDialog();
                        if (fbd.SelectedPath != String.Empty)
                        {
                            path = fbd.SelectedPath;
                        }
                        else
                        {
                            return;
                        }
                        
                        BitLockerVolumeStatus bvms = (BitLockerVolumeStatus)volume.SaveExternalKeyToFile(path, keyID);

                        switch (bvms)
                        {
                            case BitLockerVolumeStatus.S_OK:
                                {
                                    MessageBox.Show(password + " was successfully saved to " + path + ".",
                                        "Save BEK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    break;
                                }
                            case BitLockerVolumeStatus.FVE_E_LOCKED_VOLUME:
                                {
                                    MessageBox.Show(password + " could not be saved because the volume is now locked.",
                                        "Save BEK", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    break;
                                }
                            case BitLockerVolumeStatus.E_INVALIDARG:
                                {
                                    MessageBox.Show(password + " could not be saved because the key protector is not a BEK.",
                                        "Save BEK", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    break;
                                }
                            case BitLockerVolumeStatus.ERROR_PATH_NOT_FOUND:
                                {
                                    MessageBox.Show(password + " could not be saved because the path " + path + " is invalid.",
                                        "Save BEK", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    break;
                                }
                            default:
                                {
                                    MessageBox.Show("BitLocker returned an unexpected error:  " + bvms.ToString(),
                                        "Severe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                        }

                        break;
                    }
                case "NUMERIC_PASSWORD":
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();

                        sb.Append("Security - It's everyone's responsibility.");
                        sb.Append("\r\n\r\n");
                        sb.Append("Recovery Password Details for Volume " + volume.DriveLetter);
                        sb.Append("\r\n\r\n");
                        sb.Append("Volume ID:  " + keyID);
                        sb.Append("\r\n");
                        sb.Append("Password :  " + password);
                        sb.Append("\r\n\r\n");
                        sb.Append("Security is everyone's responsibility.  Keep this information in a safe place.");

                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Title = "Save Recovery Password Details";
                        sfd.Filter = "Text files (*.txt)|*.txt";
                        sfd.ShowDialog();

                        if (sfd.FileName != String.Empty)
                        {
                            try
                            {
                                System.IO.StreamWriter writer = new System.IO.StreamWriter(sfd.FileName);
                                writer.Write(sb.ToString());
                                writer.Flush();
                                writer.Close();
                                writer.Dispose();
                                MessageBox.Show("Recovery Password details saved successfully.", "Save Password",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Exceptions were detected writing the file:  " + ex.Message, "Save Password",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }

                        break;
                    }
                case "ALL_TYPES":
                case "TPM_AND_PIN":
                case "TRUSTED_PLATFORM_MODULE":
                default:
                    {
                        MessageBox.Show("The selected key does not contain any saveable data.", "Save Key Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        break;
                    }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count == 0)
                {
                    btnSave.Enabled = false;
                }
                else
                {
                    btnSave.Enabled = true;
                }
            }
            catch
            {
                btnSave.Enabled = false;
            }
        }
    }
}
