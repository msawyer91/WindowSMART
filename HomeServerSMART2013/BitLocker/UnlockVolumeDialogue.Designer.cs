namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    partial class UnlockVolumeDialogue
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnlockVolumeDialogue));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.rbBek = new System.Windows.Forms.RadioButton();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.rbNumPassword = new System.Windows.Forms.RadioButton();
            this.rbSmartCard = new System.Windows.Forms.RadioButton();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.rbPassword = new System.Windows.Forms.RadioButton();
            this.tbBekFile = new System.Windows.Forms.TextBox();
            this.btnBrowseBek = new System.Windows.Forms.Button();
            this.tbNumPassword = new System.Windows.Forms.TextBox();
            this.btnBrowseCert = new System.Windows.Forms.Button();
            this.tbSmartCard = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnUnlock = new System.Windows.Forms.Button();
            this.cbShowNumPassword = new System.Windows.Forms.CheckBox();
            this.cbShowPassword = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(68, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(430, 22);
            this.label2.TabIndex = 7;
            this.label2.Text = "Please select a method to unlcok the volume, and supply the appropriate credentia" +
    "ls.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(66, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 29);
            this.label1.TabIndex = 6;
            this.label1.Text = "Unlock Volume";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.SetupApi;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Unlock16;
            this.pictureBox2.Location = new System.Drawing.Point(12, 93);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.TabIndex = 8;
            this.pictureBox2.TabStop = false;
            // 
            // rbBek
            // 
            this.rbBek.AutoSize = true;
            this.rbBek.Enabled = false;
            this.rbBek.Location = new System.Drawing.Point(34, 93);
            this.rbBek.Name = "rbBek";
            this.rbBek.Size = new System.Drawing.Size(84, 17);
            this.rbBek.TabIndex = 9;
            this.rbBek.TabStop = true;
            this.rbBek.Text = "External Key";
            this.rbBek.UseVisualStyleBackColor = true;
            this.rbBek.CheckedChanged += new System.EventHandler(this.rbBek_CheckedChanged);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Keyboard16;
            this.pictureBox4.Location = new System.Drawing.Point(12, 137);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(16, 16);
            this.pictureBox4.TabIndex = 11;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Cert16;
            this.pictureBox6.Location = new System.Drawing.Point(12, 181);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(16, 16);
            this.pictureBox6.TabIndex = 13;
            this.pictureBox6.TabStop = false;
            // 
            // rbNumPassword
            // 
            this.rbNumPassword.AutoSize = true;
            this.rbNumPassword.Enabled = false;
            this.rbNumPassword.Location = new System.Drawing.Point(34, 137);
            this.rbNumPassword.Name = "rbNumPassword";
            this.rbNumPassword.Size = new System.Drawing.Size(110, 17);
            this.rbNumPassword.TabIndex = 15;
            this.rbNumPassword.TabStop = true;
            this.rbNumPassword.Text = "48-Digit Password";
            this.rbNumPassword.UseVisualStyleBackColor = true;
            this.rbNumPassword.CheckedChanged += new System.EventHandler(this.rbNumPassword_CheckedChanged);
            // 
            // rbSmartCard
            // 
            this.rbSmartCard.AutoSize = true;
            this.rbSmartCard.Enabled = false;
            this.rbSmartCard.Location = new System.Drawing.Point(34, 181);
            this.rbSmartCard.Name = "rbSmartCard";
            this.rbSmartCard.Size = new System.Drawing.Size(77, 17);
            this.rbSmartCard.TabIndex = 16;
            this.rbSmartCard.TabStop = true;
            this.rbSmartCard.Text = "Smart Card";
            this.rbSmartCard.UseVisualStyleBackColor = true;
            this.rbSmartCard.CheckedChanged += new System.EventHandler(this.rbSmartCard_CheckedChanged);
            // 
            // pictureBox8
            // 
            this.pictureBox8.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Keyboard16;
            this.pictureBox8.Location = new System.Drawing.Point(12, 225);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(16, 16);
            this.pictureBox8.TabIndex = 17;
            this.pictureBox8.TabStop = false;
            // 
            // rbPassword
            // 
            this.rbPassword.AutoSize = true;
            this.rbPassword.Enabled = false;
            this.rbPassword.Location = new System.Drawing.Point(34, 225);
            this.rbPassword.Name = "rbPassword";
            this.rbPassword.Size = new System.Drawing.Size(138, 17);
            this.rbPassword.TabIndex = 18;
            this.rbPassword.TabStop = true;
            this.rbPassword.Text = "Alphanumeric Password";
            this.rbPassword.UseVisualStyleBackColor = true;
            this.rbPassword.CheckedChanged += new System.EventHandler(this.rbPassword_CheckedChanged);
            // 
            // tbBekFile
            // 
            this.tbBekFile.Enabled = false;
            this.tbBekFile.Location = new System.Drawing.Point(200, 92);
            this.tbBekFile.Name = "tbBekFile";
            this.tbBekFile.ReadOnly = true;
            this.tbBekFile.Size = new System.Drawing.Size(280, 20);
            this.tbBekFile.TabIndex = 19;
            // 
            // btnBrowseBek
            // 
            this.btnBrowseBek.Enabled = false;
            this.btnBrowseBek.Location = new System.Drawing.Point(486, 90);
            this.btnBrowseBek.Name = "btnBrowseBek";
            this.btnBrowseBek.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseBek.TabIndex = 20;
            this.btnBrowseBek.Text = "Browse...";
            this.btnBrowseBek.UseVisualStyleBackColor = true;
            this.btnBrowseBek.Click += new System.EventHandler(this.btnBrowseBek_Click);
            // 
            // tbNumPassword
            // 
            this.tbNumPassword.Enabled = false;
            this.tbNumPassword.Location = new System.Drawing.Point(200, 136);
            this.tbNumPassword.MaxLength = 55;
            this.tbNumPassword.Name = "tbNumPassword";
            this.tbNumPassword.PasswordChar = '●';
            this.tbNumPassword.Size = new System.Drawing.Size(361, 20);
            this.tbNumPassword.TabIndex = 21;
            this.tbNumPassword.TextChanged += new System.EventHandler(this.tbNumPassword_TextChanged);
            // 
            // btnBrowseCert
            // 
            this.btnBrowseCert.Enabled = false;
            this.btnBrowseCert.Location = new System.Drawing.Point(486, 178);
            this.btnBrowseCert.Name = "btnBrowseCert";
            this.btnBrowseCert.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseCert.TabIndex = 23;
            this.btnBrowseCert.Text = "Select...";
            this.btnBrowseCert.UseVisualStyleBackColor = true;
            this.btnBrowseCert.Click += new System.EventHandler(this.btnBrowseCert_Click);
            // 
            // tbSmartCard
            // 
            this.tbSmartCard.Enabled = false;
            this.tbSmartCard.Location = new System.Drawing.Point(200, 180);
            this.tbSmartCard.Name = "tbSmartCard";
            this.tbSmartCard.ReadOnly = true;
            this.tbSmartCard.Size = new System.Drawing.Size(280, 20);
            this.tbSmartCard.TabIndex = 22;
            // 
            // tbPassword
            // 
            this.tbPassword.Enabled = false;
            this.tbPassword.Location = new System.Drawing.Point(200, 224);
            this.tbPassword.MaxLength = 55;
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '●';
            this.tbPassword.Size = new System.Drawing.Size(361, 20);
            this.tbPassword.TabIndex = 24;
            this.tbPassword.TextChanged += new System.EventHandler(this.tbPassword_TextChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.ShieldYellow24;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(302, 270);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 36);
            this.btnCancel.TabIndex = 26;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnUnlock
            // 
            this.btnUnlock.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnUnlock.Enabled = false;
            this.btnUnlock.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Keyboard32;
            this.btnUnlock.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUnlock.Location = new System.Drawing.Point(183, 270);
            this.btnUnlock.Name = "btnUnlock";
            this.btnUnlock.Size = new System.Drawing.Size(85, 36);
            this.btnUnlock.TabIndex = 25;
            this.btnUnlock.Text = "Unlock";
            this.btnUnlock.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUnlock.UseVisualStyleBackColor = true;
            // 
            // cbShowNumPassword
            // 
            this.cbShowNumPassword.AutoSize = true;
            this.cbShowNumPassword.Enabled = false;
            this.cbShowNumPassword.Location = new System.Drawing.Point(211, 159);
            this.cbShowNumPassword.Name = "cbShowNumPassword";
            this.cbShowNumPassword.Size = new System.Drawing.Size(156, 17);
            this.cbShowNumPassword.TabIndex = 27;
            this.cbShowNumPassword.Text = "Show Password Characters";
            this.cbShowNumPassword.UseVisualStyleBackColor = true;
            this.cbShowNumPassword.CheckedChanged += new System.EventHandler(this.cbShowNumPassword_CheckedChanged);
            // 
            // cbShowPassword
            // 
            this.cbShowPassword.AutoSize = true;
            this.cbShowPassword.Enabled = false;
            this.cbShowPassword.Location = new System.Drawing.Point(211, 247);
            this.cbShowPassword.Name = "cbShowPassword";
            this.cbShowPassword.Size = new System.Drawing.Size(156, 17);
            this.cbShowPassword.TabIndex = 28;
            this.cbShowPassword.Text = "Show Password Characters";
            this.cbShowPassword.UseVisualStyleBackColor = true;
            this.cbShowPassword.CheckedChanged += new System.EventHandler(this.cbShowPassword_CheckedChanged);
            // 
            // UnlockVolumeDialogue
            // 
            this.AcceptButton = this.btnUnlock;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(573, 316);
            this.Controls.Add(this.cbShowPassword);
            this.Controls.Add(this.cbShowNumPassword);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUnlock);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.btnBrowseCert);
            this.Controls.Add(this.tbSmartCard);
            this.Controls.Add(this.tbNumPassword);
            this.Controls.Add(this.btnBrowseBek);
            this.Controls.Add(this.tbBekFile);
            this.Controls.Add(this.rbPassword);
            this.Controls.Add(this.pictureBox8);
            this.Controls.Add(this.rbSmartCard);
            this.Controls.Add(this.rbNumPassword);
            this.Controls.Add(this.pictureBox6);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.rbBek);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UnlockVolumeDialogue";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Unlock Volume";
            this.Load += new System.EventHandler(this.UnlockVolumeDialogue_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.RadioButton rbBek;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.RadioButton rbNumPassword;
        private System.Windows.Forms.RadioButton rbSmartCard;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.RadioButton rbPassword;
        private System.Windows.Forms.TextBox tbBekFile;
        private System.Windows.Forms.Button btnBrowseBek;
        private System.Windows.Forms.TextBox tbNumPassword;
        private System.Windows.Forms.Button btnBrowseCert;
        private System.Windows.Forms.TextBox tbSmartCard;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnUnlock;
        private System.Windows.Forms.CheckBox cbShowNumPassword;
        private System.Windows.Forms.CheckBox cbShowPassword;
    }
}