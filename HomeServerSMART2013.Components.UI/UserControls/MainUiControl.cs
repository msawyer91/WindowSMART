using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using WSSControls.BelovedComponents;
using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components;
using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class MainUiControl : UserControl
    {
        #region Private Member Variables
        private SmartDefinitions definitions;
        private SmartData smartDataTable;
        private ServiceController serviceController;
        //private PleaseWait pleaseWaitBanner;
        private Mexi_Sexi.MexiSexi mexiSexi;

        private int badSectorCountOrRetiredBlockCount = 0;
        private int pendingSectorCountOrSsdLifeLeft = 0;
        private int endToEndErrorCountOrLifeCurve = 0;
        private int recalibrationRetryCount = 0;
        private int reallocationEventCount = 0;
        private int spinRetryCount = 0;
        private int uncorrectableSectorCount = 0;
        private int ultraAtaCrcErrorCount = 0;
        private int temperature = 0;
        private int fTemperature = 0;
        private int kTemperature = 0;
        private int airflowTemperature = 0;
        private int fAirflowTemperature = 0;
        private int kAirflowTemperature = 0;
        private int absurdTemperatureThreshold = 90;
        private int powerOnHours = -1;

        private String lastSelectedPath = String.Empty;

        private bool isDiskCritical = false;
        private bool isDiskWarning = false;
        private bool isDiskGeriatric = false;
        private bool isUnknown = true;
        private bool isThresholdExceeded = false;
        private bool isWmiFailurePredicted = false;
        private bool isWindowsServerSolutions = false;
        private bool isDiskTemperatureInvalid = false;

        // SSD support
        private bool isSsd = false;
        private bool isTrimSupported = false;
        private String ssdControllerManufacturer = String.Empty;
        private bool isSsdThrottled = false;

        // Widnows 8
        private bool isWindows8 = false;

        // Skinning
        private bool useDefaultSkinning = true;
        private int windowBackground = 0;

        // Multi-threading makes the UI look better (more responsive) during slow WMI calls.
        private Thread processingThread;
        private bool isWorkerProcessRunning = false;

        // Delegates are required for UI updates will occur from worker processes.
        private delegate void UpdatePhysicalDiskListView(ListViewItem lvi);
        private delegate void PhysicalDiskSelection();
        private delegate void SortListViewSelectFirstDisk();
        private delegate void UpdateTheProgressBar(int progress);
        private delegate void EpicFailDelegate(Exception ex, String message, String resolution);
        private UpdatePhysicalDiskListView physicalDiskUpdate;
        private PhysicalDiskSelection firstDiskSelector;
        private SortListViewSelectFirstDisk sortViewFirstSelect;
        private UpdateTheProgressBar progressBarUpdater;
        private EpicFailDelegate dlgEpicFail;

        // GPO
        private int gpoDpa;
        private int gpoTempCtl;
        private int gpoVirtualIgnore;
        private int gpoAllowIgnoredItems;
        private int gpoEmailNotificate;
        private int gpoProwlNotificate;
        private int gpoNmaNotificate;
        private int gpoAdvancedSettings;
        private int gpoDebuggingControl;
        private int gpoSSD;
        private int gpoCheckForUpdates;
        private int gpoUiTheme;
        private int gpoUseSupportMessage;

        //
        // CONFIGURABLE OPTIONS !!
        //
        Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
        Microsoft.Win32.RegistryKey dojoNorthSubKey;
        Microsoft.Win32.RegistryKey configurationKey;

        bool isRegistryAvailable;
        bool ignoreHot;
        bool ignoreWarm;
        bool reportCritical;
        bool reportWarnings;

        bool fallbackToWmi;
        bool debugMode;
        bool advancedSii;
        bool ignoreVirtualDisks;

        bool invokedFromHss;

        int criticalTemperatureThreshold;
        int overheatedTemperatureThreshold;
        int hotTemperatureThreshold;
        int warmTemperatureThreshold;
        int pollingInterval;

        // SSD thresholds
        private int ssdLifeLeftCritical;
        private int ssdLifeLeftWarning;
        private int ssdRetirementCritical;
        private int ssdRetirementWarning;

        // Logging
        private String debugLogLocation;

        // Phantom disks
        private List<String> phantomDisks;

        String temperaturePreference;
        #endregion

        #region Constructor
        public MainUiControl(Mexi_Sexi.MexiSexi ms, bool isHss)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.MainUiControl");
            InitializeComponent();
            mexiSexi = ms;
            invokedFromHss = isHss;
            phantomDisks = new List<String>();

            // Windows 8 check
            isWindows8 = Utilities.Utility.IsSystemWindows8();

            listViewPhysicalDisks.ListViewItemSorter = new ListViewColumnSorter();
            definitions = new SmartDefinitions();
            smartDataTable = new SmartData();

            // Initialize the delegates.
            physicalDiskUpdate = new UpdatePhysicalDiskListView(DAddListViewItemToPhysicalDisks);
            firstDiskSelector = new PhysicalDiskSelection(DSelectFirstPhysicalDisk);
            sortViewFirstSelect = new SortListViewSelectFirstDisk(DSortListView);
            progressBarUpdater = new UpdateTheProgressBar(DUpdateProgressBar);
            dlgEpicFail = new EpicFailDelegate(DEpicFail);

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

            SetDefaults();
            LoadDataFromRegistry();

            isWindowsServerSolutions = OperatingSystem.IsWindowsServerSolutionsProduct(this);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.MainUiControl");
        }
        #endregion

        #region Configuration Section
        private void SetDefaults()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.SetDefaults");
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
            ignoreHot = false;
            ignoreWarm = false;

            // We do report criticals and warnings, unless changed by the user.
            reportCritical = true;
            reportWarnings = true;

            // Default poling interval 15 minutes (900000 milliseconds)
            pollingInterval = 900000;

            // Debugging/troubleshooting options.
            fallbackToWmi = true;
            debugMode = false;
            advancedSii = false;

            // Ignore virtual disks ~ Dan ~
            ignoreVirtualDisks = true;

            // SSD thresholds
            ssdLifeLeftCritical = 10;
            ssdLifeLeftWarning = 30;
            ssdRetirementCritical = 150;
            ssdRetirementWarning = 50;

            // Logging
            debugLogLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Dojo North Software\\Logs";

            SiAuto.Main.LogMessage("=== Default Configuraion Values ===");
            SiAuto.Main.LogString("temperaturePreference", temperaturePreference);
            SiAuto.Main.LogInt("criticalTemperatureThreshold", criticalTemperatureThreshold);
            SiAuto.Main.LogInt("overheatedTemperatureThreshold", overheatedTemperatureThreshold);
            SiAuto.Main.LogInt("hotTemperatureThreshold", hotTemperatureThreshold);
            SiAuto.Main.LogInt("warmTemperatureThreshold", warmTemperatureThreshold);
            SiAuto.Main.LogBool("ignoreHot", ignoreHot);
            SiAuto.Main.LogBool("ignoreWarm", ignoreWarm);
            SiAuto.Main.LogBool("ignoreOverheated", ignoreVirtualDisks);
            SiAuto.Main.LogBool("reportCritical", reportCritical);
            SiAuto.Main.LogBool("reportWarnings", reportWarnings);
            SiAuto.Main.LogBool("fallbackToWmi", fallbackToWmi);
            SiAuto.Main.LogBool("debugMode", debugMode);
            SiAuto.Main.LogBool("advancedSii", advancedSii);
            SiAuto.Main.LogInt("pollingInterval", pollingInterval);

            // SSD thresholds
            SiAuto.Main.LogInt("ssdLifeLeftCritical", ssdLifeLeftCritical);
            SiAuto.Main.LogInt("ssdLifeLeftWarning", ssdLifeLeftWarning);
            SiAuto.Main.LogInt("ssdRetirementCritical", ssdRetirementCritical);
            SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);

            // Skinning
            SiAuto.Main.LogBool("useDefaultSkinning", useDefaultSkinning);
            SiAuto.Main.LogInt("windowBackground", windowBackground);

            // Logging
            SiAuto.Main.LogString("debugLogLocation", debugLogLocation);
            SiAuto.Main.LogMessage("=== End of Default Configuraion Values ===");

            // GPO
            gpoDpa = 2;
            gpoTempCtl = 2;
            gpoVirtualIgnore = 2;
            gpoAllowIgnoredItems = 2;
            gpoEmailNotificate = 2;
            gpoProwlNotificate = 2;
            gpoNmaNotificate = 2;
            gpoAdvancedSettings = 2;
            gpoDebuggingControl = 2;
            gpoSSD = 2;
            gpoCheckForUpdates = 2;
            gpoUiTheme = 2;
            gpoUseSupportMessage = 2;



            if (!isRegistryAvailable)
            {
                // Maybe the Registry has never been configured before. Let's try to configure it.
                try
                {
                    dojoNorthSubKey = registryHklm.CreateSubKey(Properties.Resources.RegistryDojoNorthRootKey);

                    configurationKey = dojoNorthSubKey.CreateSubKey(Properties.Resources.RegistryConfigurationKey);
                    isRegistryAvailable = true;
                    SaveData();
                }
                catch (Exception)
                {
                    isRegistryAvailable = false;
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.SetDefaults");
        }

        private void SaveData()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.SaveData");
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

                configurationKey.SetValue(Properties.Resources.RegistryConfigCriticalTempThreshold, criticalTemperatureThreshold);
                configurationKey.SetValue(Properties.Resources.RegistryConfigOverheatedTempThreshold, overheatedTemperatureThreshold);
                configurationKey.SetValue(Properties.Resources.RegistryConfigHotTempThreshold, hotTemperatureThreshold);
                configurationKey.SetValue(Properties.Resources.RegistryConfigWarmTempThreshold, warmTemperatureThreshold);

                configurationKey.SetValue(Properties.Resources.RegistryConfigIgnoreHot, ignoreHot);
                configurationKey.SetValue(Properties.Resources.RegistryConfigIgnoreWarm, ignoreWarm);
                configurationKey.SetValue(Properties.Resources.RegistryConfigReportCritical, reportCritical);
                configurationKey.SetValue(Properties.Resources.RegistryConfigReportWarning, reportWarnings);
                configurationKey.SetValue(Properties.Resources.RegistryConfigPollingInterval, pollingInterval);

                configurationKey.SetValue(Properties.Resources.RegistryConfigFallbackToWmi, fallbackToWmi);
                configurationKey.SetValue(Properties.Resources.RegistryConfigEnableDebugLogging, debugMode);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSiIAdvanced, advancedSii);

                configurationKey.SetValue(Properties.Resources.RegistryConfigIgnoreVirtualDisks, ignoreVirtualDisks);

                // SSD thresholds
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdLifeLeftCritical, ssdLifeLeftCritical);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdLifeLeftWarning, ssdLifeLeftWarning);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdRetirementCritical, ssdRetirementCritical);
                configurationKey.SetValue(Properties.Resources.RegistryConfigSsdRetirementWarning, ssdRetirementWarning);

                isRegistryAvailable = true;
            }
            catch (Exception)
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.SaveData");
        }

        private void LoadDataFromRegistry()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.LoadDataFromRegistry");
            try
            {
                temperaturePreference = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigTemperaturePreference);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigTemperaturePreference, temperaturePreference);
            }

            try
            {
                ignoreHot = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIgnoreHot));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigIgnoreHot, ignoreHot);
            }

            try
            {
                ignoreWarm = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIgnoreWarm));
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigIgnoreWarm, ignoreWarm);
            }

            try
            {
                criticalTemperatureThreshold = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigCriticalTempThreshold);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigCriticalTempThreshold, criticalTemperatureThreshold);
            }

            try
            {
                overheatedTemperatureThreshold = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigOverheatedTempThreshold);
            }
            catch
            {
                configurationKey.SetValue(Properties.Resources.RegistryConfigOverheatedTempThreshold, overheatedTemperatureThreshold);
            }

            try
            {
                hotTemperatureThreshold = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigHotTempThreshold);
            }
            catch
            {
            }

            try
            {
                warmTemperatureThreshold = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigWarmTempThreshold);
            }
            catch
            {
            }

            try
            {
                pollingInterval = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigPollingInterval);
            }
            catch
            {
            }

            try
            {
                reportCritical = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigReportCritical));
            }
            catch
            {
            }

            try
            {
                reportWarnings = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigReportWarning));
            }
            catch
            {
            }

            try
            {
                fallbackToWmi = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigFallbackToWmi));
            }
            catch
            {
            }

            try
            {
                ignoreVirtualDisks = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIgnoreVirtualDisks));
            }
            catch
            {
                ignoreVirtualDisks = true;
            }

            try
            {
                debugMode = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigEnableDebugLogging));
            }
            catch
            {
            }

            try
            {
                advancedSii = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigSiIAdvanced));
            }
            catch
            {
            }

            // SSD thresholds
            try
            {
                ssdLifeLeftCritical = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigSsdLifeLeftCritical);
            }
            catch
            {
            }

            try
            {
                ssdLifeLeftWarning = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigSsdLifeLeftWarning);
            }
            catch
            {
            }

            try
            {
                ssdRetirementCritical = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigSsdRetirementCritical);
            }
            catch
            {
            }

            try
            {
                ssdRetirementWarning = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigSsdRetirementWarning);
            }
            catch
            {
            }

            // Skinning
            try
            {
                windowBackground = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigWindowBackground);
            }
            catch
            {
                windowBackground = 0;
            }

            try
            {
                useDefaultSkinning = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigUseDefaultSkinning));
            }
            catch
            {
                useDefaultSkinning = true;
            }

            // GPO - only really concerned about ignored items in main form!
            try
            {
                gpoAllowIgnoredItems = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigGpoAllowIgnoredItems);
            }
            catch
            {
                gpoAllowIgnoredItems = 2;
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
                
            }

            SiAuto.Main.LogMessage("=== Configuraion Values as Defined in Registry ===");
            SiAuto.Main.LogString("temperaturePreference", temperaturePreference);
            SiAuto.Main.LogInt("criticalTemperatureThreshold", criticalTemperatureThreshold);
            SiAuto.Main.LogInt("overheatedTemperatureThreshold", overheatedTemperatureThreshold);
            SiAuto.Main.LogInt("hotTemperatureThreshold", hotTemperatureThreshold);
            SiAuto.Main.LogInt("warmTemperatureThreshold", warmTemperatureThreshold);
            SiAuto.Main.LogBool("ignoreHot", ignoreHot);
            SiAuto.Main.LogBool("ignoreWarm", ignoreWarm);
            SiAuto.Main.LogBool("ignoreOverheated", ignoreVirtualDisks);
            SiAuto.Main.LogBool("reportCritical", reportCritical);
            SiAuto.Main.LogBool("reportWarnings", reportWarnings);
            SiAuto.Main.LogBool("fallbackToWmi", fallbackToWmi);
            SiAuto.Main.LogBool("debugMode", debugMode);
            SiAuto.Main.LogBool("advancedSii", advancedSii);

            SiAuto.Main.LogInt("pollingInterval", pollingInterval);

            // SSD thresholds
            SiAuto.Main.LogInt("ssdLifeLeftCritical", ssdLifeLeftCritical);
            SiAuto.Main.LogInt("ssdLifeLeftWarning", ssdLifeLeftWarning);
            SiAuto.Main.LogInt("ssdRetirementCritical", ssdRetirementCritical);
            SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
            SiAuto.Main.LogString("debugLogLocation", debugLogLocation);

            // Skinning
            SiAuto.Main.LogBool("useDefaultSkinning", useDefaultSkinning);
            SiAuto.Main.LogInt("windowBackground", windowBackground);

            // GPO
            SiAuto.Main.LogInt("allowIgnoredItems", gpoAllowIgnoredItems);

            SiAuto.Main.LogMessage("=== End of Configuraion Values as Defined in Registry ===");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.LoadDataFromRegistry");
        }
        #endregion

        /// <summary>
        /// Resets all counts and flags on a disk selection change.
        /// </summary>
        private void ResetDiskState()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.ResetDiskState");
            badSectorCountOrRetiredBlockCount = 0;
            pendingSectorCountOrSsdLifeLeft = 0;
            endToEndErrorCountOrLifeCurve = 0;
            reallocationEventCount = 0;
            spinRetryCount = 0;
            uncorrectableSectorCount = 0;
            ultraAtaCrcErrorCount = 0;
            temperature = 0;
            fTemperature = 0;
            kTemperature = 0;
            airflowTemperature = 0;
            fAirflowTemperature = 0;
            kAirflowTemperature = 0;
            powerOnHours = -1;

            isDiskCritical = false;
            isDiskWarning = false;
            isDiskGeriatric = false;
            isUnknown = true;
            isThresholdExceeded = false;
            isWmiFailurePredicted = false;
            isSsdThrottled = false;
            isDiskTemperatureInvalid = false;
            SiAuto.Main.LogMessage("Disk state reset.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.ResetDiskState");
        }

        private void listViewPhysicalDisks_SelectedIndexChanged(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.listViewPhysicalDisks_SelectedIndexChanged");

            //SetIgnoreVisibility();
            if (listViewPhysicalDisks.SelectedItems != null && listViewPhysicalDisks.SelectedItems.Count != 0)
            {
                SiAuto.Main.LogMessage("A valid item was selected.");
                buttonIgnoreDisk.Enabled = true;
                buttonExport.Enabled = true;
                SiAuto.Main.LogMessage("Ignore Disk button is enabled.");

                ListViewItem lvi = null;
                bool diskFound = false;

                // This try block runs code to see if the selected index change is really a change. If the user clicks the column header,
                // it really isn't a change, just a sort. This should reduce or eliminate the flashing/disappearing of the control on
                // sort operations.
                try
                {
                    // Make sure we're not talking about the same disk.
                    lvi = listViewPhysicalDisks.SelectedItems[0];
                    if (String.Compare(lastSelectedPath, lvi.SubItems[0].Text, true) == 0)
                    {
                        SiAuto.Main.LogMessage("Last selected path and current selected disk are the same; no refresh needed.");
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.listViewPhysicalDisks_SelectedIndexChanged");
                        return;
                    }
                    else
                    {
                        SiAuto.Main.LogString("lastSelectedPath", lastSelectedPath);
                        SiAuto.Main.LogString("currentSelected", lvi.SubItems[0].Text);
                        SiAuto.Main.LogMessage("The current selected item and the last selected path are different; will refresh.");
                        lastSelectedPath = lvi.SubItems[0].Text;
                    }
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogWarning(ex.Message);
                    SiAuto.Main.LogException(ex);
                }

                try
                {
                    // Determine what is selected.
                    this.Invoke(progressBarUpdater, new object[] { 10 });
                    lvi = listViewPhysicalDisks.SelectedItems[0];

                    // Browse all WMI physical disks.
                    if (isWindows8)
                    {
                        foreach (ManagementObject drive in new ManagementObjectSearcher(Properties.Resources.WmiQueryScope,
                          Properties.Resources.WmiQueryStringWin8).Get())
                        {
                            SiAuto.Main.LogMessage("[Physical Disk] Enumerating WMI disks in MSFT_PhysicalDisk.");
                            if (drive["DeviceID"] == null)
                            {
                                String friendlyName = String.Empty;
                                try
                                {
                                    if (drive["FriendlyName"] == null)
                                    {
                                        SiAuto.Main.LogWarning("[Storage Spaces] Phantom Disk detected. Name unknown.");
                                    }
                                    else
                                    {
                                        friendlyName = drive["FriendlyName"].ToString();
                                        SiAuto.Main.LogWarning("[Storage Spaces] Phantom Disk detected: " + friendlyName);
                                    }
                                }
                                catch
                                {
                                }

                                if (!phantomDisks.Contains(friendlyName))
                                {
                                    InjectPhantomDisk(friendlyName);
                                    QMessageBox.Show("The system detected a phantom disk. This occurs when a physical disk participating in a Windows 8 Storage Space " +
                                        "has been removed from the system, but the disk's record is still a part of the Storage Space. The disk may have failed, or has " +
                                        "been removed without its record being removed from the Storage Space.\n\nPlease check your Storage Spaces and ensure all disks " +
                                        "are present and online. If you recently removed and/or replaced a disk, and you do not intend to return that disk to the " +
                                        "Storage Space, please go into Storage Spaces and remove the disk from the pool.\n\nThe following may help identify the disk: " +
                                        friendlyName, "Phantom Disk Detected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                            }
                            if (lvi.SubItems[0].Text.ToUpper().Contains("\\\\.\\PHANTOMDISK"))
                            {
                                QMessageBox.Show("You selected a phantom disk. Phantom disks are disks that were actively participating in a Windows 8 Storage Space, but " +
                                    "are no longer detected. The disk may have failed, removed by you, taken offline or lost power. If you knowingly removed the disk, " +
                                    "this message will continue to appear until you remove the disk from the Storage Space. If this message was not expected, please check " +
                                    "all of your Storage Space disks to ensure that they are powered on and connected. If the disk is connected and powered on, you can try " +
                                    "power cycling the disk if it is located in an external enclosure. If the disk is internal, try rebooting the computer. If that doesn't work, " +
                                    "check all data and power cables to ensure the disk is actually connected. A cable may be faulty or could have come loose.\n\n" +
                                    "This text may help you identify the disk in question: " + lvi.SubItems[1].Text, "Phantom Disk Selected",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                SiAuto.Main.LogMessage("[Physical Disk] Reset Disk State");
                                this.Invoke(progressBarUpdater, new object[] { 25 });
                                ResetDiskState();
                                SiAuto.Main.LogMessage("[Physical Disk] Populate Disk Info");
                                this.Invoke(progressBarUpdater, new object[] { 50 });
                                PopulateDiskInfo(null);
                                SiAuto.Main.LogMessage("[Physical Disk] Populate SMART Info");
                                this.Invoke(progressBarUpdater, new object[] { 75 });
                                PopulateSmartInfo(null);
                                SiAuto.Main.LogMessage("[Physical Disk] Set diskFound to true");
                                this.Invoke(progressBarUpdater, new object[] { 90 });
                                diskFound = true;
                                this.Invoke(progressBarUpdater, new object[] { 00 });
                                break;
                            }
                            if (drive["DeviceID"] != null && String.Compare("\\\\.\\PHYSICALDRIVE" + drive["DeviceID"].ToString(), lvi.SubItems[0].Text, true) == 0)
                            {
                                SiAuto.Main.LogMessage("[Physical Disk] Reset Disk State");
                                this.Invoke(progressBarUpdater, new object[] { 25 });
                                ResetDiskState();
                                SiAuto.Main.LogMessage("[Physical Disk] Populate Disk Info");
                                this.Invoke(progressBarUpdater, new object[] { 50 });
                                PopulateDiskInfo(drive);
                                SiAuto.Main.LogMessage("[Physical Disk] Populate SMART Info");
                                this.Invoke(progressBarUpdater, new object[] { 75 });
                                PopulateSmartInfo(drive);
                                SiAuto.Main.LogMessage("[Physical Disk] Set diskFound to true");
                                this.Invoke(progressBarUpdater, new object[] { 90 });
                                diskFound = true;
                            }
                            this.Invoke(progressBarUpdater, new object[] { 00 });
                        }
                    }
                    else
                    {
                        foreach (ManagementObject drive in new ManagementObjectSearcher(
                          Properties.Resources.WmiQueryStringNonWin8).Get())
                        {
                            SiAuto.Main.LogMessage("[Physical Disk] Enumerating WMI disks in Win32_DiskDrive.");
                            if (String.Compare(drive["DeviceID"].ToString(), lvi.SubItems[0].Text, true) == 0)
                            {
                                SiAuto.Main.LogMessage("[Physical Disk] Reset Disk State");
                                this.Invoke(progressBarUpdater, new object[] { 25 });
                                ResetDiskState();
                                SiAuto.Main.LogMessage("[Physical Disk] Populate Disk Info");
                                this.Invoke(progressBarUpdater, new object[] { 50 });
                                PopulateDiskInfo(drive);
                                SiAuto.Main.LogMessage("[Physical Disk] Populate SMART Info");
                                this.Invoke(progressBarUpdater, new object[] { 75 });
                                PopulateSmartInfo(drive);
                                SiAuto.Main.LogMessage("[Physical Disk] Set diskFound to true");
                                this.Invoke(progressBarUpdater, new object[] { 90 });
                                diskFound = true;
                            }
                            this.Invoke(progressBarUpdater, new object[] { 00 });
                        }
                    }
                    if (!diskFound)
                    {
                        SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Physical Disk] Disk " + lvi.SubItems[0].Text + " was NOT found in WMI.");
                        this.Invoke(progressBarUpdater, new object[] { 0 });
                        QMessageBox.Show(Properties.Resources.ErrorMessageDiskNotAvailable, Properties.Resources.ErrorMessageTitleDiskNotAvailable,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogFatal("[Epic FAIL] " + ex.Message);
                    SiAuto.Main.LogException(ex);
                    this.Invoke(progressBarUpdater, new object[] { 0 });
                    this.Invoke(dlgEpicFail, new object[] { ex, "An internal error has occurred executing a manual disk polling operation.",
                        "If you recently performed a \"Reset Everything\" operation, you MUST restart the Dashboard. Please close and re-launch " +
                        "the Dashboard. If you restarted the Home Server SMART Service or performed a \"Restore Defaults\" then it is recommended " +
                        "that you close and re-launch the Dashboard."});
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.listViewPhysicalDisks_SelectedIndexChanged");
                    return;
                }
            }
            else
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.listViewPhysicalDisks_SelectedIndexChanged");
                return;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.listViewPhysicalDisks_SelectedIndexChanged");
        }

        /// <summary>
        /// Populates the hardware/capacity FancyListView with details about the selected disk.
        /// </summary>
        /// <param name="drive"></param>
        private void PopulateDiskInfo(ManagementObject drive)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.PopulateDiskInfo");
            listViewDiskInfo.Items.Clear();

            if (drive == null)
            {
                ListViewItem lviNull = new ListViewItem(new String[] { "Model", "Phantom Disk" });
                lviNull.Group = listViewDiskInfo.Groups[0];
                listViewDiskInfo.Items.Add(lviNull);
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.PopulateDiskInfo");
                return;
            }

            SiAuto.Main.LogMessage("Fetch disk details.");

            DiskInfo info = new DiskInfo();
            info.Populate(isWindows8 ? "\\\\.\\PHYSICALDRIVE" + drive["DeviceID"].ToString() : drive["DeviceID"].ToString());

            SiAuto.Main.LogMessage("Display disk data in view.");
            ListViewItem lvi = new ListViewItem(new String[] { "Model", info.Model });
            lvi.Group = listViewDiskInfo.Groups[0];
            listViewDiskInfo.Items.Add(lvi);

            lvi = new ListViewItem(new String[] { "Serial Number", info.SerialNumber.Trim() });
            lvi.Group = listViewDiskInfo.Groups[0];
            listViewDiskInfo.Items.Add(lvi);

            lvi = new ListViewItem(new String[] { "Firmware Version", info.FirmwareRev.Trim() });
            lvi.Group = listViewDiskInfo.Groups[0];
            listViewDiskInfo.Items.Add(lvi);

            if (isWindows8)
            {
                lvi = new ListViewItem(new String[] { "Bus Type", info.InterfaceType });
                lvi.Group = listViewDiskInfo.Groups[0];
                listViewDiskInfo.Items.Add(lvi);
            }
            else
            {
                lvi = new ListViewItem(new String[] { "Description", info.Description });
                lvi.Group = listViewDiskInfo.Groups[0];
                listViewDiskInfo.Items.Add(lvi);

                lvi = new ListViewItem(new String[] { "Interface", info.InterfaceType });
                lvi.Group = listViewDiskInfo.Groups[0];
                listViewDiskInfo.Items.Add(lvi);

                lvi = new ListViewItem(new String[] { "Media Type", info.MediaType });
                lvi.Group = listViewDiskInfo.Groups[0];
                listViewDiskInfo.Items.Add(lvi);

                lvi = new ListViewItem(new String[] { "Name", info.Name });
                lvi.Group = listViewDiskInfo.Groups[0];
                listViewDiskInfo.Items.Add(lvi);

                lvi = new ListViewItem(new String[] { "Partition Count", info.PartitionCount });
                lvi.Group = listViewDiskInfo.Groups[0];
                listViewDiskInfo.Items.Add(lvi);
            }

            lvi = new ListViewItem(new String[] { "Status", info.Status });
            lvi.Group = listViewDiskInfo.Groups[0];
            listViewDiskInfo.Items.Add(lvi);

            if (String.Compare(info.FailurePredicted, "True", true) == 0)
            {
                lvi = new ListViewItem(new String[] { "Failure Predicted", "Yes" });
            }
            else
            {
                lvi = new ListViewItem(new String[] { "Failure Predicted", "No" });
            }
            lvi.Group = listViewDiskInfo.Groups[0];
            listViewDiskInfo.Items.Add(lvi);

            if (info.IsSsd)
            {
                lvi = new ListViewItem(new String[] { "Controller Manufacturer", info.SsdControllerManufacturer });
                lvi.Group = listViewDiskInfo.Groups[1];
                listViewDiskInfo.Items.Add(lvi);

                if (String.Compare(info.IsTrimSupported.ToString(), "True", true) == 0)
                {
                    lvi = new ListViewItem(new String[] { "TRIM Supported", "Yes" });
                }
                else
                {
                    lvi = new ListViewItem(new String[] { "TRIM Supported", "No" });
                }
                lvi.Group = listViewDiskInfo.Groups[1];
                listViewDiskInfo.Items.Add(lvi);
            }
            else
            {
                if (info.SsdControllerManufacturer == "0")
                {
                    lvi = new ListViewItem(new String[] { "Spindle Speed", "Not Reported by Drive" });
                }
                else if (info.SsdControllerManufacturer == "65535")
                {
                    lvi = new ListViewItem(new String[] { "Spindle Speed", "Unknown" });
                }
                else
                {
                    lvi = new ListViewItem(new String[] { "Spindle Speed", info.SsdControllerManufacturer + " RPM" });
                }
                lvi.Group = listViewDiskInfo.Groups[0];
                listViewDiskInfo.Items.Add(lvi);
            }

            if (isWindows8)
            {
                lvi = new ListViewItem(new String[] { "Physical Sector Size", info.PhysicalSectorSize });
                lvi.Group = listViewDiskInfo.Groups[2];
                listViewDiskInfo.Items.Add(lvi);

                lvi = new ListViewItem(new String[] { "Logical Sector Size", info.LogicalSectorSize });
                lvi.Group = listViewDiskInfo.Groups[2];
                listViewDiskInfo.Items.Add(lvi);
            }
            else
            {
                lvi = new ListViewItem(new String[] { "Bytes Per Sector", info.BytesPerSector });
                lvi.Group = listViewDiskInfo.Groups[2];
                listViewDiskInfo.Items.Add(lvi);

                lvi = new ListViewItem(new String[] { "Cylinders", info.Cylinders });
                lvi.Group = listViewDiskInfo.Groups[2];
                listViewDiskInfo.Items.Add(lvi);

                lvi = new ListViewItem(new String[] { "Heads", info.Heads });
                lvi.Group = listViewDiskInfo.Groups[2];
                listViewDiskInfo.Items.Add(lvi);

                lvi = new ListViewItem(new String[] { "Total Sectors", info.TotalSectors });
                lvi.Group = listViewDiskInfo.Groups[2];
                listViewDiskInfo.Items.Add(lvi);

                lvi = new ListViewItem(new String[] { "Tracks", info.Tracks });
                lvi.Group = listViewDiskInfo.Groups[2];
                listViewDiskInfo.Items.Add(lvi);

                lvi = new ListViewItem(new String[] { "Tracks Per Cylinder", info.TracksPerCylinder });
                lvi.Group = listViewDiskInfo.Groups[2];
                listViewDiskInfo.Items.Add(lvi);

                lvi = new ListViewItem(new String[] { "Total Bytes", info.TotalBytes });
                lvi.Group = listViewDiskInfo.Groups[2];
                listViewDiskInfo.Items.Add(lvi);
            }

            lvi = new ListViewItem(new String[] { "Advertised Capacity", info.AdvertisedCapacity + " GB" });
            lvi.Group = listViewDiskInfo.Groups[2];
            listViewDiskInfo.Items.Add(lvi);

            lvi = new ListViewItem(new String[] { "Real Capacity", info.RealCapacity + " GB" });
            lvi.Group = listViewDiskInfo.Groups[2];
            listViewDiskInfo.Items.Add(lvi);

            if (isWindows8)
            {
                int diskNumber = Utilities.Utility.GetDriveIdFromPath("\\\\.\\PHYSICALDRIVE" + drive["DeviceID"].ToString());
                if (diskNumber != -1)
                {
                    List<int> diskList = new List<int>();
                    diskList.Add(diskNumber);
                    List<String> listOfDisks = Utilities.Utility.GetDriveLettersFromDriveNumbers(diskList);
                    foreach (String mountPoint in listOfDisks)
                    {
                        double freeSpace = -1;
                        double diskCapacity = Utilities.Utility.GetTotalDiskSizeByDriveLetter(mountPoint, out freeSpace);
                        lvi = new ListViewItem(new String[] { "Drive Letter", mountPoint + " " + Utilities.Utility.GetVolumeLabelFsFromLetter(mountPoint) });
                        lvi.Group = listViewDiskInfo.Groups[3];
                        listViewDiskInfo.Items.Add(lvi);
                        if (freeSpace != -1 && diskCapacity != -1)
                        {
                            lvi = new ListViewItem(new String[] { "Drive " + mountPoint + " Space", Utilities.Utility.CreateDiskCapacityFreeString(diskCapacity, freeSpace) });
                            lvi.Group = listViewDiskInfo.Groups[3];
                            listViewDiskInfo.Items.Add(lvi);
                        }
                    }
                    if (listOfDisks.Count == 0)
                    {
                        lvi = new ListViewItem(new String[] { "Drive Letter", "Storage Space volume or no drive letter" });
                        lvi.Group = listViewDiskInfo.Groups[3];
                        listViewDiskInfo.Items.Add(lvi);
                    }
                }
            }
            else
            {
                try
                {
                    SiAuto.Main.LogMessage("Attempting to get partition and drive letter/mount point data.");
                    foreach (ManagementObject partition in new ManagementObjectSearcher(
                        "ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + drive["DeviceID"]
                        + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition").Get())
                    {
                        // associate partitions with logical disks (drive letter volumes)
                        SiAuto.Main.LogMessage("Associating partitions on " + drive["DeviceID"]);

                        foreach (ManagementObject disk in new ManagementObjectSearcher(
                            "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='"
                              + partition["DeviceID"]
                              + "'} WHERE AssocClass = Win32_LogicalDiskToPartition").Get())
                        {
                            SiAuto.Main.LogMessage("Associating locical drives on " + partition["DeviceID"]);
                            String mountPoint = disk["Name"].ToString();
                            double freeSpace = -1;
                            double diskCapacity = Utilities.Utility.GetTotalDiskSizeByDriveLetter(mountPoint, out freeSpace);
                            lvi = new ListViewItem(new String[] { (mountPoint.Contains('\\') ? "Mount Point" : "Drive Letter"), mountPoint + " " + Utilities.Utility.GetVolumeLabelFsFromLetter(mountPoint) });
                            lvi.Group = listViewDiskInfo.Groups[3];
                            listViewDiskInfo.Items.Add(lvi);
                            if (freeSpace != -1 && diskCapacity != -1)
                            {
                                lvi = new ListViewItem(new String[] { "Drive " + mountPoint + " Space", Utilities.Utility.CreateDiskCapacityFreeString(diskCapacity, freeSpace) });
                                lvi.Group = listViewDiskInfo.Groups[3];
                                listViewDiskInfo.Items.Add(lvi);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogError("Unexpected error performing partition or logical drive associations: " + ex.Message);
                    SiAuto.Main.LogException(ex);
                }
            }

            SiAuto.Main.LogMessage("Done displaying disk data.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.PopulateDiskInfo");
        }

        /// <summary>
        /// Populate the SMART info into the view for the user to see.
        /// </summary>
        /// <param name="drive"></param>
        private void PopulateSmartInfo(ManagementObject drive)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.PopulateSmartInfo");
            SiAuto.Main.LogMessage("Clear listViewSmartDetals.");
            listViewSmartDetails.Items.Clear();
            if (drive == null)
            {
                SiAuto.Main.LogMessage("Object drive is null; this disk is a phantom, so no action required.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.PopulateDiskInfo");
                return;
            }
            SiAuto.Main.LogMessage("Reload configuration to pick up any changes.");
            LoadDataFromRegistry();

            SiAuto.Main.LogMessage("Populate smartImageList for FancyListView.");
            ImageList smartImageList = new ImageList();
            smartImageList.Images.Add("Healthy", Properties.Resources.Healthy16);
            smartImageList.Images.Add("Warning", Properties.Resources.Warning16);
            smartImageList.Images.Add("Geriatric", Properties.Resources.Geriatric16);
            smartImageList.Images.Add("Fail", Properties.Resources.Critical16);
            smartImageList.Images.Add("SuperCritical", Properties.Resources.SuperCriticalAttribute);
            smartImageList.Images.Add("Blank", Properties.Resources.icon_blank_16);
            listViewSmartDetails.SmallImageList = smartImageList;

            SiAuto.Main.LogMessage("Get SMART data from the Registry.");
            Byte[] smartData;
            Byte[] smartThreshold;

            bool isSmartAvailable = GetSmartDataFromRegistry(drive, out smartData, out smartThreshold);

            if (smartData == null)
            {
                isSmartAvailable = false;
            }
            SiAuto.Main.LogMessage("SMART data retrieved.");

            if (isSmartAvailable && smartData[2] == 0 && smartData[14] == 0 &&
                smartData[26] == 0 && smartData[38] == 0)
            {
                // This is an unknown disk with all zeros, so we can say SMART is NOT available.
                SiAuto.Main.LogWarning("SMART data returned was all zeros.");
                isSmartAvailable = false;
            }

            SiAuto.Main.LogBool("isSmartAvailable", isSmartAvailable);
            if (!isSmartAvailable)
            {
                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Warning] SMART not available for the selected disk.");
            }

            if (isSmartAvailable)
            {
                SiAuto.Main.LogMessage("SMART is available; processing the disk.");
                isUnknown = false;

                // Let's harvest the harvestable data.
                // SMART data is 512 bytes, and a drive can have up to 30 attributes defined.
                // We skip bytes 0-1; the attributes then are 12 bytes apiece.
                //
                // First attribute is bytes 2-13.
                // Second attribute is bytes 14-25.
                // And so on...
                int threshold = 0;
                int smartValue = 0;
                int worst = 0;
                String binaryFlags = String.Empty;
                String rawData = String.Empty;
                String attributeName = String.Empty;
                String description = String.Empty;
                int attributeID = 0;
                String hexAttributeID = String.Empty;
                int flags;
                bool isCritical = false;
                bool isMajorHealthFlag = false;

                // Raw data is given in the 7th through 12th bytes of each attribute.
                // Low order is first byte, ascending to the last byte, high order.
                // For display to the user we have to reverse 'em so that they make sense.
                // If SMART says 0123456789AB, we need to reverse it to BA9876543210.
                int rawLow, raw1, raw2, raw3, raw4, rawHigh;
                rawLow = raw1 = raw2 = raw3 = raw4 = rawHigh = 0;

                for (int i = 0; i < 30; i++)
                {
                    int offset = 12 * i;

                    // Attribute ID: 2
                    attributeID = smartData[offset + 2];
                    if (attributeID == 0)
                    {
                        // Done!
                        SiAuto.Main.LogColored(System.Drawing.Color.Chartreuse, "attributeID == 0; we will skip this item");
                        continue;
                    }
                    hexAttributeID = (attributeID.ToString("X").Length == 1 ?
                        "0" + attributeID.ToString("X") : attributeID.ToString("X"));

                    flags = smartData[offset + 3];
                    binaryFlags = Utilities.Utility.NormalizeBinaryFlags(Convert.ToString(flags, 2)); // converts flags to the binary string
                    threshold = smartThreshold[offset + 3];
                    smartValue = smartData[offset + 5];
                    worst = smartData[offset + 6];
                    rawLow = smartData[offset + 7];
                    raw1 = smartData[offset + 8];
                    raw2 = smartData[offset + 9];
                    raw3 = smartData[offset + 10];
                    raw4 = smartData[offset + 11];
                    rawHigh = smartData[offset + 12];
                    rawData = ConcatenateRawData(rawHigh, raw4, raw3, raw2, raw1, rawLow);
                    if (binaryFlags.Substring(7, 1) == "1")
                    {
                        isCritical = true;
                    }
                    else
                    {
                        isCritical = false;
                    }

                    DataRow[] smartRows;

                    if (isSsd)
                    {
                        switch (ssdControllerManufacturer.ToUpper())
                        {
                            case "INDILINX EVEREST":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining Indilinx Everest SSD");
                                    smartRows = definitions.SsdEverestDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "INDILINX BAREFOOT 3":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining Indilinx Barefoot 3 SSD");
                                    smartRows = definitions.SsdBarefoot3Definitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "INDILINX":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining Indilinx SSD");
                                    smartRows = definitions.SsdIndilinxDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "INTEL":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining Intel SSD");
                                    smartRows = definitions.SsdIntelDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "TOSHIBA":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining Toshiba SSD");
                                    smartRows = definitions.SsdToshibaDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "JMICRON":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining JMicron SSD");
                                    smartRows = definitions.SsdJMicronDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "LAMD":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining LAMD SSD");
                                    smartRows = definitions.SsdLamdDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "MARVELL":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining Marvell SSD");
                                    smartRows = definitions.SsdMarvellDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "MICRON":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining Micron SSD");
                                    smartRows = definitions.SsdMicronDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "SAMSUNG":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining Samsung SSD");
                                    smartRows = definitions.SsdSamsungDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "SANDFORCE":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining SandForce SSD");
                                    smartRows = definitions.SsdSandForceDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "SMART MODULAR":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining SMART Modular SSD");
                                    smartRows = definitions.SsdSmartModularDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "STEC":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining STEC SSD");
                                    smartRows = definitions.SsdStecDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "KINGSTON":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining Kingston SSD");
                                    smartRows = definitions.SsdKingstonDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "TRANSCEND/SILICON MOTION":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining Transcend/Silicon Motion SSD");
                                    smartRows = definitions.SsdTranscendDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "KINGSPEC":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining KingSpec SSD");
                                    smartRows = definitions.SsdKingSpecDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "SMARTBUY/PHISON":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining Smartbuy/Phison SSD");
                                    smartRows = definitions.SsdSmartbuyDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "ADATA":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining ADATA SSD");
                                    smartRows = definitions.SsdAdataDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "SANDISK":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining SanDisk SSD");
                                    smartRows = definitions.SsdSanDiskDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            case "SK HYNIX":
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining SK hynix SSD");
                                    smartRows = definitions.SsdSkHynixDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                            default:
                                {
                                    SiAuto.Main.LogColored(System.Drawing.Color.LavenderBlush, "[Disk Health Check] Defining Generic/Unknown SSD");
                                    smartRows = definitions.SsdGenericDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                                    break;
                                }
                        }

                        if (smartRows != null && smartRows.Length > 0)
                        {
                            DataRow smartRow = smartRows[0];
                            attributeName = smartRow["AttributeName"].ToString();
                            isMajorHealthFlag = (bool)smartRow["IsCritical"];
                            description = smartRow["Description"].ToString();
                        }
                        else
                        {
                            attributeName = Properties.Resources.SmartAttributeVendorSpecific;
                            description = Properties.Resources.SmartAttributeVendorSpecific;
                            isMajorHealthFlag = false;
                        }
                        SiAuto.Main.LogInt("attributeID", attributeID);
                        SiAuto.Main.LogString("attributeName", attributeName);
                        SiAuto.Main.LogBool("isMajorHealthFlag", isMajorHealthFlag);

                        PopulateSmartInfoSsd(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                    }
                    else
                    {
                        smartRows = definitions.HddDefinitions.Definitions.Select("Key=" + attributeID.ToString());
                        if (smartRows != null && smartRows.Length > 0)
                        {
                            DataRow smartRow = smartRows[0];
                            attributeName = smartRow["AttributeName"].ToString();
                            isMajorHealthFlag = (bool)smartRow["IsCritical"];
                            description = smartRow["Description"].ToString();
                        }
                        else
                        {
                            attributeName = Properties.Resources.SmartAttributeVendorSpecific;
                            description = Properties.Resources.SmartAttributeVendorSpecific;
                            isMajorHealthFlag = false;
                        }
                        SiAuto.Main.LogInt("attributeID", attributeID);
                        SiAuto.Main.LogString("attributeName", attributeName);
                        SiAuto.Main.LogBool("isMajorHealthFlag", isMajorHealthFlag);
                        PopulateSmartInfoHdd(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                    }
                }
                PostProcessAndUpdateDiskHealth();
            }
            else
            {
                SiAuto.Main.LogMessage("Updating UI for disk with no available SMART data.");
                labelBadSectors.Text = Properties.Resources.LabelBadSectors + Properties.Resources.ItemNotApplicable;
                labelPendingSectors.Text = Properties.Resources.LabelPendingSectors + Properties.Resources.ItemNotApplicable;
                labelReallocationEvents.Text = Properties.Resources.LabelReallocationEvents + Properties.Resources.ItemNotApplicable;
                labelTemperature.Text = Properties.Resources.LabelTemperature + Properties.Resources.ItemNotApplicable;

                labelBadSectors.Image = Properties.Resources.Warning16;
                labelPendingSectors.Image = Properties.Resources.Warning16;
                labelReallocationEvents.Image = Properties.Resources.Warning16;
                labelTemperature.Image = Properties.Resources.Warning16;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.PopulateSmartInfo");
        }

        private void PopulateSmartInfoHdd(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.PopulateSmartInfoHdd");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if ((attributeID == 5 || attributeID == 10 || attributeID == 184 || attributeID == 196 ||
                    attributeID == 197 || attributeID == 198) && rawData.Substring(8) != "0000")
                {
                    // Bad Sectors, End-to-End Errors, Pending Bad Sectors, Reallocations, Uncorrectable Sectors, Spin Retries
                    // These are the most serious and most fatal of all attributes so watch 'em closely.  Even one
                    // of these is a bad thing. Spin Retry too. Doesn't matter if the threshold is zero.
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogWarning("[HDD Check] Disk health Degraded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    imageSubItem.ImageKey = "Geriatric";
                    isDiskGeriatric = true;
                    SiAuto.Main.LogWarning("[HDD Check] Disk health Geriatric on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                    SiAuto.Main.LogMessage("[HDD Check] Disk health Healthy on attribute " + attributeID.ToString());
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else if ((attributeID == 5 || attributeID == 10 || attributeID == 184 || attributeID == 196 ||
                    attributeID == 197 || attributeID == 198) && rawData.Substring(8) != "0000")
                {
                    // Bad Sectors, End-to-End Errors, Pending Bad Sectors, Reallocations, Uncorrectable Sectors, Spin Retries
                    // These are the most serious and most fatal of all attributes so watch 'em closely.  Even one
                    // of these is a bad thing. Spin Retry too.
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogWarning("[HDD Check] Disk health Degraded on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                    SiAuto.Main.LogMessage("[HDD Check] Disk health Healthy on attribute " + attributeID.ToString());
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                // Bad sectors use only the lowest 4 bytes; likewise with reallocations, pending, etc.
                // Including 190 attribute for airflow temperature too.
                if (attributeID == 194 || attributeID == 231 || attributeID == 190)
                {
                    // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogLong("counter (value)", counter);
                }

                if (attributeID == 5 || attributeID == 9 || attributeID == 184 || attributeID == 196 || attributeID == 197 || attributeID == 198 || attributeID == 199)
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogLong("counter (value)", counter);
                }

                switch (attributeID)
                {
                    case 5: // Bad Sectors
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            SiAuto.Main.LogInt("badSectorCountOrRetiredBlockCount", badSectorCountOrRetiredBlockCount);
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 10: // Spin Retry
                        {
                            spinRetryCount = (int)counter;
                            SiAuto.Main.LogInt("spinRetryCount", spinRetryCount);
                            break;
                        }
                    case 11: // Recalibration Retry
                        {
                            recalibrationRetryCount = (int)counter;
                            SiAuto.Main.LogInt("recalibrationRetryCount", recalibrationRetryCount);
                            break;
                        }
                    case 184: // End-to-End Error
                        {
                            endToEndErrorCountOrLifeCurve = (int)counter;
                            SiAuto.Main.LogInt("endToEndErrorCountOrLifeCurve", endToEndErrorCountOrLifeCurve);
                            break;
                        }
                    case 196: // Reallocation Event Count
                        {
                            reallocationEventCount = (int)counter;
                            SiAuto.Main.LogInt("reallocationEventCount", reallocationEventCount);
                            break;
                        }
                    case 197: // Pending Sectors
                        {
                            pendingSectorCountOrSsdLifeLeft = (int)counter;
                            SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            break;
                        }
                    case 198: // Uncorrectable Sectors
                        {
                            uncorrectableSectorCount = (int)counter;
                            SiAuto.Main.LogInt("uncorrectableSectorCount", uncorrectableSectorCount);
                            break;
                        }
                    case 199: // CRC errors (not as serious as the ones above)
                        {
                            ultraAtaCrcErrorCount = (int)counter;
                            SiAuto.Main.LogInt("ultraAtaCrcErrorCount", ultraAtaCrcErrorCount);
                            if (ultraAtaCrcErrorCount > 50)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    case 190:
                        {
                            airflowTemperature = (int)counter;
                            fAirflowTemperature = (airflowTemperature * 9 / 5) + 32;
                            kAirflowTemperature = fAirflowTemperature + 273;

                            SiAuto.Main.LogInt("airflowTemperature", airflowTemperature);

                            if (airflowTemperature > absurdTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                isDiskTemperatureInvalid = true;
                            }
                            else if (airflowTemperature >= criticalTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Critical Airflow Temperature - " + airflowTemperature.ToString() + " degrees; critical threshold " + criticalTemperatureThreshold.ToString());
                                isDiskCritical = true;
                            }
                            else if (airflowTemperature >= overheatedTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Airflow Temperature - " + airflowTemperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (airflowTemperature >= hotTemperatureThreshold && !ignoreHot)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Airflow Temperature - " + airflowTemperature.ToString() + " degrees; hot threshold " + hotTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (airflowTemperature >= warmTemperatureThreshold && !ignoreWarm)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Airflow Temperature - " + airflowTemperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    case 194:
                    case 231: // C2 and E7 are both temperature
                        {
                            if (temperature == 0) // If both are defined we'll read just one.
                            {
                                temperature = (int)counter;
                                fTemperature = (temperature * 9 / 5) + 32;
                                kTemperature = temperature + 273;
                                SiAuto.Main.LogInt("temperature", temperature);

                                if (temperature > absurdTemperatureThreshold)
                                {
                                    imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                    imageSubItem.ImageKey = "Fail";
                                    SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                        "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                    isDiskTemperatureInvalid = true;
                                }
                                else if (temperature >= criticalTemperatureThreshold)
                                {
                                    imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                    imageSubItem.ImageKey = "Fail";
                                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Critical Temperature - " + temperature.ToString() + " degrees; critical threshold " + criticalTemperatureThreshold.ToString());
                                    isDiskCritical = true;
                                }
                                else if (temperature >= overheatedTemperatureThreshold)
                                {
                                    imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                    imageSubItem.ImageKey = "Fail";
                                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                    isDiskWarning = true;
                                }
                                else if (temperature >= hotTemperatureThreshold && !ignoreHot)
                                {
                                    imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                    imageSubItem.ImageKey = "Warning";
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Temperature - " + temperature.ToString() + " degrees; hot threshold " + hotTemperatureThreshold.ToString());
                                    isDiskWarning = true;
                                }
                                else if (temperature >= warmTemperatureThreshold && !ignoreWarm)
                                {
                                    imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Temperature - " + temperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                    imageSubItem.ImageKey = "Warning";
                                }
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoHdd");
        }

        private void PopulateSmartInfoSsd(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsd");
            SiAuto.Main.LogString("ssdControllerManufacturer", ssdControllerManufacturer);
            SiAuto.Main.LogString("attributeName", attributeName);
            SiAuto.Main.LogInt("attributeID", attributeID);
            switch (ssdControllerManufacturer.ToUpper())
            {
                case "INDILINX EVEREST":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for Indilinx Everest");
                        PopulateSmartInfoSsdEverest(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                case "INDILINX BAREFOOT 3":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for Indilinx Barefoot 3");
                        PopulateSmartInfoSsdBarefoot3(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                case "INDILINX":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for Indilinx");
                        PopulateSmartInfoSsdIndilinx(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                case "INTEL":
                case "STEC":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for " + (ssdControllerManufacturer.ToUpper() == "INTEL" ? "Intel" : "STEC"));
                        // Intel and STEC share many common attributes so we'll use a single method and sort them out inside.
                        PopulateSmartInfoSsdIntelStec(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData, (ssdControllerManufacturer.ToUpper() == "INTEL"));
                        break;
                    }
                case "LAMD":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for LAMD");
                        PopulateSmartInfoSsdLamd(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                case "SANDFORCE":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for SandForce");
                        PopulateSmartInfoSsdSandForce(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                case "TOSHIBA":
                case "JMICRON":
                    {
                        // Like Intel and STEC, Toshiba and JMicron share many common attributes so we sort them out inside a single method.
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for " + (ssdControllerManufacturer.ToUpper() == "TOSHIBA" ? "Toshiba" : "JMicron"));
                        PopulateSmartInfoSsdToshibaJMicron(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData, (ssdControllerManufacturer.ToUpper() == "JMICRON"));
                        break;
                    }
                case "MICRON":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for Micron");
                        PopulateSmartInfoSsdMicron(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                case "MARVELL":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for Marvell");
                        PopulateSmartInfoSsdMarvell(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                case "SAMSUNG":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for Samsung");
                        PopulateSmartInfoSsdSamsung(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                case "SMART MODULAR":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for SMART Modular");
                        PopulateSmartInfoSsdSmartModular(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                case "KINGSTON":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for Kingston");
                        PopulateSmartInfoSsdSmartModular(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                case "TRANSCEND/SILICON MOTION":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for Transcend/Silicon Motion");
                        PopulateSmartInfoSsdTranscend(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                case "KINGSPEC":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for KingSpec");
                        PopulateSmartInfoSsdKingSpec(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                case "SMARTBUY/PHISON":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for Smartbuy/Phison");
                        PopulateSmartInfoSsdSmartbuy(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                case "ADATA":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for ADATA");
                        PopulateSmartInfoSsdSmartModular(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                case "SANDISK":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for SanDisk");
                        PopulateSmartInfoSsdSanDisk(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                case "SK HYNIX":
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for SK hynix");
                        PopulateSmartInfoSsdSkHynix(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
                default:
                    {
                        SiAuto.Main.LogMessage("[SSD Check] Performing check for Generic/Unknown/Other");
                        PopulateSmartInfoSsdSamsung(attributeName, isMajorHealthFlag, isCritical, description, binaryFlags, attributeID, hexAttributeID,
                            smartValue, threshold, worst, rawData);
                        break;
                    }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsd");
        }

        private void PopulateSmartInfoSsdEverest(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdEverest");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Retired Sectors
            if ((attributeID == 5) && rawData.Substring(8) != "0000")
            {
                // Retired Block Count
                // These are serious as they indicate drive wear, but aren't necessarily as fatal on an SSD. We'll
                // allow 50 of each before running up the flag, and we go critical at 150.
                int eventCount = 0;
                try
                {
                    SiAuto.Main.LogMessage("Checking retired sector count.");
                    eventCount = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                catch { }

                if (eventCount >= ssdRetirementCritical)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Event count meets or exceeds critical threshold. Setting disk to Critical.");
                    SiAuto.Main.LogInt("ssdRetirementCountCritical", ssdRetirementCritical);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                else if (eventCount >= ssdRetirementWarning)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] Event count meets or exceeds warning threshold. Setting disk to Warning.");
                    SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                // Temperature for SandForce is attribute 194.
                if (attributeID == 194)
                {
                    // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                }

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                if (attributeID == 5 || attributeID == 9) // Retired blocks, Power-On Hours
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 5: // Retired Blocks
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 194:
                        {
                            temperature = (int)counter;
                            fTemperature = (temperature * 9 / 5) + 32;
                            kTemperature = temperature + 273;

                            if (temperature > absurdTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                isDiskTemperatureInvalid = true;
                            }
                            else if (temperature >= criticalTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Critical Temperature - " + temperature.ToString() + " degrees; overheated threshold " + criticalTemperatureThreshold.ToString());
                                isDiskCritical = true;
                            }
                            else if (temperature >= overheatedTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= hotTemperatureThreshold && !ignoreHot)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Temperature - " + temperature.ToString() + " degrees; warm threshold " + hotTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Temperature - " + temperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    case 233: // % Life remaining
                        {
                            // % of life remaining
                            pendingSectorCountOrSsdLifeLeft = smartValue;
                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdEverest");
        }

        private void PopulateSmartInfoSsdBarefoot3(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdBarefoot3");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Retired Sectors
            if ((attributeID == 5) && rawData.Substring(8) != "0000")
            {
                // Retired Block Count
                // These are serious as they indicate drive wear, but aren't necessarily as fatal on an SSD. We'll
                // allow 50 of each before running up the flag, and we go critical at 150.
                int eventCount = 0;
                try
                {
                    SiAuto.Main.LogMessage("Checking retired sector count.");
                    eventCount = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                catch { }

                if (eventCount >= ssdRetirementCritical)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Event count meets or exceeds critical threshold. Setting disk to Critical.");
                    SiAuto.Main.LogInt("ssdRetirementCountCritical", ssdRetirementCritical);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                else if (eventCount >= ssdRetirementWarning)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] Event count meets or exceeds warning threshold. Setting disk to Warning.");
                    SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                // Temperature for SandForce is attribute 194.
                if (attributeID == 194)
                {
                    // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                }

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                if (attributeID == 5 || attributeID == 9) // Retired blocks, Power-On Hours
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 5: // Retired Blocks
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 194: // temperature
                        {
                            temperature = (int)counter;
                            fTemperature = (temperature * 9 / 5) + 32;
                            kTemperature = temperature + 273;

                            if (temperature > absurdTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                isDiskTemperatureInvalid = true;
                            }
                            else if (temperature >= criticalTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Critical Temperature - " + temperature.ToString() + " degrees; overheated threshold " + criticalTemperatureThreshold.ToString());
                                isDiskCritical = true;
                            }
                            else if (temperature >= overheatedTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= hotTemperatureThreshold && !ignoreHot)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Temperature - " + temperature.ToString() + " degrees; warm threshold " + hotTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Temperature - " + temperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    case 233: // % Life remaining
                        {
                            // % of life remaining
                            pendingSectorCountOrSsdLifeLeft = smartValue;
                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdBarefoot3");
        }

        private void PopulateSmartInfoSsdIndilinx(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdIndilinx");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                if (attributeID == 9)
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 209: // % Life remaining
                        {
                            // % of life remaining
                            pendingSectorCountOrSsdLifeLeft = smartValue;
                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdIndilinx");
        }

        private void PopulateSmartInfoSsdToshibaJMicron(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData, bool isJMicron)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoToshibaJMicron");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Retired sectors and sectors pending retirement.
            if ((attributeID == 5 || attributeID == 197) && rawData.Substring(8) != "0000")
            {
                // Retired Block Count and Pending Sectorss
                // These are serious as they indicate drive wear, but aren't necessarily as fatal on an SSD. We'll
                // allow 50 of each before running up the flag, and we go critical at 150.
                int eventCount = 0;
                try
                {
                    eventCount = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }
                catch { }

                if (eventCount >= ssdRetirementCritical)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Event count meets or exceeds critical threshold. Setting disk to Critical.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementCountCritical", ssdRetirementCritical);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                else if (eventCount >= ssdRetirementWarning)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] Event count meets or exceeds warning threshold. Setting disk to Warning.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                // Temperature for SandForce is attribute 194.
                if (attributeID == 194)
                {
                    // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                }

                if (attributeID == 5 || attributeID == 9 || attributeID == 175 ||  attributeID == 197)
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 5: // Retired Blocks
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 173: // Media Wearout Indicator
                        {
                            // % of life remaining
                            if (!isJMicron)
                            {
                                // Toshiba SMART value starts at 200 so divide by 2.
                                pendingSectorCountOrSsdLifeLeft = smartValue / 2;
                                if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                {
                                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                                    imageSubItem.ImageKey = "Fail";
                                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                    SiAuto.Main.LogInt("smartValue", smartValue);
                                    SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                                }
                                else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                {
                                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                    imageSubItem.ImageKey = "Warning";
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                    SiAuto.Main.LogInt("smartValue", smartValue);
                                    SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                                }
                            }
                            break;
                        }
                    case 175: // Bad Cluster Table Count (ECC Fail Count)
                        {
                            if (isJMicron)
                            {
                                reallocationEventCount = (int)counter;
                            }
                            break;
                        }
                    case 197: // Reallocation Event Count
                        {
                            if (isJMicron)
                            {
                                pendingSectorCountOrSsdLifeLeft = (int)counter;
                            }
                            break;
                        }
                    case 194:
                        {
                            temperature = (int)counter;
                            fTemperature = (temperature * 9 / 5) + 32;
                            kTemperature = temperature + 273;

                            if (temperature > absurdTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                isDiskTemperatureInvalid = true;
                            }
                            else if (temperature >= criticalTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Critical Temperature - " + temperature.ToString() + " degrees; overheated threshold " + criticalTemperatureThreshold.ToString());
                                isDiskCritical = true;
                            }
                            else if (temperature >= overheatedTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= hotTemperatureThreshold && !ignoreHot)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Temperature - " + temperature.ToString() + " degrees; warm threshold " + hotTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Temperature - " + temperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoToshibaJMicron");
        }

        private void PopulateSmartInfoSsdTranscend(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoTranscend");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Retired sectors.
            if ((attributeID == 5) && rawData.Substring(8) != "0000")
            {
                // Retired Block Count
                // These are serious as they indicate drive wear, but aren't necessarily as fatal on an SSD. We'll
                // allow 50 of each before running up the flag, and we go critical at 150.
                int eventCount = 0;
                try
                {
                    eventCount = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }
                catch { }

                if (eventCount >= ssdRetirementCritical)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Event count meets or exceeds critical threshold. Setting disk to Critical.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementCountCritical", ssdRetirementCritical);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                else if (eventCount >= ssdRetirementWarning)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] Event count meets or exceeds warning threshold. Setting disk to Warning.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                // Temperature for SandForce is attribute 194.
                if (attributeID == 194)
                {
                    // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                }

                if (attributeID == 5 || attributeID == 9)
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 5: // Retired Blocks
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 169: // Media Wearout Indicator
                        {
                            // % of life remaining
                            pendingSectorCountOrSsdLifeLeft = smartValue;
                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            break;
                        }
                    case 194:
                        {
                            temperature = (int)counter;
                            fTemperature = (temperature * 9 / 5) + 32;
                            kTemperature = temperature + 273;

                            if (temperature > absurdTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                isDiskTemperatureInvalid = true;
                            }
                            else if (temperature >= criticalTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Critical Temperature - " + temperature.ToString() + " degrees; overheated threshold " + criticalTemperatureThreshold.ToString());
                                isDiskCritical = true;
                            }
                            else if (temperature >= overheatedTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= hotTemperatureThreshold && !ignoreHot)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Temperature - " + temperature.ToString() + " degrees; warm threshold " + hotTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Temperature - " + temperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoTranscend");
        }

        private void PopulateSmartInfoSsdSandForce(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdSandForce");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Retired sectors and reallocations.
            if ((attributeID == 5 || attributeID == 196) && rawData.Substring(8) != "0000")
            {
                // Retired Block Count and Reallocation Events
                // These are serious as they indicate drive wear, but aren't necessarily as fatal on an SSD. We'll
                // allow 50 of each before running up the flag, and we go critical at 150.
                int eventCount = 0;
                try
                {
                    eventCount = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }
                catch { }

                if (eventCount >= ssdRetirementCritical)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Event count meets or exceeds critical threshold. Setting disk to Critical.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementCountCritical", ssdRetirementCritical);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                else if (eventCount >= ssdRetirementWarning)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] Event count meets or exceeds warning threshold. Setting disk to Warning.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                // Temperature for SandForce is attribute 194.
                if (attributeID == 194)
                {
                    // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                }

                if (attributeID == 5 || attributeID == 9 || attributeID == 196)
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 5: // Retired Blocks
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 196: // Reallocation Event Count
                        {
                            reallocationEventCount = (int)counter;
                            break;
                        }
                    case 231: // % Life remaining
                        {
                            // % of life remaining
                            pendingSectorCountOrSsdLifeLeft = smartValue;
                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            break;
                        }
                    case 230: // Lie Curve
                        {
                            // 100 = good; 90 or less = throttled
                            endToEndErrorCountOrLifeCurve = smartValue;
                            SiAuto.Main.LogMessage("[SSD Check] Life Curve Status: " + endToEndErrorCountOrLifeCurve.ToString());
                            if (endToEndErrorCountOrLifeCurve <= 90)
                            {
                                imageSubItem.Text = "Throttled";
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD Life Curve indicates excessive writes; performance is throttled.");
                            }
                            break;
                        }
                    case 194:
                        {
                            temperature = (int)counter;
                            fTemperature = (temperature * 9 / 5) + 32;
                            kTemperature = temperature + 273;

                            if (temperature > absurdTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                isDiskTemperatureInvalid = true;
                            }
                            else if (temperature >= criticalTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Critical Temperature - " + temperature.ToString() + " degrees; overheated threshold " + criticalTemperatureThreshold.ToString());
                                isDiskCritical = true;
                            }
                            else if (temperature >= overheatedTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= hotTemperatureThreshold && !ignoreHot)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Temperature - " + temperature.ToString() + " degrees; warm threshold " + hotTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Temperature - " + temperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdSandForce");
        }

        private void PopulateSmartInfoSsdLamd(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdLamd");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Retired sectors and reallocations.
            if ((attributeID == 5) && rawData.Substring(8) != "0000")
            {
                // Retired Block Count and Reallocation Events
                // These are serious as they indicate drive wear, but aren't necessarily as fatal on an SSD. We'll
                // allow 50 of each before running up the flag, and we go critical at 150.
                int eventCount = 0;
                try
                {
                    eventCount = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }
                catch { }

                if (eventCount >= ssdRetirementCritical)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Event count meets or exceeds critical threshold. Setting disk to Critical.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementCountCritical", ssdRetirementCritical);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                else if (eventCount >= ssdRetirementWarning)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] Event count meets or exceeds warning threshold. Setting disk to Warning.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                // Temperature for SandForce is attribute 194.
                if (attributeID == 194)
                {
                    // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                }

                if (attributeID == 5 || attributeID == 9)
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 5: // Retired Blocks
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 231: // % Life remaining
                        {
                            // % of life remaining
                            pendingSectorCountOrSsdLifeLeft = smartValue;
                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            break;
                        }
                    case 194:
                        {
                            temperature = (int)counter;
                            fTemperature = (temperature * 9 / 5) + 32;
                            kTemperature = temperature + 273;

                            if (temperature > absurdTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                isDiskTemperatureInvalid = true;
                            }
                            else if (temperature >= criticalTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Critical Temperature - " + temperature.ToString() + " degrees; overheated threshold " + criticalTemperatureThreshold.ToString());
                                isDiskCritical = true;
                            }
                            else if (temperature >= overheatedTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= hotTemperatureThreshold && !ignoreHot)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Temperature - " + temperature.ToString() + " degrees; warm threshold " + hotTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Temperature - " + temperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdLamd");
        }

        private void PopulateSmartInfoSsdMarvell(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdMarvell");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Retired sectors and reallocations.
            if ((attributeID == 5) && rawData.Substring(8) != "0000")
            {
                // Retired Block Count and Reallocation Events
                // These are serious as they indicate drive wear, but aren't necessarily as fatal on an SSD. We'll
                // allow 50 of each before running up the flag, and we go critical at 150.
                int eventCount = 0;
                try
                {
                    eventCount = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }
                catch { }

                if (eventCount >= ssdRetirementCritical)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Event count meets or exceeds critical threshold. Setting disk to Critical.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementCountCritical", ssdRetirementCritical);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                else if (eventCount >= ssdRetirementWarning)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] Event count meets or exceeds warning threshold. Setting disk to Warning.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // Marvell has no temperature value (maybe no sensor?)

                if (attributeID == 5 || attributeID == 9 || attributeID == 196)
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 5: // Retired Blocks
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 196: // Reallocation Event Count
                        {
                            reallocationEventCount = (int)counter;
                            break;
                        }
                    case 177: // % Life remaining (wear leveling)
                        {
                            // % of life remaining
                            pendingSectorCountOrSsdLifeLeft = smartValue;
                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdMarvell");
        }

        private void PopulateSmartInfoSsdSamsung(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdSamsung");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Retired sectors and reallocations.
            if ((attributeID == 5) && rawData.Substring(8) != "0000")
            {
                // Retired Block Count and Reallocation Events
                // These are serious as they indicate drive wear, but aren't necessarily as fatal on an SSD. We'll
                // allow 50 of each before running up the flag, and we go critical at 150.
                int eventCount = 0;
                try
                {
                    eventCount = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }
                catch { }

                if (eventCount >= ssdRetirementCritical)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Event count meets or exceeds critical threshold. Setting disk to Critical.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementCountCritical", ssdRetirementCritical);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                else if (eventCount >= ssdRetirementWarning)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] Event count meets or exceeds warning threshold. Setting disk to Warning.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                // Temperature for SandForce is attribute 194.
                if (attributeID == 190)
                {
                    // Attributes 190 is temperature on Samsung. Some manufacturers use a lot of the raw data field
                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                }

                if (attributeID == 5 || attributeID == 9 || attributeID == 179 || attributeID == 183)
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 5: // Retired Blocks
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 179: // Reserve Blocks Used
                        {
                            reallocationEventCount = (int)counter;
                            break;
                        }
                    case 183: // Runtime Bad Blocks
                        {
                            endToEndErrorCountOrLifeCurve = (int)counter;
                            break;
                        }
                    case 177: // % Life remaining
                        {
                            // % of life remaining
                            pendingSectorCountOrSsdLifeLeft = smartValue;
                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            break;
                        }
                    case 190:
                        {
                            temperature = (int)counter;
                            fTemperature = (temperature * 9 / 5) + 32;
                            kTemperature = temperature + 273;

                            if (temperature > absurdTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                isDiskTemperatureInvalid = true;
                            }
                            else if (temperature >= criticalTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Critical Temperature - " + temperature.ToString() + " degrees; overheated threshold " + criticalTemperatureThreshold.ToString());
                                isDiskCritical = true;
                            }
                            else if (temperature >= overheatedTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= hotTemperatureThreshold && !ignoreHot)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Temperature - " + temperature.ToString() + " degrees; warm threshold " + hotTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Temperature - " + temperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdSamsung");
        }

        private void PopulateSmartInfoSsdSmartModular(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdSmartModular");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Retired sectors.
            if ((attributeID == 252) && rawData.Substring(8) != "0000")
            {
                // Retired Block Count
                // These are serious as they indicate drive wear, but aren't necessarily as fatal on an SSD. We'll
                // allow 50 of each before running up the flag, and we go critical at 150.
                int eventCount = 0;
                try
                {
                    eventCount = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }
                catch { }

                if (eventCount >= ssdRetirementCritical)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Event count meets or exceeds critical threshold. Setting disk to Critical.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementCountCritical", ssdRetirementCritical);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                else if (eventCount >= ssdRetirementWarning)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] Event count meets or exceeds warning threshold. Setting disk to Warning.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                // Temperature for SandForce is attribute 194.
                if (attributeID == 194)
                {
                    // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                }

                if (attributeID == 252 || attributeID == 9)
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 252: // Retired Blocks
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 232: // % Life remaining
                        {
                            // % of life remaining
                            pendingSectorCountOrSsdLifeLeft = smartValue;
                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            break;
                        }
                    case 194:
                        {
                            temperature = (int)counter;
                            fTemperature = (temperature * 9 / 5) + 32;
                            kTemperature = temperature + 273;

                            if (temperature > absurdTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                isDiskTemperatureInvalid = true;
                            }
                            else if (temperature >= criticalTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Critical Temperature - " + temperature.ToString() + " degrees; overheated threshold " + criticalTemperatureThreshold.ToString());
                                isDiskCritical = true;
                            }
                            else if (temperature >= overheatedTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= hotTemperatureThreshold && !ignoreHot)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Temperature - " + temperature.ToString() + " degrees; warm threshold " + hotTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Temperature - " + temperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdSmartModular");
        }

        private void PopulateSmartInfoSsdMicron(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdMicron");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Retired sectors and reallocations.
            if ((attributeID == 5 || attributeID == 196) && rawData.Substring(8) != "0000")
            {
                // Retired Block Count and Reallocation Events
                // These are serious as they indicate drive wear, but aren't necessarily as fatal on an SSD. We'll
                // allow 50 of each before running up the flag, and we go critical at 150.
                int eventCount = 0;
                try
                {
                    eventCount = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }
                catch { }

                if (eventCount >= ssdRetirementCritical)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Event count meets or exceeds critical threshold. Setting disk to Critical.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementCountCritical", ssdRetirementCritical);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                else if (eventCount >= ssdRetirementWarning)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] Event count meets or exceeds warning threshold. Setting disk to Warning.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                // Temperature for SandForce is attribute 194.
                if (attributeID == 194)
                {
                    // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                }

                if (attributeID == 5 || attributeID == 9 || attributeID == 196)
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 5: // Retired Blocks
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 196: // Reallocation Event Count
                        {
                            reallocationEventCount = (int)counter;
                            break;
                        }
                    case 173: // Wear Leveling
                        {
                            endToEndErrorCountOrLifeCurve = (int)counter;
                            break;
                        }
                    case 202: // % Life remaining
                        {
                            // % of life remaining
                            pendingSectorCountOrSsdLifeLeft = smartValue;
                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            break;
                        }
                    case 194:
                        {
                            temperature = (int)counter;
                            fTemperature = (temperature * 9 / 5) + 32;
                            kTemperature = temperature + 273;

                            if (temperature > absurdTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                isDiskTemperatureInvalid = true;
                            }
                            else if (temperature >= criticalTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Critical Temperature - " + temperature.ToString() + " degrees; overheated threshold " + criticalTemperatureThreshold.ToString());
                                isDiskCritical = true;
                            }
                            else if (temperature >= overheatedTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= hotTemperatureThreshold && !ignoreHot)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Temperature - " + temperature.ToString() + " degrees; warm threshold " + hotTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Temperature - " + temperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdMicron");
        }

        private void PopulateSmartInfoSsdIntelStec(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
           int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData, bool isIntel)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdIntelStec");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Retired sectors, reallocations, etc...
            if ((attributeID == 5 || attributeID == 196 || attributeID == 197) && rawData.Substring(8) != "0000")
            {
                // Retired Block Count and Reallocation Events (latter applies to STEC only)
                // These are serious as they indicate drive wear, but aren't necessarily as fatal on an SSD. We'll
                // allow 50 of each before running up the flag, and we go critical at 150.
                int eventCount = 0;
                try
                {
                    eventCount = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }
                catch { }

                if (eventCount >= ssdRetirementCritical)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Event count meets or exceeds critical threshold. Setting disk to Critical.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementCountCritical", ssdRetirementCritical);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                else if (eventCount >= ssdRetirementWarning)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] Event count meets or exceeds warning threshold. Setting disk to Warning.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                // Temperature for SandForce is attribute 194.
                if (attributeID == 194)
                {
                    // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                }

                if (attributeID == 5 || attributeID == 9 || attributeID == 196 || attributeID == 197)
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 5: // Retired Blocks (both)
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 196: // Reallocation Event Count (STEC)
                        {
                            if (!isIntel)
                            {
                                reallocationEventCount = (int)counter;
                            }
                            break;
                        }
                    case 233: // % Life remaining (Intel)
                        {
                            // % of life remaining
                            if (isIntel)
                            {
                                pendingSectorCountOrSsdLifeLeft = smartValue;
                                if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                                {
                                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                                    imageSubItem.ImageKey = "Fail";
                                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                    SiAuto.Main.LogInt("smartValue", smartValue);
                                    SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                                }
                                else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                                {
                                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                    imageSubItem.ImageKey = "Warning";
                                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                    SiAuto.Main.LogInt("smartValue", smartValue);
                                    SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                                }
                            }
                            break;
                        }
                    case 184: // End-to-End Errors
                        {
                            endToEndErrorCountOrLifeCurve = (int)counter;
                            if (endToEndErrorCountOrLifeCurve > 5)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Fail";
                                isDiskCritical = true;
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Excessive end-to-end errors detected (greater than 5). Setting disk to Critical.");
                                SiAuto.Main.LogInt("endToEndErrorCountOrLifeCurve", endToEndErrorCountOrLifeCurve);
                            }
                            else if (endToEndErrorCountOrLifeCurve > 0)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                                isDiskWarning = true;
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] End-to-end errors detected. Setting disk to Warning.");
                                SiAuto.Main.LogInt("endToEndErrorCountOrLifeCurve", endToEndErrorCountOrLifeCurve);
                            }
                            else
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                                imageSubItem.ImageKey = "Healthy";
                            }
                            break;
                        }
                    case 194:
                        {
                            temperature = (int)counter;
                            fTemperature = (temperature * 9 / 5) + 32;
                            kTemperature = temperature + 273;

                            if (temperature > absurdTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                isDiskTemperatureInvalid = true;
                            }
                            else if (temperature >= criticalTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskCritical = true;
                            }
                            else if (temperature >= overheatedTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= hotTemperatureThreshold && !ignoreHot)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Temperature - " + temperature.ToString() + " degrees; warm threshold " + hotTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Temperature - " + temperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    case 197:
                        {
                            if (!isIntel)
                            {
                                pendingSectorCountOrSsdLifeLeft = (int)counter;
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoIntelStec");
        }

        private void PopulateSmartInfoSsdSmartbuy(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdSmartbuy");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Retired sectors.
            if ((attributeID == 5) && rawData.Substring(8) != "0000")
            {
                // Retired Block Count
                // These are serious as they indicate drive wear, but aren't necessarily as fatal on an SSD. We'll
                // allow 50 of each before running up the flag, and we go critical at 150.
                int eventCount = 0;
                try
                {
                    eventCount = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }
                catch { }

                if (eventCount >= ssdRetirementCritical)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Event count meets or exceeds critical threshold. Setting disk to Critical.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementCountCritical", ssdRetirementCritical);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                else if (eventCount >= ssdRetirementWarning)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] Event count meets or exceeds warning threshold. Setting disk to Warning.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                // Temperature for SandForce is attribute 194.
                if (attributeID == 194)
                {
                    // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                }

                if (attributeID == 5 || attributeID == 9)
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 5: // Retired Blocks (Smartbuy/Phison has no attribute 5 but we keep here for compatibility)
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 231: // % Life remaining
                        {
                            // % of life remaining
                            pendingSectorCountOrSsdLifeLeft = smartValue;
                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            break;
                        }
                    case 194:
                        {
                            temperature = (int)counter;
                            fTemperature = (temperature * 9 / 5) + 32;
                            kTemperature = temperature + 273;

                            if (temperature > absurdTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                isDiskTemperatureInvalid = true;
                            }
                            else if (temperature >= criticalTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Critical Temperature - " + temperature.ToString() + " degrees; overheated threshold " + criticalTemperatureThreshold.ToString());
                                isDiskCritical = true;
                            }
                            else if (temperature >= overheatedTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= hotTemperatureThreshold && !ignoreHot)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Temperature - " + temperature.ToString() + " degrees; warm threshold " + hotTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Temperature - " + temperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdSmartbuy");
        }

        private void PopulateSmartInfoSsdSanDisk(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdSanDisk");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Retired sectors.
            if ((attributeID == 5) && rawData.Substring(8) != "0000")
            {
                // Retired Block Count
                // These are serious as they indicate drive wear, but aren't necessarily as fatal on an SSD. We'll
                // allow 50 of each before running up the flag, and we go critical at 150.
                int eventCount = 0;
                try
                {
                    eventCount = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }
                catch { }

                if (eventCount >= ssdRetirementCritical)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Event count meets or exceeds critical threshold. Setting disk to Critical.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementCountCritical", ssdRetirementCritical);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                else if (eventCount >= ssdRetirementWarning)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] Event count meets or exceeds warning threshold. Setting disk to Warning.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                // Temperature for SandForce is attribute 194.
                if (attributeID == 194)
                {
                    // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                }

                if (attributeID == 5 || attributeID == 9)
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 5: // Retired Blocks (Smartbuy/Phison has no attribute 5 but we keep here for compatibility)
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 230: // % Life remaining - Media Wearout
                        {
                            // % of life remaining
                            // SanDisk has 230 and 232 so we take the worse of the two to calculate the health
                            int temp = smartValue;
                            if (pendingSectorCountOrSsdLifeLeft == 0 || (pendingSectorCountOrSsdLifeLeft > 0 && temp < pendingSectorCountOrSsdLifeLeft))
                            {
                                pendingSectorCountOrSsdLifeLeft = temp;
                            }
                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            break;
                        }
                    case 232: // % Life remaining - Available Reserved Space
                        {
                            // % of life remaining
                            // SanDisk has 230 and 232 so we take the worse of the two to calculate the health
                            int temp = smartValue;
                            if (pendingSectorCountOrSsdLifeLeft == 0 || (pendingSectorCountOrSsdLifeLeft > 0 && temp < pendingSectorCountOrSsdLifeLeft))
                            {
                                pendingSectorCountOrSsdLifeLeft = temp;
                            }
                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            break;
                        }
                    case 194:
                        {
                            temperature = (int)counter;
                            fTemperature = (temperature * 9 / 5) + 32;
                            kTemperature = temperature + 273;

                            if (temperature > absurdTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                isDiskTemperatureInvalid = true;
                            }
                            else if (temperature >= criticalTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Critical Temperature - " + temperature.ToString() + " degrees; overheated threshold " + criticalTemperatureThreshold.ToString());
                                isDiskCritical = true;
                            }
                            else if (temperature >= overheatedTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= hotTemperatureThreshold && !ignoreHot)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Temperature - " + temperature.ToString() + " degrees; warm threshold " + hotTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Temperature - " + temperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdSanDisk");
        }

        private void PopulateSmartInfoSsdSkHynix(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdSkHynix");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Retired sectors.
            if ((attributeID == 5) && rawData.Substring(8) != "0000")
            {
                // Retired Block Count
                // These are serious as they indicate drive wear, but aren't necessarily as fatal on an SSD. We'll
                // allow 50 of each before running up the flag, and we go critical at 150.
                int eventCount = 0;
                try
                {
                    eventCount = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }
                catch { }

                if (eventCount >= ssdRetirementCritical)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Event count meets or exceeds critical threshold. Setting disk to Critical.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementCountCritical", ssdRetirementCritical);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                else if (eventCount >= ssdRetirementWarning)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] Event count meets or exceeds warning threshold. Setting disk to Warning.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                // Temperature for SandForce is attribute 194.
                if (attributeID == 194)
                {
                    // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                }

                if (attributeID == 5 || attributeID == 9)
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 5: // Retired Blocks (Smartbuy/Phison has no attribute 5 but we keep here for compatibility)
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 180: // % Life remaining - Unused Reserved Block Count
                        {
                            // % of life remaining
                            pendingSectorCountOrSsdLifeLeft = smartValue;
                            if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] SSD life remaining less than or equal to Critical threshold. Setting disk to Critical.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            else if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning)
                            {
                                imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] SSD life remaining less than or equal to Warning threshold. Setting disk to Warning.");
                                SiAuto.Main.LogInt("smartValue", smartValue);
                                SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            }
                            break;
                        }
                    case 194:
                        {
                            temperature = (int)counter;
                            fTemperature = (temperature * 9 / 5) + 32;
                            kTemperature = temperature + 273;

                            if (temperature > absurdTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                isDiskTemperatureInvalid = true;
                            }
                            else if (temperature >= criticalTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Critical Temperature - " + temperature.ToString() + " degrees; overheated threshold " + criticalTemperatureThreshold.ToString());
                                isDiskCritical = true;
                            }
                            else if (temperature >= overheatedTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= hotTemperatureThreshold && !ignoreHot)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Temperature - " + temperature.ToString() + " degrees; warm threshold " + hotTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Temperature - " + temperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdSkHynix");
        }

        private void PopulateSmartInfoSsdKingSpec(String attributeName, bool isMajorHealthFlag, bool isCritical, String description, String binaryFlags,
            int attributeID, String hexAttributeID, int smartValue, int threshold, int worst, String rawData)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdKingSpec");
            FancyListView.ImageSubItem imageSubItem = new FancyListView.ImageSubItem();

            if (threshold == 0)
            {
                if (smartValue == threshold)
                {
                    // TODO - determine what really IS geriatric.
                    //imageSubItem.Text = Properties.Resources.SmartAttributeStatusGeriatric;
                    //imageSubItem.ImageKey = "Geriatric";
                    //isDiskGeriatric = true;
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }
            else
            {
                // Health Item
                if (smartValue < threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusFailed;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] FAIL - Threshold Exceeded on attribute " + attributeID.ToString());
                }
                else if (smartValue == threshold)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    isThresholdExceeded = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] WARN - Threshold Met on attribute " + attributeID.ToString());
                }
                else
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusHealthy;
                    imageSubItem.ImageKey = "Healthy";
                }
            }

            // Retired sectors.
            if (attributeID == 5 && rawData.Substring(8) != "0000")
            {
                // Retired Block Count and Pending Sectorss
                // These are serious as they indicate drive wear, but aren't necessarily as fatal on an SSD. We'll
                // allow 50 of each before running up the flag, and we go critical at 150.
                int eventCount = 0;
                try
                {
                    eventCount = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }
                catch { }

                if (eventCount >= ssdRetirementCritical)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Fail";
                    isDiskCritical = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "[SSD Check] Event count meets or exceeds critical threshold. Setting disk to Critical.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementCountCritical", ssdRetirementCritical);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
                else if (eventCount >= ssdRetirementWarning)
                {
                    imageSubItem.Text = Properties.Resources.SmartAttributeStatusDegraded;
                    imageSubItem.ImageKey = "Warning";
                    isDiskWarning = true;
                    SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[SSD Check] Event count meets or exceeds warning threshold. Setting disk to Warning.");
                    SiAuto.Main.LogInt("attributeID", attributeID);
                    SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                    SiAuto.Main.LogInt("eventCount", eventCount);
                }
            }

            // Keep track of certain attributes.
            try
            {
                long counter = long.Parse(rawData, System.Globalization.NumberStyles.HexNumber);

                // The raw data is 12 characters long. Temperatures are displayed only from the 2 lowest order bytes.
                // Temperature for SandForce is attribute 194.
                if (attributeID == 194)
                {
                    // Attributes 194 and 231 are temperature. Some manufacturers use a lot of the raw data field
                    // for other data which can throw off temperature calculation, so we grab the last couple fields.
                    counter = Int32.Parse(rawData.Substring(10), System.Globalization.NumberStyles.HexNumber);
                }

                if (attributeID == 5 || attributeID == 9)
                {
                    counter = Int32.Parse(rawData.Substring(8), System.Globalization.NumberStyles.HexNumber);
                }

                switch (attributeID)
                {
                    case 5: // Retired Blocks
                        {
                            badSectorCountOrRetiredBlockCount = (int)counter;
                            break;
                        }
                    case 9: // Power-On Hours
                        {
                            powerOnHours = (int)counter;
                            SiAuto.Main.LogInt("powerOnHours", powerOnHours);
                            break;
                        }
                    case 196: // Reallocation Event Count
                        {
                            reallocationEventCount = (int)counter;
                            break;
                        }
                    case 194:
                        {
                            temperature = (int)counter;
                            fTemperature = (temperature * 9 / 5) + 32;
                            kTemperature = temperature + 273;

                            if (temperature > absurdTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusAbsurd;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "[Health Check] SSD Temperature exceeds absurdity threshold of 90. The sensor is assumed to be wrong.");
                                isDiskTemperatureInvalid = true;
                            }
                            else if (temperature >= criticalTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusCritical;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Critical Temperature - " + temperature.ToString() + " degrees; overheated threshold " + criticalTemperatureThreshold.ToString());
                                isDiskCritical = true;
                            }
                            else if (temperature >= overheatedTemperatureThreshold)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusOverheated;
                                imageSubItem.ImageKey = "Fail";
                                SiAuto.Main.LogColored(System.Drawing.Color.Red, "[HDD Check] Overheated Temperature - " + temperature.ToString() + " degrees; overheated threshold " + overheatedTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= hotTemperatureThreshold && !ignoreHot)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusHot;
                                imageSubItem.ImageKey = "Warning";
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Hot Temperature - " + temperature.ToString() + " degrees; warm threshold " + hotTemperatureThreshold.ToString());
                                isDiskWarning = true;
                            }
                            else if (temperature >= warmTemperatureThreshold && !ignoreWarm)
                            {
                                imageSubItem.Text = Properties.Resources.SmartTemperatureStatusWarm;
                                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[HDD Check] Warm Temperature - " + temperature.ToString() + " degrees; warm threshold " + warmTemperatureThreshold.ToString());
                                imageSubItem.ImageKey = "Warning";
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                FancyListView.ImageSubItem majorHealthSubItem = new FancyListView.ImageSubItem();
                ListViewItem.ListViewSubItem isCriticalSubItem = new ListViewItem.ListViewSubItem();
                isCriticalSubItem.Text = (isCritical ? "Yes" : "No");
                isCriticalSubItem.Tag = binaryFlags;

                if (isMajorHealthFlag)
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "SuperCritical";
                }
                else
                {
                    majorHealthSubItem.Text = String.Empty;
                    majorHealthSubItem.ImageKey = "Blank";
                }
                majorHealthSubItem.Tag = isMajorHealthFlag;

                ListViewItem lvi = new ListViewItem(new String[] { String.Empty });
                lvi.SubItems.Add(majorHealthSubItem);
                lvi.SubItems.Add(attributeID.ToString());
                lvi.SubItems.Add(hexAttributeID);
                lvi.SubItems.Add(attributeName);
                lvi.SubItems.Add(isCriticalSubItem);
                lvi.SubItems.Add((threshold > 0 ? "Pre-Fail" : "Advisory"));
                lvi.SubItems.Add(threshold.ToString());
                lvi.SubItems.Add(smartValue.ToString());
                lvi.SubItems.Add(worst.ToString());
                lvi.SubItems.Add(imageSubItem);
                lvi.SubItems.Add(rawData);
                lvi.SubItems.Add(binaryFlags);
                lvi.ToolTipText = description;
                listViewSmartDetails.Items.Add(lvi);
                listViewSmartDetails.ShowItemToolTips = true;
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PopulateSmartInfoSsdKingSpec");
        }

        private String ConcatenateRawData(int rawHigh, int raw4, int raw3, int raw2, int raw1, int rawLow)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.ConcatenateRawData");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.ConcatenateRawData");
            return (rawHigh.ToString("X").Length == 1 ? "0" + rawHigh.ToString("X") : rawHigh.ToString("X"))
                        + (raw4.ToString("X").Length == 1 ? "0" + raw4.ToString("X") : raw4.ToString("X"))
                        + (raw3.ToString("X").Length == 1 ? "0" + raw3.ToString("X") : raw3.ToString("X"))
                        + (raw2.ToString("X").Length == 1 ? "0" + raw2.ToString("X") : raw2.ToString("X"))
                        + (raw1.ToString("X").Length == 1 ? "0" + raw1.ToString("X") : raw1.ToString("X"))
                        + (rawLow.ToString("X").Length == 1 ? "0" + rawLow.ToString("X") : rawLow.ToString("X"));
        }

        /// <summary>
        /// Gets a string that indicates whether a drive failure is predicted in the MSStorageDriver.
        /// </summary>
        /// <param name="drive">WMI collection of drives to check.</param>
        /// <returns>true/false based on whether or not the disk is about to die.</returns>
        private String GetFailurePredictionFlag(ManagementObject drive)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.GetFailurePredictionFlag");
            DataRow[] smartDataRows = smartDataTable.SmartDataTable.Select();

            isWmiFailurePredicted = false;

            if (smartDataRows != null && smartDataRows.Length > 0)
            {
                foreach (DataRow smartDataRow in smartDataRows)
                {
                    if (String.Compare(drive["PNPDeviceID"].ToString() + "_0", smartDataRow["Key"].ToString(), true) != 0)
                    {
                        // Not the disk we want; skip to the next.
                        continue;
                    }
                    // Get the failure flag.
                    isWmiFailurePredicted = (bool)smartDataRow["FailurePredicted"];
                    break; // No need to check anymore disks.
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetFailurePredictionFlag");
            return isWmiFailurePredicted.ToString();
        }

        private void listViewPhysicalDisks_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.listViewPhysicalDisks_ColumnClick");
            listViewPhysicalDisks.SuspendLayout();
            ((ListViewColumnSorter)this.listViewPhysicalDisks.ListViewItemSorter).SortColumn = e.Column;

            SiAuto.Main.LogMessage("Sorting disks.");
            listViewPhysicalDisks.Sort();
            listViewPhysicalDisks.ResumeLayout();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.listViewPhysicalDisks_ColumnClick");
        }

        private void listViewSmartDetails_DoubleClick(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.listViewSmartDetails_DoubleClick");
            if (listViewSmartDetails.SelectedItems != null && listViewSmartDetails.SelectedItems.Count != 0)
            {
                ListViewItem lvi = listViewSmartDetails.SelectedItems[0];
                AttributeDetails ad = new AttributeDetails(lvi, useDefaultSkinning, isWindowsServerSolutions);
                ad.ShowDialog();
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.listViewSmartDetails_DoubleClick");
        }

        private void listViewSmartDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.listViewSmartDetails_SelectedIndexChanged");
            SetIgnoreVisibility();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.listViewSmartDetails_SelectedIndexChanged");
        }

        private void MainUiControl_Load(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.MainUiControl_Load");
            // Set background
            SetWindowBackground();
            
            // Set labels from string table.
            SiAuto.Main.LogMessage("Populate text labels.");
            labelAppTitle.Text = OperatingSystem.IsWindowsServerSolutionsProduct(this) ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart;
            labelSelectPhysical.Text = Properties.Resources.ListViewPhysicalDisksTitle;
            labelDiskInfo.Text = Properties.Resources.ListViewDiskInfoTitle;
            labelSmartDetails.Text = Properties.Resources.ListViewSmartInfoTitle;

            SetAppTitle();

            try
            {
                SiAuto.Main.LogMessage("Start worker thread to populate disk info.");
                processingThread = new Thread(new ParameterizedThreadStart(ReapPhysicalDiskInfo));
                processingThread.Name = "Initial Process Thread";
                processingThread.Start(new bool[] { true, false });
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Failed to start worker thread. " + ex.Message);
                SiAuto.Main.LogException(ex);
                EpicFail fail = new EpicFail(ex, Properties.Resources.ErrorMessageWorkerProcessStartFail, "Your server may be running low on memory or have two many Microsoft .NET " +
                    "based applications running. Try closing unnecessary applications or rebooting the server. If this error persists, please tender a bug report.", isWindowsServerSolutions);
                fail.ShowDialog();
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.MainUiControl_Load");
        }

        /// <summary>
        /// Reaps physical disk information from WMI, along with pertinent SMART-related data for each disk.
        /// </summary>
        /// <param name="data">bool value passed to worker thread - set to true if first disk should be selected at conclusion of thread run; false to leave alone.</param>
        private void ReapPhysicalDiskInfo(object data)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.ReapPhysicalDiskInfo");
            isWorkerProcessRunning = true;
            bool[] dataArray = (bool[])data;
            bool selectFirstDisk = dataArray[0];
            bool forceRefresh = dataArray[1];
            DiskStatus status = DiskStatus.Unknown;

            if (forceRefresh)
            {
                try
                {
                    UpdateProgressBar(40);
                    String phantomDiskList = String.Empty;
                    DiskEnumerator.RefreshDiskInfo(true, fallbackToWmi, advancedSii, ignoreVirtualDisks, out phantomDiskList);
                    UpdateProgressBar(50);
                }
                catch (InvalidOperationException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    EpicFail fail = new EpicFail(ex, "", isWindowsServerSolutions);
                    fail.ShowDialog();
                    UpdateProgressBar(0);
                    return;
                }
            }

            bool windowOpen = true;

            try
            {
                // Browse all WMI physical disks.
                String wmiQuery = isWindows8 ? Properties.Resources.WmiQueryStringWin8 : Properties.Resources.WmiQueryStringNonWin8;
                String queryScope = isWindows8 ? Properties.Resources.WmiQueryScope : Properties.Resources.WmiQueryScopeDefault;
                String friendlyName = String.Empty;

                int phantomIndex = 0;

                foreach (ManagementObject drive in new ManagementObjectSearcher(queryScope, wmiQuery).Get())
                {
                    bool isPhantom = false;
                    // NULL check for Storage Spaces phantom disks.
                    if (isWindows8 && drive["DeviceID"] == null)
                    {
                        friendlyName = String.Empty;
                        try
                        {
                            if (drive["FriendlyName"] == null)
                            {
                                friendlyName = "Storage Spaces Phantom Disk";
                                SiAuto.Main.LogWarning("[Storage Spaces] Phantom Disk detected. Name unknown.");
                            }
                            else
                            {
                                friendlyName = drive["FriendlyName"].ToString();
                                SiAuto.Main.LogWarning("[Storage Spaces] Phantom Disk detected: " + friendlyName);
                            }
                        }
                        catch
                        {
                        }
                        isPhantom = true;
                    }
                    decimal capacity = 0.00M;
                    decimal giggage = 0.00M;
                    if (drive["Size"] != null)
                    {
                        giggage = Decimal.Parse(drive["Size"].ToString());
                    }
                    else
                    {
                        giggage = 0.00M;
                    }
                    capacity = Decimal.Round(giggage / 1073741824.00M, 2);

                    String model = String.Empty;
                    if (isPhantom)
                    {
                        SiAuto.Main.LogMessage("Disk is phantom; cannot get preferred model. Will use " + friendlyName + " instead.");
                        model = friendlyName;
                    }
                    else
                    {
                        SiAuto.Main.LogBool("isWindows8", isWindows8);
                        SiAuto.Main.LogMessage("Attempt to get preferred model name.");
                        model = Utilities.Utility.GetDiskPreferredModel(drive, isWindows8);
                        if (isWindows8)
                        {
                            if (String.IsNullOrEmpty(model))
                            {
                                SiAuto.Main.LogMessage("The model name returned was null or empty; trying to concatenate Manufacturer + Model via WMI.");
                                try
                                {
                                    model = drive["Manufacturer"].ToString() + drive["Model"].ToString();
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                SiAuto.Main.LogString("Model name was returned.", model);
                            }
                        }

                        try
                        {
                            if (String.IsNullOrEmpty(model))
                            {
                                SiAuto.Main.LogWarning("Model is still null or empty; trying additional methods.");
                                if (String.IsNullOrEmpty(model) && drive["Model"] != null)
                                {
                                    SiAuto.Main.LogMessage("Trying non-null Model.");
                                    model = drive["Model"].ToString();
                                }
                                else if (String.IsNullOrEmpty(model) && drive["Model"] == null && drive["PNPDeviceID"] != null)
                                {
                                    SiAuto.Main.LogMessage("Trying non-null PNPDeviceID.");
                                    model = drive["PNPDeviceID"].ToString() + " " + capacity.ToString() + " GB";
                                }
                                else if (String.IsNullOrEmpty(model))
                                {
                                    SiAuto.Main.LogMessage("No satisfactory name exists; using Undefined.");
                                    model = "Undefined " + capacity.ToString() + " GB";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogWarning(ex.Message);
                            SiAuto.Main.LogException(ex);
                            model = "Unrecognized " + capacity.ToString() + " GB (Possible VD)";
                        }
                    }

                    if (ignoreVirtualDisks && KnownVirtualDisks.IsDiskOnVirtualDiskList(model))
                    {
                        // It's a virtual disk we're ignoring, so skip it (continue).
                        continue;
                    }

                    bool isIgnored = false;
                    int celsiusTemperature = 0;

                    if (isPhantom)
                    {
                        status = DiskStatus.Critical;
                        celsiusTemperature = 0;
                    }
                    else
                    {
                        status = GetDiskStatusFromRegistry(drive, out isIgnored, out celsiusTemperature);
                        if (isIgnored)
                        {
                            continue;
                        }
                    }

                    String temperatureString = String.Empty;
                    if (celsiusTemperature == 0)
                    {
                        temperatureString = "N/A";
                        SiAuto.Main.LogMessage("celsiusTemperature is zero; assuming N/A");
                    }
                    else
                    {
                        SiAuto.Main.LogMessage("Temperature display preference is " + temperaturePreference);
                        switch (temperaturePreference)
                        {
                            case "F":
                                {
                                    int fTemp = (celsiusTemperature * 9 / 5) + 32;
                                    temperatureString = fTemp.ToString() + Properties.Resources.SettingsDegreesF;
                                    break;
                                }
                            case "K":
                                {
                                    int kTemp = celsiusTemperature + 273;
                                    temperatureString = kTemp.ToString() + Properties.Resources.SettingsDegreesK;
                                    break;
                                }
                            case "C":
                            default:
                                {
                                    temperatureString = celsiusTemperature.ToString() + Properties.Resources.SettingsDegreesC;
                                    break;
                                }
                        }
                        SiAuto.Main.LogValue("temperatureString", temperatureString);
                    }

                    FancyListView.ImageSubItem modelSubItem = new FancyListView.ImageSubItem();
                    SiAuto.Main.LogValue("status", status);
                    switch (status)
                    {
                        case DiskStatus.Critical:
                            {
                                modelSubItem.ImageKey = "Critical";
                                break;
                            }
                        case DiskStatus.Warning:
                            {
                                modelSubItem.ImageKey = "Warning";
                                break;
                            }
                        case DiskStatus.Geriatric:
                            {
                                modelSubItem.ImageKey = "Geriatric";
                                break;
                            }
                        case DiskStatus.Healthy:
                            {
                                modelSubItem.ImageKey = "Healthy";
                                break;
                            }
                        case DiskStatus.Unknown:
                        default:
                            {
                                modelSubItem.ImageKey = "Unknown";
                                break;
                            }
                    }
                    modelSubItem.Text = model;

                    String diskPathName = String.Empty;
                    if (isPhantom)
                    {
                        diskPathName = "\\\\.\\PHANTOMDISK" + phantomIndex.ToString();
                    }
                    else
                    {
                        diskPathName = (isWindows8 ? "\\\\.\\PHYSICALDRIVE" + drive["DeviceID"].ToString() : drive["DeviceID"].ToString());
                    }
                    decimal columnTag = 0;

                    if (isPhantom)
                    {
                        columnTag = phantomIndex + 65535;
                    }
                    else
                    {
                        columnTag = Utilities.Utility.GetDriveIdFromPath(diskPathName);
                    }

                    if (columnTag == -1)
                    {
                        columnTag = 0;
                    }

                    SiAuto.Main.LogDecimal("columnTag", columnTag);

                    ListViewItem item = new ListViewItem(new String[] { diskPathName });
                    item.SubItems.Add(modelSubItem);
                    item.SubItems[0].Tag = columnTag;
                    item.SubItems.Add(capacity.ToString() + " GB");
                    item.SubItems[2].Tag = giggage;
                    item.SubItems.Add(temperatureString);
                    item.SubItems[3].Tag = celsiusTemperature;

                    AddListViewItemToPhysicalDisks(item);
                }
                UpdateProgressBar(80);

                if (selectFirstDisk)
                {
                    ((ListViewColumnSorter)this.listViewPhysicalDisks.ListViewItemSorter).SortColumn = 0;
                    ((ListViewColumnSorter)this.listViewPhysicalDisks.ListViewItemSorter).Order = SortOrder.Ascending;

                    //TODO: Move to delegate.
                    SortListView();
                    UpdateProgressBar(90);
                    SelectFirstPhysicalDisk();
                    UpdateProgressBar(100);
                }
                else
                {
                    lastSelectedPath = "65535";
                    UpdateProgressBar(100);
                }
                Thread.Sleep(1000);
            }
            catch (ThreadAbortException)
            {
                // Do nothing; just quit
            }
            catch (InvalidOperationException)
            {
                // User may have closed the window; just quit
                windowOpen = false;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("not found"))
                {
                    EpicFail fail = new EpicFail(ex, "", isWindowsServerSolutions);
                    fail.ShowDialog();
                    //QMessageBox.Show(Properties.Resources.ErrorMessageWorkerProcessWmiNotSupported + ex.Message, Properties.Resources.WindowTitleWarning,
                    //    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    EpicFail fail = new EpicFail(ex, "Not much you can do here. It's a bug. You didn't do anything wrong.", isWindowsServerSolutions);
                    fail.ShowDialog();
                    //QMessageBox.Show(Properties.Resources.ErrorMessageWorkerProcessWmiFail + ex.Message, Properties.Resources.ErrorMessageTitleSevere,
                    //    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                if (windowOpen)
                {
                    UpdateProgressBar(0);
                }
                isWorkerProcessRunning = false;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.ReapPhysicalDiskInfo");
        }

        /// <summary>
        /// Gets a disk's health status from the Registry.
        /// </summary>
        /// <param name="drive">The drive to check.</param>
        /// <param name="isIgnored">Output flag that specifies whether a disk is ignored.</param>
        /// <returns>The DiskStatus object that reflects the health condition of the disk.</returns>
        private DiskStatus GetDiskStatusFromRegistry(ManagementObject drive, out bool isIgnored, out int celsiusTemperature)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.GetDiskStatusFromRegistry");
            isIgnored = false;
            celsiusTemperature = 0;

            try
            {
                SiAuto.Main.LogMessage("Acquire Registry objects");
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;
                SiAuto.Main.LogMessage("Registry objects acquired");

                SiAuto.Main.LogMessage("Open dojoNorthSubKey and monitored disks key");
                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryMonitoredDisksKey, false);
                SiAuto.Main.LogMessage("dojoNorthSubKey and monitored disks key opened");

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, false);
                    String diskTestPath = String.Empty;
                    if (isWindows8)
                    {
                        diskTestPath = "\\\\.\\PHYSICALDRIVE" + drive["DeviceID"].ToString();
                    }
                    else
                    {
                        diskTestPath = drive["DeviceID"].ToString();
                    }
                    if (String.Compare(diskTestPath, (String)diskKey.GetValue("DevicePath"), true) == 0)
                    {
                        SiAuto.Main.LogMessage("[Retrieve Data] Getting Registry data for " + diskTestPath);
                        bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                        if (!activeFlag)
                        {
                            // Inactive/Stale disk.
                            SiAuto.Main.LogWarning("Disk is inactive or stale; skipping. Closing diskKeyh.");
                            diskKey.Close();
                            SiAuto.Main.LogMessage("diskKey object closed.");
                            continue;
                        }

                        try
                        {
                            String dateIgnored = (String)diskKey.GetValue("DateIgnored", String.Empty);
                            DateTime result;
                            if (DateTime.TryParse(dateIgnored, out result))
                            {
                                isIgnored = true;
                                SiAuto.Main.LogWarning("Disk ignored date flag is set; the disk is being ignored.");
                                SiAuto.Main.LogValue("dateIgnored", dateIgnored);
                                diskKey.Close();
                                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetDiskStatusFromRegistry");
                                return DiskStatus.Unknown;
                            }
                        }
                        catch
                        {

                        }

                        try
                        {
                            celsiusTemperature = (int)diskKey.GetValue("CelsiusTemperature", 0);
                        }
                        catch
                        {
                            celsiusTemperature = 0;
                        }
                        SiAuto.Main.LogValue("celsiusTemperature", celsiusTemperature);

                        try
                        {
                            if (bool.Parse((String)diskKey.GetValue("IsDiskCritical")))
                            {
                                SiAuto.Main.LogColored(System.Drawing.Color.Red,
                                    "Disk status is Critical");
                                diskKey.Close();
                                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetDiskStatusFromRegistry");
                                return DiskStatus.Critical;
                            }
                            else if (bool.Parse((String)diskKey.GetValue("IsDiskWarning")))
                            {
                                SiAuto.Main.LogColored(System.Drawing.Color.Orange,
                                    "Disk status is Warning");
                                diskKey.Close();
                                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetDiskStatusFromRegistry");
                                return DiskStatus.Warning;
                            }
                            else if (bool.Parse((String)diskKey.GetValue("IsDiskGeriatric")))
                            {
                                SiAuto.Main.LogColored(System.Drawing.Color.Blue,
                                    "Disk status is Geriatric");
                                diskKey.Close();
                                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetDiskStatusFromRegistry");
                                return DiskStatus.Geriatric;
                            }
                            else if (bool.Parse((String)diskKey.GetValue("IsDiskUnknown")))
                            {
                                SiAuto.Main.LogColored(System.Drawing.Color.Purple,
                                    "Disk status is Unknown");
                                diskKey.Close();
                                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetDiskStatusFromRegistry");
                                return DiskStatus.Unknown;
                            }
                            else // healthy
                            {
                                SiAuto.Main.LogColored(System.Drawing.Color.Green,
                                    "Disk status is Healthy");
                                diskKey.Close();
                                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetDiskStatusFromRegistry");
                                return DiskStatus.Healthy;
                            }
                        }
                        catch (Exception ex2)
                        {
                            SiAuto.Main.LogException(ex2);
                            SiAuto.Main.LogColored(System.Drawing.Color.Purple,
                                    "Disk status is Unknown (exception thrown)");
                            diskKey.Close();
                            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetDiskStatusFromRegistry");
                            return DiskStatus.Unknown;
                        }
                    }
                    diskKey.Close();
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogException(ex);
                SiAuto.Main.LogColored(System.Drawing.Color.Red,
                                    "Disk status is Unknown (exception thrown)");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetDiskStatusFromRegistry");
                return DiskStatus.Unknown;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetDiskStatusFromRegistry");
            return DiskStatus.Unknown;
        }

        private bool GetSmartDataFromRegistry(ManagementObject drive, out byte[] smartData, out byte[] smartThreshold)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.GetSmartDataFromRegistry");
            smartData = null;
            smartThreshold = null;

            try
            {
                Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey;

                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);

                Microsoft.Win32.RegistryKey monitoredDisksKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryMonitoredDisksKey, false);

                foreach (String diskKeyName in monitoredDisksKey.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey diskKey;
                    diskKey = monitoredDisksKey.OpenSubKey(diskKeyName, false);
                    String diskTestPath = String.Empty;
                    if (isWindows8)
                    {
                        diskTestPath = "\\\\.\\PHYSICALDRIVE" + drive["DeviceID"].ToString();
                    }
                    else
                    {
                        diskTestPath = drive["DeviceID"].ToString();
                    }
                    if (String.Compare(diskTestPath, (String)diskKey.GetValue("DevicePath"), true) == 0)
                    {
                        try
                        {
                            smartData = (byte[])diskKey.GetValue("RawSmartData");
                            smartThreshold = (byte[])diskKey.GetValue("RawThresholdData");
                            isSsd = bool.Parse((String)diskKey.GetValue("IsSsd", "False"));
                            if (isSsd)
                            {
                                ssdControllerManufacturer = (String)diskKey.GetValue("SsdController", String.Empty);
                            }
                            else
                            {
                                ssdControllerManufacturer = String.Empty;
                            }
                            isTrimSupported = bool.Parse((String)diskKey.GetValue("IsTrimSupported"));
                            bool activeFlag = (bool.Parse((String)diskKey.GetValue("IsActive")));
                            diskKey.Close();
                            if (!activeFlag)
                            {
                                // Inactive/Stale disk.
                                continue;
                            }
                            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetSmartDataFromRegistry");
                            return true;
                        }
                        catch
                        {
                            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetSmartDataFromRegistry");
                            diskKey.Close();
                            return false;
                        }
                    }
                    diskKey.Close();
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Exception was thrown reading from the Registry: " + ex.Message);
                SiAuto.Main.LogException(ex);
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetSmartDataFromRegistry");
                return false;
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.GetSmartDataFromRegistry");
            return false;
        }

        private void UpdateProgressBar(int progress)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.UpdateProgressBar");
            SiAuto.Main.LogMessage("[Delegate Invoke] Invoke progressBarUpdater delegate.");
            // Can occur if the window is closed too fast
            try
            {
                this.Invoke(progressBarUpdater, new object[] { progress });
            }
            catch
            {
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.UpdateProgressBar");
        }

        private void DUpdateProgressBar(int progress)
        {
            SiAuto.Main.EnterMethod("[Delegate Method] HomeServerSMART2013.UI.MainUIControl.DUpdateProgressBar");
            if (progress == 0 || progress == 100)
            {
                labelWelcome.Text = "Ready";
                if (progress == 0)
                {
                    buttonRefresh.Enabled = true;
                    buttonFastRefresh.Enabled = true;
                }
            }
            else
            {
                labelWelcome.Text = "Working...";
            }
            toolStripProgressBar1.Value = progress;
            SiAuto.Main.LeaveMethod("[Delegate Method] HomeServerSMART2013.UI.MainUIControl.DUpdateProgressBar");
        }

        private void DEpicFail(Exception ex, String message, String resolution)
        {
            SiAuto.Main.EnterMethod("[Delegate Method] HomeServerSMART2013.UI.MainUIControl.DEpicFail");
            if (String.IsNullOrEmpty(message))
            {
                EpicFail fail = new EpicFail(ex, resolution, isWindowsServerSolutions);
                fail.ShowDialog();
            }
            else
            {
                EpicFail fail = new EpicFail(ex, message, resolution, isWindowsServerSolutions);
                fail.ShowDialog();
            }
            SiAuto.Main.LeaveMethod("[Delegate Method] HomeServerSMART2013.UI.MainUIControl.DEpicFail");
        }

        /// <summary>
        /// Invokes the corresponding delegate method which sorts the ListView of physical disks
        /// Can be called from the main or worker threads.
        /// </summary>
        private void SortListView()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.SortListView");
            SiAuto.Main.LogMessage("[Delegate Invoke] Invoke sortViewFirstSelect delegate.");
            this.Invoke(sortViewFirstSelect);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.SortListView");
        }

        /// <summary>
        /// Delegate method that sorts the ListView of physical disks. This must be performed as a
        /// delegate because this operation may be requested from a worker thread.
        /// </summary>
        private void DSortListView()
        {
            SiAuto.Main.EnterMethod("[Delegate Method] HomeServerSMART2013.UI.MainUIControl.DSortListView");
            listViewPhysicalDisks.Sort();
            SiAuto.Main.LeaveMethod("[Delegate Method] HomeServerSMART2013.UI.MainUIControl.DSortListView");
        }

        /// <summary>
        /// Runs processing needed after disk polling data has been collected. Calculates the health of the disks, if desired,
        /// and refreshes the UI. Default performs the full update.
        /// </summary>
        private void PostProcessAndUpdateDiskHealth()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PostProcessAndUpdateDiskHealth");
            SiAuto.Main.LogMessage("Calling overload with value true.");
            PostProcessAndUpdateDiskHealth(true);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PostProcessAndUpdateDiskHealth");
        }

        /// <summary>
        /// Runs processing needed after disk polling data has been collected. Calculates the health of the disks, if desired,
        /// and refreshes the UI. Default performs the full update.
        /// </summary>
        /// <param name="isFullUpdate">true to perform full update; false to perform UI refresh only.</param>
        private void PostProcessAndUpdateDiskHealth(bool isFullUpdate)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.PostProcessAndUpdateDiskHealth");
            SiAuto.Main.LogBool("isFullUpdate", isFullUpdate);
            SiAuto.Main.LogBool("isSsd", isSsd);
            if (isFullUpdate)
            {
                // Disk Health Priorities - if multiple are set the precedence is as follows:
                // 1. Critical
                // 2. Warning
                // 3. Geriatric
                // 4. Healthy
                if (isSsd)
                {
                    SiAuto.Main.LogString("ssdControllerManufacturer", ssdControllerManufacturer.ToUpper());
                    SiAuto.Main.LogMessage("Current Critical, Warning and Geriatric state conditions below for SSD - may already be Critical, Warning or Geriatric depending on some conditions.");
                    SiAuto.Main.LogBool("isDiskCritical", isDiskCritical);
                    SiAuto.Main.LogBool("isDiskWarning", isDiskWarning);
                    SiAuto.Main.LogBool("isDiskGeriatric", isDiskGeriatric);
                    SiAuto.Main.LogMessage("Checking SSD value states for potential Critical conditions.");
                    if (ssdControllerManufacturer.ToUpper() == "STEC")
                    {
                        if (badSectorCountOrRetiredBlockCount >= ssdRetirementCritical || reallocationEventCount >= ssdRetirementCritical || pendingSectorCountOrSsdLifeLeft >= ssdRetirementCritical || endToEndErrorCountOrLifeCurve > 5)
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Post Process Update] Setting STEC SSD to Critical");
                            SiAuto.Main.LogInt("badSectorCountOrRetiredBlockCount", badSectorCountOrRetiredBlockCount);
                            SiAuto.Main.LogInt("reallocationEventCount", reallocationEventCount);
                            SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            SiAuto.Main.LogInt("endToEndErrorCountOrLifeCurve", endToEndErrorCountOrLifeCurve);
                            isDiskCritical = true;
                        }
                    }
                    else if (ssdControllerManufacturer.ToUpper() == "SAMSUNG")
                    {
                        if (badSectorCountOrRetiredBlockCount >= ssdRetirementCritical || reallocationEventCount >= ssdRetirementCritical || endToEndErrorCountOrLifeCurve >= ssdRetirementCritical)
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Post Process Update] Setting Samsung SSD to Critical");
                            SiAuto.Main.LogInt("badSectorCountOrRetiredBlockCount", badSectorCountOrRetiredBlockCount);
                            SiAuto.Main.LogInt("reallocationEventCount", reallocationEventCount);
                            SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            SiAuto.Main.LogInt("endToEndErrorCountOrLifeCurve", endToEndErrorCountOrLifeCurve);
                            isDiskCritical = true;
                        }
                    }
                    else if (ssdControllerManufacturer.ToUpper() == "JMICRON")
                    {
                        if (badSectorCountOrRetiredBlockCount >= ssdRetirementCritical || reallocationEventCount >= ssdRetirementCritical || pendingSectorCountOrSsdLifeLeft >= ssdRetirementCritical)
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Post Process Update] Setting JMicron SSD to Critical");
                            SiAuto.Main.LogInt("badSectorCountOrRetiredBlockCount", badSectorCountOrRetiredBlockCount);
                            SiAuto.Main.LogInt("reallocationEventCount", reallocationEventCount);
                            SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            SiAuto.Main.LogInt("endToEndErrorCountOrLifeCurve", endToEndErrorCountOrLifeCurve);
                            isDiskCritical = true;
                        }
                    }
                    else
                    {
                        // Bad sectors >= critical thresh OR reallocations >= critical thresh OR life left <= critical thresh OR (Intel AND end-to-end errors > 5)
                        if (badSectorCountOrRetiredBlockCount >= ssdRetirementCritical || reallocationEventCount >= ssdRetirementCritical ||
                            (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical && ssdControllerManufacturer.ToUpper() != "KINGSPEC") ||
                            (ssdControllerManufacturer.ToUpper() == "INTEL" && endToEndErrorCountOrLifeCurve > 5))
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Post Process Update] Setting SSD to Critical");
                            SiAuto.Main.LogInt("badSectorCountOrRetiredBlockCount", badSectorCountOrRetiredBlockCount);
                            SiAuto.Main.LogInt("reallocationEventCount", reallocationEventCount);
                            SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            SiAuto.Main.LogInt("endToEndErrorCountOrLifeCurve", endToEndErrorCountOrLifeCurve);
                            isDiskCritical = true;
                        }
                    }

                    SiAuto.Main.LogMessage("Checking SSD value states for potential Warning conditions.");
                    if (ssdControllerManufacturer.ToUpper() == "STEC")
                    {
                        if (badSectorCountOrRetiredBlockCount >= ssdRetirementWarning || reallocationEventCount >= ssdRetirementWarning || pendingSectorCountOrSsdLifeLeft >= ssdRetirementWarning || endToEndErrorCountOrLifeCurve > 0)
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Post Process Update] Setting STEC SSD to Warning");
                            SiAuto.Main.LogInt("badSectorCountOrRetiredBlockCount", badSectorCountOrRetiredBlockCount);
                            SiAuto.Main.LogInt("reallocationEventCount", reallocationEventCount);
                            SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            SiAuto.Main.LogInt("endToEndErrorCountOrLifeCurve", endToEndErrorCountOrLifeCurve);
                            isDiskWarning = true;
                        }
                    }
                    else if (ssdControllerManufacturer.ToUpper() == "SAMSUNG")
                    {
                        if (badSectorCountOrRetiredBlockCount >= ssdRetirementWarning || reallocationEventCount >= ssdRetirementWarning || endToEndErrorCountOrLifeCurve >= ssdRetirementWarning)
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Post Process Update] Setting Samsung SSD to Critical");
                            SiAuto.Main.LogInt("badSectorCountOrRetiredBlockCount", badSectorCountOrRetiredBlockCount);
                            SiAuto.Main.LogInt("reallocationEventCount", reallocationEventCount);
                            SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            SiAuto.Main.LogInt("endToEndErrorCountOrLifeCurve", endToEndErrorCountOrLifeCurve);
                            isDiskWarning = true;
                        }
                    }
                    else if (ssdControllerManufacturer.ToUpper() == "JMICRON")
                    {
                        if (badSectorCountOrRetiredBlockCount >= ssdRetirementWarning || reallocationEventCount >= ssdRetirementWarning || pendingSectorCountOrSsdLifeLeft >= ssdRetirementWarning)
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Post Process Update] Setting Samsung SSD to Critical");
                            SiAuto.Main.LogInt("badSectorCountOrRetiredBlockCount", badSectorCountOrRetiredBlockCount);
                            SiAuto.Main.LogInt("reallocationEventCount", reallocationEventCount);
                            SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            SiAuto.Main.LogInt("endToEndErrorCountOrLifeCurve", endToEndErrorCountOrLifeCurve);
                            isDiskWarning = true;
                        }
                    }
                    else
                    {
                        // Bad sectors >= warning thresh OR reallocations >= warning thresh OR life left <= warning thresh OR (Intel AND end-to-end errors > 0) OR (SandForce AND life curve <= 90)
                        if (badSectorCountOrRetiredBlockCount >= ssdRetirementWarning || reallocationEventCount >= ssdRetirementWarning || (ssdControllerManufacturer.ToUpper() == "INTEL" && endToEndErrorCountOrLifeCurve > 0) ||
                            (ssdControllerManufacturer.ToUpper() == "SANDFORCE" && endToEndErrorCountOrLifeCurve <= 90 && endToEndErrorCountOrLifeCurve > 0) ||
                            (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning && ssdControllerManufacturer.ToUpper() != "KINGSPEC"))
                        {
                            SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Post Process Update] Setting SSD to Warning");
                            SiAuto.Main.LogInt("ssdRetirementWarning", ssdRetirementWarning);
                            SiAuto.Main.LogInt("ssdLifeLeftWarning", ssdLifeLeftWarning);
                            SiAuto.Main.LogInt("badSectorCountOrRetiredBlockCount", badSectorCountOrRetiredBlockCount);
                            SiAuto.Main.LogInt("reallocationEventCount", reallocationEventCount);
                            SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                            SiAuto.Main.LogInt("endToEndErrorCountOrLifeCurve", endToEndErrorCountOrLifeCurve);
                            isDiskWarning = true;
                        }
                    }
                }
                else
                {
                    SiAuto.Main.LogMessage("Current Critical, Warning and Geriatric state conditions below for HDD - may already be Critical, Warning or Geriatric depending on some conditions.");
                    SiAuto.Main.LogBool("isDiskCritical", isDiskCritical);
                    SiAuto.Main.LogBool("isDiskWarning", isDiskWarning);
                    SiAuto.Main.LogBool("isDiskGeriatric", isDiskGeriatric);
                    SiAuto.Main.LogMessage("Checking HDD value states for potential Critical conditions.");

                    if (badSectorCountOrRetiredBlockCount >= 50 || endToEndErrorCountOrLifeCurve > 5 || reallocationEventCount >= 50 ||
                        pendingSectorCountOrSsdLifeLeft >= ssdLifeLeftWarning || uncorrectableSectorCount >= 50 || spinRetryCount >= 8)
                    {
                        SiAuto.Main.LogColored(System.Drawing.Color.Red, "[Post Process Update] Setting HDD to Critical");
                        SiAuto.Main.LogInt("badSectorCountOrRetiredBlockCount", badSectorCountOrRetiredBlockCount);
                        SiAuto.Main.LogInt("reallocationEventCount", reallocationEventCount);
                        SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                        SiAuto.Main.LogInt("endToEndErrorCountOrLifeCurve", endToEndErrorCountOrLifeCurve);
                        SiAuto.Main.LogInt("uncorrectableSectorCount", uncorrectableSectorCount);
                        SiAuto.Main.LogInt("spinRetryCount", spinRetryCount);
                        isDiskCritical = true;
                    }

                    if (badSectorCountOrRetiredBlockCount > 0 || endToEndErrorCountOrLifeCurve > 0 || reallocationEventCount > 0 ||
                        pendingSectorCountOrSsdLifeLeft > 0 || uncorrectableSectorCount > 0 || spinRetryCount >= 3)
                    {
                        SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Post Process Update] Setting HDD to Warning");
                        SiAuto.Main.LogInt("badSectorCountOrRetiredBlockCount", badSectorCountOrRetiredBlockCount);
                        SiAuto.Main.LogInt("reallocationEventCount", reallocationEventCount);
                        SiAuto.Main.LogInt("pendingSectorCountOrSsdLifeLeft", pendingSectorCountOrSsdLifeLeft);
                        SiAuto.Main.LogInt("endToEndErrorCountOrLifeCurve", endToEndErrorCountOrLifeCurve);
                        SiAuto.Main.LogInt("uncorrectableSectorCount", uncorrectableSectorCount);
                        SiAuto.Main.LogInt("spinRetryCount", spinRetryCount);
                        isDiskWarning = true;
                    }

                    if (ultraAtaCrcErrorCount > 50)
                    {
                        SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "[Post Process Update] Setting HDD to Warning on Ultra ATA CRC Error Count");
                        SiAuto.Main.LogInt("ultraAtaCrcErrorCount", ultraAtaCrcErrorCount);
                        isDiskWarning = true;
                    }
                }

                // Compute the Health % and update the UI

                if (isSsd)
                {
                    SiAuto.Main.LogMessage("[Post Process Update] Calculate health percentage for SSD and update UI.");
                    // For SandForce we consider bad sectors to be retired blocks, pending sectors to be SSD life left and
                    // reallocations to be reallocations.
                    SiAuto.Main.LogMessage("[Post Process Update] Updating status bar for Indilinx/Everest/Barefoot 3/Intel/SandForce/STEC/Samsung/JMicron/Micron");
                    labelBadSectors.Text = "Retired Sectors: " + badSectorCountOrRetiredBlockCount.ToString();
                    labelBadSectors.Image = (badSectorCountOrRetiredBlockCount >= 60 ? Properties.Resources.Critical16 : (badSectorCountOrRetiredBlockCount > 20 ? Properties.Resources.Warning16 :
                        Properties.Resources.Healthy16));
                    SiAuto.Main.LogMessage("[Post Process Update] Retired Sectors: " + badSectorCountOrRetiredBlockCount.ToString());
                    if (ssdControllerManufacturer.ToUpper() == "STEC" || ssdControllerManufacturer.ToUpper() == "JMICRON" || ssdControllerManufacturer.ToUpper() == "TOSHIBA")
                    {
                        labelPendingSectors.Text = (ssdControllerManufacturer.ToUpper() == "TOSHIBA" ? "SSD Life Left: " + pendingSectorCountOrSsdLifeLeft.ToString() + "%" :
                            "Sectors Pending Retirement: " + pendingSectorCountOrSsdLifeLeft.ToString());
                        if (ssdControllerManufacturer.ToUpper() == "TOSHIBA")
                        {
                            labelPendingSectors.Image = (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical ? Properties.Resources.Critical16 :
                                (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning ? Properties.Resources.Warning16 : Properties.Resources.Healthy16));
                        }
                        else
                        {
                            labelPendingSectors.Image = (pendingSectorCountOrSsdLifeLeft >= ssdRetirementCritical ? Properties.Resources.Critical16 : (pendingSectorCountOrSsdLifeLeft >= ssdLifeLeftWarning ? Properties.Resources.Warning16 :
                                Properties.Resources.Healthy16));
                        }
                        labelReallocationEvents.Text = (ssdControllerManufacturer.ToUpper() == "STEC" ? Properties.Resources.LabelReallocationEvents : "ECC Failures: ")
                            + reallocationEventCount.ToString();
                        labelReallocationEvents.Image = (reallocationEventCount >= ssdRetirementCritical ? Properties.Resources.Critical16 : (reallocationEventCount >= ssdLifeLeftWarning ? Properties.Resources.Warning16 :
                            Properties.Resources.Healthy16));
                        SiAuto.Main.LogMessage("[Post Process Update] (STEC/JMICRON specific) Sectors Pending Retirement: " + pendingSectorCountOrSsdLifeLeft.ToString());
                        SiAuto.Main.LogMessage("[Post Process Update] (STEC/JMICRON specific) Reallocation/ECC Fail Events: " + reallocationEventCount.ToString());
                    }
                    else if (ssdControllerManufacturer.ToUpper() == "SAMSUNG")
                    {
                        labelPendingSectors.Text = "SSD Life Left: " + pendingSectorCountOrSsdLifeLeft.ToString() + "%";
                        labelPendingSectors.Image = (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical ? Properties.Resources.Critical16 :
                            (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning ? Properties.Resources.Warning16 : Properties.Resources.Healthy16));
                        labelReallocationEvents.Text = "Reserve Blocks Used: " + reallocationEventCount.ToString();
                        labelReallocationEvents.Image = (reallocationEventCount >= ssdRetirementCritical ? Properties.Resources.Critical16 : (reallocationEventCount >= ssdLifeLeftWarning ? Properties.Resources.Warning16 :
                            Properties.Resources.Healthy16));
                    }
                    else if (ssdControllerManufacturer.ToUpper() == "KINGSPEC")
                    {
                        labelPendingSectors.Text = "SSD Life Left: " + Properties.Resources.ItemNotApplicable;
                        labelPendingSectors.Image = Properties.Resources.bar_icon_delete_red;
                        labelReallocationEvents.Text = Properties.Resources.LabelReallocationEvents + reallocationEventCount.ToString();
                        labelReallocationEvents.Image = labelReallocationEvents.Image = (reallocationEventCount >= ssdRetirementCritical ? Properties.Resources.Critical16 : (reallocationEventCount >= ssdLifeLeftWarning ? Properties.Resources.Warning16 :
                            Properties.Resources.Healthy16));
                    }
                    else
                    {
                        labelPendingSectors.Text = "SSD Life Left: " + pendingSectorCountOrSsdLifeLeft.ToString() + "%";
                        labelPendingSectors.Image = (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical ? Properties.Resources.Critical16 :
                            (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning ? Properties.Resources.Warning16 : Properties.Resources.Healthy16));
                        labelReallocationEvents.Text = Properties.Resources.LabelReallocationEvents + ((ssdControllerManufacturer.ToUpper() == "MICRON" || ssdControllerManufacturer.ToUpper() == "MARVELL") ?
                            reallocationEventCount.ToString() : Properties.Resources.ItemNotApplicable);
                        labelReallocationEvents.Image = (ssdControllerManufacturer.ToUpper() == "MICRON" || ssdControllerManufacturer.ToUpper() == "MARVELL") ?
                            (reallocationEventCount >= ssdRetirementCritical ? Properties.Resources.Critical16 : (reallocationEventCount >= ssdLifeLeftWarning ? Properties.Resources.Warning16 :
                            Properties.Resources.Healthy16)) : Properties.Resources.bar_icon_delete_red;
                        SiAuto.Main.LogMessage("[Post Process Update] SSD Life Left: " + pendingSectorCountOrSsdLifeLeft.ToString() + "%");
                        SiAuto.Main.LogMessage("[Post Process Update] Reallocation Events: " + (ssdControllerManufacturer.ToUpper() == "MICRON" ? reallocationEventCount.ToString() :
                            "N/A"));
                    }
                }
                else
                {
                    SiAuto.Main.LogMessage("[Post Process Update] Updating status bar for HDD.");
                    labelBadSectors.Text = Properties.Resources.LabelBadSectors + badSectorCountOrRetiredBlockCount.ToString();
                    labelBadSectors.Image = (badSectorCountOrRetiredBlockCount >= 20 ? Properties.Resources.Critical16 : (badSectorCountOrRetiredBlockCount > 0 ? Properties.Resources.Warning16 :
                        Properties.Resources.Healthy16));
                    labelPendingSectors.Text = Properties.Resources.LabelPendingSectors + pendingSectorCountOrSsdLifeLeft.ToString();
                    labelPendingSectors.Image = (pendingSectorCountOrSsdLifeLeft >= 30 ? Properties.Resources.Critical16 : (pendingSectorCountOrSsdLifeLeft > 0 ? Properties.Resources.Warning16 :
                        Properties.Resources.Healthy16));
                    labelReallocationEvents.Text = Properties.Resources.LabelReallocationEvents + reallocationEventCount.ToString();
                    labelReallocationEvents.Image = (reallocationEventCount >= 20 ? Properties.Resources.Critical16 : (reallocationEventCount > 0 ? Properties.Resources.Warning16 :
                        Properties.Resources.Healthy16));
                    SiAuto.Main.LogMessage("[Post Process Update] Bad Sectors: " + badSectorCountOrRetiredBlockCount.ToString());
                    SiAuto.Main.LogMessage("[Post Process Update] Pending Sectors: " + pendingSectorCountOrSsdLifeLeft.ToString());
                    SiAuto.Main.LogMessage("[Post Process Update] Reallocation Events: " + reallocationEventCount.ToString());
                }

                if (isSsd && (String.Compare(ssdControllerManufacturer, "Indilinx Everest", true) == 0 || String.Compare(ssdControllerManufacturer, "Indilinx", true) == 0 ||
                    String.Compare(ssdControllerManufacturer, "Indilinx Barefoot 3") == 0) && temperature == 0)
                {
                    // Indilinx, Indilinx Everest, and Indilinx Barefoot 3 SSDs have no temperature sensor.
                    SiAuto.Main.LogWarning("[Post Process Update] Indilinx, Indilinx Everest, and Indilinx Barefoot3 have no temperature sensor.");
                    labelTemperature.Text = Properties.Resources.LabelTemperature + Properties.Resources.ItemNotApplicable;
                    labelTemperature.Image = Properties.Resources.bar_icon_delete_red;
                }
                else if (temperature == 0 && airflowTemperature == 0)
                {
                    SiAuto.Main.LogWarning("[Post Process Update] Temperature is zero. The disk may not have a temperature sensor.");
                    labelTemperature.Text = Properties.Resources.LabelTemperature + Properties.Resources.ItemNotApplicable;
                    labelTemperature.Image = Properties.Resources.bar_icon_delete_red;
                }
                else
                {
                    SiAuto.Main.LogMessage("[Post Process Update] Add temperature to status bar.");
                    if (temperature == 0)
                    {
                        SiAuto.Main.LogWarning("[Post Process Update] Temperature is zero but a valid airflow temperature exists; that will be used.");
                    }
                    SiAuto.Main.LogString("temperaturePreference", temperaturePreference);
                    switch (temperaturePreference)
                    {
                        case "F":
                            {
                                labelTemperature.Text = Properties.Resources.LabelTemperature + (temperature == 0 ? fAirflowTemperature.ToString() : fTemperature.ToString())
                                    + Properties.Resources.LabelTemperatureF;
                                break;
                            }
                        case "K":
                            {
                                labelTemperature.Text = Properties.Resources.LabelTemperature + (temperature == 0 ? kAirflowTemperature.ToString() : kTemperature.ToString()) + Properties.Resources.LabelTemperatureK;
                                break;
                            }
                        case "C":
                        default:
                            {
                                labelTemperature.Text = Properties.Resources.LabelTemperature + (temperature == 0 ? airflowTemperature.ToString() : temperature.ToString()) + Properties.Resources.LabelTemperatureC;
                                break;
                            }
                    }
                    SiAuto.Main.LogBool("ignoreHot", ignoreHot);
                    SiAuto.Main.LogBool("ignoreWarm", ignoreWarm);
                    if (isDiskTemperatureInvalid)
                    {
                        labelTemperature.Image = Properties.Resources.Unknown16;
                    }
                    else if (ignoreWarm && ignoreHot)
                    {
                        if (temperature == 0)
                        {
                            labelTemperature.Image = (airflowTemperature >= criticalTemperatureThreshold ?
                                Properties.Resources.Critical16 : (airflowTemperature >= overheatedTemperatureThreshold ? Properties.Resources.Warning16 :
                                Properties.Resources.Healthy16));
                        }
                        else
                        {
                            labelTemperature.Image = (temperature >= criticalTemperatureThreshold ?
                                Properties.Resources.Critical16 : (temperature >= overheatedTemperatureThreshold ? Properties.Resources.Warning16 :
                                Properties.Resources.Healthy16));
                        }
                    }
                    else if (ignoreWarm)
                    {
                        if (temperature == 0)
                        {
                            labelTemperature.Image = (airflowTemperature >= criticalTemperatureThreshold ?
                                Properties.Resources.Critical16 : (airflowTemperature >= hotTemperatureThreshold ? Properties.Resources.Warning16 :
                                Properties.Resources.Healthy16));
                        }
                        else
                        {
                            labelTemperature.Image = (temperature >= criticalTemperatureThreshold ?
                                Properties.Resources.Critical16 : (temperature >= hotTemperatureThreshold ? Properties.Resources.Warning16 :
                                Properties.Resources.Healthy16));
                        }
                    }
                    else // nothing ignored (you cannot ignore hot but allow warm)
                    {
                        if (temperature == 0)
                        {
                            labelTemperature.Image = (airflowTemperature >= criticalTemperatureThreshold ?
                                Properties.Resources.Critical16 : (airflowTemperature >= warmTemperatureThreshold ? Properties.Resources.Warning16 :
                                Properties.Resources.Healthy16));
                        }
                        else
                        {
                            labelTemperature.Image = (temperature >= criticalTemperatureThreshold ?
                                Properties.Resources.Critical16 : (temperature >= warmTemperatureThreshold ? Properties.Resources.Warning16 :
                                Properties.Resources.Healthy16));
                        }
                    }
                }

                if (powerOnHours > -1)
                {
                    int quotient = 0;
                    int remainder = 0;
                    quotient = Math.DivRem(powerOnHours, 24, out remainder);

                    labelRuntime.Text = "Runtime: " + (quotient == 1 ? "1 Day" : quotient.ToString() + " Days") + ", " +
                        (remainder == 1 ? "1 Hour" : remainder.ToString() + " Hours");
                }
                else
                {
                    labelRuntime.Text = Properties.Resources.ItemNotApplicable;
                }
            }
            else
            {
                SiAuto.Main.LogMessage("[Post Process Update] Updating status bar for unknown disk.");
                labelBadSectors.Text = Properties.Resources.LabelBadSectors + Properties.Resources.LabelZero;
                labelBadSectors.Image = Properties.Resources.Healthy16;
                labelPendingSectors.Text = Properties.Resources.LabelPendingSectors + Properties.Resources.LabelZero;
                labelPendingSectors.Image = Properties.Resources.Healthy16;
                labelReallocationEvents.Text = Properties.Resources.LabelReallocationEvents + Properties.Resources.LabelZero;
                labelReallocationEvents.Image = Properties.Resources.Healthy16;
                labelTemperature.Text = Properties.Resources.LabelTemperature + Properties.Resources.LabelZeroTemperature;
                labelTemperature.Image = Properties.Resources.Healthy16;
                labelRuntime.Text = Properties.Resources.ItemNotApplicable;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.PostProcessAndUpdateDiskHealth");
        }

        private void AddListViewItemToPhysicalDisks(ListViewItem lvi)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.AddListViewItemToPhysicalDisks");
            SiAuto.Main.LogMessage("[Delegate Invoke] Invoke physicalDiskUpdate delegate.");
            this.Invoke(physicalDiskUpdate, new object[] { lvi });
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.AddListViewItemToPhysicalDisks");
        }

        private void DAddListViewItemToPhysicalDisks(ListViewItem lvi)
        {
            SiAuto.Main.EnterMethod("[Delegate Method] HomeServerSMART2013.UI.MainUIControl.AddListViewItemToPhysicalDisks");
            ImageList smartImageList = new ImageList();
            smartImageList.Images.Add("Healthy", Properties.Resources.Healthy16);
            smartImageList.Images.Add("Warning", Properties.Resources.Warning16);
            smartImageList.Images.Add("Critical", Properties.Resources.Critical16);
            smartImageList.Images.Add("Geriatric", Properties.Resources.Geriatric16);
            smartImageList.Images.Add("Unknown", Properties.Resources.Unknown16);
            listViewPhysicalDisks.SmallImageList = smartImageList;
            listViewPhysicalDisks.Items.Add(lvi);
            SiAuto.Main.LeaveMethod("[Delegate Method] HomeServerSMART2013.UI.MainUIControl.AddListViewItemToPhysicalDisks");
        }

        private void SelectFirstPhysicalDisk()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.SelectFirstPhysicalDisk");
            SiAuto.Main.LogMessage("[Delegate Invoke] Invoke firstDiskSelector delegate.");
            this.Invoke(firstDiskSelector);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.SelectFirstPhysicalDisk");
        }

        private void DSelectFirstPhysicalDisk()
        {
            SiAuto.Main.EnterMethod("[Delegate Method] HomeServerSMART2013.UI.MainUIControl.DSelectFirstPhysicalDisk");
            if (listViewPhysicalDisks.Items.Count > 0)
            {
                listViewPhysicalDisks.Items[0].Selected = true;
            }
            SiAuto.Main.LeaveMethod("[Delegate Method] HomeServerSMART2013.UI.MainUIControl.DSelectFirstPhysicalDisk");
        }

        private void SetIgnoreVisibility()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.SetIgnoreVisibility");
            if (listViewPhysicalDisks.SelectedItems != null && listViewPhysicalDisks.SelectedItems.Count != 0 &&
                listViewSmartDetails.SelectedItems != null && listViewSmartDetails.SelectedItems.Count != 0)
            {
                ListViewItem item = listViewSmartDetails.SelectedItems[0];
                int attributeID = 0;
                int threshold = 0;
                int attribValue = 0;
                long rawData = 0;
                String model = String.Empty;
                String path = String.Empty;

                try
                {
                    attributeID = Int32.Parse(item.SubItems[2].Text);
                    if (attributeID == 5 || attributeID == 184 || attributeID == 196 || attributeID == 197 || attributeID == 198)
                    {
                        // Take only the low-order 4 bytes of some values.
                        rawData = Int32.Parse(item.SubItems[11].Text.Substring(8), System.Globalization.NumberStyles.AllowHexSpecifier);
                    }
                    else
                    {
                        rawData = long.Parse(item.SubItems[11].Text, System.Globalization.NumberStyles.AllowHexSpecifier);
                    }
                    threshold = Int32.Parse(item.SubItems[7].Text);
                    attribValue = Int32.Parse(item.SubItems[8].Text);

                    if (threshold > 0 && (attribValue <= threshold))
                    {
                        // Cannot ignore a met or exceeded threshold.
                        buttonIgnoreProblem.Enabled = false;
                    }
                    else if (rawData > 0 && (attributeID == 5 || attributeID == 10 || attributeID == 170 || attributeID == 184 || attributeID == 196 ||
                        attributeID == 197 || attributeID == 198 || attributeID == 199))
                    {
                        if ((ssdControllerManufacturer.ToUpper() == "STEC" || ssdControllerManufacturer.ToUpper() == "SANDFORCE" ||
                            ssdControllerManufacturer.ToUpper() == "INTEL" || ssdControllerManufacturer.ToUpper() == "SMARTBUY/PHISON") && attributeID == 170)
                        {
                            buttonIgnoreProblem.Enabled = false;
                        }
                        else
                        {
                            buttonIgnoreProblem.Enabled = (gpoAllowIgnoredItems != 0);
                        }
                    }
                    else
                    {
                        buttonIgnoreProblem.Enabled = false;
                    }
                }
                catch
                {
                    buttonIgnoreProblem.Enabled = false;
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.SetIgnoreVisibility");
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.buttonRefresh_Click");
            try
            {
                buttonRefresh.Enabled = false;
                buttonFastRefresh.Enabled = false;
                buttonIgnoreDisk.Enabled = false;
                buttonExport.Enabled = false;

                if (isWorkerProcessRunning)
                {
                    // Don't refresh while a refresh is already running.
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.buttonRefresh_Click");
                    return;
                }

                try
                {
                    serviceController = new ServiceController(Properties.Resources.ServiceNameHss);
                }
                catch (Exception)
                {
                    QMessageBox.Show("Unable to bind Windows Service \"dnhsSmart\" (" + (isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") +
                        " Service) in the service controller. Please reboot the Server and " +
                        "try again. If the problem persists, please re-install " + (isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") + ".",
                        "Severe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    serviceController = null;
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.buttonRefresh_Click");
                    return;
                }

                switch (serviceController.Status)
                {
                    case ServiceControllerStatus.Stopped:
                        {
                            if (QMessageBox.Show("The " + (isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") +
                                " Service is currently stopped. Do you want to start it?",
                                "Service Halted", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                try
                                {
                                    serviceController.Start();
                                }
                                catch (Exception ex)
                                {
                                    QMessageBox.Show("Cannot start the service: " + ex.Message, "Severe",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                finally
                                {
                                    serviceController.Dispose();
                                }
                            }
                            else
                            {
                                QMessageBox.Show("Refresh cannot be performed because the service is halted.",
                                    "Service Halted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.buttonRefresh_Click");
                                return;
                            }
                            break;
                        }
                    case ServiceControllerStatus.Running:
                        {
                            serviceController.Dispose();
                            break;
                        }
                    case ServiceControllerStatus.Paused:
                        {
                            if (QMessageBox.Show("The " + (isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") +
                                " Service is currently paused. Do you want to resume it?",
                                "Service Paused", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                try
                                {
                                    serviceController.Start();
                                }
                                catch (Exception ex)
                                {
                                    QMessageBox.Show("Cannot resume the service: " + ex.Message, "Severe",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                finally
                                {
                                    serviceController.Dispose();
                                }
                            }
                            else
                            {
                                QMessageBox.Show("Refresh cannot be performed because the service is paused.",
                                    "Service Paused", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.buttonRefresh_Click");
                                return;
                            }
                            break;
                        }
                    case ServiceControllerStatus.ContinuePending:
                    case ServiceControllerStatus.PausePending:
                    case ServiceControllerStatus.StartPending:
                    case ServiceControllerStatus.StopPending:
                    default:
                        {
                            QMessageBox.Show("Refresh cannot be performed because the " + (isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") + " Service is " +
                                "in a Pending state. Please try again in a few minutes.", "Service State Pending",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            serviceController.Dispose();
                            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.buttonRefresh_Click");
                            return;
                        }
                }

                toolStripProgressBar1.Value = 5;
                listViewPhysicalDisks.Items.Clear();
                toolStripProgressBar1.Value = 10;
                listViewDiskInfo.Items.Clear();
                toolStripProgressBar1.Value = 15;
                listViewSmartDetails.Items.Clear();
                toolStripProgressBar1.Value = 20;
                LoadDataFromRegistry();
                toolStripProgressBar1.Value = 25;
                PostProcessAndUpdateDiskHealth(false);
                toolStripProgressBar1.Value = 30;

                // Refresh everything to update the view.

                try
                {
                    processingThread = new Thread(new ParameterizedThreadStart(ReapPhysicalDiskInfo));
                    processingThread.Name = "Initial Process Thread";
                    processingThread.Start(new bool[] { false, true });
                }
                catch (Exception ex)
                {
                    isWorkerProcessRunning = false;
                    toolStripProgressBar1.Value = 0;
                    QMessageBox.Show(Properties.Resources.ErrorMessageWorkerProcessStartFail + ex.Message, Properties.Resources.ErrorMessageTitleSevere,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    lastSelectedPath = "65535";
                }
            }
            catch (Exception ex)
            {
                toolStripProgressBar1.Value = 0;
                EpicFail fail;
                if (isWindowsServerSolutions)
                {
                    fail = new EpicFail(ex, "An internal error has occurred executing a manual disk polling operation.",
                        "If you just performed a \"Reset EVERYTHING\" operation, you MUST restart the Dashboard. Please close and re-launch " +
                        "the Dashboard. If you restarted the Home Server SMART Service or performed a \"Restore Defaults\" then it is recommended " +
                        "that you close and re-launch the Dashboard.", true);
                }
                else
                {
                    fail = new EpicFail(ex, "An internal error has occurred while executing a manual disk polling operation.",
                        "If you just performed a \"Reset EVERYTHING\" operation and have not yet restarted WindowSMART 24/7, you must restart WindowSMART. " +
                        "Please close and re-launch WindowSMART. If you are seeing this message and recently restarted the WindowSMART Service or " +
                        "performed a \"Restore Defaults\" operation, then it is recommended that you close and re-launch WindowSMART.", false);
                }
                fail.ShowDialog();
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.buttonRefresh_Click");
        }

        private void buttonIgnoreProblem_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.buttonIgnoreProblem_Click");
            if (listViewPhysicalDisks.SelectedItems != null && listViewPhysicalDisks.SelectedItems.Count != 0 &&
                listViewSmartDetails.SelectedItems != null && listViewSmartDetails.SelectedItems.Count != 0)
            {
                ListViewItem item = listViewSmartDetails.SelectedItems[0];
                if (QMessageBox.Show("Ignoring the problem " + item.SubItems[4].Text + " will only ignore the current " +
                    "raw number of the problem. " + (isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") +
                    " will no longer generate alerts for this item unless one of " +
                    "the following two conditions (or both) is true:\n\n     1) The raw value of this problem increases.\n" +
                    "     2) The SMART threshold of this value is met or exceeded.\n\nIf the number of problems (raw value) " +
                    "increases, you can elect to ignore the problem (not recommended). If the SMART threshold is met or " +
                    "exceeded, you will no longer be allowed to ignore the problem.\n\nBE WARNED: Choosing to ignore a " +
                    "problem with " + item.SubItems[4].Text + " is risky and may only be hiding a potentially serious " +
                    "problem. Ignoring a problem is NOT recommended. Problems you ignore will only stop generating alerts; " +
                    "they will still be flagged to you in the UI.\n\nAre you sure you want to ignore this problem?", "Warning!",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    int attributeID = 0;
                    long rawData = 0;
                    String model = String.Empty;
                    String path = String.Empty;

                    try
                    {
                        attributeID = Int32.Parse(item.SubItems[2].Text);
                        rawData = long.Parse(item.SubItems[11].Text, System.Globalization.NumberStyles.AllowHexSpecifier);
                        path = listViewPhysicalDisks.SelectedItems[0].SubItems[0].Text;
                        model = listViewPhysicalDisks.SelectedItems[0].SubItems[1].Text;

                        SiAuto.Main.LogMessage("Ignoring a problem on attribute " + attributeID.ToString());
                        SiAuto.Main.LogLong("rawData", rawData);
                        SiAuto.Main.LogString("path", path);
                        SiAuto.Main.LogString("model", model);

                        switch (attributeID)
                        {
                            case 5: // Bad Sectors
                                {
                                    DiskEnumerator.IgnoreBadSectors(path, model, rawData);
                                    break;
                                }
                            case 10: // Spin Retries
                                {
                                    DiskEnumerator.IgnoreSpinRetries(path, model, rawData);
                                    break;
                                }
                            case 184: // End-to-End Errors
                                {
                                    DiskEnumerator.IgnoreEndToEndErrors(path, model, rawData);
                                    break;
                                }
                            case 196: // Reallocations
                                {
                                    DiskEnumerator.IgnoreReallocations(path, model, rawData);
                                    break;
                                }
                            case 197: // Pending Bad Sectors
                                {
                                    DiskEnumerator.IgnorePendingSectors(path, model, rawData);
                                    break;
                                }
                            case 198: // Offline Bad Sectors
                                {
                                    DiskEnumerator.IgnoreOfflineBadSectors(path, model, rawData);
                                    break;
                                }
                            case 199: // CRC Errors
                                {
                                    DiskEnumerator.IgnoreCrcErrors(path, model, rawData);
                                    break;
                                }
                        }
                        QMessageBox.Show("You are now ignoring for the selected disk:\n\n     Attribute: " + item.SubItems[4].Text + "\n" +
                            "     Event Count: " + rawData.ToString() + "\n\nThe event will be cleared from WHS the next time the " +
                            (isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") + " Service polls your disks.",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        QMessageBox.Show("Failed to ignore " + item.SubItems[4].Text + ":" + ex.Message, "Severe",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.buttonIgnoreProblem_Click");
        }

        private void buttonSmartStatus_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.buttonSmartStatus_Click");
            GanderizeDiskStatus();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.buttonSmartStatus_Click");
        }

        private void buttonIgnoreDisk_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.buttonIgnoreDisk_Click");
            if (listViewPhysicalDisks.SelectedItems != null && listViewPhysicalDisks.SelectedItems.Count != 0)
            {
                String path = listViewPhysicalDisks.SelectedItems[0].SubItems[0].Text;
                String model = listViewPhysicalDisks.SelectedItems[0].SubItems[1].Text;
                String capacity = listViewPhysicalDisks.SelectedItems[0].SubItems[2].Text;

                if (QMessageBox.Show("Ignoring an entire disk is NOT recommended. You should ignore a disk ONLY if " +
                    (isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") +
                    " is displaying information you know to be incorrect. In most cases, " + (isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") +
                    " will not display " +
                    "any data at all (such as on a flash drive), and the disk will show an \"Unknown\" SMART status, rather than displaying incorrect data. " +
                    "However, if you have confirmed that " + (isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") +
                    " is in fact displaying incorrect data for this disk, " +
                    "you may wish to ignore it, particularly if " + (isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") +
                    " is generating alerts for it. You should NEVER " +
                    "ignore a disk for the sake of hiding real problems.\n\nIf you choose to ignore this disk, it will no longer " +
                    "appear in the list of physical disks and alerts will never be generated for it, even if the disk has health " +
                    "problems.\n\nYou have selected to ignore disk:\n     Path: " + path + "\n     Model: " + model + "\n     Capacity: " + capacity +
                    "\n\nAre you sure you want to ignore this disk and remove it from SMART monitoring?", "Warning!",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    try
                    {
                        if (QMessageBox.Show("Are you really sure you want to ignore this disk?", "Confirm Ignoring of Disk",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            DiskEnumerator.IgnoreDisk(path, model);
                            listViewPhysicalDisks.SelectedItems[0].Remove();
                            QMessageBox.Show("You are now ignoring the selected disk:\n\n     Path: " + path + "\n     Model: " + model +
                                "\n     Capacity: " + capacity + "\n\nAny active alerts for this disk will be cleared the next time the " +
                                (isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") + " Service polls your disks.",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        QMessageBox.Show("Failed to ignore " + model + ":" + ex.Message, "Severe",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.buttonIgnoreDisk_Click");
        }

        private void buttonDocumentation_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.buttonDocumentation_Click");
            if (isWindowsServerSolutions)
            {
                Help.ShowHelp(this, System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Dojo North Software\\Home Server SMART 2012\\HomeServerSMART.chm");
            }
            else
            {
                Help.ShowHelp(this, "WindowSMART.chm");
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.buttonDocumentation_Click");
        }

        private void buttonFastRefresh_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.buttonFastRefresh_Click");
            FastRefreshDisks();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.buttonFastRefresh_Click");
        }

        private void FastRefreshDisks()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.FastRefreshDisks");
            buttonRefresh.Enabled = false;
            buttonFastRefresh.Enabled = false;
            buttonIgnoreDisk.Enabled = false;
            buttonExport.Enabled = false;
            listViewPhysicalDisks.Items.Clear();

            try
            {
                // Browse all WMI physical disks.
                String wmiQuery = isWindows8 ? Properties.Resources.WmiQueryStringWin8 : Properties.Resources.WmiQueryStringNonWin8;
                String queryScope = isWindows8 ? Properties.Resources.WmiQueryScope : Properties.Resources.WmiQueryScopeDefault;
                String friendlyName = String.Empty;

                int phantomIndex = 0;

                foreach (ManagementObject drive in new ManagementObjectSearcher(queryScope, wmiQuery).Get())
                {
                    bool isPhantom = false;
                    // NULL check for Storage Spaces phantom disks.
                    if (drive["DeviceID"] == null)
                    {
                        friendlyName = String.Empty;
                        try
                        {
                            if (drive["FriendlyName"] == null)
                            {
                                SiAuto.Main.LogWarning("[Storage Spaces] Phantom Disk detected. Name unknown.");
                            }
                            else
                            {
                                friendlyName = drive["FriendlyName"].ToString();
                                SiAuto.Main.LogWarning("[Storage Spaces] Phantom Disk detected: " + friendlyName);
                            }
                        }
                        catch
                        {
                        }
                        isPhantom = true;
                    }
                    decimal capacity = 0.00M;
                    decimal giggage = 0.00M;
                    if (drive["Size"] != null)
                    {
                        giggage = Decimal.Parse(drive["Size"].ToString());
                    }
                    else
                    {
                        giggage = 0.00M;
                    }
                    capacity = Decimal.Round(giggage / 1073741824.00M, 2);

                    String model = String.Empty;
                    if (isPhantom)
                    {
                        SiAuto.Main.LogMessage("Disk is phantom; cannot get preferred model. Will use " + friendlyName + " instead.");
                        model = friendlyName;
                    }
                    else
                    {
                        SiAuto.Main.LogBool("isWindows8", isWindows8);
                        SiAuto.Main.LogMessage("Attempt to get preferred model name.");
                        model = Utilities.Utility.GetDiskPreferredModel(drive, isWindows8);
                        if (isWindows8)
                        {
                            if (String.IsNullOrEmpty(model))
                            {
                                SiAuto.Main.LogMessage("The model name returned was null or empty; trying to concatenate Manufacturer + Model via WMI.");
                                try
                                {
                                    model = drive["Manufacturer"].ToString() + drive["Model"].ToString();
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                SiAuto.Main.LogString("Model name was returned.", model);
                            }
                        }

                        try
                        {
                            if (String.IsNullOrEmpty(model))
                            {
                                SiAuto.Main.LogWarning("Model is still null or empty; trying additional methods.");
                                if (String.IsNullOrEmpty(model) && drive["Model"] != null)
                                {
                                    SiAuto.Main.LogMessage("Trying non-null Model.");
                                    model = drive["Model"].ToString();
                                }
                                else if (String.IsNullOrEmpty(model) && drive["Model"] == null && drive["PNPDeviceID"] != null)
                                {
                                    SiAuto.Main.LogMessage("Trying non-null PNPDeviceID.");
                                    model = drive["PNPDeviceID"].ToString() + " " + capacity.ToString() + " GB";
                                }
                                else if (String.IsNullOrEmpty(model))
                                {
                                    SiAuto.Main.LogMessage("No satisfactory name exists; using Undefined.");
                                    model = "Undefined " + capacity.ToString() + " GB";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogWarning(ex.Message);
                            SiAuto.Main.LogException(ex);
                            model = "Exception " + capacity.ToString() + " GB (Possible VD)";
                        }
                    }

                    if (ignoreVirtualDisks && KnownVirtualDisks.IsDiskOnVirtualDiskList(model))
                    {
                        // It's a virtual disk we're ignoring, so skip it (continue).
                        continue;
                    }

                    bool isIgnored = false;
                    int celsiusTemperature = 0;
                    DiskStatus status = DiskStatus.Unknown;

                    if (isPhantom)
                    {
                        status = DiskStatus.Critical;
                        celsiusTemperature = 0;
                    }
                    else
                    {
                        status = GetDiskStatusFromRegistry(drive, out isIgnored, out celsiusTemperature);
                        if (isIgnored)
                        {
                            continue;
                        }
                    }

                    String temperatureString = String.Empty;
                    if (celsiusTemperature == 0)
                    {
                        temperatureString = "N/A";
                    }
                    else
                    {
                        switch (temperaturePreference)
                        {
                            case "F":
                                {
                                    int fTemp = (celsiusTemperature * 9 / 5) + 32;
                                    temperatureString = fTemp.ToString() + Properties.Resources.SettingsDegreesF;
                                    break;
                                }
                            case "K":
                                {
                                    int kTemp = celsiusTemperature + 273;
                                    temperatureString = kTemp.ToString() + Properties.Resources.SettingsDegreesK;
                                    break;
                                }
                            case "C":
                            default:
                                {
                                    temperatureString = celsiusTemperature.ToString() + Properties.Resources.SettingsDegreesC;
                                    break;
                                }
                        }
                    }

                    FancyListView.ImageSubItem modelSubItem = new FancyListView.ImageSubItem();
                    switch (status)
                    {
                        case DiskStatus.Critical:
                            {
                                modelSubItem.ImageKey = "Critical";
                                break;
                            }
                        case DiskStatus.Warning:
                            {
                                modelSubItem.ImageKey = "Warning";
                                break;
                            }
                        case DiskStatus.Geriatric:
                            {
                                modelSubItem.ImageKey = "Geriatric";
                                break;
                            }
                        case DiskStatus.Healthy:
                            {
                                modelSubItem.ImageKey = "Healthy";
                                break;
                            }
                        case DiskStatus.Unknown:
                        default:
                            {
                                modelSubItem.ImageKey = "Unknown";
                                break;
                            }
                    }
                    modelSubItem.Text = model;

                    String diskPathName = String.Empty;
                    if (isPhantom)
                    {
                        diskPathName = "\\\\.\\PHANTOMDISK" + phantomIndex.ToString();
                    }
                    else
                    {
                        diskPathName = (isWindows8 ? "\\\\.\\PHYSICALDRIVE" + drive["DeviceID"].ToString() : drive["DeviceID"].ToString());
                    }
                    decimal columnTag = 0;

                    if (isPhantom)
                    {
                        columnTag = phantomIndex + 65535;
                    }
                    else
                    {
                        columnTag = Utilities.Utility.GetDriveIdFromPath(diskPathName);
                    }

                    if (columnTag == -1)
                    {
                        columnTag = 0;
                    }

                    SiAuto.Main.LogDecimal("columnTag", columnTag);

                    //ListViewItem item = new ListViewItem(new String[] { drive["DeviceID"].ToString() });
                    ListViewItem item = new ListViewItem(new String[] { diskPathName });
                    item.SubItems.Add(modelSubItem);
                    item.SubItems[0].Tag = columnTag;
                    item.SubItems.Add(capacity.ToString() + " GB");
                    item.SubItems[2].Tag = giggage;
                    item.SubItems.Add(temperatureString);
                    item.SubItems[3].Tag = celsiusTemperature;

                    AddListViewItemToPhysicalDisks(item);
                }
                lastSelectedPath = "65535";
                SelectFirstPhysicalDisk();
            }
            catch (Exception ex)
            {
                QMessageBox.Show("The refresh failed. " + ex.Message, "Refresh Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                buttonRefresh.Enabled = true;
                buttonFastRefresh.Enabled = true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.FastRefreshDisks");
        }

        private void GanderizeDiskStatus()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.GanderizeDiskStatus");
            SmartStatus stat = new SmartStatus(useDefaultSkinning);

            if (isDiskCritical)
            {
                stat.SetWindowTitle(Properties.Resources.WindowTitleCritical, isDiskCritical, isDiskWarning);
            }
            else if (isDiskWarning)
            {
                stat.SetWindowTitle(Properties.Resources.WindowTitleWarning, isDiskCritical, isDiskWarning);
            }
            else if (isDiskGeriatric)
            {
                stat.SetWindowTitle(Properties.Resources.WindowTitleGeriatric, isDiskCritical, isDiskGeriatric);
            }
            else if (isUnknown)
            {
                stat.SetWindowTitle(Properties.Resources.WindowTitleUnknown, isDiskCritical, isUnknown, isWmiFailurePredicted, true);
            }
            else
            {
                stat.SetWindowTitle(Properties.Resources.WindowTitleHealthy, false, false);
            }

            // REPORTING PRECEDENCE:
            // 1. Exceeded health thresholds (FAIL)
            // 2. Bad Sectors, End-to-End Errors, Pending Sectors, Reallocation Events, Uncorrectable Sectors, Spin Retry
            // 2a. Same as 2, except at "warning" level
            // 3. Ultra ATA CRC Errors
            // 4. Temperature
            // 5. Geriatrics

            // The SERIOUS Stuff:
            if (isThresholdExceeded)
            {
                // If the disk is not Critical, that means we have a MET (but not EXCEEDED) threshold. So display it as a warning.
                if (isDiskCritical)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleThresholdExceeded, Properties.Resources.SmartWindowStatusMsgThresholdExceeded,
                        true, false);
                }
                else
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleThresholdExceeded, Properties.Resources.SmartWindowStatusMsgThresholdExceeded,
                        false, true);
                }
            }

            if (isSsd)
            {
                if (badSectorCountOrRetiredBlockCount >= ssdRetirementCritical)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleRetiredBlocks, "There are " + badSectorCountOrRetiredBlockCount.ToString() + " retired blocks in " +
                        "the SSD flash memory. Their contents have been reallocated to the spare area.", true, false);
                }

                if (ssdControllerManufacturer.ToUpper() == "STEC" && endToEndErrorCountOrLifeCurve >= 5)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleEndToEndErrors, Properties.Resources.SmartWindowStatusMsgEndToEndMajor,
                        true, false);
                }

                if (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftCritical && ssdControllerManufacturer.ToUpper() != "STEC" && ssdControllerManufacturer.ToUpper() != "JMICRON" &&
                    ssdControllerManufacturer.ToUpper() != "KINGSPEC")
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleLowLifeLeft, "SSD estimated life remaining is critically low. Estimated SSD Life Left is " +
                        pendingSectorCountOrSsdLifeLeft.ToString() + "%. You should strongly consider replacing the disk.", true, false);
                }
            }
            else
            {
                if (badSectorCountOrRetiredBlockCount >= 50)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleBadSectors, "There are " + badSectorCountOrRetiredBlockCount.ToString() + " bad sectors on " +
                        "the disk surface. Their contents have been reallocated to the spare area.", true, false);
                }

                if (endToEndErrorCountOrLifeCurve >= 3)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleEndToEndErrors, Properties.Resources.SmartWindowStatusMsgEndToEndMajor,
                        true, false);
                }

                if (reallocationEventCount >= 50)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleLargeReallocation, "There have been " + reallocationEventCount.ToString() +
                        " attempted sector reallocation events on the disk.", true, false);
                }

                if (pendingSectorCountOrSsdLifeLeft >= 30)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleLargePending, "There are " + pendingSectorCountOrSsdLifeLeft.ToString() + " sectors " +
                        "that are waiting to be reallocated.", true, false);
                }

                if (uncorrectableSectorCount >= 50)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleLargeUncorrectable, "There are " + uncorrectableSectorCount.ToString() +
                        " uncorrectable sector errors on the disk.", true, false);
                }

                if (spinRetryCount >= 8)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleHighSpinRetry, "The drive motor has encountered difficulty spinning up " +
                        spinRetryCount.ToString() + " times.", true, false);
                }

                if (temperature >= criticalTemperatureThreshold && temperature <= absurdTemperatureThreshold)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleCriticalTemp, Properties.Resources.SmartWindowStatusMsgCriticalTemperature, true, false);
                }

                if (airflowTemperature >= criticalTemperatureThreshold && airflowTemperature <= absurdTemperatureThreshold)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleCriticalAirflow, Properties.Resources.SmartWindowStatusMsgCriticalAirflow, true, false);
                }
            }

            if (isDiskCritical)
            {
                stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleHealthCritical, Properties.Resources.SmartWindowStatusMsgHealthCritical, true, false);
            }

            // WARNINGS
            if (isSsd)
            {
                if (badSectorCountOrRetiredBlockCount >= ssdRetirementWarning && badSectorCountOrRetiredBlockCount < ssdRetirementCritical)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleRetiredBlocks, "There are " + badSectorCountOrRetiredBlockCount.ToString() + " retired blocks in " +
                        "the SSD flash memory. Their contents have been reallocated to the spare area.", false, true);
                }

                if (ssdControllerManufacturer.ToUpper() == "STEC" && endToEndErrorCountOrLifeCurve > 0 && endToEndErrorCountOrLifeCurve < 3)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleEndToEndErrors, Properties.Resources.SmartWindowStatusMsgEndToEndMinor,
                        false, true);
                }

                if (ssdControllerManufacturer.ToUpper() != "STEC" && (pendingSectorCountOrSsdLifeLeft <= ssdLifeLeftWarning && pendingSectorCountOrSsdLifeLeft > ssdLifeLeftCritical))
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleLowLifeLeft, "The SSD is estimated to be in the final 1/3 of its life, based on the media wearout " +
                        "indicator. Estimated SSD Life Left is " + pendingSectorCountOrSsdLifeLeft.ToString() + "%. If you consistently perform a lot of write-intensive activities " +
                        "on this disk, you may burn through it quickly. On the other hand, if you've had the disk for at least a couple of years, then it may still perform reliably " +
                        "for another year or more.", false, true);
                }
            }
            else
            {
                if (badSectorCountOrRetiredBlockCount > 0 && badSectorCountOrRetiredBlockCount < 50)
                {
                    if (badSectorCountOrRetiredBlockCount > 1)
                    {
                        stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleBadSectors, "There are " + badSectorCountOrRetiredBlockCount.ToString() + " bad sectors on " +
                            "the disk surface. Their contents have been reallocated to the spare area.", false, true);
                    }
                    else
                    {
                        stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleBadSectors, "There is " + badSectorCountOrRetiredBlockCount.ToString() + " bad sector on " +
                            "the disk surface. Its contents havs been reallocated to the spare area.", false, true);
                    }
                }

                if (endToEndErrorCountOrLifeCurve > 0 && endToEndErrorCountOrLifeCurve < 3)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleEndToEndErrors, Properties.Resources.SmartWindowStatusMsgEndToEndMinor,
                        false, true);
                }

                if (reallocationEventCount > 0 && reallocationEventCount < 50)
                {
                    if (reallocationEventCount > 1)
                    {
                        stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleReallocations, "There have been " + reallocationEventCount.ToString() +
                            " attempted sector reallocation events on the disk.", false, true);
                    }
                    else
                    {
                        stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleReallocations, "There has been " + reallocationEventCount.ToString() +
                            " attempted sector reallocation event on the disk.", false, true);
                    }
                }

                if (pendingSectorCountOrSsdLifeLeft > 0 && pendingSectorCountOrSsdLifeLeft < 30)
                {
                    if (pendingSectorCountOrSsdLifeLeft > 1)
                    {
                        stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitlePending, "There are " + pendingSectorCountOrSsdLifeLeft.ToString() + " sectors " +
                            "that are waiting to be reallocated.", false, true);
                    }
                    else
                    {
                        stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitlePending, "There is " + pendingSectorCountOrSsdLifeLeft.ToString() + " sector " +
                            "that is waiting to be reallocated.", false, true);
                    }
                }

                if (uncorrectableSectorCount > 0 && uncorrectableSectorCount < 50)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleUncorrectable, "There are " + uncorrectableSectorCount.ToString() +
                        " uncorrectable sector errors on the disk.", false, true);
                }

                if (spinRetryCount > 3 && spinRetryCount < 8)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleSpinRetries, "The drive motor has encountered difficulty spinning up " +
                        spinRetryCount.ToString() + " times.", false, true);
                }

                if (ultraAtaCrcErrorCount > 50)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleLargeCrc, "There have been " + ultraAtaCrcErrorCount.ToString() +
                        " CRC errors detected on the disk. Check for bad or oxidized cables or connectors.", false, true);
                }
            }

            if (isDiskTemperatureInvalid)
            {
                stat.AddItemToPanel(Properties.Resources.SmartTemperatureStatusAbsurd, Properties.Resources.SmartWindowStatusMsgAbsurdTemperature, false, true);
            }

            if (!isDiskCritical && isDiskWarning)
            {
                stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleHealthWarning, Properties.Resources.SmartWindowStatusMsgHealthWarning, false, true);
            }

            if (temperature >= overheatedTemperatureThreshold && temperature < criticalTemperatureThreshold)
            {
                stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleOverheated, Properties.Resources.SmartWindowStatusMsgOverheatedTemperature,
                    true, false);
            }
            else if (temperature >= hotTemperatureThreshold && temperature < criticalTemperatureThreshold && !ignoreHot)
            {
                stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleHot, Properties.Resources.SmartWindowStatusMsgHotTemperature, false, true);
            }
            else if (temperature >= warmTemperatureThreshold && temperature < criticalTemperatureThreshold && !ignoreWarm)
            {
                stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleWarm, Properties.Resources.SmartWindowStatusMsgWarmTemperature, false, true);
            }

            if (airflowTemperature >= overheatedTemperatureThreshold && airflowTemperature < criticalTemperatureThreshold)
            {
                stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleOverheated, Properties.Resources.SmartWindowStatusMsgOverheatedAirflow,
                    true, false);
            }
            else if (airflowTemperature >= hotTemperatureThreshold && airflowTemperature < criticalTemperatureThreshold && !ignoreHot)
            {
                stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleHot, Properties.Resources.SmartWindowStatusMsgHotAirflow, false, true);
            }
            else if (airflowTemperature >= warmTemperatureThreshold && airflowTemperature < criticalTemperatureThreshold && !ignoreWarm)
            {
                stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleWarm, Properties.Resources.SmartWindowStatusMsgWarmAirflow, false, true);
            }

            if (!isDiskCritical && !isDiskWarning && isDiskGeriatric)
            {
                stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleGeriatric, Properties.Resources.SmartWindowStatusMsgHealthGeriatric, false, true);
            }

            if (!isDiskCritical && !isDiskWarning && !isUnknown)
            {
                stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleHealthHealthy, Properties.Resources.SmartWindowStatusMsgHealthHealthy, false, false);
            }
            else if (!isDiskCritical && !isDiskWarning && isUnknown)
            {
                if (isWmiFailurePredicted)
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleWmiFailTrue, Properties.Resources.SmartWindowStatusMsgWmiFailTrue, true, false);
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleHealthUnknown, Properties.Resources.SmartWindowStatusMsgHealthUnknown,
                        false, true);
                }
                else
                {
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleHealthUnknown, Properties.Resources.SmartWindowStatusMsgHealthUnknown,
                        false, true);
                    stat.AddItemToPanel(Properties.Resources.SmartWindowStatusTitleWmiFailFalse, Properties.Resources.SmartWindowStatusMsgWmiFailFalse, false, false);
                }
            }

            // SSDs with wearout indicators / life remaining
            if (isSsd)
            {
                String header = "The SSD Life Left percentage is calculated based on a media wearout attribute, and gives an estimate of how many additional " +
                    "write operations can be performed on the SSD. Estimated Life Left: {0}% determined by attribute {1} {2}.";
                String lifeLeft = String.Empty;
                switch (ssdControllerManufacturer.ToUpper())
                {
                    case "MICRON":
                        {
                            lifeLeft = String.Format(header, pendingSectorCountOrSsdLifeLeft, "202", "Percentage of the Rated Lifetime Used");
                            break;
                        }
                    case "INDILINX":
                        {
                            lifeLeft = String.Format(header, pendingSectorCountOrSsdLifeLeft, "209", "Remaining Life (%)");
                            break;
                        }
                    case "INDILINX EVEREST":
                    case "INDILINX BAREFOOT 3":
                    case "INTEL":
                        {
                            lifeLeft = String.Format(header, pendingSectorCountOrSsdLifeLeft, "233", "Media Wearout Indicator");
                            break;
                        }
                    case "MARVELL":
                    case "SAMSUNG":
                        {
                            lifeLeft = String.Format(header, pendingSectorCountOrSsdLifeLeft, "177", "Wear Leveling Count");
                            break;
                        }
                    case "SANDFORCE":
                    case "LAMD":
                    case "SMARTBUY/PHISON":
                        {
                            lifeLeft = String.Format(header, pendingSectorCountOrSsdLifeLeft, "231", "SSD Life Left");
                            break;
                        }
                    case "SMART MODULAR":
                        {
                            lifeLeft = String.Format(header, pendingSectorCountOrSsdLifeLeft, "232", "Endurance Remaining");
                            break;
                        }
                    case "SANDISK":
                        {
                            lifeLeft = String.Format(header, pendingSectorCountOrSsdLifeLeft, "230/232", " Media Wearout Indicator/Spare Block Remaining (whichever is lower)");
                            break;
                        }
                    case "SK HYNIX":
                        {
                            lifeLeft = String.Format(header, pendingSectorCountOrSsdLifeLeft, "180", "Unused Reserved Block Count");
                            break;
                        }
                    case "TRANSCEND/SILICON MOTION":
                        {
                            lifeLeft = String.Format(header, pendingSectorCountOrSsdLifeLeft, "169", "Percentage of Life Remaining");
                            break;
                        }
                    case "TOSHIBA":
                        {
                            lifeLeft = String.Format(header, pendingSectorCountOrSsdLifeLeft, "173", "Media Wearout Indicator");
                            break;
                        }
                    default:  // JMICRON, KINGSPEC - no known indicator
                        {
                            break;
                        }
                }

                if (String.IsNullOrEmpty(lifeLeft))
                {
                    stat.AddItemToPanel("SSD Life Left", "This SSD has no SSD life left monitor or media wearout indicator, which is a value " + (isWindowsServerSolutions ?
                        "Home Server SMART" : "WindowSMART") + " can use to calculate the disk's health rating. As a result, only failing attributes and degradations " +
                        "in critical attributes such as Retired Sectors will affect the disk's health rating.", false, true);
                }
                else
                {
                    stat.AddItemToPanel("SSD Life Left", lifeLeft, false, false);
                }
            }

            if (isSsd && ssdControllerManufacturer.ToUpper() == "MICRON" && endToEndErrorCountOrLifeCurve > 0)
            {
                stat.AddItemToPanel("Wear Leveling", "Average erase count of all good blocks: " + endToEndErrorCountOrLifeCurve,
                    false, false);
            }
            else if (isSsd && ssdControllerManufacturer.ToUpper() == "SAMSUNG" && endToEndErrorCountOrLifeCurve > 0)
            {
                stat.AddItemToPanel("Runtime Bad Blocks", "Bad blocks detected at runtime: " + endToEndErrorCountOrLifeCurve,
                    false, true);
            }

            stat.ShowDialog();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.GanderizeDiskStatus");
        }

        private void SetWindowBackground()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.SetWindowBackground");
            switch (windowBackground)
            {
                case 1: // Lightning
                    {
                        if (isWindowsServerSolutions)
                        {
                            this.BackgroundImage = Properties.Resources.Lightning;
                            this.BackgroundImageLayout = ImageLayout.Stretch;
                        }
                        else
                        {
                            this.BackgroundImage = Properties.Resources.Lightning986;
                            this.BackgroundImageLayout = ImageLayout.Stretch;
                        }
                        labelAppTitle.ForeColor = Color.DarkMagenta;
                        labelBeta.ForeColor = Color.DarkMagenta;
                        labelSelectPhysical.ForeColor = Color.DarkMagenta;
                        labelDiskInfo.ForeColor = Color.DarkMagenta;
                        labelSmartDetails.ForeColor = Color.DarkMagenta;
                        labelDoubleClick.ForeColor = Color.DarkMagenta;
                        break;
                    }
                case 2:
                    {
                        if (isWindowsServerSolutions)
                        {
                            this.BackgroundImage = Properties.Resources.CrackedGlass;
                            this.BackgroundImageLayout = ImageLayout.Stretch;
                        }
                        else
                        {
                            this.BackgroundImage = Properties.Resources.CrackedGlass986;
                            this.BackgroundImageLayout = ImageLayout.Stretch;
                        }
                        labelAppTitle.ForeColor = Color.Black;
                        labelBeta.ForeColor = Color.Black;
                        labelSelectPhysical.ForeColor = Color.Black;
                        labelDiskInfo.ForeColor = Color.Black;
                        labelSmartDetails.ForeColor = Color.Black;
                        labelDoubleClick.ForeColor = Color.Black;
                        break;
                    }
                case 3:
                    {
                        if (isWindowsServerSolutions)
                        {
                            this.BackgroundImage = null;
                            this.BackgroundImageLayout = ImageLayout.None;
                        }
                        else
                        {
                            this.BackgroundImage = null;
                            this.BackgroundImageLayout = ImageLayout.None;
                        }
                        labelAppTitle.ForeColor = Color.Black;
                        labelBeta.ForeColor = Color.Black;
                        labelSelectPhysical.ForeColor = Color.Black;
                        labelDiskInfo.ForeColor = Color.Black;
                        labelSmartDetails.ForeColor = Color.Black;
                        labelDoubleClick.ForeColor = Color.Black;
                        break;
                    }
                case 0:
                default:
                    {
                        if (isWindowsServerSolutions)
                        {
                            this.BackgroundImage = Properties.Resources.MetalGrate;
                            this.BackgroundImageLayout = ImageLayout.Tile;
                        }
                        else
                        {
                            this.BackgroundImage = Properties.Resources.MetalGrate;
                            this.BackgroundImageLayout = ImageLayout.Tile;
                        }
                        labelAppTitle.ForeColor = Color.Black;
                        labelBeta.ForeColor = Color.Black;
                        labelSelectPhysical.ForeColor = Color.Black;
                        labelDiskInfo.ForeColor = Color.Black;
                        labelSmartDetails.ForeColor = Color.Black;
                        labelDoubleClick.ForeColor = Color.Black;
                        break;
                    }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.SetWindowBackground");
        }

        public void CheckForBackgroundChange()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.CheckForBackgroundChange");
            int oldBackground = windowBackground;
            try
            {
                windowBackground = (int)configurationKey.GetValue(Properties.Resources.RegistryConfigWindowBackground);
            }
            catch(Exception ex)
            {
                SiAuto.Main.LogWarning(ex.Message);
                windowBackground = 0;
            }

            if (oldBackground == windowBackground)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.CheckForBackgroundChange");
                return;
            }
            else
            {
                SetWindowBackground();
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.CheckForBackgroundChange");
        }

        private void menuItemExportHtml_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.menuItemExportHtml_Click");

            if (listViewPhysicalDisks.SelectedItems == null || listViewPhysicalDisks.SelectedItems.Count == 0)
            {
                SelectFirstPhysicalDisk();
            }

            String filename = GetExportFileName("Save Exported HTML Report", "HTML documents (*.html)|*.html|All files (*.*)|*.*", "html");
            if (String.IsNullOrEmpty(filename))
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.menuItemExportHtml_Click");
                return;
            }
            ExportSelectedDiskToHtml(filename);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.menuItemExportHtml_Click");
        }

        private void menuItemExportCsv_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.menuItemExportCsv_Click");

            if (listViewPhysicalDisks.SelectedItems == null || listViewPhysicalDisks.SelectedItems.Count == 0)
            {
                SelectFirstPhysicalDisk();
            }

            String filename = GetExportFileName("Save Exported CSV Report", "CSV files (*.csv)|*.csv|All files (*.*)|*.*", "csv");
            if (String.IsNullOrEmpty(filename))
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.menuItemExportCsv_Click");
                return;
            }
            ExportSelectedDiskToExcel(filename, false);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.menuItemExportCsv_Click");
        }

        private void menuItemExportTsv_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.menuItemExportTsv_Click");

            if (listViewPhysicalDisks.SelectedItems == null || listViewPhysicalDisks.SelectedItems.Count == 0)
            {
                SelectFirstPhysicalDisk();
            }

            String filename = GetExportFileName("Save Exported TSV Report", "TSV files (*.tsv)|*.tsv|All files (*.*)|*.*", "tsv");
            if (String.IsNullOrEmpty(filename))
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.menuItemExportTsv_Click");
                return;
            }
            ExportSelectedDiskToExcel(filename, true);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.menuItemExportTsv_Click");
        }

        private void menuItemExportText_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUiControl.menuItemExportText_Click");

            if (listViewPhysicalDisks.SelectedItems == null || listViewPhysicalDisks.SelectedItems.Count == 0)
            {
                SelectFirstPhysicalDisk();
            }

            String filename = GetExportFileName("Save Exported Text Report", "Text files (*.txt)|*.txt|All files (*.*)|*.*", "txt");
            if (String.IsNullOrEmpty(filename))
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.menuItemExportText_Click");
                return;
            }
            ExportSelectedDiskToText(filename);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUiControl.menuItemExportText_Click");
        }

        String GetExportFileName(String message, String filter, String defaultExt)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = message;
            sfd.Filter = filter;
            sfd.AddExtension = true;
            sfd.DefaultExt = defaultExt;
            sfd.ShowDialog();
            return sfd.FileName;
        }

        private bool GetDiskTestSupport(ListViewItem lvi, out String testStatus, out int shortLength, out int longLength, out int conveyanceLength)
        {
            shortLength = 0;
            longLength = 0;
            conveyanceLength = 0;
            bool isSelfTestDataAvailable = false;
            testStatus = "0";

            try
            {
                // Browse all WMI physical disks.
                String wmiQuery = isWindows8 ? Properties.Resources.WmiQueryStringWin8 : Properties.Resources.WmiQueryStringNonWin8;
                String queryScope = isWindows8 ? Properties.Resources.WmiQueryScope : Properties.Resources.WmiQueryScopeDefault;

                foreach (ManagementObject drive in new ManagementObjectSearcher(queryScope, wmiQuery).Get())
                {
                    SiAuto.Main.LogMessage("[Physical Disk] Enumerating WMI disks.");
                    String diskTestPath = String.Empty;
                    // NULL check for Storage Spaces phantom disks.
                    if (drive["DeviceID"] == null)
                    {
                        String friendlyName = String.Empty;
                        try
                        {
                            if (drive["FriendlyName"] == null)
                            {
                                SiAuto.Main.LogWarning("[Storage Spaces] Phantom Disk detected. Name unknown.");
                            }
                            else
                            {
                                friendlyName = drive["FriendlyName"].ToString();
                                SiAuto.Main.LogWarning("[Storage Spaces] Phantom Disk detected: " + friendlyName);
                            }
                        }
                        catch
                        {
                        }
                        continue;
                    }
                    if (isWindows8)
                    {
                        diskTestPath = "\\\\.\\PHYSICALDRIVE" + drive["DeviceID"].ToString();
                    }
                    else
                    {
                        diskTestPath = drive["DeviceID"].ToString();
                    }
                    if (String.Compare(diskTestPath, lvi.SubItems[0].Text, true) == 0)
                    {
                        bool isIgnored = false;
                        byte[] smartData = Utilities.Utility.GetDiskStatusFromRegistry(drive, isWindows8, out isIgnored);
                        if (isIgnored)
                        {
                            continue;
                        }

                        // Byte 363 of the SMART data holds the test status (7:4 - status code, 3:0 - percent remaining)
                        testStatus = "0";
                        try
                        {
                            testStatus = Utilities.Utility.GetTestStatus(smartData[363]);
                            isSelfTestDataAvailable = true;
                        }
                        catch
                        {
                            // No test data, status or support information, so skip.
                            continue;
                        }

                        try
                        {
                            shortLength = smartData[372];
                        }
                        catch
                        {
                            shortLength = 0;
                        }

                        try
                        {
                            longLength = smartData[373];
                        }
                        catch
                        {
                            longLength = 0;
                        }

                        try
                        {
                            conveyanceLength = smartData[374];
                        }
                        catch
                        {
                            conveyanceLength = 0;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return isSelfTestDataAvailable;
        }

        private void ExportSelectedDiskToHtml(String filename)
        {
            DiskInfo di = new DiskInfo();
            ListViewItem diskItem = null;
            if (listViewPhysicalDisks.SelectedItems == null || listViewPhysicalDisks.SelectedItems.Count == 0)
            {
                QMessageBox.Show("The view state is corrupt. Please re-poll or refresh the disks, then select a disk and try again.", "Invalid View State",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!di.Populate(listViewPhysicalDisks.SelectedItems[0].SubItems[0].Text))
            {
                return;
            }

            diskItem = listViewPhysicalDisks.SelectedItems[0];
            bool isSelfTestDataAvailable = false;
            int shortLength = 0;
            int longLength = 0;
            int conveyanceLength = 0;
            String testStatus = String.Empty;

            isSelfTestDataAvailable = GetDiskTestSupport(diskItem, out testStatus, out shortLength, out longLength, out conveyanceLength);

            // Build the HTML document.
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            sb.AppendLine("<title>Disk Health Summary</title>");
            sb.AppendLine("");
            sb.AppendLine("<!-- CSS Section -->");
            sb.AppendLine("<style type=\"text/css\">");
            sb.AppendLine("body");
            sb.AppendLine("{");
            sb.AppendLine("	background-color: #FFFFFF;");
            sb.AppendLine("	margin: 5px;");
            sb.AppendLine("	font-family: \"Arial\", Geneva, Sans-Serif;");
            sb.AppendLine("	color: #000000;");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("h1");
            sb.AppendLine("{");
            sb.AppendLine("	font-family: \"Arial\", Geneva, Sans-Serif;");
            sb.AppendLine("	font-size: 22px;");
            sb.AppendLine("	color: Blue;");
            sb.AppendLine("	text-align: center;");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("h2");
            sb.AppendLine("{");
            sb.AppendLine("	font-family: \"Arial\", Geneva, Sans-Serif;");
            sb.AppendLine("	font-size: 18px;");
            sb.AppendLine("	color: Black;");
            sb.AppendLine(" text-align: center;");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("h3");
            sb.AppendLine("{");
            sb.AppendLine("	font-family: \"Arial\", Geneva, sans-serif;");
            sb.AppendLine("	font-size: 16px;");
            sb.AppendLine("	color: Black;");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine(".finePrint");
            sb.AppendLine("{");
            sb.AppendLine("	font-family: \"Arial\", Geneva, sans-serif;");
            sb.AppendLine("	font-size: 10px;");
            sb.AppendLine("	color: Black;");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine(".finePrintSuper");
            sb.AppendLine("{");
            sb.AppendLine("	font-family: \"Arial\", Geneva, sans-serif;");
            sb.AppendLine("	font-size: 10px;");
            sb.AppendLine("	color: Black;");
            sb.AppendLine(" vertical-align: super;");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine(".tableCaption");
            sb.AppendLine("{");
            sb.AppendLine("	font-family: \"Arial\", Geneva, Sans-Serif;");
            sb.AppendLine("	font-size: 12px;");
            sb.AppendLine("	font-weight: bold;");
            sb.AppendLine("	color: Black;");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine(".tableContent");
            sb.AppendLine("{");
            sb.AppendLine("	font-family: \"Arial\", Geneva, Sans-Serif;");
            sb.AppendLine("	font-size: 12px;");
            sb.AppendLine("	color: Black;");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine(".tableContentHealthy");
            sb.AppendLine("{");
            sb.AppendLine("	font-family: \"Arial\", Geneva, Sans-Serif;");
            sb.AppendLine("	font-size: 12px;");
            sb.AppendLine("	font-weight:: bold;");
            sb.AppendLine("	color: Green;");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine(".tableContentDegraded");
            sb.AppendLine("{");
            sb.AppendLine("	font-family: \"Arial\", Geneva, Sans-Serif;");
            sb.AppendLine("	font-size: 12px;");
            sb.AppendLine("	font-weight:: bold;");
            sb.AppendLine("	color: #F60;");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine(".tableContentCritical");
            sb.AppendLine("{");
            sb.AppendLine("	font-family: \"Arial\", Geneva, Sans-Serif;");
            sb.AppendLine("	font-size: 12px;");
            sb.AppendLine("	font-weight:: bold;");
            sb.AppendLine("	color: Red;");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine(".tableContentGeezery");
            sb.AppendLine("{");
            sb.AppendLine("	font-family: \"Arial\", Geneva, Sans-Serif;");
            sb.AppendLine("	font-size: 12px;");
            sb.AppendLine("	font-weight:: bold;");
            sb.AppendLine("	color: Blue;");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("</style>");
            sb.AppendLine("");
            sb.AppendLine("</head>");
            sb.AppendLine("");
            sb.AppendLine("<body>");
            sb.AppendLine("<h2>" + (isWindowsServerSolutions ? "Home Server SMART " : "WindowSMART ") + "24/7</h2>");
            sb.AppendLine("<h1>SMART Disk Details Report</h1>");
            sb.AppendLine("<h3>Computer Information</h3>");
            sb.AppendLine("<table width=\"100%\" border=\"1\" cellpadding=\"2\" summary=\"Contains information about the computer from which the export is taken.\">");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">Computer Name</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + Environment.MachineName + "</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">Operating System</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + OSInfo.Name + "</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">OS Edition</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + OSInfo.Edition + "</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">Service Pack</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + OSInfo.ServicePack + "</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">Version</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + OSInfo.VersionString + "</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">System Type</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + OSInfo.Bits + "-bit Operating System</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("</table>");
            sb.AppendLine("<p>&nbsp;</p>");
            sb.AppendLine("<h3>Disk Information</h3>");
            sb.AppendLine("<table width=\"100%\" border=\"1\" cellpadding=\"2\" summary=\"Contains information about the disk being analyzed.\">");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">Disk Path</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.Name + "</td>");
            sb.AppendLine("    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Advertised Capacity</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.AdvertisedCapacity + " GB</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">Model</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.Model + "</td>");
            sb.AppendLine("    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Real Capacity</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.RealCapacity + " GB</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">Serial Number</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.SerialNumber + "</td>");
            sb.AppendLine("    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Bytes Per Sector</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.BytesPerSector + "<span style=\"vertical-align: super\">&dagger;</span></td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">Firmware Revision</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.FirmwareRev + "</td>");
            sb.AppendLine("    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Cylinders</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.Cylinders + "</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">Description</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.Description + "</td>");
            sb.AppendLine("    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Heads</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.Heads + "</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">Interface</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.InterfaceType + "</td>");
            sb.AppendLine("    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Total Sectors</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.TotalSectors + "</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">Media Type</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.MediaType + "</td>");
            sb.AppendLine("    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Tracks</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.Tracks + "</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">Partition Count</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.PartitionCount + "</td>");
            sb.AppendLine("    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Tracks Per Cylinder</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.TracksPerCylinder + "</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">Status</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.Status + "</td>");
            sb.AppendLine("    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Total Bytes</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + di.TotalBytes + "</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">Failure Predicted</td>");
            sb.AppendLine("    <td class=\"tableContent\">" + ((String.Compare(di.FailurePredicted, "True", true) == 0) ? "Yes" : "No") + "</td>");
            sb.AppendLine("    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            sb.AppendLine("    <td class=\"tableCaption\">&nbsp;</td>");
            sb.AppendLine("    <td class=\"tableContent\">&nbsp;</td>");
            sb.AppendLine("  </tr>");

            if (di.IsSsd)
            {
                sb.AppendLine("  <tr>");
                sb.AppendLine("    <td class=\"tableCaption\">Solid State Disk</td>");
                sb.AppendLine("    <td class=\"tableContent\">Yes</td>");
                sb.AppendLine("    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>");
                sb.AppendLine("    <td class=\"tableCaption\">SSD Controller</td>");
                sb.AppendLine("    <td class=\"tableContent\">" + di.SsdControllerManufacturer + "</td>");
                sb.AppendLine("  </tr>");
                sb.AppendLine("  <tr>");
                sb.AppendLine("    <td class=\"tableCaption\">TRIM Supported</td>");
                sb.AppendLine("    <td class=\"tableContent\">" + ((String.Compare(di.IsTrimSupported.ToString(), "True", true) == 0) ? "Yes" : "No") + "<span style=\"vertical-align: super\">&Dagger;</span></td>");
                sb.AppendLine("    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>");
                sb.AppendLine("    <td class=\"tableCaption\">&nbsp;</td>");
                sb.AppendLine("    <td class=\"tableContent\">&nbsp;</td>");
                sb.AppendLine("  </tr>");
            }
            else
            {
                sb.AppendLine("  <tr>");
                sb.AppendLine("    <td class=\"tableCaption\">Solid State Disk</td>");
                sb.AppendLine("    <td class=\"tableContent\">No (Hard Disk)</td>");
                sb.AppendLine("    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>");
                sb.AppendLine("    <td class=\"tableCaption\">Spindle Speed</td>");
                
                if (di.SsdControllerManufacturer == "0")
                {
                    sb.AppendLine("    <td class=\"tableContent\">Not Reported by Drive</td>");
                }
                else if (di.SsdControllerManufacturer == "65535")
                {
                    sb.AppendLine("    <td class=\"tableContent\">Unknown</td>");
                }
                else
                {
                    sb.AppendLine("    <td class=\"tableContent\">" + di.SsdControllerManufacturer + " RPM</td>");
                }
                sb.AppendLine("  </tr>");
            }
            sb.AppendLine("</table>");
            sb.AppendLine("<span class=\"finePrintSuper\">&dagger;</span><span class=\"finePrint\">");
            sb.AppendLine((isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") + " reports Bytes Per Sector as reported by ");
            sb.AppendLine("Windows. It MAY report 512 bytes/sector even on Advanced Format Disks (AFDs), which have 4096 byte physical sectors. ");
            sb.AppendLine("This is especially true on all versions of Windows prior to Windows Vista/Server 2008.</span><br/>");
            if (di.IsSsd)
            {
                sb.AppendLine("<span class=\"finePrintSuper\">&Dagger;</span><span class=\"finePrint\">");
                sb.AppendLine("This value indicates whether the SSD itself reports that it <i>supports</i> the TRIM function. This ");
                sb.AppendLine("does not indicate that TRIM is necessarily active. For TRIM to operate, not only must the SSD support it, ");
                sb.AppendLine("but also the operating system, disk controller AND disk controller driver.</span><br/>");
            }
            sb.AppendLine("<h3>SMART Details</h3>");

            sb.AppendLine("<table width=\"100%\" border=\"1\" cellpadding=\"2\" summary=\"Contains SMART details for the selected disk.\">");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">ID (Dec)</td>");
            sb.AppendLine("    <td class=\"tableCaption\">ID (Hex)</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Attribute</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Critical</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Type</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Flags</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Threshold</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Value</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Worst</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Status</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Raw Data</td>");
            sb.AppendLine("  </tr>");

            foreach (ListViewItem lvi in listViewSmartDetails.Items)
            {
                sb.AppendLine("  <tr>");
                sb.AppendLine("    <td class=\"tableContent\">" + lvi.SubItems[2].Text + "</td>"); // ID Dec
                sb.AppendLine("    <td class=\"tableContent\">" + lvi.SubItems[3].Text + "</td>"); // ID Hex
                sb.AppendLine("    <td class=\"tableContent\">" + lvi.SubItems[4].Text + "</td>"); // Attribute
                sb.AppendLine("    <td class=\"tableContent\">" + lvi.SubItems[5].Text + "</td>"); // Critical?
                sb.AppendLine("    <td class=\"tableContent\">" + lvi.SubItems[6].Text + "</td>"); // Type
                sb.AppendLine("    <td class=\"tableContent\">" + GetFlagsFromBinary((String)lvi.SubItems[5].Tag) + "</td>"); // Flags
                sb.AppendLine("    <td class=\"tableContent\">" + lvi.SubItems[7].Text + "</td>"); // Thresh
                sb.AppendLine("    <td class=\"tableContent\">" + lvi.SubItems[8].Text + "</td>"); // Value
                sb.AppendLine("    <td class=\"tableContent\">" + lvi.SubItems[9].Text + "</td>"); // Worst
                sb.AppendLine("    " + WriteHtmlHealthStatus(lvi.SubItems[10], lvi.SubItems[2].Text)); // Status
                sb.AppendLine("    <td class=\"tableContent\">" + lvi.SubItems[11].Text + "</td>"); // Raw
                sb.AppendLine("  </tr>");
            }
            sb.AppendLine("</table>");

            sb.AppendLine("<p>Manufacturer Set Flags: SP = Self-Preserving, ER = Error Rate, EC = Event Count, P = Performance, S = Statistical, C = Critical</p>");

            sb.AppendLine("<h3>Disk Self-Test Details</h3>");

            sb.AppendLine("<table width=\"50%\" border=\"1\" cellpadding=\"2\" summary=\"Contains SMART details for the selected disk.\">");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableCaption\">Test</td>");
            sb.AppendLine("    <td class=\"tableCaption\">Details</td>");
            sb.AppendLine("  </tr>");

            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableContent\">Last Test Status</td>"); // ID Dec
            sb.AppendLine("    <td class=\"tableContent\">" + testStatus + "</td>"); // ID Hex
            sb.AppendLine("  <tr>");

            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableContent\">Short Test</td>"); // ID Dec
            sb.AppendLine("    <td class=\"tableContent\">" + (shortLength == 0 ? "Not Supported by Device" : "Supported, " + shortLength.ToString() + " minutes") + "</td>"); // ID Hex
            sb.AppendLine("  <tr>");

            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableContent\">Extended Test</td>"); // ID Dec
            sb.AppendLine("    <td class=\"tableContent\">" + (longLength == 0 ? "Not Supported by Device" : "Supported, " + longLength.ToString() + " minutes") + "</td>"); // ID Hex
            sb.AppendLine("  <tr>");

            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"tableContent\">Conveyance Test</td>"); // ID Dec
            sb.AppendLine("    <td class=\"tableContent\">" + (conveyanceLength == 0 ? "Not Supported by Device" : "Supported, " + conveyanceLength.ToString() + " minutes") + "</td>"); // ID Hex
            sb.AppendLine("  <tr>");

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(filename);
                writer.Write(sb.ToString());
                writer.Flush();
                writer.Close();
                QMessageBox.Show("Exported HTML report " + filename + " was created successfully.", "HTML Export", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                QMessageBox.Show("Unable to save HTML report: " + ex.Message, "HTML Export", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void ExportSelectedDiskToExcel(String filename, bool useTsvInsteadOfCsv)
        {
            DiskInfo di = new DiskInfo();
            ListViewItem diskItem = null;
            if (listViewPhysicalDisks.SelectedItems == null || listViewPhysicalDisks.SelectedItems.Count == 0)
            {
                QMessageBox.Show("The view state is corrupt. Please re-poll or refresh the disks, then select a disk and try again.", "Invalid View State",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!di.Populate(listViewPhysicalDisks.SelectedItems[0].SubItems[0].Text))
            {
                return;
            }

            String separatorCharacter = useTsvInsteadOfCsv ? "\t" : ",";

            diskItem = listViewPhysicalDisks.SelectedItems[0];
            bool isSelfTestDataAvailable = false;
            int shortLength = 0;
            int longLength = 0;
            int conveyanceLength = 0;
            String testStatus = String.Empty;

            isSelfTestDataAvailable = GetDiskTestSupport(diskItem, out testStatus, out shortLength, out longLength, out conveyanceLength);

            // Build the CSV/TSV document.
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine((isWindowsServerSolutions ? "Home Server SMART " : "WindowSMART ") + "24/7");
            sb.AppendLine("SMART Disk Details Report");
            sb.AppendLine(String.Empty);
            sb.AppendLine("Computer Information");
            sb.AppendLine("Computer Name" + separatorCharacter + Environment.MachineName);
            sb.AppendLine("Operating System" + separatorCharacter + OSInfo.Name);
            sb.AppendLine("OS Edition" + separatorCharacter + OSInfo.Edition);
            sb.AppendLine("Service Pack" + separatorCharacter + OSInfo.ServicePack);
            sb.AppendLine("Version" + separatorCharacter + OSInfo.VersionString);
            sb.AppendLine("System Type" + separatorCharacter + OSInfo.Bits + "-bit Operating System");
            sb.AppendLine(String.Empty);
            sb.AppendLine("Disk Information");
            sb.AppendLine("Disk Path" + separatorCharacter + di.Name + separatorCharacter + String.Empty + separatorCharacter + "Advertised Capacity" + separatorCharacter + di.AdvertisedCapacity);
            sb.AppendLine("Model" + separatorCharacter + di.Model + separatorCharacter + String.Empty + separatorCharacter + "Real Capacity" + separatorCharacter + di.RealCapacity);
            sb.AppendLine("Serial Number" + separatorCharacter + di.SerialNumber + separatorCharacter + String.Empty + separatorCharacter + "Bytes Per Sector" + separatorCharacter + di.BytesPerSector + "*");
            sb.AppendLine("Firmware Revision" + separatorCharacter + di.FirmwareRev + separatorCharacter + String.Empty + separatorCharacter + "Cylinders" + separatorCharacter + di.Cylinders);
            sb.AppendLine("Description" + separatorCharacter + di.Description + separatorCharacter + String.Empty + separatorCharacter + "Heads" + separatorCharacter + di.Heads);
            sb.AppendLine("Interface" + separatorCharacter + di.InterfaceType + separatorCharacter + String.Empty + separatorCharacter + "Total Sectors" + separatorCharacter + di.TotalSectors);
            sb.AppendLine("Media Type" + separatorCharacter + di.MediaType + separatorCharacter + String.Empty + separatorCharacter + "Tracks" + separatorCharacter + di.Tracks);
            sb.AppendLine("Partition Count" + separatorCharacter + di.PartitionCount + separatorCharacter + String.Empty + separatorCharacter + "Tracks Per Cylinder" + separatorCharacter + di.TracksPerCylinder);
            sb.AppendLine("Status" + separatorCharacter + di.Status + separatorCharacter + String.Empty + separatorCharacter + "Total Bytes" + separatorCharacter + di.TotalBytes);
            sb.AppendLine("Failure Predicted" + separatorCharacter + ((String.Compare(di.FailurePredicted.ToString(), "True", true) == 0) ? "Yes" : "No"));
            if (di.IsSsd)
            {
                sb.AppendLine("Solid State Disk" + separatorCharacter + "Yes" + separatorCharacter + String.Empty + separatorCharacter + "SSD Controller" + separatorCharacter + di.SsdControllerManufacturer);
                sb.AppendLine("TRIM Supported" + separatorCharacter + ((String.Compare(di.IsTrimSupported.ToString(), "True", true) == 0) ? "Yes" : "No") + "**");
            }
            else
            {
                String spindleSpeed = String.Empty;
                if( di.SsdControllerManufacturer == "0")
                {
                    spindleSpeed = "Not Reported by Drive";
                }
                else if(di.SsdControllerManufacturer == "65535")
                {
                    spindleSpeed = "Unknown";
                }
                else
                {
                    spindleSpeed = di.SsdControllerManufacturer + " RPM";
                }
                sb.AppendLine("Solid State Disk" + separatorCharacter + "No (Hard Disk)" + separatorCharacter + String.Empty + separatorCharacter + "Spindle Speed" + separatorCharacter + spindleSpeed);
            }
            sb.AppendLine(String.Empty);
            sb.AppendLine("* " + (isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") + " reports Bytes Per Sector as reported by Windows. It MAY report 512 bytes/sector even on Advanced Format Disks (AFDs)");
            sb.AppendLine("which have 4096 byte physical sectors. This is especially true on all versions of Windows prior to Windows Vista/Server 2008.");
            if (di.IsSsd)
            {
                sb.AppendLine("** This value indicates whether the SSD itself reports that it supports the TRIM function. This does not indicate that TRIM is necessarily active. For TRIM to operate the SSD ");
                sb.AppendLine("must support it; TRIM must also be supported by operating system and disk controller (and controller driver).");
            }
            sb.AppendLine(String.Empty);
            sb.AppendLine(String.Empty);
            sb.AppendLine("SMART Details");

            sb.AppendLine("ID (Dec)" + separatorCharacter + "ID (Hex)" + separatorCharacter + "Attribute" + separatorCharacter + "Critical" + separatorCharacter +
                "Type" + separatorCharacter + "Flags" + separatorCharacter + "Threshold" + separatorCharacter + "Value" + separatorCharacter + "Worst" +
                separatorCharacter + "Status" + separatorCharacter + "Raw Data");

            foreach (ListViewItem lvi in listViewSmartDetails.Items)
            {
                sb.AppendLine(lvi.SubItems[2].Text + separatorCharacter + lvi.SubItems[3].Text + separatorCharacter + lvi.SubItems[4].Text + separatorCharacter +
                    lvi.SubItems[5].Text + separatorCharacter + lvi.SubItems[6].Text + separatorCharacter + GetFlagsFromBinary((String)lvi.SubItems[5].Tag) +
                    separatorCharacter + lvi.SubItems[7].Text + separatorCharacter + lvi.SubItems[8].Text + separatorCharacter +
                    lvi.SubItems[9].Text + separatorCharacter + lvi.SubItems[10].Text + separatorCharacter + lvi.SubItems[11].Text + separatorCharacter);
            }
            sb.AppendLine(String.Empty);
            sb.AppendLine("Manufacturer Set Flags: SP = Self-Preserving; ER = Error Rate; EC = Event Count; P = Performance; S = Statistical; C = Critical");

            sb.AppendLine(String.Empty);
            sb.AppendLine(String.Empty);
            sb.AppendLine("Disk Self-Test Details");

            sb.AppendLine("Test" + separatorCharacter + "Details");
            sb.AppendLine("Last Test Status" + separatorCharacter + testStatus);
            sb.AppendLine("Short Test" + separatorCharacter + (shortLength == 0 ? "Not Supported by Device" : "Supported; " + shortLength.ToString() + " minutes"));
            sb.AppendLine("Extended Test" + separatorCharacter + (longLength == 0 ? "Not Supported by Device" : "Supported; " + longLength.ToString() + " minutes"));
            sb.AppendLine("Conveyance Test" + separatorCharacter + (conveyanceLength == 0 ? "Not Supported by Device" : "Supported, " + conveyanceLength.ToString() + " minutes"));

            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(filename);
                writer.Write(sb.ToString());
                writer.Flush();
                writer.Close();
                QMessageBox.Show("Exported " + (useTsvInsteadOfCsv ? "TSV" : "CSV") + " report " + filename + " was created successfully.", "CSV/TSV Export", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                QMessageBox.Show("Unable to save HTML " + (useTsvInsteadOfCsv ? "TSV" : "CSV") + " report: " + ex.Message, "CSV/TSV Export", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void ExportSelectedDiskToText(String filename)
        {
            DiskInfo di = new DiskInfo();
            ListViewItem diskItem = null;
            if (listViewPhysicalDisks.SelectedItems == null || listViewPhysicalDisks.SelectedItems.Count == 0)
            {
                QMessageBox.Show("The view state is corrupt. Please re-poll or refresh the disks, then select a disk and try again.", "Invalid View State",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!di.Populate(listViewPhysicalDisks.SelectedItems[0].SubItems[0].Text))
            {
                return;
            }

            diskItem = listViewPhysicalDisks.SelectedItems[0];
            bool isSelfTestDataAvailable = false;
            int shortLength = 0;
            int longLength = 0;
            int conveyanceLength = 0;
            String testStatus = String.Empty;

            isSelfTestDataAvailable = GetDiskTestSupport(diskItem, out testStatus, out shortLength, out longLength, out conveyanceLength);

            String separatorCharacter = "  ";

            // Build the CSV/TSV document.
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine((isWindowsServerSolutions ? "Home Server SMART " : "WindowSMART ") + "24/7");
            sb.AppendLine("SMART Disk Details Report");
            sb.AppendLine(String.Empty);
            sb.AppendLine("Computer Information");
            sb.AppendLine(PadStringWithTrailingSpaces("Computer Name", 20) + separatorCharacter + Environment.MachineName);
            sb.AppendLine(PadStringWithTrailingSpaces("Operating System",20) + separatorCharacter + OSInfo.Name);
            sb.AppendLine(PadStringWithTrailingSpaces("OS Edition", 20) + separatorCharacter + OSInfo.Edition);
            sb.AppendLine(PadStringWithTrailingSpaces("Service Pack", 20) + separatorCharacter + OSInfo.ServicePack);
            sb.AppendLine(PadStringWithTrailingSpaces("Version", 20) + separatorCharacter + OSInfo.VersionString);
            sb.AppendLine(PadStringWithTrailingSpaces("System Type", 20) + separatorCharacter + OSInfo.Bits + "-bit Operating System");
            sb.AppendLine(String.Empty);
            sb.AppendLine("Disk Information");
            sb.AppendLine(PadStringWithTrailingSpaces("Disk Path", 20) + separatorCharacter + PadStringWithTrailingSpaces(di.Name, 40) + separatorCharacter + String.Empty + separatorCharacter + PadStringWithTrailingSpaces("Advertised Capacity", 25) + separatorCharacter + di.AdvertisedCapacity);
            sb.AppendLine(PadStringWithTrailingSpaces("Model", 20) + separatorCharacter + PadStringWithTrailingSpaces(di.Model, 40) + separatorCharacter + String.Empty + separatorCharacter + PadStringWithTrailingSpaces("Real Capacity", 25) + separatorCharacter + di.RealCapacity);
            sb.AppendLine(PadStringWithTrailingSpaces("Serial Number", 20) + separatorCharacter + PadStringWithTrailingSpaces(di.SerialNumber, 40) + separatorCharacter + String.Empty + separatorCharacter + PadStringWithTrailingSpaces("Bytes Per Sector", 25) + separatorCharacter + di.BytesPerSector + "*");
            sb.AppendLine(PadStringWithTrailingSpaces("Firmware Revision", 20) + separatorCharacter + PadStringWithTrailingSpaces(di.FirmwareRev, 40) + separatorCharacter + String.Empty + separatorCharacter + PadStringWithTrailingSpaces("Cylinders", 25) + separatorCharacter + di.Cylinders);
            sb.AppendLine(PadStringWithTrailingSpaces("Description", 20) + separatorCharacter + PadStringWithTrailingSpaces(di.Description, 40) + separatorCharacter + String.Empty + separatorCharacter + PadStringWithTrailingSpaces("Heads", 25) + separatorCharacter + di.Heads);
            sb.AppendLine(PadStringWithTrailingSpaces("Interface", 20) + separatorCharacter + PadStringWithTrailingSpaces(di.InterfaceType, 40) + separatorCharacter + String.Empty + separatorCharacter + PadStringWithTrailingSpaces("Total Sectors", 25) + separatorCharacter + di.TotalSectors);
            sb.AppendLine(PadStringWithTrailingSpaces("Media Type", 20) + separatorCharacter + PadStringWithTrailingSpaces(di.MediaType, 40) + separatorCharacter + String.Empty + separatorCharacter + PadStringWithTrailingSpaces("Tracks", 25) + separatorCharacter + di.Tracks);
            sb.AppendLine(PadStringWithTrailingSpaces("Partition Count", 20) + separatorCharacter + PadStringWithTrailingSpaces(di.PartitionCount, 40) + separatorCharacter + String.Empty + separatorCharacter + PadStringWithTrailingSpaces("Tracks Per Cylinder", 25) + separatorCharacter + di.TracksPerCylinder);
            sb.AppendLine(PadStringWithTrailingSpaces("Status", 20) + separatorCharacter + PadStringWithTrailingSpaces(di.Status, 40) + separatorCharacter + String.Empty + separatorCharacter + PadStringWithTrailingSpaces("Total Bytes", 25) + separatorCharacter + di.TotalBytes);
            sb.AppendLine(PadStringWithTrailingSpaces("Failure Predicted", 20) + separatorCharacter + PadStringWithTrailingSpaces(((String.Compare(di.FailurePredicted.ToString(), "True", true) == 0) ? "Yes" : "No"), 40));
            if (di.IsSsd)
            {
                sb.AppendLine(PadStringWithTrailingSpaces("Solid State Disk", 20) + separatorCharacter + PadStringWithTrailingSpaces("Yes", 40) + separatorCharacter + String.Empty + separatorCharacter + PadStringWithTrailingSpaces("SSD Controller", 25) + separatorCharacter + di.SsdControllerManufacturer);
                sb.AppendLine(PadStringWithTrailingSpaces("TRIM Supported", 20) + separatorCharacter + PadStringWithTrailingSpaces(((String.Compare(di.IsTrimSupported.ToString(), "True", true) == 0) ? "Yes" : "No") + "**", 40));
            }
            else
            {
                String spindleSpeed = String.Empty;
                if (di.SsdControllerManufacturer == "0")
                {
                    spindleSpeed = "Not Reported by Drive";
                }
                else if (di.SsdControllerManufacturer == "65535")
                {
                    spindleSpeed = "Unknown";
                }
                else
                {
                    spindleSpeed = di.SsdControllerManufacturer + " RPM";
                }
                sb.AppendLine(PadStringWithTrailingSpaces("Solid State Disk", 20) + separatorCharacter + PadStringWithTrailingSpaces("No (Hard Disk)", 40) + separatorCharacter + String.Empty + separatorCharacter + PadStringWithTrailingSpaces("Spindle Speed", 25) + separatorCharacter + spindleSpeed);
            }
            sb.AppendLine(String.Empty);
            sb.AppendLine("* " + (isWindowsServerSolutions ? "Home Server SMART" : "WindowSMART") + " reports Bytes Per Sector as reported by Windows. It MAY report 512 bytes/sector even on Advanced Format Disks (AFDs)");
            sb.AppendLine("which have 4096 byte physical sectors. This is especially true on all versions of Windows prior to Windows Vista/Server 2008.");
            if (di.IsSsd)
            {
                sb.AppendLine("** This value indicates whether the SSD itself reports that it supports the TRIM function. This does not indicate that TRIM is necessarily active. For TRIM to operate the SSD ");
                sb.AppendLine("must support it; TRIM must also be supported by operating system and disk controller (and controller driver).");
            }
            sb.AppendLine(String.Empty);
            sb.AppendLine(String.Empty);
            sb.AppendLine("SMART Details");

            sb.AppendLine(PadStringWithTrailingSpaces("ID (Dec)", 8) + separatorCharacter + PadStringWithTrailingSpaces("ID (Hex)", 8) + separatorCharacter +
                PadStringWithTrailingSpaces("Attribute", 45) + separatorCharacter + PadStringWithTrailingSpaces("Critical", 8) + separatorCharacter +
                PadStringWithTrailingSpaces("Type", 8) + separatorCharacter + PadStringWithTrailingSpaces("Flags", 20) + separatorCharacter +
                PadStringWithTrailingSpaces("Thresh", 6) + separatorCharacter + PadStringWithTrailingSpaces("Value", 6) + separatorCharacter +
                PadStringWithTrailingSpaces("Worst", 6) + separatorCharacter + PadStringWithTrailingSpaces("Status", 10) + separatorCharacter + "Raw Data");

            // Long Tent Poles
            // ID (Dec) = 8, ID (Hex) = 8, Attribute = 45, Critical = 8, Type = 8, Flags = 20,Threshold = 6, Value = 6, Worst = 6, Status = 10, Raw Data = 12

            foreach (ListViewItem lvi in listViewSmartDetails.Items)
            {
                sb.AppendLine(PadStringWithTrailingSpaces(lvi.SubItems[2].Text, 8) + separatorCharacter +
                    PadStringWithTrailingSpaces(lvi.SubItems[3].Text, 8) + separatorCharacter +
                    PadStringWithTrailingSpaces(lvi.SubItems[4].Text, 45) + separatorCharacter +
                    PadStringWithTrailingSpaces(lvi.SubItems[5].Text, 8) + separatorCharacter +
                    PadStringWithTrailingSpaces(lvi.SubItems[6].Text, 8) + separatorCharacter +
                    PadStringWithTrailingSpaces(GetFlagsFromBinary((String)lvi.SubItems[5].Tag), 20) + separatorCharacter +
                    PadStringWithTrailingSpaces(lvi.SubItems[7].Text, 6) + separatorCharacter +
                    PadStringWithTrailingSpaces(lvi.SubItems[8].Text, 6) + separatorCharacter +
                    PadStringWithTrailingSpaces(lvi.SubItems[9].Text, 6) + separatorCharacter +
                    PadStringWithTrailingSpaces(lvi.SubItems[10].Text, 10) + separatorCharacter +
                    lvi.SubItems[11].Text + separatorCharacter);
            }
            sb.AppendLine(String.Empty);
            sb.AppendLine("Manufacturer Set Flags: SP = Self-Preserving, ER = Error Rate, EC = Event Count, P = Performance, S = Statistical, C = Critical");

            sb.AppendLine(String.Empty);
            sb.AppendLine(String.Empty);
            sb.AppendLine("Disk Self-Test Details");

            sb.AppendLine(PadStringWithTrailingSpaces("Test", 16) + separatorCharacter + "Details");
            sb.AppendLine(PadStringWithTrailingSpaces("Last Test Status", 16) + separatorCharacter + testStatus);
            sb.AppendLine(PadStringWithTrailingSpaces("Short Test", 16) + separatorCharacter + (shortLength == 0 ? "Not Supported by Device" : "Supported, " + shortLength.ToString() + " minutes"));
            sb.AppendLine(PadStringWithTrailingSpaces("Extended Test", 16) + separatorCharacter + (longLength == 0 ? "Not Supported by Device" : "Supported, " + longLength.ToString() + " minutes"));
            sb.AppendLine(PadStringWithTrailingSpaces("Conveyance Test", 16) + separatorCharacter + (conveyanceLength == 0 ? "Not Supported by Device" : "Supported, " + conveyanceLength.ToString() + " minutes"));


            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(filename);
                writer.Write(sb.ToString());
                writer.Flush();
                writer.Close();
                QMessageBox.Show("Exported text report " + filename + " was created successfully.", "Text Export", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                QMessageBox.Show("Unable to save text report: " + ex.Message, "Text Export", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private String GetFlagsFromBinary(String flags)
        {
            if (String.IsNullOrEmpty(flags))
            {
                return "None";
            }
            else if (flags.Length != 8)
            {
                return "Unknown";
            }

            String flagList = String.Empty;

            // Self-Preserving
            if (flags.Substring(2, 1) == "1")
            {
                flagList += "SP; ";
            }

            // Error Rate
            if (flags.Substring(4, 1) == "1")
            {
                flagList += "ER; ";
            }

            // Event Count
            if (flags.Substring(3, 1) == "1")
            {
                flagList += "EC; ";
            }

            // Performance
            if (flags.Substring(5, 1) == "1")
            {
                flagList += "P; ";
            }

            // Statistical
            if (flags.Substring(6, 1) == "1")
            {
                flagList += "S; ";
            }

            // Critical
            if (flags.Substring(7, 1) == "1")
            {
                flagList += "C";
            }

            flagList = flagList.Trim();
            if (flagList.EndsWith(";"))
            {
                flagList = flagList.Substring(0, flagList.Length - 1);
            }

            if (String.IsNullOrEmpty(flagList))
            {
                flagList = "None";
            }

            return flagList;
        }

        private String WriteHtmlHealthStatus(ListViewItem.ListViewSubItem subItem, String attributeID)
        {
            switch (subItem.Text)
            {
                case "Fail":
                case "Critical":
                case "Overheated":
                    {
                        return "<td class=\"tableContentCritical\">" + subItem.Text + "</td>";
                    }
                case "Degraded":
                case "Warm":
                case "Hot":
                case "Caution":
                    {
                        return "<td class=\"tableContentDegraded\">" + subItem.Text + "</td>";
                    }
                case "Geriatric":
                    {
                        return "<td class=\"tableContentGeezery\">" + subItem.Text + "</td>";
                    }
                case "Healthy":
                default:
                    {
                        return "<td class=\"tableContentHealthy\">" + subItem.Text + "</td>";
                    }
            }
        }

        private String PadStringWithTrailingSpaces(String stringToPad, int longTentPole)
        {
            if (stringToPad.Length == longTentPole)
            {
                return stringToPad;
            }
            else if (stringToPad.Length > longTentPole)
            {
                return stringToPad.Substring(0, longTentPole - 3) + "...";
            }
            else
            {
                int spacesToAdd = longTentPole - stringToPad.Length;
                for (int i = 0; i < spacesToAdd; i++)
                {
                    stringToPad += " ";
                }
                return stringToPad;
            }
        }

        private void menuTests_Click(object sender, EventArgs e)
        {
            SelfTests tests = new SelfTests(fallbackToWmi, advancedSii, ignoreVirtualDisks);
            tests.ShowDialog();
        }

        /// <summary>
        /// Displays Settings - available only for WSS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings(null);
            settings.ShowDialog();
            this.CheckForBackgroundChange();
        }

        public void SetAppTitle()
        {
            if (isWindowsServerSolutions)
            {
                labelBeta.Text = "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                buttonExport.Text = "Settings && More";
                settingsToolStripMenuItem.Visible = true;
            }
            else if (invokedFromHss)
            {
                if (mexiSexi == null)
                {
                    labelBeta.Text = "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
                else
                {
                    if (!mexiSexi.IsMexiSexi)
                    {
                        labelBeta.Text = "Trial v3.4";
                    }
                    else if (mexiSexi.IsHomeEdition)
                    {
                        labelBeta.Text = "Home Edition v3.4";
                    }
                    else
                    {
                        labelBeta.Text = "Professional Edition v3.4";
                    }
                }
                //labelBeta.Text += " (Unsupported)";
                buttonExport.Text = "Settings && More";
                settingsToolStripMenuItem.Visible = true;

                QMessageBox.Show(Properties.Resources.ApplicationTitleHss + " was intended for Windows Home Server and Microsoft's \"Essentials\" family " +
                    "of Server products. You are running a full Standard or Datacenter version of Windows Server, with the Essentials role installed. " +
                    "Home Server SMART is a freeware add-in for Windows Home Server and the Essentials family of Server products. When used with a " +
                    "full Standard/Datacenter edition of Windows Server, the product switches operating modes to our shareware product, " +
                    Properties.Resources.ApplicationTitleWindowSmart + ". This requires you to purchase a license to continue using the product after " +
                    "30 days. In order to apply your purchased license key, you will need to start an elevated PowerShell session. Use the Import-Module " +
                    "cmdlet to import WindowSMARTPowerShell.dll, then use the Set-WindowSmartLicense cmdlet to apply the license key.", "Configuration " +
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (mexiSexi == null)
                {
                    labelBeta.Text = "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
                else
                {
                    if (!mexiSexi.IsMexiSexi)
                    {
                        labelBeta.Text = "Trial v3.4";
                    }
                    else if (mexiSexi.IsHomeEdition)
                    {
                        labelBeta.Text = "Home Edition v3.4";
                    }
                    else
                    {
                        labelBeta.Text = "Professional Edition v3.4";
                    }
                }
            }
        }

        private void uploadDebugDiagnosticsToolStripMenuItem_Click(object sender, EventArgs e)
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
                if(isWindowsServerSolutions)
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
            GenerateDiags diags = new GenerateDiags(isWindowsServerSolutions, isWindows8);
            diags.ShowDialog();
            DeveloperDiagnosticDebuggingReport report = new DeveloperDiagnosticDebuggingReport(debugLogLocation, diags.DiagnosticDetails, mexiSexi.IsMexiSexi);
            report.ShowDialog();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.UploadDebugDiagnostics");
        }

        private bool IsDebuggingAvailable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.IsDebuggingAvailable");
            SiAuto.Main.LogString("debugLogLocation", debugLogLocation);
            try
            {
                if (Directory.Exists(debugLogLocation))
                {
                    SiAuto.Main.LogMessage("Debug folder exists, checking for SIL files.");
                    String[] files = Directory.GetFiles(debugLogLocation, "*.sil");
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
            catch(Exception ex)
            {
                SiAuto.Main.LogWarning("Logging is not available. " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.IsDebuggingAvailable");
            return false;
        }

        private void InjectPhantomDisk(String disk)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.InjectPhantomDisk");
            if (phantomDisks.Contains(disk))
            {
                SiAuto.Main.LogMessage("Phantom disk already exists: " + disk);
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.InjectPhantomDisk");
                return;
            }
            else
            {
                SiAuto.Main.LogMessage("Adding " + disk + " to phantom disks list.");
                phantomDisks.Add(disk);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.InjectPhantomDisk");
        }

        private void CullPhantomDisk(String disk)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.UI.MainUIControl.CullPhantomDisk");
            if (!phantomDisks.Contains(disk))
            {
                SiAuto.Main.LogMessage("Disk " + disk + " is not registered as a phantom.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.CullPhantomDisk");
                return;
            }
            else
            {
                SiAuto.Main.LogMessage("Removing " + disk + " from phantom disks list.");
                phantomDisks.Remove(disk);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.UI.MainUIControl.CullPhantomDisk");
        }
    }

    // Enums
    public enum DiskStatus : int
    {
        Healthy = 0,
        Critical = 1,
        Warning = 2,
        Geriatric = 3,
        Unknown = 4
    }
}
