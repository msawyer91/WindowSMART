using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdBarefoot3Definitions
    {
        DataTable ssdBarefoot3Definitions;

        public SmartSsdBarefoot3Definitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdBarefoot3Definitions");
            PopulateSsdBarefoot3DataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdBarefoot3Definitions");
        }

        public void PopulateSsdBarefoot3DataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdEverestDefinitions.PopulateSsdBarefoot3DataTable");
            // Indilinx Everest controllers - different set of attributes than regular Indilinx
            ssdBarefoot3Definitions = new DataTable("SsdBarefoot3SMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdBarefoot3Definitions.Columns.Add("Key", typeof(int));
            ssdBarefoot3Definitions.Columns.Add("Dec", typeof(String));
            ssdBarefoot3Definitions.Columns.Add("Hex", typeof(String));
            ssdBarefoot3Definitions.Columns.Add("IsCritical", typeof(bool));
            ssdBarefoot3Definitions.Columns.Add("AttributeName", typeof(String));
            ssdBarefoot3Definitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdBarefoot3Definitions.NewRow();

            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Accumulated Runtime Bad Blocks";
            row["Description"] = "This Attribute indicates the cumulative number of retired blocks since leaving the factory. The SSD retires blocks if they cannot be successfully written.";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power-On Hours Count";
            row["Description"] = "This Attribute indicates the cumulative power-on hours of the SSD. Bytes 3-4 can be ignored. Bytes 5-10 contain the total number of hours.";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle Count";
            row["Description"] = "This Attribute indicates the cumulative number of power cycles to the SSD. Bytes 5-10 contain the total number of cycles. Bytes 3-4 can be ignored.";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 64;
            row["Dec"] = "64";
            row["Hex"] = "100";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Blocks Erased";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 171;
            row["Dec"] = "171";
            row["Hex"] = "AB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Available Over-Provisioned Block Count";
            row["Description"] = "This Attribute indicates the number of Over-Provisioning blocks are available. Over-Provisioning helps improve drive life and performance.";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 174;
            row["Dec"] = "174";
            row["Hex"] = "AE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unplanned Power Cycle Count";
            row["Description"] = "This Attribute indicates the number of times the SSD unexpectedly lost power. This typically occurs if the computer is powered off without properly shutting down the operating system. This can also occur if the SSD is disconnected without performing a Safely Remove operation first.";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Factory Bad Block Count Total";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 195;
            row["Dec"] = "195";
            row["Hex"] = "C3";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Programming Failures";
            row["Description"] = "This Attribute indicates the number of times the SSD failed to write data to a block. Writing (or programming) is a separate operation from erasing, which is tracked in the next attribute.";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 196;
            row["Dec"] = "196";
            row["Hex"] = "C4";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Erase Failures";
            row["Description"] = "This Attribute indicates the number of times the SSD failed to erase a block. SSDs cannot simply overwrite existing data in a block; the block must be erased first. This attribute tracks erase failures; the previous attribute tracks write failures.";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 197;
            row["Dec"] = "197";
            row["Hex"] = "C5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Read Failures (Uncorrectable)";
            row["Description"] = "This Attribute indicates the number of times the SSD failed to read the data from a block, and error correction also failed to recover the data. Thus the data was likely lost.";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 198;
            row["Dec"] = "198";
            row["Hex"] = "C6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Host Reads";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 199;
            row["Dec"] = "199";
            row["Hex"] = "C7";
            row["IsCritical"] = false;
            row["AttributeName"] = "Host Writes";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 202;
            row["Dec"] = "202";
            row["Hex"] = "CA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Number of Read Bits Corrected";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 205;
            row["Dec"] = "205";
            row["Hex"] = "CD";
            row["IsCritical"] = false;
            row["AttributeName"] = "Maximum Rated P/E Count";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 206;
            row["Dec"] = "206";
            row["Hex"] = "CE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Minimum Erase Count";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 207;
            row["Dec"] = "207";
            row["Hex"] = "CF";
            row["IsCritical"] = false;
            row["AttributeName"] = "Maximum Erase Count";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 208;
            row["Dec"] = "208";
            row["Hex"] = "D0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Average Erase Count";
            row["Description"] = "This Attribute indicates the average number of times each block has gone through a P/E (program/erase) cycle. Used as a means of determining life remaining.";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 210;
            row["Dec"] = "210";
            row["Hex"] = "D2";
            row["IsCritical"] = false;
            row["AttributeName"] = "SATA CRC Error Count";
            row["Description"] = "This Attribute indicates the number of SATA CRC errors that were detected and corrected while reading or writing from the SSD. This is usually an indication of a loose or damaged data cable.";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 212;
            row["Dec"] = "212";
            row["Hex"] = "D4";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Count NAND Page Reads Requiring Read Retry";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 213;
            row["Dec"] = "213";
            row["Hex"] = "D5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Count of Simple Read Retry Attempts";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 214;
            row["Dec"] = "214";
            row["Hex"] = "D6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Count of Adaptive Read Retry Attempts";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 221;
            row["Dec"] = "221";
            row["Hex"] = "DD";
            row["IsCritical"] = false;
            row["AttributeName"] = "Internal Data Path Protection Uncorrectable Errors";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 222;
            row["Dec"] = "222";
            row["Hex"] = "DE";
            row["IsCritical"] = false;
            row["AttributeName"] = "RAID Recovery Count";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 224;
            row["Dec"] = "224";
            row["Hex"] = "E0";
            row["IsCritical"] = false;
            row["AttributeName"] = "In Warranty";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 225;
            row["Dec"] = "225";
            row["Hex"] = "E1";
            row["IsCritical"] = false;
            row["AttributeName"] = "DAS Polarity";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 226;
            row["Dec"] = "226";
            row["Hex"] = "E2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Partial P-fail";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 230;
            row["Dec"] = "230";
            row["Hex"] = "E6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Write Throttling Activation Flag";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 231;
            row["Dec"] = "231";
            row["Hex"] = "E7";
            row["IsCritical"] = false;
            row["AttributeName"] = "Life Left (MWI)";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 233;
            row["Dec"] = "233";
            row["Hex"] = "E9";
            row["IsCritical"] = true;
            row["AttributeName"] = "Lifetime Remaining (Wearout Indicator)";
            row["Description"] = "This Attribute indicates the percentage of \"remaining life\" of the SSD based on the endurance specification of the NAND Flash and the number of retired blocks. Byte 3 contains the initial normalized value of 100 (64h), which indicates 100% life remaining. Byte 4 contains the worst case normalized value of 0. Byte 5 contains the current percent of life remaining, which has an initial value of 100 (64h) and then decrements by units of 1 down to a worst case value of 0.";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 241;
            row["Dec"] = "241";
            row["Hex"] = "F1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Host Writes (GB)";
            row["Description"] = "This Attribute indicates the number of gigabytes written to the SSD.";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 242;
            row["Dec"] = "242";
            row["Hex"] = "F2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Host Reads (GB)";
            row["Description"] = "This Attribute indicates the number of gigabytes read from the SSD.";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 249;
            row["Dec"] = "249";
            row["Hex"] = "F9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total NAND Programming Count (GB)";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            row = ssdBarefoot3Definitions.NewRow();
            row["Key"] = 249;
            row["Dec"] = "249";
            row["Hex"] = "F9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total NAND Read Count";
            row["Description"] = "";
            ssdBarefoot3Definitions.Rows.Add(row);

            ssdBarefoot3Definitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdEverestDefinitions.PopulateSsdBarefoot3DataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdBarefoot3Definitions;
            }
        }
    }
}