using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdGenericDefinitions
    {
        DataTable ssdGenericDefinitions;

        public SmartSsdGenericDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdGenericDefinitions");
            PopulateSsdGenericDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdGenericDefinitions");
        }

        public void PopulateSsdGenericDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdGenericDefinitions.PopulateSsdGenericDataTable");
            ssdGenericDefinitions = new DataTable("SsdGenericSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdGenericDefinitions.Columns.Add("Key", typeof(int));
            ssdGenericDefinitions.Columns.Add("Dec", typeof(String));
            ssdGenericDefinitions.Columns.Add("Hex", typeof(String));
            ssdGenericDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdGenericDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdGenericDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdGenericDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = false;
            row["AttributeName"] = "Raw Read Error Rate";
            row["Description"] = "Count of raw data errors while data from media, including retry errors or data error (uncorrectable).";
            ssdGenericDefinitions.Rows.Add(row);

            row = ssdGenericDefinitions.NewRow();
            row["Key"] = 2;
            row["Dec"] = "02";
            row["Hex"] = "02";
            row["IsCritical"] = true;
            row["AttributeName"] = "Throughput Performance";
            row["Description"] = "Internally measured average and worst data transfer rate.";
            ssdGenericDefinitions.Rows.Add(row);

            row = ssdGenericDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Retired Sector Count";
            row["Description"] = "Count of reallocated blocks. This is the count of reallocated or remapped sectors during normal operation from the grown defects table.";
            ssdGenericDefinitions.Rows.Add(row);

            row = ssdGenericDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Hours";
            row["Description"] = "Number of hours elapsed in the Power-On state.";
            ssdGenericDefinitions.Rows.Add(row);

            row = ssdGenericDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle";
            row["Description"] = "Number of power-on events.";
            ssdGenericDefinitions.Rows.Add(row);

            row = ssdGenericDefinitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = false;
            row["AttributeName"] = "End-to-End Error Count";
            row["Description"] = "Tracks the number of end to end internal card data path errors that were detected.";
            ssdGenericDefinitions.Rows.Add(row);

            row = ssdGenericDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "Temperature of the base casting.";
            ssdGenericDefinitions.Rows.Add(row);

            row = ssdGenericDefinitions.NewRow();
            row["Key"] = 196;
            row["Dec"] = "196";
            row["Hex"] = "C4";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reallocation Event";
            row["Description"] = "Total number of remapping events during normal operation and offline surface scanning.";
            ssdGenericDefinitions.Rows.Add(row);

            row = ssdGenericDefinitions.NewRow();
            row["Key"] = 197;
            row["Dec"] = "197";
            row["Hex"] = "C5";
            row["IsCritical"] = true;
            row["AttributeName"] = "Current Pending Sector Count";
            row["Description"] = "Number of blocks marked suspect due to uncorrectable errors.";
            ssdGenericDefinitions.Rows.Add(row);

            ssdGenericDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartHddDefinitions.PopulateSsdGenericDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdGenericDefinitions;
            }
        }
    }
}
