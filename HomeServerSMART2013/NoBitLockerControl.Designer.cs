using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI
{
    partial class NoBitLockerControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NoBitLockerControl));
            this.buttonEncrypt = new System.Windows.Forms.Button();
            this.buttonDecrypt = new System.Windows.Forms.Button();
            this.buttonLock = new System.Windows.Forms.Button();
            this.buttonUnlock = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.encryptionProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.volumeStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonDocumentation = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonOptions = new WSSControls.BelovedComponents.SplitButton();
            this.buttonAdvanced = new WSSControls.BelovedComponents.SplitButton();
            this.buttonProtectors = new WSSControls.BelovedComponents.SplitButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonInstallNow = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonEncrypt
            // 
            this.buttonEncrypt.Enabled = false;
            this.buttonEncrypt.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEncrypt.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Encrypt48;
            this.buttonEncrypt.Location = new System.Drawing.Point(17, 4);
            this.buttonEncrypt.Name = "buttonEncrypt";
            this.buttonEncrypt.Size = new System.Drawing.Size(120, 72);
            this.buttonEncrypt.TabIndex = 0;
            this.buttonEncrypt.Text = "Encrypt Disk";
            this.buttonEncrypt.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonEncrypt.UseVisualStyleBackColor = true;
            // 
            // buttonDecrypt
            // 
            this.buttonDecrypt.Enabled = false;
            this.buttonDecrypt.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDecrypt.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Decrypt48;
            this.buttonDecrypt.Location = new System.Drawing.Point(143, 4);
            this.buttonDecrypt.Name = "buttonDecrypt";
            this.buttonDecrypt.Size = new System.Drawing.Size(120, 72);
            this.buttonDecrypt.TabIndex = 1;
            this.buttonDecrypt.Text = "Decrypt Disk";
            this.buttonDecrypt.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonDecrypt.UseVisualStyleBackColor = true;
            // 
            // buttonLock
            // 
            this.buttonLock.Enabled = false;
            this.buttonLock.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLock.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Lock24;
            this.buttonLock.Location = new System.Drawing.Point(269, 4);
            this.buttonLock.Name = "buttonLock";
            this.buttonLock.Size = new System.Drawing.Size(128, 32);
            this.buttonLock.TabIndex = 2;
            this.buttonLock.Text = "Lock";
            this.buttonLock.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonLock.UseVisualStyleBackColor = true;
            // 
            // buttonUnlock
            // 
            this.buttonUnlock.Enabled = false;
            this.buttonUnlock.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUnlock.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Unlock24;
            this.buttonUnlock.Location = new System.Drawing.Point(269, 44);
            this.buttonUnlock.Name = "buttonUnlock";
            this.buttonUnlock.Size = new System.Drawing.Size(128, 32);
            this.buttonUnlock.TabIndex = 3;
            this.buttonUnlock.Text = "Unlock";
            this.buttonUnlock.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonUnlock.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBar,
            this.toolStripStatusLabel3,
            this.encryptionProgress,
            this.toolStripStatusLabel1,
            this.volumeStatus,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 512);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(982, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusBar
            // 
            this.statusBar.AutoSize = false;
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(500, 17);
            this.statusBar.Text = "Welcome to Taryn BitLocker Manager for Home Server SMART!";
            this.statusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel3.Text = " ";
            // 
            // encryptionProgress
            // 
            this.encryptionProgress.Name = "encryptionProgress";
            this.encryptionProgress.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel1.Text = " ";
            // 
            // volumeStatus
            // 
            this.volumeStatus.AutoSize = false;
            this.volumeStatus.Name = "volumeStatus";
            this.volumeStatus.Size = new System.Drawing.Size(300, 17);
            this.volumeStatus.Text = "BitLocker Not Enabled on this Server";
            this.volumeStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(45, 17);
            this.toolStripStatusLabel2.Spring = true;
            this.toolStripStatusLabel2.Text = " ";
            // 
            // buttonDocumentation
            // 
            this.buttonDocumentation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDocumentation.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDocumentation.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Help48;
            this.buttonDocumentation.Location = new System.Drawing.Point(846, 4);
            this.buttonDocumentation.Name = "buttonDocumentation";
            this.buttonDocumentation.Size = new System.Drawing.Size(120, 72);
            this.buttonDocumentation.TabIndex = 19;
            this.buttonDocumentation.Text = "Documentation";
            this.buttonDocumentation.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonDocumentation.UseVisualStyleBackColor = true;
            this.buttonDocumentation.Click += new System.EventHandler(this.buttonDocumentation_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Enabled = false;
            this.buttonRefresh.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRefresh.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.RefreshDisk24;
            this.buttonRefresh.Location = new System.Drawing.Point(403, 4);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(128, 32);
            this.buttonRefresh.TabIndex = 20;
            this.buttonRefresh.Text = "Refresh View";
            this.buttonRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonRefresh.UseVisualStyleBackColor = true;
            // 
            // buttonOptions
            // 
            this.buttonOptions.AlwaysDropDown = true;
            this.buttonOptions.ClickedImage = "Clicked";
            this.buttonOptions.DisabledImage = "Disabled";
            this.buttonOptions.Enabled = false;
            this.buttonOptions.FocusedImage = "Focused";
            this.buttonOptions.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOptions.HoverImage = "Hover";
            this.buttonOptions.ImageKey = "Normal";
            this.buttonOptions.Location = new System.Drawing.Point(537, 4);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.NormalImage = "Normal";
            this.buttonOptions.Size = new System.Drawing.Size(128, 32);
            this.buttonOptions.TabIndex = 22;
            this.buttonOptions.Text = "Options";
            this.buttonOptions.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.buttonOptions.UseVisualStyleBackColor = true;
            // 
            // buttonAdvanced
            // 
            this.buttonAdvanced.AlwaysDropDown = true;
            this.buttonAdvanced.ClickedImage = "Clicked";
            this.buttonAdvanced.DisabledImage = "Disabled";
            this.buttonAdvanced.Enabled = false;
            this.buttonAdvanced.FocusedImage = "Focused";
            this.buttonAdvanced.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAdvanced.HoverImage = "Hover";
            this.buttonAdvanced.ImageKey = "Normal";
            this.buttonAdvanced.Location = new System.Drawing.Point(537, 44);
            this.buttonAdvanced.Name = "buttonAdvanced";
            this.buttonAdvanced.NormalImage = "Normal";
            this.buttonAdvanced.Size = new System.Drawing.Size(128, 32);
            this.buttonAdvanced.TabIndex = 21;
            this.buttonAdvanced.Text = "Advanced";
            this.buttonAdvanced.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.buttonAdvanced.UseVisualStyleBackColor = true;
            // 
            // buttonProtectors
            // 
            this.buttonProtectors.AlwaysDropDown = true;
            this.buttonProtectors.ClickedImage = "Clicked";
            this.buttonProtectors.DisabledImage = "Disabled";
            this.buttonProtectors.Enabled = false;
            this.buttonProtectors.FocusedImage = "Focused";
            this.buttonProtectors.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonProtectors.HoverImage = "Hover";
            this.buttonProtectors.ImageKey = "Normal";
            this.buttonProtectors.Location = new System.Drawing.Point(403, 44);
            this.buttonProtectors.Name = "buttonProtectors";
            this.buttonProtectors.NormalImage = "Normal";
            this.buttonProtectors.Size = new System.Drawing.Size(128, 32);
            this.buttonProtectors.TabIndex = 5;
            this.buttonProtectors.Text = "Protectors";
            this.buttonProtectors.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.buttonProtectors.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.IgnoreProblem48;
            this.pictureBox1.Location = new System.Drawing.Point(17, 109);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 23;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(70, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(667, 86);
            this.label1.TabIndex = 24;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // buttonInstallNow
            // 
            this.buttonInstallNow.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.ShieldMulti16;
            this.buttonInstallNow.Location = new System.Drawing.Point(74, 179);
            this.buttonInstallNow.Name = "buttonInstallNow";
            this.buttonInstallNow.Size = new System.Drawing.Size(107, 32);
            this.buttonInstallNow.TabIndex = 25;
            this.buttonInstallNow.Text = "Install It Now...";
            this.buttonInstallNow.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonInstallNow.UseVisualStyleBackColor = true;
            this.buttonInstallNow.Click += new System.EventHandler(this.buttonInstallNow_Click);
            // 
            // NoBitLockerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonInstallNow);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonOptions);
            this.Controls.Add(this.buttonAdvanced);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonDocumentation);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.buttonProtectors);
            this.Controls.Add(this.buttonUnlock);
            this.Controls.Add(this.buttonLock);
            this.Controls.Add(this.buttonDecrypt);
            this.Controls.Add(this.buttonEncrypt);
            this.MaximumSize = new System.Drawing.Size(65535, 65535);
            this.MinimumSize = new System.Drawing.Size(770, 350);
            this.Name = "NoBitLockerControl";
            this.Size = new System.Drawing.Size(982, 534);
            this.Load += new System.EventHandler(this.BitLockerControl_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonEncrypt;
        private System.Windows.Forms.Button buttonDecrypt;
        private System.Windows.Forms.Button buttonLock;
        private System.Windows.Forms.Button buttonUnlock;
        private WSSControls.BelovedComponents.SplitButton buttonProtectors;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusBar;
        private System.Windows.Forms.ToolStripProgressBar encryptionProgress;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel volumeStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.Button buttonDocumentation;
        private System.Windows.Forms.Button buttonRefresh;
        private WSSControls.BelovedComponents.SplitButton buttonAdvanced;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private WSSControls.BelovedComponents.SplitButton buttonOptions;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonInstallNow;
    }
}
