using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockyBitsServer
{
    public class ServerDebug
    {

        public static void WriteLine(string message)
        {
            Debug.WriteLine("Server: " + message);
        }
    }
}
