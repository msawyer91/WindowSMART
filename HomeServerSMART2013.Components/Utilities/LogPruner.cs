using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Debugging
{
    public sealed class LogPruner
    {
        public static void ObliterateOldLogs(String path, String prefix, String extension, int obliterationDayLimit)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.Debugging.LogPruner.ObliterateOldLogs");
            SiAuto.Main.LogString("path", path);
            SiAuto.Main.LogString("prefix", prefix);
            SiAuto.Main.LogString("extension", extension);
            SiAuto.Main.LogInt("obliterationDayLimit", obliterationDayLimit);
            DateTime date = DateTime.Now;
            DateTime obliterateDate = date.AddDays(-obliterationDayLimit);
            SiAuto.Main.LogDateTime("date", date);
            SiAuto.Main.LogDateTime("obliterateDate", obliterateDate);

            SiAuto.Main.LogMessage("[Logfile Obliterator] The Server automatically obliterates logs older than 14 days.");

            DirectoryInfo fileListing = new DirectoryInfo(path);
            SiAuto.Main.LogMessage("[Logfile Obliterator] Detecting obliteration candidates with prefix " + prefix + " and extension " + extension);
            int candidates = 0;
            int itemsWhacked = 0;
            int exceptionsDetected = 0;
            try
            {
                foreach (FileInfo f in fileListing.GetFiles())
                {
                    if (f.Name.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase) && f.Name.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase) &&
                        f.LastWriteTime <= obliterateDate)
                    {
                        candidates++;
                        try
                        {
                            SiAuto.Main.LogMessage("[Logfile Obliterator] Obliterating " + f.FullName);
                            File.Delete(f.FullName);
                            itemsWhacked++;
                            SiAuto.Main.LogMessage("[Logfile Obliterator] Obliterated.");
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogWarning("[Logfile Obliterator] Obliteration failed. " + ex.Message);
                            SiAuto.Main.LogException(ex);
                            exceptionsDetected++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("[Logfile Obliterator] Logfile enumeration operation failed. " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LogInt("candidates", candidates);
            SiAuto.Main.LogInt("itemsWhacked", itemsWhacked);
            SiAuto.Main.LogInt("exceptionsDetected", exceptionsDetected);

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.Debugging.LogPruner.ObliterateOldLogs");
        }
    }
}
