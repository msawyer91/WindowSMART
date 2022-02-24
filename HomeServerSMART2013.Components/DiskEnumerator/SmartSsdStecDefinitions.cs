using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdStecDefinitions
    {
        DataTable ssdStecDefinitions;

        public SmartSsdStecDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartStecDefinitions");
            PopulateSsdStecDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartStecDefinitions");
        }

        public void PopulateSsdStecDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartStecDefinitions.PopulateSsdStecDataTable");
            ssdStecDefinitions = new DataTable("SsdStecSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdStecDefinitions.Columns.Add("Key", typeof(int));
            ssdStecDefinitions.Columns.Add("Dec", typeof(String));
            ssdStecDefinitions.Columns.Add("Hex", typeof(String));
            ssdStecDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdStecDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdStecDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdStecDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = false;
            row["AttributeName"] = "Raw Read Error";
            row["Description"] = "Count of raw data errors while data from media, including retry errors or data error (uncorrectable).";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 2;
            row["Dec"] = "02";
            row["Hex"] = "02";
            row["IsCritical"] = true;
            row["AttributeName"] = "Throughput Performance";
            row["Description"] = "Internally measured average and worst data transfer rate.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Retired Sector Count";
            row["Description"] = "Count of reallocated blocks. This is the count of reallocated or remapped sectors during normal operation from the grown defects table.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Hours";
            row["Description"] = "Number of hours elapsed in the Power-On state.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle";
            row["Description"] = "Number of power-on events.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 13;
            row["Dec"] = "13";
            row["Hex"] = "0D";
            row["IsCritical"] = false;
            row["AttributeName"] = "Soft Read Error Rate";
            row["Description"] = "Number of corrected read errors reported to the operating system.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 100;
            row["Dec"] = "100";
            row["Hex"] = "64";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase/Program Cycles";
            row["Description"] = "Count of erase program cycles for entire card.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 103;
            row["Dec"] = "103";
            row["Hex"] = "67";
            row["IsCritical"] = false;
            row["AttributeName"] = "Translation Table Rebuild";
            row["Description"] = "Power backup fault or internal error resulting in loss of system unit tables.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 170;
            row["Dec"] = "170";
            row["Hex"] = "AA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Reserved Block Count";
            row["Description"] = "The number of reserved spares for bad block handling.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 171;
            row["Dec"] = "171";
            row["Hex"] = "AB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Count";
            row["Description"] = "Count of flash program failures.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 172;
            row["Dec"] = "172";
            row["Hex"] = "AC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Count";
            row["Description"] = "Count of flash erase command failures.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 174;
            row["Dec"] = "174";
            row["Hex"] = "AE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unexpected Power Loss";
            row["Description"] = "Attribute counts number of unexpected power loss events.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 177;
            row["Dec"] = "177";
            row["Hex"] = "B1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Wear Leveling Count";
            row["Description"] = "Worst case erase count.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 178;
            row["Dec"] = "178";
            row["Hex"] = "B2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unexpected Power Loss";
            row["Description"] = "This attribute is used to count the number of unexpected power loss events.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 180;
            row["Dec"] = "180";
            row["Hex"] = "B4";
            row["IsCritical"] = false;
            row["AttributeName"] = "Reserved Block Count";
            row["Description"] = "Number of reserved spares for bad block handling.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 181;
            row["Dec"] = "181";
            row["Hex"] = "B5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Count";
            row["Description"] = "Count of flash program failures.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 182;
            row["Dec"] = "182";
            row["Hex"] = "B6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Count";
            row["Description"] = "Count of flash erase command failures.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 183;
            row["Dec"] = "183";
            row["Hex"] = "B7";
            row["IsCritical"] = false;
            row["AttributeName"] = "Runtime Bad Block";
            row["Description"] = "Total number of data blocks with detected, uncorrectable errors encountered during normal operation.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = false;
            row["AttributeName"] = "End-to-End Error Detection";
            row["Description"] = "Tracks the number of end to end internal card data path errors that were detected.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 187;
            row["Dec"] = "187";
            row["Hex"] = "BB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Reported Uncorrectable Errors";
            row["Description"] = "Number of uncorrectable errors reported at the interface.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 188;
            row["Dec"] = "188";
            row["Hex"] = "B9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Command Timeout";
            row["Description"] = "Tracks the number of command time outs as defined by an active command being interrupted.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "Temperature of the base casting.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 195;
            row["Dec"] = "195";
            row["Hex"] = "C3";
            row["IsCritical"] = false;
            row["AttributeName"] = "Hardware ECC Recovered";
            row["Description"] = "Count of ECC errors encountered and corrected using hardware error correction.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 196;
            row["Dec"] = "196";
            row["Hex"] = "C4";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reallocation Event";
            row["Description"] = "Total number of remapping events during normal operation and offline surface scanning.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 197;
            row["Dec"] = "197";
            row["Hex"] = "C5";
            row["IsCritical"] = true;
            row["AttributeName"] = "Current Pending Sector Count";
            row["Description"] = "Number of blocks marked suspect due to uncorrectable errors.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 198;
            row["Dec"] = "198";
            row["Hex"] = "C6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Offline Surface Scan";
            row["Description"] = "Number of uncorrected errors that occurred during offline scan.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 199;
            row["Dec"] = "199";
            row["Hex"] = "C7";
            row["IsCritical"] = false;
            row["AttributeName"] = "UDMA/SATA CRC Error";
            row["Description"] = "Number of CRC errors during UDMA or SATA mode.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 200;
            row["Dec"] = "200";
            row["Hex"] = "C8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Average Wear Leveling";
            row["Description"] = "Average number of block erase counts for all blocks on the solid state drive.";
            ssdStecDefinitions.Rows.Add(row);

            row = ssdStecDefinitions.NewRow();
            row["Key"] = 233;
            row["Dec"] = "233";
            row["Hex"] = "E8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Number of Writes Count";
            row["Description"] = "Count of write commands originating from the host side.";
            ssdStecDefinitions.Rows.Add(row);

            ssdStecDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartStecDefinitions.PopulateSsdStecDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdStecDefinitions;
            }
        }
    }
}
