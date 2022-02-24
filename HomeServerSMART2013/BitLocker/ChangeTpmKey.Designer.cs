namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    partial class ChangeTpmKey
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeTpmKey));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelVolumeLabel = new System.Windows.Forms.Label();
            this.labelSysVol = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnBrowseStartupKey = new System.Windows.Forms.Button();
            this.tbStartupKeyLocation = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbConfirmPin = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbPin = new System.Windows.Forms.TextBox();
            this.labelPin = new System.Windows.Forms.Label();
            this.tpmStartupOption = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEncrypt = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(68, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(572, 47);
            this.label2.TabIndex = 12;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(66, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 29);
            this.label1.TabIndex = 11;
            this.label1.Text = "Repair/Change TPM Key";
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelVolumeLabel);
            this.groupBox2.Controls.Add(this.labelSysVol);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.btnBrowseStartupKey);
            this.groupBox2.Controls.Add(this.tbStartupKeyLocation);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.tbConfirmPin);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.tbPin);
            this.groupBox2.Controls.Add(this.labelPin);
            this.groupBox2.Controls.Add(this.tpmStartupOption);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(12, 97);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(628, 146);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "New Trusted Platform Module (TPM) Settings";
            // 
            // labelVolumeLabel
            // 
            this.labelVolumeLabel.AutoSize = true;
            this.labelVolumeLabel.Location = new System.Drawing.Point(456, 120);
            this.labelVolumeLabel.Name = "labelVolumeLabel";
            this.labelVolumeLabel.Size = new System.Drawing.Size(41, 13);
            this.labelVolumeLabel.TabIndex = 19;
            this.labelVolumeLabel.Text = "label10";
            // 
            // labelSysVol
            // 
            this.labelSysVol.AutoSize = true;
            this.labelSysVol.Location = new System.Drawing.Point(456, 102);
            this.labelSysVol.Name = "labelSysVol";
            this.labelSysVol.Size = new System.Drawing.Size(35, 13);
            this.labelSysVol.TabIndex = 18;
            this.labelSysVol.Text = "label7";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(368, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Volume Label:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(368, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "System Volume:";
            // 
            // btnBrowseStartupKey
            // 
            this.btnBrowseStartupKey.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Save;
            this.btnBrowseStartupKey.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBrowseStartupKey.Location = new System.Drawing.Point(541, 69);
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
            this.tbStartupKeyLocation.Location = new System.Drawing.Point(371, 44);
            this.tbStartupKeyLocation.Name = "tbStartupKeyLocation";
            this.tbStartupKeyLocation.ReadOnly = true;
            this.tbStartupKeyLocation.Size = new System.Drawing.Size(248, 20);
            this.tbStartupKeyLocation.TabIndex = 9;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(368, 28);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(140, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Startup Key Location (USB):";
            // 
            // tbConfirmPin
            // 
            this.tbConfirmPin.Location = new System.Drawing.Point(125, 99);
            this.tbConfirmPin.MaxLength = 20;
            this.tbConfirmPin.Name = "tbConfirmPin";
            this.tbConfirmPin.PasswordChar = '●';
            this.tbConfirmPin.Size = new System.Drawing.Size(219, 20);
            this.tbConfirmPin.TabIndex = 7;
            this.tbConfirmPin.TextChanged += new System.EventHandler(this.tbConfirmPin_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 102);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Confirm PIN";
            // 
            // tbPin
            // 
            this.tbPin.Location = new System.Drawing.Point(125, 76);
            this.tbPin.MaxLength = 20;
            this.tbPin.Name = "tbPin";
            this.tbPin.PasswordChar = '●';
            this.tbPin.Size = new System.Drawing.Size(219, 20);
            this.tbPin.TabIndex = 5;
            this.tbPin.TextChanged += new System.EventHandler(this.tbPin_TextChanged);
            // 
            // labelPin
            // 
            this.labelPin.AutoSize = true;
            this.labelPin.Location = new System.Drawing.Point(6, 79);
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
            this.tpmStartupOption.Location = new System.Drawing.Point(125, 44);
            this.tpmStartupOption.Name = "tpmStartupOption";
            this.tpmStartupOption.Size = new System.Drawing.Size(219, 21);
            this.tpmStartupOption.TabIndex = 3;
            this.tpmStartupOption.SelectedIndexChanged += new System.EventHandler(this.tpmStartupOption_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "TPM Startup Option";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(18, 251);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(305, 34);
            this.label3.TabIndex = 14;
            this.label3.Text = "If a TPM key exists on the System volume, it will be replaced. If a TPM key does " +
    "not exist, a new one will be created.";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.ShieldRed24;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(555, 249);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 36);
            this.btnCancel.TabIndex = 16;
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
            this.btnEncrypt.Location = new System.Drawing.Point(455, 249);
            this.btnEncrypt.Name = "btnEncrypt";
            this.btnEncrypt.Size = new System.Drawing.Size(85, 36);
            this.btnEncrypt.TabIndex = 15;
            this.btnEncrypt.Text = "Apply";
            this.btnEncrypt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEncrypt.UseVisualStyleBackColor = true;
            // 
            // ChangeTpmKey
            // 
            this.AcceptButton = this.btnEncrypt;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(652, 297);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnEncrypt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangeTpmKey";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Repair/Change TPM Key";
            this.Load += new System.EventHandler(this.ChangeTpmKey_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnBrowseStartupKey;
        private System.Windows.Forms.TextBox tbStartupKeyLocation;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbConfirmPin;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbPin;
        private System.Windows.Forms.Label labelPin;
        private System.Windows.Forms.ComboBox tpmStartupOption;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnEncrypt;
        private System.Windows.Forms.Label labelVolumeLabel;
        private System.Windows.Forms.Label labelSysVol;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
    }
}