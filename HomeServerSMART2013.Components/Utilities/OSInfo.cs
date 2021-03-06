using System;
using System.Runtime.InteropServices;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.Components
{
    /// <summary>
    /// Provides detailed information about the host operating system.
    /// </summary>
    static public class OSInfo
    {
        #region BITS
        /// <summary>
        /// Determines if the current application is 32 or 64-bit.
        /// </summary>
        static public int Bits
        {
            get
            {
                return IntPtr.Size * 8;
            }
        }
        #endregion BITS

        #region EDITION
        static private string s_Edition;
        /// <summary>
        /// Gets the edition of the operating system running on this computer.
        /// </summary>
        static public string Edition
        {
            get
            {
                if (s_Edition != null)
                    return s_Edition;  //***** RETURN *****//

                string edition = String.Empty;

                System.OperatingSystem osVersion = Environment.OSVersion;
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
                osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

                if (GetVersionEx(ref osVersionInfo))
                {
                    int majorVersion = osVersion.Version.Major;
                    int minorVersion = osVersion.Version.Minor;
                    byte productType = osVersionInfo.wProductType;
                    short suiteMask = osVersionInfo.wSuiteMask;

                    #region VERSION 4
                    if (majorVersion == 4)
                    {
                        if (productType == VER_NT_WORKSTATION)
                        {
                            // Windows NT 4.0 Workstation
                            edition = "Workstation";
                        }
                        else if (productType == VER_NT_SERVER)
                        {
                            if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                            {
                                // Windows NT 4.0 Server Enterprise
                                edition = "Enterprise Server";
                            }
                            else
                            {
                                // Windows NT 4.0 Server
                                edition = "Standard Server";
                            }
                        }
                    }
                    #endregion VERSION 4

                    #region VERSION 5
                    else if (majorVersion == 5)
                    {
                        if (productType == VER_NT_WORKSTATION)
                        {
                            if ((suiteMask & VER_SUITE_PERSONAL) != 0)
                            {
                                // Windows XP Home Edition
                                edition = "Home";
                            }
                            else
                            {
                                // Windows XP / Windows 2000 Professional
                                edition = "Professional";
                            }
                        }
                        else if (productType == VER_NT_SERVER)
                        {
                            if (minorVersion == 0)
                            {
                                if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                                {
                                    // Windows 2000 Datacenter Server
                                    edition = "Datacenter Server";
                                }
                                else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                                {
                                    // Windows 2000 Advanced Server
                                    edition = "Advanced Server";
                                }
                                else
                                {
                                    // Windows 2000 Server
                                    edition = "Server";
                                }
                            }
                            else
                            {
                                if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                                {
                                    // Windows Server 2003 Datacenter Edition
                                    edition = "Datacenter";
                                }
                                else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                                {
                                    // Windows Server 2003 Enterprise Edition
                                    edition = "Enterprise";
                                }
                                else if ((suiteMask & VER_SUITE_BLADE) != 0)
                                {
                                    // Windows Server 2003 Web Edition
                                    edition = "Web Edition";
                                }
                                else
                                {
                                    // Windows Server 2003 Standard Edition
                                    edition = "Standard";
                                }
                            }
                        }
                    }
                    #endregion VERSION 5

                    #region VERSION 6
                    else if (majorVersion == 6)
                    {
                        int ed;
                        if (GetProductInfo(majorVersion, minorVersion,
                            osVersionInfo.wServicePackMajor, osVersionInfo.wServicePackMinor,
                            out ed))
                        {
                            switch (ed)
                            {
                                case PRODUCT_BUSINESS:
                                    edition = "Business";
                                    break;
                                case PRODUCT_BUSINESS_N:
                                    edition = "Business N";
                                    break;
                                case PRODUCT_CLUSTER_SERVER:
                                    edition = "HPC Edition";
                                    break;
                                case PRODUCT_DATACENTER_SERVER:
                                    edition = "Datacenter Server";
                                    break;
                                case PRODUCT_DATACENTER_SERVER_CORE:
                                    edition = "Datacenter Server (core installation)";
                                    break;
                                case PRODUCT_ENTERPRISE:
                                    edition = "Enterprise";
                                    break;
                                case PRODUCT_ENTERPRISE_N:
                                    edition = "Enterprise N";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER:
                                    edition = "Enterprise Server";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_CORE:
                                    edition = "Enterprise Server (core installation)";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_CORE_V:
                                    edition = "Enterprise Server without Hyper-V (core installation)";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_IA64:
                                    edition = "Enterprise Server for Itanium-based Systems";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_V:
                                    edition = "Enterprise Server without Hyper-V";
                                    break;
                                case PRODUCT_HOME_BASIC:
                                    edition = "Home Basic";
                                    break;
                                case PRODUCT_HOME_BASIC_N:
                                    edition = "Home Basic N";
                                    break;
                                case PRODUCT_HOME_PREMIUM:
                                    edition = "Home Premium";
                                    break;
                                case PRODUCT_HOME_PREMIUM_N:
                                    edition = "Home Premium N";
                                    break;
                                case PRODUCT_HYPERV:
                                    edition = "Microsoft Hyper-V Server";
                                    break;
                                case PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT:
                                    edition = "Windows Essential Business Management Server";
                                    break;
                                case PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING:
                                    edition = "Windows Essential Business Messaging Server";
                                    break;
                                case PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY:
                                    edition = "Windows Essential Business Security Server";
                                    break;
                                case PRODUCT_SERVER_FOR_SMALLBUSINESS:
                                    edition = "Windows Essential Server Solutions";
                                    break;
                                case PRODUCT_SERVER_FOR_SMALLBUSINESS_V:
                                    edition = "Windows Essential Server Solutions without Hyper-V";
                                    break;
                                case PRODUCT_SMALLBUSINESS_SERVER:
                                    edition = "Windows Small Business Server";
                                    break;
                                case PRODUCT_STANDARD_SERVER:
                                    edition = "Standard Server";
                                    break;
                                case PRODUCT_STANDARD_SERVER_CORE:
                                    edition = "Standard Server (core installation)";
                                    break;
                                case PRODUCT_STANDARD_SERVER_CORE_V:
                                    edition = "Standard Server without Hyper-V (core installation)";
                                    break;
                                case PRODUCT_STANDARD_SERVER_V:
                                    edition = "Standard Server without Hyper-V";
                                    break;
                                case PRODUCT_STARTER:
                                    edition = "Starter";
                                    break;
                                case PRODUCT_STORAGE_ENTERPRISE_SERVER:
                                    edition = "Enterprise Storage Server";
                                    break;
                                case PRODUCT_STORAGE_EXPRESS_SERVER:
                                    edition = "Express Storage Server";
                                    break;
                                case PRODUCT_STORAGE_STANDARD_SERVER:
                                    edition = "Standard Storage Server";
                                    break;
                                case PRODUCT_STORAGE_WORKGROUP_SERVER:
                                    edition = "Workgroup Storage Server";
                                    break;
                                case PRODUCT_UNDEFINED:
                                    edition = "Unknown product";
                                    break;
                                case PRODUCT_ULTIMATE:
                                    edition = "Ultimate";
                                    break;
                                case PRODUCT_ULTIMATE_N:
                                    edition = "Ultimate N";
                                    break;
                                case PRODUCT_WEB_SERVER:
                                    edition = "Web Server";
                                    break;
                                case PRODUCT_WEB_SERVER_CORE:
                                    edition = "Web Server (core installation)";
                                    break;
                                case PRODUCT_SERVER_FOUNDATION:
                                    edition = "Server Foundation";
                                    break;
                                case PRODUCT_HOME_PREMIUM_SERVER:
                                    edition = "Windows Home Server 2011";
                                    break;
                                case PRODUCT_DATACENTER_SERVER_V:
                                    edition = "Server Datacenter without Hyper-V";
                                    break;
                                case PRODUCT_DATACENTER_SERVER_CORE_V:
                                    edition = "Server Datacenter Core without Hyper-V";
                                    break;
                                case PRODUCT_STORAGE_EXPRESS_SERVER_CORE:
                                    edition = "Storage Server Express (core installation)";
                                    break;
                                case PRODUCT_STORAGE_STANDARD_SERVER_CORE:
                                    edition = "Storage Server Standard (core installation)";
                                    break;
                                case PRODUCT_STORAGE_WORKGROUP_SERVER_CORE:
                                    edition = "Storage Server Workgroup (core installation)";
                                    break;
                                case PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE:
                                    edition = "Storage Server Enterprise (core installation)";
                                    break;
                                case PRODUCT_STARTER_N:
                                    edition = "Starter N";
                                    break;
                                case PRODUCT_PROFESSIONAL:
                                    edition = "Professional";
                                    break;
                                case PRODUCT_PROFESSIONAL_E:
                                    edition = "Professional E";
                                    break;
                                case PRODUCT_PROFESSIONAL_N:
                                    edition = "Professional N";
                                    break;
                                case PRODUCT_SB_SOLUTION_SERVER:
                                    edition = "Windows Small Business Server 2011 Essentials";
                                    break;
                                case PRODUCT_SERVER_FOR_SB_SOLUTIONS:
                                    edition = "Server for SB Solutions";
                                    break;
                                case PRODUCT_STANDARD_SERVER_SOLUTIONS:
                                    edition = "Server Solutions Premium";
                                    break;
                                case PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE:
                                    edition = "Server Solutions Premium (core installation)";
                                    break;
                                case PRODUCT_SB_SOLUTION_SERVER_EM:
                                    edition = "Server for SB Solutions EM";
                                    break;
                                case PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM:
                                    edition = "Server for SB Solutions EM";
                                    break;
                                case PRODUCT_SOLUTION_EMBEDDEDSERVER:
                                    edition = "Windows MultiPoint Server";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT:
                                    edition = "Windows Essential Server Solution Management";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL:
                                    edition = "Windows Essential Server Solution Additional";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC:
                                    edition = "Windows Essential Server Solution Management SVC";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC:
                                    edition = "Windows Essential Server Solution Additional SVC";
                                    break;
                                case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE:
                                    edition = "Small Business Server Premium (core installation)";
                                    break;
                                case PRODUCT_CLUSTER_SERVER_V:
                                    edition = "Server Hyper Core V";
                                    break;
                                case PRODUCT_STARTER_E:
                                    edition = "Starter E";
                                    break;
                                case PRODUCT_HOME_BASIC_E:
                                    edition = "Home Basic E";
                                    break;
                                case PRODUCT_HOME_PREMIUM_E:
                                    edition = "Home Premium E";
                                    break;
                                case PRODUCT_ULTIMATE_E:
                                    edition = "Ultimate E";
                                    break;
                                case PRODUCT_ENTERPRISE_E:
                                    edition = "Enterprise E";
                                    break;
                                case PRODUCT_ENTERPRISE_EVALUATION:
                                    edition = "Enterprise Evaluation";
                                    break;
                                case PRODUCT_MULTIPOINT_STANDARD_SERVER:
                                    edition = "Windows MultiPoint Server Standard";
                                    break;
                                case PRODUCT_MULTIPOINT_PREMIUM_SERVER:
                                    edition = "Windows MultiPoint Server Premium";
                                    break;
                                case PRODUCT_STANDARD_EVALUATION_SERVER:
                                    edition = "Server Standard (evaluation installation)";
                                    break;
                                case PRODUCT_DATACENTER_EVALUATION_SERVER:
                                    edition = "Server Datacenter (evaluation installation)";
                                    break;
                                case PRODUCT_ENTERPRISE_N_EVALUATION:
                                    edition = "Enterprise N (evaluation installation)";
                                    break;
                                case PRODUCT_STORAGE_WORKGROUP_EVALUATION_SERVER:
                                    edition = "Storage Server Workgroup (evaluation installation)";
                                    break;
                                case PRODUCT_STORAGE_STANDARD_EVALUATION_SERVER:
                                    edition = "Storage Server Standard (evaluation installation)";
                                    break;
                                case PRODUCT_CORE:
                                    edition = "Home";
                                    break;
                                case PRODUCT_CORE_N:
                                    edition = "Home N";
                                    break;
                                case PRODUCT_CORE_COUNTRYSPECIFIC:
                                    edition = "Home China";
                                    break;
                                case PRODUCT_CORE_SINGLELANGUAGE:
                                    edition = "Home Single Language";
                                    break;
                                case PRODUCT_PROFESSIONAL_WMC:
                                    edition = "Professional with Media Center";
                                    break;
                                case PRODUCT_MOBILE_CORE:
                                    edition = "Mobile";
                                    break;
                                case PRODUCT_EDUCATION:
                                    edition = "Education";
                                    break;
                                case PRODUCT_EDUCATION_N:
                                    edition = "Education N";
                                    break;
                            }
                        }
                    }
                    #endregion VERSION 6

                    #region VERSION 10
                    else if (majorVersion == 10)
                    {
                        int ed;
                        if (GetProductInfo(majorVersion, minorVersion,
                            osVersionInfo.wServicePackMajor, osVersionInfo.wServicePackMinor,
                            out ed))
                        {
                            switch (ed)
                            {
                                case PRODUCT_BUSINESS:
                                    edition = "Business";
                                    break;
                                case PRODUCT_BUSINESS_N:
                                    edition = "Business N";
                                    break;
                                case PRODUCT_CLUSTER_SERVER:
                                    edition = "HPC Edition";
                                    break;
                                case PRODUCT_DATACENTER_SERVER:
                                    edition = "Datacenter Server";
                                    break;
                                case PRODUCT_DATACENTER_SERVER_CORE:
                                    edition = "Datacenter Server (core installation)";
                                    break;
                                case PRODUCT_ENTERPRISE:
                                    edition = "Enterprise";
                                    break;
                                case PRODUCT_ENTERPRISE_N:
                                    edition = "Enterprise N";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER:
                                    edition = "Enterprise Server";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_CORE:
                                    edition = "Enterprise Server (core installation)";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_CORE_V:
                                    edition = "Enterprise Server without Hyper-V (core installation)";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_IA64:
                                    edition = "Enterprise Server for Itanium-based Systems";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_V:
                                    edition = "Enterprise Server without Hyper-V";
                                    break;
                                case PRODUCT_HOME_BASIC:
                                    edition = "Home Basic";
                                    break;
                                case PRODUCT_HOME_BASIC_N:
                                    edition = "Home Basic N";
                                    break;
                                case PRODUCT_HOME_PREMIUM:
                                    edition = "Home Premium";
                                    break;
                                case PRODUCT_HOME_PREMIUM_N:
                                    edition = "Home Premium N";
                                    break;
                                case PRODUCT_HYPERV:
                                    edition = "Microsoft Hyper-V Server";
                                    break;
                                case PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT:
                                    edition = "Windows Essential Business Management Server";
                                    break;
                                case PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING:
                                    edition = "Windows Essential Business Messaging Server";
                                    break;
                                case PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY:
                                    edition = "Windows Essential Business Security Server";
                                    break;
                                case PRODUCT_SERVER_FOR_SMALLBUSINESS:
                                    edition = "Windows Essential Server Solutions";
                                    break;
                                case PRODUCT_SERVER_FOR_SMALLBUSINESS_V:
                                    edition = "Windows Essential Server Solutions without Hyper-V";
                                    break;
                                case PRODUCT_SMALLBUSINESS_SERVER:
                                    edition = "Windows Small Business Server";
                                    break;
                                case PRODUCT_STANDARD_SERVER:
                                    edition = "Standard Server";
                                    break;
                                case PRODUCT_STANDARD_SERVER_CORE:
                                    edition = "Standard Server (core installation)";
                                    break;
                                case PRODUCT_STANDARD_SERVER_CORE_V:
                                    edition = "Standard Server without Hyper-V (core installation)";
                                    break;
                                case PRODUCT_STANDARD_SERVER_V:
                                    edition = "Standard Server without Hyper-V";
                                    break;
                                case PRODUCT_STARTER:
                                    edition = "Starter";
                                    break;
                                case PRODUCT_STORAGE_ENTERPRISE_SERVER:
                                    edition = "Enterprise Storage Server";
                                    break;
                                case PRODUCT_STORAGE_EXPRESS_SERVER:
                                    edition = "Express Storage Server";
                                    break;
                                case PRODUCT_STORAGE_STANDARD_SERVER:
                                    edition = "Standard Storage Server";
                                    break;
                                case PRODUCT_STORAGE_WORKGROUP_SERVER:
                                    edition = "Workgroup Storage Server";
                                    break;
                                case PRODUCT_UNDEFINED:
                                    edition = "Unknown product";
                                    break;
                                case PRODUCT_ULTIMATE:
                                    edition = "Ultimate";
                                    break;
                                case PRODUCT_ULTIMATE_N:
                                    edition = "Ultimate N";
                                    break;
                                case PRODUCT_WEB_SERVER:
                                    edition = "Web Server";
                                    break;
                                case PRODUCT_WEB_SERVER_CORE:
                                    edition = "Web Server (core installation)";
                                    break;
                                case PRODUCT_SERVER_FOUNDATION:
                                    edition = "Server Foundation";
                                    break;
                                case PRODUCT_HOME_PREMIUM_SERVER:
                                    edition = "Windows Home Server 2011";
                                    break;
                                case PRODUCT_DATACENTER_SERVER_V:
                                    edition = "Server Datacenter without Hyper-V";
                                    break;
                                case PRODUCT_DATACENTER_SERVER_CORE_V:
                                    edition = "Server Datacenter Core without Hyper-V";
                                    break;
                                case PRODUCT_STORAGE_EXPRESS_SERVER_CORE:
                                    edition = "Storage Server Express (core installation)";
                                    break;
                                case PRODUCT_STORAGE_STANDARD_SERVER_CORE:
                                    edition = "Storage Server Standard (core installation)";
                                    break;
                                case PRODUCT_STORAGE_WORKGROUP_SERVER_CORE:
                                    edition = "Storage Server Workgroup (core installation)";
                                    break;
                                case PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE:
                                    edition = "Storage Server Enterprise (core installation)";
                                    break;
                                case PRODUCT_STARTER_N:
                                    edition = "Starter N";
                                    break;
                                case PRODUCT_PROFESSIONAL:
                                    edition = "Professional";
                                    break;
                                case PRODUCT_PROFESSIONAL_E:
                                    edition = "Professional E";
                                    break;
                                case PRODUCT_PROFESSIONAL_N:
                                    edition = "Professional N";
                                    break;
                                case PRODUCT_SB_SOLUTION_SERVER:
                                    edition = "Windows Small Business Server 2011 Essentials";
                                    break;
                                case PRODUCT_SERVER_FOR_SB_SOLUTIONS:
                                    edition = "Server for SB Solutions";
                                    break;
                                case PRODUCT_STANDARD_SERVER_SOLUTIONS:
                                    edition = "Server Solutions Premium";
                                    break;
                                case PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE:
                                    edition = "Server Solutions Premium (core installation)";
                                    break;
                                case PRODUCT_SB_SOLUTION_SERVER_EM:
                                    edition = "Server for SB Solutions EM";
                                    break;
                                case PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM:
                                    edition = "Server for SB Solutions EM";
                                    break;
                                case PRODUCT_SOLUTION_EMBEDDEDSERVER:
                                    edition = "Windows MultiPoint Server";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT:
                                    edition = "Windows Essential Server Solution Management";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL:
                                    edition = "Windows Essential Server Solution Additional";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC:
                                    edition = "Windows Essential Server Solution Management SVC";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC:
                                    edition = "Windows Essential Server Solution Additional SVC";
                                    break;
                                case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE:
                                    edition = "Small Business Server Premium (core installation)";
                                    break;
                                case PRODUCT_CLUSTER_SERVER_V:
                                    edition = "Server Hyper Core V";
                                    break;
                                case PRODUCT_STARTER_E:
                                    edition = "Starter E";
                                    break;
                                case PRODUCT_HOME_BASIC_E:
                                    edition = "Home Basic E";
                                    break;
                                case PRODUCT_HOME_PREMIUM_E:
                                    edition = "Home Premium E";
                                    break;
                                case PRODUCT_ULTIMATE_E:
                                    edition = "Ultimate E";
                                    break;
                                case PRODUCT_ENTERPRISE_E:
                                    edition = "Enterprise E";
                                    break;
                                case PRODUCT_ENTERPRISE_EVALUATION:
                                    edition = "Enterprise Evaluation";
                                    break;
                                case PRODUCT_MULTIPOINT_STANDARD_SERVER:
                                    edition = "Windows MultiPoint Server Standard";
                                    break;
                                case PRODUCT_MULTIPOINT_PREMIUM_SERVER:
                                    edition = "Windows MultiPoint Server Premium";
                                    break;
                                case PRODUCT_STANDARD_EVALUATION_SERVER:
                                    edition = "Server Standard (evaluation installation)";
                                    break;
                                case PRODUCT_DATACENTER_EVALUATION_SERVER:
                                    edition = "Server Datacenter (evaluation installation)";
                                    break;
                                case PRODUCT_ENTERPRISE_N_EVALUATION:
                                    edition = "Enterprise N (evaluation installation)";
                                    break;
                                case PRODUCT_STORAGE_WORKGROUP_EVALUATION_SERVER:
                                    edition = "Storage Server Workgroup (evaluation installation)";
                                    break;
                                case PRODUCT_STORAGE_STANDARD_EVALUATION_SERVER:
                                    edition = "Storage Server Standard (evaluation installation)";
                                    break;
                                case PRODUCT_CORE:
                                    edition = "Home";
                                    break;
                                case PRODUCT_CORE_N:
                                    edition = "Home N";
                                    break;
                                case PRODUCT_CORE_COUNTRYSPECIFIC:
                                    edition = "Home China";
                                    break;
                                case PRODUCT_CORE_SINGLELANGUAGE:
                                    edition = "Home Single Language";
                                    break;
                                case PRODUCT_PROFESSIONAL_WMC:
                                    edition = "Professional with Media Center";
                                    break;
                                case PRODUCT_MOBILE_CORE:
                                    edition = "Mobile";
                                    break;
                                case PRODUCT_EDUCATION:
                                    edition = "Education";
                                    break;
                                case PRODUCT_EDUCATION_N:
                                    edition = "Education N";
                                    break;
                            }
                        }
                    }
                    #endregion VERSION 10
                }

                s_Edition = edition;
                return edition;
            }
        }
        #endregion EDITION

        #region NAME
        static private string s_Name;
        /// <summary>
        /// Gets the name of the operating system running on this computer.
        /// </summary>
        static public string Name
        {
            get
            {
                if (s_Name != null)
                    return s_Name;  //***** RETURN *****//

                string name = "unknown";

                System.OperatingSystem osVersion = Environment.OSVersion;
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
                osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

                if (GetVersionEx(ref osVersionInfo))
                {
                    int majorVersion = osVersion.Version.Major;
                    int minorVersion = osVersion.Version.Minor;
                    int buildNumber = osVersion.Version.Build;

                    switch (osVersion.Platform)
                    {
                        case PlatformID.Win32Windows:
                            {
                                if (majorVersion == 4)
                                {
                                    string csdVersion = osVersionInfo.szCSDVersion;
                                    switch (minorVersion)
                                    {
                                        case 0:
                                            if (csdVersion == "B" || csdVersion == "C")
                                                name = "Windows 95 OSR2";
                                            else
                                                name = "Windows 95";
                                            break;
                                        case 10:
                                            if (csdVersion == "A")
                                                name = "Windows 98 Second Edition";
                                            else
                                                name = "Windows 98";
                                            break;
                                        case 90:
                                            name = "Windows Me";
                                            break;
                                    }
                                }
                                break;
                            }

                        case PlatformID.Win32NT:
                            {
                                byte productType = osVersionInfo.wProductType;

                                switch (majorVersion)
                                {
                                    case 3:
                                        {
                                            name = "Windows NT 3.51";
                                            break;
                                        }
                                    case 4:
                                        {
                                            switch (productType)
                                            {
                                                case VER_NT_WORKSTATION:
                                                    {
                                                        name = "Windows NT 4.0";
                                                        break;
                                                    }
                                                case VER_NT_SERVER:
                                                case VER_NT_DOMAIN_CONTROLLER:
                                                    {
                                                        name = "Windows NT 4.0 Server";
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        name = "Windows NT 4.0";
                                                        break;
                                                    }
                                            }
                                            break;
                                        }
                                    case 5:
                                        {
                                            switch (minorVersion)
                                            {
                                                case 0:
                                                    {
                                                        name = "Windows 2000";
                                                        break;
                                                    }
                                                case 1:
                                                    {
                                                        name = "Windows XP " + (((osVersionInfo.wSuiteMask & VER_SUITE_PERSONAL) == VER_SUITE_PERSONAL) ?
                                                            "Home" : "Professional");
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        if (osVersionInfo.wProductType == VER_NT_WORKSTATION && Bits == 64)
                                                        {
                                                            name = "Windows XP Professional x64 Edition";
                                                        }
                                                        else
                                                        {
                                                            if ((osVersionInfo.wSuiteMask & VER_SUITE_WH_SERVER) == VER_SUITE_WH_SERVER)
                                                            {
                                                                name = "Windows Home Server \"v1\"";
                                                            }
                                                            else if (GetSystemMetrics(SystemMetric.SM_SERVERR2) != 0)
                                                            {
                                                                name = "Windows Server 2003 R2";
                                                            }
                                                            else
                                                            {
                                                                name = "Windows Server 2003";
                                                            }
                                                        }
                                                        break;
                                                    }
                                            }
                                            break;
                                        }
                                    case 6:
                                        {
                                            switch (minorVersion)
                                            {
                                                case 0:
                                                    {
                                                        switch (productType)
                                                        {
                                                            case VER_NT_WORKSTATION:
                                                                name = "Windows Vista";
                                                                break;
                                                            case VER_NT_DOMAIN_CONTROLLER:
                                                            case VER_NT_SERVER:
                                                                name = "Windows Server 2008";
                                                                break;
                                                        }
                                                        break;
                                                    }
                                                case 1:
                                                    {
                                                        if ((osVersionInfo.wSuiteMask & VER_SUITE_WH_SERVER) == VER_SUITE_WH_SERVER)
                                                        {
                                                            name = "Windows Home Server 2011";
                                                        }
                                                        else if((osVersionInfo.wSuiteMask & VER_SUITE_SMALLBUSINESS) == VER_SUITE_SMALLBUSINESS)
                                                        {
                                                            name = "Windows Small Business Server 2011 Essentials";
                                                        }
                                                        else if ((osVersionInfo.wSuiteMask & VER_SUITE_STORAGE_SERVER) == VER_SUITE_STORAGE_SERVER)
                                                        {
                                                            name = "Windows Storage Server 2008 R2 Essentials";
                                                        }
                                                        else
                                                        {
                                                            switch (productType)
                                                            {
                                                                case VER_NT_WORKSTATION:
                                                                    name = "Windows 7";
                                                                    break;
                                                                case VER_NT_DOMAIN_CONTROLLER:
                                                                case VER_NT_SERVER:
                                                                    name = "Windows Server 2008 R2";
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        switch (productType)
                                                        {
                                                            case VER_NT_WORKSTATION:
                                                                name = "Windows 8";
                                                                break;
                                                            case VER_NT_DOMAIN_CONTROLLER:
                                                            case VER_NT_SERVER:
                                                                name = "Windows Server 2012";
                                                                break;
                                                        }
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        switch (productType)
                                                        {
                                                            case VER_NT_WORKSTATION:
                                                                name = "Windows 8.1";
                                                                break;
                                                            case VER_NT_DOMAIN_CONTROLLER:
                                                            case VER_NT_SERVER:
                                                                name = "Windows Server 2012 R2";
                                                                break;
                                                        }
                                                        break;
                                                    }
                                            }
                                            break;
                                        }
                                    case 10:
                                        {
                                            switch (minorVersion)
                                            {
                                                case 0:
                                                    {
                                                        switch (productType)
                                                        {
                                                            case VER_NT_WORKSTATION:
                                                                name = "Windows 10";
                                                                break;
                                                            case VER_NT_DOMAIN_CONTROLLER:
                                                            case VER_NT_SERVER:
                                                                name = "Windows Server 2016";
                                                                break;
                                                        }
                                                        break;
                                                    }
                                                //case 1:
                                                //    {
                                                //        if ((osVersionInfo.wSuiteMask & VER_SUITE_WH_SERVER) == VER_SUITE_WH_SERVER)
                                                //        {
                                                //            name = "Windows Home Server 2011";
                                                //        }
                                                //        else if ((osVersionInfo.wSuiteMask & VER_SUITE_SMALLBUSINESS) == VER_SUITE_SMALLBUSINESS)
                                                //        {
                                                //            name = "Windows Small Business Server 2011 Essentials";
                                                //        }
                                                //        else if ((osVersionInfo.wSuiteMask & VER_SUITE_STORAGE_SERVER) == VER_SUITE_STORAGE_SERVER)
                                                //        {
                                                //            name = "Windows Storage Server 2008 R2 Essentials";
                                                //        }
                                                //        else
                                                //        {
                                                //            switch (productType)
                                                //            {
                                                //                case VER_NT_WORKSTATION:
                                                //                    name = "Windows 7";
                                                //                    break;
                                                //                case VER_NT_DOMAIN_CONTROLLER:
                                                //                case VER_NT_SERVER:
                                                //                    name = "Windows Server 2008 R2";
                                                //                    break;
                                                //            }
                                                //        }
                                                //        break;
                                                //    }
                                                //case 2:
                                                //    {
                                                //        switch (productType)
                                                //        {
                                                //            case VER_NT_WORKSTATION:
                                                //                name = "Windows 8";
                                                //                break;
                                                //            case VER_NT_DOMAIN_CONTROLLER:
                                                //            case VER_NT_SERVER:
                                                //                name = "Windows Server 2012";
                                                //                break;
                                                //        }
                                                //        break;
                                                //    }
                                                //case 3:
                                                //    {
                                                //        switch (productType)
                                                //        {
                                                //            case VER_NT_WORKSTATION:
                                                //                name = "Windows 8.1";
                                                //                break;
                                                //            case VER_NT_DOMAIN_CONTROLLER:
                                                //            case VER_NT_SERVER:
                                                //                name = "Windows Server 2012 R2";
                                                //                break;
                                                //        }
                                                //        break;
                                                //    }
                                                default:
                                                    {
                                                        switch (productType)
                                                        {
                                                            case VER_NT_WORKSTATION:
                                                                name = "Windows 10";
                                                                break;
                                                            case VER_NT_DOMAIN_CONTROLLER:
                                                            case VER_NT_SERVER:
                                                                if (buildNumber >= 17633)
                                                                {
                                                                    name = "Windows Server 2019";
                                                                }
                                                                else
                                                                {
                                                                    name = "Windows Server 2016";
                                                                }
                                                                break;
                                                        }
                                                        break;
                                                    }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                }

                s_Name = name;
                return name;
            }
        }
        #endregion NAME

        #region PINVOKE
        #region GET
        #region PRODUCT INFO
        [DllImport("Kernel32.dll")]
        internal static extern bool GetProductInfo(
            int osMajorVersion,
            int osMinorVersion,
            int spMajorVersion,
            int spMinorVersion,
            out int edition);

        [DllImport("user32.dll")]
        internal static extern int GetSystemMetrics(SystemMetric smIndex);

        internal enum SystemMetric : int
        {
            SM_TABLETPC = 86,
            SM_MEDIACENTER = 87,
            SM_STARTER = 88,
            SM_SERVERR2 = 89
        }
        #endregion PRODUCT INFO

        #region VERSION
        [DllImport("kernel32.dll")]
        private static extern bool GetVersionEx(ref OSVERSIONINFOEX osVersionInfo);
        #endregion VERSION
        #endregion GET

        #region OSVERSIONINFOEX
        [StructLayout(LayoutKind.Sequential)]
        private struct OSVERSIONINFOEX
        {
            public int dwOSVersionInfoSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            public short wServicePackMajor;
            public short wServicePackMinor;
            public short wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }
        #endregion OSVERSIONINFOEX

        #region PRODUCT
        private const int PRODUCT_UNDEFINED = 0x00000000;
        private const int PRODUCT_ULTIMATE = 0x00000001;
        private const int PRODUCT_HOME_BASIC = 0x00000002;
        private const int PRODUCT_HOME_PREMIUM = 0x00000003;
        private const int PRODUCT_ENTERPRISE = 0x00000004;
        private const int PRODUCT_HOME_BASIC_N = 0x00000005;
        private const int PRODUCT_BUSINESS = 0x00000006;
        private const int PRODUCT_STANDARD_SERVER = 0x00000007;
        private const int PRODUCT_DATACENTER_SERVER = 0x00000008;
        private const int PRODUCT_SMALLBUSINESS_SERVER = 0x00000009;
        private const int PRODUCT_ENTERPRISE_SERVER = 0x0000000A;
        private const int PRODUCT_STARTER = 0x0000000B;
        private const int PRODUCT_DATACENTER_SERVER_CORE = 0x0000000C;
        private const int PRODUCT_STANDARD_SERVER_CORE = 0x0000000D;
        private const int PRODUCT_ENTERPRISE_SERVER_CORE = 0x0000000E;
        private const int PRODUCT_ENTERPRISE_SERVER_IA64 = 0x0000000F;
        private const int PRODUCT_BUSINESS_N = 0x00000010;
        private const int PRODUCT_WEB_SERVER = 0x00000011;
        private const int PRODUCT_CLUSTER_SERVER = 0x00000012;
        private const int PRODUCT_HOME_SERVER = 0x00000013;
        private const int PRODUCT_STORAGE_EXPRESS_SERVER = 0x00000014;
        private const int PRODUCT_STORAGE_STANDARD_SERVER = 0x00000015;
        private const int PRODUCT_STORAGE_WORKGROUP_SERVER = 0x00000016;
        private const int PRODUCT_STORAGE_ENTERPRISE_SERVER = 0x00000017;
        private const int PRODUCT_SERVER_FOR_SMALLBUSINESS = 0x00000018;
        private const int PRODUCT_SMALLBUSINESS_SERVER_PREMIUM = 0x00000019;
        private const int PRODUCT_HOME_PREMIUM_N = 0x0000001A;
        private const int PRODUCT_ENTERPRISE_N = 0x0000001B;
        private const int PRODUCT_ULTIMATE_N = 0x0000001C;
        private const int PRODUCT_WEB_SERVER_CORE = 0x0000001D;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT = 0x0000001E;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY = 0x0000001F;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING = 0x00000020;
        private const int PRODUCT_SERVER_FOUNDATION = 0x00000021;
        private const int PRODUCT_HOME_PREMIUM_SERVER = 0x00000022;
        private const int PRODUCT_SERVER_FOR_SMALLBUSINESS_V = 0x00000023;
        private const int PRODUCT_STANDARD_SERVER_V = 0x00000024;
        private const int PRODUCT_DATACENTER_SERVER_V = 0x00000025;
        private const int PRODUCT_ENTERPRISE_SERVER_V = 0x00000026;
        private const int PRODUCT_DATACENTER_SERVER_CORE_V = 0x00000027;
        private const int PRODUCT_STANDARD_SERVER_CORE_V = 0x00000028;
        private const int PRODUCT_ENTERPRISE_SERVER_CORE_V = 0x00000029;
        private const int PRODUCT_HYPERV = 0x0000002A;
        private const int PRODUCT_STORAGE_EXPRESS_SERVER_CORE = 0x0000002B;
        private const int PRODUCT_STORAGE_STANDARD_SERVER_CORE = 0x0000002C;
        private const int PRODUCT_STORAGE_WORKGROUP_SERVER_CORE = 0x0000002D;
        private const int PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE = 0x0000002E;
        private const int PRODUCT_STARTER_N = 0x0000002F;
        private const int PRODUCT_PROFESSIONAL = 0x00000030;
        private const int PRODUCT_PROFESSIONAL_N = 0x00000031;
        private const int PRODUCT_SB_SOLUTION_SERVER = 0x00000032;
        private const int PRODUCT_SERVER_FOR_SB_SOLUTIONS = 0x00000033;
        private const int PRODUCT_STANDARD_SERVER_SOLUTIONS = 0x00000034;
        private const int PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE = 0x00000035;
        private const int PRODUCT_SB_SOLUTION_SERVER_EM = 0x00000036;
        private const int PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM = 0x00000037;
        private const int PRODUCT_SOLUTION_EMBEDDEDSERVER = 0x00000038;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT = 0x0000003B;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL = 0x0000003C;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC = 0x0000003D;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC = 0x0000003E;
        private const int PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE = 0x0000003F;
        private const int PRODUCT_CLUSTER_SERVER_V = 0x00000040;
        private const int PRODUCT_STARTER_E = 0x00000042;
        private const int PRODUCT_HOME_BASIC_E = 0x00000043;
        private const int PRODUCT_HOME_PREMIUM_E = 0x00000044;
        private const int PRODUCT_PROFESSIONAL_E = 0x00000045;
        private const int PRODUCT_ENTERPRISE_E = 0x00000046;
        private const int PRODUCT_ULTIMATE_E = 0x00000047;
        private const int PRODUCT_ENTERPRISE_EVALUATION = 0x00000048;
        private const int PRODUCT_MULTIPOINT_STANDARD_SERVER = 0x0000004C;
        private const int PRODUCT_MULTIPOINT_PREMIUM_SERVER = 0x0000004D;
        private const int PRODUCT_STANDARD_EVALUATION_SERVER = 0x0000004F;
        private const int PRODUCT_DATACENTER_EVALUATION_SERVER = 0x00000050;
        private const int PRODUCT_ENTERPRISE_N_EVALUATION = 0x00000054;
        private const int PRODUCT_STORAGE_WORKGROUP_EVALUATION_SERVER = 0x0000005F;
        private const int PRODUCT_STORAGE_STANDARD_EVALUATION_SERVER = 0x00000060;
        private const int PRODUCT_CORE_N = 0x00000062;
        private const int PRODUCT_CORE_COUNTRYSPECIFIC = 0x00000063;
        private const int PRODUCT_CORE_SINGLELANGUAGE = 0x00000064;
        private const int PRODUCT_CORE = 0x00000065;
        private const int PRODUCT_PROFESSIONAL_WMC = 0x00000067;
        private const int PRODUCT_MOBILE_CORE = 0x00000068;
        private const int PRODUCT_EDUCATION = 0x00000079;
        private const int PRODUCT_EDUCATION_N = 0x0000007A;
        #endregion PRODUCT

        #region VERSIONS
        private const int VER_NT_WORKSTATION = 1;
        private const int VER_NT_DOMAIN_CONTROLLER = 2;
        private const int VER_NT_SERVER = 3;
        private const int VER_SUITE_SMALLBUSINESS = 1;
        private const int VER_SUITE_ENTERPRISE = 2;
        private const int VER_SUITE_BACKOFFICE = 4;
        private const int VER_SUITE_TERMINAL = 16;
        private const int VER_SUITE_SMALLBUSINESS_RESTRICTED = 32;
        private const int VER_SUITE_EMBEDDEDNT = 64;
        private const int VER_SUITE_DATACENTER = 128;
        private const int VER_SUITE_SINGLEUSERTS = 256;
        private const int VER_SUITE_PERSONAL = 512;
        private const int VER_SUITE_BLADE = 1024;
        private const int VER_SUITE_STORAGE_SERVER = 8192;
        private const int VER_SUITE_COMPUTE_SERVER = 16384;
        private const int VER_SUITE_WH_SERVER = 32768;
        #endregion VERSIONS
        #endregion PINVOKE

        #region SERVICE PACK
        /// <summary>
        /// Gets the service pack information of the operating system running on this computer.
        /// </summary>
        static public string ServicePack
        {
            get
            {
                string servicePack = String.Empty;
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();

                osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

                if (GetVersionEx(ref osVersionInfo))
                {
                    servicePack = osVersionInfo.szCSDVersion;
                }

                return servicePack;
            }
        }
        #endregion SERVICE PACK

        #region VERSION
        #region BUILD
        /// <summary>
        /// Gets the build version number of the operating system running on this computer.
        /// </summary>
        static public int BuildVersion
        {
            get
            {
                return Environment.OSVersion.Version.Build;
            }
        }
        #endregion BUILD

        #region FULL
        #region STRING
        /// <summary>
        /// Gets the full version string of the operating system running on this computer.
        /// </summary>
        static public string VersionString
        {
            get
            {
                return Environment.OSVersion.Version.ToString();
            }
        }
        #endregion STRING

        #region VERSION
        /// <summary>
        /// Gets the full version of the operating system running on this computer.
        /// </summary>
        static public Version Version
        {
            get
            {
                return Environment.OSVersion.Version;
            }
        }
        #endregion VERSION
        #endregion FULL

        #region MAJOR
        /// <summary>
        /// Gets the major version number of the operating system running on this computer.
        /// </summary>
        static public int MajorVersion
        {
            get
            {
                return Environment.OSVersion.Version.Major;
            }
        }
        #endregion MAJOR

        #region MINOR
        /// <summary>
        /// Gets the minor version number of the operating system running on this computer.
        /// </summary>
        static public int MinorVersion
        {
            get
            {
                return Environment.OSVersion.Version.Minor;
            }
        }
        #endregion MINOR

        #region REVISION
        /// <summary>
        /// Gets the revision version number of the operating system running on this computer.
        /// </summary>
        static public int RevisionVersion
        {
            get
            {
                return Environment.OSVersion.Version.Revision;
            }
        }
        #endregion REVISION
        #endregion VERSION
    }
}