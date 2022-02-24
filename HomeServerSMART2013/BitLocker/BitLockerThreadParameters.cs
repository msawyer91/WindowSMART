using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public sealed class BitLockerThreadParameters
    {
        private String deviceId;
        private String argument;

        public BitLockerThreadParameters(String device)
        {
            deviceId = device;
            argument = String.Empty;
        }

        public BitLockerThreadParameters(String device, String arg)
        {
            deviceId = device;
            argument = arg;
        }

        public String DeviceId
        {
            get
            {
                return deviceId;
            }
        }

        public String Argument
        {
            get
            {
                return argument;
            }
        }
    }
}
