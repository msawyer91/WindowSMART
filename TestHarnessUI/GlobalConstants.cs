using System;
using System.Collections.Generic;
using System.Text;

namespace DojoNorthSoftware.HomeServer.HomeServerSMARTService.Enumerator
{
    public class GlobalConstants
    {
        // Constants for SMART
        public const byte READ_ATTRIBUTES = 0xD0;
        public const byte READ_THRESHOLDS = 0xD1;
        public const byte RETURN_SMART_STATUS = 0xDA;
        public const UInt32 READ_ATTRIBUTE_BUFFER_SIZE = 512;
        public const UInt32 READ_THRESHOLD_BUFFER_SIZE = 512;
        public const UInt32 IDENTIFY_BUFFER_SIZE = 512;
        public const UInt32 SMART_LOG_SECTOR_SIZE = 512;

        // Standard target 160 (0xA0)
        public const byte DEFAULT_TARGET_ID = 0xA0;
    }
}
