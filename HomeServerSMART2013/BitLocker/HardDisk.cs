using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public sealed class HardDisk
    {
        private string driveLetter;
        private string deviceID;
        private string physicalID;
        private string volumeLabel;
        private string fileSystem;
        private string deviceInterface;
        private string lockStatus;
        private string volumeType;
        private string protectionStatus;
        private string encryptionPercentage;
        private string encryptionStatus;
        private string encryptionMethod;
        private string caption;
        private bool autoUnlockEnabled;
        private decimal capacity;
        private decimal freeSpace;
        private MetadataVersion bitlockerVersion;
        private DiskType diskType;

        public HardDisk()
        {
            driveLetter = String.Empty;
            deviceID = String.Empty;
            physicalID = String.Empty;
            volumeLabel = String.Empty;
            fileSystem = String.Empty;
            deviceInterface = String.Empty;
            lockStatus = String.Empty;
            volumeType = String.Empty;
            protectionStatus = String.Empty;
            encryptionPercentage = String.Empty;
            encryptionStatus = String.Empty;
            encryptionMethod = String.Empty;
            caption = String.Empty;
            capacity = 0.00M;
            freeSpace = 0.00M;
            autoUnlockEnabled = false;
            bitlockerVersion = MetadataVersion.VERSION_UNKNOWN;
            diskType = DiskType.DISK_UNKNOWN;
        }

        public HardDisk(string letter, string interfaceType, string physicalId, string deviceId, string cap)
        {
            driveLetter = letter;
            deviceID = deviceId;
            physicalID = physicalId;
            volumeLabel = String.Empty;
            fileSystem = String.Empty;
            deviceInterface = interfaceType;
            lockStatus = String.Empty;
            volumeType = String.Empty;
            protectionStatus = String.Empty;
            encryptionPercentage = String.Empty;
            encryptionStatus = String.Empty;
            encryptionMethod = String.Empty;
            caption = cap;
            capacity = 0.00M;
            freeSpace = 0.00M;
            autoUnlockEnabled = false;
            bitlockerVersion = MetadataVersion.VERSION_UNKNOWN;
            diskType = DiskType.DISK_UNKNOWN;
        }

        /// <summary>
        /// Gets or sets the volume's drive letter.
        /// </summary>
        public String DriveLetter
        {
            get
            {
                return driveLetter;
            }
            set
            {
                driveLetter = value;
            }
        }

        /// <summary>
        /// Gets or sets the volume's Windows device ID.
        /// </summary>
        public String DeviceID
        {
            get
            {
                return deviceID;
            }
            set
            {
                deviceID = value;
            }
        }

        /// <summary>
        /// Gets or sets the volume's Windows physical ID.
        /// </summary>
        public String PhysicalID
        {
            get
            {
                return physicalID;
            }
            set
            {
                physicalID = value;
            }
        }

        /// <summary>
        /// Gets or sets the volume's drive label.
        /// </summary>
        public String VolumeLabel
        {
            get
            {
                return volumeLabel;
            }
            set
            {
                volumeLabel = value;
            }
        }

        /// <summary>
        /// Gets or sets the volume's file system?
        /// </summary>
        public String FileSystem
        {
            get
            {
                return fileSystem;
            }
            set
            {
                fileSystem = value;
            }
        }

        /// <summary>
        /// Gets or sets the volume's device interface.
        /// </summary>
        public String DeviceInterface
        {
            get
            {
                return deviceInterface;
            }
            set
            {
                deviceInterface = value;
            }
        }

        public String LockStatus
        {
            get
            {
                return lockStatus;
            }
            set
            {
                lockStatus = value;
            }
        }

        public String VolumeType
        {
            get
            {
                return volumeType;
            }
            set
            {
                volumeType = value;
            }
        }

        public String ProtectionStatus
        {
            get
            {
                return protectionStatus;
            }
            set
            {
                protectionStatus = value;
            }
        }

        public String EncryptionPercentage
        {
            get
            {
                return encryptionPercentage;
            }
            set
            {
                encryptionPercentage = value;
            }
        }

        public String EncryptionStatus
        {
            get
            {
                return encryptionStatus;
            }
            set
            {
                encryptionStatus = value;
            }
        }

        public String EncryptionMethod
        {
            get
            {
                return encryptionMethod;
            }
            set
            {
                encryptionMethod = value;
            }
        }

        /// <summary>
        /// Gets or sets the Model of the hard disk.
        /// </summary>
        public String Caption
        {
            get
            {
                return caption;
            }
            set
            {
                caption = value;
            }
        }

        public bool IsAutoUnlockEnabled
        {
            get
            {
                return autoUnlockEnabled;
            }
            set
            {
                autoUnlockEnabled = value;
            }
        }

        public decimal Capacity
        {
            get
            {
                return capacity;
            }
            set
            {
                capacity = value;
            }
        }

        public decimal FreeSpace
        {
            get
            {
                return freeSpace;
            }
            set
            {
                freeSpace = value;
            }
        }

        public MetadataVersion BitLockerVersion
        {
            get
            {
                return bitlockerVersion;
            }
            set
            {
                bitlockerVersion = value;
            }
        }

        public DiskType HdType
        {
            get
            {
                return diskType;
            }
            set
            {
                diskType = value;
            }
        }
    }
}
