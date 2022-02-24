using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdJMicronDefinitions
    {
        DataTable ssdJMicronDefinitions;

        public SmartSsdJMicronDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdJMicronDefinitions");
            PopulateSsdJMicronDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdJMicronDefinitions");
        }

        public void PopulateSsdJMicronDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdJMicronDefinitions.PopulateSsdJMicronDataTable");
            ssdJMicronDefinitions = new DataTable("SsdJMicronSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdJMicronDefinitions.Columns.Add("Key", typeof(int));
            ssdJMicronDefinitions.Columns.Add("Dec", typeof(String));
            ssdJMicronDefinitions.Columns.Add("Hex", typeof(String));
            ssdJMicronDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdJMicronDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdJMicronDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdJMicronDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = false;
            row["AttributeName"] = "Read Error Rate";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 2;
            row["Dec"] = "02";
            row["Hex"] = "02";
            row["IsCritical"] = false;
            row["AttributeName"] = "Throughput Performance";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 3;
            row["Dec"] = "03";
            row["Hex"] = "03";
            row["IsCritical"] = false;
            row["AttributeName"] = "Spin-Up Time";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reallocated Sector Count";
            row["Description"] = "Tracks the total number of retired sectors. Also an indicator of remaining life.";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 7;
            row["Dec"] = "07";
            row["Hex"] = "07";
            row["IsCritical"] = false;
            row["AttributeName"] = "Seek Error Rate";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 8;
            row["Dec"] = "08";
            row["Hex"] = "08";
            row["IsCritical"] = false;
            row["AttributeName"] = "Seek Time Performance";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Hours Count";
            row["Description"] = "Count of hours in power-on state. The raw value of this attribute shows total count of hours in power-on state.";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 10;
            row["Dec"] = "10";
            row["Hex"] = "0A";
            row["IsCritical"] = true;
            row["AttributeName"] = "Spin Retry Count";
            row["Description"] = "Although an SSD contains no moving parts, this attribute indicates the number of times the SSD failed to start when power was applied. This can be an indicator of excessive power draw or a failing onboard power supply.";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Drive Power Cycle Count";
            row["Description"] = "This attribute indicates the count of full disk power on/off cycles.";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 167;
            row["Dec"] = "167";
            row["Hex"] = "A7";
            row["IsCritical"] = false;
            row["AttributeName"] = "Vendor-Specific Attribute";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 168;
            row["Dec"] = "168";
            row["Hex"] = "A8";
            row["IsCritical"] = false;
            row["AttributeName"] = "SATA PHY Error Count";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 169;
            row["Dec"] = "169";
            row["Hex"] = "A9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Vendor-Specific Attribute";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 170;
            row["Dec"] = "170";
            row["Hex"] = "AA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Bad Block Count";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 173;
            row["Dec"] = "173";
            row["Hex"] = "AD";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Count";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 175;
            row["Dec"] = "175";
            row["Hex"] = "AF";
            row["IsCritical"] = true;
            row["AttributeName"] = "Bad Cluster Table Count (ECC Fail Count)";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 192;
            row["Dec"] = "192";
            row["Hex"] = "C0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unexpected Power Loss Count";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "Temperature reported from the on-board sensor.";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 197;
            row["Dec"] = "197";
            row["Hex"] = "C5";
            row["IsCritical"] = true;
            row["AttributeName"] = "Current Pending Sector Counter";
            row["Description"] = "Shows the current count of \"unstable\" sectors, which may be retired on a subsequent write attempt.";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 229;
            row["Dec"] = "229";
            row["Hex"] = "E5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Halt System, Flash ID";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 232;
            row["Dec"] = "232";
            row["Hex"] = "E8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Firmware Version Information";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 233;
            row["Dec"] = "233";
            row["Hex"] = "E8";
            row["IsCritical"] = false;
            row["AttributeName"] = "ECC Fail Record";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 234;
            row["Dec"] = "234";
            row["Hex"] = "EA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Average Erase Count, Max Erase Count";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 235;
            row["Dec"] = "235";
            row["Hex"] = "EB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Good Block Count, System Block Count";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            row = ssdJMicronDefinitions.NewRow();
            row["Key"] = 240;
            row["Dec"] = "240";
            row["Hex"] = "F0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Write Head";
            row["Description"] = "";
            ssdJMicronDefinitions.Rows.Add(row);

            ssdJMicronDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdJMicronDefinitions.PopulateSsdJMicronDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdJMicronDefinitions;
            }
        }
    }
}
