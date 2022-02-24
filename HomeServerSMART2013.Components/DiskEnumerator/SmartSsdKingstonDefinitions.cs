using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdKingstonDefinitions
    {
        DataTable ssdKingstonDefinitions;

        public SmartSsdKingstonDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdKingstonDefinitions");
            PopulateSsdKingstonDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdKingstonDefinitions");
        }

        public void PopulateSsdKingstonDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdKingstonDefinitions.PopulateSsdKingstonDataTable");
            ssdKingstonDefinitions = new DataTable("SsdKingstonSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdKingstonDefinitions.Columns.Add("Key", typeof(int));
            ssdKingstonDefinitions.Columns.Add("Dec", typeof(String));
            ssdKingstonDefinitions.Columns.Add("Hex", typeof(String));
            ssdKingstonDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdKingstonDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdKingstonDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdKingstonDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = false;
            row["AttributeName"] = "Raw Read Error Rate";
            row["Description"] = "Count of raw data errors while data from media, including retry errors or data error (uncorrectable).";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 2;
            row["Dec"] = "02";
            row["Hex"] = "02";
            row["IsCritical"] = false;
            row["AttributeName"] = "Throughput Performance";
            row["Description"] = "Internally measured average and worst data transfer rate.";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 3;
            row["Dec"] = "03";
            row["Hex"] = "03";
            row["IsCritical"] = false;
            row["AttributeName"] = "Spin Up Time";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Retired Sector Count";
            row["Description"] = "Count of reallocated blocks. This is the count of reallocated or remapped sectors during normal operation from the grown defects table.";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 7;
            row["Dec"] = "07";
            row["Hex"] = "07";
            row["IsCritical"] = false;
            row["AttributeName"] = "Seek Error Rate";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Hours";
            row["Description"] = "Number of hours elapsed in the Power-On state.";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 11;
            row["Dec"] = "11";
            row["Hex"] = "0A";
            row["IsCritical"] = false;
            row["AttributeName"] = "Spin Retry Count";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle Count";
            row["Description"] = "Number of power-on events.";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 168;
            row["Dec"] = "168";
            row["Hex"] = "A8";
            row["IsCritical"] = false;
            row["AttributeName"] = "SATA PHY Error Count";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 170;
            row["Dec"] = "170";
            row["Hex"] = "AA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Bad Block Count";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 173;
            row["Dec"] = "173";
            row["Hex"] = "AD";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Count";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 175;
            row["Dec"] = "175";
            row["Hex"] = "AF";
            row["IsCritical"] = false;
            row["AttributeName"] = "Bad Cluster Table Count";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = false;
            row["AttributeName"] = "End-to-End Error Count";
            row["Description"] = "Tracks the number of end to end internal card data path errors that were detected.";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 187;
            row["Dec"] = "187";
            row["Hex"] = "BB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Uncorrectable Errors";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 192;
            row["Dec"] = "192";
            row["Hex"] = "C0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unsafe Shutdown Count";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "Temperature of the base casting.";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 196;
            row["Dec"] = "196";
            row["Hex"] = "C4";
            row["IsCritical"] = false;
            row["AttributeName"] = "Reallocation Event/Later Bad Block Count";
            row["Description"] = "Total number of remapping events during normal operation and offline surface scanning.";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 197;
            row["Dec"] = "197";
            row["Hex"] = "C5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Current Pending Sector Count";
            row["Description"] = "Number of blocks marked suspect due to uncorrectable errors.";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 199;
            row["Dec"] = "199";
            row["Hex"] = "C7";
            row["IsCritical"] = true;
            row["AttributeName"] = "CRC Error Count";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 218;
            row["Dec"] = "218";
            row["Hex"] = "DA";
            row["IsCritical"] = false;
            row["AttributeName"] = "CRC Error Count";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 231;
            row["Dec"] = "231";
            row["Hex"] = "E7";
            row["IsCritical"] = true;
            row["AttributeName"] = "SSD Life Left";
            row["Description"] = "Indicates the approximate SSD life left, in terms of P/E cycles or flash blocks currently available for use.";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 232;
            row["Dec"] = "232";
            row["Hex"] = "E8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Available Reserved Space";
            row["Description"] = "Indicates the amount of reserve flash memory space in service.";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 233;
            row["Dec"] = "233";
            row["Hex"] = "E9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Lifetime Writes to Flash";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 235;
            row["Dec"] = "235";
            row["Hex"] = "EB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Fail Backup Health";
            row["Description"] = "Indicates the condition of an external hold up circuit based on test results from the SF-2000 \"Supercap Test\".";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 240;
            row["Dec"] = "240";
            row["Hex"] = "F0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Write Head";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 241;
            row["Dec"] = "241";
            row["Hex"] = "F1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Lifetime Writes from Host (GB)";
            row["Description"] = "Indicates the number of bytes (in 1GB resolution) written to the drive by a host system, over the life of the drive.";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 242;
            row["Dec"] = "242";
            row["Hex"] = "F2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Lifetime Reads from Host (64 GB)";
            row["Description"] = "Indicates the number of bytes (in 64GB resolution) read from the drive by a host system, over the life of the drive.";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 244;
            row["Dec"] = "244";
            row["Hex"] = "F4";
            row["IsCritical"] = false;
            row["AttributeName"] = "Average Erase Count";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 245;
            row["Dec"] = "245";
            row["Hex"] = "F5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Maximum Erase Count";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            row = ssdKingstonDefinitions.NewRow();
            row["Key"] = 246;
            row["Dec"] = "246";
            row["Hex"] = "F6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Erase Count";
            row["Description"] = "";
            ssdKingstonDefinitions.Rows.Add(row);

            ssdKingstonDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdKingstonDefinitions.PopulateSsdKingstonDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdKingstonDefinitions;
            }
        }
    }
}
