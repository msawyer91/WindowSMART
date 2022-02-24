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
    public partial class KeyProtectorDialogue : Form
    {
        private String selectedKey;

        public KeyProtectorDialogue(ArrayList listOfKeys, String caption, String thirdColumnName, bool isReplace, bool isEscrow)
        {
            InitializeComponent();

            label1.Text = caption;
            columnHeader3.Text = thirdColumnName;

            foreach (KeyProtectorInfo kpi in listOfKeys)
            {
                if (kpi.BekFileName == null || kpi.BekFileName == String.Empty)
                {
                    listView1.Items.Add(new ListViewItem(new String[] { kpi.ID, kpi.FriendlyName, kpi.Password }));
                }
                else
                {
                    listView1.Items.Add(new ListViewItem(new String[] { kpi.ID, kpi.FriendlyName, kpi.BekFileName }));
                }
            }

            if (isReplace)
            {
                this.Text = "Replace Key Protectors";
                btnDelete.Text = "Replace Key";
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyGandererDialogue));
                this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            }
            else if (isEscrow)
            {
                this.Text = "Escrow Key to Active Directory";
                btnDelete.Text = "Escrow Key";
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyGandererDialogue));
                this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count == 0)
                {
                    btnDelete.Enabled = false;
                }
                else
                {
                    btnDelete.Enabled = true;
                }
            }
            catch
            {
                btnDelete.Enabled = false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                selectedKey = listView1.SelectedItems[0].SubItems[0].Text;
            }
            catch
            {
                selectedKey = String.Empty;
            }
        }

        public String SelectedKey
        {
            get
            {
                return selectedKey;
            }
        }
    }
}
