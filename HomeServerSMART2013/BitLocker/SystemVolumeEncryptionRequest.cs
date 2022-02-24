using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public sealed class SystemVolumeEncryptionRequest
    {
        // Up to 3 keys can possibly be created for the system volume.
        // In Windows 7/Server 2008 R2, there can be two more (password and cert).
        private String volume;
        private String volumeLabel;
        private bool createRecoveryKey;
        private bool createRecoveryPw;
        private bool createAutoPw;
        private String startupKeyPath;
        private String recoveryPassword;
        private String recoveryKeyPath;
        private String recoveryPasswordPath;
        private EncryptionMethod method;
        private bool allowPrintingAndSaving;
        private int protectorsToCreate;
        private String pin;
        private String tpmType;
        private bool performHardwareTest;
        private bool rebootImmediately;

        // Key Prot IDs
        private String startupKeyProtectorId;
        private String recoveryKeyProtectorId;
        private String recoveryPwProtectorId;

        public SystemVolumeEncryptionRequest(String vol, String label, bool createRecKey, bool createRecPw, bool createAuto,
            String startupPath, String recKeyPath, String recPassPath, EncryptionMethod meth, bool allowPrint,
            String p, String tpm, bool hwTest, bool reboot)
        {
            volume = vol;
            volumeLabel = label;
            createRecoveryKey = createRecKey;
            createRecoveryPw = createRecPw;
            createAutoPw = createAuto;
            recoveryKeyPath = recKeyPath;
            recoveryPasswordPath = recPassPath;
            startupKeyPath = startupPath;
            method = meth;
            allowPrintingAndSaving = allowPrint;
            protectorsToCreate = 0;
            pin = p;
            tpmType = tpm;
            performHardwareTest = hwTest;
            rebootImmediately = reboot;

            if (createRecoveryKey)
                protectorsToCreate++;
            if (createRecoveryPw)
                protectorsToCreate++;
            // Create startup key.
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

        public int ProtectorsToCreate
        {
            get
            {
                return protectorsToCreate;
            }
        }

        public String StartupKeyProtectorId
        {
            get
            {
                return startupKeyProtectorId;
            }
            set
            {
                startupKeyProtectorId = value;
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

        public String Pin
        {
            get
            {
                return pin;
            }
        }

        public String TpmType
        {
            get
            {
                return tpmType;
            }
        }

        public String StartupKeyPath
        {
            get
            {
                return startupKeyPath;
            }
        }

        public bool PerformHardwareTest
        {
            get
            {
                return performHardwareTest;
            }
        }

        public bool RebootImmediately
        {
            get
            {
                return rebootImmediately;
            }
        }
    }
}
