using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public sealed class DataVolumeEncryptionRequest
    {
        // In Windows Vista/Server 2008, up to 3 keys can possibly be created for a data volume.
        // In Windows 7/Server 2008 R2, there can be two more (password and cert).
        private String volume;
        private String volumeLabel;
        private String fileSystem;
        private bool createRecoveryKey;
        private bool createRecoveryPw;
        private bool createAutoPw;
        private String recoveryPassword;
        private bool createAutoUnlockKey;
        private String recoveryKeyPath;
        private String recoveryPasswordPath;
        private EncryptionMethod method;
        private bool allowPrintingAndSaving;
        private int protectorsToCreate;

        // Windows 7 and later ONLY
        private bool createCertKey;
        private String certThumbprint;
        private bool createPassword;
        private String password;
        private bool createBl2gReader;
        private bool setOrgId;
        private String orgId;

        // Key Prot IDs
        private String recoveryKeyProtectorId;
        private String recoveryPwProtectorId;
        private String autoUnlockProtectorId;
        private String certificateProtectorId;
        private String passwordProtectorId;

        /// <summary>
        /// Initializes a new DataVolumeEncryptionRequest for Windows Vista and Server 2008.
        /// </summary>
        /// <param name="recoveryKey"></param>
        /// <param name="recoveryPw"></param>
        /// <param name="recoveryPath"></param>
        /// <param name="m"></param>
        /// <param name="allowPrint"></param>
        /// <param name="autoUnlock"></param>
        public DataVolumeEncryptionRequest(String vol, String label, String fs, bool recoveryKey, bool recoveryPw, bool autoPw,
            String keyPath, String pwPath, EncryptionMethod m, bool allowPrint, bool autoUnlock)
        {
            volume = vol;
            volumeLabel = label;
            fileSystem = fs;
            createRecoveryKey = recoveryKey;
            createAutoPw = autoPw;
            createRecoveryPw = recoveryPw;
            recoveryKeyPath = keyPath;
            recoveryPasswordPath = pwPath;
            method = m;
            allowPrintingAndSaving = allowPrint;
            createAutoUnlockKey = autoUnlock;
            createCertKey = false;
            certThumbprint = String.Empty;
            createPassword = false;
            password = String.Empty;
            createBl2gReader = false;
            setOrgId = false;
            orgId = String.Empty;

            if (createRecoveryKey)
                protectorsToCreate++;
            if (createRecoveryPw)
                protectorsToCreate++;
            if (createAutoUnlockKey)
                protectorsToCreate++;
            if (createAutoPw)
                protectorsToCreate++;
        }

        /// <summary>
        /// Creates a new data volume encryption request for volumes under Windows 7, Server 2008 R2 and Windows Server Solutions.
        /// </summary>
        /// <param name="vol">Drive letter or Windows physical device ID for letterless drives.</param>
        /// <param name="label">Volume label.</param>
        /// <param name="fs">File system (i.e. NTFS, FAT32).</param>
        /// <param name="recoveryKey">Set true to create a recovery key.</param>
        /// <param name="recoveryPw">Set true to create a recovery password.</param>
        /// <param name="autoPw">Set to true to create an automatic recovery password.</param>
        /// <param name="keyPath">Fully qualified path where the recovery key should be saved.</param>
        /// <param name="pwPath">Fully qualified path where the recovery password should be saved.</param>
        /// <param name="m">Encryption method.</param>
        /// <param name="allowPrint">Set to true to allow the user to print and save the encryption details.</param>
        /// <param name="autoUnlock">Set to true to create an automatic unlocking key.</param>
        /// <param name="createCert">Set to true to create a certificate key protector.</param>
        /// <param name="tp">Certificate thumbprint.</param>
        /// <param name="passwd">Set to true to create an alphanumeric password.</param>
        /// <param name="userPass">The alphanumeric password.</param>
        /// <param name="reader">Set to true to create the BL2G reader on the volume. Not valid for NTFS.</param>
        /// <param name="setOrg">Set to true to set the organization ID field.</param>
        /// <param name="org">Organization ID field to set.</param>
        public DataVolumeEncryptionRequest(String vol, String label, String fs, bool recoveryKey, bool recoveryPw, bool autoPw,
            String keyPath, String pwPath, EncryptionMethod m, bool allowPrint, bool autoUnlock,
            bool createCert, String tp, bool passwd, String userPass, bool reader, bool setOrg, String org)
        {
            volume = vol;
            volumeLabel = label;
            fileSystem = fs;
            createRecoveryKey = recoveryKey;
            createRecoveryPw = recoveryPw;
            createAutoPw = autoPw;
            recoveryKeyPath = keyPath;
            recoveryPasswordPath = pwPath;
            method = m;
            allowPrintingAndSaving = allowPrint;
            createAutoUnlockKey = autoUnlock;
            createCertKey = createCert;
            certThumbprint = tp;
            createPassword = passwd;
            password = userPass;
            createBl2gReader = reader;
            setOrgId = setOrg;
            orgId = org;

            if (createRecoveryKey)
                protectorsToCreate++;
            if (createRecoveryPw)
                protectorsToCreate++;
            if (createAutoUnlockKey)
                protectorsToCreate++;
            if (createAutoPw)
                protectorsToCreate++;
            if (createCertKey)
                protectorsToCreate++;
            if (createPassword)
                protectorsToCreate++;
        }

        // Properties
        public String Volume
        {
            get
            {
                return volume;
            }
        }

        public String VolumeLabel
        {
            get
            {
                return volumeLabel;
            }
        }

        public String FileSystem
        {
            get
            {
                return fileSystem;
            }
        }

        public bool CreateRecoveryKey
        {
            get
            {
                return createRecoveryKey;
            }
        }

        public bool CreateRecoveryPassword
        {
            get
            {
                return createRecoveryPw;
            }
        }

        public bool CreateAutoPassword
        {
            get
            {
                return createAutoPw;
            }
        }

        public String RecoveryKeyPath
        {
            get
            {
                return recoveryKeyPath;
            }
        }

        public String RecoveryPasswordPath
        {
            get
            {
                return recoveryPasswordPath;
            }
        }

        public EncryptionMethod Method
        {
            get
            {
                return method;
            }
        }

        public bool AllowPrintingAndSaving
        {
            get
            {
                return allowPrintingAndSaving;
            }
        }

        public bool CreateAutoUnlockKey
        {
            get
            {
                return createAutoUnlockKey;
            }
        }

        public int ProtectorsToCreate
        {
            get
            {
                return protectorsToCreate;
            }
        }

        public bool CreateCertKey
        {
            get
            {
                return createCertKey;
            }
        }

        public String CertificateThumbprint
        {
            get
            {
                return certThumbprint;
            }
        }

        public bool CreatePassword
        {
            get
            {
                return createPassword;
            }
        }

        public String Password
        {
            get
            {
                return password;
            }
        }

        public bool CreateBl2gReader
        {
            get
            {
                return createBl2gReader;
            }
        }

        public bool SetOrgId
        {
            get
            {
                return setOrgId;
            }
        }

        public String OrgId
        {
            get
            {
                return orgId;
            }
        }

        public String RecoveryKeyProtectorId
        {
            get
            {
                return recoveryKeyProtectorId;
            }
            set
            {
                recoveryKeyProtectorId = value;
            }
        }

        public String RecoveryPasswordProtectorId
        {
            get
            {
                return recoveryPwProtectorId;
            }
            set
            {
                recoveryPwProtectorId = value;
            }
        }

        public String RecoveryPassword
        {
            get
            {
                return recoveryPassword;
            }
            set
            {
                recoveryPassword = value;
            }
        }

        public String AutoUnlockProtectorId
        {
            get
            {
                return autoUnlockProtectorId;
            }
            set
            {
                autoUnlockProtectorId = value;
            }
        }

        public String CertificateProtectorId
        {
            get
            {
                return certificateProtectorId;
            }
            set
            {
                certificateProtectorId = value;
            }
        }

        public String PasswordProtectorId
        {
            get
            {
                return passwordProtectorId;
            }
            set
            {
                passwordProtectorId = value;
            }
        }
    }
}
