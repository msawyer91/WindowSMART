using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.ServiceProcess;
using System.Text;

namespace DojoNorthSoftware.WindowSMART
{
    [Cmdlet(VerbsCommon.Get, "WindowSmartServiceStatus")]
    public class GetWSmartServiceStatus : Cmdlet
    {
        protected override void EndProcessing()
        {
            ServiceController controller;

            try
            {
                controller = new ServiceController(Properties.Resources.ServiceNameHss);

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine(String.Empty);
                sb.AppendLine("     Service Name        : " + controller.ServiceName);
                sb.AppendLine("     Service Display Name: " + controller.DisplayName);
                sb.AppendLine("     Service Status      : " + controller.Status.ToString());
                sb.AppendLine("     Server              : " + System.Environment.MachineName);
                sb.AppendLine("     Execution Flags     : " + (controller.CanPauseAndContinue ? "CAN_PAUSE " : "") +
                    (controller.CanShutdown ? "SHUTDOWN_NOTIFY " : "") + (controller.CanStop ? "CAN_STOP" : ""));
                sb.AppendLine(String.Empty);
                WriteObject(sb.ToString(), true);
            }
            catch (Exception ex)
            {
                throw new WindowSmartPSException("Service bind failed.", ex);
            }
        }
    }

    [Cmdlet(VerbsCommon.Set, "WindowSmartService")]
    public class SetWSmartService : Cmdlet
    {
        [Parameter(Mandatory = false)]
        public SwitchParameter Stop;

        [Parameter(Mandatory = false)]
        public SwitchParameter Start;

        [Parameter(Mandatory = false)]
        public SwitchParameter Restart;

        protected override void EndProcessing()
        {
            int parmsPresent = 0;
            if (Stop.IsPresent)
            {
                parmsPresent++;
            }
            if (Start.IsPresent)
            {
                parmsPresent++;
            }
            if (Restart.IsPresent)
            {
                parmsPresent++;
            }

            if (parmsPresent == 0)
            {
                throw new WindowSmartPSException("No parameters were specified. You must specify -Start, -Stop or -Restart.");
            }
            else if (parmsPresent > 1)
            {
                throw new WindowSmartPSException("Too many parameters were specified. You must specify only one parameter: -Start, -Stop or -Restart.");
            }

            if (Stop.IsPresent)
            {
                PowerShellActions.StopService();
            }
            else if (Start.IsPresent)
            {
                PowerShellActions.StartService();
            }
            else
            {
                PowerShellActions.StopService();
                PowerShellActions.StartService();
            }
        }

        [Cmdlet(VerbsCommon.Clear, "WindowSmartLicense")]
        public class ClearWSmartLicense : Cmdlet
        {
            [Parameter(Mandatory = false)]
            public SwitchParameter Restart;

            protected override void EndProcessing()
            {
                if (ShouldProcess("WindowSMART License"))
                {
                    if (ShouldContinue("Clear-WindowSmartLicense", "Are you sure you want to remove the license and revert to trial mode?"))
                    {
                        PowerShellActions.VoidLicense();
                        WriteObject("WindowSMART license was successfully invalidated and reverted to trial mode.");
                        if (Restart.IsPresent)
                        {
                            PowerShellActions.StopService();
                            PowerShellActions.StartService();
                        }
                        else
                        {
                            WriteObject("Changes will not take effect until you restart the WindowSMART service or");
                            WriteObject("reboot the computer. Use Set-WindowSmartService -Restart to restart the");
                            WriteObject("service.");
                        }
                    }
                }
            }
        }

        [Cmdlet(VerbsCommon.Get, "WindowSmartLicense")]
        public class GetWSmartLicense : Cmdlet
        {
            [Parameter(Mandatory = false)]
            public SwitchParameter Restart;

            protected override void EndProcessing()
            {
                Guid refGuid = new Guid("{00000000-0000-0000-0000-000000000000}");
                String result = PowerShellActions.GetLicense(refGuid);
                if (String.IsNullOrEmpty(result))
                {
                    WriteWarning("No license data was returned. It may not be supported or required on this computer.");
                }
                else
                {
                    WriteObject(result);
                }
            }
        }

        [Cmdlet(VerbsCommon.Set, "WindowSmartLicense")]
        public class SetWSmartLicense : Cmdlet
        {
            [Parameter(Mandatory = false)]
            public String LiteralPath;

            [Parameter(Mandatory = false)]
            public String Xml;

            [Parameter(Mandatory = false)]
            public SwitchParameter Restart;

            [Parameter(Mandatory = false)]
            public SwitchParameter Rekey;

            protected override void EndProcessing()
            {
                if ((String.IsNullOrEmpty(LiteralPath) && String.IsNullOrEmpty(Xml)) ||
                    (!String.IsNullOrEmpty(LiteralPath) && !String.IsNullOrEmpty(Xml)))
                {
                    throw new WindowSmartPSException("You must specify either -LiteralPath or -Xml, but you may not specify both.");
                }

                int result = 0x0;
                if (String.IsNullOrEmpty(LiteralPath))
                {
                    result = PowerShellActions.ApplyLicense(Xml, false, Restart.IsPresent);
                }
                else
                {
                    result = PowerShellActions.ApplyLicense(LiteralPath, true, Restart.IsPresent);
                }

                if (result == 0x0)
                {
                    WriteObject("License operation successful (0x0).");
                }
                else if (result == 0x1)
                {
                    WriteWarning("License installation succeeded. However, an error occurred restarting the service.");
                    WriteWarning("Please use Set-WindowSmartService -Restart to restart it manually.");
                }
                else
                {
                    throw new WindowSmartPSException("Error applying license. Error code (0x" + result.ToString() + ")");
                }
            }
        }

        [Cmdlet(VerbsCommon.Get, "WindowSmartDiskInfo")]
        public class GetWSmartDiskInfo : Cmdlet
        {
            [Parameter(Mandatory = true)]
            public String DiskPath;
            [Parameter(Mandatory = false)]
            public SwitchParameter ExportToHtml;
            [Parameter(Mandatory = false)]
            public SwitchParameter ExportToText;
            [Parameter(Mandatory = false)]
            public String ExportFileName;
            
            protected override void EndProcessing()
            {
                if (String.IsNullOrEmpty(DiskPath))
                {
                    throw new WindowSmartPSException("A valid disk path must be specified. You may specify either the disk number (i.e. 0) or the full path (i.e. \\\\.\\PHYSICALDRIVE0).");
                }
                if (ExportToHtml.IsPresent && ExportToText.IsPresent)
                {
                    throw new WindowSmartPSException("You may specify either -ExportToHtml or -ExportToText, but not both.");
                }
            }
        }
    }
}
