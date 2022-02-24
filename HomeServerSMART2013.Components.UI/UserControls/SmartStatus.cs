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
    public partial class SmartStatus : Form
    {
        private List<MessageListBoxItem> messageList;
        private bool useDefaultSkinning;

        public SmartStatus(bool defaultSkinning)
        {
            InitializeComponent();

            // UI Updates
            pictureBox1.Size = new Size(base.Width, pictureBox1.Image.Height);

            messageList = new List<MessageListBoxItem>();
            useDefaultSkinning = defaultSkinning;
        }

        private void qButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SmartStatus_Load(object sender, EventArgs e)
        {
            if (!useDefaultSkinning)
            {
                pictureBox1.Image = Properties.Resources.HealthTopBanner418SBS;
            }

            foreach (MessageListBoxItem item in messageList)
            {
                messageListBoxSmartStatus.AddItem(item);
            }
        }

        public void AddItemToPanel(String messageTitle, String messageBody, bool isCritical, bool isWarning)
        {
            MessageListBoxItem newItem = new MessageListBoxItem((isCritical ? Color.Red : (isWarning ? Color.Yellow : Color.Green)));
            newItem.Title = messageTitle;
            newItem.Description.Text = messageBody;
            newItem.Icon = ((isCritical ? CommonImages.StatusCritical24Icon :
                (isWarning ? CommonImages.StatusAtRisk24Icon : CommonImages.StatusHealthy24Icon)));
            //messageListBoxSmartStatus.AddItem(newItem);
            messageList.Add(newItem);
        }

        /// <summary>
        /// Sets the window title with the desired status label and colored icon.
        /// </summary>
        /// <param name="title">Text to display as the title.</param>
        /// <param name="isCritical">true if status is critical; false if not.</param>
        /// <param name="isWarning">true if status is warning; false if not (note a true critical status takes precedence).</param>
        /// <param name="isWmiFailurePredicted">true if WMI is predicting a failure.</param>
        public void SetWindowTitle(String title, bool isCritical, bool isWarning, bool isWmiFailurePredicted, bool reportWmi)
        {
            if (reportWmi)
            {
                statusLbl.BackColor = Color.Transparent;
                statusLbl.ForeColor = Color.White;
                statusLbl.Font = new Font(new FontFamily("Tahoma"), 10, FontStyle.Bold);
                statusLbl.Parent = this.pictureBox1;
                statusLbl.Anchor = AnchorStyles.Right;
                statusLbl.SetBounds((this.pictureBox1.Width - this.statusLbl.Width) - 15, (this.pictureBox1.Height - this.statusLbl.Height) - 15, this.statusLbl.Width, this.statusLbl.Height);
                statusLbl.BringToFront();
                statusLbl.Text = string.Format("Status: {0}", title);
                statusLbl.Image = ((isWmiFailurePredicted ? CommonImages.StatusCritical24 : CommonImages.StatusAtRisk24));
                this.Icon = ((isWmiFailurePredicted ? CommonImages.StatusCritical24Icon : CommonImages.StatusAtRisk24Icon));
            }
            else
            {
                statusLbl.BackColor = Color.Transparent;
                statusLbl.ForeColor = Color.White;
                statusLbl.Font = new Font(new FontFamily("Tahoma"), 10, FontStyle.Bold);
                statusLbl.Parent = this.pictureBox1;
                statusLbl.Anchor = AnchorStyles.Right;
                statusLbl.SetBounds((this.pictureBox1.Width - this.statusLbl.Width) - 15, (this.pictureBox1.Height - this.statusLbl.Height) - 15, this.statusLbl.Width, this.statusLbl.Height);
                statusLbl.BringToFront();
                statusLbl.Text = string.Format("Status: {0}", title);
                statusLbl.Image = ((isCritical ? CommonImages.StatusCritical24 :
                    (isWarning ? CommonImages.StatusAtRisk24 : CommonImages.StatusHealthy24)));
                this.Icon = ((isCritical ? CommonImages.StatusCritical24Icon :
                    (isWarning ? CommonImages.StatusAtRisk24Icon : CommonImages.StatusHealthy24Icon)));
            }
        }

        public void SetWindowTitle(String title, bool isCritical, bool isWarning)
        {
            SetWindowTitle(title, isCritical, isWarning, false, false);
        }

        private void messageListBoxSmartStatus_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
