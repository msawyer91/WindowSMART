using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdSamsungDefinitions
    {
        DataTable ssdSamsungDefinitions;

        public SmartSsdSamsungDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdSamsungDefinitions");
            PopulateSsdSamsungDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdSamsungDefinitions");
        }

        public void PopulateSsdSamsungDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdSamsungDefinitions.PopulateSsdSamsungDataTable");
            ssdSamsungDefinitions = new DataTable("SsdSamsungSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdSamsungDefinitions.Columns.Add("Key", typeof(int));
            ssdSamsungDefinitions.Columns.Add("Dec", typeof(String));
            ssdSamsungDefinitions.Columns.Add("Hex", typeof(String));
            ssdSamsungDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdSamsungDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdSamsungDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdSamsungDefinitions.NewRow();

            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reallocated Sectors Count";
            row["Description"] = "Count of reallocated blocks. This is the count of reallocated or remapped sectors during normal operation from the grown defects table.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Time Count";
            row["Description"] = "Total number of hours in power-on state.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Drive Power Cycle Count";
            row["Description"] = "This attribute indicates the count of full hard disk power on/off cycles.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 170;
            row["Dec"] = "170";
            row["Hex"] = "AA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unused Reserved Block Count (Chip)";
            row["Description"] = "The number of remaining reserved blocks.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 171;
            row["Dec"] = "171";
            row["Hex"] = "AB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Count (Chip)";
            row["Description"] = "Counts the number of flash program failures.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 172;
            row["Dec"] = "172";
            row["Hex"] = "AC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Count (Chip)";
            row["Description"] = "Counts the number of flash erase command failures.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 173;
            row["Dec"] = "173";
            row["Hex"] = "AD";
            row["IsCritical"] = false;
            row["AttributeName"] = "Wear Leveling Count";
            row["Description"] = "Indicates the worst-case erase count. Used as an indicator of SSD life remaining.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 174;
            row["Dec"] = "174";
            row["Hex"] = "AE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unexpected Power Loss Count";
            row["Description"] = "Number of times the drive unexpectedly lost power.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 175;
            row["Dec"] = "175";
            row["Hex"] = "AF";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Count (Chip)";
            row["Description"] = "Counts the number of flash program failures.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 176;
            row["Dec"] = "176";
            row["Hex"] = "B0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Count (Chip)";
            row["Description"] = "Counts the number of flash erase command failures.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 177;
            row["Dec"] = "177";
            row["Hex"] = "B1";
            row["IsCritical"] = true;
            row["AttributeName"] = "Wear Leveling Count";
            row["Description"] = "Indicates the worst-case erase count. Used as an indicator of SSD life remaining.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 178;
            row["Dec"] = "178";
            row["Hex"] = "B2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Used Rserved Block Count (Chip)";
            row["Description"] = "Indicates the number of reserved blocks used to replace retired blocks.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 179;
            row["Dec"] = "179";
            row["Hex"] = "B3";
            row["IsCritical"] = true;
            row["AttributeName"] = "Used Reserved Block Count (Total)";
            row["Description"] = "Indicates the number of reserved blocks used to replace retired blocks.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 180;
            row["Dec"] = "180";
            row["Hex"] = "B4";
            row["IsCritical"] = true;
            row["AttributeName"] = "Unused Reserved Block Count (Total)";
            row["Description"] = "Indicates the number of reserved blocks available to replace retired blocks.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 181;
            row["Dec"] = "181";
            row["Hex"] = "B5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Count (Total)";
            row["Description"] = "Four bytes used to show the program failures since the drive was deployed.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 182;
            row["Dec"] = "182";
            row["Hex"] = "B6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Count (Total)";
            row["Description"] = "Four bytes used to show the number of block erase failures since the drive was deployed.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 183;
            row["Dec"] = "183";
            row["Hex"] = "B7";
            row["IsCritical"] = true;
            row["AttributeName"] = "Runtime Bad Block (Total)";
            row["Description"] = "Four bytes used to show the number of bad blocks detected at runtime.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = true;
            row["AttributeName"] = "Error Detection";
            row["Description"] = "";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 187;
            row["Dec"] = "187";
            row["Hex"] = "BB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Uncorrectable Error Count";
            row["Description"] = "";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 190;
            row["Dec"] = "190";
            row["Hex"] = "BE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "Indicate's the drive's temperature.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Airflow Temperature";
            row["Description"] = "Temperature reported from the on-board sensor.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 195;
            row["Dec"] = "195";
            row["Hex"] = "C3";
            row["IsCritical"] = false;
            row["AttributeName"] = "ECC Rate";
            row["Description"] = "Indicates the rate of errors detected and corrected by error correcting code.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 198;
            row["Dec"] = "198";
            row["Hex"] = "C6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Off-Line Uncorrectable Error Count";
            row["Description"] = "";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 199;
            row["Dec"] = "199";
            row["Hex"] = "C7";
            row["IsCritical"] = false;
            row["AttributeName"] = "Ultra ATA CRC Error Count";
            row["Description"] = "The number of errors in data transfer via the interface cable as determined by ICRC (Interface Cyclic Redundancy Check). Usually indicates a faulty data cable, but in some cases can be seen in external USB cases where the USB bridge chip is not fully compatible with the host device driver.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 201;
            row["Dec"] = "201";
            row["Hex"] = "C9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Supercap Status";
            row["Description"] = "";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 202;
            row["Dec"] = "202";
            row["Hex"] = "CA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Exception Mode Status";
            row["Description"] = "";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 233;
            row["Dec"] = "233";
            row["Hex"] = "E9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Normalized Media Wear-out";
            row["Description"] = "";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 235;
            row["Dec"] = "235";
            row["Hex"] = "EB";
            row["IsCritical"] = false;
            row["AttributeName"] = "POR Recovery Count";
            row["Description"] = "Indicates the number of unsafe shutdowns from which the drive has had to recover. A shutdown is considered to be \"unsafe\" if the computer was powered down by any means other than a \"graceful\" operating system-initiated shutdown.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 241;
            row["Dec"] = "241";
            row["Hex"] = "F1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total LBAs Written";
            row["Description"] = "Indicates the total number of logical block addresses (LBAs) written.";
            ssdSamsungDefinitions.Rows.Add(row);

            row = ssdSamsungDefinitions.NewRow();
            row["Key"] = 242;
            row["Dec"] = "242";
            row["Hex"] = "F2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total LBAs Read";
            row["Description"] = "Indicates the total number of logical block addresses (LBAs) written.";
            ssdSamsungDefinitions.Rows.Add(row);

            ssdSamsungDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdSamsungDefinitions.PopulateSsdSamsungDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdSamsungDefinitions;
            }
        }
    }
}
