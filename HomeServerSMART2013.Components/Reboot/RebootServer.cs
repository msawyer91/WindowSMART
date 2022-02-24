using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Reboot
{
    public sealed class RebootServer
    {
        public static string REBOOT_SERVER_REASON_CODE = "Taryn10Guinevere28";

        [Flags]
        public enum ExitWindows : uint
        {
            // ONE of the following five:
            LogOff = 0x00000000,
            ShutDown = 0x00000001,
            Reboot = 0x00000002,
            PowerOff = 0x00000008,
            RestartApps = 0x00000040,
            // plus AT MOST ONE of the following two:
            Force = 0x00000004,
            ForceIfHung = 0x00000010,
        }

        [Flags]
        enum ShutdownReason : uint
        {
            MajorApplication = 0x00040000,
            MajorHardware = 0x00010000,
            MajorLegacyApi = 0x00070000,
            MajorOperatingSystem = 0x00020000,
            MajorOther = 0x00000000,
            MajorPower = 0x00060000,
            MajorSoftware = 0x00030000,
            MajorSystem = 0x00050000,

            MinorBlueScreen = 0x0000000F,
            MinorCordUnplugged = 0x0000000b,
            MinorDisk = 0x00000007,
            MinorEnvironment = 0x0000000c,
            MinorHardwareDriver = 0x0000000d,
            MinorHotfix = 0x00000011,
            MinorHung = 0x00000005,
            MinorInstallation = 0x00000002,
            MinorMaintenance = 0x00000001,
            MinorMMC = 0x00000019,
            MinorNetworkConnectivity = 0x00000014,
            MinorNetworkCard = 0x00000009,
            MinorOther = 0x00000000,
            MinorOtherDriver = 0x0000000e,
            MinorPowerSupply = 0x0000000a,
            MinorProcessor = 0x00000008,
            MinorReconfig = 0x00000004,
            MinorSecurity = 0x00000013,
            MinorSecurityFix = 0x00000012,
            MinorSecurityFixUninstall = 0x00000018,
            MinorServicePack = 0x00000010,
            MinorServicePackUninstall = 0x00000016,
            MinorTermSrv = 0x00000020,
            MinorUnstable = 0x00000006,
            MinorUpgrade = 0x00000003,
            MinorWMI = 0x00000015,

            FlagUserDefined = 0x40000000,
            FlagPlanned = 0x80000000
        }

        /// <summary>
        /// Orders a mandatory reboot on the Server.
        /// </summary>
        /// <param name="force">Forces running processes to exit if they are hung.</param>
        /// <param name="bruteForce">Perform a brute force shutdown - processes are forced to exit whether hung or not (may result in data loss).</param>
        public static void Reboot(bool force)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Reboot.RebootServer.Reboot");
            if (GrabRebootPerms())
            {
                SiAuto.Main.LogColored(System.Drawing.Color.Yellow, "A reboot of the Server has been ordered.");
                SiAuto.Main.LogBool("force", force);
                if (force)
                {
                    ExitWindowsEx(ExitWindows.Reboot | ExitWindows.ForceIfHung, ShutdownReason.FlagPlanned | ShutdownReason.MajorApplication);
                    SiAuto.Main.LogMessage("ExitWindowsEx interop call issued.");
                }
                else
                {
                    ExitWindowsEx(ExitWindows.Reboot, ShutdownReason.FlagPlanned | ShutdownReason.MajorApplication);
                    SiAuto.Main.LogMessage("ExitWindowsEx interop call issued.");
                }
            }
            else
            {
                String error = "The application request to acquire the token perms required to reboot the Server was rejected by the Server.";
                SiAuto.Main.LogError(error);
                throw new UnauthorizedAccessException(error);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Reboot.RebootServer.Reboot");
        }

        /// <summary>
        /// Orders the Server to shut down immediately.
        /// </summary>
        /// <param name="force">Forces running processes to exit if they are hung.</param>
        /// <param name="bruteForce">Perform a brute force shutdown - processes are forced to exit whether hung or not (may result in data loss).</param>
        public static void ShutDown(bool force)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Reboot.RebootServer.ShutDown");
            if (GrabRebootPerms())
            {
                SiAuto.Main.LogColored(System.Drawing.Color.Red, "A shutdown of the Server has been ordered.");
                SiAuto.Main.LogBool("force", force);
                if (force)
                {
                    ExitWindowsEx(ExitWindows.PowerOff | ExitWindows.ForceIfHung, ShutdownReason.FlagPlanned | ShutdownReason.MajorApplication);
                    SiAuto.Main.LogMessage("ExitWindowsEx interop call issued.");
                }
                else
                {
                    ExitWindowsEx(ExitWindows.PowerOff, ShutdownReason.FlagPlanned | ShutdownReason.MajorApplication);
                    SiAuto.Main.LogMessage("ExitWindowsEx interop call issued.");
                }
            }
            else
            {
                String error = "The application request to acquire the token perms required to shut down the Server was rejected by the Server.";
                SiAuto.Main.LogError(error);
                throw new UnauthorizedAccessException(error);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Reboot.RebootServer.ShutDown");
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ExitWindowsEx(ExitWindows uFlags, ShutdownReason dwReason);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall,
        ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr
        phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name,
        ref long pluid);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        internal const string SE_TIME_ZONE_NAMETEXT = "SeTimeZonePrivilege"; //http://msdn.microsoft.com/en-us/library/bb530716(VS.85).aspx

        private static bool GrabRebootPerms()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Reboot.RebootServer.GrabRebootPerms");
            try
            {
                bool retVal;
                TokPriv1Luid tp;
                IntPtr hproc = GetCurrentProcess();
                IntPtr htok = IntPtr.Zero;
                retVal = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
                tp.Count = 1;
                tp.Luid = 0;
                tp.Attr = SE_PRIVILEGE_ENABLED;
                retVal = LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);
                retVal = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Reboot.RebootServer.GrabRebootPerms");
                return retVal;
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogColored(System.Drawing.Color.Red, ex.Message);
                SiAuto.Main.LogException(ex);
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Reboot.RebootServer.GrabRebootPerms");
                return false;
            }
        }
    }
}
