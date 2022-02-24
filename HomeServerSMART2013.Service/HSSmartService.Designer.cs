namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Service
{
    partial class HSSmartService
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
            this.attributeRefresh = new System.Timers.Timer();
            this.updateGanderizer = new System.Timers.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.attributeRefresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updateGanderizer)).BeginInit();
            // 
            // attributeRefresh
            // 
            this.attributeRefresh.Interval = 900000D;
            this.attributeRefresh.Elapsed += new System.Timers.ElapsedEventHandler(this.attributeRefresh_Elapsed);
            // 
            // updateGanderizer
            // 
            this.updateGanderizer.Interval = 7200000D;
            this.updateGanderizer.Elapsed += new System.Timers.ElapsedEventHandler(this.updateGanderizer_Elapsed);
            // 
            // HSSmartService
            // 
            this.CanHandlePowerEvent = true;
            this.CanShutdown = true;
            this.ServiceName = "Home Server SMART Service";
            ((System.ComponentModel.ISupportInitialize)(this.attributeRefresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updateGanderizer)).EndInit();

        }

        #endregion

        private System.Timers.Timer attributeRefresh;
        private System.Timers.Timer updateGanderizer;
    }
}
