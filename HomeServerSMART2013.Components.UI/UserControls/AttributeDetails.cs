using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class AttributeDetails : Form
    {
        private ListViewItem item;
        private bool useDefaultSkinning;
        private bool isWindowsServerSolutions;
        private int thresh = -1;
        private int currentValue = -1;
        private int worstValue = -1;

        public AttributeDetails(ListViewItem lvi, bool defaultSkinning, bool isWss)
        {
            InitializeComponent();

            // UI Updates
            pictureBox1.Size = new Size(base.Width, pictureBox1.Image.Height);
            
            // Get item info.
            item = lvi;

            // Skinning
            useDefaultSkinning = defaultSkinning;

            // Is WSS?
            isWindowsServerSolutions = isWss;
        }

        private void AttributeDetails_Load(object sender, EventArgs e)
        {
            // Update window title...
            this.Text = item.SubItems[4].Text;

            // Skinning...
            if (!useDefaultSkinning)
            {
                pictureBox1.Image = Properties.Resources.HealthTopBannerSBS;
            }

            // And the status label...
            statusLbl.BackColor = Color.Transparent;
            statusLbl.ForeColor = Color.White;
            statusLbl.Font = new Font(new FontFamily("Tahoma"), 10, FontStyle.Bold);
            statusLbl.Parent = this.pictureBox1;
            statusLbl.Anchor = AnchorStyles.Right;
            statusLbl.SetBounds((this.pictureBox1.Width - this.statusLbl.Width) - 15, (this.pictureBox1.Height - this.statusLbl.Height) - 15, this.statusLbl.Width, this.statusLbl.Height);
            statusLbl.BringToFront();
            statusLbl.Text = string.Format("Attribute {0}: {1}", item.SubItems[2].Text, item.SubItems[4].Text);
            labelDec.Text = item.SubItems[2].Text;
            labelHex.Text = item.SubItems[3].Text;
            labelName.Text = item.SubItems[4].Text;
            labelPFA.Text = item.SubItems[6].Text;
            labelThreshold.Text = item.SubItems[7].Text;
            Int32.TryParse(labelThreshold.Text, out thresh);
            labelValue.Text = item.SubItems[8].Text;
            Int32.TryParse(labelValue.Text, out currentValue);
            labelWorst.Text = item.SubItems[9].Text;
            Int32.TryParse(labelWorst.Text, out worstValue);
            textBoxDescription.Text = item.ToolTipText;

            FancyListView.ImageSubItem subItem = (FancyListView.ImageSubItem)item.SubItems[10];
            ListViewItem.ListViewSubItem isCriticalSubItem = (ListViewItem.ListViewSubItem)item.SubItems[5];
            switch (subItem.Text)
            {
                case "Fail":
                case "Critical":
                case "Overheated":
                    {
                        statusLbl.Image = CommonImages.StatusCritical24;
                        this.Icon = CommonImages.StatusCritical24Icon;
                        labelStatus.Image = CommonImages.ErrorImage16x16;

                        if (item.SubItems[2].Text == "190" ||
                            item.SubItems[2].Text == "194" ||
                            item.SubItems[2].Text == "231")
                        {
                            if (subItem.Text == "Fail")
                            {
                                labelStatus.Text = "Extreme disk temperature has caused this attribute to fail.";
                            }
                            else
                            {
                                labelStatus.Text = "Disk Temperature is " + subItem.Text + ". Cool the disk immediately.";
                            }
                        }
                        else
                        {
                            labelStatus.Text = "This attribute has failed. Replace the disk as soon as possible.";
                        }
                        break;
                    }
                case "Degraded":
                case "Warm":
                case "Hot":
                case "Caution":
                case "Geriatric":
                    {
                        statusLbl.Image = CommonImages.StatusAtRisk24;
                        this.Icon = CommonImages.StatusAtRisk24Icon;
                        labelStatus.Image = CommonImages.WarningImage16x16;

                        if (item.SubItems[2].Text == "190" ||
                            item.SubItems[2].Text == "194" ||
                            item.SubItems[2].Text == "231")
                        {
                            labelStatus.Text = "Disk Temperature is too high. Cool the disk as soon as possible.";
                        }
                        else if (subItem.Text == "Geriatric")
                        {
                            labelStatus.Text = "This attribute is Geriatric. Your disk may be approaching the end of its life.";
                        }
                        else
                        {
                            if (isCriticalSubItem.Text == "Yes" && labelThreshold.Text == labelValue.Text)
                            {
                                labelStatus.Text = "The Value equals the Threshold. This attribute could fail at any time.";
                            }
                            else
                            {
                                labelStatus.Text = "Potentially serious problems have been detected on the disk.";
                            }
                        }
                        break;
                    }
                case "Healthy":
                default:
                    {
                        statusLbl.Image = CommonImages.StatusHealthy24;
                        this.Icon = CommonImages.StatusHealthy24Icon;
                        labelStatus.Image = CommonImages.GreenCheckImage16x16;
                        labelStatus.Text = "This attribute is healthy. No further attention is required.";
                        break;
                    }
            }
            
            labelFlags.Text = GetFlagsFromBinary((String)isCriticalSubItem.Tag);
            labelCritical.Text = isCriticalSubItem.Text;
            
            FancyListView.ImageSubItem scSubItem = (FancyListView.ImageSubItem)item.SubItems[1];
            if ((bool)scSubItem.Tag)
            {
                labelSuperCritical.Text = (isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart) + " " +
                    Properties.Resources.ExplanationTextSuperCriticalYes;
            }
            else
            {
                labelSuperCritical.Text = (isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart) + " " +
                    Properties.Resources.ExplanationTextSuperCriticalNo;
            }

            if (String.Compare(isCriticalSubItem.Text, "no", true) == 0 &&
                (bool)scSubItem.Tag)
            {
                labelCritical.Text = "Yes";
                linkLabelSuperCritical.Visible = true;
            }

            if (labelPFA.Text == "Pre-Fail")
            {
                labelPreFailExplanation.Text = Properties.Resources.ExplanationTextPreFail;                
            }
            else
            {
                labelPreFailExplanation.Text = Properties.Resources.ExplanationTextAdvisory;
            }

            tt.SetToolTip(linkLabelSuperCritical, "The disk vendor does not consider this a critical attribute.\nHowever, " + (isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") +
                " considers this a super-critical attribute\nfor all drives, regardless of the vendor flag. Thus, the value\nfor \"Critical\" is shown as \"Yes.\"");

            textBoxDescription.DeselectAll();
            buttonClose.Focus();
        }

        private String GetFlagsFromBinary(String flags)
        {
            if (String.IsNullOrEmpty(flags))
            {
                return "None";
            }
            else if (flags.Length != 8)
            {
                return "Unknown";
            }

            String flagList = String.Empty;

            // Self-Preserving
            if (flags.Substring(2, 1) == "1")
            {
                flagList += "Self-Preserving, ";
            }

            // Error Rate
            if (flags.Substring(4, 1) == "1")
            {
                flagList += "Error Rate, ";
            }

            // Event Count
            if (flags.Substring(3, 1) == "1")
            {
                flagList += "Event Count, ";
            }

            // Performance
            if (flags.Substring(5, 1) == "1")
            {
                flagList += "Performance, ";
            }

            // Statistical
            if (flags.Substring(6, 1) == "1")
            {
                flagList += "Statistical, ";
            }

            // Critical
            if (flags.Substring(7, 1) == "1")
            {
                flagList += "Critical";
            }

            flagList = flagList.Trim();
            if (flagList.EndsWith(","))
            {
                flagList = flagList.Substring(0, flagList.Length - 1);
            }

            return flagList;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AttributeDetails_Shown(object sender, EventArgs e)
        {
            textBoxDescription.DeselectAll();
            buttonClose.Focus();
        }

        private void radioButtonHex_CheckedChanged(object sender, EventArgs e)
        {
            SetDecHexValues(radioButtonHex.Checked);
        }

        private void radioButtonDec_CheckedChanged(object sender, EventArgs e)
        {
            SetDecHexValues(radioButtonHex.Checked);
        }

        private void SetDecHexValues(bool hexadecimal)
        {
            if (hexadecimal)
            {
                if (thresh != -1)
                {
                    labelThreshold.Text = "0x" + thresh.ToString("X");
                }
                else
                {
                    labelThreshold.Text = "NaN";
                }

                if (currentValue != -1)
                {
                    labelValue.Text = "0x" + currentValue.ToString("X");
                }
                else
                {
                    labelValue.Text = "NaN";
                }

                if (worstValue != -1)
                {
                    labelWorst.Text = "0x" + worstValue.ToString("X");
                }
                else
                {
                    labelWorst.Text = "NaN";
                }
            }
            else
            {
                if (thresh != -1)
                {
                    labelThreshold.Text = thresh.ToString();
                }
                else
                {
                    labelThreshold.Text = "NaN";
                }

                if (currentValue != -1)
                {
                    labelValue.Text = currentValue.ToString();
                }
                else
                {
                    labelValue.Text = "NaN";
                }

                if (worstValue != -1)
                {
                    labelWorst.Text = worstValue.ToString();
                }
                else
                {
                    labelWorst.Text = "NaN";
                }
            }
        }
    }
}
