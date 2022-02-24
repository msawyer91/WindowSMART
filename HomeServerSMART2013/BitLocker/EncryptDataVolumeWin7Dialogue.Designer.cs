namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    partial class EncryptDataVolumeWin7Dialogue
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EncryptDataVolumeWin7Dialogue));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
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
            this.btnSelectCert = new System.Windows.Forms.Button();
            this.cbAutoUnlock = new System.Windows.Forms.CheckBox();
            this.tbCertThumbprint = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbSmartCard = new System.Windows.Forms.CheckBox();
            this.tbConfirmPassword = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.cbPassword = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxSaveExtra = new System.Windows.Forms.CheckBox();
            this.cbNoBitLockerToGo = new System.Windows.Forms.CheckBox();
            this.tbBitLockerId = new System.Windows.Forms.TextBox();
            this.cbBitLockerId = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.labelAD = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.labelFileSystem = new System.Windows.Forms.Label();
            this.labelVolumeLabel = new System.Windows.Forms.Label();
            this.labelVolume = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEncrypt = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(68, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(660, 44);
            this.label2.TabIndex = 12;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(66, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(400, 29);
            this.label1.TabIndex = 11;
            this.label1.Text = "Encrypt a Windows Server Data Volume";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.BitLocker48;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
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
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Encryption Method and Recovery";
            // 
            // btnBrowseRecoveryKey
            // 
            this.btnBrowseRecoveryKey.Enabled = false;
            this.btnBrowseRecoveryKey.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Save;
            this.btnBrowseRecoveryKey.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBrowseRecoveryKey.Location = new System.Drawing.Point(266, 160);
            this.btnBrowseRecoveryKey.Name = "btnBrowseRecoveryKey";
            this.btnBrowseRecoveryKey.Size = new System.Drawing.Size(78, 27);
            this.btnBrowseRecoveryKey.TabIndex = 9;
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
            this.tbRecoveryKeyLocation.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 148);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(201, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Recovery Key Location (MUST be USB):";
            // 
            // btnBrowseRecoveryPassword
            // 
            this.btnBrowseRecoveryPassword.Enabled = false;
            this.btnBrowseRecoveryPassword.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Save;
            this.btnBrowseRecoveryPassword.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBrowseRecoveryPassword.Location = new System.Drawing.Point(266, 123);
            this.btnBrowseRecoveryPassword.Name = "btnBrowseRecoveryPassword";
            this.btnBrowseRecoveryPassword.Size = new System.Drawing.Size(78, 27);
            this.btnBrowseRecoveryPassword.TabIndex = 6;
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
            this.tbRecoveryPwLocation.TabIndex = 5;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 111);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(149, 13);
            this.label11.TabIndex = 11;
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
            this.label4.Location = new System.Drawing.Point(6, 63);
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
            this.groupBox2.Controls.Add(this.btnSelectCert);
            this.groupBox2.Controls.Add(this.cbAutoUnlock);
            this.groupBox2.Controls.Add(this.tbCertThumbprint);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.cbSmartCard);
            this.groupBox2.Controls.Add(this.tbConfirmPassword);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.tbPassword);
            this.groupBox2.Controls.Add(this.cbPassword);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(378, 85);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(350, 193);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Drive Unlocking";
            // 
            // btnSelectCert
            // 
            this.btnSelectCert.Location = new System.Drawing.Point(32, 125);
            this.btnSelectCert.Name = "btnSelectCert";
            this.btnSelectCert.Size = new System.Drawing.Size(61, 23);
            this.btnSelectCert.TabIndex = 14;
            this.btnSelectCert.Text = "Select...";
            this.btnSelectCert.UseVisualStyleBackColor = true;
            this.btnSelectCert.Click += new System.EventHandler(this.btnSelectCert_Click);
            // 
            // cbAutoUnlock
            // 
            this.cbAutoUnlock.AutoSize = true;
            this.cbAutoUnlock.Checked = true;
            this.cbAutoUnlock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoUnlock.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbAutoUnlock.Location = new System.Drawing.Point(9, 159);
            this.cbAutoUnlock.Name = "cbAutoUnlock";
            this.cbAutoUnlock.Size = new System.Drawing.Size(318, 18);
            this.cbAutoUnlock.TabIndex = 16;
            this.cbAutoUnlock.Text = "Automatic Unlocking (volume auto-unlocks on this computer)";
            this.cbAutoUnlock.UseVisualStyleBackColor = true;
            this.cbAutoUnlock.CheckedChanged += new System.EventHandler(this.cbAutoUnlock_CheckedChanged);
            // 
            // tbCertThumbprint
            // 
            this.tbCertThumbprint.Location = new System.Drawing.Point(106, 127);
            this.tbCertThumbprint.Name = "tbCertThumbprint";
            this.tbCertThumbprint.ReadOnly = true;
            this.tbCertThumbprint.Size = new System.Drawing.Size(238, 20);
            this.tbCertThumbprint.TabIndex = 15;
            this.tbCertThumbprint.TextChanged += new System.EventHandler(this.tbCertThumbprint_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(103, 112);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Certificate Thumbprint";
            // 
            // cbSmartCard
            // 
            this.cbSmartCard.AutoSize = true;
            this.cbSmartCard.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbSmartCard.Location = new System.Drawing.Point(9, 110);
            this.cbSmartCard.Name = "cbSmartCard";
            this.cbSmartCard.Size = new System.Drawing.Size(84, 18);
            this.cbSmartCard.TabIndex = 13;
            this.cbSmartCard.Text = "Smart Card";
            this.cbSmartCard.UseVisualStyleBackColor = true;
            this.cbSmartCard.CheckedChanged += new System.EventHandler(this.cbSmartCard_CheckedChanged);
            // 
            // tbConfirmPassword
            // 
            this.tbConfirmPassword.Location = new System.Drawing.Point(106, 83);
            this.tbConfirmPassword.MaxLength = 99;
            this.tbConfirmPassword.Name = "tbConfirmPassword";
            this.tbConfirmPassword.PasswordChar = '●';
            this.tbConfirmPassword.Size = new System.Drawing.Size(238, 20);
            this.tbConfirmPassword.TabIndex = 12;
            this.tbConfirmPassword.TextChanged += new System.EventHandler(this.tbConfirmPassword_TextChanged);
            this.tbConfirmPassword.Leave += new System.EventHandler(this.tbConfirmPassword_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 86);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(91, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Confirm Password";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(106, 61);
            this.tbPassword.MaxLength = 99;
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '●';
            this.tbPassword.Size = new System.Drawing.Size(238, 20);
            this.tbPassword.TabIndex = 11;
            this.tbPassword.TextChanged += new System.EventHandler(this.tbPassword_TextChanged);
            this.tbPassword.Leave += new System.EventHandler(this.tbPassword_Leave);
            // 
            // cbPassword
            // 
            this.cbPassword.AutoSize = true;
            this.cbPassword.Checked = true;
            this.cbPassword.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPassword.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbPassword.Location = new System.Drawing.Point(9, 62);
            this.cbPassword.Name = "cbPassword";
            this.cbPassword.Size = new System.Drawing.Size(78, 18);
            this.cbPassword.TabIndex = 10;
            this.cbPassword.Text = "Password";
            this.cbPassword.UseVisualStyleBackColor = true;
            this.cbPassword.CheckedChanged += new System.EventHandler(this.cbPassword_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(218, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Choose how you want to unlock this volume.";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBoxSaveExtra);
            this.groupBox3.Controls.Add(this.cbNoBitLockerToGo);
            this.groupBox3.Controls.Add(this.tbBitLockerId);
            this.groupBox3.Controls.Add(this.cbBitLockerId);
            this.groupBox3.Location = new System.Drawing.Point(12, 284);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(350, 148);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Advanced Options";
            // 
            // checkBoxSaveExtra
            // 
            this.checkBoxSaveExtra.Checked = true;
            this.checkBoxSaveExtra.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSaveExtra.Location = new System.Drawing.Point(9, 100);
            this.checkBoxSaveExtra.Name = "checkBoxSaveExtra";
            this.checkBoxSaveExtra.Size = new System.Drawing.Size(327, 33);
            this.checkBoxSaveExtra.TabIndex = 20;
            this.checkBoxSaveExtra.Text = "After encryption starts, allow me to save or print extra copies of recovery infor" +
    "mation (recommended)";
            this.checkBoxSaveExtra.UseVisualStyleBackColor = true;
            // 
            // cbNoBitLockerToGo
            // 
            this.cbNoBitLockerToGo.AutoSize = true;
            this.cbNoBitLockerToGo.Location = new System.Drawing.Point(9, 77);
            this.cbNoBitLockerToGo.Name = "cbNoBitLockerToGo";
            this.cbNoBitLockerToGo.Size = new System.Drawing.Size(224, 17);
            this.cbNoBitLockerToGo.TabIndex = 19;
            this.cbNoBitLockerToGo.Text = "Do not install the BitLocker To Go Reader";
            this.cbNoBitLockerToGo.UseVisualStyleBackColor = true;
            // 
            // tbBitLockerId
            // 
            this.tbBitLockerId.Location = new System.Drawing.Point(28, 51);
            this.tbBitLockerId.MaxLength = 260;
            this.tbBitLockerId.Name = "tbBitLockerId";
            this.tbBitLockerId.ReadOnly = true;
            this.tbBitLockerId.Size = new System.Drawing.Size(316, 20);
            this.tbBitLockerId.TabIndex = 18;
            // 
            // cbBitLockerId
            // 
            this.cbBitLockerId.AutoSize = true;
            this.cbBitLockerId.Location = new System.Drawing.Point(9, 28);
            this.cbBitLockerId.Name = "cbBitLockerId";
            this.cbBitLockerId.Size = new System.Drawing.Size(206, 17);
            this.cbBitLockerId.TabIndex = 17;
            this.cbBitLockerId.Text = "Set the BitLocker Organization ID field";
            this.cbBitLockerId.UseVisualStyleBackColor = true;
            this.cbBitLockerId.CheckedChanged += new System.EventHandler(this.cbBitLockerId_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.labelAD);
            this.groupBox4.Location = new System.Drawing.Point(378, 284);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(350, 99);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Active Directory";
            // 
            // labelAD
            // 
            this.labelAD.Location = new System.Drawing.Point(6, 25);
            this.labelAD.Name = "labelAD";
            this.labelAD.Size = new System.Drawing.Size(338, 53);
            this.labelAD.TabIndex = 0;
            this.labelAD.Text = "label7";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Location = new System.Drawing.Point(12, 438);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(350, 87);
            this.groupBox5.TabIndex = 17;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Compatibility";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(9, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(335, 54);
            this.label9.TabIndex = 0;
            this.label9.Text = resources.GetString("label9.Text");
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.labelFileSystem);
            this.groupBox6.Controls.Add(this.labelVolumeLabel);
            this.groupBox6.Controls.Add(this.labelVolume);
            this.groupBox6.Location = new System.Drawing.Point(378, 384);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(350, 92);
            this.groupBox6.TabIndex = 18;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Volume Details";
            // 
            // labelFileSystem
            // 
            this.labelFileSystem.AutoSize = true;
            this.labelFileSystem.Location = new System.Drawing.Point(6, 66);
            this.labelFileSystem.Name = "labelFileSystem";
            this.labelFileSystem.Size = new System.Drawing.Size(79, 13);
            this.labelFileSystem.TabIndex = 2;
            this.labelFileSystem.Text = "labelFileSystem";
            // 
            // labelVolumeLabel
            // 
            this.labelVolumeLabel.AutoSize = true;
            this.labelVolumeLabel.Location = new System.Drawing.Point(6, 46);
            this.labelVolumeLabel.Name = "labelVolumeLabel";
            this.labelVolumeLabel.Size = new System.Drawing.Size(90, 13);
            this.labelVolumeLabel.TabIndex = 1;
            this.labelVolumeLabel.Text = "labelVolumeLabel";
            // 
            // labelVolume
            // 
            this.labelVolume.AutoSize = true;
            this.labelVolume.Location = new System.Drawing.Point(6, 26);
            this.labelVolume.Name = "labelVolume";
            this.labelVolume.Size = new System.Drawing.Size(64, 13);
            this.labelVolume.TabIndex = 0;
            this.labelVolume.Text = "labelVolume";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.ShieldRed24;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(643, 489);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 36);
            this.btnCancel.TabIndex = 22;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnEncrypt
            // 
            this.btnEncrypt.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnEncrypt.Enabled = false;
            this.btnEncrypt.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.ShieldGreen24;
            this.btnEncrypt.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEncrypt.Location = new System.Drawing.Point(543, 489);
            this.btnEncrypt.Name = "btnEncrypt";
            this.btnEncrypt.Size = new System.Drawing.Size(85, 36);
            this.btnEncrypt.TabIndex = 21;
            this.btnEncrypt.Text = "Encrypt";
            this.btnEncrypt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEncrypt.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(384, 484);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(135, 36);
            this.label10.TabIndex = 23;
            this.label10.Text = "Click Encrypt to begin encrypting your volume.";
            // 
            // EncryptDataVolumeWin7Dialogue
            // 
            this.AcceptButton = this.btnEncrypt;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(740, 534);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnEncrypt);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
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
            this.Name = "EncryptDataVolumeWin7Dialogue";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Encrypt a Data Volume";
            this.Load += new System.EventHandler(this.EncryptDataVolumeWin7Dialogue_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbRecoveryKey;
        private System.Windows.Forms.CheckBox cbRecoveryPw;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox encryptionMethod;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.CheckBox cbPassword;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox cbAutoUnlock;
        private System.Windows.Forms.TextBox tbCertThumbprint;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox cbSmartCard;
        private System.Windows.Forms.TextBox tbConfirmPassword;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cbBitLockerId;
        private System.Windows.Forms.TextBox tbBitLockerId;
        private System.Windows.Forms.CheckBox cbNoBitLockerToGo;
        private System.Windows.Forms.CheckBox checkBoxSaveExtra;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label labelAD;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label labelVolume;
        private System.Windows.Forms.Label labelVolumeLabel;
        private System.Windows.Forms.Label labelFileSystem;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnEncrypt;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnBrowseRecoveryKey;
        private System.Windows.Forms.TextBox tbRecoveryKeyLocation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnBrowseRecoveryPassword;
        private System.Windows.Forms.TextBox tbRecoveryPwLocation;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnSelectCert;
    }
}