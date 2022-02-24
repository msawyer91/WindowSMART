using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class IdentifySsdManufacturer
    {
        public static string SOLID_STATE_DISK_GLOBAL_ID = "Sl0bberhe@d";

        public static SsdManufacturer GetSsdManufacturerFromSmartAttributes(byte[] smartData, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
            int[] attributeList = new int[30];

            for (int i = 0; i < 30; i++)
            {
                int offset = 12 * i;

                // Attribute ID: 2
                attributeList[i] = smartData[offset + 2];
                if (attributeList[i] == 0)
                {
                    // Done!
                    break;
                }
            }

            // Must do Micron before SandForce because SandForce and Micron all start 1, 5, 9, 12, 170; this would cause
            // a Micron controller to be registered as a SandForce. Micron has 171, 172, 173 and more so get it first.
            // No overlap on other controllers so we can process them in any order. They're listed below in the order of
            // likelihood for best performance, at least SandForce, Intel and Samsung.
            if (IsSsdKingSpec(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as LAMD.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_KINGSPEC;
            }
            else if (IsSsdMicron(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as Micron.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_MICRON;
            }
            else if (IsSsdLamd(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as LAMD.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_LAMD;
            }
            else if (IsSsdSandForce(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as SandForce.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_SANDFORCE;
            }
            else if (IsSsdSamsung(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as Samsung.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_SAMSUNG;
            }
            else if (IsSsdIntel(attributeList, model)) // Intel matches first four Samsung 05, 09, 0C, AA on shorter pattern so Intel checked AFTER Samsung
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as Intel.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_INTEL;
            }
            else if (IsSsdIndilinx(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as Indilinx.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_INDILINX;
            }
            else if (IsSsdEverest(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as Indilinx Everest.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_EVEREST;
            }
            else if (IsSsdBarefoot3(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as Indilinx Barefoot 3.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_BAREFOOT3;
            }
            else if (IsSsdToshiba(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as Toshiba.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_TOSHIBA;
            }
            else if (IsSsdJMicron(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as JMicron.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_JMICRON;
            }
            else if (IsSsdMarvell(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as Marvell.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_MARVELL;
            }
            else if (IsSsdSmartModular(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as SMART Modular.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_SMART_MOD;
            }
            else if (IsSsdStec(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as Stec.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_STEC;
            }
            else if (IsSsdLamd(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as LAMD.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_LAMD;
            }
            else if (IsSsdKingston(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as LAMD.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_KINGSTON;
            }
            else if (IsSsdTranscend(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as Transcend/Silicon Motion.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_TRANSCEND;
            }
            else if (IsSsdPhison(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as LAMD.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_PHISON;
            }
            else if (IsSsdAdata(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as LAMD.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_ADATA;
            }
            else if (IsSsdSanDisk(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as LAMD.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_SANDISK;
            }
            else if (IsSsdSkHynix(attributeList, model))
            {
                SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as SK hynix.");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
                return SsdManufacturer.SSD_MANUFACTURER_SKHYNIX;
            }

            // Didn't match to anything, so handle as a generic.
            SiAuto.Main.LogColored(System.Drawing.Color.LightBlue, "[SSD Identification] Controller identified as Unknown/Generic.");
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.GetSsdManufacturerFromSmartAttributes");
            return SsdManufacturer.SSD_MANUFACTURER_OTHER;
        }

        /// <summary>
        /// Returns whether or not an SSD uses a SandForce controller based on its SMART data.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if SandForce; false otherwise.</returns>
        private static bool IsSsdSandForce(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSandForce");
            if (attributeList[0] == 1 && attributeList[1] == 5 && attributeList[2] == 9 &&
                attributeList[3] == 12 && attributeList[4] == 170)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 5, 9, 12, 170 - SandForce");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSandForce");
                return true;
            }
            // new 1/8/18
            if (attributeList[0] == 1 && attributeList[1] == 5 && attributeList[2] == 9 &&
                attributeList[3] == 12 && attributeList[4] == 13 && attributeList[5] == 100 && 
                attributeList[6] == 170)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 5, 9, 12, 13, 100, 170 - SandForce");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSandForce");
                return true;
            }
            // new 1/8/18
            if (attributeList[0] == 1 && attributeList[1] == 2 && attributeList[2] == 3 &&
                attributeList[3] == 5 && attributeList[4] == 7 && attributeList[5] == 8 &&
                attributeList[6] == 9 && attributeList[7] == 10 && attributeList[8] == 12 &&
                attributeList[9] == 167 && attributeList[10] == 168 && attributeList[11] == 169 &&
                attributeList[12] == 170 && attributeList[13] == 173 && attributeList[14] == 175 &&
                attributeList[15] == 177)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 2, 3, 5, 7, 8, 9, 10, 12, 167, 168, 169, 170, 173, 175, 177 - SandForce");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSandForce");
                return true;
            }
            // This should be Micron, not SandForce 1/8/18
            //if (attributeList[0] == 1 && attributeList[1] == 5 && attributeList[2] == 9 &&
            //    attributeList[3] == 12 && attributeList[4] == 171 && attributeList[5] == 172
            //    && !model.Contains("CRUCIAL"))
            //{
            //    SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 5, 9, 12, 171, 172 - SandForce");
            //    SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSandForce");
            //    return true;
            //}
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSandForce");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses a LAMD controller based on its SMART data.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if LAMD; false otherwise.</returns>
        private static bool IsSsdLamd(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdLamd");
            if (attributeList[0] == 1 && attributeList[1] == 5 && attributeList[2] == 9 &&
                attributeList[3] == 12 && attributeList[4] == 171 && attributeList[5] == 172 &&
                attributeList[6] == 181)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 5, 9, 12, 171, 172, 181 - LAMD");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdLamd");
                return true;
            }
            if (attributeList[0] == 1 && attributeList[1] == 5 && attributeList[2] == 9 &&
                attributeList[3] == 11 && attributeList[4] == 12 && attributeList[5] == 174)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 5, 9, 11, 12, 174 - LAMD");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdLamd");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdLamd");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses an Intel controller based on its SMART data.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if Intel; false otherwise.</returns>
        private static bool IsSsdIntel(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdIntel");
            if (attributeList[0] == 3 && attributeList[1] == 4 && attributeList[2] == 5 &&
                attributeList[3] == 9 && attributeList[4] == 12 && attributeList[5] == 192 &&
                ((attributeList[6] == 232 && attributeList[7] == 233) || attributeList[6] == 225))
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 3, 4, 5, 9, 12, 192, ((232, 233) | 225) - Intel");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdIntel");
                return true;
            }
            if (attributeList[0] == 3 && attributeList[1] == 4 && attributeList[2] == 5 &&
                attributeList[3] == 9 && attributeList[4] == 12 && attributeList[5] == 170)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 3, 4, 5, 9, 12, 170 - Intel");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdIntel");
                return true;
            }
            // Unsure of this one - 1/8/18
            if (attributeList[0] == 5 && attributeList[1] == 9 && attributeList[2] == 12 &&
                attributeList[3] == 170)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 5, 9, 12, 170 - Intel");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdIntel");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdIntel");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses a Samsung controller based on its SMART data.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if Samsung; false otherwise.</returns>
        private static bool IsSsdSamsung(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSamsung");
            // added attributes B3, B5, B6 1/8/18
            if (attributeList[0] == 5 && attributeList[1] == 9 && attributeList[2] == 12 &&
                attributeList[3] == 177 && attributeList[4] == 179 && attributeList[5] == 181 &&
                attributeList[6] == 182)
            {
                // SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 5, 9, 12, 177 - Samsung"); 1/8/18
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 5, 9, 12, 177, 179, 181, 182 - Samsung");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSamsung");
                return true;
            }
            if (attributeList[0] == 9 && attributeList[1] == 12 && attributeList[2] == 178 &&
                attributeList[3] == 179 && attributeList[4] == 180)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 9, 12, 178, 179, 180 - Samsung");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSamsung");
                return true;
            }
            if (attributeList[0] == 9 && attributeList[1] == 12 && attributeList[2] == 175 &&
                attributeList[3] == 176 && attributeList[4] == 177 && attributeList[5] == 178 &&
                attributeList[6] == 179 && attributeList[7] == 180)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 9, 12, 175, 176, 177, 178, 179, 180 - Samsung");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSamsung");
                return true;
            }
            // new 1/8/18
            if (attributeList[0] == 5 && attributeList[1] == 9 && attributeList[2] == 12 &&
                attributeList[3] == 170 && attributeList[4] == 171 && attributeList[5] == 172 &&
                attributeList[6] == 173 && attributeList[7] == 174 && attributeList[8] == 178 &&
                attributeList[9] == 180)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 5, 9, 12, 170, 171, 172, 173, 174, 178, 180 - Samsung");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSamsung");
                return true;
            }
            // new 1/8/18
            if (attributeList[0] == 9 && attributeList[1] == 12 && attributeList[2] == 177 &&
                attributeList[3] == 178 && attributeList[4] == 179 && attributeList[5] == 180 &&
                attributeList[6] == 183)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 9, 12, 177, 178, 179, 180, 183 - Samsung");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSamsung");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSamsung");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses an Indilinx controller based on its SMART data.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if Indilinx; false otherwise.</returns>
        private static bool IsSsdIndilinx(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdIndilinx");
            // added attributes C3, C4 1/8/18
            if (attributeList[0] == 1 && attributeList[1] == 9 && attributeList[2] == 12 &&
                attributeList[3] == 184 && attributeList[4] == 195 && attributeList[5] == 196)
            {
                // SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 9, 12, 184 - Indilinx"); 1/8/18
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 9, 12, 184, 195, 196 - Indilinx");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdIndilinx");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdIndilinx");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses an Everest (officially Indilinx Everest) controller based on its SMART data.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if Indilinx; false otherwise.</returns>
        private static bool IsSsdEverest(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdEverest");
            // added attributes 09, 0C, E8, E9 1/8/18
            if (attributeList[0] == 1 && attributeList[1] == 3 && attributeList[2] == 4 &&
                attributeList[3] == 5 && attributeList[4] == 9 && attributeList[5] == 12 &&
                attributeList[6] == 232 && attributeList[7] == 233)
            {
                // SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 3, 4, 5 - Indilinx Everest"); 1/8/18
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 3, 4, 5, 9, 12, 232, 233 - Indilinx Everest");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdEverest");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdEverest");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses an Indilinx Barefoot 3 controller based on its SMART data.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if Indilinx BF3; false otherwise.</returns>
        private static bool IsSsdBarefoot3(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdBarefoot3");
            // added atributes AE, C3, C4, C5, C6 1/8/18
            if (attributeList[0] == 5 && attributeList[1] == 9 && attributeList[2] == 12 &&
                attributeList[3] == 171 && attributeList[4] == 174 && attributeList[5] == 195 &&
                attributeList[6] == 196 && attributeList[7] == 197 && attributeList[8] == 198)
            {
                // SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 5, 9, 12, 171 - Indilinx Barefoot 3"); 1/8/18
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 5, 9, 12, 171, 174, 195, 196, 197, 198 - Indilinx Barefoot 3");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdBarefoot3");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdBarefoot3");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses a JMicron controller based on its SMART data.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if JMicron; false otherwise.</returns>
        private static bool IsSsdToshiba(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdToshiba");
            if (attributeList[0] == 9 && attributeList[1] == 12 && attributeList[2] == 194 &&
                attributeList[3] == 229 && attributeList[4] == 232 && attributeList[5] == 233 &&
                model.ToUpper().Contains("TOSHIBA"))
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 9, 12, 194, 229, 232, 233 - Toshiba");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdJMicron");
                return true;
            }
            // added attributes A8, AF, C0, C2 1/18/2018, but this caused issues so added original method below
            if (attributeList[0] == 1 && attributeList[1] == 2 && attributeList[2] == 3 &&
                attributeList[3] == 5 && attributeList[4] == 7 && attributeList[5] == 8 &&
                attributeList[6] == 9 && attributeList[7] == 10 && attributeList[8] == 12 &&
                attributeList[9] == 168 && attributeList[10] == 175 && attributeList[11] == 192 &&
                attributeList[12] == 194 && model.ToUpper().Contains("TOSHIBA"))
            {
                // SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 2, 3, 5, 7, 8, 9, 10, 12 - Toshiba"); 1/8/18
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 2, 3, 5, 7, 8, 9, 10, 12, 168, 175, 192, 194 - Toshiba");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdToshiba");
                return true;
            }
            // this was the original method; restored 1/9/18 - issue identified on a Toshiba SSD
            if (attributeList[0] == 1 && attributeList[1] == 2 && attributeList[2] == 3 &&
                attributeList[3] == 5 && attributeList[4] == 7 && attributeList[5] == 8 &&
                attributeList[6] == 9 && attributeList[7] == 10 && attributeList[8] == 12 &&
                model.ToUpper().Contains("TOSHIBA"))
            {
                // SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 2, 3, 5, 7, 8, 9, 10, 12 - JMicron"); 1/8/18
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 2, 3, 5, 7, 8, 9, 10, 12 - Toshiba");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdToshiba");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdToshiba");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses a JMicron controller based on its SMART data.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if JMicron; false otherwise.</returns>
        private static bool IsSsdJMicron(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdJMicron");
            if (attributeList[0] == 9 && attributeList[1] == 12 && attributeList[2] == 194 &&
                attributeList[3] == 229 && attributeList[4] == 232 && attributeList[5] == 233)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 9, 12, 194, 229, 232, 233 - JMicron");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdJMicron");
                return true;
            }
            // added attributes A8, AF, C0, C2 1/18/2018, but this caused issues so added original method below
            if (attributeList[0] == 1 && attributeList[1] == 2 && attributeList[2] == 3 &&
                attributeList[3] == 5 && attributeList[4] == 7 && attributeList[5] == 8 &&
                attributeList[6] == 9 && attributeList[7] == 10 && attributeList[8] == 12 &&
                attributeList[9] == 168 && attributeList[10] == 175 && attributeList[11] == 192 &&
                attributeList[12] == 194)
            {
                // SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 2, 3, 5, 7, 8, 9, 10, 12 - JMicron"); 1/8/18
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 2, 3, 5, 7, 8, 9, 10, 12, 168, 175, 192, 194 - JMicron");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdJMicron");
                return true;
            }
            // this was the original method; restored 1/9/18 - issue identified on a Toshiba SSD
            if (attributeList[0] == 1 && attributeList[1] == 2 && attributeList[2] == 3 &&
                attributeList[3] == 5 && attributeList[4] == 7 && attributeList[5] == 8 &&
                attributeList[6] == 9 && attributeList[7] == 10 && attributeList[8] == 12)
            {
                // SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 2, 3, 5, 7, 8, 9, 10, 12 - JMicron"); 1/8/18
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 2, 3, 5, 7, 8, 9, 10, 12 - JMicron");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdJMicron");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdJMicron");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses a Micron controller based on its SMART data.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if Micron; false otherwise.</returns>
        private static bool IsSsdMicron(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdMicron");
            if (attributeList[0] == 1 && attributeList[1] == 5 && attributeList[2] == 9 &&
                attributeList[3] == 12 && attributeList[4] == 170 && attributeList[5] == 171 &&
                attributeList[6] == 172 && attributeList[7] == 173 && attributeList[8] == 174 &&
                attributeList[9] == 181 && attributeList[10] == 183)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 5, 9, 12, 170, 171, 172, 173, 174, 181, 183 - Micron");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdMicron");
                return true;
            }
            // new 1/8/18 Micron MU02
            if (attributeList[0] == 1 && attributeList[1] == 5 && attributeList[2] == 9 &&
                attributeList[3] == 12 && attributeList[4] == 160 && attributeList[5] == 161 &&
                attributeList[6] == 163 && attributeList[7] == 164 && attributeList[8] == 165 &&
                attributeList[9] == 166 && attributeList[10] == 167)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 5, 9, 12, 170, 171, 172, 173, 174, 181, 183 - Micron");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdMicron");
                return true;
            }
            // new 1/8/18 Micron MU02
            if (attributeList[0] == 1 && attributeList[1] == 5 && attributeList[2] == 9 &&
                attributeList[3] == 12 && attributeList[4] == 160 && attributeList[5] == 161 &&
                attributeList[6] == 163 && attributeList[7] == 148 && attributeList[8] == 149 &&
                attributeList[9] == 150 && attributeList[10] == 151)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 5, 9, 12, 170, 171, 172, 173, 174, 181, 183 - Micron");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdMicron");
                return true;
            }
            // The below was also in SandForce but believed to be Micron
            if (attributeList[0] == 1 && attributeList[1] == 5 && attributeList[2] == 9 &&
                attributeList[3] == 12 && attributeList[4] == 171 && attributeList[5] == 172
                && model.Contains("CRUCIAL"))
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 5, 9, 12, 171, 172 and Crucial - Micron");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdMicron");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdMicron");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses a SMART Modular controller based on its SMART data.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if SMART Modular; false otherwise.</returns>
        private static bool IsSsdSmartModular(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSmartModular");
            if (attributeList[0] == 9 && attributeList[1] == 232 && attributeList[2] == 233 &&
                attributeList[3] == 234 && attributeList[4] == 235)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 9, 232, 233, 234, 235 - SMART Modular");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSmartModular");
                return true;
            }
            if (attributeList[0] == 9 && attributeList[1] == 12 && attributeList[2] == 192 &&
                attributeList[3] == 197 && attributeList[4] == 198 && attributeList[5] == 199 &&
                attributeList[6] == 248 && attributeList[7] == 252 && attributeList[8] == 254)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 9, 12, 191, 197, 198, 199, 248, 252, 254 - SMART Modular");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSmartModular");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSmartModular");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses a STEC controller based on its SMART data.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if STEC; false otherwise.</returns>
        private static bool IsSsdStec(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSmartStec");
            if (attributeList[0] == 1 && attributeList[1] == 2 && attributeList[2] == 5 &&
                attributeList[3] == 9 && attributeList[4] == 12 && attributeList[5] == 13 &&
                attributeList[6] == 100)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 2, 5, 9, 12, 13, 100 - STEC");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSmartStec");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSmartStec");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses a Marvell controller based on its SMART data. (Common in Plextor SSDs)
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if Marvell; false otherwise.</returns>
        private static bool IsSsdMarvell(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdMarvell");
            // added attributes 178, 181, 182 1/8/18
            if (attributeList[0] == 1 && attributeList[1] == 5 && attributeList[2] == 9 &&
                attributeList[3] == 12 && attributeList[4] == 177 && attributeList[5] == 178 &&
                attributeList[6] == 181 && attributeList[7] == 182)
            {
                // SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 5, 9, 12, 177 - Marvell/Plextor"); 1/8/18
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 5, 9, 12, 177, 178, 181, 182 - Marvell/Plextor");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdMarvell");
                return true;
            }            
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdMarvell");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses a Kingston-Phison controller based on its SMART data. Added 1/8/18.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if Kingston; false otherwise.</returns>
        private static bool IsSsdKingston(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdKingston");
            // added attributes 178, 181, 182 1/8/18
            if (attributeList[0] == 1 && attributeList[1] == 2 && attributeList[2] == 3 &&
                attributeList[3] == 5 && attributeList[4] == 7 && attributeList[5] == 8 &&
                attributeList[6] == 9 && attributeList[7] == 10 && attributeList[8] == 12 &&
                attributeList[9] == 168)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 2, 3, 5, 7, 8, 9, 10, 12, 168 - Kingston-Phison");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdKingston");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdKingston");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses a Transcent/Silicon Motion controller based on its SMART data.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if Transcent/Silicon Motion; false otherwise.</returns>
        private static bool IsSsdTranscend(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdTranscend");
            // added attributes 178, 181, 182 1/8/18
            if (attributeList[0] == 1 && attributeList[1] == 5 && attributeList[2] == 9 &&
                attributeList[3] == 12 && attributeList[4] == 160 && attributeList[5] == 161 &&
                attributeList[6] == 162 && attributeList[7] == 163 && attributeList[8] == 164 &&
                attributeList[9] == 165 && attributeList[10] == 166 && attributeList[11] == 167 &&
                attributeList[12] == 168 && attributeList[13] == 169 && attributeList[14] == 192)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 5, 9, 12, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 192 - Transcend/Silicon Motion");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdTranscend");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdTranscend");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses a KingSpec controller based on its SMART data. Added 1/8/18.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if KingSpec; false otherwise.</returns>
        private static bool IsSsdKingSpec(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdKingSpec");
            if (attributeList[0] == 1 && attributeList[1] == 5 && attributeList[2] == 9 &&
                attributeList[3] == 12 && attributeList[4] == 160 && attributeList[5] == 161 &&
                attributeList[6] == 163 && attributeList[7] == 164 && attributeList[8] == 165 &&
                attributeList[9] == 166 && attributeList[10] == 167 && attributeList[11] == 192)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 5, 9, 12, 160, 161, 163, 164, 165, 166, 167, 192 - KingSpec");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdKingSpec");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdKingSpec");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses a Smartbuy/Phison controller based on its SMART data. Added 1/8/18.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if Smartbuy/Phison; false otherwise.</returns>
        private static bool IsSsdPhison(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdPhison");
            if (attributeList[0] == 1 && attributeList[1] == 9 && attributeList[2] == 12 &&
                attributeList[3] == 168 && attributeList[4] == 170)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 9, 12, 168, 170 - Smartbuy/Phison");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdPhison");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdPhison");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses an ADATA controller based on its SMART data. Added 1/8/18.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if ADATA; false otherwise.</returns>
        private static bool IsSsdAdata(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdAdata");
            if (attributeList[0] == 9 && attributeList[1] == 12 && attributeList[2] == 167 &&
                attributeList[3] == 168 && attributeList[4] == 169)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 12, 167, 168, 169 - ADATA");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdAdata");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdAdata");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses a SanDisk controller based on its SMART data. Added 1/8/18.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if SanDisk; false otherwise.</returns>
        private static bool IsSsdSanDisk(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSanDisk");
            if (attributeList[0] == 5 && attributeList[1] == 9 && attributeList[2] == 12 &&
                attributeList[3] == 165)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 5, 9, 12, 165 - SanDisk");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSanDisk");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSanDisk");
            return false;
        }

        /// <summary>
        /// Returns whether or not an SSD uses an SK hynix controller based on its SMART data. Added 1/11/18.
        /// </summary>
        /// <param name="attributeList">List of attribute IDs.</param>
        /// <returns>true if SK hynix; false otherwise.</returns>
        private static bool IsSsdSkHynix(int[] attributeList, String model)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSkHynix");
            if (attributeList[0] == 1 && attributeList[1] == 5 && attributeList[2] == 9 &&
                attributeList[3] == 12 && attributeList[4] == 100)
            {
                SiAuto.Main.LogMessage("[SSD Identification] Pattern match on attributes 1, 5, 9, 12, 100 - SK hynix");
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSkHynix");
                return true;
            }
            SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.IdentifySsdManufacturer.IsSsdSkHynix");
            return false;
        }
    }

    public enum SsdManufacturer : int
    {
        SSD_MANUFACTURER_SANDFORCE = 0,
        SSD_MANUFACTURER_INTEL = 1,
        SSD_MANUFACTURER_SAMSUNG = 2,
        SSD_MANUFACTURER_MICRON = 3,
        SSD_MANUFACTURER_JMICRON = 4,
        SSD_MANUFACTURER_STEC = 5,
        SSD_MANUFACTURER_SMART_MOD = 6,
        SSD_MANUFACTURER_INDILINX = 7,
        SSD_MANUFACTURER_EVEREST = 8,
        SSD_MANUFACTURER_OTHER = 9,
        SSD_MANUFACTURER_MARVELL = 10,
        SSD_MANUFACTURER_BAREFOOT3 = 11,
        SSD_MANUFACTURER_LAMD = 12,
        SSD_MANUFACTURER_SKHYNIX = 13,
        SSD_MANUFACTURER_KINGSTON = 14,
        SSD_MANUFACTURER_TRANSCEND = 15,
        SSD_MANUFACTURER_KINGSPEC = 16,
        SSD_MANUFACTURER_PHISON = 17,
        SSD_MANUFACTURER_ADATA = 18,
        SSD_MANUFACTURER_SANDISK = 19,
        SSD_MANUFACTURER_TOSHIBA = 20,
        SSD_MANUFACTURER_HARD_DISK
    }
}
