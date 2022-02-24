namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    partial class UpgradeVolumeDialogue
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpgradeVolumeDialogue));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.currentBitLockerVersion = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.newBitLockerVersion = new System.Windows.Forms.ComboBox();
            this.buttonUpgrade = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(461, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "This tool upgrades the BitLocker metadata on the volume from the Windows Vista fo" +
    "rmat to the Windows 7 format.  This operation is permanent and cannot be undone." +
    "";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(461, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Should you choose to upgrade the BitLocker metadata on the volume:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(355, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "• You cannot downgrade the BitLocker metadata back to Windows Vista.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(34, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(418, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "• You will NOT be able to unlock the drive on Windows Vista or Windows Server 200" +
    "8.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(34, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(354, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "• You will be able to use Windows 7 key protectors such as Passphrases.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(34, 130);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(423, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "• You will be able to use Windows 7 family identification fields and data recover" +
    "y agents.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(34, 152);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(345, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "• On a removable data volume, BitLocker will become BitLocker To Go.";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(34, 174);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(165, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "• Enhanced PINs will be allowed.";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 209);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(130, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Current BitLocker Version:";
            // 
            // currentBitLockerVersion
            // 
            this.currentBitLockerVersion.Location = new System.Drawing.Point(159, 206);
            this.currentBitLockerVersion.Name = "currentBitLockerVersion";
            this.currentBitLockerVersion.ReadOnly = true;
            this.currentBitLockerVersion.Size = new System.Drawing.Size(314, 20);
            this.currentBitLockerVersion.TabIndex = 9;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 247);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(118, 13);
            this.label10.TabIndex = 10;
            this.label10.Text = "New BitLocker Version:";
            // 
            // newBitLockerVersion
            // 
            this.newBitLockerVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.newBitLockerVersion.FormattingEnabled = true;
            this.newBitLockerVersion.Location = new System.Drawing.Point(159, 244);
            this.newBitLockerVersion.Name = "newBitLockerVersion";
            this.newBitLockerVersion.Size = new System.Drawing.Size(314, 21);
            this.newBitLockerVersion.TabIndex = 11;
            // 
            // buttonUpgrade
            // 
            this.buttonUpgrade.Location = new System.Drawing.Point(159, 281);
            this.buttonUpgrade.Name = "buttonUpgrade";
            this.buttonUpgrade.Size = new System.Drawing.Size(75, 33);
            this.buttonUpgrade.TabIndex = 12;
            this.buttonUpgrade.Text = "Upgrade";
            this.buttonUpgrade.UseVisualStyleBackColor = true;
            this.buttonUpgrade.Click += new System.EventHandler(this.buttonUpgrade_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(251, 281);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 33);
            this.buttonCancel.TabIndex = 13;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // UpgradeVolumeDialogue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(485, 329);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonUpgrade);
            this.Controls.Add(this.newBitLockerVersion);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.currentBitLockerVersion);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpgradeVolumeDialogue";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Upgrade BitLocker to Windows 7";
            this.Load += new System.EventHandler(this.UpgradeVolumeDialogue_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox currentBitLockerVersion;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox newBitLockerVersion;
        private System.Windows.Forms.Button buttonUpgrade;
        private System.Windows.Forms.Button buttonCancel;
    }
}