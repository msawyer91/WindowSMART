using System;
using System.Collections.Generic;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    public sealed class EncryptionDetails
    {
        private bool createBek;
        private bool createPassword;
        private String bekFilePath;
        private String driveLetter;
        private EncryptionMethod method;

        public EncryptionDetails(bool bek, bool password, String path, String letter, EncryptionMethod em)
        {
            createBek = bek;
            createPassword = password;
            bekFilePath = path;
            driveLetter = letter;
            method = em;
        }

        public bool CreateBek
        {
            get
            {
                return createBek;
            }
        }

        public bool CreatePassword
        {
            get
            {
                return createPassword;
            }
        }

        public String BekFilePath
        {
            get
            {
                return bekFilePath;
            }
        }

        public String DriveLetter
        {
            get
            {
                return driveLetter;
            }
        }

        public EncryptionMethod Method
        {
            get
            {
                return method;
            }
        }
    }
}
