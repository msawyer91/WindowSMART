using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Instrumentation;

// Specify the namespace into which the data should be published.
[assembly: Instrumented("Root/DojoNorthSoftware/WindowSmart")]

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    // Let the system now that the InstallUtil.exe tool will need to run agianst this assembly.
    [System.ComponentModel.RunInstaller(true)]
    public class WmiServiceInstaller : DefaultManagementProjectInstaller
    {
        public WmiServiceInstaller()
        {
            ManagementInstaller managementInstaller = new ManagementInstaller();
            Installers.Add(managementInstaller);
        }
    }
}