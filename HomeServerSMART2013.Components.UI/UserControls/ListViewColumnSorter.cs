using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls
{
    public sealed class ListViewColumnSorter : WSSControls.BelovedComponents.ListViewColumnSorter
    {
        public override int Compare(object x, object y)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.UserControls.ListViewColumnSorter.Compare");
            ListViewItem item1 = (ListViewItem)x;
            ListViewItem item2 = (ListViewItem)y;

            int result = 0;
            try
            {
                if (item1.SubItems[SortColumn].Tag != null && item2.SubItems[SortColumn].Tag != null)
                {
                    try
                    {
                        SiAuto.Main.LogMessage("Attempting decimal sort on non-null Tag field.");
                        result = ((Decimal)item1.SubItems[SortColumn].Tag).CompareTo((Decimal)item2.SubItems[SortColumn].Tag);
                        SiAuto.Main.LogMessage("Decimal sort was successful; the data was numeric and was sorted by number.");
                    }
                    catch
                    {
                        SiAuto.Main.LogMessage("Decimal sort failed; the data was not numeric and will be sorted by text.");
                        result = String.Compare((String)item1.SubItems[SortColumn].Tag, (String)item2.SubItems[SortColumn].Tag);
                    }
                }
                else
                {
                    SiAuto.Main.LogMessage("Tag field was null so sorting by text.");
                    result = String.Compare(item1.SubItems[SortColumn].Text, item2.SubItems[SortColumn].Text);
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogWarning("Compare Error: " + ex.Message);
            }

            if (Order == SortOrder.Ascending)
            {
                SiAuto.Main.LogInt("result", result);
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.UserControls.ListViewColumnSorter.Compare");
                return result;
            }
            else if (Order == SortOrder.Descending)
            {
                SiAuto.Main.LogInt("result (negated)", -result);
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.UserControls.ListViewColumnSorter.Compare");
                return -result;
            }
            else
            {
                SiAuto.Main.LogInt("result (zero)", 0);
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.UserControls.ListViewColumnSorter.Compare");
                return 0;
            }
        }
    }
}
