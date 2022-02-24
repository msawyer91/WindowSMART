using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdMicronDefinitions
    {
        DataTable ssdMicronDefinitions;

        public SmartSsdMicronDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdMicronDefinitions");
            PopulateSsdMicronDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdMicronDefinitions");
        }

        public void PopulateSsdMicronDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdMicronDefinitions.PopulateSsdMicronDataTable");
            ssdMicronDefinitions = new DataTable("SsdMicronSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdMicronDefinitions.Columns.Add("Key", typeof(int));
            ssdMicronDefinitions.Columns.Add("Dec", typeof(String));
            ssdMicronDefinitions.Columns.Add("Hex", typeof(String));
            ssdMicronDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdMicronDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdMicronDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdMicronDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = false;
            row["AttributeName"] = "Raw Read Error Rate";
            row["Description"] = "The data field holds the raw sum of correctable and uncorrectable ECC (error-correcting code) error events over the life of the drive. If this ever exceeds 4,294,967,295, this value will wrap around.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Retired Block Count";
            row["Description"] = "This value gives the total bad block count of the drive minus the number of one-time programmable (OTP) bad blocks.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Hours";
            row["Description"] = "This value gives the raw number of hours that the device has been online over its life.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle Count";
            row["Description"] = "This value gives the raw number of power cycle events that this drive has experienced.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 13;
            row["Dec"] = "13";
            row["Hex"] = "0D";
            row["IsCritical"] = false;
            row["AttributeName"] = "Soft Error Rate";
            row["Description"] = "";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 14;
            row["Dec"] = "14";
            row["Hex"] = "0E";
            row["IsCritical"] = false;
            row["AttributeName"] = "Device Capacity (NAND)";
            row["Description"] = "";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 15;
            row["Dec"] = "15";
            row["Hex"] = "0F";
            row["IsCritical"] = false;
            row["AttributeName"] = "User Capacity";
            row["Description"] = "";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 16;
            row["Dec"] = "16";
            row["Hex"] = "10";
            row["IsCritical"] = false;
            row["AttributeName"] = "Spare Blocks Available";
            row["Description"] = "";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 17;
            row["Dec"] = "17";
            row["Hex"] = "11";
            row["IsCritical"] = false;
            row["AttributeName"] = "Remaining Spare Blocks";
            row["Description"] = "";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 100;
            row["Dec"] = "100";
            row["Hex"] = "64";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Erase Count";
            row["Description"] = "";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 170;
            row["Dec"] = "170";
            row["Hex"] = "AA";
            row["IsCritical"] = true;
            row["AttributeName"] = "Grown Failing Block Count";
            row["Description"] = "This attribute tracks the number of blocks/pages utilized to replace bad blocks. This is based on specified NAND part/die";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 171;
            row["Dec"] = "171";
            row["Hex"] = "AB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Count";
            row["Description"] = "This value contains the raw number of flash program failure events. If this value ever would exceed 65,535 (FFFF), it will stay at 65,535.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 172;
            row["Dec"] = "172";
            row["Hex"] = "AC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Count";
            row["Description"] = "This value contains the raw number of flash erase failure events. If this value ever would exceed 65,535 (FFFF), it will stay at 65,535.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 173;
            row["Dec"] = "173";
            row["Hex"] = "AD";
            row["IsCritical"] = true;
            row["AttributeName"] = "Wear Leveling Count";
            row["Description"] = "Average erase count of all good blocks.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 174;
            row["Dec"] = "174";
            row["Hex"] = "AE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unexpected Power Loss Count";
            row["Description"] = "Counts the number of unexpected power loss events since the drive was deployed.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 180;
            row["Dec"] = "180";
            row["Hex"] = "B4";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unused Reserve NAND Blocks";
            row["Description"] = "Number of remaining spare flash blocks on the SSD.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 181;
            row["Dec"] = "181";
            row["Hex"] = "B5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Non-4K Aligned Access Count";
            row["Description"] = "This attribute contains two values: the low order 16 bits of the raw data contain the total unaligned reads counter, divided by 60,000, with a ceiling value of 65,535; the high order 16 bits of the raw data contain the total unaligned writes counter, divided by 60,000, with a ceiling value of 65,535.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 183;
            row["Dec"] = "183";
            row["Hex"] = "B7";
            row["IsCritical"] = false;
            row["AttributeName"] = "SATA Interface Downshift";
            row["Description"] = "Four bytes used to show the number of block erase failures since the drive was deployed (identical to attribute 172).";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Error Correction Count";
            row["Description"] = "Count of end-to-end error corrections.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 187;
            row["Dec"] = "187";
            row["Hex"] = "BB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Reported Uncorrectable Errors";
            row["Description"] = "This value is the total number of ECC (error-correcting code) correction failures reported by the sequencer.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 188;
            row["Dec"] = "188";
            row["Hex"] = "BC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Command Timeouts";
            row["Description"] = "This value is the total number of ECC (error-correcting code) correction failures reported by the sequencer.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 189;
            row["Dec"] = "189";
            row["Hex"] = "BD";
            row["IsCritical"] = false;
            row["AttributeName"] = "Factory Bad Block Count";
            row["Description"] = "One-time programmable (OTP) bad block count.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Enclosure Temperature";
            row["Description"] = "Reports the temperature inside the SSD enclosure.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 195;
            row["Dec"] = "195";
            row["Hex"] = "C3";
            row["IsCritical"] = false;
            row["AttributeName"] = "Cumulative ECC Bit Correction Count";
            row["Description"] = "";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 196;
            row["Dec"] = "196";
            row["Hex"] = "C4";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reallocation Event Count";
            row["Description"] = "This value gives the total bad block count of the drive minus the number of one-time programmable (OTP) bad blocks.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 197;
            row["Dec"] = "197";
            row["Hex"] = "C5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Current Pending Sector Count";
            row["Description"] = "Will always be 0 because reallocation will be done on the fly.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 198;
            row["Dec"] = "198";
            row["Hex"] = "C6";
            row["IsCritical"] = false;
            row["AttributeName"] = "SMART Off-Line Scan Uncorrectable Error Count";
            row["Description"] = "Uncorrectable errors detected during SMART off-line scan.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 199;
            row["Dec"] = "199";
            row["Hex"] = "C7";
            row["IsCritical"] = false;
            row["AttributeName"] = "Ultra DMA CRC Error Rate";
            row["Description"] = "Gives the number of captured FIS interface general CRC (cyclic redundancy check) errors over the life of the drive, for both reads and writes, since the most recent power cycle. If this counter ever reaches 4.294.967.295, it will wrap around.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 202;
            row["Dec"] = "202";
            row["Hex"] = "CA";
            row["IsCritical"] = true;
            row["AttributeName"] = "Percentage of the Rated Lifetime Used";
            row["Description"] = "The average erase count of all blocks on Channel 0 CE 0 divided by the specified MaxEraseCount (10k for MLC or 100k for SLC), reported as a percentage from 0 to 100%.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 206;
            row["Dec"] = "206";
            row["Hex"] = "CE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Write Error Rate";
            row["Description"] = "Report the number of program failures divided by 60,000. If the value ever reaches 65,535, it will remain there thereafter.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 210;
            row["Dec"] = "210";
            row["Hex"] = "D2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Successful RAIN Recovery Count";
            row["Description"] = "The total number of translation units (TUs) successfully recovered by Micron's redundant array of independent NAND (RAIN) technology.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 234;
            row["Dec"] = "234";
            row["Hex"] = "EA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Bytes Read";
            row["Description"] = "";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 242;
            row["Dec"] = "242";
            row["Hex"] = "F2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Write Protect Progress";
            row["Description"] = "";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 243;
            row["Dec"] = "243";
            row["Hex"] = "F3";
            row["IsCritical"] = false;
            row["AttributeName"] = "ECC Bits Corrected";
            row["Description"] = "";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 244;
            row["Dec"] = "244";
            row["Hex"] = "F4";
            row["IsCritical"] = false;
            row["AttributeName"] = "ECC Cumulative Threshold Events";
            row["Description"] = "";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 245;
            row["Dec"] = "245";
            row["Hex"] = "F5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Cumulative Program NAND Pages";
            row["Description"] = "";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 246;
            row["Dec"] = "246";
            row["Hex"] = "F6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Cumulative Host Sector Writes";
            row["Description"] = "The total number of sectors written by the host.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 247;
            row["Dec"] = "247";
            row["Hex"] = "F7";
            row["IsCritical"] = false;
            row["AttributeName"] = "Host Program NAND Pages Count";
            row["Description"] = "This value stores the cumulative host program NAND page count.";
            ssdMicronDefinitions.Rows.Add(row);

            row = ssdMicronDefinitions.NewRow();
            row["Key"] = 248;
            row["Dec"] = "248";
            row["Hex"] = "F8";
            row["IsCritical"] = false;
            row["AttributeName"] = "FTL Program NAND Pages Count";
            row["Description"] = "This value stores the cumulative FTL program page count. This attribute tracks the number of NAND pages programmed by the FTL which are in addition to operations programmed by the host.";
            ssdMicronDefinitions.Rows.Add(row);

            ssdMicronDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdMicronDefinitions.PopulateSsdMicronDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdMicronDefinitions;
            }
        }
    }
}
