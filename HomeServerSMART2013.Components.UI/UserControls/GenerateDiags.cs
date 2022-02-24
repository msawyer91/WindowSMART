using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class GenerateDiags : Form
    {
        private String diagnosticDetails;
        private Debugging.ReapSystemData reaper;
        private delegate void Appriser(int percent, String message, bool exeunt);
        private Appriser dlgAppriser;

        public GenerateDiags(bool isWss, bool isWin8)
        {
            InitializeComponent();

            reaper = new Debugging.ReapSystemData(isWss, isWin8);
            dlgAppriser = new Appriser(DApprise);
        }

        public String DiagnosticDetails
        {
            get
            {
                return diagnosticDetails;
            }
        }

        private void GenerateDiags_Load(object sender, EventArgs e)
        {
            Thread reaperThread = new Thread(new ThreadStart(ReapDetails));
            reaperThread.Name = "Diagnostic Reaper";
            reaperThread.Start();
        }

        private void ReapDetails()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            try
            {
                Apprise(1, "Starting", false);
                Thread.Sleep(1500);
                Apprise(20, "Collecting general machine information", false);
                sb.AppendLine(reaper.Reap(true));
                Thread.Sleep(2000);
                Apprise(30, "Detecting installed disks", false);
                sb.AppendLine(reaper.DeepReapGeneralWmiQuery());
                Thread.Sleep(1000);
                Apprise(60, "Detecting Silicon Image devices", false);
                sb.AppendLine(reaper.DeepReapSiIQuery());
                Thread.Sleep(1000);
                Apprise(75, "Detecting USB devices", false);
                sb.AppendLine(reaper.DeepReapUsbInfo());
                Thread.Sleep(1000);
                Apprise(95, "Compiling results", false);
                diagnosticDetails = sb.ToString();
                Thread.Sleep(1000);
                Apprise(100, "Finished", true);
            }
            catch (Exception ex)
            {
                Apprise(100, "Failed! " + ex.Message, false);
                Thread.Sleep(5000);
                diagnosticDetails = sb.ToString();
                Apprise(100, "Finished", true);
            }
        }

        private void Apprise(int percent, String message, bool exeunt)
        {
            this.Invoke(dlgAppriser, new object[] { percent, message, exeunt });
        }

        private void DApprise(int percent, String message, bool exeunt)
        {
            progressBar1.Value = percent;
            label2.Text = message;
            if (exeunt)
            {
                this.Close();
            }
        }
    }
}
