using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    partial class SmartStatus
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.statusLbl = new WSSControls.BelovedComponents.TextImageLabel();
            this.messageListBoxSmartStatus = new WSSControls.BelovedComponents.MessageListBox();
            this.qButton1 = new WSSControls.BelovedComponents.QButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.HealthTopBanner418;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(418, 57);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // statusLbl
            // 
            this.statusLbl.AutoSize = true;
            this.statusLbl.BackColor = System.Drawing.Color.Transparent;
            this.statusLbl.FlatAppearance.BorderSize = 0;
            this.statusLbl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.statusLbl.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.statusLbl.Location = new System.Drawing.Point(304, 22);
            this.statusLbl.Name = "statusLbl";
            this.statusLbl.Size = new System.Drawing.Size(95, 23);
            this.statusLbl.TabIndex = 1;
            this.statusLbl.TabStop = false;
            this.statusLbl.Text = "statusLbl";
            this.statusLbl.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.statusLbl.UseVisualStyleBackColor = false;
            // 
            // messageListBoxSmartStatus
            // 
            this.messageListBoxSmartStatus.AutoScroll = true;
            this.messageListBoxSmartStatus.BackColor = System.Drawing.SystemColors.Window;
            this.messageListBoxSmartStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.messageListBoxSmartStatus.Location = new System.Drawing.Point(0, 63);
            this.messageListBoxSmartStatus.Name = "messageListBoxSmartStatus";
            this.messageListBoxSmartStatus.Size = new System.Drawing.Size(418, 436);
            this.messageListBoxSmartStatus.TabIndex = 2;
            this.messageListBoxSmartStatus.Paint += new System.Windows.Forms.PaintEventHandler(this.messageListBoxSmartStatus_Paint);
            // 
            // qButton1
            // 
            this.qButton1.AutoSize = true;
            this.qButton1.BackColor = System.Drawing.Color.Transparent;
            this.qButton1.DisabledForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.qButton1.FlatAppearance.BorderSize = 0;
            this.qButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.qButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.qButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.qButton1.Font = new System.Drawing.Font("Tahoma", 8F);
            this.qButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.qButton1.IsHovered = false;
            this.qButton1.IsPressed = false;
            this.qButton1.Location = new System.Drawing.Point(343, 505);
            this.qButton1.Margins = 0;
            this.qButton1.MaximumSize = new System.Drawing.Size(360, 21);
            this.qButton1.MinimumSize = new System.Drawing.Size(72, 21);
            this.qButton1.Name = "qButton1";
            this.qButton1.Size = new System.Drawing.Size(72, 21);
            this.qButton1.TabIndex = 3;
            this.qButton1.Text = "Close";
            this.qButton1.UseVisualStyleBackColor = false;
            this.qButton1.Click += new System.EventHandler(this.qButton1_Click);
            // 
            // SmartStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.qButton1;
            this.ClientSize = new System.Drawing.Size(420, 532);
            this.Controls.Add(this.qButton1);
            this.Controls.Add(this.messageListBoxSmartStatus);
            this.Controls.Add(this.statusLbl);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SmartStatus";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "S.M.A.R.T. Status";
            this.Load += new System.EventHandler(this.SmartStatus_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private TextImageLabel statusLbl;
        private MessageListBox messageListBoxSmartStatus;
        private QButton qButton1;
    }
}