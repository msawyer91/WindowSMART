using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdIndilinxDefinitions
    {
        DataTable ssdIndilinxDefinitions;

        public SmartSsdIndilinxDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdIndilinxDefinitions");
            PopulateSsdIndilinxDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdIndilinxDefinitions");
        }

        public void PopulateSsdIndilinxDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdIndilinxDefinitions.PopulateSsdIndilinxDataTable");
            ssdIndilinxDefinitions = new DataTable("SsdIndilinxSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdIndilinxDefinitions.Columns.Add("Key", typeof(int));
            ssdIndilinxDefinitions.Columns.Add("Dec", typeof(String));
            ssdIndilinxDefinitions.Columns.Add("Hex", typeof(String));
            ssdIndilinxDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdIndilinxDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdIndilinxDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdIndilinxDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = false;
            row["AttributeName"] = "Raw Read Error Rate";
            row["Description"] = "Raw Read Error Rate";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Hours";
            row["Description"] = "Count of hours in power-on state. The raw value of this attribute shows total count of hours in power-on state.";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle Count";
            row["Description"] = "This attribute indicates the count of full hard disk power on/off cycles.";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Initial Bad Block Count";
            row["Description"] = "The number of bad blocks present on the drive at time of manufacturing.";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 195;
            row["Dec"] = "195";
            row["Hex"] = "C3";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Failure Block Count";
            row["Description"] = "Program failures since the drive was deployed.";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 196;
            row["Dec"] = "196";
            row["Hex"] = "C4";
            row["IsCritical"] = true;
            row["AttributeName"] = "Erase Failure Block Count";
            row["Description"] = "Program failures since the drive was deployed.";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 197;
            row["Dec"] = "197";
            row["Hex"] = "C5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Read Failure Block Count (Uncorrectable)";
            row["Description"] = "Uncorrectable read failures since the drive was deployed.";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 198;
            row["Dec"] = "198";
            row["Hex"] = "C6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Count of Read Sectors";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 199;
            row["Dec"] = "199";
            row["Hex"] = "C7";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Count of Write Sectors";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 200;
            row["Dec"] = "200";
            row["Hex"] = "C8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Count of Read Commands";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 201;
            row["Dec"] = "201";
            row["Hex"] = "C9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Count of Write Commands";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 202;
            row["Dec"] = "202";
            row["Hex"] = "CA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Count of Error Bits from Flash";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 203;
            row["Dec"] = "203";
            row["Hex"] = "CB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Count of Read Sectors with Correctable Errors";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 204;
            row["Dec"] = "204";
            row["Hex"] = "CC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Bad Block Full Flag";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 205;
            row["Dec"] = "205";
            row["Hex"] = "CD";
            row["IsCritical"] = false;
            row["AttributeName"] = "Maximum PE Count Specification";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 206;
            row["Dec"] = "206";
            row["Hex"] = "CE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Minimum Erase Count";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 207;
            row["Dec"] = "207";
            row["Hex"] = "CF";
            row["IsCritical"] = false;
            row["AttributeName"] = "Maximum Erase Count";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 208;
            row["Dec"] = "208";
            row["Hex"] = "D0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Average Erase Count";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 209;
            row["Dec"] = "209";
            row["Hex"] = "D1";
            row["IsCritical"] = true;
            row["AttributeName"] = "Remaining Life (%)";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 210;
            row["Dec"] = "210";
            row["Hex"] = "D2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Reserved";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 211;
            row["Dec"] = "211";
            row["Hex"] = "D3";
            row["IsCritical"] = false;
            row["AttributeName"] = "SATA Error Count CRC";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 212;
            row["Dec"] = "212";
            row["Hex"] = "D4";
            row["IsCritical"] = false;
            row["AttributeName"] = "SATA Error Count Handshake";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            row = ssdIndilinxDefinitions.NewRow();
            row["Key"] = 213;
            row["Dec"] = "213";
            row["Hex"] = "D5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Reserved";
            row["Description"] = "";
            ssdIndilinxDefinitions.Rows.Add(row);

            ssdIndilinxDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdIndilinxDefinitions.PopulateSsdIndilinxDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdIndilinxDefinitions;
            }
        }
    }
}
