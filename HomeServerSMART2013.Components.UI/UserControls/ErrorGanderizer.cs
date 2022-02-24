using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class ErrorGanderizer : Form
    {
        String exceptionText;
        public ErrorGanderizer(String errorMessage)
        {
            InitializeComponent();
            exceptionText = errorMessage;
        }

        private void ErrorGanderizer_Load(object sender, EventArgs e)
        {
            textBox1.Text = exceptionText;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
