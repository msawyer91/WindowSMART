namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    partial class EncryptSystemVolumeDialogue
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EncryptSystemVolumeDialogue));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBrowseRecoveryKey = new System.Windows.Forms.Button();
            this.tbRecoveryKeyLocation = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnBrowseRecoveryPassword = new System.Windows.Forms.Button();
            this.tbRecoveryPwLocation = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cbRecoveryKey = new System.Windows.Forms.CheckBox();
            this.cbRecoveryPw = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.encryptionMethod = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbNoTpm = new System.Windows.Forms.CheckBox();
            this.btnBrowseStartupKey = new System.Windows.Forms.Button();
            this.tbStartupKeyLocation = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbConfirmPin = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbPin = new System.Windows.Forms.TextBox();
            this.labelPin = new System.Windows.Forms.Label();
            this.tpmStartupOption = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labelAD = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbSkipCheck = new System.Windows.Forms.CheckBox();
            this.cbReboot = new System.Windows.Forms.CheckBox();
            this.cbPerformCheck = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEncrypt = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(68, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(660, 44);
            this.label2.TabIndex = 6;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(66, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(386, 29);
            this.label1.TabIndex = 5;
            this.label1.Text = "Encrypt the Operating System Volume";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnBrowseRecoveryKey);
            this.groupBox1.Controls.Add(this.tbRecoveryKeyLocation);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnBrowseRecoveryPassword);
            this.groupBox1.Controls.Add(this.tbRecoveryPwLocation);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.cbRecoveryKey);
            this.groupBox1.Controls.Add(this.cbRecoveryPw);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.encryptionMethod);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 85);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(350, 193);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "All BitLocker Editions";
            // 
            // btnBrowseRecoveryKey
            // 
            this.btnBrowseRecoveryKey.Enabled = false;
            this.btnBrowseRecoveryKey.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Save;
            this.btnBrowseRecoveryKey.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBrowseRecoveryKey.Location = new System.Drawing.Point(266, 160);
            this.btnBrowseRecoveryKey.Name = "btnBrowseRecoveryKey";
            this.btnBrowseRecoveryKey.Size = new System.Drawing.Size(78, 27);
            this.btnBrowseRecoveryKey.TabIndex = 18;
            this.btnBrowseRecoveryKey.Text = "Browse...";
            this.btnBrowseRecoveryKey.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBrowseRecoveryKey.UseVisualStyleBackColor = true;
            this.btnBrowseRecoveryKey.Click += new System.EventHandler(this.btnBrowseRecoveryKey_Click);
            // 
            // tbRecoveryKeyLocation
            // 
            this.tbRecoveryKeyLocation.Location = new System.Drawing.Point(6, 164);
            this.tbRecoveryKeyLocation.Name = "tbRecoveryKeyLocation";
            this.tbRecoveryKeyLocation.ReadOnly = true;
            this.tbRecoveryKeyLocation.Size = new System.Drawing.Size(248, 20);
            this.tbRecoveryKeyLocation.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 148);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(149, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Recovery Key Location (USB)";
            // 
            // btnBrowseRecoveryPassword
            // 
            this.btnBrowseRecoveryPassword.Enabled = false;
            this.btnBrowseRecoveryPassword.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Save;
            this.btnBrowseRecoveryPassword.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBrowseRecoveryPassword.Location = new System.Drawing.Point(266, 123);
            this.btnBrowseRecoveryPassword.Name = "btnBrowseRecoveryPassword";
            this.btnBrowseRecoveryPassword.Size = new System.Drawing.Size(78, 27);
            this.btnBrowseRecoveryPassword.TabIndex = 16;
            this.btnBrowseRecoveryPassword.Text = "Browse...";
            this.btnBrowseRecoveryPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBrowseRecoveryPassword.UseVisualStyleBackColor = true;
            this.btnBrowseRecoveryPassword.Click += new System.EventHandler(this.btnBrowseRecoveryPassword_Click);
            // 
            // tbRecoveryPwLocation
            // 
            this.tbRecoveryPwLocation.Location = new System.Drawing.Point(6, 127);
            this.tbRecoveryPwLocation.Name = "tbRecoveryPwLocation";
            this.tbRecoveryPwLocation.ReadOnly = true;
            this.tbRecoveryPwLocation.Size = new System.Drawing.Size(248, 20);
            this.tbRecoveryPwLocation.TabIndex = 15;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 111);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(149, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "Recovery Password Location:";
            // 
            // cbRecoveryKey
            // 
            this.cbRecoveryKey.AutoSize = true;
            this.cbRecoveryKey.Checked = true;
            this.cbRecoveryKey.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRecoveryKey.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbRecoveryKey.Location = new System.Drawing.Point(125, 84);
            this.cbRecoveryKey.Name = "cbRecoveryKey";
            this.cbRecoveryKey.Size = new System.Drawing.Size(142, 18);
            this.cbRecoveryKey.TabIndex = 4;
            this.cbRecoveryKey.Text = "Create a Recovery Key";
            this.cbRecoveryKey.UseVisualStyleBackColor = true;
            this.cbRecoveryKey.CheckedChanged += new System.EventHandler(this.cbRecoveryKey_CheckedChanged);
            // 
            // cbRecoveryPw
            // 
            this.cbRecoveryPw.AutoSize = true;
            this.cbRecoveryPw.Checked = true;
            this.cbRecoveryPw.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRecoveryPw.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbRecoveryPw.Location = new System.Drawing.Point(125, 61);
            this.cbRecoveryPw.Name = "cbRecoveryPw";
            this.cbRecoveryPw.Size = new System.Drawing.Size(160, 18);
            this.cbRecoveryPw.TabIndex = 3;
            this.cbRecoveryPw.Text = "Create a 48-Digit Password";
            this.cbRecoveryPw.UseVisualStyleBackColor = true;
            this.cbRecoveryPw.CheckedChanged += new System.EventHandler(this.cbRecoveryPw_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Recovery Options";
            // 
            // encryptionMethod
            // 
            this.encryptionMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.encryptionMethod.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.encryptionMethod.FormattingEnabled = true;
            this.encryptionMethod.Items.AddRange(new object[] {
            "AES-128 with Diffuser",
            "AES-256 with Diffuser",
            "AES-128",
            "AES-256"});
            this.encryptionMethod.Location = new System.Drawing.Point(125, 28);
            this.encryptionMethod.Name = "encryptionMethod";
            this.encryptionMethod.Size = new System.Drawing.Size(219, 21);
            this.encryptionMethod.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Encryption Method";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbNoTpm);
            this.groupBox2.Controls.Add(this.btnBrowseStartupKey);
            this.groupBox2.Controls.Add(this.tbStartupKeyLocation);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.tbConfirmPin);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.tbPin);
            this.groupBox2.Controls.Add(this.labelPin);
            this.groupBox2.Controls.Add(this.tpmStartupOption);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(378, 85);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(350, 193);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Trusted Platform Module (TPM)";
            // 
            // cbNoTpm
            // 
            this.cbNoTpm.AutoSize = true;
            this.cbNoTpm.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbNoTpm.Location = new System.Drawing.Point(9, 165);
            this.cbNoTpm.Name = "cbNoTpm";
            this.cbNoTpm.Size = new System.Drawing.Size(174, 18);
            this.cbNoTpm.TabIndex = 11;
            this.cbNoTpm.Text = "Require Startup Key (no TPM)";
            this.cbNoTpm.UseVisualStyleBackColor = true;
            // 
            // btnBrowseStartupKey
            // 
            this.btnBrowseStartupKey.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Save;
            this.btnBrowseStartupKey.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBrowseStartupKey.Location = new System.Drawing.Point(266, 123);
            this.btnBrowseStartupKey.Name = "btnBrowseStartupKey";
            this.btnBrowseStartupKey.Size = new System.Drawing.Size(78, 27);
            this.btnBrowseStartupKey.TabIndex = 10;
            this.btnBrowseStartupKey.Text = "Browse...";
            this.btnBrowseStartupKey.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBrowseStartupKey.UseVisualStyleBackColor = true;
            this.btnBrowseStartupKey.Click += new System.EventHandler(this.btnBrowseStartupKey_Click);
            // 
            // tbStartupKeyLocation
            // 
            this.tbStartupKeyLocation.Location = new System.Drawing.Point(6, 127);
            this.tbStartupKeyLocation.Name = "tbStartupKeyLocation";
            this.tbStartupKeyLocation.ReadOnly = true;
            this.tbStartupKeyLocation.Size = new System.Drawing.Size(248, 20);
            this.tbStartupKeyLocation.TabIndex = 9;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 111);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(140, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Startup Key Location (USB):";
            // 
            // tbConfirmPin
            // 
            this.tbConfirmPin.Location = new System.Drawing.Point(125, 83);
            this.tbConfirmPin.MaxLength = 20;
            this.tbConfirmPin.Name = "tbConfirmPin";
            this.tbConfirmPin.PasswordChar = '●';
            this.tbConfirmPin.Size = new System.Drawing.Size(219, 20);
            this.tbConfirmPin.TabIndex = 7;
            this.tbConfirmPin.TextChanged += new System.EventHandler(this.tbConfirmPin_TextChanged);
            this.tbConfirmPin.Leave += new System.EventHandler(this.tbConfirmPin_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 86);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Confirm PIN";
            // 
            // tbPin
            // 
            this.tbPin.Location = new System.Drawing.Point(125, 60);
            this.tbPin.MaxLength = 20;
            this.tbPin.Name = "tbPin";
            this.tbPin.PasswordChar = '●';
            this.tbPin.Size = new System.Drawing.Size(219, 20);
            this.tbPin.TabIndex = 5;
            this.tbPin.TextChanged += new System.EventHandler(this.tbPin_TextChanged);
            this.tbPin.Leave += new System.EventHandler(this.tbPin_Leave);
            // 
            // labelPin
            // 
            this.labelPin.AutoSize = true;
            this.labelPin.Location = new System.Drawing.Point(6, 63);
            this.labelPin.Name = "labelPin";
            this.labelPin.Size = new System.Drawing.Size(53, 13);
            this.labelPin.TabIndex = 4;
            this.labelPin.Text = "Enter PIN";
            // 
            // tpmStartupOption
            // 
            this.tpmStartupOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tpmStartupOption.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.tpmStartupOption.FormattingEnabled = true;
            this.tpmStartupOption.Location = new System.Drawing.Point(125, 28);
            this.tpmStartupOption.Name = "tpmStartupOption";
            this.tpmStartupOption.Size = new System.Drawing.Size(219, 21);
            this.tpmStartupOption.TabIndex = 3;
            this.tpmStartupOption.SelectedIndexChanged += new System.EventHandler(this.tpmStartupOption_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "TPM Startup Option";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.BitLocker48;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelAD);
            this.groupBox3.Location = new System.Drawing.Point(378, 284);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(350, 95);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Active Directory";
            // 
            // labelAD
            // 
            this.labelAD.Location = new System.Drawing.Point(6, 25);
            this.labelAD.Name = "labelAD";
            this.labelAD.Size = new System.Drawing.Size(338, 53);
            this.labelAD.TabIndex = 0;
            this.labelAD.Text = "label7";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbSkipCheck);
            this.groupBox4.Controls.Add(this.cbReboot);
            this.groupBox4.Controls.Add(this.cbPerformCheck);
            this.groupBox4.Location = new System.Drawing.Point(12, 284);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(350, 145);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Hardware Test";
            // 
            // cbSkipCheck
            // 
            this.cbSkipCheck.Enabled = false;
            this.cbSkipCheck.Location = new System.Drawing.Point(9, 70);
            this.cbSkipCheck.Name = "cbSkipCheck";
            this.cbSkipCheck.Size = new System.Drawing.Size(333, 59);
            this.cbSkipCheck.TabIndex = 2;
            this.cbSkipCheck.Text = resources.GetString("cbSkipCheck.Text");
            this.cbSkipCheck.UseVisualStyleBackColor = true;
            this.cbSkipCheck.CheckedChanged += new System.EventHandler(this.cbSkipCheck_CheckedChanged);
            // 
            // cbReboot
            // 
            this.cbReboot.AutoSize = true;
            this.cbReboot.Location = new System.Drawing.Point(9, 47);
            this.cbReboot.Name = "cbReboot";
            this.cbReboot.Size = new System.Drawing.Size(242, 17);
            this.cbReboot.TabIndex = 1;
            this.cbReboot.Text = "Reboot immediately when this dialogue closes";
            this.cbReboot.UseVisualStyleBackColor = true;
            // 
            // cbPerformCheck
            // 
            this.cbPerformCheck.AutoSize = true;
            this.cbPerformCheck.Checked = true;
            this.cbPerformCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPerformCheck.Location = new System.Drawing.Point(9, 24);
            this.cbPerformCheck.Name = "cbPerformCheck";
            this.cbPerformCheck.Size = new System.Drawing.Size(333, 17);
            this.cbPerformCheck.TabIndex = 0;
            this.cbPerformCheck.Text = "Perform the BitLocker System Check before beginning encryption";
            this.cbPerformCheck.UseVisualStyleBackColor = true;
            this.cbPerformCheck.CheckedChanged += new System.EventHandler(this.cbPerformCheck_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.ShieldRed24;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(640, 393);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 36);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnEncrypt
            // 
            this.btnEncrypt.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnEncrypt.Enabled = false;
            this.btnEncrypt.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.ShieldGreen24;
            this.btnEncrypt.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEncrypt.Location = new System.Drawing.Point(540, 393);
            this.btnEncrypt.Name = "btnEncrypt";
            this.btnEncrypt.Size = new System.Drawing.Size(85, 36);
            this.btnEncrypt.TabIndex = 13;
            this.btnEncrypt.Text = "Encrypt";
            this.btnEncrypt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEncrypt.UseVisualStyleBackColor = true;
            this.btnEncrypt.Click += new System.EventHandler(this.btnEncrypt_Click);
            // 
            // EncryptSystemVolumeDialogue
            // 
            this.AcceptButton = this.btnEncrypt;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(740, 438);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnEncrypt);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EncryptSystemVolumeDialogue";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Encrypt the System Volume";
            this.Load += new System.EventHandler(this.EncryptSystemVolumeDialogue_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox encryptionMethod;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbRecoveryKey;
        private System.Windows.Forms.CheckBox cbRecoveryPw;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox tpmStartupOption;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbConfirmPin;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbPin;
        private System.Windows.Forms.Label labelPin;
        private System.Windows.Forms.CheckBox cbNoTpm;
        private System.Windows.Forms.Button btnBrowseStartupKey;
        private System.Windows.Forms.TextBox tbStartupKeyLocation;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label labelAD;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cbSkipCheck;
        private System.Windows.Forms.CheckBox cbReboot;
        private System.Windows.Forms.CheckBox cbPerformCheck;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnEncrypt;
        private System.Windows.Forms.Button btnBrowseRecoveryKey;
        private System.Windows.Forms.TextBox tbRecoveryKeyLocation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnBrowseRecoveryPassword;
        private System.Windows.Forms.TextBox tbRecoveryPwLocation;
        private System.Windows.Forms.Label label11;
    }
}