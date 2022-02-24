using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.WindowsServerSolutions.Administration.ObjectModel;

using DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UI.UserControls;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI
{
    public class HssMainUiSubTabPage : ControlRendererPage
    {
        public HssMainUiSubTabPage()
            : base(new Guid("3484e5ea-9129-42f1-ac1c-d38beaba6eb1"), // Put your fixed, static guid here
                      "Server Disk Health",
                      "View the detailed SMART data and health analysis for hard drives and SSDs installed in your Server.") { }

        protected override ControlRendererPageContent CreateContent()
        {
            return ControlRendererPageContent.Create(new MainUiControl(null, true));
        }
    }
}
