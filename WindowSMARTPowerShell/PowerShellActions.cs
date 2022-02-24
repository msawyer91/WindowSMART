using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components;

namespace DojoNorthSoftware.WindowSMART
{
    public sealed class PowerShellActions
    {
        public static string SW_WINDOWS_SKU = "XtlKtnVnWtT/rFfft8WjHmhSXdFrqGT9KFkaBIJqn5Nwdq0dXakM2+NVYQDhPcDGj9qovDUnJujc4fBIrlKIpPl/LADT8Wkl+ZpW2YBMmuHjnJQtuueAPfySA3unmcnl";
        public static void StartService()
        {
            ServiceController controller;
            bool invalidState = false;
            String state = String.Empty;

            // Make sure the service is in a startable state.
            try
            {
                controller = new ServiceController(Properties.Resources.ServiceNameHss);
                if (controller.Status != ServiceControllerStatus.Stopped)
                {
                    state = controller.Status.ToString();
                    controller.Close();
                    controller.Dispose();
                    invalidState = true;
                }
            }
            catch (Exception ex)
            {
                throw new WindowSmartPSException("Service bind failed. This operation requires elevation. Please check perms and try again.", ex);
            }

            if (invalidState)
            {
                throw new WindowSmartPSException(("Service must be in a Stopped state to start it. Current state: " + state));
            }

            // Stop the service.
            try
            {
                Thread.Sleep(500);
                controller.Start();
            }
            catch (Exception ex)
            {
                controller.Close();
                controller.Dispose();
                throw new WindowSmartPSException("Service stop command failed.", ex);
            }

            // Give the service 15 seconds to start.
            int timer = 0;
            controller.Refresh();
            while (timer < 15 && controller.Status != ServiceControllerStatus.Running)
            {
                System.Threading.Thread.Sleep(1000);
                controller.Refresh();
                timer++;
            }

            if (controller.Status == ServiceControllerStatus.Running)
            {
                controller.Close();
                controller.Dispose();
            }
            else
            {
                controller.Close();
                controller.Dispose();
                throw new WindowSmartPSException("Service did not start in a reasonable amount of time. Wait a few seconds and try again. You may also " +
                    "want to try Get-WindowSmartServiceStatus.");
            }
        }

        public static void StopService()
        {
            ServiceController controller;
            bool invalidState = false;
            String state = String.Empty;

            // Make sure the service is in a stoppable state.
            try
            {
                controller = new ServiceController(Properties.Resources.ServiceNameHss);
                if (controller.Status != ServiceControllerStatus.Running)
                {
                    state = controller.Status.ToString();
                    controller.Close();
                    controller.Dispose();
                    invalidState = true;
                }
            }
            catch (Exception ex)
            {
                throw new WindowSmartPSException("Service bind failed. This operation requires elevation. Please check perms and try again.", ex);
            }

            if (invalidState)
            {
                throw new WindowSmartPSException(("Service must be in a Running (started) state to stop it. Current state: " + state));
            }

            // Stop the service.
            try
            {
                Thread.Sleep(500);
                controller.Stop();
            }
            catch (Exception ex)
            {
                controller.Close();
                controller.Dispose();
                throw new WindowSmartPSException("Service start command failed.", ex);
            }

            // Give the service 15 seconds to wrap things up.
            int timer = 0;
            controller.Refresh();
            while (timer < 15 && controller.Status != ServiceControllerStatus.Stopped)
            {
                System.Threading.Thread.Sleep(1000);
                controller.Refresh();
                timer++;
            }

            if (controller.Status == ServiceControllerStatus.Stopped)
            {
                controller.Close();
                controller.Dispose();
            }
            else
            {
                throw new WindowSmartPSException("Service did not stop in a reasonable amount of time. Wait a few seconds and try again. You may also " +
                    "want to try Get-WindowSmartServiceStatus.");
            }
        }

        public static int ApplyLicense(String slabLicenseFile, bool isFileName, bool restartService)
        {
            if (isFileName)
            {
                if (!System.IO.File.Exists(slabLicenseFile))
                {
                    throw new System.IO.FileNotFoundException("The system cannot find the file specified: " + slabLicenseFile);
                }

                try
                {
                    int retVal = DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Mexi_Sexi.Enterprise.DoEnterpriseTransaction(slabLicenseFile);
                    if (retVal == 0x0)
                    {
                        if ((restartService))
                        {
                            if (DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.ServiceHelper.ServiceStaticMethods.RebootService(false) == 0x0)
                            {
                                return 0x0;
                            }
                            else
                            {
                                return 0x1;
                            }
                        }
                        else
                        {
                            return 0x0;
                        }
                    }
                    else
                    {
                        return retVal;
                    }
                }
                catch (Exception ex)
                {
                    throw new WindowSmartPSException("Unexpected error applying the license (exit code 0x9). " + ex.Message, ex);
                }
            }
            else
            {
                String tempXml = String.Empty;
                try
                {
                    String path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    tempXml = path + "\\license.slab";
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(tempXml);
                    writer.Write(slabLicenseFile);
                    writer.Flush();
                    writer.Close();
                }
                catch
                {
                    throw;
                }

                try
                {
                    int retVal = DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Mexi_Sexi.Enterprise.DoEnterpriseTransaction(tempXml);
                    if (retVal == 0x0)
                    {
                        if ((restartService))
                        {
                            if (DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.ServiceHelper.ServiceStaticMethods.RebootService(false) == 0x0)
                            {
                                return 0x0;
                            }
                            else
                            {
                                return 0x1;
                            }
                        }
                        else
                        {
                            return 0x0;
                        }
                    }
                    else
                    {
                        return retVal;
                    }
                }
                catch (Exception ex)
                {
                    throw new WindowSmartPSException("Unexpected error applying the license (exit code 0x9). " + ex.Message, ex);
                }
                finally
                {
                    try
                    {
                        System.IO.File.Delete(tempXml);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static String GetLicense(Guid refGuid)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            String concatenatedString = String.Empty;
            // License Test
            object slobberhead;
            uint theSlab = DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.LegacyOs.IsLegacyOs(out slobberhead, false);
            // theSlab contains return code; slobberhead = object with date/time installed (or 1/1/1980 if bad things happened)
            if (theSlab == 0x0)
            {
                // New user
                concatenatedString = DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.LegacyOs.Concatenate(SW_WINDOWS_SKU, 
                    Properties.Resources.RegistryConfigPowerShell, true, false, out refGuid);
            }
            else if (theSlab == 0x1)
            {
                // Existing user and no errors returned with info
                concatenatedString = DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.LegacyOs.Concatenate(SW_WINDOWS_SKU,
                    Properties.Resources.RegistryConfigPowerShell, false, false, out refGuid);
            }
            else
            {
                // Date was invalid but we still grab the license because it could be valid (if it is we don't care about the date).
                concatenatedString = "0xF," + DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.LegacyOs.Concatenate(SW_WINDOWS_SKU,
                    Properties.Resources.RegistryConfigPowerShell, false, false, out refGuid);
            }
            String xmlText = concatenatedString;
            DateTime result = (DateTime)slobberhead;

            DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Mexi_Sexi.MexiSexi mexiSexi;
            try
            {
                mexiSexi = new DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Mexi_Sexi.MexiSexi(xmlText, result, refGuid, theSlab);
            }
            catch
            {
                mexiSexi = null;
            }

            if (mexiSexi == null)
            {
                return String.Empty;
            }

            sb.AppendLine(String.Empty);
            if (mexiSexi.IsMexiSexi)
            {
                sb.AppendLine("     Licensed User       : " + mexiSexi.UserInfo.UserName);
                sb.AppendLine("     Licensed Company    : " + mexiSexi.UserInfo.Company);
                sb.AppendLine("     Email Address       : " + mexiSexi.UserInfo.EmailAddress);
                sb.AppendLine("     License Class       : " + DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Utilities.Utility.GetProductRestrictions(
                    mexiSexi.UserInfo.ProgramType.Substring(4)));
            }
            else if (mexiSexi.IsError)
            {
                throw new WindowSmartPSException("The license data is corrupt or invalid. Please use Set-WindowSmartLicense to apply a valid license. (Error code 0x" +
                    mexiSexi.ReferenceCode.ToString("X") + ")");
            }
            else
            {
                sb.AppendLine("     Licensed User       : Unregistered - 30-day Trial");
                sb.AppendLine("     Email Address       : 30DayTrial@dojonorthsoftware.net");
                sb.AppendLine("     Trial Expires       : " + mexiSexi.Checker.ToString());
            }

            return sb.ToString();
        }

        public static void VoidLicense()
        {
            try
            {
                if (DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.OperatingSystem.IsWindowsServerSolutionsProduct(String.Empty))
                {
                    throw new WindowSmartPSException("The requested operation is not supported on this version of Windows.");
                }
                Guid dotNetFourCurrent = new Guid("{00000000-0000-0000-0000-000000000000}");
                Guid dotNetFourUpdateAvailable = new Guid("{00000000-0000-0000-0000-000000000000}");
                String result = WindowsServerSolutions.HomeServerSMART2013.Components.LegacyOs.Concatenate(SW_WINDOWS_SKU, Properties.Resources.RegistryConfigHelperServiceKey, true, true, out dotNetFourCurrent);
                if (dotNetFourCurrent != dotNetFourUpdateAvailable)
                {
                    return;
                }
                throw new WindowSmartPSException("Removal of the license was unsuccessful. No error was reported; however, the license read-back shows the license is still installed.");
            }
            catch (Exception ex)
            {
                throw new WindowSmartPSException("An error occurred while trying to remove the license: " + ex.Message, ex);
            }
        }
    }
}
