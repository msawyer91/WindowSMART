using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

using aejw.Network;
using Chilkat;
using DojoNorthSoftware.Pushover;
using Growl.Connector;
using Gurock.SmartInspect;
using NMALib;
using Prowl;
using RestSharp;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public class HssServiceHelper
    {
        private Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
        private Microsoft.Win32.RegistryKey dojoNorthSubKey;
        private Microsoft.Win32.RegistryKey monitoredDisksKey;
        private Microsoft.Win32.RegistryKey configurationKey;

        private int pollingInterval;
        private String temperaturePreference;
        private bool reportCritical;
        private bool reportWarnings;
        private bool reportGeriatric;
        private bool ignoreOverheated;
        private bool preserveOnUninstall;
        private bool ignoreHot;
        private bool ignoreWarm;
        private bool fallbackToWmi;
        private bool debugMode;
        private bool advancedSii;
        private int criticalTemperatureThreshold;
        private int overheatedTemperatureThreshold;
        private int hotTemperatureThreshold;
        private int warmTemperatureThreshold;
        private FeverishDisk feverishDisks;
        private ServiceHelper.DailySummary summary;
        //private Wmi.WmiData wmiAlerts;

        private Mexi_Sexi.MexiSexi mexiSexi;

        // Email Configuration ~ Jake Scherer ~
        private String mailServer;
        private int serverPort;
        private String senderFriendlyName;
        private String senderEmailAddress;
        private String recipientFriendlyName;
        private String recipientEmailAddress;
        private String mailUser;
        private String mailPassword;
        private String recipientEmail2;
        private String recipientEmail3;
        private bool isMailConfigValid;
        private bool authenticationEnabled;
        private bool mailAlertsEnabled;
        private bool useSsl;
        private bool sendDailySummary;
        private bool sendPlaintext;

        // BitLocker configuration
        private bool bitLockerShowAllDrives;
        private bool bitLockerPreventLocking;
        private bool bitLockerHideTab;

        // Helper Service ~ Dan Garland ~
        private bool useHelperService;
        private String helperServiceDiskPoolProvider;
        private bool ignoreVirtualDisks;

        // SSD thresholds
        private int ssdLifeLeftCritical;
        private int ssdLifeLeftWarning;
        private int ssdRetirementCritical;
        private int ssdRetirementWarning;

        // Notifications
        private bool isProwlEnabled;
        private bool isNmaEnabled;
        private bool isGrowlEnabled;
        private bool isGrowlRemoteEnabled;
        private bool isSnarlEnabled;
        private bool isTwitterEnabled;
        private bool isToastyEnabled;
        private String prowlApiKey;
        private String nmaApiKey;
        private String growlRemoteComputer;
        private String growlPassword;
        private String toastyApiKey;
        private int growlPort;
        private bool isPushoverEnabled;
        private String pushoverUserKey;
        private String pushoverClearedSound;
        private String pushoverCriticalSound;
        private String pushoverWarningSound;
        private String pushoverDeviceTarget;
        private const String PUSHOVER_DEFAULT_SOUND = "(Device default sound)";

        // Interface connectors for Growl and Snarl
        private GrowlConnector growl;
        private Utilities.GrowlNotificationTypes gnt;
        private GrowlConnector snarl;
        private Utilities.GrowlNotificationTypes snt;

        // WSS?
        private bool isWindowsServerSolutions;

        // Skinning
        private bool useDefaultSkinning;
        private int windowBackground;

        // System Notifications
        private bool notifyOnPowerChange;
        private bool notifyOnFilthyShutdown;

        // Logging
        private String debugLogLocation;

        // Custom notification
        private String customNotificate;

        // Is Drive Bender installed? ~ Dan Garland ~
        // We set this flag at service startup and don't check again. DB, when installed, requires a reboot.
        // So DB is either here or not, and we don't keep checking for it.
        private bool isDriveBenderInstalled;

        // Crypto engine ~ Jake Scherer ~
        private Components.Cryptography.DoubleEncryptor crypto;

        // GPO
        private int gpoDpa;
        private int gpoTempCtl;
        private int gpoVirtualIgnore;
        private int gpoAllowIgnoredItems;
        private int gpoEmailNotificate;
        private int gpoProwlNotificate;
        private int gpoNmaNotificate;
        private int gpoWpNotificate;
        private int gpoGrowlNotificate;
        private int gpoSnarlNotificate;
        private int gpoPushoverNotificate;
        private int gpoAdvancedSettings;
        private int gpoDebuggingControl;
        private int gpoSSD;
        private int gpoCheckForUpdates;
        private int gpoUiTheme;
        private int gpoUseSupportMessage;

        // Emergency Ops
        private bool performEmergencyBackup;
        private bool noHotBackup;
        private bool backupLocal;
        private String localBackupPath;
        private String uncBackupPath;
        private String uncBackupUser;
        private String uncBackupPassword;
        private bool performThermalShutdown;
        private bool performCustomBackup;
        private bool doDebugBackup;
        private bool doDebugCustomBackup;
        private String debugBadDisks;
        private System.Threading.Thread backupThread;
        private System.Threading.Thread customBackupThread;
        private String customBackupProgram;
        private String customBackupArgs;

        // XML file path
        private String xmlFilePath;
        private String xmlAlertFileName;
        private String xmlAlertFile;
        private String xmlSummaryFileName;
        private String xmlSummaryFile;

        // Update tracking for XML
        private bool isUpdateAvailable;
        private String updateVersion;
        private String updateDate;
        private String updateUrl;

        private HssServiceHelper()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.HssServiceHelper");

            SiAuto.Main.LogMessage("Determining Windows SKU.");
            isWindowsServerSolutions = Components.OperatingSystem.IsWindowsServerSolutionsProduct(this);
            SiAuto.Main.LogBool("isWindowsServerSolutions", isWindowsServerSolutions);

            // Initialize the Windows Server Solutions subsystem. ~ Jake ~
            try
            {
                if (isWindowsServerSolutions)
                {
                    SiAuto.Main.LogMessage("Attempting to initialize the Windows Server Solutions Environment.");
                    InitializeWssEnvironment();
                    SiAuto.Main.LogMessage("The Windows Server Solutions Environment has been initialized.");
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogWarning("The Windows Server Solutions Environment was not initialized. If this computer is not a WSS operating system or connected to a WSS Server, " +
                    "you can safely ignore this warning and the exception stack trace immediately following it.");
                SiAuto.Main.LogException(ex);
            }

            // Check for Windows XP and make Registry update to allow SHA256CryptoServiceProvider to be used.
            SiAuto.Main.LogMessage("Checking for Windows XP.");
            if (LegacyOs.IsWindowsXp())
            {
                SiAuto.Main.LogMessage("OS is Windows XP. Checking Registry for support of SHA256CryptoServiceProvider.");
                if (LegacyOs.IsXpSha256CryptoSupported())
                {
                    SiAuto.Main.LogMessage("Necessary Registry entry already exists; no action required.");
                }
                else
                {
                    SiAuto.Main.LogWarning("SHA256CryptoServiceProvider support not configured in the Registry; enabling it now.");
                    LegacyOs.EnableXpSha256CryptoSupport();
                }
            }
            else
            {
                SiAuto.Main.LogMessage("OS is NOT Windows XP.");
            }

            // Initialize the cryptography engine. ~ Jake - copied from SettingsControl.cs ~
            SiAuto.Main.LogMessage("Initialize the cryptography engine.");
            crypto = new Components.Cryptography.DoubleEncryptor("T@rynPr3ttyL1ttl3Sl0bberh3@d",
                "J0shuaFl@l3ssB@byH3ad");
            crypto.SetCbcRequired("BaronessOfBeauty");
            SiAuto.Main.LogMessage("Cryptography engine is initialized.");

            SiAuto.Main.LogMessage("Acquire Registry objects.");
            dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

            monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);
            configurationKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryConfigurationKey);
            SiAuto.Main.LogMessage("Acquired Registry objects.");

            SiAuto.Main.LogMessage("Set default in-memory configuration.");
            SetDefaultConfiguration();
            SiAuto.Main.LogMessage("Load configuration from the Registry. If this is the first time HSS has been run, the default configuration will be applied.");
            SiAuto.Main.LogMessage("If the configuration is defined but values are corrupt or missing, those individual values will be reset to their defaults.");
            LoadConfiguration();
            SiAuto.Main.LogMessage("Close Registry objects.");
            monitoredDisksKey.Close();

            SiAuto.Main.LogMessage("Initialize the FeverishDisk object.");
            feverishDisks = new FeverishDisk();

            SiAuto.Main.LogMessage("Checking for the presence of disk pooling software (i.e. Drive Bender, StableBit DrivePool).");
            isDriveBenderInstalled = BitLocker.UtilityMethods.IsDriveBenderInstalled();
            SiAuto.Main.LogBool("isDriveBenderInstalled", isDriveBenderInstalled);

            SiAuto.Main.LogMessage("Getting executing assembly location.");
            xmlFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            SiAuto.Main.LogString("Got executing assembly location.", xmlFilePath);

            xmlAlertFileName = Properties.Resources.ActiveAlertsXml;
            SiAuto.Main.LogString("xmlAlertFileName", xmlAlertFileName);

            xmlAlertFile = xmlFilePath + "\\" + xmlAlertFileName;
            SiAuto.Main.LogString("xmlAlertFile", xmlAlertFile);

            xmlSummaryFileName = Properties.Resources.DailySummaryXml;
            SiAuto.Main.LogString("xmlSummaryFileName", xmlSummaryFileName);

            xmlSummaryFile = xmlFilePath + "\\" + xmlSummaryFileName;
            SiAuto.Main.LogString("xmlSummaryFile", xmlSummaryFile);

            // Update tracking for Tray app.
            isUpdateAvailable = false;
            updateDate = String.Empty;
            updateUrl = String.Empty;
            updateVersion = String.Empty;

            //try
            //{
            //    SiAuto.Main.LogMessage("[Service Initialization Initial Check] Initializing the WMI namespace.");
            //    wmiAlerts = new Wmi.WmiData();
            //    wmiAlerts.AlertCount = 0;
            //    wmiAlerts.InstalledVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //    wmiAlerts.ActiveAlerts = null;
            //    SiAuto.Main.LogMessage("[Service Initialization Initial Check] Publishing the WMI namespace.");
            //    Instrumentation.Publish(wmiAlerts);
            //    SiAuto.Main.LogMessage("[Service Initialization Initial Check] Published the WMI namespace.");
            //}
            //catch (Exception ex)
            //{
            //    SiAuto.Main.LogError("Failed to publish WMI namespace: " + ex.Message);
            //    SiAuto.Main.LogException(ex);
            //}

            // Filthy shutdown cheque.
            if (notifyOnFilthyShutdown && WasLastShutdownFilthy())
            {
                WindowsEventLogger.LogWarning("The system has recovered from an unclean shutdown. This can occur if Windows encountered a Stop error (BSOD) or if the computer " +
                    "was powered down or rebooted without being gracefully shut down. In rare cases this could be if the " + (isWindowsServerSolutions ?
                    Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart) + " service crashed. If this message is unexpected, " +
                    "you may want to check the computer to determine the cause of the problem.", 53871, Properties.Resources.EventLogTaryn);
                PostSpecialNotificate("The system has recovered from an unclean shutdown. This can occur if Windows encountered a Stop error (BSOD) or if the computer " +
                    "was powered down or rebooted without being gracefully shut down. In rare cases this could be if the " + (isWindowsServerSolutions ?
                    Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart) + " service crashed. If this message is unexpected, " +
                    "you may want to check the computer to determine the cause of the problem. Computer: " + System.Environment.MachineName, "Unclean Shutdown Detected", false,
                    "Windows", 65533);
            }

            try
            {
                SiAuto.Main.LogMessage("[Service Initialization Initial Check] Remove active alerts XML file if it exists.");
                if (System.IO.File.Exists(xmlAlertFile))
                {
                    try
                    {
                        System.IO.File.Delete(xmlAlertFile);
                        SiAuto.Main.LogMessage("Active alerts file successfully deleted.");
                    }
                    catch (Exception ex)
                    {
                        SiAuto.Main.LogError("Failed to delete active alerts file: " + ex.Message);
                        SiAuto.Main.LogException(ex);
                    }
                }
                else
                {
                    SiAuto.Main.LogMessage("Nothing to delete.");
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogFatal("Exceptions were detected executing the HssServiceHelper constructor. " + ex.Message);
                SiAuto.Main.LogException(ex);
                WindowsEventLogger.LogError("HssServiceHelper constructor failed: " + ex.Message +
                    "\n\n" + ex.StackTrace, 53882, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
            }
            finally
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.HssServiceHelper Constructor");
            }
        }

        private void SetDefaultConfiguration()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SetDefaultConfiguration");
            // Temperature preference is assumed to be Celsius (C) until otherwise loaded.
            temperaturePreference = "C";

            // Set default temperatures.
            criticalTemperatureThreshold = 65;
            overheatedTemperatureThreshold = 55;
            hotTemperatureThreshold = 50;
            warmTemperatureThreshold = 42;

            // Set alternate temps all to zero for now; we'll reset them when we get the Celsius data.
            //tempCriticalF = tempCriticalK = tempHotF = tempHotK = tempOverheatedF = tempOverheatedK = tempWarmF = tempWarmK = 0;

            // Do not ignore hot or warm, unless changed by user.
            ignoreOverheated = false;
            ignoreHot = false;
            ignoreWarm = false;

            // Preserve settings on uninstall.
            preserveOnUninstall = true;

            // New in service-based version
            pollingInterval = 900000;               // 15 minutes (in milliseconds)
            reportCritical = true;                  // criticals reported
            reportWarnings = true;                  // warnings reported
            reportGeriatric = true;                 // geriatrics reported

            // Debugging/troubleshooting options.
            fallbackToWmi = true;
            debugMode = false;
            advancedSii = false;

            // Default BitLocker
            bitLockerShowAllDrives = true;
            bitLockerPreventLocking = false;
            bitLockerHideTab = false;

            // Default Email Configuration ~ Jake Scherer ~
            mailServer = String.Empty;
            serverPort = 25;
            senderFriendlyName = String.Empty;
            senderEmailAddress = String.Empty;
            recipientFriendlyName = String.Empty;
            recipientEmailAddress = String.Empty;
            recipientEmail2 = String.Empty;
            recipientEmail3 = String.Empty;
            authenticationEnabled = false;
            mailUser = "Undefined";
            mailPassword = "N0t_Sp3c1fied";
            mailAlertsEnabled = false;
            useSsl = false;
            sendDailySummary = true;
            sendPlaintext = false;
            isMailConfigValid = false;

            // Default for Use Helper Service ~ Dan Garland ~
            useHelperService = false;
            helperServiceDiskPoolProvider = String.Empty;
            ignoreVirtualDisks = true;

            // SSD thresholds
            ssdLifeLeftCritical = 10;
            ssdLifeLeftWarning = 30;
            ssdRetirementCritical = 150;
            ssdRetirementWarning = 50;

            // Notifications
            isProwlEnabled = false;
            isNmaEnabled = false;
            isGrowlEnabled = false;
            isGrowlRemoteEnabled = false;
            isSnarlEnabled = false;
            isPushoverEnabled = false;
            isTwitterEnabled = false;
            isToastyEnabled = false;
            prowlApiKey = String.Empty;
            nmaApiKey = String.Empty;
            growlRemoteComputer = String.Empty;
            growlPassword = "N@yNayDefined";
            growlPort = 23053;
            toastyApiKey = String.Empty;
            pushoverUserKey = String.Empty;
            pushoverClearedSound = PUSHOVER_DEFAULT_SOUND;
            pushoverCriticalSound = PUSHOVER_DEFAULT_SOUND;
            pushoverWarningSound = PUSHOVER_DEFAULT_SOUND;
            pushoverDeviceTarget = String.Empty;

            // Skinning
            useDefaultSkinning = true;
            windowBackground = 0;

            // System Options
            notifyOnPowerChange = true;
            notifyOnFilthyShutdown = true;
            
            // Emergency Ops
            performEmergencyBackup = false;
            noHotBackup = true;
            backupLocal = true;
            localBackupPath = String.Empty;
            uncBackupPath = String.Empty;
            uncBackupUser = String.Empty;
            uncBackupPassword = "F1ng3rL1ckin";
            performThermalShutdown = false;
            performCustomBackup = false;
            doDebugBackup = false;
            doDebugCustomBackup = false;
            customBackupProgram = String.Empty;
            customBackupArgs = String.Empty;
            debugBadDisks = String.Empty;

            // Logging
            debugLogLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Dojo North Software\\Logs";

            // Custom notificate
            customNotificate = String.Empty;

            SiAuto.Main.LogMessage("=== Default Configuraion Values ===");
            SiAuto.Main.LogString("temperaturePreference", temperaturePreference);
            SiAuto.Main.LogInt("criticalTemperatureThreshold", criticalTemperatureThreshold);
            SiAuto.Main.LogInt("overheatedTemperatureThreshold", overheatedTemperatureThreshold);
            SiAuto.Main.LogInt("hotTemperatureThreshold", hotTemperatureThreshold);
            SiAuto.Main.LogInt("warmTemperatureThreshold", warmTemperatureThreshold);
            SiAuto.Main.LogBool("ignoreOverheated", ignoreOverheated);
            SiAuto.Main.LogBool("ignoreHot", ignoreHot);
            SiAuto.Main.LogBool("ignoreWarm", ignoreWarm);
            SiAuto.Main.LogBool("ignoreOverheated", ignoreVirtualDisks);
            SiAuto.Main.LogBool("preserveOnUninstall", preserveOnUninstall);
            SiAuto.Main.LogBool("reportCritical", reportCritical);
            SiAuto.Main.LogBool("reportWarnings", reportWarnings);
            SiAuto.Main.LogBool("reportGeriatric", reportGeriatric);
            SiAuto.Main.LogBool("fallbackToWmi", fallbackToWmi);
            SiAuto.Main.LogBool("debugMode", debugMode);
            SiAuto.Main.LogBool("advancedSii", advancedSii);
            SiAuto.Main.LogBool("bitLockerHideTab", bitLockerHideTab);
            SiAuto.Main.LogBool("bitLockerPreventLocking", bitLockerPreventLocking);
            SiAuto.Main.LogBool("bitLockerShowAllDrives", bitLockerShowAllDrives);
            SiAuto.Main.LogBool("useHelperService", useHelperService);
            SiAuto.Main.LogString("helperServiceDiskPoolProvider", helperServiceDiskPoolProvider);
            SiAuto.Main.LogInt("pollingInterval", pollingInterval);

            SiAuto.Main.LogString("mailServer", mailServer);
            SiAuto.Main.LogString("senderEmailAddress", senderEmailAddress);
            SiAuto.Main.LogString("senderFriendlyName", senderFriendlyName);
            SiAuto.Main.LogString("recipientEmailAddress", recipientEmailAddress);
            SiAuto.Main.LogString("recipientFriendlyName", recipientFriendlyName);
            SiAuto.Main.LogString("recipientEmail2", recipientEmail2);
            SiAuto.Main.LogString("recipientEmail3", recipientEmail3);
            SiAuto.Main.LogMessage("Username and Password are not logged.");
            SiAuto.Main.LogBool("authenticationEnabled", authenticationEnabled);
            SiAuto.Main.LogInt("serverPort", serverPort);
            SiAuto.Main.LogBool("mailAlertsEnabled", mailAlertsEnabled);
            SiAuto.Main.LogBool("useSsl", useSsl);
            SiAuto.Main.LogBool("sendDailySummary", sendDailySummary);
            SiAuto.Main.LogBool("sendPlaintext", sendPlaintext);
            SiAuto.Main.LogBool("isMailConfigValid", isMailConfigValid);

            // SSD thresholds
            SiAuto.Main.LogInt("ssdLifeLeftCritical", ssdLifeLeftCritical);
            SiAuto.Main.LogInt("ssdLifeLeftWarning", ssdLifeLeftWarning);
            SiAuto.Main.LogInt("ssdRetirementCritical", ssdRetirementCritical);
            SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);

            // Notifications
            SiAuto.Main.LogBool("isProwlEnabled", isProwlEnabled);
            SiAuto.Main.LogBool("isNmaEnabled", isNmaEnabled);
            SiAuto.Main.LogBool("isGrowlEnabled", isGrowlEnabled);
            SiAuto.Main.LogBool("isGrowlRemoteEnabled", isGrowlRemoteEnabled);
            SiAuto.Main.LogBool("isSnarlEnabled", isSnarlEnabled);
            SiAuto.Main.LogBool("isPushoverEnabled", isPushoverEnabled);
            SiAuto.Main.LogBool("isTwitterEnabled", isTwitterEnabled);
            SiAuto.Main.LogBool("isToastyEnabled", isToastyEnabled);
            SiAuto.Main.LogString("prowlApiKey", prowlApiKey);
            SiAuto.Main.LogString("nmaApiKey", nmaApiKey);
            SiAuto.Main.LogString("growlRemoteComputer", growlRemoteComputer);
            SiAuto.Main.LogInt("growlPort", growlPort);
            SiAuto.Main.LogString("toastyApiKey", toastyApiKey);
            SiAuto.Main.LogString("pushoverUserKey", pushoverUserKey);
            SiAuto.Main.LogString("pushoverClearedSound", pushoverClearedSound);
            SiAuto.Main.LogString("pushoverCriticalSound", pushoverCriticalSound);
            SiAuto.Main.LogString("pushoverWarningSound", pushoverWarningSound);
            SiAuto.Main.LogString("pushoverDeviceTarget", pushoverDeviceTarget);

            // Skinning
            SiAuto.Main.LogBool("useDefaultSkinning", useDefaultSkinning);
            SiAuto.Main.LogInt("windowBackground", windowBackground);

            // System Notifications
            SiAuto.Main.LogBool("notifyOnFilthyShutdown", notifyOnFilthyShutdown);
            SiAuto.Main.LogBool("notifyOnPowerChange", notifyOnPowerChange);

            SiAuto.Main.LogString("debugLogLocation", debugLogLocation);

            SiAuto.Main.LogString("customNotificate", customNotificate);
            SiAuto.Main.LogMessage("=== End of Default Configuraion Values ===");

            SiAuto.Main.LogMessage("=== Setting default GPO configuration state; all items set to 2 (not configured) ===");
            gpoDpa = 2;
            gpoTempCtl = 2;
            gpoVirtualIgnore = 2;
            gpoAllowIgnoredItems = 2;
            gpoEmailNotificate = 2;
            gpoProwlNotificate = 2;
            gpoNmaNotificate = 2;
            gpoWpNotificate = 2;
            gpoGrowlNotificate = 2;
            gpoSnarlNotificate = 2;
            gpoPushoverNotificate = 2;
            gpoAdvancedSettings = 2;
            gpoDebuggingControl = 2;
            gpoSSD = 2;
            gpoCheckForUpdates = 2;
            gpoUiTheme = 2;
            gpoUseSupportMessage = 2;
            SiAuto.Main.LogInt("gpoDpa", gpoDpa);
            SiAuto.Main.LogInt("gpoTempCtl", gpoTempCtl);
            SiAuto.Main.LogInt("gpoVirtualIgnore", gpoVirtualIgnore);
            SiAuto.Main.LogInt("gpoAllowIgnoredItems", gpoAllowIgnoredItems);
            SiAuto.Main.LogInt("gpoEmailNotificate", gpoEmailNotificate);
            SiAuto.Main.LogInt("gpoProwlNotificate", gpoProwlNotificate);
            SiAuto.Main.LogInt("gpoNmaNotificate", gpoNmaNotificate);
            SiAuto.Main.LogInt("gpoWpNotificate", gpoWpNotificate);
            SiAuto.Main.LogInt("gpoPushoverNotificate", gpoPushoverNotificate);
            SiAuto.Main.LogInt("gpoGrowlNotificate", gpoGrowlNotificate);
            SiAuto.Main.LogInt("gpoSnarlNotificate", gpoSnarlNotificate);
            SiAuto.Main.LogInt("gpoAdvancedSettings", gpoAdvancedSettings);
            SiAuto.Main.LogInt("gpoDebuggingControl", gpoDebuggingControl);
            SiAuto.Main.LogInt("gpoSSD", gpoSSD);
            SiAuto.Main.LogInt("gpoCheckForUpdates", gpoCheckForUpdates);
            SiAuto.Main.LogInt("gpoUiTheme", gpoUiTheme);
            SiAuto.Main.LogInt("gpoUseSupportMessage", gpoUseSupportMessage);
            SiAuto.Main.LogMessage("=== End setting default GPO configuration state; all items set to 2 (not configured) ===");

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SetDefaultConfiguration");
        }

        private void LoadConfiguration()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.LoadConfiguration");
            bool exceptionsDetected = false;

            try
            {
                temperaturePreference = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigTemperaturePreference);
                if (String.IsNullOrEmpty(temperaturePreference))
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigTemperaturePreference, "C");
                    exceptionsDetected = true;
                    SiAuto.Main.LogWarning("Temperature Preference was undefined or defined value was corrupt; it has been reset to default.");
                }
            }
            catch
            {
                SiAuto.Main.LogWarning("Temperature Preference was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigTemperaturePreference, temperaturePreference);
                exceptionsDetected = true;
            }

            try
            {
                ignoreHot = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIgnoreHot));
            }
            catch
            {
                SiAuto.Main.LogWarning("Ignore Hot was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIgnoreHot, ignoreHot);
                exceptionsDetected = true;
            }

            try
            {
                ignoreWarm = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIgnoreWarm));
            }
            catch
            {
                SiAuto.Main.LogWarning("Ignore Warm was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIgnoreWarm, ignoreWarm);
                exceptionsDetected = true;
            }

            try
            {
                preserveOnUninstall = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigPreserveOnUninstall));
            }
            catch
            {
                preserveOnUninstall = true;
                SiAuto.Main.LogWarning("Preserve on Uninstall was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigPreserveOnUninstall, preserveOnUninstall);
                exceptionsDetected = true;
            }

            try
            {
                criticalTemperatureThreshold = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigCriticalTempThreshold);
            }
            catch
            {
                SiAuto.Main.LogWarning("Critical Temperature Threshold was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigCriticalTempThreshold, criticalTemperatureThreshold);
                exceptionsDetected = true;
            }

            try
            {
                overheatedTemperatureThreshold = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigOverheatedTempThreshold);
            }
            catch
            {
                SiAuto.Main.LogWarning("Overheated Temperature Threshold was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigOverheatedTempThreshold, overheatedTemperatureThreshold);
                exceptionsDetected = true;
            }

            try
            {
                hotTemperatureThreshold = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigHotTempThreshold);
            }
            catch
            {
                SiAuto.Main.LogWarning("Hot Temperature Threshold was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigHotTempThreshold, hotTemperatureThreshold);
                exceptionsDetected = true;
            }

            try
            {
                warmTemperatureThreshold = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigWarmTempThreshold);
            }
            catch
            {
                SiAuto.Main.LogWarning("Warm Temperature Threshold was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigWarmTempThreshold, warmTemperatureThreshold);
                exceptionsDetected = true;
            }

            try
            {
                pollingInterval = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigPollingInterval);
            }
            catch
            {
                SiAuto.Main.LogWarning("Polling Interval was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigPollingInterval, pollingInterval);
                exceptionsDetected = true;
            }

            try
            {
                reportCritical = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigReportCritical));
            }
            catch
            {
                SiAuto.Main.LogWarning("Report Critical was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigReportCritical, reportCritical);
                exceptionsDetected = true;
            }

            try
            {
                reportWarnings = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigReportWarning));
            }
            catch
            {
                SiAuto.Main.LogWarning("Report Warning was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigReportWarning, reportWarnings);
                exceptionsDetected = true;
            }

            try
            {
                reportGeriatric = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigReportGeriatric));
            }
            catch
            {
                SiAuto.Main.LogWarning("Report Geriatric was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigReportGeriatric, reportGeriatric);
                exceptionsDetected = true;
            }

            try
            {
                fallbackToWmi = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigFallbackToWmi));
            }
            catch
            {
                SiAuto.Main.LogWarning("Fallback to WMI was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigFallbackToWmi, fallbackToWmi);
                exceptionsDetected = true;
            }

            try
            {
                debugMode = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigEnableDebugLogging));
            }
            catch
            {
                SiAuto.Main.LogWarning("Debug Mode was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigEnableDebugLogging, debugMode);
                exceptionsDetected = true;
            }

            try
            {
                advancedSii = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigSiIAdvanced));
            }
            catch
            {
                SiAuto.Main.LogWarning("SiI Advanced was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigSiIAdvanced, advancedSii);
                exceptionsDetected = true;
            }

            // BitLocker configuration
            try
            {
                bitLockerShowAllDrives = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigBitLockerShowAllDrives));
            }
            catch
            {
                SiAuto.Main.LogWarning("BitLocker Show All Drives was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigBitLockerShowAllDrives, bitLockerShowAllDrives);
                exceptionsDetected = true;
            }

            try
            {
                bitLockerPreventLocking = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigBitLockerPreventLocking));
            }
            catch
            {
                SiAuto.Main.LogWarning("BitLocker Prevent Locking was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigBitLockerPreventLocking, bitLockerPreventLocking);
                exceptionsDetected = true;
            }

            try
            {
                bitLockerHideTab = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigBitLockerHideTab));
            }
            catch
            {
                SiAuto.Main.LogWarning("Hide BitLocker Tab was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigBitLockerHideTab, bitLockerHideTab);
                exceptionsDetected = true;
            }

            // Email Configuration ~ Jake Scherer ~
            try
            {
                isMailConfigValid = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsEmailConfigValid));
            }
            catch
            {
                SiAuto.Main.LogWarning("Is Email Config Valid was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
                exceptionsDetected = true;
            }

            try
            {
                mailServer = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailServer);
            }
            catch
            {
                SiAuto.Main.LogWarning("Mail Server was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailServer, mailServer);
                exceptionsDetected = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                serverPort = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigServerPort);
            }
            catch
            {
                SiAuto.Main.LogWarning("Server Port was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigServerPort, 25);
                serverPort = 25;
                exceptionsDetected = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                senderFriendlyName = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigSenderFriendly);
            }
            catch
            {
                SiAuto.Main.LogWarning("Sender Friendly was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigSenderFriendly, senderFriendlyName);
                exceptionsDetected = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                senderEmailAddress = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigSenderEmail);
            }
            catch
            {
                SiAuto.Main.LogWarning("Sender Email was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigSenderEmail, senderEmailAddress);
                exceptionsDetected = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                recipientFriendlyName = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientFriendly);
            }
            catch
            {
                SiAuto.Main.LogWarning("Recipient Friendly was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientFriendly, recipientFriendlyName);
                exceptionsDetected = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                recipientEmailAddress = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientEmail);
            }
            catch
            {
                SiAuto.Main.LogWarning("Recipient Email was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail, recipientEmailAddress);
                exceptionsDetected = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                recipientEmail2 = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientEmail2);
            }
            catch
            {
                SiAuto.Main.LogWarning("Recipient Email 2 was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail2, recipientEmail2);
                exceptionsDetected = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                recipientEmail3 = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientEmail3);
            }
            catch
            {
                SiAuto.Main.LogWarning("Recipient Email was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail3, recipientEmail3);
                exceptionsDetected = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                authenticationEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigAuthenticationEnabled));
            }
            catch
            {
                SiAuto.Main.LogWarning("Authentication Enabled was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigAuthenticationEnabled, authenticationEnabled);
                exceptionsDetected = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                mailUser = crypto.Decrypt((String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailUser));
            }
            catch
            {
                SiAuto.Main.LogWarning("Mail User was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailUser, crypto.Encrypt(mailUser));
                exceptionsDetected = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                mailPassword = crypto.Decrypt((String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailPassword));
            }
            catch
            {
                SiAuto.Main.LogWarning("Mail Password was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailPassword, crypto.Encrypt(mailPassword));
                exceptionsDetected = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                mailAlertsEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailAlertsEnabled));
            }
            catch
            {
                SiAuto.Main.LogWarning("Mail Alerts Enabled was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailAlertsEnabled, mailAlertsEnabled);
                exceptionsDetected = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                useSsl = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigUseSSL));
            }
            catch
            {
                SiAuto.Main.LogWarning("Use SSL was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigUseSSL, useSsl);
                exceptionsDetected = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                sendDailySummary = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigSendDailySummary));
            }
            catch
            {
                SiAuto.Main.LogWarning("Send Daily Summary was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigSendDailySummary, sendDailySummary);
                exceptionsDetected = true;
            }

            try
            {
                sendPlaintext = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigSendPlaintext));
            }
            catch
            {
                SiAuto.Main.LogWarning("Send Plaintext was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigSendPlaintext, sendPlaintext);
                exceptionsDetected = true;
            }

            try
            {
                ignoreVirtualDisks = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIgnoreVirtualDisks));
            }
            catch
            {
                SiAuto.Main.LogWarning("Ignore Virtual Disks was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIgnoreVirtualDisks, ignoreVirtualDisks);
                exceptionsDetected = true;
            }

            try
            {
                useHelperService = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigUseHelperService));
            }
            catch
            {
                SiAuto.Main.LogWarning("Use Helper Service was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigUseHelperService, useHelperService);
                exceptionsDetected = true;
            }

            try
            {
                helperServiceDiskPoolProvider = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigDiskPoolProvider);
            }
            catch
            {
                SiAuto.Main.LogWarning("Disk Pool Provider was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigDiskPoolProvider, String.Empty);
                exceptionsDetected = true;
            }

            // SSD thresholds
            try
            {
                ssdLifeLeftCritical = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigSsdLifeLeftCritical);
            }
            catch
            {
                SiAuto.Main.LogWarning("SSD Life Left Critical Threshold was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdLifeLeftCritical, ssdLifeLeftCritical);
                exceptionsDetected = true;
            }

            try
            {
                ssdLifeLeftWarning = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigSsdLifeLeftWarning);
            }
            catch
            {
                SiAuto.Main.LogWarning("SSD Life Left Warning Threshold was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdLifeLeftWarning, ssdLifeLeftWarning);
                exceptionsDetected = true;
            }

            try
            {
                ssdRetirementCritical = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigSsdRetirementCritical);
            }
            catch
            {
                SiAuto.Main.LogWarning("SSD Life Retired Sector Critical Threshold was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdRetirementCritical, ssdRetirementCritical);
                exceptionsDetected = true;
            }

            try
            {
                ssdRetirementWarning = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigSsdRetirementWarning);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdRetirementWarning, ssdRetirementWarning);
                exceptionsDetected = true;
                SiAuto.Main.LogWarning("SSD Life Retired Sector Warning Threshold was undefined or defined value was corrupt; it has been reset to default.");
            }

            // Emergency Ops
            try
            {
                performEmergencyBackup = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigPerformEmergencyBackup));
            }
            catch
            {
                SiAuto.Main.LogWarning("Perform Emergency Backup was undefined or defined value was corrupt; it has been reset to default.");
                performEmergencyBackup = false;
                configurationKey.SetValue(Properties.Resources.RegistryConfigPerformEmergencyBackup, performEmergencyBackup);
                exceptionsDetected = true;
            }

            try
            {
                performCustomBackup = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigPerformCustomBackup));
            }
            catch
            {
                SiAuto.Main.LogWarning("Perform Custom Backup was undefined or defined value was corrupt; it has been reset to default.");
                performCustomBackup = false;
                configurationKey.SetValue(Properties.Resources.RegistryConfigPerformCustomBackup, performCustomBackup);
                exceptionsDetected = true;
            }

            try
            {
                noHotBackup = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigNoHotBackup));
            }
            catch
            {
                SiAuto.Main.LogWarning("No Hot Backup was undefined or defined value was corrupt; it has been reset to default.");
                noHotBackup = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigNoHotBackup, noHotBackup);
                exceptionsDetected = true;
            }

            try
            {
                backupLocal = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigBackupLocal));
            }
            catch
            {
                SiAuto.Main.LogWarning("Backup Local was undefined or defined value was corrupt; it has been reset to default.");
                backupLocal = false;
                configurationKey.SetValue(Properties.Resources.RegistryConfigBackupLocal, backupLocal);
                exceptionsDetected = true;
            }

            try
            {
                performThermalShutdown = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigThermalShutdown));
            }
            catch
            {
                SiAuto.Main.LogWarning("Perform Thermal Shutdown was undefined or defined value was corrupt; it has been reset to default.");
                performThermalShutdown = false;
                configurationKey.SetValue(Properties.Resources.RegistryConfigThermalShutdown, performThermalShutdown);
                exceptionsDetected = true;
            }

            try
            {
                doDebugBackup = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigDoDebugBackup));
            }
            catch
            {
                doDebugBackup = false;
            }

            try
            {
                doDebugCustomBackup = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigDoDebugCustomBackup));
            }
            catch
            {
                doDebugCustomBackup = false;
            }

            try
            {
                debugBadDisks = (String)configurationKey.GetValue("DebugBadDisks");
            }
            catch
            {
                debugBadDisks = String.Empty;
            }

            try
            {
                localBackupPath = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigLocalBackupPath);
                if (String.IsNullOrEmpty(localBackupPath))
                {
                    localBackupPath = String.Empty;
                }
            }
            catch
            {
                SiAuto.Main.LogWarning("Local Backup Path was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigLocalBackupPath, localBackupPath);
                exceptionsDetected = true;
            }

            try
            {
                uncBackupPath = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigUncBackupPath);
                if (String.IsNullOrEmpty(uncBackupPath))
                {
                    uncBackupPath = String.Empty;
                }
            }
            catch
            {
                SiAuto.Main.LogWarning("UNC Backup Path was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigUncBackupPath, uncBackupPath);
                exceptionsDetected = true;
            }

            try
            {
                uncBackupUser = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigUncBackupUser);
            }
            catch
            {
                SiAuto.Main.LogWarning("UNC Backup User was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigUncBackupUser, uncBackupUser);
                exceptionsDetected = true;
            }

            try
            {
                uncBackupPassword = crypto.Decrypt((String)configurationKey.GetValue(Properties.Resources.RegistryConfigUncBackupPassword));
            }
            catch
            {
                SiAuto.Main.LogWarning("UNC Backup Password was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigUncBackupPassword, crypto.Encrypt(uncBackupPassword));
                exceptionsDetected = true;
            }

            try
            {
                customBackupProgram = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigCustomBackupProgram);
                if (String.IsNullOrEmpty(customBackupProgram))
                {
                    customBackupProgram = String.Empty;
                }
            }
            catch
            {
                SiAuto.Main.LogWarning("Custom Backup Program was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigCustomBackupProgram, customBackupProgram);
                exceptionsDetected = true;
            }

            try
            {
                customBackupArgs = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigCustomBackupArgs);
                if (String.IsNullOrEmpty(customBackupArgs))
                {
                    customBackupArgs = String.Empty;
                }
            }
            catch
            {
                SiAuto.Main.LogWarning("Custom Backup Args was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigCustomBackupArgs, customBackupArgs);
                exceptionsDetected = true;
            }

            // Skinning
            try
            {
                useDefaultSkinning = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigUseDefaultSkinning));
            }
            catch
            {
                SiAuto.Main.LogWarning("Use Default Skinning was undefined or defined value was corrupt; it has been reset to default.");
                useDefaultSkinning = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigUseDefaultSkinning, useDefaultSkinning);
                exceptionsDetected = true;
            }

            try
            {
                windowBackground = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigWindowBackground);
            }
            catch
            {
                SiAuto.Main.LogWarning("Window Background was undefined or defined value was corrupt; it has been reset to default (metal grate).");
                windowBackground = 0;
                configurationKey.SetValue(Properties.Resources.RegistryConfigWindowBackground, windowBackground);
                exceptionsDetected = true;
            }

            // Notifications
            try
            {
                isProwlEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsProwlEnabled));
            }
            catch
            {
                SiAuto.Main.LogWarning("Is Prowl Enabled was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsProwlEnabled, isProwlEnabled);
                exceptionsDetected = true;
            }

            try
            {
                isNmaEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsNmaEnabled));
            }
            catch
            {
                SiAuto.Main.LogWarning("Is NMA Enabled was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsNmaEnabled, isProwlEnabled);
                exceptionsDetected = true;
            }

            try
            {
                isGrowlEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsGrowlEnabled));
            }
            catch
            {
                SiAuto.Main.LogWarning("Is Growl Enabled was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsGrowlEnabled, isGrowlEnabled);
                exceptionsDetected = true;
            }

            try
            {
                isGrowlRemoteEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsGrowlRemoteEnabled));
            }
            catch
            {
                SiAuto.Main.LogWarning("Is Growl Remote Enabled was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsGrowlRemoteEnabled, isGrowlRemoteEnabled);
                exceptionsDetected = true;
            }

            try
            {
                isSnarlEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsSnarlEnabled));
            }
            catch
            {
                SiAuto.Main.LogWarning("Is Prowl Enabled was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsSnarlEnabled, isSnarlEnabled);
                exceptionsDetected = true;
            }

            try
            {
                isTwitterEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsTwitterEnabled));
            }
            catch
            {
                SiAuto.Main.LogWarning("Is Prowl Enabled was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsTwitterEnabled, isTwitterEnabled);
                exceptionsDetected = true;
            }

            try
            {
                isToastyEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsToastyEnabled));
            }
            catch
            {
                SiAuto.Main.LogWarning("Is Prowl Enabled was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsToastyEnabled, isToastyEnabled);
                exceptionsDetected = true;
            }

            try
            {
                prowlApiKey = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigProwlApiKeychain);
            }
            catch
            {
                SiAuto.Main.LogWarning("Prowl API Keychain was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigProwlApiKeychain, prowlApiKey);
                exceptionsDetected = true;
            }

            try
            {
                nmaApiKey = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigNmaApiKeychain);
            }
            catch
            {
                SiAuto.Main.LogWarning("NMA API Keychain was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigNmaApiKeychain, nmaApiKey);
                exceptionsDetected = true;
            }

            try
            {
                growlRemoteComputer = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigGrowlRemoteMachine);
            }
            catch
            {
                SiAuto.Main.LogWarning("Growl Remote Computer was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigGrowlRemoteMachine, growlRemoteComputer);
                exceptionsDetected = true;
            }

            try
            {
                growlPort = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGrowlRemotePort);
            }
            catch
            {
                SiAuto.Main.LogWarning("Growl Remote Port was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigGrowlRemotePort, growlPort);
                exceptionsDetected = true;
            }

            try
            {
                growlPassword = crypto.Decrypt((String)configurationKey.GetValue(Properties.Resources.RegistryConfigGrowlPassword));
            }
            catch
            {
                SiAuto.Main.LogWarning("Growl Password was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigGrowlPassword, crypto.Encrypt(growlPassword));
                exceptionsDetected = true;
            }

            try
            {
                toastyApiKey = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigWindowsPhoneGuids);
            }
            catch
            {
                SiAuto.Main.LogWarning("Toasty Keychain was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigWindowsPhoneGuids, toastyApiKey);
                exceptionsDetected = true;
            }

            try
            {
                isPushoverEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsPushoverEnabled));
            }
            catch
            {
                SiAuto.Main.LogWarning("Is Pushover Enabled was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsPushoverEnabled, isPushoverEnabled);
                exceptionsDetected = true;
            }

            try
            {
                pushoverUserKey = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigPushoverKey);
            }
            catch
            {
                SiAuto.Main.LogWarning("Pushover user key was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverKey, pushoverUserKey);
                exceptionsDetected = true;
            }

            try
            {
                pushoverClearedSound = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigPushoverClearedSound);
            }
            catch
            {
                SiAuto.Main.LogWarning("Pushover cleared sound was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverClearedSound, PUSHOVER_DEFAULT_SOUND);
                exceptionsDetected = true;
            }

            try
            {
                pushoverCriticalSound = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigPushoverCriticalSound);
            }
            catch
            {
                SiAuto.Main.LogWarning("Pushover critical sound was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverCriticalSound, PUSHOVER_DEFAULT_SOUND);
                exceptionsDetected = true;
            }

            try
            {
                pushoverWarningSound = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigPushoverWarningSound);
            }
            catch
            {
                SiAuto.Main.LogWarning("Pushover warning sound was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverWarningSound, PUSHOVER_DEFAULT_SOUND);
                exceptionsDetected = true;
            }

            try
            {
                pushoverDeviceTarget = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigPushoverDeviceTarget);
            }
            catch
            {
                SiAuto.Main.LogWarning("Pushover device target was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverDeviceTarget, pushoverDeviceTarget);
                exceptionsDetected = true;
            }

            // System Notifications
            try
            {
                notifyOnPowerChange = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigNotifyPowerChange));
            }
            catch
            {
                SiAuto.Main.LogWarning("Notify On Power Change was undefined or defined value was corrupt; it has been reset to default.");
                notifyOnPowerChange = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigNotifyPowerChange, notifyOnPowerChange);
                exceptionsDetected = true;
            }

            try
            {
                notifyOnFilthyShutdown = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigNotifyFilthyShutdown));
            }
            catch
            {
                SiAuto.Main.LogWarning("Notify On Filthy Shutdown was undefined or defined value was corrupt; it has been reset to default.");
                notifyOnFilthyShutdown = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigNotifyFilthyShutdown, notifyOnPowerChange);
                exceptionsDetected = true;
            }

            // Logging
            try
            {
                String defaultLogLocation = debugLogLocation;
                debugLogLocation = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigLogLocation);
                if (String.IsNullOrEmpty(debugLogLocation) || !System.IO.Directory.Exists(debugLogLocation))
                {
                    debugLogLocation = defaultLogLocation;
                    throw new System.IO.DirectoryNotFoundException("Invalid logfile path.");
                }
            }
            catch
            {
                SiAuto.Main.LogWarning("SmartInspect Debug Log Location was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigLogLocation, debugLogLocation);
                exceptionsDetected = true;
            }

            // Custom notificate
            try
            {
                customNotificate = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigCustomSupportMessage);
            }
            catch
            {
                SiAuto.Main.LogWarning("Custom Notificate was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigCustomSupportMessage, String.Empty);
                exceptionsDetected = true;
            }

            if (exceptionsDetected)
            {
                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Exceptions were detected by the Server while fetching configuration " +
                    "data from the Registry. This is normal the very first time you run Home Server SMART, because " +
                    "no Registry data exists. However, if this message appears repeatedly, it could indicate data " +
                    "corruption. This can also occur after upgrading Home Server SMART because new Registry values may have been defined.\n\n" +
                    "At least one configuration value has been set to its default.\n\nValues that were reset immediately precede this message.");
            }

            SiAuto.Main.LogMessage("=== Configuraion Values as Defined in Registry ===");
            SiAuto.Main.LogString("temperaturePreference", temperaturePreference);
            SiAuto.Main.LogInt("criticalTemperatureThreshold", criticalTemperatureThreshold);
            SiAuto.Main.LogInt("overheatedTemperatureThreshold", overheatedTemperatureThreshold);
            SiAuto.Main.LogInt("hotTemperatureThreshold", hotTemperatureThreshold);
            SiAuto.Main.LogInt("warmTemperatureThreshold", warmTemperatureThreshold);
            SiAuto.Main.LogBool("ignoreOverheated", ignoreOverheated);
            SiAuto.Main.LogBool("ignoreHot", ignoreHot);
            SiAuto.Main.LogBool("ignoreWarm", ignoreWarm);
            SiAuto.Main.LogBool("ignoreOverheated", ignoreVirtualDisks);
            SiAuto.Main.LogBool("preserveOnUninstall", preserveOnUninstall);
            SiAuto.Main.LogBool("reportCritical", reportCritical);
            SiAuto.Main.LogBool("reportWarnings", reportWarnings);
            SiAuto.Main.LogBool("reportGeriatric", reportGeriatric);
            SiAuto.Main.LogBool("fallbackToWmi", fallbackToWmi);
            SiAuto.Main.LogBool("debugMode", debugMode);
            SiAuto.Main.LogBool("advancedSii", advancedSii);
            SiAuto.Main.LogBool("bitLockerHideTab", bitLockerHideTab);
            SiAuto.Main.LogBool("bitLockerPreventLocking", bitLockerPreventLocking);
            SiAuto.Main.LogBool("bitLockerShowAllDrives", bitLockerShowAllDrives);
            SiAuto.Main.LogBool("useHelperService", useHelperService);
            SiAuto.Main.LogString("helperServiceDiskPoolProvider", helperServiceDiskPoolProvider);
            SiAuto.Main.LogInt("pollingInterval", pollingInterval);

            SiAuto.Main.LogString("mailServer", mailServer);
            SiAuto.Main.LogString("senderEmailAddress", senderEmailAddress);
            SiAuto.Main.LogString("senderFriendlyName", senderFriendlyName);
            SiAuto.Main.LogString("recipientEmailAddress", recipientEmailAddress);
            SiAuto.Main.LogString("recipientFriendlyName", recipientFriendlyName);
            SiAuto.Main.LogString("recipientEmail2", recipientEmail2);
            SiAuto.Main.LogString("recipientEmail3", recipientEmail3);
            SiAuto.Main.LogMessage("Username and Password are not logged.");
            SiAuto.Main.LogBool("authenticationEnabled", authenticationEnabled);
            SiAuto.Main.LogInt("serverPort", serverPort);
            SiAuto.Main.LogBool("mailAlertsEnabled", mailAlertsEnabled);
            SiAuto.Main.LogBool("useSsl", useSsl);
            SiAuto.Main.LogBool("sendDailySummary", sendDailySummary);
            SiAuto.Main.LogBool("sendPlaintext", sendPlaintext);
            SiAuto.Main.LogBool("isMailConfigValid", isMailConfigValid);
            // SSD thresholds
            SiAuto.Main.LogInt("ssdLifeLeftCritical", ssdLifeLeftCritical);
            SiAuto.Main.LogInt("ssdLifeLeftWarning", ssdLifeLeftWarning);
            SiAuto.Main.LogInt("ssdRetirementCritical", ssdRetirementCritical);
            SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);

            // Notifications
            SiAuto.Main.LogBool("isProwlEnabled", isProwlEnabled);
            SiAuto.Main.LogBool("isNmaEnabled", isNmaEnabled);
            SiAuto.Main.LogBool("isGrowlEnabled", isGrowlEnabled);
            SiAuto.Main.LogBool("isGrowlRemoteEnabled", isGrowlRemoteEnabled);
            SiAuto.Main.LogBool("isSnarlEnabled", isSnarlEnabled);
            SiAuto.Main.LogBool("isTwitterEnabled", isTwitterEnabled);
            SiAuto.Main.LogBool("isToastyEnabled", isToastyEnabled);
            SiAuto.Main.LogBool("isPushoverEnabled", isPushoverEnabled);
            SiAuto.Main.LogString("prowlApiKey", prowlApiKey);
            SiAuto.Main.LogString("nmaApiKey", nmaApiKey);
            SiAuto.Main.LogString("growlRemoteComputer", growlRemoteComputer);
            SiAuto.Main.LogInt("growlPort", growlPort);
            SiAuto.Main.LogString("toastyApiKey", toastyApiKey);
            SiAuto.Main.LogString("pushoverUserKey", pushoverUserKey);
            SiAuto.Main.LogString("pushoverClearedSound", pushoverClearedSound);
            SiAuto.Main.LogString("pushoverCriticalSound", pushoverCriticalSound);
            SiAuto.Main.LogString("pushoverWarningSound", pushoverWarningSound);
            SiAuto.Main.LogString("pushoverDeviceTarget", pushoverDeviceTarget);

            // Skinning
            SiAuto.Main.LogBool("useDefaultSkinning", useDefaultSkinning);
            SiAuto.Main.LogInt("windowBackground", windowBackground);

            // System Notifications
            SiAuto.Main.LogBool("notifyOnFilthyShutdown", notifyOnFilthyShutdown);
            SiAuto.Main.LogBool("notifyOnPowerChange", notifyOnPowerChange);

            SiAuto.Main.LogString("debugLogLocation", debugLogLocation);

            SiAuto.Main.LogString("customNotificate", customNotificate);
            SiAuto.Main.LogMessage("=== End of Configuraion Values as Defined in Registry ===");

            SiAuto.Main.LogMessage("=== Checking GPO configuration values as defined in Registry - in non-corporate environments there probably won't be any ===");
            SiAuto.Main.LogMessage("0 = policy configured and set to Disabled; 1 = policy configured and set to Enabled; 2 = policy set to Not Configured)");

            try
            {
                gpoDpa = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoDpa);
                SiAuto.Main.LogMessage("Disk polling and analysis is configured and using value " + gpoDpa.ToString());
                if (gpoDpa == 0)
                {
                    SiAuto.Main.LogFatal("[Policy Error] Invalid policy configuration state - Dpa!");
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Severe] Policy \"Dpa\" cannot be set to Disabled. Please reconfigure the GPO to Enabled or Not Configured.");
                }
            }
            catch
            {
                gpoDpa = 2;
                SiAuto.Main.LogMessage("Disk polling and analysis will use default value 2.");
            }

            try
            {
                gpoTempCtl = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoTempCtl);
                SiAuto.Main.LogMessage("Disk temperature prefs and alerts is configured and using value " + gpoTempCtl.ToString());
                if (gpoTempCtl == 0)
                {
                    SiAuto.Main.LogFatal("[Policy Error] Invalid policy configuration state - TempCtl!");
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Severe] Policy \"TempCtl\" cannot be set to Disabled. Please reconfigure the GPO to Enabled or Not Configured.");
                }
            }
            catch
            {
                gpoTempCtl = 2;
                SiAuto.Main.LogMessage("Disk temperature prefs and alerts will use default value 2.");
            }

            try
            {
                gpoVirtualIgnore = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoVirtualIgnore);
                SiAuto.Main.LogMessage("VD enumeration is configured and using value " + gpoVirtualIgnore.ToString());
            }
            catch
            {
                gpoVirtualIgnore = 2;
                SiAuto.Main.LogMessage("VD enumeration will use default value 2.");
            }

            try
            {
                gpoAllowIgnoredItems = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoAllowIgnoredItems);
                SiAuto.Main.LogMessage("Allow ignored items is configured and using value " + gpoAllowIgnoredItems.ToString());
            }
            catch
            {
                gpoAllowIgnoredItems = 2;
                SiAuto.Main.LogMessage("Allow ignored items will use default value 2.");
            }

            try
            {
                gpoEmailNotificate = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoEmailNotificate);
                SiAuto.Main.LogMessage("Email notificate is configured and using value " + gpoEmailNotificate.ToString());
                if (gpoEmailNotificate == 0)
                {
                    // Disable email alerts by policy!
                    mailAlertsEnabled = false;
                    SiAuto.Main.LogBool("mailAlertsEnabled", mailAlertsEnabled);
                }
            }
            catch
            {
                gpoEmailNotificate = 2;
                SiAuto.Main.LogMessage("Email notificate will use default value 2.");
            }

            // We set the email config valid flag to TRUE if mail policy is enabled, and email alerts are enabled.
            // This ensures mail will be sent!
            if (mailAlertsEnabled && gpoEmailNotificate == 1 && !isMailConfigValid)
            {
                SiAuto.Main.LogWarning("The mail config valid flag is set to FALSE. However, mail policy is set to ENABLED and mail configuration is ACTIVE. It is assumed the configuration is valid. Setting to TRUE.");
                isMailConfigValid = true;
            }

            try
            {
                gpoProwlNotificate = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoProwlNotificate);
                SiAuto.Main.LogMessage("Prowl notificate is configured and using value " + gpoProwlNotificate.ToString());
                if (gpoProwlNotificate == 0)
                {
                    // Disable Prowl by policy!
                    isProwlEnabled = false;
                    SiAuto.Main.LogBool("isProwlEnabled", isProwlEnabled);
                }
            }
            catch
            {
                gpoProwlNotificate = 2;
                SiAuto.Main.LogMessage("Prowl notificate will use default value 2.");
            }

            try
            {
                gpoNmaNotificate = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoNmaNotificate);
                SiAuto.Main.LogMessage("NMA notificate is configured and using value " + gpoNmaNotificate.ToString());
                if (gpoNmaNotificate == 0)
                {
                    // Disable NMA by policy!
                    isNmaEnabled = false;
                    SiAuto.Main.LogBool("isNmaEnabled", isNmaEnabled);
                }
            }
            catch
            {
                gpoNmaNotificate = 2;
                SiAuto.Main.LogMessage("NMA notificate will use default value 2.");
            }

            try
            {
                gpoWpNotificate = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoWindowsPhoneNotificate);
                SiAuto.Main.LogMessage("Toasty notificate is configured and using value " + gpoWpNotificate.ToString());
                if (gpoWpNotificate == 0)
                {
                    // Disable Boxcar by policy!
                    isToastyEnabled = false;
                    SiAuto.Main.LogBool("isToastyEnabled", isToastyEnabled);
                }
            }
            catch
            {
                gpoWpNotificate = 2;
                SiAuto.Main.LogMessage("Toasty notificate will use default value 2.");
            }

            try
            {
                gpoGrowlNotificate = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoGrowlNotificate);
                SiAuto.Main.LogMessage("Growl notificate is configured and using value " + gpoGrowlNotificate.ToString());
                if (gpoGrowlNotificate == 0)
                {
                    // Disable Growl by policy!
                    isGrowlEnabled = false;
                    isGrowlRemoteEnabled = false;
                    SiAuto.Main.LogBool("isGrowlEnabled", isGrowlEnabled);
                    SiAuto.Main.LogBool("isGrowlRemoteEnabled", isGrowlRemoteEnabled);
                }
            }
            catch
            {
                gpoGrowlNotificate = 2;
                SiAuto.Main.LogMessage("Growl notificate will use default value 2.");
            }

            try
            {
                gpoSnarlNotificate = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoSnarlNotificate);
                SiAuto.Main.LogMessage("Snarl notificate is configured and using value " + gpoSnarlNotificate.ToString());
                if (gpoSnarlNotificate == 0)
                {
                    // Disable Snarl by policy!
                    isSnarlEnabled = false;
                    SiAuto.Main.LogBool("isSnarlEnabled", isSnarlEnabled);
                }
            }
            catch
            {
                gpoSnarlNotificate = 2;
                SiAuto.Main.LogMessage("Snarl notificate will use default value 2.");
            }

            try
            {
                gpoAdvancedSettings = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoAdvancedSettings);
                SiAuto.Main.LogMessage("Advanced settings is configured and using value " + gpoAdvancedSettings.ToString());
            }
            catch
            {
                gpoAdvancedSettings = 2;
                SiAuto.Main.LogMessage("Advanced settings will use default value 2.");
            }

            try
            {
                gpoDebuggingControl = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoDebuggingControl);
                SiAuto.Main.LogMessage("Debugging control is configured and using value " + gpoDebuggingControl.ToString());
            }
            catch
            {
                gpoDebuggingControl = 2;
                SiAuto.Main.LogMessage("Debugging control will use default value 2.");
            }

            try
            {
                gpoSSD = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoSSD);
                SiAuto.Main.LogMessage("SSD is configured and using value " + gpoSSD.ToString());
                if (gpoSSD == 0)
                {
                    SiAuto.Main.LogFatal("[Policy Error] Invalid policy configuration state - SSD!");
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Severe] Policy \"SSD\" cannot be set to Disabled. Please reconfigure the GPO to Enabled or Not Configured.");
                }
            }
            catch
            {
                gpoSSD = 2;
                SiAuto.Main.LogMessage("SSD will use default value 2.");
            }

            try
            {
                gpoCheckForUpdates = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoCheckForUpdates);
                SiAuto.Main.LogMessage("Check for updates is configured and using value " + gpoCheckForUpdates.ToString());
            }
            catch
            {
                gpoCheckForUpdates = 2;
                SiAuto.Main.LogMessage("Check for updates will use default value 2.");
            }

            try
            {
                gpoUiTheme = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoUiTheme);
                SiAuto.Main.LogMessage("UI theme is configured and using value " + gpoUiTheme.ToString());
            }
            catch
            {
                gpoUiTheme = 2;
                SiAuto.Main.LogMessage("UI theme will use default value 2.");
            }

            try
            {
                gpoUseSupportMessage = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoUseSupportMessage);
                SiAuto.Main.LogMessage("Use support message is configured and using value " + gpoUseSupportMessage.ToString());
                if (gpoUseSupportMessage == 0)
                {
                    // Clear support message (in case one is set).
                    customNotificate = String.Empty;
                    SiAuto.Main.LogString("customNotificate", customNotificate);
                }
            }
            catch
            {
                gpoUseSupportMessage = 2;
                SiAuto.Main.LogMessage("Use support message will use default value 2.");
            }

            SiAuto.Main.LogMessage("=== Done checking/setting GPO configuration flags. ===");

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.LoadConfiguration");
        }

        /// <summary>
        /// Enumerates the disks and saves their data to the Registry.
        /// </summary>
        public String EnumerateDisksAndSaveData()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.EnumerateDisksAndSaveData");
            String phantomDiskList = String.Empty;
            try
            {
                DiskEnumerator.RefreshDiskInfo(fallbackToWmi, advancedSii, ignoreVirtualDisks, out phantomDiskList);
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogFatal("Exception was thrown during a disk polling operation: " + ex.Message);
                SiAuto.Main.LogException(ex);
                WindowsEventLogger.LogError(ex.Message + "\n\n" + ex.StackTrace, 53881, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.EnumerateDisksAndSaveData");
            return phantomDiskList;
        }

        public void ServiceAutoPollDisks()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.ServiceAutoPollDisks");
            try
            {
                LoadConfiguration();
                String phantomDiskList = String.Empty;
                phantomDiskList = EnumerateDisksAndSaveData();
                if (String.IsNullOrEmpty(phantomDiskList))
                {
                    SiAuto.Main.LogMessage("No phantom disks were detected. If an active alert for phantoms exists, it will be cleared.");
                }
                else
                {
                    SiAuto.Main.LogWarning("At least one phantom disk was detected by the Server. Phantom(s): " + phantomDiskList);
                }
                CheckForFeverishDisks();
                if (Utilities.Utility.IsSystemWindows8())
                {
                    ProcessPhantoms(phantomDiskList);
                }
                ClearStaleDisks();
                CheckChangeGrowlState();
                PostAlert();
                WriteXmlAlertsFile();
                ProcessDailySummaryData(true);
                if (performEmergencyBackup || performCustomBackup)
                {
                    List<int> failingDiskList = CheckForEmergencyBackup();
                    if (failingDiskList.Count > 0)
                    {
                        SiAuto.Main.LogWarning("Emergency backup check returned a non-zero count of failing disks; emergency backup will be attempted.");
                        if (performEmergencyBackup)
                        {
                            RunEmergencyBackup(false, failingDiskList);
                        }
                        if (performCustomBackup)
                        {
                            RunCustomBackup(false, failingDiskList);
                        }
                    }
                }
                RunThermalShutdownCheck();
            }
            catch (Exception ex)
            {
                WindowsEventLogger.LogError("ServiceAutoPollDisks failed: " + ex.Message +
                    "\n\n" + ex.StackTrace, 53880, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.ServiceAutoPollDisks");
        }

        private void CheckChangeGrowlState()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.CheckChangeGrowlState");
            // Growl and Snarl?
            if (isGrowlEnabled)
            {
                if (growl == null)
                {
                    InitializeGrowl();
                }
            }
            else
            {
                growl = null;
                if (gnt != null)
                {
                    gnt = null;
                }
                System.GC.Collect();
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.CheckChangeGrowlState");
        }

        private void CheckChangeSnarlState()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.CheckChangeSnarlState");
            // Growl and Snarl?
            if (isSnarlEnabled)
            {
                if (snarl == null)
                {
                    InitializeSnarl();
                }
            }
            else
            {
                snarl = null;
                if (snt != null)
                {
                    snt = null;
                }
                System.GC.Collect();
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.CheckChangeSnarlState");
        }

        public void ServiceCheckForUpdates()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.ServiceCheckForUpdates");
            if (gpoCheckForUpdates == 0)
            {
                SiAuto.Main.LogMessage("Update check is prohibited by policy; update check is cancelled.");
            }
            else if (!preserveOnUninstall)
            {
                SiAuto.Main.LogWarning("Automatic update checking is disabled by user request (preserveOnUninstall flag).");
            }
            else if (Components.Utilities.CheckForUpdates.IsUpdateCheckNeeded())
            {
                Components.Utilities.UpdateInfo info = Components.Utilities.CheckForUpdates.CheckUpdates();
                if (info.IsUpdateAvailable && !info.IsError)
                {
                    SiAuto.Main.LogMessage("Update " + info.AvailableVersion + " is available.");
                    if (isWindowsServerSolutions)
                    {
                        Components.Utilities.CheckForUpdates.UpdateLastUpdateCheck(new Guid("52cbe9ea-dfe4-4556-af45-4e79ed35e42b"), info.CurrentVersion);
                    }
                    else if (mexiSexi != null && mexiSexi.IsMexiSexi)
                    {
                        Components.Utilities.CheckForUpdates.UpdateLastUpdateCheck(mexiSexi.UserInfo.UserGuid, info.CurrentVersion);
                    }

                    SiAuto.Main.LogMessage("Send update notificate.");
                    UpdateNotificate(info);
                    isUpdateAvailable = true;
                    updateVersion = info.AvailableVersion;
                    updateDate = info.ReleaseDate;
                    updateUrl = info.MoreInfoUrl;
                }
                else if (info.IsError)
                {
                    WindowsEventLogger.LogWarning("Exceptions were detected checking for updates. " + info.ErrorInfo.Message + "\n\n" +
                        info.ErrorInfo.StackTrace, 53886, Properties.Resources.EventLogJoshua);
                    SiAuto.Main.LogError("An error occurred during the update check: " + info.ErrorInfo.Message);
                    SiAuto.Main.LogException(info.ErrorInfo);
                }
                else
                {
                    SiAuto.Main.LogMessage("No updates available.");
                    Components.Utilities.CheckForUpdates.UpdateLastUpdateCheck(mexiSexi.UserInfo.UserGuid, info.CurrentVersion);
                    isUpdateAvailable = false;
                    updateVersion = String.Empty;
                    updateDate = String.Empty;
                    updateUrl = String.Empty;
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.ServiceCheckForUpdates");
        }

        public void UpdateNotificate(Components.Utilities.UpdateInfo info)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.UpdateNotificate");
            String appTitle = (isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart);
            String notificate = appTitle +
                " application update is available! You're running version " + info.CurrentVersion + "; the available version is " +
                info.AvailableVersion + ", released on " + info.ReleaseDate + ". For more details, please visit " + info.MoreInfoUrl + ". " +
                "To go directly to the download page, please visit " + info.DirectDownloadUrl + ".";
            PostUpdateNotificate(notificate, appTitle + " Update");
            WindowsEventLogger.LogInformation(notificate, 53865, Properties.Resources.EventLogTaryn);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.UpdateNotificate");
        }

        public void CheckForFeverishDisks()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.CheckForFeverishDisks");
            try
            {
                DiskEnumerator.CheckDiskHealth(feverishDisks, criticalTemperatureThreshold, overheatedTemperatureThreshold, hotTemperatureThreshold,
                    warmTemperatureThreshold, ssdLifeLeftCritical, ssdLifeLeftWarning, ssdRetirementCritical, ssdRetirementWarning, ignoreOverheated,
                    ignoreHot, ignoreWarm, reportWarnings, reportGeriatric, reportCritical, temperaturePreference);
            }
            catch (Exception ex)
            {
                WindowsEventLogger.LogError("CheckForFeverishDisks failed: " +
                    ex.Message + "\n\n" + ex.StackTrace, 53884, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.CheckForFeverishDisks");
        }

        /// <summary>
        /// Clears events from the WHS notification stack for a disk no longer connected to the Server.
        /// </summary>
        public void ClearStaleDisks()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.ClearStaleDisks");
            DiskEnumerator.ClearStaleDiskEvents(feverishDisks);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.ClearStaleDisks");
        }

        private void PostAlert()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.PostAlert");
            foreach (DataRow row in feverishDisks.FeverishDisks.Select())
            {
                try
                {
                    switch ((AlertAction)row["Action"])
                    {
                        case AlertAction.Insert:
                            {
                                // Insert a new item.
                                summary.Insert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["AttributeName"].ToString(),
                                    row["HealthTitle"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Insert);
                                SendProwlAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Insert);
                                SendNmaAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Insert);
                                SendToastyAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Insert);
                                SendEmailAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"],
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(),
                                    (bool)row["IsCritical"], row["Guid"].ToString(), AlertAction.Insert);
                                SendGrowlAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Insert);
                                SendSnarlAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Insert);
                                SendPushoverAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Insert);
                                SiAuto.Main.LogMessage("Performed a WHSInfo INSERT with GUID " + row["Guid"].ToString() + " (" + row["HealthTitle"].ToString() + ")");
                                row["Action"] = AlertAction.Hold;
                                row.AcceptChanges();
                                break;
                            }
                        case AlertAction.Remove:
                            {
                                // Remove an existing item.
                                summary.Insert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["AttributeName"].ToString(),
                                    row["HealthTitle"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Remove);
                                SendProwlAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Remove);
                                SendNmaAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Remove);
                                SendToastyAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Remove);
                                SendEmailAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"],
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(),
                                    (bool)row["IsCritical"], row["Guid"].ToString(), AlertAction.Remove);
                                SendGrowlAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Remove);
                                SendSnarlAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Remove);
                                SendPushoverAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Remove);
                                SiAuto.Main.LogMessage("Performed a WHSInfo REMOVE with GUID " + row["Guid"].ToString() + " (" + row["HealthTitle"].ToString() + ")");
                                row.Delete();
                                row.AcceptChanges();
                                break;
                            }
                        case AlertAction.Update:
                            {
                                // Remove the existing item, then replace it with a new one.
                                summary.Insert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["AttributeName"].ToString(),
                                    row["HealthTitle"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Update);
                                SendProwlAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Update);
                                SendNmaAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Update);
                                SendToastyAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Update);
                                SendEmailAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"],
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(),
                                    (bool)row["IsCritical"], row["Guid"].ToString(), AlertAction.Update);
                                SendGrowlAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Update);
                                SendSnarlAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Update);
                                SendPushoverAlert(row["DiskModel"].ToString(), row["DiskPath"].ToString(), (int)row["AttributeID"], row["HealthTitle"].ToString(),
                                    row["AttributeName"].ToString(), row["HealthMessage"].ToString(), (bool)row["IsCritical"], AlertAction.Update);
                                row["Action"] = AlertAction.Hold;
                                row.AcceptChanges();
                                break;
                            }
                        case AlertAction.Hold:
                        default:
                            {
                                SiAuto.Main.LogMessage("No changes on event GUID " + row["Guid"].ToString() + " (" + row["HealthTitle"].ToString() + ")");
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogError("Failed to extract data from FeverishDisk data row. " + ex.Message);
                    SiAuto.Main.LogException(ex);
                }
            }
            SiAuto.Main.LogMessage("Commit changes to feverish disks table.");
            feverishDisks.FeverishDisks.AcceptChanges();
            SiAuto.Main.LogMessage("Changes committed.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.PostAlert");
        }

        private void WriteXmlAlertsFile()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.WriteXmlAlertsFile");
            try
            {
                SiAuto.Main.LogMessage("Initialize the I/O XML stream.");
                System.Text.StringBuilder stream = new System.Text.StringBuilder();
                SiAuto.Main.LogMessage("Initialize the XmlWriterSettings.");
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.Encoding = Encoding.UTF8;
                SiAuto.Main.LogMessage("Initialize the XmlWriter.");
                XmlWriter writer = XmlWriter.Create(stream, settings);

                SiAuto.Main.LogMessage("Begin writing XML document.");
                writer.WriteStartDocument(); // <?xml version="1.0" encoding="utf-8"?>

                writer.WriteStartElement("alerts", "");

                int count = feverishDisks.FeverishDisks.Rows.Count;
                SiAuto.Main.LogInt("Active alert count", count);
                writer.WriteElementString("count", count.ToString());

                SiAuto.Main.LogMessage("Write out the alerts (if any exist).");
                foreach (DataRow row in feverishDisks.FeverishDisks.Select())
                {
                    SiAuto.Main.LogMessage("Writing out individual alert.");
                    writer.WriteStartElement("alert");

                    writer.WriteAttributeString("healthTitle", row["HealthTitle"].ToString());
                    writer.WriteAttributeString("diskModel", row["DiskModel"].ToString());
                    writer.WriteAttributeString("diskPath", row["DiskPath"].ToString());
                    writer.WriteAttributeString("attributeID", row["AttributeID"].ToString());
                    writer.WriteAttributeString("attributeName", row["AttributeName"].ToString());
                    writer.WriteAttributeString("healthMessage", row["HealthMessage"].ToString());
                    writer.WriteAttributeString("isCritical", row["IsCritical"].ToString());
                    writer.WriteAttributeString("correlationID", row["Guid"].ToString());

                    writer.WriteEndElement(); // alert
                }

                writer.WriteStartElement("update");
                writer.WriteAttributeString("isAvailable", isUpdateAvailable.ToString());
                writer.WriteAttributeString("version", updateVersion);
                writer.WriteAttributeString("releaseDate", updateDate);
                writer.WriteAttributeString("url", updateUrl);
                writer.WriteEndElement(); // update

                writer.WriteEndElement(); // alerts

                SiAuto.Main.LogMessage("Write the XML end elment and close the XML stream.");
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();

                SiAuto.Main.LogMessage("Open the XML file for writing in overwrite mode.");
                System.IO.StreamWriter xmlOutput = new System.IO.StreamWriter(xmlAlertFile, false);
                SiAuto.Main.LogMessage("Write out the file.");
                xmlOutput.Write(stream.ToString());
                SiAuto.Main.LogMessage("Flush and close the I/O stream.");
                xmlOutput.Flush();
                xmlOutput.Close();
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Exceptions were detected writing the XML alerts. " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.WriteXmlAlertsFile");
        }

        private void WriteXmlFatalFile(bool isSevere)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.WriteXmlAlertsFile");
            try
            {
                SiAuto.Main.LogMessage("Initialize the I/O XML stream.");
                System.Text.StringBuilder stream = new System.Text.StringBuilder();
                SiAuto.Main.LogMessage("Initialize the XmlWriterSettings.");
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.Encoding = Encoding.UTF8;
                SiAuto.Main.LogMessage("Initialize the XmlWriter.");
                XmlWriter writer = XmlWriter.Create(stream, settings);

                SiAuto.Main.LogMessage("Begin writing XML document.");
                writer.WriteStartDocument(); // <?xml version="1.0" encoding="utf-8"?>

                writer.WriteStartElement("alerts", "");

                writer.WriteStartElement("severe");

                writer.WriteAttributeString("statusMessage", isSevere.ToString());
                writer.WriteEndElement(); // alert

                writer.WriteEndElement(); // alerts

                SiAuto.Main.LogMessage("Write the XML end elment and close the XML stream.");
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();

                SiAuto.Main.LogMessage("Open the XML file for writing in overwrite mode.");
                System.IO.StreamWriter xmlOutput = new System.IO.StreamWriter(xmlAlertFile, false);
                SiAuto.Main.LogMessage("Write out the file.");
                xmlOutput.Write(stream.ToString());
                SiAuto.Main.LogMessage("Flush and close the I/O stream.");
                xmlOutput.Flush();
                xmlOutput.Close();
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Exceptions were detected writing the XML alerts. " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.WriteXmlAlertsFile");
        }

        private void PostUpdateNotificate(String notificate, String title)
        {
            PostUpdateNotificate(notificate, title, false, false, false);
        }

        private void PostUpdateNotificate(String notificate, String title, bool isHyperFatal, bool isGeezer, bool isUrgent)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.PostUpdateNotificate");
            String imageUri = String.Empty;
            String emailRef = String.Empty;
            String servername = Environment.MachineName;
            notificate += (isWindowsServerSolutions ? " Server: " : " Computer: ") + servername;

            if (isHyperFatal)
            {
                imageUri = Properties.Resources.RemoteNotifyIconHyperfatal;
                emailRef = "{DEADDEAD-0000-9999-FFFF-DEADDEADDEAD}";
            }
            else if (isGeezer)
            {
                imageUri = Properties.Resources.RemoteNotifyIconHyperfatal;
                emailRef = "{00000000-3333-6666-9999-AAAACCCCFFFF}";
            }
            else if (isUrgent)
            {
                imageUri = Properties.Resources.RemoteNotifyIconWarningRaised;
                emailRef = "{FFFFEEEE-DDDD-CCCC-BBBB-AAAA99998888}";
            }
            else
            {
                imageUri = Properties.Resources.RemoteNotifyIconGeneral;
                emailRef = "{00000000-1111-2222-3333-444444444444}";
            }
            SendProwlAlert(notificate, title, (isHyperFatal || isGeezer));
            SendNmaAlert(notificate, title, (isHyperFatal || isGeezer));
            SendToastyAlert(notificate, title, (isHyperFatal || isGeezer), imageUri);
            SendPushoverAlert(notificate, title, !(isHyperFatal || isGeezer), (isHyperFatal || isGeezer));
            if (isHyperFatal || isGeezer || isUrgent)
            {
                SendGrowlAlert(notificate, title, true, null, Growl.Connector.Priority.High);
                SendSnarlAlert(notificate, title, true, null, Growl.Connector.Priority.High);
                try
                {
                    SendEmailAlert(title, notificate, System.Net.Mail.MailPriority.High, emailRef);
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogFatal("[Chilkat Fatal Error] " + ex.Message);
                    SiAuto.Main.LogException(ex);
                }
            }
            else
            {
                if (gnt != null)
                {
                    SendGrowlAlert(notificate, title, false, gnt.General.Icon, Growl.Connector.Priority.Normal);
                }
                else
                {
                    SiAuto.Main.LogWarning("Growl gnt flag is null; skipping growl alert (since gnt.General.Icon is NULL).");
                }

                if (snt != null)
                {
                    SendSnarlAlert(notificate, title, false, snt.General.Icon, Growl.Connector.Priority.Normal);
                }
                else
                {
                    SiAuto.Main.LogWarning("Snarl snt flag is null; skipping growl alert (since gnt.General.Icon is NULL).");
                }

                try
                {
                    SendEmailAlert(title, notificate, System.Net.Mail.MailPriority.Normal, emailRef);
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogFatal("[Chilkat Fatal Error] " + ex.Message);
                    SiAuto.Main.LogException(ex);
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.PostUpdateNotificate");
        }

        #region Windows Phone Alerts
        /// <summary>
        /// Sends a notification to a Windows Phone running Toasty.
        /// </summary>
        /// <param name="message">The message body.</param>
        /// <param name="healthTitle">The health alert title.</param>
        /// <param name="isCritical">Set to true to indicate a critical alert.</param>
        private void SendToastyAlert(String message, String healthTitle, bool isCritical)
        {
            SendToastyAlert(message, healthTitle, isCritical, String.Empty);
        }

        /// <summary>
        /// Sends a notification to a Windows Phone running Toasty.
        /// </summary>
        /// <param name="message">The message body.</param>
        /// <param name="healthTitle">The health alert title.</param>
        /// <param name="isCritical">Set to true to indicate a critical alert.</param>
        /// <param name="explicitIcon">A String that specifies the explicit icon to use. To let WindowSMART decide, set to String.Empty.</param>
        private void SendToastyAlert(String message, String healthTitle, bool isCritical, String explicitIcon)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SendToastyAlert");
            SiAuto.Main.LogBool("isToastyEnabled", isToastyEnabled);
            SiAuto.Main.LogString("toastyApiKey", toastyApiKey);
            String servername = Environment.MachineName;

            if (!isToastyEnabled)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendToastyAlert");
                return;
            }

            String[] guids = toastyApiKey.Split(',');
            List<String> rancidGuids = new List<String>();

            foreach (String guid in guids)
            {
                try
                {
                    SiAuto.Main.LogMessage("Composing Toasty alert message for device ID " + guid);

                    // Construct the URI for calling the service.
                    String deviceID = guid;
                    String title = healthTitle;
                    String image = String.Empty;
                    String text = message.ToLower();

                    if (String.IsNullOrEmpty(explicitIcon))
                    {
                        if (text.Contains("cleared alert"))
                        {
                            image = Properties.Resources.RemoteNotifyIconGeneral;
                        }
                        else if (isCritical && text.Contains("smart threshold exceeds condition"))
                        {
                            image = Properties.Resources.RemoteNotifyIconHyperfatal;
                        }
                        else if (text.Contains("smart value is equal to the non-zero threshold"))
                        {
                            image = Properties.Resources.RemoteNotifyIconWarning;
                        }
                        else if (text.Contains("licensing system detected tampering with the license"))
                        {
                            image = Properties.Resources.RemoteNotifyIconHyperfatal;
                        }
                        else if (text.Contains("trial has expired"))
                        {
                            image = Properties.Resources.RemoteNotifyIconHyperfatal;
                        }
                        else if (isCritical)
                        {
                            image = Properties.Resources.RemoteNotifyIconCritical;
                        }
                        else
                        {
                            image = Properties.Resources.RemoteNotifyIconWarning;
                        }
                    }
                    else
                    {
                        image = explicitIcon;
                    }

                    String parameters = deviceID + Properties.Resources.ToastyParamDeviceID + deviceID + Properties.Resources.ToastyParamTitle + HttpUtility.UrlEncode(title) +
                        Properties.Resources.ToastyParamText + HttpUtility.UrlEncode(message) + Properties.Resources.ToastyParamImage + HttpUtility.UrlEncode(image) + Properties.Resources.ToastyParamSender;

                    String uri = Properties.Resources.ToastyApiUri + parameters;

                    SiAuto.Main.LogString("uri", uri);

                    // Create the HTTP GET request
                    SiAuto.Main.LogMessage("Create the HTTP GET request.");
                    WebRequest req = WebRequest.Create(uri);
                    SiAuto.Main.LogMessage("Get the HTTP response.");
                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                    SiAuto.Main.LogMessage("Read the response.");
                    System.IO.StreamReader reader = new System.IO.StreamReader(resp.GetResponseStream());
                    String response = reader.ReadToEnd();
                    reader.Close();
                    resp.Close();

                    SiAuto.Main.LogMessage("Toasty web service returned status: " + response);
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogError("Toasty web service returned an error: " + ex.Message);
                    SiAuto.Main.LogError("Error returned while processing device ID " + guid);
                    SiAuto.Main.LogException(ex);
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendToastyAlert");
        }

        /// <summary>
        /// Sends a notification to a Windows Phone running Toasty.
        /// </summary>
        /// <param name="diskName">Disk name.</param>
        /// <param name="diskPath">Disk path.</param>
        /// <param name="attributeID">Attribute ID.</param>
        /// <param name="healthTitle">Health message title.</param>
        /// <param name="attributeName">SMART attribute name.</param>
        /// <param name="healthMessage">Message body.</param>
        /// <param name="isCritical">Set to true to indicate a critical alert.</param>
        /// <param name="action">The AlertAction enum that identifies the action state of the alert.</param>
        private void SendToastyAlert(String diskName, String diskPath, int attributeID, String healthTitle, String attributeName,
            String healthMessage, bool isCritical, AlertAction action)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SendToastyAlert (overload)");
            SiAuto.Main.LogBool("isToastyEnabled", isToastyEnabled);
            SiAuto.Main.LogString("toastyApiKey", toastyApiKey);
            String servername = Environment.MachineName;

            if (!isToastyEnabled)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendToastyAlert (overload)");
                return;
            }

            SiAuto.Main.LogMessage("Construct the alert message.");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (action == AlertAction.Remove)
            {
                sb.Append("Cleared Alert: ");
            }
            else
            {
                sb.Append(isCritical ? "Critical Alert: " : "Warning Alert: ");
            }
            sb.Append((isWindowsServerSolutions ? "Server " : "Computer ") + servername + ", ");
            sb.Append("Disk " + diskName + ", Path " + diskPath + ", attribute " + attributeID.ToString() + " (" + attributeName + ") - ");
            sb.Append(healthMessage);
            if (action == AlertAction.Update)
            {
                sb.Append(" (This is an update to an existing alert - the alert condition has changed.)");
            }

            SendToastyAlert(sb.ToString(), healthTitle, isCritical);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendToastyAlert (overload)");
        }
        #endregion

        #region Growl Alerts
        private void SendGrowlAlert(String diskName, String diskPath, int attributeID, String healthTitle, String attributeName, String healthMessage, bool isCritical,
            AlertAction action)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SendGrowlAlert (overload)");
            SiAuto.Main.LogBool("isGrowlEnabled", isGrowlEnabled);
            SiAuto.Main.LogBool("isGrowlRemoteEnabled", isGrowlRemoteEnabled);
            String servername = Environment.MachineName;

            if (!isGrowlEnabled)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendGrowlAlert (overload)");
                return;
            }

            SiAuto.Main.LogMessage("Construct the alert message.");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append(healthTitle + ": ");

            if (action == AlertAction.Remove)
            {
                sb.Append("Cleared Alert: ");
            }
            else
            {
                sb.Append(isCritical ? "Critical Alert: " : "Warning Alert: ");
            }
            sb.Append((isWindowsServerSolutions ? "Server " : "Computer ") + servername + ", ");
            sb.Append("Disk " + diskName + ", Path " + diskPath + ", attribute " + attributeID.ToString() + " (" + attributeName + ") - ");
            sb.Append(healthMessage);
            if (action == AlertAction.Update)
            {
                sb.Append(" (This is an update to an existing alert - the alert condition has changed.)");
            }

            if (action == AlertAction.Remove)
            {
                try
                {
                    if (gnt != null)
                    {
                        SendGrowlAlert(sb.ToString(), healthTitle, isCritical, gnt.Cleared.Icon, Growl.Connector.Priority.VeryLow);
                    }
                    else
                    {
                        SiAuto.Main.LogWarning("Growl gnt flag is null; skipping growl alert (since gnt.Cleared.Icon is NULL).");
                    }
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogError("Unable to send Growl cleared alert: " + ex.Message);
                    SiAuto.Main.LogException(ex);
                }
            }
            else
            {
                try
                {
                    SendGrowlAlert(sb.ToString(), healthTitle, isCritical, null, Growl.Connector.Priority.VeryLow);
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogError("Unable to send Growl alert: " + ex.Message);
                    SiAuto.Main.LogException(ex);
                }
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendGrowlAlert (overload)");
        }

        /// <summary>
        /// Sends a Growl alert.
        /// </summary>
        /// <param name="message">The body of the message.</param>
        /// <param name="title">The title of the message.</param>
        /// <param name="isCritical">Set to true to specify the alert is critical.</param>
        /// <param name="res">The icon to use. Specify null to let WindowSMART decide based on content.</param>
        /// <param name="priority">The Growl priority level. Set to VeryLow to let WindowSMART decide based on content.</param>
        private void SendGrowlAlert(String message, String title, bool isCritical, Growl.CoreLibrary.Resource res, Growl.Connector.Priority priority)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SendGrowlAlert");
            if (!isGrowlEnabled)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendGrowlAlert");
                return;
            }

            if (growl == null)
            {
                InitializeGrowl();
            }

            String text = message.ToLower();

            if (res == null)
            {
                if (text.Contains("cleared alert"))
                {
                    res = gnt.General.Icon;
                }
                else if (isCritical && text.Contains("smart threshold exceeds condition"))
                {
                    res = gnt.Hyperfatal.Icon;
                }
                else if (text.Contains("smart value is equal to the non-zero threshold"))
                {
                    res = gnt.Warning.Icon;
                }
                else if (text.Contains("licensing system detected tampering with the license"))
                {
                    res = gnt.Hyperfatal.Icon;
                }
                else if (text.Contains("trial has expired"))
                {
                    res = gnt.Hyperfatal.Icon;
                }
                else if (isCritical)
                {
                    res = gnt.Critical.Icon;
                }
                else
                {
                    res = gnt.Warning.Icon;
                }
            }

            if (priority == Growl.Connector.Priority.VeryLow)
            {
                if (text.Contains("cleared alert"))
                {
                    priority = Growl.Connector.Priority.Normal;
                }
                else if (isCritical && text.Contains("smart threshold exceeds condition"))
                {
                    priority = Growl.Connector.Priority.Emergency;
                }
                else if (text.Contains("smart value is equal to the non-zero threshold"))
                {
                    priority = Growl.Connector.Priority.Moderate;
                }
                else if (text.Contains("licensing system detected tampering with the license"))
                {
                    priority = Growl.Connector.Priority.High;
                }
                else if (text.Contains("trial has expired"))
                {
                    priority = Growl.Connector.Priority.High;
                }
                else if (isCritical)
                {
                    priority = Growl.Connector.Priority.High;
                }
                else
                {
                    priority = Growl.Connector.Priority.Moderate;
                }
            }

            try
            {
                message += (isGrowlRemoteEnabled ? (isWindowsServerSolutions ? " Server: " : " Computer: ") + Environment.MachineName : String.Empty);

                Notification growlNotificate = new Notification(isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart,
                        gnt.AppWarning.Name, DateTime.Now.Ticks.ToString(), title, message, res, false, priority, String.Empty);
                growl.Notify(growlNotificate);
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to generate Growl notificate or send alert: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendGrowlAlert");
        }
        #endregion

        #region Snarl Alerts
        private void SendSnarlAlert(String diskName, String diskPath, int attributeID, String healthTitle, String attributeName, String healthMessage, bool isCritical,
            AlertAction action)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SendSnarlAlert (overload)");
            SiAuto.Main.LogBool("isSnarlEnabled", isSnarlEnabled);
            String servername = Environment.MachineName;

            if (!isSnarlEnabled)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendSnarlAlert (overload)");
                return;
            }

            SiAuto.Main.LogMessage("Construct the alert message.");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append(healthTitle + ": ");

            if (action == AlertAction.Remove)
            {
                sb.Append("Cleared Alert: ");
            }
            else
            {
                sb.Append(isCritical ? "Critical Alert: " : "Warning Alert: ");
            }
            sb.Append(", ");
            sb.Append("Disk " + diskName + ", Path " + diskPath + ", attribute " + attributeID.ToString() + " (" + attributeName + ") - ");
            sb.Append(healthMessage);
            if (action == AlertAction.Update)
            {
                sb.Append(" (This is an update to an existing alert - the alert condition has changed.)");
            }

            if (action == AlertAction.Remove)
            {
                try
                {
                    if (snt != null)
                    {
                        SendSnarlAlert(sb.ToString(), healthTitle, isCritical, snt.Cleared.Icon, Growl.Connector.Priority.VeryLow);
                    }
                    else
                    {
                        SiAuto.Main.LogWarning("Snarl snt flag is null; skipping Snarl alert (since snt.Cleared.Icon is NULL).");
                    }
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogError("Unable to send Snarl cleared alert: " + ex.Message);
                    SiAuto.Main.LogException(ex);
                }
            }
            else
            {
                try
                {
                    SendSnarlAlert(sb.ToString(), healthTitle, isCritical, null, Growl.Connector.Priority.VeryLow);
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogError("Unable to send Snarl alert: " + ex.Message);
                    SiAuto.Main.LogException(ex);
                }
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendSnarlAlert (overload)");
        }

        /// <summary>
        /// Sends a Snarl alert.
        /// </summary>
        /// <param name="message">The body of the message.</param>
        /// <param name="title">The title of the message.</param>
        /// <param name="isCritical">Set to true to specify the alert is critical.</param>
        /// <param name="res">The icon to use. Specify null to let WindowSMART decide based on content.</param>
        /// <param name="priority">The Growl priority level. Set to VeryLow to let WindowSMART decide based on content.</param>
        private void SendSnarlAlert(String message, String title, bool isCritical, Growl.CoreLibrary.Resource res, Growl.Connector.Priority priority)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SendSnarlAlert");
            if (!isSnarlEnabled)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendSnarlAlert");
                return;
            }

            if (snarl == null)
            {
                InitializeSnarl();
            }

            String text = message.ToLower();

            if (res == null)
            {
                if (text.Contains("cleared alert"))
                {
                    res = snt.General.Icon;
                }
                else if (isCritical && text.Contains("smart threshold exceeds condition"))
                {
                    res = snt.Hyperfatal.Icon;
                }
                else if (text.Contains("smart value is equal to the non-zero threshold"))
                {
                    res = snt.Warning.Icon;
                }
                else if (text.Contains("licensing system detected tampering with the license"))
                {
                    res = snt.Hyperfatal.Icon;
                }
                else if (text.Contains("trial has expired"))
                {
                    res = snt.Hyperfatal.Icon;
                }
                else if (isCritical)
                {
                    res = snt.Critical.Icon;
                }
                else
                {
                    res = snt.Warning.Icon;
                }
            }

            if (priority == Growl.Connector.Priority.VeryLow)
            {
                if (text.Contains("cleared alert"))
                {
                    priority = Growl.Connector.Priority.Normal;
                }
                else if (isCritical && text.Contains("smart threshold exceeds condition"))
                {
                    priority = Growl.Connector.Priority.Emergency;
                }
                else if (text.Contains("smart value is equal to the non-zero threshold"))
                {
                    priority = Growl.Connector.Priority.Moderate;
                }
                else if (text.Contains("licensing system detected tampering with the license"))
                {
                    priority = Growl.Connector.Priority.High;
                }
                else if (text.Contains("trial has expired"))
                {
                    priority = Growl.Connector.Priority.High;
                }
                else if (isCritical)
                {
                    priority = Growl.Connector.Priority.High;
                }
                else
                {
                    priority = Growl.Connector.Priority.Moderate;
                }
            }

            try
            {
                message += (isWindowsServerSolutions ? " Server: " : " Computer: ") + Environment.MachineName;

                Notification snarlNotificate = new Notification(isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart,
                        snt.AppWarning.Name, DateTime.Now.Ticks.ToString(), title, message, res, false, priority, String.Empty);
                snarl.Notify(snarlNotificate);
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to generate Snarl notificate or send alert: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendSnarlAlert");
        }
        #endregion

        #region Pushover (iOS/Android) Alerts
        private void SendPushoverAlert(String message, String title)
        {
            SendPushoverAlert(message, title, true, false);
        }

        private void SendPushoverAlert(String message, String title, bool isCleared, bool isCritical)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SendPushoverAlert");
            SiAuto.Main.LogBool("isPushoverEnabled", isPushoverEnabled);
            SiAuto.Main.LogString("pushoverUserKey", pushoverUserKey);

            if (!isPushoverEnabled)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendPushoverAlert");
                return;
            }

            SiAuto.Main.LogString("message", message);
            SiAuto.Main.LogString("title", title);
            SiAuto.Main.LogBool("isCleared", isCleared);
            SiAuto.Main.LogBool("isCritical", isCritical);

            PushoverSound sound = PushoverSound.DeviceDefault;
            if (isCleared)
            {
                sound = Utilities.Utility.ResolveSoundFromString(pushoverClearedSound);
            }
            else if (isCritical)
            {
                sound = Utilities.Utility.ResolveSoundFromString(pushoverCriticalSound);
            }
            else // warnings
            {
                sound = Utilities.Utility.ResolveSoundFromString(pushoverWarningSound);
            }
            SiAuto.Main.LogObjectValue("sound", sound);

            Exception except;

            bool push = Pushover.Pushover.SendNotification(Properties.Resources.PushoverApiToken, pushoverUserKey, title, message, (isCritical ? Pushover.Priority.High : Pushover.Priority.Normal),
                sound, out except);

            if (push)
            {
                SiAuto.Main.LogMessage("Pushover notification sent successfully.");
            }
            else
            {
                SiAuto.Main.LogError("Pushover notification failed. If an exception is available it will be displayed next.");
                if (except != null)
                {
                    SiAuto.Main.LogException(except);
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendPushoverAlert (overload)");
        }

        private void SendPushoverAlert(String diskName, String diskPath, int attributeID, String healthTitle, String attributeName,
            String healthMessage, bool isCritical, AlertAction action)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SendPushoverAlert (overload)");
            SiAuto.Main.LogBool("isPushoverEnabled", isPushoverEnabled);
            SiAuto.Main.LogString("pushoverUserKey", pushoverUserKey);
            String servername = Environment.MachineName;

            if (!isPushoverEnabled)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendPushoverAlert (overload)");
                return;
            }

            SiAuto.Main.LogMessage("Construct the alert message.");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (action == AlertAction.Remove)
            {
                sb.Append("Cleared Alert: ");
            }
            else
            {
                sb.Append(isCritical ? "Critical Alert: " : "Warning Alert: ");
            }
            sb.Append((isWindowsServerSolutions ? "Server " : "Computer ") + servername + ", ");
            sb.Append("Disk " + diskName + ", Path " + diskPath + ", attribute " + attributeID.ToString() + " (" + attributeName + ") - ");
            sb.Append(healthMessage);
            if (action == AlertAction.Update)
            {
                sb.Append(" (This is an update to an existing alert - the alert condition has changed.)");
            }

            SendPushoverAlert(sb.ToString(), healthTitle, action == AlertAction.Remove, isCritical);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendPushoverAlert (overload)");
        }
        #endregion

        #region iOS Device (Prowl) Alerts
        private void SendProwlAlert(String message, String healthTitle, bool isCritical)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SendProwlAlert");
            SiAuto.Main.LogBool("isProwlEnabled", isProwlEnabled);
            SiAuto.Main.LogString("prowlApiKey", prowlApiKey);
            String servername = Environment.MachineName;

            if (!isProwlEnabled)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendProwlAlert");
                return;
            }

            try
            {
                SiAuto.Main.LogMessage("Create the ProwlClientConfiguration object.");
                ProwlClientConfiguration config = new ProwlClientConfiguration();

                config.ApplicationName = isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart;
                config.ProviderKey = isWindowsServerSolutions ? Properties.Resources.ProwlApiKeyHss : Properties.Resources.ProwlApiKeyWs2;
                config.ApiKeychain = prowlApiKey;

                SiAuto.Main.LogString("config.ApplicationName", config.ApplicationName);
                SiAuto.Main.LogString("config.ApiKeychain", config.ApiKeychain);

                SiAuto.Main.LogMessage("Create the ProwlNotification object.");
                ProwlNotification notificate = new ProwlNotification();

                notificate.Description = message + (String.IsNullOrEmpty(customNotificate) ? String.Empty : " " + customNotificate);
                notificate.Priority = isCritical ? ProwlNotificationPriority.High : ProwlNotificationPriority.Normal;
                notificate.Event = healthTitle;

                SiAuto.Main.LogString("notificate.Description", notificate.Description);
                SiAuto.Main.LogString("notificate.Event", healthTitle);

                SiAuto.Main.LogMessage("Create the ProwlClient object with ProwlClientConfiguration.");
                ProwlClient client = new ProwlClient(config);

                SiAuto.Main.LogMessage("Attempt to send Prowl notification.");
                client.PostNotification(notificate);
                SiAuto.Main.LogMessage("Prowl notification was sent successfully, or no error was detected on the attempt.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Sending the Prowl notification failed. " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendProwlAlert");
        }

        private void SendProwlAlert(String diskName, String diskPath, int attributeID, String healthTitle, String attributeName,
            String healthMessage, bool isCritical, AlertAction action)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SendProwlAlert (overload)");
            SiAuto.Main.LogBool("isProwlEnabled", isProwlEnabled);
            SiAuto.Main.LogString("prowlApiKey", prowlApiKey);
            String servername = Environment.MachineName;

            if (!isProwlEnabled)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendProwlAlert (overload)");
                return;
            }

            SiAuto.Main.LogMessage("Construct the alert message.");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (action == AlertAction.Remove)
            {
                sb.Append("Cleared Alert: ");
            }
            else
            {
                sb.Append(isCritical ? "Critical Alert: " : "Warning Alert: ");
            }
            sb.Append((isWindowsServerSolutions ? "Server " : "Computer ") + servername + ", ");
            sb.Append("Disk " + diskName + ", Path " + diskPath + ", attribute " + attributeID.ToString() + " (" + attributeName + ") - ");
            sb.Append(healthMessage);
            if (action == AlertAction.Update)
            {
                sb.Append(" (This is an update to an existing alert - the alert condition has changed.)");
            }

            SendProwlAlert(sb.ToString(), healthTitle, isCritical);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendProwlAlert (overload)");
        }
        #endregion

        #region Android Alerts
        private void SendNmaAlert(String message, String healthTitle, bool isCritical)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SendNmaAlert");
            SiAuto.Main.LogBool("isNmaEnabled", isNmaEnabled);
            SiAuto.Main.LogString("nmaApiKey", nmaApiKey);
            String servername = Environment.MachineName;

            if (!isNmaEnabled)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendNmaAlert");
                return;
            }

            try
            {
                SiAuto.Main.LogMessage("Create the NMAClientConfiguration object.");
                NMAClientConfiguration config = new NMAClientConfiguration();

                config.ApplicationName = isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart;
                config.ProviderKey = String.Empty;
                config.ApiKeychain = nmaApiKey;

                SiAuto.Main.LogString("config.ApplicationName", config.ApplicationName);
                SiAuto.Main.LogString("config.ApiKeychain", config.ApiKeychain);

                SiAuto.Main.LogMessage("Create the NMANotification object.");
                NMANotification notificate = new NMANotification();

                notificate.Description = message + (String.IsNullOrEmpty(customNotificate) ? String.Empty : " " + customNotificate);
                notificate.Priority = isCritical ? NMANotificationPriority.High : NMANotificationPriority.Normal;
                notificate.Event = healthTitle;

                SiAuto.Main.LogString("notificate.Description", notificate.Description);
                SiAuto.Main.LogString("notificate.Event", healthTitle);

                SiAuto.Main.LogMessage("Create the NMAClient object with NMAClientConfiguration.");
                NMAClient client = new NMAClient(config);

                SiAuto.Main.LogMessage("Attempt to send NMA notification.");
                client.PostNotification(notificate);
                SiAuto.Main.LogMessage("NMA notification was sent successfully, or no error was detected on the attempt.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Sending the NMA notification failed. " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendNmaAlert");
        }

        private void SendNmaAlert(String diskName, String diskPath, int attributeID, String healthTitle, String attributeName,
            String healthMessage, bool isCritical, AlertAction action)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SendNmaAlert (overload)");
            SiAuto.Main.LogBool("isNmaEnabled", isNmaEnabled);
            SiAuto.Main.LogString("nmaApiKey", nmaApiKey);
            String servername = Environment.MachineName;

            if (!isNmaEnabled)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendNmaAlert (overload)");
                return;
            }

            SiAuto.Main.LogMessage("Construct the alert message.");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (action == AlertAction.Remove)
            {
                sb.Append("Cleared Alert: ");
            }
            else
            {
                sb.Append(isCritical ? "Critical Alert: " : "Warning Alert: ");
            }
            sb.Append((isWindowsServerSolutions ? "Server " : "Computer ") + servername + ", ");
            sb.Append("Disk " + diskName + ", Path " + diskPath + ", attribute " + attributeID.ToString() + " (" + attributeName + ") - ");
            sb.Append(healthMessage);
            if (action == AlertAction.Update)
            {
                sb.Append(" (This is an update to an existing alert - the alert condition has changed.)");
            }

            SendNmaAlert(sb.ToString(), healthTitle, isCritical);

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendNmaAlert (overload)");
        }
        #endregion

        #region Email Alerts
        private void SendEmailAlert(String subject, String body, System.Net.Mail.MailPriority priority, String guid)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SendEmailAlert");
            SiAuto.Main.LogBool("isMailConfigValid", isMailConfigValid);
            SiAuto.Main.LogBool("mailAlertsEnabled", mailAlertsEnabled);
            if (!isMailConfigValid || !mailAlertsEnabled)
            {
                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Mail config invalid or alerts disabled. No mail will be sent.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendEmailAlert");
                return;
            }

            try
            {
                // Set up the Chilkat.MailMan object and license it.
                SiAuto.Main.LogMessage("Set up the Chilkat.MailMan object and license it.");
                MailMan mailman = new MailMan();
                bool success = mailman.UnlockComponent("DOJONO.CB1012021_bKaXHW7WockC");

                if (!success)
                {
                    SiAuto.Main.LogError("Failed to activate MailMan object: " + mailman.LastErrorText);
                    throw new UnauthorizedAccessException("Cannot activate Chilkat.MailMan email module. The licensing module encountered an exception.");
                }

                // Point to the mail server.
                // Set the SMTP Server and SSL options.
                mailman.SmtpHost = mailServer;
                mailman.SmtpPort = serverPort;
                mailman.SmtpSsl = useSsl;

                // Creds
                if (authenticationEnabled)
                {
                    mailman.SmtpUsername = mailUser;
                    mailman.SmtpPassword = mailPassword;
                }

                // Create a new email object.
                SiAuto.Main.LogMessage("Compose email message.");
                Email msg = new Email();
                SiAuto.Main.LogString("senderEmailAddress", senderEmailAddress);
                SiAuto.Main.LogString("senderFriendlyName", senderFriendlyName);
                msg.From = senderFriendlyName + " <" + senderEmailAddress + ">";
                SiAuto.Main.LogString("recipientEmailAddress", recipientEmailAddress);
                SiAuto.Main.LogString("recipientFriendlyName", recipientFriendlyName);
                msg.AddTo(recipientFriendlyName, recipientEmailAddress);
                if (!String.IsNullOrEmpty(recipientEmail2))
                {
                    SiAuto.Main.LogMessage("recipiientEmail2 is NOT null or empty.");
                    SiAuto.Main.LogString("recipientEmail2", recipientEmail2);
                    msg.AddCC(String.Empty, recipientEmail2);
                }
                if (!String.IsNullOrEmpty(recipientEmail3))
                {
                    SiAuto.Main.LogMessage("recipiientEmail3 is NOT null or empty.");
                    SiAuto.Main.LogString("recipientEmail3", recipientEmail3);
                    msg.AddCC(String.Empty, recipientEmail3);
                }

                msg.Subject = subject;

                if (sendPlaintext)
                {
                    msg.Body = body;
                }
                else
                {
                    msg.SetHtmlBody(body);
                }

                if (priority == System.Net.Mail.MailPriority.High)
                {
                    msg.AddHeaderField("X-Priority", "1");
                }
                else if (priority == System.Net.Mail.MailPriority.Low)
                {
                    msg.AddHeaderField("X-Priority", "5");
                }
                else
                {
                    msg.AddHeaderField("X-Priority", "3");
                }

                SiAuto.Main.LogObjectValue("Email message.", msg);
                
                //System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                //msg.From = new System.Net.Mail.MailAddress(senderEmailAddress, senderFriendlyName);
                //System.Net.Mail.MailAddress recipient = new System.Net.Mail.MailAddress(recipientEmailAddress, recipientFriendlyName);
                //msg.To.Add(recipient);
                //if (!String.IsNullOrEmpty(recipientEmail2))
                //{
                //    msg.CC.Add(new System.Net.Mail.MailAddress(recipientEmail2));
                //}
                //if (!String.IsNullOrEmpty(recipientEmail3))
                //{
                //    msg.CC.Add(new System.Net.Mail.MailAddress(recipientEmail3));
                //}
                //msg.IsBodyHtml = true;
                //msg.Body = body;
                //msg.Priority = priority;

                try
                {
                    SiAuto.Main.LogMessage("Send email message.");
                    //System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(mailServer, serverPort);
                    //client.UseDefaultCredentials = false;
                    //client.EnableSsl = useSsl;

                    //if (authenticationEnabled)
                    //{
                    //    SiAuto.Main.LogMessage("Authentication is enabled; user defined creds will be used.");
                    //    client.Credentials = new System.Net.NetworkCredential(mailUser, mailPassword);
                    //}
                    //else
                    //{
                    //    SiAuto.Main.LogMessage("No creds were defined.");
                    //    client.Credentials = null;
                    //}
                    SiAuto.Main.LogMessage("Attempting to send message.");
                    //client.Send(msg);
                    bool sendSuccess = mailman.SendEmail(msg);
                    if (sendSuccess)
                    {
                        SiAuto.Main.LogMessage("Email sent successfully.");
                    }
                    else
                    {
                        SiAuto.Main.LogError("Email send failed: " + mailman.LastErrorText);
                    }
                    mailman.CloseSmtpConnection();
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogError("Failed to send email. Correlation ID=" + guid);
                    SiAuto.Main.LogException(ex);
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to compose email. Correlation ID=" + guid);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendEmailAlert");
        }

        private void SendEmailAlert(String diskName, String diskPath, int attributeID, String attributeName, String healthMessage, bool isCritical, String guid, AlertAction action)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SendEmailAlert (overload)");
            SiAuto.Main.LogBool("isMailConfigValid", isMailConfigValid);
            SiAuto.Main.LogBool("mailAlertsEnabled", mailAlertsEnabled);
            if (!isMailConfigValid || !mailAlertsEnabled)
            {
                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "Mail config invalid or alerts disabled. No mail will be sent.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendEmailAlert (overload)");
                return;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            String servername = Environment.MachineName;

            SiAuto.Main.LogObjectValue("AlertAction", action);
            switch (action)
            {
                case AlertAction.Insert:
                    {
                        if (sendPlaintext)
                        {
                            sb.Append((isWindowsServerSolutions ? "Home Server SMART 24/7 " : "WindowSMART 24/7 ") + (isCritical ? "Critical Alert " : "Warning Alert ") + "\r\n\r\n");
                            sb.Append((isWindowsServerSolutions ? "Server: " : "Computer: ") + servername + "\r\n");
                            sb.Append("Disk: " + diskName + "\r\n");
                            sb.Append("Path: " + diskPath + "\r\n");
                            sb.Append("Attribute ID: " + attributeID.ToString() + "\r\n");
                            sb.Append("Attribute Name: " + attributeName + "\r\n");
                            sb.Append("Alert Status: " + (isCritical ? "Critical " : "Warning") + "\r\n");
                            sb.Append("Message: " + healthMessage + "\r\n\r\n");
                        }
                        else
                        {
                            sb.Append("<h2>" + (isWindowsServerSolutions ? "Home Server SMART 24/7 " : "WindowSMART 24/7 ") + (isCritical ? "<font color=\"red\">Critical Alert</font> " : "<font color=\"orange\">Warning Alert</font> ") + "</h2><hr/>");
                            sb.Append("<b>" + (isWindowsServerSolutions ? "Server: " : "Computer: ") + "</b> " + servername + "<br/>");
                            sb.Append("<b>Disk: </b>" + diskName + "<br/>");
                            sb.Append("<b>Path: </b>" + diskPath + "<br/>");
                            sb.Append("<b>Attribute ID: </b>" + attributeID.ToString() + "<br/>");
                            sb.Append("<b>Attribute Name: </b>" + attributeName + "<br/>");
                            sb.Append("<b>Alert Status: </b>" + (isCritical ? "<font color=\"red\">Critical</font> " : "<font color=\"orange\">Warning</font>") + "<br/>");
                            sb.Append("<b>Message: </b>" + healthMessage + "<br/><br/>");
                        }
                        SiAuto.Main.LogColored(isCritical ? System.Drawing.Color.Red : System.Drawing.Color.Yellow,
                            (isCritical ? "[Feverish Disk Critical Notificate]" : "[Feverish Disk Degraded Notificate]") + " Disk=" + diskName + ", Path=" + diskPath +
                            ", Message=" + healthMessage);
                        break;
                    }
                case AlertAction.Remove:
                    {
                        if (sendPlaintext)
                        {
                            sb.Append((isWindowsServerSolutions ? "Home Server SMART 24/7 " : "WindowSMART 24/7 ") + (isCritical ? "Critical Alert " : "Warning Alert ") + " - CLEARED\r\n\r\n");
                            sb.Append("" + (isWindowsServerSolutions ? "Server: " : "Computer: ") + " " + servername + "\r\n");
                            sb.Append("Disk: " + diskName + "\r\n");
                            sb.Append("Path: " + diskPath + "\r\n");
                            sb.Append("Attribute ID: " + attributeID.ToString() + "\r\n");
                            sb.Append("Attribute Name: " + attributeName + "\r\n");
                            sb.Append("Alert Status: " + "CLEARED\r\n");
                            sb.Append("Message: " + healthMessage + " (This issue has CLEARED.)\r\n\r\n");
                        }
                        else
                        {
                            sb.Append("<h2>" + (isWindowsServerSolutions ? "Home Server SMART 24/7 " : "WindowSMART 24/7 ") + (isCritical ? "Critical Alert " : "Warning Alert ") + " - CLEARED</h2><hr/>");
                            sb.Append("<b>" + (isWindowsServerSolutions ? "Server: " : "Computer: ") + "</b> " + servername + "<br/>");
                            sb.Append("<b>Disk: </b>" + diskName + "<br/>");
                            sb.Append("<b>Path: </b>" + diskPath + "<br/>");
                            sb.Append("<b>Attribute ID: </b>" + attributeID.ToString() + "<br/>");
                            sb.Append("<b>Attribute Name: </b>" + attributeName + "<br/>");
                            sb.Append("<b>Alert Status: </b>" + "<font color=\"green\">CLEARED</font><br/>");
                            sb.Append("<b>Message: </b>" + healthMessage + " (This issue has CLEARED.)<br/><br/>");
                        }
                        SiAuto.Main.LogColored(System.Drawing.Color.Green,
                            (isCritical ? "[Feverish Disk Critical Cleared]" : "[Feverish Disk Degraded Cleared]") + " Disk=" + diskName + ", Path=" + diskPath +
                            ", Message=" + healthMessage);
                        break;
                    }
                case AlertAction.Update:
                    {
                        if (sendPlaintext)
                        {
                            sb.Append((isWindowsServerSolutions ? "Home Server SMART 24/7 " : "WindowSMART 24/7 ") + (isCritical ? "Critical Update " : "Warning Update ") + "\r\n\r\n");
                            sb.Append("*** AN ALERT CONDITION ON THIS DISK HAS CHANGED ***\r\n");
                            sb.Append("" + (isWindowsServerSolutions ? "Server: " : "Computer: ") + " " + servername + "\r\n");
                            sb.Append("Disk: " + diskName + "\r\n");
                            sb.Append("Path: " + diskPath + "\r\n");
                            sb.Append("Attribute ID: " + attributeID.ToString() + "\r\n");
                            sb.Append("Attribute Name: " + attributeName + "\r\n");
                            sb.Append("Alert Status: " + (isCritical ? "<font color=\"red\">Critical - UPDATED</font> " : "<font color=\"orange\">Warning - UPDATED</font>") + "\r\n");
                            sb.Append("Message: " + healthMessage + "\r\n\r\n");
                        }
                        else
                        {
                            sb.Append("<h2>" + (isWindowsServerSolutions ? "Home Server SMART 24/7 " : "WindowSMART 24/7 ") + (isCritical ? "<font color=\"red\">Critical Update</font> " : "<font color=\"orange\">Warning Update</font> ") + "</h2><hr/>");
                            sb.Append("*** AN ALERT CONDITION ON THIS DISK HAS CHANGED ***<br/>");
                            sb.Append("<b>" + (isWindowsServerSolutions ? "Server: " : "Computer: ") + "</b> " + servername + "<br/>");
                            sb.Append("<b>Disk: </b>" + diskName + "<br/>");
                            sb.Append("<b>Path: </b>" + diskPath + "<br/>");
                            sb.Append("<b>Attribute ID: </b>" + attributeID.ToString() + "<br/>");
                            sb.Append("<b>Attribute Name: </b>" + attributeName + "<br/>");
                            sb.Append("<b>Alert Status: </b>" + (isCritical ? "<font color=\"red\">Critical - UPDATED</font> " : "<font color=\"orange\">Warning - UPDATED</font>") + "<br/>");
                            sb.Append("<b>Message: </b>" + healthMessage + "<br/><br/>");
                        }
                        SiAuto.Main.LogColored(isCritical ? System.Drawing.Color.Red : System.Drawing.Color.Yellow,
                            (isCritical ? "[Feverish Disk Critical Notificate]" : "[Feverish Disk Degraded Notificate]") + " Disk=" + diskName + ", Path=" + diskPath +
                            ", Message=" + healthMessage);
                        break;
                    }
                case AlertAction.Hold:
                default:
                    {
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendEmailAlert");
                        return;
                    }
            }
            if (!String.IsNullOrEmpty(customNotificate))
            {
                if (sendPlaintext)
                {
                    sb.Append(customNotificate + "\r\n\r\n");
                }
                else
                {
                    sb.Append(customNotificate + "<br/><br/>");
                }
            }
            if (sendPlaintext)
            {
                sb.Append("\r\nDate/Time Generated: " + DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToLongTimeString() + "\r\n");
                sb.Append("Correlation ID: " + guid + "\r\n\r\n");
                sb.Append("This email was automatically generated by " + (isWindowsServerSolutions ? "Home Server SMART 24/7." : "WindowSMART 24/7.") + " Please do not reply to it.");
            }
            else
            {
                sb.Append("<hr/><small><i>Date/Time Generated: " + DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToLongTimeString() + "<br/>");
                sb.Append("Correlation ID: " + guid + "<br/><br/></i>");
                sb.Append("This email was automatically generated by " + (isWindowsServerSolutions ? "Home Server SMART 24/7." : "WindowSMART 24/7.") + " Please do not reply to it.</small>");
            }

            String subject = String.Empty;
            if (isWindowsServerSolutions)
            {
                subject = ((action == AlertAction.Insert || action == AlertAction.Update) ?
                    "Home Server SMART Disk Health Alert" : "Home Server SMART Disk Health Alert Cleared");
            }
            else
            {
                subject = ((action == AlertAction.Insert || action == AlertAction.Update) ?
                    "WindowSMART Disk Health Alert" : "WindowSMART Disk Health Alert Cleared");
            }

            try
            {
                SendEmailAlert(subject, sb.ToString(), (((action == AlertAction.Insert || action == AlertAction.Update) && isCritical) ?
                        System.Net.Mail.MailPriority.High : System.Net.Mail.MailPriority.Normal), guid);
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogFatal("[Chilkat Fatal Error] " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SendEmailAlert (overload)");
        }
        #endregion

        #region Singleton Instance Initialization

        /// <summary>Lock synchronization object used in Singleton initialization.</summary> 
        private static object syncLock = new object();
        /// <summary>Singleton instance of the ComplianceService.</summary>
        private static HssServiceHelper instance;

        public static HssServiceHelper GetInstance(String xmlText, DateTime result, Guid refGuid, uint retVal)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.GetInstance");
            if (instance == null)
            {
                lock (syncLock)
                {
                    if (instance == null)
                    {
                        instance = new HssServiceHelper();
                        instance.ConfigureInstance(xmlText, result, refGuid, retVal);
                    }
                }
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.GetInstance");
            return instance;
        }

        #endregion

        #region Daily Summary
        private void ProcessDailySummaryData(bool doServiceRecycle)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.ProcessDailySummaryData");
            bool recycleNeeded = false;
            // Read
            summary.ReadCurrentFile(xmlSummaryFile);
            // Process
            SiAuto.Main.LogInt("eventCount", summary.EventCount);
            SiAuto.Main.LogBool("needSummarySend", summary.NeedSummarySend);
            SiAuto.Main.LogBool("doServiceRecycle", doServiceRecycle);
            SiAuto.Main.LogBool("recycleNeeded", recycleNeeded);
            if (summary.NeedSummarySend)
            {
                // Write to flush in-memory alerts that aren't in the file yet.
                SiAuto.Main.LogMessage("Flush to the daily summary file to ensure in-memory alerts are captured.");
                summary.ComposeNewFile(xmlSummaryFile, true);
                SiAuto.Main.LogBool("sendDailySummary", sendDailySummary);
                SiAuto.Main.LogBool("sendPlaintext", sendPlaintext);
                if (sendDailySummary)
                {
                    try
                    {
                        SendEmailAlert((isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart) + " Daily Alert Summary",
                            summary.GenerateMessageBody(xmlSummaryFile, isWindowsServerSolutions), System.Net.Mail.MailPriority.Normal, "{00000000-0000-0000-0000-000000000000}");
                    }
                    catch (Exception ex)
                    {
                        SiAuto.Main.LogFatal("[Chilkat Fatal Error] " + ex.Message);
                        SiAuto.Main.LogException(ex);
                    }
                }
                summary.PowerEventCount = 0;
                summary.Reset();
                SiAuto.Main.LogBool("doServiceRecycle", doServiceRecycle);
                SiAuto.Main.LogBool("recycleNeeded", recycleNeeded);
                recycleNeeded = doServiceRecycle;
            }
            // Write
            summary.ComposeNewFile(xmlSummaryFile);
            // Reset
            summary.Reset();
            // Recycle?
            if (recycleNeeded)
            {
                //System.Threading.Thread.Sleep(3000);
                //RecycleService();
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.ProcessDailySummaryData");
        }
        #endregion

        public void ConfigureInstance(String xmlText, DateTime result, Guid refGuid, uint retVal)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.ConfigureInstance");
            // License
            mexiSexi = new Components.Mexi_Sexi.MexiSexi(xmlText, result, refGuid, retVal);

            // Growl and Snarl?
            if (isGrowlEnabled)
            {
                if (growl == null)
                {
                    InitializeGrowl();

                    if (growl != null)
                    {
                        byte[] iconData = System.IO.File.ReadAllBytes(xmlFilePath + "\\" + Properties.Resources.IconAlertHyperfatal);
                        String remotePC = isGrowlRemoteEnabled ? " Computer: " + Environment.MachineName : String.Empty;
                        Notification growlNotificate = new Notification(isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart,
                            gnt.General.Name, DateTime.Now.Ticks.ToString(), "Service Initialization", gnt.GrowlApplication.Name + " service is starting." + remotePC,
                            gnt.General.Icon, false, Growl.Connector.Priority.Normal, String.Empty);
                        growl.Notify(growlNotificate);
                    }
                }
            }

            if (isSnarlEnabled)
            {
                if (snarl == null)
                {
                    InitializeSnarl();

                    if (snarl != null)
                    {
                        byte[] iconData = System.IO.File.ReadAllBytes(xmlFilePath + "\\" + Properties.Resources.IconAlertHyperfatal);
                        String thisPC = " Computer: " + Environment.MachineName;
                        Notification growlNotificate = new Notification(isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart,
                            snt.General.Name, DateTime.Now.Ticks.ToString(), "Service Initialization", snt.GrowlApplication.Name + " service is starting." + thisPC,
                            snt.General.Icon, false, Growl.Connector.Priority.Normal, String.Empty);
                        snarl.Notify(growlNotificate);
                    }
                }
            }

            summary = new ServiceHelper.DailySummary();
            summary.Insert(isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart, String.Empty, 65535,
                "Service Message", "Service Starting", "Service is starting. This could be because the computer was rebooted, or the service itself was manually restarted.", false, AlertAction.Insert);

            if (isWindowsServerSolutions)
            {
                PerformStartupCheck();
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.ConfigureInstance");
                return;
            }

            // TODO: Enforce after beta!
            if (mexiSexi.IsError || mexiSexi.IsGeezery)
            {
                String mexiSexiError = GeezeryNotificate(result, mexiSexi.ReferenceCode);
                throw new System.ComponentModel.LicenseException(typeof(Mexi_Sexi.MexiSexi), this, mexiSexiError);
            }
            else if (!mexiSexi.IsMexiSexi)
            {
                SiAuto.Main.LogWarning("[Trial] Trial license is valid.");
            }

            if (doDebugBackup || doDebugCustomBackup)
            {
                List<int> flunkaroo = new List<int>();
                if (String.IsNullOrEmpty(debugBadDisks))
                {
                    flunkaroo.Add(0);
                }
                else
                {
                    try
                    {
                        foreach (String disk in debugBadDisks.Split(','))
                        {
                            try
                            {
                                flunkaroo.Add(Int32.Parse(disk));
                            }
                            catch
                            {
                                SiAuto.Main.LogWarning("Didn't add " + disk + " because it didn't parse.");
                            }
                        }
                    }
                    catch
                    {
                        SiAuto.Main.LogWarning("Failed to split debug bad disks list.");
                    }
                    finally
                    {
                        if (flunkaroo.Count == 0)
                        {
                            flunkaroo.Add(0);
                        }
                    }
                }

                if (doDebugBackup)
                {
                    RunEmergencyBackup(true, flunkaroo);
                }
                if (doDebugCustomBackup)
                {
                    RunCustomBackup(true, flunkaroo);
                }
            }

            PerformStartupCheck();

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.ConfigureInstance");
        }

        /// <summary>
        /// Sends a notice via the user-enabled notification methods.
        /// </summary>
        /// <param name="result">result code</param>
        /// <param name="refID">reference ID</param>
        /// <returns></returns>
        private String GeezeryNotificate(DateTime result, UInt32 refID)
        {
            bool isHyperFatal = false;

            if (result.Year == 1980 && result.Month == 1 && result.Day == 1)
            {
                isHyperFatal = true;
            }
            else if (refID != 0x0 && refID != 0x1)
            {
                isHyperFatal = true;
            }
            else
            {
                isHyperFatal = false;
            }

            if (isHyperFatal)
            {
                SiAuto.Main.LogFatal("[Cataclysmic] License tampering detected. " + mexiSexi.ReferenceCode);
                String mexiSexiError = "The WindowSMART 24/7 licensing system detected tampering with the license. " +
                    "(0x" + mexiSexi.ReferenceCode.ToString("X") + ") The licensing system assumes such tampering is an attempt to bypass the trial, and so your trial has been terminated. The " +
                    "WindowSMART 24/7 service is halting. The health of your hard drives and SSDs is no longer being monitored. To resume monitoring, you must " +
                    "purchase a license.";
                PostUpdateNotificate(mexiSexiError, "Trial Terminated", true, false, false);
                WriteXmlFatalFile(true);
                return mexiSexiError;
            }
            else
            {
                SiAuto.Main.LogFatal("[Expired] Your trial has expired.");
                String mexiSexiError = "Your WindowSMART 24/7 trial has expired. The WindowSMART 24/7 service is halting. The health of " +
                    "your hard drives and SSDs is no longer being monitored. To resume monitoring, you must purchase a license.";
                PostUpdateNotificate(mexiSexiError, "Trial Expired", false, true, false);
                WriteXmlFatalFile(false);
                return mexiSexiError;
            }
        }

        /// <summary>
        /// Performs an initial startup check.
        /// </summary>
        public void PerformStartupCheck()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.PerformStartupCheck");
            SiAuto.Main.LogMessage("[Service Initialization Initial Check] Enumerate the disks and save data to the Registry.");
            String phantomDiskList = String.Empty;
            phantomDiskList = EnumerateDisksAndSaveData();
            if (String.IsNullOrEmpty(phantomDiskList))
            {
                SiAuto.Main.LogMessage("No phantom disks were detected. If an active alert for phantoms exists, it will be cleared.");
            }
            else
            {
                SiAuto.Main.LogWarning("At least one phantom disk was detected by the Server. Phantom(s): " + phantomDiskList);
            }
            SiAuto.Main.LogMessage("[Service Initialization Initial Check] Check for feverish disks.");
            CheckForFeverishDisks();
            if (Utilities.Utility.IsSystemWindows8())
            {
                SiAuto.Main.LogMessage("[Service Initialization Initial Check] Check for Storage Spaces phantom disks.");
                ProcessPhantoms(phantomDiskList);
            }
            SiAuto.Main.LogMessage("[Service Initialization Initial Check] Clear stale disks from memory.");
            ClearStaleDisks();
            SiAuto.Main.LogMessage("[Service Initialization Initial Check] Post alerts based on feverish disks.");
            PostAlert();
            SiAuto.Main.LogMessage("[Service Initialization Initial Check] Writing out a fresh XML alerts file.");
            WriteXmlAlertsFile();
            SiAuto.Main.LogMessage("[Service Initialization Initial Check] Processing daily summary data.");
            ProcessDailySummaryData(false);
            SiAuto.Main.LogMessage("[Service Initialization Initial Check] Checking to see if an emergency backup needs to run.");
            if (performEmergencyBackup || performCustomBackup)
            {
                List<int> failingDiskList = CheckForEmergencyBackup();
                if (failingDiskList.Count > 0)
                {
                    SiAuto.Main.LogWarning("Emergency backup check returned a non-zero count of failing disks; emergency backup will be attempted.");
                    if (performEmergencyBackup)
                    {
                        RunEmergencyBackup(false, failingDiskList);
                    }
                    if (performCustomBackup)
                    {
                        RunCustomBackup(false, failingDiskList);
                    }
                }
            }
            SiAuto.Main.LogMessage("[Service Initialization Initial Check] Initial checks complete.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.PerformStartupCheck");
        }

        public void TearDown()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.TearDown");
            DiskEnumerator.TearDown();

            if (isGrowlEnabled && growl != null)
            {
                SiAuto.Main.LogMessage("[Teardown] Sending Growl notification that service is shutting down.");
                String remotePC = isGrowlRemoteEnabled ? " Computer: " + Environment.MachineName : String.Empty;
                Notification growlNotificate = new Notification(isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart,
                    gnt.AppWarning.Name, DateTime.Now.Ticks.ToString(), "Service Shutdown", gnt.GrowlApplication.Name + " service is shutting down. If this is unexpected, please " +
                    "check the computer or, in a business environment, contact your system administrator." + remotePC, gnt.AppWarning.Icon, false, Growl.Connector.Priority.High, String.Empty);
                growl.Notify(growlNotificate);
                SiAuto.Main.LogMessage("[Teardown] Growl notification has been sent.");
            }

            //SiAuto.Main.LogMessage("[Teardown] Revoke the WMI namespace.");
            //try
            //{
            //    Instrumentation.Revoke(wmiAlerts);
            //    SiAuto.Main.LogMessage("[Teardown] Successfully revoked the WMI namespace.");
            //}
            //catch (Exception ex)
            //{
            //    SiAuto.Main.LogError("Failed to revoke the WMI namespace: " + ex.Message);
            //    SiAuto.Main.LogException(ex);
            //}

            SiAuto.Main.LogMessage("[Teardown] Write out daily summary XML file.");
            summary.Insert(isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart, String.Empty, 65535,
                "Service Message", "Service Halting", "Service is shutting down. This could be because the computer is being rebooted, shut down, or the service itself was manually stopped.", false, AlertAction.Insert);
            ProcessDailySummaryData(false);

            SiAuto.Main.LogMessage("[Teardown] Whack active alerts XML file.");
            if (System.IO.File.Exists(xmlAlertFile))
            {
                try
                {
                    System.IO.File.Delete(xmlAlertFile);
                    SiAuto.Main.LogMessage("Active alerts file successfully whacked.");
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogError("Failed to delete active alerts file: " + ex.Message);
                    SiAuto.Main.LogException(ex);
                }
            }
            else
            {
                SiAuto.Main.LogMessage("Nothing to delete.");
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.TearDown");
        }

        public int ServicePollingInterval
        {
            get
            {
                try
                {
                    pollingInterval = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigPollingInterval);
                }
                catch
                {
                    pollingInterval = 900000; // 15 minutes
                }
                return pollingInterval;
            }
        }

        public void RefreshPowerEventNotifyState()
        {
            try
            {
                notifyOnPowerChange = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigNotifyPowerChange));
            }
            catch(Exception ex)
            {
                SiAuto.Main.LogException(ex);
            }
        }

        private void InitializeWssEnvironment()
        {
            Microsoft.WindowsServerSolutions.Common.WindowsServerSolutionsEnvironment.Initialize();
        }

        private void growl_NotificationCallback(Response response, CallbackData callbackData, object state)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.growl_NotificationCallback");
            string text = String.Format("Response Type: {0}\r\nNotification ID: {1}\r\nCallback Data: {2}\r\nCallback Data Type: {3}\r\n", callbackData.Result, callbackData.NotificationID, callbackData.Data, callbackData.Type);
            SiAuto.Main.LogMessage("Growl Callback Received - " + text);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.growl_NotificationCallback");
        }

        private void snarl_NotificationCallback(Response response, CallbackData callbackData, object state)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.snarl_NotificationCallback");
            string text = String.Format("Response Type: {0}\r\nNotification ID: {1}\r\nCallback Data: {2}\r\nCallback Data Type: {3}\r\n", callbackData.Result, callbackData.NotificationID, callbackData.Data, callbackData.Type);
            SiAuto.Main.LogMessage("Snarl Callback Received - " + text);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.snarl_NotificationCallback");
        }

        private void InitializeGrowl()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.InitializeGrowl");
            try
            {
                String pDubya = (growlPassword == "N@yNayDefined") ? String.Empty : growlPassword;
                if (isGrowlEnabled && isGrowlRemoteEnabled)
                {
                    SiAuto.Main.LogMessage("Attempting to connect to Growl on remote computer " + growlRemoteComputer + ", port " + growlPort.ToString());
                    growl = new GrowlConnector(pDubya, growlRemoteComputer, growlPort);
                }
                else if (isGrowlEnabled)
                {
                    SiAuto.Main.LogMessage("Attempting to connect to Growl on the local machine.");
                    growl = new GrowlConnector(pDubya);
                }
                growl.NotificationCallback += new GrowlConnector.CallbackEventHandler(growl_NotificationCallback);

                growl.EncryptionAlgorithm = Growl.Connector.Cryptography.SymmetricAlgorithmType.PlainText;

                gnt = new Utilities.GrowlNotificationTypes(isWindowsServerSolutions);
                // gnt.GrowlApplication is how you access the Growl Application object.

                SiAuto.Main.LogMessage("Registering with Growl.");
                growl.Register(gnt.GrowlApplication, new NotificationType[] { gnt.AppWarning, gnt.Cleared, gnt.Critical, gnt.General, gnt.Hyperfatal, gnt.Warning });
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Cannot initialize Growl: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.InitializeGrowl");
        }

        /// <summary>
        /// Initialize Snarl using the Growl API. The Snarl API doesn't work when run from a Windows Service.
        /// </summary>
        private void InitializeSnarl()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.InitializeSnarl");
            try
            {
                if (isSnarlEnabled)
                {
                    SiAuto.Main.LogMessage("Attempting to connect to Snarl on the local machine.");
                    snarl = new GrowlConnector();
                }
                snarl.NotificationCallback += new GrowlConnector.CallbackEventHandler(snarl_NotificationCallback);

                snarl.EncryptionAlgorithm = Growl.Connector.Cryptography.SymmetricAlgorithmType.PlainText;

                snt = new Utilities.GrowlNotificationTypes(isWindowsServerSolutions);
                // gnt.GrowlApplication is how you access the Growl Application object.

                SiAuto.Main.LogMessage("Registering with Snarl.");
                snarl.Register(snt.GrowlApplication, new NotificationType[] { snt.AppWarning, snt.Cleared, snt.Critical, snt.General, snt.Hyperfatal, snt.Warning });
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Cannot initialize Snarl: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.InitializeSnarl");
        }

        private void RunEmergencyBackup(bool runAsDebug, List<int> failingDiskList)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.RunEmergencyBackup");

            if (backupThread != null)
            {
                SiAuto.Main.LogWarning("Emergency Backup was ordered but the thread is not null. A backup has already been invoked, so exiting.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.RunEmergencyBackup");
                return;
            }

            if (runAsDebug)
            {
                SiAuto.Main.LogWarning("Emergency Backup was ordered by debug command.");
                PostUpdateNotificate("An Emergency Backup has been ordered due to DEBUG command. This should only be done to test the " +
                    "Emergency Backup capability. Please be sure to remove this debug command when finished testing, and restart the " +
                    "service.", "Emergency Backup - DEBUG Request", false, false, true);
            }
            else
            {
                SiAuto.Main.LogWarning("Emergency Backup was ordered due to disk failure signal.");
                PostUpdateNotificate("An Emergency Backup has been ordered due to DISK FAILURE detection. This backup is being performed " +
                    "at your request to preserve data in the event of a disk failure indication.", "Emergency Backup", false, false, true);
            }
            backupThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(DoEmergencyBackup));
            backupThread.Name = "Emergency Backup Thread";
            backupThread.Start(failingDiskList);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.RunEmergencyBackup");
        }

        private void RunCustomBackup(bool runAsDebug, List<int> failingDiskList)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.RunCustomBackup");

            if (customBackupThread != null)
            {
                SiAuto.Main.LogWarning("Emergency Custom Backup was ordered but the thread is not null. A backup has already been invoked, so exiting.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.RunCustomBackup");
                return;
            }

            if (runAsDebug)
            {
                SiAuto.Main.LogWarning("Emergency Custom Backup was ordered by debug command.");
                PostUpdateNotificate("An Emergency Custom Backup has been ordered due to DEBUG command. This should only be done to test the " +
                    "Emergency Custom Backup capability. Please be sure to remove this debug command when finished testing, and restart the " +
                    "service.", "Custom Backup - DEBUG Request", false, false, true);
            }
            else
            {
                SiAuto.Main.LogWarning("Emergency Custom Backup was ordered due to disk failure signal.");
                PostUpdateNotificate("An Emergency Custom Backup has been ordered due to DISK FAILURE detection. This backup is being performed " +
                    "at your request to preserve data in the event of a disk failure indication.", "Custom Backup", false, false, true);
            }
            customBackupThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(DoCustomBackup));
            customBackupThread.Name = "Custom Backup Thread";
            customBackupThread.Start(failingDiskList);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.RunCustomBackup");
        }

        private void DoEmergencyBackup(object cadavericDisks)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.DoEmergencyBackup (worker thread)");

            List<String> failingDriveLetters = null;

            try
            {
                failingDriveLetters = Utilities.Utility.GetDriveLettersFromDriveNumbers((List<int>)cadavericDisks);
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Cannot get failing drive letters; will attempt to back up everything.");
                SiAuto.Main.LogException(ex);
                failingDriveLetters = new List<String>();
            }

            Array arrayIncludes;
            Array arrayExcludes;

            List<String> folders = new List<String>();
            List<String> excludes = new List<String>();

            String target = String.Empty;

            if (!backupLocal)
            {
                SiAuto.Main.LogMessage("UNC non-local backup specified. Will attempt to map a drive to the share " + uncBackupPath + " on drive B, followed by D, E, F...Z.");
                bool isMapSuccessful = false;
                String[] driveArray = { "B:", "D:", "E:", "F:", "G:", "H:", "I:", "J:", "K:", "L:", "M:", "N:", "O:", "P:", "Q:", "R:", "S:", "T:", "U:", "V:", "W:", "X:", "Y:", "Z:" };

                foreach (String driveLetter in driveArray)
                {
                    try
                    {
                        SiAuto.Main.LogMessage("Attempting to map drive " + driveLetter + " to UNC path " + uncBackupPath + " for user account " + uncBackupUser);
                        NetworkDrive drive = new NetworkDrive();
                        drive.Force = false;
                        drive.LocalDrive = driveLetter;
                        drive.Persistent = false;
                        drive.PromptForCredentials = false;
                        drive.SaveCredentials = false;
                        drive.ShareName = uncBackupPath;
                        drive.MapDrive(uncBackupUser, uncBackupPassword);
                        SiAuto.Main.LogMessage("Successfully mapped " + driveLetter + " to UNC path " + uncBackupPath + "; we can proceed with the backup.");
                        isMapSuccessful = true;
                        target = driveLetter;
                        break;
                    }
                    catch (Exception ex)
                    {
                        SiAuto.Main.LogWarning("Failed to map " + driveLetter + " to UNC path " + uncBackupPath + ": " + ex.Message);
                        WindowsEventLogger.LogWarning("Failed to map " + driveLetter + " to UNC path " + uncBackupPath + ": " + ex.Message,
                            53887, Properties.Resources.EventLogJoshua);
                    }
                }

                if (!isMapSuccessful)
                {
                    SiAuto.Main.LogFatal("Unable to map UNC share " + uncBackupPath + " to any available drive. Backup cannot continue.");
                    WindowsEventLogger.LogError("Failed to map any drive to UNC path " + uncBackupPath,
                            53888, Properties.Resources.EventLogJoshua);
                    PostUpdateNotificate("Emergency Backup cannot proceed. Unable to map to network UNC share " + uncBackupPath + " on any drive letter. To " +
                        "determine the specific reason(s) for the failure to map the network drive, please open the Application event log and look for Warning " +
                        "events with ID number 53888.", "Emergency Backup Failed", false, false, true);
                }
            }
            else
            {
                foreach (String failingDriveLetter in failingDriveLetters)
                {
                    if (String.Compare(failingDriveLetter.Substring(0, 2), localBackupPath.Substring(0, 2), true) == 0)
                    {
                        SiAuto.Main.LogFatal("The backup target is on a disk that is signaling failure. The backup will be cancelled.");
                        WindowsEventLogger.LogError("The backup target is on " + failingDriveLetter + ", which is a drive that indicates it " +
                            "will fail. Emergency Backup cannot proceed.", 53869, Properties.Resources.EventLogTaryn);
                        PostUpdateNotificate("Emergency Backup cannot run - The backup target " + failingDriveLetter + " is on a disk signaling it will fail.",
                            "Emergency Backup Failed", false, false, true);
                        try
                        {
                            NetworkDrive drive = new NetworkDrive();
                            drive.LocalDrive = target;
                            drive.Force = true;
                            drive.UnMapDrive();
                        }
                        catch
                        {
                        }
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.DoEmergencyBackup (worker thread)");
                        return;
                    }
                }
            }

            try
            {
                arrayIncludes = (String[])configurationKey.GetValue(Properties.Resources.RegistryConfigIncludeDirectories);

                if (failingDriveLetters == null || failingDriveLetters.Count == 0)
                {
                    foreach (String item in arrayIncludes)
                    {
                        SiAuto.Main.LogMessage("Adding " + item + " to the backup set.");
                        folders.Add(item);
                    }
                }
                else
                {
                    foreach (String item in arrayIncludes)
                    {
                        foreach (String failingDriveLetter in failingDriveLetters)
                        {
                            if (String.Compare(failingDriveLetter.Substring(0, 2), item.Substring(0, 2), true) == 0)
                            {
                                SiAuto.Main.LogMessage("Adding " + item + " to the backup set.");
                                folders.Add(item);
                            }
                            else
                            {
                                SiAuto.Main.LogWarning("Skipping " + item + " because it is not on a disk indicating failure.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Cannot read backup sources! " + ex.Message);
                SiAuto.Main.LogException(ex);
                WindowsEventLogger.LogError("Emergency Backup cannot run - Unable to read the desired backup source names from the Registry. " + ex.Message,
                            53893, Properties.Resources.EventLogJoshua);
                PostUpdateNotificate("Emergency Backup cannot run - Unable to read the desired backup source names from the Registry. " + ex.Message,
                    "Emergency Backup Failed", false, false, true);
                try
                {
                    NetworkDrive drive = new NetworkDrive();
                    drive.LocalDrive = target;
                    drive.Force = true;
                    drive.UnMapDrive();
                }
                catch
                {
                }
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.DoEmergencyBackup (worker thread)");
                return;
            }

            String driveToUnmap = target;

            if (backupLocal)
            {
                // If local, set path now. If UNC, path is already set.
                target = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigLocalBackupPath);
            }
            else
            {
                String machineName = Environment.MachineName;
                if (target.EndsWith("\\"))
                {
                    target += machineName + "\\";
                }
                else
                {
                    target += "\\" + machineName;
                }
            }

            try
            {
                if (String.IsNullOrEmpty(target))
                {
                    throw new ArgumentException("The specified target is null or empty. Cannot back up to null target.");
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Invalid target! " + ex.Message);
                SiAuto.Main.LogException(ex);
                PostUpdateNotificate("Emergency Backup cannot run - The backup target (destination) specified is null or empty, or an error occurred reading " +
                    "the Registry. " + ex.Message, "Emergency Backup Failed", false, false, true);
                try
                {
                    NetworkDrive drive = new NetworkDrive();
                    drive.LocalDrive = driveToUnmap;
                    drive.Force = true;
                    drive.UnMapDrive();
                }
                catch
                {
                }
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.DoEmergencyBackup (worker thread)");
                return;
            }

            try
            {
                arrayExcludes = (String[])configurationKey.GetValue(Properties.Resources.RegistryConfigExcludeItems);
                foreach (String item in arrayExcludes)
                {
                    excludes.Add(item);
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Cannot read excluded items! " + ex.Message);
                SiAuto.Main.LogException(ex);
            }

            foreach (String folder in folders)
            {
                Utilities.SilvermistBackup backup = new Utilities.SilvermistBackup(folder, target, excludes);
                backup.Start();

                PostUpdateNotificate("Backup of " + folder + " completed. Backed up " + backup.FolderCount.ToString() + " folder(s), " +
                    backup.FileCount.ToString() + " file(s), " + backup.ByteCount.ToString() + " byte(s). There were " + backup.ErrorCount.ToString() +
                    " error(s), " + backup.SkippedFolders.ToString() + " skipped folder(s) and " + backup.SkippedFiles.ToString() + " skipped file(s). " +
                    "Please don't shut down your computer or attempt to replace the disk yet - wait for a message indicating all backups are done first!",
                    "Backup Complete for " + folder, false, false, false);
            }

            PostUpdateNotificate("Backups of all attempted folders have been completed. Please check previous notifications to see whether those backups " +
                "were successful, or if they had errors. If you are satisfied that all attempted backups succeeded, you may safely replace the disk in question. " +
                "(NOTE: If you also had a custom emergency backup running, make sure that finishes first!)", "Emergency Backup Complete", false, false, false);

            try
            {
                NetworkDrive drive = new NetworkDrive();
                drive.LocalDrive = driveToUnmap;
                drive.Force = true;
                drive.UnMapDrive();
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.DoEmergencyBackup (worker thread)");
        }

        private void DoCustomBackup(object cadavericDisks)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.DoCustomBackup (worker thread)");
            List<String> failingDriveLetters = null;

            try
            {
                failingDriveLetters = Utilities.Utility.GetDriveLettersFromDriveNumbers((List<int>)cadavericDisks);
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Cannot get failing drive letters; will attempt to back up everything.");
                SiAuto.Main.LogException(ex);
                failingDriveLetters = new List<String>();
            }

            String driveLetterList = String.Empty;
            foreach (String driveLetter in failingDriveLetters)
            {
                driveLetterList += driveLetter + ",";
            }
            if (driveLetterList.EndsWith(","))
            {
                driveLetterList = driveLetterList.Substring(0, driveLetterList.Length - 1);
            }

            SiAuto.Main.LogMessage("Comma-separated list of drive letters of failing disks: " + driveLetterList);

            String driveNumberList = String.Empty;
            foreach (int driveNumber in (List<int>)cadavericDisks)
            {
                driveNumberList += driveNumber.ToString() + ",";
            }
            if (driveNumberList.EndsWith(","))
            {
                driveNumberList = driveNumberList.Substring(0, driveNumberList.Length - 1);
            }

            SiAuto.Main.LogMessage("Comma-separated list of drive numbers of failing disks: " + driveNumberList);

            try
            {
                String argsToUse = customBackupArgs.Replace("{DRIVELETTERS}", driveLetterList).Replace("{PHYSICALDISKS}", driveNumberList);
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(customBackupProgram, argsToUse);
                psi.UseShellExecute = false;
                psi.RedirectStandardError = true;
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = true;
                psi.ErrorDialog = false;
                psi.CreateNoWindow = true;

                System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);
                process.WaitForExit(5000);
                SiAuto.Main.LogMessage("No error occurred launching the third-party backup program.");
                PostUpdateNotificate("The Emergency Custom Backup program was successfully launched. Please note this does not necessarily " +
                    "indicate a successful backup. Please check with the third-party backup program you ran to validate success and/or " +
                    "failure.", "Custom Backup Launched", false, false, false);
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Cannot invoke third-party backup program: " + ex.Message);
                SiAuto.Main.LogException(ex);
                PostUpdateNotificate("Emergency Custom Backup cannot run - Cannot launch the third-party backup program. " + ex.Message,
                    "Custom Backup Failed", false, false, true);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.DoCustomBackup (worker thread)");
        }

        private void RunThermalShutdownCheck()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.RunThermalShutdownCheck");
            SiAuto.Main.LogBool("performThermalShutdown", performThermalShutdown);

            SiAuto.Main.LogMessage("Thermal shutdown check option is enabled; checking for disks that have been overheated or critically hot for 3 or more consecutive pollings.");
            try
            {
                SiAuto.Main.LogMessage("Acquire Registry objects.");
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);
                SiAuto.Main.LogMessage("Acquired Registry objects.");

                // Look for inactive (stale) disks. These are disks that have been disconnected.
                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    try
                    {
                        SiAuto.Main.LogString("diskKeyName", diskKeyName);
                        SiAuto.Main.LogMessage("Checking " + diskKeyName + " for staleness or ignored.");
                        Microsoft.Win32.RegistryKey diskKey;
                        diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, false);
                        bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                        if (!activeFlag)
                        {
                            SiAuto.Main.LogMessage("Disk " + diskKeyName + " is stale; skipping thermal check.");
                            continue;
                        }
                        String dateIgnored = (String)diskKey.GetValue("DateIgnored", String.Empty);
                        SiAuto.Main.LogString("dateIgnored (date/time parse check)", dateIgnored);
                        DateTime result;
                        if (DateTime.TryParse(dateIgnored, out result))
                        {
                            // Ignored disk.
                            SiAuto.Main.LogMessage("Disk " + diskKeyName + " has a valid DateIgnored date; skipping thermal check.");
                            diskKey.Close();
                            continue;
                        }
                        SiAuto.Main.LogMessage("Disk is active and not ignored; checking the consecutive overheat count.");

                        int overheatCount = 0;
                        overheatCount = (int)diskKey.GetValue("ConsecutiveOverheatCount");

                        if (overheatCount >= 3 && performThermalShutdown)
                        {
                            WindowsEventLogger.LogError("At least one disk has remained in an overheated or critically hot state for 3 or more consecutive disk polling intervals. " +
                                "Per the configured option \"perform thermal shutdown,\" this computer will be shut down in 1 minute.", 53855, Properties.Resources.EventLogTaryn);
                            DoThermalShutdown();
                        }
                        else if (overheatCount >= 3)
                        {
                            WindowsEventLogger.LogError("At least one disk has remained in an overheated or critically hot state for 3 or more consecutive disk polling intervals.",
                               53856, Properties.Resources.EventLogTaryn);
                        }
                        else
                        {
                            SiAuto.Main.LogMessage("Disk " + diskKeyName + " consecutive overheat check is OK.");
                        }
                    }
                    catch (Exception ex)
                    {
                        SiAuto.Main.LogWarning("Exception was thrown assessing disk " + diskKeyName + " for consecutive overheats. " + ex.Message);
                        SiAuto.Main.LogException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Exception was thrown during the thermal shutdown check. " + ex.Message);
                SiAuto.Main.LogException(ex);
                WindowsEventLogger.LogError("Exception was thrown during the thermal shutdown check. " + ex.Message, 53890, Properties.Resources.EventLogJoshua);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.RunThermalShutdownCheck");
        }

        private void DoThermalShutdown()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.DoThermalShutdown");

            PostUpdateNotificate("At least one disk has remained overheated or critically hot for 3 or more consecutive polling intervals. Per your specified option, this computer will be shut down in " +
                "1 minute. Your computer likely has a serious airflow problem such as a failed or clogged fan, obstructed or clogged vents, or is placed too close to a heat source.", "Thermal Shutdown",
                true, false, false);

            System.Threading.Thread.Sleep(60000);

            try
            {
                WindowsEventLogger.LogWarning("A thermal shutdown has been ordered.", 53866, Properties.Resources.EventLogTaryn);
                Reboot.RebootServer.ShutDown(true);
            }
            catch (Exception ex)
            {
                WindowsEventLogger.LogError("A thermal shutdown was ordered, but an error occurred processing the shutdown request. " + ex.Message, 53889, Properties.Resources.EventLogJoshua);
                SiAuto.Main.LogError(ex.Message);
                SiAuto.Main.LogException(ex);
                PostUpdateNotificate("The thermal overheat shutdown request failed. " + ex.Message, "Thermal Shutdown Failed", false, false, true);
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.DoThermalShutdown");
        }

        private List<int> CheckForEmergencyBackup()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.CheckForEmergencyBackup");

            if (!performEmergencyBackup && !performCustomBackup)
            {
                SiAuto.Main.LogWarning("Neither emergency backup method is enabled. Skipping check.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.CheckForEmergencyBackup");
                return new List<int>();
            }

            List<int> failingDriveList = new List<int>();

            try
            {
                SiAuto.Main.LogMessage("Acquire Registry objects.");
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryMonitoredDisksKey);
                SiAuto.Main.LogMessage("Acquired Registry objects.");

                // Look for inactive (stale) disks. These are disks that have been disconnected.
                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    try
                    {
                        SiAuto.Main.LogString("diskKeyName", diskKeyName);
                        SiAuto.Main.LogMessage("Checking " + diskKeyName + " for staleness or ignored.");
                        Microsoft.Win32.RegistryKey diskKey;
                        diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, false);
                        bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                        if (!activeFlag)
                        {
                            SiAuto.Main.LogMessage("Disk " + diskKeyName + " is stale; skipping TEC check.");
                            continue;
                        }
                        String dateIgnored = (String)diskKey.GetValue("DateIgnored", String.Empty);
                        SiAuto.Main.LogString("dateIgnored (date/time parse check)", dateIgnored);
                        DateTime result;
                        if (DateTime.TryParse(dateIgnored, out result))
                        {
                            // Ignored disk.
                            SiAuto.Main.LogMessage("Disk " + diskKeyName + " has a valid DateIgnored date; skipping TEC check.");
                            diskKey.Close();
                            continue;
                        }
                        SiAuto.Main.LogMessage("Disk is active and not ignored; checking for TEC.");

                        SiAuto.Main.LogMessage("Getting disk number and failure state.");
                        int diskNumber = Utilities.Utility.GetDriveIdFromPath((String)diskKey.GetValue("DevicePath"));
                        SiAuto.Main.LogInt("diskNumber", diskNumber);
                        bool wmiFailPredicted = bool.Parse((String)diskKey.GetValue("FailurePredicted"));
                        SiAuto.Main.LogBool("wmiFailPredicted", wmiFailPredicted);
                        bool diskTec = bool.Parse((String)diskKey.GetValue("DiskTec"));
                        SiAuto.Main.LogBool("diskTec", diskTec);
                        bool hotTec = bool.Parse((String)diskKey.GetValue("HotTec"));
                        SiAuto.Main.LogBool("hotTec", hotTec);

                        if (wmiFailPredicted || diskTec)
                        {
                            if (wmiFailPredicted)
                            {
                                SiAuto.Main.LogFatal("Physical disk " + diskNumber.ToString() + " reports failure via WMI; adding to list.");
                            }
                            else
                            {
                                SiAuto.Main.LogFatal("Physical disk " + diskNumber.ToString() + " reports failure via SMART; adding to list.");
                            }
                            failingDriveList.Add(diskNumber);
                            break;
                        }
                        if (!noHotBackup && hotTec)
                        {
                            SiAuto.Main.LogFatal("Physical disk " + diskNumber.ToString() + " reports hot TEC and hot TEC backup enabled; adding to list.");
                            failingDriveList.Add(diskNumber);
                        }
                    }
                    catch (Exception ex)
                    {
                        SiAuto.Main.LogWarning("Exception was thrown assessing disk " + diskKeyName + " for TEC status. " + ex.Message);
                        SiAuto.Main.LogException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Exception was thrown during the emergency backup TEC check. " + ex.Message);
                SiAuto.Main.LogException(ex);
                WindowsEventLogger.LogError("Exception was thrown during the emergency backup TEC check. " + ex.Message, 53896, Properties.Resources.EventLogJoshua);
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.CheckForEmergencyBackup");
            return failingDriveList;
        }

        public void ProcessPhantoms(String listOfPhantomDisks)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.ProcessPhantoms");
            bool doesAlertExist = feverishDisks.ItemExists(Properties.Resources.PhantomDiskGuid);
            SiAuto.Main.LogBool("doesAlertExist", doesAlertExist);
            if (String.IsNullOrEmpty(listOfPhantomDisks) && doesAlertExist)
            {
                SiAuto.Main.LogMessage("Phantom disk alert is active, but no phantoms exist. Remove alert.");
                feverishDisks.RemoveItem(Properties.Resources.PhantomDiskGuid);
            }
            else if (String.IsNullOrEmpty(listOfPhantomDisks))
            {
                SiAuto.Main.LogMessage("Phantom disk alert is inactive, and no phantoms exist. Nothing to do.");
            }
            else
            {
                feverishDisks.AddItem(Properties.Resources.PhantomDiskGuid, "Storage Spaces", "N/A", Properties.Resources.FeverishSmartStatusPhantomTitle,
                    Properties.Resources.FeverishSmartStatusPhantomMessage + " " + listOfPhantomDisks, 65530, "Phantom Disk", false,
                    doesAlertExist ? AlertAction.Update : AlertAction.Insert, 53843);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.ProcessPhantoms");
        }

        private List<String> GetDriveLettersFromFailingDiskList(List<int> failingDiskList)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.GetDriveLettersFromFailingDiskList");
            List<String> driveLetters = new List<String>();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.GetDriveLettersFromFailingDiskList");
            return driveLetters;
        }

        public void PostPowerNotificate(String message, String title, bool isCritical, String extraTitle, int attributeID)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.PostPowerNotificate");
            if (summary != null)
            {
                summary.PowerEventCount++;
                SiAuto.Main.LogInt("summary.PowerEventCount", summary.PowerEventCount);
                summary.Insert(isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart, String.Empty, attributeID,
                    extraTitle, title, message, false, AlertAction.Insert);
            }
            else
            {
                SiAuto.Main.LogWarning("Summary object is null.");
            }

            WindowsEventLogger.LogWarning("A " + extraTitle + " was detected: " + title + "\n\n" + message, 53874);

            if (notifyOnPowerChange && ((summary == null) || (summary != null && summary.PowerEventCount <= 5)))
            {
                PostSpecialNotificate(message, title, isCritical, extraTitle, attributeID);

                if (summary != null && summary.PowerEventCount == 5)
                {
                    PostSpecialNotificate("There have been 5 power-related events in the last 24 hours. This may be the result of a bad laptop battery, faulty " +
                        "uninterruptable power supply or unstable power at the computer's location. No more power-related alerts will be sent today. If you " +
                        "regularly receive this alert, consider checking or replacing the laptop battery or uninterruptable power supply, if applicable, or " +
                        "consult an electrician to check for power problems.", "Too Many Power Events", true);
                    WindowsEventLogger.LogError("Too many power events detected in last 24 hours. Power events will continue to be logged but no further notifications " +
                        "will be sent today. Consider checking the battery or uninterruptable power supply, if applicable. Also consider consulting an electrician to " +
                        "check for building electrical problems.", 53875);
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.PostPowerNotificate");
        }

        public void PostSpecialNotificate(String message, String title, bool isCritical)
        {
            PostUpdateNotificate(message, title, false, false, isCritical);
        }

        public void PostSpecialNotificate(String message, String title, bool isCritical, String extraTitle, int attributeID)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.PostSpecialNotificate");
            PostUpdateNotificate(message, title, false, false, isCritical);
            if (summary == null)
            {
                return;
            }
            summary.Insert(isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart, String.Empty, attributeID,
                extraTitle, title, message, false, AlertAction.Insert);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.PostSpecialNotificate");
        }

        private bool WasLastShutdownFilthy()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.WasLastShutdownFilthy");
            try
            {
                if (System.IO.File.Exists(xmlAlertFile))
                {
                    SiAuto.Main.LogWarning("XML alert file exists when it shouldn't; the last shutdown is assumed to be filthy.");
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.WasLastShutdownFilthy");
                    return true;
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LogMessage("The last shutdown was clean.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.WasLastShutdownFilthy");
            return false;
        }

        private void RecycleService()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.RecycleService");
            try
            {
                SiAuto.Main.LogMessage("Attempting daily service recycle.");
                String path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                System.Diagnostics.ProcessStartInfo psi;
                psi = new System.Diagnostics.ProcessStartInfo(path + "\\HomeServerSMART2013.Service.exe", "/reboot");
                psi.UseShellExecute = false;
                psi.RedirectStandardError = true;
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = true;
                psi.ErrorDialog = false;
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);
                SiAuto.Main.LogMessage("Successfully spawned service recycle call.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.RecycleService");
        }
    }
}