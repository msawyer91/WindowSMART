using  WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI
{
    partial class BitLockerControl
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
            this.buttonEncrypt = new System.Windows.Forms.Button();
            this.buttonDecrypt = new System.Windows.Forms.Button();
            this.buttonLock = new System.Windows.Forms.Button();
            this.buttonUnlock = new System.Windows.Forms.Button();
            this.protectorsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAutoUnlock = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAutoUnlockEnable = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAutoUnlockDisable = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBek = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBekAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBekChange = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBekDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNumPassword = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNumPasswordAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNumPasswordDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSmartCards = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSmartCardsAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSmartCardsDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPassword = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPasswordAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPasswordChange = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPasswordDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSuspend = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSuspendOff = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSuspendOn = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewProtectors = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.encryptionProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.volumeStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonQuickHelp = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.advancedMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuRepairTpm = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEscrowKeysToAd = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUpgrade = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDriveBender = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewGpo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuChangeVolumeLabel = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuPause = new System.Windows.Forms.ToolStripMenuItem();
            this.menuResume = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonOptions = new WSSControls.BelovedComponents.SplitButton();
            this.buttonAdvanced = new WSSControls.BelovedComponents.SplitButton();
            this.lvEncryptableVolumes = new WSSControls.BelovedComponents.FancyListView();
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
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonProtectors = new WSSControls.BelovedComponents.SplitButton();
            this.protectorsMenu.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.advancedMenu.SuspendLayout();
            this.optionsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonEncrypt
            // 
            this.buttonEncrypt.Enabled = false;
            this.buttonEncrypt.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEncrypt.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Encrypt48;
            this.buttonEncrypt.Location = new System.Drawing.Point(17, 4);
            this.buttonEncrypt.Name = "buttonEncrypt";
            this.buttonEncrypt.Size = new System.Drawing.Size(120, 72);
            this.buttonEncrypt.TabIndex = 0;
            this.buttonEncrypt.Text = "Encrypt Disk";
            this.buttonEncrypt.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonEncrypt.UseVisualStyleBackColor = true;
            this.buttonEncrypt.Click += new System.EventHandler(this.buttonEncrypt_Click);
            // 
            // buttonDecrypt
            // 
            this.buttonDecrypt.Enabled = false;
            this.buttonDecrypt.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDecrypt.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Decrypt48;
            this.buttonDecrypt.Location = new System.Drawing.Point(143, 4);
            this.buttonDecrypt.Name = "buttonDecrypt";
            this.buttonDecrypt.Size = new System.Drawing.Size(120, 72);
            this.buttonDecrypt.TabIndex = 1;
            this.buttonDecrypt.Text = "Decrypt Disk";
            this.buttonDecrypt.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonDecrypt.UseVisualStyleBackColor = true;
            this.buttonDecrypt.Click += new System.EventHandler(this.buttonDecrypt_Click);
            // 
            // buttonLock
            // 
            this.buttonLock.Enabled = false;
            this.buttonLock.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLock.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Lock24;
            this.buttonLock.Location = new System.Drawing.Point(269, 4);
            this.buttonLock.Name = "buttonLock";
            this.buttonLock.Size = new System.Drawing.Size(128, 32);
            this.buttonLock.TabIndex = 2;
            this.buttonLock.Text = "Lock";
            this.buttonLock.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonLock.UseVisualStyleBackColor = true;
            this.buttonLock.Click += new System.EventHandler(this.buttonLock_Click);
            // 
            // buttonUnlock
            // 
            this.buttonUnlock.Enabled = false;
            this.buttonUnlock.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUnlock.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Unlock24;
            this.buttonUnlock.Location = new System.Drawing.Point(269, 44);
            this.buttonUnlock.Name = "buttonUnlock";
            this.buttonUnlock.Size = new System.Drawing.Size(128, 32);
            this.buttonUnlock.TabIndex = 3;
            this.buttonUnlock.Text = "Unlock";
            this.buttonUnlock.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonUnlock.UseVisualStyleBackColor = true;
            this.buttonUnlock.Click += new System.EventHandler(this.buttonUnlock_Click);
            // 
            // protectorsMenu
            // 
            this.protectorsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAutoUnlock,
            this.menuBek,
            this.menuNumPassword,
            this.menuSmartCards,
            this.menuPassword,
            this.menuSuspend,
            this.menuViewProtectors});
            this.protectorsMenu.Name = "protectorsMenu";
            this.protectorsMenu.Size = new System.Drawing.Size(174, 158);
            // 
            // menuAutoUnlock
            // 
            this.menuAutoUnlock.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAutoUnlockEnable,
            this.menuAutoUnlockDisable});
            this.menuAutoUnlock.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.ShieldMulti16;
            this.menuAutoUnlock.Name = "menuAutoUnlock";
            this.menuAutoUnlock.Size = new System.Drawing.Size(173, 22);
            this.menuAutoUnlock.Text = "&Automatic Unlock";
            // 
            // menuAutoUnlockEnable
            // 
            this.menuAutoUnlockEnable.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.ShieldGreen16;
            this.menuAutoUnlockEnable.Name = "menuAutoUnlockEnable";
            this.menuAutoUnlockEnable.Size = new System.Drawing.Size(112, 22);
            this.menuAutoUnlockEnable.Text = "&Enable";
            this.menuAutoUnlockEnable.Click += new System.EventHandler(this.menuAutoUnlockEnable_Click);
            // 
            // menuAutoUnlockDisable
            // 
            this.menuAutoUnlockDisable.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.ShieldYellow16;
            this.menuAutoUnlockDisable.Name = "menuAutoUnlockDisable";
            this.menuAutoUnlockDisable.Size = new System.Drawing.Size(112, 22);
            this.menuAutoUnlockDisable.Text = "&Disable";
            this.menuAutoUnlockDisable.Click += new System.EventHandler(this.menuAutoUnlockDisable_Click);
            // 
            // menuBek
            // 
            this.menuBek.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuBekAdd,
            this.menuBekChange,
            this.menuBekDelete});
            this.menuBek.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Unlock16;
            this.menuBek.Name = "menuBek";
            this.menuBek.Size = new System.Drawing.Size(173, 22);
            this.menuBek.Text = "&External Key";
            // 
            // menuBekAdd
            // 
            this.menuBekAdd.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Add;
            this.menuBekAdd.Name = "menuBekAdd";
            this.menuBekAdd.Size = new System.Drawing.Size(124, 22);
            this.menuBekAdd.Text = "&Add...";
            this.menuBekAdd.Click += new System.EventHandler(this.menuBekAdd_Click);
            // 
            // menuBekChange
            // 
            this.menuBekChange.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Change;
            this.menuBekChange.Name = "menuBekChange";
            this.menuBekChange.Size = new System.Drawing.Size(124, 22);
            this.menuBekChange.Text = "&Change...";
            this.menuBekChange.Click += new System.EventHandler(this.menuBekChange_Click);
            // 
            // menuBekDelete
            // 
            this.menuBekDelete.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Subtract;
            this.menuBekDelete.Name = "menuBekDelete";
            this.menuBekDelete.Size = new System.Drawing.Size(124, 22);
            this.menuBekDelete.Text = "&Delete...";
            this.menuBekDelete.Click += new System.EventHandler(this.menuBekDelete_Click);
            // 
            // menuNumPassword
            // 
            this.menuNumPassword.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNumPasswordAdd,
            this.menuNumPasswordDelete});
            this.menuNumPassword.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Keyboard16;
            this.menuNumPassword.Name = "menuNumPassword";
            this.menuNumPassword.Size = new System.Drawing.Size(173, 22);
            this.menuNumPassword.Text = "&Numeric Password";
            // 
            // menuNumPasswordAdd
            // 
            this.menuNumPasswordAdd.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Add;
            this.menuNumPasswordAdd.Name = "menuNumPasswordAdd";
            this.menuNumPasswordAdd.Size = new System.Drawing.Size(116, 22);
            this.menuNumPasswordAdd.Text = "&Add...";
            this.menuNumPasswordAdd.Click += new System.EventHandler(this.menuNumPasswordAdd_Click);
            // 
            // menuNumPasswordDelete
            // 
            this.menuNumPasswordDelete.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Subtract;
            this.menuNumPasswordDelete.Name = "menuNumPasswordDelete";
            this.menuNumPasswordDelete.Size = new System.Drawing.Size(116, 22);
            this.menuNumPasswordDelete.Text = "&Delete...";
            this.menuNumPasswordDelete.Click += new System.EventHandler(this.menuNumPasswordDelete_Click);
            // 
            // menuSmartCards
            // 
            this.menuSmartCards.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSmartCardsAdd,
            this.menuSmartCardsDelete});
            this.menuSmartCards.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Cert16;
            this.menuSmartCards.Name = "menuSmartCards";
            this.menuSmartCards.Size = new System.Drawing.Size(173, 22);
            this.menuSmartCards.Text = "&Smart Cards";
            // 
            // menuSmartCardsAdd
            // 
            this.menuSmartCardsAdd.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Add;
            this.menuSmartCardsAdd.Name = "menuSmartCardsAdd";
            this.menuSmartCardsAdd.Size = new System.Drawing.Size(116, 22);
            this.menuSmartCardsAdd.Text = "&Add...";
            this.menuSmartCardsAdd.Click += new System.EventHandler(this.menuSmartCardsAdd_Click);
            // 
            // menuSmartCardsDelete
            // 
            this.menuSmartCardsDelete.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Subtract;
            this.menuSmartCardsDelete.Name = "menuSmartCardsDelete";
            this.menuSmartCardsDelete.Size = new System.Drawing.Size(116, 22);
            this.menuSmartCardsDelete.Text = "&Delete...";
            this.menuSmartCardsDelete.Click += new System.EventHandler(this.menuSmartCardsDelete_Click);
            // 
            // menuPassword
            // 
            this.menuPassword.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuPasswordAdd,
            this.menuPasswordChange,
            this.menuPasswordDelete});
            this.menuPassword.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Keyboard16;
            this.menuPassword.Name = "menuPassword";
            this.menuPassword.Size = new System.Drawing.Size(173, 22);
            this.menuPassword.Text = "&Password";
            // 
            // menuPasswordAdd
            // 
            this.menuPasswordAdd.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Add;
            this.menuPasswordAdd.Name = "menuPasswordAdd";
            this.menuPasswordAdd.Size = new System.Drawing.Size(124, 22);
            this.menuPasswordAdd.Text = "&Add...";
            this.menuPasswordAdd.Click += new System.EventHandler(this.menuPasswordAdd_Click);
            // 
            // menuPasswordChange
            // 
            this.menuPasswordChange.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Change;
            this.menuPasswordChange.Name = "menuPasswordChange";
            this.menuPasswordChange.Size = new System.Drawing.Size(124, 22);
            this.menuPasswordChange.Text = "&Change...";
            this.menuPasswordChange.Click += new System.EventHandler(this.menuPasswordChange_Click);
            // 
            // menuPasswordDelete
            // 
            this.menuPasswordDelete.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Subtract;
            this.menuPasswordDelete.Name = "menuPasswordDelete";
            this.menuPasswordDelete.Size = new System.Drawing.Size(124, 22);
            this.menuPasswordDelete.Text = "&Delete...";
            this.menuPasswordDelete.Click += new System.EventHandler(this.menuPasswordDelete_Click);
            // 
            // menuSuspend
            // 
            this.menuSuspend.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSuspendOff,
            this.menuSuspendOn});
            this.menuSuspend.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.ShieldYellow16;
            this.menuSuspend.Name = "menuSuspend";
            this.menuSuspend.Size = new System.Drawing.Size(173, 22);
            this.menuSuspend.Text = "Enable/&Suspend";
            // 
            // menuSuspendOff
            // 
            this.menuSuspendOff.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.ShieldGreen16;
            this.menuSuspendOff.Name = "menuSuspendOff";
            this.menuSuspendOff.Size = new System.Drawing.Size(119, 22);
            this.menuSuspendOff.Text = "&Enable";
            this.menuSuspendOff.Click += new System.EventHandler(this.menuSuspendOff_Click);
            // 
            // menuSuspendOn
            // 
            this.menuSuspendOn.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.ShieldRed16;
            this.menuSuspendOn.Name = "menuSuspendOn";
            this.menuSuspendOn.Size = new System.Drawing.Size(119, 22);
            this.menuSuspendOn.Text = "&Suspend";
            this.menuSuspendOn.Click += new System.EventHandler(this.menuSuspendOn_Click);
            // 
            // menuViewProtectors
            // 
            this.menuViewProtectors.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.hard_drive_repair_16;
            this.menuViewProtectors.Name = "menuViewProtectors";
            this.menuViewProtectors.Size = new System.Drawing.Size(173, 22);
            this.menuViewProtectors.Text = "&View...";
            this.menuViewProtectors.Click += new System.EventHandler(this.menuViewProtectors_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBar,
            this.toolStripStatusLabel3,
            this.encryptionProgress,
            this.toolStripStatusLabel1,
            this.volumeStatus,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 512);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(982, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusBar
            // 
            this.statusBar.AutoSize = false;
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(500, 17);
            this.statusBar.Text = "Welcome to Taryn BitLocker Manager for Home Server SMART!";
            this.statusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel3.Text = " ";
            // 
            // encryptionProgress
            // 
            this.encryptionProgress.Name = "encryptionProgress";
            this.encryptionProgress.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel1.Text = " ";
            // 
            // volumeStatus
            // 
            this.volumeStatus.AutoSize = false;
            this.volumeStatus.Name = "volumeStatus";
            this.volumeStatus.Size = new System.Drawing.Size(300, 17);
            this.volumeStatus.Text = "No Volume Selected";
            this.volumeStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(45, 17);
            this.toolStripStatusLabel2.Spring = true;
            this.toolStripStatusLabel2.Text = " ";
            // 
            // buttonQuickHelp
            // 
            this.buttonQuickHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonQuickHelp.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonQuickHelp.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Help48;
            this.buttonQuickHelp.Location = new System.Drawing.Point(846, 4);
            this.buttonQuickHelp.Name = "buttonQuickHelp";
            this.buttonQuickHelp.Size = new System.Drawing.Size(120, 72);
            this.buttonQuickHelp.TabIndex = 19;
            this.buttonQuickHelp.Text = "Documentation";
            this.buttonQuickHelp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonQuickHelp.UseVisualStyleBackColor = true;
            this.buttonQuickHelp.Click += new System.EventHandler(this.buttonQuickHelp_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRefresh.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.RefreshDisk24;
            this.buttonRefresh.Location = new System.Drawing.Point(403, 4);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(128, 32);
            this.buttonRefresh.TabIndex = 20;
            this.buttonRefresh.Text = "Refresh View";
            this.buttonRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // advancedMenu
            // 
            this.advancedMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRepairTpm,
            this.menuEscrowKeysToAd,
            this.menuUpgrade,
            this.menuDriveBender});
            this.advancedMenu.Name = "advancedMenu";
            this.advancedMenu.Size = new System.Drawing.Size(257, 114);
            // 
            // menuRepairTpm
            // 
            this.menuRepairTpm.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Unlock16;
            this.menuRepairTpm.Name = "menuRepairTpm";
            this.menuRepairTpm.Size = new System.Drawing.Size(256, 22);
            this.menuRepairTpm.Text = "&Repair/Change TPM Key...";
            this.menuRepairTpm.Click += new System.EventHandler(this.menuRepairTpm_Click);
            // 
            // menuEscrowKeysToAd
            // 
            this.menuEscrowKeysToAd.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.server_backup_16;
            this.menuEscrowKeysToAd.Name = "menuEscrowKeysToAd";
            this.menuEscrowKeysToAd.Size = new System.Drawing.Size(256, 22);
            this.menuEscrowKeysToAd.Text = "&Escrow Key(s) to Active Directory...";
            this.menuEscrowKeysToAd.Click += new System.EventHandler(this.menuEscrowKeysToAd_Click);
            // 
            // menuUpgrade
            // 
            this.menuUpgrade.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.ShieldMulti16;
            this.menuUpgrade.Name = "menuUpgrade";
            this.menuUpgrade.Size = new System.Drawing.Size(256, 22);
            this.menuUpgrade.Text = "&Upgrade BitLocker...";
            this.menuUpgrade.Click += new System.EventHandler(this.menuUpgrade_Click);
            // 
            // menuDriveBender
            // 
            this.menuDriveBender.Enabled = false;
            this.menuDriveBender.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.DriveBender16;
            this.menuDriveBender.Name = "menuDriveBender";
            this.menuDriveBender.Size = new System.Drawing.Size(256, 22);
            this.menuDriveBender.Text = "Drive &Bender and Pools...";
            this.menuDriveBender.Visible = false;
            this.menuDriveBender.Click += new System.EventHandler(this.menuDriveBender_Click);
            // 
            // menuViewGpo
            // 
            this.menuViewGpo.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Policy;
            this.menuViewGpo.Name = "menuViewGpo";
            this.menuViewGpo.Size = new System.Drawing.Size(224, 22);
            this.menuViewGpo.Text = "&View Group Policy Settings...";
            this.menuViewGpo.Click += new System.EventHandler(this.menuViewGpo_Click);
            // 
            // menuChangeVolumeLabel
            // 
            this.menuChangeVolumeLabel.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Change;
            this.menuChangeVolumeLabel.Name = "menuChangeVolumeLabel";
            this.menuChangeVolumeLabel.Size = new System.Drawing.Size(224, 22);
            this.menuChangeVolumeLabel.Text = "&Change Volume Label...";
            this.menuChangeVolumeLabel.Click += new System.EventHandler(this.menuChangeVolumeLabel_Click);
            // 
            // optionsMenu
            // 
            this.optionsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuPause,
            this.menuResume,
            this.menuViewGpo,
            this.menuChangeVolumeLabel});
            this.optionsMenu.Name = "optionsMenu";
            this.optionsMenu.Size = new System.Drawing.Size(225, 114);
            // 
            // menuPause
            // 
            this.menuPause.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Pause;
            this.menuPause.Name = "menuPause";
            this.menuPause.Size = new System.Drawing.Size(224, 22);
            this.menuPause.Text = "&Pause Encryption";
            this.menuPause.Click += new System.EventHandler(this.menuPause_Click);
            // 
            // menuResume
            // 
            this.menuResume.Image = global::DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.Properties.Resources.Resume;
            this.menuResume.Name = "menuResume";
            this.menuResume.Size = new System.Drawing.Size(224, 22);
            this.menuResume.Text = "&Resume Encryption";
            this.menuResume.Click += new System.EventHandler(this.menuResume_Click);
            // 
            // buttonOptions
            // 
            this.buttonOptions.AlwaysDropDown = true;
            this.buttonOptions.ClickedImage = "Clicked";
            this.buttonOptions.ContextMenuStrip = this.optionsMenu;
            this.buttonOptions.DisabledImage = "Disabled";
            this.buttonOptions.FocusedImage = "Focused";
            this.buttonOptions.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOptions.HoverImage = "Hover";
            this.buttonOptions.ImageKey = "Normal";
            this.buttonOptions.Location = new System.Drawing.Point(537, 4);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.NormalImage = "Normal";
            this.buttonOptions.Size = new System.Drawing.Size(128, 32);
            this.buttonOptions.TabIndex = 22;
            this.buttonOptions.Text = "Options";
            this.buttonOptions.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.buttonOptions.UseVisualStyleBackColor = true;
            // 
            // buttonAdvanced
            // 
            this.buttonAdvanced.AlwaysDropDown = true;
            this.buttonAdvanced.ClickedImage = "Clicked";
            this.buttonAdvanced.ContextMenuStrip = this.advancedMenu;
            this.buttonAdvanced.DisabledImage = "Disabled";
            this.buttonAdvanced.FocusedImage = "Focused";
            this.buttonAdvanced.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAdvanced.HoverImage = "Hover";
            this.buttonAdvanced.ImageKey = "Normal";
            this.buttonAdvanced.Location = new System.Drawing.Point(537, 44);
            this.buttonAdvanced.Name = "buttonAdvanced";
            this.buttonAdvanced.NormalImage = "Normal";
            this.buttonAdvanced.Size = new System.Drawing.Size(128, 32);
            this.buttonAdvanced.TabIndex = 21;
            this.buttonAdvanced.Text = "Advanced";
            this.buttonAdvanced.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.buttonAdvanced.UseVisualStyleBackColor = true;
            // 
            // lvEncryptableVolumes
            // 
            this.lvEncryptableVolumes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvEncryptableVolumes.BitmapList = null;
            this.lvEncryptableVolumes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvEncryptableVolumes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
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
            this.columnHeader11,
            this.columnHeader12});
            this.lvEncryptableVolumes.FullRowSelect = true;
            this.lvEncryptableVolumes.Location = new System.Drawing.Point(17, 92);
            this.lvEncryptableVolumes.MinimumSize = new System.Drawing.Size(500, 200);
            this.lvEncryptableVolumes.MultiSelect = false;
            this.lvEncryptableVolumes.Name = "lvEncryptableVolumes";
            this.lvEncryptableVolumes.OwnerDraw = true;
            this.lvEncryptableVolumes.Size = new System.Drawing.Size(949, 218);
            this.lvEncryptableVolumes.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvEncryptableVolumes.TabIndex = 6;
            this.lvEncryptableVolumes.UseCompatibleStateImageBehavior = false;
            this.lvEncryptableVolumes.View = System.Windows.Forms.View.Details;
            this.lvEncryptableVolumes.SelectedIndexChanged += new System.EventHandler(this.lvEncryptableVolumes_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Vol";
            this.columnHeader1.Width = 50;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Label";
            this.columnHeader2.Width = 155;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "File System";
            this.columnHeader3.Width = 88;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Capacity/Free (GB)";
            this.columnHeader4.Width = 136;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Encryption Status";
            this.columnHeader5.Width = 131;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Locking";
            this.columnHeader6.Width = 0;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Key Protectors";
            this.columnHeader7.Width = 100;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Encryption %";
            this.columnHeader8.Width = 96;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Method";
            this.columnHeader9.Width = 97;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Auto Unlock";
            this.columnHeader10.Width = 94;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Interface";
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Volume Type";
            this.columnHeader12.Width = 93;
            // 
            // buttonProtectors
            // 
            this.buttonProtectors.AlwaysDropDown = true;
            this.buttonProtectors.ClickedImage = "Clicked";
            this.buttonProtectors.ContextMenuStrip = this.protectorsMenu;
            this.buttonProtectors.DisabledImage = "Disabled";
            this.buttonProtectors.FocusedImage = "Focused";
            this.buttonProtectors.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonProtectors.HoverImage = "Hover";
            this.buttonProtectors.ImageKey = "Normal";
            this.buttonProtectors.Location = new System.Drawing.Point(403, 44);
            this.buttonProtectors.Name = "buttonProtectors";
            this.buttonProtectors.NormalImage = "Normal";
            this.buttonProtectors.Size = new System.Drawing.Size(128, 32);
            this.buttonProtectors.TabIndex = 5;
            this.buttonProtectors.Text = "Protectors";
            this.buttonProtectors.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.buttonProtectors.UseVisualStyleBackColor = true;
            // 
            // BitLockerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonOptions);
            this.Controls.Add(this.buttonAdvanced);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonQuickHelp);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.lvEncryptableVolumes);
            this.Controls.Add(this.buttonProtectors);
            this.Controls.Add(this.buttonUnlock);
            this.Controls.Add(this.buttonLock);
            this.Controls.Add(this.buttonDecrypt);
            this.Controls.Add(this.buttonEncrypt);
            this.MaximumSize = new System.Drawing.Size(65535, 65535);
            this.MinimumSize = new System.Drawing.Size(770, 350);
            this.Name = "BitLockerControl";
            this.Size = new System.Drawing.Size(982, 534);
            this.Load += new System.EventHandler(this.BitLockerControl_Load);
            this.protectorsMenu.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.advancedMenu.ResumeLayout(false);
            this.optionsMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonEncrypt;
        private System.Windows.Forms.Button buttonDecrypt;
        private System.Windows.Forms.Button buttonLock;
        private System.Windows.Forms.Button buttonUnlock;
        private WSSControls.BelovedComponents.SplitButton buttonProtectors;
        private System.Windows.Forms.ContextMenuStrip protectorsMenu;
        private FancyListView lvEncryptableVolumes;
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
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusBar;
        private System.Windows.Forms.ToolStripProgressBar encryptionProgress;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel volumeStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.Button buttonQuickHelp;
        private System.Windows.Forms.Button buttonRefresh;
        private SplitButton buttonAdvanced;
        private System.Windows.Forms.ContextMenuStrip advancedMenu;
        private System.Windows.Forms.ToolStripMenuItem menuAutoUnlock;
        private System.Windows.Forms.ToolStripMenuItem menuBek;
        private System.Windows.Forms.ToolStripMenuItem menuNumPassword;
        private System.Windows.Forms.ToolStripMenuItem menuSmartCards;
        private System.Windows.Forms.ToolStripMenuItem menuPassword;
        private System.Windows.Forms.ToolStripMenuItem menuSuspend;
        private System.Windows.Forms.ToolStripMenuItem menuViewProtectors;
        private System.Windows.Forms.ToolStripMenuItem menuAutoUnlockEnable;
        private System.Windows.Forms.ToolStripMenuItem menuAutoUnlockDisable;
        private System.Windows.Forms.ToolStripMenuItem menuBekAdd;
        private System.Windows.Forms.ToolStripMenuItem menuBekDelete;
        private System.Windows.Forms.ToolStripMenuItem menuNumPasswordAdd;
        private System.Windows.Forms.ToolStripMenuItem menuNumPasswordDelete;
        private System.Windows.Forms.ToolStripMenuItem menuSmartCardsAdd;
        private System.Windows.Forms.ToolStripMenuItem menuSmartCardsDelete;
        private System.Windows.Forms.ToolStripMenuItem menuPasswordAdd;
        private System.Windows.Forms.ToolStripMenuItem menuPasswordChange;
        private System.Windows.Forms.ToolStripMenuItem menuPasswordDelete;
        private System.Windows.Forms.ToolStripMenuItem menuSuspendOff;
        private System.Windows.Forms.ToolStripMenuItem menuSuspendOn;
        private System.Windows.Forms.ToolStripMenuItem menuViewGpo;
        private System.Windows.Forms.ToolStripMenuItem menuRepairTpm;
        private System.Windows.Forms.ToolStripMenuItem menuEscrowKeysToAd;
        private System.Windows.Forms.ToolStripMenuItem menuChangeVolumeLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private SplitButton buttonOptions;
        private System.Windows.Forms.ToolStripMenuItem menuBekChange;
        private System.Windows.Forms.ContextMenuStrip optionsMenu;
        private System.Windows.Forms.ToolStripMenuItem menuPause;
        private System.Windows.Forms.ToolStripMenuItem menuResume;
        private System.Windows.Forms.ToolStripMenuItem menuUpgrade;
        private System.Windows.Forms.ToolStripMenuItem menuDriveBender;
    }
}
