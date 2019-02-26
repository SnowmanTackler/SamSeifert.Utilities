using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Networking
{
    public static class IPEndPointExtensions
    {
        public static IPEndPoint Local(int port, String Description = null)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return new IPEndPoint(ip, port);
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}
