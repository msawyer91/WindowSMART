using System;
using System.Reflection;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.Debugging
{
    public sealed class AssemblyInformation
    {
        private String version;
        private String name;
        private String fullName;
        private String errorMessage;
        private bool isDetected;
        private AssemblyName[] references;

        public AssemblyInformation()
        {
            version = String.Empty;
            name = String.Empty;
            fullName = String.Empty;
            errorMessage = "AssemblyInformation object is initialized.";
            isDetected = false;
            references = null;
        }

        public String Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public String FullName
        {
            get
            {
                return fullName;
            }
            set
            {
                fullName = value;
            }
        }

        public String ErrorMessage
        {
            get
            {
                return errorMessage;
            }
            set
            {
                errorMessage = value;
            }
        }

        public bool IsDetected
        {
            get
            {
                return isDetected;
            }
            set
            {
                isDetected = value;
            }
        }

        public AssemblyName[] ReferencedAssemblies
        {
            get
            {
                return references;
            }
            set
            {
                references = value;
            }
        }
    }
}
