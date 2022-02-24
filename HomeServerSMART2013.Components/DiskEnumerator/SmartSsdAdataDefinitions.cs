using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdAdataDefinitions
    {
        DataTable ssdAdataDefinitions;

        public SmartSsdAdataDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdAdataDefinitions");
            PopulateSsdAdataDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdAdataDefinitions");
        }

        public void PopulateSsdAdataDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdAdataDefinitions.PopulateSsdAdataDataTable");
            ssdAdataDefinitions = new DataTable("SsdAdataSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdAdataDefinitions.Columns.Add("Key", typeof(int));
            ssdAdataDefinitions.Columns.Add("Dec", typeof(String));
            ssdAdataDefinitions.Columns.Add("Hex", typeof(String));
            ssdAdataDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdAdataDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdAdataDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdAdataDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = false;
            row["AttributeName"] = "Raw Read Error Rate";
            row["Description"] = "Count of raw data errors while data from media, including retry errors or data error (uncorrectable).";
            ssdAdataDefinitions.Rows.Add(row);

            row = ssdAdataDefinitions.NewRow();
            row["Key"] = 2;
            row["Dec"] = "02";
            row["Hex"] = "02";
            row["IsCritical"] = true;
            row["AttributeName"] = "Throughput Performance";
            row["Description"] = "Internally measured average and worst data transfer rate.";
            ssdAdataDefinitions.Rows.Add(row);

            row = ssdAdataDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Retired Sector Count";
            row["Description"] = "Count of reallocated blocks. This is the count of reallocated or remapped sectors during normal operation from the grown defects table.";
            ssdAdataDefinitions.Rows.Add(row);

            row = ssdAdataDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Hours";
            row["Description"] = "Number of hours elapsed in the Power-On state.";
            ssdAdataDefinitions.Rows.Add(row);

            row = ssdAdataDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle";
            row["Description"] = "Number of power-on events.";
            ssdAdataDefinitions.Rows.Add(row);

            row = ssdAdataDefinitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = false;
            row["AttributeName"] = "End-to-End Error Count";
            row["Description"] = "Tracks the number of end to end internal card data path errors that were detected.";
            ssdAdataDefinitions.Rows.Add(row);

            row = ssdAdataDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "Temperature of the base casting.";
            ssdAdataDefinitions.Rows.Add(row);

            row = ssdAdataDefinitions.NewRow();
            row["Key"] = 196;
            row["Dec"] = "196";
            row["Hex"] = "C4";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reallocation Event";
            row["Description"] = "Total number of remapping events during normal operation and offline surface scanning.";
            ssdAdataDefinitions.Rows.Add(row);

            row = ssdAdataDefinitions.NewRow();
            row["Key"] = 197;
            row["Dec"] = "197";
            row["Hex"] = "C5";
            row["IsCritical"] = true;
            row["AttributeName"] = "Current Pending Sector Count";
            row["Description"] = "Number of blocks marked suspect due to uncorrectable errors.";
            ssdAdataDefinitions.Rows.Add(row);

            ssdAdataDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdAdataDefinitions.PopulateSsdAdataDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdAdataDefinitions;
            }
        }
    }
}
