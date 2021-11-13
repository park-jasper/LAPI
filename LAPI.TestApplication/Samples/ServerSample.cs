using System;
using System.Collections.Immutable;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Abstractions.Cryptography;
using LAPI.Domain.Builder;
using LAPI.Domain.Contracts.Cryptography;
using LAPI.Domain.Extensions;
using LAPI.Domain.Model.Cryptography;
using LAPI.ExteriorProviders.LocalNetwork;
using LAPI.Providers.Aes;
using LAPI.Providers.Ssl;

namespace LAPI.TestApplication.Samples
{
    public class ServerSample
    {
        private ImmutableArray<byte> applicationSpecificPresharedKey;
        private Guid serverId;
        private X509Certificate2 serverCertificate;

        private SymmetricKey CurrentOtp { get; set; }

        public async Task RunServerExample()
        {
            var server = new LapiServerBuilder()
                .WithGuid(() => serverId)
                .WithPresharedKey(() => applicationSpecificPresharedKey)
                .WithTcpServer(IPAddress.Loopback, 1234)
                .WithSslEncryption(serverCertificate)
                .WithAesOtpCryptography(() => CurrentOtp)
                .BuildServer();

            var serverControl = server.RunServer(
                serverCertificate,
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