using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class HelpAbout : Form
    {
        private String expiration;
        private bool useDefaultSkinning;
        private Mexi_Sexi.MexiSexi mexiSexi;
        
        public HelpAbout(bool defaultSkinning)
        {
            InitializeComponent();
            expiration = String.Empty;
            //expiration = " (Expires: " + License.GetExpirationDate().ToString() + ")";
            useDefaultSkinning = defaultSkinning;
            mexiSexi = null;
        }

        public HelpAbout(bool defaultSkinning, Mexi_Sexi.MexiSexi ms)
        {
            InitializeComponent();
            expiration = String.Empty;
            //expiration = " (Expires: " + License.GetExpirationDate().ToString() + ")";
            useDefaultSkinning = defaultSkinning;
            mexiSexi = ms;
        }

        private void HelpAbout_Load(object sender, EventArgs e)
        {
            String title = OperatingSystem.IsWindowsServerSolutionsProduct(this) ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart;
            if (!useDefaultSkinning)
            {
                panel1.BackgroundImage = Properties.Resources.HelpAboutSBS;
            }
            this.Text = "About " + title;
            label1.Text = title;
            labelVersion.Text = "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + expiration;

            if (mexiSexi == null)
            {
                groupBox1.Text = "System Info";
                var name = (from x in new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get().OfType<ManagementObject>()
                            select x.GetPropertyValue("Caption")).First();
                labelEntity.Text = name != null ? name.ToString() : "Unknown";
                labelEmail.Text = Environment.OSVersion.VersionString;
                labelRestrictions.Text = "Licensed for Non-Commercial && Commercial Use";
            }
            else
            {
                if (mexiSexi.IsMexiSexi)
                {
                    // Registered
                    if (mexiSexi.UserInfo.DisplayUser && mexiSexi.UserInfo.DisplayCompany)
                    {
                        if (String.Compare(mexiSexi.UserInfo.UserName, mexiSexi.UserInfo.Company, true) == 0)
                        {
                            labelEntity.Text = mexiSexi.UserInfo.UserName.Replace("&", "&&");
                        }
                        else
                        {
                            labelEntity.Text = mexiSexi.UserInfo.UserName.Replace("&", "&&") + " / " + mexiSexi.UserInfo.Company.Replace("&", "&&");
                        }
                    }
                    else if (mexiSexi.UserInfo.DisplayCompany)
                    {
                        labelEntity.Text = mexiSexi.UserInfo.Company.Replace("&", "&&");
                    }
                    else
                    {
                        labelEntity.Text = mexiSexi.UserInfo.UserName.Replace("&", "&&");
                    }
                    labelEmail.Text = mexiSexi.UserInfo.EmailAddress;
                    if (!mexiSexi.UserInfo.ProgramType.EndsWith("FRFA"))
                    {
                        pictureBox2.Image = Properties.Resources.DiskHeartbeat200x200;
                    }
                    labelRestrictions.Text = Utilities.Utility.GetProductRestrictions(mexiSexi.UserInfo.ProgramType.Substring(4));
                }
                else
                {
                    pictureBox2.Image = Properties.Resources.DiskHeartbeat200x200;
                    labelEntity.Text = "Unregistered - 30 Day Trial";
                    labelEmail.Text = "30DayTrial@dojonorthsoftware.net";
                    labelRestrictions.Text = "Trial Expires " + mexiSexi.Checker.ToString();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Credits creds = new Credits();
            creds.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Utilities.Utility.LaunchBrowser(Properties.Resources.ResourceUrlAsp);
        }
    }
}
