using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.ServiceHelper
{
    public sealed class DailySummary
    {
        private DataTable eventsOfTheDay;
        private int eventCount;
        private int powerEventCount;
        private bool needSummarySend;

        public DailySummary()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.ServiceHelper.DailySummary");
            ComposeDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.ServiceHelper.DailySummary");
        }

        /// <summary>
        /// Constructs the data table.
        /// </summary>
        public void ComposeDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.ServiceHelper.DailySummary.ComposeDataTable");
            eventsOfTheDay = new DataTable("DailyEvents");
            eventCount = 0;
            powerEventCount = 0;
            needSummarySend = false;

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            eventsOfTheDay.Columns.Add("Key", typeof(Guid));
            eventsOfTheDay.Columns.Add("WhenDetected", typeof(DateTime));
            eventsOfTheDay.Columns.Add("DiskModel", typeof(String));
            eventsOfTheDay.Columns.Add("DiskPath", typeof(String));
            eventsOfTheDay.Columns.Add("AttributeID", typeof(int));
            eventsOfTheDay.Columns.Add("AttributeName", typeof(String));
            eventsOfTheDay.Columns.Add("HealthTitle", typeof(String));
            eventsOfTheDay.Columns.Add("HealthMessage", typeof(String));
            eventsOfTheDay.Columns.Add("IsCritical", typeof(bool));
            eventsOfTheDay.Columns.Add("Action", typeof(String));
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.ServiceHelper.DailySummary.ComposeDataTable");
        }

        public void Insert(String model, String path, int attributeID, String attributeName, String healthTitle, String healthMessage, bool isCritical, AlertAction action)
        {
            Insert(DateTime.MinValue, model, path, attributeID, attributeName, healthTitle, healthMessage, isCritical, action);
        }

        public void Insert(DateTime whenDetected, String model, String path, int attributeID, String attributeName, String healthTitle, String healthMessage, bool isCritical, AlertAction action)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.ServiceHelper.DailySummary.Insert");
            if (action == AlertAction.Hold)
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.ServiceHelper.DailySummary.Insert");
                return;
            }

            SiAuto.Main.LogDateTime("whenDetected", whenDetected);
            SiAuto.Main.LogString("model", model);
            SiAuto.Main.LogString("path", path);
            SiAuto.Main.LogInt("attributeID", attributeID);
            SiAuto.Main.LogString("attributeName", attributeName);
            SiAuto.Main.LogString("healthTitle", healthTitle);
            SiAuto.Main.LogString("healthMessage", healthMessage);
            SiAuto.Main.LogBool("isCritical", isCritical);
            SiAuto.Main.LogString("AlertAction", action.ToString());

            try
            {
                DateTime now = DateTime.Now;
                DataRow row = eventsOfTheDay.NewRow();

                row["Key"] = Guid.NewGuid();
                if (whenDetected == DateTime.MinValue)
                {
                    row["WhenDetected"] = now;
                }
                else
                {
                    row["WhenDetected"] = whenDetected;
                }
                row["DiskModel"] = model;
                row["DiskPath"] = path;
                row["AttributeID"] = attributeID;
                row["AttributeName"] = attributeName;
                row["HealthTitle"] = healthTitle;
                row["HealthMessage"] = healthMessage;
                row["IsCritical"] = isCritical;
                if (action == AlertAction.Insert)
                {
                    row["Action"] = "Insert";
                }
                else if (action == AlertAction.Remove)
                {
                    row["Action"] = "Remove";
                }
                else
                {
                    // Update
                    row["Action"] = "Update";
                }

                SiAuto.Main.LogMessage("Item inserted; committing changes.");
                eventsOfTheDay.Rows.Add(row);
                eventsOfTheDay.AcceptChanges();
                eventCount++;
                SiAuto.Main.LogMessage("Item inserted; changes committed.");
                SiAuto.Main.LogMessage("There are now " + eventCount.ToString() + " events in the summary list.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogWarning("Exceptions were detected inserting a new summary alert entry: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.ServiceHelper.DailySummary.Insert");
        }

        public String GenerateMessageBody(String filename, bool isWindowsServerSolutions)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.DailySummary.GenerateMessageBody");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            String fileCount = String.Empty;
            String reportDate = String.Empty;

            try
            {
                SiAuto.Main.LogMessage("Instantiate new XmlDocument object.");
                XmlDocument doc = new XmlDocument();
                SiAuto.Main.LogMessage("Open XML alerts file " + filename);
                System.IO.StreamReader reader = new System.IO.StreamReader(filename);
                String xmlContent = reader.ReadToEnd();
                SiAuto.Main.LogMessage("Contents of " + xmlContent + " have been read; closing file.");
                reader.Close();
                SiAuto.Main.LogMessage("StreamReader closed. Begin XML parse.");
                doc.LoadXml(xmlContent);
                SiAuto.Main.LogMessage("XmlDocument is populated. XML is well-formed.");

                SiAuto.Main.LogMessage("Starting FOREACH loop to iterate the XML root level.");

                System.Text.StringBuilder miniSB = new System.Text.StringBuilder();

                foreach (XmlNode rootNode in doc.ChildNodes)
                {
                    if (rootNode.Name == "alerts")
                    {
                        SiAuto.Main.LogMessage("Starting FOREACH loop to iterate the \"alerts\" level.");
                        foreach (XmlNode alertsNode in rootNode.ChildNodes)
                        {
                            if (alertsNode.Name == "count")
                            {
                                fileCount = alertsNode.InnerText;
                            }
                            else if (alertsNode.Name == "date")
                            {
                                reportDate = alertsNode.InnerText;
                            }
                            else if (alertsNode.Name == "alert")
                            {
                                SiAuto.Main.LogMessage("XML node name is \"alert\"");

                                String whenDetected = String.Empty;
                                String diskModel = String.Empty;
                                String diskPath = String.Empty;
                                String attributeID = String.Empty;
                                String attributeName = String.Empty;
                                String healthTitle = String.Empty;
                                String healthMessage = String.Empty;
                                String actionMode = String.Empty;
                                String action = String.Empty;

                                bool isCritical = false;

                                foreach (XmlAttribute attrib in alertsNode.Attributes)
                                {
                                    switch (attrib.Name)
                                    {
                                        case "diskModel":
                                            {
                                                diskModel = attrib.Value;
                                                SiAuto.Main.LogString("diskModel", diskModel);
                                                break;
                                            }
                                        case "diskPath":
                                            {
                                                diskPath = attrib.Value;
                                                SiAuto.Main.LogString("diskPath", diskPath);
                                                break;
                                            }
                                        case "attributeID":
                                            {
                                                attributeID = attrib.Value;
                                                break;
                                            }
                                        case "attributeName":
                                            {
                                                attributeName = attrib.Value;
                                                SiAuto.Main.LogString("attributeName", attributeName);
                                                break;
                                            }
                                        case "healthTitle":
                                            {
                                                healthTitle = attrib.Value;
                                                SiAuto.Main.LogString("healthTitle", healthTitle);
                                                break;
                                            }
                                        case "healthMessage":
                                            {
                                                healthMessage = attrib.Value;
                                                SiAuto.Main.LogString("healthMessage", healthMessage);
                                                break;
                                            }
                                        case "isCritical":
                                            {
                                                bool.TryParse(attrib.Value, out isCritical);
                                                SiAuto.Main.LogBool("isCritical", isCritical);
                                                break;
                                            }
                                        case "whenDetected":
                                            {
                                                whenDetected = attrib.Value;
                                                break;
                                            }
                                        case "actionMode":
                                            {
                                                action = attrib.Value;
                                                break;
                                            }
                                        default:
                                            {
                                                SiAuto.Main.LogWarning("Unrecognized attribute name " + attrib.Name + ", value " + attrib.Value);
                                                break;
                                            }
                                    }
                                }
                                // Build the inside table.
                                miniSB.AppendLine("<tr>");
                                miniSB.Append("<td class=\"tableContent\">");
                                miniSB.Append(whenDetected);
                                miniSB.Append("</td>");
                                miniSB.Append("<td class=\"tableContent\">");
                                miniSB.Append(diskModel);
                                miniSB.Append("</td>");
                                miniSB.Append("<td class=\"tableContent\">");
                                miniSB.Append(diskPath);
                                miniSB.Append("</td>");
                                miniSB.Append("<td class=\"tableContent\">");
                                miniSB.Append(attributeID);
                                miniSB.Append("</td>");
                                miniSB.Append("<td class=\"tableContent\">");
                                miniSB.Append(attributeName);
                                miniSB.Append("</td>");
                                miniSB.Append("<td class=\"tableContent\">");
                                miniSB.Append(healthTitle);
                                miniSB.Append("</td>");
                                miniSB.Append("<td class=\"tableContent\">");
                                miniSB.Append(healthMessage);
                                miniSB.Append("</td>");
                                miniSB.Append("<td class=\"tableContent\">");
                                miniSB.Append(isCritical.ToString());
                                miniSB.Append("</td>");
                                miniSB.Append("<td class=\"tableContent\">");
                                miniSB.Append(action);
                                miniSB.Append("</td>");
                                miniSB.AppendLine("</tr>");
                            }
                            else
                            {
                                SiAuto.Main.LogWarning("Unrecognized node name: " + alertsNode.Name);
                            }
                        }
                        SiAuto.Main.LogMessage("Ending FOREACH loop to iterate the \"alerts\" level.");
                    }
                }
                SiAuto.Main.LogMessage("Ending FOREACH loop to iterate the XML root level.");

                sb.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
                sb.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
                sb.AppendLine("<head>");
                sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
                sb.AppendLine("<title>Disk Health Summary</title>");
                sb.AppendLine("");
                sb.AppendLine("<!-- CSS Section -->");
                sb.AppendLine("<style type=\"text/css\">");
                sb.AppendLine("body");
                sb.AppendLine("{");
                sb.AppendLine("	background-color: #FFFFFF;");
                sb.AppendLine("	margin: 5px;");
                sb.AppendLine("	font-family: \"Arial\", Geneva, Sans-Serif;");
                sb.AppendLine("	color: #000000;");
                sb.AppendLine("}");
                sb.AppendLine("");
                sb.AppendLine("h1");
                sb.AppendLine("{");
                sb.AppendLine("	font-family: \"Arial\", Geneva, Sans-Serif;");
                sb.AppendLine("	font-size: 22px;");
                sb.AppendLine("	color: Blue;");
                sb.AppendLine("	text-align: center;");
                sb.AppendLine("}");
                sb.AppendLine("");
                sb.AppendLine("h2");
                sb.AppendLine("{");
                sb.AppendLine("	font-family: \"Arial\", Geneva, Sans-Serif;");
                sb.AppendLine("	font-size: 18px;");
                sb.AppendLine("	color: Black;");
                sb.AppendLine(" text-align: center;");
                sb.AppendLine("}");
                sb.AppendLine("");
                sb.AppendLine("h3");
                sb.AppendLine("{");
                sb.AppendLine("	font-family: \"Arial\", Geneva, sans-serif;");
                sb.AppendLine("	font-size: 16px;");
                sb.AppendLine("	color: Black;");
                sb.AppendLine("}");
                sb.AppendLine("");
                sb.AppendLine(".finePrint");
                sb.AppendLine("{");
                sb.AppendLine("	font-family: \"Arial\", Geneva, sans-serif;");
                sb.AppendLine("	font-size: 10px;");
                sb.AppendLine("	color: Black;");
                sb.AppendLine("}");
                sb.AppendLine("");
                sb.AppendLine(".finePrintSuper");
                sb.AppendLine("{");
                sb.AppendLine("	font-family: \"Arial\", Geneva, sans-serif;");
                sb.AppendLine("	font-size: 10px;");
                sb.AppendLine("	color: Black;");
                sb.AppendLine(" vertical-align: super;");
                sb.AppendLine("}");
                sb.AppendLine("");
                sb.AppendLine(".tableCaption");
                sb.AppendLine("{");
                sb.AppendLine("	font-family: \"Arial\", Geneva, Sans-Serif;");
                sb.AppendLine("	font-size: 12px;");
                sb.AppendLine("	font-weight: bold;");
                sb.AppendLine("	color: Black;");
                sb.AppendLine("}");
                sb.AppendLine("");
                sb.AppendLine(".tableContent");
                sb.AppendLine("{");
                sb.AppendLine("	font-family: \"Arial\", Geneva, Sans-Serif;");
                sb.AppendLine("	font-size: 12px;");
                sb.AppendLine("	color: Black;");
                sb.AppendLine("}");
                sb.AppendLine("");
                sb.AppendLine("</style>");
                sb.AppendLine("");
                sb.AppendLine("</head>");

                sb.AppendLine("<body>");
                sb.AppendLine("<h2>" + (isWindowsServerSolutions ? "Home Server SMART " : "WindowSMART ") + "24/7</h2>");
                sb.AppendLine("<h1>Daily Disk Health Summary Report</h1>");
                sb.AppendLine("<h3>Report for Computer " + System.Environment.MachineName + " - " + fileCount + " Events Detected</h3>");
                sb.AppendLine("<h3>Events for " + reportDate + "</h3>");

                if (fileCount == "0")
                {
                    sb.AppendLine("<br/>");
                    sb.AppendLine("No disk health alerts or service start/stop events were detected.");
                }
                else
                {
                    sb.AppendLine("<table width=\"100%\" border=\"1\" cellpadding=\"2\" summary=\"Displays a summary for the day's events.\">");
                    sb.AppendLine("  <tr>");
                    sb.AppendLine("    <td class=\"tableCaption\">Time</td>");
                    sb.AppendLine("    <td class=\"tableCaption\">Disk</td>");
                    sb.AppendLine("    <td class=\"tableCaption\">Path</td>");
                    sb.AppendLine("    <td class=\"tableCaption\">ID</td>");
                    sb.AppendLine("    <td class=\"tableCaption\">Attribute</td>");
                    sb.AppendLine("    <td class=\"tableCaption\">Title</td>");
                    sb.AppendLine("    <td class=\"tableCaption\">Message</td>");
                    sb.AppendLine("    <td class=\"tableCaption\">Critical?</td>");
                    sb.AppendLine("    <td class=\"tableCaption\">Alert Type</td>");
                    sb.AppendLine("  </tr>");

                    sb.AppendLine(miniSB.ToString());

                    sb.AppendLine("</table>");
                }
                sb.AppendLine("</body>");
                sb.AppendLine("</html>");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Exceptions were detected reading the XML alerts or parsing XML: " + ex.Message);
                SiAuto.Main.LogException(ex);
                sb.Clear();
            }

            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.DailySummary.GenerateMessageBody");
            return sb.ToString();
        }

        public void ReadCurrentFile(String filename)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.DailySummary.ReadCurrentFile");
            int fileCount = 0;

            SiAuto.Main.LogMessage("There are currently " + eventCount.ToString() + " events in the summary list.");
            
            try
            {
                SiAuto.Main.LogMessage("Instantiate new XmlDocument object.");
                XmlDocument doc = new XmlDocument();
                SiAuto.Main.LogMessage("Open XML alerts file " + filename);
                System.IO.StreamReader reader = new System.IO.StreamReader(filename);
                String xmlContent = reader.ReadToEnd();
                SiAuto.Main.LogMessage("Contents of " + xmlContent + " have been read; closing file.");
                reader.Close();
                SiAuto.Main.LogMessage("StreamReader closed. Begin XML parse.");
                doc.LoadXml(xmlContent);
                SiAuto.Main.LogMessage("XmlDocument is populated. XML is well-formed.");

                SiAuto.Main.LogMessage("Starting FOREACH loop to iterate the XML root level.");
                foreach (XmlNode rootNode in doc.ChildNodes)
                {
                    if (rootNode.Name == "alerts")
                    {
                        SiAuto.Main.LogMessage("Starting FOREACH loop to iterate the \"alerts\" level.");
                        foreach (XmlNode alertsNode in rootNode.ChildNodes)
                        {
                            if (alertsNode.Name == "count")
                            {
                                SiAuto.Main.LogMessage("XML node name is \"count\"");
                                fileCount = 0;
                                bool result = Int32.TryParse(alertsNode.InnerText, out fileCount);
                                if (result)
                                {
                                    SiAuto.Main.LogMessage("Detected " + fileCount.ToString() + " alerts in the summary data file.");
                                }
                                else
                                {
                                    SiAuto.Main.LogWarning("Failed to parse active alert count. Using default zero.");
                                }
                                SiAuto.Main.LogMessage("Adding " + fileCount.ToString() + " alerts to the " + eventCount.ToString() + " events currently in the summary list.");
                                SiAuto.Main.LogMessage("There will be " + (fileCount + eventCount).ToString() + " events in the summary list.");
                            }
                            else if (alertsNode.Name == "powerEvents")
                            {
                                SiAuto.Main.LogMessage("XML node name is \"powerEvents\"");
                                int inFile = 0;
                                bool result = Int32.TryParse(alertsNode.InnerText, out inFile);
                                if (result)
                                {
                                    SiAuto.Main.LogMessage("Detected " + inFile.ToString() + " power events in the summary data file.");
                                }
                                else
                                {
                                    SiAuto.Main.LogWarning("Failed to parse power event count. Using default zero.");
                                }

                                if (inFile > powerEventCount)
                                {
                                    powerEventCount = inFile;
                                }
                            }
                            else if (alertsNode.Name == "date")
                            {
                                DateTime reportDate;
                                DateTime.TryParse(alertsNode.InnerText, out reportDate);

                                SiAuto.Main.LogDateTime("reportDate", reportDate);
                                SiAuto.Main.LogDateTime("DateTime.Today", DateTime.Today);

                                if (reportDate == DateTime.MinValue ||
                                    reportDate < DateTime.Today)
                                {
                                    SiAuto.Main.LogWarning("Report date is older than today's date; setting flag for sending a report.");
                                    needSummarySend = true;
                                }
                                else
                                {
                                    SiAuto.Main.LogWarning("Report date and today's date are the same; clearing flag for sending a report.");
                                    needSummarySend = false;
                                }
                            }
                            else if (alertsNode.Name == "alert")
                            {
                                SiAuto.Main.LogMessage("XML node name is \"alert\"");

                                DateTime whenDetected = DateTime.MinValue;
                                String diskModel = String.Empty;
                                String diskPath = String.Empty;
                                int attributeID = 0;
                                String attributeName = String.Empty;
                                String healthTitle = String.Empty;
                                String healthMessage = String.Empty;
                                String actionMode = String.Empty;
                                AlertAction action = AlertAction.Hold;
                                                
                                bool isCritical = false;

                                foreach (XmlAttribute attrib in alertsNode.Attributes)
                                {
                                    switch (attrib.Name)
                                    {
                                        case "diskModel":
                                            {
                                                diskModel = attrib.Value;
                                                SiAuto.Main.LogString("diskModel", diskModel);
                                                break;
                                            }
                                        case "diskPath":
                                            {
                                                diskPath = attrib.Value;
                                                SiAuto.Main.LogString("diskPath", diskPath);
                                                break;
                                            }
                                        case "attributeID":
                                            {
                                                Int32.TryParse(attrib.Value, out attributeID);
                                                SiAuto.Main.LogInt("attributeID", attributeID);
                                                break;
                                            }
                                        case "attributeName":
                                            {
                                                attributeName = attrib.Value;
                                                SiAuto.Main.LogString("attributeName", attributeName);
                                                break;
                                            }
                                        case "healthTitle":
                                            {
                                                healthTitle = attrib.Value;
                                                SiAuto.Main.LogString("healthTitle", healthTitle);
                                                break;
                                            }
                                        case "healthMessage":
                                            {
                                                healthMessage = attrib.Value;
                                                SiAuto.Main.LogString("healthMessage", healthMessage);
                                                break;
                                            }
                                        case "isCritical":
                                            {
                                                bool.TryParse(attrib.Value, out isCritical);
                                                SiAuto.Main.LogBool("isCritical", isCritical);
                                                break;
                                            }
                                        case "whenDetected":
                                            {
                                                DateTime.TryParse(attrib.Value, out whenDetected);
                                                SiAuto.Main.LogDateTime("whenDetected", whenDetected);
                                                break;
                                            }
                                        case "actionMode":
                                            {
                                                actionMode = attrib.Value;

                                                if (actionMode == "Insert")
                                                {
                                                    action = AlertAction.Insert;
                                                }
                                                else if (actionMode == "Remove")
                                                {
                                                    action = AlertAction.Remove;
                                                }
                                                else if (actionMode == "Update")
                                                {
                                                    action = AlertAction.Update;
                                                }
                                                else
                                                {
                                                    action = AlertAction.Hold;
                                                }
                                                break;
                                            }
                                        default:
                                            {
                                                SiAuto.Main.LogWarning("Unrecognized attribute name " + attrib.Name + ", value " + attrib.Value);
                                                break;
                                            }
                                    }

                                    Insert(whenDetected, diskModel, diskPath, attributeID, attributeName, healthTitle, healthMessage, isCritical, action);
                                }
                            }
                            else
                            {
                                SiAuto.Main.LogWarning("Unrecognized node name: " + alertsNode.Name);
                            }
                        }
                        SiAuto.Main.LogMessage("Ending FOREACH loop to iterate the \"alerts\" level.");
                    }
                }
                SiAuto.Main.LogMessage("Ending FOREACH loop to iterate the XML root level.");
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Exceptions were detected reading the XML alerts or parsing XML: " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.DailySummary.ReadCurrentFile");
        }

        public void Reset()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.DailySummary.Reset");
            SiAuto.Main.LogMessage("Deleting all rows.");
            eventsOfTheDay.Rows.Clear();
            eventsOfTheDay.AcceptChanges();
            eventCount = 0;
            SiAuto.Main.LogMessage("Rows deleted; changes committed.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.DailySummary.Reset");
        }

        public void ComposeNewFile(String filename)
        {
            ComposeNewFile(filename, false);
        }

        public void ComposeNewFile(String filename, bool writeYesterday)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.HssServiceHelper.DailySummary.ComposeNewFile");
            try
            {
                SiAuto.Main.LogMessage("Initialize the I/O XML stream.");
                System.Text.StringBuilder stream = new System.Text.StringBuilder();
                SiAuto.Main.LogMessage("Initialize the XmlWriterSettings.");
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.Encoding = Encoding.UTF8;
                SiAuto.Main.LogMessage("Initialize the XmlWriter.");
                XmlWriter writer = XmlWriter.Create(stream, settings);

                SiAuto.Main.LogMessage("Begin writing XML document.");
                writer.WriteStartDocument(); // <?xml version="1.0" encoding="utf-8"?>

                writer.WriteStartElement("alerts", "");

                DateTime date = DateTime.Now;
                SiAuto.Main.LogDateTime("Today's date", date);
                SiAuto.Main.LogBool("writeYesterday", writeYesterday);
                if (writeYesterday)
                {
                    writer.WriteElementString("date", date.AddDays(-1).ToShortDateString());
                }
                else
                {
                    writer.WriteElementString("date", date.ToShortDateString());
                }

                int count = eventsOfTheDay.Rows.Count;
                SiAuto.Main.LogInt("Daily alert count", count);
                writer.WriteElementString("count", count.ToString());
                SiAuto.Main.LogInt("Power event count", powerEventCount);
                writer.WriteElementString("powerEvents", powerEventCount.ToString());

                SiAuto.Main.LogMessage("There are " + eventCount.ToString() + " events in the summary list.");

                if (count != eventCount)
                {
                    SiAuto.Main.LogWarning("The number of rows in the data table (" + count.ToString() + ") doesn't match the expected number of events (" + eventCount.ToString() + ").");
                }

                SiAuto.Main.LogMessage("Write out the alerts (if any exist).");
                int alertsWritten = 0;
                foreach (DataRow row in eventsOfTheDay.Select("AttributeID > -1", "WhenDetected DESC"))
                {
                    SiAuto.Main.LogMessage("Writing out individual alert.");
                    writer.WriteStartElement("alert");

                    writer.WriteAttributeString("whenDetected", ((DateTime)row["WhenDetected"]).ToString("G"));
                    writer.WriteAttributeString("diskModel", row["DiskModel"].ToString());
                    writer.WriteAttributeString("diskPath", row["DiskPath"].ToString());
                    writer.WriteAttributeString("attributeID", row["AttributeID"].ToString());
                    writer.WriteAttributeString("attributeName", row["AttributeName"].ToString());
                    writer.WriteAttributeString("healthTitle", row["HealthTitle"].ToString());
                    writer.WriteAttributeString("healthMessage", row["HealthMessage"].ToString());
                    writer.WriteAttributeString("isCritical", row["IsCritical"].ToString());
                    writer.WriteAttributeString("actionMode", row["Action"].ToString());
                    
                    writer.WriteEndElement(); // alert
                    alertsWritten++;
                }

                SiAuto.Main.LogMessage(alertsWritten.ToString() + " were written to the summary data file.");

                writer.WriteEndElement(); // alerts

                SiAuto.Main.LogMessage("Write the XML end elment and close the XML stream.");
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();

                SiAuto.Main.LogMessage("Open the XML file for writing in overwrite mode.");
                System.IO.StreamWriter xmlOutput = new System.IO.StreamWriter(filename, false);
                SiAuto.Main.LogMessage("Write out the file.");
                xmlOutput.Write(stream.ToString());
                SiAuto.Main.LogMessage("Flush and close the I/O stream.");
                xmlOutput.Flush();
                xmlOutput.Close();
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogError("Exceptions were detected writing the XML alerts. " + ex.Message);
                SiAuto.Main.LogException(ex);
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.HssServiceHelper.DailySummary.ComposeNewFile");
        }

        public int EventCount
        {
            get
            {
                return eventCount;
            }
        }

        public int PowerEventCount
        {
            get
            {
                return powerEventCount;
            }
            set
            {
                powerEventCount = value;
            }
        }

        public bool NeedSummarySend
        {
            get
            {
                return needSummarySend;
            }
        }
    }
}
