using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdTranscendDefinitions
    {
        DataTable ssdTranscendDefinitions;

        public SmartSsdTranscendDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdTranscendDefinitions");
            PopulateSsdTranscendDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdTranscendDefinitions");
        }

        public void PopulateSsdTranscendDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdTranscendDefinitions.PopulateSsdTranscendDataTable");
            ssdTranscendDefinitions = new DataTable("SsdTranscendSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdTranscendDefinitions.Columns.Add("Key", typeof(int));
            ssdTranscendDefinitions.Columns.Add("Dec", typeof(String));
            ssdTranscendDefinitions.Columns.Add("Hex", typeof(String));
            ssdTranscendDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdTranscendDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdTranscendDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdTranscendDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = false;
            row["AttributeName"] = "Read Error Rate";
            row["Description"] = "Count of raw data errors while data from media, including retry errors or data error (uncorrectable).";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Retired Sector Count";
            row["Description"] = "Count of reallocated blocks. This is the count of reallocated or remapped sectors during normal operation from the grown defects table.";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Time Count";
            row["Description"] = "Number of hours elapsed in the Power-On state.";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle Count";
            row["Description"] = "Number of power-on events.";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 160;
            row["Dec"] = "160";
            row["Hex"] = "A0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Uncorrectable Sectors Count (Read/Write)";
            row["Description"] = "Number of uncorrectable sectors that were detected on either read and/or write operations.";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 161;
            row["Dec"] = "161";
            row["Hex"] = "A1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Number of Valid Spare Blocks";
            row["Description"] = "Tracks the number of spare blocks remaining for reallocation of retired sectors.";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 162;
            row["Dec"] = "162";
            row["Hex"] = "A2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Number of Cache Data Block";
            row["Description"] = "";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 163;
            row["Dec"] = "163";
            row["Hex"] = "A3";
            row["IsCritical"] = false;
            row["AttributeName"] = "Number of Initial Invalid Blocks";
            row["Description"] = "Reports the number of sectors that were unusable when the disk left the factory.";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 164;
            row["Dec"] = "164";
            row["Hex"] = "A4";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Erase Count";
            row["Description"] = "";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 165;
            row["Dec"] = "165";
            row["Hex"] = "A5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Maximum Erase Count";
            row["Description"] = "";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 166;
            row["Dec"] = "166";
            row["Hex"] = "A6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Minimum Erase Count";
            row["Description"] = "";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 167;
            row["Dec"] = "167";
            row["Hex"] = "A7";
            row["IsCritical"] = false;
            row["AttributeName"] = "Average Erase Count";
            row["Description"] = "";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 168;
            row["Dec"] = "168";
            row["Hex"] = "A8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Max Erase Count of Spec";
            row["Description"] = "";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 169;
            row["Dec"] = "169";
            row["Hex"] = "A9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Percentage of Life Remaining";
            row["Description"] = "Gives the estimated lifetime % remaining.";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 192;
            row["Dec"] = "192";
            row["Hex"] = "C0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unsafe Shutdown Count";
            row["Description"] = "Reports the number of times the disk reported that the operating system was not shut down in a graceful manner (may indicate a blue screen or power outage).";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "Reports the temperature of the disk.";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 195;
            row["Dec"] = "195";
            row["Hex"] = "C3";
            row["IsCritical"] = false;
            row["AttributeName"] = "Hardware ECC Recovered";
            row["Description"] = "Tracks the number of correctable errors that were detected and corrected using error correction code.";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 196;
            row["Dec"] = "196";
            row["Hex"] = "C4";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reallocation Event Count";
            row["Description"] = "Total number of remapping events during normal operation and offline surface scanning.";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 199;
            row["Dec"] = "199";
            row["Hex"] = "C7";
            row["IsCritical"] = true;
            row["AttributeName"] = "Ultra DMA CRC Error Count";
            row["Description"] = "Reports the number of errors that were detected by the disk controller. Usually an indication of a faulty data cable.";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 241;
            row["Dec"] = "241";
            row["Hex"] = "F1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Host Writes (32MB increments)";
            row["Description"] = "";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 242;
            row["Dec"] = "242";
            row["Hex"] = "F2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Host Reads (32MB increments)";
            row["Description"] = "";
            ssdTranscendDefinitions.Rows.Add(row);

            row = ssdTranscendDefinitions.NewRow();
            row["Key"] = 245;
            row["Dec"] = "245";
            row["Hex"] = "F5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Flash Write Count (32MB increments)";
            row["Description"] = "";
            ssdTranscendDefinitions.Rows.Add(row);

            ssdTranscendDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdTranscendDefinitions.PopulateSsdTranscendDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdTranscendDefinitions;
            }
        }
    }
}
