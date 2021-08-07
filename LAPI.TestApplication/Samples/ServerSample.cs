using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Abstractions.Cryptography;
using LAPI.Domain.Contracts.Cryptography;
using LAPI.Domain.Extensions;
using LAPI.ExteriorProviders.LocalNetwork;

namespace LAPI.TestApplication.Samples
{
    public class ServerSample
    {
        private byte[] applicationSpecificPresharedKey;
        private Guid serverId;
        private X509Certificate2 serverCertificate;

        private ICryptographicService CurrentOtp { get; set; }

        public async Task RunServerExample()
        {
            var server = new TcpServer(IPAddress.Loopback, 1234);
            var serverControl = Lapi.RunServer(
                server,
                applicationSpecificPresharedKey,
                serverId,
                serverCertificate,
                () => CurrentOtp,
                OnClientConnected,
                null,
                clientId => null);
            await Task.Delay(TimeSpan.FromMinutes(5));
            serverControl.StopServer();
        }

        private async void OnClientConnected(Guid clientId, AuthenticatedStream clientStream)
        {
            var textResult = await clientStream.ReceiveStringSafelyAsync(CancellationToken.None);
            if (textResult.Successful)
            {
                Console.WriteLine($"Client sent text: '{textResult.Result}'");
            }

            await clientStream.WriteSafelyAsync("Hi there client", CancellationToken.None);
        }
    }
}