using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DojoNorthSoftware.WindowsServerSolutions.HomeServerSMART2013.UI.BitLocker
{
    /// <summary>
    /// Exception that represents a condition that exists when an expected BitLocker EncryptableVolume object has gone missing.
    /// </summary>
    public sealed class DiskNotFoundException : Exception
    {
        public DiskNotFoundException()
        {
        }

        public DiskNotFoundException(String message)
            : base(message)
        {
        }

        public DiskNotFoundException(String message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
