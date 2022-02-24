namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    partial class ConfigureSilvermist
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureSilvermist));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonMoveDown = new System.Windows.Forms.Button();
            this.buttonMoveUp = new System.Windows.Forms.Button();
            this.buttonRemDir = new System.Windows.Forms.Button();
            this.buttonAddDir = new System.Windows.Forms.Button();
            this.listBoxInclude = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonRemoveExclusion = new System.Windows.Forms.Button();
            this.buttonAddExclusion = new System.Windows.Forms.Button();
            this.listBoxItemsToExclude = new System.Windows.Forms.ListBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(729, 44);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonMoveDown);
            this.groupBox1.Controls.Add(this.buttonMoveUp);
            this.groupBox1.Controls.Add(this.buttonRemDir);
            this.groupBox1.Controls.Add(this.buttonAddDir);
            this.groupBox1.Controls.Add(this.listBoxInclude);
            this.groupBox1.Location = new System.Drawing.Point(15, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(726, 165);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Folders to Include";
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.Enabled = false;
            this.buttonMoveDown.Location = new System.Drawing.Point(628, 125);
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(83, 27);
            this.buttonMoveDown.TabIndex = 4;
            this.buttonMoveDown.Text = "Move Down";
            this.buttonMoveDown.UseVisualStyleBackColor = true;
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.Enabled = false;
            this.buttonMoveUp.Location = new System.Drawing.Point(628, 92);
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(83, 27);
            this.buttonMoveUp.TabIndex = 3;
            this.buttonMoveUp.Text = "Move Up";
            this.buttonMoveUp.UseVisualStyleBackColor = true;
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // buttonRemDir
            // 
            this.buttonRemDir.Enabled = false;
            this.buttonRemDir.Location = new System.Drawing.Point(628, 52);
            this.buttonRemDir.Name = "buttonRemDir";
            this.buttonRemDir.Size = new System.Drawing.Size(83, 27);
            this.buttonRemDir.TabIndex = 2;
            this.buttonRemDir.Text = "Remove";
            this.buttonRemDir.UseVisualStyleBackColor = true;
            this.buttonRemDir.Click += new System.EventHandler(this.buttonRemDir_Click);
            // 
            // buttonAddDir
            // 
            this.buttonAddDir.Location = new System.Drawing.Point(628, 19);
            this.buttonAddDir.Name = "buttonAddDir";
            this.buttonAddDir.Size = new System.Drawing.Size(83, 27);
            this.buttonAddDir.TabIndex = 1;
            this.buttonAddDir.Text = "Add...";
            this.buttonAddDir.UseVisualStyleBackColor = true;
            this.buttonAddDir.Click += new System.EventHandler(this.buttonAddDir_Click);
            // 
            // listBoxInclude
            // 
            this.listBoxInclude.FormattingEnabled = true;
            this.listBoxInclude.Location = new System.Drawing.Point(15, 18);
            this.listBoxInclude.Name = "listBoxInclude";
            this.listBoxInclude.Size = new System.Drawing.Size(597, 134);
            this.listBoxInclude.TabIndex = 0;
            this.listBoxInclude.SelectedIndexChanged += new System.EventHandler(this.listBoxInclude_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonRemoveExclusion);
            this.groupBox2.Controls.Add(this.buttonAddExclusion);
            this.groupBox2.Controls.Add(this.listBoxItemsToExclude);
            this.groupBox2.Location = new System.Drawing.Point(15, 230);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(726, 125);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Item(s) to Exclude";
            // 
            // buttonRemoveExclusion
            // 
            this.buttonRemoveExclusion.Enabled = false;
            this.buttonRemoveExclusion.Location = new System.Drawing.Point(628, 52);
            this.buttonRemoveExclusion.Name = "buttonRemoveExclusion";
            this.buttonRemoveExclusion.Size = new System.Drawing.Size(83, 27);
            this.buttonRemoveExclusion.TabIndex = 2;
            this.buttonRemoveExclusion.Text = "Remove";
            this.buttonRemoveExclusion.UseVisualStyleBackColor = true;
            // 
            // buttonAddExclusion
            // 
            this.buttonAddExclusion.Location = new System.Drawing.Point(628, 19);
            this.buttonAddExclusion.Name = "buttonAddExclusion";
            this.buttonAddExclusion.Size = new System.Drawing.Size(83, 27);
            this.buttonAddExclusion.TabIndex = 1;
            this.buttonAddExclusion.Text = "Add...";
            this.buttonAddExclusion.UseVisualStyleBackColor = true;
            this.buttonAddExclusion.Click += new System.EventHandler(this.buttonAddExclusion_Click);
            // 
            // listBoxItemsToExclude
            // 
            this.listBoxItemsToExclude.FormattingEnabled = true;
            this.listBoxItemsToExclude.Location = new System.Drawing.Point(15, 18);
            this.listBoxItemsToExclude.Name = "listBoxItemsToExclude";
            this.listBoxItemsToExclude.Size = new System.Drawing.Size(597, 95);
            this.listBoxItemsToExclude.TabIndex = 0;
            this.listBoxItemsToExclude.SelectedIndexChanged += new System.EventHandler(this.listBoxItemsToExclude_SelectedIndexChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(276, 368);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(83, 27);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(388, 368);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(83, 27);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // ConfigureSilvermist
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(753, 407);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigureSilvermist";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Emergency Backup";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonMoveDown;
        private System.Windows.Forms.Button buttonMoveUp;
        private System.Windows.Forms.Button buttonRemDir;
        private System.Windows.Forms.Button buttonAddDir;
        private System.Windows.Forms.ListBox listBoxInclude;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonRemoveExclusion;
        private System.Windows.Forms.Button buttonAddExclusion;
        private System.Windows.Forms.ListBox listBoxItemsToExclude;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}