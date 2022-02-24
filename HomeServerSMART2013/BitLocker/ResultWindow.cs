using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public partial class ResultWindow : Form
    {
        // Printing-specific Items
        //private int checkPrint;
        PrintDocument printDoc;

        bool saveSuccess = false;
        RichTextBoxEx rtb;

        public ResultWindow(String results)
        {
            InitializeComponent();
            textBox1.Text = results;
            rtb = new RichTextBoxEx();
            rtb.Text = results;
        }

        public ResultWindow(String results, String titleText)
        {
            InitializeComponent();
            textBox1.Text = results;
            label1.Text = titleText;
            rtb = new RichTextBoxEx();
            rtb.Text = results;
        }

        public ResultWindow(String results, String path, String driveLetter, bool saveNow)
        {
            InitializeComponent();
            textBox1.Text = results;
            rtb = new RichTextBoxEx();
            rtb.Text = results;

            // Append the trailing "\" if it is missing.
            String normalizedPath = path.EndsWith("\\") ? path : path + "\\";

            // Save if the saveNow flag is set AND the ORIGINAL path is not null/empty.
            if (saveNow && !String.IsNullOrEmpty(path))
            {
                try
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(normalizedPath + "Recovery Data for Volume "
                        + driveLetter.Substring(0, 1) + ".txt");
                    writer.Write(textBox1.Text);
                    writer.Flush();
                    writer.Close();
                    saveSuccess = true;
                    MessageBox.Show("A copy of the encryption information has been saved to " +
                        path + " at your request.", "Save Password Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exceptions were detected saving the file: " + ex.Message, "Severe",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save Encryption Results";
            sfd.Filter = "Text files (*.txt)|*.txt";
            sfd.ShowDialog();

            if (sfd.FileName != String.Empty)
            {
                try
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(sfd.FileName);
                    writer.Write(textBox1.Text);
                    writer.Flush();
                    writer.Close();
                    saveSuccess = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exceptions were detected saving the file: " + ex.Message, "Severe",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ResultWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            if (!saveSuccess)
            {
                if (MessageBox.Show("Your encryption details have not yet been saved.  Are you sure you want to close this window?",
                    "Results Not Saved", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    e.Cancel = false;
                }
            }
            else
            {
                e.Cancel = false;
            }
        }

        // Printing
        // C#
        public void PrintRichTextContents()
        {
            printDoc = new PrintDocument();
            printDialog1.Document = printDoc;
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDoc.BeginPrint += new PrintEventHandler(printDoc_BeginPrint);
                printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
                printDoc.EndPrint += new PrintEventHandler(printDoc_EndPrint);
                // Start printing process
                printDoc.Print();
            }
        }

        // variable to trace text to print for pagination
        private int m_nFirstCharOnPage;

        private void printDoc_BeginPrint(object sender,
            System.Drawing.Printing.PrintEventArgs e)
        {
            // Start at the beginning of the text
            m_nFirstCharOnPage = 0;
        }

        private void printDoc_PrintPage(object sender,
            System.Drawing.Printing.PrintPageEventArgs e)
        {
            // To print the boundaries of the current page margins
            // uncomment the next line:
            // e.Graphics.DrawRectangle(System.Drawing.Pens.Blue, e.MarginBounds);

            // make the RichTextBoxEx calculate and render as much text as will
            // fit on the page and remember the last character printed for the
            // beginning of the next page
            m_nFirstCharOnPage = rtb.FormatRange(false,
                                                    e,
                                                    m_nFirstCharOnPage,
                                                    rtb.TextLength);

            // check if there are more pages to print
            if (m_nFirstCharOnPage < rtb.TextLength)
                e.HasMorePages = true;
            else
                e.HasMorePages = false;
        }

        private void printDoc_EndPrint(object sender,
            System.Drawing.Printing.PrintEventArgs e)
        {
            // Clean up cached information
            rtb.FormatRangeDone();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintRichTextContents();
        }
    }
}
