namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    partial class EpicFail
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EpicFail));
            this.pictureBoxDunce = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.textBoxTroubleshoot = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxStackTrace = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonBugReport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDunce)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxDunce
            // 
            this.pictureBoxDunce.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxDunce.Name = "pictureBoxDunce";
            this.pictureBoxDunce.Size = new System.Drawing.Size(204, 400);
            this.pictureBoxDunce.TabIndex = 0;
            this.pictureBoxDunce.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(233, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(462, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "Aw, snap! It looks like we ran into a bit of a problem here. The information belo" +
    "w can help aid in troubleshooting. If the problem persists, please fill out a Bu" +
    "g Report.";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(233, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(462, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "What went wrong, and why? (Error message and possible causes)";
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(236, 73);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.ReadOnly = true;
            this.textBoxMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMessage.Size = new System.Drawing.Size(459, 61);
            this.textBoxMessage.TabIndex = 3;
            this.textBoxMessage.Text = "Exceptions were detected by the Server.";
            // 
            // textBoxTroubleshoot
            // 
            this.textBoxTroubleshoot.Location = new System.Drawing.Point(236, 177);
            this.textBoxTroubleshoot.Multiline = true;
            this.textBoxTroubleshoot.Name = "textBoxTroubleshoot";
            this.textBoxTroubleshoot.ReadOnly = true;
            this.textBoxTroubleshoot.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxTroubleshoot.Size = new System.Drawing.Size(459, 91);
            this.textBoxTroubleshoot.TabIndex = 5;
            this.textBoxTroubleshoot.Text = "Steps go here.";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(233, 154);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(462, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Is there anything I can do? (Troubleshooting/Resolution steps)";
            // 
            // textBoxStackTrace
            // 
            this.textBoxStackTrace.Location = new System.Drawing.Point(236, 313);
            this.textBoxStackTrace.Multiline = true;
            this.textBoxStackTrace.Name = "textBoxStackTrace";
            this.textBoxStackTrace.ReadOnly = true;
            this.textBoxStackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxStackTrace.Size = new System.Drawing.Size(459, 70);
            this.textBoxStackTrace.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(233, 290);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(462, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Geeky, technical stuff (Stack trace)";
            // 
            // buttonCopy
            // 
            this.buttonCopy.Location = new System.Drawing.Point(498, 389);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(107, 23);
            this.buttonCopy.TabIndex = 8;
            this.buttonCopy.Text = "Copy to Clipboard";
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(620, 389);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 9;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonBugReport
            // 
            this.buttonBugReport.Location = new System.Drawing.Point(236, 389);
            this.buttonBugReport.Name = "buttonBugReport";
            this.buttonBugReport.Size = new System.Drawing.Size(107, 23);
            this.buttonBugReport.TabIndex = 10;
            this.buttonBugReport.Text = "Bug Report...";
            this.buttonBugReport.UseVisualStyleBackColor = true;
            this.buttonBugReport.Click += new System.EventHandler(this.buttonBugReport_Click);
            // 
            // EpicFail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 428);
            this.Controls.Add(this.buttonBugReport);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonCopy);
            this.Controls.Add(this.textBoxStackTrace);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxTroubleshoot);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxDunce);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EpicFail";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Epic FAIL";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.EpicFail_Load);
            this.Shown += new System.EventHandler(this.EpicFail_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDunce)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxDunce;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.TextBox textBoxTroubleshoot;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxStackTrace;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonBugReport;
    }
}