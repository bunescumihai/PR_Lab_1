using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ReceiveException: Exception
    {
        string message = "Connection interrupted while receiving data";
        public override string Message { get { return message; } }
    }

    internal class SendException: Exception
    {
        string message = "Connection interrupted while sending data";
        public override string Message { get { return message; } }
    }
}
