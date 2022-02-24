using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Utilities
{
    public sealed class UpdateInfo
    {
        String currentVersion;
        String availableVersion;
        String directDownloadUrl;
        String moreInfoUrl;
        String latestReleaseDate;
        bool isNewerAvailable;
        bool isExceptionThrown;
        Exception errorInfo;

        /// <summary>
        /// Initializes a new UpdateInfo object with the default values.
        /// </summary>
        public UpdateInfo()
        {
            currentVersion = String.Empty;
            availableVersion = String.Empty;
            directDownloadUrl = String.Empty;
            moreInfoUrl = String.Empty;
            latestReleaseDate = String.Empty;
            isNewerAvailable = false;
            isExceptionThrown = false;
            errorInfo = null;
        }

        /// <summary>
        /// Checks the udpate status based on supplied information. The IsUpdateAvailable property will be set to true if
        /// an update is available.
        /// </summary>
        public void Check()
        {
            //if (currentVersion == availableVersion)
            //{
            //    isNewerAvailable = false;
            //}
            //else
            //{
            //    isNewerAvailable = true;
            //}
            try
            {
                Version current = new Version(currentVersion);
                Version available = new Version(availableVersion);
                var result = current.CompareTo(available);
                isNewerAvailable = result < 0;
            }
            catch
            {
                isNewerAvailable = false;
            }
        }

        public String CurrentVersion
        {
            get
            {
                return currentVersion;
            }
            set
            {
                currentVersion = value;
            }
        }

        public String AvailableVersion
        {
            get
            {
                return availableVersion;
            }
            set
            {
                availableVersion = value;
            }
        }

        public String DirectDownloadUrl
        {
            get
            {
                return directDownloadUrl;
            }
            set
            {
                directDownloadUrl = value;
            }
        }

        public String MoreInfoUrl
        {
            get
            {
                return moreInfoUrl;
            }
            set
            {
                moreInfoUrl = value;
            }
        }

        public String ReleaseDate
        {
            get
            {
                return latestReleaseDate;
            }
            set
            {
                latestReleaseDate = value;
            }
        }

        public bool IsUpdateAvailable
        {
            get
            {
                return isNewerAvailable;
            }
        }

        public bool IsError
        {
            get
            {
                return isExceptionThrown;
            }
            set
            {
                isExceptionThrown = true;
            }
        }

        public Exception ErrorInfo
        {
            get
            {
                return errorInfo;
            }
            set
            {
                errorInfo = value;
            }
        }
    }
}
