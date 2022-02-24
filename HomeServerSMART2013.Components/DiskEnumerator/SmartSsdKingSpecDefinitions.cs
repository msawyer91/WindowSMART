using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdKingSpecDefinitions
    {
        DataTable ssdKingSpecDefinitions;

        public SmartSsdKingSpecDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdKingSpecDefinitions");
            PopulateSsdKingSpecDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdKingSpecDefinitions");
        }

        public void PopulateSsdKingSpecDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdKingSpecDefinitions.PopulateSsdKingSpecDataTable");
            ssdKingSpecDefinitions = new DataTable("SsdKingSpecSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdKingSpecDefinitions.Columns.Add("Key", typeof(int));
            ssdKingSpecDefinitions.Columns.Add("Dec", typeof(String));
            ssdKingSpecDefinitions.Columns.Add("Hex", typeof(String));
            ssdKingSpecDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdKingSpecDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdKingSpecDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdKingSpecDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = false;
            row["AttributeName"] = "Raw Read Error Rate";
            row["Description"] = "";
            ssdKingSpecDefinitions.Rows.Add(row);

            row = ssdKingSpecDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Retired Sector Count";
            row["Description"] = "Count of reallocated blocks. This is the count of reallocated or remapped sectors during normal operation from the grown defects table.";
            ssdKingSpecDefinitions.Rows.Add(row);

            row = ssdKingSpecDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Hours";
            row["Description"] = "Number of hours elapsed in the Power-On state.";
            ssdKingSpecDefinitions.Rows.Add(row);

            row = ssdKingSpecDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Drive Power Cycle Count";
            row["Description"] = "Number of power-on events.";
            ssdKingSpecDefinitions.Rows.Add(row);

            row = ssdKingSpecDefinitions.NewRow();
            row["Key"] = 192;
            row["Dec"] = "192";
            row["Hex"] = "C0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unsafe Shutdown Count";
            row["Description"] = "";
            ssdKingSpecDefinitions.Rows.Add(row);

            row = ssdKingSpecDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Disk Temperature";
            row["Description"] = "Temperature of the base casting.";
            ssdKingSpecDefinitions.Rows.Add(row);

            row = ssdKingSpecDefinitions.NewRow();
            row["Key"] = 195;
            row["Dec"] = "195";
            row["Hex"] = "C3";
            row["IsCritical"] = false;
            row["AttributeName"] = "Hardware ECC Recovered";
            row["Description"] = "";
            ssdKingSpecDefinitions.Rows.Add(row);

            row = ssdKingSpecDefinitions.NewRow();
            row["Key"] = 196;
            row["Dec"] = "196";
            row["Hex"] = "C4";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reallocation Event Count";
            row["Description"] = "Total number of remapping events during normal operation and offline surface scanning.";
            ssdKingSpecDefinitions.Rows.Add(row);

            row = ssdKingSpecDefinitions.NewRow();
            row["Key"] = 241;
            row["Dec"] = "241";
            row["Hex"] = "F1";
            row["IsCritical"] = true;
            row["AttributeName"] = "Total Host Writes";
            row["Description"] = "";
            ssdKingSpecDefinitions.Rows.Add(row);

            row = ssdKingSpecDefinitions.NewRow();
            row["Key"] = 242;
            row["Dec"] = "242";
            row["Hex"] = "F2";
            row["IsCritical"] = true;
            row["AttributeName"] = "Total Host Reads";
            row["Description"] = "";
            ssdKingSpecDefinitions.Rows.Add(row);

            ssdKingSpecDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdKingSpecDefinitions.PopulateSsdKingSpecDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdKingSpecDefinitions;
            }
        }
    }
}
