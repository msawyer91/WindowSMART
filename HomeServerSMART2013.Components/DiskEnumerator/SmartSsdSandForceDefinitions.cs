using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdSandForceDefinitions
    {
        DataTable ssdSandForceDefinitions;

        public SmartSsdSandForceDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdSandForceDefinitions");
            PopulateSsdSandForceDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdSandForceDefinitions");
        }

        public void PopulateSsdSandForceDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdSandForceDefinitions.PopulateSsdSandForceDataTable");
            ssdSandForceDefinitions = new DataTable("SsdSandForceSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdSandForceDefinitions.Columns.Add("Key", typeof(int));
            ssdSandForceDefinitions.Columns.Add("Dec", typeof(String));
            ssdSandForceDefinitions.Columns.Add("Hex", typeof(String));
            ssdSandForceDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdSandForceDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdSandForceDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdSandForceDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = false;
            row["AttributeName"] = "Raw Read Error Rate";
            row["Description"] = "Raw error related to ECC errors. Correctable and uncorrectable RAISE (Redundant Array of Independent Silicon Elements) errors are included in the error count.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Retired Block Count";
            row["Description"] = "Tracks the total number of retired blocks. Data is calculated as 100 – (100 x RBC : MRB), where RBC is the retired block count and MRB is the minimum required blocks. Also an indicator of remaining life.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Hours";
            row["Description"] = "Count of hours in power-on state. The raw value of this attribute shows total count of hours in power-on state.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Device Power Cycle Count";
            row["Description"] = "This attribute indicates the count of full hard disk power on/off cycles.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 13;
            row["Dec"] = "13";
            row["Hex"] = "0D";
            row["IsCritical"] = false;
            row["AttributeName"] = "Soft Read Error Rate";
            row["Description"] = "The number of corrected (CECC) read errors reported.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 100;
            row["Dec"] = "100";
            row["Hex"] = "64";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program/Erase Cycles";
            row["Description"] = "This attribute counts the number of flash program and erase cycles across the entire drive over the life of the drive.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 170;
            row["Dec"] = "170";
            row["Hex"] = "AA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Reserved Block Count";
            row["Description"] = "The number of reserved spares for bad block handling.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 171;
            row["Dec"] = "171";
            row["Hex"] = "AB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Block Count";
            row["Description"] = "Counts the number of flash program failures.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 172;
            row["Dec"] = "172";
            row["Hex"] = "AC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Block Count";
            row["Description"] = "Counts the number of flash erase failures.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 174;
            row["Dec"] = "174";
            row["Hex"] = "AE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unexpected Power Loss Count";
            row["Description"] = "Counts the number of unexpected power loss events since the drive was deployed.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 177;
            row["Dec"] = "177";
            row["Hex"] = "B1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Wear Range Delta";
            row["Description"] = "Returns the percent difference in wear between the most-worn block and least-worn block.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 181;
            row["Dec"] = "181";
            row["Hex"] = "B5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Count";
            row["Description"] = "Four bytes used to show the program failures since the drive was deployed (identical to attribute 171).";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 182;
            row["Dec"] = "182";
            row["Hex"] = "B6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Count";
            row["Description"] = "Four bytes used to show the number of block erase failures since the drive was deployed (identical to attribute 172).";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Reported I/O Error Detection Code Rate";
            row["Description"] = "I/O Error Detection Code rate. This attribute tracks the number of end-to-end CRC errors encountered during host initiated reads and writes.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 187;
            row["Dec"] = "187";
            row["Hex"] = "BB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Reported Uncorrectable Errors";
            row["Description"] = "This attribute tracks the umber of uncorrectable RAISE (Redundant Array of Independent Silicon Elements) errors reported back to the host for all data access commands.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "Temperature reported from the on-board sensor.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 195;
            row["Dec"] = "195";
            row["Hex"] = "C3";
            row["IsCritical"] = false;
            row["AttributeName"] = "On the Fly ECC Uncorrectable Error Count";
            row["Description"] = "This attribute tracks the number of uncorrectable errors.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 196;
            row["Dec"] = "196";
            row["Hex"] = "C4";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reallocation Event Count";
            row["Description"] = "This attribute tracks the number of blocks that fail programming which are reallocated as a result.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 198;
            row["Dec"] = "198";
            row["Hex"] = "C6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Uncorrectable Sector Count";
            row["Description"] = "Uncorrectable sector count relative to the number of sectors read this power cycle. The normalized value is only computed when the number of bits is greater than 1010. The count is cleared at power-on reset and wraps to 1010 when it exceeds 10^12.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 199;
            row["Dec"] = "199";
            row["Hex"] = "C7";
            row["IsCritical"] = false;
            row["AttributeName"] = "SATA R-Errors (CRC) Error Count";
            row["Description"] = "SATA-R Errors (CRC) Error Count.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 201;
            row["Dec"] = "201";
            row["Hex"] = "C9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Uncorrectable Soft Read Error Rate";
            row["Description"] = "Number of soft read errors that cannot be fixed on-the-fly and requires deep recovery via RAISE (Redundant Array of Independent Silicon Elements).";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 204;
            row["Dec"] = "204";
            row["Hex"] = "CC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Soft ECC Correction Rate";
            row["Description"] = "Number of errors corrected by RAISE (Redundant Array of Independent Silicon Elements) that cannot be fixed on-the-fly and requires ECC (multilevel) to correct.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 230;
            row["Dec"] = "230";
            row["Hex"] = "E6";
            row["IsCritical"] = true;
            row["AttributeName"] = "Life Curve Status";
            row["Description"] = "A life curve used to help predict life in terms of the endurance based on the number of writes to flash.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 231;
            row["Dec"] = "231";
            row["Hex"] = "E7";
            row["IsCritical"] = true;
            row["AttributeName"] = "SSD Life Left";
            row["Description"] = "A measure of the estimated life left, based on a combination of PE cycles and available reserve blocks. 100 is a new drive, 10 = replace as it has sufficient reserved blocks but the PE cycles have been used, 0 = insufficient reserved blocks, drive is read only to allow recovery of data on the drive.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 232;
            row["Dec"] = "232";
            row["Hex"] = "E8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Available Reserved Space";
            row["Description"] = "The number of reserved blocks remaining.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 235;
            row["Dec"] = "235";
            row["Hex"] = "EB";
            row["IsCritical"] = false;
            row["AttributeName"] = "SuperCap (Power Fail Backup) Health";
            row["Description"] = "Power Fail Backup Health is an estimation of capacitive hold-up capability based on a timed discharge test, wherein discharge (past a pre-defined voltage threshold) faster than a predefined time-value threshold indicates a capacitor bank whose capacitance value is degraded past the point of reliability to protect SSD data. If an SSD has never run a \"SuperCapacitor Test\", the normalized value of this Attribute remains at '100'.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 241;
            row["Dec"] = "241";
            row["Hex"] = "F1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Lifetime Writes from Host";
            row["Description"] = "Indicates the total amount of data written from hosts since the drive was deployed. This is stored in 4 bytes. The number stores represents the number of bytes written by the host to the drive, in 64 GB increments.";
            ssdSandForceDefinitions.Rows.Add(row);

            row = ssdSandForceDefinitions.NewRow();
            row["Key"] = 242;
            row["Dec"] = "242";
            row["Hex"] = "F2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Lifetime Reads from Host";
            row["Description"] = "Indicates the total amount of data read to hosts since the drive was deployed. This is stored in 4 bytes. The number stores represents the number of bytes read by the host to the drive, in 64 GB increments.";
            ssdSandForceDefinitions.Rows.Add(row);

            ssdSandForceDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdSandForceDefinitions.PopulateSsdSandForceDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdSandForceDefinitions;
            }
        }
    }
}
