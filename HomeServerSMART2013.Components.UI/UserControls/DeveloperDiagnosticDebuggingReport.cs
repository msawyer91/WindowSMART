using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using Chilkat;
using Gurock.SmartInspect;
using WSSControls.BelovedComponents;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public partial class DeveloperDiagnosticDebuggingReport : Form
    {
        private String loggingLocation;
        private int logFileCount;
        private long fileBytes;
        private bool uploadInProgress;
        private String zipFile;
        private String shortZipFile;
        private String textFile;
        private int percentage;
        private long zipFileSize;
        private bool uploadComplete;
        private bool isMexiSexi;
        private String debuggingTarget;
        private String newUser;
        private String newPDubya;

        private delegate void ProgressMonitor(String message, int percent);
        private delegate void UpdateDiags(String message);
        private delegate void Notificate();
        private delegate void Reboosterize();
        private ProgressMonitor progress;
        private UpdateDiags diagUpdater;
        private Notificate notifier;
        private Reboosterize hyperdiaperizerReboosterizer;

        private Thread uploadThread;

        public DeveloperDiagnosticDebuggingReport(String logfileDir, String results, bool mexiSexi)
        {
            InitializeComponent();

            loggingLocation = logfileDir;
            logFileCount = 0;
            fileBytes = 0;
            uploadInProgress = false;
            textBox1.Text = results;
            zipFile = String.Empty;
            shortZipFile = String.Empty;
            percentage = 0;
            zipFileSize = 0;
            uploadComplete = false;
            isMexiSexi = mexiSexi;

            progress = new ProgressMonitor(DUpdateProgress);
            diagUpdater = new UpdateDiags(DUpdateDiagnostics);
            notifier = new Notificate(DNotifyDone);
            hyperdiaperizerReboosterizer = new Reboosterize(DOfferNonSecureFtp);
            debuggingTarget = "yourendpoint.example.com";
            newUser = String.Empty;
            newPDubya = String.Empty;
        }

        private void DeveloperDiagnosticDebuggingReport_Load(object sender, EventArgs e)
        {
            try
            {
                String[] files = Directory.GetFiles(loggingLocation, "*.sil", SearchOption.TopDirectoryOnly);
                foreach (String file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    logFileCount++;
                    fileBytes += fi.Length;
                }
            }
            catch (Exception ex)
            {
                QMessageBox.Show("Unable to collect logfile data; some details will not be available. " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                label3.Text = loggingLocation;
                label5.Text = logFileCount.ToString();
                label7.Text = fileBytes.ToString() + " (uncompressed)";
            }

            textBox1.Focus();
            textBox1.DeselectAll();
            textBox1.Select(textBox1.Text.Length - 1, 0);
            textBox1.Focus();
            textBox2.Focus();

            if (logFileCount == 0)
            {
                QMessageBox.Show("Since there are no debug logs, a description must be provided before you can upload the bug report. Please describe the problem(s) you are " +
                    "experiencing, and if you'd like a reply, please include your email address.", "No Log Files Detected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (logFileCount == 0 && String.IsNullOrEmpty(textBox2.Text))
            {
                QMessageBox.Show("Since there are no debug logs, a description must be provided before you can upload the bug report. Please describe the problem(s) you are " +
                    "experiencing, and if you'd like a reply, please include your email address.", "No Log Files Detected", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            if (!string.IsNullOrEmpty(textBoxEmail.Text) && !Utilities.Utility.IsEmailAddressValid(textBoxEmail.Text))
            {
                QMessageBox.Show("Email address entered is invalid. Please enter a valid email address. If you do not want to provide an email address, please delete " +
                    "the value you entered for the email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            button1.Enabled = false;
            textBox2.ReadOnly = true;
            textBoxEmail.ReadOnly = true;
            uploadInProgress = true;

            uploadThread = new Thread(new ThreadStart(ZipAndSend));
            uploadThread.Name = "Zip and Send Thread";
            uploadThread.Start();

            if (fileBytes > 1073741824)
            {
                QMessageBox.Show("You have more than 1 GB of log files. It may take a few minutes to encrypt and compress them before the upload begins.",
                    "Large Log Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (uploadInProgress)
            {
                if (QMessageBox.Show("Do you want to cancel the upload in progress? If you had a large number of zip files to zip, especially if they were large, it may take " +
                    "several minutes, during which time the task may seem unresponsive.", "Cancel Upload", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                    == DialogResult.Yes)
                {
                    try
                    {
                        uploadThread.Abort();
                        int tryCount = 30;
                        while (tryCount > 0 && uploadInProgress)
                        {
                            UpdateProgress("Signaling worker process to abort...", percentage);
                            Thread.Sleep(1000);
                            tryCount--;
                        }
                    }
                    catch
                    {
                        Thread.Sleep(1000);
                    }

                    int abortCount = 0;
                    while (uploadInProgress)
                    {
                        if (abortCount >= 3)
                        {
                            if(QMessageBox.Show("A background operation is taking a long time to execute. If you had a large number of zip files to zip, especially if they were large, " +
                                "it may take several minutes for the task to complete.", "Abort Excessive Delay", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                            {
                                QMessageBox.Show("The background thread may still be executing. If you close the diagnostic window before it completes, the application may become unstable.",
                                    "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                break;
                            }
                        }
                        else
                        {
                            QMessageBox.Show("A background operation is taking a long time to execute. If you had a large number of zip files to zip, especially if they were large, " +
                                "it may take several minutes for the task to complete.", "Abort Delay", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            Thread.Sleep(5000);
                        }
                        abortCount++;
                    }

                    uploadInProgress = false;
                    button2.Text = "Close";
                }
            }
            else
            {
                this.Close();
            }
        }

        private void UpdateProgress(String message, int percent)
        {
            this.Invoke(progress, new object[] { message, percent });
        }

        private void DUpdateProgress(String message, int percent)
        {
            try
            {
                label8.Text = message;
                progressBar1.Value = percent;
                if (percent == 100)
                {
                    button2.Text = "Close";
                }
            }
            catch
            {
            }
        }

        private void UpdateDiagnostics(String message)
        {
            this.Invoke(diagUpdater, new object[] { message });
        }

        private void DUpdateDiagnostics(String message)
        {
            try
            {
                textBox1.Text += message;
            }
            catch
            {
            }
        }

        private void NotifyDone()
        {
            this.Invoke(notifier);
        }

        private void DNotifyDone()
        {
            try
            {
                if (uploadComplete)
                {
                    button1.Enabled = false;
                    button2.Text = "Close";
                }
                else
                {
                    button1.Enabled = true;
                    button2.Text = "Cancel";
                }
            }
            catch
            {
            }
        }

        private void OfferNonSecureFtp()
        {
            this.Invoke(hyperdiaperizerReboosterizer);
        }

        private void DOfferNonSecureFtp()
        {
            checkBox1.Visible = true;
        }

        private void ZipAndSend()
        {
            try
            {
                UpdateProgress("Encrypting and compressing your log files. This may take a few minutes.", 0);

                GenerateReportManifest();

                if (ZipFile())
                {
                    UpdateProgress("Preparing to upload", 0);
                    UploadZipFile();
                }
                else
                {
                    UpdateProgress("Zip failed. Details posted to diagnostic log above.", 0);
                }
            }
            catch (ThreadAbortException)
            {
                UpdateProgress("Cancelled at your request", percentage);
            }
            catch (Exception ex)
            {
                UpdateProgress("Unexpected failure. " + ex.Message, percentage);
                UpdateDiagnostics("Unexpected failure. " + ex.Message + "\r\n" + ex.ToString());
                QMessageBox.Show("Unable to generate zip package: " + ex.Message, "Zip Generation Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            finally
            {
                DeleteZipFile();
                NotifyDone();
                uploadInProgress = false;
            }
        }

        private void GenerateReportManifest()
        {
            System.Text.StringBuilder stream = new System.Text.StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.Encoding = Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(stream, settings);

            writer.WriteStartDocument();

            writer.WriteStartElement("bugReport", "");

            writer.WriteStartElement("windowSmart", "");
            writer.WriteAttributeString("version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            writer.WriteAttributeString("winVer", OSInfo.VersionString);
            writer.WriteAttributeString("winName", OSInfo.Name);
            writer.WriteAttributeString("winEdition", OSInfo.Edition);
            writer.WriteAttributeString("servicePack", OSInfo.ServicePack);
            writer.WriteAttributeString("platform", OSInfo.Bits.ToString());
            writer.WriteAttributeString("isMexiSexi", isMexiSexi.ToString());
            writer.WriteEndElement(); // windowSmart

            writer.WriteStartElement("reportStats", "");
            writer.WriteAttributeString("debuggingEnabled", logFileCount > 0 ? true.ToString() : false.ToString());
            writer.WriteAttributeString("silFileCount", logFileCount.ToString());
            writer.WriteAttributeString("installedDisks", "");
            writer.WriteAttributeString("debugBytes", fileBytes.ToString());
            writer.WriteEndElement(); // reportStats

            writer.WriteEndElement(); // bugReport

            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();

            StreamWriter manifestWriter = new StreamWriter(loggingLocation + "\\manifest.xml");
            manifestWriter.Write(stream.ToString());
            manifestWriter.Flush();
            manifestWriter.Close();
        }

        private bool ZipFile()
        {
            bool success = false;
            bool textFileExists = false;
            try
            {
                textFile = loggingLocation + "\\" + Environment.MachineName + "_Report.txt";
                StreamWriter writer = new StreamWriter(textFile);
                writer.Write(textBox1.Text);
                writer.WriteLine(String.Empty);
                writer.WriteLine("********** USER-PROVIDED DETAILS **********");
                writer.Write(textBox2.Text);
                writer.WriteLine("*******************************************");
                writer.WriteLine(String.Empty);
                writer.WriteLine("********** MISCELLANEOUS DETAILS **********");
                writer.WriteLine("Debug File Count = " + logFileCount.ToString());
                writer.WriteLine("Debug File Bytes = " + fileBytes.ToString());
                writer.WriteLine("*******************************************");
                writer.Flush();
                writer.Close();
                textFileExists = true;
            }
            catch (Exception ex)
            {
                textBox1.Text += "\r\nUnable to dump text report to disk: " + ex.Message;
            }

            try
            {
                Zip winzip = new Zip();
                bool activate = winzip.UnlockComponent("DOJONO.CB1012021_bKaXHW7WockC");
                if (!activate)
                {
                    throw new LicenseException(typeof(Zip), winzip, "Unable to activate Chilkat zip module.");
                }
                String tempFile = Environment.MachineName + "_" + Guid.NewGuid().ToString("B") + ".zip";
                zipFile = loggingLocation + "\\" + tempFile;
                winzip.NewZip(zipFile);
                winzip.Encryption = 4;
                winzip.EncryptKeyLength = 256;
                // Password is "hidden" in two separate areas -- this was done when product was commercial to prevent
                // reverse engineering to crack the password. You can probably simplify it!
                winzip.SetPassword(Components.Utilities.Utility.CHILKAT_DOTNET_4 + "2013" + Properties.Resources.DiagnosticConfigUpload);
                winzip.AppendFiles(loggingLocation.Replace("\\", "/") + "/*.sil", false);
                if (textFileExists)
                {
                    winzip.AppendOneFileOrDir(textFile, false);
                }

                if (File.Exists(loggingLocation + "\\" + "manifest.xml"))
                {
                    winzip.AppendOneFileOrDir(loggingLocation + "\\" + "manifest.xml", false);
                }

                success = winzip.WriteZipAndClose();

                if (!success)
                {
                    UpdateDiagnostics("\r\n" + winzip.LastErrorText);
                }

                FileInfo fi = new FileInfo(zipFile);
                if (fi != null)
                {
                    shortZipFile = fi.Name;
                    zipFileSize = fi.Length;
                }
                else
                {
                    shortZipFile = Guid.NewGuid().ToString("B") + ".zip";
                    zipFileSize = 0;
                }

                try
                {
                    File.Delete(textFile);
                    File.Delete(loggingLocation + "\\" + "manifest.xml");
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                UpdateDiagnostics("\r\n" + ex.Message);
            }
            return success;
        }

        private bool ReapAlternateUploadLocation()
        {
            try
            {
                Microsoft.Win32.RegistryKey machineKey = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey dojoNorthSubKey = machineKey.OpenSubKey(Properties.Resources.RegistryDojoNorthRootKey, false);
                Microsoft.Win32.RegistryKey configurationKey = dojoNorthSubKey.OpenSubKey(Properties.Resources.RegistryConfigurationKey, false);
                String newTarget = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigDebugTarget);
                newUser = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigDebugUser);
                newPDubya = (String)configurationKey.GetValue(Properties.Resources.RegistryConfigDebugPass);

                if (String.IsNullOrEmpty(newTarget) || String.IsNullOrEmpty(newUser) || String.IsNullOrEmpty(newPDubya))
                {
                    return false;
                }
                debuggingTarget = newTarget;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void UploadZipFile()
        {
            Ftp2 uploader = new Ftp2();
            try
            {
                uploader = new Ftp2();
                bool activate = uploader.UnlockComponent("DOJONO.CB1012021_bKaXHW7WockC");
                if (!activate)
                {
                    throw new LicenseException(typeof(Ftp2), uploader, "Unable to activate Chilkat FTP module.");
                }

                percentage = 0;

                // Try to connect up to three times.
                bool connected = false;
                int attempt = 0;

                while (!connected && attempt < 3)
                {
                    attempt++;
                    UpdateProgress("Connecting to Server... (attempt " + attempt.ToString() + " of 3)", percentage);
                    Thread.Sleep(1500);
                    bool useAlternateTarget = ReapAlternateUploadLocation();
                    uploader.Hostname = debuggingTarget;
                    if (useAlternateTarget)
                    {
                        uploader.Username = newUser;
                        uploader.Password = newPDubya;
                    }
                    else
                    {
                        uploader.Username = "windowsmart2013";
                        uploader.Password = "Flawless04Head28McHyper2010Perfect";
                    }

                    if (checkBox1.Checked)
                    {
                        uploader.AuthTls = false;
                    }
                    else
                    {
                        uploader.AuthTls = true;
                    }

                    connected = uploader.Connect();
                    if (connected)
                    {
                        //UpdateDiagnostics("\r\n\r\n*** FTP connect over Secure channel successful: " + uploader.LastErrorText);
                        try
                        {
                            String client = Properties.Resources.ApplicationTitleWindowSmart;
                            String version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                            uploader.SendCommand("CSID Name=" + client + "; Version=" + version + ";");
                        }
                        catch
                        {
                        }
                        break;
                    }
                    UpdateProgress("Connection failed - will retry in a few seconds", percentage);
                    Thread.Sleep(5000);
                }

                if (!connected)
                {
                    UpdateProgress("FTP connection failed; giving up.", percentage);
                    UpdateDiagnostics("FTP connection failed.\r\n" + uploader.LastErrorText);
                    OfferNonSecureFtp();
                    return;
                }

                uploader.ChangeRemoteDir("ws2013diagnostics");
                

                bool fileStart = false;
                attempt = 0;
                while (!fileStart && attempt < 3)
                {
                    attempt++;
                    UpdateProgress("Starting upload... (attempt " + attempt.ToString() + " of 3)", percentage);
                    Thread.Sleep(1500);
                    fileStart = uploader.AsyncPutFileStart(zipFile, shortZipFile);
                    if (fileStart)
                    {
                        break;
                    }
                    UpdateProgress("Upload failed - will retry in a few seconds", percentage);
                    Thread.Sleep(5000);
                }

                if (!connected)
                {
                    UpdateProgress("FTP upload failed; giving up.", percentage);
                    UpdateDiagnostics("FTP upload failed.\r\n" + uploader.LastErrorText);
                    OfferNonSecureFtp();
                    return;
                }

                bool allBytesSent = false;
                while (!uploader.AsyncFinished)
                {
                    long bytesSent = uploader.AsyncBytesSent64;
                    decimal transferRate = Decimal.Round((decimal)uploader.UploadTransferRate / 1024.00M, 2);
                    SiAuto.Main.LogDouble("calcPercent", ((double)bytesSent / (double)zipFileSize) * 100.0);
                    int calcPercent = (int)(((double)bytesSent / (double)zipFileSize) * 100.0);
                    if (zipFileSize > 0)
                    {
                        percentage = calcPercent;
                        UpdateProgress("Uploading " + bytesSent.ToString() + "/" + zipFileSize.ToString() + " bytes (" + transferRate.ToString() + " KB/sec)", percentage);
                    }
                    else
                    {
                        UpdateProgress("Uploading " + bytesSent.ToString() + " bytes (" + transferRate.ToString() + " bytes/sec)", percentage);
                    }
                    uploader.SleepMs(250);
                    allBytesSent = (bytesSent == zipFileSize);
                }

                if (uploader.AsyncSuccess || allBytesSent)
                {
                    uploadComplete = true;
                    UpdateProgress("Upload successful. Thank you for your report!", 100);
                }
                else
                {
                    UpdateProgress("Upload failed. Please try again later.", percentage);
                    UpdateDiagnostics("FTP upload failed.\r\n" + uploader.LastErrorText);
                    OfferNonSecureFtp();
                }
            }
            catch (Exception ex)
            {
                UpdateProgress("Upload failed. " + ex.Message, percentage);
                UpdateDiagnostics(ex.Message + "\r\n" + uploader.LastErrorText);
                OfferNonSecureFtp();
            }
            finally
            {
                uploader.Disconnect();
            }
        }

        private void DeveloperDiagnosticDebuggingReport_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (uploadInProgress)
            {
                e.Cancel = true;
            }

            if (e.CloseReason == CloseReason.TaskManagerClosing || e.CloseReason == CloseReason.ApplicationExitCall || e.CloseReason == CloseReason.WindowsShutDown)
            {
                e.Cancel = false;
            }
        }

        private void DeleteZipFile()
        {
            try
            {
                File.Delete(zipFile);
            }
            catch
            {
            }

            try
            {
                File.Delete(loggingLocation + "\\manifest.xml");
            }
            catch
            {
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                QMessageBox.Show("You can use regular FTP if you are experiencing difficulties uploading your report with secure FTP (also known as FTP Explicit TLS). By checking " +
                    "this box, you can attempt to upload your report using regular FTP. Regular FTP does not use any encryption to transport the file. Please bear in mind the " +
                    "file is still encrypted, and thus its contents remain protected. However, if you prefer the absolute highest level of security, you should not use regular FTP.",
                    "FTP Caution", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
