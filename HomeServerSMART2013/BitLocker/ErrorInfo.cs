using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public static class ErrorInfo
    {
        private static String guenivereOfGorgeous;

        static ErrorInfo()
        {
            guenivereOfGorgeous = "BaronessOfBeauty";
        }

        public static String GetErrorMessage(BitLockerVolumeStatus bvms, out String symbolicName, out String errorCode)
        {
            String errorMessage = String.Empty;
            bool isDefaultHandler = false;
            switch (bvms)
            {
                case BitLockerVolumeStatus.S_OK:
                    {
                        errorCode = "0x0";
                        errorMessage = "The command completed successfully.";
                        break;
                    }
                case BitLockerVolumeStatus.S_FALSE:
                    {
                        errorCode = "0x1";
                        errorMessage = "Exceptions were detected.";
                        break;
                    }
                case BitLockerVolumeStatus.ERROR_CERT_FILE_NOT_FOUND:
                    {
                        errorCode = "0x2";
                        errorMessage = "The system cannot find the certificate file specified.";
                        break;
                    }
                case BitLockerVolumeStatus.ERROR_INVALID_DATA_BLVERSION:
                    {
                        errorCode = "0xD";
                        errorMessage = "The driver returned an unsupported data format.";
                        break;
                    }
                case BitLockerVolumeStatus.ERROR_INVALID_OPERATION:
                    {
                        errorCode = "0x10DD";
                        errorMessage = "A key must be specified before encryption may begin.";
                        break;
                    }
                case BitLockerVolumeStatus.E_INVALIDARG_BLVERSION:
                    {
                        errorCode = "0xCCD802F";
                        errorMessage = "One or more of the arguments are not valid.";
                        break;
                    }
                case BitLockerVolumeStatus.ERROR_FILE_NOT_FOUND:
                    {
                        errorCode = "0x80070002";
                        errorMessage = "The system cannot find the file specified.";
                        break;
                    }
                case BitLockerVolumeStatus.ERROR_PATH_NOT_FOUND:
                    {
                        errorCode = "0x80070003";
                        errorMessage = "The system cannot find the path specified.";
                        break;
                    }
                case BitLockerVolumeStatus.ERROR_INVALID_DATA:
                    {
                        errorCode = "0x8007000D";
                        errorMessage = "The data specified is invalid.";
                        break;
                    }
                case BitLockerVolumeStatus.E_ACCESS_DENIED:
                    {
                        errorCode = "0x80070005";
                        errorMessage = "Access is denied.";
                        break;
                    }
                case BitLockerVolumeStatus.E_INVALIDARG:
                    {
                        errorCode = "0x80070057";
                        errorMessage = "One or more arguments specified but are invalid, corrupted, out of range or not supported " +
                            "by the called method.";
                        break;
                    }
                case BitLockerVolumeStatus.ERROR_DISK_FULL:
                    {
                        errorCode = "0x80070070";
                        errorMessage = "The disk is full. Delete some files on the disk and try again.";
                        break;
                    }
                case BitLockerVolumeStatus.ERROR_ELEMENT_NOT_FOUND:
                    {
                        errorCode = "0x80070490";
                        errorMessage = "Element not found.";
                        break;
                    }
                case BitLockerVolumeStatus.E_CANNOT_ESCROW_KEY_TO_ACTIVE_DIRECTORY:
                    {
                        errorCode = "0x8007054B";
                        errorMessage = "The specified domain does not exist or cannot be contacted. Group Policy requires key " +
                            "escrow to Active Directory, but no Active Directory domain controller is available. Ensure you are " +
                            "connected to the domain network, or if a remote access solution such as VPN is available, enable " +
                            "it and try the operation again.";
                        break;
                    }
                case BitLockerVolumeStatus.ERROR_INVALID_OPERATION2:
                    {
                        errorCode = "0x800710DD";
                        errorMessage = "A key must be specified before encryption may begin.";
                        break;
                    }
                case BitLockerVolumeStatus.NTE_BAD_KEYSET:
                    {
                        errorCode = "0x80090016";
                        errorMessage = "The key container specified does not exist, you do not have access " +
                            "to the key container or the Protected Storage Service is not running.";
                        break;
                    }
                case BitLockerVolumeStatus.NTE_KEYSET_NOT_DEFINED:
                    {
                        errorCode = "0x80090019";
                        errorMessage = "The keyset is not defined. This is usually caused by an improperly " +
                            "installed smart card reader, or, if the cryptographic service provider " +
                            "(driver) software is not from Microsoft, it may be incorrectly installed or " +
                            "corrupted. This can also occur if the smart card is using the Microsoft generic " +
                            "smart card driver, which provides basic functionality but doesn't necessarily " +
                            "support all smart card features. Not all smart cards are compatible with BitLocker.";
                        break;
                    }
                case BitLockerVolumeStatus.NTE_SILENT_CONTEXT:
                    {
                        errorCode = "0x80090022";
                        errorMessage = "Provider could not perform the action since the context was acquired " +
                            "as silent. The most likely cause of this error is that your hardware token " +
                            "(Smart Card) has been removed. Make sure your Smart Card is inserted, the Smart " +
                            "Card reader is active and the Smart Card is being recognized by Windows, and " +
                            "try again.";
                        break;
                    }
                case BitLockerVolumeStatus.CRYPT_E_NO_KEY_PROPERTY:
                    {
                        errorCode = "0x8009200B";
                        errorMessage = "Cannot find the certificate and private key for decryption.";
                        break;
                    }
                case BitLockerVolumeStatus.TBS_E_SERVICE_NOT_RUNNING:
                    {
                        errorCode = "0x80284008";
                        errorMessage = "No compatible Trusted Platform Module (TPM) was detected in this computer.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_E_SYSTEM_CANCELLED:
                    {
                        errorCode = "0x80100012";
                        errorMessage = "Windows cancelled the Smart Card action because a system shutdown " +
                            "is in progress.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_F_COMM_ERROR:
                    {
                        errorCode = "0x80100013";
                        errorMessage = "A Smart Card internal communications error was detected.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_F_UNKNOWN_ERROR:
                    {
                        errorCode = "0x80100014";
                        errorMessage = "A Smart Card internal error of unknown origin was detected.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_E_READER_UNAVAILABLE:
                    {
                        errorCode = "0x80100017";
                        errorMessage = "The specified Smart Card reader is currently unavailable.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_P_SHUTDOWN:
                    {
                        errorCode = "0x80100018";
                        errorMessage = "The operation has been stopped to allow the server application " +
                            "to exit.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_E_NO_SERVICE:
                    {
                        errorCode = "0x8010001D";
                        errorMessage = "Smart Cards for Windows is not running.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_E_SERVICE_STOPPED:
                    {
                        errorCode = "0x8010001E";
                        errorMessage = "Smart Cards for Windows has been shut down.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_E_UNEXPECTED:
                    {
                        errorCode = "0x8010001F";
                        errorMessage = "An unexpected Smart Card error has occurred.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_E_NO_SUCH_CERTIFICATE:
                    {
                        errorCode = "0x8010002C";
                        errorMessage = "The requested certificate does not exist.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_E_CERTIFICATE_UNAVAILABLE:
                    {
                        errorCode = "0x8010002D";
                        errorMessage = "The requested certificate could not be obtained.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_E_NO_READERS_AVAILABLE:
                    {
                        errorCode = "0x8010002E";
                        errorMessage = "Cannot find a Smart Card reader. If you have a " +
                            "Smart Card reader, make sure it is plugged in and, if necessary, " +
                            "powered on and try the operation again.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_E_COMM_DATA_LOST:
                    {
                        errorCode = "0x8010002F";
                        errorMessage = "A communications error with the Smart Card has " +
                            "been detected. Retry the operation.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_E_NO_KEY_CONTAINER:
                    {
                        errorCode = "0x80100030";
                        errorMessage = "The requested key container does not exist.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_E_SERVER_TOO_BUSY:
                    {
                        errorCode = "0x80100031";
                        errorMessage = "Smart Cards for Windows is too busy to complete " +
                            "this operation.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_E_PIN_CACHE_EXPIRED:
                    {
                        errorCode = "0x80100032";
                        errorMessage = "The Smart Card PIN cache has expired.";
                        break;
                    }
                case BitLockerVolumeStatus.SCARD_E_NO_PIN_CACHE:
                    {
                        errorCode = "0x80100033";
                        errorMessage = "The Smart Card PIN cannot be cached.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_LOCKED_VOLUME:
                    {
                        errorCode = "0x80310000";
                        errorMessage = "The drive is locked by BitLocker Drive Encryption. Please unlock the drive " +
                            "before performing any operations against it.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NOT_ENCRYPTED:
                    {
                        errorCode = "0x80310001";
                        errorMessage = "The volume is not encrypted with BitLocker Drive Encryption.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NO_TPM_BIOS:
                    {
                        errorCode = "0x80310002";
                        errorMessage = "The BIOS did not correctly communicate with the TPM. Please contact your " +
                            "computer manufacturer for BIOS upgrade instructions.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NO_MBR_METRIC:
                    {
                        errorCode = "0x80310003";
                        errorMessage = "The BIOS did not correctly communicate with the Master Boot Record (MBR). " +
                            "Please contact your computer manufacturer for BIOS upgrade instructions.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NO_BOOTSECTOR_METRIC:
                    {
                        errorCode = "0x80310004";
                        errorMessage = "A bootable CD/DVD was in your computer when Windows started. Please remove " +
                            "it and reboot your computer to turn on BitLocker. If a bootable CD/DVD was not in the " +
                            "computer, your system drive might need an updated Master Boot Record (MBR).";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NO_BOOTMGR_METRIC:
                    {
                        errorCode = "0x80310005";
                        errorMessage = "You have an incompatible boot sector. Update the Windows Boot Mnaager (BOOTMGR).";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_WRONG_BOOTMGR:
                    {
                        errorCode = "0x80310006";
                        errorMessage = "You have an incompatible boot manager. Update the Windows Boot Manager (BOOTMGR).";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_SECURE_KEY_REQUIRED:
                    {
                        errorCode = "0x80310007";
                        errorMessage = "No secure key protector has been defined.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NOT_ACTIVATED:
                    {
                        errorCode = "0x80310008";
                        errorMessage = "BitLocker Drive Encryption is not enabled on this volume. Please turn on "
                            + "BitLocker.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_ACTION_NOT_ALLOWED:
                    {
                        errorCode = "0x80310009";
                        errorMessage = "BitLocker Drive Encryption could not perform the requested action. This " +
                            "condition may occur when two requests are issued at the same time.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_AD_SCHEMA_NOT_INSTALLED:
                    {
                        errorCode = "0x8031000A";
                        errorMessage = "The Active Directory Domain Services forest does not contain the required " +
                            "attributes and classes to host BitLocker Drive Encryption or Trusted Platform Module " +
                            "information. Please contact your domain administrator.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_AD_INVALID_DATATYPE:
                    {
                        errorCode = "0x8031000B";
                        errorMessage = "The type of data obtained from Active Directory was not expected. " +
                            "Please contact your domain administrator.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_AD_INVALID_DATASIZE:
                    {
                        errorCode = "0x8031000C";
                        errorMessage = "The size of the data obtained from Active Directory was not expected. " +
                            "Please contact your domain administrator.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_AD_NO_VALUES:
                    {
                        errorCode = "0x8031000D";
                        errorMessage = "The attribute read from Active Directory has no (zero) values. " +
                            "Please contact your domain administrator.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_AD_ATTR_NOT_SET:
                    {
                        errorCode = "0x8031000E";
                        errorMessage = "The attribute was not set in Active Directory. Please contact your " +
                            "domain administrator.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_AD_GUID_NOT_FOUND:
                    {
                        errorCode = "0x8031000F";
                        errorMessage = "The specified GUID could not be found in Active Directory. Please contact " +
                            "your domain administrator.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_BAD_INFORMATION:
                    {
                        errorCode = "0x80310010";
                        errorMessage = "The control block for the encrypted volume is invalid.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_TOO_SMALL:
                    {
                        errorCode = "0x80310011";
                        errorMessage = "The volume cannot be encrypted because it does not have enough free space.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_SYSTEM_VOLUME:
                    {
                        errorCode = "0x80310012";
                        errorMessage = "The volume cannot be encrypted because it contains the system boot information.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_FAILED_WRONG_FS:
                    {
                        errorCode = "0x80310013";
                        errorMessage = "The volume cannot be encrypted because the file system is not supported. " +
                            "In Windows Vista and Server 2008, only NTFS volumes can be encrypted. In Windows 7 " +
                            "and Server 2008 R2, you can encrypt FAT, FAT32, exFAT and NTFS volumes.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_BAD_PARTITION_SIZE:
                    {
                        errorCode = "0x80310014";
                        errorMessage = "The file system size is larger than the partition size in the partition table.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NOT_SUPPORTED:
                    {
                        errorCode = "0x80310015";
                        errorMessage = "This volume cannot be encrypted. It is not supported by BitLocker Drive " +
                            "Encryption.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_BAD_DATA:
                    {
                        errorCode = "0x80310016";
                        errorMessage = "The data supplied is malformed.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_VOLUME_NOT_BOUND:
                    {
                        errorCode = "0x80310017";
                        errorMessage = "Automatic unlocking is already disabled for the volume on this computer.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_TPM_NOT_OWNED:
                    {
                        errorCode = "0x80310018";
                        errorMessage = "You must take ownership of the Trusted Platform Module (TPM).";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NOT_DATA_VOLUME:
                    {
                        errorCode = "0x80310019";
                        errorMessage = "The volume specified is not a data volume.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_AD_INSUFFICIENT_BUFFER:
                    {
                        errorCode = "0x8031001A";
                        errorMessage = "The buffer supplied to a function was insufficient to contain the returned data.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_CONV_READ:
                    {
                        errorCode = "0x8031001B";
                        errorMessage = "A read operation failed while converting the volume.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_CONV_WRITE:
                    {
                        errorCode = "0x8031001C";
                        errorMessage = "A write operation failed while converting the volume.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_KEY_REQUIRED:
                    {
                        errorCode = "0x8031001D";
                        errorMessage = "One or more key protectors are required for this volume (currently there " +
                            "are none).";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_CLUSTERING_NOT_SUPPORTED:
                    {
                        errorCode = "0x8031001E";
                        errorMessage = "BitLocker Drive Encryption does not support Cluster configurations.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_VOLUME_BOUND_ALREADY:
                    {
                        errorCode = "0x8031001F";
                        errorMessage = "Automatic unlocking is already enabled on this volume for this computer.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_OS_NOT_PROTECTED:
                    {
                        errorCode = "0x80310020";
                        errorMessage = "The operating system volume is not protected by BitLocker Drive Encryption. " +
                            "Enable BitLocker on the OS volume and try again.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_PROTECTION_DISABLED:
                    {
                        errorCode = "0x80310021";
                        errorMessage = "All key protectors are disabled; the encryption key is exposed in the clear " +
                            "on the disk.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_RECOVERY_KEY_REQUIRED:
                    {
                        errorCode = "0x80310022";
                        errorMessage = "A recovery key protector is required.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_FOREIGN_VOLUME:
                    {
                        errorCode = "0x80310023";
                        errorMessage = "This volume cannot be bound to a TPM.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_OVERLAPPED_UPDATE:
                    {
                        errorCode = "0x80310024";
                        errorMessage = "The control block for the encrypted volume was updated by another thread. " +
                            "Try the operation again.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_TPM_SRK_AUTH_NOT_ZERO:
                    {
                        errorCode = "0x80310025";
                        errorMessage = "The authorization data for the Storage Root Key (SRK) of the Trusted Platform " +
                            "Module (TPM) is not zero and is therefore incompatible with BitLocker Drive Encryption.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_FAILED_SECTOR_SIZE:
                    {
                        errorCode = "0x80310026";
                        errorMessage = "The volume encryption algorithm cannot be used on this sector size.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_FAILED_AUTHENTICATION:
                    {
                        errorCode = "0x80310027";
                        errorMessage = "The volume cannot be unlocked with the provided information. Confirm the " +
                            "information and try again.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NOT_OS_VOLUME:
                    {
                        errorCode = "0x80310028";
                        errorMessage = "The volume specified is not the operating system volume.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_AUTOUNLOCK_ENABLED:
                    {
                        errorCode = "0x80310029";
                        errorMessage = "BitLocker Drive Encryption cannot be turned off because automatic unlocking is " +
                            "enabled on at least one volume. Please turn off automatic unlocking on all other volumes " +
                            "before turning off BitLocker on the current volume.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_WRONG_BOOTSECTOR:
                    {
                        errorCode = "0x8031002A";
                        errorMessage = "The system partition boot sector does not perform TPM measurements.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_WRONG_SYSTEM_FS:
                    {
                        errorCode = "0x8031002B";
                        errorMessage = "BitLocker requires the file system to be NTFS. Convert the volume to NTFS, " +
                            "then enable BitLocker.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_PASSWORD_REQUIRED:
                    {
                        errorCode = "0x8031002C";
                        errorMessage = "Group Policy requires a recovery password before encryption may begin.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_CANNOT_SET_FVEK_ENCRYPTED:
                    {
                        errorMessage = "BitLocker cannot encrypt the volume because encryption is already in progress " +
                            "and the encryption method specified does not match the encryption method already in use.";
                        errorCode = "0x8031002D";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_CANNOT_ENCRYPT_NO_KEY:
                    {
                        errorCode = "0x8031002E";
                        errorMessage = "A key must be specified before encryption may begin.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_BOOTABLE_CDDVD:
                    {
                        errorCode = "0x80310030";
                        errorMessage = "BitLocker Drive Encryption detected bootable media (CD, DVD or USB) in the " +
                            "computer. Please remove the media and reboot the computer.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_PROTECTOR_EXISTS:
                    {
                        errorCode = "0x80310031";
                        errorMessage = "An instance of this key protector already exists on the volume.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_RELATIVE_PATH:
                    {
                        errorCode = "0x80310032";
                        errorMessage = "The file cannot be saved to a relative path.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_PROTECTOR_NOT_FOUND:
                    {
                        errorCode = "0x80310033";
                        errorMessage = "The specified key protector was not found on the volume. Try another key " +
                            "protector.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_INVALID_KEY_FORMAT:
                    {
                        errorCode = "0x80310034";
                        errorMessage = "The recovery password file on the USB device is corrupt. Try another USB device.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_INVALID_PASSWORD_FORMAT:
                    {
                        errorCode = "0x80310035";
                        errorMessage = "The format of the recovery password is invalid. Confirm the recovery password " +
                            "and try again.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_FIPS_RNG_CHECK_FAILED:
                    {
                        errorCode = "0x80310036";
                        errorMessage = "The random number generator check test failed.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_FIPS_PREVENTS_RECOVERY_PASSWORD:
                    {
                        errorCode = "0x80310037";
                        errorMessage = "The Group Policy setting requiring FIPS compliance prevented the recovery " +
                            "password from being generated or used. Please contact your domain administrator for " +
                            "more information.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_FIPS_PREVENTS_EXTERNAL_KEY_EXPORT:
                    {
                        errorCode = "0x80310038";
                        errorMessage = "The Group Policy setting requiring FIPS compliance prevented the recovery " +
                            "password from being saved. Please contact your domain administrator for more information.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NOT_DECRYPTED:
                    {
                        errorCode = "0x80310039";
                        errorMessage = "The volume must be fully decrypted to complete this operation.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_INVALID_PROTECTOR_TYPE:
                    {
                        errorCode = "0x8031003A";
                        errorMessage = "The specified key protector is not of the correct type.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NO_PROTECTORS_TO_TEST:
                    {
                        errorCode = "0x8031003B";
                        errorMessage = "No TPM protectors exist on the volume to perform the hardware test.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_KEYFILE_NOT_FOUND:
                    {
                        errorCode = "0x8031003C";
                        errorMessage = "The BitLocker startup key or recovery password could not be read from " +
                            "the USB device. Ensure the USB device is plugged into the computer, then turn on " +
                            "BitLocker and try again. If the problem persists, contact the computer manufacturer " +
                            "for BIOS upgrade instructions. Your computer may not support boot-time reading of " +
                            "USB devices. This problem may also occur if you have more than one USB storage device " +
                            "connected at boot-time (any combination of flash drives, hard drives or other storage-" +
                            "capable devices like smart phones). Ensure only one USB device is connected and try " +
                            "again.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_KEYFILE_INVALID:
                    {
                        errorCode = "0x8031003D";
                        errorMessage = "The BitLocker startup key or recovery password is corrupt or invalid.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_KEYFILE_NO_VMK:
                    {
                        errorCode = "0x8031003E";
                        errorMessage = "The BitLocker encryption key could not be obtained from the startup key " +
                            "or recovery password.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_TPM_DISABLED:
                    {
                        errorCode = "0x8031003F";
                        errorMessage = "The Trusted Platform Module (TPM) is disabled.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NOT_ALLOWED_IN_SAFE_MODE:
                    {
                        errorCode = "0x80310040";
                        errorMessage = "BitLocker Drive Encryption can only be used for recovery purposes in Safe Mode.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_TPM_INVALID_PCR:
                    {
                        errorCode = "0x80310041";
                        errorMessage = "The Trusted Platform Module (TPM) was not able to unlock the volume because the " +
                            "system boot information changed.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_TPM_NO_VMK:
                    {
                        errorCode = "0x80310042";
                        errorMessage = "The BitLocker encryption key could not be obtained from the Trusted Platform " +
                            "Module (TPM).";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_PIN_INVALID:
                    {
                        errorCode = "0x80310043";
                        errorMessage = "The BitLocker encryption key could not be obtained from the Trusted Platform " +
                            "Module (TPM) and PIN.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_AUTH_INVALID_APPLICATION:
                    {
                        errorCode = "0x80310044";
                        errorMessage = "A boot application has changed since BitLocker was enabled.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_AUTH_INVALID_CONFIG:
                    {
                        errorCode = "0x80310045";
                        errorMessage = "The Boot Configuration Data (BCD) settings have changed since BitLocker " +
                            "was enabled.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_FIPS_DISABLE_PROTECTION_NOT_ALLOWED:
                    {
                        errorCode = "0x80310046";
                        errorMessage = "The Group Policy setting requiring FIPS compliance prevented BitLocker " +
                            "from being disabled. If you need to decrypt your volume because of a compromised key " +
                            "protector, create a new key protector on the volume, then delete the protector(s) you " +
                            "believe have been compromised. Please contact your domain administrator for more information.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_FS_NOT_EXTENDED:
                    {
                        errorCode = "0x80310047";
                        errorMessage = "The file system does not extend to the end of the volume.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_FIRMWARE_TYPE_NOT_SUPPORTED:
                    {
                        errorCode = "0x80310048";
                        errorMessage = "BitLocker Drive Encryption cannot be enabled on this computer. Please contact " +
                            "your computer manufacturer for BIOS upgrade instructions.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NO_LICENSE:
                    {
                        errorCode = "0x80310049";
                        errorMessage = "This Windows license does not support BitLocker Drive Encryption. To use " +
                            "BitLocker, please upgrade your Windows license.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NOT_ON_STACK:
                    {
                        errorCode = "0x8031004A";
                        errorMessage = "Critical BitLocker Drive Encryption system files are not available. Use " +
                            "Windows Startup Repair to restore files.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_FS_MOUNTED:
                    {
                        errorCode = "0x8031004B";
                        errorMessage = "This operation cannot be performed while the volume is in use.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_TOKEN_NOT_IMPERSONATED:
                    {
                        errorCode = "0x8031004C";
                        errorMessage = "The access token associated with the current thread is not an impersonated " +
                            "token.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_DRY_RUN_FAILED:
                    {
                        errorCode = "0x8031004D";
                        errorMessage = "The BitLocker encryption key could not be obtained.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_REBOOT_REQUIRED:
                    {
                        errorCode = "0x8031004E";
                        errorMessage = "No action was taken because a mandatory reboot is pending. Please reboot the " +
                            "computer and try again.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_DEBUGGER_ENABLED:
                    {
                        errorCode = "0x8031004F";
                        errorMessage = "Boot debugging is enabled. Run BCDEDIT to turn it off.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_RAW_ACCESS:
                    {
                        errorCode = "0x80310050";
                        errorMessage = "No action was taken because BitLocker Drive Encryption is in raw access mode.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_RAW_BLOCKED:
                    {
                        errorCode = "0x80310051";
                        errorMessage = "BitLocker Drive Encryption cannot enter raw access mode for this volume.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_BCD_APPLICATIONS_PATH_INCORRECT:
                    {
                        errorCode = "0x80310052";
                        errorMessage = "The path specified in the Boot Configuration Data (BCD) for a BitLocker " +
                            "Drive Encryption integrity-protected application is incorrect. Please verify and " +
                            "correct your BCD settings and try again.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NOT_ALLOWED_IN_VERSION:
                    {
                        errorCode = "0x80310053";
                        errorMessage = "BitLocker Drive Encryption can only be used for recovery purposes in " +
                            "this version of Windows.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NO_AUTOUNLOCK_MASTER_KEY:
                    {
                        errorCode = "0x80310054";
                        errorMessage = "The auto-unlock master key was not available from the operating system " +
                            "volume. Retry the operation using the BitLocker WMI interface.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_MOR_FAILED:
                    {
                        errorCode = "0x80310055";
                        errorMessage = "The system firmware failed to enable clearing of system memory on reboot.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_HIDDEN_VOLUME:
                    {
                        errorCode = "0x80310056";
                        errorMessage = "The hidden volume cannot be encrypted.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_TRANSIENT_STATE:
                    {
                        errorCode = "0x80310057";
                        errorMessage = "BitLocker encryption keys were ignored because the volume was in a transient state.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_PUBKEY_NOT_ALLOWED:
                    {
                        errorCode = "0x80310058";
                        errorMessage = "Public key based protectors are not allowed on this volume.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_VOLUME_HANDLE_OPEN:
                    {
                        errorCode = "0x80310059";
                        errorMessage = "BitLocker Drive Encryption is already performing an operation on this volume. Please " +
                            "complete all operations before continuing.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NO_FEATURE_LICENSE:
                    {
                        errorCode = "0x8031005A";
                        errorMessage = "This Windows license does not support this feature of BitLocker Drive Encryption. " +
                            "To enable this functionality, please upgrade your Windows license.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_INVALID_STARTUP_OPTIONS:
                    {
                        errorCode = "0x8031005B";
                        errorMessage = "The policy for startup options is inconsistent. Contact your system administrator " +
                            "for more information.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_RECOVERY_PASSWORD_NOT_ALLOWED:
                    {
                        errorCode = "0x8031005C";
                        errorMessage = "Group Policy does not permit the creation of a recovery password.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_RECOVERY_PASSWORD_REQUIRED:
                    {
                        errorCode = "0x8031005D";
                        errorMessage = "Group Policy requires the creation of a recovery password.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_RECOVERY_KEY_NOT_ALLOWED:
                    {
                        errorCode = "0x8031005E";
                        errorMessage = "Group Policy does not permit the creation of a recovery key.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_RECOVERY_KEY_REQUIRED:
                    {
                        errorCode = "0x8031005F";
                        errorMessage = "Group Policy requires the creation of a recovery key.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_STARTUP_PIN_NOT_ALLOWED:
                    {
                        errorCode = "0x80310060";
                        errorMessage = "Group Policy does not permit the use of a PIN at startup. Please choose " +
                            "a different BitLocker startup option.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_STARTUP_PIN_REQUIRED:
                    {
                        errorCode = "0x80310061";
                        errorMessage = "Group Policy requires the use of a PIN at startup. Please choose this " +
                            "BitLocker startup option.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_STARTUP_KEY_NOT_ALLOWED:
                    {
                        errorCode = "0x80310062";
                        errorMessage = "Group Policy does not permit the use of a startup key. Please choose a " +
                            "different BitLocker startup option.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_STARTUP_KEY_REQUIRED:
                    {
                        errorCode = "0x80310063";
                        errorMessage = "Group Policy requires the use of a startup key. Please choose this " +
                            "BitLocker startup option.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_STARTUP_PIN_KEY_NOT_ALLOWED:
                    {
                        errorCode = "0x80310064";
                        errorMessage = "Group Policy does not permit the use of a startup key and PIN. Please " +
                            "choose a different BitLocker startup option.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_STARTUP_PIN_KEY_REQUIRED:
                    {
                        errorCode = "0x80310065";
                        errorMessage = "Group Policy requires the use of a startup key and PIN. Please choose " +
                            "this BitLocker startup option.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_STARTUP_TPM_NOT_ALLOWED:
                    {
                        errorCode = "0x80310066";
                        errorMessage = "Group Policy does not permit the use of TPM-only at startup. Please choose " +
                            "a different BitLocker startup option.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_STARTUP_TPM_REQUIRED:
                    {
                        errorCode = "0x80310067";
                        errorMessage = "Group Policy requires the use of TPM-only at startup. Please choose this " +
                            "BitLocker startup option.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_INVALID_PIN_LENGTH:
                    {
                        errorCode = "0x80310068";
                        errorMessage = "The PIN provided does not meet the minimum PIN length requirements.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_KEY_PROTECTOR_NOT_SUPPORTED:
                    {
                        errorCode = "0x80310069";
                        errorMessage = "The key protector is not supported by the version of BitLocker Drive " +
                            "Encryption currently on the volume. Upgrade the volume to add the key protector. " +
                            "(WARNING: Upgrading BitLocker is irreversible.)";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_PASSPHRASE_NOT_ALLOWED:
                    {
                        errorCode = "0x8031006A";
                        errorMessage = "Group Policy does not permit the creation of a password.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_PASSPHRASE_REQUIRED:
                    {
                        errorCode = "0x8031006B";
                        errorMessage = "Group Policy requires the creation of a password. If you are trying to " +
                            "create a different protector type, add the password first. Then try to add any other " +
                            "protector(s) again.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_FIPS_PREVENTS_PASSPHRASE:
                    {
                        errorCode = "0x8031006C";
                        errorMessage = "The Group Policy setting requiring FIPS compliance prevented the passphrase " +
                            "from being generated or used. Please contact your domain administrator for more information.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_OS_VOLUME_PASSPHRASE_NOT_ALLOWED:
                    {
                        errorCode = "0x8031006D";
                        errorMessage = "A password cannot be added to the operating system volume.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_INVALID_BITLOCKER_OID:
                    {
                        errorCode = "0x8031006E";
                        errorMessage = "The available BitLocker object identifier (OID) is a badly-formed OID.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_VOLUME_TOO_SMALL:
                    {
                        errorCode = "0x8031006F";
                        errorMessage = "The volume is too small to be protected using BitLocker Drive Encryption.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_DV_NOT_SUPPORTED_ON_FS:
                    {
                        errorCode = "0x80310070";
                        errorMessage = "The selected Discovery Volume type is incompatible with the file system on the drive.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_DV_NOT_ALLOWED_BY_GP:
                    {
                        errorCode = "0x80310071";
                        errorMessage = "The selected Discovery Volume type is not allowed by the machine's Group Policy " +
                            "settings.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_USER_CERTIFICATE_NOT_ALLOWED:
                    {
                        errorCode = "0x80310072";
                        errorMessage = "Group Policy does not permit user certificates such as smart cards to be used " +
                            "with BitLocker.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_USER_CERTIFICATE_REQUIRED:
                    {
                        errorCode = "0x80310073";
                        errorMessage = "Group Policy requires the use of a user certificate, such as a smart card.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_USER_CERT_MUST_BE_HW:
                    {
                        errorCode = "0x80310074";
                        errorMessage = "Group Policy requires the use of a hardware-based user certificate (smart card) " +
                            "to use BitLocker.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_USER_CONFIGURE_FDV_AUTOUNLOCK_NOT_ALLOWED:
                    {
                        errorCode = "0x80310075";
                        errorMessage = "Group Policy does not permit the use of automatic unlocking on a fixed data " +
                            "volume.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_USER_CONFIGURE_RDV_AUTOUNLOCK_NOT_ALLOWED:
                    {
                        errorCode = "0x80310076";
                        errorMessage = "Group Policy does not permit the use of automatic unlocking on roaming data " +
                            "volumes.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_USER_CONFIGURE_RDV_NOT_ALLOWED:
                    {
                        errorCode = "0x80310077";
                        errorMessage = "Group Policy does not permit the use of BitLocker Drive Encryption on roaming " +
                            "data volumes.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_USER_ENABLE_RDV_NOT_ALLOWED:
                    {
                        errorCode = "0x80310078";
                        errorMessage = "Group Policy does not permit you to enable BitLocker Drive Encryption on " +
                            "roaming data volumes.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_USER_DISABLE_RDV_NOT_ALLOWED:
                    {
                        errorCode = "0x80310079";
                        errorMessage = "Group Policy does not permit you to disable BitLocker Drive Encryption on " +
                            "roaming data volumes. If you need to decrypt your volume because of a compromised key " +
                            "protector, create a new key protector on the volume, then delete the protector(s) you " +
                            "believe have been compromised.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_INVALID_PASSPHRASE_LENGTH:
                    {
                        errorCode = "0x80310080";
                        errorMessage = "The password provided does not meet the minimum or maximum length " +
                            "requirements.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_PASSPHRASE_TOO_SIMPLE:
                    {
                        errorCode = "0x80310081";
                        errorMessage = "Your password does not meet the complexity requirements set by your " +
                            "system administrator. Try adding upper and lower case characters, numerals and symbols.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_RECOVERY_PARTITION:
                    {
                        errorCode = "0x80310082";
                        errorMessage = "The volume cannot be encrypted because it is reserved for Windows System " +
                            "Recovery Options.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_CONFLICT_FDV_RK_OFF_AUK_ON:
                    {
                        errorCode = "0x80310083";
                        errorMessage = "BitLocker setup cannot start. There are conflicting Group Policy settings for " +
                            "fixed data volumes. Please contact your system administrator. Automatic unlocking cannot " +
                            "be used when recovery keys have been disabled.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_CONFLICT_RDV_RK_OFF_AUK_ON:
                    {
                        errorCode = "0x80310084";
                        errorMessage = "BitLocker setup cannot start. There are conflicting Group Policy settings for " +
                            "roaming data volumes. Please contact your system administrator. Automatic unlocking cannot " +
                            "be used when recovery keys have been disabled.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NON_BITLOCKER_OID:
                    {
                        errorCode = "0x80310085";
                        errorMessage = "The Enhanced Key Usage (EKU) attribute of the specified certificate does not permit " +
                            "it to be used for BitLocker Drive Encryption. BitLocker does not require that a certificate " +
                            "have an EKU attribute, but if one is configured, it must be set to an object identifier (OID) " +
                            "that matches the OID configured for BitLocker.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_PROHIBITS_SELFSIGNED:
                    {
                        errorCode = "0x80310086";
                        errorMessage = "Group Policy does not permit the use of self-signed certificates.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_CONFLICT_RO_AND_STARTUP_KEY_REQUIRED:
                    {
                        errorCode = "0x80310087";
                        errorMessage = "BitLocker setup cannot start. There are conflicting Group Policy settings. Please " +
                            "contact your system administrator. Use of startup keys cannot be required when read-only policy " +
                            "for unprotected data volumes is enabled.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_CONV_RECOVERY_FAILED:
                    {
                        errorCode = "0x80310088";
                        errorMessage = "BitLocker Drive Encryption failed to recover from aborted conversion. This could be " +
                            "due to either all conversion logs being corrupted or the media being write protected.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_VIRTUALIZED_SPACE_TOO_BIG:
                    {
                        errorCode = "0x80310089";
                        errorMessage = "The requested virtualization size is too big.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_CONFLICT_OSV_RP_OFF_ADB_ON:
                    {
                        errorCode = "0x80310090";
                        errorMessage = "BitLocker setup cannot start. There are conflicting Group Policy settings for " +
                            "recovery options on the volume. Please contact your system administrator. Backup to Active " +
                            "Directory on the operating system volume cannot be required when recovery passwords are " +
                            "disallowed for it.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_CONFLICT_FDV_RP_OFF_ADB_ON:
                    {
                        errorCode = "0x80310091";
                        errorMessage = "BitLocker setup cannot start. There are conflicting Group Policy settings for " +
                            "recovery options on fixed data volumes. Please contact your system administrator. Backup to Active " +
                            "Directory on a fixed data volume cannot be required when recovery passwords are " +
                            "disallowed for it.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_POLICY_CONFLICT_RDV_RP_OFF_ADB_ON:
                    {
                        errorCode = "0x80310092";
                        errorMessage = "BitLocker setup cannot start. There are conflicting Group Policy settings for " +
                            "recovery options on roaming data volumes. Please contact your system administrator. Backup to Active " +
                            "Directory on a roaming data volume cannot be required when recovery passwords are " +
                            "disallowed for it.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_NON_BITLOCKER_KU:
                    {
                        errorCode = "0x80310093";
                        errorMessage = "The Key Usage (KU) attribute of the specified certificate does not permit it to " +
                            "be used with BitLocker Drive Encryption. BitLocker does not require a certificate to have a KU " +
                            "attribute, but if one is configured it must be set to either Key Encipherment or Key Agreement.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_PRIVATEKEY_AUTH_FAILED:
                    {
                        errorCode = "0x80310094";
                        errorMessage = "The private key, associated with the specified certificate, could not be authorized." +
                            "The private key authorization was either not provided or the provided authorization was invalid.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_REMOVAL_OF_DRA_FAILED:
                    {
                        errorCode = "0x80310095";
                        errorMessage = "Removal of the Data Recovery Agent (DRA) certificate must be done using the " +
                            "Certificates MMC snap-in.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_OPERATION_NOT_SUPPORTED_ON_VISTA_VOLUME:
                    {
                        errorCode = "0x80310096";
                        errorMessage = "This volume was encrypted using the version of BitLocker Drive Encryption " +
                            "included with Windows Vista and Windows Server 2008, which does not support Organizational " +
                            "Identifiers. To specify Organizational Identifiers on this volume, you must upgrade the " +
                            "drive encryption to the Windows 7 version, which is the default for Windows 7 and Windows " +
                            "Server 2008 R2. (WARNING: Upgrading the BitLocker version is irreversible, and the volue " +
                            "can no longer be unlocked by Windows Vista or Server 2008.)";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_CANT_LOCK_AUTOUNLOCK_ENABLED_VOLUME:
                    {
                        errorCode = "0x80310097";
                        errorMessage = "The drive cannot be locked because it is automatically unlocked on this " +
                            "computer. Remove the automatic unlock protector if you want to lock this volume.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_FIPS_HASH_KDF_NOT_ALLOWED:
                    {
                        errorCode = "0x80310098";
                        errorMessage = "The default BitLocker Key Derivation Function SP800-56A for ECC smart cards " +
                            "is not supported by your smart card. The Group Policy setting requiring FIPS compliance " +
                            "prevents BitLocker from using any other key derivation function for encryption. You " +
                            "must use a FIPS-compliant smart card in FIPS restricted environments.";
                        break;
                    }
                case BitLockerVolumeStatus.FVE_E_INVALID_PIN_CHARS:
                    {
                        errorCode = "0x8031009A";
                        errorMessage = "The PIN specified contains characters that are invalid. When the \"Allow " +
                            "enhanced PINs for startup\" Group Policy is disabled, or if you are using Windows " +
                            "Vista or Windows Server 2008, only numerals are allowed.";
                        break;
                    }
                default:
                    {
                        isDefaultHandler = true;
                        int val = (int)bvms;
                        String hexVal = val.ToString("X");
                        errorCode = "0x" + hexVal;
                        errorMessage = "Exceptions were detected, but an unknown error code was returned by " +
                            "BitLocker Drive Encryption. Please contact Dojo North Software and report the technical details shown below.";
                        break;
                    }
            }

            if (isDefaultHandler)
            {
                symbolicName = "EXCEPTIONS_WERE_DETECTED";
            }
            else
            {
                symbolicName = bvms.ToString();
            }
            return errorMessage;
        }

        public static String EmpressOfExceedinglyPretty
        {
            get
            {
                return guenivereOfGorgeous;
            }
        }
    }
}
