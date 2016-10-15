using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Networking
{
    public class Destination
    {
        public readonly IPAddress _IP;
        public readonly int _Port;
        private readonly String _About;

        public override string ToString()
        {
            return this._About;
        }

        public Destination(int port, IPAddress ip, String description = null)
        {
            this._Port = port;
            this._IP = ip;
            this._About = description == null ? (this._IP + ":" + this._Port) : description;
        }


        public static Destination Local(int port, String Description = null)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return new Destination(port, ip, Description);
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}
