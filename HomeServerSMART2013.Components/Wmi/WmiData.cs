using System;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Wmi
{
    [InstrumentationClass(InstrumentationType.Instance)]
    public sealed class WmiData
    {
        private int alertCount;
        private String installedVersion;
        private String[] alerts;

        public int AlertCount
        {
            get
            {
                return alertCount;
            }
            set
            {
                alertCount = value;
            }
        }

        public String InstalledVersion
        {
            get
            {
                return installedVersion;
            }
            set
            {
                installedVersion = value;
            }
        }

        public String[] ActiveAlerts
        {
            get
            {
                return alerts;
            }
            set
            {
                alerts = value;
            }
        }
    }
}
