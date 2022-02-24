using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdSkHynixDefinitions
    {
        DataTable ssdSkHynixDefinitions;

        public SmartSsdSkHynixDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdSkHynixDefinitions");
            PopulateSsdSkHynixDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdSkHynixDefinitions");
        }

        public void PopulateSsdSkHynixDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdSkHynixDefinitions.PopulateSsdSkHynixDataTable");
            ssdSkHynixDefinitions = new DataTable("SsdSkHynixSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdSkHynixDefinitions.Columns.Add("Key", typeof(int));
            ssdSkHynixDefinitions.Columns.Add("Dec", typeof(String));
            ssdSkHynixDefinitions.Columns.Add("Hex", typeof(String));
            ssdSkHynixDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdSkHynixDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdSkHynixDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdSkHynixDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = true;
            row["AttributeName"] = "Raw Read Error Rate";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Retired Block Count";
            row["Description"] = "Count of reallocated blocks. This is the count of reallocated or remapped sectors during normal operation from the grown defects table.";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power-On Hours";
            row["Description"] = "Total number of hours in power-on state.";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle Count";
            row["Description"] = "This attribute indicates the count of full hard disk power on/off cycles.";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 64;
            row["Dec"] = "31";
            row["Hex"] = "64";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Erase Count";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 168;
            row["Dec"] = "168";
            row["Hex"] = "A8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Minimum Erase Count";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 169;
            row["Dec"] = "169";
            row["Hex"] = "A9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Maximum Erase Count";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 171;
            row["Dec"] = "171";
            row["Hex"] = "AB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Count";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 172;
            row["Dec"] = "172";
            row["Hex"] = "AC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Count";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 174;
            row["Dec"] = "174";
            row["Hex"] = "AE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unexpected Power Loss Count";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 175;
            row["Dec"] = "175";
            row["Hex"] = "AF";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Count (Worst Case)";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 176;
            row["Dec"] = "176";
            row["Hex"] = "B0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Count (Worst Case)";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 177;
            row["Dec"] = "177";
            row["Hex"] = "B1";
            row["IsCritical"] = true;
            row["AttributeName"] = "Wear Leveling Count";
            row["Description"] = "Indicates the worst-case erase count. Used as an indicator of SSD life remaining.";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 178;
            row["Dec"] = "178";
            row["Hex"] = "B2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Used Rserved Block Count (Worst Case)";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 179;
            row["Dec"] = "179";
            row["Hex"] = "B3";
            row["IsCritical"] = false;
            row["AttributeName"] = "Used Reserved Block Count";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 180;
            row["Dec"] = "180";
            row["Hex"] = "B4";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unused Reserved Block Count";
            row["Description"] = "Indicates the number of reserved blocks available to replace retired blocks.";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = true;
            row["AttributeName"] = "End-to-End Error Detection Count";
            row["Description"] = "A count of errors that occurred in the drive's on-board cache RAM.";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 187;
            row["Dec"] = "187";
            row["Hex"] = "BB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Uncorrectable Error Count";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 188;
            row["Dec"] = "188";
            row["Hex"] = "BC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Command Timeout Count";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "Temperature reported from the on-board sensor.";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 195;
            row["Dec"] = "195";
            row["Hex"] = "C3";
            row["IsCritical"] = false;
            row["AttributeName"] = "On-the-Fly ECC Uncorrectable Error Rate";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 196;
            row["Dec"] = "196";
            row["Hex"] = "C4";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reallocation Event Count";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 198;
            row["Dec"] = "198";
            row["Hex"] = "C6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Uncorrectable Sector Count";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 199;
            row["Dec"] = "199";
            row["Hex"] = "C7";
            row["IsCritical"] = false;
            row["AttributeName"] = "CRC Error Count";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 204;
            row["Dec"] = "204";
            row["Hex"] = "CC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Soft ECC Correction Rate";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 212;
            row["Dec"] = "212";
            row["Hex"] = "D4";
            row["IsCritical"] = false;
            row["AttributeName"] = "PHY Error Count";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 234;
            row["Dec"] = "234";
            row["Hex"] = "EA";
            row["IsCritical"] = false;
            row["AttributeName"] = "NAND Written";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 241;
            row["Dec"] = "241";
            row["Hex"] = "F1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Lifetime Writes from the Host in GB";
            row["Description"] = "Indicates the total number of gigabytes written.";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 242;
            row["Dec"] = "242";
            row["Hex"] = "F2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Lifetime Reads from the Host in GB";
            row["Description"] = "Indicates the total number of gigabytes read.";
            ssdSkHynixDefinitions.Rows.Add(row);

            row = ssdSkHynixDefinitions.NewRow();
            row["Key"] = 250;
            row["Dec"] = "250";
            row["Hex"] = "FA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Number of NAND Read Retries";
            row["Description"] = "";
            ssdSkHynixDefinitions.Rows.Add(row);

            ssdSkHynixDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdSkHynixDefinitions.PopulateSsdSkHynixDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdSkHynixDefinitions;
            }
        }
    }
}
