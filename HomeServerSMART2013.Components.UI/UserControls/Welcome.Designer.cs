namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    partial class Welcome
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Welcome));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelTrialExpiration = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonContinue = new System.Windows.Forms.Button();
            this.buttonEnterLicense = new System.Windows.Forms.Button();
            this.buttonPurchase = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.HSS128;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(146, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(410, 44);
            this.label1.TabIndex = 1;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // labelTrialExpiration
            // 
            this.labelTrialExpiration.AutoSize = true;
            this.labelTrialExpiration.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTrialExpiration.Location = new System.Drawing.Point(146, 63);
            this.labelTrialExpiration.Name = "labelTrialExpiration";
            this.labelTrialExpiration.Size = new System.Drawing.Size(100, 13);
            this.labelTrialExpiration.TabIndex = 2;
            this.labelTrialExpiration.Text = "[Trial Expiration]";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(146, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(399, 53);
            this.label2.TabIndex = 3;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // buttonContinue
            // 
            this.buttonContinue.Location = new System.Drawing.Point(12, 155);
            this.buttonContinue.Name = "buttonContinue";
            this.buttonContinue.Size = new System.Drawing.Size(128, 26);
            this.buttonContinue.TabIndex = 4;
            this.buttonContinue.Text = "Continue Trial";
            this.buttonContinue.UseVisualStyleBackColor = true;
            this.buttonContinue.Click += new System.EventHandler(this.buttonContinue_Click);
            // 
            // buttonEnterLicense
            // 
            this.buttonEnterLicense.Location = new System.Drawing.Point(149, 155);
            this.buttonEnterLicense.Name = "buttonEnterLicense";
            this.buttonEnterLicense.Size = new System.Drawing.Size(128, 26);
            this.buttonEnterLicense.TabIndex = 5;
            this.buttonEnterLicense.Text = "Enter License Key...";
            this.buttonEnterLicense.UseVisualStyleBackColor = true;
            this.buttonEnterLicense.Click += new System.EventHandler(this.buttonEnterLicense_Click);
            // 
            // buttonPurchase
            // 
            this.buttonPurchase.Location = new System.Drawing.Point(283, 155);
            this.buttonPurchase.Name = "buttonPurchase";
            this.buttonPurchase.Size = new System.Drawing.Size(128, 26);
            this.buttonPurchase.TabIndex = 6;
            this.buttonPurchase.Text = "Purchase a License...";
            this.buttonPurchase.UseVisualStyleBackColor = true;
            this.buttonPurchase.Click += new System.EventHandler(this.buttonPurchase_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonExit.Location = new System.Drawing.Point(417, 155);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(128, 26);
            this.buttonExit.TabIndex = 7;
            this.buttonExit.Text = "Exit WindowSMART";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // Welcome
            // 
            this.AcceptButton = this.buttonEnterLicense;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonExit;
            this.ClientSize = new System.Drawing.Size(559, 195);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonPurchase);
            this.Controls.Add(this.buttonEnterLicense);
            this.Controls.Add(this.buttonContinue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelTrialExpiration);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Welcome";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Welcome to WindowSMART 24/7!";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Welcome_FormClosing);
            this.Load += new System.EventHandler(this.Welcome_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTrialExpiration;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonContinue;
        private System.Windows.Forms.Button buttonEnterLicense;
        private System.Windows.Forms.Button buttonPurchase;
        private System.Windows.Forms.Button buttonExit;
    }
}