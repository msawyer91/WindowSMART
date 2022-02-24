using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Mexi_Sexi
{
    public sealed class MexiSexi
    {
        private XmlDocument xmlDoc;
        private bool isGeezery;
        private bool isError;
        private bool isMexiSexi;
        private Components.UserControls.UserObject user;
        private uint referenceCode;
        private DateTime checker;
        private bool isHomeEdition;

        public MexiSexi(String xmlDocumentString, DateTime result, Guid refGuid, uint refCode)
        {
            Refresh(xmlDocumentString, result, refGuid, refCode);
        }

        public void Refresh(String xmlDocumentString, DateTime result, Guid refGuid, uint refCode)
        {
            // isGeezery = date expired (or install date in future - tampering)
            // isMexiSexi = the license is Mexi-Sexi (in good shape)
            bool isFutureInstall = false;
            isGeezery = false;
            isError = false;
            isMexiSexi = false;
            isHomeEdition = false;
            referenceCode = refCode;
            checker = new DateTime(result.Year, result.Month, result.Day, 23, 59, 59, DateTimeKind.Local).AddDays(30);
            if (xmlDocumentString.StartsWith("0xF,") || License.IsExpired(checker, this, false))
            {
                isGeezery = true;
            }

            if (result > DateTime.Now)
            {
                // Commented out below line -- set in ParseXml if necessary.
                //referenceCode = 0x10280713; // Install date is in the future.
                isFutureInstall = true;
            }

            xmlDoc = GetXml(xmlDocumentString);
            ParseXml(refGuid, isFutureInstall);
        }

        private XmlDocument GetXml(String xmlString)
        {
            if (xmlString.StartsWith("0xF,"))
            {
                xmlString = xmlString.Substring(4);
            }

            XmlDocument doc = null;
            try
            {
                doc = Components.Licensing.XmlSerializer.DeserializeXml(xmlString, false, String.Empty, String.Empty);
                if (String.IsNullOrEmpty(doc.InnerXml) || !doc.HasChildNodes)
                {
                    // Decode failed
                    isMexiSexi = false;
                    isError = true;
                    if (referenceCode == 0x0 || referenceCode == 0x1)
                    {
                        referenceCode = 0x1028070A; // XML decode failed
                    }
                    return new XmlDocument();
                }
                return doc;
            }
            catch(Exception)
            {
                isMexiSexi = false;
                isError = true;
                if (referenceCode == 0x0 || referenceCode == 0x1)
                {
                    referenceCode = 0x1028070B; // XML decode threw exception
                }
                return new XmlDocument();
            }
        }

        private void ParseXml(Guid refGuid, bool isFutureInstall)
        {
            if (isError)
            {
                return;
            }

            // Parse the XML
            String userName = String.Empty;
            String company = String.Empty;
            String email = String.Empty;
            String guid = String.Empty;
            String code = String.Empty;
            bool displayUser = false;
            bool displayCompany = false;

            foreach (XmlNode node in xmlDoc.ChildNodes)
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
                                                    userName = attrib.Value;
                                                    break;
                                                }
                                            case "companyName":
                                                {
                                                    company = attrib.Value;
                                                    break;
                                                }
                                            case "emailAddy":
                                                {
                                                    email = attrib.Value;
                                                    break;
                                                }
                                            case "guid":
                                                {
                                                    guid = attrib.Value;
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
                                                    code = attrib.Value;
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
                                                        displayUser = bool.Parse(attrib.Value);
                                                    }
                                                    catch
                                                    {
                                                        displayUser = false;
                                                        referenceCode = 0x10280711;
                                                        isError = true;
                                                    }
                                                    break;
                                                }
                                            case "companyName":
                                                {
                                                    try
                                                    {
                                                        displayCompany = bool.Parse(attrib.Value);
                                                    }
                                                    catch
                                                    {
                                                        displayCompany = false;
                                                        referenceCode = 0x10280712;
                                                        isError = true;
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

            if (code.Length != 8)
            {
                if (referenceCode == 0x0 || referenceCode == 0x1)
                {
                    referenceCode = 0x1028070C; // product code length invalid
                }
                isError = true;
            }
            else
            {
                String codeA = code.Substring(0, 4);
                String codeB = code.Substring(4);

                if (codeA != "WS2_" && codeA != "TWB_")
                {
                    if (referenceCode == 0x0 || referenceCode == 0x1)
                    {
                        referenceCode = 0x1028070D; // Invalid product code prefix
                    }
                    isError = true;
                }

                if (codeB != "FAML" && codeB != "SOHO" && codeB != "SBZ1" && codeB != "SBZ2" && codeB != "MBZ1" && codeB != "MBZ2" &&
                    codeB != "LBIZ" && codeB != "EBIZ" && codeB != "NPRF" && codeB != "EDUC" && codeB != "GOVT" && codeB != "FRFA" &&
                    codeB != "UREG" && codeB != "HOME" && codeB != "PROF" && codeB != "INDF" && codeB != "SNGL")
                {
                    if (referenceCode == 0x0 || referenceCode == 0x1)
                    {
                        referenceCode = 0x1028070E; // Invalid product code suffix
                    }
                    isError = true;
                }

                if (codeB == "HOME" || codeB == "SNGL")
                {
                    isHomeEdition = true;
                }
            }

            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(company) ||
                String.IsNullOrEmpty(email) || String.IsNullOrEmpty(guid))
            {
                if (referenceCode == 0x0 || referenceCode == 0x1)
                {
                    referenceCode = 0x1028070F; // mandatory data missing
                }
                isError = true;
            }

            Guid testGuid = new Guid(guid);
            if (refGuid.CompareTo(testGuid) != 0)
            {
                if (referenceCode == 0x0 || referenceCode == 0x1)
                {
                    referenceCode = 0x10280710; // GUID mismatch
                }
                isError = true;
            }

            if (isError)
            {
                isMexiSexi = false;
                user = new Components.UserControls.UserObject(new Guid("00000000-0000-0000-0000-000000000000"), String.Empty, String.Empty, String.Empty, "WS2_UREG",
                    false, false);
            }
            else
            {
                // No errors up to this point so we're good to go!
                // *SES* We do our future install date checks here. If a valid key has been provided, we don't care
                // about a future install date since we assume the user has paid the registration fee. So only an unregistered
                // (UREG && !isGeezery) or an expired (UREG && isGeezery) install should do this check.
                isMexiSexi = true;
                user = new Components.UserControls.UserObject(refGuid, userName, company, email, code, displayUser, displayCompany);
                if (isGeezery && code.EndsWith("UREG"))
                {
                    isMexiSexi = false;
                    if (isFutureInstall)
                    {
                        // Future install date on expired trial; set isError to true and appropriate reference code.
                        isError = true;
                        referenceCode = 0x10280713; // Install date is in the future.
                    }
                }
                else if (isMexiSexi && code.EndsWith("UREG"))
                {
                    isMexiSexi = false;
                    if (isFutureInstall)
                    {
                        // Future install date on unregistered edition; set isError to true and appropriate reference code.
                        isError = true;
                        referenceCode = 0x10280713; // Install date is in the future.
                    }
                }
                else
                {
                    isGeezery = false;
                }
            }
        }

        public bool IsMexiSexi
        {
            get
            {
                return isMexiSexi;
            }
        }

        public bool IsGeezery
        {
            get
            {
                return isGeezery;
            }
        }

        public bool IsError
        {
            get
            {
                return isError;
            }
        }

        public bool IsHomeEdition
        {
            get
            {
                return isHomeEdition;
            }
        }

        public Components.UserControls.UserObject UserInfo
        {
            get
            {
                return user;
            }
        }

        public uint ReferenceCode
        {
            get
            {
                return referenceCode;
            }
        }

        public DateTime Checker
        {
            get
            {
                return checker;
            }
        }
    }
}
