using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UserControls
{
    public partial class ReviewInfo : Form
    {
        public ReviewInfo(String user, String company, String email, String productClass)
        {
            InitializeComponent();

            textBoxUser.Text = user;
            textBoxCompany.Text = company;
            textBoxEmail.Text = email;
            textBoxClass.Text = GetProductClass(productClass);
            textBoxRestrictions.Text = GetProductRestrictions(productClass);
        }

        private String GetProductClass(String productClass)
        {
            switch (productClass)
            {
                case "FAML":
                case "SOHO":
                case "SBZ1":
                case "SBZ2":
                case "MBZ1":
                case "MBZ2":
                case "LBIZ":
                case "EBIZ":
                case "NPRF":
                case "EDUC":
                case "GOVT":
                case "FRFA":
                case "PROF":
                    {
                        return "Professional Edition";
                    }
                case "HOME":
                    {
                        return "Home Edition";
                    }
                case "INDF":
                    {
                        return "Professional Indefinite Evaluation";
                    }
                case "SNGL":
                    {
                        return "Home Single PC Edition";
                    }
                default:
                    {
                        return "Invalid";
                    }
            }
        }

        private String GetProductRestrictions(String productClass)
        {
            switch (productClass)
            {
                case "FAML":
                case "SOHO":
                case "SBZ1":
                case "SBZ2":
                case "MBZ1":
                case "MBZ2":
                case "LBIZ":
                case "EBIZ":
                case "NPRF":
                case "EDUC":
                case "GOVT":
                    {
                        return "Professional Edition (legacy license)";
                    }
                case "FRFA":
                    {
                        return "None";
                    }
                case "HOME":
                    {
                        return "Home Edition";
                    }
                case "SNGL":
                    {
                        return "Home Single PC Edition";
                    }
                case "PROF":
                    {
                        return "Professional Edition";
                    }
                case "INDF":
                    {
                        return "Evaluation - Non-production Use Only";
                    }
                default:
                    {
                        return "Invalid";
                    }
            }
        }
    }
}
