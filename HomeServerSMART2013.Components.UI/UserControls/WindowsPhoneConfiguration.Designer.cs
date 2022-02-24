namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    partial class WindowsPhoneConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WindowsPhoneConfiguration));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label16 = new System.Windows.Forms.Label();
            this.line11 = new WSSControls.BelovedComponents.Line();
            this.label1 = new System.Windows.Forms.Label();
            this.cbEnableWindowsPhone = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbWp7Guids = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonValidate = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.Toasty64;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(82, 16);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(179, 13);
            this.label16.TabIndex = 12;
            this.label16.Text = "Instant Notification Via Toasty";
            // 
            // line11
            // 
            this.line11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line11.Color = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line11.Location = new System.Drawing.Point(268, 21);
            this.line11.Name = "line11";
            this.line11.Size = new System.Drawing.Size(245, 1);
            this.line11.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(82, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(431, 41);
            this.label1.TabIndex = 14;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // cbEnableWindowsPhone
            // 
            this.cbEnableWindowsPhone.AutoSize = true;
            this.cbEnableWindowsPhone.Location = new System.Drawing.Point(12, 97);
            this.cbEnableWindowsPhone.Name = "cbEnableWindowsPhone";
            this.cbEnableWindowsPhone.Size = new System.Drawing.Size(190, 17);
            this.cbEnableWindowsPhone.TabIndex = 15;
            this.cbEnableWindowsPhone.Text = "Enable Toasty for Windows Phone";
            this.cbEnableWindowsPhone.UseVisualStyleBackColor = true;
            this.cbEnableWindowsPhone.CheckedChanged += new System.EventHandler(this.cbEnableProwl_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Toasty Device ID";
            // 
            // tbWp7Guids
            // 
            this.tbWp7Guids.Enabled = false;
            this.tbWp7Guids.Location = new System.Drawing.Point(113, 126);
            this.tbWp7Guids.MaxLength = 65535;
            this.tbWp7Guids.Name = "tbWp7Guids";
            this.tbWp7Guids.Size = new System.Drawing.Size(328, 20);
            this.tbWp7Guids.TabIndex = 17;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 191);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(501, 85);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "What\'s a Toasty Device ID?";
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
            this.buttonValidate.Location = new System.Drawing.Point(447, 123);
            this.buttonValidate.Name = "buttonValidate";
            this.buttonValidate.Size = new System.Drawing.Size(66, 23);
            this.buttonValidate.TabIndex = 19;
            this.buttonValidate.Text = "Validate";
            this.buttonValidate.UseVisualStyleBackColor = true;
            this.buttonValidate.Click += new System.EventHandler(this.buttonValidate_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(179, 282);
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
            this.buttonCancel.Location = new System.Drawing.Point(278, 282);
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
            this.label4.Location = new System.Drawing.Point(116, 149);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(327, 26);
            this.label4.TabIndex = 23;
            this.label4.Text = "You may define as many keys as desired, separated by commas. Do\r\nnot include spac" +
    "es or curly braces (\"{\" and \"}\") in the list of keys.";
            // 
            // WindowsPhoneConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(533, 317);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonValidate);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tbWp7Guids);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbEnableWindowsPhone);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.line11);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WindowsPhoneConfiguration";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Toasty for Windows Phone Configuration";
            this.Load += new System.EventHandler(this.ProwlConfiguration_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label16;
        private WSSControls.BelovedComponents.Line line11;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbEnableWindowsPhone;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbWp7Guids;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonValidate;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label4;
    }
}