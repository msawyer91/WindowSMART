namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    partial class NmaConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NmaConfiguration));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label16 = new System.Windows.Forms.Label();
            this.line11 = new WSSControls.BelovedComponents.Line();
            this.label1 = new System.Windows.Forms.Label();
            this.cbEnableNma = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbNmaApiKey = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonValidate = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageNma = new System.Windows.Forms.TabPage();
            this.tabPagePushover = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.tbPushoverDevice = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.comboBoxCleared = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxCritical = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxWarning = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonValidatePushover = new System.Windows.Forms.Button();
            this.cbEnablePushover = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbPushoverKey = new System.Windows.Forms.TextBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.line2 = new WSSControls.BelovedComponents.Line();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageNma.SuspendLayout();
            this.tabPagePushover.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.NotifyMyAndroid64;
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
            this.label16.Size = new System.Drawing.Size(241, 13);
            this.label16.TabIndex = 12;
            this.label16.Text = "Instant Notification Via Notify My Android";
            // 
            // line11
            // 
            this.line11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line11.Color = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line11.Location = new System.Drawing.Point(330, 27);
            this.line11.Name = "line11";
            this.line11.Size = new System.Drawing.Size(185, 1);
            this.line11.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(87, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(431, 41);
            this.label1.TabIndex = 14;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // cbEnableNma
            // 
            this.cbEnableNma.AutoSize = true;
            this.cbEnableNma.Location = new System.Drawing.Point(17, 103);
            this.cbEnableNma.Name = "cbEnableNma";
            this.cbEnableNma.Size = new System.Drawing.Size(145, 17);
            this.cbEnableNma.TabIndex = 15;
            this.cbEnableNma.Text = "Enable Notify My Android";
            this.cbEnableNma.UseVisualStyleBackColor = true;
            this.cbEnableNma.CheckedChanged += new System.EventHandler(this.cbEnableProwl_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "NMA API Key";
            // 
            // tbNmaApiKey
            // 
            this.tbNmaApiKey.Enabled = false;
            this.tbNmaApiKey.Location = new System.Drawing.Point(114, 132);
            this.tbNmaApiKey.Name = "tbNmaApiKey";
            this.tbNmaApiKey.Size = new System.Drawing.Size(332, 20);
            this.tbNmaApiKey.TabIndex = 17;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(15, 176);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(501, 85);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "What\'s a NMA API Key?";
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
            this.label4.Location = new System.Drawing.Point(125, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(253, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "You may define up to 5 keys, separated by commas.";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageNma);
            this.tabControl1.Controls.Add(this.tabPagePushover);
            this.tabControl1.Location = new System.Drawing.Point(12, 62);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(543, 305);
            this.tabControl1.TabIndex = 24;
            // 
            // tabPageNma
            // 
            this.tabPageNma.Controls.Add(this.pictureBox1);
            this.tabPageNma.Controls.Add(this.label4);
            this.tabPageNma.Controls.Add(this.line11);
            this.tabPageNma.Controls.Add(this.label16);
            this.tabPageNma.Controls.Add(this.label1);
            this.tabPageNma.Controls.Add(this.buttonValidate);
            this.tabPageNma.Controls.Add(this.cbEnableNma);
            this.tabPageNma.Controls.Add(this.groupBox1);
            this.tabPageNma.Controls.Add(this.label2);
            this.tabPageNma.Controls.Add(this.tbNmaApiKey);
            this.tabPageNma.Location = new System.Drawing.Point(4, 22);
            this.tabPageNma.Name = "tabPageNma";
            this.tabPageNma.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNma.Size = new System.Drawing.Size(535, 279);
            this.tabPageNma.TabIndex = 0;
            this.tabPageNma.Text = "Notify My Android";
            this.tabPageNma.UseVisualStyleBackColor = true;
            // 
            // tabPagePushover
            // 
            this.tabPagePushover.Controls.Add(this.label14);
            this.tabPagePushover.Controls.Add(this.tbPushoverDevice);
            this.tabPagePushover.Controls.Add(this.label13);
            this.tabPagePushover.Controls.Add(this.comboBoxCleared);
            this.tabPagePushover.Controls.Add(this.label9);
            this.tabPagePushover.Controls.Add(this.comboBoxCritical);
            this.tabPagePushover.Controls.Add(this.label8);
            this.tabPagePushover.Controls.Add(this.comboBoxWarning);
            this.tabPagePushover.Controls.Add(this.label6);
            this.tabPagePushover.Controls.Add(this.label5);
            this.tabPagePushover.Controls.Add(this.buttonValidatePushover);
            this.tabPagePushover.Controls.Add(this.cbEnablePushover);
            this.tabPagePushover.Controls.Add(this.label7);
            this.tabPagePushover.Controls.Add(this.tbPushoverKey);
            this.tabPagePushover.Controls.Add(this.pictureBox3);
            this.tabPagePushover.Controls.Add(this.line2);
            this.tabPagePushover.Controls.Add(this.label11);
            this.tabPagePushover.Controls.Add(this.label12);
            this.tabPagePushover.Location = new System.Drawing.Point(4, 22);
            this.tabPagePushover.Name = "tabPagePushover";
            this.tabPagePushover.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePushover.Size = new System.Drawing.Size(535, 279);
            this.tabPagePushover.TabIndex = 1;
            this.tabPagePushover.Text = "Pushover";
            this.tabPagePushover.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(291, 241);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(238, 18);
            this.label14.TabIndex = 38;
            this.label14.Text = "Leave blank unless you know what you\'re doing!";
            // 
            // tbPushoverDevice
            // 
            this.tbPushoverDevice.Location = new System.Drawing.Point(294, 213);
            this.tbPushoverDevice.Name = "tbPushoverDevice";
            this.tbPushoverDevice.Size = new System.Drawing.Size(201, 20);
            this.tbPushoverDevice.TabIndex = 37;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(291, 191);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(204, 13);
            this.label13.TabIndex = 36;
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
            this.comboBoxCleared.TabIndex = 35;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(24, 241);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 13);
            this.label9.TabIndex = 34;
            this.label9.Text = "Cleared Sound";
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
            this.comboBoxCritical.TabIndex = 33;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(24, 216);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 13);
            this.label8.TabIndex = 32;
            this.label8.Text = "Critical Sound";
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
            this.comboBoxWarning.TabIndex = 31;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 191);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "Warning Sound";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(125, 155);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(247, 13);
            this.label5.TabIndex = 29;
            this.label5.Text = "Visit https://pushover.net to get your free user key.";
            // 
            // buttonValidatePushover
            // 
            this.buttonValidatePushover.Enabled = false;
            this.buttonValidatePushover.Location = new System.Drawing.Point(452, 129);
            this.buttonValidatePushover.Name = "buttonValidatePushover";
            this.buttonValidatePushover.Size = new System.Drawing.Size(66, 23);
            this.buttonValidatePushover.TabIndex = 28;
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
            this.cbEnablePushover.TabIndex = 24;
            this.cbEnablePushover.Text = "Enable Pushover";
            this.cbEnablePushover.UseVisualStyleBackColor = true;
            this.cbEnablePushover.CheckedChanged += new System.EventHandler(this.cbEnablePushover_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(24, 134);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "User Key";
            // 
            // tbPushoverKey
            // 
            this.tbPushoverKey.Enabled = false;
            this.tbPushoverKey.Location = new System.Drawing.Point(114, 132);
            this.tbPushoverKey.Name = "tbPushoverKey";
            this.tbPushoverKey.Size = new System.Drawing.Size(332, 20);
            this.tbPushoverKey.TabIndex = 26;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.Pushover64;
            this.pictureBox3.Location = new System.Drawing.Point(17, 18);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(64, 64);
            this.pictureBox3.TabIndex = 19;
            this.pictureBox3.TabStop = false;
            // 
            // line2
            // 
            this.line2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line2.Location = new System.Drawing.Point(283, 27);
            this.line2.Name = "line2";
            this.line2.Size = new System.Drawing.Size(235, 1);
            this.line2.TabIndex = 21;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(87, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(194, 13);
            this.label11.TabIndex = 20;
            this.label11.Text = "Instant Notification Via Pushover";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(87, 41);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(431, 41);
            this.label12.TabIndex = 22;
            this.label12.Text = resources.GetString("label12.Text");
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(12, 15);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(543, 40);
            this.label10.TabIndex = 25;
            this.label10.Text = resources.GetString("label10.Text");
            // 
            // NmaConfiguration
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
            this.Icon = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.NotifyMyAndroid;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NmaConfiguration";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Notify My Android Configuration";
            this.Load += new System.EventHandler(this.ProwlConfiguration_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageNma.ResumeLayout(false);
            this.tabPageNma.PerformLayout();
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
        private System.Windows.Forms.CheckBox cbEnableNma;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbNmaApiKey;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonValidate;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageNma;
        private System.Windows.Forms.TabPage tabPagePushover;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.PictureBox pictureBox3;
        private WSSControls.BelovedComponents.Line line2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonValidatePushover;
        private System.Windows.Forms.CheckBox cbEnablePushover;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbPushoverKey;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox tbPushoverDevice;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox comboBoxCleared;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBoxCritical;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxWarning;
        private System.Windows.Forms.Label label6;
    }
}