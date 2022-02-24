using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdSmartbuyDefinitions
    {
        DataTable ssdSmartbuyDefinitions;

        public SmartSsdSmartbuyDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdSmartbuyDefinitions");
            PopulateSsdSmartbuyDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdSmartbuyDefinitions");
        }

        public void PopulateSsdSmartbuyDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdSmartbuyDefinitions.PopulateSsdSmartbuyDataTable");
            ssdSmartbuyDefinitions = new DataTable("SsdSmartbuySMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdSmartbuyDefinitions.Columns.Add("Key", typeof(int));
            ssdSmartbuyDefinitions.Columns.Add("Dec", typeof(String));
            ssdSmartbuyDefinitions.Columns.Add("Hex", typeof(String));
            ssdSmartbuyDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdSmartbuyDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdSmartbuyDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdSmartbuyDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = true;
            row["AttributeName"] = "Raw Read Error Rate";
            row["Description"] = "Count of raw data errors while data from media, including retry errors or data error (uncorrectable).";
            ssdSmartbuyDefinitions.Rows.Add(row);

            row = ssdSmartbuyDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Time Count";
            row["Description"] = "Number of hours elapsed in the Power-On state.";
            ssdSmartbuyDefinitions.Rows.Add(row);

            row = ssdSmartbuyDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Drive Power Cycle Count";
            row["Description"] = "Number of power-on events.";
            ssdSmartbuyDefinitions.Rows.Add(row);

            row = ssdSmartbuyDefinitions.NewRow();
            row["Key"] = 168;
            row["Dec"] = "168";
            row["Hex"] = "A8";
            row["IsCritical"] = false;
            row["AttributeName"] = "SATA PHY Error Count";
            row["Description"] = "";
            ssdSmartbuyDefinitions.Rows.Add(row);

            row = ssdSmartbuyDefinitions.NewRow();
            row["Key"] = 170;
            row["Dec"] = "170";
            row["Hex"] = "AA";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reserved Block Count";
            row["Description"] = "The number of reserve blocks remaining for use as active blocks are retired due to media wearout.";
            ssdSmartbuyDefinitions.Rows.Add(row);

            row = ssdSmartbuyDefinitions.NewRow();
            row["Key"] = 173;
            row["Dec"] = "173";
            row["Hex"] = "AD";
            row["IsCritical"] = false;
            row["AttributeName"] = "Wear Leveling Count";
            row["Description"] = "";
            ssdSmartbuyDefinitions.Rows.Add(row);

            row = ssdSmartbuyDefinitions.NewRow();
            row["Key"] = 192;
            row["Dec"] = "192";
            row["Hex"] = "C0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unexpected Power Loss Count";
            row["Description"] = "The number of times power to the disk was lost unexpectedly.";
            ssdSmartbuyDefinitions.Rows.Add(row);

            row = ssdSmartbuyDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Disk Temperature";
            row["Description"] = "Temperature of the base casting.";
            ssdSmartbuyDefinitions.Rows.Add(row);

            row = ssdSmartbuyDefinitions.NewRow();
            row["Key"] = 218;
            row["Dec"] = "218";
            row["Hex"] = "DA";
            row["IsCritical"] = true;
            row["AttributeName"] = "FlashROM ECC Correction Count";
            row["Description"] = "";
            ssdSmartbuyDefinitions.Rows.Add(row);

            row = ssdSmartbuyDefinitions.NewRow();
            row["Key"] = 231;
            row["Dec"] = "231";
            row["Hex"] = "E7";
            row["IsCritical"] = true;
            row["AttributeName"] = "SSD Life Remaining";
            row["Description"] = "An estimation of SSD life remaining based on the write activity on the disk across all good blocks.";
            ssdSmartbuyDefinitions.Rows.Add(row);

            row = ssdSmartbuyDefinitions.NewRow();
            row["Key"] = 241;
            row["Dec"] = "241";
            row["Hex"] = "F1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Lifetime Writes (GB)";
            row["Description"] = "Number of blocks marked suspect due to uncorrectable errors.";
            ssdSmartbuyDefinitions.Rows.Add(row);

            ssdSmartbuyDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdSmartbuyDefinitions.PopulateSsdSmartbuyDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdSmartbuyDefinitions;
            }
        }
    }
}
