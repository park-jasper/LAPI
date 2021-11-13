using System;
using System.Collections.Immutable;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Domain.Builder;
using LAPI.Domain.Extensions;
using LAPI.Providers.Aes;
using LAPI.Providers.Ssl;

namespace LAPI.TestApplication.Samples
{
    public class ClientSample
    {
        private ImmutableArray<byte> applicationSpecificPresharedKey;
        private Guid clientId;
        private X509Certificate2 clientCertificate;

        public async Task ClientConnectSample(
            IPAddress serverIpAddress,
            Guid serverId,
            X509Certificate serverCertificate)
        {
            var client = new LapiClientBuilder()
                .WithGuid(() => clientId)
                .WithPresharedKey(() => applicationSpecificPresharedKey)
                .WithSslEncryption(clientCertificate)
                .WithAesOtpCryptography(() => null)
                .BuildClient();

            var tcpClient = new TcpClient(AddressFamily.InterNetwork);
            await tcpClient.ConnectAsync(serverIpAddress, 1234);

            var result = await client.ConnectToServerAsync(
                tcpClient.GetStream(),
                serverId,
                serverCertificate);

            if (result.Successful)
            {
                var stream = result.Result;
                await stream.WriteSafelyAsync("Hi there, server!", CancellationToken.None);
                var response = await stream.ReceiveStringSafelyAsync(CancellationToken.None);
                if (response.Successful)
                {
                    Console.WriteLine($"Server responded with: '{response.Result}'");
                }
            }
        }
    }
}