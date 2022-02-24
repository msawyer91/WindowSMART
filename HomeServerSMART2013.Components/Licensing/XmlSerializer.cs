using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Licensing
{
    public sealed class XmlSerializer
    {
        public static String SerializeXml(String xmlDocumentString)
        {
            // Encrypts the data (converts from XML to secure)
            String base64 = Components.UserControls.Aero.USE_WINDOWS_VISTA_AERO;
            String method = Components.KnownVirtualDisks.KVD_DISK_ID;
            String xml = Components.XmlManager.EncodeXml(xmlDocumentString, method, base64);
            return xml;
        }

        public static XmlDocument DeserializeXml(String xmlDocumentString, bool isRegistration, String xmlFormat,
            String xmlNamespace)
        {
            // Decrypts the data (converts from secure to XML)
            String method = String.Empty;
            String base64 = String.Empty;
            String xml = String.Empty;

            if (isRegistration)
            {
                method = xmlFormat;
                base64 = xmlNamespace;
                xml = Components.XmlManager.DecodeXml(xmlDocumentString, method, base64, true);
            }
            else
            {
                method = Components.Reboot.RebootServer.REBOOT_SERVER_REASON_CODE;
                base64 = SmartMethods.SMART_DISK_IDENTIFY_DEVICE;
                xml = Components.XmlManager.DecodeXml(xmlDocumentString, method, base64, false);
            }
            
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(xml);
                return xmlDoc;
            }
            catch
            {
                return new XmlDocument();
            }
        }
    }
}
