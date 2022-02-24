using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public static class Certs
    {
        public static String GetThumbprintFromStore(IntPtr windowHandle, out String subjectName)
        {
            String thumbprint = String.Empty;
            subjectName = String.Empty;
            try
            {
                // Open the X509 certificate store "MY" (the Personal certs for the current user).
                X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
                // Open the store in read-only mode (we don't want to change anything!); do not create if no store exists.
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                // Get a collection of certs in the MY store.
                X509Certificate2Collection myCertCollection = (X509Certificate2Collection)store.Certificates;
                // Get only certs that match our application policy - BitLocker OID.
                X509Certificate2Collection blSupportedCertCollection = (X509Certificate2Collection)
                    myCertCollection.Find(X509FindType.FindByApplicationPolicy, ((String.IsNullOrEmpty(GPConfig.CertificateOid) ? "1.3.6.1.4.1.311.67.1.1" : GPConfig.CertificateOid)), true);
                // Let the user pick the cert they want.
                X509Certificate2Collection scollection = X509Certificate2UI.SelectFromCollection(blSupportedCertCollection,
                    "Smart Card Certificate Select", "Select a certificate to use with BitLocker.", X509SelectionFlag.SingleSelection,
                    windowHandle);
                foreach (X509Certificate2 x509 in scollection)
                {
                    // Get and return the details.
                    byte[] rawdata = x509.RawData;
                    thumbprint = x509.Thumbprint;
                    subjectName = x509.Subject;
                    x509.Reset();
                }
                store.Close();
            }
            catch (CryptographicException)
            {
                thumbprint = String.Empty;
            }
            return thumbprint;
        }
    }
}
