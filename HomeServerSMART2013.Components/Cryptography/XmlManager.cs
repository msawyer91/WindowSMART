using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class XmlManager
    {
        public static String EncodeXml(String xml, String encodingMethod, String useBase64)
        {
            Cryptography.DoubleEncryptor encryptor = new Cryptography.DoubleEncryptor(Components.Reboot.RebootServer.REBOOT_SERVER_REASON_CODE + useBase64,
                encodingMethod + SmartMethods.SMART_DISK_IDENTIFY_DEVICE);
            encryptor.SetCbcRequired(OperatingSystem.WINDOWS_8_METRO);

            return encryptor.Encrypt(xml);
        }

        public static String DecodeXml(String encodedXml, String encodingMethod, String useBase64, bool isRegistration)
        {
            Cryptography.DoubleEncryptor encryptor = null;
            if (isRegistration)
            {
                encryptor = new Cryptography.DoubleEncryptor(encodingMethod + Utilities.Utility.LOGFILE_ERROR_DUMP_LOCATION,
                    Components.Debugging.VersionInfo.MICROSOFT_DOT_NET_RUNTIME + useBase64);
                encryptor.SetCbcRequired(OperatingSystem.WINDOWS_OS_LEGACY);
            }
            else
            {
                encryptor = new Cryptography.DoubleEncryptor(encodingMethod + UserControls.Aero.USE_WINDOWS_VISTA_AERO,
                    KnownVirtualDisks.KVD_DISK_ID + useBase64);
                encryptor.SetCbcRequired(OperatingSystem.WINDOWS_8_METRO);
            }

            String xml = encryptor.Decrypt(encodedXml);
            return xml;
        }
    }
}
