// This is the main DLL file.

#include "stdafx.h"

#include "HomeServerSMART.Utility.h"

#include <afxole.h>

/*---------------------------------------------------------------------------*/
// \\\\.\\PhysicalDriveX
/*---------------------------------------------------------------------------*/
static HANDLE GetIoCtrlHandle(BYTE index)
{
	CString	strDevice;
	strDevice.Format(_T("\\\\.\\PhysicalDrive%d"), index);

	return ::CreateFile(strDevice, GENERIC_READ | GENERIC_WRITE, 
		FILE_SHARE_READ | FILE_SHARE_WRITE,
		NULL, OPEN_EXISTING, 0, NULL);
}

/*---------------------------------------------------------------------------*/
// Silicon Image Support
/*---------------------------------------------------------------------------*/
static HANDLE GetIoCtrlHandle(INT scsiPort, INT scsiPath, INT scsiTarget, DWORD siliconImageType)
{
	return GetIoCtrlHandle(scsiPort, siliconImageType);
}

static HANDLE GetIoCtrlHandle(INT scsiPort, DWORD siliconImageType)
{
	HANDLE hScsiDriveIOCTL = 0;
	CString driveName;

	driveName.Format(_T("\\\\.\\Scsi%d:"), scsiPort);
	hScsiDriveIOCTL = CreateFile(driveName, GENERIC_READ | GENERIC_WRITE,
							FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);
	if(hScsiDriveIOCTL == INVALID_HANDLE_VALUE)
	{
		TCHAR szBusPathBody[MAX_PATH];
		TCHAR szSilDeviceName[MAX_PATH];

		if(siliconImageType == 3512)
		{
			siliconImageType = 3112;
		}

		wsprintf(szBusPathBody, _T("tempBusSil%d"), siliconImageType);
		wsprintf(szSilDeviceName, _T("\\Device\\Scsi\\SI%d1"), siliconImageType) ;

		if(DefineDosDevice(DDD_RAW_TARGET_PATH, szBusPathBody, szSilDeviceName))
		{
			driveName.Format(_T("\\\\.\\%s"), szBusPathBody);
			return CreateFile(driveName, GENERIC_READ | GENERIC_WRITE,
						FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);
		}
	}
	return hScsiDriveIOCTL;
}

// The OLD way
//static HANDLE GetIoCtrlHandle(INT scsiPort, INT scsiPath, INT scsiTarget, DWORD siliconImageType)
//{
//	HANDLE hScsiDriveIOCTL = 0;
//	CString driveName;
//
//	// Try the hard way first.
//	TCHAR szBusPathBody[MAX_PATH];
//	TCHAR szSilDeviceName[MAX_PATH];
//
//	if(siliconImageType == 3512)
//	{
//		siliconImageType = 3112;
//	}
//
//	//LPTSTR siiDriveName;
//	//siiDriveName.Format(_T("\\Device\\Scsi\\SI%d1Port1Path%dTarget%d", siliconImageType, scsiPath, scsiTarget));
//
//	wsprintf(szBusPathBody, _T("tempBusSil%d"), siliconImageType);
//	wsprintf(szSilDeviceName, _T("\\Device\\Scsi\\Si%d1Port1Path%dTarget%dLun0"), siliconImageType, scsiPath, scsiTarget);
//
//	if(DefineDosDevice(DDD_RAW_TARGET_PATH, szBusPathBody, szSilDeviceName))
//	{
//		driveName.Format(_T("\\\\.\\%s"), szBusPathBody);
//		hScsiDriveIOCTL = CreateFile(driveName, GENERIC_READ | GENERIC_WRITE,
//			FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);
//	}
//
//	// If the hard way doesn't work, try the easy way.
//	if(hScsiDriveIOCTL == INVALID_HANDLE_VALUE)
//	{
//		driveName.Format(_T("\\\\.\\Scsi%d:"), scsiPort);
//		hScsiDriveIOCTL = CreateFile(driveName, GENERIC_READ | GENERIC_WRITE,
//			FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);
//	}
//	return hScsiDriveIOCTL;
//}

/*---------------------------------------------------------------------------*/
// CSMI support - SAS, some RAID
/*---------------------------------------------------------------------------*/
static HANDLE GetIoCtrlHandleCsmi(INT scsiPort)
{
	HANDLE hScsiDriveIOCTL = 0;
	CString driveName;

	driveName.Format(_T("\\\\.\\Scsi%d:"), scsiPort);
	hScsiDriveIOCTL = CreateFile(driveName, GENERIC_READ | GENERIC_WRITE,
							FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);
	return hScsiDriveIOCTL;
}

static CSMI_SAS_PHY_ENTITY GetCsmiDiskPhyEntity(INT scsiPort, INT scsiTarget, INT scsiBus)
{
	CSMI_SAS_PHY_ENTITY phyEntity = {0};

	CsmiType = CSMI_TYPE_ENABLE_ALL;

	if(CsmiType == CSMI_TYPE_DISABLE)
	{
		return phyEntity;
	}

	HANDLE hHandle = GetIoCtrlHandleCsmi(scsiPort);
	if(hHandle == INVALID_HANDLE_VALUE)
	{
		return phyEntity;
	}

	CSMI_SAS_DRIVER_INFO_BUFFER driverInfoBuf = {0};
	if(! CsmiIoctl(hHandle, CC_CSMI_SAS_GET_DRIVER_INFO, &driverInfoBuf.IoctlHeader, sizeof(driverInfoBuf)))
	{
		CloseHandle(hHandle);
		return phyEntity;
	}


	CSMI_SAS_RAID_INFO_BUFFER raidInfoBuf = {0};
	if(! CsmiIoctl(hHandle, CC_CSMI_SAS_GET_RAID_INFO, &raidInfoBuf.IoctlHeader, sizeof(raidInfoBuf)))
	{
		CloseHandle(hHandle);
		return phyEntity;
	}

//	CArray<CSMI_SAS_RAID_DRIVES, CSMI_SAS_RAID_DRIVES> raidDrives;
	CArray<UCHAR, UCHAR> raidDrives;

	DWORD size = sizeof(CSMI_SAS_RAID_CONFIG_BUFFER) + sizeof(CSMI_SAS_RAID_DRIVES) * raidInfoBuf.Information.uNumRaidSets * raidInfoBuf.Information.uMaxDrivesPerSet;
	PCSMI_SAS_RAID_CONFIG_BUFFER buf = (PCSMI_SAS_RAID_CONFIG_BUFFER)VirtualAlloc(NULL, size, MEM_COMMIT, PAGE_READWRITE);
	for(UINT i = 0; i < raidInfoBuf.Information.uNumRaidSets; i++)
	{
		buf->Configuration.uRaidSetIndex = i;
		if(! CsmiIoctl(hHandle, CC_CSMI_SAS_GET_RAID_CONFIG, &(buf->IoctlHeader), size))
		{
			CloseHandle(hHandle);
			return phyEntity;
		}
		else
		{
			for(UINT j = 0; j < raidInfoBuf.Information.uMaxDrivesPerSet; j++)
			{
				if(buf->Configuration.Drives[j].bModel[0] != '\0')
				{
				//	raidDrives.Add(buf->Configuration.Drives[j]);
					raidDrives.Add(buf->Configuration.Drives[j].bSASAddress[2]);
				}
			}
		}
	}
	VirtualFree(buf, 0, MEM_RELEASE);

	CSMI_SAS_PHY_INFO phyInfo = {0};
	CSMI_SAS_PHY_INFO_BUFFER phyInfoBuf = {0};
	if (! CsmiIoctl(hHandle, CC_CSMI_SAS_GET_PHY_INFO, &phyInfoBuf.IoctlHeader, sizeof(phyInfoBuf)))
	{
		CloseHandle(hHandle);
		return phyEntity;
	}
	memcpy(&phyInfo, &(phyInfoBuf.Information), sizeof(phyInfoBuf.Information));
	
	IDENTIFY_DEVICE identify = {0};	
	if(phyInfo.bNumberOfPhys <= sizeof(phyInfo.Phy)/sizeof(phyInfo.Phy[0]))
	{
		for(int i = 0; i < phyInfo.bNumberOfPhys; i++)
		{
			if(CsmiType == CSMI_TYPE_ENABLE_RAID)
			{
				for(int j = 0; j < raidDrives.GetCount(); j++)
				{
				//	PCSMI_SAS_RAID_DRIVES test;
				//	test = &(raidDrives.GetAt(j));
				//	if(memcmp(raidDrives.GetAt(j).bSASAddress, phyInfo.Phy[i].Attached.bSASAddress, 8) == 0)
				//	if(raidDrives.GetAt(j).bSASAddress[2] == phyInfo.Phy[i].Attached.bSASAddress[2])
					if(raidDrives.GetAt(j) == phyInfo.Phy[i].Attached.bSASAddress[2])
					{
						if(DoIdentifyDeviceCsmi(scsiPort, &(phyInfo.Phy[i]), &identify))
						{
							if(scsiTarget == 0)
							{
								if(phyInfo.Phy[i].bPortIdentifier == scsiBus)
								{
									phyEntity = phyInfo.Phy[i];
									break;
								}
							}
							else if(i == scsiTarget)
							{
								phyEntity = phyInfo.Phy[i];
								break;
							}
						}
						break;
					}
				}
			}
			else
			{
				if(DoIdentifyDeviceCsmi(scsiPort, &(phyInfo.Phy[i]), &identify))
				{
					if(scsiTarget == 0)
					{
						//if(i == scsiBus)
						//{
						//	//phyEntity = phyInfo.Phy[i];
						//	for(int x = 0; x < phyInfo.bNumberOfPhys; x++)
						//	{
						//		if(phyInfo.Phy[x].bPortIdentifier == scsiBus)
						//		{
						//			phyEntity = phyInfo.Phy[x];
						//			break;
						//		}
						//	}
						//	break;
						//}
						if(phyInfo.Phy[i].bPortIdentifier == scsiBus)
						{
							phyEntity = phyInfo.Phy[i];
							break;
						}
					}
					else if(i == scsiTarget)
					{
						phyEntity = phyInfo.Phy[i];
						break;
					}
				}
			}
		}
	}	

	CloseHandle(hHandle);
	return phyEntity;
}

/*
 * IDENTIFY_DEVICE Section
 */

static IDENTIFY_DEVICE_RESULT IdentifyInternalDrive(INT physicalDriveId, BYTE target, IDENTITY_DATA* outData)
{
	BOOL	bRet = FALSE;
	HANDLE	hIoCtrl;
	DWORD	dwReturned;
	CString cstr;

	IDENTIFY_DEVICE_RESULT result;
	IDENTIFY_DEVICE* data = new IDENTIFY_DEVICE();

	IDENTIFY_DEVICE_OUTDATA	sendCmdOutParam;
	SENDCMDINPARAMS	sendCmd;

	if(outData == NULL)
	{
		result = IDENTIFY_DEVICE_NULL;
		return result;
	}

	//DebugPrint(_T("SendAtaCommandPd - IDENTIFY_DEVICE (ATA_PASS_THROUGH)"));
	bRet = IssueCommandToNonRemovable(physicalDriveId, target, 0xEC, 0x00, 0x00, (PBYTE)data, sizeof(IDENTIFY_DEVICE));
	cstr = data->Model;

	if(bRet == FALSE || cstr.IsEmpty())
	{
		::ZeroMemory(data, sizeof(IDENTIFY_DEVICE));
		hIoCtrl = GetIoCtrlHandle(physicalDriveId);
		if(hIoCtrl == INVALID_HANDLE_VALUE)
		{
			result = IDENTIFY_DEVICE_INVALID_HANDLE;
			return result;
		}
		::ZeroMemory(&sendCmdOutParam, sizeof(IDENTIFY_DEVICE_OUTDATA));
		::ZeroMemory(&sendCmd, sizeof(SENDCMDINPARAMS));

		sendCmd.irDriveRegs.bCommandReg			= ID_CMD;
		sendCmd.irDriveRegs.bSectorCountReg		= 1;
		sendCmd.irDriveRegs.bSectorNumberReg	= 1;
		sendCmd.irDriveRegs.bDriveHeadReg		= target;
		sendCmd.cBufferSize						= IDENTIFY_BUFFER_SIZE;

		//DebugPrint(_T("IssueCommandToNonRemovable - IDENTIFY_DEVICE"));
		bRet = ::DeviceIoControl(hIoCtrl, DFP_RECEIVE_DRIVE_DATA, 
			&sendCmd, sizeof(SENDCMDINPARAMS),
			&sendCmdOutParam, sizeof(IDENTIFY_DEVICE_OUTDATA),
			&dwReturned, NULL);

		::CloseHandle(hIoCtrl);
		
		if(bRet == FALSE || dwReturned != sizeof(IDENTIFY_DEVICE_OUTDATA))
		{
			if( bRet == FALSE )
			{
				result = IDENTIFY_DEVICE_IO_CONTROL_FALSE;
			}
			else
			{
				result = IDENTIFY_DEVICE_OUTDATA_SIZE_MISMATCH;
			}
			return result;
		}

		memcpy_s(data, sizeof(IDENTIFY_DEVICE), sendCmdOutParam.SendCmdOutParam.bBuffer, sizeof(IDENTIFY_DEVICE));
		// Moved remand to below the curly brace. ~ Dan ~
	}
	// Remand the data into our smaller IDENTITY_DATA structure so it can be returned.
	outData->LogicalCylinders = data->LogicalCylinders;
	outData->LogicalHeads = data->LogicalHeads;
	outData->LogicalSectors = data->LogicalSectors;
	outData->BufferSize = data->BufferSize;
	outData->TotalAddressableSectors = data->TotalAddressableSectors;
	outData->PioMode = data->PioMode;
	outData->SerialAtaCapabilities = data->SerialAtaCapabilities;
	outData->SerialAtaFeaturesSupported = data->SerialAtaFeaturesSupported;
	outData->SerialAtaFeaturesEnabled = data->SerialAtaFeaturesEnabled;
	outData->UltraDmaMode = data->UltraDmaMode;
	outData->TimeReqForSecurityErase = data->TimeReqForSecurityErase;
	outData->TimeReqForEnhancedSecure = data->TimeReqForEnhancedSecure;
	outData->AcousticManagement = data->AcousticManagement;
	outData->SectorSize = data->SectorSize;
	outData->DataSetManagement = data->DataSetManagement;
	outData->NominalMediaRotationRate = data->NominalMediaRotationRate;
	for(int i = 0; i < 20; i++)
	{
		outData->SerialNumber[i] = data->SerialNumber[i];
	}
	for(int i = 0; i < 40; i++)
	{
		outData->Model[i] = data->Model[i];
	}
	for(int i = 0; i < 8; i++)
	{
		outData->FirmwareRev[i] = data->FirmwareRev[i];
	}

	return IDENTIFY_DEVICE_RESULT_OK;
}

/*
 *  Support for Common Storage Management Interface (CSMI) drives, which includes some RAID controllers and SAS disks.
 */
static IDENTIFY_DEVICE_RESULT IdentifyCsmiDrive(INT scsiPort, INT scsiTarget, INT scsiBus, IDENTITY_DATA* outData)
{
	BOOL retVal = FALSE;
	CSMI_SAS_PHY_ENTITY sasPhyEntity = GetCsmiDiskPhyEntity(scsiPort, scsiTarget, scsiBus);
	IDENTIFY_DEVICE* identityData = new IDENTIFY_DEVICE();
	retVal = IssueCommandToNonRemovableCsmi(scsiPort, &sasPhyEntity, 0xEC, 0x00, 0x00, (PBYTE)identityData, sizeof(IDENTIFY_DEVICE));
	if(retVal)
	{
		// Remand the data into our smaller IDENTITY_DATA structure so it can be returned.
		outData->LogicalCylinders = identityData->LogicalCylinders;
		outData->LogicalHeads = identityData->LogicalHeads;
		outData->LogicalSectors = identityData->LogicalSectors;
		outData->BufferSize = identityData->BufferSize;
		outData->TotalAddressableSectors = identityData->TotalAddressableSectors;
		outData->PioMode = identityData->PioMode;
		outData->SerialAtaCapabilities = identityData->SerialAtaCapabilities;
		outData->SerialAtaFeaturesSupported = identityData->SerialAtaFeaturesSupported;
		outData->SerialAtaFeaturesEnabled = identityData->SerialAtaFeaturesEnabled;
		outData->UltraDmaMode = identityData->UltraDmaMode;
		outData->TimeReqForSecurityErase = identityData->TimeReqForSecurityErase;
		outData->TimeReqForEnhancedSecure = identityData->TimeReqForEnhancedSecure;
		outData->AcousticManagement = identityData->AcousticManagement;
		outData->SectorSize = identityData->SectorSize;
		outData->DataSetManagement = identityData->DataSetManagement;
		outData->NominalMediaRotationRate = identityData->NominalMediaRotationRate;
		for(int i = 0; i < 20; i++)
		{
			outData->SerialNumber[i] = identityData->SerialNumber[i];
		}
		for(int i = 0; i < 40; i++)
		{
			outData->Model[i] = identityData->Model[i];
		}
		for(int i = 0; i < 8; i++)
		{
			outData->FirmwareRev[i] = identityData->FirmwareRev[i];
		}
		return IDENTIFY_DEVICE_RESULT_OK;
	}
	else
	{
		return IDENTIFY_DEVICE_IO_CONTROL_FALSE;
	}
}

static SMART_QUERY_RESULT GetCsmiSmartAttributes(INT scsiPort, INT scsiTarget, INT scsiBus, SMART_DATA* smart)
{
	//SendAtaCommandCsmi(scsiPort, sasPhyEntity, SMART_CMD, READ_ATTRIBUTES, 0x00, (PBYTE)asi->SmartReadData, sizeof(asi->SmartReadData)
	BOOL retVal = FALSE;
	CSMI_SAS_PHY_ENTITY sasPhyEntity = GetCsmiDiskPhyEntity(scsiPort, scsiTarget, scsiBus);
	retVal = IssueCommandToNonRemovableCsmi(scsiPort, &sasPhyEntity, SMART_CMD, READ_ATTRIBUTES, 0x00, (PBYTE)smart, sizeof(SMART_DATA));
	if(retVal)
	{
		return SMART_QUERY_RESULT_OK;
	}
	else
	{
		return SMART_QUERY_IO_CONTROL_FALSE;
	}
}

static SMART_QUERY_RESULT GetCsmiSmartThresholds(INT scsiPort, INT scsiTarget, INT scsiBus, SMART_DATA* smart)
{
	//SendAtaCommandCsmi(scsiPort, sasPhyEntity, SMART_CMD, READ_THRESHOLDS, 0x00, (PBYTE)asi->SmartReadThreshold, sizeof(asi->SmartReadThreshold)
	BOOL retVal = FALSE;
	CSMI_SAS_PHY_ENTITY sasPhyEntity = GetCsmiDiskPhyEntity(scsiPort, scsiTarget, scsiBus);
	retVal = IssueCommandToNonRemovableCsmi(scsiPort, &sasPhyEntity, SMART_CMD, READ_THRESHOLDS, 0x00, (PBYTE)smart, sizeof(SMART_DATA));
	if(retVal)
	{
		return SMART_QUERY_RESULT_OK;
	}
	else
	{
		return SMART_QUERY_IO_CONTROL_FALSE;
	}
}

static BOOL DoIdentifyDeviceCsmi(INT scsiPort, PCSMI_SAS_PHY_ENTITY sasPhyEntity, IDENTIFY_DEVICE* data)
{
	BOOL flag = FALSE;
	return IssueCommandToNonRemovableCsmi(scsiPort, sasPhyEntity, 0xEC, 0x00, 0x00, (PBYTE)data, sizeof(IDENTIFY_DEVICE));
}

static IDENTIFY_DEVICE_RESULT IdentifyUsbDrive(INT physicalDriveId, BYTE target, IDENTITY_DATA* outData, COMMAND_TYPE type)
{
	BOOL	bRet;
	HANDLE	hIoCtrl;
	DWORD	dwReturned;
	DWORD	length;

	SCSI_PASS_THROUGH_WITH_BUFFERS sptwb;

	IDENTIFY_DEVICE_RESULT result;
	IDENTIFY_DEVICE* data = new IDENTIFY_DEVICE();

	if(data == NULL)
	{
		result = IDENTIFY_DEVICE_NULL;
		return result;
	}

	::ZeroMemory(data, sizeof(IDENTIFY_DEVICE));

	hIoCtrl = GetIoCtrlHandle(physicalDriveId);
	if(hIoCtrl == INVALID_HANDLE_VALUE)
	{
		result = IDENTIFY_DEVICE_INVALID_HANDLE;
			return result;
	}

	::ZeroMemory(&sptwb,sizeof(SCSI_PASS_THROUGH_WITH_BUFFERS));

	sptwb.Spt.Length = sizeof(SCSI_PASS_THROUGH);
	sptwb.Spt.PathId = 0;
	sptwb.Spt.TargetId = 0;
	sptwb.Spt.Lun = 0;
	sptwb.Spt.SenseInfoLength = 24;
	sptwb.Spt.DataIn = SCSI_IOCTL_DATA_IN;
	sptwb.Spt.DataTransferLength = IDENTIFY_BUFFER_SIZE;
	sptwb.Spt.TimeOutValue = 2;
	sptwb.Spt.DataBufferOffset = offsetof(SCSI_PASS_THROUGH_WITH_BUFFERS, DataBuf);
	sptwb.Spt.SenseInfoOffset = offsetof(SCSI_PASS_THROUGH_WITH_BUFFERS, SenseBuf);

	if(type == CMD_TYPE_SAT)
	{
		sptwb.Spt.CdbLength = 12;
		sptwb.Spt.Cdb[0] = 0xA1;//ATA PASS THROUGH(12) OPERATION CODE(A1h)
		sptwb.Spt.Cdb[1] = (4 << 1) | 0; //MULTIPLE_COUNT=0,PROTOCOL=4(PIO Data-In),Reserved
		sptwb.Spt.Cdb[2] = (1 << 3) | (1 << 2) | 2;//OFF_LINE=0,CK_COND=0,Reserved=0,T_DIR=1(ToDevice),BYTE_BLOCK=1,T_LENGTH=2
		sptwb.Spt.Cdb[3] = 0;//FEATURES (7:0)
		sptwb.Spt.Cdb[4] = 1;//SECTOR_COUNT (7:0)
		sptwb.Spt.Cdb[5] = 0;//LBA_LOW (7:0)
		sptwb.Spt.Cdb[6] = 0;//LBA_MID (7:0)
		sptwb.Spt.Cdb[7] = 0;//LBA_HIGH (7:0)
		sptwb.Spt.Cdb[8] = target;
		sptwb.Spt.Cdb[9] = ID_CMD;//COMMAND
	}
	else if(type == CMD_TYPE_SUNPLUS)
	{
		sptwb.Spt.CdbLength = 12;
		sptwb.Spt.Cdb[0] = 0xF8;
		sptwb.Spt.Cdb[1] = 0x00;
		sptwb.Spt.Cdb[2] = 0x22;
		sptwb.Spt.Cdb[3] = 0x10;
		sptwb.Spt.Cdb[4] = 0x01;
		sptwb.Spt.Cdb[5] = 0x00; 
		sptwb.Spt.Cdb[6] = 0x01; 
		sptwb.Spt.Cdb[7] = 0x00; 
		sptwb.Spt.Cdb[8] = 0x00;
		sptwb.Spt.Cdb[9] = 0x00;
		sptwb.Spt.Cdb[10] = target; 
		sptwb.Spt.Cdb[11] = 0xEC; // ID_CMD
	}
	else if(type == CMD_TYPE_IO_DATA)
	{
		sptwb.Spt.CdbLength = 12;
		sptwb.Spt.Cdb[0] = 0xE3;
		sptwb.Spt.Cdb[1] = 0x00;
		sptwb.Spt.Cdb[2] = 0x00;
		sptwb.Spt.Cdb[3] = 0x01;
		sptwb.Spt.Cdb[4] = 0x01;
		sptwb.Spt.Cdb[5] = 0x00; 
		sptwb.Spt.Cdb[6] = 0x00; 
		sptwb.Spt.Cdb[7] = target;
		sptwb.Spt.Cdb[8] = 0xEC;  // ID_CMD
		sptwb.Spt.Cdb[9] = 0x00;
		sptwb.Spt.Cdb[10] = 0x00; 
		sptwb.Spt.Cdb[11] = 0x00;
	}
	else if(type == CMD_TYPE_LOGITEC)
	{
		sptwb.Spt.CdbLength = 10;
		sptwb.Spt.Cdb[0] = 0xE0;
		sptwb.Spt.Cdb[1] = 0x00;
		sptwb.Spt.Cdb[2] = 0x00;
		sptwb.Spt.Cdb[3] = 0x00;
		sptwb.Spt.Cdb[4] = 0x00;
		sptwb.Spt.Cdb[5] = 0x00; 
		sptwb.Spt.Cdb[6] = 0x00; 
		sptwb.Spt.Cdb[7] = target; 
		sptwb.Spt.Cdb[8] = 0xEC;  // ID_CMD
		sptwb.Spt.Cdb[9] = 0x4C;
	}
	else if(type == CMD_TYPE_JMICRON)
	{
		sptwb.Spt.CdbLength = 12;
		sptwb.Spt.Cdb[0] = 0xDF;
		sptwb.Spt.Cdb[1] = 0x10;
		sptwb.Spt.Cdb[2] = 0x00;
		sptwb.Spt.Cdb[3] = 0x02;
		sptwb.Spt.Cdb[4] = 0x00;
		sptwb.Spt.Cdb[5] = 0x00; 
		sptwb.Spt.Cdb[6] = 0x01; 
		sptwb.Spt.Cdb[7] = 0x00; 
		sptwb.Spt.Cdb[8] = 0x00;
		sptwb.Spt.Cdb[9] = 0x00;
		sptwb.Spt.Cdb[10] = target; 
		sptwb.Spt.Cdb[11] = 0xEC; // ID_CMD
	}
	else if(type == CMD_TYPE_CYPRESS)
	{
		sptwb.Spt.CdbLength = 16;
		sptwb.Spt.Cdb[0] = 0x24;
		sptwb.Spt.Cdb[1] = 0x24;
		sptwb.Spt.Cdb[2] = 0x00;
		sptwb.Spt.Cdb[3] = 0xBE;
		sptwb.Spt.Cdb[4] = 0x01;
		sptwb.Spt.Cdb[5] = 0x00; 
		sptwb.Spt.Cdb[6] = 0x00; 
		sptwb.Spt.Cdb[7] = 0x01; 
		sptwb.Spt.Cdb[8] = 0x00;
		sptwb.Spt.Cdb[9] = 0x00;
		sptwb.Spt.Cdb[10] = 0x00; 
		sptwb.Spt.Cdb[11] = target;
		sptwb.Spt.Cdb[12] = 0xEC; // ID_CMD
		sptwb.Spt.Cdb[13] = 0x00;
		sptwb.Spt.Cdb[14] = 0x00;
		sptwb.Spt.Cdb[15] = 0x00;
	}
	else
	{
		result = IDENTIFY_DEVICE_EXCEPTION;
		return result;
	}

	length = offsetof(SCSI_PASS_THROUGH_WITH_BUFFERS, DataBuf) + sptwb.Spt.DataTransferLength;

	bRet = ::DeviceIoControl(hIoCtrl, IOCTL_SCSI_PASS_THROUGH, 
		&sptwb, sizeof(SCSI_PASS_THROUGH),
		&sptwb, length,	&dwReturned, NULL);

	::CloseHandle(hIoCtrl);
	
	if(bRet == FALSE || dwReturned != length)
	{
		if( bRet == FALSE )
		{
			result = IDENTIFY_DEVICE_IO_CONTROL_FALSE;
		}
		else
		{
			result = IDENTIFY_DEVICE_OUTDATA_SIZE_MISMATCH;
		}
		return result;
	}

	memcpy_s(data, sizeof(IDENTIFY_DEVICE), sptwb.DataBuf, sizeof(IDENTIFY_DEVICE));
	// Remand the data into our smaller IDENTITY_DATA structure so it can be returned.
	outData->LogicalCylinders = data->LogicalCylinders;
	outData->LogicalHeads = data->LogicalHeads;
	outData->LogicalSectors = data->LogicalSectors;
	outData->BufferSize = data->BufferSize;
	outData->TotalAddressableSectors = data->TotalAddressableSectors;
	outData->PioMode = data->PioMode;
	outData->SerialAtaCapabilities = data->SerialAtaCapabilities;
	outData->SerialAtaFeaturesSupported = data->SerialAtaFeaturesSupported;
	outData->SerialAtaFeaturesEnabled = data->SerialAtaFeaturesEnabled;
	outData->UltraDmaMode = data->UltraDmaMode;
	outData->TimeReqForSecurityErase = data->TimeReqForSecurityErase;
	outData->TimeReqForEnhancedSecure = data->TimeReqForEnhancedSecure;
	outData->AcousticManagement = data->AcousticManagement;
	outData->SectorSize = data->SectorSize;
	outData->DataSetManagement = data->DataSetManagement;
	outData->NominalMediaRotationRate = data->NominalMediaRotationRate;
	for(int i = 0; i < 20; i++)
	{
		outData->SerialNumber[i] = data->SerialNumber[i];
	}
	for(int i = 0; i < 40; i++)
	{
		outData->Model[i] = data->Model[i];
	}
	for(int i = 0; i < 8; i++)
	{
		outData->FirmwareRev[i] = data->FirmwareRev[i];
	}

	return IDENTIFY_DEVICE_RESULT_OK;
}

static IDENTIFY_DEVICE_RESULT IdentifySiiDrive(INT physicalDriveId, INT scsiPort, INT scsiBus, INT scsiTarget, DWORD siliconImageType, IDENTITY_DATA* outData)
{
	int done = FALSE;
	int controller = 0;
	int current = 0;
	HANDLE hScsiDriveIOCTL = 0;

	IDENTIFY_DEVICE_RESULT result;
	IDENTIFY_DEVICE* data = new IDENTIFY_DEVICE();

	hScsiDriveIOCTL = GetIoCtrlHandle(scsiPort, scsiBus, scsiTarget, siliconImageType);

	if(hScsiDriveIOCTL == INVALID_HANDLE_VALUE)
	{
		// Let's give 'er another try.
		hScsiDriveIOCTL = GetIoCtrlHandle(physicalDriveId);
	}

	if(hScsiDriveIOCTL != INVALID_HANDLE_VALUE)
	{
		SilIdentDev sid;
		memset(&sid, 0, sizeof(sid));
		
		sid.sic.HeaderLength = sizeof(SRB_IO_CONTROL);
		memcpy(sid.sic.Signature, "CMD_IDE ", 8);
		sid.sic.Timeout = 5;
		sid.sic.ControlCode = CTL_CODE(FILE_DEVICE_CONTROLLER, 0x802, METHOD_BUFFERED, FILE_ANY_ACCESS);
		sid.sic.ReturnCode = 0xffffffff;
		sid.sic.Length = sizeof(sid) - offsetof(SilIdentDev, port);
		//sid.port = scsiBus;
		sid.port = scsiTarget;
		sid.maybe_always1 = 1 ;

		DWORD dwReturnBytes;
		if(DeviceIoControl(hScsiDriveIOCTL, IOCTL_SCSI_MINIPORT, &sid, sizeof(sid), &sid, sizeof(sid), &dwReturnBytes, NULL))
		{
			memcpy_s(data, sizeof(IDENTIFY_DEVICE), &sid.id_data, sizeof(IDENTIFY_DEVICE));
			result = IDENTIFY_DEVICE_RESULT_OK;
			// Remand the data into our smaller IDENTITY_DATA structure so it can be returned.
			outData->LogicalCylinders = data->LogicalCylinders;
			outData->LogicalHeads = data->LogicalHeads;
			outData->LogicalSectors = data->LogicalSectors;
			outData->BufferSize = data->BufferSize;
			outData->TotalAddressableSectors = data->TotalAddressableSectors;
			outData->PioMode = data->PioMode;
			outData->SerialAtaCapabilities = data->SerialAtaCapabilities;
			outData->SerialAtaFeaturesSupported = data->SerialAtaFeaturesSupported;
			outData->SerialAtaFeaturesEnabled = data->SerialAtaFeaturesEnabled;
			outData->UltraDmaMode = data->UltraDmaMode;
			outData->TimeReqForSecurityErase = data->TimeReqForSecurityErase;
			outData->TimeReqForEnhancedSecure = data->TimeReqForEnhancedSecure;
			outData->AcousticManagement = data->AcousticManagement;
			outData->SectorSize = data->SectorSize;
			outData->DataSetManagement = data->DataSetManagement;
			outData->NominalMediaRotationRate = data->NominalMediaRotationRate;
			for(int i = 0; i < 20; i++)
			{
				outData->SerialNumber[i] = data->SerialNumber[i];
			}
			for(int i = 0; i < 40; i++)
			{
				outData->Model[i] = data->Model[i];
			}
			for(int i = 0; i < 8; i++)
			{
				outData->FirmwareRev[i] = data->FirmwareRev[i];
			}
			CloseHandle(hScsiDriveIOCTL);
			return result;
		}

		CloseHandle(hScsiDriveIOCTL);
	}
	else
	{
		result = IDENTIFY_DEVICE_INVALID_HANDLE;
			return result;
	}
}

static BOOL IssueCommandToNonRemovableCsmi(INT scsiPort, PCSMI_SAS_PHY_ENTITY sasPhyEntity, BYTE main, BYTE sub, BYTE param, PBYTE data, DWORD dataSize)
{
	HANDLE hIoCtrl = GetIoCtrlHandleCsmi(scsiPort);
	if(hIoCtrl == INVALID_HANDLE_VALUE)
	{
		return	FALSE;
	}

	DWORD size = sizeof(CSMI_SAS_STP_PASSTHRU_BUFFER) + dataSize;
	CSMI_SAS_STP_PASSTHRU_BUFFER* buf = (CSMI_SAS_STP_PASSTHRU_BUFFER*)VirtualAlloc(NULL, size, MEM_COMMIT, PAGE_READWRITE);

	buf->Parameters.bPhyIdentifier = sasPhyEntity->Attached.bPhyIdentifier;
	buf->Parameters.bPortIdentifier = sasPhyEntity->bPortIdentifier;
	memcpy(&(buf->Parameters.bDestinationSASAddress), sasPhyEntity->Attached.bSASAddress, sizeof(sasPhyEntity->Attached.bSASAddress));
	buf->Parameters.bConnectionRate = CSMI_SAS_LINK_RATE_NEGOTIATED;

	if(main == 0xEF) // AAM/APM
	{
		buf->Parameters.uFlags = CSMI_SAS_STP_UNSPECIFIED;
	}
	else
	{
		buf->Parameters.uFlags = CSMI_SAS_STP_PIO | CSMI_SAS_STP_READ;
	}
	buf->Parameters.uDataLength = dataSize;

	buf->Parameters.bCommandFIS[ 0] = 0x27; // Type: host-to-device FIS
	buf->Parameters.bCommandFIS[ 1] = 0x80; // Bit7: Update command register

	if(main == SMART_CMD)
	{
		buf->Parameters.bCommandFIS[ 2] = main;
		buf->Parameters.bCommandFIS[ 3] = sub;
		buf->Parameters.bCommandFIS[ 4] = 0;
		buf->Parameters.bCommandFIS[ 5] = SMART_CYL_LOW;
		buf->Parameters.bCommandFIS[ 6] = SMART_CYL_HI;
		buf->Parameters.bCommandFIS[ 7] = 0xA0; // target
		buf->Parameters.bCommandFIS[ 8] = 0;
		buf->Parameters.bCommandFIS[ 9] = 0;
		buf->Parameters.bCommandFIS[10] = 0;
		buf->Parameters.bCommandFIS[11] = 0;
		buf->Parameters.bCommandFIS[12] = param;
		buf->Parameters.bCommandFIS[13] = 0;
	}
	else
	{
		buf->Parameters.bCommandFIS[ 2] = main;
		buf->Parameters.bCommandFIS[ 3] = sub;
		buf->Parameters.bCommandFIS[ 4] = 0;
		buf->Parameters.bCommandFIS[ 5] = 0;
		buf->Parameters.bCommandFIS[ 6] = 0;
		buf->Parameters.bCommandFIS[ 7] = 0xA0; // target
		buf->Parameters.bCommandFIS[ 8] = 0;
		buf->Parameters.bCommandFIS[ 9] = 0;
		buf->Parameters.bCommandFIS[10] = 0;
		buf->Parameters.bCommandFIS[11] = 0;
		buf->Parameters.bCommandFIS[12] = param;
		buf->Parameters.bCommandFIS[13] = 0;
	}

	if(! CsmiIoctl(hIoCtrl, CC_CSMI_SAS_STP_PASSTHRU, &buf->IoctlHeader, size))
	{
		CloseHandle(hIoCtrl);
		VirtualFree(buf, 0, MEM_RELEASE);
		return FALSE;
	}

	if(main != 0xEF && buf->bDataBuffer && data != NULL)
	{
		memcpy_s(data, dataSize, buf->bDataBuffer, dataSize);
	}
	
	CloseHandle(hIoCtrl);
	VirtualFree(buf, 0, MEM_RELEASE);
	
	return	TRUE;
}

static BOOL CsmiIoctl(HANDLE hHandle, UINT code, SRB_IO_CONTROL *csmiBuf, UINT csmiBufSize)
{
	// Determine signature
	const CHAR *sig;
	switch (code)
	{
		case CC_CSMI_SAS_GET_DRIVER_INFO:
			sig = CSMI_ALL_SIGNATURE;
			break;
		case CC_CSMI_SAS_GET_PHY_INFO:
		case CC_CSMI_SAS_STP_PASSTHRU:
			sig = CSMI_SAS_SIGNATURE;
			break;
		case CC_CSMI_SAS_GET_RAID_INFO:
		case CC_CSMI_SAS_GET_RAID_CONFIG:
			sig = CSMI_RAID_SIGNATURE;
			break;
		default:
			return FALSE;
	}

	// Set header
	csmiBuf->HeaderLength = sizeof(IOCTL_HEADER);
	strncpy_s((char *)csmiBuf->Signature, sizeof(csmiBuf->Signature), sig, sizeof(csmiBuf->Signature));
	csmiBuf->Timeout = CSMI_SAS_TIMEOUT;
	csmiBuf->ControlCode = code;
	csmiBuf->ReturnCode = 0;
	csmiBuf->Length = csmiBufSize - sizeof(IOCTL_HEADER);

	// Call function
	DWORD num_out = 0;
	if (!DeviceIoControl(hHandle, IOCTL_SCSI_MINIPORT, 
		csmiBuf, csmiBufSize, csmiBuf, csmiBufSize, &num_out, (OVERLAPPED*)0))
	{
		long err = GetLastError();
		
		if (err == ERROR_INVALID_FUNCTION || err == ERROR_NOT_SUPPORTED 
			|| err == ERROR_DEV_NOT_EXIST)
		{
			return FALSE;
		}
	}

	// Check result
	return TRUE;
}

static BOOL IssueCommandToNonRemovable(INT physicalDriveId, BYTE target, BYTE main, BYTE sub, BYTE param, PBYTE data, DWORD dataSize)
{
	BOOL	bRet;
	HANDLE	hIoCtrl;
	DWORD	dwReturned;

	OSVERSIONINFOEX m_Os;
	BOOL bosVersionInfoEx;
	BOOL m_FlagAtaPassThrough;
	BOOL m_FlagAtaPassThroughSmart;

	ZeroMemory(&m_Os, sizeof(OSVERSIONINFOEX));
	m_Os.dwOSVersionInfoSize = sizeof(OSVERSIONINFOEX);
	if(!(bosVersionInfoEx = GetVersionEx((OSVERSIONINFO *)&m_Os)))
	{
		m_Os.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
		GetVersionEx((OSVERSIONINFO *)&m_Os);
	}

	m_FlagAtaPassThrough = FALSE;
	m_FlagAtaPassThroughSmart = FALSE;
	if(m_Os.dwMajorVersion >= 6 || (m_Os.dwMajorVersion == 5 && m_Os.dwMinorVersion == 2))
	{
		m_FlagAtaPassThrough = TRUE;
		m_FlagAtaPassThroughSmart = TRUE;
	}
	else if(m_Os.dwMajorVersion == 5 && m_Os.dwMinorVersion == 1)
	{
		CString cstr;
		cstr = m_Os.szCSDVersion;
		cstr.Replace(_T("Service Pack "), _T(""));
		if(_tstoi(cstr) >= 2)
		{
			m_FlagAtaPassThrough = TRUE;
			m_FlagAtaPassThroughSmart = TRUE;
		}
	}

	hIoCtrl = GetIoCtrlHandle(physicalDriveId);
	if(hIoCtrl == INVALID_HANDLE_VALUE)
	{
		return	FALSE;
	}

	if(m_FlagAtaPassThrough)
	{
		ATA_PASS_THROUGH_EX_WITH_BUFFERS ab;
		::ZeroMemory(&ab, sizeof(ab));
		ab.Apt.Length = sizeof(ATA_PASS_THROUGH_EX);
		ab.Apt.TimeOutValue = 2;
		DWORD size = offsetof(ATA_PASS_THROUGH_EX_WITH_BUFFERS, Buf);
		ab.Apt.DataBufferOffset = size;

		if(dataSize > 0)
		{
			if(dataSize > sizeof(ab.Buf))
			{
				return FALSE;
			}
			ab.Apt.AtaFlags = ATA_FLAGS_DATA_IN;
			ab.Apt.DataTransferLength = dataSize;
			ab.Buf[0] = 0xCF; // magic number
			size += dataSize;
		}

		ab.Apt.CurrentTaskFile.bFeaturesReg = sub;
		ab.Apt.CurrentTaskFile.bSectorCountReg = param;
		ab.Apt.CurrentTaskFile.bDriveHeadReg = target;
		ab.Apt.CurrentTaskFile.bCommandReg = main;

		if(main == SMART_CMD)
		{
			ab.Apt.CurrentTaskFile.bCylLowReg		= SMART_CYL_LOW;
			ab.Apt.CurrentTaskFile.bCylHighReg		= SMART_CYL_HI;
			ab.Apt.CurrentTaskFile.bSectorCountReg  = 1;
			ab.Apt.CurrentTaskFile.bSectorNumberReg = 1;
		}

		bRet = ::DeviceIoControl(hIoCtrl, IOCTL_ATA_PASS_THROUGH,
			&ab, size, &ab, size, &dwReturned, NULL);
		::CloseHandle(hIoCtrl);
		if(bRet && dataSize && data != NULL)
		{
			memcpy_s(data, dataSize, ab.Buf, dataSize);
		}
	}
	else if(m_Os.dwMajorVersion <= 4)
	{
		return FALSE;
	}
	else
	{
		DWORD size = sizeof(CMD_IDE_PATH_THROUGH) - 1 + dataSize;
		CMD_IDE_PATH_THROUGH* buf = (CMD_IDE_PATH_THROUGH*)VirtualAlloc(NULL, size, MEM_COMMIT, PAGE_READWRITE);

		buf->reg.bFeaturesReg		= sub;
		buf->reg.bSectorCountReg	= param;
		buf->reg.bSectorNumberReg	= 0;
		buf->reg.bCylLowReg			= 0;
		buf->reg.bCylHighReg		= 0;
		buf->reg.bDriveHeadReg		= target;
		buf->reg.bCommandReg		= main;
		buf->reg.bReserved		    = 0;
		buf->length					= dataSize;

		bRet = ::DeviceIoControl(hIoCtrl, IOCTL_IDE_PASS_THROUGH,
			buf, size, buf, size, &dwReturned, NULL);
		::CloseHandle(hIoCtrl);
		if(bRet && dataSize && data != NULL)
		{
			memcpy_s(data, dataSize, buf->buffer, dataSize);
		}
		VirtualFree(buf, 0, MEM_RELEASE);
	}

	return	bRet;
}


/*
 * SMART Section
 */

/*---------------------------------------------------------------------------*/
// USB
/*---------------------------------------------------------------------------*/
// Attempts to get the SMART *thresholds* from "USB" disks. If you get the bridge chip type (i.e. Sunplus, IO Data, etc.) you
// can specify it. Otherwise just send CMD_TYPE_SAT.
static SMART_QUERY_RESULT GetUsbSmartAttributes(INT PhysicalDriveId, BYTE target, COMMAND_TYPE cmdType, SMART_DATA_UCHAR* smart)
{
	BOOL	bRet;
	HANDLE	hIoCtrl;
	DWORD	dwReturned;
	DWORD length;

	SMART_QUERY_RESULT result;

	SCSI_PASS_THROUGH_WITH_BUFFERS sptwb;

	hIoCtrl = GetIoCtrlHandle(PhysicalDriveId);
	if(hIoCtrl == INVALID_HANDLE_VALUE)
	{
		result = SMART_QUERY_INVALID_HANDLE;
		return result;
	}

	::ZeroMemory(&sptwb,sizeof(SCSI_PASS_THROUGH_WITH_BUFFERS));

    sptwb.Spt.Length = sizeof(SCSI_PASS_THROUGH);
    sptwb.Spt.PathId = 0;
    sptwb.Spt.TargetId = 0;
    sptwb.Spt.Lun = 0;
    sptwb.Spt.SenseInfoLength = 24;
    sptwb.Spt.DataIn = SCSI_IOCTL_DATA_IN;
    sptwb.Spt.DataTransferLength = READ_ATTRIBUTE_BUFFER_SIZE;
    sptwb.Spt.TimeOutValue = 2;
    sptwb.Spt.DataBufferOffset = offsetof(SCSI_PASS_THROUGH_WITH_BUFFERS, DataBuf);
    sptwb.Spt.SenseInfoOffset = offsetof(SCSI_PASS_THROUGH_WITH_BUFFERS, SenseBuf);

	if(cmdType == CMD_TYPE_SAT)
	{
		sptwb.Spt.CdbLength = 12;
		sptwb.Spt.Cdb[0] = 0xA1;//ATA PASS THROUGH(12) OPERATION CODE(A1h)
		sptwb.Spt.Cdb[1] = (4 << 1) | 0; //MULTIPLE_COUNT=0,PROTOCOL=4(PIO Data-In),Reserved
		sptwb.Spt.Cdb[2] = (1 << 3) | (1 << 2) | 2;//OFF_LINE=0,CK_COND=0,Reserved=0,T_DIR=1(ToDevice),BYTE_BLOCK=1,T_LENGTH=2
		sptwb.Spt.Cdb[3] = READ_ATTRIBUTES;//FEATURES (7:0)
		sptwb.Spt.Cdb[4] = 1;//SECTOR_COUNT (7:0)
		sptwb.Spt.Cdb[5] = 1;//LBA_LOW (7:0)
		sptwb.Spt.Cdb[6] = SMART_CYL_LOW;//LBA_MID (7:0)
		sptwb.Spt.Cdb[7] = SMART_CYL_HI;//LBA_HIGH (7:0)
		sptwb.Spt.Cdb[8] = target;
		sptwb.Spt.Cdb[9] = SMART_CMD;//COMMAND
	}
	else if(cmdType == CMD_TYPE_SUNPLUS)
	{
		sptwb.Spt.CdbLength = 12;
		sptwb.Spt.Cdb[0] = 0xF8;
		sptwb.Spt.Cdb[1] = 0x00;
		sptwb.Spt.Cdb[2] = 0x22;
		sptwb.Spt.Cdb[3] = 0x10;
		sptwb.Spt.Cdb[4] = 0x01;
		sptwb.Spt.Cdb[5] = 0xD0;  // READ_ATTRIBUTES
		sptwb.Spt.Cdb[6] = 0x01; 
		sptwb.Spt.Cdb[7] = 0x00; 
		sptwb.Spt.Cdb[8] = 0x4F;  // SMART_CYL_LOW 
		sptwb.Spt.Cdb[9] = 0xC2;  // SMART_CYL_HIGH
		sptwb.Spt.Cdb[10] = target; 
		sptwb.Spt.Cdb[11] = 0xB0; // SMART_CMD
	}
	else if(cmdType == CMD_TYPE_IO_DATA)
	{
		sptwb.Spt.CdbLength = 12;
		sptwb.Spt.Cdb[0] = 0xE3;
		sptwb.Spt.Cdb[1] = 0x00; // ?
		sptwb.Spt.Cdb[2] = 0xD0; // READ_ATTRIBUTES
		sptwb.Spt.Cdb[3] = 0x00; // ?
		sptwb.Spt.Cdb[4] = 0x00; // ?
		sptwb.Spt.Cdb[5] = 0x4F; // SMART_CYL_LOW
		sptwb.Spt.Cdb[6] = 0xC2; // SMART_CYL_HIGH
		sptwb.Spt.Cdb[7] = target; // 
		sptwb.Spt.Cdb[8] = 0xB0; // SMART_CMD
		sptwb.Spt.Cdb[9] = 0x00;  
		sptwb.Spt.Cdb[10] = 0x00; 
		sptwb.Spt.Cdb[11] = 0x00;
	}
	else if(cmdType == CMD_TYPE_LOGITEC)
	{
		sptwb.Spt.CdbLength = 10;
		sptwb.Spt.Cdb[0] = 0xE0;
		sptwb.Spt.Cdb[1] = 0x00;
		sptwb.Spt.Cdb[2] = 0xD0; // READ_ATTRIBUTES
		sptwb.Spt.Cdb[3] = 0x00; 
		sptwb.Spt.Cdb[4] = 0x00;
		sptwb.Spt.Cdb[5] = 0x4F; // SMART_CYL_LOW
		sptwb.Spt.Cdb[6] = 0xC2; // SMART_CYL_HIGH
		sptwb.Spt.Cdb[7] = target; 
		sptwb.Spt.Cdb[8] = 0xB0; // SMART_CMD
		sptwb.Spt.Cdb[9] = 0x4C;
	}
	else if(cmdType == CMD_TYPE_JMICRON)
	{
		sptwb.Spt.CdbLength = 12;
		sptwb.Spt.Cdb[0] = 0xDF;
		sptwb.Spt.Cdb[1] = 0x10;
		sptwb.Spt.Cdb[2] = 0x00;
		sptwb.Spt.Cdb[3] = 0x02;
		sptwb.Spt.Cdb[4] = 0x00;
		sptwb.Spt.Cdb[5] = 0xD0;  // READ_ATTRIBUTES
		sptwb.Spt.Cdb[6] = 0x01; 
		sptwb.Spt.Cdb[7] = 0x01; 
		sptwb.Spt.Cdb[8] = 0x4F;  // SMART_CYL_LOW
		sptwb.Spt.Cdb[9] = 0xC2;  // SMART_CYL_HIGH
		sptwb.Spt.Cdb[10] = target; 
		sptwb.Spt.Cdb[11] = 0xB0; // SMART_CMD
	}
	else if(cmdType == CMD_TYPE_CYPRESS)
	{
		sptwb.Spt.CdbLength = 16;
		sptwb.Spt.Cdb[0] = 0x24;
		sptwb.Spt.Cdb[1] = 0x24;
		sptwb.Spt.Cdb[2] = 0x00;
		sptwb.Spt.Cdb[3] = 0xBE;
		sptwb.Spt.Cdb[4] = 0x01;
		sptwb.Spt.Cdb[5] = 0x00; 
		sptwb.Spt.Cdb[6] = 0xD0;  // READ_ATTRIBUTES
		sptwb.Spt.Cdb[7] = 0x00; 
		sptwb.Spt.Cdb[8] = 0x00;
		sptwb.Spt.Cdb[9] = 0x4F;  // SMART_CYL_LOW
		sptwb.Spt.Cdb[10] = 0xC2; // SMART_CYL_HIGH
		sptwb.Spt.Cdb[11] = target;
		sptwb.Spt.Cdb[12] = 0xB0; // ID_CMD
		sptwb.Spt.Cdb[13] = 0x00;
		sptwb.Spt.Cdb[14] = 0x00;
		sptwb.Spt.Cdb[15] = 0x00;
	}
	else
	{
		result = SMART_QUERY_EXCEPTION;
		return result;
	}

	length = offsetof(SCSI_PASS_THROUGH_WITH_BUFFERS, DataBuf) + sptwb.Spt.DataTransferLength;
	bRet = ::DeviceIoControl(hIoCtrl, IOCTL_SCSI_PASS_THROUGH, 
		&sptwb, sizeof(SCSI_PASS_THROUGH),
		&sptwb, length,	&dwReturned, NULL);

	::CloseHandle(hIoCtrl);
	
	if(bRet == FALSE || dwReturned != (length))
	{
		if( bRet == FALSE )
		{
			result = SMART_QUERY_IO_CONTROL_FALSE;
		}
		else
		{
			result = SMART_QUERY_OUTDATA_SIZE_MISMATCH;
		}
		return result;
	}
		
	for( int i = 0; i < 512; i++ )
	{
		smart->Data[i] = sptwb.DataBuf[i];
	}
		
	bRet = true;
	result = SMART_QUERY_RESULT_OK;
	return result;
}

// Attempts to get the SMART *thresholds* from "USB" disks. If you get the bridge chip type (i.e. Sunplus, IO Data, etc.) you
// can specify it. Otherwise just send CMD_TYPE_SAT.
static SMART_QUERY_RESULT GetUsbSmartThresholds(INT physicalDriveId, BYTE target, COMMAND_TYPE cmdType, SMART_DATA_UCHAR* smart)
{
	BOOL	bRet;
	HANDLE	hIoCtrl;
	DWORD	dwReturned;
	DWORD length;

	SMART_QUERY_RESULT result;

	SCSI_PASS_THROUGH_WITH_BUFFERS sptwb;

	hIoCtrl = GetIoCtrlHandle(physicalDriveId);
	if(hIoCtrl == INVALID_HANDLE_VALUE)
	{
		result = SMART_QUERY_INVALID_HANDLE;
		return result;
	}

	::ZeroMemory(&sptwb,sizeof(SCSI_PASS_THROUGH_WITH_BUFFERS));

    sptwb.Spt.Length = sizeof(SCSI_PASS_THROUGH);
    sptwb.Spt.PathId = 0;
    sptwb.Spt.TargetId = 0;
    sptwb.Spt.Lun = 0;
    sptwb.Spt.SenseInfoLength = 24;
    sptwb.Spt.DataIn = SCSI_IOCTL_DATA_IN;
    sptwb.Spt.DataTransferLength = READ_THRESHOLD_BUFFER_SIZE;
    sptwb.Spt.TimeOutValue = 2;
    sptwb.Spt.DataBufferOffset = offsetof(SCSI_PASS_THROUGH_WITH_BUFFERS, DataBuf);
    sptwb.Spt.SenseInfoOffset = offsetof(SCSI_PASS_THROUGH_WITH_BUFFERS, SenseBuf);

	if(cmdType == CMD_TYPE_SAT)
	{
		sptwb.Spt.CdbLength = 12;
		sptwb.Spt.Cdb[0] = 0xA1; ////ATA PASS THROUGH(12) OPERATION CODE (A1h)
		sptwb.Spt.Cdb[1] = (4 << 1) | 0; //MULTIPLE_COUNT=0,PROTOCOL=4(PIO Data-In),Reserved
		sptwb.Spt.Cdb[2] = (1 << 3) | (1 << 2) | 2;//OFF_LINE=0,CK_COND=0,Reserved=0,T_DIR=1(ToDevice),BYTE_BLOCK=1,T_LENGTH=2
		sptwb.Spt.Cdb[3] = READ_THRESHOLDS;//FEATURES (7:0)
		sptwb.Spt.Cdb[4] = 1;//SECTOR_COUNT (7:0)
		sptwb.Spt.Cdb[5] = 1;//LBA_LOW (7:0)
		sptwb.Spt.Cdb[6] = SMART_CYL_LOW;//LBA_MID (7:0)
		sptwb.Spt.Cdb[7] = SMART_CYL_HI;//LBA_HIGH (7:0)
		sptwb.Spt.Cdb[8] = target;
		sptwb.Spt.Cdb[9] = SMART_CMD;//COMMAND
	}
	else if(cmdType == CMD_TYPE_SUNPLUS)
	{
		sptwb.Spt.CdbLength = 12;
		sptwb.Spt.Cdb[0] = 0xF8;
		sptwb.Spt.Cdb[1] = 0x00;
		sptwb.Spt.Cdb[2] = 0x22;
		sptwb.Spt.Cdb[3] = 0x10;
		sptwb.Spt.Cdb[4] = 0x01;
		sptwb.Spt.Cdb[5] = 0xD1; // READ_THRESHOLD
		sptwb.Spt.Cdb[6] = 0x01; 
		sptwb.Spt.Cdb[7] = 0x01; 
		sptwb.Spt.Cdb[8] = 0x4F; // SMART_CYL_LOW
		sptwb.Spt.Cdb[9] = 0xC2; // SMART_CYL_HIGH
		sptwb.Spt.Cdb[10] = target; 
		sptwb.Spt.Cdb[11] = 0xB0;// SMART_CMD
	}
	else if(cmdType == CMD_TYPE_IO_DATA)
	{
		sptwb.Spt.CdbLength = 12;
		sptwb.Spt.Cdb[0] = 0xE3;
		sptwb.Spt.Cdb[1] = 0x00; // ?
		sptwb.Spt.Cdb[2] = 0xD1; // READ_THRESHOLD
		sptwb.Spt.Cdb[3] = 0x00; // ?
		sptwb.Spt.Cdb[4] = 0x00; // ?
		sptwb.Spt.Cdb[5] = 0x4F; // SMART_CYL_LOW 
		sptwb.Spt.Cdb[6] = 0xC2; // SMART_CYL_HIGH
		sptwb.Spt.Cdb[7] = target; // 
		sptwb.Spt.Cdb[8] = 0xB0; // SMART_CMD
		sptwb.Spt.Cdb[9] = 0x00;  
		sptwb.Spt.Cdb[10] = 0x00; 
		sptwb.Spt.Cdb[11] = 0x00;
	}
	else if(cmdType == CMD_TYPE_LOGITEC)
	{
		sptwb.Spt.CdbLength = 10;
		sptwb.Spt.Cdb[0] = 0xE0;
		sptwb.Spt.Cdb[1] = 0x00;
		sptwb.Spt.Cdb[2] = 0xD1; // READ_THRESHOLD
		sptwb.Spt.Cdb[3] = 0x00;
		sptwb.Spt.Cdb[4] = 0x00;
		sptwb.Spt.Cdb[5] = 0x4F; // SMART_CYL_LOW
		sptwb.Spt.Cdb[6] = 0xC2; // SMART_CYL_HIGH
		sptwb.Spt.Cdb[7] = target; 
		sptwb.Spt.Cdb[8] = 0xB0; // SMART_CMD
		sptwb.Spt.Cdb[9] = 0x4C;
	}
	else if(cmdType == CMD_TYPE_JMICRON)
	{
		sptwb.Spt.CdbLength = 12;
		sptwb.Spt.Cdb[0] = 0xDF;
		sptwb.Spt.Cdb[1] = 0x10;
		sptwb.Spt.Cdb[2] = 0x00;
		sptwb.Spt.Cdb[3] = 0x02;
		sptwb.Spt.Cdb[4] = 0x00;
		sptwb.Spt.Cdb[5] = 0xD1;  // READ_THRESHOLD
		sptwb.Spt.Cdb[6] = 0x01; 
		sptwb.Spt.Cdb[7] = 0x01; 
		sptwb.Spt.Cdb[8] = 0x4F;  // SMART_CYL_LOW
		sptwb.Spt.Cdb[9] = 0xC2;  // SMART_CYL_HIGH
		sptwb.Spt.Cdb[10] = target; 
		sptwb.Spt.Cdb[11] = 0xB0; // SMART_CMD
	}
	else if(cmdType == CMD_TYPE_CYPRESS)
	{
		sptwb.Spt.CdbLength = 16;
		sptwb.Spt.Cdb[0] = 0x24;
		sptwb.Spt.Cdb[1] = 0x24;
		sptwb.Spt.Cdb[2] = 0x00;
		sptwb.Spt.Cdb[3] = 0xBE;
		sptwb.Spt.Cdb[4] = 0x01;
		sptwb.Spt.Cdb[5] = 0x00; 
		sptwb.Spt.Cdb[6] = 0xD1;  // READ_THRESHOLD  
		sptwb.Spt.Cdb[7] = 0x00; 
		sptwb.Spt.Cdb[8] = 0x00;
		sptwb.Spt.Cdb[9] = 0x4F;  // SMART_CYL_LOW
		sptwb.Spt.Cdb[10] = 0xC2; // SMART_CYL_HIGH
		sptwb.Spt.Cdb[11] = target;
		sptwb.Spt.Cdb[12] = 0xB0; // ID_CMD
		sptwb.Spt.Cdb[13] = 0x00;
		sptwb.Spt.Cdb[14] = 0x00;
		sptwb.Spt.Cdb[15] = 0x00;
	}
	else
	{
		result = SMART_QUERY_EXCEPTION;
		return result;
	}

	length = offsetof(SCSI_PASS_THROUGH_WITH_BUFFERS, DataBuf) + sptwb.Spt.DataTransferLength;
	bRet = ::DeviceIoControl(hIoCtrl, IOCTL_SCSI_PASS_THROUGH, 
		&sptwb, sizeof(SCSI_PASS_THROUGH),
		&sptwb, length,	&dwReturned, NULL);
	
	::CloseHandle(hIoCtrl);

	if(bRet == FALSE || dwReturned != (length))
	{
		if( bRet == FALSE )
		{
			result = SMART_QUERY_IO_CONTROL_FALSE;
		}
		else
		{
			result = SMART_QUERY_OUTDATA_SIZE_MISMATCH;
		}
		return result;
	}
		
	for( int i = 0; i < 512; i++ )
	{
		smart->Data[i] = sptwb.DataBuf[i];
	}
		
	bRet = true;
	result = SMART_QUERY_RESULT_OK;
	return result;
}



/*---------------------------------------------------------------------------*/
// Internal SATA/IDE
/*---------------------------------------------------------------------------*/

// Attempts to get the SMART *attributes* from "internal" disks (SATA, IDE, eSATA).
static SMART_QUERY_RESULT GetInternalSmartAttributes(INT physicalDriveId, BYTE target, SMART_DATA* smart)
{
	BOOL	bRet = FALSE;
	HANDLE	hIoCtrl;
	DWORD	dwReturned;

	SMART_QUERY_RESULT result;

	SMART_READ_DATA_OUTDATA	sendCmdOutParam;
	SENDCMDINPARAMS	sendCmd;

	hIoCtrl = GetIoCtrlHandle(physicalDriveId);
	if(hIoCtrl == INVALID_HANDLE_VALUE)
	{
		result = SMART_QUERY_INVALID_HANDLE;
		return result;
	}

	::ZeroMemory(&sendCmdOutParam, sizeof(SMART_READ_DATA_OUTDATA));
	::ZeroMemory(&sendCmd, sizeof(SENDCMDINPARAMS));

	sendCmd.irDriveRegs.bFeaturesReg	= READ_ATTRIBUTES;
	sendCmd.irDriveRegs.bSectorCountReg = 1;
	sendCmd.irDriveRegs.bSectorNumberReg= 1;
	sendCmd.irDriveRegs.bCylLowReg		= SMART_CYL_LOW;
	sendCmd.irDriveRegs.bCylHighReg		= SMART_CYL_HI;
	sendCmd.irDriveRegs.bDriveHeadReg	= target;
	sendCmd.irDriveRegs.bCommandReg		= SMART_CMD;
	sendCmd.cBufferSize					= READ_ATTRIBUTE_BUFFER_SIZE;

	bRet = ::DeviceIoControl(hIoCtrl, DFP_RECEIVE_DRIVE_DATA, 
		&sendCmd, sizeof(SENDCMDINPARAMS),
		&sendCmdOutParam, sizeof(SMART_READ_DATA_OUTDATA),
		&dwReturned, NULL);

	::CloseHandle(hIoCtrl);
	
	if(bRet == FALSE || dwReturned != sizeof(SMART_READ_DATA_OUTDATA))
	//if( bRet == FALSE )
	{
		if( bRet == FALSE )
		{
			result = SMART_QUERY_IO_CONTROL_FALSE;
		}
		else
		{
			result = SMART_QUERY_OUTDATA_SIZE_MISMATCH;
		}
		return result;
	}

	// The SMART data doesn't come back quite right -- the first byte is wedged in the bBuffer field so we need to adjust.
	smart->Data[0] = sendCmdOutParam.SendCmdOutParam.bBuffer[0];
		
	for( int i = 0; i < READ_ATTRIBUTE_BUFFER_SIZE - 1; i++ )
	{
		smart->Data[i + 1] = sendCmdOutParam.Data[i];
	}

	bRet = true;
	result = SMART_QUERY_RESULT_OK;
	return result;
}

// Attempts to get the SMART *thresholds* from "internal" disks (SATA, IDE, eSATA).
static SMART_QUERY_RESULT GetInternalSmartThresholds(INT physicalDriveId, BYTE target, SMART_DATA* smart)
{
	BOOL	bRet = FALSE;
	HANDLE	hIoCtrl;
	DWORD	dwReturned;

	SMART_QUERY_RESULT result;

	SMART_READ_DATA_OUTDATA	sendCmdOutParam;
	SENDCMDINPARAMS	sendCmd;

	hIoCtrl = GetIoCtrlHandle(physicalDriveId);
	if(hIoCtrl == INVALID_HANDLE_VALUE)
	{
		result = SMART_QUERY_INVALID_HANDLE;
		return result;
	}

	::ZeroMemory(&sendCmdOutParam, sizeof(SMART_READ_DATA_OUTDATA));
	::ZeroMemory(&sendCmd, sizeof(SENDCMDINPARAMS));

	sendCmd.irDriveRegs.bFeaturesReg	= READ_THRESHOLDS;
	sendCmd.irDriveRegs.bSectorCountReg = 1;
	sendCmd.irDriveRegs.bSectorNumberReg= 1;
	sendCmd.irDriveRegs.bCylLowReg		= SMART_CYL_LOW;
	sendCmd.irDriveRegs.bCylHighReg		= SMART_CYL_HI;
	sendCmd.irDriveRegs.bDriveHeadReg	= target;
	sendCmd.irDriveRegs.bCommandReg		= SMART_CMD;
	sendCmd.cBufferSize					= READ_THRESHOLD_BUFFER_SIZE;

	bRet = ::DeviceIoControl(hIoCtrl, DFP_RECEIVE_DRIVE_DATA, 
		&sendCmd, sizeof(SENDCMDINPARAMS),
		&sendCmdOutParam, sizeof(SMART_READ_DATA_OUTDATA),
		&dwReturned, NULL);

	::CloseHandle(hIoCtrl);
	
	if(bRet == FALSE || dwReturned != sizeof(SMART_READ_DATA_OUTDATA))
	//if( bRet == FALSE )
	{
		if( bRet == FALSE )
		{
			result = SMART_QUERY_IO_CONTROL_FALSE;
		}
		else
		{
			result = SMART_QUERY_OUTDATA_SIZE_MISMATCH;
		}
		return result;
	}

	// The SMART data doesn't come back quite right -- the first byte is wedged in the bBuffer field so we need to adjust.
	smart->Data[0] = sendCmdOutParam.SendCmdOutParam.bBuffer[0];
		
	for( int i = 0; i < READ_ATTRIBUTE_BUFFER_SIZE - 1; i++ )
	{
		smart->Data[i + 1] = sendCmdOutParam.Data[i];
	}
		
	bRet = true;
	result = SMART_QUERY_RESULT_OK;
	return result;
}

/*---------------------------------------------------------------------------*/
// Disks attached to Silicon Image SiI
/*---------------------------------------------------------------------------*/
// Attempts to get the SMART *attributes* from "SiI" disks.
static SMART_QUERY_RESULT GetSiiSmartAttributes(INT physicalDriveId, SMART_DATA_SII* smart)
{
	HANDLE hScsiDriveIOCTL = 0;

	hScsiDriveIOCTL = GetIoCtrlHandle(physicalDriveId);

	STORAGE_PREDICT_FAILURE spf;
	DWORD dwRetBytes;

	if(hScsiDriveIOCTL != INVALID_HANDLE_VALUE)
	{
		if (DeviceIoControl(hScsiDriveIOCTL, IOCTL_STORAGE_PREDICT_FAILURE,
			&spf, sizeof(spf), &spf, sizeof(spf), &dwRetBytes, NULL))
		{
			for( int i = 0; i < 512; i++ )
			{
				smart->Data[i] = spf.VendorSpecific[i];
			}
			smart->FailurePredicted = spf.PredictFailure;
			CloseHandle(hScsiDriveIOCTL);
			return SMART_QUERY_RESULT_OK;
		}
		CloseHandle(hScsiDriveIOCTL);
		return SMART_QUERY_EXCEPTION;
	}
	return SMART_QUERY_INVALID_HANDLE;
}

// Attempts to get the SMART *attributes* from "SiI" disks. This is our second try if the first fails.
static SMART_QUERY_RESULT GetSiiSmartAttributes(INT scsiPort, INT scsiPath, INT scsiTarget, DWORD siliconImageType, SMART_DATA_SII* smart)
{
	HANDLE hScsiDriveIOCTL = 0;

	hScsiDriveIOCTL = GetIoCtrlHandle(scsiPort, scsiPath, scsiTarget, siliconImageType);

	STORAGE_PREDICT_FAILURE spf;
	DWORD dwRetBytes;
	if (DeviceIoControl(hScsiDriveIOCTL, IOCTL_STORAGE_PREDICT_FAILURE,
		&spf, sizeof(spf), &spf, sizeof(spf), &dwRetBytes, NULL))
	{
		for( int i = 0; i < 512; i++ )
		{
			smart->Data[i] = spf.VendorSpecific[i];
		}
		smart->FailurePredicted = spf.PredictFailure;
		return SMART_QUERY_RESULT_OK;
	}
	return SMART_QUERY_EXCEPTION;
}

// Note that Silicon Image SiI controllers don't have thresholds! IOCTL_STORAGE_PREDICT_FAILURE
// gets attributes but not thresholds.  That's why I just use WMI to try getting the thresholds.
//static SMART_QUERY_RESULT GetSmartThresholdSi(INT physicalDriveId, SMART_DATA_SII* smart)
//{
//	
//}

/*---------------------------------------------------------------------------*/
// SCSI
/*---------------------------------------------------------------------------*/
// Attempts to get the SMART *attributes* from "SCSI" disks.
static SMART_QUERY_RESULT GetScsiSmartAttributes(INT scsiPort, INT scsiTargetId, ATA_SMART_INFO* asi, SMART_DATA* smart)
{
	HANDLE hScsiDriveIOCTL = 0;
	CString driveName;
	driveName.Format(_T("\\\\.\\Scsi%d:"), scsiPort);
	hScsiDriveIOCTL = CreateFile(driveName, GENERIC_READ | GENERIC_WRITE,
							FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);
	if(hScsiDriveIOCTL != INVALID_HANDLE_VALUE)
	{
		BYTE buffer[sizeof(SRB_IO_CONTROL) + sizeof(SENDCMDOUTPARAMS) + READ_ATTRIBUTE_BUFFER_SIZE];
		SRB_IO_CONTROL *p = (SRB_IO_CONTROL *)buffer;
		SENDCMDINPARAMS *pin = (SENDCMDINPARAMS *)(buffer + sizeof(SRB_IO_CONTROL));
		DWORD dummy;
		memset(buffer, 0, sizeof(buffer));
		p->HeaderLength = sizeof(SRB_IO_CONTROL);
		p->Timeout = 2;
		p->Length = sizeof(SENDCMDOUTPARAMS) + READ_ATTRIBUTE_BUFFER_SIZE;
		p->ControlCode = IOCTL_SCSI_MINIPORT_READ_SMART_ATTRIBS;
		memcpy((char *)p->Signature, "SCSIDISK", 8);
		pin->irDriveRegs.bFeaturesReg		= READ_ATTRIBUTES;
		pin->irDriveRegs.bSectorCountReg	= 1;
		pin->irDriveRegs.bSectorNumberReg	= 1;
		pin->irDriveRegs.bCylLowReg			= SMART_CYL_LOW;
		pin->irDriveRegs.bCylHighReg		= SMART_CYL_HI;
		pin->irDriveRegs.bCommandReg		= SMART_CMD;
		pin->cBufferSize					= READ_ATTRIBUTE_BUFFER_SIZE;
		pin->bDriveNumber					= scsiTargetId;

		if(DeviceIoControl(hScsiDriveIOCTL, IOCTL_SCSI_MINIPORT, 
								buffer, sizeof(SRB_IO_CONTROL) + sizeof(SENDCMDINPARAMS) - 1,
								buffer, sizeof(SRB_IO_CONTROL) + sizeof(SENDCMDOUTPARAMS) + READ_ATTRIBUTE_BUFFER_SIZE,
								&dummy, NULL))
		{
			SENDCMDOUTPARAMS *pOut = (SENDCMDOUTPARAMS *)(buffer + sizeof(SRB_IO_CONTROL));

			memcpy_s(&(asi->SmartReadData), 512, &(pOut->bBuffer), 512);
			CloseHandle(hScsiDriveIOCTL);

			for( int i = 0; i < 512; i++ )
			{
				smart->Data[i] = asi->SmartReadData[i];
			}
		}
		else
		{
			return SMART_QUERY_EXCEPTION;
		}
	}
	return SMART_QUERY_INVALID_HANDLE;
}

// Attempts to get the SMART *thresholds* from "SCSI" disks.
static SMART_QUERY_RESULT GetScsiSmartThresholds(INT scsiPort, INT scsiTargetId, ATA_SMART_INFO* asi, SMART_DATA* smart)
{
	HANDLE hScsiDriveIOCTL = 0;
	CString driveName;
	driveName.Format(_T("\\\\.\\Scsi%d:"), scsiPort);
	hScsiDriveIOCTL = CreateFile(driveName, GENERIC_READ | GENERIC_WRITE,
							FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);
	if(hScsiDriveIOCTL != INVALID_HANDLE_VALUE)
	{
		BYTE buffer[sizeof(SRB_IO_CONTROL) + sizeof(SENDCMDOUTPARAMS) + READ_THRESHOLD_BUFFER_SIZE];
		SRB_IO_CONTROL *p = (SRB_IO_CONTROL *)buffer;
		SENDCMDINPARAMS *pin = (SENDCMDINPARAMS *)(buffer + sizeof(SRB_IO_CONTROL));
		DWORD dummy;
		memset(buffer, 0, sizeof(buffer));
		p->HeaderLength = sizeof(SRB_IO_CONTROL);
		p->Timeout = 2;
		p->Length = sizeof(SENDCMDOUTPARAMS) + READ_THRESHOLD_BUFFER_SIZE;
		p->ControlCode = IOCTL_SCSI_MINIPORT_READ_SMART_THRESHOLDS;
		memcpy((char *)p->Signature, "SCSIDISK", 8);
		pin->irDriveRegs.bFeaturesReg		= READ_THRESHOLDS;
		pin->irDriveRegs.bSectorCountReg	= 1;
		pin->irDriveRegs.bSectorNumberReg	= 1;
		pin->irDriveRegs.bCylLowReg			= SMART_CYL_LOW;
		pin->irDriveRegs.bCylHighReg		= SMART_CYL_HI;
		pin->irDriveRegs.bCommandReg		= SMART_CMD;
		pin->cBufferSize					= READ_THRESHOLD_BUFFER_SIZE;
		pin->bDriveNumber					= scsiTargetId;

		if(DeviceIoControl(hScsiDriveIOCTL, IOCTL_SCSI_MINIPORT, 
								buffer, sizeof(SRB_IO_CONTROL) + sizeof(SENDCMDINPARAMS) - 1,
								buffer, sizeof(SRB_IO_CONTROL) + sizeof(SENDCMDOUTPARAMS) + READ_THRESHOLD_BUFFER_SIZE,
								&dummy, NULL))
		{
			SENDCMDOUTPARAMS *pOut = (SENDCMDOUTPARAMS *)(buffer + sizeof(SRB_IO_CONTROL));
			if(*(pOut->bBuffer) > 0)
			{
				int j = 0;
				memcpy_s(&(asi->SmartReadThreshold), 512, &(pOut->bBuffer), 512);
				for(int i = 0; i < MAX_ATTRIBUTE; i++)
				{
					memcpy(	&(asi->Threshold[j]), 
						&(pOut->bBuffer[i * sizeof(SMART_THRESHOLD) + 2]), sizeof(SMART_THRESHOLD));
					if(asi->Threshold[j].Id != 0)
					{
						j++;
					}
				}

				for( int i = 0; i < 512; i++ )
				{
					smart->Data[i] = asi->SmartReadThreshold[i];
				}
			}
		}
	}
	CloseHandle (hScsiDriveIOCTL);
	
	if(asi->AttributeCount > 0)
	{
		return SMART_QUERY_RESULT_OK;
	}
	else
	{
		return SMART_QUERY_INVALID_HANDLE;
	}
}

/*---------------------------------------------------------------------------*/
// SMART Self Tests
/*---------------------------------------------------------------------------*/

static DWORD SelfTestInternalDrive(INT physicalDriveId, BYTE target, INT testId)
{
	/*
	 * Test IDs to use:
	 *	1 = short self-test
	 *  2 = extended self-test
	 *  3 = conveyance self test
	 *  127 = abort test
	 */
	BOOL	bRet = FALSE;
	HANDLE	hIoCtrl;
	DWORD	dwReturned;
	DWORD WINAPI errorInfo;

	SMART_QUERY_RESULT result;

	SENDCMDINPARAMS	sendCmdOutParam;
	SENDCMDINPARAMS	sendCmd;
	
	hIoCtrl = GetIoCtrlHandle(physicalDriveId);
	if(hIoCtrl == INVALID_HANDLE_VALUE)
	{
		result = SMART_QUERY_INVALID_HANDLE;
		return 0x2;
	}

	::ZeroMemory(&sendCmdOutParam, sizeof(SENDCMDINPARAMS));
	::ZeroMemory(&sendCmd, sizeof(SENDCMDINPARAMS));

	sendCmd.irDriveRegs.bFeaturesReg	= EXECUTE_OFFLINE_DIAGS;
	sendCmd.irDriveRegs.bSectorCountReg = 1;
	sendCmd.irDriveRegs.bSectorNumberReg= testId; // LBA Low
	sendCmd.irDriveRegs.bCylLowReg		= SMART_CYL_LOW; // LBA Mid
	sendCmd.irDriveRegs.bCylHighReg		= SMART_CYL_HI; // LBA High
	sendCmd.irDriveRegs.bDriveHeadReg	= target;
	sendCmd.irDriveRegs.bCommandReg		= SMART_CMD;
	sendCmd.cBufferSize					= READ_ATTRIBUTE_BUFFER_SIZE;

	bRet = ::DeviceIoControl(hIoCtrl, DFP_SEND_DRIVE_COMMAND, 
		&sendCmd, sizeof(SENDCMDINPARAMS),
		&sendCmdOutParam, sizeof(SENDCMDINPARAMS),
		&dwReturned, NULL);

	errorInfo = 0x0;

	if(bRet == FALSE)
	{
		errorInfo = ::GetLastError();
	}

	::CloseHandle(hIoCtrl);
	
	if( bRet == FALSE )
	{
		if(errorInfo == 0x0)
		{
			errorInfo = 0x1;
		}
	}
	return errorInfo;
}


static DWORD SelfTestUsbDrive(INT physicalDriveId, BYTE target, COMMAND_TYPE cmdType, INT testId)
{
	/*
	 * Test IDs to use:
	 *	1 = short self-test
	 *  2 = extended self-test
	 *  3 = conveyance self test
	 *  127 = abort test
	 */
	BOOL	bRet;
	HANDLE	hIoCtrl;
	DWORD	dwReturned;
	DWORD length;
	DWORD WINAPI errorInfo;

	SMART_QUERY_RESULT result;

	SCSI_PASS_THROUGH_WITH_BUFFERS sptwb;

	hIoCtrl = GetIoCtrlHandle(physicalDriveId);
	if(hIoCtrl == INVALID_HANDLE_VALUE)
	{
		result = SMART_QUERY_INVALID_HANDLE;
		return 0x2;
	}

	::ZeroMemory(&sptwb,sizeof(SCSI_PASS_THROUGH_WITH_BUFFERS));

    sptwb.Spt.Length = sizeof(SCSI_PASS_THROUGH);
    sptwb.Spt.PathId = 0;
    sptwb.Spt.TargetId = 0;
    sptwb.Spt.Lun = 0;
    sptwb.Spt.SenseInfoLength = 24;
    sptwb.Spt.DataIn = SCSI_IOCTL_DATA_IN;
    sptwb.Spt.DataTransferLength = 512;//READ_ATTRIBUTE_BUFFER_SIZE;
    sptwb.Spt.TimeOutValue = 2;
    sptwb.Spt.DataBufferOffset = offsetof(SCSI_PASS_THROUGH_WITH_BUFFERS, DataBuf);
    sptwb.Spt.SenseInfoOffset = offsetof(SCSI_PASS_THROUGH_WITH_BUFFERS, SenseBuf);

	if(cmdType == CMD_TYPE_SAT)
	{
		sptwb.Spt.CdbLength = 12;
		sptwb.Spt.Cdb[0] = 0xA1;//ATA PASS THROUGH(12) OPERATION CODE(A1h)
		sptwb.Spt.Cdb[1] = (4 << 1) | 0; //MULTIPLE_COUNT=0,PROTOCOL=4(PIO Data-In),Reserved
		sptwb.Spt.Cdb[2] = (1 << 3) | (1 << 2) | 2;//OFF_LINE=0,CK_COND=0,Reserved=0,T_DIR=1(ToDevice),BYTE_BLOCK=1,T_LENGTH=2
		sptwb.Spt.Cdb[3] = 0xD4;//FEATURES (7:0)
		sptwb.Spt.Cdb[4] = 1;//SECTOR_COUNT (7:0)
		sptwb.Spt.Cdb[5] = testId;//LBA_LOW (7:0)
		sptwb.Spt.Cdb[6] = SMART_CYL_LOW;//LBA_MID (7:0)
		sptwb.Spt.Cdb[7] = SMART_CYL_HI;//LBA_HIGH (7:0)
		sptwb.Spt.Cdb[8] = target;
		sptwb.Spt.Cdb[9] = SMART_CMD;//COMMAND
	}
	else
	{
		result = SMART_QUERY_EXCEPTION;
		return 0x3;
	}

	length = offsetof(SCSI_PASS_THROUGH_WITH_BUFFERS, DataBuf) + sptwb.Spt.DataTransferLength;
	bRet = ::DeviceIoControl(hIoCtrl, IOCTL_SCSI_PASS_THROUGH, 
		&sptwb, sizeof(SCSI_PASS_THROUGH),
		&sptwb, length,	&dwReturned, NULL);

	errorInfo = 0x0;

	if(bRet == FALSE)
	{
		errorInfo = ::GetLastError();
	}

	::CloseHandle(hIoCtrl);
	
	if( bRet == FALSE )
	{
		if(errorInfo == 0x0)
		{
			errorInfo = 0x1;
		}
	}

	return errorInfo;
}

/*
 *  These extern "C" __declspec(dllexport) lines are for exporting the methods for which you would like to create DLL entry points.
 *  This is mandatory for calling into this DLL with P/Invoke.
 */
extern "C" __declspec(dllexport) IDENTIFY_DEVICE_RESULT IdentifyInternalDriveRpm(INT physicalDriveId, BYTE target, IDENTITY_DATA* outData)
{
    return IdentifyInternalDrive(physicalDriveId, target, outData);
}

extern "C" __declspec(dllexport) IDENTIFY_DEVICE_RESULT IdentifyUsbDriveRpm(INT physicalDriveId, BYTE target, IDENTITY_DATA* outData, COMMAND_TYPE type)
{
	return IdentifyUsbDrive(physicalDriveId, target, outData, type);
}

extern "C" __declspec(dllexport) IDENTIFY_DEVICE_RESULT IdentifySiiDriveRpm(INT physicalDriveId, INT scsiPort, INT scsiBus, INT scsiTarget, DWORD siliconImageType, IDENTITY_DATA* outData)
{
	return IdentifySiiDrive(physicalDriveId, scsiPort, scsiBus, scsiTarget, siliconImageType, outData);
}

extern "C" __declspec(dllexport) IDENTIFY_DEVICE_RESULT IdentifyCsmiDriveRpm(INT scsiPort, INT scsiTarget, INT scsiBus, IDENTITY_DATA* outData)
{
	return IdentifyCsmiDrive(scsiPort, scsiTarget, scsiBus, outData);
}

extern "C" __declspec(dllexport) SMART_QUERY_RESULT GetCsmiDriveSmartAttributes(INT scsiPort, INT scsiTarget, INT scsiBus, SMART_DATA* smart)
{
    return GetCsmiSmartAttributes(scsiPort, scsiTarget, scsiBus, smart);
}

extern "C" __declspec(dllexport) SMART_QUERY_RESULT GetCsmiDriveSmartThresholds(INT scsiPort, INT scsiTarget, INT scsiBus, SMART_DATA* smart)
{
    return GetCsmiSmartThresholds(scsiPort, scsiTarget, scsiBus, smart);
}

extern "C" __declspec(dllexport) SMART_QUERY_RESULT GetInternalDriveSmartAttributes(INT physicalDriveId, BYTE target, SMART_DATA* smart)
{
    return GetInternalSmartAttributes(physicalDriveId, target, smart);
}

extern "C" __declspec(dllexport) SMART_QUERY_RESULT GetInternalDriveSmartThresholds(INT physicalDriveId, BYTE target, SMART_DATA* smart)
{
    return GetInternalSmartThresholds(physicalDriveId, target, smart);
}

extern "C" __declspec(dllexport) SMART_QUERY_RESULT GetUsbDriveSmartAttributes(INT physicalDriveId, BYTE target, COMMAND_TYPE cmdType, SMART_DATA_UCHAR* smart)
{
	return GetUsbSmartAttributes(physicalDriveId, target, cmdType, smart);
}

extern "C" __declspec(dllexport) SMART_QUERY_RESULT GetUsbDriveSmartThresholds(INT physicalDriveId, BYTE target, COMMAND_TYPE cmdType, SMART_DATA_UCHAR* smart)
{
	return GetUsbSmartThresholds(physicalDriveId, target, cmdType, smart);
}

extern "C" __declspec(dllexport) SMART_QUERY_RESULT GetSiIDriveSmartAttributes(INT physicalDriveId, SMART_DATA_SII* smart)
{
	return GetSiiSmartAttributes(physicalDriveId, smart);
}

extern "C" __declspec(dllexport) SMART_QUERY_RESULT GetSiIDriveSmartAttributesSecondary(INT scsiPort, INT scsiPath, INT scsiTarget, DWORD siliconImageType, SMART_DATA_SII* smart)
{
	return GetSiiSmartAttributes(scsiPort, scsiPath, scsiTarget, siliconImageType, smart);
}

extern "C" __declspec(dllexport) SMART_QUERY_RESULT GetScsiDriveSmartAttributes(INT scsiPort, INT scsiTargetId, ATA_SMART_INFO* asi, SMART_DATA* smart)
{
	return GetScsiSmartAttributes(scsiPort, scsiTargetId, asi, smart);
}

extern "C" __declspec(dllexport) SMART_QUERY_RESULT GetScsiDriveSmartThresholds(INT scsiPort, INT scsiTargetId, ATA_SMART_INFO* asi, SMART_DATA* smart)
{
	return GetScsiSmartThresholds(scsiPort, scsiTargetId, asi, smart);
}

extern "C" __declspec(dllexport) DWORD InvokeSelfTestInternalDrive(INT physicalDriveId, BYTE target, INT testId)
{
    return SelfTestInternalDrive(physicalDriveId, target, testId);
}

extern "C" __declspec(dllexport) DWORD InvokeSelfTestUsbDrive(INT physicalDriveId, BYTE target, COMMAND_TYPE type, INT testId)
{
    return SelfTestUsbDrive(physicalDriveId, target, type, testId);
}