using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DeviceScanner
    {
        public static bool IsHostAccessible(string hostNameOrAddress)
        {
            Ping ping = new Ping();
            PingReply reply = ping.Send(hostNameOrAddress, 1000);

            return reply != null && reply.Status == IPStatus.Success;
        }
    }
}
