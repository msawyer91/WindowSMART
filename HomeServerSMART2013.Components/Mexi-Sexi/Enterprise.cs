using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Mexi_Sexi
{
    public sealed class Enterprise
    {
        public static int DoEnterpriseTransaction(String filename)
        {
            Licensing.Registration.LocalLicense license;

            int retVal = ValidateDoc(filename, out license);
            if (retVal == 0x0)
            {
                String prose = ComposeProse(license);

                if (String.IsNullOrEmpty(prose))
                {
                    return 0xB;
                }

                String storageElement = String.Empty;
                try
                {
                    storageElement = Licensing.XmlSerializer.SerializeXml(prose);
                }
                catch (Exception)
                {
                    return 0xC;
                }

                bool inject = LegacyOs.Inject(storageElement, license.LicenseGuid);
                if (inject)
                {
                    return 0x0;
                }
                else
                {
                    return 0xD;
                }
            }
            else
            {
                return retVal;
            }
        }

        private static int ValidateDoc(String filename, out Licensing.Registration.LocalLicense license)
        {
            XmlDocument xmldoc = null;
            license = new Licensing.Registration.LocalLicense();

            try
            {
                xmldoc = new XmlDocument();
                xmldoc.Load(filename);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Warning - XML read failed; will retry as string read: " + ex.Message);
                int retVal = 0xA;

                try
                {
                    xmldoc = new XmlDocument();
                    System.IO.StreamReader reader = new System.IO.StreamReader(filename);
                    String xmlString = reader.ReadToEnd();
                    reader.Close();
                    xmldoc.LoadXml(xmlString);
                    Console.WriteLine("XML load succeeded on string read, will continue.");
                }
                catch (Exception ex2)
                {
                    Console.WriteLine("Severe - " + ex2.Message);
                    return retVal;
                }
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
                return 0x4281000; // no root
            }
            else if (!herbieHancockDetected)
            {
                return 0x4281001; // no HerbieHancock
            }
            else if (!johnHancockDetected)
            {
                return 0x4281002; // no JohnHancock
            }

            if (String.IsNullOrEmpty(herbieHancock))
            {
                return 0x4281003; // Herbie Hancock was defined in XML but null or empty
            }
            else if (String.IsNullOrEmpty(johnHancock))
            {
                return 0x4281004; // John Hancock was defined in XML but null or empty
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
                    return 0x4281005; // John Hancock verification failed
                }
            }
            catch
            {
                return 0x4281006; // Exception in crypto section
            }

            XmlDocument userDoc = Licensing.XmlSerializer.DeserializeXml(herbieHancock, true, Components.BitLocker.UtilityMethods.BITLOCKER_METADATA_VERSION,
                Components.IdentifySsdManufacturer.SOLID_STATE_DISK_GLOBAL_ID);

            if (!userDoc.HasChildNodes || String.IsNullOrEmpty(userDoc.InnerXml))
            {
                return 0x4281007; // Exception while decrypting
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
                                                        return 0x4281008; // corrupted value
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
                                                        return 0x4281009; // corrupted value
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
                return 0x428100A; // invalid product code length
            }
            else
            {
                String codeA = license.Product.Substring(0, 4);
                String codeB = license.Product.Substring(4);

                if (codeA != "WS2_" && codeA != "TWB_")
                {
                    return 0x428100B; // license may be valid, but for wrong product
                }

                if (codeB != "FAML" && codeB != "SOHO" && codeB != "SBZ1" && codeB != "SBZ2" && codeB != "MBZ1" && codeB != "MBZ2" &&
                    codeB != "LBIZ" && codeB != "EBIZ" && codeB != "NPRF" && codeB != "EDUC" && codeB != "GOVT" && codeB != "FRFA" &&
                    codeB != "UREG" && codeB != "HOME" && codeB != "PROF" && codeB != "INDF" && codeB != "SNGL")
                {
                    return 0x428100C; // invalid product code suffix
                }
            }

            if (String.IsNullOrEmpty(license.UserName) || String.IsNullOrEmpty(license.Company) ||
                String.IsNullOrEmpty(license.EmailAddress) || String.IsNullOrEmpty(license.LicenseGuid))
            {
                return 0x428100D; // mandatory data missing
            }

            return 0x0;
        }

        private static String ComposeProse(Licensing.Registration.LocalLicense license)
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
    }
}
