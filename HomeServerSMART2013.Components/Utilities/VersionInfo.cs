using System;
using System.Reflection;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Debugging
{
    public static class VersionInfo
    {
        public static string MICROSOFT_DOT_NET_RUNTIME = "Pr3ttyL1ttle";

        public static AssemblyInformation GetAssemblyVersion(String fileName)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Debugging.GetAssemblyVersion");
            Assembly asm = null;
            AssemblyInformation info = new AssemblyInformation();

            try
            {
                asm = Assembly.LoadFrom(fileName);
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogWarning(ex.Message);
                SiAuto.Main.LogException(ex);
                info.ErrorMessage = ex.Message;
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Debugging.GetAssemblyVersion");
                return info;
            }

            try
            {
                if (asm != null)
                {
                    info.Name = asm.GetName().Name;
                    info.Version = asm.GetName().Version.ToString();
                    info.FullName = asm.GetName().ToString();
                    info.ReferencedAssemblies = asm.GetReferencedAssemblies();
                    info.IsDetected = true;
                    info.ErrorMessage = String.Empty;
                }
                else
                {
                    info.ErrorMessage = "Invalid assembly";
                    info.IsDetected = true;
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogWarning(ex.Message);
                SiAuto.Main.LogException(ex);
                info.ErrorMessage = ex.Message;
                info.IsDetected = true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Debugging.GetAssemblyVersion");
            return info;
        }
    }
}
