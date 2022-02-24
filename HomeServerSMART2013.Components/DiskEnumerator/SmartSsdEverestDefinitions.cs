using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdEverestDefinitions
    {
        DataTable ssdEverestDefinitions;

        public SmartSsdEverestDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdEverestDefinitions");
            PopulateSsdEverestDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdEverestDefinitions");
        }

        public void PopulateSsdEverestDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdEverestDefinitions.PopulateSsdEverestDataTable");
            // Indilinx Everest controllers - different set of attributes than regular Indilinx
            ssdEverestDefinitions = new DataTable("SsdEverestSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdEverestDefinitions.Columns.Add("Key", typeof(int));
            ssdEverestDefinitions.Columns.Add("Dec", typeof(String));
            ssdEverestDefinitions.Columns.Add("Hex", typeof(String));
            ssdEverestDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdEverestDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdEverestDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdEverestDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = false;
            row["AttributeName"] = "Raw Read Error Rate";
            row["Description"] = "This Attribute indicates the internal raw read error rate based on the ratio of corrected error bits and total bits read. The raw read error rate has been normalized and can defined as follows:  RRER = (1/10^n); where n is the raw value returned in Byte 5. n can vary between a normalized max value of 12 (0Ch) down to a worst case value of 0. Byte 4 indicates the worst case normalized value of 0. Byte 3 can be ignored.";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 3;
            row["Dec"] = "03";
            row["Hex"] = "03";
            row["IsCritical"] = false;
            row["AttributeName"] = "Spin Up Time";
            row["Description"] = "This Attribute has a fixed value of 0 and is included to maintain compatibility with HDD SMART Attributes.";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 4;
            row["Dec"] = "04";
            row["Hex"] = "04";
            row["IsCritical"] = false;
            row["AttributeName"] = "Start/Stop Count";
            row["Description"] = "This Attribute has a fixed value of 0 and is included to maintain compatibility with HDD SMART Attributes.";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Retired Sector Count";
            row["Description"] = "This Attribute indicates the cumulative number of retired blocks since leaving the factory. Bytes 5-10 contain the total number of runtime bad blocks. Bytes 3-4 can be ignored.";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power-On Hours";
            row["Description"] = "This Attribute indicates the cumulative power-on hours of the SSD. Bytes 3-4 can be ignored. Bytes 5-10 contain the total number of hours.";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle Count";
            row["Description"] = "This Attribute indicates the cumulative number of power cycles to the SSD. Bytes 5-10 contain the total number of cycles. Bytes 3-4 can be ignored.";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 100;
            row["Dec"] = "100";
            row["Hex"] = "64";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Blocks Erased";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 167;
            row["Dec"] = "167";
            row["Hex"] = "A7";
            row["IsCritical"] = false;
            row["AttributeName"] = "SSD Protect Mode";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 168;
            row["Dec"] = "168";
            row["Hex"] = "A8";
            row["IsCritical"] = false;
            row["AttributeName"] = "SATA PHY Error Count";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 169;
            row["Dec"] = "169";
            row["Hex"] = "A9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Bad Block Count";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Factory Bad Block Count";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 192;
            row["Dec"] = "192";
            row["Hex"] = "C0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unexpected Power Loss Count";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "Drive temperature.";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 202;
            row["Dec"] = "202";
            row["Hex"] = "CA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Number of Corrected Bits";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 205;
            row["Dec"] = "205";
            row["Hex"] = "CD";
            row["IsCritical"] = false;
            row["AttributeName"] = "Maximum Rated P/E Counts";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 206;
            row["Dec"] = "206";
            row["Hex"] = "CE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Minimum Erase Counts";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 207;
            row["Dec"] = "207";
            row["Hex"] = "CF";
            row["IsCritical"] = false;
            row["AttributeName"] = "Maximum Erase Counts";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 211;
            row["Dec"] = "211";
            row["Hex"] = "D3";
            row["IsCritical"] = false;
            row["AttributeName"] = "SATA Uncorrectable Error Count";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 212;
            row["Dec"] = "212";
            row["Hex"] = "D4";
            row["IsCritical"] = false;
            row["AttributeName"] = "NAND Page Reads During Retry";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 213;
            row["Dec"] = "213";
            row["Hex"] = "D5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Simple Read Retry Attempts";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 214;
            row["Dec"] = "214";
            row["Hex"] = "D6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Adaptive Read Retry Attempts";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 221;
            row["Dec"] = "221";
            row["Hex"] = "DD";
            row["IsCritical"] = false;
            row["AttributeName"] = "Internal Data Path Uncorrectable Errors";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 222;
            row["Dec"] = "222";
            row["Hex"] = "DE";
            row["IsCritical"] = false;
            row["AttributeName"] = "RAID Recovery Count";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 230;
            row["Dec"] = "230";
            row["Hex"] = "E6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Loss Protection";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 232;
            row["Dec"] = "232";
            row["Hex"] = "E8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Count of Write Sectors";
            row["Description"] = "This Attribute indicates the cumulative number of 512B sectors written by the host. Bytes 5-10 contain the total number of sectors written by the host. Bytes 3-4 can be ignored.";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 233;
            row["Dec"] = "233";
            row["Hex"] = "E9";
            row["IsCritical"] = true;
            row["AttributeName"] = "Remaining Life (Wearout Indicator)";
            row["Description"] = "This Attribute indicates the percentage of \"remaining life\" of the SSD based on the endurance specification of the NAND Flash and the number of retired blocks. Byte 3 contains the initial normalized value of 100 (64h), which indicates 100% life remaining. Byte 4 contains the worst case normalized value of 0. Byte 5 contains the current percent of life remaining, which has an initial value of 100 (64h) and then decrements by units of 1 down to a worst case value of 0.";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 241;
            row["Dec"] = "241";
            row["Hex"] = "F1";
            row["IsCritical"] = true;
            row["AttributeName"] = "Total Host Writes";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 242;
            row["Dec"] = "242";
            row["Hex"] = "F2";
            row["IsCritical"] = true;
            row["AttributeName"] = "Total Host Reads";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            row = ssdEverestDefinitions.NewRow();
            row["Key"] = 251;
            row["Dec"] = "251";
            row["Hex"] = "FB";
            row["IsCritical"] = false;
            row["AttributeName"] = "NAND Read Count";
            row["Description"] = "";
            ssdEverestDefinitions.Rows.Add(row);

            ssdEverestDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdEverestDefinitions.PopulateSsdEverestDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdEverestDefinitions;
            }
        }
    }
}
