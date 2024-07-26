using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace eVerification.codes
{
   public class NetworkStatus
    {

        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        public static bool CheckNet()
        {
            int desc;
            return InternetGetConnectedState(out desc, 0);
        }


        public static bool IsConnectedToInternet()
        {
            const int maxHops = 30;
            Uri myUri = new Uri(eVerification.Properties.Settings.Default.finger_verif_url);
            string host = myUri.Host;   
             
            for (int ttl = 1; ttl <= maxHops; ttl++)
            {
                var options = new PingOptions(ttl, true);
                byte[] buffer = new byte[32];
                PingReply reply;
                try
                {
                    using (var pinger = new Ping())
                    {
                        reply = pinger.Send(host, 10000, buffer, options);
                    }
                }
                catch (PingException pingex)
                {
                    return false;
                }

                string address = reply.Address?.ToString() ?? null; 

                if (reply.Status != IPStatus.TtlExpired && reply.Status != IPStatus.Success)
                { 
                    return false;
                }

                //if (IsRoutableAddress(reply.Address))
                //{ 
                    return true;
                //}
            }

            return false;
        }

        private static bool IsRoutableAddress(IPAddress addr)
        {
            if (addr == null)
            {
                return false;
            }
            else if (addr.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return !addr.IsIPv6LinkLocal && !addr.IsIPv6SiteLocal;
            }
            else // IPv4
            {
                byte[] bytes = addr.GetAddressBytes();
                if (bytes[0] == 10)
                {   // Class A network
                    return false;
                }
                else if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                {   
                    return false;
                }
                else if (bytes[0] == 192 && bytes[1] == 168)
                {   
                    return false;
                }
                else
                {   // None of the above, so must be routable
                    return true;
                }
            }
        }
    }
}
