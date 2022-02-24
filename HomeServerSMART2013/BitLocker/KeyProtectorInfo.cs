using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public sealed class KeyProtectorInfo
    {
        private KeyProtectorTypes protectorType;
        private String protectorID;
        private String protectorFriendlyName;
        private String protectorFileName;
        private String protectorPassword;

        public KeyProtectorInfo(KeyProtectorTypes type, String id, String friendlyName)
        {
            protectorType = type;
            protectorID = id;
            if (friendlyName.Trim() == String.Empty)
            {
                protectorFriendlyName = "N/A";
            }
            else
            {
                protectorFriendlyName = friendlyName;
            }
            protectorFileName = String.Empty;
            protectorPassword = String.Empty;
        }

        public KeyProtectorTypes Type
        {
            get
            {
                return protectorType;
            }
        }

        public String ID
        {
            get
            {
                return protectorID;
            }
        }

        public String FriendlyName
        {
            get
            {
                return protectorFriendlyName;
            }
        }

        public String BekFileName
        {
            get
            {
                return protectorFileName;
            }
            set
            {
                protectorFileName = value;
            }
        }

        public String Password
        {
            get
            {
                return protectorPassword;
            }
            set
            {
                protectorPassword = value;
            }
        }
    }
}
