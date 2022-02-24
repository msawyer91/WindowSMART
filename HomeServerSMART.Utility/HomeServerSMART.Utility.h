// HomeServerSMART.Utility.h

// Compiler Options
#define CSMI_SUPPORT

#pragma once

#include "resource.h"		// main symbols

#include "winioctl.h"
#include "SPTIUtil.h"

	static const int MAX_DISK = 32;// FIX
	static const int MAX_ATTRIBUTE = 30; // FIX
	static const int MAX_SEARCH_PHYSICAL_DRIVE = 32;
	static const int MAX_SEARCH_SCSI_PORT = 16;
	static const int MAX_SEARCH_SCSI_TARGET_ID = 8;

	static const int SCSI_MINIPORT_BUFFER_SIZE = 512;

	enum SMART_QUERY_RESULT
	{
		SMART_QUERY_RESULT_OK = 0,
		SMART_QUERY_INVALID_HANDLE,
		SMART_QUERY_IO_CONTROL_FALSE,
		SMART_QUERY_OUTDATA_SIZE_MISMATCH,
		SMART_QUERY_EXCEPTION,
		SMART_QUERY_PENDING
	};

	enum IDENTIFY_DEVICE_RESULT
	{
		IDENTIFY_DEVICE_RESULT_OK = 0,
		IDENTIFY_DEVICE_INVALID_HANDLE,
		IDENTIFY_DEVICE_IO_CONTROL_FALSE,
		IDENTIFY_DEVICE_OUTDATA_SIZE_MISMATCH,
		IDENTIFY_DEVICE_EXCEPTION,
		IDENTIFY_DEVICE_NULL,
		IDENTIFY_DEVICE_PENDING
	};

	enum COMMAND_TYPE
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

	enum VENDOR_ID
	{
		HDD_GENERAL           = 0,
		SSD_GENERAL           = 1,
		SSD_VENDOR_MTRON      = 2,
		SSD_VENDOR_INDILINX   = 3,
		SSD_VENDOR_JMICRON    = 4,
		SSD_VENDOR_INTEL      = 5,
		SSD_VENDOR_SAMSUNG    = 6,
		SSD_VENDOR_SANDFORCE  = 7,
		SSD_VENDOR_MAX        = 99,

		VENDOR_UNKNOWN      = 0x0000,
		USB_VENDOR_BUFFALO  = 0x0411,
		USB_VENDOR_IO_DATA  = 0x04BB,
		USB_VENDOR_LOGITEC  = 0x0789,
		USB_VENDOR_INITIO   = 0x13FD,
		USB_VENDOR_SUNPLUS  = 0x04FC,
		USB_VENDOR_JMICRON  = 0x152D,
		USB_VENDOR_CYPRESS  = 0x04B4,
		USB_VENDOR_OXFORD   = 0x0928,
		USB_VENDOR_PROLIFIC = 0x067B,
		USB_VENDOR_ALL      = 0xFFFF,
	};

	enum INTERFACE_TYPE
	{
		INTERFACE_TYPE_UNKNOWN = 0,
		INTERFACE_TYPE_PATA,
		INTERFACE_TYPE_SATA,
		INTERFACE_TYPE_USB,
		INTERFACE_TYPE_IEEE1394
	};

	enum IO_CONTROL_CODE
	{
		DFP_SEND_DRIVE_COMMAND	= 0x0007C084,
		DFP_RECEIVE_DRIVE_DATA	= 0x0007C088,
		IOCTL_SCSI_MINIPORT     = 0x0004D008,
		IOCTL_IDE_PASS_THROUGH  = 0x0004D028, // 2000 or later
		IOCTL_ATA_PASS_THROUGH  = 0x0004D02C, // XP SP2 and 2003 or later
	};

	enum CSMI_TYPE
	{
		CSMI_TYPE_DISABLE = 0,
		CSMI_TYPE_ENABLE_AUTO,
		CSMI_TYPE_ENABLE_RAID,
		CSMI_TYPE_ENABLE_ALL,
	};

#pragma pack(push,1)

	typedef	struct _IDENTIFY_DEVICE_OUTDATA
	{
		SENDCMDOUTPARAMS	SendCmdOutParam;
		BYTE				Data[IDENTIFY_BUFFER_SIZE - 1];
	} IDENTIFY_DEVICE_OUTDATA, *PIDENTIFY_DEVICE_OUTDATA;

	typedef	struct _SMART_READ_DATA_OUTDATA
	{
		SENDCMDOUTPARAMS	SendCmdOutParam;
		BYTE				Data[READ_ATTRIBUTE_BUFFER_SIZE - 1];
	} SMART_READ_DATA_OUTDATA, *PSMART_READ_DATA_OUTDATA;

	typedef struct _CMD_IDE_PATH_THROUGH
	{
		IDEREGS	reg;
		DWORD   length;
		BYTE    buffer[1];
	} CMD_IDE_PATH_THROUGH, *PCMD_IDE_PATH_THROUGH;

	static const int ATA_FLAGS_DRDY_REQUIRED = 0x01;
	static const int ATA_FLAGS_DATA_IN       = 0x02;
	static const int ATA_FLAGS_DATA_OUT      = 0x04;
	static const int ATA_FLAGS_48BIT_COMMAND = 0x08;

	typedef struct _ATA_PASS_THROUGH_EX
	{
		WORD    Length;
		WORD    AtaFlags;
		BYTE    PathId;
		BYTE    TargetId;
		BYTE    Lun;
		BYTE    ReservedAsUchar;
		DWORD   DataTransferLength;
		DWORD   TimeOutValue;
		DWORD   ReservedAsUlong;
		DWORD   DataBufferOffset;
	//	DWORD_PTR   DataBufferOffset;
		IDEREGS PreviousTaskFile;
		IDEREGS CurrentTaskFile;
	} ATA_PASS_THROUGH_EX, *PCMD_ATA_PASS_THROUGH_EX;

	typedef struct
	{
		ATA_PASS_THROUGH_EX Apt;
		DWORD Filer;
		BYTE  Buf[512];
	} ATA_PASS_THROUGH_EX_WITH_BUFFERS;

	typedef	struct SMART_ATTRIBUTE
	{
		BYTE	Id;
		WORD	StatusFlags;
		BYTE	CurrentValue;
		BYTE	WorstValue;
		BYTE	RawValue[6];
		BYTE	Reserved;
	};

	typedef	struct SMART_THRESHOLD
	{
		BYTE	Id;
		BYTE	ThresholdValue;
		BYTE	Reserved[10];
	};

	struct SMART_DATA
	{
		BYTE	Data[512];
	};

	struct SMART_DATA_UCHAR
	{
		UCHAR	Data[512];
	};

	struct SMART_DATA_SII
	{
		BYTE	Data[512];
		BYTE	FailurePredicted;
	};

	struct IDENTITY_DATA
	{
		WORD		LogicalCylinders;						//1	Obsolete
		WORD		LogicalHeads;							//3 Obsolete
		WORD		LogicalSectors;							//6 Obsolete
		WORD		BufferSize;								//21 Obsolete
		DWORD		TotalAddressableSectors;				//60-61
		WORD		PioMode;								//64
		WORD		SerialAtaCapabilities;					//76
		WORD		SerialAtaFeaturesSupported;				//78
		WORD		SerialAtaFeaturesEnabled;				//79
		WORD		UltraDmaMode;							//88
		WORD		TimeReqForSecurityErase;				//89
		WORD		TimeReqForEnhancedSecure;				//90
		WORD		AcousticManagement;						//94
		WORD		SectorSize;								//106
		WORD		DataSetManagement;						//169
		WORD		NominalMediaRotationRate;				//217
		CHAR		SerialNumber[20];						//10-19
		CHAR		Model[40];								//27-46
		CHAR		FirmwareRev[8];							//23-26
	};

	typedef struct SRB_IO_CONTROL
	{
	   ULONG	HeaderLength;
	   UCHAR	Signature[8];
	   ULONG	Timeout;
	   ULONG	ControlCode;
	   ULONG	ReturnCode;
	   ULONG	Length;
	};

	typedef struct {
		SRB_IO_CONTROL sic ;
		USHORT port ;
		USHORT maybe_always1 ;
		ULONG unknown[5] ;
		//IDENTIFY_DEVICE id_data ;
		WORD id_data[256] ;
	} SilIdentDev ;

	struct IDENTIFY_DEVICE
	{
		WORD		GeneralConfiguration;					//0
		WORD		LogicalCylinders;						//1	Obsolete
		WORD		SpecificConfiguration;					//2
		WORD		LogicalHeads;							//3 Obsolete
		WORD		Retired1[2];							//4-5
		WORD		LogicalSectors;							//6 Obsolete
		DWORD		ReservedForCompactFlash;				//7-8
		WORD		Retired2;								//9
		CHAR		SerialNumber[20];						//10-19
		WORD		Retired3;								//20
		WORD		BufferSize;								//21 Obsolete
		WORD		Obsolute4;								//22
		CHAR		FirmwareRev[8];							//23-26
		CHAR		Model[40];								//27-46
		WORD		MaxNumPerInterupt;						//47
		WORD		Reserved1;								//48
		WORD		Capabilities1;							//49
		WORD		Capabilities2;							//50
		DWORD		Obsolute5;								//51-52
		WORD		Field88and7064;							//53
		WORD		Obsolute6[5];							//54-58
		WORD		MultSectorStuff;						//59
		DWORD		TotalAddressableSectors;				//60-61
		WORD		Obsolute7;								//62
		WORD		MultiWordDma;							//63
		WORD		PioMode;								//64
		WORD		MinMultiwordDmaCycleTime;				//65
		WORD		RecommendedMultiwordDmaCycleTime;		//66
		WORD		MinPioCycleTimewoFlowCtrl;				//67
		WORD		MinPioCycleTimeWithFlowCtrl;			//68
		WORD		Reserved2[6];							//69-74
		WORD		QueueDepth;								//75
		WORD		SerialAtaCapabilities;					//76
		WORD		ReservedForFutureSerialAta;				//77
		WORD		SerialAtaFeaturesSupported;				//78
		WORD		SerialAtaFeaturesEnabled;				//79
		WORD		MajorVersion;							//80
		WORD		MinorVersion;							//81
		WORD		CommandSetSupported1;					//82
		WORD		CommandSetSupported2;					//83
		WORD		CommandSetSupported3;					//84
		WORD		CommandSetEnabled1;						//85
		WORD		CommandSetEnabled2;						//86
		WORD		CommandSetDefault;						//87
		WORD		UltraDmaMode;							//88
		WORD		TimeReqForSecurityErase;				//89
		WORD		TimeReqForEnhancedSecure;				//90
		WORD		CurrentPowerManagement;					//91
		WORD		MasterPasswordRevision;					//92
		WORD		HardwareResetResult;					//93
		WORD		AcousticManagement;						//94
		WORD		StreamMinRequestSize;					//95
		WORD		StreamingTimeDma;						//96
		WORD		StreamingAccessLatency;					//97
		DWORD		StreamingPerformance;					//98-99
		ULONGLONG	MaxUserLba;								//100-103
		WORD		StremingTimePio;						//104
		WORD		Reserved3;								//105
		WORD		SectorSize;								//106
		WORD		InterSeekDelay;							//107
		WORD		IeeeOui;								//108
		WORD		UniqueId3;								//109
		WORD		UniqueId2;								//110
		WORD		UniqueId1;								//111
		WORD		Reserved4[4];							//112-115
		WORD		Reserved5;								//116
		DWORD		WordsPerLogicalSector;					//117-118
		WORD		Reserved6[8];							//119-126
		WORD		RemovableMediaStatus;					//127
		WORD		SecurityStatus;							//128
		WORD		VendorSpecific[31];						//129-159
		WORD		CfaPowerMode1;							//160
		WORD		ReservedForCompactFlashAssociation[7];	//161-167
		WORD		DeviceNominalFormFactor;				//168
		WORD		DataSetManagement;						//169
		WORD		AdditionalProductIdentifier[4];			//170-173
		WORD		Reserved7[2];							//174-175
		CHAR		CurrentMediaSerialNo[60];				//176-205
		WORD		SctCommandTransport;					//206
		WORD		ReservedForCeAta1[2];					//207-208
		WORD		AlignmentOfLogicalBlocks;				//209
		DWORD		WriteReadVerifySectorCountMode3;		//210-211
		DWORD		WriteReadVerifySectorCountMode2;		//212-213
		WORD		NvCacheCapabilities;					//214
		DWORD		NvCacheSizeLogicalBlocks;				//215-216
		WORD		NominalMediaRotationRate;				//217
		WORD		Reserved8;								//218
		WORD		NvCacheOptions1;						//219
		WORD		NvCacheOptions2;						//220
		WORD		Reserved9;								//221
		WORD		TransportMajorVersionNumber;			//222
		WORD		TransportMinorVersionNumber;			//223
		WORD		ReservedForCeAta2[10];					//224-233
		WORD		MinimumBlocksPerDownloadMicrocode;		//234
		WORD		MaximumBlocksPerDownloadMicrocode;		//235
		WORD		Reserved10[19];							//236-254
		WORD		IntegrityWord;							//255
	};
#pragma	pack(pop)

#ifdef CSMI_SUPPORT

///////////////////
// from csmisas.h
///////////////////

// IOCTL Control Codes
// (IoctlHeader.ControlCode)

// Control Codes requiring CSMI_ALL_SIGNATURE

#define CC_CSMI_SAS_GET_DRIVER_INFO    1
#define CC_CSMI_SAS_GET_CNTLR_CONFIG   2
#define CC_CSMI_SAS_GET_CNTLR_STATUS   3
#define CC_CSMI_SAS_FIRMWARE_DOWNLOAD  4

// Control Codes requiring CSMI_RAID_SIGNATURE

#define CC_CSMI_SAS_GET_RAID_INFO      10
#define CC_CSMI_SAS_GET_RAID_CONFIG    11
#define CC_CSMI_SAS_GET_RAID_FEATURES  12
#define CC_CSMI_SAS_SET_RAID_CONTROL   13
#define CC_CSMI_SAS_GET_RAID_ELEMENT   14
#define CC_CSMI_SAS_SET_RAID_OPERATION 15

// Control Codes requiring CSMI_SAS_SIGNATURE

#define CC_CSMI_SAS_GET_PHY_INFO       20
#define CC_CSMI_SAS_SET_PHY_INFO       21
#define CC_CSMI_SAS_GET_LINK_ERRORS    22
#define CC_CSMI_SAS_SMP_PASSTHRU       23
#define CC_CSMI_SAS_SSP_PASSTHRU       24
#define CC_CSMI_SAS_STP_PASSTHRU       25
#define CC_CSMI_SAS_GET_SATA_SIGNATURE 26
#define CC_CSMI_SAS_GET_SCSI_ADDRESS   27
#define CC_CSMI_SAS_GET_DEVICE_ADDRESS 28
#define CC_CSMI_SAS_TASK_MANAGEMENT    29
#define CC_CSMI_SAS_GET_CONNECTOR_INFO 30
#define CC_CSMI_SAS_GET_LOCATION       31

// Control Codes requiring CSMI_PHY_SIGNATURE

#define CC_CSMI_SAS_PHY_CONTROL        60

#define IOCTL_HEADER SRB_IO_CONTROL
#define PIOCTL_HEADER PSRB_IO_CONTROL

/*************************************************************************/
/* OS INDEPENDENT CODE                                                   */
/*************************************************************************/

/* * * * * * * * * * Class Independent IOCTL Constants * * * * * * * * * */

// Return codes for all IOCTL's regardless of class
// (IoctlHeader.ReturnCode)

#define CSMI_SAS_STATUS_SUCCESS              0
#define CSMI_SAS_STATUS_FAILED               1
#define CSMI_SAS_STATUS_BAD_CNTL_CODE        2
#define CSMI_SAS_STATUS_INVALID_PARAMETER    3
#define CSMI_SAS_STATUS_WRITE_ATTEMPTED      4

// Signature value
// (IoctlHeader.Signature)

#define CSMI_ALL_SIGNATURE    "CSMIALL"

// Timeout value default of 60 seconds
// (IoctlHeader.Timeout)

#define CSMI_ALL_TIMEOUT      60

//  Direction values for data flow on this IOCTL
// (IoctlHeader.Direction, Linux only)
#define CSMI_SAS_DATA_READ    0
#define CSMI_SAS_DATA_WRITE   1

// I/O Bus Types
// ISA and EISA bus types are not supported
// (bIoBusType)

#define CSMI_SAS_BUS_TYPE_PCI       3
#define CSMI_SAS_BUS_TYPE_PCMCIA    4

// Controller Status
// (uStatus)

#define CSMI_SAS_CNTLR_STATUS_GOOD     1
#define CSMI_SAS_CNTLR_STATUS_FAILED   2
#define CSMI_SAS_CNTLR_STATUS_OFFLINE  3
#define CSMI_SAS_CNTLR_STATUS_POWEROFF 4

// Offline Status Reason
// (uOfflineReason)

#define CSMI_SAS_OFFLINE_REASON_NO_REASON             0
#define CSMI_SAS_OFFLINE_REASON_INITIALIZING          1
#define CSMI_SAS_OFFLINE_REASON_BACKSIDE_BUS_DEGRADED 2
#define CSMI_SAS_OFFLINE_REASON_BACKSIDE_BUS_FAILURE  3

// Controller Class
// (bControllerClass)

#define CSMI_SAS_CNTLR_CLASS_HBA    5

// Controller Flag bits
// (uControllerFlags)

#define CSMI_SAS_CNTLR_SAS_HBA          0x00000001
#define CSMI_SAS_CNTLR_SAS_RAID         0x00000002
#define CSMI_SAS_CNTLR_SATA_HBA         0x00000004
#define CSMI_SAS_CNTLR_SATA_RAID        0x00000008
#define CSMI_SAS_CNTLR_SMART_ARRAY      0x00000010

// for firmware download
#define CSMI_SAS_CNTLR_FWD_SUPPORT      0x00010000
#define CSMI_SAS_CNTLR_FWD_ONLINE       0x00020000
#define CSMI_SAS_CNTLR_FWD_SRESET       0x00040000
#define CSMI_SAS_CNTLR_FWD_HRESET       0x00080000
#define CSMI_SAS_CNTLR_FWD_RROM         0x00100000

// for RAID configuration supported
#define CSMI_SAS_CNTLR_RAID_CFG_SUPPORT 0x01000000

// Download Flag bits
// (uDownloadFlags)
#define CSMI_SAS_FWD_VALIDATE       0x00000001
#define CSMI_SAS_FWD_SOFT_RESET     0x00000002
#define CSMI_SAS_FWD_HARD_RESET     0x00000004

// Firmware Download Status
// (usStatus)
#define CSMI_SAS_FWD_SUCCESS        0
#define CSMI_SAS_FWD_FAILED         1
#define CSMI_SAS_FWD_USING_RROM     2
#define CSMI_SAS_FWD_REJECT         3
#define CSMI_SAS_FWD_DOWNREV        4

// Firmware Download Severity
// (usSeverity>
#define CSMI_SAS_FWD_INFORMATION    0
#define CSMI_SAS_FWD_WARNING        1
#define CSMI_SAS_FWD_ERROR          2
#define CSMI_SAS_FWD_FATAL          3

/* * * * * * * * * * SAS RAID Class IOCTL Constants  * * * * * * * * */

// Return codes for the RAID IOCTL's regardless of class
// (IoctlHeader.ReturnCode)

#define CSMI_SAS_RAID_SET_OUT_OF_RANGE       1000
#define CSMI_SAS_RAID_SET_BUFFER_TOO_SMALL   1001
#define CSMI_SAS_RAID_SET_DATA_CHANGED       1002

// Signature value
// (IoctlHeader.Signature)

#define CSMI_RAID_SIGNATURE    "CSMIARY"

// Timeout value default of 60 seconds
// (IoctlHeader.Timeout)

#define CSMI_RAID_TIMEOUT      60

// RAID Types
// (bRaidType)
#define CSMI_SAS_RAID_TYPE_NONE     0
#define CSMI_SAS_RAID_TYPE_0        1
#define CSMI_SAS_RAID_TYPE_1        2
#define CSMI_SAS_RAID_TYPE_10       3
#define CSMI_SAS_RAID_TYPE_5        4
#define CSMI_SAS_RAID_TYPE_15       5
#define CSMI_SAS_RAID_TYPE_6        6
#define CSMI_SAS_RAID_TYPE_50       7
#define CSMI_SAS_RAID_TYPE_VOLUME   8
#define CSMI_SAS_RAID_TYPE_1E       9
#define CSMI_SAS_RAID_TYPE_OTHER    255
// the last value 255 was already defined for other
// so end is defined as 254
#define CSMI_SAS_RAID_TYPE_END      254

// RAID Status
// (bStatus)
#define CSMI_SAS_RAID_SET_STATUS_OK             0
#define CSMI_SAS_RAID_SET_STATUS_DEGRADED       1
#define CSMI_SAS_RAID_SET_STATUS_REBUILDING     2
#define CSMI_SAS_RAID_SET_STATUS_FAILED         3
#define CSMI_SAS_RAID_SET_STATUS_OFFLINE        4
#define CSMI_SAS_RAID_SET_STATUS_TRANSFORMING   5
#define CSMI_SAS_RAID_SET_STATUS_QUEUED_FOR_REBUILD         6
#define CSMI_SAS_RAID_SET_STATUS_QUEUED_FOR_TRANSFORMATION  7

// RAID Drive Count
// (bDriveCount, 0xF1 to 0xFF are reserved)
#define CSMI_SAS_RAID_DRIVE_COUNT_TOO_BIG   0xF1
#define CSMI_SAS_RAID_DRIVE_COUNT_SUPRESSED 0xF2

// RAID Data Type
// (bDataType)
#define CSMI_SAS_RAID_DATA_DRIVES           0
#define CSMI_SAS_RAID_DATA_DEVICE_ID        1
#define CSMI_SAS_RAID_DATA_ADDITIONAL_DATA  2

// RAID Drive Status
// (bDriveStatus)
#define CSMI_SAS_DRIVE_STATUS_OK          0
#define CSMI_SAS_DRIVE_STATUS_REBUILDING  1
#define CSMI_SAS_DRIVE_STATUS_FAILED      2
#define CSMI_SAS_DRIVE_STATUS_DEGRADED    3
#define CSMI_SAS_DRIVE_STATUS_OFFLINE     4
#define CSMI_SAS_DRIVE_STATUS_QUEUED_FOR_REBUILD 5

// RAID Drive Usage
// (bDriveUsage)
#define CSMI_SAS_DRIVE_CONFIG_NOT_USED      0
#define CSMI_SAS_DRIVE_CONFIG_MEMBER        1
#define CSMI_SAS_DRIVE_CONFIG_SPARE         2
#define CSMI_SAS_DRIVE_CONFIG_SPARE_ACTIVE  3

// RAID Drive Type
// (bDriveType)
#define CSMI_SAS_DRIVE_TYPE_UNKNOWN         0
#define CSMI_SAS_DRIVE_TYPE_SINGLE_PORT_SAS 1
#define CSMI_SAS_DRIVE_TYPE_DUAL_PORT_SAS   2
#define CSMI_SAS_DRIVE_TYPE_SATA            3
#define CSMI_SAS_DRIVE_TYPE_SATA_PS         4
#define CSMI_SAS_DRIVE_TYPE_OTHER           255

// RAID Write Protect
// (bWriteProtect)
#define CSMI_SAS_RAID_SET_WRITE_PROTECT_UNKNOWN     0
#define CSMI_SAS_RAID_SET_WRITE_PROTECT_UNCHANGED   0
#define CSMI_SAS_RAID_SET_WRITE_PROTECT_ENABLED     1
#define CSMI_SAS_RAID_SET_WRITE_PROTECT_DISABLED    2

// RAID Cache Setting
// (bCacheSetting)
#define CSMI_SAS_RAID_SET_CACHE_UNKNOWN             0
#define CSMI_SAS_RAID_SET_CACHE_UNCHANGED           0
#define CSMI_SAS_RAID_SET_CACHE_ENABLED             1
#define CSMI_SAS_RAID_SET_CACHE_DISABLED            2
#define CSMI_SAS_RAID_SET_CACHE_CORRUPT             3

// RAID Features
// (uFeatures)
#define CSMI_SAS_RAID_FEATURE_TRANSFORMATION    0x00000001
#define CSMI_SAS_RAID_FEATURE_REBUILD           0x00000002
#define CSMI_SAS_RAID_FEATURE_SPLIT_MIRROR      0x00000004
#define CSMI_SAS_RAID_FEATURE_MERGE_MIRROR      0x00000008
#define CSMI_SAS_RAID_FEATURE_LUN_RENUMBER      0x00000010
#define CSMI_SAS_RAID_FEATURE_SURFACE_SCAN      0x00000020
#define CSMI_SAS_RAID_FEATURE_SPARES_SHARED     0x00000040

// RAID Priority
// (bDefaultTransformPriority, etc.)
#define CSMI_SAS_PRIORITY_UNKNOWN   0
#define CSMI_SAS_PRIORITY_UNCHANGED 0
#define CSMI_SAS_PRIORITY_AUTO      1
#define CSMI_SAS_PRIORITY_OFF       2
#define CSMI_SAS_PRIORITY_LOW       3
#define CSMI_SAS_PRIORITY_MEDIUM    4
#define CSMI_SAS_PRIORITY_HIGH      5

// RAID Transformation Rules
// (uRaidSetTransformationRules)
#define CSMI_SAS_RAID_RULE_AVAILABLE_MEMORY     0x00000001
#define CSMI_SAS_RAID_RULE_OVERLAPPED_EXTENTS   0x00000002

// RAID Cache Ratios Supported
// (bCacheRatiosSupported)
// from 0 to 100 defines the write to read ratio, 0 is 100% write
#define CSMI_SAS_RAID_CACHE_RATIO_RANGE     101
#define CSMI_SAS_RAID_CACHE_RATIO_FIXED     102
#define CSMI_SAS_RAID_CACHE_RATIO_AUTO      103
#define CSMI_SAS_RAID_CACHE_RATIO_END       255

// RAID Cache Ratio Flag
// (bCacheRatioFlag)
#define CSMI_SAS_RAID_CACHE_RATIO_DISABLE   0
#define CSMI_SAS_RAID_CACHE_RATIO_ENABLE    1

// RAID Clear Configuration Signature
// (bClearConfiguration)
#define CSMI_SAS_RAID_CLEAR_CONFIGURATION_SIGNATURE "RAIDCLR"

// RAID Failure Codes
// (uFailureCode)
#define CSMI_SAS_FAIL_CODE_OK                           0
#define CSMI_SAS_FAIL_CODE_PARAMETER_INVALID            1000
#define CSMI_SAS_FAIL_CODE_TRANSFORM_PRIORITY_INVALID   1001
#define CSMI_SAS_FAIL_CODE_REBUILD_PRIORITY_INVALID     1002
#define CSMI_SAS_FAIL_CODE_CACHE_RATIO_INVALID          1003
#define CSMI_SAS_FAIL_CODE_SURFACE_SCAN_INVALID         1004
#define CSMI_SAS_FAIL_CODE_CLEAR_CONFIGURATION_INVALID  1005
#define CSMI_SAS_FAIL_CODE_ELEMENT_INDEX_INVALID        1006
#define CSMI_SAS_FAIL_CODE_SUBELEMENT_INDEX_INVALID     1007
#define CSMI_SAS_FAIL_CODE_EXTENT_INVALID               1008
#define CSMI_SAS_FAIL_CODE_BLOCK_COUNT_INVALID          1009
#define CSMI_SAS_FAIL_CODE_DRIVE_INDEX_INVALID          1010
#define CSMI_SAS_FAIL_CODE_EXISTING_LUN_INVALID         1011
#define CSMI_SAS_FAIL_CODE_RAID_TYPE_INVALID            1012
#define CSMI_SAS_FAIL_CODE_STRIPE_SIZE_INVALID          1013
#define CSMI_SAS_FAIL_CODE_TRANSFORMATION_INVALID       1014
#define CSMI_SAS_FAIL_CODE_CHANGE_COUNT_INVALID         1015
#define CSMI_SAS_FAIL_CODE_ENUMERATION_TYPE_INVALID     1016

#define CSMI_SAS_FAIL_CODE_EXCEEDED_RAID_SET_COUNT      2000
#define CSMI_SAS_FAIL_CODE_DUPLICATE_LUN                2001

#define CSMI_SAS_FAIL_CODE_WAIT_FOR_OPERATION           3000

// RAID Enumeration Types
// (uEnumerationType)
#define CSMI_SAS_RAID_ELEMENT_TYPE_DRIVE                0
#define CSMI_SAS_RAID_ELEMENT_TYPE_MODULE               1
#define CSMI_SAS_RAID_ELEMENT_TYPE_DRIVE_RAID_SET       2
#define CSMI_SAS_RAID_ELEMENT_TYPE_EXTENT_DRIVE         3

// RAID Extent Types
// (bExtentType)
#define CSMI_SAS_RAID_EXTENT_RESERVED       0
#define CSMI_SAS_RAID_EXTENT_METADATA       1
#define CSMI_SAS_RAID_EXTENT_ALLOCATED      2
#define CSMI_SAS_RAID_EXTENT_UNALLOCATED    3

// RAID Operation Types
// (uOperationType)
#define CSMI_SAS_RAID_SET_CREATE            0
#define CSMI_SAS_RAID_SET_LABEL             1
#define CSMI_SAS_RAID_SET_TRANSFORM         2
#define CSMI_SAS_RAID_SET_DELETE            3
#define CSMI_SAS_RAID_SET_WRITE_PROTECT     4
#define CSMI_SAS_RAID_SET_CACHE             5
#define CSMI_SAS_RAID_SET_ONLINE_STATE      6
#define CSMI_SAS_RAID_SET_SPARE             7

// RAID Transform Types
// (bTransformType)
#define CSMI_SAS_RAID_SET_TRANSFORM_SPLIT_MIRROR    0
#define CSMI_SAS_RAID_SET_TRANSFORM_MERGE_RAID_0    1
#define CSMI_SAS_RAID_SET_TRANSFORM_LUN_RENUMBER    2
#define CSMI_SAS_RAID_SET_TRANSFORM_RAID_SET        3

// RAID Online State
// (bOnlineState)
#define CSMI_SAS_RAID_SET_STATE_UNKNOWN     0
#define CSMI_SAS_RAID_SET_STATE_ONLINE      1
#define CSMI_SAS_RAID_SET_STATE_OFFLINE     2

/* * * * * * * * * * SAS HBA Class IOCTL Constants * * * * * * * * * */

// Return codes for SAS IOCTL's
// (IoctlHeader.ReturnCode)

#define CSMI_SAS_PHY_INFO_CHANGED            CSMI_SAS_STATUS_SUCCESS
#define CSMI_SAS_PHY_INFO_NOT_CHANGEABLE     2000
#define CSMI_SAS_LINK_RATE_OUT_OF_RANGE      2001

#define CSMI_SAS_PHY_DOES_NOT_EXIST          2002
#define CSMI_SAS_PHY_DOES_NOT_MATCH_PORT     2003
#define CSMI_SAS_PHY_CANNOT_BE_SELECTED      2004
#define CSMI_SAS_SELECT_PHY_OR_PORT          2005
#define CSMI_SAS_PORT_DOES_NOT_EXIST         2006
#define CSMI_SAS_PORT_CANNOT_BE_SELECTED     2007
#define CSMI_SAS_CONNECTION_FAILED           2008

#define CSMI_SAS_NO_SATA_DEVICE              2009
#define CSMI_SAS_NO_SATA_SIGNATURE           2010
#define CSMI_SAS_SCSI_EMULATION              2011
#define CSMI_SAS_NOT_AN_END_DEVICE           2012
#define CSMI_SAS_NO_SCSI_ADDRESS             2013
#define CSMI_SAS_NO_DEVICE_ADDRESS           2014

// Signature value
// (IoctlHeader.Signature)

#define CSMI_SAS_SIGNATURE    "CSMISAS"

// Timeout value default of 60 seconds
// (IoctlHeader.Timeout)

#define CSMI_SAS_TIMEOUT      60

// Device types
// (bDeviceType)

#define CSMI_SAS_PHY_UNUSED               0x00
#define CSMI_SAS_NO_DEVICE_ATTACHED       0x00
#define CSMI_SAS_END_DEVICE               0x10
#define CSMI_SAS_EDGE_EXPANDER_DEVICE     0x20
#define CSMI_SAS_FANOUT_EXPANDER_DEVICE   0x30

// Protocol options
// (bInitiatorPortProtocol, bTargetPortProtocol)

#define CSMI_SAS_PROTOCOL_SATA   0x01
#define CSMI_SAS_PROTOCOL_SMP    0x02
#define CSMI_SAS_PROTOCOL_STP    0x04
#define CSMI_SAS_PROTOCOL_SSP    0x08

// Negotiated and hardware link rates
// (bNegotiatedLinkRate, bMinimumLinkRate, bMaximumLinkRate)

#define CSMI_SAS_LINK_RATE_UNKNOWN  0x00
#define CSMI_SAS_PHY_DISABLED       0x01
#define CSMI_SAS_LINK_RATE_FAILED   0x02
#define CSMI_SAS_SATA_SPINUP_HOLD   0x03
#define CSMI_SAS_SATA_PORT_SELECTOR 0x04
#define CSMI_SAS_LINK_RATE_1_5_GBPS 0x08
#define CSMI_SAS_LINK_RATE_3_0_GBPS 0x09
#define CSMI_SAS_LINK_VIRTUAL       0x10

// Discover state
// (bAutoDiscover)

#define CSMI_SAS_DISCOVER_NOT_SUPPORTED   0x00
#define CSMI_SAS_DISCOVER_NOT_STARTED     0x01
#define CSMI_SAS_DISCOVER_IN_PROGRESS     0x02
#define CSMI_SAS_DISCOVER_COMPLETE        0x03
#define CSMI_SAS_DISCOVER_ERROR           0x04

// Phy features

#define CSMI_SAS_PHY_VIRTUAL_SMP          0x01

// Programmed link rates
// (bMinimumLinkRate, bMaximumLinkRate)
// (bProgrammedMinimumLinkRate, bProgrammedMaximumLinkRate)

#define CSMI_SAS_PROGRAMMED_LINK_RATE_UNCHANGED 0x00
#define CSMI_SAS_PROGRAMMED_LINK_RATE_1_5_GBPS  0x08
#define CSMI_SAS_PROGRAMMED_LINK_RATE_3_0_GBPS  0x09

// Link rate
// (bNegotiatedLinkRate in CSMI_SAS_SET_PHY_INFO)

#define CSMI_SAS_LINK_RATE_NEGOTIATE      0x00
#define CSMI_SAS_LINK_RATE_PHY_DISABLED   0x01

// Signal class
// (bSignalClass in CSMI_SAS_SET_PHY_INFO)

#define CSMI_SAS_SIGNAL_CLASS_UNKNOWN     0x00
#define CSMI_SAS_SIGNAL_CLASS_DIRECT      0x01
#define CSMI_SAS_SIGNAL_CLASS_SERVER      0x02
#define CSMI_SAS_SIGNAL_CLASS_ENCLOSURE   0x03

// Link error reset
// (bResetCounts)

#define CSMI_SAS_LINK_ERROR_DONT_RESET_COUNTS   0x00
#define CSMI_SAS_LINK_ERROR_RESET_COUNTS        0x01

// Phy identifier
// (bPhyIdentifier)

#define CSMI_SAS_USE_PORT_IDENTIFIER   0xFF

// Port identifier
// (bPortIdentifier)

#define CSMI_SAS_IGNORE_PORT           0xFF

// Programmed link rates
// (bConnectionRate)

#define CSMI_SAS_LINK_RATE_NEGOTIATED  0x00
#define CSMI_SAS_LINK_RATE_1_5_GBPS    0x08
#define CSMI_SAS_LINK_RATE_3_0_GBPS    0x09

// Connection status
// (bConnectionStatus)

#define CSMI_SAS_OPEN_ACCEPT                          0
#define CSMI_SAS_OPEN_REJECT_BAD_DESTINATION          1
#define CSMI_SAS_OPEN_REJECT_RATE_NOT_SUPPORTED       2
#define CSMI_SAS_OPEN_REJECT_NO_DESTINATION           3
#define CSMI_SAS_OPEN_REJECT_PATHWAY_BLOCKED          4
#define CSMI_SAS_OPEN_REJECT_PROTOCOL_NOT_SUPPORTED   5
#define CSMI_SAS_OPEN_REJECT_RESERVE_ABANDON          6
#define CSMI_SAS_OPEN_REJECT_RESERVE_CONTINUE         7
#define CSMI_SAS_OPEN_REJECT_RESERVE_INITIALIZE       8
#define CSMI_SAS_OPEN_REJECT_RESERVE_STOP             9
#define CSMI_SAS_OPEN_REJECT_RETRY                    10
#define CSMI_SAS_OPEN_REJECT_STP_RESOURCES_BUSY       11
#define CSMI_SAS_OPEN_REJECT_WRONG_DESTINATION        12

// SSP Status
// (bSSPStatus)

#define CSMI_SAS_SSP_STATUS_UNKNOWN     0x00
#define CSMI_SAS_SSP_STATUS_WAITING     0x01
#define CSMI_SAS_SSP_STATUS_COMPLETED   0x02
#define CSMI_SAS_SSP_STATUS_FATAL_ERROR 0x03
#define CSMI_SAS_SSP_STATUS_RETRY       0x04
#define CSMI_SAS_SSP_STATUS_NO_TAG      0x05

// SSP Flags
// (uFlags)

#define CSMI_SAS_SSP_READ           0x00000001
#define CSMI_SAS_SSP_WRITE          0x00000002
#define CSMI_SAS_SSP_UNSPECIFIED    0x00000004

#define CSMI_SAS_SSP_TASK_ATTRIBUTE_SIMPLE         0x00000000
#define CSMI_SAS_SSP_TASK_ATTRIBUTE_HEAD_OF_QUEUE  0x00000010
#define CSMI_SAS_SSP_TASK_ATTRIBUTE_ORDERED        0x00000020
#define CSMI_SAS_SSP_TASK_ATTRIBUTE_ACA            0x00000040

// SSP Data present
// (bDataPresent)

#define CSMI_SAS_SSP_NO_DATA_PRESENT         0x00
#define CSMI_SAS_SSP_RESPONSE_DATA_PRESENT   0x01
#define CSMI_SAS_SSP_SENSE_DATA_PRESENT      0x02

// STP Flags
// (uFlags)

#define CSMI_SAS_STP_READ           0x00000001
#define CSMI_SAS_STP_WRITE          0x00000002
#define CSMI_SAS_STP_UNSPECIFIED    0x00000004
#define CSMI_SAS_STP_PIO            0x00000010
#define CSMI_SAS_STP_DMA            0x00000020
#define CSMI_SAS_STP_PACKET         0x00000040
#define CSMI_SAS_STP_DMA_QUEUED     0x00000080
#define CSMI_SAS_STP_EXECUTE_DIAG   0x00000100
#define CSMI_SAS_STP_RESET_DEVICE   0x00000200

// Task Management Flags
// (uFlags)

#define CSMI_SAS_TASK_IU               0x00000001
#define CSMI_SAS_HARD_RESET_SEQUENCE   0x00000002
#define CSMI_SAS_SUPPRESS_RESULT       0x00000004

// Task Management Functions
// (bTaskManagement)

#define CSMI_SAS_SSP_ABORT_TASK           0x01
#define CSMI_SAS_SSP_ABORT_TASK_SET       0x02
#define CSMI_SAS_SSP_CLEAR_TASK_SET       0x04
#define CSMI_SAS_SSP_LOGICAL_UNIT_RESET   0x08
#define CSMI_SAS_SSP_CLEAR_ACA            0x40
#define CSMI_SAS_SSP_QUERY_TASK           0x80

// Task Management Information
// (uInformation)

#define CSMI_SAS_SSP_TEST           1
#define CSMI_SAS_SSP_EXCEEDED       2
#define CSMI_SAS_SSP_DEMAND         3
#define CSMI_SAS_SSP_TRIGGER        4

// Connector Pinout Information
// (uPinout)

#define CSMI_SAS_CON_UNKNOWN              0x00000001
#define CSMI_SAS_CON_SFF_8482             0x00000002
#define CSMI_SAS_CON_SFF_8470_LANE_1      0x00000100
#define CSMI_SAS_CON_SFF_8470_LANE_2      0x00000200
#define CSMI_SAS_CON_SFF_8470_LANE_3      0x00000400
#define CSMI_SAS_CON_SFF_8470_LANE_4      0x00000800
#define CSMI_SAS_CON_SFF_8484_LANE_1      0x00010000
#define CSMI_SAS_CON_SFF_8484_LANE_2      0x00020000
#define CSMI_SAS_CON_SFF_8484_LANE_3      0x00040000
#define CSMI_SAS_CON_SFF_8484_LANE_4      0x00080000

// Connector Location Information
// (bLocation)

// same as uPinout above...
// #define CSMI_SAS_CON_UNKNOWN              0x01
#define CSMI_SAS_CON_INTERNAL             0x02
#define CSMI_SAS_CON_EXTERNAL             0x04
#define CSMI_SAS_CON_SWITCHABLE           0x08
#define CSMI_SAS_CON_AUTO                 0x10
#define CSMI_SAS_CON_NOT_PRESENT          0x20
#define CSMI_SAS_CON_NOT_CONNECTED        0x80

// Device location identification
// (bIdentify)

#define CSMI_SAS_LOCATE_UNKNOWN           0x00
#define CSMI_SAS_LOCATE_FORCE_OFF         0x01
#define CSMI_SAS_LOCATE_FORCE_ON          0x02

// Location Valid flags
// (uLocationFlags)

#define CSMI_SAS_LOCATE_SAS_ADDRESS_VALID           0x00000001
#define CSMI_SAS_LOCATE_SAS_LUN_VALID               0x00000002
#define CSMI_SAS_LOCATE_ENCLOSURE_IDENTIFIER_VALID  0x00000004
#define CSMI_SAS_LOCATE_ENCLOSURE_NAME_VALID        0x00000008
#define CSMI_SAS_LOCATE_BAY_PREFIX_VALID            0x00000010
#define CSMI_SAS_LOCATE_BAY_IDENTIFIER_VALID        0x00000020
#define CSMI_SAS_LOCATE_LOCATION_STATE_VALID        0x00000040

/* * * * * * * * SAS Phy Control Class IOCTL Constants * * * * * * * * */

// Return codes for SAS Phy Control IOCTL's
// (IoctlHeader.ReturnCode)

// Signature value
// (IoctlHeader.Signature)

#define CSMI_PHY_SIGNATURE    "CSMIPHY"

// Phy Control Functions
// (bFunction)

// values 0x00 to 0xFF are consistent in definition with the SMP PHY CONTROL
// function defined in the SAS spec
#define CSMI_SAS_PC_NOP                   0x00000000
#define CSMI_SAS_PC_LINK_RESET            0x00000001
#define CSMI_SAS_PC_HARD_RESET            0x00000002
#define CSMI_SAS_PC_PHY_DISABLE           0x00000003
// 0x04 to 0xFF reserved...
#define CSMI_SAS_PC_GET_PHY_SETTINGS      0x00000100

// Link Flags
#define CSMI_SAS_PHY_ACTIVATE_CONTROL     0x00000001
#define CSMI_SAS_PHY_UPDATE_SPINUP_RATE   0x00000002
#define CSMI_SAS_PHY_AUTO_COMWAKE         0x00000004

// Device Types for Phy Settings
// (bType)
#define CSMI_SAS_UNDEFINED 0x00
#define CSMI_SAS_SATA      0x01
#define CSMI_SAS_SAS       0x02

// Transmitter Flags
// (uTransmitterFlags)
#define CSMI_SAS_PHY_PREEMPHASIS_DISABLED    0x00000001

// Receiver Flags
// (uReceiverFlags)
#define CSMI_SAS_PHY_EQUALIZATION_DISABLED   0x00000001

// Pattern Flags
// (uPatternFlags)
// #define CSMI_SAS_PHY_ACTIVATE_CONTROL     0x00000001
#define CSMI_SAS_PHY_DISABLE_SCRAMBLING      0x00000002
#define CSMI_SAS_PHY_DISABLE_ALIGN           0x00000004
#define CSMI_SAS_PHY_DISABLE_SSC             0x00000008

#define CSMI_SAS_PHY_FIXED_PATTERN           0x00000010
#define CSMI_SAS_PHY_USER_PATTERN            0x00000020

// Fixed Patterns
// (bFixedPattern)
#define CSMI_SAS_PHY_CJPAT                   0x00000001
#define CSMI_SAS_PHY_ALIGN                   0x00000002

// Type Flags
// (bTypeFlags)
#define CSMI_SAS_PHY_POSITIVE_DISPARITY      0x01
#define CSMI_SAS_PHY_NEGATIVE_DISPARITY      0x02
#define CSMI_SAS_PHY_CONTROL_CHARACTER       0x04

// Miscellaneous
#define SLOT_NUMBER_UNKNOWN   0xFFFF

#pragma pack(8)

	typedef struct _CSMI_SAS_DRIVER_INFO
	{
		UCHAR  szName[81];
		UCHAR  szDescription[81];
		USHORT usMajorRevision;
		USHORT usMinorRevision;
		USHORT usBuildRevision;
		USHORT usReleaseRevision;
		USHORT usCSMIMajorRevision;
		USHORT usCSMIMinorRevision;
	} CSMI_SAS_DRIVER_INFO, *PCSMI_SAS_DRIVER_INFO;

	typedef struct _CSMI_SAS_DRIVER_INFO_BUFFER
	{
		SRB_IO_CONTROL IoctlHeader;
		CSMI_SAS_DRIVER_INFO Information;
	} CSMI_SAS_DRIVER_INFO_BUFFER, *PCSMI_SAS_DRIVER_INFO_BUFFER;

	typedef struct _CSMI_SAS_IDENTIFY
	{
		UCHAR bDeviceType;
		UCHAR bRestricted;
		UCHAR bInitiatorPortProtocol;
		UCHAR bTargetPortProtocol;
		UCHAR bRestricted2[8];
		UCHAR bSASAddress[8];
		UCHAR bPhyIdentifier;
		UCHAR bSignalClass;
		UCHAR bReserved[6];
	} CSMI_SAS_IDENTIFY, *PCSMI_SAS_IDENTIFY;

	typedef struct _CSMI_SAS_PHY_ENTITY
	{
		CSMI_SAS_IDENTIFY Identify;
		UCHAR bPortIdentifier;
		UCHAR bNegotiatedLinkRate;
		UCHAR bMinimumLinkRate;
		UCHAR bMaximumLinkRate;
		UCHAR bPhyChangeCount;
		UCHAR bAutoDiscover;
		UCHAR bPhyFeatures;
		UCHAR bReserved;
		CSMI_SAS_IDENTIFY Attached;
	} CSMI_SAS_PHY_ENTITY, *PCSMI_SAS_PHY_ENTITY;

	typedef struct _CSMI_SAS_PHY_INFO
	{
		UCHAR bNumberOfPhys;
		UCHAR bReserved[3];
		CSMI_SAS_PHY_ENTITY Phy[32];
	} CSMI_SAS_PHY_INFO, *PCSMI_SAS_PHY_INFO;

	typedef struct _CSMI_SAS_PHY_INFO_BUFFER
	{
		IOCTL_HEADER IoctlHeader;
		CSMI_SAS_PHY_INFO Information;
	} CSMI_SAS_PHY_INFO_BUFFER, *PCSMI_SAS_PHY_INFO_BUFFER;

	typedef struct _CSMI_SAS_STP_PASSTHRU
	{
		UCHAR bPhyIdentifier;
		UCHAR bPortIdentifier;
		UCHAR bConnectionRate;
		UCHAR bReserved;
		UCHAR bDestinationSASAddress[8];
		UCHAR bReserved2[4];
		UCHAR bCommandFIS[20];
		ULONG uFlags;
		ULONG uDataLength;
	} CSMI_SAS_STP_PASSTHRU, *PCSMI_SAS_STP_PASSTHRU;
	
	typedef struct _CSMI_SAS_STP_PASSTHRU_STATUS
	{
		UCHAR bConnectionStatus;
		UCHAR bReserved[3];
		UCHAR bStatusFIS[20];
		ULONG uSCR[16];
		ULONG uDataBytes;
	} CSMI_SAS_STP_PASSTHRU_STATUS, *PCSMI_SAS_STP_PASSTHRU_STATUS;
	
	typedef struct _CSMI_SAS_STP_PASSTHRU_BUFFER
	{
		SRB_IO_CONTROL IoctlHeader;
		CSMI_SAS_STP_PASSTHRU Parameters;
		CSMI_SAS_STP_PASSTHRU_STATUS Status;
		UCHAR bDataBuffer[1];
	} CSMI_SAS_STP_PASSTHRU_BUFFER, *PCSMI_SAS_STP_PASSTHRU_BUFFER;

// CC_CSMI_SAS_RAID_INFO

	typedef struct _CSMI_SAS_RAID_INFO
	{
		UINT uNumRaidSets;
		UINT uMaxDrivesPerSet;
		UINT uMaxRaidSets;
		UCHAR  bMaxRaidTypes;
		UCHAR  bReservedByteFields[7];
		struct
		{
			UINT uLowPart;
			UINT uHighPart;
		} ulMinRaidSetBlocks;
		struct
		{
			UINT uLowPart;
			UINT uHighPart;
		} ulMaxRaidSetBlocks;
		UINT uMaxPhysicalDrives;
		UINT uMaxExtents;
		UINT uMaxModules;
		UINT uMaxTransformationMemory;
		UINT uChangeCount;
		UCHAR  bReserved[44];
	} CSMI_SAS_RAID_INFO, *PCSMI_SAS_RAID_INFO;

	typedef struct _CSMI_SAS_RAID_INFO_BUFFER
	{
		IOCTL_HEADER IoctlHeader;
		CSMI_SAS_RAID_INFO Information;
	} CSMI_SAS_RAID_INFO_BUFFER, *PCSMI_SAS_RAID_INFO_BUFFER;

// CC_CSMI_SAS_GET_RAID_CONFIG

	typedef struct _CSMI_SAS_RAID_DRIVES
	{
		UCHAR  bModel[40];
		UCHAR  bFirmware[8];
		UCHAR  bSerialNumber[40];
		UCHAR  bSASAddress[8];
		UCHAR  bSASLun[8];
		UCHAR  bDriveStatus;
		UCHAR  bDriveUsage;
		USHORT usBlockSize;
		UCHAR  bDriveType;
		UCHAR  bReserved[15];
		UINT uDriveIndex;
	   struct
	   {
		  UINT uLowPart;
		  UINT uHighPart;
	   } ulTotalUserBlocks;
	} CSMI_SAS_RAID_DRIVES,
	  *PCSMI_SAS_RAID_DRIVES;

typedef struct _CSMI_SAS_RAID_DEVICE_ID {
   UCHAR  bDeviceIdentificationVPDPage[1];
} CSMI_SAS_RAID_DEVICE_ID,
  *PCSMI_SAS_RAID_DEVICE_ID;

typedef struct _CSMI_SAS_RAID_SET_ADDITIONAL_DATA {
   UCHAR  bLabel[16];
   UCHAR  bRaidSetLun[8];
   UCHAR  bWriteProtection;
   UCHAR  bCacheSetting;
   UCHAR  bCacheRatio;
   USHORT usBlockSize;
   UCHAR  bReservedBytes[11];
   struct
   {
	  UINT uLowPart;
	  UINT uHighPart;
   } ulRaidSetExtentOffset;
   struct
   {
	  UINT uLowPart;
	  UINT uHighPart;
   } ulRaidSetBlocks;
   UINT uStripeSizeInBlocks;
   UINT uSectorsPerTrack;
   UCHAR  bApplicationScratchPad[16];
   UINT uNumberOfHeads;
   UINT uNumberOfTracks;
   UCHAR  bReserved[24];
} CSMI_SAS_RAID_SET_ADDITIONAL_DATA,
  *PCSMI_SAS_RAID_SET_ADDITIONAL_DATA;

typedef struct _CSMI_SAS_RAID_CONFIG {
   UINT uRaidSetIndex;
   UINT uCapacity;
   UINT uStripeSize;
   UCHAR  bRaidType;
   UCHAR  bStatus;
   UCHAR  bInformation;
   UCHAR  bDriveCount;
   UCHAR  bDataType;
   UCHAR  bReserved[11];
   UINT uFailureCode;
   UINT uChangeCount;
   union {
	  CSMI_SAS_RAID_DRIVES Drives[1];
	  CSMI_SAS_RAID_DEVICE_ID DeviceId[1];
	  CSMI_SAS_RAID_SET_ADDITIONAL_DATA Data[1];
   };
} CSMI_SAS_RAID_CONFIG,
   *PCSMI_SAS_RAID_CONFIG;

typedef struct _CSMI_SAS_RAID_CONFIG_BUFFER {
   IOCTL_HEADER IoctlHeader;
   CSMI_SAS_RAID_CONFIG Configuration;
} CSMI_SAS_RAID_CONFIG_BUFFER,
  *PCSMI_SAS_RAID_CONFIG_BUFFER;


#pragma pack()
#endif


struct ATA_SMART_INFO
{
		BYTE				SmartReadData[512];
		BYTE				SmartReadThreshold[512];
		DWORD				AttributeCount;
		SMART_THRESHOLD		Threshold[MAX_ATTRIBUTE];
};

	struct EXTERNAL_DISK_INFO
	{
		CString Enclosure;
		DWORD	UsbVendorId;
		DWORD	UsbProductId;
	};

	BOOL FlagUsbSat;
	BOOL FlagUsbSunplus;
	BOOL FlagUsbIodata;
	BOOL FlagUsbLogitec;
	BOOL FlagUsbJmicron;
	BOOL FlagUsbCypress;

	DWORD CsmiType;

// My Methods - modifications of original methods to support external calls; also made them static since
// we don't need them as instance members.
static IDENTIFY_DEVICE_RESULT IdentifyInternalDrive(INT physicalDriveId, BYTE target, IDENTITY_DATA* outData);
static IDENTIFY_DEVICE_RESULT IdentifyUsbDrive(INT physicalDriveId, BYTE target, IDENTITY_DATA* outData, COMMAND_TYPE type);
static IDENTIFY_DEVICE_RESULT IdentifySiiDrive(INT physicalDriveId, INT scsiPort, INT scsiBus, INT scsiTarget, DWORD siliconImageType, IDENTITY_DATA* outData);
static IDENTIFY_DEVICE_RESULT IdentifyCsmiDrive(INT scsiPort, INT scsiTarget, INT scsiBus, IDENTITY_DATA* outData);
static CSMI_SAS_PHY_ENTITY GetCsmiDiskPhyEntity(INT scsiPort, INT scsiTarget, INT scsiBus);
static HANDLE GetIoCtrlHandleCsmi(INT scsiPort);
static HANDLE GetIoCtrlHandle(INT scsiPort, INT scsiPath, INT scsiTarget, DWORD siliconImageType);
static HANDLE GetIoCtrlHandle(INT scsiPort, DWORD siliconImageType);
static BOOL DoIdentifyDeviceCsmi(INT scsiPort, PCSMI_SAS_PHY_ENTITY sasPhyEntity, IDENTIFY_DEVICE* data);
static BOOL CsmiIoctl(HANDLE hHandle, UINT code, SRB_IO_CONTROL *csmiBuf, UINT csmiBufSize);
static BOOL IssueCommandToNonRemovable(INT physicalDriveId, BYTE target, BYTE main, BYTE sub, BYTE param, PBYTE data, DWORD dataSize);
static BOOL IssueCommandToNonRemovableCsmi(INT scsiPort, PCSMI_SAS_PHY_ENTITY sasPhyEntity, BYTE main, BYTE sub, BYTE param, PBYTE data, DWORD dataSize);
static SMART_QUERY_RESULT GetInternalSmartAttributes(INT physicalDriveId, BYTE target, SMART_DATA* smart);
static SMART_QUERY_RESULT GetInternalSmartThresholds(INT physicalDriveId, BYTE target, SMART_DATA* smart);
static SMART_QUERY_RESULT GetCsmiSmartAttributes(INT scsiPort, INT scsiTarget, INT scsiBus, SMART_DATA* smart);
static SMART_QUERY_RESULT GetCsmiSmartThresholds(INT scsiPort, INT scsiTarget, INT scsiBus, SMART_DATA* smart);
static SMART_QUERY_RESULT GetUsbSmartAttributes(INT PhysicalDriveId, BYTE target, COMMAND_TYPE cmdType, SMART_DATA_UCHAR* smart);
static SMART_QUERY_RESULT GetUsbSmartThresholds(INT physicalDriveId, BYTE target, COMMAND_TYPE cmdType, SMART_DATA_UCHAR* smart);
static SMART_QUERY_RESULT GetSiiSmartAttributes(INT physicalDriveId, SMART_DATA_SII* smart);
static SMART_QUERY_RESULT GetSiiSmartAttributes(INT scsiPort, INT scsiPath, INT scsiTarget, DWORD siliconImageType, SMART_DATA_SII* smart);
static SMART_QUERY_RESULT GetScsiSmartAttributes(INT scsiPort, INT scsiTargetId, SMART_DATA* smart);
static SMART_QUERY_RESULT GetScsiSmartThresholds(INT scsiPort, INT scsiTargetId, SMART_DATA* smart);
static DWORD SelfTestInternalDrive(INT physicalDriveId, BYTE target, INT testId);
static DWORD SelfTestUsbDrive(INT physicalDriveId, BYTE target, COMMAND_TYPE cmdType, INT testId);