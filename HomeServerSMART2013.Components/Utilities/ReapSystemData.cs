using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Windows.Forms;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Debugging
{
    public class ReapSystemData
    {
        private String programFiles;
        private String installLocation;
        private String expectedVersion;
        private String componentsVersion;
        private String uiComponentsVersion;
        private String uiVersion;
        private String serviceVersion;
        private String trayVersion;
        private int problemCount;
        private bool isWindowsServerSolutions;
        private bool isWindows8;

        public ReapSystemData(bool isWss, bool isW8)
        {
            isWindowsServerSolutions = isWss;
            isWindows8 = isW8;
            expectedVersion = componentsVersion = uiVersion = serviceVersion = trayVersion = "0.0.0.0";
            ConfigurePaths();
        }

        private void ConfigurePaths()
        {
            programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            if (isWindowsServerSolutions)
            {
                installLocation = programFiles + "\\" + Properties.Resources.InstallLocationPfSubtree;
            }
            else
            {
                installLocation = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            }
        }

        public String Reap(bool addHeader)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            expectedVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (addHeader)
            {
                sb.AppendLine(AddHeaderInfo());
            }
            sb.AppendLine(CollectGeneralComputerInfo());
            sb.AppendLine(GanderizeAssemblies());
            sb.AppendLine(ValidateInstallation());
            sb.AppendLine(VersionMatch());
            sb.AppendLine(HelperFiles());
            return sb.ToString();
        }

        private String AddHeaderInfo()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine(((isWindowsServerSolutions ? Properties.Resources.ApplicationTitleHss : Properties.Resources.ApplicationTitleWindowSmart) + " DIAGNOSTIC DEBUGGING REPORT").ToUpper());
            sb.AppendLine("Copyright (c) 2010-2018 Dojo North Software, LLC * All Rights Reserved");
            sb.AppendLine(String.Empty);
            return sb.ToString();
        }

        private String CollectGeneralComputerInfo()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            String separatorCharacter = "  ";

            sb.AppendLine("******************** BEGIN GENERAL COMPUTER INFO SECTION ********************");
            sb.AppendLine(PadStringWithTrailingSpaces("Computer Name", 20) + separatorCharacter + Environment.MachineName);
            sb.AppendLine(PadStringWithTrailingSpaces("Operating System", 20) + separatorCharacter + OSInfo.Name);
            sb.AppendLine(PadStringWithTrailingSpaces("OS Edition", 20) + separatorCharacter + OSInfo.Edition);
            sb.AppendLine(PadStringWithTrailingSpaces("Service Pack", 20) + separatorCharacter + OSInfo.ServicePack);
            sb.AppendLine(PadStringWithTrailingSpaces("Version", 20) + separatorCharacter + OSInfo.VersionString);
            sb.AppendLine(PadStringWithTrailingSpaces("System Type", 20) + separatorCharacter + OSInfo.Bits + "-bit Operating System");
            sb.AppendLine("******************** END OF GENERAL COMPUTER INFO SECTION ********************");
            sb.AppendLine(String.Empty);

            return sb.ToString();
        }

        private String GanderizeAssemblies()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("******************** BEGIN INSTALL VALIDATION SECTION ********************");
            sb.AppendLine("*** Self-Test Stage 1 of 4 - Collect Assembly Information ***");
            sb.AppendLine("");
            sb.AppendLine("*******************");
            sb.AppendLine("* CORE ASSEMBLIES *");
            sb.AppendLine("*******************");
            sb.AppendLine("");
            sb.AppendLine("=== HomeServerSMART2013.Components.dll ===");
            Debugging.AssemblyInformation info = Debugging.VersionInfo.GetAssemblyVersion(installLocation + "\\HomeServerSMART2013.Components.dll");
            if (info.IsDetected && String.IsNullOrEmpty(info.ErrorMessage))
            {
                sb.AppendLine("   Name: " + info.Name);
                sb.AppendLine("   Version: " + info.Version);
                componentsVersion = info.Version;
                sb.AppendLine("   Full Name: " + info.FullName);
                sb.AppendLine("");
                sb.AppendLine("   >>> Referenced Assemblies...");
                if (info.ReferencedAssemblies == null || info.ReferencedAssemblies.Length == 0)
                {
                    sb.AppendLine("      None detected.");
                }
                else
                {
                    foreach (System.Reflection.AssemblyName asm in info.ReferencedAssemblies)
                    {
                        sb.AppendLine("      " + asm.FullName);
                    }
                }
            }
            else
            {
                sb.AppendLine("   Exceptions were detected: " + info.ErrorMessage);
                sb.AppendLine("   Assembly file " + (info.IsDetected ? "WAS" : "was NOT") + " detected at the specified location.");
                problemCount++;
            }
            info = null;

            sb.AppendLine("");
            sb.AppendLine("=== HomeServerSMART2013.Components.UI.dll ===");
            info = VersionInfo.GetAssemblyVersion(installLocation + "\\HomeServerSMART2013.Components.UI.dll");
            if (info.IsDetected && String.IsNullOrEmpty(info.ErrorMessage))
            {
                sb.AppendLine("   Name: " + info.Name);
                sb.AppendLine("   Version: " + info.Version);
                uiComponentsVersion = info.Version;
                sb.AppendLine("   Full Name: " + info.FullName);
                sb.AppendLine("");
                sb.AppendLine("   >>> Referenced Assemblies...");
                if (info.ReferencedAssemblies == null || info.ReferencedAssemblies.Length == 0)
                {
                    sb.AppendLine("      None detected.");
                }
                else
                {
                    foreach (System.Reflection.AssemblyName asm in info.ReferencedAssemblies)
                    {
                        sb.AppendLine("      " + asm.FullName);
                    }
                }
            }

            sb.AppendLine("");
            if (isWindowsServerSolutions)
            {
                sb.AppendLine("=== HomeServerSMART2013.dll ===");
                info = VersionInfo.GetAssemblyVersion(installLocation + "\\HomeServerSMART2013.dll");
            }
            else
            {
                sb.AppendLine("=== WindowSMART.exe ===");
                info = VersionInfo.GetAssemblyVersion(installLocation + "\\WindowSMART.exe");
            }
            if (info.IsDetected && String.IsNullOrEmpty(info.ErrorMessage))
            {
                sb.AppendLine("   Name: " + info.Name);
                sb.AppendLine("   Version: " + info.Version);
                uiVersion = info.Version;
                sb.AppendLine("   Full Name: " + info.FullName);
                sb.AppendLine("");
                sb.AppendLine("   >>> Referenced Assemblies...");
                if (info.ReferencedAssemblies == null || info.ReferencedAssemblies.Length == 0)
                {
                    sb.AppendLine("      None detected.");
                }
                else
                {
                    foreach (System.Reflection.AssemblyName asm in info.ReferencedAssemblies)
                    {
                        sb.AppendLine("      " + asm.FullName);
                    }
                }
            }
            else
            {
                sb.AppendLine("   Exceptions were detected: " + info.ErrorMessage);
                sb.AppendLine("   Assembly file " + (info.IsDetected ? "WAS" : "was NOT") + " detected at the specified location.");
                problemCount++;
            }

            sb.AppendLine("");
            sb.AppendLine("=== HomeServerSMART2013.Service.exe ===");
            info = VersionInfo.GetAssemblyVersion(installLocation + "\\HomeServerSMART2013.Service.exe");
            if (info.IsDetected && String.IsNullOrEmpty(info.ErrorMessage))
            {
                sb.AppendLine("   Name: " + info.Name);
                sb.AppendLine("   Version: " + info.Version);
                serviceVersion = info.Version;
                sb.AppendLine("   Full Name: " + info.FullName);
                sb.AppendLine("");
                sb.AppendLine("   >>> Referenced Assemblies...");
                if (info.ReferencedAssemblies == null || info.ReferencedAssemblies.Length == 0)
                {
                    sb.AppendLine("      None detected.");
                }
                else
                {
                    foreach (System.Reflection.AssemblyName asm in info.ReferencedAssemblies)
                    {
                        sb.AppendLine("      " + asm.FullName);
                    }
                }
            }
            else
            {
                sb.AppendLine("   Exceptions were detected: " + info.ErrorMessage);
                sb.AppendLine("   Assembly file " + (info.IsDetected ? "WAS" : "was NOT") + " detected at the specified location.");
                problemCount++;
            }

            sb.AppendLine("");
            sb.AppendLine("=== WindowSMARTTray.exe ===");
            info = VersionInfo.GetAssemblyVersion(installLocation + "\\WindowSMARTTray.exe");
            if (info.IsDetected && String.IsNullOrEmpty(info.ErrorMessage))
            {
                sb.AppendLine("   Name: " + info.Name);
                sb.AppendLine("   Version: " + info.Version);
                trayVersion = info.Version;
                sb.AppendLine("   Full Name: " + info.FullName);
                sb.AppendLine("");
                sb.AppendLine("   >>> Referenced Assemblies...");
                if (info.ReferencedAssemblies == null || info.ReferencedAssemblies.Length == 0)
                {
                    sb.AppendLine("      None detected.");
                }
                else
                {
                    foreach (System.Reflection.AssemblyName asm in info.ReferencedAssemblies)
                    {
                        sb.AppendLine("      " + asm.FullName);
                    }
                }
            }
            else
            {
                sb.AppendLine("   Exceptions were detected: " + info.ErrorMessage);
                sb.AppendLine("   Assembly file " + (info.IsDetected ? "WAS" : "was NOT") + " detected at the specified location.");
                problemCount++;
            }

            sb.AppendLine("");
            sb.AppendLine("=== HomeServerSMART.Utility.dll ===");
            if (System.IO.File.Exists(installLocation + "\\HomeServerSMART.Utility.dll"))
            {
                sb.AppendLine("   COM interop component HomeServerSMART.Utility.dll was detected at the expected location.");
            }
            else
            {
                sb.AppendLine("   Critical COM interop component HomeServerSMART.Utility.dll is missing.");
                problemCount++;
            }

            sb.AppendLine("");
            sb.AppendLine("*********************");
            sb.AppendLine("* HELPER ASSEMBLIES *");
            sb.AppendLine("*********************");
            sb.AppendLine("");

            sb.AppendLine(String.Empty);
            sb.AppendLine("=== ChilkatDotNet4.dll ===");
            info = VersionInfo.GetAssemblyVersion(installLocation + "\\ChilkatDotNet4.dll");
            if (info.IsDetected && String.IsNullOrEmpty(info.ErrorMessage))
            {
                sb.AppendLine("   Name: " + info.Name);
                sb.AppendLine("   Version: " + info.Version);
                sb.AppendLine("   Full Name: " + info.FullName);
                sb.AppendLine("");
                sb.AppendLine("   >>> Referenced Assemblies...");
                if (info.ReferencedAssemblies == null || info.ReferencedAssemblies.Length == 0)
                {
                    sb.AppendLine("      None detected.");
                }
                else
                {
                    foreach (System.Reflection.AssemblyName asm in info.ReferencedAssemblies)
                    {
                        sb.AppendLine("      " + asm.FullName);
                    }
                }
            }
            else
            {
                sb.AppendLine("   Exceptions were detected: " + info.ErrorMessage);
                sb.AppendLine("   Assembly file " + (info.IsDetected ? "WAS" : "was NOT") + " detected at the specified location.");
                problemCount++;
            }

            sb.AppendLine(String.Empty);
            sb.AppendLine("=== Growl.Connector.dll ===");
            info = VersionInfo.GetAssemblyVersion(installLocation + "\\Growl.Connector.dll");
            if (info.IsDetected && String.IsNullOrEmpty(info.ErrorMessage))
            {
                sb.AppendLine("   Name: " + info.Name);
                sb.AppendLine("   Version: " + info.Version);
                sb.AppendLine("   Full Name: " + info.FullName);
                sb.AppendLine("");
                sb.AppendLine("   >>> Referenced Assemblies...");
                if (info.ReferencedAssemblies == null || info.ReferencedAssemblies.Length == 0)
                {
                    sb.AppendLine("      None detected.");
                }
                else
                {
                    foreach (System.Reflection.AssemblyName asm in info.ReferencedAssemblies)
                    {
                        sb.AppendLine("      " + asm.FullName);
                    }
                }
            }
            else
            {
                sb.AppendLine("   Exceptions were detected: " + info.ErrorMessage);
                sb.AppendLine("   Assembly file " + (info.IsDetected ? "WAS" : "was NOT") + " detected at the specified location.");
                problemCount++;
            }

            sb.AppendLine(String.Empty);
            sb.AppendLine("=== Growl.CoreLibrary.dll ===");
            info = VersionInfo.GetAssemblyVersion(installLocation + "\\Growl.CoreLibrary.dll");
            if (info.IsDetected && String.IsNullOrEmpty(info.ErrorMessage))
            {
                sb.AppendLine("   Name: " + info.Name);
                sb.AppendLine("   Version: " + info.Version);
                sb.AppendLine("   Full Name: " + info.FullName);
                sb.AppendLine("");
                sb.AppendLine("   >>> Referenced Assemblies...");
                if (info.ReferencedAssemblies == null || info.ReferencedAssemblies.Length == 0)
                {
                    sb.AppendLine("      None detected.");
                }
                else
                {
                    foreach (System.Reflection.AssemblyName asm in info.ReferencedAssemblies)
                    {
                        sb.AppendLine("      " + asm.FullName);
                    }
                }
            }
            else
            {
                sb.AppendLine("   Exceptions were detected: " + info.ErrorMessage);
                sb.AppendLine("   Assembly file " + (info.IsDetected ? "WAS" : "was NOT") + " detected at the specified location.");
                problemCount++;
            }

            sb.AppendLine(String.Empty);
            sb.AppendLine("=== Gurock.SmartInspect.dll ===");
            info = VersionInfo.GetAssemblyVersion(installLocation + "\\Gurock.SmartInspect.dll");
            if (info.IsDetected && String.IsNullOrEmpty(info.ErrorMessage))
            {
                sb.AppendLine("   Name: " + info.Name);
                sb.AppendLine("   Version: " + info.Version);
                sb.AppendLine("   Full Name: " + info.FullName);
                sb.AppendLine("");
                sb.AppendLine("   >>> Referenced Assemblies...");
                if (info.ReferencedAssemblies == null || info.ReferencedAssemblies.Length == 0)
                {
                    sb.AppendLine("      None detected.");
                }
                else
                {
                    foreach (System.Reflection.AssemblyName asm in info.ReferencedAssemblies)
                    {
                        sb.AppendLine("      " + asm.FullName);
                    }
                }
            }
            else
            {
                sb.AppendLine("   Exceptions were detected: " + info.ErrorMessage);
                sb.AppendLine("   Assembly file " + (info.IsDetected ? "WAS" : "was NOT") + " detected at the specified location.");
                problemCount++;
            }

            sb.AppendLine(String.Empty);
            sb.AppendLine("=== NMALib.dll ===");
            info = VersionInfo.GetAssemblyVersion(installLocation + "\\NMALib.dll");
            if (info.IsDetected && String.IsNullOrEmpty(info.ErrorMessage))
            {
                sb.AppendLine("   Name: " + info.Name);
                sb.AppendLine("   Version: " + info.Version);
                sb.AppendLine("   Full Name: " + info.FullName);
                sb.AppendLine("");
                sb.AppendLine("   >>> Referenced Assemblies...");
                if (info.ReferencedAssemblies == null || info.ReferencedAssemblies.Length == 0)
                {
                    sb.AppendLine("      None detected.");
                }
                else
                {
                    foreach (System.Reflection.AssemblyName asm in info.ReferencedAssemblies)
                    {
                        sb.AppendLine("      " + asm.FullName);
                    }
                }
            }
            else
            {
                sb.AppendLine("   Exceptions were detected: " + info.ErrorMessage);
                sb.AppendLine("   Assembly file " + (info.IsDetected ? "WAS" : "was NOT") + " detected at the specified location.");
                problemCount++;
            }

            sb.AppendLine(String.Empty);
            sb.AppendLine("=== Prowl.dll ===");
            info = VersionInfo.GetAssemblyVersion(installLocation + "\\Prowl.dll");
            if (info.IsDetected && String.IsNullOrEmpty(info.ErrorMessage))
            {
                sb.AppendLine("   Name: " + info.Name);
                sb.AppendLine("   Version: " + info.Version);
                sb.AppendLine("   Full Name: " + info.FullName);
                sb.AppendLine("");
                sb.AppendLine("   >>> Referenced Assemblies...");
                if (info.ReferencedAssemblies == null || info.ReferencedAssemblies.Length == 0)
                {
                    sb.AppendLine("      None detected.");
                }
                else
                {
                    foreach (System.Reflection.AssemblyName asm in info.ReferencedAssemblies)
                    {
                        sb.AppendLine("      " + asm.FullName);
                    }
                }
            }
            else
            {
                sb.AppendLine("   Exceptions were detected: " + info.ErrorMessage);
                sb.AppendLine("   Assembly file " + (info.IsDetected ? "WAS" : "was NOT") + " detected at the specified location.");
                problemCount++;
            }

            sb.AppendLine(String.Empty);
            sb.AppendLine("=== Pushover.dll ===");
            info = VersionInfo.GetAssemblyVersion(installLocation + "\\Pushover.dll");
            if (info.IsDetected && String.IsNullOrEmpty(info.ErrorMessage))
            {
                sb.AppendLine("   Name: " + info.Name);
                sb.AppendLine("   Version: " + info.Version);
                sb.AppendLine("   Full Name: " + info.FullName);
                sb.AppendLine("");
                sb.AppendLine("   >>> Referenced Assemblies...");
                if (info.ReferencedAssemblies == null || info.ReferencedAssemblies.Length == 0)
                {
                    sb.AppendLine("      None detected.");
                }
                else
                {
                    foreach (System.Reflection.AssemblyName asm in info.ReferencedAssemblies)
                    {
                        sb.AppendLine("      " + asm.FullName);
                    }
                }
            }
            else
            {
                sb.AppendLine("   Exceptions were detected: " + info.ErrorMessage);
                sb.AppendLine("   Assembly file " + (info.IsDetected ? "WAS" : "was NOT") + " detected at the specified location.");
                problemCount++;
            }

            sb.AppendLine(String.Empty);
            sb.AppendLine("=== RestSharp.dll ===");
            info = VersionInfo.GetAssemblyVersion(installLocation + "\\RestSharp.dll");
            if (info.IsDetected && String.IsNullOrEmpty(info.ErrorMessage))
            {
                sb.AppendLine("   Name: " + info.Name);
                sb.AppendLine("   Version: " + info.Version);
                sb.AppendLine("   Full Name: " + info.FullName);
                sb.AppendLine("");
                sb.AppendLine("   >>> Referenced Assemblies...");
                if (info.ReferencedAssemblies == null || info.ReferencedAssemblies.Length == 0)
                {
                    sb.AppendLine("      None detected.");
                }
                else
                {
                    foreach (System.Reflection.AssemblyName asm in info.ReferencedAssemblies)
                    {
                        sb.AppendLine("      " + asm.FullName);
                    }
                }
            }
            else
            {
                sb.AppendLine("   Exceptions were detected: " + info.ErrorMessage);
                sb.AppendLine("   Assembly file " + (info.IsDetected ? "WAS" : "was NOT") + " detected at the specified location.");
                problemCount++;
            }

            sb.AppendLine(String.Empty);
            sb.AppendLine("=== SnarlInterface.dll ===");
            info = VersionInfo.GetAssemblyVersion(installLocation + "\\SnarlInterface.dll");
            if (info.IsDetected && String.IsNullOrEmpty(info.ErrorMessage))
            {
                sb.AppendLine("   Name: " + info.Name);
                sb.AppendLine("   Version: " + info.Version);
                sb.AppendLine("   Full Name: " + info.FullName);
                sb.AppendLine("");
                sb.AppendLine("   >>> Referenced Assemblies...");
                if (info.ReferencedAssemblies == null || info.ReferencedAssemblies.Length == 0)
                {
                    sb.AppendLine("      None detected.");
                }
                else
                {
                    foreach (System.Reflection.AssemblyName asm in info.ReferencedAssemblies)
                    {
                        sb.AppendLine("      " + asm.FullName);
                    }
                }
            }
            else
            {
                sb.AppendLine("   Exceptions were detected: " + info.ErrorMessage);
                sb.AppendLine("   Assembly file " + (info.IsDetected ? "WAS" : "was NOT") + " detected at the specified location.");
                problemCount++;
            }

            sb.AppendLine(String.Empty);
            sb.AppendLine("=== WssControls.dll ===");
            info = VersionInfo.GetAssemblyVersion(installLocation + "\\WssControls.dll");
            if (info.IsDetected && String.IsNullOrEmpty(info.ErrorMessage))
            {
                sb.AppendLine("   Name: " + info.Name);
                sb.AppendLine("   Version: " + info.Version);
                sb.AppendLine("   Full Name: " + info.FullName);
                sb.AppendLine("");
                sb.AppendLine("   >>> Referenced Assemblies...");
                if (info.ReferencedAssemblies == null || info.ReferencedAssemblies.Length == 0)
                {
                    sb.AppendLine("      None detected.");
                }
                else
                {
                    foreach (System.Reflection.AssemblyName asm in info.ReferencedAssemblies)
                    {
                        sb.AppendLine("      " + asm.FullName);
                    }
                }
            }
            else
            {
                sb.AppendLine("   Exceptions were detected: " + info.ErrorMessage);
                sb.AppendLine("   Assembly file " + (info.IsDetected ? "WAS" : "was NOT") + " detected at the specified location.");
                problemCount++;
            }

            sb.AppendLine(String.Empty);
            sb.AppendLine("*** Self-Test - Stage 1 of 4 Complete ***");
            sb.AppendLine(String.Empty);
            return sb.ToString();
        }

        private String ValidateInstallation()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("");
            sb.AppendLine("*** Self-Test Stage 2 of 4 - Validate Installation ***");
            sb.AppendLine("");

            Microsoft.Win32.RegistryKey registryHklm = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey dojoNorthSubKey;
            Microsoft.Win32.RegistryKey configurationKey;

            sb.AppendLine("   Verify Root Registry Key Present and Writable");
            try
            {
                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, true);
                dojoNorthSubKey.SetValue("SelfTest", "Test");
                dojoNorthSubKey.DeleteValue("SelfTest");
                dojoNorthSubKey.Close();
                sb.AppendLine("      SUCCESS: Open key in R/W mode, create value, delete value, close key.");
            }
            catch (Exception ex)
            {
                sb.AppendLine("      FAILURE: Unable to read/write to HSS root registry location. " + ex.Message);
                problemCount++;
            }

            sb.AppendLine("   Verify Configuration Key Present and Writable");
            try
            {
                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, true);
                configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, true);
                configurationKey.SetValue("SelfTest", "Test");
                configurationKey.DeleteValue("SelfTest");
                configurationKey.Close();
                dojoNorthSubKey.Close();
                sb.AppendLine("      SUCCESS: Open key in R/W mode, create value, delete value, close key.");
            }
            catch (Exception ex)
            {
                sb.AppendLine("      FAILURE: Unable to read/write to HSS config registry location. " + ex.Message);
                problemCount++;
            }

            sb.AppendLine("   Verify Disk Key Present and Writable");
            try
            {
                dojoNorthSubKey = registryHklm.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, true);
                configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryMonitoredDisksKey, true);
                configurationKey.CreateSubKey("SelfTestNode");
                Microsoft.Win32.RegistryKey tempKey = configurationKey.OpenSubKey("SelfTestNode", true);
                tempKey.SetValue("SelfTest", "Test");
                tempKey.DeleteValue("SelfTest");
                tempKey.Close();
                configurationKey.DeleteSubKey("SelfTestNode");
                configurationKey.Close();
                dojoNorthSubKey.Close();
                sb.AppendLine("      SUCCESS: Open key in R/W mode, create subkey, create value, delete value, delete subkey, close key.");
            }
            catch (Exception ex)
            {
                sb.AppendLine("      FAILURE: Unable to read/write to HSS monitored disk registry location. " + ex.Message);
                problemCount++;
            }

            sb.AppendLine("   Verify Service Installed Correctly");
            try
            {
                System.ServiceProcess.ServiceController controller = new System.ServiceProcess.ServiceController(Properties.Resources.ServiceNameHss);
                sb.AppendLine("      Initialized ServiceController object with name \"dnhsSmart\" (Home Server SMART Service).");
                if (controller == null)
                {
                    sb.AppendLine("      ServiceController object is NULL; service is not installed correctly.");
                    problemCount++;
                }
                else
                {
                    String serviceState = controller.Status.ToString();
                    sb.AppendLine("      Service State: " + serviceState + " (service appears to be installed correctly)");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine("Failed to bind the service or get its status: " + ex.Message);
            }

            sb.AppendLine(String.Empty);
            sb.AppendLine("*** Self-Test - Stage 2 of 4 Complete ***");
            sb.AppendLine(String.Empty);
            return sb.ToString();
        }

        private String VersionMatch()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine(String.Empty);
            sb.AppendLine("*** Self-Test Stage 3 of 4 - Assembly Version Match ***");
            sb.AppendLine(String.Empty);

            sb.AppendLine("The expected product version is " + expectedVersion);
            sb.AppendLine(String.Empty);
            sb.AppendLine("HomeServerSMART2013.Components.dll version is " + componentsVersion + (expectedVersion == componentsVersion ? "" : "(mismatch)"));
            sb.AppendLine("HomeServerSMART2013.Components.UI.dll version is " + uiComponentsVersion + (expectedVersion == uiComponentsVersion ? "" : "(mismatch)"));
            if (isWindowsServerSolutions)
            {
                sb.AppendLine("HomeServerSMART2013.dll version is " + uiVersion + (expectedVersion == uiVersion ? "" : "(mismatch)"));
            }
            else
            {
                sb.AppendLine("WindowSMART.exe version is " + uiVersion + (expectedVersion == uiVersion ? "" : "(mismatch)"));
            }
            sb.AppendLine("HomeServerSMART2013.Service.exe version is " + serviceVersion + (expectedVersion == serviceVersion ? "" : "(mismatch)"));
            sb.AppendLine("WindowSMARTTray.exe version is " + trayVersion + (expectedVersion == trayVersion ? "" : "(mismatch)"));

            if (expectedVersion != componentsVersion)
                problemCount++;
            if (expectedVersion != uiComponentsVersion)
                problemCount++;
            if (expectedVersion != uiVersion)
                problemCount++;
            if (expectedVersion != serviceVersion)
                problemCount++;

            sb.AppendLine(String.Empty);
            sb.AppendLine("*** Self-Test - Stage 3 of 4 Complete ***");
            sb.AppendLine(String.Empty);
            return sb.ToString();
        }

        private String HelperFiles()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            bool fileExists = false;

            sb.AppendLine(String.Empty);
            sb.AppendLine("*** Self-Test Stage 4 of 4 - Helper Files ***");
            sb.AppendLine(String.Empty);

            fileExists = System.IO.File.Exists(installLocation + "\\" + Properties.Resources.IconAlertAppWarning);
            sb.AppendLine("App Warning icon file " + Properties.Resources.IconAlertAppWarning + ": " + (fileExists ? "Detected" : "Missing"));
            if (!fileExists)
            {
                problemCount++;
            }

            fileExists = System.IO.File.Exists(installLocation + "\\" + Properties.Resources.IconAlertCritical);
            sb.AppendLine("Critical Alert icon file " + Properties.Resources.IconAlertCritical + ": " + (fileExists ? "Detected" : "Missing"));
            if (!fileExists)
            {
                problemCount++;
            }

            fileExists = System.IO.File.Exists(installLocation + "\\" + Properties.Resources.IconAlertGeneral);
            sb.AppendLine("General Notificate icon file " + Properties.Resources.IconAlertGeneral + ": " + (fileExists ? "Detected" : "Missing"));
            if (!fileExists)
            {
                problemCount++;
            }

            fileExists = System.IO.File.Exists(installLocation + "\\" + Properties.Resources.IconAlertCleared);
            sb.AppendLine("Cleared Alert icon file " + Properties.Resources.IconAlertCleared + ": " + (fileExists ? "Detected" : "Missing"));
            if (!fileExists)
            {
                problemCount++;
            }

            fileExists = System.IO.File.Exists(installLocation + "\\" + Properties.Resources.IconAlertWarning);
            sb.AppendLine("Warning icon file " + Properties.Resources.IconAlertWarning + ": " + (fileExists ? "Detected" : "Missing"));
            if (!fileExists)
            {
                problemCount++;
            }

            fileExists = System.IO.File.Exists(installLocation + "\\" + Properties.Resources.IconAlertHyperfatal);
            sb.AppendLine("Hyperfatal icon file " + Properties.Resources.IconAlertHyperfatal + ": " + (fileExists ? "Detected" : "Missing"));
            if (!fileExists)
            {
                problemCount++;
            }

            if (isWindowsServerSolutions)
            {
                fileExists = System.IO.File.Exists(installLocation + "\\HomeServerSMART.chm");
                sb.AppendLine("Documentation file HomeServerSMART.chm: " + (fileExists ? "Detected" : "Missing"));
            }
            else
            {
                fileExists = System.IO.File.Exists(installLocation + "\\WindowSMART.chm");
                sb.AppendLine("Documentation file WindowSMART.chm: " + (fileExists ? "Detected" : "Missing"));
            }
            if (!fileExists)
            {
                problemCount++;
            }

            fileExists = System.IO.File.Exists(installLocation + "\\usb.if");
            sb.AppendLine("USB vendor definition file usb.if: " + (fileExists ? "Detected" : "Missing"));
            if (!fileExists)
            {
                problemCount++;
            }

            sb.AppendLine(String.Empty);
            sb.AppendLine("*** Self-Test - Stage 4 of 4 Complete ***");
            sb.AppendLine(String.Empty);
            sb.AppendLine(String.Empty);
            sb.AppendLine("Installation Issues Detected: " + problemCount.ToString());
            sb.AppendLine(String.Empty);

            sb.AppendLine("******************** END OF INSTALL VALIDATION SECTION ********************");
            return sb.ToString();
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

        public String DeepReapGeneralWmiQuery()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("******************** BEGIN GENERAL WMI QUERY SECTION ********************");

            if (isWindows8)
            {
                sb.AppendLine(String.Empty);
                sb.AppendLine("WINDOWS 8 ADVISORY - WINDOWS 8 / SERVER 2012 DETECTED");
                sb.AppendLine("If you are using Windows 8 Storage Spaces, the results you see for Win32_DiskDrive may not");
                sb.AppendLine("represent the physical disks actually installed in the computer. Disks participating in a");
                sb.AppendLine("Storage Spaces pool will not appear in Win32_DiskDrive results, but rather the pooled disk");
                sb.AppendLine("will appear. Since you are running Windows 8 or Server 2012, we will also run a WMI query");
                sb.AppendLine("on the new WMI class MSFT_PhysicalDisk, which will display all of the physical disks.");
                sb.AppendLine(String.Empty);
            }
            
            sb.AppendLine("****** Win32_DiskDrive General Query ******\r\n");
            int diskCount = 0;

            try
            {
                sb.AppendLine("WMI query:  \"select * from Win32_DiskDrive\"");
                sb.AppendLine("Executing FOREACH:  foreach (ManagementObject drive in new ManagementObjectSearcher(\"select * from Win32_DiskDrive\").Get())");
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                       "select * from Win32_DiskDrive").Get())
                {
                    sb.AppendLine("\tDetected Disk with DeviceID: " + drive["DeviceID"].ToString());
                    diskCount++;
                }
                sb.AppendLine("Win32_DiskDrive Result:  SUCCESS");
            }
            catch (Exception ex)
            {
                sb.AppendLine("Win32_DiskDrive Exception: " + ex.Message);
                sb.AppendLine("Win32_DiskDrive Stack Trace: " + ex.StackTrace);
                sb.AppendLine("Win32_DiskDrive Result:  FAILED");
            }
            finally
            {
                sb.AppendLine("We detected " + diskCount.ToString() + " disks in your Server.");
                sb.AppendLine("****** Win32_DiskDrive General Query Complete ******\r\n");
            }

            sb.AppendLine("****** Win32_DiskDrive Per-Disk Size/Model Data Fetch Query ******\r\n");

            try
            {
                sb.AppendLine("WMI query:  \"select * from Win32_DiskDrive\"");
                sb.AppendLine("Executing FOREACH:  foreach (ManagementObject drive in new ManagementObjectSearcher(\"select * from Win32_DiskDrive\").Get())");
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                       "select * from Win32_DiskDrive").Get())
                {
                    sb.AppendLine("\tDetected Disk with DeviceID: " + drive["DeviceID"].ToString());
                    sb.AppendLine("\t\tDisk Model: " + (drive["Model"] == null ? "Model is NULL" : drive["Model"].ToString()));
                    if (drive["Size"] != null)
                    {
                        sb.AppendLine("\t\tDisk Size : " + drive["Size"].ToString() + " bytes");
                        sb.AppendLine("\t\tAttempting conversion to GB...");
                        decimal capacity = 0.00M;
                        decimal giggage = 0.00M;
                        giggage = Decimal.Parse(drive["Size"].ToString());
                        capacity = Decimal.Round(giggage / 1073741824.00M, 2);
                        sb.AppendLine("\t\tDisk Size : " + capacity.ToString() + " GB\r\n");
                    }
                    else
                    {
                        sb.AppendLine("\t\tDisk Size : NULL\r\n");
                    }
                }
                sb.AppendLine("Per-Disk Size/Model Result:  SUCCESS");
            }
            catch (Exception ex)
            {
                sb.AppendLine("Per-Disk Size/Model Exception: " + ex.Message);
                sb.AppendLine("Per-Disk Size/Model Trace: " + ex.StackTrace);
                sb.AppendLine("Per-Disk Size/Model Result:  FAILED");
            }
            finally
            {
                sb.AppendLine("****** Win32_DiskDrive Per-Disk Size/Model Data Fetch Query Complete ******\r\n");
            }

            if (isWindows8)
            {
                sb.AppendLine("****** MSFT_PhysicalDisk Per-Disk Size/Model Data Fetch Query ******\r\n");
                diskCount = 0;

                try
                {
                    sb.AppendLine("WMI query:  \"select * from MSFT_PhysicalDisk\" (this is a Windows 8/Server 2012 query)");
                    sb.AppendLine("Executing FOREACH:  foreach (ManagementObject drive in new ManagementObjectSearcher(\"select * from MSFT_PhysicalDisk\").Get())");
                    foreach (ManagementObject drive in new ManagementObjectSearcher(Properties.Resources.WmiQueryScope,
                           "select * from MSFT_PhysicalDisk").Get())
                    {
                        sb.AppendLine("\tDetected Disk with DeviceID: " + drive["DeviceID"].ToString());
                        diskCount++;
                    }
                    sb.AppendLine("MSFT_PhysicalDisk Result:  SUCCESS");
                }
                catch (Exception ex)
                {
                    sb.AppendLine("MSFT_PhysicalDisk Exception: " + ex.Message);
                    sb.AppendLine("MSFT_PhysicalDisk Stack Trace: " + ex.StackTrace);
                    sb.AppendLine("MSFT_PhysicalDisk Result:  FAILED");
                }
                finally
                {
                    sb.AppendLine("We detected " + diskCount.ToString() + " disks in your Server.");
                    sb.AppendLine("****** MSFT_PhysicalDisk General Query Complete ******\r\n");
                }

                sb.AppendLine("****** MSFT_PhysicalDisk General Query ******\r\n");
                diskCount = 0;

                try
                {
                    sb.AppendLine("WMI query:  \"select * from MSFT_PhysicalDisk\" (this is a Windows 8/Server 2012 query)");
                    sb.AppendLine("Executing FOREACH:  foreach (ManagementObject drive in new ManagementObjectSearcher(\"select * from MSFT_PhysicalDisk\").Get())");
                    foreach (ManagementObject drive in new ManagementObjectSearcher(Properties.Resources.WmiQueryScope,
                           "select * from MSFT_PhysicalDisk").Get())
                    {
                        sb.AppendLine("\tDetected Disk with DeviceID: " + drive["DeviceID"].ToString());
                        diskCount++;
                    }
                    sb.AppendLine("MSFT_PhysicalDisk Result:  SUCCESS");
                }
                catch (Exception ex)
                {
                    sb.AppendLine("MSFT_PhysicalDisk Exception: " + ex.Message);
                    sb.AppendLine("MSFT_PhysicalDisk Stack Trace: " + ex.StackTrace);
                    sb.AppendLine("MSFT_PhysicalDisk Result:  FAILED");
                }
                finally
                {
                    sb.AppendLine("We detected " + diskCount.ToString() + " disks in your Server.");
                    sb.AppendLine("****** MSFT_PhysicalDisk General Query Complete ******\r\n");
                }

                sb.AppendLine("****** MSFT_PhysicalDisk Per-Disk Size/Model Data Fetch Query ******\r\n");

                try
                {
                    sb.AppendLine("WMI query:  \"select * from MSFT_PhysicalDisk\"");
                    sb.AppendLine("Executing FOREACH:  foreach (ManagementObject drive in new ManagementObjectSearcher(\"select * from MSFT_PhysicalDisk\").Get())");
                    foreach (ManagementObject drive in new ManagementObjectSearcher(Properties.Resources.WmiQueryScope,
                           "select * from MSFT_PhysicalDisk").Get())
                    {
                        sb.AppendLine("\tDetected Disk with DeviceID: " + drive["DeviceID"].ToString());
                        try
                        {
                            sb.AppendLine("\t\tDisk Model: " + (drive["Model"] == null ? "Model is NULL" : drive["Model"].ToString()));
                        }
                        catch (Exception dex)
                        {
                            sb.AppendLine("\t\tDisk Model: " + dex.Message);
                        }
                        if (drive["Size"] != null)
                        {
                            sb.AppendLine("\t\tDisk Size : " + drive["Size"].ToString() + " bytes");
                            sb.AppendLine("\t\tAttempting conversion to GB...");
                            decimal capacity = 0.00M;
                            decimal giggage = 0.00M;
                            giggage = Decimal.Parse(drive["Size"].ToString());
                            capacity = Decimal.Round(giggage / 1073741824.00M, 2);
                            sb.AppendLine("\t\tDisk Size : " + capacity.ToString() + " GB\r\n");
                        }
                        else
                        {
                            sb.AppendLine("\t\tDisk Size : NULL\r\n");
                        }
                    }
                    sb.AppendLine("Per-Disk Size/Model Result:  SUCCESS");
                }
                catch (Exception ex)
                {
                    sb.AppendLine("Per-Disk Size/Model Exception: " + ex.Message);
                    sb.AppendLine("Per-Disk Size/Model Trace: " + ex.StackTrace);
                    sb.AppendLine("Per-Disk Size/Model Result:  FAILED");
                }
                finally
                {
                    sb.AppendLine("****** MSFT_PhysicalDisk Per-Disk Size/Model Data Fetch Query Complete ******\r\n");
                }
            }

            sb.AppendLine("******************** END OF GENERAL WMI QUERY SECTION ********************");
            return sb.ToString();
        }

        public String DeepReapSiIQuery()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("******************** BEGIN SII CONTROLLER WMI QUERY SECTION ********************");
            sb.AppendLine(String.Empty);

            bool isSiIDetected = false;
            List<string> siIControllers = new List<string>();

            sb.AppendLine("****** Detecting Silicon Image SiI Controllers ******\r\n");
            try
            {
                sb.AppendLine("WMI query:  \"select Name, DeviceID from Win32_SCSIController\"");
                sb.AppendLine("Executing FOREACH:  foreach (ManagementObject drive in new ManagementObjectSearcher(\"select Name, DeviceID from Win32_SCSIController\").Get())");
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                       "select Name, DeviceID from Win32_SCSIController").Get())
                {
                    sb.AppendLine("\tDetected Controller with DeviceID: " + drive["DeviceID"].ToString());
                    String controllerName = drive["Name"].ToString();
                    if (controllerName.ToUpper().Contains("SILICON IMAGE"))
                    {
                        siIControllers.Add(drive["DeviceID"].ToString());
                        sb.AppendLine("\t\tSilicon Image SiI Controller detected!");
                        isSiIDetected = true;
                    }
                }
                sb.AppendLine("SiI Detection Result:  " + (isSiIDetected ? "At least one SiI controller detected." :
                    "No SiI controllers detected. No further action required."));
            }
            catch (Exception ex)
            {
                sb.AppendLine("SiI Detection Exception: " + ex.Message);
                sb.AppendLine("SiI Detection Stack Trace: " + ex.StackTrace);
                sb.AppendLine("SiI Detection Result:  FAILED (cannot continue further; halting)");
                isSiIDetected = false;
            }
            finally
            {
                sb.AppendLine("****** Silicon Image SiI Controller Detection is Complete ******\r\n");
            }

            if (!isSiIDetected)
            {
                sb.AppendLine("******************** END OF SII CONTROLLER WMI QUERY SECTION ********************");
                return sb.ToString();
            }

            sb.AppendLine("****** Searching for Disks on SiI Controllers ******\r\n");

            isSiIDetected = false;
            List<string> nonRemovables = new List<string>();

            foreach (String controller in siIControllers)
            {
                String testController = controller.Replace("\\", "\\\\");
                try
                {
                    sb.AppendLine("WMI query:  \"ASSOCIATORS OF {Win32_SCSIController.DeviceID=\"" + testController + "\"} WHERE AssocClass = Win32_SCSIControllerDevice\"");
                    sb.AppendLine("Executing FOREACH:  foreach (ManagementObject drive in new ManagementObjectSearcher(\"ASSOCIATORS OF {Win32_SCSIController.DeviceID=\"" + testController + "\"} WHERE AssocClass = Win32_SCSIControllerDevice\").Get())");
                    foreach (ManagementObject drive in new ManagementObjectSearcher(
                           "ASSOCIATORS OF {Win32_SCSIController.DeviceID=\"" + testController + "\"} WHERE AssocClass = Win32_SCSIControllerDevice").Get())
                    {
                        sb.AppendLine("\tDetected Device with DeviceID: " + drive["DeviceID"].ToString());
                        String device = drive["DeviceID"].ToString();
                        if (device.ToUpper().Contains("SCSI\\DISK"))
                        {
                            nonRemovables.Add(drive["DeviceID"].ToString());
                            sb.AppendLine("\t\tDevice is a disk drive.");
                            isSiIDetected = true;
                        }
                        else
                        {
                            sb.AppendLine("\t\tDevice is NOT a disk drive.");
                        }
                    }
                    sb.AppendLine("SiI Disk Detection Result:  " + (isSiIDetected ? "At least one non-removable disk detected." :
                        "No non-removable disks detected. No further action required."));
                }
                catch (Exception ex)
                {
                    sb.AppendLine("SiI Disk Detection Exception: " + ex.Message);
                    sb.AppendLine("SiI Disk Detection Stack Trace: " + ex.StackTrace);
                    sb.AppendLine("SiI Disk Detection Result:  FAILED (cannot continue further; halting)");
                }
                finally
                {
                    
                }
            }

            sb.AppendLine("****** SiI Disk Detection is Complete ******\r\n");
            sb.AppendLine("******************** END OF SII CONTROLLER WMI QUERY SECTION ********************");

            return sb.ToString();
        }

        public String DeepReapUsbInfo()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("******************** BEGIN USB WMI QUERY SECTION ********************");
            sb.AppendLine(String.Empty);

            System.IO.StreamReader reader = null;
            DataTable vendorTable = new DataTable("Vendors");

            DataColumn[] keys = new DataColumn[1];
            DataColumn idColumn = new DataColumn();
            idColumn.ColumnName = "VID";
            idColumn.DataType = typeof(int);
            keys[0] = idColumn;
            vendorTable.Columns.Add(idColumn);
            vendorTable.Columns.Add("Vendor", typeof(String));
            vendorTable.PrimaryKey = keys;
            vendorTable.AcceptChanges();
            bool lookupAvailable = false;

            try
            {
                if (System.IO.File.Exists(installLocation + "\\usb.if"))
                {
                    int vendors = 0;
                    int errors = 0;
                    sb.AppendLine("*** Loading usb.org vendor data from usb.if...");
                    reader = new System.IO.StreamReader(installLocation + "\\usb.if");
                    while (!reader.EndOfStream)
                    {
                        int vendorID = 0;
                        String line = reader.ReadLine();
                        try
                        {
                            //vendorID = Int32.Parse(line.Substring(0, 4));
                            vendorID = Int32.Parse(line.Substring(0, line.IndexOf('|')));
                            DataRow row = vendorTable.NewRow();
                            row["VID"] = vendorID;
                            row["Vendor"] = line.Substring(line.IndexOf('|') + 1);
                            vendorTable.Rows.Add(row);
                        }
                        catch (System.Data.ConstraintException)
                        {
                            sb.AppendLine("\tWARNING: Error in usb.if, line " + vendors.ToString() + ". Duplicate vendor ID " + line.Substring(0, 4) + ". Each VID must be defined only once.");
                            errors++;
                        }
                        catch
                        {
                            sb.AppendLine("\tWARNING: Error in usb.if, line " + vendors.ToString() + ". Invalid vendor ID " + line.Substring(0, 4) + "; cannot parse to integer.");
                            errors++;
                        }
                        vendors++;
                    }
                    vendors = vendors - errors;
                    vendorTable.AcceptChanges();
                    sb.AppendLine("*** Load done, " + vendors.ToString() + " vendors loaded.\r\n");
                    lookupAvailable = true;
                }
                else
                {
                    sb.AppendLine("WARNING: usb.if was not detected; vendor lookups will not be available.");
                    lookupAvailable = false;
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine("WARNING: usb.if failed to load (" + ex.Message + "); vendor lookups will not be available.");
                lookupAvailable = false;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            try
            {
                sb.AppendLine("Getting a list of all USB devices installed in the Server.\r\n");

                sb.AppendLine("\t***************");
                sb.AppendLine("\t* USB Devices *");
                sb.AppendLine("\t***************\r\n");

                int deviceNo = 0;
                foreach (ManagementObject hub in new ManagementObjectSearcher("select * from Win32_USBHub").Get())
                {
                    sb.AppendLine("Device " + deviceNo.ToString());
                    String vidPid = hub["DeviceID"].ToString();
                    sb.AppendLine("\tDevice ID:" + vidPid);
                    sb.AppendLine("\tPNP ID:" + hub["PNPDeviceID"].ToString());
                    if (vidPid.Contains("VID_") && vidPid.Contains("PID_"))
                    {
                        sb.AppendLine("\t\tVID: " + vidPid.Substring(vidPid.IndexOf("VID_") + 4, 4) + "    PID: " + vidPid.Substring(vidPid.IndexOf("PID_") + 4, 4));
                        if (lookupAvailable)
                        {
                            int vid = Int32.Parse(vidPid.Substring(vidPid.IndexOf("VID_") + 4, 4), System.Globalization.NumberStyles.HexNumber);
                            DataRow[] rows = vendorTable.Select("VID='" + vid.ToString() + "'");
                            if (rows != null && rows.Length > 0)
                            {
                                foreach (DataRow row in rows)
                                {
                                    sb.AppendLine("\t\tVendor: " + row["Vendor"].ToString());
                                }
                            }
                            else
                            {
                                sb.AppendLine("\t\tVendor " + vidPid.Substring(vidPid.IndexOf("VID_") + 4, 4) + " (" + vid.ToString() + ") is NOT DEFINED in usb.org definitions file usb.if.");
                            }
                        }
                    }
                    sb.AppendLine("\tDescription:" + (hub["Description"] == null ? "None" : hub["Description"].ToString()));
                    sb.AppendLine("");
                    deviceNo++;
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine("Damn, unable to enumerate USB devices. " + ex.Message);
            }

            try
            {
                sb.AppendLine("Getting a list of all USB storage devices installed in the Server.\r\n");

                sb.AppendLine("\t***********************");
                sb.AppendLine("\t* USB Storage Devices *");
                sb.AppendLine("\t***********************\r\n");

                int deviceNo = 0;

                // Browse all WMI physical disks.
                foreach (ManagementObject drive in new ManagementObjectSearcher(
                  "select * from Win32_DiskDrive").Get())
                {
                    String diskPath = String.Empty;
                    String diskID = String.Empty;

                    diskPath = drive["DeviceID"].ToString();
                    diskID = drive["PNPDeviceID"].ToString();

                    if (diskID.Contains("USBSTOR"))
                    {
                        sb.AppendLine("USB Storage Device " + deviceNo.ToString());
                        sb.AppendLine("\tDevice ID:" + diskPath);
                        sb.AppendLine("\tPNP ID:" + diskID);
                        sb.AppendLine("\tModel: " + (drive["Model"] == null ? "Not Available" : drive["Model"].ToString()));
                    }
                }
                sb.AppendLine("");
            }
            catch (Exception ex)
            {
                sb.AppendLine("Damn, unable to enumerate USB disks. " + ex.Message);
            }
            finally
            {
                sb.AppendLine("******************** END OF USB WMI QUERY SECTION ********************\r\n");
            }

            return sb.ToString();
        }
    }
}
