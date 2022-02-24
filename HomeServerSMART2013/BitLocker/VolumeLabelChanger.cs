using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public partial class VolumeLabelChanger : Form
    {
        private String label;
        private String letter;

        public VolumeLabelChanger(String volumeLetter, String volumeLabel)
        {
            InitializeComponent();

            letter = volumeLetter;
            label = volumeLabel;
        }

        private void VolumeLabelChanger_Load(object sender, EventArgs e)
        {
            label1.Text = "Please specify the new volume label for drive " + letter;
            textBoxCurrent.Text = label;
            textBoxNew.Text = label;
            textBoxNew.Focus();
            textBoxNew.SelectAll();
        }

        public String VolumeLabel
        {
            get
            {
                return label;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            label = textBoxNew.Text.Trim();
        }
    }
}
