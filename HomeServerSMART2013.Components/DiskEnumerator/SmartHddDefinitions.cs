using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    /// <summary>
    /// Contains data about the SMART hddDefinitions.  Source of data is wikipedia.
    /// "http://en.wikipedia.org/wiki/S.M.A.R.T."
    /// </summary>
    public sealed class SmartHddDefinitions
    {
        DataTable hddDefinitions;

        public SmartHddDefinitions()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartHddDefinitions.SmartHddDefinitions");
            PopulateHddDataTable();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartHddDefinitions.SmartHddDefinitions");
        }

        public void PopulateHddDataTable()
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.SmartHddDefinitions.PopulateHddDataTable");
            hddDefinitions = new DataTable("HddSMARTDefs");

            // Header Row (columns)
            DataColumn idColumn = new DataColumn();
            hddDefinitions.Columns.Add("Key", typeof(int));
            hddDefinitions.Columns.Add("Dec", typeof(String));
            hddDefinitions.Columns.Add("Hex", typeof(String));
            hddDefinitions.Columns.Add("IsCritical", typeof(bool));
            hddDefinitions.Columns.Add("AttributeName", typeof(String));
            hddDefinitions.Columns.Add("Description", typeof(String));

            // Load the data!
            DataRow row = hddDefinitions.NewRow();

            row["Key"] = 1;
            row["Dec"] = "01";
            row["Hex"] = "01";
            row["IsCritical"] = true;
            row["AttributeName"] = "Read Error Rate";
            row["Description"] = "Indicates the rate of hardware read errors that occurred when reading data from a disk surface. A non-zero value indicates a problem with the disk surface, read/write heads, or the heads are not centered exactly over the track.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 2;
            row["Dec"] = "02";
            row["Hex"] = "02";
            row["IsCritical"] = false;
            row["AttributeName"] = "Throughput Performance";
            row["Description"] = "Overall (general) throughput performance of a hard disk drive. If the value of this attribute is decreasing there is a high probability that there is a problem with the disk.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 3;
            row["Dec"] = "03";
            row["Hex"] = "03";
            row["IsCritical"] = true;
            row["AttributeName"] = "Spin-Up Time";
            row["Description"] = "Average time of spindle spin up (from zero RPM to fully operational [millisecs]). Can indicate a problem with the drive motor or bearings.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 4;
            row["Dec"] = "04";
            row["Hex"] = "04";
            row["IsCritical"] = false;
            row["AttributeName"] = "Start/Stop Count";
            row["Description"] = "A tally of spindle start/stop cycles. The spindle turns on, and hence the count is increased, both when the hard disk is turned on after having before been turned entirely off (disconnected from power source) and when the hard disk returns from having previously been put to sleep mode.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 5;
            row["Dec"] = "05";
            row["Hex"] = "05";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reallocated Sectors Count";
            row["Description"] = "Count of reallocated sectors. When the hard drive finds a read/write/verification error, it marks this sector as \"reallocated\" and transfers data to a special reserved area (spare area). This process is also known as remapping, and \"reallocated\" sectors are called remaps. This is why, on modern hard disks, \"bad sectors\" cannot be found while testing the surface – all bad blocks are hidden in reallocated sectors. However, as the number of reallocated sectors increases, the read/write speed tends to decrease. The raw value normally represents a count of the number of bad sectors that have been found and remapped. Thus, the higher the attribute value, the more sectors the drive has had to reallocate.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 6;
            row["Dec"] = "06";
            row["Hex"] = "06";
            row["IsCritical"] = false;
            row["AttributeName"] = "Read Channel Margin";
            row["Description"] = "Margin of a channel while reading data. The function of this attribute is not specified.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 7;
            row["Dec"] = "07";
            row["Hex"] = "07";
            row["IsCritical"] = true;
            row["AttributeName"] = "Seek Error Rate";
            row["Description"] = "Rate of seek errors of the magnetic heads. If there is a partial failure in the mechanical positioning system, then seek errors will arise. Such a failure may be due to numerous factors, such as damage to a servo, or thermal widening of the hard disk. More seek errors indicates a worsening condition of a disk’s surface or the mechanical subsystem, or both. Note that Seagate drives often report a raw value that is very high, even on new drives, and this does not normally indicate a failure.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 8;
            row["Dec"] = "08";
            row["Hex"] = "08";
            row["IsCritical"] = false;
            row["AttributeName"] = "Seek Time Performance";
            row["Description"] = "Average performance of seek operations of the magnetic heads. If this attribute is decreasing, it is a sign of problems in the mechanical subsystem.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 9;
            row["Dec"] = "09";
            row["Hex"] = "09";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power-On Hours";
            row["Description"] = "Count of hours in power-on state. The raw value of this attribute shows total count of hours (or minutes, or seconds, depending on manufacturer) in power-on state.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 10;
            row["Dec"] = "10";
            row["Hex"] = "0A";
            row["IsCritical"] = true;
            row["AttributeName"] = "Spin Retry Count";
            row["Description"] = "Count of retry of spin start attempts. This attribute stores a total count of the spin start attempts to reach the fully operational speed (under the condition that the first attempt was unsuccessful). An increase of this attribute value is a sign of problems in the hard disk mechanical subsystem.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 11;
            row["Dec"] = "11";
            row["Hex"] = "0B";
            row["IsCritical"] = false;
            row["AttributeName"] = "Recalibration Retries";
            row["Description"] = "This attribute indicates the number of times recalibration was requested (under the condition that the first attempt was unsuccessful). An increase of this attribute value is a sign of problems in the hard disk mechanical subsystem.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 12;
            row["Dec"] = "12";
            row["Hex"] = "0C";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power Cycle Count";
            row["Description"] = "This attribute indicates the count of full hard disk power on/off cycles.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 13;
            row["Dec"] = "13";
            row["Hex"] = "0D";
            row["IsCritical"] = false;
            row["AttributeName"] = "Soft Read Error Rate";
            row["Description"] = "Uncorrected read errors reported to the operating system.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 183;
            row["Dec"] = "183";
            row["Hex"] = "B7";
            row["IsCritical"] = false;
            row["AttributeName"] = "SATA Downshift Error Count";
            row["Description"] = "Vendor Specific Attribute (WD and Samsung)";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 184;
            row["Dec"] = "184";
            row["Hex"] = "B8";
            row["IsCritical"] = true;
            row["AttributeName"] = "End-to-End Error";
            row["Description"] = "Number of parity errors during transfer between the cache RAM and the host.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 185;
            row["Dec"] = "185";
            row["Hex"] = "B9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Head Stability";
            row["Description"] = "Vendor Specific Attribute (WD)";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 186;
            row["Dec"] = "186";
            row["Hex"] = "BA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Induced Op-Vibration Detection";
            row["Description"] = "Vendor Specific Attribute (WD)";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 187;
            row["Dec"] = "187";
            row["Hex"] = "BB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Reported Uncorrectable Errors";
            row["Description"] = "A number of errors that could not be recovered using hardware ECC (see attribute 195).";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 188;
            row["Dec"] = "188";
            row["Hex"] = "BC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Command Timeout";
            row["Description"] = "A number of aborted operations due to HDD timeout. Normally this attribute value should be equal to zero and if the value is far above zero, then most likely there will be some serious problems with power supply or an oxidized data cable.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 189;
            row["Dec"] = "189";
            row["Hex"] = "BD";
            row["IsCritical"] = false;
            row["AttributeName"] = "High Fly Writes";
            row["Description"] = "HDD producers implement a Fly Height Monitor that attempts to provide additional protections for write operations by detecting when a recording head is flying outside its normal operating range. If an unsafe fly height condition is encountered, the write process is stopped, and the information is rewritten or reallocated to a safe region of the hard drive. This attribute indicates the count of these errors detected over the lifetime of the drive.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 190;
            row["Dec"] = "190";
            row["Hex"] = "BE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Airflow Temperature";
            row["Description"] = "The temperature of the air inside the hard disk chamber.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 191;
            row["Dec"] = "191";
            row["Hex"] = "BF";
            row["IsCritical"] = false;
            row["AttributeName"] = "G-Sense Error Rate";
            row["Description"] = "The number of errors resulting from externally-induced shock and vibration.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 192;
            row["Dec"] = "192";
            row["Hex"] = "C0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power-Off Retract Count";
            row["Description"] = "Number of times the heads are loaded off the media. Heads can be unloaded without actually powering off (or Emergency Retract Cycle count – Fujitsu).";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 193;
            row["Dec"] = "193";
            row["Hex"] = "C1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Load/Unload Cycle Count";
            row["Description"] = "Count of load/unload cycles into head landing zone position.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 194;
            row["Dec"] = "194";
            row["Hex"] = "C2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "Current internal temperature.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 195;
            row["Dec"] = "195";
            row["Hex"] = "C3";
            row["IsCritical"] = false;
            row["AttributeName"] = "Hardware ECC Recovered";
            row["Description"] = "Time between ECC-corrected errors or number of ECC on-the-fly errors.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 196;
            row["Dec"] = "196";
            row["Hex"] = "C4";
            row["IsCritical"] = true;
            row["AttributeName"] = "Reallocation Event Count";
            row["Description"] = "Count of remap operations. The raw value of this attribute shows the total number of attempts to transfer data from reallocated sectors to a spare area. Both successful & unsuccessful attempts are counted.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 197;
            row["Dec"] = "197";
            row["Hex"] = "C5";
            row["IsCritical"] = true;
            row["AttributeName"] = "Current Pending Sector Count";
            row["Description"] = "Number of \"unstable\" sectors (waiting to be remapped, because of read errors). If an unstable sector is subsequently written or read successfully, this value is decreased and the sector is not remapped. Read errors on a sector will not remap the sector (since it might be readable later); instead, the drive firmware remembers that the sector needs to be remapped, and remaps it the next time it's written. Pending sectors can be thought of as \"on probation.\"";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 198;
            row["Dec"] = "198";
            row["Hex"] = "C6";
            row["IsCritical"] = true;
            row["AttributeName"] = "Offline Uncorrectable Sector Count";
            row["Description"] = "The total number of uncorrectable errors when reading/writing a sector. A rise in the value of this attribute indicates defects of the disk surface and/or problems in the mechanical subsystem (or Off-Line Scan Uncorrectable Sector Count – Fujitsu).";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 199;
            row["Dec"] = "198";
            row["Hex"] = "C7";
            row["IsCritical"] = false;
            row["AttributeName"] = "Ultra DMA CRC Error Count";
            row["Description"] = "The number of errors in data transfer via the interface cable as determined by ICRC (Interface Cyclic Redundancy Check). Usually indicates a faulty data cable, but in some cases can be seen in external USB cases where the USB bridge chip is not fully compatible with the host device driver.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 200;
            row["Dec"] = "200";
            row["Hex"] = "C8";
            row["IsCritical"] = false;
            row["AttributeName"] = "Write Error Rate/Multi-Zone Error Rate";
            row["Description"] = "The total number of errors when writing a sector.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 201;
            row["Dec"] = "201";
            row["Hex"] = "C9";
            row["IsCritical"] = false;
            row["AttributeName"] = "Soft Read Error Rate";
            row["Description"] = "Number of off-track errors.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 202;
            row["Dec"] = "202";
            row["Hex"] = "CA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Data Address Mark Errors";
            row["Description"] = "Vendor Specific Attribute.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 203;
            row["Dec"] = "203";
            row["Hex"] = "CB";
            row["IsCritical"] = false;
            row["AttributeName"] = "Run Out Cancel";
            row["Description"] = "Number of ECC errors.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 204;
            row["Dec"] = "204";
            row["Hex"] = "CC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Soft ECC Correction";
            row["Description"] = "Number of errors corrected by software ECC.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 205;
            row["Dec"] = "205";
            row["Hex"] = "CD";
            row["IsCritical"] = false;
            row["AttributeName"] = "Thermal Asperity Rate";
            row["Description"] = "Number of errors due to high temperature.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 206;
            row["Dec"] = "206";
            row["Hex"] = "CE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Flying Height";
            row["Description"] = "Height of heads above the disk surface. A flying height that's too low increases the chances of a head crash while a flying height that's too high increases the chances of a read/write error.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 207;
            row["Dec"] = "207";
            row["Hex"] = "CF";
            row["IsCritical"] = false;
            row["AttributeName"] = "Spin High Current";
            row["Description"] = "Amount of surge current used to spin up the drive.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 208;
            row["Dec"] = "208";
            row["Hex"] = "D0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Spin Buzz";
            row["Description"] = "Number of buzz routines needed to spin up the drive due to insufficient power.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 209;
            row["Dec"] = "209";
            row["Hex"] = "D1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Offline Seek Performance";
            row["Description"] = "Drive’s seek performance during its internal tests.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 211;
            row["Dec"] = "211";
            row["Hex"] = "D3";
            row["IsCritical"] = false;
            row["AttributeName"] = "Vibration During Write";
            row["Description"] = "Vibration detected during write.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 212;
            row["Dec"] = "212";
            row["Hex"] = "D4";
            row["IsCritical"] = false;
            row["AttributeName"] = "Shock During Write";
            row["Description"] = "Shock detected during write.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 220;
            row["Dec"] = "220";
            row["Hex"] = "DC";
            row["IsCritical"] = false;
            row["AttributeName"] = "Disk Shift";
            row["Description"] = "Distance the disk has shifted relative to the spindle (usually due to shock or temperature). Unit of measure is unknown.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 221;
            row["Dec"] = "221";
            row["Hex"] = "DD";
            row["IsCritical"] = false;
            row["AttributeName"] = "G-Sense Error Rate";
            row["Description"] = "The number of errors resulting from externally-induced shock and vibration.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 222;
            row["Dec"] = "222";
            row["Hex"] = "DE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Loaded Hours";
            row["Description"] = "Time spent operating under data load (movement of magnetic head armature).";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 223;
            row["Dec"] = "223";
            row["Hex"] = "DF";
            row["IsCritical"] = false;
            row["AttributeName"] = "Load/Unload Retry Count";
            row["Description"] = "Number of times head changes position.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 224;
            row["Dec"] = "224";
            row["Hex"] = "E0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Load Friction";
            row["Description"] = "Resistance caused by friction in mechanical parts while operating.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 225;
            row["Dec"] = "225";
            row["Hex"] = "E1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Load/Unload Cycle Count";
            row["Description"] = "Count of load/unload cycles into head landing zone position.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 226;
            row["Dec"] = "226";
            row["Hex"] = "E2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Load 'In'-time";
            row["Description"] = "Total time of loading on the magnetic heads actuator (time not spent in parking area).";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 227;
            row["Dec"] = "227";
            row["Hex"] = "E3";
            row["IsCritical"] = false;
            row["AttributeName"] = "Torque Amplification Count";
            row["Description"] = "Number of attempts to compensate for platter speed variations.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 228;
            row["Dec"] = "228";
            row["Hex"] = "E4";
            row["IsCritical"] = false;
            row["AttributeName"] = "Power-Off Retract Cycle";
            row["Description"] = "The number of times the magnetic armature was retracted automatically as a result of cutting power.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 230;
            row["Dec"] = "230";
            row["Hex"] = "E6";
            row["IsCritical"] = false;
            row["AttributeName"] = "GMR Head Amplitude";
            row["Description"] = "Amplitude of \"thrashing\" (distance of repetitive forward/reverse head motion).";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 231;
            row["Dec"] = "231";
            row["Hex"] = "E7";
            row["IsCritical"] = false;
            row["AttributeName"] = "Temperature";
            row["Description"] = "Current internal temperature.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 240;
            row["Dec"] = "240";
            row["Hex"] = "F0";
            row["IsCritical"] = false;
            row["AttributeName"] = "Head Flying Hours";
            row["Description"] = "Time while head is positioning.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 241;
            row["Dec"] = "241";
            row["Hex"] = "F1";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total LBAs Written";
            row["Description"] = "Total LBAs written.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 242;
            row["Dec"] = "242";
            row["Hex"] = "F2";
            row["IsCritical"] = false;
            row["AttributeName"] = "Total LBAs Read";
            row["Description"] = "Total LBAs read.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 250;
            row["Dec"] = "250";
            row["Hex"] = "FA";
            row["IsCritical"] = false;
            row["AttributeName"] = "Read Error Retry Rate";
            row["Description"] = "Number of errors detected while reading from a disk.";
            hddDefinitions.Rows.Add(row);

            row = hddDefinitions.NewRow();
            row["Key"] = 254;
            row["Dec"] = "254";
            row["Hex"] = "FE";
            row["IsCritical"] = false;
            row["AttributeName"] = "Free Fall Protection";
            row["Description"] = "Number of \"free fall events\" detected.";
            hddDefinitions.Rows.Add(row);

            hddDefinitions.AcceptChanges();
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.SmartHddDefinitions.PopulateHddDataTable");
        }

        public DataTable Definitions
        {
            get
            {
                return hddDefinitions;
            }
        }
    }
}
