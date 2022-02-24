using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SMART_DATA
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] Data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SMART_DATA_UCHAR
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] Data;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct SMART_DATA_SII
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] VendorSpecific;
        public byte FailurePredicted;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ATA_SMART_INFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] SmartReadData;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] SmartReadThreshold;
        UInt32 AttributeCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        SMART_THRESHOLD[] Threshold;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct SMART_THRESHOLD
	{
		public byte Id;
		public byte ThresholdValue;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] Reserved;
	};

    //[StructLayout(LayoutKind.Sequential)]
    //struct IDENTIFY_DEVICE
    //{
    //    public ushort GeneralConfiguration;					//0
    //    public ushort LogicalCylinders;						//1	Obsolete
    //    public ushort SpecificConfiguration;					//2
    //    public ushort LogicalHeads;							//3 Obsolete
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    //    public ushort[] Retired1;   							//4-5
    //    public ushort LogicalSectors;							//6 Obsolete
    //    public UInt32 ReservedForCompactFlash;				//7-8
    //    public ushort Retired2;								//9
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    //    public char[] SerialNumber;   						//10-19
    //    public ushort Retired3;								//20
    //    public ushort BufferSize;								//21 Obsolete
    //    public ushort Obsolute4;								//22
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    //    public char[] FirmwareRev;							//23-26
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
    //    public char[] Model;  								//27-46
    //    public ushort MaxNumPerInterupt;						//47
    //    public ushort Reserved1;								//48
    //    public ushort Capabilities1;							//49
    //    public ushort Capabilities2;							//50
    //    public UInt32 Obsolute5;								//51-52
    //    public ushort Field88and7064;							//53
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    //    public ushort[] Obsolute6;  							//54-58
    //    public ushort MultSectorStuff;						//59
    //    public UInt32 TotalAddressableSectors;				//60-61
    //    public ushort Obsolute7;								//62
    //    public ushort MultiWordDma;							//63
    //    public ushort PioMode;								//64
    //    public ushort MinMultiwordDmaCycleTime;				//65
    //    public ushort RecommendedMultiwordDmaCycleTime;		//66
    //    public ushort MinPioCycleTimewoFlowCtrl;				//67
    //    public ushort MinPioCycleTimeWithFlowCtrl;			//68
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    //    public ushort[] Reserved2;					    		//69-74
    //    public ushort QueueDepth;								//75
    //    public ushort SerialAtaCapabilities;					//76
    //    public ushort ReservedForFutureSerialAta;				//77
    //    public ushort SerialAtaFeaturesSupported;				//78
    //    public ushort SerialAtaFeaturesEnabled;				//79
    //    public ushort MajorVersion;							//80
    //    public ushort MinorVersion;							//81
    //    public ushort CommandSetSupported1;					//82
    //    public ushort CommandSetSupported2;					//83
    //    public ushort CommandSetSupported3;					//84
    //    public ushort CommandSetEnabled1;						//85
    //    public ushort CommandSetEnabled2;						//86
    //    public ushort CommandSetDefault;						//87
    //    public ushort UltraDmaMode;							//88
    //    public ushort TimeReqForSecurityErase;				//89
    //    public ushort TimeReqForEnhancedSecure;				//90
    //    public ushort CurrentPowerManagement;					//91
    //    public ushort MasterPasswordRevision;					//92
    //    public ushort HardwareResetResult;					//93
    //    public ushort AcoustricManagement;					//94
    //    public ushort StreamMinRequestSize;					//95
    //    public ushort StreamingTimeDma;						//96
    //    public ushort StreamingAccessLatency;					//97
    //    public UInt32 StreamingPerformance;					//98-99
    //    public ulong MaxUserLba;								//100-103
    //    public ushort StremingTimePio;						//104
    //    public ushort Reserved3;								//105
    //    public ushort SectorSize;								//106
    //    public ushort InterSeekDelay;							//107
    //    public ushort IeeeOui;								//108
    //    public ushort UniqueId3;								//109
    //    public ushort UniqueId2;								//110
    //    public ushort UniqueId1;								//111
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    //    public ushort[] Reserved4;  							//112-115
    //    public ushort Reserved5;								//116
    //    public UInt32 WordsPerLogicalSector;					//117-118
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    //    public ushort[] Reserved6;  							//119-126
    //    public ushort RemovableMediaStatus;					//127
    //    public ushort SecurityStatus;							//128
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
    //    public ushort[] VendorSpecific;					    	//129-159
    //    public ushort CfaPowerMode1;							//160
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
    //    public ushort[] ReservedForCompactFlashAssociation; 	//161-167
    //    public ushort DeviceNominalFormFactor;				//168
    //    public ushort DataSetManagement;						//169
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    //    public ushort[] AdditionalProductIdentifier;			//170-173
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    //    public ushort[] Reserved7;  							//174-175
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
    //    public char[] CurrentMediaSerialNo;			    	//176-205
    //    public ushort SctCommandTransport;					//206
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    //    public ushort[] ReservedForCeAta1;				    	//207-208
    //    public ushort AlignmentOfLogicalBlocks;				//209
    //    public UInt32 WriteReadVerifySectorCountMode3;		//210-211
    //    public UInt32 WriteReadVerifySectorCountMode2;		//212-213
    //    public ushort NvCacheCapabilities;					//214
    //    public UInt32 NvCacheSizeLogicalBlocks;				//215-216
    //    public ushort NominalMediaRotationRate;				//217
    //    public ushort Reserved8;								//218
    //    public ushort NvCacheOptions1;						//219
    //    public ushort NvCacheOptions2;						//220
    //    public ushort Reserved9;								//221
    //    public ushort TransportMajorVersionNumber;			//222
    //    public ushort TransportMinorVersionNumber;			//223
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    //    public ushort[] ReservedForCeAta2;  					//224-233
    //    public ushort MinimumBlocksPerDownloadMicrocode;		//234
    //    public ushort MaximumBlocksPerDownloadMicrocode;		//235
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    //    public ushort[] Reserved10; 							//236-254
    //    public ushort IntegrityWord;							//255
    //};

    struct IDENTITY_DATA
    {
        public ushort LogicalCylinders;						//1	Obsolete
        public ushort LogicalHeads;							//3 Obsolete
        public ushort LogicalSectors;							//6 Obsolete
        public ushort BufferSize;								//21 Obsolete
        public UInt32 TotalAddressableSectors;				//60-61
        public ushort PioMode;								//64
        public ushort SerialAtaCapabilities;					//76
        public ushort SerialAtaFeaturesSupported;				//78
        public ushort SerialAtaFeaturesEnabled;				//79
        public ushort UltraDmaMode;							//88
        public ushort TimeReqForSecurityErase;				//89
        public ushort TimeReqForEnhancedSecure;				//90
        public ushort AcousticManagement;					//94
        public ushort SectorSize;								//106
        public ushort DataSetManagement;                    //169
        public ushort NominalMediaRotationRate;				//217
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] SerialNumber;						//10-19
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public char[] Model;								//27-46
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] FirmwareRev;							//23-26
    }

    enum SMART_QUERY_RESULT
    {
        SMART_QUERY_RESULT_OK = 0,
        SMART_QUERY_INVALID_HANDLE,
        SMART_QUERY_IO_CONTROL_FALSE,
        SMART_QUERY_OUTDATA_SIZE_MISMATCH,
        SMART_QUERY_EXCEPTION,
        SMART_QUERY_PENDING
    };

    public enum IDENTIFY_DEVICE_RESULT
    {
        IDENTIFY_DEVICE_RESULT_OK = 0,
        IDENTIFY_DEVICE_INVALID_HANDLE,
        IDENTIFY_DEVICE_IO_CONTROL_FALSE,
        IDENTIFY_DEVICE_OUTDATA_SIZE_MISMATCH,
        IDENTIFY_DEVICE_EXCEPTION,
        IDENTIFY_DEVICE_NULL,
        IDENTIFY_DEVICE_PENDING
    };

    public enum COMMAND_TYPE
    {
        CMD_TYPE_PHYSICAL_DRIVE = 0,
        CMD_TYPE_SCSI_MINIPORT,
        CMD_TYPE_SILICON_IMAGE,
        CMD_TYPE_SAT,				// SAT = SCSI_ATA_TRANSLATION
        CMD_TYPE_SUNPLUS,
        CMD_TYPE_IO_DATA,
        CMD_TYPE_LOGITEC,
        CMD_TYPE_JMICRON,
        CMD_TYPE_CYPRESS,
        CMD_TYPE_PROLIFIC,
        CMD_TYPE_DEBUG
    };
}
