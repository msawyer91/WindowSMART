using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DojoNorthSoftware.WindowSMART
{
    public class WindowSmartPSException : Exception
    {
        public WindowSmartPSException()
            : base()
        {
        }

        public WindowSmartPSException(string message)
            : base(message)
        {
        }

        public WindowSmartPSException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Constructor needed for serialization when an exception propagates from a remoting server to the client.
        protected WindowSmartPSException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        {
        }
    }
}
