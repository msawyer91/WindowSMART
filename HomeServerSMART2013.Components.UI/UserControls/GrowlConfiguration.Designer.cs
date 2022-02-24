namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    partial class GrowlConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GrowlConfiguration));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label16 = new System.Windows.Forms.Label();
            this.line11 = new WSSControls.BelovedComponents.Line();
            this.label1 = new System.Windows.Forms.Label();
            this.cbEnableNma = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbGrowlTarget = new System.Windows.Forms.TextBox();
            this.buttonValidate = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.rbDisplayLocal = new System.Windows.Forms.RadioButton();
            this.rbDeliverRemote = new System.Windows.Forms.RadioButton();
            this.tbGrowlPassword = new System.Windows.Forms.TextBox();
            this.cbCustomPort = new System.Windows.Forms.CheckBox();
            this.customPort = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customPort)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.Growl64;
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
            this.label16.Size = new System.Drawing.Size(181, 13);
            this.label16.TabIndex = 12;
            this.label16.Text = "Desktop Notification Via Growl";
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
            // cbEnableNma
            // 
            this.cbEnableNma.AutoSize = true;
            this.cbEnableNma.Location = new System.Drawing.Point(12, 97);
            this.cbEnableNma.Name = "cbEnableNma";
            this.cbEnableNma.Size = new System.Drawing.Size(150, 17);
            this.cbEnableNma.TabIndex = 15;
            this.cbEnableNma.Text = "Enable Growl Notifications";
            this.cbEnableNma.UseVisualStyleBackColor = true;
            this.cbEnableNma.CheckedChanged += new System.EventHandler(this.cbEnableProwl_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 206);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(263, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "If Growl is set up with a password, please enter it here:";
            // 
            // tbGrowlTarget
            // 
            this.tbGrowlTarget.Enabled = false;
            this.tbGrowlTarget.Location = new System.Drawing.Point(291, 142);
            this.tbGrowlTarget.Name = "tbGrowlTarget";
            this.tbGrowlTarget.Size = new System.Drawing.Size(222, 20);
            this.tbGrowlTarget.TabIndex = 18;
            // 
            // buttonValidate
            // 
            this.buttonValidate.Enabled = false;
            this.buttonValidate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonValidate.ForeColor = System.Drawing.Color.Red;
            this.buttonValidate.Location = new System.Drawing.Point(25, 246);
            this.buttonValidate.Name = "buttonValidate";
            this.buttonValidate.Size = new System.Drawing.Size(161, 36);
            this.buttonValidate.TabIndex = 22;
            this.buttonValidate.Text = "Register with Growl";
            this.buttonValidate.UseVisualStyleBackColor = true;
            this.buttonValidate.Click += new System.EventHandler(this.buttonValidate_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(179, 312);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 26);
            this.buttonOK.TabIndex = 23;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(278, 312);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 26);
            this.buttonCancel.TabIndex = 24;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // rbDisplayLocal
            // 
            this.rbDisplayLocal.AutoSize = true;
            this.rbDisplayLocal.Checked = true;
            this.rbDisplayLocal.Enabled = false;
            this.rbDisplayLocal.Location = new System.Drawing.Point(25, 120);
            this.rbDisplayLocal.Name = "rbDisplayLocal";
            this.rbDisplayLocal.Size = new System.Drawing.Size(229, 17);
            this.rbDisplayLocal.TabIndex = 16;
            this.rbDisplayLocal.TabStop = true;
            this.rbDisplayLocal.Text = "Display Growl notifications on this computer";
            this.rbDisplayLocal.UseVisualStyleBackColor = true;
            // 
            // rbDeliverRemote
            // 
            this.rbDeliverRemote.AutoSize = true;
            this.rbDeliverRemote.Enabled = false;
            this.rbDeliverRemote.Location = new System.Drawing.Point(25, 143);
            this.rbDeliverRemote.Name = "rbDeliverRemote";
            this.rbDeliverRemote.Size = new System.Drawing.Size(262, 17);
            this.rbDeliverRemote.TabIndex = 17;
            this.rbDeliverRemote.TabStop = true;
            this.rbDeliverRemote.Text = "Deliver Growl notifications to the named computer:";
            this.rbDeliverRemote.UseVisualStyleBackColor = true;
            this.rbDeliverRemote.CheckedChanged += new System.EventHandler(this.rbDeliverRemote_CheckedChanged);
            // 
            // tbGrowlPassword
            // 
            this.tbGrowlPassword.Enabled = false;
            this.tbGrowlPassword.Location = new System.Drawing.Point(291, 203);
            this.tbGrowlPassword.Name = "tbGrowlPassword";
            this.tbGrowlPassword.PasswordChar = '●';
            this.tbGrowlPassword.Size = new System.Drawing.Size(222, 20);
            this.tbGrowlPassword.TabIndex = 21;
            // 
            // cbCustomPort
            // 
            this.cbCustomPort.AutoSize = true;
            this.cbCustomPort.Enabled = false;
            this.cbCustomPort.Location = new System.Drawing.Point(45, 166);
            this.cbCustomPort.Name = "cbCustomPort";
            this.cbCustomPort.Size = new System.Drawing.Size(103, 17);
            this.cbCustomPort.TabIndex = 19;
            this.cbCustomPort.Text = "Use custom port";
            this.cbCustomPort.UseVisualStyleBackColor = true;
            this.cbCustomPort.CheckedChanged += new System.EventHandler(this.cbCustomPort_CheckedChanged);
            // 
            // customPort
            // 
            this.customPort.Enabled = false;
            this.customPort.Location = new System.Drawing.Point(146, 165);
            this.customPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.customPort.Name = "customPort";
            this.customPort.Size = new System.Drawing.Size(56, 20);
            this.customPort.TabIndex = 20;
            this.customPort.Value = new decimal(new int[] {
            23053,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(202, 240);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(319, 59);
            this.label3.TabIndex = 27;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // GrowlConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(533, 351);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.customPort);
            this.Controls.Add(this.cbCustomPort);
            this.Controls.Add(this.tbGrowlPassword);
            this.Controls.Add(this.rbDeliverRemote);
            this.Controls.Add(this.rbDisplayLocal);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonValidate);
            this.Controls.Add(this.tbGrowlTarget);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbEnableNma);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.line11);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GrowlConfiguration";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Growl Configuration";
            this.Load += new System.EventHandler(this.ProwlConfiguration_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label16;
        private WSSControls.BelovedComponents.Line line11;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbEnableNma;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbGrowlTarget;
        private System.Windows.Forms.Button buttonValidate;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.RadioButton rbDisplayLocal;
        private System.Windows.Forms.RadioButton rbDeliverRemote;
        private System.Windows.Forms.TextBox tbGrowlPassword;
        private System.Windows.Forms.CheckBox cbCustomPort;
        private System.Windows.Forms.NumericUpDown customPort;
        private System.Windows.Forms.Label label3;
    }
}