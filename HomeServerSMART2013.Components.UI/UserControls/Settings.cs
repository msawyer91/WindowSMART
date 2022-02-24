using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Gurock.SmartInspect;
using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class Settings : Form
    {
        private Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
        private Microsoft.Win32.RegistryKey dojoNorthSubKey;
        private Microsoft.Win32.RegistryKey configurationKey;

        private DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Cryptography.DoubleEncryptor crypto;

        private ServiceController controller;

        private bool isRegistryAvailable;
        private bool isDebugChangeMessageShown;
        private bool ignoreHot;
        private bool ignoreWarm;
        private bool reportCritical;
        private bool reportWarnings;
        private bool reportGeriatric;
        private bool preserveOnUninstall;
        private bool loadInProgress;
        private bool driveBenderInstalled;
        private bool drivePoolInstalled;
        private Mexi_Sexi.MexiSexi mexiSexi;
        private bool isUiBlocking;
        private bool gpoApprisalShown;

        // BitLocker configuration
        private bool bitLockerShowAllDrives;
        private bool bitLockerPreventLocking;
        private bool bitLockerHideTab;

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

        // Helper Service ~ Dan Garland ~
        private bool useHelperService;
        private String helperServiceDiskPoolProvider;
        private bool ignoreVirtualDisks;

        // Debug options -- next three bools
        private bool fallbackToWmi;
        private bool debugMode;
        private bool advancedSii;

        // Temperatures
        private int tempCriticalF;
        private int tempOverheatedF;
        private int tempHotF;
        private int tempWarmF;
        private int tempCriticalK;
        private int tempOverheatedK;
        private int tempHotK;
        private int tempWarmK;

        // Temperature thresholds and polling interval
        private int criticalTemperatureThreshold;
        private int overheatedTemperatureThreshold;
        private int hotTemperatureThreshold;
        private int warmTemperatureThreshold;
        private int pollingInterval;

        // SSD thresholds
        private int ssdLifeLeftCritical;
        private int ssdLifeLeftWarning;
        private int ssdRetirementCritical;
        private int ssdRetirementWarning;

        // Logging
        private String debugLogLocation;

        // WSS
        private bool isWindowsServerSolutions;

        // Skinning
        private bool useDefaultSkinning;
        private int windowBackground; // 0 = Metal Grate, 1 = Lightning, 2 = Cracked Glass, 3 = None

        // Notifications
        private bool isProwlEnabled;
        private bool isNmaEnabled;
        private bool isPushoverEnabled;
        private bool isWindowsPhoneEnabled;
        private String prowlApiKey;
        private String nmaApiKey;
        private String wp7Guids;
        private String pushoverUserKey;
        private String pushoverClearedSound;
        private String pushoverCriticalSound;
        private String pushoverWarningSound;
        private String pushoverDeviceTarget;
        private bool isGrowlEnabled;
        private bool isGrowlRemoteEnabled;
        private bool isSnarlEnabled;
        private String growlRemoteTarget;
        private int growlPort;
        private String growlPassword;
        private const String PUSHOVER_DEFAULT_SOUND = "(Device default sound)";

        // System Notifications
        private bool notifyOnPowerChange;
        private bool notifyOnFilthyShutdown;

        // ImageList for FancyListView
        private ImageList ignoreImageList;

        // Delegates for UI updates from worker threads.
        private delegate void UpdateServiceStatusMessage(Image img, String msg);
        private delegate void RefreshState();
        private UpdateServiceStatusMessage serviceStatusUpdater;
        private RefreshState stateRefresher;

        String temperaturePreference;

        // GPO
        private int gpoDpa;
        private int gpoTempCtl;
        private int gpoVirtualIgnore;
        private int gpoAllowIgnoredItems;
        private int gpoAllowServiceControl;
        private int gpoEmailNotificate;
        private int gpoProwlNotificate;
        private int gpoNmaNotificate;
        private int gpoPushoverNotificate;
        private int gpoWpNotificate;
        private int gpoToastyNotificate;
        private int gpoGrowlNotificate;
        private int gpoSnarlNotificate;
        private int gpoAdvancedSettings;
        private int gpoDebuggingControl;
        private int gpoSSD;
        private int gpoCheckForUpdates;
        private int gpoUiTheme;
        private int gpoUseSupportMessage;
        private int gpoEmergencyBackup;
        private bool allowShutdown;
        private bool allowDeleteStale;
        private bool allowRestoreDefaults;
        private bool allowResetEverything;
        private String gpoDebuggingAllowed;

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
        private String customBackupProgram;
        private String customBackupArgs;

        public Settings(Mexi_Sexi.MexiSexi ms)
        {
            InitializeComponent();

            // Is WSS?
            isWindowsServerSolutions = OperatingSystem.IsWindowsServerSolutionsProduct(this);
            isDebugChangeMessageShown = false;

            // Initialize the cryptography engine.
            crypto = new Components.Cryptography.DoubleEncryptor("T@rynPr3ttyL1ttl3Sl0bberh3@d",
                "J0shuaFl@l3ssB@byH3ad");
            crypto.SetCbcRequired("BaronessOfBeauty");

            // Set the ImageList.
            ignoreImageList = new ImageList();
            ignoreImageList.Images.Add("Ignore", Properties.Resources.Ignore16);
            ignoreImageList.Images.Add("Resume", Properties.Resources.Healthy16);
            // The InitializeComponent() method has run so we can manipulate the image lists.
            listViewIgnoredDisks.SmallImageList = ignoreImageList;
            listViewIgnoredProblems.SmallImageList = ignoreImageList;

            // Initialize the delegates.
            serviceStatusUpdater = new UpdateServiceStatusMessage(DUpdateServiceState);
            stateRefresher = new RefreshState(DRefreshServiceState);
            isUiBlocking = false;

            // GPO apprisals.
            gpoApprisalShown = false;

            // Try connecting to the service.
            try
            {
                controller = new ServiceController(Properties.Resources.ServiceNameHss);
            }
            catch (Exception ex)
            {
                DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.WindowsEventLogger.LogError(
                    "Unable to bind Windows Service \"dnhsSmart\" (WindowSMART Service) in the service controller. Some " +
                    "management transactions will not be possible. " + ex.Message, 53892, Properties.Resources.EventLogJoshua, Properties.Resources.ApplicationEventLog);
            }

            // Try connecting to the Registry.
            try
            {
                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);
                configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, false);
                if (dojoNorthSubKey == null || configurationKey == null)
                {
                    configurationKey.Close();
                    dojoNorthSubKey.Close();
                    isRegistryAvailable = false;
                }
                else
                {
                    dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, true);
                    configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, true);
                    isRegistryAvailable = true;
                }
            }
            catch (Exception)
            {
                isRegistryAvailable = false;
            }

            if (ms == null)
            {
                mexiSexi = null;
            }
            else
            {
                mexiSexi = ms;
            }

            // Are drive pooling software installed?
            driveBenderInstalled = DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.BitLocker.UtilityMethods.IsDriveBenderInstalled();
            drivePoolInstalled = DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.BitLocker.UtilityMethods.IsDriveBenderInstalled();

            // Set the defaults -- we'll load from the Registry if available.
            SetDefaults();
        }

        private void SetDefaults()
        {
            // Temperature preference is assumed to be Celsius (C) until otherwise loaded.
            temperaturePreference = "C";

            // Set default temperatures.
            criticalTemperatureThreshold = 65;
            overheatedTemperatureThreshold = 55;
            hotTemperatureThreshold = 50;
            warmTemperatureThreshold = 42;

            // Set alternate temps all to zero for now; we'll reset them when we get the Celsius data.
            tempCriticalF = tempCriticalK = tempHotF = tempHotK = tempOverheatedF = tempOverheatedK = tempWarmF = tempWarmK = 0;

            // Do not ignore hot or warm, unless changed by user.
            ignoreHot = false;
            ignoreWarm = false;

            // We do report criticals and warnings, unless changed by the user.
            reportCritical = true;
            reportWarnings = true;

            // Geriatrics
            reportGeriatric = true;

            // Preserve settings on uninstall
            preserveOnUninstall = true;

            // Default troubleshooting/debugging options.
            fallbackToWmi = true;
            debugMode = false;
            advancedSii = false;

            // We assume the email configuration is invalid until proven otherwise.
            isMailConfigValid = false;

            // Default poling interval 15 minutes (900000 milliseconds)
            pollingInterval = 900000;

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

            // Default BitLocker
            bitLockerShowAllDrives = true;
            bitLockerPreventLocking = false;
            bitLockerHideTab = false;

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
            isWindowsPhoneEnabled = false;
            isPushoverEnabled = false;
            prowlApiKey = String.Empty;
            nmaApiKey = String.Empty;
            wp7Guids = String.Empty;
            isGrowlEnabled = false;
            isGrowlRemoteEnabled = false;
            isSnarlEnabled = false;
            growlRemoteTarget = String.Empty;
            growlPort = 23053;
            growlPassword = "N@yNayDefined";
            pushoverUserKey = String.Empty;
            pushoverClearedSound = PUSHOVER_DEFAULT_SOUND;
            pushoverCriticalSound = PUSHOVER_DEFAULT_SOUND;
            pushoverWarningSound = PUSHOVER_DEFAULT_SOUND;
            pushoverDeviceTarget = String.Empty;

            // System Options
            notifyOnPowerChange = true;
            notifyOnFilthyShutdown = true;

            // Skinning
            useDefaultSkinning = true;
            windowBackground = 0;

            // Logging
            debugLogLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Dojo North Software\\Logs";

            // Emergency Ops
            performEmergencyBackup = false;
            performCustomBackup = false;
            noHotBackup = true;
            backupLocal = true;
            localBackupPath = String.Empty;
            uncBackupPath = String.Empty;
            uncBackupUser = String.Empty;
            uncBackupPassword = "F1ng3rL1ckin";
            performThermalShutdown = false;
            customBackupProgram = String.Empty;
            customBackupArgs = String.Empty;

            // GPO
            gpoDpa = 2;
            gpoTempCtl = 2;
            gpoVirtualIgnore = 2;
            gpoAllowIgnoredItems = 2;
            gpoAllowServiceControl = 2;
            gpoEmailNotificate = 2;
            gpoProwlNotificate = 2;
            gpoNmaNotificate = 2;
            gpoPushoverNotificate = 2;
            gpoWpNotificate = 2;
            gpoGrowlNotificate = 2;
            gpoSnarlNotificate = 2;
            gpoAdvancedSettings = 2;
            gpoDebuggingControl = 2;
            gpoSSD = 2;
            gpoCheckForUpdates = 2;
            gpoUiTheme = 2;
            gpoUseSupportMessage = 2;
            gpoEmergencyBackup = 2;
            allowShutdown = true;
            allowDeleteStale = true;
            allowRestoreDefaults = true;
            allowResetEverything = true;
            gpoDebuggingAllowed = "Allowed";

            if (!isRegistryAvailable)
            {
                try
                {
                    dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                    configurationKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryConfigurationKey);
                    isRegistryAvailable = true;
                    SaveData();
                }
                catch (Exception ex)
                {
                    isRegistryAvailable = false;
                    QMessageBox.Show(Properties.Resources.ErrorMessageRegistryWriteFailed + ex.Message, Properties.Resources.ErrorMessageTitleSevere,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveData()
        {
            if (!isRegistryAvailable)
            {
                QMessageBox.Show(Properties.Resources.ErrorMessageRegistryWriteFailed, Properties.Resources.ErrorMessageTitleSevere,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool isPartiallyDefined = false;
            //isMailConfigValid = IsEmailConfigurationValid(out isPartiallyDefined, out mailAlertsEnabled);

            // If the email alerts are partially defined (at least one value populated), enabled BUT invalid, display
            // a message to the user. ~ Jake ~
            if (isPartiallyDefined && mailAlertsEnabled && !isMailConfigValid)
            {
                QMessageBox.Show(Properties.Resources.ErrorMessageInvalidEmailConfig, Properties.Resources.ErrorMessageTitleInvalidEmail,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            try
            {
                // Temperature preference.
                switch (temperaturePreference)
                {
                    case "F":
                        {
                            configurationKey.SetValue(Properties.Resources.RegistryConfigTemperaturePreference, "F");
                            break;
                        }
                    case "K":
                        {
                            configurationKey.SetValue(Properties.Resources.RegistryConfigTemperaturePreference, "K");
                            break;
                        }
                    case "C":
                    default:
                        {
                            configurationKey.SetValue(Properties.Resources.RegistryConfigTemperaturePreference, "C");
                            break;
                        }
                }

                SiAuto.Main.LogMessage("Saving temperature configuration.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigCriticalTempThreshold, criticalTemperatureThreshold);
                configurationKey.SetValue(Properties.Resources.RegistryConfigOverheatedTempThreshold, overheatedTemperatureThreshold);
                configurationKey.SetValue(Properties.Resources.RegistryConfigHotTempThreshold, hotTemperatureThreshold);
                configurationKey.SetValue(Properties.Resources.RegistryConfigWarmTempThreshold, warmTemperatureThreshold);

                SiAuto.Main.LogMessage("Saving preserve on uninstall configuration.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigPreserveOnUninstall, preserveOnUninstall);

                SiAuto.Main.LogMessage("Saving ignore hot/warm configuration.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIgnoreHot, ignoreHot);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIgnoreWarm, ignoreWarm);
                configurationKey.SetValue(Properties.Resources.RegistryConfigReportGeriatric, reportGeriatric);

                SiAuto.Main.LogMessage("Saving reporting and polling configuration.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigReportCritical, reportCritical);
                configurationKey.SetValue(Properties.Resources.RegistryConfigReportWarning, reportWarnings);
                configurationKey.SetValue(Properties.Resources.RegistryConfigPollingInterval, pollingInterval);

                SiAuto.Main.LogMessage("Saving advanced/debug configuration.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigFallbackToWmi, fallbackToWmi);
                configurationKey.SetValue(Properties.Resources.RegistryConfigEnableDebugLogging, debugMode);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSiIAdvanced, advancedSii);

                // SSD thresholds
                SiAuto.Main.LogMessage("Saving SSD threshold configuration.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdLifeLeftCritical, ssdLifeLeftCritical);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdLifeLeftWarning, ssdLifeLeftWarning);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdRetirementCritical, ssdRetirementCritical);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdRetirementWarning, ssdRetirementWarning);

                // Email Configuration ~ Jake Scherer ~
                SiAuto.Main.LogMessage("Saving email configuration.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailServer, mailServer);
                configurationKey.SetValue(Properties.Resources.RegistryConfigServerPort, serverPort);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSenderFriendly, senderFriendlyName);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSenderEmail, senderEmailAddress);
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientFriendly, recipientFriendlyName);
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail, recipientEmailAddress);
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail2, recipientEmail2);
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail3, recipientEmail3);
                configurationKey.SetValue(Properties.Resources.RegistryConfigAuthenticationEnabled, authenticationEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailUser, crypto.Encrypt(mailUser));
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailPassword, crypto.Encrypt(mailPassword));
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailAlertsEnabled, mailAlertsEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigUseSSL, useSsl);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSendDailySummary, sendDailySummary);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSendPlaintext, sendPlaintext);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, isMailConfigValid);

                // Notifications
                SiAuto.Main.LogMessage("Saving Boxcar/Prowl/NMA/Windows Phone configuration.");
                SiAuto.Main.LogBool("isProwlEnabled", isProwlEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsProwlEnabled, isProwlEnabled);
                SiAuto.Main.LogBool("isNmaEnabled", isNmaEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsNmaEnabled, isNmaEnabled);
                SiAuto.Main.LogBool("isWindowsPhoneEnabled", isWindowsPhoneEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsToastyEnabled, isWindowsPhoneEnabled);
                SiAuto.Main.LogString("prowlApiKey", prowlApiKey);
                configurationKey.SetValue(Properties.Resources.RegistryConfigProwlApiKeychain, prowlApiKey);
                SiAuto.Main.LogString("nmaApiKey", nmaApiKey);
                configurationKey.SetValue(Properties.Resources.RegistryConfigNmaApiKeychain, nmaApiKey);
                SiAuto.Main.LogString("wp7Guids", wp7Guids);
                configurationKey.SetValue(Properties.Resources.RegistryConfigWindowsPhoneGuids, wp7Guids);
                SiAuto.Main.LogBool("isGrowlEnabled", isGrowlEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsGrowlEnabled, isGrowlEnabled);
                SiAuto.Main.LogBool("isGrowlRemoteEnabled", isGrowlRemoteEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsGrowlRemoteEnabled, isGrowlRemoteEnabled);
                SiAuto.Main.LogBool("isSnarlEnabled", isSnarlEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsSnarlEnabled, isSnarlEnabled);
                SiAuto.Main.LogString("growlRemoteTarget", growlRemoteTarget);
                configurationKey.SetValue(Properties.Resources.RegistryConfigGrowlRemoteMachine, growlRemoteTarget);
                SiAuto.Main.LogInt("growlPort", growlPort);
                configurationKey.SetValue(Properties.Resources.RegistryConfigGrowlRemotePort, growlPort);
                SiAuto.Main.LogString("growlPassword", "Passwords are NOT Logged");
                configurationKey.SetValue(Properties.Resources.RegistryConfigGrowlPassword, crypto.Encrypt(growlPassword));
                SiAuto.Main.LogString("pushoverUserKey", pushoverUserKey);
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverKey, pushoverUserKey);
                SiAuto.Main.LogString("pushoverDeviceTarget", pushoverDeviceTarget);
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverDeviceTarget, pushoverDeviceTarget);
                SiAuto.Main.LogString("pushoverClearedSound", pushoverClearedSound);
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverClearedSound, pushoverClearedSound);
                SiAuto.Main.LogString("pushoverCriticalSound", pushoverCriticalSound);
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverCriticalSound, pushoverCriticalSound);
                SiAuto.Main.LogString("pushoverWarningSound", pushoverWarningSound);
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverWarningSound, pushoverWarningSound);
                SiAuto.Main.LogBool("isPushoverEnabled", isPushoverEnabled);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsPushoverEnabled, isPushoverEnabled);

                // System Notifications
                SiAuto.Main.LogMessage("Saving system notifications configuration.");
                SiAuto.Main.LogBool("notifyOnPowerChange", notifyOnPowerChange);
                SiAuto.Main.LogBool("notifyOnFilthyShutdown", notifyOnFilthyShutdown);
                configurationKey.SetValue(Properties.Resources.RegistryConfigNotifyFilthyShutdown, notifyOnFilthyShutdown);
                configurationKey.SetValue(Properties.Resources.RegistryConfigNotifyPowerChange, notifyOnPowerChange);

                // Skinning
                SiAuto.Main.LogMessage("Saving skinning configuration.");
                SiAuto.Main.LogBool("useDefaultSkinning", useDefaultSkinning);
                SiAuto.Main.LogInt("windowBackground", windowBackground);
                configurationKey.SetValue(Properties.Resources.RegistryConfigUseDefaultSkinning, useDefaultSkinning);
                configurationKey.SetValue(Properties.Resources.RegistryConfigWindowBackground, windowBackground);

                // BitLocker
                SiAuto.Main.LogMessage("Saving BitLocker configuration.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigBitLockerShowAllDrives, bitLockerShowAllDrives);
                configurationKey.SetValue(Properties.Resources.RegistryConfigBitLockerPreventLocking, bitLockerPreventLocking);
                configurationKey.SetValue(Properties.Resources.RegistryConfigBitLockerHideTab, bitLockerHideTab);

                // Emergency Ops
                SiAuto.Main.LogMessage("Saving Emergency Ops configuration.");
                uncBackupPath = textBoxUncBackup.Text;
                uncBackupUser = textBoxBackupUsername.Text;
                uncBackupPassword = textBoxBackupPassword.Text;
                localBackupPath = textBoxLocalBackup.Text;
                customBackupProgram = textBoxCustomBackupProgram.Text;
                customBackupArgs = textBoxCustomBackupArgs.Text;
                configurationKey.SetValue(Properties.Resources.RegistryConfigUncBackupPath, uncBackupPath);
                configurationKey.SetValue(Properties.Resources.RegistryConfigUncBackupUser, uncBackupUser);
                configurationKey.SetValue(Properties.Resources.RegistryConfigUncBackupPassword, crypto.Encrypt(uncBackupPassword));
                configurationKey.SetValue(Properties.Resources.RegistryConfigLocalBackupPath, localBackupPath);
                configurationKey.SetValue(Properties.Resources.RegistryConfigPerformEmergencyBackup, performEmergencyBackup);
                configurationKey.SetValue(Properties.Resources.RegistryConfigNoHotBackup, noHotBackup);
                configurationKey.SetValue(Properties.Resources.RegistryConfigBackupLocal, backupLocal);
                configurationKey.SetValue(Properties.Resources.RegistryConfigThermalShutdown, performThermalShutdown);
                configurationKey.SetValue(Properties.Resources.RegistryConfigCustomBackupProgram, customBackupProgram);
                configurationKey.SetValue(Properties.Resources.RegistryConfigPerformCustomBackup, performCustomBackup);
                configurationKey.SetValue(Properties.Resources.RegistryConfigCustomBackupArgs, customBackupArgs);

                // Virtual disk ignoring ~ Dan ~
                SiAuto.Main.LogMessage("Saving VD ignore configuration.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIgnoreVirtualDisks, ignoreVirtualDisks);

                // Logging
                SiAuto.Main.LogMessage("Saving debug log configuration.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigLogLocation, debugLogLocation);
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Exceptions were detected during the save. " + ex.Message);
                SiAuto.Main.LogException(ex);
                QMessageBox.Show(Properties.Resources.ErrorMessageRegistryWriteFailed + ex.Message, Properties.Resources.ErrorMessageTitleSevere,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonSave.Enabled = false;
                buttonCancel.Text = "false";
            }
        }

        private void LoadDataFromRegistry()
        {
            bool exceptionsDetected = false;
            bool cruftDataDetected = false;
            String errors = String.Empty;

            try
            {
                temperaturePreference = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigTemperaturePreference);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigTemperaturePreference, temperaturePreference);
                exceptionsDetected = true;
                errors += "TemperaturePreference ";
            }

            try
            {
                ignoreHot = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIgnoreHot));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigIgnoreHot, ignoreHot);
                exceptionsDetected = true;
                errors += "IgnoreHot ";
            }

            try
            {
                ignoreWarm = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIgnoreWarm));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigIgnoreWarm, ignoreWarm);
                exceptionsDetected = true;
                errors += "IgnoreWarm ";
            }

            try
            {
                preserveOnUninstall = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigPreserveOnUninstall));
            }
            catch
            {
                preserveOnUninstall = true;
                configurationKey.SetValue(Properties.Resources.RegistryConfigPreserveOnUninstall, preserveOnUninstall);
                exceptionsDetected = true;
                errors += "PreserveOnUninstall ";
            }

            try
            {
                criticalTemperatureThreshold = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigCriticalTempThreshold);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigCriticalTempThreshold, criticalTemperatureThreshold);
                exceptionsDetected = true;
                errors += "CriticalTempThreshold ";
            }

            try
            {
                upDownTempCritical.Value = criticalTemperatureThreshold;
            }
            catch
            {
                cruftDataDetected = true;
            }

            try
            {
                overheatedTemperatureThreshold = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigOverheatedTempThreshold);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigOverheatedTempThreshold, overheatedTemperatureThreshold);
                exceptionsDetected = true;
                errors += "OverheatedTempThreshold ";
            }

            try
            {
                upDownTempOverheated.Value = overheatedTemperatureThreshold;
            }
            catch
            {
                cruftDataDetected = true;
            }

            try
            {
                hotTemperatureThreshold = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigHotTempThreshold);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigHotTempThreshold, hotTemperatureThreshold);
                exceptionsDetected = true;
                errors += "HotTempThreshold ";
            }

            try
            {
                upDownTempHot.Value = hotTemperatureThreshold;
            }
            catch
            {
                cruftDataDetected = true;
            }

            try
            {
                warmTemperatureThreshold = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigWarmTempThreshold);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigWarmTempThreshold, warmTemperatureThreshold);
                exceptionsDetected = true;
                errors += "WarmTempThreshold ";
            }

            try
            {
                upDownTempWarm.Value = warmTemperatureThreshold;
            }
            catch
            {
                cruftDataDetected = true;
            }

            try
            {
                pollingInterval = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigPollingInterval);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigPollingInterval, pollingInterval);
                exceptionsDetected = true;
                errors += "PollingInterval ";
            }

            try
            {
                reportCritical = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigReportCritical));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigReportCritical, reportCritical);
                exceptionsDetected = true;
                errors += "ReportCritical ";
            }

            try
            {
                reportWarnings = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigReportWarning));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigReportWarning, reportWarnings);
                exceptionsDetected = true;
                errors += "ReportWarning ";
            }

            try
            {
                reportGeriatric = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigReportGeriatric));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigReportGeriatric, reportGeriatric);
                exceptionsDetected = true;
                errors += "ReportGeriatric ";
            }

            try
            {
                fallbackToWmi = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigFallbackToWmi));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigFallbackToWmi, fallbackToWmi);
                exceptionsDetected = true;
                errors += "FallbackToWmi ";
            }

            try
            {
                debugMode = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigEnableDebugLogging));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigEnableDebugLogging, debugMode);
                exceptionsDetected = true;
                errors += "EnableDebugLogging ";
            }

            try
            {
                advancedSii = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigSiIAdvanced));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigSiIAdvanced, advancedSii);
                exceptionsDetected = true;
                errors += "SiIAdvanced ";
            }

            // BitLocker configuration
            try
            {
                bitLockerShowAllDrives = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigBitLockerShowAllDrives));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigBitLockerShowAllDrives, bitLockerShowAllDrives);
                exceptionsDetected = true;
                errors += "BitLockerShowAllDrives ";
            }

            try
            {
                bitLockerPreventLocking = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigBitLockerPreventLocking));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigBitLockerPreventLocking, bitLockerPreventLocking);
                exceptionsDetected = true;
                errors += "BitLockerPreventLocking ";
            }

            try
            {
                bitLockerHideTab = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigBitLockerHideTab));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigBitLockerHideTab, bitLockerHideTab);
                exceptionsDetected = true;
                errors += "HideBitLockerTab ";
            }

            // Email Configuration ~ Jake Scherer ~
            try
            {
                isMailConfigValid = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsEmailConfigValid));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
                exceptionsDetected = true;
                errors += "IsEmailConfigValid ";
            }

            try
            {
                mailServer = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailServer);
                if (String.IsNullOrEmpty(mailServer))
                {
                    mailServer = String.Empty;
                }
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailServer, mailServer);
                exceptionsDetected = true;
                errors += "MailServer ";
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                serverPort = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigServerPort);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigServerPort, 25);
                serverPort = 25;
                exceptionsDetected = true;
                errors += "ServerPort ";
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                senderFriendlyName = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigSenderFriendly);
                if (String.IsNullOrEmpty(senderFriendlyName))
                {
                    senderFriendlyName = String.Empty;
                }
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigSenderFriendly, senderFriendlyName);
                exceptionsDetected = true;
                errors += "SenderFriendly ";
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                senderEmailAddress = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigSenderEmail);
                if (String.IsNullOrEmpty(senderEmailAddress))
                {
                    senderEmailAddress = String.Empty;
                }
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigSenderEmail, senderEmailAddress);
                exceptionsDetected = true;
                errors += "SenderEmail ";
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                recipientFriendlyName = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientFriendly);
                if (String.IsNullOrEmpty(recipientFriendlyName))
                {
                    recipientFriendlyName = String.Empty;
                }
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientFriendly, recipientFriendlyName);
                exceptionsDetected = true;
                errors += "RecipientFriendly ";
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                recipientEmailAddress = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientEmail);
                if (String.IsNullOrEmpty(recipientEmailAddress))
                {
                    recipientEmailAddress = String.Empty;
                }
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail, recipientEmailAddress);
                exceptionsDetected = true;
                errors += "RecipientEmail ";
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                recipientEmail2 = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientEmail2);
                if (String.IsNullOrEmpty(recipientEmail2))
                {
                    recipientEmail2 = String.Empty;
                }
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail2, recipientEmail2);
                exceptionsDetected = true;
                errors += "RecipientEmail2 ";
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                recipientEmail3 = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientEmail3);
                if (String.IsNullOrEmpty(recipientEmail3))
                {
                    recipientEmail3 = String.Empty;
                }
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail3, recipientEmail3);
                exceptionsDetected = true;
                errors += "RecipientEmail3 ";
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                authenticationEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigAuthenticationEnabled));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigAuthenticationEnabled, authenticationEnabled);
                exceptionsDetected = true;
                errors += "AuthenticationEnabled ";
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                mailUser = crypto.Decrypt((String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailUser));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailUser, crypto.Encrypt(mailUser));
                exceptionsDetected = true;
                errors += "MailUser ";
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                mailPassword = crypto.Decrypt((String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailPassword));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailPassword, crypto.Encrypt(mailPassword));
                exceptionsDetected = true;
                errors += "MailPassword ";
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                mailAlertsEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailAlertsEnabled));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigMailAlertsEnabled, mailAlertsEnabled);
                exceptionsDetected = true;
                errors += "MailAlertsEnabled ";
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                useSsl = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigUseSSL));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigUseSSL, useSsl);
                exceptionsDetected = true;
                errors += "UseSSL ";
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                isMailConfigValid = false;
            }

            try
            {
                sendDailySummary = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigSendDailySummary));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigSendDailySummary, sendDailySummary);
                exceptionsDetected = true;
                errors += "SendDailySummary ";
            }

            try
            {
                sendPlaintext = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigSendPlaintext));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigSendPlaintext, sendPlaintext);
                exceptionsDetected = true;
                errors += "SendPlaintext ";
            }

            try
            {
                ignoreVirtualDisks = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIgnoreVirtualDisks));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigIgnoreVirtualDisks, ignoreVirtualDisks);
                exceptionsDetected = true;
                errors += "IgnoreVirtualDisks ";
            }

            try
            {
                useHelperService = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigUseHelperService));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigUseHelperService, useHelperService);
                exceptionsDetected = true;
                errors += "UseHelperService ";
            }

            try
            {
                helperServiceDiskPoolProvider = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigDiskPoolProvider);
                if (String.IsNullOrEmpty(helperServiceDiskPoolProvider))
                {
                    helperServiceDiskPoolProvider = String.Empty;
                    configurationKey.SetValue(Properties.Resources.RegistryConfigDiskPoolProvider, helperServiceDiskPoolProvider);
                }
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigDiskPoolProvider, String.Empty);
                exceptionsDetected = true;
                errors += "DiskPoolProvider ";
            }

            // SSD thresholds
            try
            {
                ssdLifeLeftCritical = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigSsdLifeLeftCritical);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdLifeLeftCritical, ssdLifeLeftCritical);
                exceptionsDetected = true;
                errors += "CriticalTempThreshold ";
            }

            try
            {
                upDownSsdLifeLeftCritical.Value = ssdLifeLeftCritical;
            }
            catch
            {
                cruftDataDetected = true;
            }

            try
            {
                ssdLifeLeftWarning = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigSsdLifeLeftWarning);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdLifeLeftWarning, ssdLifeLeftWarning);
                exceptionsDetected = true;
                errors += "CriticalTempThreshold ";
            }

            try
            {
                upDownSsdLifeLeftWarning.Value = ssdLifeLeftWarning;
            }
            catch
            {
                cruftDataDetected = true;
            }

            try
            {
                ssdRetirementCritical = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigSsdRetirementCritical);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdRetirementCritical, ssdRetirementCritical);
                exceptionsDetected = true;
                errors += "CriticalTempThreshold ";
            }

            try
            {
                upDownSsdRetirementCritical.Value = ssdRetirementCritical;
            }
            catch
            {
                cruftDataDetected = true;
            }

            try
            {
                ssdRetirementWarning = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigSsdRetirementWarning);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdRetirementWarning, ssdRetirementWarning);
                exceptionsDetected = true;
                errors += "CriticalTempThreshold ";
            }

            try
            {
                upDownSsdRetirementWarning.Value = ssdRetirementWarning;
            }
            catch
            {
                cruftDataDetected = true;
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
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsNmaEnabled, isNmaEnabled);
                exceptionsDetected = true;
            }

            try
            {
                isWindowsPhoneEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsToastyEnabled));
            }
            catch
            {
                SiAuto.Main.LogWarning("Is Windows Phone Enabled was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsToastyEnabled, isWindowsPhoneEnabled);
                exceptionsDetected = true;
            }

            try
            {
                prowlApiKey = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigProwlApiKeychain);
                if (String.IsNullOrEmpty(prowlApiKey))
                {
                    prowlApiKey = String.Empty;
                }
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
                if (String.IsNullOrEmpty(nmaApiKey))
                {
                    nmaApiKey = String.Empty;
                }
            }
            catch
            {
                SiAuto.Main.LogWarning("NMA API Keychain was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigNmaApiKeychain, nmaApiKey);
                exceptionsDetected = true;
            }

            try
            {
                wp7Guids = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigWindowsPhoneGuids);
                if (String.IsNullOrEmpty(wp7Guids))
                {
                    wp7Guids = String.Empty;
                }
            }
            catch
            {
                SiAuto.Main.LogWarning("Windows Phone (Toasty) GUID was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigWindowsPhoneGuids, wp7Guids);
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
                SiAuto.Main.LogWarning("Is Snarl Enabled was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigIsSnarlEnabled, isSnarlEnabled);
                exceptionsDetected = true;
            }

            try
            {
                growlRemoteTarget = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigGrowlRemoteMachine);
                if (String.IsNullOrEmpty(growlRemoteTarget))
                {
                    growlRemoteTarget = String.Empty;
                }
            }
            catch
            {
                SiAuto.Main.LogWarning("Growl Remote Target was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigGrowlRemoteMachine, growlRemoteTarget);
                exceptionsDetected = true;
            }

            try
            {
                growlPort = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGrowlRemotePort);
            }
            catch
            {
                SiAuto.Main.LogWarning("Growl Port was undefined or defined value was corrupt; it has been reset to default.");
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
                if (String.IsNullOrEmpty(pushoverUserKey))
                {
                    pushoverUserKey = String.Empty;
                }
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
                if (String.IsNullOrEmpty(pushoverClearedSound))
                {
                    pushoverClearedSound = PUSHOVER_DEFAULT_SOUND;
                }
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
                if (String.IsNullOrEmpty(pushoverCriticalSound))
                {
                    pushoverCriticalSound = PUSHOVER_DEFAULT_SOUND;
                }
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
                if (String.IsNullOrEmpty(pushoverWarningSound))
                {
                    pushoverWarningSound = PUSHOVER_DEFAULT_SOUND;
                }
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
                if (String.IsNullOrEmpty(pushoverDeviceTarget))
                {
                    pushoverDeviceTarget = String.Empty;
                }
            }
            catch
            {
                SiAuto.Main.LogWarning("Pushover device target was undefined or defined value was corrupt; it has been reset to default.");
                configurationKey.SetValue(Properties.Resources.RegistryConfigPushoverDeviceTarget, pushoverDeviceTarget);
                exceptionsDetected = true;
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
                backupLocal = true;
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
                configurationKey.SetValue(Properties.Resources.RegistryConfigWindowBackground, 0);
                exceptionsDetected = true;
            }

            // Logging
            try
            {
                String defaultLogLocation = debugLogLocation;
                debugLogLocation = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigLogLocation);
                if (String.Compare(debugLogLocation, "%DEFAULT%", true) == 0)
                {
                    SiAuto.Main.LogMessage("Using default log location because log location is set to %DEFAULT%.");
                    debugLogLocation = defaultLogLocation;
                }
                if (String.IsNullOrEmpty(debugLogLocation) || !System.IO.Directory.Exists(debugLogLocation))
                {
                    debugLogLocation = defaultLogLocation;
                    throw new System.IO.DirectoryNotFoundException("Invalid logfile path.");
                }
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigLogLocation, debugLogLocation);
                exceptionsDetected = true;
                errors += "DebugLogLocation ";
            }

            // GPO (we don't track exceptions for these)
            try
            {
                gpoDpa = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoDpa);
                if (gpoDpa == 0)
                {
                    QMessageBox.Show("Please contact your system administrator and inform them that the WindowSMART 24/7 policy setting " +
                        "\"disk polling and analysis\" defined in system policy is not configured correctly. This policy setting has been " +
                        "set to Disabled. The valid settings are Enabled or Not Configured.", "System Policy Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Hand);
                }
            }
            catch
            {
                gpoDpa = 2;
            }

            try
            {
                gpoTempCtl = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoTempCtl);
                if (gpoTempCtl == 0)
                {
                    QMessageBox.Show("Please contact your system administrator and inform them that the WindowSMART 24/7 policy setting " +
                        "\"disk temperature preferences and alerts\" defined in system policy is not configured correctly. This policy setting has been " +
                        "set to Disabled. The valid settings are Enabled or Not Configured.", "System Policy Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Hand);
                }
            }
            catch
            {
                gpoTempCtl = 2;
            }

            try
            {
                gpoVirtualIgnore = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoVirtualIgnore);
            }
            catch
            {
                gpoVirtualIgnore = 2;
            }

            try
            {
                gpoAllowIgnoredItems = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoAllowIgnoredItems);
            }
            catch
            {
                gpoAllowIgnoredItems = 2;
            }

            try
            {
                gpoAllowServiceControl = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoAllowServiceControl);
            }
            catch
            {
                gpoAllowServiceControl = 2;
            }

            try
            {
                allowShutdown = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoAllowShutdownReboot));
            }
            catch
            {
                allowShutdown = true;
            }

            try
            {
                gpoEmailNotificate = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoEmailNotificate);
                if (gpoEmailNotificate == 0)
                {
                    // Disable email alerts by policy!
                    mailAlertsEnabled = false;
                }
            }
            catch
            {
                gpoEmailNotificate = 2;
            }

            try
            {
                gpoProwlNotificate = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoProwlNotificate);
                if (gpoProwlNotificate == 0)
                {
                    // Disable Prowl by policy!
                    isProwlEnabled = false;
                }
            }
            catch
            {
                gpoProwlNotificate = 2;
            }

            try
            {
                gpoPushoverNotificate = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoPushoverNotificate);
                if (gpoPushoverNotificate == 0)
                {
                    // Disable Boxcar by policy!
                    isPushoverEnabled = false;
                }
            }
            catch
            {
                gpoPushoverNotificate = 2;
            }

            try
            {
                gpoNmaNotificate = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoNmaNotificate);
                if (gpoNmaNotificate == 0)
                {
                    // Disable NMA by policy!
                    isNmaEnabled = false;
                }
            }
            catch
            {
                gpoNmaNotificate = 2;
            }

            try
            {
                gpoWpNotificate = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoWindowsPhoneNotificate);
                if (gpoWpNotificate == 0)
                {
                    // Disable Windows Phone by policy!
                    isWindowsPhoneEnabled = false;
                }
            }
            catch
            {
                gpoWpNotificate = 2;
            }

            try
            {
                gpoGrowlNotificate = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoGrowlNotificate);
                if (gpoGrowlNotificate == 0)
                {
                    // Disable Growl by policy!
                    isGrowlEnabled = false;
                    isGrowlRemoteEnabled = false;
                }
            }
            catch
            {
                gpoGrowlNotificate = 2;
            }

            try
            {
                gpoSnarlNotificate = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoSnarlNotificate);
                if (gpoSnarlNotificate == 0)
                {
                    // Disable Growl by policy!
                    isSnarlEnabled = false;
                }
            }
            catch
            {
                gpoSnarlNotificate = 2;
            }

            try
            {
                gpoAdvancedSettings = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoAdvancedSettings);
            }
            catch
            {
                gpoAdvancedSettings = 2;
            }

            try
            {
                allowDeleteStale = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoDeleteStale));
            }
            catch
            {
                allowDeleteStale = true;
            }

            try
            {
                allowRestoreDefaults = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoRestoreDefaults));
            }
            catch
            {
                allowRestoreDefaults = true;
            }

            try
            {
                allowResetEverything = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoResetEverything));
            }
            catch
            {
                allowResetEverything = true;
            }

            try
            {
                gpoDebuggingControl = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoDebuggingControl);
            }
            catch
            {
                gpoDebuggingControl = 2;
            }

            try
            {
                gpoDebuggingAllowed = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigLogRequirement);
            }
            catch
            {
                gpoDebuggingAllowed = "Allowed";
            }

            try
            {
                gpoSSD = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoSSD);
                if (gpoSSD == 0)
                {
                    QMessageBox.Show("Please contact your system administrator and inform them that the WindowSMART 24/7 policy setting " +
                        "\"SSD-specific settings\" defined in system policy is not configured correctly. This policy setting has been " +
                        "set to Disabled. The valid settings are Enabled or Not Configured.", "System Policy Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Hand);
                }
            }
            catch
            {
                gpoSSD = 2;
            }

            try
            {
                gpoCheckForUpdates = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoCheckForUpdates);
            }
            catch
            {
                gpoCheckForUpdates = 2;
            }

            try
            {
                gpoUiTheme = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoUiTheme);
            }
            catch
            {
                gpoUiTheme = 2;
            }

            try
            {
                gpoUseSupportMessage = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoUseSupportMessage);
            }
            catch
            {
                gpoUseSupportMessage = 2;
            }

            try
            {
                gpoEmergencyBackup = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoEmergencyBackup);
                if (gpoEmergencyBackup == 0)
                {
                    // Disable Growl by policy!
                    performEmergencyBackup = false;
                    performCustomBackup = false;
                }
            }
            catch
            {
                gpoEmergencyBackup = 2;
            }

            if(cruftDataDetected)
            {
                QMessageBox.Show("Cruft data was detected in the Registry. Either the Registry data has been corrupted, or was manually edited. The configuration data should " +
                    "not be edited manually. Some configuration data could not be correctly loaded. Please perform a \"Reset Defaults\" operation to restore the default configuration, " +
                    "and then make any customizations through this Settings dialogue.", "Severe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (exceptionsDetected)
            {
                QMessageBox.Show(Properties.Resources.ErrorMessageRegistryReadsFailedWarn, Properties.Resources.WindowTitleWarning,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CalculateAlternateTemperatures()
        {
            // Celsius
            criticalTemperatureThreshold = (int)upDownTempCritical.Value;
            overheatedTemperatureThreshold = (int)upDownTempOverheated.Value;
            hotTemperatureThreshold = (int)upDownTempHot.Value;
            warmTemperatureThreshold = (int)upDownTempWarm.Value;

            // Fahrenheit
            tempCriticalF = CalculateFahrenheitTemperature(criticalTemperatureThreshold);
            tempOverheatedF = CalculateFahrenheitTemperature(overheatedTemperatureThreshold);
            tempHotF = CalculateFahrenheitTemperature(hotTemperatureThreshold);
            tempWarmF = CalculateFahrenheitTemperature(warmTemperatureThreshold);

            // Kelvin
            tempCriticalK = CalculateKelvinTemperature(criticalTemperatureThreshold);
            tempOverheatedK = CalculateKelvinTemperature(overheatedTemperatureThreshold);
            tempHotK = CalculateKelvinTemperature(hotTemperatureThreshold);
            tempWarmK = CalculateKelvinTemperature(warmTemperatureThreshold);
        }

        private void PopulateData()
        {
            loadInProgress = true;

            if (isRegistryAvailable)
            {
                LoadDataFromRegistry();
            }
            else
            {
                // Temperature preference is assumed to be Celsius (C) until otherwise loaded.
                temperaturePreference = "C";

                criticalTemperatureThreshold = 65;
                upDownTempCritical.Value = criticalTemperatureThreshold;
                overheatedTemperatureThreshold = 55;
                upDownTempOverheated.Value = overheatedTemperatureThreshold;
                hotTemperatureThreshold = 50;
                upDownTempHot.Value = hotTemperatureThreshold;
                warmTemperatureThreshold = 42;
                upDownTempWarm.Value = warmTemperatureThreshold;
                checkBoxThermalShutdown.Checked = performThermalShutdown;

                // Do not ignore hot or warm, unless changed by user.
                ignoreHot = false;
                ignoreWarm = false;

                // Geriatrics
                reportGeriatric = true;

                // SSD thresholds
                ssdLifeLeftCritical = 10;
                ssdLifeLeftWarning = 30;
                ssdRetirementCritical = 150;
                ssdRetirementWarning = 50;
                upDownSsdLifeLeftCritical.Value = ssdLifeLeftCritical;
                upDownSsdLifeLeftWarning.Value = ssdLifeLeftWarning;
                upDownSsdRetirementCritical.Value = ssdRetirementCritical;
                upDownSsdRetirementWarning.Value = ssdRetirementWarning;
            }

            CalculateAlternateTemperatures();

            switch (temperaturePreference)
            {
                case "F":
                    {
                        radioButtonFahrenheit.Checked = true;
                        break;
                    }
                case "K":
                    {
                        radioButtonKelvin.Checked = true;
                        break;
                    }
                case "C":
                default:
                    {
                        radioButtonCelsius.Checked = true;
                        break;
                    }
            }

            checkBoxIgnoreHot.Checked = ignoreHot;
            checkBoxIgnoreWarm.Checked = ignoreWarm;
            if (checkBoxIgnoreHot.Checked == false && checkBoxIgnoreWarm.Checked == false)
            {
                checkBoxIgnoreHot.Enabled = false;
            }
            checkBoxThermalShutdown.Checked = performThermalShutdown;

            checkBoxPreserve.Checked = preserveOnUninstall;

            checkBoxAlertCritical.Checked = reportCritical;
            checkBoxAlertWarning.Checked = reportWarnings;
            checkBoxAlertGeriatric.Checked = reportGeriatric;

            try
            {
                numericUpDown1.Value = (pollingInterval / 1000) / 60;
            }
            catch
            {
                numericUpDown1.Value = 10;
            }

            // BitLocker
            cbBlShowAllVolumes.Checked = bitLockerShowAllDrives;
            cbBlPreventLocking.Checked = bitLockerPreventLocking;
            cbHideBitLocker.Checked = bitLockerHideTab;

            checkBoxFallbackWmi.Checked = fallbackToWmi;
            checkBoxDebugMode.Checked = debugMode;
            checkBoxAdvancedSii.Checked = advancedSii;

            cbIgnoreVirtual.Checked = ignoreVirtualDisks;

            textBoxDebugLocation.Text = debugLogLocation;

            // Skinning
            // Main Window Background
            switch (windowBackground)
            {
                case 1: // Lightning
                    {
                        radioButtonLightning.Checked = true;
                        break;
                    }
                case 2: // Cracked Glass
                    {
                        radioButtonCrackedGlass.Checked = true;
                        break;
                    }
                case 3: // No Texture
                    {
                        radioButtonNoTexture.Checked = true;
                        break;
                    }
                case 0: // Metal Grate
                default: // Invalid Value so use 0
                    {
                        radioButtonMetalGrate.Checked = true;
                        break;
                    }
            }

            // Header Colors
            if (useDefaultSkinning)
            {
                radioButtonGreen.Checked = true;
            }
            else
            {
                radioButtonBlue.Checked = true;
            }

            // Now apply Policy Settings!
            // DPA
            if (gpoDpa < 2)
            {
                numericUpDown1.Enabled = false;
                checkBoxAlertWarning.Enabled = false;
                checkBoxAlertGeriatric.Enabled = false;
            }

            // Temperatures
            if (gpoTempCtl < 2)
            {
                radioButtonCelsius.Enabled = false;
                radioButtonFahrenheit.Enabled = false;
                radioButtonKelvin.Enabled = false;
                upDownTempCritical.Enabled = false;
                upDownTempOverheated.Enabled = false;
                upDownTempHot.Enabled = false;
                upDownTempWarm.Enabled = false;
                checkBoxIgnoreHot.Enabled = false;
                checkBoxIgnoreWarm.Enabled = false;
                checkBoxThermalShutdown.Enabled = false;
            }

            // Virtual disks
            if (gpoVirtualIgnore < 2)
            {
                cbIgnoreVirtual.Enabled = false;
            }

            // Service Control
            if (gpoAllowServiceControl < 2)
            {
                if (gpoAllowServiceControl == 0)
                {
                    buttonStartService.Enabled = false;
                    buttonStopService.Enabled = false;
                    buttonRestartService.Enabled = false;
                    buttonReboot.Enabled = false;
                    buttonShutDown.Enabled = false;
                    checkBoxForce.Enabled = false;
                    checkBoxEnableReboot.Enabled = false;
                }
                else
                {
                    if (!allowShutdown)
                    {
                        buttonReboot.Enabled = false;
                        buttonShutDown.Enabled = false;
                        checkBoxForce.Enabled = false;
                        checkBoxEnableReboot.Enabled = false;
                    }
                }
            }

            // Email, Boxcar, Prowl, NMA and Windows Phone
            if (gpoEmailNotificate < 2)
            {
                buttonConfigureMail.Enabled = false;
            }

            if (gpoNmaNotificate < 2)
            {
                //buttonConfigureNma.Enabled = false;
            }

            if (gpoWpNotificate < 2)
            {
                buttonConfigureWP7.Enabled = false;
            }

            if (gpoGrowlNotificate < 2)
            {
                buttonConfigureGrowl.Enabled = false;
            }

            if (gpoSnarlNotificate < 2)
            {
                buttonConfigureSnarl.Enabled = false;
            }

            // Advanced (non debug)
            if (gpoAdvancedSettings < 2)
            {
                if (gpoAdvancedSettings == 0)
                {
                    buttonDeleteStale.Enabled = false;
                    buttonRestoreDefaults.Enabled = false;
                    buttonResetEverything.Enabled = false;
                }
                else
                {
                    buttonDeleteStale.Enabled = allowDeleteStale;
                    buttonRestoreDefaults.Enabled = allowRestoreDefaults;
                    buttonResetEverything.Enabled = allowResetEverything;
                }
                checkBoxFallbackWmi.Enabled = false;
                checkBoxAdvancedSii.Enabled = false;
            }

            // Debug
            if (gpoDebuggingControl == 0 || (gpoDebuggingControl == 1 && String.Compare(gpoDebuggingAllowed, "Forbidden", true) == 0))
            {
                // Forbidden
                checkBoxDebugMode.Enabled = false;
                checkBoxDebugMode.Checked = false;
                textBoxDebugLocation.ReadOnly = true;
                buttonChangeLogLocation.Enabled = false;
            }
            else if (gpoDebuggingControl == 1 && String.Compare(gpoDebuggingAllowed, "Mandatory", true) == 0)
            {
                // Mandatory
                checkBoxDebugMode.Enabled = false;
                checkBoxDebugMode.Checked = true;
                textBoxDebugLocation.ReadOnly = true;
                buttonChangeLogLocation.Enabled = false;
            }
            else if (gpoDebuggingControl == 1)
            {
                // Allowed but path is set by policy.
                textBoxDebugLocation.ReadOnly = true;
                buttonChangeLogLocation.Enabled = false;
            }

            // SSD
            if (gpoSSD < 2)
            {
                upDownSsdLifeLeftCritical.Enabled = false;
                upDownSsdLifeLeftWarning.Enabled = false;
                upDownSsdRetirementCritical.Enabled = false;
                upDownSsdRetirementWarning.Enabled = false;
            }

            // Updates
            if (gpoCheckForUpdates == 0)
            {
                buttonCheck.Enabled = false;
                checkBoxPreserve.Enabled = false;
            }

            // Theme
            if (gpoUiTheme < 2)
            {
                radioButtonMetalGrate.Enabled = false;
                radioButtonLightning.Enabled = false;
                radioButtonCrackedGlass.Enabled = false;
                radioButtonNoTexture.Enabled = false;
                radioButtonGreen.Enabled = false;
                radioButtonBlue.Enabled = false;
            }

            // System Notifications
            checkBoxAlertAmperes.Checked = notifyOnPowerChange;
            checkBoxAlertFilthy.Checked = notifyOnFilthyShutdown;

            // Emergency Ops
            checkBoxBackup.Checked = performEmergencyBackup;
            checkBoxNoHotBackup.Checked = noHotBackup;
            if (backupLocal)
            {
                radioButtonBackupLocal.Checked = true;
            }
            else
            {
                radioButtonBackupUnc.Checked = true;
            }
            SetLocalBackupOptions(performEmergencyBackup);

            textBoxLocalBackup.Text = localBackupPath;
            textBoxUncBackup.Text = uncBackupPath;
            textBoxBackupUsername.Text = uncBackupUser;

            if (uncBackupPassword == "F1ng3rL1ckin")
            {
                textBoxBackupPassword.Text = String.Empty;
            }
            else
            {
                textBoxBackupPassword.Text = uncBackupPassword;
            }

            checkBoxRunProgram.Checked = performCustomBackup;
            textBoxCustomBackupProgram.Text = customBackupProgram;
            textBoxCustomBackupArgs.Text = customBackupArgs;

            // Emergency Ops GPO
            if (gpoEmergencyBackup < 2)
            {
                checkBoxBackup.Enabled = false;
                checkBoxNoHotBackup.Enabled = false;
                buttonSelectTarget.Enabled = false;
                radioButtonBackupLocal.Enabled = false;
                radioButtonBackupUnc.Enabled = false;
                textBoxUncBackup.Enabled = false;
                textBoxBackupUsername.Enabled = false;
                textBoxBackupPassword.Enabled = false;
                checkBoxRunProgram.Enabled = false;
                buttonSelectCustomProgram.Enabled = false;
                textBoxCustomBackupArgs.Enabled = false;
            }

            // GPO Policy Settings begin

            if ((gpoAdvancedSettings < 2 || gpoAllowIgnoredItems < 2 || gpoCheckForUpdates < 2 || gpoDebuggingControl < 2 ||
                gpoDpa < 2 || gpoEmailNotificate < 2 || gpoNmaNotificate < 2 || gpoProwlNotificate < 2 || gpoWpNotificate < 2 || gpoSSD < 2 ||
                gpoTempCtl < 2 || gpoUiTheme < 2 || gpoUseSupportMessage < 2 || gpoVirtualIgnore < 2 || gpoAllowServiceControl < 2 || gpoGrowlNotificate < 2 || gpoSnarlNotificate < 2 ||
                gpoEmergencyBackup < 2) && !gpoApprisalShown)
            {
                QMessageBox.Show("Some or all WindowSMART 24/7 settings have been configured via policy by your system administrator. " +
                    "These settings will appear dimmed and cannot be changed by you. For further assistance, please contact your " +
                    "system administrator or helpdesk support staff.", "Policy Restrictions Enforced", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                gpoApprisalShown = true;
            }
            // End policy settings

            buttonSave.Enabled = false;
            buttonApply.Enabled = false;
            buttonCancel.Text = "Close";

            loadInProgress = false;
        }

        private int CalculateFahrenheitTemperature(int celsius)
        {
            decimal fahrenheit = (decimal)(9.00 / 5.00) * (decimal)celsius + (decimal)32.00;
            return Decimal.ToInt32(fahrenheit);
        }

        private int CalculateKelvinTemperature(int celsius)
        {
            return celsius + 273;
        }

        private void EnableButtons()
        {
            buttonSave.Enabled = true;
            buttonApply.Enabled = true;
            buttonCancel.Text = "Cancel";
        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            if (mexiSexi == null)
            {
                HelpAbout about = new HelpAbout(useDefaultSkinning);
                about.ShowDialog();
            }
            else
            {
                HelpAbout about = new HelpAbout(useDefaultSkinning, mexiSexi);
                about.ShowDialog();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2) // Ignored Items tab
            {
                RefreshIgnoredItems();
            }
            else if (tabControl1.SelectedIndex == 3) // Service Control tab
            {
                RefreshServiceStateEx();
            }
        }

        #region Events for Up/Down Temperature Changes
        private void upDownTempCritical_ValueChanged(object sender, EventArgs e)
        {
            if (upDownTempCritical.Value <= upDownTempOverheated.Value)
            {
                upDownTempOverheated.Value = upDownTempCritical.Value - 1;
            }
            CalculateAlternateTemperatures();
            UpdateAlternateTemperatures();
            EnableButtons();
        }

        private void upDownTempOverheated_ValueChanged(object sender, EventArgs e)
        {
            if (upDownTempOverheated.Value <= upDownTempHot.Value)
            {
                upDownTempHot.Value = upDownTempOverheated.Value - 1;
            }
            if (upDownTempOverheated.Value >= upDownTempCritical.Value)
            {
                upDownTempCritical.Value = upDownTempOverheated.Value + 1;
            }
            CalculateAlternateTemperatures();
            UpdateAlternateTemperatures();
            EnableButtons();
        }

        private void upDownTempHot_ValueChanged(object sender, EventArgs e)
        {
            if (upDownTempHot.Value <= upDownTempWarm.Value)
            {
                upDownTempWarm.Value = upDownTempHot.Value - 1;
            }
            if (upDownTempHot.Value >= upDownTempOverheated.Value)
            {
                upDownTempOverheated.Value = upDownTempHot.Value + 1;
            }
            CalculateAlternateTemperatures();
            UpdateAlternateTemperatures();
            EnableButtons();
        }

        private void upDownTempWarm_ValueChanged(object sender, EventArgs e)
        {
            if (upDownTempWarm.Value >= upDownTempHot.Value)
            {
                upDownTempHot.Value = upDownTempWarm.Value + 1;
            }
            CalculateAlternateTemperatures();
            UpdateAlternateTemperatures();
            EnableButtons();
        }

        private void radioButtonCelsius_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCelsius.Checked)
            {
                wizardPageFieldDegAltCritical.Visible = false;
                wizardPageFieldDegAltOverheated.Visible = false;
                wizardPageFieldDegAltHot.Visible = false;
                wizardPageFieldDegAltWarm.Visible = false;
                temperaturePreference = "C";
            }
            EnableButtons();
        }

        private void radioButtonFahrenheit_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonFahrenheit.Checked)
            {
                CalculateAlternateTemperatures();
                UpdateAlternateTemperatures();
                wizardPageFieldDegAltCritical.Visible = true;
                wizardPageFieldDegAltOverheated.Visible = true;
                wizardPageFieldDegAltHot.Visible = true;
                wizardPageFieldDegAltWarm.Visible = true;
                temperaturePreference = "F";
            }
            EnableButtons();
        }

        private void radioButtonKelvin_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonKelvin.Checked)
            {
                UpdateAlternateTemperatures();
                CalculateAlternateTemperatures();
                wizardPageFieldDegAltCritical.Visible = true;
                wizardPageFieldDegAltOverheated.Visible = true;
                wizardPageFieldDegAltHot.Visible = true;
                wizardPageFieldDegAltWarm.Visible = true;
                temperaturePreference = "K";
            }
            EnableButtons();
        }

        private void UpdateAlternateTemperatures()
        {
            if (radioButtonCelsius.Checked)
            {
                return;
            }

            if (radioButtonFahrenheit.Checked)
            {
                wizardPageFieldDegAltCritical.Text = tempCriticalF.ToString() + Properties.Resources.SettingsDegreesF;
                wizardPageFieldDegAltOverheated.Text = tempOverheatedF.ToString() + Properties.Resources.SettingsDegreesF;
                wizardPageFieldDegAltHot.Text = tempHotF.ToString() + Properties.Resources.SettingsDegreesF;
                wizardPageFieldDegAltWarm.Text = tempWarmF.ToString() + Properties.Resources.SettingsDegreesF;
            }
            else
            {
                wizardPageFieldDegAltCritical.Text = tempCriticalK.ToString() + Properties.Resources.SettingsDegreesK;
                wizardPageFieldDegAltOverheated.Text = tempOverheatedK.ToString() + Properties.Resources.SettingsDegreesK;
                wizardPageFieldDegAltHot.Text = tempHotK.ToString() + Properties.Resources.SettingsDegreesK;
                wizardPageFieldDegAltWarm.Text = tempWarmK.ToString() + Properties.Resources.SettingsDegreesK;
            }
        }
        #endregion

        #region Events for Changes on General Tab
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            pollingInterval = (int)((numericUpDown1.Value * 60) * 1000);
            EnableButtons();
        }

        private void checkBoxAlertCritical_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxAlertCritical.Checked)
            {
                if (QMessageBox.Show("Ignoring critical disk health alerts is highly discouraged. By ignoring critical alerts, you will NOT " +
                    "be informed if a disk is exhibiting a critical problem. Do you want to continue and disable critical alerts anyway?",
                    "Ignore Critical Alerts", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    checkBoxAlertCritical.Checked = true;
                    return;
                }
            }
            reportCritical = checkBoxAlertCritical.Checked;
            //if (checkBoxAlertCritical.Checked)
            //{
            //    checkBoxIgnoreOverheated.Enabled = true;
            //}
            //else
            //{
            //    checkBoxIgnoreOverheated.Enabled = false;
            //    checkBoxIgnoreOverheated.Checked = false;
            //}
            EnableButtons();
        }

        private void checkBoxIgnoreOverheated_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxAlertWarning_CheckedChanged(object sender, EventArgs e)
        {
            reportWarnings = checkBoxAlertWarning.Checked;
            if (checkBoxAlertWarning.Checked)
            {
                checkBoxIgnoreHot.Enabled = true;
                checkBoxIgnoreWarm.Enabled = true;
            }
            else
            {
                checkBoxIgnoreHot.Enabled = false;
                checkBoxIgnoreWarm.Enabled = false;
            }
            EnableButtons();
        }

        private void checkBoxIgnoreWarm_CheckedChanged(object sender, EventArgs e)
        {
            ignoreWarm = checkBoxIgnoreWarm.Checked;
            if (ignoreWarm)
            {
                checkBoxIgnoreHot.Enabled = true;
            }
            else
            {
                checkBoxIgnoreHot.Checked = false;
                checkBoxIgnoreHot.Enabled = false;
            }

            EnableButtons();
        }

        private void checkBoxIgnoreHot_CheckedChanged(object sender, EventArgs e)
        {
            ignoreHot = checkBoxIgnoreHot.Checked;
            EnableButtons();
        }

        private void checkBoxAlertGeriatric_CheckedChanged(object sender, EventArgs e)
        {
            reportGeriatric = checkBoxAlertGeriatric.Checked;
            EnableButtons();
        }
        #endregion

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveData();
            this.Close();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            SaveData();
            buttonSave.Enabled = false;
            buttonApply.Enabled = false;
            buttonCancel.Text = "Close";
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            PopulateData();
            this.Close();
        }

        private void SettingsControl_Load(object sender, EventArgs e)
        {
            wizardPageFieldTempC1.Text = wizardPageFieldTempC2.Text = wizardPageFieldTempC3.Text = wizardPageFieldTempC4.Text =
                Properties.Resources.SettingsDegreesC;

            // Home Server SMART specific items!
            if (isWindowsServerSolutions)
            {
                this.Text = Properties.Resources.ApplicationTitleHss + " Settings";
                label41.Visible = true;
                line12.Visible = true;
                cbBlPreventLocking.Visible = true;
                cbBlPreventLocking.Enabled = true;
                cbBlShowAllVolumes.Visible = true;
                cbBlShowAllVolumes.Enabled = true;
                cbHideBitLocker.Visible = true;
                cbHideBitLocker.Enabled = true;
                buttonAbout.Text = "About Home Server SMART";
                label17.Text = "Start/Stop Home Server SMART Service";

                radioButtonCrackedGlass.Visible = false;
                radioButtonLightning.Visible = false;
                pictureBox5.Visible = false;
                pictureBox6.Visible = false;
                radioButtonNoTexture.Location = new Point(26, 163);
                label33.Visible = true;

                linkLabel1.Text = Properties.Resources.ApplicationTitleHss + " Homepage";
            }

            PopulateData();
        }

        private void buttonDeleteStale_Click(object sender, EventArgs e)
        {
            if (controller == null || controller.Status != ServiceControllerStatus.Running)
            {
                QMessageBox.Show("The WindowSMART Service is not running. Please start the WindowSMART " +
                    "Service.", "Service Required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (QMessageBox.Show("Deleting stale disks cleans data about disks no longer in the Server from the " +
                "Registry. You should only delete stale disk information if you've removed one or more disks " +
                "from the Server that you do not plan to reinstall.\n\nDo you want to delete stale disk data " +
                "from the Registry?", "Delete Stale Disks", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                try
                {
                    Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                    Microsoft.Win32.RegistryKey subkey;

                    subkey = hklm.CreateSubKey("SOFTWARE\\Dojo North Software\\HomeServerSMART");

                    Microsoft.Win32.RegistryKey monitoredDisksKey = subkey.OpenSubKey("MonitoredDisks", true);

                    int disksWhacked = 0;

                    foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                    {
                        Microsoft.Win32.RegistryKey tempKey = monitoredDisksKey.OpenSubKey(diskKeyName);
                        if (tempKey.GetValue("Name") == null || String.IsNullOrEmpty((String)tempKey.GetValue("Name")) &&
                            tempKey.GetValue("DevicePath") == null || String.IsNullOrEmpty((String)tempKey.GetValue("DevicePath")))
                        {
                            tempKey.Close();
                            monitoredDisksKey.DeleteSubKeyTree(diskKeyName);
                            disksWhacked++;
                        }
                        else
                        {
                            tempKey.Close();
                        }
                    }
                    monitoredDisksKey.Close();
                    subkey.Close();
                    hklm.Close();
                    if (disksWhacked > 0)
                    {
                        QMessageBox.Show("Successfully purged " + disksWhacked.ToString() + " stale disks.", "Delete Stale Disks",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        QMessageBox.Show("There were no stale disks requiring deletion.", "Delete Stale Disks",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    QMessageBox.Show("Deletion of stale disks failed. " + ex.Message, "Severe",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void buttonRestoreDefaults_Click(object sender, EventArgs e)
        {
            if (QMessageBox.Show(Properties.Resources.ConfirmResetDefaults, Properties.Resources.WindowTitleConfirm, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SetDefaults();
                SaveData();
                PopulateData();
            }
        }

        private void buttonResetEverything_Click(object sender, EventArgs e)
        {
            // Moved this to separate method. ~ Dan ~
            ResetEverything();
        }

        private void ResetEverything()
        {
            if (QMessageBox.Show("Resetting everything will delete all WindowSMART data from the Registry and restart " +
                "the WindowSMART Service. All data stored about your disks, as well as all preferences, will be deleted. " +
                "Your disks will be re-polled when the service restarts, and new data collected. Preferences will be reset to " +
                "their defaults.\n\nYou should only reset everything if you're having serious problems with WindowSMART " +
                "that the other reset options did not fix. Are you sure you want to reset everything?", "Reset Everything",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                try
                {
                    int timer = 0;
                    try
                    {
                        controller.Stop();
                    }
                    catch (Exception ex)
                    {
                        QMessageBox.Show("Unable to stop WindowSMART Service. " + ex.Message, "Severe", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    controller.Refresh();
                    while (timer < 15 && controller.Status != ServiceControllerStatus.Stopped)
                    {
                        System.Threading.Thread.Sleep(1000);
                        controller.Refresh();
                        timer++;
                    }

                    if (controller.Status == ServiceControllerStatus.Stopped)
                    {

                    }
                    else
                    {
                        QMessageBox.Show("Service did not stop in a reasonable amount of time.", "Stop Service", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    // Now delete everything.
                    dojoNorthSubKey = registryHklm.OpenSubKey("SOFTWARE\\Dojo North Software\\HomeServerSMART", true);
                    dojoNorthSubKey.DeleteSubKeyTree("MonitoredDisks");
                    dojoNorthSubKey.DeleteSubKeyTree("Configuration");
                    dojoNorthSubKey.Close();
                    if (isWindowsServerSolutions)
                    {
                        dojoNorthSubKey = registryHklm.OpenSubKey("SOFTWARE\\Dojo North Software", true);
                        dojoNorthSubKey.DeleteSubKeyTree("Taryn BitLocker Manager");
                        dojoNorthSubKey.Close();
                    }

                    QMessageBox.Show("All WindowSMART configuration has been deleted from the Registry. The WindowSMART Service " +
                        "will now be restarted and default configuration restored. There will be a brief delay to allow the service time to " +
                        "start and initialize the default configuration, after which the Settings dialogue will close automatically.\n\n" +
                        "Please exit out of and restart WindowSMART after the Settings dialogue closes.", "Reset Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    try
                    {
                        controller.Start();
                    }
                    catch (Exception ex)
                    {
                        QMessageBox.Show("Unable to start WindowSMART Service. " + ex.Message, "Severe", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    timer = 0;
                    controller.Refresh();
                    while (timer < 15 && controller.Status != ServiceControllerStatus.Running)
                    {
                        System.Threading.Thread.Sleep(1000);
                        controller.Refresh();
                        timer++;
                    }

                    // Sleep for 5 seconds to allow configuration to be created.
                    System.Threading.Thread.Sleep(5000);

                    dojoNorthSubKey = registryHklm.OpenSubKey("SOFTWARE\\Dojo North Software\\HomeServerSMART", true);
                    configurationKey = dojoNorthSubKey.OpenSubKey("Configuration", true);
                    isRegistryAvailable = true;

                    // Restore the helper service state!
                    configurationKey.SetValue(Properties.Resources.RegistryConfigUseHelperService, useHelperService);
                    configurationKey.SetValue(Properties.Resources.RegistryConfigDiskPoolProvider, helperServiceDiskPoolProvider);

                    PopulateData();
                    this.Close();
                }
                catch (Exception ex)
                {
                    QMessageBox.Show(ex.Message, "Severe");
                }
            }
        }

        private void checkBoxEnableReboot_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEnableReboot.Checked)
            {
                buttonReboot.Enabled = true;
                buttonShutDown.Enabled = true;
                checkBoxForce.Enabled = true;
            }
            else
            {
                buttonReboot.Enabled = false;
                buttonShutDown.Enabled = false;
                checkBoxForce.Enabled = false;
            }
        }

        private void buttonReboot_Click(object sender, EventArgs e)
        {
            if (QMessageBox.Show("Rebooting the Server will cause a service interruption to all users and devices currently accessing the Server. " +
                "If a backup or restore is currently in progress, it will be aborted.\n\nDo you still want to reboot the Server?", "Reboot Server",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }

            try
            {
                DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Reboot.RebootServer.Reboot(
                    checkBoxForce.Checked);
            }
            catch (Exception ex)
            {
                EpicFail fail = new EpicFail(ex, "Please try the operation again. If the problem persists, please tender a bug report.", isWindowsServerSolutions);
                fail.Show();
            }
        }

        private void buttonShutDown_Click(object sender, EventArgs e)
        {
            if (QMessageBox.Show("Shutting down the Server will cause a service interruption to all users and devices currently accessing the Server. " +
                "If a backup or restore is currently in progress, it will be aborted.\n\nIf you do not have physical access to your Server, you " +
                "will not be able to restart it remotely and will need someone else to power it on for you.\n\nDo you still want to " +
                "shut down the Server?", "Shut Down Server",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }

            try
            {
                DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Reboot.RebootServer.ShutDown(
                    checkBoxForce.Checked);
            }
            catch (Exception ex)
            {
                EpicFail fail = new EpicFail(ex, "Please try the operation again. If the problem persists, please tender a bug report.", isWindowsServerSolutions);
                fail.Show();
            }
        }

        private void buttonStartService_Click(object sender, EventArgs e)
        {
            if (RefreshServiceStateEx() && (controller.Status == ServiceControllerStatus.Stopped || controller.Status == ServiceControllerStatus.Paused))
            {
                try
                {
                    buttonStartService.Enabled = false;
                    Thread threadStarter = new Thread(new ParameterizedThreadStart(StartService));
                    threadStarter.Name = "Service Starter";
                    if (controller.Status == ServiceControllerStatus.Stopped)
                    {
                        // Start
                        threadStarter.Start(false);
                    }
                    else
                    {
                        // Resume
                        threadStarter.Start(true);
                    }
                }
                catch (Exception ex)
                {
                    EpicFail fail = new EpicFail(ex, "Server is unable to allocate thread resources. The Server may be out of memory. Close unnecessary applications " +
                        "and/or stop unnecessary services and try the operation again.", isWindowsServerSolutions);
                    fail.Show();
                }
            }
        }

        private void buttonStopService_Click(object sender, EventArgs e)
        {
            if (RefreshServiceStateEx() && (controller.Status == ServiceControllerStatus.Running) &&
                (QMessageBox.Show("Stopping the WindowSMART Service will cause all SMART disk monitoring, alerting and reporting " +
                "to stop until the service is restarted. Do you really want to stop the WindowSMART Service?", "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes))
            {
                try
                {
                    buttonStopService.Enabled = false;
                    buttonRestartService.Enabled = false;
                    Thread threadStopper = new Thread(new ThreadStart(StopService));
                    threadStopper.Name = "Service Stopper";
                    threadStopper.Start();
                }
                catch (Exception ex)
                {
                    EpicFail fail = new EpicFail(ex, "Server is unable to allocate thread resources. The Server may be out of memory. Close unnecessary applications " +
                        "and/or stop unnecessary services and try the operation again.", isWindowsServerSolutions);
                    fail.Show();
                }
            }
        }

        private void buttonRestartService_Click(object sender, EventArgs e)
        {
            if (RefreshServiceStateEx() && (controller.Status == ServiceControllerStatus.Running) &&
                (QMessageBox.Show("Restarting the WindowSMART Service will cause all SMART disk monitoring, alerting and reporting " +
                "to be briefly interrupted. Continue restarting the WindowSMART Service?", "Restart Service",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
            {
                CallServiceRebooter();
            }
        }

        private void CallServiceRebooter()
        {
            try
            {
                buttonStopService.Enabled = false;
                buttonRestartService.Enabled = false;
                Thread threadStopper = new Thread(new ThreadStart(RestartService));
                threadStopper.Name = "Service Rebooter";
                threadStopper.Start();
            }
            catch (Exception ex)
            {
                EpicFail fail = new EpicFail(ex, "Server is unable to allocate thread resources. The Server may be out of memory. Close unnecessary applications " +
                    "and/or stop unnecessary services and try the operation again.", isWindowsServerSolutions);
                fail.Show();
            }
        }

        private void RefreshServiceState()
        {
            this.Invoke(stateRefresher);
        }

        private void DRefreshServiceState()
        {
            RefreshServiceStateEx();
        }

        private bool RefreshServiceStateEx()
        {
            try
            {
                if (controller != null)
                {
                    controller.Refresh();
                    if (controller.Status == ServiceControllerStatus.Running)
                    {
                        pictureBoxServiceStatus.Image = Properties.Resources.Healthy16;
                        labelServiceStatus.Text = "WindowSMART service is running.";
                        buttonStartService.Enabled = false;
                        buttonStopService.Enabled = (gpoAllowServiceControl != 0);
                        buttonRestartService.Enabled = (gpoAllowServiceControl != 0);

                        if (gpoAllowServiceControl < 2 || !allowShutdown)
                        {
                            buttonReboot.Enabled = false;
                            buttonShutDown.Enabled = false;
                            checkBoxForce.Enabled = false;
                            checkBoxEnableReboot.Enabled = false;
                        }
                        return true;
                    }
                    else if (controller.Status == ServiceControllerStatus.Stopped)
                    {
                        pictureBoxServiceStatus.Image = Properties.Resources.Warning16;
                        labelServiceStatus.Text = "WindowSMART service is stopped.";
                        buttonStartService.Enabled = (gpoAllowServiceControl != 0);
                        buttonStopService.Enabled = false;
                        buttonRestartService.Enabled = false;

                        if (gpoAllowServiceControl < 2 || !allowShutdown)
                        {
                            buttonReboot.Enabled = false;
                            buttonShutDown.Enabled = false;
                            checkBoxForce.Enabled = false;
                            checkBoxEnableReboot.Enabled = false;
                        }
                        return true;
                    }
                    else
                    {
                        buttonStartService.Enabled = false;
                        buttonStopService.Enabled = false;
                        buttonRestartService.Enabled = false;
                        switch (controller.Status)
                        {
                            case ServiceControllerStatus.StartPending:
                                {
                                    pictureBoxServiceStatus.Image = Properties.Resources.Refresh;
                                    labelServiceStatus.Text = "WindowSMART service is starting.";
                                    break;
                                }
                            case ServiceControllerStatus.StopPending:
                                {
                                    pictureBoxServiceStatus.Image = Properties.Resources.Warning16;
                                    labelServiceStatus.Text = "WindowSMART service is stopping.";
                                    break;
                                }
                            case ServiceControllerStatus.Paused:
                                {
                                    pictureBoxServiceStatus.Image = Properties.Resources.Warning16;
                                    labelServiceStatus.Text = "WindowSMART service has been paused; click Start to resume it.";
                                    buttonStartService.Enabled = true;
                                    break;
                                }
                            case ServiceControllerStatus.PausePending:
                                {
                                    pictureBoxServiceStatus.Image = Properties.Resources.Warning16;
                                    labelServiceStatus.Text = "WindowSMART service is pausing.";
                                    break;
                                }
                            case ServiceControllerStatus.ContinuePending:
                                {
                                    pictureBoxServiceStatus.Image = Properties.Resources.Refresh;
                                    labelServiceStatus.Text = "WindowSMART service is resuming from pause.";
                                    break;
                                }
                            default:
                                {
                                    pictureBoxServiceStatus.Image = Properties.Resources.Critical16;
                                    labelServiceStatus.Text = "WindowSMART service has failed. Please reinstall the add-in.";
                                    break;
                                }
                        }
                        if (gpoAllowServiceControl < 2 || !allowShutdown)
                        {
                            buttonReboot.Enabled = false;
                            buttonShutDown.Enabled = false;
                            checkBoxForce.Enabled = false;
                            checkBoxEnableReboot.Enabled = false;
                        }
                        return true;
                    }
                }
                else
                {
                    pictureBoxServiceStatus.Image = Properties.Resources.Critical16;
                    labelServiceStatus.Text = "WindowSMART is not installed correctly. Please reinstall the add-in.";
                    buttonStartService.Enabled = false;
                    buttonStopService.Enabled = false;
                    buttonRestartService.Enabled = false;

                    if (gpoAllowServiceControl < 2)
                    {
                        if (gpoAllowServiceControl == 0 || !allowShutdown)
                        {
                            buttonReboot.Enabled = false;
                            buttonShutDown.Enabled = false;
                            checkBoxForce.Enabled = false;
                            checkBoxEnableReboot.Enabled = false;
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                pictureBoxServiceStatus.Image = Properties.Resources.Critical16;
                labelServiceStatus.Text = ex.Message;
                buttonStartService.Enabled = false;
                buttonStopService.Enabled = false;
                buttonRestartService.Enabled = false;
                if (gpoAllowServiceControl < 2)
                {
                    if (gpoAllowServiceControl == 0 || !allowShutdown)
                    {
                        buttonReboot.Enabled = false;
                        buttonShutDown.Enabled = false;
                        checkBoxForce.Enabled = false;
                        checkBoxEnableReboot.Enabled = false;
                    }
                }
                return false;
            }
        }

        private void StartService(object resumeFromPause)
        {
            bool resume = (bool)resumeFromPause;
            int timer = 0;
            isUiBlocking = true;

            if (resume)
            {
                UpdateServiceState(Properties.Resources.Refresh, "WindowSMART service is resuming from pause.");
            }
            else
            {
                UpdateServiceState(Properties.Resources.Refresh, "WindowSMART service is starting.");
            }
            Thread.Sleep(500);

            try
            {
                if (resume)
                {
                    controller.Continue();
                }
                else
                {
                    controller.Start();
                }
            }
            catch (Exception ex)
            {
                UpdateServiceState(Properties.Resources.Critical16, "WindowSMART service encountered an error.");
                QMessageBox.Show("Unable to " + (resume ? "resume" : "start") + " WindowSMART Service. " + ex.Message, "Severe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                controller.Refresh();
                isUiBlocking = false;
                return;
            }

            controller.Refresh();
            while (timer < 15 && controller.Status != ServiceControllerStatus.Running)
            {
                System.Threading.Thread.Sleep(1000);
                controller.Refresh();
                timer++;
            }

            if (controller.Status == ServiceControllerStatus.Running)
            {
                UpdateServiceState(Properties.Resources.Healthy16, "WindowSMART service is running.");
            }
            else
            {
                UpdateServiceState(Properties.Resources.Warning16, "WindowSMART service hung on starting.");
                QMessageBox.Show("Service did not start in a reasonable amount of time.", "Start Service", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            RefreshServiceState();
            isUiBlocking = false;
        }

        private void StopService()
        {
            isUiBlocking = true;
            int timer = 0;
            UpdateServiceState(Properties.Resources.Warning16, "WindowSMART service is stopping.");
            Thread.Sleep(500);

            try
            {
                controller.Stop();
            }
            catch (Exception ex)
            {
                UpdateServiceState(Properties.Resources.Critical16, "WindowSMART service encountered an error.");
                QMessageBox.Show("Unable to stop WindowSMART Service. " + ex.Message, "Severe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                controller.Refresh();
                isUiBlocking = false;
                return;
            }

            controller.Refresh();
            while (timer < 15 && controller.Status != ServiceControllerStatus.Stopped)
            {
                System.Threading.Thread.Sleep(1000);
                controller.Refresh();
                timer++;
            }

            if (controller.Status == ServiceControllerStatus.Stopped)
            {
                UpdateServiceState(Properties.Resources.Warning16, "WindowSMART service is stopped.");
            }
            else
            {
                UpdateServiceState(Properties.Resources.Warning16, "WindowSMART service hung on shutdown.");
                QMessageBox.Show("Service did not stop in a reasonable amount of time.", "Stop Service", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            isUiBlocking = false;
            RefreshServiceState();
        }

        private void RestartService()
        {
            isUiBlocking = true;
            int timer = 0;
            UpdateServiceState(Properties.Resources.Refresh, "WindowSMART service is stopping.");
            Thread.Sleep(500);

            try
            {
                controller.Stop();
            }
            catch (Exception ex)
            {
                UpdateServiceState(Properties.Resources.Critical16, "WindowSMART service encountered an error.");
                QMessageBox.Show("Unable to stop WindowSMART Service. " + ex.Message, "Severe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                controller.Refresh();
                isUiBlocking = false;
                return;
            }

            controller.Refresh();
            while (timer < 15 && controller.Status != ServiceControllerStatus.Stopped)
            {
                System.Threading.Thread.Sleep(1000);
                controller.Refresh();
                timer++;
            }

            if (controller.Status == ServiceControllerStatus.Stopped)
            {
                UpdateServiceState(Properties.Resources.Refresh, "WindowSMART service is stopped.");
                Thread.Sleep(500);
            }
            else
            {
                UpdateServiceState(Properties.Resources.Warning16, "WindowSMART service hung on shutdown.");
                QMessageBox.Show("Service did not stop in a reasonable amount of time.", "Start Service", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            controller.Refresh();

            if (controller.Status == ServiceControllerStatus.Stopped)
            {
                timer = 0;
                UpdateServiceState(Properties.Resources.Refresh, "WindowSMART service is starting.");

                try
                {
                    controller.Start();
                }
                catch (Exception ex)
                {
                    UpdateServiceState(Properties.Resources.Critical16, "WindowSMART service encountered an error.");
                    QMessageBox.Show("Unable to start WindowSMART Service. " + ex.Message, "Severe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    controller.Refresh();
                    isUiBlocking = false;
                    return;
                }

                controller.Refresh();
                while (timer < 15 && controller.Status != ServiceControllerStatus.Running)
                {
                    System.Threading.Thread.Sleep(1000);
                    controller.Refresh();
                    timer++;
                }

                if (controller.Status == ServiceControllerStatus.Running)
                {
                    UpdateServiceState(Properties.Resources.Healthy16, "WindowSMART service is running.");
                }
                else
                {
                    UpdateServiceState(Properties.Resources.Warning16, "WindowSMART service hung on starting.");
                    QMessageBox.Show("Service did not start in a reasonable amount of time.", "Start Service", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                isUiBlocking = false;
                RefreshServiceState();
            }
            else
            {
                isUiBlocking = false;
                RefreshServiceState();
                return;
            }
        }

        private void UpdateServiceState(Image serviceImage, String message)
        {
            this.Invoke(serviceStatusUpdater, new object[] { serviceImage, message });
        }

        private void DUpdateServiceState(Image serviceImage, String message)
        {
            pictureBoxServiceStatus.Image = serviceImage;
            labelServiceStatus.Text = message;
        }

        private void RefreshIgnoredItems()
        {
            // Clear the FancyListViews.
            listViewIgnoredProblems.Items.Clear();
            listViewIgnoredDisks.Items.Clear();

            // Then look for items.
            try
            {
                Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey subkey;

                subkey = hklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                Microsoft.Win32.RegistryKey monitoredDisksKey = subkey.OpenSubKey(Properties.Resources.RegistryMonitoredDisksKey, false);

                WSSControls.BelovedComponents.FancyListView.ImageSubItem ignoredItem = new WSSControls.BelovedComponents.FancyListView.ImageSubItem();
                ignoredItem.Text = String.Empty;
                ignoredItem.ImageKey = "Ignore";

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey tempKey = monitoredDisksKey.OpenSubKey(diskKeyName, false);
                    bool activeFlag = (bool.Parse((String)tempKey.GetValue("IsActive")));
                    if (!activeFlag)
                    {
                        // Inactive/Stale disk.
                        continue;
                    }

                    String modelName = (String)tempKey.GetValue("Model");
                    String path = (String)tempKey.GetValue("DevicePath");

                    long itemsIgnored = 0;

                    try
                    {
                        // IGNORED ITEMS
                        // Bad Sectors
                        itemsIgnored = long.Parse((String)tempKey.GetValue("BadSectorsIgnored"));
                        if (itemsIgnored > 0)
                        {
                            ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                            lvi.SubItems.Add(ignoredItem);
                            lvi.SubItems.Add(modelName);
                            lvi.SubItems.Add("Reallocated Sectors Count");
                            lvi.SubItems.Add(itemsIgnored.ToString());
                            listViewIgnoredProblems.Items.Add(lvi);
                        }
                        itemsIgnored = 0;

                        // CRC Errors
                        itemsIgnored = long.Parse((String)tempKey.GetValue("CrcErrorsIgnored"));
                        if (itemsIgnored > 0)
                        {
                            ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                            lvi.SubItems.Add(ignoredItem);
                            lvi.SubItems.Add(modelName);
                            lvi.SubItems.Add("Ultra DMA CRC Error Count");
                            lvi.SubItems.Add(itemsIgnored.ToString());
                            listViewIgnoredProblems.Items.Add(lvi);
                        }
                        itemsIgnored = 0;

                        // End-to-End Errors
                        itemsIgnored = long.Parse((String)tempKey.GetValue("EndToEndErrorsIgnored"));
                        if (itemsIgnored > 0)
                        {
                            ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                            lvi.SubItems.Add(ignoredItem);
                            lvi.SubItems.Add(modelName);
                            lvi.SubItems.Add("End-to-End Error");
                            lvi.SubItems.Add(itemsIgnored.ToString());
                            listViewIgnoredProblems.Items.Add(lvi);
                        }
                        itemsIgnored = 0;

                        // Offline Bad Sectors
                        itemsIgnored = long.Parse((String)tempKey.GetValue("OfflineBadSectorsIgnored"));
                        if (itemsIgnored > 0)
                        {
                            ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                            lvi.SubItems.Add(ignoredItem);
                            lvi.SubItems.Add(modelName);
                            lvi.SubItems.Add("Offline Uncorrectable Sector Count");
                            lvi.SubItems.Add(itemsIgnored.ToString());
                            listViewIgnoredProblems.Items.Add(lvi);
                        }
                        itemsIgnored = 0;

                        // Pending Bad Sectors
                        itemsIgnored = long.Parse((String)tempKey.GetValue("PendingSectorsIgnored"));
                        if (itemsIgnored > 0)
                        {
                            ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                            lvi.SubItems.Add(ignoredItem);
                            lvi.SubItems.Add(modelName);
                            lvi.SubItems.Add("Current Pending Sector Count");
                            lvi.SubItems.Add(itemsIgnored.ToString());
                            listViewIgnoredProblems.Items.Add(lvi);
                        }
                        itemsIgnored = 0;

                        // Reallocation Events
                        itemsIgnored = long.Parse((String)tempKey.GetValue("ReallocationEventsIgnored"));
                        if (itemsIgnored > 0)
                        {
                            ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                            lvi.SubItems.Add(ignoredItem);
                            lvi.SubItems.Add(modelName);
                            lvi.SubItems.Add("Reallocation Event Count");
                            lvi.SubItems.Add(itemsIgnored.ToString());
                            listViewIgnoredProblems.Items.Add(lvi);
                        }
                        itemsIgnored = 0;

                        // Spin Retries
                        itemsIgnored = long.Parse((String)tempKey.GetValue("SpinRetriesIgnored"));
                        if (itemsIgnored > 0)
                        {
                            ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                            lvi.SubItems.Add(ignoredItem);
                            lvi.SubItems.Add(modelName);
                            lvi.SubItems.Add("Spin Retry Count");
                            lvi.SubItems.Add(itemsIgnored.ToString());
                            listViewIgnoredProblems.Items.Add(lvi);
                        }
                        itemsIgnored = 0;
                    }
                    catch
                    {

                    }

                    try
                    {
                        // IGNORED DISKS
                        String dateIgnored = (String)tempKey.GetValue("DateIgnored", String.Empty);
                        DateTime result;
                        if (DateTime.TryParse(dateIgnored, out result))
                        {
                            // Ignored disk.
                            ListViewItem lvi = new ListViewItem(new String[] { path });
                            lvi.SubItems.Add(ignoredItem);
                            lvi.SubItems.Add(modelName);
                            lvi.SubItems.Add(result.ToShortDateString());
                            listViewIgnoredDisks.Items.Add(lvi);
                        }
                    }
                    catch
                    {

                    }
                }
                monitoredDisksKey.Close();
                subkey.Close();
                hklm.Close();
            }
            catch (Exception)
            {
            }
        }

        private void buttonBugReport_Click(object sender, EventArgs e)
        {
            UploadDebugDiagnostics();
        }

        private void UploadDebugDiagnostics()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.UploadDebugDiagnostics");
            bool denyDiagnostics = false;
            try
            {
                SiAuto.Main.LogMessage("Checking to ensure policy allows debugging.");
                bool parseResult = bool.TryParse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoDenyDebug), out denyDiagnostics);
                SiAuto.Main.LogBool("parseResult", parseResult);
                SiAuto.Main.LogBool("denyDiagnostics", denyDiagnostics);
            }
            catch
            {
                denyDiagnostics = false;
            }

            try
            {
                int diagnosticValue = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoUseDiagnostics);
                SiAuto.Main.LogInt("diagnosticValue", diagnosticValue);
                if (diagnosticValue == 0)
                {
                    denyDiagnostics = true;
                }
            }
            catch
            {
                SiAuto.Main.LogInt("diagnosticValue", 2);
            }

            if (denyDiagnostics)
            {
                QMessageBox.Show("The ability to generate or send a diagnostic debugging report has been blocked by your system administrator.", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.UploadDebugDiagnostics");
                return;
            }

            if (!IsDebuggingAvailable())
            {
                DialogResult result = DialogResult.None;
                if (isWindowsServerSolutions)
                {
                    result = QMessageBox.Show("There are no debug logs available. While you may still submit a report, we recommend that you go into Settings " +
                        "and enable Debug Logging. Please go into Settings and select the Advanced tab, and select the option \"Enable SmartInspect logging.\" " +
                        "Click Apply, and then restart the WindowSMART service by going to the Service Control tab and clicking Restart Service. You should also " +
                        "restart the WindowSMART client. The service and client restart fully activates the debug log generation. Let the system run for a little " +
                        "while, and then retry your report submission.\n\nDebug logs are NOT required to submit a bug or diagnostic report. However, they greatly " +
                        "improve the quality of the report and will likely result in a much faster response because we can troubleshoot the problem better when " +
                        "debug logs are available.\n\nDo you still want to create a report without debug logs?", "No Debug Data", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button2);
                }
                else
                {
                    result = QMessageBox.Show("There are no debug logs available. While you may still submit a report, we recommend that you go into Settings " +
                        "and enable Debug Logging. Please go into Settings and select the Advanced tab, and select the option \"Enable SmartInspect logging.\" " +
                        "Click Apply, and then restart the Home Server SMART service by going to the Service Control tab and clicking Restart Service. You should also " +
                        "restart the server Dashboard. The service and Dashboard restart fully activates the debug log generation. Let the system run for a little " +
                        "while, and then retry your report submission.\n\nDebug logs are NOT required to submit a bug or diagnostic report. However, they greatly " +
                        "improve the quality of the report and will likely result in a much faster response because we can troubleshoot the problem better when " +
                        "debug logs are available.\n\nDo you still want to create a report without debug logs?", "No Debug Data", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button2);
                }

                if (result == DialogResult.No)
                {
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.UploadDebugDiagnostics");
                    return;
                }
            }
            GenerateDiags diags = new GenerateDiags(isWindowsServerSolutions, Utilities.Utility.IsSystemWindows8());
            diags.ShowDialog();
            DeveloperDiagnosticDebuggingReport report = new DeveloperDiagnosticDebuggingReport(debugLogLocation, diags.DiagnosticDetails, mexiSexi.IsMexiSexi);
            report.ShowDialog();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.UploadDebugDiagnostics");
        }

        private void buttonResumeProblem_Click(object sender, EventArgs e)
        {
            if (listViewIgnoredProblems.SelectedItems != null && listViewIgnoredProblems.SelectedItems.Count != 0)
            {
                // Get info about the disk from the FancyListView.
                // Columns 0-1 are the blank and image, respectively. Colum 2 is disk, 3 is problem and 4 is count.
                ListViewItem lvi = listViewIgnoredProblems.SelectedItems[0];
                String diskName = lvi.SubItems[2].Text;
                String problemName = lvi.SubItems[3].Text;
                long problemCount = 0;
                bool updateSuccess = false;

                try
                {
                    problemCount = Int32.Parse(lvi.SubItems[4].Text);
                }
                catch (Exception ex)
                {
                    EpicFail fail = new EpicFail(ex, "Memory reserved to control listViewIgnoredProblems is corrupted. Please restart WindowSMART. " +
                        "If the problem persists, please reboot the Server. If the problem remains after a reboot, please submit a bug report.", isWindowsServerSolutions);
                    fail.ShowDialog();
                    return;
                }

                Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey subkey;

                subkey = hklm.CreateSubKey("SOFTWARE\\Dojo North Software\\HomeServerSMART");

                Microsoft.Win32.RegistryKey monitoredDisksKey = subkey.OpenSubKey("MonitoredDisks", true);

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey tempKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);

                    String model = (String)tempKey.GetValue("Model");
                    if (String.Compare(diskName, model, true) == 0)
                    {
                        long savedProblemCount = 0;
                        bool isCorrupt = false;

                        switch (problemName)
                        {
                            case "Reallocated Sectors Count":
                                {
                                    try
                                    {
                                        savedProblemCount = long.Parse((String)tempKey.GetValue("BadSectorsIgnored"));
                                    }
                                    catch
                                    {
                                        isCorrupt = true;
                                    }

                                    if (problemCount == savedProblemCount || isCorrupt)
                                    {
                                        try
                                        {
                                            tempKey.SetValue("BadSectorsIgnored", (long)0);
                                            updateSuccess = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            QMessageBox.Show("Failed to reset Registry value. " + ex.Message, "Warning",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        }
                                    }
                                    break;
                                }
                            case "Ultra DMA CRC Error Count":
                                {
                                    try
                                    {
                                        savedProblemCount = long.Parse((String)tempKey.GetValue("CrcErrorsIgnored"));
                                    }
                                    catch
                                    {
                                        isCorrupt = true;
                                    }

                                    if (problemCount == savedProblemCount || isCorrupt)
                                    {
                                        try
                                        {
                                            tempKey.SetValue("CrcErrorsIgnored", (long)0);
                                            updateSuccess = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            QMessageBox.Show("Failed to reset Registry value. " + ex.Message, "Warning",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        }
                                    }
                                    break;
                                }
                            case "End-to-End Error Count":
                                {
                                    try
                                    {
                                        savedProblemCount = long.Parse((String)tempKey.GetValue("EndToEndErrorsIgnored"));
                                    }
                                    catch
                                    {
                                        isCorrupt = true;
                                    }

                                    if (problemCount == savedProblemCount || isCorrupt)
                                    {
                                        try
                                        {
                                            tempKey.SetValue("EndToEndErrorsIgnored", (long)0);
                                            updateSuccess = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            QMessageBox.Show("Failed to reset Registry value. " + ex.Message, "Warning",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        }
                                    }
                                    break;
                                }
                            case "Offline Uncorrectable Sector Count":
                                {
                                    try
                                    {
                                        savedProblemCount = long.Parse((String)tempKey.GetValue("OfflineBadSectorsIgnored"));
                                    }
                                    catch
                                    {
                                        isCorrupt = true;
                                    }

                                    if (problemCount == savedProblemCount || isCorrupt)
                                    {
                                        try
                                        {
                                            tempKey.SetValue("OfflineBadSectorsIgnored", (long)0);
                                            updateSuccess = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            QMessageBox.Show("Failed to reset Registry value. " + ex.Message, "Warning",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        }
                                    }
                                    break;
                                }
                            case "Current Pending Sector Count":
                                {
                                    try
                                    {
                                        savedProblemCount = long.Parse((String)tempKey.GetValue("PendingSectorsIgnored"));
                                    }
                                    catch
                                    {
                                        isCorrupt = true;
                                    }

                                    if (problemCount == savedProblemCount || isCorrupt)
                                    {
                                        try
                                        {
                                            tempKey.SetValue("PendingSectorsIgnored", (long)0);
                                            updateSuccess = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            QMessageBox.Show("Failed to reset Registry value. " + ex.Message, "Warning",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        }
                                    }
                                    break;
                                }
                            case "Reallocation Event Count":
                                {
                                    try
                                    {
                                        savedProblemCount = long.Parse((String)tempKey.GetValue("ReallocationEventsIgnored"));
                                    }
                                    catch
                                    {
                                        isCorrupt = true;
                                    }

                                    if (problemCount == savedProblemCount || isCorrupt)
                                    {
                                        try
                                        {
                                            tempKey.SetValue("ReallocationEventsIgnored", (long)0);
                                            updateSuccess = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            QMessageBox.Show("Failed to reset Registry value. " + ex.Message, "Warning",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        }
                                    }
                                    break;
                                }
                            case "Spin Retry Count":
                                {
                                    try
                                    {
                                        savedProblemCount = long.Parse((String)tempKey.GetValue("SpinRetriesIgnored"));
                                    }
                                    catch
                                    {
                                        isCorrupt = true;
                                    }

                                    if (problemCount == savedProblemCount || isCorrupt)
                                    {
                                        try
                                        {
                                            tempKey.SetValue("SpinRetriesIgnored", (long)0);
                                            updateSuccess = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            QMessageBox.Show("Failed to reset Registry value. " + ex.Message, "Warning",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                }
                monitoredDisksKey.Close();
                subkey.Close();
                hklm.Close();

                if (updateSuccess)
                {
                    FancyListView.ImageSubItem subItem = new FancyListView.ImageSubItem();
                    subItem.Text = String.Empty;
                    subItem.ImageKey = "Resume";
                    lvi.SubItems[1] = subItem;
                }
                else
                {
                    QMessageBox.Show("Exceptions were detected, or no changes required.", "Resume Monitoring");
                }
            }
        }

        private void buttonResumeAllProblems_Click(object sender, EventArgs e)
        {
            if (QMessageBox.Show("Resetting ignored problems will reset all ignored problem counters to zero for " +
                "each disk. Problems that are currently not being reported due to being ignored will be reported again. " +
                "Do you want to reset all ignored problems?", "Reset Ignored Problems", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                try
                {
                    Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                    Microsoft.Win32.RegistryKey subkey;

                    subkey = hklm.CreateSubKey("SOFTWARE\\Dojo North Software\\HomeServerSMART");

                    Microsoft.Win32.RegistryKey monitoredDisksKey = subkey.OpenSubKey("MonitoredDisks", true);

                    int disksUpdated = 0;

                    foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                    {
                        Microsoft.Win32.RegistryKey tempKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);
                        tempKey.SetValue("BadSectorsIgnored", (long)0);
                        tempKey.SetValue("EndToEndErrorsIgnored", (long)0);
                        tempKey.SetValue("PendingSectorsIgnored", (long)0);
                        tempKey.SetValue("ReallocationEventsIgnored", (long)0);
                        tempKey.SetValue("SpinRetriesIgnored", (long)0);
                        tempKey.SetValue("OfflineBadSectorsIgnored", (long)0);
                        tempKey.SetValue("CrcErrorsIgnored", (long)0);
                        disksUpdated++;
                    }
                    monitoredDisksKey.Close();
                    subkey.Close();
                    hklm.Close();
                    listViewIgnoredProblems.Items.Clear();
                    QMessageBox.Show("Successfully reset " + disksUpdated.ToString() + "  disks.", "Reset Ignored Problems",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    QMessageBox.Show("Resetting of problems failed. " + ex.Message, "Severe",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void buttonResumeDisk_Click(object sender, EventArgs e)
        {
            if (listViewIgnoredDisks.SelectedItems != null && listViewIgnoredDisks.SelectedItems.Count != 0)
            {
                // Get info about the disk from the FancyListView.
                // Columns 0-1 are the blank and image, respectively. Colum 2 is disk, 3 is problem and 4 is count.
                ListViewItem lvi = listViewIgnoredDisks.SelectedItems[0];
                String diskName = lvi.SubItems[2].Text;
                String diskPath = lvi.SubItems[0].Text;
                String dateIgnored = lvi.SubItems[3].Text;
                bool updateSuccess = false;

                Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey subkey;

                subkey = hklm.CreateSubKey("SOFTWARE\\Dojo North Software\\HomeServerSMART");

                Microsoft.Win32.RegistryKey monitoredDisksKey = subkey.OpenSubKey("MonitoredDisks", true);

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey tempKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);

                    String model = (String)tempKey.GetValue("Model");
                    String path = (String)tempKey.GetValue("DevicePath");
                    String date = (String)tempKey.GetValue("DateIgnored");

                    if (String.Compare(diskName, model, true) == 0 &&
                        String.Compare(diskPath, diskPath, true) == 0 &&
                        String.Compare(dateIgnored, date, true) == 0)
                    {
                        tempKey.SetValue("DateIgnored", String.Empty);
                        updateSuccess = true;
                    }
                }
                monitoredDisksKey.Close();
                subkey.Close();
                hklm.Close();

                if (updateSuccess)
                {
                    FancyListView.ImageSubItem subItem = new FancyListView.ImageSubItem();
                    subItem.Text = String.Empty;
                    subItem.ImageKey = "Resume";
                    lvi.SubItems[1] = subItem;
                    //QMessageBox.Show("Successful update.", "Danke!"); ~ Jake ~
                }
                else
                {
                    QMessageBox.Show("Exceptions were detected, or no changes required.", "Resume Monitoring");
                }
            }
        }

        private void buttonResumeAllDisks_Click(object sender, EventArgs e)
        {
            if (QMessageBox.Show("Resetting ignored disks will resume monitoring on all ignored disks. " +
                "Problems that are currently not being reported due to being on an ignored disk will be reported again. " +
                "Do you want to reset all ignored disks?", "Reset Ignored Disks", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                try
                {
                    Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                    Microsoft.Win32.RegistryKey subkey;

                    subkey = hklm.CreateSubKey("SOFTWARE\\Dojo North Software\\HomeServerSMART");

                    Microsoft.Win32.RegistryKey monitoredDisksKey = subkey.OpenSubKey("MonitoredDisks", true);

                    int disksUpdated = 0;

                    foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                    {
                        Microsoft.Win32.RegistryKey tempKey = monitoredDisksKey.OpenSubKey(diskKeyName, true);
                        tempKey.SetValue("DateIgnored", String.Empty);
                        disksUpdated++;
                    }
                    monitoredDisksKey.Close();
                    subkey.Close();
                    hklm.Close();
                    listViewIgnoredDisks.Items.Clear();
                    QMessageBox.Show("Successfully reset " + disksUpdated.ToString() + "  disks.", "Reset Ignored Problems",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    QMessageBox.Show("Resetting of disks failed. " + ex.Message, "Severe",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        #region Debug Options
        private void checkBoxFallbackWmi_CheckedChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void checkBoxDebugMode_CheckedChanged(object sender, EventArgs e)
        {
            EnableButtons();
            if (loadInProgress)
            {
                return;
            }

            debugMode = checkBoxDebugMode.Checked;

            if (!isDebugChangeMessageShown)
            {
                isDebugChangeMessageShown = true;
                QMessageBox.Show("Changing Debug Logging requires a service restart for the changes to take effect. Make sure you " +
                    "save your changes prior to restarting the service.", "Service Restart Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void checkBoxAdvancedSii_CheckedChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }
        #endregion

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            try
            {
                HssUpdate.HssUpdate client = new HssUpdate.HssUpdate();
                String installedVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                String latestVersion = client.ReapHs2Version();
                labelLatestVersion.Text = latestVersion;
                labelReleaseDate.Text = client.ReapHs2ReleaseDate();


                if (installedVersion == latestVersion)
                {
                    QMessageBox.Show("You are running the latest version of WindowSMART 24/7, v" + latestVersion, "No Update Necessary",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    linkLabelDownload.Text = "Download";
                    linkLabelDownload.Tag = client.ReapHs2DownloadLink();
                    linkLabelMoreInfo.Text = "More Info";
                    linkLabelMoreInfo.Tag = client.ReapHs2InfoLink();

                    try
                    {
                        Version installed = new Version(installedVersion);
                        Version latest = new Version(latestVersion);
                        var result = installed.CompareTo(latest);
                        if (result > 0)
                        {
                            labelLatestVersion.Text += " (Version " + installed + " installed)";
                            QMessageBox.Show("You are running a newer version of WindowSMART 24/7 than is currently available. This is normal " +
                                "if you are running a beta or pre-release version of the software.\n\nIf you wish to revert to the current " +
                                "production version, you may use the Download link provided. Once downloaded, you will need to uninstall WindowSMART " +
                                "first, as the installer will not allow you to install an older version on top of a newer version.\n\nIf you have " +
                                "update notifications enabled, you will receive a notification when an updated production version is released.",
                                "Newer Version Installed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                QMessageBox.Show("Unable to get version information: " + ex.Message, "Severe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                buttonCheck.Enabled = true;
            }
        }

        private void linkLabelDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utilities.Utility.LaunchBrowser((String)linkLabelDownload.Tag);
        }

        private void linkLabelMoreInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utilities.Utility.LaunchBrowser((String)linkLabelMoreInfo.Tag);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (isWindowsServerSolutions)
            {
                Utilities.Utility.LaunchBrowser(Properties.Resources.ResourceUrl);
            }
            else
            {
                Utilities.Utility.LaunchBrowser(Properties.Resources.ResourceUrlWs2);
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utilities.Utility.LaunchBrowser(Properties.Resources.ResourceUrlWikipedia);
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utilities.Utility.LaunchBrowser(Properties.Resources.ResourceUrlPcTechGuide);
        }

        private void cbIgnoreVirtual_CheckedChanged(object sender, EventArgs e)
        {
            ignoreVirtualDisks = cbIgnoreVirtual.Checked;
            EnableButtons();
        }

        private void cbBlShowAllVolumes_CheckedChanged(object sender, EventArgs e)
        {
            bitLockerShowAllDrives = cbBlShowAllVolumes.Checked;
            EnableButtons();
        }

        private void cbBlPreventLocking_CheckedChanged(object sender, EventArgs e)
        {
            bitLockerPreventLocking = cbBlPreventLocking.Checked;
            EnableButtons();
        }

        private void cbHideBitLocker_CheckedChanged(object sender, EventArgs e)
        {
            bitLockerHideTab = cbHideBitLocker.Checked;
            EnableButtons();
        }

        private void buttonChangeLogLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (System.IO.Directory.Exists(textBoxDebugLocation.Text))
            {
                fbd.SelectedPath = textBoxDebugLocation.Text;
            }
            else if (System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Dojo North Software\\Logs"))
            {
                fbd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Dojo North Software\\Logs";
            }
            else if (System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)))
            {
                fbd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            }

            fbd.ShowNewFolderButton = true;
            fbd.Description = "Please select the new logging location";
            fbd.ShowDialog();

            if (String.IsNullOrEmpty(fbd.SelectedPath) || String.Compare(fbd.SelectedPath, textBoxDebugLocation.Text, true) == 0)
            {
                return;
            }
            textBoxDebugLocation.Text = fbd.SelectedPath;
            debugLogLocation = textBoxDebugLocation.Text;
            EnableButtons();
            QMessageBox.Show("Be sure to click Save Changes to save the new logging location. Please note that changes to the logging location " +
                "won't take effect until the next time the service is restarted.", "Logging Location Change", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textBoxDebugLocation_TextChanged(object sender, EventArgs e)
        {
            debugLogLocation = textBoxDebugLocation.Text;
            EnableButtons();
        }

        private void upDownSsdLifeLeftCritical_ValueChanged(object sender, EventArgs e)
        {
            if (upDownSsdLifeLeftCritical.Value >= upDownSsdLifeLeftWarning.Value)
            {
                upDownSsdLifeLeftWarning.Value = upDownSsdLifeLeftCritical.Value + 1;
            }
            ssdLifeLeftCritical = (int)upDownSsdLifeLeftCritical.Value;
            EnableButtons();
        }

        private void upDownSsdLifeLeftWarning_ValueChanged(object sender, EventArgs e)
        {
            if (upDownSsdLifeLeftWarning.Value <= upDownSsdLifeLeftCritical.Value)
            {
                upDownSsdLifeLeftCritical.Value = upDownSsdLifeLeftWarning.Value - 1;
            }
            ssdLifeLeftWarning = (int)upDownSsdLifeLeftWarning.Value;
            EnableButtons();
        }

        private void upDownSsdRetirementCritical_ValueChanged(object sender, EventArgs e)
        {
            if (upDownSsdRetirementCritical.Value <= upDownSsdRetirementWarning.Value)
            {
                upDownSsdRetirementWarning.Value = upDownSsdRetirementCritical.Value - 1;
            }
            ssdRetirementCritical = (int)upDownSsdRetirementCritical.Value;
            EnableButtons();
        }

        private void upDownSsdRetirementWarning_ValueChanged(object sender, EventArgs e)
        {
            if (upDownSsdRetirementWarning.Value >= upDownSsdRetirementCritical.Value)
            {
                upDownSsdRetirementCritical.Value = upDownSsdRetirementWarning.Value + 1;
            }
            ssdRetirementWarning = (int)upDownSsdRetirementWarning.Value;
            EnableButtons();
        }

        private void buttonConfigureMail_Click(object sender, EventArgs e)
        {
            ConfigureEmail();
        }

        private void ConfigureEmail()
        {
            EmailConfiguration config = new EmailConfiguration(configurationKey, crypto);
            config.ShowDialog();
            if (config.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                // Re-read Email Configuration from Registry
                // Email Configuration ~ Jake Scherer ~
                try
                {
                    isMailConfigValid = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsEmailConfigValid));
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                    isMailConfigValid = false;
                }

                try
                {
                    mailServer = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailServer);
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigMailServer, mailServer);
                    configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                    isMailConfigValid = false;
                }

                try
                {
                    serverPort = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigServerPort);
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigServerPort, 25);
                    serverPort = 25;
                    configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                    isMailConfigValid = false;
                }

                try
                {
                    senderFriendlyName = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigSenderFriendly);
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigSenderFriendly, senderFriendlyName);
                    configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                    isMailConfigValid = false;
                }

                try
                {
                    senderEmailAddress = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigSenderEmail);
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigSenderEmail, senderEmailAddress);
                    configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                    isMailConfigValid = false;
                }

                try
                {
                    recipientFriendlyName = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientFriendly);
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientFriendly, recipientFriendlyName);
                    configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                    isMailConfigValid = false;
                }

                try
                {
                    recipientEmailAddress = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientEmail);
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail, recipientEmailAddress);
                    configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                    isMailConfigValid = false;
                }

                try
                {
                    recipientEmail2 = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientEmail2);
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail, recipientEmail2);
                    configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                    isMailConfigValid = false;
                }

                try
                {
                    recipientEmail3 = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigRecipientEmail3);
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigRecipientEmail, recipientEmail3);
                    configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                    isMailConfigValid = false;
                }

                try
                {
                    authenticationEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigAuthenticationEnabled));
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigAuthenticationEnabled, authenticationEnabled);
                    configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                    isMailConfigValid = false;
                }

                try
                {
                    mailUser = crypto.Decrypt((String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailUser));
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigMailUser, crypto.Encrypt(mailUser));
                    configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                    isMailConfigValid = false;
                }

                try
                {
                    mailPassword = crypto.Decrypt((String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailPassword));
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigMailPassword, crypto.Encrypt(mailPassword));
                    configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                    isMailConfigValid = false;
                }

                try
                {
                    mailAlertsEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigMailAlertsEnabled));
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigMailAlertsEnabled, mailAlertsEnabled);
                    configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                    isMailConfigValid = false;
                }

                try
                {
                    useSsl = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigUseSSL));
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigUseSSL, useSsl);
                    configurationKey.SetValue(Properties.Resources.RegistryConfigIsEmailConfigValid, false);
                    isMailConfigValid = false;
                }

                try
                {
                    sendDailySummary = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigSendDailySummary));
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigSendDailySummary, sendDailySummary);
                }

                try
                {
                    sendPlaintext = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigSendPlaintext));
                }
                catch
                {
                    configurationKey.SetValue(Properties.Resources.RegistryConfigSendPlaintext, sendPlaintext);
                }
            }
        }

        private void buttonConfigureProwl_Click(object sender, EventArgs e)
        {
            ConfigureProwl();
        }

        private void ConfigureProwl()
        {
            ProwlConfiguration config = new ProwlConfiguration(configurationKey, isWindowsServerSolutions, gpoProwlNotificate, gpoPushoverNotificate);
            config.ShowDialog();
            if (config.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                isProwlEnabled = config.ProwlEnabled;
                prowlApiKey = config.ProwlApiKey;

                isPushoverEnabled = config.PushoverEnabled;
                pushoverUserKey = config.PushoverUserKey;
                pushoverDeviceTarget = config.PushoverDeviceTarget;
                pushoverClearedSound = config.PushoverClearedSound;
                pushoverCriticalSound = config.PushoverCriticalSound;
                pushoverWarningSound = config.PushoverWarningSound;
            }
        }

        private void buttonConfigureNma_Click(object sender, EventArgs e)
        {
            ConfigureNma();
        }

        private void ConfigureNma()
        {
            NmaConfiguration config = new NmaConfiguration(configurationKey, isWindowsServerSolutions, gpoNmaNotificate, gpoPushoverNotificate);
            config.ShowDialog();

            if (config.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                isNmaEnabled = config.NmaEnabled;
                nmaApiKey = config.NmaApiKey;

                isPushoverEnabled = config.PushoverEnabled;
                pushoverUserKey = config.PushoverUserKey;
                pushoverDeviceTarget = config.PushoverDeviceTarget;
                pushoverClearedSound = config.PushoverClearedSound;
                pushoverCriticalSound = config.PushoverCriticalSound;
                pushoverWarningSound = config.PushoverWarningSound;
            }
        }

        private void radioButtonGreen_CheckedChanged(object sender, EventArgs e)
        {
            useDefaultSkinning = true;
            EnableButtons();
        }

        private void radioButtonBlue_CheckedChanged(object sender, EventArgs e)
        {
            useDefaultSkinning = false;
            EnableButtons();
        }

        private void radioButtonMetalGrate_CheckedChanged(object sender, EventArgs e)
        {
            windowBackground = 0;
            SetWindowBackground();
            EnableButtons();
        }

        private void radioButtonCrackedGlass_CheckedChanged(object sender, EventArgs e)
        {
            windowBackground = 2;
            SetWindowBackground();
            EnableButtons();
        }

        private void radioButtonLightning_CheckedChanged(object sender, EventArgs e)
        {
            windowBackground = 1;
            SetWindowBackground();
            EnableButtons();
        }

        private void radioButtonNoTexture_CheckedChanged(object sender, EventArgs e)
        {
            windowBackground = 3;
            SetWindowBackground();
            EnableButtons();
        }

        private void SetWindowBackground()
        {
            if (radioButtonMetalGrate.Checked)
            {
                windowBackground = 0;
            }
            else if (radioButtonLightning.Checked)
            {
                windowBackground = 1;
            }
            else if (radioButtonCrackedGlass.Checked)
            {
                windowBackground = 2;
            }
            else if (radioButtonNoTexture.Checked)
            {
                windowBackground = 3;
            }
            else
            {
                windowBackground = 0;
            }
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isUiBlocking)
            {
                QMessageBox.Show("A background service operation is still in progress. Please wait for it to complete before " +
                    "trying to close the Settings dialogue. Any settings you attempted to save were saved successfully.",
                    "Background Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
            }
        }

        private void buttonConfigureWP7_Click(object sender, EventArgs e)
        {
            ConfigureWp7();
        }

        private void ConfigureWp7()
        {
            WindowsPhoneConfiguration config = new WindowsPhoneConfiguration(configurationKey, isWindowsServerSolutions);
            config.ShowDialog();

            if (config.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                isWindowsPhoneEnabled = config.NmaEnabled;
                wp7Guids = config.NmaApiKey;
            }
        }

        private void buttonConfigureTwitter_Click(object sender, EventArgs e)
        {
            ConfigureTwitter();
        }

        private void ConfigureTwitter()
        {
            TwitterConfiguration tc = new TwitterConfiguration(configurationKey, isWindowsServerSolutions);
            tc.ShowDialog();
        }

        private void buttonConfigureGrowl_Click(object sender, EventArgs e)
        {
            ConfigureGrowl();
        }

        private void ConfigureGrowl()
        {
            GrowlConfiguration gc = new GrowlConfiguration(configurationKey, isWindowsServerSolutions, crypto);
            gc.ShowDialog();
            bool changesDetected = false;

            // Detect changes from local to remote.
            if (gc.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                SiAuto.Main.LogBool("isGrowlEnabled", isGrowlEnabled);
                SiAuto.Main.LogBool("gc.GrowlEnabled", gc.GrowlEnabled);
                SiAuto.Main.LogBool("isGrowlRemoteEnabled", isGrowlRemoteEnabled);
                SiAuto.Main.LogBool("gc.GrowlRemoteEnabled", gc.GrowlRemoteEnabled);
                if ((isGrowlEnabled == gc.GrowlEnabled) && (isGrowlRemoteEnabled != gc.GrowlRemoteEnabled))
                {
                    SiAuto.Main.LogWarning("Growl is enabled and the remote mode has been changed. A service restart is required.");
                    changesDetected = true;
                }
                isGrowlEnabled = gc.GrowlEnabled;
                isGrowlRemoteEnabled = gc.GrowlRemoteEnabled;
                SiAuto.Main.LogInt("growlPort", growlPort);
                SiAuto.Main.LogInt("gc.GrowlPort", gc.GrowlRemotePort);
                if ((isGrowlEnabled == gc.GrowlEnabled) && (growlPort != gc.GrowlRemotePort))
                {
                    SiAuto.Main.LogWarning("Growl is enabled and the remote port has been changed. A service restart is required.");
                    changesDetected = true;
                }
                growlPort = gc.GrowlRemotePort;

                SiAuto.Main.LogString("growlRemoteTarget", growlRemoteTarget);
                SiAuto.Main.LogString("gc.GrowlRemoteTarget", gc.GrowlRemoteTarget);
                if ((isGrowlEnabled == gc.GrowlEnabled) && (growlRemoteTarget != gc.GrowlRemoteTarget))
                {
                    SiAuto.Main.LogWarning("Growl is enabled and the remote machine has been changed. A service restart is required.");
                    changesDetected = true;
                }
                growlRemoteTarget = gc.GrowlRemoteTarget;

                if ((isGrowlEnabled == gc.GrowlEnabled) && (growlPassword != gc.GrowlPassword))
                {
                    SiAuto.Main.LogWarning("Growl is enabled and the password has been changed. A service restart is required.");
                    changesDetected = true;
                }
                growlPassword = gc.GrowlPassword;
            }
            else
            {
                return;
            }

            if (changesDetected)
            {
                if(QMessageBox.Show("Changes to the Growl configuration have been detected that require a service restart. Changing the notifications from local to remote " +
                    "(or vice versa), the remote port, target machine or password cannot be fully completed until the " +
                    (isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart) + " Service is restarted. " +
                    "Do you want to restart the " + (isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart) +
                    " Service now?", "Service Restart Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    tabPage4.Select();
                    tabPage4.Focus();
                    CallServiceRebooter();
                }
            }
        }

        private void buttonConfigureSnarl_Click(object sender, EventArgs e)
        {
            ConfigureSnarl();
        }

        private void ConfigureSnarl()
        {
            SnarlConfiguration sc = new SnarlConfiguration(configurationKey, isWindowsServerSolutions);
            sc.ShowDialog();

            if (sc.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                isSnarlEnabled = sc.SnarlEnabled;
            }
        }

        private void buttonBackupHelp_Click(object sender, EventArgs e)
        {
            String softwareName = isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart;
            QMessageBox.Show(
                "In order for " + softwareName + " to back up to a network (UNC) location (aka \"shared drive\"), the Windows service needs to connect to " +
                "the shared location. This cannot be done without a username and password.\n\nIf you are in a WORKGROUP configuration (most homes are in this " +
                "configuration), you should enter the username in the format COMPUTERNAME\\USERNAME.\n\nIf you are in a DOMAIN configuration (most businesses " +
                "are in this configuration), you should enter the username in the format DOMAINNAME\\USERNAME.\n\nIf your system administrator enforces " +
                "password expiration, you will need to change your password here.", "Help for Backing Up Over the Network", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void checkBoxBackup_CheckedChanged(object sender, EventArgs e)
        {
            performEmergencyBackup = checkBoxBackup.Checked;
            buttonSelectFiles.Enabled = performEmergencyBackup;

            radioButtonBackupLocal.Enabled = performEmergencyBackup;
            radioButtonBackupUnc.Enabled = performEmergencyBackup;

            SetLocalBackupOptions(performEmergencyBackup);
            EnableButtons();
        }

        private void radioButtonBackupLocal_CheckedChanged(object sender, EventArgs e)
        {
            backupLocal = radioButtonBackupLocal.Checked;
            SetLocalBackupOptions(true);
            EnableButtons();
        }

        private void SetLocalBackupOptions(bool enableAll)
        {
            if (enableAll)
            {
                if (backupLocal)
                {
                    buttonSelectTarget.Enabled = true;
                    textBoxUncBackup.Enabled = false;
                    textBoxBackupUsername.Enabled = false;
                    textBoxBackupPassword.Enabled = false;
                    buttonUncTest.Enabled = false;
                }
                else
                {
                    buttonSelectTarget.Enabled = false;
                    textBoxUncBackup.Enabled = true;
                    textBoxBackupUsername.Enabled = true;
                    textBoxBackupPassword.Enabled = true;
                    buttonUncTest.Enabled = true;
                }
            }
            else
            {
                buttonSelectTarget.Enabled = false;
                textBoxUncBackup.Enabled = false;
                textBoxBackupUsername.Enabled = false;
                textBoxBackupPassword.Enabled = false;
                buttonUncTest.Enabled = false;
            }
        }

        private void checkBoxThermalShutdown_CheckedChanged(object sender, EventArgs e)
        {
            performThermalShutdown = checkBoxThermalShutdown.Checked;
            EnableButtons();
        }

        private void checkBoxRunProgram_CheckedChanged(object sender, EventArgs e)
        {
            performCustomBackup = checkBoxRunProgram.Checked;
            textBoxCustomBackupArgs.Enabled = performCustomBackup;
            buttonSelectCustomProgram.Enabled = performCustomBackup;
            EnableButtons();
        }

        private void checkBoxNoHotBackup_CheckedChanged(object sender, EventArgs e)
        {
            noHotBackup = checkBoxNoHotBackup.Checked;
            EnableButtons();
        }

        private void checkBoxThermalShutdown_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBoxThermalShutdown.Checked && (numericUpDown1.Value < 5 || numericUpDown1.Value > 20))
            {
                QMessageBox.Show("Your disk polling interval is currently set to " + numericUpDown1.Value.ToString() + " minutes. The ideal " +
                    "polling range is 5-20 minutes. If the polling interval is set to less than 5 minutes, a disk that is overheated for as little as 3 " +
                    "minutes could trigger a premature shutdown. On the other hand, a polling interval longer than 20 minutes could potentially \"miss\" " +
                    "disks that run overheated for extended periods, but due to the length of the polling interval, long periods of continuous " +
                    "overheating may go undetected.", "Non-Optimal Polling Interval", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            performThermalShutdown = checkBoxThermalShutdown.Checked;
            EnableButtons();
        }

        private void textBoxUncBackup_TextChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void textBoxBackupUsername_TextChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void textBoxBackupPassword_TextChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void textBoxLocalBackup_TextChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void textBoxCustomBackupProgram_TextChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void buttonSelectFiles_Click(object sender, EventArgs e)
        {
            ConfigureSilvermist silvermist = new ConfigureSilvermist();
            silvermist.ShowDialog();
        }

        private void buttonRunProgramHelp_Click(object sender, EventArgs e)
        {
            QMessageBox.Show("If a disk failure is detected, you can run a program of your choosing, such as a third party backup program. You can specify the program name " +
                "by itself, or with optional arguments. If you would like to pass a comma-separated list of physical disk number(s) reporting a failure, specify {PHYSICALDISKS} as an argument. If " +
                "you would like to pass a comma-separated list of drive letter(s) of failing disk(s), if " + (isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart) +
                " can obtain the letters, specify {DRIVELETTERS}. If specified, you must specify the arguments in all caps, and include the curly braces.\n\nExamples:\n\n" +
                "    /Backup /Disks={DRIVELETTERS}\n\n    /Backup /Disks={PHYSICALDISKS}", "Run a Third Party Program Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonUncTest_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxUncBackup.Text))
            {
                QMessageBox.Show("You must specify a UNC path (network share location).", "Missing Network Share Location", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxUncBackup.Focus();
                return;
            }
            if (String.IsNullOrEmpty(textBoxBackupUsername.Text))
            {
                QMessageBox.Show("You must specify a username for a network backup.", "Missing Username", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxBackupUsername.Focus();
                return;
            }
            if (String.IsNullOrEmpty(textBoxBackupUsername.Text))
            {
                QMessageBox.Show("You must specify a password for a network backup.", "Missing Password", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxBackupPassword.Focus();
                return;
            }

            if (QMessageBox.Show("This test will attempt to perform the following actions, using the username and password you specified. This test will ensure that, should a " +
                "disk failure be detected in the future, the backup can successfully run.\n\n1. Map drive letter B: to " + textBoxUncBackup.Text + "\n" +
                "2. Create a temporary file on B:\n3. Delete the temporary file\n4. Un-map the B: drive.\n\nIf these operations are successful, then an automatic backup " +
                "should run successfully. If these operations fail, you need to resolve the identified problem or else an automatic backup will not run successfully. Do you " +
                "want to perform the test now?", "Test Backup to Network Share", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bool mapDriveSuccess = false;
                bool demolishMapSuccess = false;
                bool writeSuccess = false;
                bool deleteSuccess = false;

                String failureReasonMap = String.Empty;
                String failureReasonWrite = String.Empty;
                String failureReasonUnmap = String.Empty;

                aejw.Network.NetworkDrive drive = new aejw.Network.NetworkDrive();

                try
                {
                    drive.Force = false;
                    drive.LocalDrive = "B:";
                    drive.Persistent = false;
                    drive.PromptForCredentials = false;
                    drive.SaveCredentials = false;
                    drive.ShareName = textBoxUncBackup.Text;
                    drive.MapDrive(textBoxBackupUsername.Text, textBoxBackupPassword.Text);
                    mapDriveSuccess = true;
                }
                catch (Exception ex)
                {
                    failureReasonMap = ex.Message;
                }

                if (mapDriveSuccess)
                {
                    try
                    {
                        System.IO.StreamWriter writer = new System.IO.StreamWriter("B:\\windowsmarttemporary.txt");
                        writer.WriteLine("Testing the ability to write to a network share.");
                        writer.Flush();
                        writer.Close();
                        writeSuccess = true;
                        System.IO.File.Delete("B:\\windowsmarttemporary.txt");
                        deleteSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        failureReasonWrite = ex.Message;
                    }
                }

                if (mapDriveSuccess)
                {
                    try
                    {
                        drive.Force = true;
                        drive.UnMapDrive();
                        demolishMapSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        failureReasonUnmap = ex.Message;
                    }
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                if (mapDriveSuccess && writeSuccess && deleteSuccess && demolishMapSuccess)
                {
                    sb.Append("The network share mapping and write test is complete. No problems were detected. Below are the results of the test.\n\n");
                }
                else if (mapDriveSuccess && writeSuccess)
                {
                    sb.Append("The network share mapping and write test completed with warnings. Some problems were detected, which MIGHT cause a problem ");
                    sb.Append("with an automatic backup. You should take corrective actions to be sure all operations can run successfully - check to be ");
                    sb.Append("sure the user account has access permissions to write, modify and delete on the remote server. Below are the results of the test.\n\n");
                }
                else
                {
                    sb.Append("The network share mapping and write test completed with errors. Serious problems were detected, which WILL cause a problem ");
                    sb.Append("with an automatic backup. You must take corrective actions to be sure all operations can run successfully - check to be ");
                    sb.Append("sure the UNC network share exists, that your user account credentials are correct, and that the user account has access ");
                    sb.Append("permissions to write, modify and delete on the remote server. Below are the results of the test.\n\n");
                }

                sb.Append("   Map B: Drive - " + (mapDriveSuccess ? "SUCCESSFUL" : "FAILED (" + failureReasonMap + ")") + "\n");
                if (mapDriveSuccess && writeSuccess)
                {
                    sb.Append("   Write Temp File - SUCCESSFUL\n");
                    sb.Append("   Delete Temp File - " + (deleteSuccess ? "SUCCESSFUL" : "FAILED (" + failureReasonWrite + ")") + "\n");
                }
                else if (mapDriveSuccess)
                {
                    sb.Append("   Write Temp File - FAILED (" + failureReasonWrite + ")\n");
                    sb.Append("   Delete Temp File - NOT TESTED\n");
                }
                else
                {
                    sb.Append("   Write Temp File - NOT TESTED\n");
                    sb.Append("   Delete Temp File - NOT TESTED\n");
                }

                if (mapDriveSuccess)
                {
                    sb.Append("   Unmap B: Drive - " + (demolishMapSuccess ? "SUCCESSFUL" : "FAILED (" + failureReasonUnmap + ")") + "\n");
                }

                if (mapDriveSuccess && writeSuccess && deleteSuccess && demolishMapSuccess)
                {
                    QMessageBox.Show(sb.ToString(), "Test Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (mapDriveSuccess && writeSuccess)
                {
                    QMessageBox.Show(sb.ToString(), "Test Completed with Warnings", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    QMessageBox.Show(sb.ToString(), "Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void buttonSelectTarget_Click(object sender, EventArgs e)
        {
            localBackupPath = SelectFolder(textBoxLocalBackup, "Please select the new logging location", false);
            textBoxLocalBackup.Text = localBackupPath;
        }

        private void buttonSelectCustomProgram_Click(object sender, EventArgs e)
        {
            customBackupProgram = SelectFolder(textBoxCustomBackupProgram, "Select Custom Backup Program", true);
            textBoxCustomBackupProgram.Text = customBackupProgram;
        }

        private String SelectFolder(System.Windows.Forms.TextBox targetTextBox, String titleMessage, bool selectAsFile)
        {
            if (selectAsFile)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Executables (*.exe)|*.exe|MS-DOS programs (*.com)|*.com|Command scripts (*.cmd)|*.cmd|Batch files (*.bat)|*.bat";
                ofd.Title = titleMessage;
                ofd.Multiselect = false;
                ofd.ShowDialog();

                if (String.IsNullOrEmpty(ofd.FileName) || String.Compare(ofd.FileName, targetTextBox.Text, true) == 0)
                {
                    return targetTextBox.Text;
                }
                EnableButtons();
                return ofd.FileName;
            }
            else
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (System.IO.Directory.Exists(targetTextBox.Text))
                {
                    fbd.SelectedPath = targetTextBox.Text;
                }
                else if (System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Dojo North Software\\Logs"))
                {
                    fbd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Dojo North Software\\Logs";
                }
                else if (System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)))
                {
                    fbd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                }

                fbd.ShowNewFolderButton = true;
                fbd.Description = titleMessage;
                fbd.ShowDialog();

                if (String.IsNullOrEmpty(fbd.SelectedPath) || String.Compare(fbd.SelectedPath, targetTextBox.Text, true) == 0)
                {
                    return targetTextBox.Text;
                }
                EnableButtons();
                return fbd.SelectedPath;
            }
        }

        private void textBoxCustomBackupArgs_TextChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private bool IsDebuggingAvailable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.IsDebuggingAvailable");
            SiAuto.Main.LogString("debugLogLocation", debugLogLocation);
            try
            {
                if (System.IO.Directory.Exists(debugLogLocation))
                {
                    SiAuto.Main.LogMessage("Debug folder exists, checking for SIL files.");
                    String[] files = System.IO.Directory.GetFiles(debugLogLocation, "*.sil");
                    if (files.Length > 0)
                    {
                        SiAuto.Main.LogMessage("Log fies present; logging is available.");
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.IsDebuggingAvailable");
                        return true;
                    }
                    SiAuto.Main.LogWarning("Log files absent; logging not available.");
                }
                SiAuto.Main.LogWarning("Debug directory does not exist; logging is not available.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogWarning("Logging is not available. " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.IsDebuggingAvailable");
            return false;
        }

        private void checkBoxAlertAmperes_CheckedChanged(object sender, EventArgs e)
        {
            notifyOnPowerChange = checkBoxAlertAmperes.Checked;
            EnableButtons();
        }

        private void checkBoxAlertFilthy_CheckedChanged(object sender, EventArgs e)
        {
            notifyOnFilthyShutdown = checkBoxAlertFilthy.Checked;
            EnableButtons();
        }

        private void checkBoxPreserve_CheckedChanged(object sender, EventArgs e)
        {
            preserveOnUninstall = checkBoxPreserve.Checked;
            EnableButtons();
        }
    }
}
