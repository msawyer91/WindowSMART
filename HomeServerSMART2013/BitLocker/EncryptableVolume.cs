using System;
using System.ComponentModel;
using System.Management;
using System.Collections;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public class EncryptableVolume : System.ComponentModel.Component
    {
        // Private property to hold the WMI namespace in which the class resides.
        private static string CreatedWmiNamespace = "ROOT\\CIMV2\\Security\\MicrosoftVolumeEncryption";

        // Private property to hold the name of WMI class which created this class.
        private static string CreatedClassName = "Win32_EncryptableVolume";

        // Private member variable to hold the ManagementScope which is used by the various methods.
        private static System.Management.ManagementScope statMgmtScope = null;

        private ManagementSystemProperties PrivateSystemProperties;

        // Underlying lateBound WMI object.
        private System.Management.ManagementObject PrivateLateBoundObject;

        // Member variable to store the 'automatic commit' behavior for the class.
        private bool AutoCommitProp;

        // Private variable to hold the embedded property representing the instance.
        private System.Management.ManagementBaseObject embeddedObj;

        // The current WMI object used
        private System.Management.ManagementBaseObject curObj;

        // Flag to indicate if the instance is an embedded object.
        private bool isEmbedded;

        // Below are different overloads of constructors to initialize an instance of the class with a WMI object.
        public EncryptableVolume()
        {
            this.InitializeObject(null, null, null);
        }

        public EncryptableVolume(string keyDeviceID)
        {
            this.InitializeObject(null, new System.Management.ManagementPath(EncryptableVolume.ConstructPath(keyDeviceID)), null);
        }

        public EncryptableVolume(System.Management.ManagementScope mgmtScope, string keyDeviceID)
        {
            this.InitializeObject(((System.Management.ManagementScope)(mgmtScope)), new System.Management.ManagementPath(EncryptableVolume.ConstructPath(keyDeviceID)), null);
        }

        public EncryptableVolume(System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions)
        {
            this.InitializeObject(null, path, getOptions);
        }

        public EncryptableVolume(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path)
        {
            this.InitializeObject(mgmtScope, path, null);
        }

        public EncryptableVolume(System.Management.ManagementPath path)
        {
            this.InitializeObject(null, path, null);
        }

        public EncryptableVolume(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions)
        {
            this.InitializeObject(mgmtScope, path, getOptions);
        }

        public EncryptableVolume(System.Management.ManagementObject theObject)
        {
            Initialize();
            if ((CheckIfProperClass(theObject) == true))
            {
                PrivateLateBoundObject = theObject;
                PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
                curObj = PrivateLateBoundObject;
            }
            else
            {
                throw new System.ArgumentException("Class name does not match.");
            }
        }

        public EncryptableVolume(System.Management.ManagementBaseObject theObject)
        {
            Initialize();
            if ((CheckIfProperClass(theObject) == true))
            {
                embeddedObj = theObject;
                PrivateSystemProperties = new ManagementSystemProperties(theObject);
                curObj = embeddedObj;
                isEmbedded = true;
            }
            else
            {
                throw new System.ArgumentException("Class name does not match.");
            }
        }

        // Property returns the namespace of the WMI class.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string OriginatingNamespace
        {
            get
            {
                return "ROOT\\CIMV2\\Security\\MicrosoftVolumeEncryption";
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ManagementClassName
        {
            get
            {
                string strRet = CreatedClassName;
                if ((curObj != null))
                {
                    if ((curObj.ClassPath != null))
                    {
                        strRet = ((string)(curObj["__CLASS"]));
                        if (((strRet == null)
                                    || (strRet == string.Empty)))
                        {
                            strRet = CreatedClassName;
                        }
                    }
                }
                return strRet;
            }
        }

        // Property pointing to an embedded object to get System properties of the WMI object.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ManagementSystemProperties SystemProperties
        {
            get
            {
                return PrivateSystemProperties;
            }
        }

        // Property returning the underlying lateBound object.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Management.ManagementBaseObject LateBoundObject
        {
            get
            {
                return curObj;
            }
        }

        // ManagementScope of the object.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Management.ManagementScope Scope
        {
            get
            {
                if ((isEmbedded == false))
                {
                    return PrivateLateBoundObject.Scope;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if ((isEmbedded == false))
                {
                    PrivateLateBoundObject.Scope = value;
                }
            }
        }

        // Property to show the commit behavior for the WMI object. If true, WMI object will be automatically saved after each property modification.(ie. Put() is called after modification of a property).
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AutoCommit
        {
            get
            {
                return AutoCommitProp;
            }
            set
            {
                AutoCommitProp = value;
            }
        }

        // The ManagementPath of the underlying WMI object.
        [Browsable(true)]
        public System.Management.ManagementPath Path
        {
            get
            {
                if ((isEmbedded == false))
                {
                    return PrivateLateBoundObject.Path;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if ((isEmbedded == false))
                {
                    if ((CheckIfProperClass(null, value, null) != true))
                    {
                        throw new System.ArgumentException("Class name does not match.");
                    }
                    PrivateLateBoundObject.Path = value;
                }
            }
        }

        // Public static scope property which is used by the various methods.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static System.Management.ManagementScope StaticScope
        {
            get
            {
                return statMgmtScope;
            }
            set
            {
                statMgmtScope = value;
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DeviceID
        {
            get
            {
                return ((string)(curObj["DeviceID"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DriveLetter
        {
            get
            {
                return ((string)(curObj["DriveLetter"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PersistentVolumeID
        {
            get
            {
                return ((string)(curObj["PersistentVolumeID"]));
            }
        }

        private bool CheckIfProperClass(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions OptionsParam)
        {
            if (((path != null)
                        && (string.Compare(path.ClassName, this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)))
            {
                return true;
            }
            else
            {
                return CheckIfProperClass(new System.Management.ManagementObject(mgmtScope, path, OptionsParam));
            }
        }

        private bool CheckIfProperClass(System.Management.ManagementBaseObject theObj)
        {
            if (((theObj != null)
                        && (string.Compare(((string)(theObj["__CLASS"])), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)))
            {
                return true;
            }
            else
            {
                System.Array parentClasses = ((System.Array)(theObj["__DERIVATION"]));
                if ((parentClasses != null))
                {
                    int count = 0;
                    for (count = 0; (count < parentClasses.Length); count = (count + 1))
                    {
                        if ((string.Compare(((string)(parentClasses.GetValue(count))), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        [Browsable(true)]
        public void CommitObject()
        {
            if ((isEmbedded == false))
            {
                PrivateLateBoundObject.Put();
            }
        }

        [Browsable(true)]
        public void CommitObject(System.Management.PutOptions putOptions)
        {
            if ((isEmbedded == false))
            {
                PrivateLateBoundObject.Put(putOptions);
            }
        }

        private void Initialize()
        {
            AutoCommitProp = true;
            isEmbedded = false;
        }

        private static string ConstructPath(string keyDeviceID)
        {
            string strPath = "ROOT\\CIMV2\\Security\\MicrosoftVolumeEncryption:Win32_EncryptableVolume";
            strPath = string.Concat(strPath, string.Concat(".DeviceID=", string.Concat("\"", string.Concat(keyDeviceID, "\""))));
            // strPath = string.Concat(strPath, string.Concat(".DeviceID=", keyDeviceID));
            return strPath;
        }

        private void InitializeObject(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions)
        {
            Initialize();
            if ((path != null))
            {
                if ((CheckIfProperClass(mgmtScope, path, getOptions) != true))
                {
                    throw new System.ArgumentException("Class name does not match.");
                }
            }
            PrivateLateBoundObject = new System.Management.ManagementObject(mgmtScope, path, getOptions);
            PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
            curObj = PrivateLateBoundObject;
        }

        // Different overloads of GetInstances() help in enumerating instances of the WMI class.
        public static EncryptableVolumeCollection GetInstances()
        {
            return GetInstances(null, null, null);
        }

        public static EncryptableVolumeCollection GetInstances(string condition)
        {
            return GetInstances(null, condition, null);
        }

        public static EncryptableVolumeCollection GetInstances(System.String[] selectedProperties)
        {
            return GetInstances(null, null, selectedProperties);
        }

        public static EncryptableVolumeCollection GetInstances(string condition, System.String[] selectedProperties)
        {
            return GetInstances(null, condition, selectedProperties);
        }

        public static EncryptableVolumeCollection GetInstances(System.Management.ManagementScope mgmtScope, System.Management.EnumerationOptions enumOptions)
        {
            if ((mgmtScope == null))
            {
                if ((statMgmtScope == null))
                {
                    mgmtScope = new System.Management.ManagementScope();
                    mgmtScope.Path.NamespacePath = "root\\CIMV2\\Security\\MicrosoftVolumeEncryption";
                }
                else
                {
                    mgmtScope = statMgmtScope;
                }
            }
            System.Management.ManagementPath pathObj = new System.Management.ManagementPath();
            pathObj.ClassName = "Win32_EncryptableVolume";
            pathObj.NamespacePath = "root\\CIMV2\\Security\\MicrosoftVolumeEncryption";
            System.Management.ManagementClass clsObject = new System.Management.ManagementClass(mgmtScope, pathObj, null);
            if ((enumOptions == null))
            {
                enumOptions = new System.Management.EnumerationOptions();
                enumOptions.EnsureLocatable = true;
            }
            return new EncryptableVolumeCollection(clsObject.GetInstances(enumOptions));
        }

        public static EncryptableVolumeCollection GetInstances(System.Management.ManagementScope mgmtScope, string condition)
        {
            return GetInstances(mgmtScope, condition, null);
        }

        public static EncryptableVolumeCollection GetInstances(System.Management.ManagementScope mgmtScope, System.String[] selectedProperties)
        {
            return GetInstances(mgmtScope, null, selectedProperties);
        }

        public static EncryptableVolumeCollection GetInstances(System.Management.ManagementScope mgmtScope, string condition, System.String[] selectedProperties)
        {
            if ((mgmtScope == null))
            {
                if ((statMgmtScope == null))
                {
                    mgmtScope = new System.Management.ManagementScope();
                    mgmtScope.Path.NamespacePath = "root\\CIMV2\\Security\\MicrosoftVolumeEncryption";
                }
                else
                {
                    mgmtScope = statMgmtScope;
                }
            }
            System.Management.ManagementObjectSearcher ObjectSearcher = new System.Management.ManagementObjectSearcher(mgmtScope, new SelectQuery("Win32_EncryptableVolume", condition, selectedProperties));
            System.Management.EnumerationOptions enumOptions = new System.Management.EnumerationOptions();
            enumOptions.EnsureLocatable = true;
            ObjectSearcher.Options = enumOptions;
            return new EncryptableVolumeCollection(ObjectSearcher.Get());
        }

        [Browsable(true)]
        public static EncryptableVolume CreateInstance()
        {
            System.Management.ManagementScope mgmtScope = null;
            if ((statMgmtScope == null))
            {
                mgmtScope = new System.Management.ManagementScope();
                mgmtScope.Path.NamespacePath = CreatedWmiNamespace;
            }
            else
            {
                mgmtScope = statMgmtScope;
            }
            System.Management.ManagementPath mgmtPath = new System.Management.ManagementPath(CreatedClassName);
            System.Management.ManagementClass tmpMgmtClass = new System.Management.ManagementClass(mgmtScope, mgmtPath, null);
            return new EncryptableVolume(tmpMgmtClass.CreateInstance());
        }

        [Browsable(true)]
        public void Delete()
        {
            PrivateLateBoundObject.Delete();
        }

        public uint ClearAllAutoUnlockKeys()
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ClearAllAutoUnlockKeys", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint Decrypt()
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("Decrypt", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint DeleteKeyProtector(string VolumeKeyProtectorID)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("DeleteKeyProtector");
                inParams["VolumeKeyProtectorID"] = ((System.String)(VolumeKeyProtectorID));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("DeleteKeyProtector", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint DeleteKeyProtectors()
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("DeleteKeyProtectors", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint DisableAutoUnlock()
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("DisableAutoUnlock", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint DisableKeyProtectors()
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("DisableKeyProtectors", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint EnableAutoUnlock(string VolumeKeyProtectorID)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("EnableAutoUnlock");
                inParams["VolumeKeyProtectorID"] = ((System.String)(VolumeKeyProtectorID));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("EnableAutoUnlock", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint EnableKeyProtectors()
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("EnableKeyProtectors", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint Encrypt(uint EncryptionMethod)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("Encrypt");
                inParams["EncryptionMethod"] = ((System.UInt32)(EncryptionMethod));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("Encrypt", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint EncryptAfterHardwareTest(uint EncryptionMethod)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("EncryptAfterHardwareTest");
                inParams["EncryptionMethod"] = ((System.UInt32)(EncryptionMethod));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("EncryptAfterHardwareTest", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetConversionStatus(out uint ConversionStatus, out uint EncryptionPercentage)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetConversionStatus", inParams, null);
                ConversionStatus = System.Convert.ToUInt32(outParams.Properties["ConversionStatus"].Value);
                EncryptionPercentage = System.Convert.ToUInt32(outParams.Properties["EncryptionPercentage"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                ConversionStatus = System.Convert.ToUInt32(0);
                EncryptionPercentage = System.Convert.ToUInt32(0);
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetEncryptionMethod(out uint EncryptionMethod)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetEncryptionMethod", inParams, null);
                EncryptionMethod = System.Convert.ToUInt32(outParams.Properties["EncryptionMethod"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                EncryptionMethod = System.Convert.ToUInt32(0);
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetExternalKeyFileName(string VolumeKeyProtectorID, out string FileName)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("GetExternalKeyFileName");
                inParams["VolumeKeyProtectorID"] = ((System.String)(VolumeKeyProtectorID));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetExternalKeyFileName", inParams, null);
                FileName = System.Convert.ToString(outParams.Properties["FileName"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                FileName = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetExternalKeyFromFile(string PathWithFileName, out byte[] ExternalKey)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("GetExternalKeyFromFile");
                inParams["PathWithFileName"] = ((System.String)(PathWithFileName));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetExternalKeyFromFile", inParams, null);
                ExternalKey = ((byte[])(outParams.Properties["ExternalKey"].Value));
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                ExternalKey = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetHardwareTestStatus(out uint TestError, out uint TestStatus)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetHardwareTestStatus", inParams, null);
                TestError = System.Convert.ToUInt32(outParams.Properties["TestError"].Value);
                TestStatus = System.Convert.ToUInt32(outParams.Properties["TestStatus"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                TestError = System.Convert.ToUInt32(0);
                TestStatus = System.Convert.ToUInt32(0);
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetKeyPackage(string VolumeKeyProtectorID, out byte[] KeyPackage)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("GetKeyPackage");
                inParams["VolumeKeyProtectorID"] = ((System.String)(VolumeKeyProtectorID));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetKeyPackage", inParams, null);
                KeyPackage = ((byte[])(outParams.Properties["KeyPackage"].Value));
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                KeyPackage = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetKeyProtectorExternalKey(string VolumeKeyProtectorID, out byte[] ExternalKey)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("GetKeyProtectorExternalKey");
                inParams["VolumeKeyProtectorID"] = ((System.String)(VolumeKeyProtectorID));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetKeyProtectorExternalKey", inParams, null);
                ExternalKey = ((byte[])(outParams.Properties["ExternalKey"].Value));
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                ExternalKey = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetKeyProtectorFriendlyName(string VolumeKeyProtectorID, out string FriendlyName)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("GetKeyProtectorFriendlyName");
                inParams["VolumeKeyProtectorID"] = ((System.String)(VolumeKeyProtectorID));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetKeyProtectorFriendlyName", inParams, null);
                FriendlyName = System.Convert.ToString(outParams.Properties["FriendlyName"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                FriendlyName = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetKeyProtectorNumericalPassword(string VolumeKeyProtectorID, out string NumericalPassword)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("GetKeyProtectorNumericalPassword");
                inParams["VolumeKeyProtectorID"] = ((System.String)(VolumeKeyProtectorID));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetKeyProtectorNumericalPassword", inParams, null);
                NumericalPassword = System.Convert.ToString(outParams.Properties["NumericalPassword"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                NumericalPassword = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetKeyProtectorPlatformValidationProfile(string VolumeKeyProtectorID, out byte[] PlatformValidationProfile)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("GetKeyProtectorPlatformValidationProfile");
                inParams["VolumeKeyProtectorID"] = ((System.String)(VolumeKeyProtectorID));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetKeyProtectorPlatformValidationProfile", inParams, null);
                PlatformValidationProfile = ((byte[])(outParams.Properties["PlatformValidationProfile"].Value));
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                PlatformValidationProfile = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetKeyProtectors(uint KeyProtectorType, out string[] VolumeKeyProtectorID)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("GetKeyProtectors");
                inParams["KeyProtectorType"] = ((System.UInt32)(KeyProtectorType));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetKeyProtectors", inParams, null);
                VolumeKeyProtectorID = ((string[])(outParams.Properties["VolumeKeyProtectorID"].Value));
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                VolumeKeyProtectorID = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetKeyProtectorType(string VolumeKeyProtectorID, out uint KeyProtectorType)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("GetKeyProtectorType");
                inParams["VolumeKeyProtectorID"] = ((System.String)(VolumeKeyProtectorID));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetKeyProtectorType", inParams, null);
                KeyProtectorType = System.Convert.ToUInt32(outParams.Properties["KeyProtectorType"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                KeyProtectorType = System.Convert.ToUInt32(0);
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetLockStatus(out uint LockStatus)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetLockStatus", inParams, null);
                LockStatus = System.Convert.ToUInt32(outParams.Properties["LockStatus"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                LockStatus = System.Convert.ToUInt32(0);
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetProtectionStatus(out uint ProtectionStatus)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetProtectionStatus", inParams, null);
                ProtectionStatus = System.Convert.ToUInt32(outParams.Properties["ProtectionStatus"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                ProtectionStatus = System.Convert.ToUInt32(0);
                return System.Convert.ToUInt32(0);
            }
        }

        public uint IsAutoUnlockEnabled(out bool IsAutoUnlockEnabled, out string VolumeKeyProtectorID)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("IsAutoUnlockEnabled", inParams, null);
                IsAutoUnlockEnabled = System.Convert.ToBoolean(outParams.Properties["IsAutoUnlockEnabled"].Value);
                VolumeKeyProtectorID = System.Convert.ToString(outParams.Properties["VolumeKeyProtectorID"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                IsAutoUnlockEnabled = System.Convert.ToBoolean(0);
                VolumeKeyProtectorID = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint IsAutoUnlockKeyStored(out bool IsAutoUnlockKeyStored)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("IsAutoUnlockKeyStored", inParams, null);
                IsAutoUnlockKeyStored = System.Convert.ToBoolean(outParams.Properties["IsAutoUnlockKeyStored"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                IsAutoUnlockKeyStored = System.Convert.ToBoolean(0);
                return System.Convert.ToUInt32(0);
            }
        }

        public uint IsKeyProtectorAvailable(uint KeyProtectorType, out bool IsKeyProtectorAvailable)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("IsKeyProtectorAvailable");
                inParams["KeyProtectorType"] = ((System.UInt32)(KeyProtectorType));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("IsKeyProtectorAvailable", inParams, null);
                IsKeyProtectorAvailable = System.Convert.ToBoolean(outParams.Properties["IsKeyProtectorAvailable"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                IsKeyProtectorAvailable = System.Convert.ToBoolean(0);
                return System.Convert.ToUInt32(0);
            }
        }

        public static uint IsNumericalPasswordValid(string NumericalPassword, out bool IsNumericalPasswordValid)
        {
            bool IsMethodStatic = true;
            if ((IsMethodStatic == true))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementPath mgmtPath = new System.Management.ManagementPath(CreatedClassName);
                if ((statMgmtScope == null))
                {
                    statMgmtScope = new System.Management.ManagementScope();
                    statMgmtScope.Path.NamespacePath = CreatedWmiNamespace;
                }
                System.Management.ManagementClass classObj = new System.Management.ManagementClass(statMgmtScope, mgmtPath, null);
                inParams = classObj.GetMethodParameters("IsNumericalPasswordValid");
                inParams["NumericalPassword"] = ((System.String)(NumericalPassword));
                System.Management.ManagementBaseObject outParams = classObj.InvokeMethod("IsNumericalPasswordValid", inParams, null);
                IsNumericalPasswordValid = System.Convert.ToBoolean(outParams.Properties["IsNumericalPasswordValid"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                IsNumericalPasswordValid = System.Convert.ToBoolean(0);
                return System.Convert.ToUInt32(0);
            }
        }

        public uint Lock(bool ForceDismount)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("Lock");
                inParams["ForceDismount"] = ((System.Boolean)(ForceDismount));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("Lock", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint PauseConversion()
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("PauseConversion", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint ProtectKeyWithExternalKey(byte[] ExternalKey, string FriendlyName, out string VolumeKeyProtectorID)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("ProtectKeyWithExternalKey");
                inParams["ExternalKey"] = ((byte[])(ExternalKey));
                inParams["FriendlyName"] = ((System.String)(FriendlyName));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ProtectKeyWithExternalKey", inParams, null);
                VolumeKeyProtectorID = System.Convert.ToString(outParams.Properties["VolumeKeyProtectorID"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                VolumeKeyProtectorID = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint ProtectKeyWithNumericalPassword(string FriendlyName, string NumericalPassword, out string VolumeKeyProtectorID)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("ProtectKeyWithNumericalPassword");
                inParams["FriendlyName"] = ((System.String)(FriendlyName));
                inParams["NumericalPassword"] = ((System.String)(NumericalPassword));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ProtectKeyWithNumericalPassword", inParams, null);
                VolumeKeyProtectorID = System.Convert.ToString(outParams.Properties["VolumeKeyProtectorID"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                VolumeKeyProtectorID = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint ProtectKeyWithTPM(string FriendlyName, byte[] PlatformValidationProfile, out string VolumeKeyProtectorID)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("ProtectKeyWithTPM");
                inParams["FriendlyName"] = ((System.String)(FriendlyName));
                inParams["PlatformValidationProfile"] = ((byte[])(PlatformValidationProfile));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ProtectKeyWithTPM", inParams, null);
                VolumeKeyProtectorID = System.Convert.ToString(outParams.Properties["VolumeKeyProtectorID"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                VolumeKeyProtectorID = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint ProtectKeyWithTPMAndPIN(string FriendlyName, string PIN, byte[] PlatformValidationProfile, out string VolumeKeyProtectorID)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("ProtectKeyWithTPMAndPIN");
                inParams["FriendlyName"] = ((System.String)(FriendlyName));
                inParams["PIN"] = ((System.String)(PIN));
                inParams["PlatformValidationProfile"] = ((byte[])(PlatformValidationProfile));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ProtectKeyWithTPMAndPIN", inParams, null);
                VolumeKeyProtectorID = System.Convert.ToString(outParams.Properties["VolumeKeyProtectorID"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                VolumeKeyProtectorID = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint ProtectKeyWithTPMAndStartupKey(byte[] ExternalKey, string FriendlyName, byte[] PlatformValidationProfile, out string VolumeKeyProtectorID)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("ProtectKeyWithTPMAndStartupKey");
                inParams["ExternalKey"] = ((byte[])(ExternalKey));
                inParams["FriendlyName"] = ((System.String)(FriendlyName));
                inParams["PlatformValidationProfile"] = ((byte[])(PlatformValidationProfile));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ProtectKeyWithTPMAndStartupKey", inParams, null);
                VolumeKeyProtectorID = System.Convert.ToString(outParams.Properties["VolumeKeyProtectorID"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                VolumeKeyProtectorID = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint ResumeConversion()
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ResumeConversion", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint SaveExternalKeyToFile(string Path, string VolumeKeyProtectorID)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("SaveExternalKeyToFile");
                inParams["Path"] = ((System.String)(Path));
                inParams["VolumeKeyProtectorID"] = ((System.String)(VolumeKeyProtectorID));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("SaveExternalKeyToFile", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint UnlockWithExternalKey(byte[] ExternalKey)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("UnlockWithExternalKey");
                inParams["ExternalKey"] = ((byte[])(ExternalKey));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("UnlockWithExternalKey", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint UnlockWithNumericalPassword(string NumericalPassword)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("UnlockWithNumericalPassword");
                inParams["NumericalPassword"] = ((System.String)(NumericalPassword));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("UnlockWithNumericalPassword", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        #region Windows 7/Server 2008 R2 Methods
        // TODO: MS documentation on this method not complete as of 3/20/2009.
        // WARNING: Do NOT use this method. It is not ready.
        public uint BackupRecoveryInformationToActiveDirectory(string VolumeKeyProtectorID)
        {
            if (isEmbedded == false)
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("BackupRecoveryInformationToActiveDirectory");
                inParams["VolumeKeyProtectorID"] = ((System.String)(VolumeKeyProtectorID));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("BackupRecoveryInformationToActiveDirectory", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        // TODO: Check to see if outParam property is VolumeKeyProtectorID or NewVolumeKeyProtectorID.
        public uint ChangeExternalKey(string VolumeKeyProtectorID, byte[] ExternalKey, out string NewVolumeKeyProtectorID)
        {
            if (isEmbedded == false)
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("ChangeExternalKey");
                inParams["VolumeKeyProtectorID"] = ((System.String)(VolumeKeyProtectorID));
                inParams["ExternalKey"] = ((byte[])(ExternalKey));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ChangeExternalKey", inParams, null);
                NewVolumeKeyProtectorID = System.Convert.ToString(outParams.Properties["NewVolumeKeyProtectorID"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                NewVolumeKeyProtectorID = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint ChangePassPhrase(string VolumeKeyProtectorID, string NewPassPhrase, out string NewProtectorID)
        {
            if (isEmbedded == false)
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("ChangePassPhrase");
                inParams["VolumeKeyProtectorID"] = ((System.String)(VolumeKeyProtectorID));
                inParams["NewPassPhrase"] = ((System.String)(NewPassPhrase));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ChangePassPhrase", inParams, null);
                NewProtectorID = System.Convert.ToString(outParams.Properties["NewProtectorID"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                NewProtectorID = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint ChangePIN(string VolumeKeyProtectorID, string NewPIN, out string NewVolumeKeyProtectorID)
        {
            if (isEmbedded == false)
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("ChangePIN");
                inParams["VolumeKeyProtectorID"] = ((System.String)(VolumeKeyProtectorID));
                inParams["NewPIN"] = ((System.String)(NewPIN));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ChangePIN", inParams, null);
                NewVolumeKeyProtectorID = System.Convert.ToString(outParams.Properties["NewVolumeKeyProtectorID"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                NewVolumeKeyProtectorID = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetIdentificationField(out string Identifier)
        {
            if (isEmbedded == false)
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetIdentificationField", inParams, null);
                Identifier = System.Convert.ToString(outParams.Properties["Identifier"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                Identifier = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetKeyProtectorCertificate(string VolumeKeyProtectorID, out byte[] PublicKey, out string CertThumbprint, out uint CertType)
        {
            if (isEmbedded == false)
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("GetKeyProtectorCertificate");
                inParams["VolumeKeyProtectorID"] = ((System.String)(VolumeKeyProtectorID));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetKeyProtectorCertificate", inParams, null);
                PublicKey = ((byte[])(outParams.Properties["PublicKey"].Value));
                CertThumbprint = ((System.String)(outParams.Properties["CertThumbprint"].Value));
                CertType = (System.Convert.ToUInt32(outParams.Properties["CertType"].Value));
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                PublicKey = null;
                CertThumbprint = null;
                CertType = System.Convert.ToUInt32(0);
                return System.Convert.ToUInt32(0);
            }
        }

        public uint GetVersion(out uint Version)
        {
            if (isEmbedded == false)
            {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetVersion", inParams, null);
                Version = (System.Convert.ToUInt32(outParams.Properties["Version"].Value));
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                Version = System.Convert.ToUInt32(0);
                return System.Convert.ToUInt32(0);
            }
        }

        public uint PrepareVolume(string DiscoveryVolumeType)
        {
            if (isEmbedded == false)
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("PrepareVolume");
                inParams["DiscoveryVolumeType"] = ((System.String)(DiscoveryVolumeType));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("PrepareVolume", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint ProtectKeyWithCertificateFile(string FriendlyName, string FileName, out string VolumeKeyProtectorID)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("ProtectKeyWithCertificateFile");
                inParams["FriendlyName"] = ((System.String)(FriendlyName));
                inParams["PathWithFileName"] = ((System.String)(FileName));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ProtectKeyWithCertificateFile", inParams, null);
                VolumeKeyProtectorID = System.Convert.ToString(outParams.Properties["VolumeKeyProtectorID"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                VolumeKeyProtectorID = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint ProtectKeyWithCertificateThumbprint(string FriendlyName, string CertThumprint, out string VolumeKeyProtectorID)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("ProtectKeyWithCertificateThumbprint");
                inParams["FriendlyName"] = ((System.String)(FriendlyName));
                inParams["CertThumbprint"] = ((System.String)(CertThumprint));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ProtectKeyWithCertificateThumbprint", inParams, null);
                VolumeKeyProtectorID = System.Convert.ToString(outParams.Properties["VolumeKeyProtectorID"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                VolumeKeyProtectorID = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint ProtectKeyWithPassphrase(string FriendlyName, string Passphrase, out string VolumeKeyProtectorID)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("ProtectKeyWithPassphrase");
                inParams["FriendlyName"] = ((System.String)(FriendlyName));
                inParams["Passphrase"] = ((System.String)(Passphrase));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ProtectKeyWithPassphrase", inParams, null);
                VolumeKeyProtectorID = System.Convert.ToString(outParams.Properties["VolumeKeyProtectorID"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                VolumeKeyProtectorID = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint ProtectKeyWithTPMAndPINAndStartupKey(string FriendlyName, byte[] PlatformValidationProfile, string PIN, byte[] ExternalKey, out string VolumeKeyProtectorID)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("ProtectKeyWithTPMAndStartupKey");
                inParams["FriendlyName"] = ((System.String)(FriendlyName));
                inParams["PlatformValidationProfile"] = ((byte[])(PlatformValidationProfile));
                inParams["PIN"] = ((System.String)(PIN));
                inParams["ExternalKey"] = ((byte[])(ExternalKey));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ProtectKeyWithTPMAndStartupKey", inParams, null);
                VolumeKeyProtectorID = System.Convert.ToString(outParams.Properties["VolumeKeyProtectorID"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                VolumeKeyProtectorID = null;
                return System.Convert.ToUInt32(0);
            }
        }

        public uint SetIdentificationField(string Identifier)
        {
            if (isEmbedded == false)
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("SetIdentificationField");
                inParams["IdentificationField"] = ((System.String)(Identifier));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("SetIdentificationField", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint UnlockWithCertificateFile(string FileName, string PIN)
        {
            if (isEmbedded == false)
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("UnlockWithCertificateFile");
                inParams["FileName"] = ((System.String)(FileName));
                inParams["PIN"] = ((System.String)(PIN));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("UnlockWithCertificateFile", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint UnlockWithCertificateThumbprint(string CertThumbprint, string PIN)
        {
            if (isEmbedded == false)
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("UnlockWithCertificateThumbprint");
                inParams["CertThumbprint"] = ((System.String)(CertThumbprint));
                inParams["PIN"] = ((System.String)(PIN));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("UnlockWithCertificateThumbprint", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint UnlockWithPassphrase(string Passphrase)
        {
            if ((isEmbedded == false))
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("UnlockWithPassphrase");
                inParams["Passphrase"] = ((System.String)(Passphrase));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("UnlockWithPassphrase", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint UpgradeVolume()
        {
            if (isEmbedded == false)
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("UpgradeVolume");
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("UpgradeVolume", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }
        #endregion

        //public uint ProtectKeyWithExternalKey(byte[] ExternalKey, string FriendlyName, out string VolumeKeyProtectorID)
        //{
        //    if ((isEmbedded == false))
        //    {
        //        System.Management.ManagementBaseObject inParams = null;
        //        inParams = PrivateLateBoundObject.GetMethodParameters("ProtectKeyWithExternalKey");
        //        inParams["ExternalKey"] = ((byte[])(ExternalKey));
        //        inParams["FriendlyName"] = ((System.String)(FriendlyName));
        //        System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ProtectKeyWithExternalKey", inParams, null);
        //        VolumeKeyProtectorID = System.Convert.ToString(outParams.Properties["VolumeKeyProtectorID"].Value);
        //        return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        //    }
        //    else
        //    {
        //        VolumeKeyProtectorID = null;
        //        return System.Convert.ToUInt32(0);
        //    }
        //}

        #region Embedded Classes
        // Enumerator implementation for enumerating instances of the class.
        public class EncryptableVolumeCollection : object, ICollection
        {

            private ManagementObjectCollection privColObj;

            public EncryptableVolumeCollection(ManagementObjectCollection objCollection)
            {
                privColObj = objCollection;
            }

            public virtual int Count
            {
                get
                {
                    return privColObj.Count;
                }
            }

            public virtual bool IsSynchronized
            {
                get
                {
                    return privColObj.IsSynchronized;
                }
            }

            public virtual object SyncRoot
            {
                get
                {
                    return this;
                }
            }

            public virtual void CopyTo(System.Array array, int index)
            {
                privColObj.CopyTo(array, index);
                int nCtr;
                for (nCtr = 0; (nCtr < array.Length); nCtr = (nCtr + 1))
                {
                    array.SetValue(new EncryptableVolume(((System.Management.ManagementObject)(array.GetValue(nCtr)))), nCtr);
                }
            }

            public virtual System.Collections.IEnumerator GetEnumerator()
            {
                return new EncryptableVolumeEnumerator(privColObj.GetEnumerator());
            }

            public class EncryptableVolumeEnumerator : object, System.Collections.IEnumerator
            {

                private ManagementObjectCollection.ManagementObjectEnumerator privObjEnum;

                public EncryptableVolumeEnumerator(ManagementObjectCollection.ManagementObjectEnumerator objEnum)
                {
                    privObjEnum = objEnum;
                }

                public virtual object Current
                {
                    get
                    {
                        return new EncryptableVolume(((System.Management.ManagementObject)(privObjEnum.Current)));
                    }
                }

                public virtual bool MoveNext()
                {
                    return privObjEnum.MoveNext();
                }

                public virtual void Reset()
                {
                    privObjEnum.Reset();
                }
            }
        }

        // TypeConverter to handle null values for ValueType properties
        public class WMIValueTypeConverter : TypeConverter
        {

            private TypeConverter baseConverter;

            private System.Type baseType;

            public WMIValueTypeConverter(System.Type inBaseType)
            {
                baseConverter = TypeDescriptor.GetConverter(inBaseType);
                baseType = inBaseType;
            }

            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type srcType)
            {
                return baseConverter.CanConvertFrom(context, srcType);
            }

            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType)
            {
                return baseConverter.CanConvertTo(context, destinationType);
            }

            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                return baseConverter.ConvertFrom(context, culture, value);
            }

            public override object CreateInstance(System.ComponentModel.ITypeDescriptorContext context, System.Collections.IDictionary dictionary)
            {
                return baseConverter.CreateInstance(context, dictionary);
            }

            public override bool GetCreateInstanceSupported(System.ComponentModel.ITypeDescriptorContext context)
            {
                return baseConverter.GetCreateInstanceSupported(context);
            }

            public override PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext context, object value, System.Attribute[] attributeVar)
            {
                return baseConverter.GetProperties(context, value, attributeVar);
            }

            public override bool GetPropertiesSupported(System.ComponentModel.ITypeDescriptorContext context)
            {
                return baseConverter.GetPropertiesSupported(context);
            }

            public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
            {
                return baseConverter.GetStandardValues(context);
            }

            public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context)
            {
                return baseConverter.GetStandardValuesExclusive(context);
            }

            public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
            {
                return baseConverter.GetStandardValuesSupported(context);
            }

            public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType)
            {
                if ((baseType.BaseType == typeof(System.Enum)))
                {
                    if ((value.GetType() == destinationType))
                    {
                        return value;
                    }
                    if ((((value == null)
                                && (context != null))
                                && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false)))
                    {
                        return "NULL_ENUM_VALUE";
                    }
                    return baseConverter.ConvertTo(context, culture, value, destinationType);
                }
                if (((baseType == typeof(bool))
                            && (baseType.BaseType == typeof(System.ValueType))))
                {
                    if ((((value == null)
                                && (context != null))
                                && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false)))
                    {
                        return "";
                    }
                    return baseConverter.ConvertTo(context, culture, value, destinationType);
                }
                if (((context != null)
                            && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false)))
                {
                    return "";
                }
                return baseConverter.ConvertTo(context, culture, value, destinationType);
            }
        }

        // Embedded class to represent WMI system Properties.
        [TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
        public class ManagementSystemProperties
        {

            private System.Management.ManagementBaseObject PrivateLateBoundObject;

            public ManagementSystemProperties(System.Management.ManagementBaseObject ManagedObject)
            {
                PrivateLateBoundObject = ManagedObject;
            }

            [Browsable(true)]
            public int GENUS
            {
                get
                {
                    return ((int)(PrivateLateBoundObject["__GENUS"]));
                }
            }

            [Browsable(true)]
            public string CLASS
            {
                get
                {
                    return ((string)(PrivateLateBoundObject["__CLASS"]));
                }
            }

            [Browsable(true)]
            public string SUPERCLASS
            {
                get
                {
                    return ((string)(PrivateLateBoundObject["__SUPERCLASS"]));
                }
            }

            [Browsable(true)]
            public string DYNASTY
            {
                get
                {
                    return ((string)(PrivateLateBoundObject["__DYNASTY"]));
                }
            }

            [Browsable(true)]
            public string RELPATH
            {
                get
                {
                    return ((string)(PrivateLateBoundObject["__RELPATH"]));
                }
            }

            [Browsable(true)]
            public int PROPERTY_COUNT
            {
                get
                {
                    return ((int)(PrivateLateBoundObject["__PROPERTY_COUNT"]));
                }
            }

            [Browsable(true)]
            public string[] DERIVATION
            {
                get
                {
                    return ((string[])(PrivateLateBoundObject["__DERIVATION"]));
                }
            }

            [Browsable(true)]
            public string SERVER
            {
                get
                {
                    return ((string)(PrivateLateBoundObject["__SERVER"]));
                }
            }

            [Browsable(true)]
            public string NAMESPACE
            {
                get
                {
                    return ((string)(PrivateLateBoundObject["__NAMESPACE"]));
                }
            }

            [Browsable(true)]
            public string PATH
            {
                get
                {
                    return ((string)(PrivateLateBoundObject["__PATH"]));
                }
            }
        }
        #endregion
    }
}
