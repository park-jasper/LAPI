using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using LAPI.Domain.Contracts;

namespace LAPI.Providers
{
    public class TcpServer : IServer
    {
        private const AddressFamily Ipv4 = AddressFamily.InterNetwork;
        private readonly TcpListener _listener;
        public TcpServer(IPAddress localIpAddress, int port)
        {
            _listener = new TcpListener(localIpAddress, port);
        }

        public void Start()
        {
            _listener.Start();
        }

        public async Task<Stream> AcceptClientAsync()
        {
            var client = await _listener.AcceptTcpClientAsync();
            return client.GetStream();
        }

        public static IPAddress GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.FirstOrDefault(IsActualLocalIpAddress);
        }

        private static bool IsActualLocalIpAddress(IPAddress ipAddress)
        {
            var isIpv4 = ipAddress.AddressFamily == Ipv4;
            var isLoopback = ipAddress.GetAddressBytes()[3] == 1;
            return isIpv4 && !isLoopback;
        }
    }
}