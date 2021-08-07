using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Domain.Extensions;

namespace LAPI.TestApplication.Samples
{
    public class ClientSample
    {
        private byte[] applicationSpecificPresharedKey;
        private Guid clientId;
        private X509Certificate2 clientCertificate;

        public async Task ClientConnectSample(
            IPAddress serverIpAddress,
            Guid serverId,
            X509Certificate serverCertificate)
        {
            var client = new TcpClient(AddressFamily.InterNetwork);
            await client.ConnectAsync(serverIpAddress, 1234);
            var result = await Lapi.ConnectToServer(
                client.GetStream(),
                applicationSpecificPresharedKey,
                clientId,
                serverId,
                clientCertificate,
                serverCertificate);
            if (result.Successful)
            {
                var stream = result.Result;
                await stream.WriteSafelyAsync(CancellationToken.None, "Hi there, server!");
                var response = await stream.ReceiveStringSafelyAsync(CancellationToken.None);
                if (response.Successful)
                {
                    Console.WriteLine($"Server responded with: '{response.Result}'");
                }
            }
        }
    }
}