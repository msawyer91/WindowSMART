using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public partial class DecryptVolumeDialogue : Form
    {
        private String driveLetter;

        public DecryptVolumeDialogue(String letter, String label)
        {
            GPConfig.ReloadConfiguration();
            InitializeComponent();
            
            driveLetter = letter;
            labelVolume.Text = "Volume to Decrypt: " + letter;
            labelVolumeLabel.Text = "Volume Label: " + label;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckChangeUpdateUI();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            CheckChangeUpdateUI();
        }

        private void CheckChangeUpdateUI()
        {
            if (checkBox1.Checked && checkBox2.Checked)
            {
                textBoxConfirm.ReadOnly = false;
            }
            else
            {
                btnDecrypt.Enabled = false;
                textBoxConfirm.Text = String.Empty;
                textBoxConfirm.ReadOnly = true;
            }
        }

        private void textBoxConfirm_TextChanged(object sender, EventArgs e)
        {
            if (String.Compare(textBoxConfirm.Text.Trim(), "Yes", false) == 0)
            {
                btnDecrypt.Enabled = true;
            }
            else
            {
                btnDecrypt.Enabled = false;
            }
        }

        private void DecryptVolumeDialogue_Load(object sender, EventArgs e)
        {

        }
    }
}
