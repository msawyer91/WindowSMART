using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Management.Instrumentation;
using System.Text;

// Specify the namespace into which the data
// should be published 
[assembly: Instrumented("Root/DojoNorthSoftware/PrettyLittleSlobberhead")]
namespace TestHarnessUI
{
    // Let the system know that the InstallUtil.exe 
    // tool will be run against this assembly
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
