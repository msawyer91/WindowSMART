using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gurock.SmartInspect;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    public sealed class License
    {
        public static string PUBLIC_KEY = "<RSAKeyValue><Modulus>0tcBDxlYmuFMGzEqjW388JehmRcvJIEk1SgCvbfOnjaqidDX8fB0Ftm4ituefgMusy0dKn6v47VBIdypqQSQHqW8M9+v1+ZTlt6YfCkvGCKb/PNTGG6UqW4jdy3RarPF510rah+sc6pYaj/bcQWp1D3xcqm5tJK18QgVrd564TC1CiLARirym9BiMhFVgxC31rFd74J+5L7AtX+oN3KbBChJakZzDan6HsuRvmTVx/RDkaohgarzCTSLpe6S10Z2kJPk8uWnxVVl7qJ5kwFIqtnT2KAlbFD1x0XTc083nXvfHNjoX8cJI0R2fqcJsxGmy4jv+w503NO1wDq8sFy67w==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        // Beta Only!
        public static DateTime GetExpirationDate()
        {
            //SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.License.GetExpirationDate");
            //DateTime expirationDate = new DateTime(2012, 7, 31, 23, 59, 59, DateTimeKind.Local);
            //SiAuto.Main.LogDateTime("expirationDate", expirationDate);
            //SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.License.GetExpirationDate");
            //return expirationDate;
            return new DateTime(2099, 12, 31, 23, 59, 59, DateTimeKind.Local);
        }

        public static bool IsExpired(DateTime dateToCheck, object componentToLicense, bool throwException)
        {
            SiAuto.Main.EnterMethod("HomeServerSMART2013.Components.License.IsExpired");
            SiAuto.Main.LogDateTime("dateToCheck", dateToCheck);
            SiAuto.Main.LogObjectValue("componentToLicense", componentToLicense);
            SiAuto.Main.LogBool("throwException (if expired)", throwException);
            if (DateTime.Now > dateToCheck)
            {
                SiAuto.Main.LogError("The license is expired.");
                if (throwException)
                {
                    SiAuto.Main.LogColored(System.Drawing.Color.Red, "The license is expired, and throwException is set to true. A LicenseException will be thrown, " +
                        "and the application or service will likely terminate.");
                    SiAuto.Main.LogFatal("[Cataclysmic] License is expired; throwException is set to true.");
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.License.IsExpired");
                    throw new System.ComponentModel.LicenseException(typeof(License), componentToLicense, "The application license has expired. Expiration: " +
                        dateToCheck.ToString());
                }
                else
                {
                    SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.License.IsExpired");
                    return true;
                }
            }
            else
            {
                SiAuto.Main.LeaveMethod("HomeServerSMART2013.Components.License.IsExpired");
                return false;
            }
        }
    }
}
