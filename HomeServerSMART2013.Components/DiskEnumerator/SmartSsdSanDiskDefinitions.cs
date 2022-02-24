using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdSanDiskDefinitions
    {
        DataTable ssdSanDiskDefinitions;

        public SmartSsdSanDiskDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdSanDiskDefinitions");
            PopulateSsdSanDiskDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdSanDiskDefinitions");
        }

        public void PopulateSsdSanDiskDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdSanDiskDefinitions.PopulateSanDiskDataTable");
            ssdSanDiskDefinitions = new DataTable("SsdSanDiskSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdSanDiskDefinitions.Columns.Add("Key", typeof(int));
            ssdSanDiskDefinitions.Columns.Add("Dec", typeof(String));
            ssdSanDiskDefinitions.Columns.Add("Hex", typeof(String));
            ssdSanDiskDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdSanDiskDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdSanDiskDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdSanDiskDefinitions.NewRow();

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reassigned Sector Count";
            row["Description"] = "Count of reallocated blocks. This is the count of reallocated or remapped sectors during normal operation from the grown defects table.";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Hours";
            row["Description"] = "Number of hours elapsed in the Power-On state.";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle Count";
            row["Description"] = "Number of power-on events.";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 165;
            row["Dec"] = "165";
            row["Hex"] = "A5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Write Erase Count";
            row["Description"] = "";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 166;
            row["Dec"] = "166";
            row["Hex"] = "A6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Minimum Program/Erase (P/E) Cycles";
            row["Description"] = "";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 167;
            row["Dec"] = "167";
            row["Hex"] = "A7";
            row["IsCritical"] = false;
            row["AttributeName"] = "Maximum Bad Blocks Per Die";
            row["Description"] = "";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 168;
            row["Dec"] = "168";
            row["Hex"] = "A8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Maximum Program/Erase (P/E) Cycles";
            row["Description"] = "";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 169;
            row["Dec"] = "169";
            row["Hex"] = "A9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Bad Blocks";
            row["Description"] = "A count of all bad blocks on the drive, including the initial bad blocks when the drive was new, plus the additional (grown) bad blocks.";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 170;
            row["Dec"] = "170";
            row["Hex"] = "AA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Grown Bad Blocks";
            row["Description"] = "A count of bad blocks that have developed since the drive left the factory.";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 171;
            row["Dec"] = "171";
            row["Hex"] = "AB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Count";
            row["Description"] = "A count of failures of the drive to write to (program) a block during a P/E cycle.";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 172;
            row["Dec"] = "172";
            row["Hex"] = "AC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Count";
            row["Description"] = "A count of failures of the drive to erase a block during a P/E cycle.";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 173;
            row["Dec"] = "173";
            row["Hex"] = "AD";
            row["IsCritical"] = false;
            row["AttributeName"] = "Average Program/Erase (P/E) Cycles";
            row["Description"] = "";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 174;
            row["Dec"] = "174";
            row["Hex"] = "AE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unexpected Power Loss Count";
            row["Description"] = "The number of times power to the drive was lost unexpectedly.";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Error Correction Count";
            row["Description"] = "";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 187;
            row["Dec"] = "187";
            row["Hex"] = "BB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Reported Uncorrectable Errors";
            row["Description"] = "";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 188;
            row["Dec"] = "188";
            row["Hex"] = "BC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Command Timeout Count";
            row["Description"] = "";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "Temperature of the drive.";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 199;
            row["Dec"] = "199";
            row["Hex"] = "C7";
            row["IsCritical"] = false;
            row["AttributeName"] = "SATA/PCIe CRC Error Count";
            row["Description"] = "";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 212;
            row["Dec"] = "212";
            row["Hex"] = "D4";
            row["IsCritical"] = false;
            row["AttributeName"] = "SATA PHY Error Count";
            row["Description"] = "";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 230;
            row["Dec"] = "230";
            row["Hex"] = "E6";
            row["IsCritical"] = true;
            row["AttributeName"] = "Media Wearout Indicator";
            row["Description"] = "An indicator of media wearout as the drive undergoes repeated P/E cycles. Used as a lifetime remaining indicator.";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 232;
            row["Dec"] = "232";
            row["Hex"] = "E8";
            row["IsCritical"] = true;
            row["AttributeName"] = "Spare Block Remaining";
            row["Description"] = "An indicator of how many reserved blocks remain on the device. Used as a lifetime remaining indicator.";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 233;
            row["Dec"] = "233";
            row["Hex"] = "E9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total GB Written to NAND";
            row["Description"] = "Total gigabytes written to the NAND flash over the lifetime of the drive.";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 241;
            row["Dec"] = "241";
            row["Hex"] = "F1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total GB Written";
            row["Description"] = "Total gigabytes written to the drive.";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 242;
            row["Dec"] = "242";
            row["Hex"] = "F2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total GB Read";
            row["Description"] = "Total gigabytes read from the drive.";
            ssdSanDiskDefinitions.Rows.Add(row);

            row = ssdSanDiskDefinitions.NewRow();
            row["Key"] = 244;
            row["Dec"] = "244";
            row["Hex"] = "F4";
            row["IsCritical"] = false;
            row["AttributeName"] = "Thermal Throttle Status";
            row["Description"] = "";
            ssdSanDiskDefinitions.Rows.Add(row);

            ssdSanDiskDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdSanDiskDefinitions.PopulateSsdSanDiskDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdSanDiskDefinitions;
            }
        }
    }
}
