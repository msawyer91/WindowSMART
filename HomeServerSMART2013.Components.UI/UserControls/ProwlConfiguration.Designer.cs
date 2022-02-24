namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    partial class ProwlConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProwlConfiguration));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label16 = new System.Windows.Forms.Label();
            this.line11 = new WSSControls.BelovedComponents.Line();
            this.label1 = new System.Windows.Forms.Label();
            this.cbEnableProwl = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbProwlApiKey = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonValidate = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageProwl = new System.Windows.Forms.TabPage();
            this.tabPagePushover = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.tbPushoverDevice = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.comboBoxCleared = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.comboBoxCritical = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.comboBoxWarning = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.buttonValidatePushover = new System.Windows.Forms.Button();
            this.cbEnablePushover = new System.Windows.Forms.CheckBox();
            this.label20 = new System.Windows.Forms.Label();
            this.tbPushoverKey = new System.Windows.Forms.TextBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.line2 = new WSSControls.BelovedComponents.Line();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageProwl.SuspendLayout();
            this.tabPagePushover.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.Prowl64;
            this.pictureBox1.Location = new System.Drawing.Point(17, 18);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(87, 22);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(172, 13);
            this.label16.TabIndex = 12;
            this.label16.Text = "Instant Notification Via Prowl";
            // 
            // line11
            // 
            this.line11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line11.Color = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line11.Location = new System.Drawing.Point(263, 27);
            this.line11.Name = "line11";
            this.line11.Size = new System.Drawing.Size(255, 1);
            this.line11.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(87, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(431, 41);
            this.label1.TabIndex = 14;
            this.label1.Text = "Prowl is a paid application, available in the App Store. It doesn\'t contain as ma" +
    "ny features as Boxcar, but costs less than the paid edition of Boxcar.";
            // 
            // cbEnableProwl
            // 
            this.cbEnableProwl.AutoSize = true;
            this.cbEnableProwl.Location = new System.Drawing.Point(17, 103);
            this.cbEnableProwl.Name = "cbEnableProwl";
            this.cbEnableProwl.Size = new System.Drawing.Size(149, 17);
            this.cbEnableProwl.TabIndex = 15;
            this.cbEnableProwl.Text = "Enable Prowl Notifications";
            this.cbEnableProwl.UseVisualStyleBackColor = true;
            this.cbEnableProwl.CheckedChanged += new System.EventHandler(this.cbEnableProwl_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 135);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Prowl API Key";
            // 
            // tbProwlApiKey
            // 
            this.tbProwlApiKey.Enabled = false;
            this.tbProwlApiKey.Location = new System.Drawing.Point(106, 132);
            this.tbProwlApiKey.MaxLength = 204;
            this.tbProwlApiKey.Name = "tbProwlApiKey";
            this.tbProwlApiKey.Size = new System.Drawing.Size(340, 20);
            this.tbProwlApiKey.TabIndex = 17;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(17, 177);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(501, 85);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "What\'s a Prowl API Key?";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(18, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(466, 41);
            this.label3.TabIndex = 15;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // buttonValidate
            // 
            this.buttonValidate.Enabled = false;
            this.buttonValidate.Location = new System.Drawing.Point(452, 129);
            this.buttonValidate.Name = "buttonValidate";
            this.buttonValidate.Size = new System.Drawing.Size(66, 23);
            this.buttonValidate.TabIndex = 19;
            this.buttonValidate.Text = "Validate";
            this.buttonValidate.UseVisualStyleBackColor = true;
            this.buttonValidate.Click += new System.EventHandler(this.buttonValidate_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(193, 382);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 26);
            this.buttonOK.TabIndex = 20;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(295, 382);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 26);
            this.buttonCancel.TabIndex = 21;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(115, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(253, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "You may define up to 5 keys, separated by commas.";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageProwl);
            this.tabControl1.Controls.Add(this.tabPagePushover);
            this.tabControl1.Location = new System.Drawing.Point(12, 62);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(543, 305);
            this.tabControl1.TabIndex = 23;
            // 
            // tabPageProwl
            // 
            this.tabPageProwl.Controls.Add(this.pictureBox1);
            this.tabPageProwl.Controls.Add(this.label4);
            this.tabPageProwl.Controls.Add(this.line11);
            this.tabPageProwl.Controls.Add(this.label16);
            this.tabPageProwl.Controls.Add(this.label1);
            this.tabPageProwl.Controls.Add(this.buttonValidate);
            this.tabPageProwl.Controls.Add(this.cbEnableProwl);
            this.tabPageProwl.Controls.Add(this.groupBox1);
            this.tabPageProwl.Controls.Add(this.label2);
            this.tabPageProwl.Controls.Add(this.tbProwlApiKey);
            this.tabPageProwl.Location = new System.Drawing.Point(4, 22);
            this.tabPageProwl.Name = "tabPageProwl";
            this.tabPageProwl.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProwl.Size = new System.Drawing.Size(535, 279);
            this.tabPageProwl.TabIndex = 0;
            this.tabPageProwl.Text = "Prowl";
            this.tabPageProwl.UseVisualStyleBackColor = true;
            // 
            // tabPagePushover
            // 
            this.tabPagePushover.Controls.Add(this.label14);
            this.tabPagePushover.Controls.Add(this.tbPushoverDevice);
            this.tabPagePushover.Controls.Add(this.label13);
            this.tabPagePushover.Controls.Add(this.comboBoxCleared);
            this.tabPagePushover.Controls.Add(this.label15);
            this.tabPagePushover.Controls.Add(this.comboBoxCritical);
            this.tabPagePushover.Controls.Add(this.label17);
            this.tabPagePushover.Controls.Add(this.comboBoxWarning);
            this.tabPagePushover.Controls.Add(this.label18);
            this.tabPagePushover.Controls.Add(this.label19);
            this.tabPagePushover.Controls.Add(this.buttonValidatePushover);
            this.tabPagePushover.Controls.Add(this.cbEnablePushover);
            this.tabPagePushover.Controls.Add(this.label20);
            this.tabPagePushover.Controls.Add(this.tbPushoverKey);
            this.tabPagePushover.Controls.Add(this.pictureBox3);
            this.tabPagePushover.Controls.Add(this.line2);
            this.tabPagePushover.Controls.Add(this.label11);
            this.tabPagePushover.Controls.Add(this.label12);
            this.tabPagePushover.Location = new System.Drawing.Point(4, 22);
            this.tabPagePushover.Name = "tabPagePushover";
            this.tabPagePushover.Size = new System.Drawing.Size(535, 279);
            this.tabPagePushover.TabIndex = 2;
            this.tabPagePushover.Text = "Pushover";
            this.tabPagePushover.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(291, 241);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(238, 18);
            this.label14.TabIndex = 52;
            this.label14.Text = "Leave blank unless you know what you\'re doing!";
            // 
            // tbPushoverDevice
            // 
            this.tbPushoverDevice.Location = new System.Drawing.Point(294, 213);
            this.tbPushoverDevice.Name = "tbPushoverDevice";
            this.tbPushoverDevice.Size = new System.Drawing.Size(201, 20);
            this.tbPushoverDevice.TabIndex = 51;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(291, 191);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(204, 13);
            this.label13.TabIndex = 50;
            this.label13.Text = "Single Device Alerts - Enter Device Name";
            // 
            // comboBoxCleared
            // 
            this.comboBoxCleared.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCleared.FormattingEnabled = true;
            this.comboBoxCleared.Items.AddRange(new object[] {
            "(Device default sound)",
            "Pushover (default)",
            "Bike",
            "Bugle",
            "Cash Register",
            "Classical",
            "Cosmic",
            "Falling",
            "Gamelan",
            "Incoming",
            "Intermission",
            "Magic",
            "Mechanical",
            "Piano Bar",
            "Siren",
            "Space Alarm",
            "Tug Boat",
            "Alien Alarm (long)",
            "Climb (long)",
            "Persistent (long)",
            "Pushover Echo (long)",
            "Up Down (long)",
            "None (silent)"});
            this.comboBoxCleared.Location = new System.Drawing.Point(114, 238);
            this.comboBoxCleared.Name = "comboBoxCleared";
            this.comboBoxCleared.Size = new System.Drawing.Size(153, 21);
            this.comboBoxCleared.TabIndex = 49;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(24, 241);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(77, 13);
            this.label15.TabIndex = 48;
            this.label15.Text = "Cleared Sound";
            // 
            // comboBoxCritical
            // 
            this.comboBoxCritical.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCritical.FormattingEnabled = true;
            this.comboBoxCritical.Items.AddRange(new object[] {
            "(Device default sound)",
            "Pushover (default)",
            "Bike",
            "Bugle",
            "Cash Register",
            "Classical",
            "Cosmic",
            "Falling",
            "Gamelan",
            "Incoming",
            "Intermission",
            "Magic",
            "Mechanical",
            "Piano Bar",
            "Siren",
            "Space Alarm",
            "Tug Boat",
            "Alien Alarm (long)",
            "Climb (long)",
            "Persistent (long)",
            "Pushover Echo (long)",
            "Up Down (long)",
            "None (silent)"});
            this.comboBoxCritical.Location = new System.Drawing.Point(114, 213);
            this.comboBoxCritical.Name = "comboBoxCritical";
            this.comboBoxCritical.Size = new System.Drawing.Size(153, 21);
            this.comboBoxCritical.TabIndex = 47;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(24, 216);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(72, 13);
            this.label17.TabIndex = 46;
            this.label17.Text = "Critical Sound";
            // 
            // comboBoxWarning
            // 
            this.comboBoxWarning.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWarning.FormattingEnabled = true;
            this.comboBoxWarning.Items.AddRange(new object[] {
            "(Device default sound)",
            "Pushover (default)",
            "Bike",
            "Bugle",
            "Cash Register",
            "Classical",
            "Cosmic",
            "Falling",
            "Gamelan",
            "Incoming",
            "Intermission",
            "Magic",
            "Mechanical",
            "Piano Bar",
            "Siren",
            "Space Alarm",
            "Tug Boat",
            "Alien Alarm (long)",
            "Climb (long)",
            "Persistent (long)",
            "Pushover Echo (long)",
            "Up Down (long)",
            "None (silent)"});
            this.comboBoxWarning.Location = new System.Drawing.Point(114, 188);
            this.comboBoxWarning.Name = "comboBoxWarning";
            this.comboBoxWarning.Size = new System.Drawing.Size(153, 21);
            this.comboBoxWarning.TabIndex = 45;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(24, 191);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(81, 13);
            this.label18.TabIndex = 44;
            this.label18.Text = "Warning Sound";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(125, 155);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(247, 13);
            this.label19.TabIndex = 43;
            this.label19.Text = "Visit https://pushover.net to get your free user key.";
            // 
            // buttonValidatePushover
            // 
            this.buttonValidatePushover.Enabled = false;
            this.buttonValidatePushover.Location = new System.Drawing.Point(452, 129);
            this.buttonValidatePushover.Name = "buttonValidatePushover";
            this.buttonValidatePushover.Size = new System.Drawing.Size(66, 23);
            this.buttonValidatePushover.TabIndex = 42;
            this.buttonValidatePushover.Text = "Validate";
            this.buttonValidatePushover.UseVisualStyleBackColor = true;
            this.buttonValidatePushover.Click += new System.EventHandler(this.button1_Click);
            // 
            // cbEnablePushover
            // 
            this.cbEnablePushover.AutoSize = true;
            this.cbEnablePushover.Location = new System.Drawing.Point(17, 103);
            this.cbEnablePushover.Name = "cbEnablePushover";
            this.cbEnablePushover.Size = new System.Drawing.Size(107, 17);
            this.cbEnablePushover.TabIndex = 39;
            this.cbEnablePushover.Text = "Enable Pushover";
            this.cbEnablePushover.UseVisualStyleBackColor = true;
            this.cbEnablePushover.CheckedChanged += new System.EventHandler(this.cbEnablePushover_CheckedChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(24, 134);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(50, 13);
            this.label20.TabIndex = 40;
            this.label20.Text = "User Key";
            // 
            // tbPushoverKey
            // 
            this.tbPushoverKey.Enabled = false;
            this.tbPushoverKey.Location = new System.Drawing.Point(114, 132);
            this.tbPushoverKey.Name = "tbPushoverKey";
            this.tbPushoverKey.Size = new System.Drawing.Size(332, 20);
            this.tbPushoverKey.TabIndex = 41;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.Pushover64;
            this.pictureBox3.Location = new System.Drawing.Point(17, 18);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(64, 64);
            this.pictureBox3.TabIndex = 15;
            this.pictureBox3.TabStop = false;
            // 
            // line2
            // 
            this.line2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line2.Location = new System.Drawing.Point(283, 27);
            this.line2.Name = "line2";
            this.line2.Size = new System.Drawing.Size(235, 1);
            this.line2.TabIndex = 17;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(87, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(194, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "Instant Notification Via Pushover";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(87, 41);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(431, 41);
            this.label12.TabIndex = 18;
            this.label12.Text = resources.GetString("label12.Text");
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(12, 15);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(543, 40);
            this.label10.TabIndex = 24;
            this.label10.Text = resources.GetString("label10.Text");
            // 
            // ProwlConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(568, 427);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProwlConfiguration";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Apple iOS Configuration";
            this.Load += new System.EventHandler(this.ProwlConfiguration_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageProwl.ResumeLayout(false);
            this.tabPageProwl.PerformLayout();
            this.tabPagePushover.ResumeLayout(false);
            this.tabPagePushover.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label16;
        private WSSControls.BelovedComponents.Line line11;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbEnableProwl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbProwlApiKey;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonValidate;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageProwl;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TabPage tabPagePushover;
        private System.Windows.Forms.PictureBox pictureBox3;
        private WSSControls.BelovedComponents.Line line2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox tbPushoverDevice;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox comboBoxCleared;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox comboBoxCritical;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox comboBoxWarning;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button buttonValidatePushover;
        private System.Windows.Forms.CheckBox cbEnablePushover;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox tbPushoverKey;
    }
}