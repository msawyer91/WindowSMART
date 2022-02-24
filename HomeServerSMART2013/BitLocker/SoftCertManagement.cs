using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    /// 
    /// Summary description for SoftCertManagement
    /// class which will handle and implemet the delegates
    /// 
    public class SoftCertManagement
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct CRYPTUI_CERT_MGR_STRUCT
        {
            public int dwSize;
            public IntPtr hwndParent;
            public int dwFlags;
            public string pwszTitle;
            public IntPtr pszInitUsageOID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CRYPT_KEY_PROV_INFO
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string ContainerName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string ProvName;
            public int ProvType;
            public int Flags;
            public int ProvParam;
            public IntPtr rgProvParam;
            public int KeySpec;
        }

        [DllImport("cryptui.dll", SetLastError = true)]
        public static extern IntPtr CryptUIDlgSelectCertificateFromStore(IntPtr hCertStore, IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)]
            string pwszTitle, [MarshalAs(UnmanagedType.LPWStr)] string pwszDisplayString, int dwDontUseColumn, int dwFlags, IntPtr pvReserved);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern IntPtr CertEnumCertificatesInStore(IntPtr hCertStore, IntPtr pPrevCertContext);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern bool CertGetNameString(IntPtr pCertContext, int dwType, int dwFlags, IntPtr pvTypePara, System.Text.StringBuilder pszNameString, Int32 cchNameString);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern bool CertGetCertificateContextProperty(IntPtr pCertContext, int dwPropId, IntPtr pvData, ref int pcbData);

        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CertOpenSystemStore(IntPtr hCryptProv, string storename);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern bool CertFreeCertificateContext(IntPtr hCertStore);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern bool CertCloseStore(IntPtr hCertStore, int dwFlags);

        [DllImport("cryptui.dll", SetLastError = true)]
        public static extern bool CryptUIDlgCertMgr(ref CRYPTUI_CERT_MGR_STRUCT pCryptUICertMgr);

        [DllImport("crypt32.dll")]
        public static extern bool CryptDecodeObject(int CertEncodingType, int lpszStructType, byte[] pbEncoded, int cbEncoded, int flags, [In(), Out()]
            byte[] pvStructInfo, ref int cbStructInfo);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern IntPtr CertFindCertificateInStore(IntPtr hCertStore, int dwCertEncodingType, int dwFindFlags, int dwFindType, [In(), MarshalAs(UnmanagedType.LPWStr)]
            string pszFindString, IntPtr pPrevCertContext);

        [StructLayout(LayoutKind.Sequential)]
        public struct PUBKEYBLOBHEADERS
        {
            ////BLOBHEADER
            public byte bType;

            // //BLOBHEADER
            public byte bVersion;

            ////BLOBHEADER
            public short reserved;

            ////BLOBHEADER
            public Int32 aiKeyAlg;

            // //RSAPUBKEY
            public int magic;

            //; '//RSAPUBKEY
            public int bitlen;

            //; //RSAPUBKEY
            public int pubexp;
        }

        public int CERT_NAME_SIMPLE_DISPLAY_TYPE = 0x4;
        public int CRYPTUI_SELECT_LOCATION_COLUMN = 0x10;
        public int CERT_KEY_PROV_INFO_PROP_ID = 0x2;
        static public int X509_ASN_ENCODING = 0x1;
        static public int PKCS_7_ASN_ENCODING = 0x10000;
        public int RSA_CSP_PUBLICKEYBLOB = 19;
        static public int ENCODING_TYPE = PKCS_7_ASN_ENCODING | X509_ASN_ENCODING;
        public int CERT_FIND_SUBJECT_STR = 0x80007;
        public byte[] pubblob;
        public string PKC12CertSelectedName = "";
        public byte[] EncKey;
        public byte[] EncIv;
        public byte[] EncData;

        //creates new instance of Rinjndael
        public RijndaelManaged Rin = new RijndaelManaged();
        public byte[] SignedData;
        public string CertForEnc = "";
        public string CertForSig = "";
    }
}