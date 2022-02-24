using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components.UserControls
{
    public sealed class UserObject
    {
        private Guid userGuid;
        private String userName;
        private String companyName;
        private String emailAddress;
        private String programType;
        private bool displayUser;
        private bool displayCompany;
        private bool isValid;

        public UserObject(Guid refGuid, String name, String company, String email, String progType, bool showUser, bool showCompany)
        {
            // Set email to empty string to designate a trial (IsValid = false).
            userGuid = refGuid;
            userName = name;
            companyName = company;
            emailAddress = email;
            programType = progType;
            displayUser = showUser;
            displayCompany = showCompany;

            if (String.IsNullOrEmpty(email))
            {
                isValid = false;
            }
            else
            {
                isValid = true;
            }
        }

        public Guid UserGuid
        {
            get
            {
                return userGuid;
            }
        }

        public String UserName
        {
            get
            {
                return userName;
            }
        }

        public String Company
        {
            get
            {
                return companyName;
            }
        }

        public String EmailAddress
        {
            get
            {
                return emailAddress;
            }
        }

        public String ProgramType
        {
            get
            {
                return programType;
            }
        }

        public bool DisplayUser
        {
            get
            {
                return displayUser;
            }
        }

        public bool DisplayCompany
        {
            get
            {
                return displayCompany;
            }
        }

        public bool IsValid
        {
            get
            {
                return isValid;
            }
        }
    }
}
