using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdSmartModularDefinitions
    {
        DataTable ssdSmartModularDefinitions;

        public SmartSsdSmartModularDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdSmartModularDefinitions");
            PopulateSsdSmartModularDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdSmartModularDefinitions");
        }

        public void PopulateSsdSmartModularDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdSmartModularDefinitions.PopulateSsdSmartModularDataTable");
            ssdSmartModularDefinitions = new DataTable("SsdSmartModularSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdSmartModularDefinitions.Columns.Add("Key", typeof(int));
            ssdSmartModularDefinitions.Columns.Add("Dec", typeof(String));
            ssdSmartModularDefinitions.Columns.Add("Hex", typeof(String));
            ssdSmartModularDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdSmartModularDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdSmartModularDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdSmartModularDefinitions.NewRow();

            row = ssdSmartModularDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power-On Hours";
            row["Description"] = "The Power-On Hours attribute specifies the total number of hours the drive has been operational. This counter starts when the drive is first manufactured and continues whenever the drive is powered on.";
            ssdSmartModularDefinitions.Rows.Add(row);

            row = ssdSmartModularDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle Count";
            row["Description"] = "The Power Cycle Count attribute indicates the total number of times power has cycled on the drive.";
            ssdSmartModularDefinitions.Rows.Add(row);

            row = ssdSmartModularDefinitions.NewRow();
            row["Key"] = 192;
            row["Dec"] = "192";
            row["Hex"] = "C0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power-Off Retract Count";
            row["Description"] = "The Power-Off Retract Count attribute returns the number of times the drive has been powered-off. This value is one less than the Power-Cycle Count attribute value.";
            ssdSmartModularDefinitions.Rows.Add(row);

            row = ssdSmartModularDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "The Temperature attribute indicates the current drive temperature in degrees Celsius.";
            ssdSmartModularDefinitions.Rows.Add(row);

            row = ssdSmartModularDefinitions.NewRow();
            row["Key"] = 197;
            row["Dec"] = "197";
            row["Hex"] = "C5";
            row["IsCritical"] = true;
            row["AttributeName"] = "Number of ECC Errors";
            row["Description"] = "The Number of ECC Errors attribute specifies the number of sectors that encountered an ECC error and were recovered.";
            ssdSmartModularDefinitions.Rows.Add(row);

            row = ssdSmartModularDefinitions.NewRow();
            row["Key"] = 198;
            row["Dec"] = "198";
            row["Hex"] = "C6";
            row["IsCritical"] = true;
            row["AttributeName"] = "Uncorrectable Errors Count";
            row["Description"] = "The Uncorrectable Errors Count attribute returns the number of uncorrectable ECC errors that occurred. This counter increments if more than four bits in the affected sector are uncorrectable.";
            ssdSmartModularDefinitions.Rows.Add(row);

            row = ssdSmartModularDefinitions.NewRow();
            row["Key"] = 199;
            row["Dec"] = "199";
            row["Hex"] = "C7";
            row["IsCritical"] = false;
            row["AttributeName"] = "UDMA CRC Error Count";
            row["Description"] = "The UDMA CRC Error Count attribute indicates the number of sectors that encountered a CRC error while in UDMA mode.";
            ssdSmartModularDefinitions.Rows.Add(row);

            row = ssdSmartModularDefinitions.NewRow();
            row["Key"] = 232;
            row["Dec"] = "232";
            row["Hex"] = "E8";
            row["IsCritical"] = true;
            row["AttributeName"] = "Endurance Remaining";
            row["Description"] = "The Endurance Remaining attribute reports the number of physical erase cycles completed on the drive as a percentage of the maximum physical erase cycles the drive supports. Because the maximum physical erase cycles is a theoretical number (100,000), a low value in this attribute does not necessarily mean the drive will fail. In other words, the drive may exceed the maximum number of erase cycles, causing the drive to report 0%, without impacting drive performance.";
            ssdSmartModularDefinitions.Rows.Add(row);

            row = ssdSmartModularDefinitions.NewRow();
            row["Key"] = 233;
            row["Dec"] = "233";
            row["Hex"] = "E9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power-On Time";
            row["Description"] = "The Power-On Time attribute indicates the total number of seconds the drive has been operational. This timer starts when the drive is manufactured in production and continues whenever the drive is powered on.";
            ssdSmartModularDefinitions.Rows.Add(row);

            row = ssdSmartModularDefinitions.NewRow();
            row["Key"] = 234;
            row["Dec"] = "234";
            row["Hex"] = "EA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Uncorrectable ECC Count";
            row["Description"] = "The Uncorrectable ECC Count attribute stores the total number of ECC errors the drive encountered but could not resolve. If an uncorrectable ECC error occurs, the drive returns the error in the Status and Error registers and increments this counter.";
            ssdSmartModularDefinitions.Rows.Add(row);

            row = ssdSmartModularDefinitions.NewRow();
            row["Key"] = 235;
            row["Dec"] = "235";
            row["Hex"] = "EB";
            row["IsCritical"] = true;
            row["AttributeName"] = "Good Block Rate";
            row["Description"] = "The Good Block Rate attribute reports the number of available reserved blocks (for spares) as a percentage of the total number of reserved blocks. Whenever the drive swaps a reserved block for a bad block, this percentage decreases.";
            ssdSmartModularDefinitions.Rows.Add(row);

            row = ssdSmartModularDefinitions.NewRow();
            row["Key"] = 251;
            row["Dec"] = "251";
            row["Hex"] = "FB";
            row["IsCritical"] = true;
            row["AttributeName"] = "Minimum Spares Remaining";
            row["Description"] = "The Minimum Spares Remaining attribute indicates the number of remaining spare blocks as a percentage of the total number of spare blocks available.";
            ssdSmartModularDefinitions.Rows.Add(row);

            row = ssdSmartModularDefinitions.NewRow();
            row["Key"] = 252;
            row["Dec"] = "252";
            row["Hex"] = "FC";
            row["IsCritical"] = true;
            row["AttributeName"] = "Newly Added Bad Flash Block";
            row["Description"] = "The Newly Added Bad Flash Block attribute indicates the total number of bad flash blocks the drive detected since it was first initialized in manufacturing.";
            ssdSmartModularDefinitions.Rows.Add(row);

            row = ssdSmartModularDefinitions.NewRow();
            row["Key"] = 254;
            row["Dec"] = "254";
            row["Hex"] = "FE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total Erase Blocks";
            row["Description"] = "The Total Erase Blocks attribute indicates the total number of times the drive has erased any erase block. Because a secure erase operation erases every block, this number increases significantly whenever a secure erase is performed.";
            ssdSmartModularDefinitions.Rows.Add(row);

            ssdSmartModularDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdSmartModularDefinitions.PopulateSsdSmartModularDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdSmartModularDefinitions;
            }
        }
    }
}
