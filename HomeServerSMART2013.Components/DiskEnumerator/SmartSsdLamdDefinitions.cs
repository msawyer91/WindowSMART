using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdLamdDefinitions
    {
        DataTable ssdLamdDefinitions;

        public SmartSsdLamdDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdLamdDefinitions");
            PopulateSsdLamdDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdLamdDefinitions");
        }

        public void PopulateSsdLamdDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdGenericDefinitions.PopulateSsdLamdDataTable");
            ssdLamdDefinitions = new DataTable("SsdStecSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdLamdDefinitions.Columns.Add("Key", typeof(int));
            ssdLamdDefinitions.Columns.Add("Dec", typeof(String));
            ssdLamdDefinitions.Columns.Add("Hex", typeof(String));
            ssdLamdDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdLamdDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdLamdDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdLamdDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = false;
            row["AttributeName"] = "Raw Read Error Rate";
            row["Description"] = "Count of raw data errors while data from media, including retry errors or data error (uncorrectable).";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Retired Sector Count";
            row["Description"] = "Count of reallocated blocks. This is the count of reallocated or remapped sectors during normal operation from the grown defects table.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Hours";
            row["Description"] = "Number of hours elapsed in the Power-On state.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 11;
            row["Dec"] = "11";
            row["Hex"] = "0B";
            row["IsCritical"] = false;
            row["AttributeName"] = "Client Custom Attribute";
            row["Description"] = "This is a custom attribute for the SSD.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle Count";
            row["Description"] = "Number of power-on events.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 171;
            row["Dec"] = "171";
            row["Hex"] = "AB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unknown Attribute";
            row["Description"] = "Technical information about this attribute is not published and therefore its use is unknown.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 172;
            row["Dec"] = "172";
            row["Hex"] = "AC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unknown Attribute";
            row["Description"] = "Technical information about this attribute is not published and therefore its use is unknown.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 174;
            row["Dec"] = "174";
            row["Hex"] = "AE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Client Custom Attribute";
            row["Description"] = "This is a custom attribute for the SSD.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 181;
            row["Dec"] = "181";
            row["Hex"] = "B5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Count Total";
            row["Description"] = "This Attribute indicates the number of times the SSD failed to write data to a block. Writing (or programming) is a separate operation from erasing, which is tracked in the next attribute.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 182;
            row["Dec"] = "182";
            row["Hex"] = "B6";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Count Total";
            row["Description"] = "This Attribute indicates the number of times the SSD failed to erase a block. SSDs cannot simply overwrite existing data in a block; the block must be erased first. This attribute tracks erase failures; the previous attribute tracks write failures.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 183;
            row["Dec"] = "183";
            row["Hex"] = "B7";
            row["IsCritical"] = false;
            row["AttributeName"] = "Client Custom Attribute";
            row["Description"] = "This is a custom attribute for the SSD.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = false;
            row["AttributeName"] = "End-to-End/IOEDC Error Count";
            row["Description"] = "Tracks the number of end to end internal card data path errors that were detected.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "Temperature of the base casting.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 201;
            row["Dec"] = "201";
            row["Hex"] = "C9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unknown Attribute";
            row["Description"] = "Technical information about this attribute is not published and therefore its use is unknown.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 204;
            row["Dec"] = "204";
            row["Hex"] = "CC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Soft ECC Correction Rate";
            row["Description"] = "Number of errors corrected by RAISE (Redundant Array of Independent Silicon Elements) that cannot be fixed on-the-fly and requires ECC (multilevel) to correct.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 231;
            row["Dec"] = "231";
            row["Hex"] = "E7";
            row["IsCritical"] = true;
            row["AttributeName"] = "SSD Life Left";
            row["Description"] = "A measure of the estimated life left, based on a combination of PE cycles and available reserve blocks. 100 is a new drive, 10 = replace as it has sufficient reserved blocks but the PE cycles have been used, 0 = insufficient reserved blocks, drive is read only to allow recovery of data on the drive.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 234;
            row["Dec"] = "234";
            row["Hex"] = "EA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unknown Attribute";
            row["Description"] = "Technical information about this attribute is not published and therefore its use is unknown.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 241;
            row["Dec"] = "241";
            row["Hex"] = "F1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total LBA Written";
            row["Description"] = "Indicates the total amount of data written from hosts since the drive was deployed. This is stored in 4 bytes. The number stores represents the number of bytes written by the host to the drive, in 64 GB increments.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 242;
            row["Dec"] = "242";
            row["Hex"] = "F2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total LBA Read";
            row["Description"] = "Indicates the total amount of data read to hosts since the drive was deployed. This is stored in 4 bytes. The number stores represents the number of bytes read by the host to the drive, in 64 GB increments.";
            ssdLamdDefinitions.Rows.Add(row);

            row = ssdLamdDefinitions.NewRow();
            row["Key"] = 250;
            row["Dec"] = "250";
            row["Hex"] = "FA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unknown Attribute";
            row["Description"] = "Technical information about this attribute is not published and therefore its use is unknown.";
            ssdLamdDefinitions.Rows.Add(row);

            ssdLamdDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartHddDefinitions.PopulateSsdLamdDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdLamdDefinitions;
            }
        }
    }
}