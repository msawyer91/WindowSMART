namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI
{
    partial class DiskPoolAutoUnlock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiskPoolAutoUnlock));
            this.label1 = new System.Windows.Forms.Label();
            this.labelServiceStatus = new System.Windows.Forms.Label();
            this.pictureBoxServiceStatus = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonEnable = new System.Windows.Forms.Button();
            this.buttonDisable = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.lvEncryptableVolumes = new WSSControls.BelovedComponents.FancyListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxServiceStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(753, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // labelServiceStatus
            // 
            this.labelServiceStatus.AutoSize = true;
            this.labelServiceStatus.Location = new System.Drawing.Point(37, 62);
            this.labelServiceStatus.Name = "labelServiceStatus";
            this.labelServiceStatus.Size = new System.Drawing.Size(199, 13);
            this.labelServiceStatus.TabIndex = 7;
            this.labelServiceStatus.Text = "Exceptions were detected by the Server.";
            // 
            // pictureBoxServiceStatus
            // 
            this.pictureBoxServiceStatus.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Critical16;
            this.pictureBoxServiceStatus.Location = new System.Drawing.Point(15, 60);
            this.pictureBoxServiceStatus.Name = "pictureBoxServiceStatus";
            this.pictureBoxServiceStatus.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxServiceStatus.TabIndex = 6;
            this.pictureBoxServiceStatus.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Select a volume to manage:";
            // 
            // buttonEnable
            // 
            this.buttonEnable.Enabled = false;
            this.buttonEnable.Location = new System.Drawing.Point(269, 264);
            this.buttonEnable.Name = "buttonEnable";
            this.buttonEnable.Size = new System.Drawing.Size(75, 23);
            this.buttonEnable.TabIndex = 11;
            this.buttonEnable.Text = "Enable AU";
            this.buttonEnable.UseVisualStyleBackColor = true;
            this.buttonEnable.Click += new System.EventHandler(this.buttonEnable_Click);
            // 
            // buttonDisable
            // 
            this.buttonDisable.Enabled = false;
            this.buttonDisable.Location = new System.Drawing.Point(350, 264);
            this.buttonDisable.Name = "buttonDisable";
            this.buttonDisable.Size = new System.Drawing.Size(75, 23);
            this.buttonDisable.TabIndex = 12;
            this.buttonDisable.Text = "Disable AU";
            this.buttonDisable.UseVisualStyleBackColor = true;
            this.buttonDisable.Click += new System.EventHandler(this.buttonDisable_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(431, 264);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 14;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // lvEncryptableVolumes
            // 
            this.lvEncryptableVolumes.BitmapList = null;
            this.lvEncryptableVolumes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvEncryptableVolumes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.lvEncryptableVolumes.FullRowSelect = true;
            this.lvEncryptableVolumes.Location = new System.Drawing.Point(15, 119);
            this.lvEncryptableVolumes.MultiSelect = false;
            this.lvEncryptableVolumes.Name = "lvEncryptableVolumes";
            this.lvEncryptableVolumes.OwnerDraw = true;
            this.lvEncryptableVolumes.Size = new System.Drawing.Size(750, 129);
            this.lvEncryptableVolumes.TabIndex = 13;
            this.lvEncryptableVolumes.UseCompatibleStateImageBehavior = false;
            this.lvEncryptableVolumes.View = System.Windows.Forms.View.Details;
            this.lvEncryptableVolumes.SelectedIndexChanged += new System.EventHandler(this.lvEncryptableVolumes_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Letter/MP/Path";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "BitLocker Volume ID";
            this.columnHeader2.Width = 180;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Volume Label";
            this.columnHeader3.Width = 120;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Pool Automatic Unlock Setting";
            this.columnHeader4.Width = 250;
            // 
            // DiskPoolAutoUnlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 302);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.lvEncryptableVolumes);
            this.Controls.Add(this.buttonDisable);
            this.Controls.Add(this.buttonEnable);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelServiceStatus);
            this.Controls.Add(this.pictureBoxServiceStatus);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DiskPoolAutoUnlock";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Automatic Unlock Manager for Pooled Disks";
            this.Load += new System.EventHandler(this.DiskPoolAutoUnlock_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxServiceStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelServiceStatus;
        private System.Windows.Forms.PictureBox pictureBoxServiceStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonEnable;
        private System.Windows.Forms.Button buttonDisable;
        private WSSControls.BelovedComponents.FancyListView lvEncryptableVolumes;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button buttonClose;
    }
}