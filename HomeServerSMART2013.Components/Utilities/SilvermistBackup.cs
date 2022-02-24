using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Utilities
{
    public sealed class SilvermistBackup
    {
        private DateTime backupStart;
        private DateTime backupEnd;

        private long folderCount;
        private long fileCount;
        private long byteCount;
        private long errorCount;
        private long skippedFolders;
        private long skippedFiles;

        private String backupRoot;
        private String destination;
        private List<String> excludedItems;

        public SilvermistBackup(String root, String target, List<String> excludes)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SilvermistBackup (constructor)");
            folderCount = 0;
            fileCount = 0;
            byteCount = 0;
            errorCount = 0;
            skippedFolders = 0;
            skippedFiles = 0;

            backupRoot = root;
            destination = target;
            excludedItems = excludes;
            SiAuto.Main.LogString("Backup configured on root", root);
            SiAuto.Main.LogString("destination", destination);
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SilvermistBackup (constructor)");
        }

        public void Start()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SilvermistBackup.Start");
            backupStart = DateTime.Now;

            if (Directory.Exists(backupRoot))
            {
                if (IsFolderExcluded(backupRoot))
                {
                    skippedFolders++;
                    SiAuto.Main.LogWarning("Backup skipping ROOT folder " + backupRoot + " because it matches an excluded item.");
                }
                else
                {
                    folderCount++;
                    BackupRecursive(backupRoot);
                }
            }

            backupEnd = DateTime.Now;
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SilvermistBackup.Start");
        }

        private void BackupRecursive(String folder)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SilvermistBackup.BackupRecursive");
            //System.Security.AccessControl.DirectorySecurity security = new System.Security.AccessControl.DirectorySecurity();
            //security.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule("SYSTEM", System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.InheritanceFlags.ContainerInherit &
            //    System.Security.AccessControl.InheritanceFlags.ObjectInherit, System.Security.AccessControl.PropagationFlags.InheritOnly, System.Security.AccessControl.AccessControlType.Allow));
            //security.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule("EVERYONE", System.Security.AccessControl.FileSystemRights.ReadAndExecute, System.Security.AccessControl.InheritanceFlags.ContainerInherit &
            //    System.Security.AccessControl.InheritanceFlags.ObjectInherit, System.Security.AccessControl.PropagationFlags.InheritOnly, System.Security.AccessControl.AccessControlType.Allow));
            try
            {
                foreach (String file in Directory.GetFiles(folder))
                {
                    try
                    {
                        String targetDir = destination;
                        if (IsFileExcluded(file))
                        {
                            skippedFiles++;
                            SiAuto.Main.LogWarning("Backup skipping file " + file + " because it matches an excluded item.");
                        }
                        else
                        {
                            FileInfo fi = new FileInfo(file);

                            String filename = fi.Name;
                            targetDir += "\\" + GetDirectoryTree(fi.DirectoryName);
                            if (!Directory.Exists(targetDir))
                            {
                                SiAuto.Main.LogMessage("Creating folder " + targetDir);
                                //Directory.CreateDirectory(targetDir, security);
                                Directory.CreateDirectory(targetDir);
                            }

                            File.Copy(file, targetDir + "\\" + filename, true);
                            fileCount++;
                            byteCount += fi.Length;
                        }
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                        SiAuto.Main.LogFatal("Thread abort was signaled; halting backup. Is the service shutting down?");
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SilvermistBackup.BackupRecursive");
                        return;
                    }
                    catch (Exception copyEx)
                    {
                        errorCount++;
                        SiAuto.Main.LogError("Unable to copy file " + file + ". " + copyEx.Message);
                        SiAuto.Main.LogException(copyEx);
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                SiAuto.Main.LogFatal("Thread abort was signaled; halting backup. Is the service shutting down?");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SilvermistBackup.BackupRecursive");
                return;
            }
            catch (Exception ex)
            {
                errorCount++;
                SiAuto.Main.LogError("Unable to process folder " + folder + ". The remaining files in this folder will not be copied. " + ex.Message);
                SiAuto.Main.LogException(ex);
            }

            try
            {
                foreach (String subfolder in Directory.GetDirectories(folder))
                {
                    if (Directory.Exists(subfolder))
                    {
                        if (IsFolderExcluded(subfolder))
                        {
                            skippedFolders++;
                            SiAuto.Main.LogWarning("Backup skipping folder " + backupRoot + " because it matches an excluded item.");
                        }
                        else
                        {
                            folderCount++;
                            BackupRecursive(subfolder);
                        }
                    }
                    else
                    {
                        errorCount++;
                        SiAuto.Main.LogWarning("Nested folder " + subfolder + " cannot be accessed or does not exist.");
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                SiAuto.Main.LogFatal("Thread abort was signaled; halting backup. Is the service shutting down?");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SilvermistBackup.BackupRecursive");
                return;
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Unable to process folder " + folder + ". The remaining files in this folder will not be copied. " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SilvermistBackup.BackupRecursive");
        }

        /// <summary>
        /// Determine if a folder is excluded. If an item contains the "\" backslash then that is a directory exclude item.
        /// </summary>
        /// <param name="folder">The folder to check.</param>
        /// <returns>true if the folder is excluded; false otherwise.</returns>
        private bool IsFolderExcluded(String folder)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SilvermistBackup.IsFolderExcluded");
            foreach (String item in excludedItems)
            {
                if (item.Contains('\\'))
                {
                    if (folder.ToUpper().Contains(item.ToUpper()))
                    {
                        SiAuto.Main.LogMessage("Exclusion match on " + item);
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SilvermistBackup.IsFolderExcluded");
                        return true;
                    }
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SilvermistBackup.IsFolderExcluded");
            return false;
        }

        /// <summary>
        /// Determine if a file is excluded.
        /// </summary>
        /// <param name="folder">The folder to check.</param>
        /// <returns>true if the folder is excluded; false otherwise.</returns>
        private bool IsFileExcluded(String file)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.SilvermistBackup.IsFileExcluded");
            foreach (String item in excludedItems)
            {
                if (!item.Contains('\\'))
                {
                    if (file.ToUpper().Contains(item.ToUpper()))
                    {
                        SiAuto.Main.LogMessage("Exclusion match on " + item);
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SilvermistBackup.IsFileExcluded");
                        return true;
                    }
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.SilvermistBackup.IsFileExcluded");
            return false;
        }

        private String GetDirectoryTree(String folder)
        {
            String letter = folder.Substring(0, 1);
            String path = folder.Substring(2);
            return letter + "\\" + path;
        }

        public long ErrorCount
        {
            get
            {
                return errorCount;
            }
        }

        public long FolderCount
        {
            get
            {
                return folderCount;
            }
        }

        public long FileCount
        {
            get
            {
                return fileCount;
            }
        }

        public long ByteCount
        {
            get
            {
                return byteCount;
            }
        }

        public long SkippedFolders
        {
            get
            {
                return skippedFolders;
            }
        }

        public long SkippedFiles
        {
            get
            {
                return skippedFiles;
            }
        }
    }
}
