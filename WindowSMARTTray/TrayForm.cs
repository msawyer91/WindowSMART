using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls;
using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Utilities;
using Gurock.SmartInspect;
using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.WindowSMARTTray
{
    public partial class TrayForm : Form
    {
        private System.Timers.Timer backgroundTimer;
        public static string SW_MUTEX = "XtlKtnVnWtT/rFfft8WjHmhSXdFrqGT9KFkaBIJqn5Nwdq0dXakM2+NVYQDhPcDGj9qovDUnJujc4fBIrlKIpPl/LADT8Wkl+ZpW2YBMmuHjnJ";
        DataTable activeAlerts;

        // Snarl alerts
        private bool isSnarlEnabled;
        private SnarlNotifications ornery;
        private bool isGrowlEnabled;
        private bool isGrowlRemoteEnabled;

        // Windows Server Solutions
        private bool isWindowsServerSolutions;

        // Registry
        private Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
        private Microsoft.Win32.RegistryKey dojoNorthSubKey;
        private Microsoft.Win32.RegistryKey configurationKey;

        // XML file path
        private String xmlFilePath;
        private String xmlAlertFileName;
        private String xmlAlertFile;

        // UI delegates
        private delegate void tooltipDisplay(int timeout, String tooltipTitle, String tooltipMessage, ToolTipIcon icon);
        private delegate void messageboxDisplay(String message, String title, MessageBoxButtons buttons, MessageBoxIcon icon);
        private tooltipDisplay displayTooltip;
        private messageboxDisplay displayMessage;

        // Items to track
        private int activeAlertCount;
        private bool isUpdateAvailable;
        private String updateVersion;
        private String updateReleaseDate;
        private String updateUrl;

        // License expiration/tampering
        private bool isSevere;
        private bool isTampering;

        // Update check index
        private int updateCheckIndex;

        // Initial run thread
        private Thread initialRunner;

        // Tooltip
        private String toolTipUrl;

        // Shutdown
        private static int WM_QUERYENDSESSION = 0x11;
        private static int WM_ENDSESSION = 0x16;

        // Help About info
        private DateTime result;
        Guid refGuid = new Guid("{00000000-0000-0000-0000-000000000000}");
        private uint retVal;
        private String xmlText;
        private Components.Mexi_Sexi.MexiSexi mexiSexi = null;

        public TrayForm(uint slobberhead, DateTime hubbaChubba)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.TrayForm");
            InitializeComponent();

            SiAuto.Main.LogMessage("Determining Windows SKU.");
            isWindowsServerSolutions = Components.OperatingSystem.IsWindowsServerSolutionsProduct(this);
            SiAuto.Main.LogBool("isWindowsServerSolutions", isWindowsServerSolutions);

            if (isWindowsServerSolutions)
            {
                this.systemTray.Text = "Home Server SMART 24/7";
            }

            SiAuto.Main.LogMessage("Acquire Registry objects.");
            try
            {
                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);
                configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, false);
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError(ex.Message);
                SiAuto.Main.LogException(ex);
                QMessageBox.Show("WindowSMART tray cannot access the Registry in READ mode: " + ex.Message, "Severe", MessageBoxButtons.OK,
                    Properties.Resources.SkullAndCrossbonesRed32);
            }

            GetGrowlSnarlConfiguration();

            SiAuto.Main.LogMessage("Acquired Registry objects.");

            // Now we do the real checking.
            String concatenatedString = String.Empty;
            retVal = slobberhead;
            if (slobberhead == 0x0)
            {
                // New user
                concatenatedString = Components.LegacyOs.Concatenate(Program.SW_WINDOWS_KEY, Properties.Resources.RegistryConfigCriticalFailure, true, false, out refGuid);
            }
            else if (slobberhead == 0x1)
            {
                // Existing user and no errors returned with info
                concatenatedString = Components.LegacyOs.Concatenate(Program.SW_WINDOWS_KEY, Properties.Resources.RegistryConfigCriticalFailure, false, false, out refGuid);
            }
            else
            {
                // Date was invalid but we still grab the license because it could be valid (if it is we don't care about the date).
                concatenatedString = "0xF," + Components.LegacyOs.Concatenate(Program.SW_WINDOWS_KEY, Properties.Resources.RegistryConfigCriticalFailure, false, false, out refGuid);
            }
            xmlText = concatenatedString;
            result = hubbaChubba;

            activeAlertCount = 0;
            isUpdateAvailable = false;
            updateVersion = String.Empty;
            updateReleaseDate = String.Empty;
            updateUrl = String.Empty;
            toolTipUrl = String.Empty;

            isSevere = false;
            isTampering = false;

            updateCheckIndex = 0;

            SiAuto.Main.LogMessage("Initialize the UI delegates.");
            displayTooltip = new tooltipDisplay(DDisplayTooltip);
            displayMessage = new messageboxDisplay(DDisplayDialogue);

            SiAuto.Main.LogMessage("Getting executing assembly location.");
            xmlFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            SiAuto.Main.LogString("Got executing assembly location.", xmlFilePath);

            xmlAlertFileName = Properties.Resources.ActiveAlertsXml;
            SiAuto.Main.LogString("xmlAlertFileName", xmlAlertFileName);

            xmlAlertFile = xmlFilePath + "\\" + xmlAlertFileName;
            SiAuto.Main.LogString("xmlAlertFile", xmlAlertFile);

            SiAuto.Main.LogMessage("Construct the FeverishDisks table of alerts.");
            activeAlerts = new DataTable("FeverishDisks");
            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            SiAuto.Main.LogMessage("Add columns to the FeverishDisks table.");
            activeAlerts.Columns.Add("Key", typeof(Guid)); // Correlation ID
            activeAlerts.Columns.Add("HealthTitle", typeof(String));
            activeAlerts.Columns.Add("DiskModel", typeof(String));
            activeAlerts.Columns.Add("DiskPath", typeof(String));
            activeAlerts.Columns.Add("AttributeID", typeof(String));
            activeAlerts.Columns.Add("AttributeName", typeof(String));
            activeAlerts.Columns.Add("HealthMessage", typeof(String));
            activeAlerts.Columns.Add("IsCritical", typeof(bool));
            activeAlerts.Columns.Add("IsActive", typeof(bool));
            activeAlerts.Columns.Add("IsNotificateNeeded", typeof(bool));
            SiAuto.Main.LogMessage("Columns added. Commit changes.");
            activeAlerts.AcceptChanges();
            SiAuto.Main.LogMessage("Changes committed.");

            if (isSnarlEnabled)
            {
                // Take a 30-second siesta. Since the tray app is designed to start at logon time, we want to make sure that, even on a slow
                // computer, Snarl has a chance to start.
                System.Threading.Thread.Sleep(30000);
            }
            EnableDisableSnarl();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.TrayForm");
        }

        private void GetGrowlSnarlConfiguration()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.GetGrowlSnarlConfiguration");
            try
            {
                isGrowlEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsGrowlEnabled));
            }
            catch
            {
                SiAuto.Main.LogWarning("Exceptions were detected reading isGrowlEnabled registry value; using default value FALSE.");
            }

            try
            {
                isGrowlRemoteEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsGrowlRemoteEnabled));
            }
            catch
            {
                SiAuto.Main.LogWarning("Exceptions were detected reading isGrowlRemoteEnabled registry value; using default value FALSE.");
            }

            try
            {
                isSnarlEnabled = bool.Parse((String)configurationKey.GetValue(Properties.Resources.RegistryConfigIsSnarlEnabled));
            }
            catch
            {
                SiAuto.Main.LogWarning("Exceptions were detected reading isSnarlEnabled registry value; using default value FALSE.");
            }

            SiAuto.Main.LogBool("isGrowlEnabled", isGrowlEnabled);
            SiAuto.Main.LogBool("isGrowlRemoteEnabled", isGrowlRemoteEnabled);
            SiAuto.Main.LogBool("isSnarlEnabled", isSnarlEnabled);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.GetGrowlSnarlConfiguration");
        }

        private void EnableDisableSnarl()
        {
            if (isSnarlEnabled)
            {
                SiAuto.Main.LogMessage("Snarl is set to enabled.");
                if (ornery == null)
                {
                    SiAuto.Main.LogMessage("Snarl notification object is currently null so we need to create a new one.");
                    ornery = new SnarlNotifications(false);
                    int result = ornery.Register();

                    if (result != 0)
                    {
                        SiAuto.Main.LogWarning("Snarl returned an error code (" + result.ToString() + ") when attempting to register. Setting the isSnarlEnabled flag to " +
                            "false. This may allow the tray app to display notifications so problems don't go undetected by the user. If the problem is corrected, the " +
                            "next refresh will register with Snarl.");
                        isSnarlEnabled = false;
                    }
                }
            }
            else
            {
                if (ornery != null)
                {
                    try
                    {
                        SiAuto.Main.LogMessage("Snarl is disabled but Snarl notification object is not null, so tearing down.");
                        ornery.Close();
                        ornery = null;
                    }
                    catch (Exception ex)
                    {
                        SiAuto.Main.LogWarning("Unable to dispose Snarl notification object: " + ex.Message);
                        SiAuto.Main.LogException(ex);
                    }
                }
            }
        }

        private void menuConsole_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.menuConsole_Click");
            StartConsole();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.menuConsole_Click");
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.menuAbout_Click");
            HelpAbout aboots;
            if (isWindowsServerSolutions || mexiSexi == null)
            {
                aboots = new HelpAbout(false);
            }
            else
            {
                aboots = new HelpAbout(true, mexiSexi);
            }
            aboots.StartPosition = FormStartPosition.CenterScreen;
            aboots.ShowDialog();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.menuAbout_Click");
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.menuExit_Click");
            try
            {
                String questionMessage = String.Empty;

                if (isSnarlEnabled)
                {
                    questionMessage = "Closing the WindowSMART 24/7 tray application will prevent WindowSMART-related Snarl notifications from appearing on your " +
                        "desktop. If you have email, Prowl or Notify My Android alerts configured, those will continue to appear. Do you " +
                        "still want to close the tray application?";
                }
                else
                {
                    questionMessage = "Closing the WindowSMART 24/7 tray application will prevent notifications from appearing on your " +
                        "desktop. If you have email, Prowl or Notify My Android alerts configured, those will continue to appear. Do you " +
                        "still want to close the tray application?";
                }

                if (isWindowsServerSolutions)
                {
                    questionMessage = questionMessage.Replace("WindowSMART", "Home Server SMART");
                }

                if (QMessageBox.Show(questionMessage, "WindowSMART Tray", MessageBoxButtons.YesNo, MessageBoxIcon.Question, FormStartPosition.CenterScreen) ==
                    DialogResult.Yes)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                System.Media.SystemSounds.Hand.Play();
                SiAuto.Main.LogError(ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.menuExit_Click");
        }

        private void TrayForm_Load(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.TrayForm_Load");
            SiAuto.Main.LogMessage("Set form Visible and ShowInTaskbar attributes to false. There is no form to display, only the tray icon.");
            this.Visible = false;
            this.ShowInTaskbar = false;
            SiAuto.Main.LogMessage("Initialize the background timer thread that does all the work.");
            backgroundTimer = new System.Timers.Timer(); // five minutes
            ((System.ComponentModel.ISupportInitialize)(this.backgroundTimer)).BeginInit();
            SiAuto.Main.LogMessage("Set interval to 5 minutes.");
            this.backgroundTimer.Interval = 300000;
            SiAuto.Main.LogMessage("Define method backgroundTimer_Elapsed method to run on the interval.");
            this.backgroundTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.backgroundTimer_Elapsed);
            ((System.ComponentModel.ISupportInitialize)(this.backgroundTimer)).EndInit();
            SiAuto.Main.LogMessage("Start the background worker thread.");
            this.backgroundTimer.Start();

            initialRunner = new Thread(new ThreadStart(InitialRunOfAlerts));
            initialRunner.Name = "Initial Cheque";
            initialRunner.Start();

            // License
            try
            {
                mexiSexi = new Components.Mexi_Sexi.MexiSexi(xmlText, result, refGuid, retVal);
            }
            catch
            {
                mexiSexi = null;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.TrayForm_Load");
        }

        private void backgroundTimer_Elapsed(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.backgroundTimer_Elapsed");
            Thread.Sleep(5000);
            // Refresh configuration.
            GetGrowlSnarlConfiguration();
            EnableDisableSnarl();
            // Check the service state.
            CheckServiceState();
            // Set all alerts to stale.
            SetAlertsStale();
            // Read the alerts file.
            ReadAlertsFile();
            // Cull and Post the alerts.
            CullAlerts();
            PostAlerts();
            // Sleep for 2 minutes.
            Thread.Sleep(120000);
            // Sound the update alarm if needed.
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.backgroundTimer_Elapsed");
        }

        private void InitialRunOfAlerts()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.InitialRunOfAlerts");
            Thread.Sleep(5000);
            // Check the service state.
            CheckServiceState();
            // Set all alerts to stale.
            SetAlertsStale();
            // Read the alerts file.
            ReadAlertsFile();
            // Cull and Post the alerts.
            CullAlerts();
            PostAlerts();
            // Sound the update alarm if needed.
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.InitialRunOfAlerts");
        }

        private void CheckServiceState()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.CheckServiceState");
            ServiceController controller;

            // Make sure the service is in a stoppable state.
            try
            {
                controller = new ServiceController(Properties.Resources.ServiceNameHss);
                if (controller.Status != ServiceControllerStatus.Running)
                {
                    DisplayTooltip(30000, isWindowsServerSolutions ? "Home Server SMART 24/7" : "WindowSMART 24/7",
                        (isWindowsServerSolutions ? "Home Server SMART " : "WindowSMART ") + "24/7 service is not running. Disk health is not being monitored.", ToolTipIcon.Error);
                    Thread.Sleep(30000);
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Exceptions were detected checking the service execution state. " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.CheckServiceState");
        }

        private void SetAlertsStale()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.SetAlertsStale");
            bool changesMade = false;
            foreach (DataRow row in activeAlerts.Select())
            {
                SiAuto.Main.LogMessage("Setting row to stale.");
                row["IsActive"] = false;
                SiAuto.Main.LogMessage("Commit changes to row.");
                row.AcceptChanges();
                changesMade = true;
                SiAuto.Main.LogMessage("Changes committed.");
            }
            if (changesMade)
            {
                SiAuto.Main.LogMessage("Row changes were detected, so commit changes to table.");
                activeAlerts.AcceptChanges();
                SiAuto.Main.LogMessage("Changes committed.");
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.SetAlertsStale");
        }

        private void ReadAlertsFile()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.ReadAlertsFile");
            isSevere = false;
            isTampering = false;

            try
            {
                SiAuto.Main.LogMessage("Instantiate new XmlDocument object.");
                XmlDocument doc = new XmlDocument();
                SiAuto.Main.LogMessage("Open XML alerts file " + xmlAlertFile);
                System.IO.StreamReader reader = new System.IO.StreamReader(xmlAlertFile);
                String xmlContent = reader.ReadToEnd();
                SiAuto.Main.LogMessage("Contents of " + xmlContent + " have been read; closing file.");
                reader.Close();
                SiAuto.Main.LogMessage("StreamReader closed. Begin XML parse.");
                doc.LoadXml(xmlContent);
                SiAuto.Main.LogMessage("XmlDocument is populated. XML is well-formed.");

                SiAuto.Main.LogMessage("Starting FOREACH loop to iterate the XML root level.");
                foreach (XmlNode rootNode in doc.ChildNodes)
                {
                    if (rootNode.Name == "alerts")
                    {
                        SiAuto.Main.LogMessage("Starting FOREACH loop to iterate the \"alerts\" level.");
                        foreach (XmlNode alertsNode in rootNode.ChildNodes)
                        {
                            if (alertsNode.Name == "count")
                            {
                                SiAuto.Main.LogMessage("XML node name is \"count\"");
                                activeAlertCount = 0;
                                bool result = Int32.TryParse(alertsNode.InnerText, out activeAlertCount);
                                if (result)
                                {
                                    SiAuto.Main.LogMessage("Detected " + activeAlertCount.ToString() + " active alerts.");
                                }
                                else
                                {
                                    SiAuto.Main.LogWarning("Failed to parse active alert count. Using default zero.");
                                }
                            }
                            else if (alertsNode.Name == "alert")
                            {
                                SiAuto.Main.LogMessage("XML node name is \"alert\"");

                                String healthTitle = String.Empty;
                                String diskModel = String.Empty;
                                String diskPath = String.Empty;
                                String attributeID = String.Empty;
                                String attributeName = String.Empty;
                                String healthMessage = String.Empty;
                                bool isCritical = false;
                                String correlationID = String.Empty;

                                foreach (XmlAttribute attrib in alertsNode.Attributes)
                                {
                                    switch (attrib.Name)
                                    {
                                        case "healthTitle":
                                            {
                                                healthTitle = attrib.Value;
                                                SiAuto.Main.LogString("healthTitle", healthTitle);
                                                break;
                                            }
                                        case "diskModel":
                                            {
                                                diskModel = attrib.Value;
                                                SiAuto.Main.LogString("diskModel", diskModel);
                                                break;
                                            }
                                        case "diskPath":
                                            {
                                                diskPath = attrib.Value;
                                                SiAuto.Main.LogString("diskPath", diskPath);
                                                break;
                                            }
                                        case "attributeID":
                                            {
                                                attributeID = attrib.Value;
                                                SiAuto.Main.LogString("attributeID", attributeID);
                                                break;
                                            }
                                        case "attributeName":
                                            {
                                                attributeName = attrib.Value;
                                                SiAuto.Main.LogString("attributeName", attributeName);
                                                break;
                                            }
                                        case "healthMessage":
                                            {
                                                healthMessage = attrib.Value;
                                                SiAuto.Main.LogString("healthMessage", healthMessage);
                                                break;
                                            }
                                        case "isCritical":
                                            {
                                                bool.TryParse(attrib.Value, out isCritical);
                                                SiAuto.Main.LogBool("isCritical", isCritical);
                                                break;
                                            }
                                        case "correlationID":
                                            {
                                                correlationID = attrib.Value;
                                                SiAuto.Main.LogString("correlationID", correlationID);
                                                break;
                                            }
                                        default:
                                            {
                                                SiAuto.Main.LogWarning("Unrecognized attribute name " + attrib.Name + ", value " + attrib.Value);
                                                break;
                                            }
                                    }

                                    SiAuto.Main.LogMessage("OK got the alert info, now let's see if it already exists.");
                                    if (DoesAlertExist(correlationID))
                                    {
                                        SiAuto.Main.LogMessage("An alert with correlation ID " + correlationID + " already exists. We'll check the existing one and update accordingly.");
                                        UpdateAlert(correlationID, healthTitle, attributeID, attributeName, isCritical);
                                    }
                                    else
                                    {
                                        SiAuto.Main.LogMessage("No alert with correlation ID " + correlationID + " currently exists. This will be created as a new alert.");
                                        InjectAlert(correlationID, healthTitle, diskModel, diskPath, attributeID, attributeName, isCritical);
                                    }
                                }
                            }
                            else if (alertsNode.Name == "update")
                            {
                                SiAuto.Main.LogMessage("XML node name is \"update\"");
                                foreach (XmlAttribute attrib in alertsNode.Attributes)
                                {
                                    switch (attrib.Name)
                                    {
                                        case "isAvailable":
                                            {
                                                bool.TryParse(attrib.Value, out isUpdateAvailable);
                                                SiAuto.Main.LogBool("isUpdateAvailable", isUpdateAvailable);
                                                break;
                                            }
                                        case "version":
                                            {
                                                updateVersion = attrib.Value;
                                                SiAuto.Main.LogString("updateVersion", updateVersion);
                                                break;
                                            }
                                        case "releaseDate":
                                            {
                                                updateReleaseDate = attrib.Value;
                                                SiAuto.Main.LogString("updateReleaseDate", updateReleaseDate);
                                                break;
                                            }
                                        case "url":
                                            {
                                                updateUrl = attrib.Value;
                                                SiAuto.Main.LogString("updateUrl", updateUrl);
                                                break;
                                            }
                                        default:
                                            {
                                                SiAuto.Main.LogWarning("Unrecognized attribute name " + attrib.Name + ", value " + attrib.Value);
                                                break;
                                            }
                                    }
                                }
                            }
                            else if (alertsNode.Name == "severe")
                            {
                                SiAuto.Main.LogMessage("XML node name is \"severe\"");
                                foreach (XmlAttribute attrib in alertsNode.Attributes)
                                {
                                    if (attrib.Name == "statusMessage")
                                    {
                                        isSevere = true;
                                        bool.TryParse(attrib.Value, out isTampering);
                                        SiAuto.Main.LogBool("isSevere", isSevere);
                                        SiAuto.Main.LogBool("isTampering", isTampering);

                                        if (isTampering)
                                        {
                                            SiAuto.Main.LogFatal("Licensing tampering has been detected.");
                                        }
                                        else
                                        {
                                            SiAuto.Main.LogFatal("The trial license is expired.");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                SiAuto.Main.LogWarning("Unrecognized node name: " + alertsNode.Name);
                            }
                        }
                        SiAuto.Main.LogMessage("Ending FOREACH loop to iterate the \"alerts\" level.");
                    }
                }
                SiAuto.Main.LogMessage("Ending FOREACH loop to iterate the XML root level.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Exceptions were detected reading the XML alerts or parsing XML: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.ReadAlertsFile");
        }

        private bool DoesAlertExist(String correlationID)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.DoesAlertExist");
            try
            {
                DataRow[] rows = activeAlerts.Select("Key='" + correlationID + "'");
                if (rows != null && rows.Length > 0 && rows[0] != null)
                {
                    SiAuto.Main.LogMessage("An alert with correlation ID " + correlationID + " exists.");
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.DoesAlertExist");
                    return true;
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError(ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.DoesAlertExist");
            return false;
        }

        private void InjectAlert(String correlationID, String healthTitle, String diskModel, String diskPath, String attributeID, String attributeName, bool isCritical)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.InjectAlert");
            try
            {
                DataRow injection = activeAlerts.NewRow();
                SiAuto.Main.LogMessage("Beginning new alert injection for correlation ID " + correlationID);
                injection["Key"] = new Guid(correlationID);
                injection["HealthTitle"] = healthTitle;
                injection["DiskModel"] = diskModel;
                injection["DiskPath"] = diskPath;
                injection["AttributeID"] = attributeID;
                injection["AttributeName"] = attributeName;
                injection["IsCritical"] = isCritical;
                injection["IsActive"] = true;
                injection["IsNotificateNeeded"] = true;
                SiAuto.Main.LogMessage("DataRow populated, committing changes.");
                activeAlerts.Rows.Add(injection);
                SiAuto.Main.LogMessage("DataRow changes committed, committing changes to the DataTable.");
                activeAlerts.AcceptChanges();
                SiAuto.Main.LogMessage("Changes committed. Injection complete.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Exceptions were detected during new alert injection: " + ex.Message);
                SiAuto.Main.LogException(ex);
                SiAuto.Main.LogWarning("Injection failed.");
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.InjectAlert");
        }

        private void UpdateAlert(String correlationID, String healthTitle, String attributeID, String attributeName, bool isCritical)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.UpdateAlert");
            try
            {
                DataRow[] rows = activeAlerts.Select("Key='" + correlationID + "'");
                if (rows != null && rows.Length > 0 && rows[0] != null)
                {
                    DataRow row = rows[0];
                    if (String.Compare(row["HealthTitle"].ToString(), healthTitle, true) == 0 &&
                        (bool)row["IsCritical"] == isCritical)
                    {
                        // Item is the same; don't update it other than IsActive flag and IsNotificateNeeded flag.
                        SiAuto.Main.LogMessage("No changes on item. We'll just set the IsActive flag to true and IsNotificateNeeded flag to false.");
                        row["IsActive"] = true;
                        row["IsNotificateNeeded"] = false;
                        row.AcceptChanges();
                        activeAlerts.AcceptChanges();
                        SiAuto.Main.LogMessage("Item updated; changes committed.");
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.UpdateAlert");
                        return;
                    }
                    else
                    {
                        // Update the item. Stuff like disk model, path won't change.
                        row["HealthTitle"] = healthTitle;
                        row["AttributeID"] = attributeID;
                        row["AttributeName"] = attributeName;
                        row["IsCritical"] = isCritical;
                        row["IsActive"] = true;
                        row["IsNotificateNeeded"] = true;
                        row.AcceptChanges();
                        activeAlerts.AcceptChanges();
                        SiAuto.Main.LogMessage("Item updated; changes committed.");
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.UpdateAlert");
                        return;
                    }
                }
                else
                {
                    SiAuto.Main.LogMessage("Hmm, the item with correlation ID " + correlationID + " has gone AWOL.");
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.HomeServerSMART2013.WindowSMARTTray.UpdateAlert");
                    return;
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Exceptions were detected during alert update: " + ex.Message);
                SiAuto.Main.LogException(ex);
                SiAuto.Main.LogWarning("Update failed.");
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.UpdateAlert");
        }

        private void CullAlerts()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.CullAlerts");
            try
            {
                DataRow[] rows = activeAlerts.Select();
                foreach (DataRow row in rows)
                {
                    if ((bool)row["IsActive"] == false)
                    {
                        SiAuto.Main.LogMessage("Deleting inactive row.");
                        row.Delete();
                        row.AcceptChanges();
                        SiAuto.Main.LogMessage("Deleted and committed changes to row.");
                    }
                }
                SiAuto.Main.LogMessage("Commit changes to active alerts table.");
                activeAlerts.AcceptChanges();
                SiAuto.Main.LogMessage("Changes committed.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError(ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.CullAlerts");
        }

        private void PostAlerts()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.PostAlerts");
            if (isSevere)
            {
                if (isTampering)
                {
                    if (!AreLocalNotificationServicesUsed())
                    {
                        DDisplayTooltip(20000, "WindowSMART 24/7 - License Tampering Detected", "The licensing system has detected tampering with the license. WindowSMART 24/7 will not function until you " +
                            "apply a valid license. The health of your disks is not being monitored. Click here to launch the console to purchase or enter your key.", ToolTipIcon.Error);
                        Thread.Sleep(20000);
                    }
                    if (isSnarlEnabled)
                    {
                        ornery.Notify("License Tampering Detected", "The licensing system has detected tampering with the license. WindowSMART 24/7 will not function until you " +
                            "apply a valid license. The health of your disks is not being monitored. Please launch the WindowSMART console to purchase or enter your key.", SnarlMessageClass.Hyperfatal);
                    }
                }
                else
                {
                    if (!AreLocalNotificationServicesUsed())
                    {
                        DDisplayTooltip(20000, "WindowSMART 24/7 - Trial Expired", "Your WindowSMART 24/7 trial has expired. Please purchase a license to continue using WindowSMART 24/7." +
                            "The health of your disks is no longer being monitored. Click here to launch the console to purchase or enter your key.", ToolTipIcon.Error);
                        Thread.Sleep(30000);
                    }
                    if (isSnarlEnabled)
                    {
                        ornery.Notify("Trial Expired", "Your WindowSMART 24/7 trial has expired. Please purchase a license to continue using WindowSMART 24/7." +
                            "The health of your disks is no longer being monitored. Please launch the WindowSMART console to purchase or enter your key.", SnarlMessageClass.Hyperfatal);
                    }
                }
                this.Icon = Properties.Resources.DiskHeartBeat16Red;
                return;
            }

            try
            {
                DataRow[] rows = activeAlerts.Select();
                int criticalCount = 0;
                int warningCount = 0;
                foreach (DataRow row in rows)
                {
                    if ((bool)row["IsNotificateNeeded"] == true)
                    {
                        SiAuto.Main.LogMessage("Found an active alert needing to be posted.");

                        if ((bool)row["IsCritical"])
                        {
                            criticalCount++;
                            if (!AreLocalNotificationServicesUsed())
                            {
                                DisplayTooltip(20000, row["DiskPath"].ToString() + " - " + row["HealthTitle"].ToString(), "Disk " + row["DiskModel"].ToString() + ", attribute " + row["AttributeID"].ToString() + " " +
                                    row["AttributeName"].ToString() + " has raised a Critical alert. Click here to open the WindowSMART 24/7 console for more details.", ToolTipIcon.Error);
                                // Sleep for 20 seconds to ensure the user has enough time to see it.
                                Thread.Sleep(20000);
                            }
                            if (isSnarlEnabled)
                            {
                                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                                sb.Append("Disk " + row["DiskModel"].ToString() + ", Path " + row["DiskPath"].ToString() + ", attribute " + row["AttributeID"].ToString() + " (" + row["AttributeName"].ToString() + ") - ");
                                sb.Append(row["HealthMessage"].ToString());
                                ornery.Notify(row["HealthTitle"].ToString(), sb.ToString(), SnarlMessageClass.Critical);
                            }
                        }
                        else
                        {
                            warningCount++;
                            if (!AreLocalNotificationServicesUsed())
                            {
                                DisplayTooltip(20000, row["DiskPath"].ToString() + " - " + row["HealthTitle"].ToString(), "Disk " + row["DiskModel"].ToString() + ", attribute " + row["AttributeID"].ToString() + " " +
                                    row["AttributeName"].ToString() + " has raised a Warning alert. Click here to open the WindowSMART 24/7 console for more details.", ToolTipIcon.Warning);
                                // Sleep for 20 seconds to ensure the user has enough time to see it.
                                Thread.Sleep(20000);
                            }
                            if (isSnarlEnabled)
                            {
                                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                                sb.Append("Disk " + row["DiskModel"].ToString() + ", Path " + row["DiskPath"].ToString() + ", attribute " + row["AttributeID"].ToString() + " (" + row["AttributeName"].ToString() + ") - ");
                                sb.Append(row["HealthMessage"].ToString());
                                ornery.Notify(row["HealthTitle"].ToString(), sb.ToString(), SnarlMessageClass.Warning);
                            }
                        }
                    }
                }
                if (criticalCount > 0)
                {
                    this.systemTray.Text = (isWindowsServerSolutions ? "Home Server SMART 24/7" : Properties.Resources.ApplicationTitle) + " - Critical (double-click for details)";
                    this.systemTray.Icon = Properties.Resources.DiskHeartBeat16Red;
                }
                else if (warningCount > 0)
                {
                    this.systemTray.Text = (isWindowsServerSolutions ? "Home Server SMART 24/7" : Properties.Resources.ApplicationTitle) + " - Warnings (double-click for details)";
                    this.systemTray.Icon = Properties.Resources.DiskHeartBeat16Yellow;
                }
                else
                {
                    this.systemTray.Text = (isWindowsServerSolutions ? "Home Server SMART 24/7" : Properties.Resources.ApplicationTitle);
                    this.systemTray.Icon = Properties.Resources.DiskHeartBeat16;
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError(ex.Message);
                SiAuto.Main.LogException(ex);
            }

            if (updateCheckIndex == 0)
            {
                if (isUpdateAvailable)
                {
                    if (!AreLocalNotificationServicesUsed())
                    {
                        DisplayTooltip(20000, (isWindowsServerSolutions ? "Home Server SMART " : "WindowSMART ") + "24/7 Update",
                            "A " + (isWindowsServerSolutions ? "Home Server SMART " : "WindowSMART ") + "24/7 update is available! Version " + updateVersion + " was released on " + updateReleaseDate +
                            ". Click here for more information and to download the update.", ToolTipIcon.Info, updateUrl);
                        Thread.Sleep(20000);
                    }
                    if (isSnarlEnabled)
                    {
                        ornery.Notify((isWindowsServerSolutions ? "Home Server SMART " : "WindowSMART "),
                            "A " + (isWindowsServerSolutions ? "Home Server SMART " : "WindowSMART ") + "24/7 update is available! Version " + updateVersion + " was released on " + updateReleaseDate +
                            ". Please visit " + updateUrl + " for more information and to download the update.",
                            SnarlMessageClass.General);
                    }
                }
                updateCheckIndex++;
                if (updateCheckIndex >= 15)
                {
                    updateCheckIndex = 0;
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.PostAlerts");
        }

        private void TrayForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.TrayForm_FormClosing");
            if (ornery != null)
            {
                try
                {
                    SiAuto.Main.LogMessage("Shutting down Snarl notification engine.");
                    ornery.Close();
                    ornery = null;
                }
                catch (Exception ex)
                {
                    SiAuto.Main.LogWarning(ex.Message);
                    SiAuto.Main.LogException(ex);
                }
            }

            try
            {
                SiAuto.Main.LogMessage("Stopping the background timer.");
                backgroundTimer.Stop();
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogWarning(ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.TrayForm_FormClosing");
        }

        private void DisplayTooltip(int timeout, String tooltipTitle, String tooltipMessage, ToolTipIcon icon, String tipUrl)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.DisplayTooltip");
            toolTipUrl = tipUrl;
            this.Invoke(displayTooltip, new object[] { timeout, tooltipTitle, tooltipMessage, icon });
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.DisplayTooltip");
        }

        private void DisplayTooltip(int timeout, String tooltipTitle, String tooltipMessage, ToolTipIcon icon)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.DisplayTooltip");
            DisplayTooltip(timeout, tooltipTitle, tooltipMessage, icon, String.Empty);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.DisplayTooltip");
        }

        private void DDisplayTooltip(int timeout, String tooltipTitle, String tooltipMessage, ToolTipIcon icon)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.DDisplayTooltip");
            SiAuto.Main.LogMessage("Running a UI delegate method to perform the operation. UI updates cannot be performed in a worker thread.");
            systemTray.ShowBalloonTip(timeout, tooltipTitle, tooltipMessage, icon);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.DDisplayTooltip");
        }

        private void DisplayDialogue(String message, String title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.DisplayDialogue");
            try
            {
                this.Invoke(displayMessage, new object[] { message, title, buttons, icon });
            }
            catch(Exception ex)
            {
                System.Media.SystemSounds.Hand.Play();
                SiAuto.Main.LogError(ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.DisplayDialogue");
        }

        private void DDisplayDialogue(String message, String title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.DDisplayDialogue");
            SiAuto.Main.LogMessage("Running a UI delegate method to perform the operation. UI updates cannot be performed in a worker thread.");
            try
            {
                QMessageBox.Show(message, title, buttons, icon, FormStartPosition.CenterScreen);
            }
            catch(Exception ex)
            {
                System.Media.SystemSounds.Hand.Play();
                SiAuto.Main.LogError(ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.DDisplayDialogue");
        }

        private void systemTray_DoubleClick(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.systemTray_DoubleClick");
            menuConsole_Click(sender, e);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.systemTray_DoubleClick");
        }

        private void systemTray_BalloonTipClicked(object sender, EventArgs e)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.systemTray_BalloonTipClicked");
            SiAuto.Main.LogString("toolTipUrl", toolTipUrl);
            if (toolTipUrl.StartsWith("http://", true, System.Globalization.CultureInfo.InvariantCulture))
            {
                Components.Utilities.Utility.LaunchBrowser(toolTipUrl);
            }
            else
            {
                StartConsole();
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.systemTray_BalloonTipClicked");
        }

        private void StartConsole()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.StartConsole");
            try
            {
                SiAuto.Main.LogMessage("Attempt to launch the WindowSMART console. This requires elevated perms.");
                System.Diagnostics.ProcessStartInfo psi;
                if (isWindowsServerSolutions)
                {
                    SiAuto.Main.LogMessage("Windows SKU reports Windows Server Solutions; will use the Dashboard.");
                    psi = new System.Diagnostics.ProcessStartInfo(Properties.Resources.DashboardLocation);
                }
                else if (System.IO.File.Exists(Properties.Resources.DashboardLocation))
                {
                    SiAuto.Main.LogMessage("Dashboard exists even though SKU doesn't report Windows Server Solutions; will use the Dashboard.");
                    psi = new System.Diagnostics.ProcessStartInfo(Properties.Resources.DashboardLocation);
                }
                else
                {
                    SiAuto.Main.LogMessage("Using WindowSMART.exe because Windows SKU does not indicate Windows Server Solutions.");
                    psi = new System.Diagnostics.ProcessStartInfo(xmlFilePath + "\\WindowSMART.exe");
                }
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                SiAuto.Main.LogMessage("Process arguments set. Starting process.");
                System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);
                SiAuto.Main.LogMessage("Process started.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Cannot start WindowSMART console. " + ex.Message);
                SiAuto.Main.LogException(ex);
                DisplayDialogue("Cannot start WindowSMART console. " + ex.Message, "Severe", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.StartConsole");
        }

        private void SendSnarlAlert(String message, String healthTitle, bool isCritical)
        {
            SendSnarlAlert(message, healthTitle, isCritical, null);
        }

        private void SendSnarlAlert(String message, String healthTitle, bool isCritical, SnarlMessageClass? msgClass)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.WindowSMARTTray.SendSnarlAlert");
            SiAuto.Main.LogBool("isSnarlEnabled", isSnarlEnabled);
            String servername = Environment.MachineName;

            if (!isSnarlEnabled)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.SendSnarlAlert");
                return;
            }
            else if (ornery == null)
            {
                ornery = new SnarlNotifications(false);
                ornery.Register();
            }

            String text = message.ToLower();
            SnarlMessageClass messageClass;

            if (msgClass == null)
            {
                if (text.Contains("cleared alert"))
                {
                    messageClass = SnarlMessageClass.Cleared;
                }
                else if (isCritical && text.Contains("smart threshold exceeds condition"))
                {
                    messageClass = SnarlMessageClass.Hyperfatal;
                }
                else if (text.Contains("smart value is equal to the non-zero threshold"))
                {
                    messageClass = SnarlMessageClass.Warning;
                }
                else if (text.Contains("licensing system detected tampering with the license"))
                {
                    messageClass = SnarlMessageClass.Hyperfatal;
                }
                else if (text.Contains("trial has expired"))
                {
                    messageClass = SnarlMessageClass.Hyperfatal;
                }
                else if (isCritical)
                {
                    messageClass = SnarlMessageClass.Critical;
                }
                else
                {
                    messageClass = SnarlMessageClass.Warning;
                }
            }
            else
            {
                messageClass = (SnarlMessageClass)msgClass;
            }

            try
            {
                ornery.Notify(healthTitle, message, messageClass);
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Snarl notification failed: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.WindowSMARTTray.SendSnarlAlert");
        }

        private bool AreLocalNotificationServicesUsed()
        {
            if (isSnarlEnabled || (isGrowlEnabled && !isGrowlRemoteEnabled))
            {
                return true;
            }
            return false;
        }
    }
}
