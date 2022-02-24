using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WSSControls.BelovedComponents;
using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class EpicFail : Form
    {
        private Exception epicFail;
        private String extraMessage;
        private String troubleshootingSteps;
        private DateTime whenDetected;
        private bool isWindowsServerSolutions;

        public EpicFail()
        {
            throw new NotImplementedException("This constructor is not used.");
        }

        public EpicFail(Exception ex, String steps, bool isWss)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.EpicFail.EpicFail");
            SiAuto.Main.LogException(ex);
            SiAuto.Main.LogString("steps", steps);
            SiAuto.Main.LogWarning("No message provided.");
            InitializeComponent();
            epicFail = ex;
            extraMessage = String.Empty;
            troubleshootingSteps = steps;
            pictureBoxDunce.Image = Properties.Resources.dunce_204x400;
            whenDetected = DateTime.Now;
            isWindowsServerSolutions = isWss;
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.EpicFail.EpicFail");
        }

        public EpicFail(Exception ex, String message, String steps, bool isWss)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.EpicFail.EpicFail");
            SiAuto.Main.LogException(ex);
            SiAuto.Main.LogString("message", message);
            SiAuto.Main.LogString("steps", steps);
            InitializeComponent();
            epicFail = ex;
            extraMessage = message;
            troubleshootingSteps = steps;
            pictureBoxDunce.Image = Properties.Resources.dunce_204x400;
            whenDetected = DateTime.Now;
            isWindowsServerSolutions = isWss;
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.EpicFail.EpicFail");
        }

        private void EpicFail_Load(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.EpicFail.EpicFail_Load");
            textBoxMessage.Text = (String.IsNullOrEmpty(extraMessage) ? epicFail.Message : extraMessage + " " + epicFail.Message);
            textBoxTroubleshoot.Text = troubleshootingSteps;
            textBoxStackTrace.Text = epicFail.ToString();
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.EpicFail.EpicFail_Load");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.EpicFail.button2_Click");
            this.Close();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.EpicFail.button2_Click");
        }

        private void EpicFail_Shown(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.EpicFail.EpicFail_Shown");
            textBoxMessage.DeselectAll();
            buttonClose.Focus();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.EpicFail.EpicFail_Shown");
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.EpicFail.buttonCopy_Click");
            this.TopMost = false;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine((isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart) + " - Epic FAIL Report");
            sb.AppendLine("Exception detected " + whenDetected.ToShortDateString() + " " + whenDetected.ToLongTimeString());
            sb.AppendLine("UTC time " + whenDetected.ToUniversalTime().ToShortDateString() + " " + whenDetected.ToUniversalTime().ToLongTimeString());
            sb.AppendLine("");
            sb.AppendLine("ERROR MESSAGE / POSSIBLE CAUSES");
            sb.AppendLine(textBoxMessage.Text);
            sb.AppendLine("");
            sb.AppendLine("TROUBLESHOOTING STEPS (if applicable)");
            sb.AppendLine(textBoxTroubleshoot.Text);
            sb.AppendLine("");
            sb.AppendLine("TECHNICAL / STACK TRACE");
            sb.AppendLine(textBoxStackTrace.Text);
            Clipboard.SetDataObject(sb.ToString());
            QMessageBox.Show("Error information has been copied to the clipboard. If you want to submit a bug report, you can paste this " +
                "information into the report. If you want to submit a bug report now, you can do one of the following:\n\n" +
                "     1. Visit https://www.dojonorthsoftware.net/bugtraq and submit a bug for HSS 24/7.\n" +
                "     2. Send an email to bugtraq@dojonorthsoftware.net\n\n" +
                "If you want to collect some additional information before sending a report, you can click the Bug Report button in the " +
                "error dialogue. This will run a small self-test to collect some additional information that can be used in a bug report " +
                "submission. No data is sent to Dojo North Software in this process. You can provide as much or as little of this " +
                "information in your report as you'd like.", "Copy Error Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.EpicFail.buttonCopy_Click");
        }

        private void buttonBugReport_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.EpicFail.buttonBugReport_Click");
            this.TopMost = false;
            DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Debugging.DebugDialogue debugger =
                new Components.Debugging.DebugDialogue(epicFail, textBoxMessage.Text, isWindowsServerSolutions);
            debugger.ShowDialog();
            this.Close();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.EpicFail.buttonBugReport_Click");
        }
    }
}
