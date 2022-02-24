namespace PrototypeEventWireup
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonUnregister = new System.Windows.Forms.Button();
            this.buttonRegister = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbDiskModel = new System.Windows.Forms.ComboBox();
            this.numericDiskNumber = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.numericTemperature = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numericProblemCount = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.tbSource = new System.Windows.Forms.TextBox();
            this.tbSeverity = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbEvent = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbEvent = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericDiskNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTemperature)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericProblemCount)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(593, 45);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonUnregister);
            this.groupBox1.Controls.Add(this.buttonRegister);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(15, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(590, 68);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Event Source Registration";
            // 
            // buttonUnregister
            // 
            this.buttonUnregister.Enabled = false;
            this.buttonUnregister.Location = new System.Drawing.Point(430, 25);
            this.buttonUnregister.Name = "buttonUnregister";
            this.buttonUnregister.Size = new System.Drawing.Size(109, 23);
            this.buttonUnregister.TabIndex = 3;
            this.buttonUnregister.Text = "Unregister Event";
            this.buttonUnregister.UseVisualStyleBackColor = true;
            this.buttonUnregister.Click += new System.EventHandler(this.buttonUnregister_Click);
            // 
            // buttonRegister
            // 
            this.buttonRegister.Enabled = false;
            this.buttonRegister.Location = new System.Drawing.Point(303, 25);
            this.buttonRegister.Name = "buttonRegister";
            this.buttonRegister.Size = new System.Drawing.Size(109, 23);
            this.buttonRegister.TabIndex = 2;
            this.buttonRegister.Text = "Register Event";
            this.buttonRegister.UseVisualStyleBackColor = true;
            this.buttonRegister.Click += new System.EventHandler(this.buttonRegister_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(18, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(244, 34);
            this.label2.TabIndex = 2;
            this.label2.Text = "To log events, the \"WindowSMART-W\" and \"WindowSMART-E\" event sources must be registered.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbDiskModel);
            this.groupBox2.Controls.Add(this.numericDiskNumber);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.numericTemperature);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.numericProblemCount);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.tbSource);
            this.groupBox2.Controls.Add(this.tbSeverity);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.cbEvent);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(15, 141);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(590, 143);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Simulated Event";
            // 
            // cbDiskModel
            // 
            this.cbDiskModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDiskModel.FormattingEnabled = true;
            this.cbDiskModel.Items.AddRange(new object[] {
            "WDC WD32200BEKT-60PVMT0",
            "WDC WD10 JPVT-16A1YT0",
            "ST31000528AS",
            "WDC WD20EARX-00PASB0",
            "ST2000DL003-9VT166",
            "INTEL SSDSC2CW180A3",
            "OCZ-VERTEX4",
            "Hitachi HDS721010CLA332",
            "Hitachi HDS722020ALA330",
            "SAMSUNG 830 Series",
            "WD My Passport0730 USB Device"});
            this.cbDiskModel.Location = new System.Drawing.Point(310, 101);
            this.cbDiskModel.Name = "cbDiskModel";
            this.cbDiskModel.Size = new System.Drawing.Size(183, 21);
            this.cbDiskModel.TabIndex = 6;
            this.cbDiskModel.SelectedIndexChanged += new System.EventHandler(this.cbDiskModel_SelectedIndexChanged);
            // 
            // numericDiskNumber
            // 
            this.numericDiskNumber.Location = new System.Drawing.Point(114, 102);
            this.numericDiskNumber.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericDiskNumber.Name = "numericDiskNumber";
            this.numericDiskNumber.Size = new System.Drawing.Size(73, 20);
            this.numericDiskNumber.TabIndex = 14;
            this.numericDiskNumber.ValueChanged += new System.EventHandler(this.numericDiskNumber_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(214, 104);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Disk Model";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 104);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Disk Number";
            // 
            // numericTemperature
            // 
            this.numericTemperature.Location = new System.Drawing.Point(310, 72);
            this.numericTemperature.Maximum = new decimal(new int[] {
            75,
            0,
            0,
            0});
            this.numericTemperature.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericTemperature.Name = "numericTemperature";
            this.numericTemperature.Size = new System.Drawing.Size(73, 20);
            this.numericTemperature.TabIndex = 12;
            this.numericTemperature.Value = new decimal(new int[] {
            38,
            0,
            0,
            0});
            this.numericTemperature.ValueChanged += new System.EventHandler(this.numericTemperature_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(214, 74);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Temperature (C)";
            // 
            // numericProblemCount
            // 
            this.numericProblemCount.Location = new System.Drawing.Point(114, 72);
            this.numericProblemCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericProblemCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericProblemCount.Name = "numericProblemCount";
            this.numericProblemCount.Size = new System.Drawing.Size(73, 20);
            this.numericProblemCount.TabIndex = 10;
            this.numericProblemCount.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericProblemCount.ValueChanged += new System.EventHandler(this.numericProblemCount_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 74);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Problem Count";
            // 
            // tbSource
            // 
            this.tbSource.Location = new System.Drawing.Point(508, 31);
            this.tbSource.Name = "tbSource";
            this.tbSource.ReadOnly = true;
            this.tbSource.Size = new System.Drawing.Size(71, 20);
            this.tbSource.TabIndex = 8;
            this.tbSource.Text = "WindowSMART-E";
            // 
            // tbSeverity
            // 
            this.tbSeverity.Location = new System.Drawing.Point(371, 31);
            this.tbSeverity.Name = "tbSeverity";
            this.tbSeverity.ReadOnly = true;
            this.tbSeverity.Size = new System.Drawing.Size(71, 20);
            this.tbSeverity.TabIndex = 6;
            this.tbSeverity.Text = "Information";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(457, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Source";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(320, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Severity";
            // 
            // cbEvent
            // 
            this.cbEvent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEvent.FormattingEnabled = true;
            this.cbEvent.Items.AddRange(new object[] {
            "----- Disk Health Events -----",
            "53820 Disk Temp Critical",
            "53821 Disk Temp Overheat",
            "53822 Disk Temp Hot",
            "53823 Disk Temp Warm",
            "53824 Disk Health Critical",
            "53825 Disk Health Questionable",
            "53826 Disk Health Geriatric",
            "53830 Bad Sectors - Critical",
            "53831 Bad Sectors - Warning",
            "53832 End-to-End Errors - Critical",
            "53833 End-to-End Errors - Warning",
            "53834 Reallocations - Critical",
            "53835 Reallocations - Warning",
            "53836 Pending Sectors - Critical",
            "53837 Pending Sectors - Warning",
            "53838 Offline Sectors - Critical",
            "53839 Offline Sectors - Warning",
            "53840 Spin Retry - Critical",
            "53841 Spin Retry - Warning",
            "53842 CRC Errors - Warning",
            "53844 JMicron SSD - Critical",
            "53845 JMicron SSD - Warning",
            "53846 Samsung SSD - Critical",
            "53847 Samsung SSD - Warning",
            "53848 SSD Life Curve - Warning",
            "53849 SSD Wearout < 10% - Critical",
            "53850 SSD Wearout < 30% - Warning",
            "53851 Health Alert Removed",
            "53852 Disk Health Unknown",
            "53853 TEC Threshold Exceeded",
            "53854 TEC Threshold Met",
            "53855 Overheat >= 3 Consec; Shutting Down",
            "53856 Overheat >= 3 Consec; No Shutdown",
            "----- Operational Events -----",
            "53860 Disk Poll Starting",
            "53861 Disk Poll Completing",
            "53862 Failed to Validate Logfile Path",
            "53863 Failed to Validate Default Logfile Path",
            "53864 Logfile Path Inaccessible",
            "53865 Product Update Available",
            "53866 Thermal Shutdown Ordered",
            "53867 Error Releasing Indiv Record",
            "53868 Error Releasing Resources",
            "53869 Cannot Back Up Target Failing",
            "----- Serious Errors -----",
            "53880 Exception During Auto-Polling",
            "53881 Exception During Polling",
            "53882 Service Failed to Initialize",
            "53883 Service Main Thread Crashed",
            "53884 Assessment of Indiv Disk Failed",
            "53885 Error Clearing Stale Disk List",
            "53886 Update Check Failed",
            "53887 UNC Drive Map Failed",
            "53888 Emergency Backup Cannot Map UNC",
            "53889 Failed to Perform Thermal Shutdown",
            "53890 Thermal Assessment Failed",
            "53891 Exception Parsing SMART Data",
            "53892 UI Error Cannot Bind Service",
            "53893 Backup Cannot Read Source List",
            "53894 Cannot Update Hot TEC Flag",
            "53895 Cannot Update General TEC Flag",
            "53896 Emergency Backup TEC Assessment Failed"});
            this.cbEvent.Location = new System.Drawing.Point(114, 31);
            this.cbEvent.Name = "cbEvent";
            this.cbEvent.Size = new System.Drawing.Size(183, 21);
            this.cbEvent.TabIndex = 4;
            this.cbEvent.SelectedIndexChanged += new System.EventHandler(this.cbEvent_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Event to Simulate";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbEvent);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Location = new System.Drawing.Point(15, 290);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(590, 150);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sample Event Text";
            // 
            // tbEvent
            // 
            this.tbEvent.Location = new System.Drawing.Point(21, 51);
            this.tbEvent.Multiline = true;
            this.tbEvent.Name = "tbEvent";
            this.tbEvent.ReadOnly = true;
            this.tbEvent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbEvent.Size = new System.Drawing.Size(558, 90);
            this.tbEvent.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 26);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(497, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "The below Error text will be logged under event source WindowSMART-E in the Applicati" +
    "on event log with ID x.";
            // 
            // buttonCreate
            // 
            this.buttonCreate.Location = new System.Drawing.Point(188, 446);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(110, 31);
            this.buttonCreate.TabIndex = 4;
            this.buttonCreate.Text = "Create Event";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(318, 446);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(110, 31);
            this.buttonExit.TabIndex = 5;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 485);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WindowSMART-to-SCOM Test Harness";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericDiskNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTemperature)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericProblemCount)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonUnregister;
        private System.Windows.Forms.Button buttonRegister;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbSource;
        private System.Windows.Forms.TextBox tbSeverity;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbEvent;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbDiskModel;
        private System.Windows.Forms.NumericUpDown numericDiskNumber;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericTemperature;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericProblemCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tbEvent;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Button buttonExit;
    }
}

