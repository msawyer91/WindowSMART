using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    partial class MainUiControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Hardware Characteristics", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Solid State Disk Information", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Capacity", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Drive Letter(s) / Mount Point(s)", System.Windows.Forms.HorizontalAlignment.Left);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.labelWelcome = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelBadSectors = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel7 = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelPendingSectors = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel8 = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelReallocationEvents = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel9 = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelTemperature = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelRuntime = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonSmartStatus = new System.Windows.Forms.Button();
            this.buttonIgnoreDisk = new System.Windows.Forms.Button();
            this.buttonIgnoreProblem = new System.Windows.Forms.Button();
            this.buttonDocumentation = new System.Windows.Forms.Button();
            this.labelAppTitle = new System.Windows.Forms.Label();
            this.labelBeta = new System.Windows.Forms.Label();
            this.labelSelectPhysical = new System.Windows.Forms.Label();
            this.labelDiskInfo = new System.Windows.Forms.Label();
            this.labelSmartDetails = new System.Windows.Forms.Label();
            this.buttonFastRefresh = new System.Windows.Forms.Button();
            this.buttonExport = new WSSControls.BelovedComponents.SplitButton();
            this.contextMenuExport = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExportHtml = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExportCsv = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExportTsv = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExportText = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTests = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadDebugDiagnosticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listViewSmartDetails = new WSSControls.BelovedComponents.FancyListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewDiskInfo = new WSSControls.BelovedComponents.FancyListView();
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewPhysicalDisks = new WSSControls.BelovedComponents.FancyListView();
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader18 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelDoubleClick = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.contextMenuExport.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelWelcome,
            this.toolStripProgressBar1,
            this.toolStripStatusLabel2,
            this.labelBadSectors,
            this.toolStripStatusLabel7,
            this.labelPendingSectors,
            this.toolStripStatusLabel8,
            this.labelReallocationEvents,
            this.toolStripStatusLabel9,
            this.labelTemperature,
            this.toolStripStatusLabel4,
            this.labelRuntime,
            this.toolStripStatusLabel3});
            this.statusStrip1.Location = new System.Drawing.Point(0, 512);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(982, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // labelWelcome
            // 
            this.labelWelcome.AutoSize = false;
            this.labelWelcome.Name = "labelWelcome";
            this.labelWelcome.Size = new System.Drawing.Size(70, 17);
            this.labelWelcome.Text = "Welcome";
            this.labelWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel2.Text = " ";
            // 
            // labelBadSectors
            // 
            this.labelBadSectors.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.bar_icon_delete_red;
            this.labelBadSectors.Name = "labelBadSectors";
            this.labelBadSectors.Size = new System.Drawing.Size(96, 17);
            this.labelBadSectors.Text = "Bad Sectors: 0";
            this.labelBadSectors.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            // 
            // toolStripStatusLabel7
            // 
            this.toolStripStatusLabel7.Name = "toolStripStatusLabel7";
            this.toolStripStatusLabel7.Size = new System.Drawing.Size(13, 17);
            this.toolStripStatusLabel7.Text = "  ";
            // 
            // labelPendingSectors
            // 
            this.labelPendingSectors.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.bar_icon_delete_red;
            this.labelPendingSectors.Name = "labelPendingSectors";
            this.labelPendingSectors.Size = new System.Drawing.Size(143, 17);
            this.labelPendingSectors.Text = "Pending Bad Sectors: 0";
            this.labelPendingSectors.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            // 
            // toolStripStatusLabel8
            // 
            this.toolStripStatusLabel8.Name = "toolStripStatusLabel8";
            this.toolStripStatusLabel8.Size = new System.Drawing.Size(13, 17);
            this.toolStripStatusLabel8.Text = "  ";
            // 
            // labelReallocationEvents
            // 
            this.labelReallocationEvents.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.bar_icon_delete_red;
            this.labelReallocationEvents.Name = "labelReallocationEvents";
            this.labelReallocationEvents.Size = new System.Drawing.Size(137, 17);
            this.labelReallocationEvents.Text = "Reallocation Events: 0";
            this.labelReallocationEvents.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            // 
            // toolStripStatusLabel9
            // 
            this.toolStripStatusLabel9.Name = "toolStripStatusLabel9";
            this.toolStripStatusLabel9.Size = new System.Drawing.Size(13, 17);
            this.toolStripStatusLabel9.Text = "  ";
            // 
            // labelTemperature
            // 
            this.labelTemperature.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.bar_icon_delete_red;
            this.labelTemperature.Name = "labelTemperature";
            this.labelTemperature.Size = new System.Drawing.Size(150, 17);
            this.labelTemperature.Text = "Temperature: 0 °C / 0 °F";
            this.labelTemperature.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(13, 17);
            this.toolStripStatusLabel4.Text = "  ";
            // 
            // labelRuntime
            // 
            this.labelRuntime.Name = "labelRuntime";
            this.labelRuntime.Size = new System.Drawing.Size(139, 17);
            this.labelRuntime.Text = "Runtime: 0 Days, 0 Hours";
            this.labelRuntime.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(68, 17);
            this.toolStripStatusLabel3.Spring = true;
            this.toolStripStatusLabel3.Text = "   ";
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRefresh.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.RefreshDisk48;
            this.buttonRefresh.Location = new System.Drawing.Point(17, 4);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(120, 72);
            this.buttonRefresh.TabIndex = 14;
            this.buttonRefresh.Text = "Query Disks";
            this.buttonRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonSmartStatus
            // 
            this.buttonSmartStatus.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSmartStatus.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.SmartStatus48;
            this.buttonSmartStatus.Location = new System.Drawing.Point(143, 4);
            this.buttonSmartStatus.Name = "buttonSmartStatus";
            this.buttonSmartStatus.Size = new System.Drawing.Size(120, 72);
            this.buttonSmartStatus.TabIndex = 15;
            this.buttonSmartStatus.Text = "Health Status";
            this.buttonSmartStatus.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonSmartStatus.UseVisualStyleBackColor = true;
            this.buttonSmartStatus.Click += new System.EventHandler(this.buttonSmartStatus_Click);
            // 
            // buttonIgnoreDisk
            // 
            this.buttonIgnoreDisk.Enabled = false;
            this.buttonIgnoreDisk.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonIgnoreDisk.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.IgnoreDisk24;
            this.buttonIgnoreDisk.Location = new System.Drawing.Point(426, 4);
            this.buttonIgnoreDisk.Name = "buttonIgnoreDisk";
            this.buttonIgnoreDisk.Size = new System.Drawing.Size(151, 32);
            this.buttonIgnoreDisk.TabIndex = 16;
            this.buttonIgnoreDisk.Text = "Ignore Disk";
            this.buttonIgnoreDisk.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonIgnoreDisk.UseVisualStyleBackColor = true;
            this.buttonIgnoreDisk.Click += new System.EventHandler(this.buttonIgnoreDisk_Click);
            // 
            // buttonIgnoreProblem
            // 
            this.buttonIgnoreProblem.Enabled = false;
            this.buttonIgnoreProblem.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonIgnoreProblem.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.IgnoreProblem24;
            this.buttonIgnoreProblem.Location = new System.Drawing.Point(426, 44);
            this.buttonIgnoreProblem.Name = "buttonIgnoreProblem";
            this.buttonIgnoreProblem.Size = new System.Drawing.Size(151, 32);
            this.buttonIgnoreProblem.TabIndex = 17;
            this.buttonIgnoreProblem.Text = "Ignore Problem";
            this.buttonIgnoreProblem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonIgnoreProblem.UseVisualStyleBackColor = true;
            this.buttonIgnoreProblem.Click += new System.EventHandler(this.buttonIgnoreProblem_Click);
            // 
            // buttonDocumentation
            // 
            this.buttonDocumentation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDocumentation.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDocumentation.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.Help48;
            this.buttonDocumentation.Location = new System.Drawing.Point(846, 4);
            this.buttonDocumentation.Name = "buttonDocumentation";
            this.buttonDocumentation.Size = new System.Drawing.Size(120, 72);
            this.buttonDocumentation.TabIndex = 18;
            this.buttonDocumentation.Text = "Documentation";
            this.buttonDocumentation.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonDocumentation.UseVisualStyleBackColor = true;
            this.buttonDocumentation.Click += new System.EventHandler(this.buttonDocumentation_Click);
            // 
            // labelAppTitle
            // 
            this.labelAppTitle.AutoSize = true;
            this.labelAppTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelAppTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAppTitle.Location = new System.Drawing.Point(583, 16);
            this.labelAppTitle.Name = "labelAppTitle";
            this.labelAppTitle.Size = new System.Drawing.Size(70, 25);
            this.labelAppTitle.TabIndex = 19;
            this.labelAppTitle.Text = "label1";
            // 
            // labelBeta
            // 
            this.labelBeta.AutoSize = true;
            this.labelBeta.BackColor = System.Drawing.Color.Transparent;
            this.labelBeta.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBeta.Location = new System.Drawing.Point(584, 42);
            this.labelBeta.Name = "labelBeta";
            this.labelBeta.Size = new System.Drawing.Size(51, 16);
            this.labelBeta.TabIndex = 20;
            this.labelBeta.Text = "label1";
            // 
            // labelSelectPhysical
            // 
            this.labelSelectPhysical.AutoSize = true;
            this.labelSelectPhysical.BackColor = System.Drawing.Color.Transparent;
            this.labelSelectPhysical.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelectPhysical.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelSelectPhysical.Location = new System.Drawing.Point(16, 85);
            this.labelSelectPhysical.Name = "labelSelectPhysical";
            this.labelSelectPhysical.Size = new System.Drawing.Size(52, 18);
            this.labelSelectPhysical.TabIndex = 21;
            this.labelSelectPhysical.Text = "label1";
            // 
            // labelDiskInfo
            // 
            this.labelDiskInfo.AutoSize = true;
            this.labelDiskInfo.BackColor = System.Drawing.Color.Transparent;
            this.labelDiskInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDiskInfo.Location = new System.Drawing.Point(495, 85);
            this.labelDiskInfo.Name = "labelDiskInfo";
            this.labelDiskInfo.Size = new System.Drawing.Size(52, 18);
            this.labelDiskInfo.TabIndex = 22;
            this.labelDiskInfo.Text = "label1";
            // 
            // labelSmartDetails
            // 
            this.labelSmartDetails.AutoSize = true;
            this.labelSmartDetails.BackColor = System.Drawing.Color.Transparent;
            this.labelSmartDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSmartDetails.Location = new System.Drawing.Point(16, 284);
            this.labelSmartDetails.Name = "labelSmartDetails";
            this.labelSmartDetails.Size = new System.Drawing.Size(52, 18);
            this.labelSmartDetails.TabIndex = 23;
            this.labelSmartDetails.Text = "label1";
            // 
            // buttonFastRefresh
            // 
            this.buttonFastRefresh.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonFastRefresh.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.RefreshDisk24;
            this.buttonFastRefresh.Location = new System.Drawing.Point(269, 4);
            this.buttonFastRefresh.Name = "buttonFastRefresh";
            this.buttonFastRefresh.Size = new System.Drawing.Size(151, 32);
            this.buttonFastRefresh.TabIndex = 24;
            this.buttonFastRefresh.Text = "Fast Refresh";
            this.buttonFastRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonFastRefresh.UseVisualStyleBackColor = true;
            this.buttonFastRefresh.Click += new System.EventHandler(this.buttonFastRefresh_Click);
            // 
            // buttonExport
            // 
            this.buttonExport.AlwaysDropDown = true;
            this.buttonExport.ClickedImage = "Clicked";
            this.buttonExport.ContextMenuStrip = this.contextMenuExport;
            this.buttonExport.DisabledImage = "Disabled";
            this.buttonExport.Enabled = false;
            this.buttonExport.FocusedImage = "Focused";
            this.buttonExport.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExport.HoverImage = "Hover";
            this.buttonExport.ImageKey = "Normal";
            this.buttonExport.Location = new System.Drawing.Point(269, 44);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.NormalImage = "Normal";
            this.buttonExport.Size = new System.Drawing.Size(151, 32);
            this.buttonExport.TabIndex = 25;
            this.buttonExport.Text = "Advanced Items";
            this.buttonExport.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.buttonExport.UseVisualStyleBackColor = true;
            // 
            // contextMenuExport
            // 
            this.contextMenuExport.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.menuTests,
            this.uploadDebugDiagnosticsToolStripMenuItem});
            this.contextMenuExport.Name = "contextMenuExport";
            this.contextMenuExport.Size = new System.Drawing.Size(218, 92);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.Policy;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.settingsToolStripMenuItem.Text = "&Settings...";
            this.settingsToolStripMenuItem.Visible = false;
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemExportHtml,
            this.menuItemExportCsv,
            this.menuItemExportTsv,
            this.menuItemExportText});
            this.toolStripMenuItem1.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.ExportCsvTsv;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(217, 22);
            this.toolStripMenuItem1.Text = "&Export Results";
            // 
            // menuItemExportHtml
            // 
            this.menuItemExportHtml.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.ExportHtml;
            this.menuItemExportHtml.Name = "menuItemExportHtml";
            this.menuItemExportHtml.Size = new System.Drawing.Size(236, 22);
            this.menuItemExportHtml.Text = "As &HTML...";
            this.menuItemExportHtml.Click += new System.EventHandler(this.menuItemExportHtml_Click);
            // 
            // menuItemExportCsv
            // 
            this.menuItemExportCsv.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.ExportCsvTsv;
            this.menuItemExportCsv.Name = "menuItemExportCsv";
            this.menuItemExportCsv.Size = new System.Drawing.Size(236, 22);
            this.menuItemExportCsv.Text = "As &Comma-Separated Values...";
            this.menuItemExportCsv.Click += new System.EventHandler(this.menuItemExportCsv_Click);
            // 
            // menuItemExportTsv
            // 
            this.menuItemExportTsv.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.ExportCsvTsv;
            this.menuItemExportTsv.Name = "menuItemExportTsv";
            this.menuItemExportTsv.Size = new System.Drawing.Size(236, 22);
            this.menuItemExportTsv.Text = "As &Tab-Separated Values...";
            this.menuItemExportTsv.Click += new System.EventHandler(this.menuItemExportTsv_Click);
            // 
            // menuItemExportText
            // 
            this.menuItemExportText.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.ExportText;
            this.menuItemExportText.Name = "menuItemExportText";
            this.menuItemExportText.Size = new System.Drawing.Size(236, 22);
            this.menuItemExportText.Text = "As T&ext...";
            this.menuItemExportText.Click += new System.EventHandler(this.menuItemExportText_Click);
            // 
            // menuTests
            // 
            this.menuTests.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.ExtendedTest;
            this.menuTests.Name = "menuTests";
            this.menuTests.Size = new System.Drawing.Size(217, 22);
            this.menuTests.Text = "Disk Self-&Tests...";
            this.menuTests.Click += new System.EventHandler(this.menuTests_Click);
            // 
            // uploadDebugDiagnosticsToolStripMenuItem
            // 
            this.uploadDebugDiagnosticsToolStripMenuItem.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.Updates16;
            this.uploadDebugDiagnosticsToolStripMenuItem.Name = "uploadDebugDiagnosticsToolStripMenuItem";
            this.uploadDebugDiagnosticsToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.uploadDebugDiagnosticsToolStripMenuItem.Text = "&Report a Bug/Diagnostics...";
            this.uploadDebugDiagnosticsToolStripMenuItem.Click += new System.EventHandler(this.uploadDebugDiagnosticsToolStripMenuItem_Click);
            // 
            // listViewSmartDetails
            // 
            this.listViewSmartDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewSmartDetails.BitmapList = null;
            this.listViewSmartDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewSmartDetails.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader16,
            this.columnHeader17});
            this.listViewSmartDetails.FullRowSelect = true;
            this.listViewSmartDetails.Location = new System.Drawing.Point(17, 304);
            this.listViewSmartDetails.MinimumSize = new System.Drawing.Size(2, 53);
            this.listViewSmartDetails.MultiSelect = false;
            this.listViewSmartDetails.Name = "listViewSmartDetails";
            this.listViewSmartDetails.OwnerDraw = true;
            this.listViewSmartDetails.Size = new System.Drawing.Size(949, 196);
            this.listViewSmartDetails.TabIndex = 3;
            this.listViewSmartDetails.UseCompatibleStateImageBehavior = false;
            this.listViewSmartDetails.View = System.Windows.Forms.View.Details;
            this.listViewSmartDetails.SelectedIndexChanged += new System.EventHandler(this.listViewSmartDetails_SelectedIndexChanged);
            this.listViewSmartDetails.DoubleClick += new System.EventHandler(this.listViewSmartDetails_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "BX";
            this.columnHeader1.Width = 0;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "";
            this.columnHeader2.Width = 30;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "ID (Dec)";
            this.columnHeader3.Width = 66;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "ID (Hex)";
            this.columnHeader4.Width = 66;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Attribute";
            this.columnHeader5.Width = 234;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Critical";
            this.columnHeader6.Width = 59;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Type";
            this.columnHeader7.Width = 59;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Threshold";
            this.columnHeader8.Width = 75;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Value";
            this.columnHeader9.Width = 75;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Worst";
            this.columnHeader10.Width = 75;
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "Status";
            this.columnHeader16.Width = 96;
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "Raw Data";
            this.columnHeader17.Width = 114;
            // 
            // listViewDiskInfo
            // 
            this.listViewDiskInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewDiskInfo.BitmapList = null;
            this.listViewDiskInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewDiskInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader14,
            this.columnHeader15});
            this.listViewDiskInfo.FullRowSelect = true;
            listViewGroup1.Header = "Hardware Characteristics";
            listViewGroup1.Name = "listViewGroupHw";
            listViewGroup2.Header = "Solid State Disk Information";
            listViewGroup2.Name = "listViewGroupSsd";
            listViewGroup3.Header = "Capacity";
            listViewGroup3.Name = "listViewGroupCapacity";
            listViewGroup4.Header = "Drive Letter(s) / Mount Point(s)";
            listViewGroup4.Name = "listViewGroupMountPoints";
            this.listViewDiskInfo.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4});
            this.listViewDiskInfo.Location = new System.Drawing.Point(496, 105);
            this.listViewDiskInfo.MinimumSize = new System.Drawing.Size(2, 166);
            this.listViewDiskInfo.MultiSelect = false;
            this.listViewDiskInfo.Name = "listViewDiskInfo";
            this.listViewDiskInfo.OwnerDraw = true;
            this.listViewDiskInfo.Size = new System.Drawing.Size(470, 166);
            this.listViewDiskInfo.TabIndex = 2;
            this.listViewDiskInfo.UseCompatibleStateImageBehavior = false;
            this.listViewDiskInfo.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "Attribute";
            this.columnHeader14.Width = 150;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "Value";
            this.columnHeader15.Width = 320;
            // 
            // listViewPhysicalDisks
            // 
            this.listViewPhysicalDisks.BitmapList = null;
            this.listViewPhysicalDisks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewPhysicalDisks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader18});
            this.listViewPhysicalDisks.FullRowSelect = true;
            this.listViewPhysicalDisks.Location = new System.Drawing.Point(17, 105);
            this.listViewPhysicalDisks.MinimumSize = new System.Drawing.Size(470, 166);
            this.listViewPhysicalDisks.MultiSelect = false;
            this.listViewPhysicalDisks.Name = "listViewPhysicalDisks";
            this.listViewPhysicalDisks.OwnerDraw = true;
            this.listViewPhysicalDisks.Size = new System.Drawing.Size(470, 166);
            this.listViewPhysicalDisks.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewPhysicalDisks.TabIndex = 1;
            this.listViewPhysicalDisks.UseCompatibleStateImageBehavior = false;
            this.listViewPhysicalDisks.View = System.Windows.Forms.View.Details;
            this.listViewPhysicalDisks.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewPhysicalDisks_ColumnClick);
            this.listViewPhysicalDisks.SelectedIndexChanged += new System.EventHandler(this.listViewPhysicalDisks_SelectedIndexChanged);
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Disk";
            this.columnHeader11.Width = 120;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Model";
            this.columnHeader12.Width = 217;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Capacity";
            this.columnHeader13.Width = 84;
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = "Temp";
            this.columnHeader18.Width = 49;
            // 
            // labelDoubleClick
            // 
            this.labelDoubleClick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDoubleClick.AutoSize = true;
            this.labelDoubleClick.BackColor = System.Drawing.Color.Transparent;
            this.labelDoubleClick.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDoubleClick.Location = new System.Drawing.Point(618, 286);
            this.labelDoubleClick.Name = "labelDoubleClick";
            this.labelDoubleClick.Size = new System.Drawing.Size(348, 16);
            this.labelDoubleClick.TabIndex = 26;
            this.labelDoubleClick.Text = "(Double-click attributes to learn more about them)";
            this.labelDoubleClick.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // MainUiControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackgroundImage = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.MetalGrate;
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.buttonFastRefresh);
            this.Controls.Add(this.labelSmartDetails);
            this.Controls.Add(this.labelDoubleClick);
            this.Controls.Add(this.labelDiskInfo);
            this.Controls.Add(this.labelSelectPhysical);
            this.Controls.Add(this.labelBeta);
            this.Controls.Add(this.labelAppTitle);
            this.Controls.Add(this.buttonDocumentation);
            this.Controls.Add(this.buttonIgnoreProblem);
            this.Controls.Add(this.buttonIgnoreDisk);
            this.Controls.Add(this.buttonSmartStatus);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.listViewSmartDetails);
            this.Controls.Add(this.listViewDiskInfo);
            this.Controls.Add(this.listViewPhysicalDisks);
            this.Controls.Add(this.statusStrip1);
            this.MaximumSize = new System.Drawing.Size(65535, 65535);
            this.MinimumSize = new System.Drawing.Size(770, 350);
            this.Name = "MainUiControl";
            this.Size = new System.Drawing.Size(982, 534);
            this.Load += new System.EventHandler(this.MainUiControl_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuExport.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel labelWelcome;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private FancyListView listViewPhysicalDisks;
        private FancyListView listViewDiskInfo;
        private FancyListView listViewSmartDetails;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.ColumnHeader columnHeader16;
        private System.Windows.Forms.ColumnHeader columnHeader17;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel labelBadSectors;
        private System.Windows.Forms.ToolStripStatusLabel labelPendingSectors;
        private System.Windows.Forms.ToolStripStatusLabel labelReallocationEvents;
        private System.Windows.Forms.ToolStripStatusLabel labelTemperature;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel7;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel8;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel9;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonSmartStatus;
        private System.Windows.Forms.Button buttonIgnoreDisk;
        private System.Windows.Forms.Button buttonIgnoreProblem;
        private System.Windows.Forms.Button buttonDocumentation;
        private System.Windows.Forms.Label labelAppTitle;
        private System.Windows.Forms.Label labelBeta;
        private System.Windows.Forms.Label labelSelectPhysical;
        private System.Windows.Forms.Label labelDiskInfo;
        private System.Windows.Forms.Label labelSmartDetails;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.Button buttonFastRefresh;
        private WSSControls.BelovedComponents.SplitButton buttonExport;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel labelRuntime;
        private System.Windows.Forms.Label labelDoubleClick;
        private System.Windows.Forms.ContextMenuStrip contextMenuExport;
        private System.Windows.Forms.ToolStripMenuItem menuItemExportHtml;
        private System.Windows.Forms.ToolStripMenuItem menuItemExportCsv;
        private System.Windows.Forms.ToolStripMenuItem menuItemExportTsv;
        private System.Windows.Forms.ToolStripMenuItem menuItemExportText;
        private System.Windows.Forms.ToolStripMenuItem menuTests;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader18;
        private System.Windows.Forms.ToolStripMenuItem uploadDebugDiagnosticsToolStripMenuItem;
    }
}
