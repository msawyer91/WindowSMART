using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class DiskIdentityData
    {
        private UInt32 rpm;
        private bool isSsd;
        private bool isTrimSupported;
        private String model;
        private String serialNumber;
        private String firmwareRevision;

        public DiskIdentityData()
        {
            SiAuto.Main.EnterMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.DiskIdentityData");
            rpm = 65535;
            isSsd = false;
            isTrimSupported = false;
            model = String.Empty;
            serialNumber = String.Empty;
            firmwareRevision = String.Empty;
            SiAuto.Main.LogMessage("DiskIdentityData is initialized with default values for RPM (65535), TRIM (false) and empty strings for " +
                "model, serialNumber and firmwareRevision.");
            SiAuto.Main.LeaveMethod("DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.DiskIdentityData");
        }

        public UInt32 Rpm
        {
            get
            {
                return rpm;
            }
            set
            {
                rpm = value;
            }
        }

        public bool IsSsd
        {
            get
            {
                return isSsd;
            }
            set
            {
                isSsd = value;
            }
        }

        public bool TrimSupported
        {
            get
            {
                return isTrimSupported;
            }
            set
            {
                isTrimSupported = value;
            }
        }

        public String Model
        {
            get
            {
                return model;
            }
            set
            {
                model = value.Replace("\0", String.Empty);
            }
        }

        public String SerialNumber
        {
            get
            {
                return serialNumber;
            }
            set
            {
                serialNumber = value.Replace("\0", String.Empty);
            }
        }

        public String FirmwareRevision
        {
            get
            {
                return firmwareRevision;
            }
            set
            {
                firmwareRevision = value.Replace("\0", String.Empty);
            }
        }
    }
}
