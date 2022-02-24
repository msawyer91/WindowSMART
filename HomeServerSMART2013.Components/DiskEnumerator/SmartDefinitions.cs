using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public class SmartDefinitions
    {
        private SmartHddDefinitions hddDefinitions;
        private SmartSsdIndilinxDefinitions ssdIndilinxDefinitions;
        private SmartSsdEverestDefinitions ssdEverestDefinitions;
        private SmartSsdBarefoot3Definitions ssdBarefoot3Definitions;
        private SmartSsdLamdDefinitions ssdLamdDefinitions;
        private SmartSsdIntelDefinitions ssdIntelDefinitions;
        private SmartSsdToshibaDefinitions ssdToshibaDefinitions;
        private SmartSsdJMicronDefinitions ssdJMicronDefinitions;
        private SmartSsdMarvellDefinitions ssdMarvellDefinitions;
        private SmartSsdMicronDefinitions ssdMicronDefinitions;
        private SmartSsdSandForceDefinitions ssdSandForceDefinitions;
        private SmartSsdSamsungDefinitions ssdSamsungDefinitions;
        private SmartSsdSmartModularDefinitions ssdSmartModularDefinitions;
        private SmartSsdStecDefinitions ssdSmartStecDefinitions;
        private SmartSsdKingstonDefinitions ssdSmartKingstonDefinitions;
        private SmartSsdTranscendDefinitions ssdSmartTranscendDefinitions;
        private SmartSsdKingSpecDefinitions ssdSmartKingSpecDefinitions;
        private SmartSsdSmartbuyDefinitions ssdSmartSmartbuyDefinitions;
        private SmartSsdAdataDefinitions ssdSmartAdataDefinitions;
        private SmartSsdSanDiskDefinitions ssdSmartSanDiskDefinitions;
        private SmartSsdSkHynixDefinitions ssdSmartSkHynixDefinitions;
        private SmartSsdGenericDefinitions ssdSmartGenericDefinitions;

        public SmartDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartDefinitions.SmartDefinitions");
            SiAuto.Main.LogMessage("[SMART Definitions] Construct HDD definitions.");
            hddDefinitions = new SmartHddDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - Indilinx definitions."); 
            ssdIndilinxDefinitions = new SmartSsdIndilinxDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - Indilinx Everest definitions."); 
            ssdEverestDefinitions = new SmartSsdEverestDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - Indilinx Barefoot 3 definitions.");
            ssdBarefoot3Definitions = new SmartSsdBarefoot3Definitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - Intel definitions.");
            ssdIntelDefinitions = new SmartSsdIntelDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - JMicron definitions.");
            ssdJMicronDefinitions = new SmartSsdJMicronDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - Marvell definitions.");
            ssdMarvellDefinitions = new SmartSsdMarvellDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - Toshiba definitions.");
            ssdToshibaDefinitions = new SmartSsdToshibaDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - Micron definitions.");
            ssdMicronDefinitions = new SmartSsdMicronDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - Samsung definitions.");
            ssdSamsungDefinitions = new SmartSsdSamsungDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - SandForce definitions."); 
            ssdSandForceDefinitions = new SmartSsdSandForceDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - SMART Modular definitions.");
            ssdSmartModularDefinitions = new SmartSsdSmartModularDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - STEC definitions.");
            ssdSmartStecDefinitions = new SmartSsdStecDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - LAMD definitions.");
            ssdLamdDefinitions = new SmartSsdLamdDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - Kingston definitions.");
            ssdSmartKingstonDefinitions = new SmartSsdKingstonDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - Transcend definitions.");
            ssdSmartTranscendDefinitions = new SmartSsdTranscendDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - KingSpec definitions.");
            ssdSmartKingSpecDefinitions = new SmartSsdKingSpecDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - Smartbuy definitions.");
            ssdSmartSmartbuyDefinitions = new SmartSsdSmartbuyDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - Adata definitions.");
            ssdSmartAdataDefinitions = new SmartSsdAdataDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - SanDisk definitions.");
            ssdSmartSanDiskDefinitions = new SmartSsdSanDiskDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - sk Hynix definitions.");
            ssdSmartSkHynixDefinitions = new SmartSsdSkHynixDefinitions();
            SiAuto.Main.LogMessage("[SMART Definitions] Construct SSD - Generic definitions.");
            ssdSmartGenericDefinitions = new SmartSsdGenericDefinitions();
            
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartDefinitions.SmartDefinitions");
        }

        public SmartHddDefinitions HddDefinitions
        {
            get
            {
                return hddDefinitions;
            }
        }

        public SmartSsdIndilinxDefinitions SsdIndilinxDefinitions
        {
            get
            {
                return ssdIndilinxDefinitions;
            }
        }

        public SmartSsdEverestDefinitions SsdEverestDefinitions
        {
            get
            {
                return ssdEverestDefinitions;
            }
        }

        public SmartSsdBarefoot3Definitions SsdBarefoot3Definitions
        {
            get
            {
                return ssdBarefoot3Definitions;
            }
        }

        public SmartSsdLamdDefinitions SsdLamdDefinitions
        {
            get
            {
                return ssdLamdDefinitions;
            }
        }

        public SmartSsdIntelDefinitions SsdIntelDefinitions
        {
            get
            {
                return ssdIntelDefinitions;
            }
        }

        public SmartSsdToshibaDefinitions SsdToshibaDefinitions
        {
            get
            {
                return ssdToshibaDefinitions;
            }
        }

        public SmartSsdJMicronDefinitions SsdJMicronDefinitions
        {
            get
            {
                return ssdJMicronDefinitions;
            }
        }

        public SmartSsdMarvellDefinitions SsdMarvellDefinitions
        {
            get
            {
                return ssdMarvellDefinitions;
            }
        }

        public SmartSsdMicronDefinitions SsdMicronDefinitions
        {
            get
            {
                return ssdMicronDefinitions;
            }
        }

        public SmartSsdSandForceDefinitions SsdSandForceDefinitions
        {
            get
            {
                return ssdSandForceDefinitions;
            }
        }

        public SmartSsdSamsungDefinitions SsdSamsungDefinitions
        {
            get
            {
                return ssdSamsungDefinitions;
            }
        }

        public SmartSsdSmartModularDefinitions SsdSmartModularDefinitions
        {
            get
            {
                return ssdSmartModularDefinitions;
            }
        }

        public SmartSsdStecDefinitions SsdStecDefinitions
        {
            get
            {
                return ssdSmartStecDefinitions;
            }
        }

        public SmartSsdKingstonDefinitions SsdKingstonDefinitions
        {
            get
            {
                return ssdSmartKingstonDefinitions;
            }
        }

        public SmartSsdTranscendDefinitions SsdTranscendDefinitions
        {
            get
            {
                return ssdSmartTranscendDefinitions;
            }
        }

        public SmartSsdKingSpecDefinitions SsdKingSpecDefinitions
        {
            get
            {
                return ssdSmartKingSpecDefinitions;
            }
        }

        public SmartSsdSmartbuyDefinitions SsdSmartbuyDefinitions
        {
            get
            {
                return ssdSmartSmartbuyDefinitions;
            }
        }

        public SmartSsdAdataDefinitions SsdAdataDefinitions
        {
            get
            {
                return ssdSmartAdataDefinitions;
            }
        }

        public SmartSsdSanDiskDefinitions SsdSanDiskDefinitions
        {
            get
            {
                return ssdSmartSanDiskDefinitions;
            }
        }

        public SmartSsdSkHynixDefinitions SsdSkHynixDefinitions
        {
            get
            {
                return ssdSmartSkHynixDefinitions;
            }
        }

        public SmartSsdGenericDefinitions SsdGenericDefinitions
        {
            get
            {
                return ssdSmartGenericDefinitions;
            }
        }
    }
}
