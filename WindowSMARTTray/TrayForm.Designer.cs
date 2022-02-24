namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.WindowSMARTTray
{
    partial class TrayForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrayForm));
            this.systemTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuConsole = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // systemTray
            // 
            this.systemTray.ContextMenuStrip = this.contextMenu;
            this.systemTray.Icon = ((System.Drawing.Icon)(resources.GetObject("systemTray.Icon")));
            this.systemTray.Text = "WindowSMART 24/7";
            this.systemTray.Visible = true;
            this.systemTray.BalloonTipClicked += new System.EventHandler(this.systemTray_BalloonTipClicked);
            this.systemTray.DoubleClick += new System.EventHandler(this.systemTray_DoubleClick);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuConsole,
            this.menuAbout,
            this.menuExit});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(127, 70);
            // 
            // menuConsole
            // 
            this.menuConsole.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.WindowSMARTTray.Properties.Resources.ShieldMulti16;
            this.menuConsole.Name = "menuConsole";
            this.menuConsole.Size = new System.Drawing.Size(126, 22);
            this.menuConsole.Text = "&Console...";
            this.menuConsole.Click += new System.EventHandler(this.menuConsole_Click);
            // 
            // menuAbout
            // 
            this.menuAbout.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.WindowSMARTTray.Properties.Resources.Unknown16;
            this.menuAbout.Name = "menuAbout";
            this.menuAbout.Size = new System.Drawing.Size(126, 22);
            this.menuAbout.Text = "&About...";
            this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
            // 
            // menuExit
            // 
            this.menuExit.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.WindowSMARTTray.Properties.Resources.Exit;
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(126, 22);
            this.menuExit.Text = "&Exit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // TrayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(130, 35);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrayForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "TrayForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TrayForm_FormClosing);
            this.Load += new System.EventHandler(this.TrayForm_Load);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon systemTray;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem menuConsole;
        private System.Windows.Forms.ToolStripMenuItem menuAbout;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
    }
}