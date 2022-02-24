namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    partial class SelfTests
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelfTests));
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonShort = new System.Windows.Forms.Button();
            this.buttonExtended = new System.Windows.Forms.Button();
            this.buttonConveyance = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.listViewPhysicalDisks = new WSSControls.BelovedComponents.FancyListView();
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.buttonExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.Refresh;
            this.buttonRefresh.Location = new System.Drawing.Point(12, 12);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(128, 29);
            this.buttonRefresh.TabIndex = 0;
            this.buttonRefresh.Text = "Refresh Test Status";
            this.buttonRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonShort
            // 
            this.buttonShort.Enabled = false;
            this.buttonShort.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.ShortTest;
            this.buttonShort.Location = new System.Drawing.Point(146, 12);
            this.buttonShort.Name = "buttonShort";
            this.buttonShort.Size = new System.Drawing.Size(128, 29);
            this.buttonShort.TabIndex = 1;
            this.buttonShort.Text = "Short Test";
            this.buttonShort.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonShort.UseVisualStyleBackColor = true;
            this.buttonShort.Click += new System.EventHandler(this.buttonShort_Click);
            // 
            // buttonExtended
            // 
            this.buttonExtended.Enabled = false;
            this.buttonExtended.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.ExtendedTest;
            this.buttonExtended.Location = new System.Drawing.Point(280, 12);
            this.buttonExtended.Name = "buttonExtended";
            this.buttonExtended.Size = new System.Drawing.Size(128, 29);
            this.buttonExtended.TabIndex = 2;
            this.buttonExtended.Text = "Extended Test";
            this.buttonExtended.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonExtended.UseVisualStyleBackColor = true;
            this.buttonExtended.Click += new System.EventHandler(this.buttonExtended_Click);
            // 
            // buttonConveyance
            // 
            this.buttonConveyance.Enabled = false;
            this.buttonConveyance.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.Conveyance;
            this.buttonConveyance.Location = new System.Drawing.Point(414, 12);
            this.buttonConveyance.Name = "buttonConveyance";
            this.buttonConveyance.Size = new System.Drawing.Size(128, 29);
            this.buttonConveyance.TabIndex = 3;
            this.buttonConveyance.Text = "Conveyance Test";
            this.buttonConveyance.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonConveyance.UseVisualStyleBackColor = true;
            this.buttonConveyance.Click += new System.EventHandler(this.buttonConveyance_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.bar_icon_delete_red;
            this.buttonCancel.Location = new System.Drawing.Point(548, 12);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(128, 29);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel Test";
            this.buttonCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // listViewPhysicalDisks
            // 
            this.listViewPhysicalDisks.BitmapList = null;
            this.listViewPhysicalDisks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewPhysicalDisks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13});
            this.listViewPhysicalDisks.FullRowSelect = true;
            this.listViewPhysicalDisks.Location = new System.Drawing.Point(12, 100);
            this.listViewPhysicalDisks.MinimumSize = new System.Drawing.Size(470, 166);
            this.listViewPhysicalDisks.MultiSelect = false;
            this.listViewPhysicalDisks.Name = "listViewPhysicalDisks";
            this.listViewPhysicalDisks.OwnerDraw = true;
            this.listViewPhysicalDisks.Size = new System.Drawing.Size(798, 170);
            this.listViewPhysicalDisks.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewPhysicalDisks.TabIndex = 5;
            this.listViewPhysicalDisks.UseCompatibleStateImageBehavior = false;
            this.listViewPhysicalDisks.View = System.Windows.Forms.View.Details;
            this.listViewPhysicalDisks.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewPhysicalDisks_ColumnClick);
            this.listViewPhysicalDisks.SelectedIndexChanged += new System.EventHandler(this.listViewPhysicalDisks_SelectedIndexChanged);
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Disk";
            this.columnHeader11.Width = 135;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Model";
            this.columnHeader12.Width = 231;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Last Test Status";
            this.columnHeader13.Width = 432;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(784, 44);
            this.label1.TabIndex = 6;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // buttonExit
            // 
            this.buttonExit.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.Properties.Resources.Exit;
            this.buttonExit.Location = new System.Drawing.Point(682, 12);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(128, 29);
            this.buttonExit.TabIndex = 7;
            this.buttonExit.Text = "Exit Tests";
            this.buttonExit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // SelfTests
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 283);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewPhysicalDisks);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonConveyance);
            this.Controls.Add(this.buttonExtended);
            this.Controls.Add(this.buttonShort);
            this.Controls.Add(this.buttonRefresh);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelfTests";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Run SMART Self Tests";
            this.Load += new System.EventHandler(this.SelfTests_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonShort;
        private System.Windows.Forms.Button buttonExtended;
        private System.Windows.Forms.Button buttonConveyance;
        private System.Windows.Forms.Button buttonCancel;
        private WSSControls.BelovedComponents.FancyListView listViewPhysicalDisks;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonExit;
    }
}