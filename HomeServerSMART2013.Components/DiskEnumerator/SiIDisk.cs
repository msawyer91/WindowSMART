using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SiIDisk
    {
        private String deviceID;
        private String siliconImageType;
        private int scsiPort;
        private int scsiBus;

        public SiIDisk(String id, String siiType, int port, int bus)
        {
            deviceID = id;
            siliconImageType = siiType;
            scsiPort = port;
            scsiBus = bus;
        }

        public SiIDisk(String id, String siiType)
        {
            deviceID = id;
            siliconImageType = siiType;
            scsiPort = 0;
            scsiBus = 0;
        }

        public String DeviceID
        {
            get
            {
                return deviceID;
            }
        }

        public String SiliconImageType
        {
            get
            {
                return siliconImageType;
            }
        }

        public int ScsiPort
        {
            get
            {
                return scsiPort;
            }
        }

        public int ScsiBus
        {
            get
            {
                return scsiBus;
            }
        }
    }
}
