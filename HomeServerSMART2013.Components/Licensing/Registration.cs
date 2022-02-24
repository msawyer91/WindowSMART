using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Licensing
{
    public partial class Registration : Form
    {
        private LocalLicense license;
        private uint commitCode;

        public Registration()
        {
            InitializeComponent();
            license = new LocalLicense();
            commitCode = 0xFFFFFFFF;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            commitCode = 0x1;
            this.Close();
        }

        private void radioButtonFile_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonFile.Checked)
            {
                buttonBrowse.Enabled = true;
                textBoxKey.ReadOnly = true;
            }
        }

        private void radioButtonPaste_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonPaste.Checked)
            {
                buttonBrowse.Enabled = false;
                textBoxKey.ReadOnly = false;
                if (!String.IsNullOrEmpty(textBoxKey.Text))
                {
                    if (MessageBox.Show("Clear existing contents?", "Clear Key", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)
                        == DialogResult.Yes)
                    {
                        textBoxKey.Clear();
                    }
                }
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select License File";
            ofd.Filter = "Slab License File (*.slab)|*.slab";
            ofd.ShowDialog();
            if (String.IsNullOrEmpty(ofd.FileName))
            {
                return;
            }

            try
            {
                if (!String.IsNullOrEmpty(textBoxKey.Text))
                {
                    if (MessageBox.Show("The existing contents in the license key box will be lost. Continue loading from file?",
                        "Import Key", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)
                        == DialogResult.No)
                    {
                        return;
                    }
                }

                StreamReader reader = new StreamReader(ofd.FileName);
                textBoxKey.Clear();
                textBoxKey.Text = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to read the license file. " + ex.Message, "Severe", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
            }
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            uint retVal = ValidateDoc();
            if (retVal == 0x0)
            {
                UserControls.ReviewInfo info = new UserControls.ReviewInfo(license.UserName,
                    license.Company, license.EmailAddress, license.Product.Substring(4));
                info.ShowDialog();
                if (info.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    String doc = ComposeProse();
                    if (String.IsNullOrEmpty(doc))
                    {
                        MessageBox.Show("The Server was unable to generate the required XML. Please try again.", "Severe",
                            MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        return;
                    }

                    String storageElement = String.Empty;
                    try
                    {
                        storageElement = XmlSerializer.SerializeXml(doc);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("The licensing system detected a cryptographic engine failure: " + ex.Message, "Severe",
                            MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        return;
                    }

                    bool inject = LegacyOs.Inject(storageElement, license.LicenseGuid);
                    if (inject)
                    {
                        commitCode = 0x0;
                        DialogResult res = MessageBox.Show("Thank you for registering WindowSMART 24/7!\n\nIt is highly recommended that you restart the WindowSMART 24/7 service. " +
                            "You can restart the service now, or you can restart it later by selecting Settings from the Edit menu, and choosing the Service Control tab in the Settings dialogue.\n\n" +
                            "Do you want to restart the WindowSMART 24/7 service now?", "Danke!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (res == System.Windows.Forms.DialogResult.Yes)
                        {
                            try
                            {
                                String path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                                System.Diagnostics.ProcessStartInfo psi;
                                psi = new System.Diagnostics.ProcessStartInfo(path + "\\HomeServerSMART2013.Service.exe", "/reboot /siesta");
                                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                                System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);

                                // Wait up to 30 seconds
                                int count = 0;
                                while (!process.HasExited)
                                {
                                    System.Threading.Thread.Sleep(1000);
                                    count++;
                                    if (count >= 30)
                                    {
                                        break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Unable to restart the WindowSMART 24/7 service (" + ex.Message + "). Please restart it manually at your earliest convenience.",
                                    "Service Restart Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        this.Close();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("License injection failed.", "Severe", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        return;
                    }
                }
            }
            else if (retVal == 0x1)
            {
                return;
            }
            else
            {
                MessageBox.Show("The provided license key is invalid. Please try a different key.\n\n" +
                    "If you believe your key to be valid, please contact Dojo North Software and report the " +
                    "error code 0x" + retVal.ToString("X") + ". Please include a copy of your license key, " +
                    "along with your order number, in your correspondence.",
                    "Invalid License Key", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        private uint ValidateDoc()
        {
            XmlDocument xmldoc = null;

            try
            {
                xmldoc = new XmlDocument();
                xmldoc.LoadXml(textBoxKey.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There is a problem with the license file: " + ex.Message + "\n\n" +
                    "A license consists of indented, well-formed XML, and should have no erroneous carriage " +
                    "returns. If your license was delivered via email and you copied and pasted the text into " +
                    "the box, it is possible that it contains extra carriage returns. " +
                    "It is not uncommon for some email clients to adjust line breaks in emails, particularly " +
                    "if the email was delivered in plaintext format (as opposed to HTML or Rich Text).",
                    "Malformed XML", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return 0x1;
            }

            // License key
            String herbieHancock = String.Empty;
            String johnHancock = String.Empty;

            bool rootDetected = false;
            bool herbieHancockDetected = false;
            bool johnHancockDetected = false;

            foreach (XmlNode node in xmldoc.ChildNodes)
            {
                if (node.Name == "license")
                {
                    rootDetected = true;
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        if (child.Name == "HerbieHancock")
                        {
                            herbieHancockDetected = true;
                            herbieHancock = child.InnerText;
                        }
                        else if (child.Name == "JohnHancock")
                        {
                            johnHancockDetected = true;
                            johnHancock = child.InnerText;
                        }
                    }
                }
            }

            if (!rootDetected)
            {
                return 0xA4281000; // no root
            }
            else if (!herbieHancockDetected)
            {
                return 0xA4281001; // no HerbieHancock
            }
            else if (!johnHancockDetected)
            {
                return 0xA4281002; // no JohnHancock
            }

            if (String.IsNullOrEmpty(herbieHancock))
            {
                return 0xA4281003; // Herbie Hancock was defined in XML but null or empty
            }
            else if (String.IsNullOrEmpty(johnHancock))
            {
                return 0xA4281004; // John Hancock was defined in XML but null or empty
            }

            try
            {
                RSACryptoServiceProvider rsacp = new RSACryptoServiceProvider(4096);
                rsacp.FromXmlString(Components.License.PUBLIC_KEY);
                ASCIIEncoding ByteConverter = new ASCIIEncoding();
                byte[] verify_this = ByteConverter.GetBytes(herbieHancock);
                byte[] signature = Convert.FromBase64String(johnHancock);
                bool verify = rsacp.VerifyData(verify_this, new SHA1CryptoServiceProvider(), signature);
                if (!verify)
                {
                    return 0xA4281005; // John Hancock verification failed
                }
            }
            catch
            {
                return 0xA4281006; // Exception in crypto section
            }

            XmlDocument userDoc = XmlSerializer.DeserializeXml(herbieHancock, true, Components.BitLocker.UtilityMethods.BITLOCKER_METADATA_VERSION,
                Components.IdentifySsdManufacturer.SOLID_STATE_DISK_GLOBAL_ID);

            if(!userDoc.HasChildNodes || String.IsNullOrEmpty(userDoc.InnerXml))
            {
                return 0xA4281007; // Exception while decrypting
            }

            foreach (XmlNode node in userDoc.ChildNodes)
            {
                if (node.Name == "license")
                {
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        switch (child.Name)
                        {
                            case "userDetail":
                                {
                                    foreach (XmlAttribute attrib in child.Attributes)
                                    {
                                        switch (attrib.Name)
                                        {
                                            case "userName":
                                                {
                                                    license.UserName = attrib.Value;
                                                    break;
                                                }
                                            case "companyName":
                                                {
                                                    license.Company = attrib.Value;
                                                    break;
                                                }
                                            case "emailAddy":
                                                {
                                                    license.EmailAddress = attrib.Value;
                                                    break;
                                                }
                                            case "guid":
                                                {
                                                    license.LicenseGuid = attrib.Value;
                                                    break;
                                                }
                                            default:
                                                {
                                                    break;
                                                }
                                        }
                                    }
                                    break;
                                }
                            case "product":
                                {
                                    foreach (XmlAttribute attrib in child.Attributes)
                                    {
                                        switch (attrib.Name)
                                        {
                                            case "code":
                                                {
                                                    license.Product = attrib.Value;
                                                    break;
                                                }
                                            default:
                                                {
                                                    break;
                                                }
                                        }
                                    }
                                    break;
                                }
                            case "displayData":
                                {
                                    foreach (XmlAttribute attrib in child.Attributes)
                                    {
                                        switch (attrib.Name)
                                        {
                                            case "userName":
                                                {
                                                    try
                                                    {
                                                        license.DisplayUser = bool.Parse(attrib.Value);
                                                    }
                                                    catch
                                                    {
                                                        return 0xA4281008; // corrupted value
                                                    }
                                                    break;
                                                }
                                            case "companyName":
                                                {
                                                    try
                                                    {
                                                        license.DisplayCompany = bool.Parse(attrib.Value);
                                                    }
                                                    catch
                                                    {
                                                        return 0xA4281009; // corrupted value
                                                    }
                                                    break;
                                                }
                                            default:
                                                {
                                                    break;
                                                }
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                }
            }

            if (license.Product.Length != 8)
            {
                return 0xA428100A; // invalid product code length
            }
            else
            {
                String codeA = license.Product.Substring(0, 4);
                String codeB = license.Product.Substring(4);

                if (codeA != "WS2_" && codeA != "TWB_")
                {
                    return 0xA428100B; // license may be valid, but for wrong product
                }

                if (codeB != "FAML" && codeB != "SOHO" && codeB != "SBZ1" && codeB != "SBZ2" && codeB != "MBZ1" && codeB != "MBZ2" &&
                    codeB != "LBIZ" && codeB != "EBIZ" && codeB != "NPRF" && codeB != "EDUC" && codeB != "GOVT" && codeB != "FRFA" &&
                    codeB != "UREG" && codeB != "HOME" && codeB != "PROF" && codeB != "INDF" && codeB != "SNGL")
                {
                    return 0xA428100C; // invalid product code suffix
                }
            }

            if (String.IsNullOrEmpty(license.UserName) || String.IsNullOrEmpty(license.Company) ||
                String.IsNullOrEmpty(license.EmailAddress) || String.IsNullOrEmpty(license.LicenseGuid))
            {
                return 0xA428100D; // mandatory data missing
            }

            return 0x0;
        }

        private String ComposeProse()
        {
            try
            {
                System.Text.StringBuilder stream = new System.Text.StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.Encoding = Encoding.UTF8;
                XmlWriter writer = XmlWriter.Create(stream, settings);

                writer.WriteStartDocument(); // <?xml version="1.0" encoding="utf-8"?>

                writer.WriteStartElement("license", "");

                writer.WriteStartElement("userDetail", "");

                writer.WriteAttributeString("userName", license.UserName);
                writer.WriteAttributeString("companyName", license.Company);
                writer.WriteAttributeString("emailAddy", license.EmailAddress);
                writer.WriteAttributeString("guid", license.LicenseGuid);

                writer.WriteEndElement(); // userDetail

                writer.WriteStartElement("product", "");

                writer.WriteAttributeString("code", license.Product);

                writer.WriteEndElement(); // product

                writer.WriteStartElement("displayData", "");

                writer.WriteAttributeString("userName", license.DisplayUser.ToString());
                writer.WriteAttributeString("companyName", license.DisplayCompany.ToString());

                writer.WriteEndElement(); // displayData

                writer.WriteEndElement(); // license

                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();

                return stream.ToString();
            }
            catch
            {
                return String.Empty;
            }
        }

        public uint CommitCode
        {
            get
            {
                return commitCode;
            }
        }

        internal class LocalLicense
        {
            String userName;
            String companyName;
            String emailAddress;
            String guid;
            String product;
            bool displayUser;
            bool displayCompany;

            public LocalLicense()
            {
                userName = String.Empty;
                companyName = String.Empty;
                emailAddress = String.Empty;
                guid = String.Empty;
                product = String.Empty;
                displayUser = false;
                displayCompany = false;
            }

            public String UserName
            {
                get
                {
                    return userName;
                }
                set
                {
                    userName = value;
                }
            }

            public String Company
            {
                get
                {
                    return companyName;
                }
                set
                {
                    companyName = value;
                }
            }

            public String EmailAddress
            {
                get
                {
                    return emailAddress;
                }
                set
                {
                    emailAddress = value;
                }
            }

            public String LicenseGuid
            {
                get
                {
                    return guid;
                }
                set
                {
                    guid = value;
                }
            }

            public String Product
            {
                get
                {
                    return product;
                }
                set
                {
                    product = value;
                }
            }

            public bool DisplayUser
            {
                get
                {
                    return displayUser;
                }
                set
                {
                    displayUser = true;
                }
            }

            public bool DisplayCompany
            {
                get
                {
                    return displayCompany;
                }
                set
                {
                    displayCompany = true;
                }
            }
        }
    }
}
