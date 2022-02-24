using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public class FeverishDisk
    {
        DataTable feverishDisks;

        public FeverishDisk()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.FeverishDisk.FeverishDisk");
            ComposeDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.FeverishDisk.FeverishDisk");
        }

        /// <summary>
        /// Constructs the data table.
        /// </summary>
        public void ComposeDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.FeverishDisk.ComposeDataTable");
            feverishDisks = new DataTable("FeverishDisks");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            feverishDisks.Columns.Add("Key", typeof(Guid));
            feverishDisks.Columns.Add("DiskModel", typeof(String));
            feverishDisks.Columns.Add("DiskPath", typeof(String));
            feverishDisks.Columns.Add("HealthTitle", typeof(String));
            feverishDisks.Columns.Add("HealthMessage", typeof(String));
            feverishDisks.Columns.Add("AttributeID", typeof(int));
            feverishDisks.Columns.Add("AttributeName", typeof(String));
            feverishDisks.Columns.Add("IsCritical", typeof(bool));
            feverishDisks.Columns.Add("Guid", typeof(String));
            feverishDisks.Columns.Add("Action", typeof(AlertAction));
            feverishDisks.Columns.Add("EventID", typeof(int));
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.FeverishDisk.ComposeDataTable");
        }

        /// <summary>
        /// Adds or updates an item in the FeverishDisks table, based on the AlertAction.  In the case
        /// of an update, the existing item is analyzed and, if it's the same as the information
        /// provided, the item is left alone.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="diskModel"></param>
        /// <param name="diskPath"></param>
        /// <param name="healthTitle"></param>
        /// <param name="healthMessage"></param>
        /// <param name="attributeId"></param>
        /// <param name="attributeName"></param>
        /// <param name="isCritical"></param>
        /// <param name="action"></param>
        public void AddItem(String key, String diskModel, String diskPath, String healthTitle, String healthMessage,
            int attributeId, String attributeName, bool isCritical, AlertAction action, int eventID)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.FeverishDisk.AddItem");
            SiAuto.Main.LogString("key", key);
            SiAuto.Main.LogString("diskModel", diskModel);
            SiAuto.Main.LogString("diskPath", diskPath);
            SiAuto.Main.LogString("healthTitle", healthTitle);
            SiAuto.Main.LogString("healthMessage", healthMessage);
            SiAuto.Main.LogInt("attributeId", attributeId);
            SiAuto.Main.LogString("attributeName", attributeName);
            SiAuto.Main.LogObjectValue("AlertAction", action);
            SiAuto.Main.LogInt("eventID", eventID);

            if (action == AlertAction.Insert)
            {
                SiAuto.Main.LogMessage("Insert item.");
                DataRow row = feverishDisks.NewRow();

                row["Key"] = new Guid(key);
                row["DiskModel"] = diskModel;
                row["DiskPath"] = diskPath;
                row["HealthTitle"] = healthTitle;
                row["HealthMessage"] = healthMessage;
                row["AttributeID"] = attributeId;
                row["AttributeName"] = attributeName;
                row["IsCritical"] = isCritical;
                row["Guid"] = key;
                row["Action"] = action;
                row["EventID"] = eventID;
                feverishDisks.Rows.Add(row);
                feverishDisks.AcceptChanges();
                SiAuto.Main.LogMessage("Item inserted; changes committed.");
                SiAuto.Main.LogMessage("Writing an event to the event log.");
                if (isCritical)
                {
                    WindowsEventLogger.LogError("A Critical disk event (" + healthTitle + ") was detected: " + healthMessage +
                        "\n\nDisk: " + diskModel + ", Path: " + diskPath + ", Attribute ID: " + attributeId + ", Attribute: " + attributeName +
                        ", Correlation ID: " + key, eventID, Properties.Resources.EventLogTaryn);
                }
                else
                {
                    WindowsEventLogger.LogError("A Warning disk event (" + healthTitle + ") was detected: " + healthMessage +
                        "\n\nDisk: " + diskModel + ", Path: " + diskPath + ", Attribute ID: " + attributeId + ", Attribute: " + attributeName +
                        ", Correlation ID: " + key, eventID, Properties.Resources.EventLogTaryn);
                }
                SiAuto.Main.LogMessage("Done writing an event to the event log.");
            }
            else if (action == AlertAction.Update)
            {
                SiAuto.Main.LogMessage("Update item.");
                DataRow[] rows = feverishDisks.Select("Key='" + key + "'");
                if (rows != null && rows.Length > 0 && rows[0] != null)
                {
                    DataRow row = rows[0];
                    if (String.Compare(row["HealthTitle"].ToString(), healthTitle, true) == 0 &&
                        (bool)row["IsCritical"] == isCritical)
                    {
                        // Item is the same; don't update it.
                        SiAuto.Main.LogMessage("No changes on item; returning.");
                        SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.FeverishDisk.AddItem");
                        return;
                    }
                    else
                    {
                        // Update the item. Stuff like disk model, path won't change.
                        row["HealthTitle"] = healthTitle;
                        row["HealthMessage"] = healthMessage;
                        row["AttributeID"] = attributeId;
                        row["AttributeName"] = attributeName;
                        row["IsCritical"] = isCritical;
                        row["Action"] = action;
                        row.AcceptChanges();
                        feverishDisks.AcceptChanges();
                        SiAuto.Main.LogMessage("Item updated; changes committed.");
                        SiAuto.Main.LogMessage("Writing an event to the event log.");
                        if (isCritical)
                        {
                            WindowsEventLogger.LogError("A Critical disk event (" + healthTitle + ") was detected: " + healthMessage +
                                "(Note this is a CHANGE to an existing event; it may have worsened or improved.)\n\n" +
                                "Disk: " + diskModel + ", Path: " + diskPath + ", Attribute ID: " + attributeId + ", Attribute: " + attributeName +
                                ", Correlation ID: " + key, eventID, Properties.Resources.EventLogTaryn);
                        }
                        else
                        {
                            WindowsEventLogger.LogError("A Warning disk event (" + healthTitle + ") was detected: " + healthMessage +
                                "(Note this is a CHANGE to an existing event; it may have worsened or improved.)\n\n" +
                                "Disk: " + diskModel + ", Path: " + diskPath + ", Attribute ID: " + attributeId + ", Attribute: " + attributeName +
                                ", Correlation ID: " + key, eventID, Properties.Resources.EventLogTaryn);
                        }
                        SiAuto.Main.LogMessage("Done writing an event to the event log.");
                    }
                }
                else
                {
                    SiAuto.Main.LogMessage("Item not found; returning.");
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.FeverishDisk.AddItem");
                    return;
                }
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.FeverishDisk.AddItem");
        }

        /// <summary>
        /// Marks an item for expurgation. The item is not removed at this time; rather it is
        /// marked so that it can be culled from the alert stack, after which the item will
        /// be obliterated.
        /// </summary>
        /// <param name="key"></param>
        public void RemoveItem(String key)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.FeverishDisk.RemoveItem");
            DataRow[] rows = feverishDisks.Select("Key='" + key + "'");
            if (rows != null && rows.Length > 0 && rows[0] != null)
            {
                DataRow row = rows[0];
                row["Action"] = AlertAction.Remove;
                row.AcceptChanges();
                feverishDisks.AcceptChanges();
                SiAuto.Main.LogMessage("Item removed; changes committed.");
                SiAuto.Main.LogMessage("Writing an event to the event log.");
                WindowsEventLogger.LogInformation("An alert item is being removed. This may be because the alert is no longer active, or an existing alert " +
                    "has changed. Alert correlation ID: " + key, 53851, Properties.Resources.EventLogTaryn);
                SiAuto.Main.LogMessage("Done writing an event to the event log.");
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.FeverishDisk.RemoveItem");
        }


        /// <summary>
        /// Determines if the item specified by key already exists in the table.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if the item exists; false otherwise.</returns>
        public bool ItemExists(String key)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.FeverishDisk.ItemExists");
            SiAuto.Main.LogString("key", key);
            SiAuto.Main.LogInt("Active alert count", feverishDisks.Rows.Count);
            DataRow[] rows = feverishDisks.Select("Key='" + key + "'");
            if (rows != null && rows.Length > 0 && rows[0] != null)
            {
                SiAuto.Main.LogBool("ItemExists", true);
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.FeverishDisk.ItemExists");
                return true;
            }
            else
            {
                SiAuto.Main.LogBool("ItemExists", false);
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.FeverishDisk.ItemExists");
                return false;
            }
        }

        public DataTable FeverishDisks
        {
            get
            {
                return feverishDisks;
            }
        }
    }

    public enum AlertAction : int
    {
        Insert = 0,
        Remove = 1,
        Hold = 2,
        Update = 3
    }
}
