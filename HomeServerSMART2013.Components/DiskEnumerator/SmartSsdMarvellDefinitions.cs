using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdMarvellDefinitions
    {
        DataTable ssdMarvellDefinitions;

        public SmartSsdMarvellDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdMarvellDefinitions");
            PopulateSsdMarvellDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdMarvellDefinitions");
        }

        public void PopulateSsdMarvellDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdMicronDefinitions.PopulateSsdMarvellDataTable");
            ssdMarvellDefinitions = new DataTable("SsdMarvellSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdMarvellDefinitions.Columns.Add("Key", typeof(int));
            ssdMarvellDefinitions.Columns.Add("Dec", typeof(String));
            ssdMarvellDefinitions.Columns.Add("Hex", typeof(String));
            ssdMarvellDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdMarvellDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdMarvellDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdMarvellDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = false;
            row["AttributeName"] = "Raw Read Error Rate";
            row["Description"] = "The data field holds the raw sum of correctable and uncorrectable ECC (error-correcting code) error events over the life of the drive. If this ever exceeds 4,294,967,295, this value will wrap around.";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Retired Sector Count";
            row["Description"] = "This Attribute indicates the cumulative number of retired blocks since leaving the factory. Bytes 5-10 contain the total number of runtime bad blocks.";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Hours";
            row["Description"] = "This value gives the raw number of hours that the device has been online over its life.";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle Count";
            row["Description"] = "This value gives the raw number of power cycle events that this drive has experienced.";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 170;
            row["Dec"] = "170";
            row["Hex"] = "AA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Grown Bad Blocks";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 171;
            row["Dec"] = "171";
            row["Hex"] = "AB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Count (Total)";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 172;
            row["Dec"] = "172";
            row["Hex"] = "AC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Count (Total)";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 173;
            row["Dec"] = "173";
            row["Hex"] = "AD";
            row["IsCritical"] = false;
            row["AttributeName"] = "Average Program/Erase Count (Total)";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 174;
            row["Dec"] = "174";
            row["Hex"] = "AE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unexpected Power Loss Count";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 175;
            row["Dec"] = "175";
            row["Hex"] = "AF";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Count (Worst Case)";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 176;
            row["Dec"] = "176";
            row["Hex"] = "B0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Count (Worst Case)";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 177;
            row["Dec"] = "177";
            row["Hex"] = "B1";
            row["IsCritical"] = true;
            row["AttributeName"] = "Wear Leveling Count";
            row["Description"] = "Indicates the worst-case erase count. Used as an indicator of SSD life remaining.";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 178;
            row["Dec"] = "178";
            row["Hex"] = "B2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Used Rserved Block Count (Worst Case)";
            row["Description"] = "Indicates the number of reserved blocks used to replace retired blocks.";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 179;
            row["Dec"] = "179";
            row["Hex"] = "B3";
            row["IsCritical"] = false;
            row["AttributeName"] = "Used Reserved Block Count (Total)";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 180;
            row["Dec"] = "180";
            row["Hex"] = "B4";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unused Reserved Block Count (Total)";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 181;
            row["Dec"] = "181";
            row["Hex"] = "B5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Count (Total)";
            row["Description"] = "Four bytes used to show the program failures since the drive was deployed.";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 182;
            row["Dec"] = "182";
            row["Hex"] = "B6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Count (Total)";
            row["Description"] = "Four bytes used to show the number of block erase failures since the drive was deployed.";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 183;
            row["Dec"] = "183";
            row["Hex"] = "B7";
            row["IsCritical"] = false;
            row["AttributeName"] = "SATA Interface Downshift";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = true;
            row["AttributeName"] = "End-to-End Data Errors Corrected";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 187;
            row["Dec"] = "187";
            row["Hex"] = "BB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Uncorrectable Error Count";
            row["Description"] = "This value is the total number of ECC (error-correcting code) correction failures reported by the sequencer.";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 188;
            row["Dec"] = "188";
            row["Hex"] = "BC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Command Timeout";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 192;
            row["Dec"] = "192";
            row["Hex"] = "C0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unsafe Shutdown Count";
            row["Description"] = "Indicates the number of unsafe shutdowns from which the drive has had to recover. A shutdown is considered to be \"unsafe\" if the computer was powered down by any means other than a \"graceful\" operating system-initiated shutdown.";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "Drive temperature.";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 195;
            row["Dec"] = "195";
            row["Hex"] = "C3";
            row["IsCritical"] = false;
            row["AttributeName"] = "ECC Rate";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 196;
            row["Dec"] = "196";
            row["Hex"] = "C4";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reallocation Event Count";
            row["Description"] = "This attribute tracks the number of blocks that fail programming which are reallocated as a result.";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 198;
            row["Dec"] = "198";
            row["Hex"] = "C6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Off-Line Scan Uncorrectable Sector Count";
            row["Description"] = "Uncorrectable errors detected during SMART off-line scan.";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 199;
            row["Dec"] = "199";
            row["Hex"] = "C7";
            row["IsCritical"] = false;
            row["AttributeName"] = "Ultra DMA CRC Error Count";
            row["Description"] = "Gives the number of captured FIS interface general CRC (cyclic redundancy check) errors over the life of the drive, for both reads and writes, since the most recent power cycle. If this counter ever reaches 4.294.967.295, it will wrap around.";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 232;
            row["Dec"] = "232";
            row["Hex"] = "E8";
            row["IsCritical"] = true;
            row["AttributeName"] = "Available Reserved Space";
            row["Description"] = "This attribute reports the number of reserve blocks remaining. The attribute value begins at 100, which indicates that the reserved space is 100% available. The threshold value for this attribute is 10% availability, which indicated that the drive is close to its end life.";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 170;
            row["Dec"] = "170";
            row["Hex"] = "E9";
            row["IsCritical"] = false;
            row["AttributeName"] = "NAND GB Written";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 241;
            row["Dec"] = "241";
            row["Hex"] = "F1";
            row["IsCritical"] = true;
            row["AttributeName"] = "Total LBA Written";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            row = ssdMarvellDefinitions.NewRow();
            row["Key"] = 242;
            row["Dec"] = "242";
            row["Hex"] = "F2";
            row["IsCritical"] = true;
            row["AttributeName"] = "Total LBA Read";
            row["Description"] = "";
            ssdMarvellDefinitions.Rows.Add(row);

            ssdMarvellDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdMicronDefinitions.PopulateSsdMarvellDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdMarvellDefinitions;
            }
        }
    }
}
