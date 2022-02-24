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
    public partial class DialogBox : Form
    {
        public DialogBox()
        {
            InitializeComponent();
            throw new NotImplementedException();
        }

        public DialogBox(String title, String message)
        {
            InitializeComponent();
            this.Text = title;
            labelDialogMessage.Text = message;
        }

        public String DialogText
        {
            get
            {
                return textBoxDialogBox.Text;
            }
        }
    }
}
