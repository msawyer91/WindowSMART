using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public static class GPConfig
    {
        // Bools to determine what options to make available to the user.
        private static bool isTpmUsable;
        private static bool vistaAllowNonTpm;
        private static bool win7AllowNonTpm;
        private static bool vistaAllowBek;
        private static bool vistaRequireBek;
        private static bool vistaAllowNumericPw;
        private static bool vistaRequireNumericPw;
        private static bool win7OsAllowBek;
        private static bool win7OsRequireBek;
        private static bool win7OsAllowNumericPw;
        private static bool win7OsRequireNumericPw;
        private static bool win7OsAllowEnhancedPin;
        private static bool win7OsHideRecoveryPage;
        private static bool win7FdvAllowBek;
        private static bool win7FdvRequireBek;
        private static bool win7FdvAllowNumericPw;
        private static bool win7FdvRequireNumericPw;
        private static bool win7FdvAllowUserCert;
        private static bool win7FdvEnforceUserCert;
        private static bool win7FdvAllowPassphrase;
        private static bool win7FdvRequirePassphrase;
        private static bool win7FdvCreateDiscoveryVolume;
        private static bool win7FdvHideRecoveryPage;
        private static bool win7RdvAllowRdv;
        private static bool win7RdvAllowBek;
        private static bool win7RdvRequireBek;
        private static bool win7RdvAllowNumericPw;
        private static bool win7RdvRequireNumericPw;
        private static bool win7RdvAllowUserCert;
        private static bool win7RdvEnforceUserCert;
        private static bool win7RdvAllowPassphrase;
        private static bool win7RdvRequirePassphrase;
        private static bool win7RdvCreateDiscoveryVolume;
        private static bool win7RdvHideRecoveryPage;
        private static bool win7RdvAllowSuspend;

        // Integers for options.
        private static int win7OsMinimumPin;
        private static int win7FdvMinimumPassphrase;
        private static int win7RdvMinimumPassphrase;

        // FIPS Compliance
        private static bool isFipsComplianceMandatory;
        private static String princessOfPretty;
        // FIPS blocks several components, including numeric passwords and enhanced passwords (passphrases)

        // Windows Vista, Server 2008 and All
        private static String vistaRequireKeyEscrow;
        private static String vistaKeyEscrow;
        private static String vistaKeyStorage;
        private static String vistaAllowWithoutTpm;
        private static String vistaStartupKey;
        private static String vistaStartupPin;
        private static String allDefaultFolder;
        private static String vistaAllAllowBek;
        private static String vistaAllAllowPassword;
        private static String allRequiredEncryptionMethod;
        private static EncryptionMethod method;
        private static String allMemoryOverwrite;
        private static String tpmStatus;

        // Windows 7 and Server 2008 R2 Only
        private static String identificationFieldString;
        private static String secondaryIdentificationField;
        private static String certificateOid;

        // FDV fixed data volumes
        private static String fdvAllowUserCert;
        private static String fdvEnforceUserCert;
        private static String fdvDenyWriteAccess;
        private static String fdvDiscoveryVolumeType;
        private static String fdvNoBitLockerToGoReader;
        private static String fdvEnforcePassphrase;
        private static String fdvPassphrase;
        private static String fdvPassphraseComplexity;
        private static String fdvPassphraseLength;
        private static String fdvActiveDirectoryBackup;
        private static String fdvActiveDirectoryInfoToStore;
        private static String fdvHideRecoveryPage;
        private static String fdvManageDra;
        private static String fdvRecovery;
        private static String fdvRecoveryKey;
        private static String fdvRecoveryPassword;
        private static String fdvRequireActiveDirectoryBackup;

        // RDV roaming (or removble) data volumes
        private static String rdvAllowBde;
        private static String rdvConfigureBde;
        private static String rdvDisableBde;
        private static String rdvAllowUserCert;
        private static String rdvEnforceUserCert;
        private static String rdvDenyWriteAccess;
        private static String rdvDenyCrossOrg;
        private static String rdvDiscoveryVolumeType;
        private static String rdvNoBitLockerToGoReader;
        private static String rdvEnforcePassphrase;
        private static String rdvPassphrase;
        private static String rdvPassphraseComplexity;
        private static String rdvPassphraseLength;
        private static String rdvActiveDirectoryBackup;
        private static String rdvActiveDirectoryInfoToStore;
        private static String rdvHideRecoveryPage;
        private static String rdvManageDra;
        private static String rdvRecovery;
        private static String rdvRecoveryKey;
        private static String rdvRecoveryPassword;
        private static String rdvRequireActiveDirectoryBackup;

        // OS volume (Windows 7/2008 R2)
        private static String useAdvancedStartup;
        private static String enableBdeWithNoTpm;
        private static String useTpm;
        private static String useTpmKey;
        private static String useTpmKeyPin;
        private static String useTpmPin;
        private static String useEnhancedPin;
        private static String minimumPin;
        private static String osActiveDirectoryBackup;
        private static String osActiveDirectoryInfoToStore;
        private static String osHideRecoveryPage;
        private static String osManageDra;
        private static String osRecovery;
        private static String osRecoveryKey;
        private static String osRecoveryPassword;
        private static String osRequireActiveDirectoryBackup;

        // Friends & Family
        private static bool isFriendFamily;

        static GPConfig()
        {
            SetAllFalse();
            ReloadConfiguration();
            tpmStatus = String.Empty;
            isTpmUsable = IsTpmInstalledAndActive(out tpmStatus);
            princessOfPretty = "T1a0r2y8n2P0r0e7t1t0y21802208027010072";
        }

        public static void ReloadConfiguration()
        {
            Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey subKey = registryHklm.OpenSubKey(Properties.Resources.RegistryConfigGpoFve);
            Microsoft.Win32.RegistryKey fipsSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryConfigGpoFips);
            Microsoft.Win32.RegistryKey ccsSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryConfigGpoFveCurrentControlSet);
            Microsoft.Win32.RegistryKey baronessSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryConfigBaronessTbm);

            // Friend/Family?
            try
            {
                int ff = (int)baronessSubKey.GetValue("Baroness");
                if (ff == 10282007)
                {
                    isFriendFamily = true;
                }
                else
                {
                    isFriendFamily = false;
                }
            }
            catch
            {
                isFriendFamily = false;
            }

            // Federal Information Processing Standard (FIPS) mandatory?
            try
            {
                int regValue = (int)fipsSubKey.GetValue("Enabled");
                switch (regValue)
                {
                    case 0:
                        {
                            isFipsComplianceMandatory = false;
                            break;
                        }
                    case 1:
                        {
                            isFipsComplianceMandatory = true;
                            break;
                        }
                    default:
                        {
                            isFipsComplianceMandatory = false;
                            break;
                        }
                }
            }
            catch
            {
                isFipsComplianceMandatory = false;
            }

            // Perform Key Escrow?
            try
            {
                int regValue = (int)subKey.GetValue("ActiveDirectoryBackup");
                switch (regValue)
                {
                    case 0:
                        {
                            vistaKeyEscrow = "No";
                            break;
                        }
                    case 1:
                        {
                            vistaKeyEscrow = "Yes";
                            break;
                        }
                    default:
                        {
                            vistaKeyEscrow = "No (Not Defined)";
                            break;
                        }
                }
            }
            catch
            {
                vistaKeyEscrow = "No (Not Defined)";
            }

            // What to Back Up
            try
            {
                int regValue = (int)subKey.GetValue("ActiveDirectoryInfoToStore");
                switch (regValue)
                {
                    case 0:
                        {
                            vistaKeyStorage = "Passwords Only";
                            break;
                        }
                    case 1:
                        {
                            vistaKeyStorage = "Passwords and Key Packages";
                            break;
                        }
                    default:
                        {
                            vistaKeyStorage = "Not Defined";
                            break;
                        }
                }
            }
            catch
            {
                vistaKeyStorage = "Not Defined";
            }

            // Allow Startup Key?
            try
            {
                int regValue = (int)subKey.GetValue("UsePartialEncryptionKey");
                switch (regValue)
                {
                    case 0:
                        {
                            vistaStartupKey = "Forbidden";
                            break;
                        }
                    case 1:
                        {
                            vistaStartupKey = "Mandatory";
                            break;
                        }
                    case 2:
                        {
                            vistaStartupKey = "Allow User to Create or Skip";
                            break;
                        }
                    default:
                        {
                            vistaStartupKey = "Allow User to Create or Skip";
                            break;
                        }
                }
            }
            catch
            {
                vistaStartupKey = "Allow User to Create or Skip";
            }

            // Allow Startup PIN?
            try
            {
                int regValue = (int)subKey.GetValue("UsePIN");
                switch (regValue)
                {
                    case 0:
                        {
                            vistaStartupPin = "Forbidden";
                            break;
                        }
                    case 1:
                        {
                            vistaStartupPin = "Mandatory";
                            break;
                        }
                    case 2:
                        {
                            vistaStartupPin = "Allow User to Create or Skip";
                            break;
                        }
                    default:
                        {
                            vistaStartupPin = "Allow User to Create or Skip";
                            break;
                        }
                }
            }
            catch
            {
                vistaStartupPin = "Allow User to Create or Skip";
            }

            // Require Key Escrow?
            try
            {
                int regValue = (int)subKey.GetValue("RequireActiveDirectoryBackup");
                switch (regValue)
                {
                    case 0:
                        {
                            vistaRequireKeyEscrow = "Not Required";
                            break;
                        }
                    case 1:
                        {
                            vistaRequireKeyEscrow = "Required";
                            break;
                        }
                    default:
                        {
                            vistaRequireKeyEscrow = "Not Defined";
                            break;
                        }
                }
            }
            catch
            {
                vistaRequireKeyEscrow = "Not Defined";
            }

            // Allow without TPM?
            try
            {
                int regValue = (int)subKey.GetValue("EnableNonTPM");
                switch (regValue)
                {
                    case 0:
                        {
                            vistaAllowWithoutTpm = "No";
                            vistaAllowNonTpm = false;
                            break;
                        }
                    case 1:
                        {
                            vistaAllowWithoutTpm = "Yes";
                            vistaAllowNonTpm = true;
                            break;
                        }
                    default:
                        {
                            vistaAllowWithoutTpm = "No (Not Defined)";
                            vistaAllowNonTpm = false;
                            break;
                        }
                }
            }
            catch
            {
                vistaAllowWithoutTpm = "No (Not Defined)";
                vistaAllowNonTpm = false;
            }

            // Save in default folder.
            try
            {
                String path = (String)subKey.GetValue("DefaultRecoveryFolderPath");
                if (path == null || path.Trim() == String.Empty)
                {
                    allDefaultFolder = "Not Defined";
                }
                else
                {
                    allDefaultFolder = path;
                }
            }
            catch
            {
                allDefaultFolder = "Not Defined";
            }

            // Required Encryption Method
            try
            {
                int regValue = (int)subKey.GetValue("EncryptionMethod");
                switch (regValue)
                {
                    case 0:
                        {
                            allRequiredEncryptionMethod = "Default AES 128-bit with Diffuser; User May Change";
                            method = EncryptionMethod.DEFAULT_METHOD;
                            break;
                        }
                    case 1:
                        {
                            allRequiredEncryptionMethod = "Enforced AES 128-bit with Diffuser; User May Not Change";
                            method = EncryptionMethod.AES_128_DIFFUSER;
                            break;
                        }
                    case 2:
                        {
                            allRequiredEncryptionMethod = "Enforced AES 256-bit with Diffuser; User May Not Change";
                            method = EncryptionMethod.AES_256_DIFFUSER;
                            break;
                        }
                    case 3:
                        {
                            allRequiredEncryptionMethod = "Enforced AES 128-bit; User May Not Change";
                            method = EncryptionMethod.AES_128;
                            break;
                        }
                    case 4:
                        {
                            allRequiredEncryptionMethod = "Enforced AES 256-bit; User May Not Change";
                            method = EncryptionMethod.AES_256;
                            break;
                        }
                    default:
                        {
                            allRequiredEncryptionMethod = "Default AES 128-bit with Diffuser; User May Change";
                            method = EncryptionMethod.DEFAULT_METHOD;
                            break;
                        }
                }
            }
            catch
            {
                allRequiredEncryptionMethod = "Default AES 128-bit with Diffuser; User May Change";
                method = EncryptionMethod.DEFAULT_METHOD;
            }

            // Allow BEK?
            try
            {
                int regValue = (int)subKey.GetValue("UseRecoveryDrive");
                switch (regValue)
                {
                    case 0:
                        {
                            vistaAllAllowBek = "Forbidden";
                            vistaAllowBek = false;
                            vistaRequireBek = false;
                            break;
                        }
                    case 1:
                        {
                            vistaAllAllowBek = "Mandatory";
                            vistaAllowBek = true;
                            vistaRequireBek = true;
                            break;
                        }
                    default:
                        {
                            vistaAllAllowBek = "User Choice";
                            vistaAllowBek = true;
                            vistaRequireBek = false;
                            break;
                        }
                }
            }
            catch
            {
                vistaAllAllowBek = "User Choice";
                vistaAllowBek = true;
                vistaRequireBek = false;
            }

            // Allow Password?
            try
            {
                int regValue = (int)subKey.GetValue("UseRecoveryPassword");
                switch (regValue)
                {
                    case 0:
                        {
                            vistaAllAllowPassword = "Forbidden";
                            vistaAllowNumericPw = false;
                            vistaRequireNumericPw = false;
                            break;
                        }
                    case 1:
                        {
                            vistaAllAllowPassword = "Mandatory";
                            vistaAllowNumericPw = true;
                            vistaRequireNumericPw = true;
                            break;
                        }
                    default:
                        {
                            vistaAllAllowPassword = "User Choice";
                            vistaAllowNumericPw = true;
                            vistaRequireNumericPw = false;
                            break;
                        }
                }
            }
            catch
            {
                vistaAllAllowPassword = "User Choice";
                vistaAllowNumericPw = true;
                vistaRequireNumericPw = false;
            }

            // Overwrite Memory on Reboot?
            try
            {
                int regValue = (int)subKey.GetValue("MorBehavior");
                switch (regValue)
                {
                    case 0:
                        {
                            allMemoryOverwrite = "Overwrite (recommended)";
                            break;
                        }
                    case 1:
                        {
                            allMemoryOverwrite = "Blocked (not recommended)";
                            break;
                        }
                    default:
                        {
                            allMemoryOverwrite = "Overwrite (recommended)";
                            break;
                        }
                }
            }
            catch
            {
                allMemoryOverwrite = "Overwrite (recommended)";
            }

            // BEGIN WINDOWS 7/SERVER 2008 ONLY SECTION
            // Organization BitLocker identification field.
            try
            {
                String idString = (String)subKey.GetValue("IdentificationFieldString");
                if (idString == null || idString.Trim() == String.Empty)
                {
                    identificationFieldString = String.Empty;
                }
                else
                {
                    identificationFieldString = idString;
                }
            }
            catch
            {
                identificationFieldString = String.Empty;
            }

            // Organization secondary/allowed identification field.
            try
            {
                String idString = (String)subKey.GetValue("SecondaryIdentificationField");
                if (idString == null || idString.Trim() == String.Empty)
                {
                    secondaryIdentificationField = "Not Defined";
                }
                else
                {
                    secondaryIdentificationField = idString;
                }
            }
            catch
            {
                secondaryIdentificationField = "Exception";
            }

            // Certificate OID.
            try
            {
                String oid = (String)subKey.GetValue("CertificateOID");
                if (oid == null || oid.Trim() == String.Empty)
                {
                    certificateOid = "1.3.6.1.4.1.311.67.1.1";
                }
                else
                {
                    certificateOid = oid;
                }
            }
            catch
            {
                certificateOid = "1.3.6.1.4.1.311.67.1.1";
            }

            // FDV
            // Allow smart cards
            try
            {
                int regValue = (int)subKey.GetValue("FDVAllowUserCert");
                switch (regValue)
                {
                    case 0:
                        {
                            fdvAllowUserCert = "Forbidden";
                            win7FdvAllowUserCert = false;
                            break;
                        }
                    case 1:
                        {
                            fdvAllowUserCert = "Allowed";
                            win7FdvAllowUserCert = true;
                            break;
                        }
                    default:
                        {
                            fdvAllowUserCert = "Allowed";
                            win7FdvAllowUserCert = true;
                            break;
                        }
                }
            }
            catch
            {
                fdvAllowUserCert = "Allowed";
                win7FdvAllowUserCert = true;
            }

            // Require smart cards
            try
            {
                int regValue = (int)subKey.GetValue("FDVEnforceUserCert");
                switch (regValue)
                {
                    case 0:
                        {
                            fdvEnforceUserCert = "Optional";
                            win7FdvEnforceUserCert = false;
                            break;
                        }
                    case 1:
                        {
                            fdvEnforceUserCert = "Mandatory";
                            win7FdvEnforceUserCert = true;
                            break;
                        }
                    default:
                        {
                            fdvEnforceUserCert = "Not Defined";
                            win7FdvEnforceUserCert = false;
                            break;
                        }
                }
            }
            catch
            {
                fdvEnforceUserCert = "Not Defined";
                win7FdvEnforceUserCert = false;
            }

            // Deny FDV write access to unencrypted drive
            try
            {
                int regValue = (int)ccsSubKey.GetValue("FDVDenyWriteAccess");
                switch (regValue)
                {
                    case 0:
                        {
                            fdvDenyWriteAccess = "Read-Write";
                            break;
                        }
                    case 1:
                        {
                            fdvDenyWriteAccess = "Read-Only";
                            break;
                        }
                    default:
                        {
                            fdvDenyWriteAccess = "Read-Write";
                            break;
                        }
                }
            }
            catch
            {
                fdvDenyWriteAccess = "Read-Write";
            }

            // Discovery Volume type.
            try
            {
                String dvType = (String)subKey.GetValue("FDVDiscoveryVolumeType");
                if (dvType == null || dvType.Trim() != String.Empty)
                {
                    fdvDiscoveryVolumeType = "FAT32";
                    win7FdvCreateDiscoveryVolume = true;
                }
                else
                {
                    fdvDiscoveryVolumeType = "Not Created";
                    win7FdvCreateDiscoveryVolume = false;
                }
            }
            catch
            {
                fdvDiscoveryVolumeType = "FAT32";
                win7FdvCreateDiscoveryVolume = true;
            }

            // Install BitLocker to Go Reader
            try
            {
                if (fdvDiscoveryVolumeType == "Not Created")
                {
                    fdvNoBitLockerToGoReader = "Do Not Install";
                }
                else
                {
                    int regValue = (int)subKey.GetValue("FDVNoBitLockerToGoReader");
                    switch (regValue)
                    {
                        case 0:
                            {
                                fdvNoBitLockerToGoReader = "Install";
                                break;
                            }
                        case 1:
                            {
                                fdvNoBitLockerToGoReader = "Do Not Install";
                                break;
                            }
                        default:
                            {
                                fdvNoBitLockerToGoReader = "Install";
                                break;
                            }
                    }
                }
            }
            catch
            {
                fdvNoBitLockerToGoReader = "Install";
            }

            // Enforce passphrase.
            try
            {
                int regValue = (int)subKey.GetValue("FDVEnforcePassphrase");
                switch (regValue)
                {
                    case 0:
                        {
                            fdvEnforcePassphrase = "Optional";
                            win7FdvRequirePassphrase = false;
                            break;
                        }
                    case 1:
                        {
                            fdvEnforcePassphrase = "Mandatory";
                            win7FdvRequirePassphrase = true;
                            break;
                        }
                    default:
                        {
                            fdvEnforcePassphrase = "Optional";
                            win7FdvRequirePassphrase = false;
                            break;
                        }
                }
            }
            catch
            {
                fdvEnforcePassphrase = "Optional";
                win7FdvRequirePassphrase = false;
            }

            // Enable passphrase.
            try
            {
                int regValue = (int)subKey.GetValue("FDVPassphrase");
                switch (regValue)
                {
                    case 0:
                        {
                            fdvPassphrase = "Forbidden";
                            win7FdvAllowPassphrase = true;
                            break;
                        }
                    case 1:
                        {
                            fdvPassphrase = "Allowed with Requirements";
                            win7FdvAllowPassphrase = true;
                            break;
                        }
                    default:
                        {
                            fdvPassphrase = "Allowed and Unrestricted";
                            win7FdvAllowPassphrase = true;
                            break;
                        }
                }
            }
            catch
            {
                fdvPassphrase = "Allowed and Unrestricted";
                win7FdvAllowPassphrase = true;
            }

            // Passphrase complexity.
            try
            {
                int regValue = (int)subKey.GetValue("FDVPassphraseComplexity");
                switch (regValue)
                {
                    case 0:
                        {
                            fdvPassphraseComplexity = "No Enforcement";
                            break;
                        }
                    case 1:
                        {
                            fdvPassphraseComplexity = "Enforced";
                            break;
                        }
                    case 2:
                        {
                            fdvPassphraseComplexity = "Used if Available";
                            break;
                        }
                    default:
                        {
                            fdvPassphraseComplexity = "No Enforcement";
                            break;
                        }
                }
            }
            catch
            {
                fdvPassphraseComplexity = "No Enforcement";
            }

            // Passphrase length.
            try
            {
                int regValue = (int)subKey.GetValue("FDVPassphraseLength");
                fdvPassphraseLength = regValue.ToString();
                win7FdvMinimumPassphrase = regValue;
            }
            catch
            {
                fdvPassphraseLength = "8";
                win7FdvMinimumPassphrase = 8;
            }

            // Perform Key Escrow?
            try
            {
                int regValue = (int)subKey.GetValue("FDVActiveDirectoryBackup");
                switch (regValue)
                {
                    case 0:
                        {
                            fdvActiveDirectoryBackup = "No";
                            break;
                        }
                    case 1:
                        {
                            fdvActiveDirectoryBackup = "Yes";
                            break;
                        }
                    default:
                        {
                            fdvActiveDirectoryBackup = "No (Not Defined)";
                            break;
                        }
                }
            }
            catch
            {
                fdvActiveDirectoryBackup = "No (Not Defined)";
            }

            // What to Back Up
            try
            {
                int regValue = (int)subKey.GetValue("FDVActiveDirectoryInfoToStore");
                switch (regValue)
                {
                    case 1:
                        {
                            fdvActiveDirectoryInfoToStore = "Passwords and Key Packages";
                            break;
                        }
                    case 2:
                        {
                            fdvActiveDirectoryInfoToStore = "Passwords Only";
                            break;
                        }
                    default:
                        {
                            fdvActiveDirectoryInfoToStore = "Not Storing in AD";
                            break;
                        }
                }
            }
            catch
            {
                fdvActiveDirectoryInfoToStore = "Not Storing in AD";
            }

            // Hide recovery page
            try
            {
                int regValue = (int)subKey.GetValue("FDVHideRecoveryPage");
                switch (regValue)
                {
                    case 0:
                        {
                            fdvHideRecoveryPage = "Displayed";
                            win7FdvHideRecoveryPage = false;
                            break;
                        }
                    case 1:
                        {
                            fdvHideRecoveryPage = "Hidden";
                            win7FdvHideRecoveryPage = true;
                            break;
                        }
                    default:
                        {
                            fdvHideRecoveryPage = "Displayed";
                            win7FdvHideRecoveryPage = false;
                            break;
                        }
                }
            }
            catch
            {
                fdvHideRecoveryPage = "Displayed";
                win7FdvHideRecoveryPage = false;
            }

            // Manage DRA
            try
            {
                int regValue = (int)subKey.GetValue("FDVManageDRA");
                switch (regValue)
                {
                    case 0:
                        {
                            fdvManageDra = "Forbidden";
                            break;
                        }
                    case 1:
                        {
                            fdvManageDra = "Allowed";
                            break;
                        }
                    default:
                        {
                            fdvManageDra = "Allowed";
                            break;
                        }
                }
            }
            catch
            {
                fdvManageDra = "Allowed";
            }

            // FDV Recovery Options
            try
            {
                int regValue = (int)subKey.GetValue("FDVRecovery");
                switch (regValue)
                {
                    case 0:
                        {
                            fdvRecovery = "Disabled; Defaults Used";
                            break;
                        }
                    case 1:
                        {
                            fdvRecovery = "Enabled; Administrator Defined";
                            break;
                        }
                    default:
                        {
                            fdvRecovery = "Not Configured; Defaults Used";
                            break;
                        }
                }
            }
            catch
            {
                fdvRecovery = "Not Configured; Defaults Used";
            }

            // Allow Recovery Key?
            try
            {
                int regValue = (int)subKey.GetValue("FDVRecoveryKey");
                switch (regValue)
                {
                    case 0:
                        {
                            fdvRecoveryKey = "Forbidden";
                            win7FdvAllowBek = false;
                            win7FdvRequireBek = false;
                            break;
                        }
                    case 1:
                        {
                            fdvRecoveryKey = "Mandatory";
                            win7FdvAllowBek = true;
                            win7FdvRequireBek = true;
                            break;
                        }
                    case 2:
                        {
                            fdvRecoveryKey = "Allow User to Create or Skip";
                            win7FdvAllowBek = true;
                            win7FdvRequireBek = false;
                            break;
                        }
                    default:
                        {
                            fdvRecoveryKey = "Allow User to Create or Skip";
                            win7FdvAllowBek = true;
                            win7FdvRequireBek = false;
                            break;
                        }
                }
            }
            catch
            {
                fdvRecoveryKey = "Allow User to Create or Skip";
                win7FdvAllowBek = true;
                win7FdvRequireBek = false;
            }

            // Allow Recovery Password?
            try
            {
                int regValue = (int)subKey.GetValue("FDVRecoveryPassword");
                switch (regValue)
                {
                    case 0:
                        {
                            fdvRecoveryPassword = "Forbidden";
                            win7FdvAllowNumericPw = false;
                            win7FdvRequireNumericPw = false;
                            break;
                        }
                    case 1:
                        {
                            fdvRecoveryPassword = "Mandatory";
                            win7FdvAllowNumericPw = true;
                            win7FdvRequireNumericPw = true;
                            break;
                        }
                    case 2:
                        {
                            fdvRecoveryPassword = "Allow User to Create or Skip";
                            win7FdvAllowNumericPw = true;
                            win7FdvRequireNumericPw = false;
                            break;
                        }
                    default:
                        {
                            fdvRecoveryPassword = "Allow User to Create or Skip";
                            win7FdvAllowNumericPw = true;
                            win7FdvRequireNumericPw = false;
                            break;
                        }
                }
            }
            catch
            {
                fdvRecoveryPassword = "Allow User to Create or Skip";
                win7FdvAllowNumericPw = true;
                win7FdvRequireNumericPw = false;
            }

            // Require Key Escrow?
            try
            {
                int regValue = (int)subKey.GetValue("FDVRequireActiveDirectoryBackup");
                switch (regValue)
                {
                    case 0:
                        {
                            fdvRequireActiveDirectoryBackup = "Not Required";
                            break;
                        }
                    case 1:
                        {
                            fdvRequireActiveDirectoryBackup = "Required";
                            break;
                        }
                    default:
                        {
                            fdvRequireActiveDirectoryBackup = "Not Defined";
                            break;
                        }
                }
            }
            catch
            {
                fdvRequireActiveDirectoryBackup = "Not Defined";
            }

            // RDV
            // Allow BDE
            try
            {
                int regValue = (int)subKey.GetValue("RDVAllowBDE");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvAllowBde = "Forbidden";
                            win7RdvAllowRdv = false;
                            break;
                        }
                    case 1:
                        {
                            rdvAllowBde = "Allowed";
                            win7RdvAllowRdv = true;
                            break;
                        }
                    default:
                        {
                            rdvAllowBde = "Allowed";
                            win7RdvAllowRdv = true;
                            break;
                        }
                }
            }
            catch
            {
                rdvAllowBde = "Allowed";
                win7RdvAllowRdv = true;
            }

            // Allow configuration of BDE
            try
            {
                int regValue = (int)subKey.GetValue("RDVConfigureBDE");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvConfigureBde = "Forbidden";
                            win7RdvAllowRdv = true;
                            break;
                        }
                    case 1:
                        {
                            rdvConfigureBde = "Allowed";
                            break;
                        }
                    default:
                        {
                            rdvConfigureBde = "Allowed";
                            break;
                        }
                }
            }
            catch
            {
                rdvConfigureBde = "Allowed";
            }

            // Allow suspension of BDE
            try
            {
                int regValue = (int)subKey.GetValue("RDVDisableBDE");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvDisableBde = "Forbidden";
                            win7RdvAllowSuspend = false;
                            break;
                        }
                    case 1:
                        {
                            rdvDisableBde = "Allowed";
                            win7RdvAllowSuspend = true;
                            break;
                        }
                    default:
                        {
                            rdvDisableBde = "Allowed";
                            win7RdvAllowSuspend = true;
                            break;
                        }
                }
            }
            catch
            {
                rdvDisableBde = "Allowed";
                win7RdvAllowSuspend = true;
            }

            // Allow smart cards
            try
            {
                int regValue = (int)subKey.GetValue("RDVAllowUserCert");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvAllowUserCert = "Forbidden";
                            win7RdvAllowUserCert = false;
                            break;
                        }
                    case 1:
                        {
                            rdvAllowUserCert = "Allowed";
                            win7RdvAllowUserCert = true;
                            break;
                        }
                    default:
                        {
                            rdvAllowUserCert = "Allowed";
                            win7RdvAllowUserCert = true;
                            break;
                        }
                }
            }
            catch
            {
                rdvAllowUserCert = "Allowed";
                win7RdvAllowUserCert = true;
            }

            // Require smart cards
            try
            {
                int regValue = (int)subKey.GetValue("RDVEnforceUserCert");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvEnforceUserCert = "Optional";
                            win7RdvEnforceUserCert = false;
                            break;
                        }
                    case 1:
                        {
                            rdvEnforceUserCert = "Mandatory";
                            win7RdvEnforceUserCert = true;
                            break;
                        }
                    default:
                        {
                            rdvEnforceUserCert = "Not Defined";
                            win7RdvEnforceUserCert = false;
                            break;
                        }
                }
            }
            catch
            {
                rdvEnforceUserCert = "Not Defined";
                win7RdvEnforceUserCert = false;
            }

            // Deny RDV write access to unencrypted drive
            try
            {
                int regValue = (int)ccsSubKey.GetValue("RDVDenyWriteAccess");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvDenyWriteAccess = "Read-Write";
                            break;
                        }
                    case 1:
                        {
                            rdvDenyWriteAccess = "Read-Only";
                            break;
                        }
                    default:
                        {
                            rdvDenyWriteAccess = "Read-Write";
                            break;
                        }
                }
            }
            catch
            {
                rdvDenyWriteAccess = "Read-Write";
            }

            // Deny RDV write access to encrypted drive from another organization.
            try
            {
                int regValue = (int)ccsSubKey.GetValue("RDVDenyCrossOrg");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvDenyCrossOrg = "Read-Write";
                            break;
                        }
                    case 1:
                        {
                            rdvDenyCrossOrg = "Read-Only";
                            break;
                        }
                    default:
                        {
                            rdvDenyCrossOrg = "Read-Write";
                            break;
                        }
                }
            }
            catch
            {
                rdvDenyCrossOrg = "Read-Write";
            }

            // Discovery Volume type.
            try
            {
                String dvType = (String)subKey.GetValue("RDVDiscoveryVolumeType");
                if (dvType == null || dvType.Trim() != String.Empty)
                {
                    rdvDiscoveryVolumeType = "FAT32";
                    win7RdvCreateDiscoveryVolume = true;
                }
                else
                {
                    rdvDiscoveryVolumeType = "Not Created";
                    win7RdvCreateDiscoveryVolume = false;
                }
            }
            catch
            {
                rdvDiscoveryVolumeType = "FAT32";
                win7RdvCreateDiscoveryVolume = true;
            }

            // Install BitLocker to Go Reader
            try
            {
                if (rdvDiscoveryVolumeType == "Not Created")
                {
                    rdvNoBitLockerToGoReader = "Do Not Install";
                }
                else
                {
                    int regValue = (int)subKey.GetValue("RDVNoBitLockerToGoReader");
                    switch (regValue)
                    {
                        case 0:
                            {
                                rdvNoBitLockerToGoReader = "Install";
                                break;
                            }
                        case 1:
                            {
                                rdvNoBitLockerToGoReader = "Do Not Install";
                                break;
                            }
                        default:
                            {
                                rdvNoBitLockerToGoReader = "Install";
                                break;
                            }
                    }
                }
            }
            catch
            {
                rdvNoBitLockerToGoReader = "Install";
            }

            // Enforce passphrase.
            try
            {
                int regValue = (int)subKey.GetValue("RDVEnforcePassphrase");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvEnforcePassphrase = "Optional";
                            win7RdvRequirePassphrase = false;
                            break;
                        }
                    case 1:
                        {
                            rdvEnforcePassphrase = "Mandatory";
                            win7RdvRequirePassphrase = true;
                            break;
                        }
                    default:
                        {
                            rdvEnforcePassphrase = "Optional";
                            win7RdvRequirePassphrase = false;
                            break;
                        }
                }
            }
            catch
            {
                rdvEnforcePassphrase = "Optional";
                win7RdvRequirePassphrase = false;
            }

            // Enable passphrase.
            try
            {
                int regValue = (int)subKey.GetValue("RDVPassphrase");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvPassphrase = "Forbidden";
                            win7RdvAllowPassphrase = false;
                            break;
                        }
                    case 1:
                        {
                            rdvPassphrase = "Allowed with Requirements";
                            win7RdvAllowPassphrase = true; ;
                            break;
                        }
                    default:
                        {
                            rdvPassphrase = "Allowed and Unrestricted";
                            win7RdvAllowPassphrase = true;
                            break;
                        }
                }
            }
            catch
            {
                rdvPassphrase = "Allowed and Unrestricted";
                win7RdvAllowPassphrase = true;
            }

            // Passphrase complexity.
            try
            {
                int regValue = (int)subKey.GetValue("RDVPassphraseComplexity");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvPassphraseComplexity = "No Enforcement";
                            break;
                        }
                    case 1:
                        {
                            rdvPassphraseComplexity = "Enforced";
                            break;
                        }
                    case 2:
                        {
                            rdvPassphraseComplexity = "Used if Available";
                            break;
                        }
                    default:
                        {
                            rdvPassphraseComplexity = "No Enforcement";
                            break;
                        }
                }
            }
            catch
            {
                rdvPassphraseComplexity = "No Enforcement";
            }

            // Passphrase length.
            try
            {
                int regValue = (int)subKey.GetValue("RDVPassphraseLength");
                rdvPassphraseLength = regValue.ToString();
                win7RdvMinimumPassphrase = regValue;
            }
            catch
            {
                rdvPassphraseLength = "8";
                win7RdvMinimumPassphrase = 8;
            }

            // Perform Key Escrow?
            try
            {
                int regValue = (int)subKey.GetValue("RDVActiveDirectoryBackup");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvActiveDirectoryBackup = "No";
                            break;
                        }
                    case 1:
                        {
                            rdvActiveDirectoryBackup = "Yes";
                            break;
                        }
                    default:
                        {
                            rdvActiveDirectoryBackup = "No (Not Defined)";
                            break;
                        }
                }
            }
            catch
            {
                rdvActiveDirectoryBackup = "No (Not Defined)";
            }

            // What to Back Up
            try
            {
                int regValue = (int)subKey.GetValue("RDVActiveDirectoryInfoToStore");
                switch (regValue)
                {
                    case 1:
                        {
                            rdvActiveDirectoryInfoToStore = "Passwords and Key Packages";
                            break;
                        }
                    case 2:
                        {
                            rdvActiveDirectoryInfoToStore = "Passwords Only";
                            break;
                        }
                    default:
                        {
                            rdvActiveDirectoryInfoToStore = "Not Storing in AD";
                            break;
                        }
                }
            }
            catch
            {
                rdvActiveDirectoryInfoToStore = "Not Storing in AD";
            }

            // Hide recovery page
            try
            {
                int regValue = (int)subKey.GetValue("RDVHideRecoveryPage");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvHideRecoveryPage = "Displayed";
                            win7RdvHideRecoveryPage = false;
                            break;
                        }
                    case 1:
                        {
                            rdvHideRecoveryPage = "Hidden";
                            win7RdvHideRecoveryPage = true;
                            break;
                        }
                    default:
                        {
                            rdvHideRecoveryPage = "Displayed";
                            win7RdvHideRecoveryPage = false;
                            break;
                        }
                }
            }
            catch
            {
                rdvHideRecoveryPage = "Displayed";
                win7RdvHideRecoveryPage = false;
            }

            // Manage DRA
            try
            {
                int regValue = (int)subKey.GetValue("RDVManageDRA");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvManageDra = "Forbidden";
                            break;
                        }
                    case 1:
                        {
                            rdvManageDra = "Allowed";
                            break;
                        }
                    default:
                        {
                            rdvManageDra = "Allowed";
                            break;
                        }
                }
            }
            catch
            {
                rdvManageDra = "Allowed";
            }

            // RDV Recovery Options
            try
            {
                int regValue = (int)subKey.GetValue("RDVRecovery");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvRecovery = "Disabled; Defaults Used";
                            break;
                        }
                    case 1:
                        {
                            rdvRecovery = "Enabled; Administrator Defined";
                            break;
                        }
                    default:
                        {
                            rdvRecovery = "Not Configured; Defaults Used";
                            break;
                        }
                }
            }
            catch
            {
                rdvRecovery = "Not Configured; Defaults Used";
            }

            // Allow Recovery Key?
            try
            {
                int regValue = (int)subKey.GetValue("RDVRecoveryKey");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvRecoveryKey = "Forbidden";
                            win7RdvAllowBek = false;
                            win7RdvRequireBek = false;
                            break;
                        }
                    case 1:
                        {
                            rdvRecoveryKey = "Mandatory";
                            win7RdvAllowBek = true;
                            win7RdvRequireBek = true;
                            break;
                        }
                    case 2:
                        {
                            rdvRecoveryKey = "Allow User to Create or Skip";
                            win7RdvAllowBek = true;
                            win7RdvRequireBek = false;
                            break;
                        }
                    default:
                        {
                            rdvRecoveryKey = "Allow User to Create or Skip";
                            win7RdvAllowBek = true;
                            win7RdvRequireBek = false;
                            break;
                        }
                }
            }
            catch
            {
                rdvRecoveryKey = "Allow User to Create or Skip";
                win7RdvAllowBek = true;
                win7RdvRequireBek = false;
            }

            // Allow Recovery Password?
            try
            {
                int regValue = (int)subKey.GetValue("RDVRecoveryPassword");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvRecoveryPassword = "Forbidden";
                            win7RdvAllowNumericPw = false;
                            win7RdvRequireNumericPw = false;
                            break;
                        }
                    case 1:
                        {
                            rdvRecoveryPassword = "Mandatory";
                            win7RdvAllowNumericPw = true;
                            win7RdvRequireNumericPw = true;
                            break;
                        }
                    case 2:
                        {
                            rdvRecoveryPassword = "Allow User to Create or Skip";
                            win7RdvAllowNumericPw = true;
                            win7RdvRequireNumericPw = false;
                            break;
                        }
                    default:
                        {
                            rdvRecoveryPassword = "Allow User to Create or Skip";
                            win7RdvAllowNumericPw = true;
                            win7RdvRequireNumericPw = false;
                            break;
                        }
                }
            }
            catch
            {
                rdvRecoveryPassword = "Allow User to Create or Skip";
                win7RdvAllowNumericPw = true;
                win7RdvRequireNumericPw = false;
            }

            // Require Key Escrow?
            try
            {
                int regValue = (int)subKey.GetValue("RDVRequireActiveDirectoryBackup");
                switch (regValue)
                {
                    case 0:
                        {
                            rdvRequireActiveDirectoryBackup = "Not Required";
                            break;
                        }
                    case 1:
                        {
                            rdvRequireActiveDirectoryBackup = "Required";
                            break;
                        }
                    default:
                        {
                            rdvRequireActiveDirectoryBackup = "Not Defined";
                            break;
                        }
                }
            }
            catch
            {
                rdvRequireActiveDirectoryBackup = "Not Defined";
            }

            // OS Volume
            // Require additional authentication.
            try
            {
                int regValue = (int)subKey.GetValue("UseAdvancedStartup");
                switch (regValue)
                {
                    case 0:
                        {
                            useAdvancedStartup = "Basic Only; TPM Required";
                            break;
                        }
                    case 1:
                        {
                            useAdvancedStartup = "Active";
                            break;
                        }
                    default:
                        {
                            useAdvancedStartup = "Basic Only; TPM Required";
                            break;
                        }
                }
            }
            catch
            {
                useAdvancedStartup = "Basic Only; TPM Required";
            }

            // Enable BDE without TPM.
            try
            {
                int regValue = (int)subKey.GetValue("EnableBDEWithNoTPM");
                switch (regValue)
                {
                    case 0:
                        {
                            enableBdeWithNoTpm = "No";
                            win7AllowNonTpm = false;
                            break;
                        }
                    case 1:
                        {
                            enableBdeWithNoTpm = "Yes";
                            win7AllowNonTpm = true;
                            break;
                        }
                    default:
                        {
                            enableBdeWithNoTpm = "No (Not Defined)";
                            win7AllowNonTpm = false;
                            break;
                        }
                }
            }
            catch
            {
                enableBdeWithNoTpm = "No (Not Defined)";
                win7AllowNonTpm = false;
            }

            // Use TPM only.
            try
            {
                int regValue = (int)subKey.GetValue("UseTPM");
                switch (regValue)
                {
                    case 0:
                        {
                            useTpm = "Forbidden";
                            break;
                        }
                    case 1:
                        {
                            useTpm = "Mandatory";
                            break;
                        }
                    case 2:
                        {
                            useTpm = "Allow User to Create or Skip";
                            break;
                        }
                    default:
                        {
                            useTpm = "Allow User to Create or Skip";
                            break;
                        }
                }
            }
            catch
            {
                useTpm = "Allow User to Create or Skip";
            }

            // Use TPM and Key.
            try
            {
                int regValue = (int)subKey.GetValue("UseTPMKey");
                switch (regValue)
                {
                    case 0:
                        {
                            useTpmKey = "Forbidden";
                            break;
                        }
                    case 1:
                        {
                            useTpmKey = "Mandatory";
                            break;
                        }
                    case 2:
                        {
                            useTpmKey = "Allow User to Create or Skip";
                            break;
                        }
                    default:
                        {
                            useTpmKey = "Allow User to Create or Skip";
                            break;
                        }
                }
            }
            catch
            {
                useTpmKey = "Allow User to Create or Skip";
            }

            // Use TPM and Key and PIN.
            try
            {
                int regValue = (int)subKey.GetValue("UseTPMKeyPIN");
                switch (regValue)
                {
                    case 0:
                        {
                            useTpmKeyPin = "Forbidden";
                            break;
                        }
                    case 1:
                        {
                            useTpmKeyPin = "Mandatory";
                            break;
                        }
                    case 2:
                        {
                            useTpmKeyPin = "Allow User to Create or Skip";
                            break;
                        }
                    default:
                        {
                            useTpmKeyPin = "Allow User to Create or Skip";
                            break;
                        }
                }
            }
            catch
            {
                useTpmKeyPin = "Allow User to Create or Skip";
            }

            // Use TPM and PIN.
            try
            {
                int regValue = (int)subKey.GetValue("UseTPMPIN");
                switch (regValue)
                {
                    case 0:
                        {
                            useTpmPin = "Forbidden";
                            break;
                        }
                    case 1:
                        {
                            useTpmPin = "Mandatory";
                            break;
                        }
                    case 2:
                        {
                            useTpmPin = "Allow User to Create or Skip";
                            break;
                        }
                    default:
                        {
                            useTpmPin = "Allow User to Create or Skip";
                            break;
                        }
                }
            }
            catch
            {
                useTpmPin = "Allow User to Create or Skip";
            }

            // Use enhanced PIN.
            try
            {
                int regValue = (int)subKey.GetValue("UseEnhancedPIN");
                switch (regValue)
                {
                    case 0:
                        {
                            useEnhancedPin = "No (Numeric Only)";
                            win7OsAllowEnhancedPin = false;
                            break;
                        }
                    case 1:
                        {
                            useEnhancedPin = "Yes";
                            win7OsAllowEnhancedPin = true;
                            break;
                        }
                    default:
                        {
                            useEnhancedPin = "No (Numeric Only)";
                            win7OsAllowEnhancedPin = false;
                            break;
                        }
                }
            }
            catch
            {
                useEnhancedPin = "No (Numeric Only)";
                win7OsAllowEnhancedPin = false;
            }

            // Minimum PIN length.
            try
            {
                int regValue = (int)subKey.GetValue("MinimumPIN");
                minimumPin = regValue.ToString();
                win7OsMinimumPin = regValue;
            }
            catch
            {
                minimumPin = "4";
                win7OsMinimumPin = 4;
            }

            // Perform Key Escrow?
            try
            {
                int regValue = (int)subKey.GetValue("OSActiveDirectoryBackup");
                switch (regValue)
                {
                    case 0:
                        {
                            osActiveDirectoryBackup = "No";
                            break;
                        }
                    case 1:
                        {
                            osActiveDirectoryBackup = "Yes";
                            break;
                        }
                    default:
                        {
                            osActiveDirectoryBackup = "No (Not Defined)";
                            break;
                        }
                }
            }
            catch
            {
                osActiveDirectoryBackup = "No (Not Defined)";
            }

            // What to Back Up
            try
            {
                int regValue = (int)subKey.GetValue("OSActiveDirectoryInfoToStore");
                switch (regValue)
                {
                    case 1:
                        {
                            osActiveDirectoryInfoToStore = "Passwords and Key Packages";
                            break;
                        }
                    case 2:
                        {
                            osActiveDirectoryInfoToStore = "Passwords Only";
                            break;
                        }
                    default:
                        {
                            osActiveDirectoryInfoToStore = "Not Storing in AD";
                            break;
                        }
                }
            }
            catch
            {
                osActiveDirectoryInfoToStore = "Not Storing in AD";
            }

            // Hide recovery page
            try
            {
                int regValue = (int)subKey.GetValue("OSHideRecoveryPage");
                switch (regValue)
                {
                    case 0:
                        {
                            osHideRecoveryPage = "Displayed";
                            win7OsHideRecoveryPage = false;
                            break;
                        }
                    case 1:
                        {
                            osHideRecoveryPage = "Hidden";
                            win7OsHideRecoveryPage = true;
                            break;
                        }
                    default:
                        {
                            osHideRecoveryPage = "Displayed";
                            win7OsHideRecoveryPage = false;
                            break;
                        }
                }
            }
            catch
            {
                osHideRecoveryPage = "Displayed";
                win7OsHideRecoveryPage = false;
            }

            // Manage DRA
            try
            {
                int regValue = (int)subKey.GetValue("OSManageDRA");
                switch (regValue)
                {
                    case 0:
                        {
                            osManageDra = "Forbidden";
                            break;
                        }
                    case 1:
                        {
                            osManageDra = "Allowed";
                            break;
                        }
                    default:
                        {
                            osManageDra = "Allowed";
                            break;
                        }
                }
            }
            catch
            {
                osManageDra = "Allowed";
            }

            // OS Recovery Options
            try
            {
                int regValue = (int)subKey.GetValue("OSRecovery");
                switch (regValue)
                {
                    case 0:
                        {
                            osRecovery = "Disabled; Defaults Used";
                            break;
                        }
                    case 1:
                        {
                            osRecovery = "Enabled; Administrator Defined";
                            break;
                        }
                    default:
                        {
                            osRecovery = "Not Configured; Defaults Used";
                            break;
                        }
                }
            }
            catch
            {
                osRecovery = "Not Configured; Defaults Used";
            }

            // Allow Recovery Key?
            try
            {
                int regValue = (int)subKey.GetValue("OSRecoveryKey");
                switch (regValue)
                {
                    case 0:
                        {
                            osRecoveryKey = "Forbidden";
                            win7OsAllowBek = false;
                            win7OsRequireBek = false;
                            break;
                        }
                    case 1:
                        {
                            osRecoveryKey = "Mandatory";
                            win7OsAllowBek = true;
                            win7OsRequireBek = true;
                            break;
                        }
                    case 2:
                        {
                            osRecoveryKey = "Allow User to Create or Skip";
                            win7OsAllowBek = true;
                            win7OsRequireBek = false;
                            break;
                        }
                    default:
                        {
                            osRecoveryKey = "Allow User to Create or Skip";
                            win7OsAllowBek = true;
                            win7OsRequireBek = false;
                            break;
                        }
                }
            }
            catch
            {
                osRecoveryKey = "Allow User to Create or Skip";
                win7OsAllowBek = true;
                win7OsRequireBek = false;
            }

            // Allow Recovery Password?
            try
            {
                int regValue = (int)subKey.GetValue("OSRecoveryPassword");
                switch (regValue)
                {
                    case 0:
                        {
                            osRecoveryPassword = "Forbidden";
                            win7OsAllowNumericPw = false;
                            win7OsRequireNumericPw = false;
                            break;
                        }
                    case 1:
                        {
                            osRecoveryPassword = "Mandatory";
                            win7OsAllowNumericPw = true;
                            win7OsRequireNumericPw = true;
                            break;
                        }
                    case 2:
                        {
                            osRecoveryPassword = "Allow User to Create or Skip";
                            win7OsAllowNumericPw = true;
                            win7OsRequireNumericPw = false;
                            break;
                        }
                    default:
                        {
                            osRecoveryPassword = "Allow User to Create or Skip";
                            win7OsAllowNumericPw = true;
                            win7OsRequireNumericPw = false;
                            break;
                        }
                }
            }
            catch
            {
                osRecoveryPassword = "Allow User to Create or Skip";
                win7OsAllowNumericPw = true;
                win7OsRequireNumericPw = false;
            }

            // Require Key Escrow?
            try
            {
                int regValue = (int)subKey.GetValue("OSRequireActiveDirectoryBackup");
                switch (regValue)
                {
                    case 0:
                        {
                            osRequireActiveDirectoryBackup = "Not Required";
                            break;
                        }
                    case 1:
                        {
                            osRequireActiveDirectoryBackup = "Required";
                            break;
                        }
                    default:
                        {
                            osRequireActiveDirectoryBackup = "Not Defined";
                            break;
                        }
                }
            }
            catch
            {
                osRequireActiveDirectoryBackup = "Not Defined";
            }
        }

        // Reset all bools to false
        private static void SetAllFalse()
        {
            isTpmUsable = false;
            vistaAllowNonTpm = false;
            win7AllowNonTpm = false;
            vistaAllowBek = false;
            vistaRequireBek = false;
            vistaAllowNumericPw = false;
            vistaRequireNumericPw = false;
            win7OsAllowBek = false;
            win7OsRequireBek = false;
            win7OsAllowNumericPw = false;
            win7OsRequireNumericPw = false;
            win7OsAllowEnhancedPin = false;
            win7OsHideRecoveryPage = false;
            win7FdvAllowBek = false;
            win7FdvRequireBek = false;
            win7FdvAllowNumericPw = false;
            win7FdvRequireNumericPw = false;
            win7FdvAllowUserCert = false;
            win7FdvEnforceUserCert = false;
            win7FdvAllowPassphrase = false;
            win7FdvRequirePassphrase = false;
            win7FdvCreateDiscoveryVolume = false;
            win7FdvHideRecoveryPage = false;
            win7RdvAllowRdv = false;
            win7RdvAllowBek = false;
            win7RdvRequireBek = false;
            win7RdvAllowNumericPw = false;
            win7RdvRequireNumericPw = false;
            win7RdvAllowUserCert = false;
            win7RdvEnforceUserCert = false;
            win7RdvAllowPassphrase = false;
            win7RdvRequirePassphrase = false;
            win7RdvCreateDiscoveryVolume = false;
            win7RdvHideRecoveryPage = false;
            win7RdvAllowSuspend = false;

            win7OsMinimumPin = 4;
            win7FdvMinimumPassphrase = 8;
            win7RdvMinimumPassphrase = 8;
        }

        /// <summary>
        /// Checks to see if the TPM is enabled and owned.
        /// </summary>
        /// <param name="tpmStatus">String describing TPM state.</param>
        /// <returns>true if TPM is available for use; false in all other cases.</returns>
        private static bool IsTpmInstalledAndActive(out String tpmStatus)
        {
            bool isTpmPresent = false;

            try
            {
                //
                // Accessing TPM-related information
                //
                UInt32 status = 0;
                object[] wmiParams = null;
                // create management class object
                ManagementClass mc = new ManagementClass("/root/CIMv2/Security/MicrosoftTpm:Win32_Tpm");
                //collection to store all management objects
                ManagementObjectCollection moc = mc.GetInstances();
                // Retrieve single instance of WMI management object
                ManagementObjectCollection.ManagementObjectEnumerator moe = moc.GetEnumerator();
                moe.MoveNext();
                ManagementObject mo = (ManagementObject)moe.Current;
                if (null == mo)
                {
                    // Cannot be used in this state
                    isTpmPresent = false;
                    tpmStatus = "Not Present or Not Supported";
                    return isTpmPresent;
                }
                else
                {
                    isTpmPresent = true;
                    tpmStatus = "Present";
                }
                if (isTpmPresent) // Query if TPM is in activated state
                {
                    wmiParams = new object[1];
                    wmiParams[0] = false;
                    status = (UInt32)mo.InvokeMethod("IsEnabled", wmiParams);
                    if (0 != status)
                    {
                        // Cannot be used in this state
                        tpmStatus += " but an error occurred checking its Enabled state: 0x" + status.ToString("X");
                        isTpmPresent = false;
                        return isTpmPresent;
                    }
                    else
                    {
                        if ((bool)wmiParams[0])
                        {
                            tpmStatus += ", Enabled";

                            wmiParams = new object[1];
                            wmiParams[0] = false;
                            status = (UInt32)mo.InvokeMethod("IsOwned", wmiParams);
                            if (0 != status)
                            {
                                // Cannot be used in this state
                                tpmStatus += " but an error occurred checking its Ownership state: 0x" + status.ToString("X");
                                isTpmPresent = false;
                                return isTpmPresent;
                            }
                            else
                            {
                                if ((bool)wmiParams[0])
                                {
                                    tpmStatus += " and ownership has been taken";
                                    isTpmPresent = true;
                                }
                                else
                                {
                                    tpmStatus += " but ownership has not been taken";
                                    isTpmPresent = false;
                                }
                            }
                        }
                        else
                        {
                            // Cannot be used in this state
                            tpmStatus += " but Disabled";
                            isTpmPresent = false;
                            return isTpmPresent;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tpmStatus = ex.Message;
                isTpmPresent = false;
            }

            return isTpmPresent;
        }

        // PROPERTIES

        public static bool IsTpmUsable
        {
            get
            {
                return isTpmUsable;
            }
        }

        public static bool VistaAllowNonTpm
        {
            get
            {
                return vistaAllowNonTpm;
            }
        }

        public static bool Win7AllowNonTpm
        {
            get
            {
                return win7AllowNonTpm;
            }
        }

        public static bool VistaAllowBek
        {
            get
            {
                return vistaAllowBek;
            }
        }

        public static bool VistaRequireBek
        {
            get
            {
                return vistaRequireBek;
            }
        }

        public static bool VistaAllowNumericPw
        {
            get
            {
                return vistaAllowNumericPw;
            }
        }

        public static bool VistaRequireNumericPw
        {
            get
            {
                return vistaRequireNumericPw;
            }
        }

        public static EncryptionMethod Method
        {
            get
            {
                return method;
            }
        }

        public static String ChancellorOfCherishable
        {
            get
            {
                return princessOfPretty;
            }
        }

        public static bool Win7OsAllowBek
        {
            get
            {
                return win7OsAllowBek;
            }
        }

        public static bool Win7OsRequireBek
        {
            get
            {
                return win7OsRequireBek;
            }
        }

        public static bool Win7OsAllowNumericPw
        {
            get
            {
                return win7OsAllowNumericPw;
            }
        }

        public static bool Win7OsRequireNumericPw
        {
            get
            {
                return win7OsRequireNumericPw;
            }
        }

        public static bool Win7OsAllowEnhancedPin
        {
            get
            {
                return win7OsAllowEnhancedPin;
            }
        }

        public static bool Win7OsHideRecoveryPage
        {
            get
            {
                return win7OsHideRecoveryPage;
            }
        }

        public static bool Win7FdvAllowBek
        {
            get
            {
                return win7FdvAllowBek;
            }
        }

        public static bool Win7FdvRequireBek
        {
            get
            {
                return win7FdvRequireBek;
            }
        }

        public static bool Win7FdvAllowNumericPw
        {
            get
            {
                return win7FdvAllowNumericPw;
            }
        }

        public static bool Win7FdvRequireNumericPw
        {
            get
            {
                return win7FdvRequireNumericPw;
            }
        }

        public static bool Win7FdvAllowUserCert
        {
            get
            {
                return win7FdvAllowUserCert;
            }
        }

        public static bool Win7FdvEnforceUserCert
        {
            get
            {
                return win7FdvEnforceUserCert;
            }
        }

        public static bool Win7FdvAllowPassphrase
        {
            get
            {
                return win7FdvAllowPassphrase;
            }
        }

        public static bool Win7FdvRequirePassphrase
        {
            get
            {
                return win7FdvRequirePassphrase;
            }
        }

        public static bool Win7FdvCreateDiscoveryVolume
        {
            get
            {
                return win7FdvCreateDiscoveryVolume;
            }
        }

        public static bool Win7FdvHideRecoveryPage
        {
            get
            {
                return win7FdvHideRecoveryPage;
            }
        }

        public static bool Win7RdvAllowRdv
        {
            get
            {
                return win7RdvAllowRdv;
            }
        }

        public static bool Win7RdvAllowBek
        {
            get
            {
                return win7RdvAllowBek;
            }
        }

        public static bool Win7RdvRequireBek
        {
            get
            {
                return win7RdvRequireBek;
            }
        }

        public static bool Win7RdvAllowNumericPw
        {
            get
            {
                return win7RdvAllowNumericPw;
            }
        }

        public static bool Win7RdvRequireNumericPw
        {
            get
            {
                return win7RdvRequireNumericPw;
            }
        }

        public static bool Win7RdvAllowUserCert
        {
            get
            {
                return win7RdvAllowUserCert;
            }
        }

        public static bool Win7RdvEnforceUserCert
        {
            get
            {
                return win7RdvEnforceUserCert;
            }
        }

        public static bool Win7RdvAllowPassphrase
        {
            get
            {
                return win7RdvAllowPassphrase;
            }
        }

        public static bool Win7RdvRequirePassphrase
        {
            get
            {
                return win7RdvRequirePassphrase;
            }
        }

        public static bool Win7RdvCreateDiscoveryVolume
        {
            get
            {
                return win7RdvCreateDiscoveryVolume;
            }
        }

        public static bool Win7RdvHideRecoveryPage
        {
            get
            {
                return win7RdvHideRecoveryPage;
            }
        }

        public static bool Win7RdvAllowSuspend
        {
            get
            {
                return win7RdvAllowSuspend;
            }
        }

        public static int Win7OsMinimumPin
        {
            get
            {
                return win7OsMinimumPin;
            }
        }

        public static int Win7FdvMinimumPassphrase
        {
            get
            {
                return win7FdvMinimumPassphrase;
            }
        }

        public static int Win7RdvMinimumPassphrase
        {
            get
            {
                return win7RdvMinimumPassphrase;
            }
        }

        /// <summary>
        /// Gets a flag that indicates whether Federal Information Processing Standard (FIPS) 140
        /// compliance is mandatory. When FIPS compliance is enforced, some BitLocker functionality
        /// is disabled. If the flag is true, FIPS compliance is enforced.
        /// </summary>
        public static bool IsFipsComplianceMandatory
        {
            get
            {
                return isFipsComplianceMandatory;
            }
        }

        /// <summary>
        /// Gets a string that specifies whether Key Escrow to Active Directory is mandatory.
        /// Applies to Vista/Server 2008.
        /// </summary>
        public static String VistaRequireKeyEscrow
        {
            get
            {
                return vistaRequireKeyEscrow;
            }
        }

        /// <summary>
        /// Gets a string that specifies whether keys are being escrowed to Active Directory.
        /// Applies to Vista/Server 2008.
        /// </summary>
        public static String VistaKeyEscrow
        {
            get
            {
                return vistaKeyEscrow;
            }
        }

        /// <summary>
        /// Gets a string that specifies what keys are being escrowed to Active Directory.
        /// Applies to Vista/Server 2008.
        /// </summary>
        public static String VistaKeyStorage
        {
            get
            {
                return vistaKeyStorage;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not BitLocker can be used on an OS volume without a TPM.
        /// Applies to Vista/Server 2008.
        /// </summary>
        public static String VistaAllowWithoutTpm
        {
            get
            {
                return vistaAllowWithoutTpm;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a BEK can be used for TPM startup.
        /// Applies to Vista/Server 2008.
        /// </summary>
        public static String VistaStartupKey
        {
            get
            {
                return vistaStartupKey;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a PIN can be used for TPM startup.
        /// Applies to Vista/Server 2008.
        /// </summary>
        public static String VistaStartupPin
        {
            get
            {
                return vistaStartupPin;
            }
        }

        /// <summary>
        /// Gets a string that specifies the default BEK save location.
        /// </summary>
        public static String AllDefaultFolder
        {
            get
            {
                return allDefaultFolder;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a BEK is allowed to protect an OS volume.
        /// Applies to Vista/Server 2008.
        /// </summary>
        public static String VistaAllAllowBek
        {
            get
            {
                return vistaAllAllowBek;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a 48-digit password is allowed to protect an OS volume.
        /// Applies to Vista/Server 2008.
        /// </summary>
        public static String VistaAllAllowPassword
        {
            get
            {
                return vistaAllAllowPassword;
            }
        }

        /// <summary>
        /// Gets a string that indicates the required encryption method.
        /// </summary>
        public static String AllRequiredEncryptionMethod
        {
            get
            {
                return allRequiredEncryptionMethod;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a memory overwrite occurs at each reboot.
        /// </summary>
        public static String AllMemoryOverwrite
        {
            get
            {
                return allMemoryOverwrite;
            }
        }

        /// <summary>
        /// Gets a string that specifies the organization id.
        /// </summary>
        public static String IdentificationFieldString
        {
            get
            {
                return identificationFieldString;
            }
        }

        /// <summary>
        /// Gets a string that indicates, as a comma-separated list, allowed organizations.
        /// </summary>
        public static String SecondaryIdentificationField
        {
            get
            {
                return secondaryIdentificationField;
            }
        }

        /// <summary>
        /// Gets a string that indicates the certificate OID.
        /// </summary>
        public static String CertificateOid
        {
            get
            {
                return certificateOid;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not user certs are allowed in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvAllowUserCert
        {
            get
            {
                return fdvAllowUserCert;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a user certs are enforced in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvEnforceUserCert
        {
            get
            {
                return fdvEnforceUserCert;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a write access is denied in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvDenyWriteAccess
        {
            get
            {
                return fdvDenyWriteAccess;
            }
        }

        /// <summary>
        /// Gets a string that indicates the Discovery Volume type in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvDiscoveryVolumeType
        {
            get
            {
                return fdvDiscoveryVolumeType;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not the BitLocker To Go Reader is installed in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvNoBitLockerToGoReader
        {
            get
            {
                return fdvNoBitLockerToGoReader;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a passphrase is required in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvEnforcePassphrase
        {
            get
            {
                return fdvEnforcePassphrase;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not passphrase policy is enabled in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvPassphrase
        {
            get
            {
                return fdvPassphrase;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not passphrase complexity is required in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvPassphraseComplexity
        {
            get
            {
                return fdvPassphraseComplexity;
            }
        }

        /// <summary>
        /// Gets a string that specifies the minimum passphrase length in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvPassphraseLength
        {
            get
            {
                return fdvPassphraseLength;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether recovery keys are escrowed to AD in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvActiveDirectoryBackup
        {
            get
            {
                return fdvActiveDirectoryBackup;
            }
        }

        /// <summary>
        /// Gets a string that indicates what is stored in AD in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvActiveDirectoryInfoToStore
        {
            get
            {
                return fdvActiveDirectoryInfoToStore;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether the Recovery Page is hidden in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvHideRecoveryPage
        {
            get
            {
                return fdvHideRecoveryPage;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a DRA is allowed in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvManageDra
        {
            get
            {
                return fdvManageDra;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not the Recovery options are enabled in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvRecovery
        {
            get
            {
                return fdvRecovery;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a Recovery Key is allowed, disallowed or required in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvRecoveryKey
        {
            get
            {
                return fdvRecoveryKey;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a Recovery Password is allowed, disallowed or required in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvRecoveryPassword
        {
            get
            {
                return fdvRecoveryPassword;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether Key Escrow to Active Directory is mandatory in Windows 7/Server 2008 R2.
        /// </summary>
        public static String FdvRequireActiveDirectoryBackup
        {
            get
            {
                return fdvRequireActiveDirectoryBackup;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not users can use BitLocker on RDV in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvAllowBde
        {
            get
            {
                return rdvAllowBde;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not users can configure BitLocker via a wizard in Windows 7/Server 2008 R2..
        /// </summary>
        public static String RdvConfigureBde
        {
            get
            {
                return rdvConfigureBde;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not users can disable BitLocker in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvDisableBde
        {
            get
            {
                return rdvDisableBde;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not user certs are allowed in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvAllowUserCert
        {
            get
            {
                return rdvAllowUserCert;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a user certs are enforced in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvEnforceUserCert
        {
            get
            {
                return rdvEnforceUserCert;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a write access is denied in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvDenyWriteAccess
        {
            get
            {
                return rdvDenyWriteAccess;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether write access is denied to other organization disks in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvDenyCrossOrg
        {
            get
            {
                return rdvDenyCrossOrg;
            }
        }

        /// <summary>
        /// Gets a string that indicates the Discovery Volume type in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvDiscoveryVolumeType
        {
            get
            {
                return rdvDiscoveryVolumeType;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not the BitLocker To Go Reader is installed in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvNoBitLockerToGoReader
        {
            get
            {
                return rdvNoBitLockerToGoReader;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a passphrase is required in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvEnforcePassphrase
        {
            get
            {
                return rdvEnforcePassphrase;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not passphrase policy is enabled in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvPassphrase
        {
            get
            {
                return rdvPassphrase;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not passphrase complexity is required in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvPassphraseComplexity
        {
            get
            {
                return rdvPassphraseComplexity;
            }
        }

        /// <summary>
        /// Gets a string that specifies the minimum passphrase length in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvPassphraseLength
        {
            get
            {
                return rdvPassphraseLength;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether recovery keys are escrowed to AD in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvActiveDirectoryBackup
        {
            get
            {
                return rdvActiveDirectoryBackup;
            }
        }

        /// <summary>
        /// Gets a string that indicates what is stored in AD in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvActiveDirectoryInfoToStore
        {
            get
            {
                return rdvActiveDirectoryInfoToStore;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether the Recovery Page is hidden in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvHideRecoveryPage
        {
            get
            {
                return rdvHideRecoveryPage;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a DRA is allowed in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvManageDra
        {
            get
            {
                return rdvManageDra;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not the Recovery options are enabled in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvRecovery
        {
            get
            {
                return rdvRecovery;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a Recovery Key is allowed, disallowed or required in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvRecoveryKey
        {
            get
            {
                return rdvRecoveryKey;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a Recovery Password is allowed, disallowed or required in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvRecoveryPassword
        {
            get
            {
                return rdvRecoveryPassword;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether Key Escrow to Active Directory is mandatory in Windows 7/Server 2008 R2.
        /// </summary>
        public static String RdvRequireActiveDirectoryBackup
        {
            get
            {
                return rdvRequireActiveDirectoryBackup;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a memory overwrite occurs at each reboot.
        /// </summary>
        public static String UseAdvancedStartup
        {
            get
            {
                return useAdvancedStartup;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a memory overwrite occurs at each reboot.
        /// </summary>
        public static String EnableBdeWithNoTpm
        {
            get
            {
                return enableBdeWithNoTpm;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a TPM only is used on Windows 7/Server 2008 R2.
        /// </summary>
        public static String UseTpm
        {
            get
            {
                return useTpm;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a TPM and Key is used on Windows 7/Server 2008 R2.
        /// </summary>
        public static String UseTpmKey
        {
            get
            {
                return useTpmKey;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a TPM and Key and PIN is used on Windows 7/Server 2008 R2.
        /// </summary>
        public static String UseTpmKeyPin
        {
            get
            {
                return useTpmKeyPin;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a TPM and PIN is used on Windows 7/Server 2008 R2.
        /// </summary>
        public static String UseTpmPin
        {
            get
            {
                return useTpmPin;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not Enhanced PINs are allowed on Windows 7/Server 2008 R2.
        /// </summary>
        public static String UseEnhancedPin
        {
            get
            {
                return useEnhancedPin;
            }
        }

        /// <summary>
        /// Gets a string that indicates the minimum PIN length in Windows 7/Server 2008 R2.
        /// </summary>
        public static String MinimumPin
        {
            get
            {
                return minimumPin;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether recovery keys are escrowed to AD in Windows 7/Server 2008 R2.
        /// </summary>
        public static String OsActiveDirectoryBackup
        {
            get
            {
                return osActiveDirectoryBackup;
            }
        }

        /// <summary>
        /// Gets a string that indicates what is stored in AD in Windows 7/Server 2008 R2.
        /// </summary>
        public static String OsActiveDirectoryInfoToStore
        {
            get
            {
                return osActiveDirectoryInfoToStore;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether the Recovery Page is hidden in Windows 7/Server 2008 R2.
        /// </summary>
        public static String OsHideRecoveryPage
        {
            get
            {
                return osHideRecoveryPage;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a DRA is allowed in Windows 7/Server 2008 R2.
        /// </summary>
        public static String OsManageDra
        {
            get
            {
                return osManageDra;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not the Recovery options are enabled in Windows 7/Server 2008 R2.
        /// </summary>
        public static String OsRecovery
        {
            get
            {
                return osRecovery;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a Recovery Key is allowed, disallowed or required in Windows 7/Server 2008 R2.
        /// </summary>
        public static String OsRecoveryKey
        {
            get
            {
                return osRecoveryKey;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether or not a Recovery Password is allowed, disallowed or required in Windows 7/Server 2008 R2.
        /// </summary>
        public static String OsRecoveryPassword
        {
            get
            {
                return osRecoveryPassword;
            }
        }

        /// <summary>
        /// Gets a string that indicates whether Key Escrow to Active Directory is mandatory in Windows 7/Server 2008 R2.
        /// </summary>
        public static String OsRequireActiveDirectoryBackup
        {
            get
            {
                return osRequireActiveDirectoryBackup;
            }
        }

        /// <summary>
        /// Gets a flag that indicates whether Friend/Family mode is enabled.
        /// </summary>
        public static bool IsFriendOrFamily
        {
            get
            {
                return isFriendFamily;
            }
        }
    }
}
