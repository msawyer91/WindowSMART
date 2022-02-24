using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Debugging
{
    public partial class DebugDialogue : Form
    {
        private Exception fail;
        private String explanation;
        private String expectedVersion;
        private String componentsVersion;
        private String uiComponentsVersion;
        private String uiVersion;
        private String serviceVersion;
        private String trayVersion;
        private int problemCount;
        private bool isWindowsServerSolutions;

        public DebugDialogue(bool isWss)
        {
            InitializeComponent();
            fail = null;
            explanation = String.Empty;
            expectedVersion = componentsVersion = uiVersion = serviceVersion = trayVersion = "0.0.0.0";
            problemCount = 0;
            isWindowsServerSolutions = isWss;
        }

        public DebugDialogue(Exception ex, String message, bool isWss)
        {
            InitializeComponent();
            fail = ex;
            explanation = message;
            expectedVersion = componentsVersion = uiVersion = serviceVersion = trayVersion = "0.0.0.0";
            problemCount = 0;
            isWindowsServerSolutions = isWss;
        }

        private void DebugDialogue_Load(object sender, EventArgs e)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            expectedVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (fail == null)
            {
            }
            else
            {
                sb.AppendLine("Bug Report invoked with error details -- error details will appear below.");
                sb.AppendLine("");
                sb.Append(DisplayErrorDetails());
            }
            ReapSystemData reaper = new ReapSystemData(isWindowsServerSolutions, Utilities.Utility.IsSystemWindows8());
            sb.Append(reaper.Reap(false));

            if (problemCount == 0)
            {
                sb.AppendLine("");
                sb.AppendLine("The self-test did not detect any problems with the installation; Home Server SMART appears to be installed correctly.");
            }
            else
            {
                sb.AppendLine("");
                sb.AppendLine("The self-test detected " + problemCount.ToString() + " serious problem(s) with the Home Server SMART installation. The problem you are " +
                    "encountering may be the result of a bad installation. You may want to consider uninstalling and reinstalling Home Server SMART and trying the " +
                    "desired operation again.");
            }
            textBox1.Text = sb.ToString();

            textBox1.Select(0, 0);
            buttonCopy.Focus();
        }

        private String DisplayErrorDetails()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("*** Begin Exception Diagnostic Details ***");
            sb.AppendLine("");
            sb.AppendLine("Explanation/Message: " + explanation);
            sb.AppendLine("");
            sb.AppendLine("Exception Message: " + fail.Message);
            sb.AppendLine("");
            sb.AppendLine("Stack Trace:");
            sb.AppendLine(fail.StackTrace);
            sb.AppendLine("");
            sb.AppendLine("*** End Exception Diagnostic Details ***");
            sb.AppendLine("");

            return sb.ToString();
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(textBox1.Text);
            System.Windows.Forms.MessageBox.Show("Error information has been copied to the clipboard. If you want to submit a bug report, you can paste this " +
                "information into the report. If you want to submit a bug report now, you can do one of the following:\n\n" +
                "     1. Visit https://www.dojonorthsoftware.net/bugtraq and submit a bug for HSS 24/7.\n" +
                "     2. Send an email to bugtraq@dojonorthsoftware.net\n\n" +
                "To protect your privacy, none of this information is transmitted to Dojo North Software. If you choose to share this information " +
                "in your bug report, you are free to do so. You may omit any of the copied details from your report if you have any concerns " +
                "that they reveal personal information. Dojo North Software uses this information solely for tracking and fixing bugs, and you " +
                "may be contacted for more information or to help verify a fix.", "Copy Error Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utilities.Utility.LaunchBrowser(Properties.Resources.HssBugTrackUrl);
        }
    }
}
