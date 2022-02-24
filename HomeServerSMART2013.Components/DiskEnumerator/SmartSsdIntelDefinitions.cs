using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class SmartSsdIntelDefinitions
    {
        DataTable ssdIntelDefinitions;

        public SmartSsdIntelDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdIntelDefinitions");
            PopulateSsdIntelDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdIntelDefinitions");
        }

        public void PopulateSsdIntelDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartSsdIntelDefinitions.PopulateSsdIntelDataTable");
            ssdIntelDefinitions = new DataTable("SsdIntelSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            ssdIntelDefinitions.Columns.Add("Key", typeof(int));
            ssdIntelDefinitions.Columns.Add("Dec", typeof(String));
            ssdIntelDefinitions.Columns.Add("Hex", typeof(String));
            ssdIntelDefinitions.Columns.Add("IsCritical", typeof(bool));
            ssdIntelDefinitions.Columns.Add("AttributeName", typeof(String));
            ssdIntelDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = ssdIntelDefinitions.NewRow();

            row["Key"] = 3;
            row["Dec"] = "03";
            row["Hex"] = "03";
            row["IsCritical"] = false;
            row["AttributeName"] = "Spin-Up Time";
            row["Description"] = "The average time it takes the spindle to spin up. Since a SSD has no moveable parts, this attribute reports a fixed raw value of zero (0) and a fixed normalized value of 100.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 4;
            row["Dec"] = "04";
            row["Hex"] = "04";
            row["IsCritical"] = false;
            row["AttributeName"] = "Start/Stop Count";
            row["Description"] = "This type of event is not an issue for SSDs. However, hard disk drives can experience only a finite number of these events and, therefore, must be tracked. This attribute reports a fixed value of zero (0) and a fixed normalized value of 100.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Retired Sector Count";
            row["Description"] = "This attribute shows the number of retired blocks since leaving the factory (also known as a grown defect count). For 50nm drives, the normalized value has an initial value of 100 but counts up from 1, for every 4 grown defects. The normalized value of this attribute becomes 1 when there are 4 grown defects, then the value is 2 when there are 8 grown defects, etc. For 34nm drives, the raw value increments for every grown defect.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power On Hours";
            row["Description"] = "This attribute reports the cumulative number of power-on hours over the life of the device.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle Count";
            row["Description"] = "This attribute reports the cumulative number of power cycle events (power on/off cycles) over the life of the device.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 170;
            row["Dec"] = "170";
            row["Hex"] = "AA";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reserved Block Count";
            row["Description"] = "This attribute reports the number of reserve blocks remaining. The attribute value begins at 100, which indicates that the reserved space is 100% available. The threshold value for this attribute is 10% availability, which indicated that the drive is close to its end life.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 171;
            row["Dec"] = "171";
            row["Hex"] = "AB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Program Fail Count";
            row["Description"] = "Percentage of NAND program failures.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 172;
            row["Dec"] = "172";
            row["Hex"] = "AC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Erase Fail Count";
            row["Description"] = "Percentage of NAND erase failures.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 174;
            row["Dec"] = "174";
            row["Hex"] = "AE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unexpected Power Loss";
            row["Description"] = "";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 175;
            row["Dec"] = "175";
            row["Hex"] = "AF";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Loss Protection Failure";
            row["Description"] = "";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 183;
            row["Dec"] = "183";
            row["Hex"] = "B7";
            row["IsCritical"] = false;
            row["AttributeName"] = "SATA Downshift Count";
            row["Description"] = "The count of the number of times SATA interface selected lower signaling rate due to error.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = true;
            row["AttributeName"] = "End-to-End Error Detection Count";
            row["Description"] = "This attribute is only available for 34nm drives and counts the number of times errors are encountered during logical block addressing (LBA) tag checks on the data path within the drive.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 187;
            row["Dec"] = "187";
            row["Hex"] = "BB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Uncorrectable Error Count";
            row["Description"] = "The raw value shows the count of errors that could not be recovered using Error Correction Code (ECC).";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 190;
            row["Dec"] = "190";
            row["Hex"] = "BE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature - Airflow Temperature (Case)";
            row["Description"] = "Reports the SSD case temperature. Raw value suggests 100 - case temperature in C degrees.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 192;
            row["Dec"] = "192";
            row["Hex"] = "C0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Unsafe Shutdown Count";
            row["Description"] = "This attribute reports the cumulative number of unsafe (unclean) shutdown events over the life of the device. An unsafe shutdown occurs whenever the device is powered off without \"standby immediate\" being the last command.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature - Device Internal Temperature";
            row["Description"] = "Reports internal temperature of the SSD. Temperature reading is the value direct from the printed circuit board (PCB) sensor without offset.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 197;
            row["Dec"] = "197";
            row["Hex"] = "C5";
            row["IsCritical"] = false;
            row["AttributeName"] = "Pending Sector Count";
            row["Description"] = "";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 199;
            row["Dec"] = "199";
            row["Hex"] = "C7";
            row["IsCritical"] = false;
            row["AttributeName"] = "CRC Error Count";
            row["Description"] = "The total number of encountered SATA interface cyclic redundancy check (CRC) errors.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 225;
            row["Dec"] = "225";
            row["Hex"] = "E1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Host Writes";
            row["Description"] = "This attribute reports the total number of sectors written by the host system. The raw value is increased by 1 for every 65,536 sectors written by the host.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 226;
            row["Dec"] = "226";
            row["Hex"] = "E2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Timed Workload Media Wear Indicator";
            row["Description"] = "This attribute tracks the drive wear seen by the device during the last wear timer loop, as a percentage of the maximum rated cycles. The raw value tracks the percentage up to 3 decimal points. This value should be divided by 1024 to get the percentage. For example: if the raw value is 4450, the percentage is 4450/1024 = 4.345%. The raw value is held at 65,535 until the wear timer (attribute E4h) reaches 60 (minutes). The normalized value is always set to 100 and should be ignored.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 227;
            row["Dec"] = "227";
            row["Hex"] = "E3";
            row["IsCritical"] = false;
            row["AttributeName"] = "Timed Workload Host Reads Percentage";
            row["Description"] = "This attribute shows the percentage of I/O operations that are read operations during the last workload timer loop. The raw value tracks this percentage and is held at 65,535 until the workload timer (attribute 228) reaches 60 (minutes). The normalized value is always set to 100 and should be ignored.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 228;
            row["Dec"] = "228";
            row["Hex"] = "E4";
            row["IsCritical"] = false;
            row["AttributeName"] = "Timed Workload Timer";
            row["Description"] = "This attribute is used to measure the time elapsed during the current workload. The attribute is reset when a \"SMART execute offline immediate\" subcommand is issued to the drive. The raw value tracks the time in minutes and has a maximum value of 232 = 4,294,967,296 minutes (8,171 years). The normalized value is always set to 100 and should be ignored.";
            ssdIntelDefinitions.Rows.Add(row);
            
            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 232;
            row["Dec"] = "232";
            row["Hex"] = "E8";
            row["IsCritical"] = true;
            row["AttributeName"] = "Available Reserved Space";
            row["Description"] = "This attribute reports the number of reserve blocks remaining. The attribute value begins at 100, which indicates that the reserved space is 100% available. The threshold value for this attribute is 10% availability, which indicated that the drive is close to its end life.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 233;
            row["Dec"] = "233";
            row["Hex"] = "E9";
            row["IsCritical"] = true;
            row["AttributeName"] = "Media Wearout Indicator";
            row["Description"] = "This attribute reports the number of cycles the NAND media has experienced. The normalized value declines linearly from 100 to 1 as the average erase cycle count increases from 0 to the maximum rated cycles. Once the normalized value reaches 1, the number will not decrease, although it is likely that significant additional wear can be put on the device.";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 234;
            row["Dec"] = "234";
            row["Hex"] = "EA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Thermal Throttle Status";
            row["Description"] = "";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 241;
            row["Dec"] = "241";
            row["Hex"] = "F1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total LBAs Written";
            row["Description"] = "Counts sectors written by the host (1 sector = 512 bytes).";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 242;
            row["Dec"] = "242";
            row["Hex"] = "F2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total LBAs Read";
            row["Description"] = "Counts sectors read by the host (1 sector = 512 bytes).";
            ssdIntelDefinitions.Rows.Add(row);

            row = ssdIntelDefinitions.NewRow();
            row["Key"] = 249;
            row["Dec"] = "249";
            row["Hex"] = "F9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total NAND Writes";
            row["Description"] = "";
            ssdIntelDefinitions.Rows.Add(row);

            ssdIntelDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartSsdIntelDefinitions.PopulateSsdIntelDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return ssdIntelDefinitions;
            }
        }
    }
}
