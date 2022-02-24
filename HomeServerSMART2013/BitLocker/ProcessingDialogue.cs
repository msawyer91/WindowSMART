using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public partial class ProcessingDialogue : Form
    {
        public ProcessingDialogue(Point parentLocation, Size parentSize)
        {
            InitializeComponent();

            int currentSizeX = this.Size.Width;
            int currentSizeY = this.Size.Height;
            int widthDifference = parentSize.Width - this.Size.Width;
            int heightDifference = parentSize.Height - this.Size.Height;
            int posX = widthDifference / 2;
            int posY = heightDifference / 2;
            posX = parentLocation.X + posX;
            posY = parentLocation.Y + posY;

            this.Location = new Point(posX, posY);
            this.SetClientSizeCore(currentSizeX, currentSizeY);
        }
    }
}
