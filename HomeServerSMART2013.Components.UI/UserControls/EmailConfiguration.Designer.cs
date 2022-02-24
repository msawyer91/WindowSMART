namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    partial class EmailConfiguration
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
            this.checkBoxEnableEmail = new System.Windows.Forms.CheckBox();
            this.buttonSendTest = new System.Windows.Forms.Button();
            this.textBoxRecipientEmail = new System.Windows.Forms.TextBox();
            this.label38 = new System.Windows.Forms.Label();
            this.textBoxRecipientName = new System.Windows.Forms.TextBox();
            this.label39 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.checkBoxEnableAuthentication = new System.Windows.Forms.CheckBox();
            this.label35 = new System.Windows.Forms.Label();
            this.checkBoxSsl = new System.Windows.Forms.CheckBox();
            this.textBoxSenderEmailAddy = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.textBoxSenderFriendly = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.numericUpDownPort = new System.Windows.Forms.NumericUpDown();
            this.label32 = new System.Windows.Forms.Label();
            this.textBoxHost = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.line10 = new WSSControls.BelovedComponents.Line();
            this.line9 = new WSSControls.BelovedComponents.Line();
            this.label1 = new System.Windows.Forms.Label();
            this.line1 = new WSSControls.BelovedComponents.Line();
            this.textBoxEmailAddy2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxEmailAddy3 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.line2 = new WSSControls.BelovedComponents.Line();
            this.checkBoxSendSummary = new System.Windows.Forms.CheckBox();
            this.line3 = new WSSControls.BelovedComponents.Line();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBoxSendPlaintext = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxEnableEmail
            // 
            this.checkBoxEnableEmail.AutoSize = true;
            this.checkBoxEnableEmail.Location = new System.Drawing.Point(16, 206);
            this.checkBoxEnableEmail.Name = "checkBoxEnableEmail";
            this.checkBoxEnableEmail.Size = new System.Drawing.Size(116, 17);
            this.checkBoxEnableEmail.TabIndex = 41;
            this.checkBoxEnableEmail.Text = "Enable Email Alerts";
            this.checkBoxEnableEmail.UseVisualStyleBackColor = true;
            this.checkBoxEnableEmail.CheckedChanged += new System.EventHandler(this.checkBoxEnableEmail_CheckedChanged);
            // 
            // buttonSendTest
            // 
            this.buttonSendTest.Location = new System.Drawing.Point(382, 200);
            this.buttonSendTest.Name = "buttonSendTest";
            this.buttonSendTest.Size = new System.Drawing.Size(117, 26);
            this.buttonSendTest.TabIndex = 43;
            this.buttonSendTest.Text = "Send Test Message";
            this.buttonSendTest.UseVisualStyleBackColor = true;
            this.buttonSendTest.Click += new System.EventHandler(this.buttonSendTest_Click);
            // 
            // textBoxRecipientEmail
            // 
            this.textBoxRecipientEmail.Location = new System.Drawing.Point(361, 165);
            this.textBoxRecipientEmail.Name = "textBoxRecipientEmail";
            this.textBoxRecipientEmail.Size = new System.Drawing.Size(138, 20);
            this.textBoxRecipientEmail.TabIndex = 39;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(266, 168);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(73, 13);
            this.label38.TabIndex = 44;
            this.label38.Text = "Email Address";
            // 
            // textBoxRecipientName
            // 
            this.textBoxRecipientName.Location = new System.Drawing.Point(107, 165);
            this.textBoxRecipientName.Name = "textBoxRecipientName";
            this.textBoxRecipientName.Size = new System.Drawing.Size(138, 20);
            this.textBoxRecipientName.TabIndex = 37;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(12, 168);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(83, 13);
            this.label39.TabIndex = 42;
            this.label39.Text = "Recipient Name";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.Location = new System.Drawing.Point(12, 143);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(140, 13);
            this.label37.TabIndex = 38;
            this.label37.Text = "Recipient Configuration";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxPassword);
            this.groupBox3.Controls.Add(this.label36);
            this.groupBox3.Controls.Add(this.textBoxUsername);
            this.groupBox3.Controls.Add(this.checkBoxEnableAuthentication);
            this.groupBox3.Controls.Add(this.label35);
            this.groupBox3.Location = new System.Drawing.Point(15, 84);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(483, 47);
            this.groupBox3.TabIndex = 36;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Authentication";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(366, 17);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '●';
            this.textBoxPassword.Size = new System.Drawing.Size(99, 20);
            this.textBoxPassword.TabIndex = 14;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(305, 20);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(53, 13);
            this.label36.TabIndex = 18;
            this.label36.Text = "Password";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(170, 17);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(99, 20);
            this.textBoxUsername.TabIndex = 13;
            // 
            // checkBoxEnableAuthentication
            // 
            this.checkBoxEnableAuthentication.AutoSize = true;
            this.checkBoxEnableAuthentication.Location = new System.Drawing.Point(18, 19);
            this.checkBoxEnableAuthentication.Name = "checkBoxEnableAuthentication";
            this.checkBoxEnableAuthentication.Size = new System.Drawing.Size(59, 17);
            this.checkBoxEnableAuthentication.TabIndex = 12;
            this.checkBoxEnableAuthentication.Text = "Enable";
            this.checkBoxEnableAuthentication.UseVisualStyleBackColor = true;
            this.checkBoxEnableAuthentication.CheckedChanged += new System.EventHandler(this.checkBoxEnableAuthentication_CheckedChanged);
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(109, 20);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(55, 13);
            this.label35.TabIndex = 16;
            this.label35.Text = "Username";
            // 
            // checkBoxSsl
            // 
            this.checkBoxSsl.AutoSize = true;
            this.checkBoxSsl.Location = new System.Drawing.Point(409, 33);
            this.checkBoxSsl.Name = "checkBoxSsl";
            this.checkBoxSsl.Size = new System.Drawing.Size(68, 17);
            this.checkBoxSsl.TabIndex = 31;
            this.checkBoxSsl.Text = "Use SSL";
            this.checkBoxSsl.UseVisualStyleBackColor = true;
            // 
            // textBoxSenderEmailAddy
            // 
            this.textBoxSenderEmailAddy.Location = new System.Drawing.Point(361, 58);
            this.textBoxSenderEmailAddy.Name = "textBoxSenderEmailAddy";
            this.textBoxSenderEmailAddy.Size = new System.Drawing.Size(138, 20);
            this.textBoxSenderEmailAddy.TabIndex = 34;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(266, 61);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(73, 13);
            this.label34.TabIndex = 35;
            this.label34.Text = "Email Address";
            // 
            // textBoxSenderFriendly
            // 
            this.textBoxSenderFriendly.Location = new System.Drawing.Point(107, 58);
            this.textBoxSenderFriendly.Name = "textBoxSenderFriendly";
            this.textBoxSenderFriendly.Size = new System.Drawing.Size(138, 20);
            this.textBoxSenderFriendly.TabIndex = 32;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(12, 61);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(72, 13);
            this.label33.TabIndex = 33;
            this.label33.Text = "Sender Name";
            // 
            // numericUpDownPort
            // 
            this.numericUpDownPort.Location = new System.Drawing.Point(332, 32);
            this.numericUpDownPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownPort.Name = "numericUpDownPort";
            this.numericUpDownPort.Size = new System.Drawing.Size(57, 20);
            this.numericUpDownPort.TabIndex = 29;
            this.numericUpDownPort.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(266, 34);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(60, 13);
            this.label32.TabIndex = 30;
            this.label32.Text = "Server Port";
            // 
            // textBoxHost
            // 
            this.textBoxHost.Location = new System.Drawing.Point(107, 31);
            this.textBoxHost.Name = "textBoxHost";
            this.textBoxHost.Size = new System.Drawing.Size(138, 20);
            this.textBoxHost.TabIndex = 28;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(12, 34);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(89, 13);
            this.label31.TabIndex = 27;
            this.label31.Text = "Server Hostname";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(12, 9);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(126, 13);
            this.label18.TabIndex = 25;
            this.label18.Text = "Sender Configuration";
            // 
            // line10
            // 
            this.line10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line10.Color = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line10.Location = new System.Drawing.Point(153, 148);
            this.line10.Name = "line10";
            this.line10.Size = new System.Drawing.Size(345, 1);
            this.line10.TabIndex = 40;
            // 
            // line9
            // 
            this.line9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line9.Color = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line9.Location = new System.Drawing.Point(143, 14);
            this.line9.Name = "line9";
            this.line9.Size = new System.Drawing.Size(355, 1);
            this.line9.TabIndex = 26;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 255);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 46;
            this.label1.Text = "Additional Recipients";
            // 
            // line1
            // 
            this.line1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line1.Location = new System.Drawing.Point(153, 260);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(345, 1);
            this.line1.TabIndex = 47;
            // 
            // textBoxEmailAddy2
            // 
            this.textBoxEmailAddy2.Location = new System.Drawing.Point(108, 275);
            this.textBoxEmailAddy2.Name = "textBoxEmailAddy2";
            this.textBoxEmailAddy2.Size = new System.Drawing.Size(138, 20);
            this.textBoxEmailAddy2.TabIndex = 48;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 278);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 49;
            this.label2.Text = "Email Address 2";
            // 
            // textBoxEmailAddy3
            // 
            this.textBoxEmailAddy3.Location = new System.Drawing.Point(361, 275);
            this.textBoxEmailAddy3.Name = "textBoxEmailAddy3";
            this.textBoxEmailAddy3.Size = new System.Drawing.Size(138, 20);
            this.textBoxEmailAddy3.TabIndex = 50;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(266, 278);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 51;
            this.label3.Text = "Email Address 3";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(270, 420);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 26);
            this.buttonCancel.TabIndex = 53;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(171, 420);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 26);
            this.buttonOK.TabIndex = 52;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 310);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 13);
            this.label4.TabIndex = 54;
            this.label4.Text = "Daily Email Summary";
            // 
            // line2
            // 
            this.line2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line2.Location = new System.Drawing.Point(153, 315);
            this.line2.Name = "line2";
            this.line2.Size = new System.Drawing.Size(345, 1);
            this.line2.TabIndex = 55;
            // 
            // checkBoxSendSummary
            // 
            this.checkBoxSendSummary.AutoSize = true;
            this.checkBoxSendSummary.Location = new System.Drawing.Point(16, 333);
            this.checkBoxSendSummary.Name = "checkBoxSendSummary";
            this.checkBoxSendSummary.Size = new System.Drawing.Size(302, 17);
            this.checkBoxSendSummary.TabIndex = 56;
            this.checkBoxSendSummary.Text = "Send a daily email report, even if no problems are detected";
            this.checkBoxSendSummary.UseVisualStyleBackColor = true;
            // 
            // line3
            // 
            this.line3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line3.Color = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(213)))), ((int)(((byte)(232)))));
            this.line3.Location = new System.Drawing.Point(153, 368);
            this.line3.Name = "line3";
            this.line3.Size = new System.Drawing.Size(345, 1);
            this.line3.TabIndex = 58;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 363);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(118, 13);
            this.label5.TabIndex = 57;
            this.label5.Text = "Compatibility / SMS";
            // 
            // checkBoxSendPlaintext
            // 
            this.checkBoxSendPlaintext.AutoSize = true;
            this.checkBoxSendPlaintext.Location = new System.Drawing.Point(16, 386);
            this.checkBoxSendPlaintext.Name = "checkBoxSendPlaintext";
            this.checkBoxSendPlaintext.Size = new System.Drawing.Size(314, 17);
            this.checkBoxSendPlaintext.TabIndex = 59;
            this.checkBoxSendPlaintext.Text = "Send all emails in plaintext (without any formatting, color, etc.)";
            this.checkBoxSendPlaintext.UseVisualStyleBackColor = true;
            // 
            // EmailConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 458);
            this.Controls.Add(this.checkBoxSendPlaintext);
            this.Controls.Add(this.line3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.checkBoxSendSummary);
            this.Controls.Add(this.line2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxEmailAddy3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxEmailAddy2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.line1);
            this.Controls.Add(this.checkBoxEnableEmail);
            this.Controls.Add(this.buttonSendTest);
            this.Controls.Add(this.textBoxRecipientEmail);
            this.Controls.Add(this.label38);
            this.Controls.Add(this.textBoxRecipientName);
            this.Controls.Add(this.label39);
            this.Controls.Add(this.label37);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.checkBoxSsl);
            this.Controls.Add(this.textBoxSenderEmailAddy);
            this.Controls.Add(this.label34);
            this.Controls.Add(this.textBoxSenderFriendly);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.numericUpDownPort);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.textBoxHost);
            this.Controls.Add(this.label31);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.line10);
            this.Controls.Add(this.line9);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.Email;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EmailConfiguration";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Email Configuration";
            this.Load += new System.EventHandler(this.EmailConfiguration_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxEnableEmail;
        private System.Windows.Forms.Button buttonSendTest;
        private System.Windows.Forms.TextBox textBoxRecipientEmail;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.TextBox textBoxRecipientName;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.CheckBox checkBoxEnableAuthentication;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.CheckBox checkBoxSsl;
        private System.Windows.Forms.TextBox textBoxSenderEmailAddy;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox textBoxSenderFriendly;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.NumericUpDown numericUpDownPort;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox textBoxHost;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label18;
        private WSSControls.BelovedComponents.Line line10;
        private WSSControls.BelovedComponents.Line line9;
        private System.Windows.Forms.Label label1;
        private WSSControls.BelovedComponents.Line line1;
        private System.Windows.Forms.TextBox textBoxEmailAddy2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxEmailAddy3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label4;
        private WSSControls.BelovedComponents.Line line2;
        private System.Windows.Forms.CheckBox checkBoxSendSummary;
        private WSSControls.BelovedComponents.Line line3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBoxSendPlaintext;
    }
}