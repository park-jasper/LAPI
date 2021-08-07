using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Abstractions.Cryptography;
using LAPI.Domain.Communication;

namespace LAPI.Domain.Model
{
    public class LapiClient
    {
        private readonly Guid ownGuid;
        private readonly ImmutableArray<byte> presharedKey;
        private readonly IAuthenticatedConnectionFactory authenticatedConnectionFactory;
        private readonly IOtpServiceFactory otpServiceFactory;

        public LapiClient(
            Guid ownGuid,
            ImmutableArray<byte> presharedKey,
            IAuthenticatedConnectionFactory authenticatedConnectionFactory,
            IOtpServiceFactory otpServiceFactory)
        {
            this.ownGuid = ownGuid;
            this.presharedKey = presharedKey;
            this.authenticatedConnectionFactory = authenticatedConnectionFactory;
            this.otpServiceFactory = otpServiceFactory;
        }

        public async Task<InitializationResult<AuthenticatedStream>> RegisterWithServerAsync(
            Stream streamToServer,
            Guid serverGuid,
            X509Certificate2 clientCertificate,
            X509Certificate serverCertificate,
            CancellationToken token)
        {
            return await Initialization.RegisterWithServerAsync(
                streamToServer,
                this.presharedKey.ToArray(),
                this.ownGuid,
                serverGuid,
                this.authenticatedConnectionFactory,
                clientCertificate,
                serverCertificate,
                this.otpServiceFactory.GetCurrentOtp(),
                token);
        }

        public async Task<InitializationResult<AuthenticatedStream>> ConnectToServerAsync(
            Stream streamToServer,
            Guid serverGuid,
            X509Certificate serverCertificate)
        {
            return await Initialization.ConnectToServerAsync(
                streamToServer,
                this.presharedKey.ToArray(),
                this.ownGuid,
                serverGuid,
                this.authenticatedConnectionFactory,
                serverCertificate);
        }

        // TODO: ServerDiscoveryResult as return?
        public async Task<CommunicationResult<IEnumerable<Guid>>> DiscoverServersOnLocalNetwork(
            TimeSpan timeout,
            int discoveryPort = Facts.Discovery.UdpDiscoveryDefaultPort)
        {
            var udpClient = new UdpClient(AddressFamily.InterNetwork)
            {
                EnableBroadcast = true,
            };
            var broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, discoveryPort);
            await udpClient.SendAsync(this.presharedKey.ToArray(), this.presharedKey.Length, broadcastEndpoint);

            udpClient.EnableBroadcast = false;
            udpClient.Client.ReceiveTimeout = (int) timeout.TotalMilliseconds;
            using (var timeoutSource = new CancellationTokenSource())
            {
                timeoutSource.CancelAfter(timeout);
                var token = timeoutSource.Token;

                var guids = new List<Guid>();
                Exception lastException = null;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var result = await udpClient.ReceiveAsync();
                        if (result.Buffer.Length == 16)
                        {
                            var guid = new Guid(result.Buffer);
                            guids.Add(guid);
                        }
                    }
                    catch (Exception exc)
                    {
                        lastException = exc;
                    }
                }

                if (guids.Any() || lastException == null)
                {
                    return CommunicationResult.FromResult(guids.AsEnumerable());
                }
                else
                {
                    return new CommunicationResult<IEnumerable<Guid>>
                    {
                        Successful = false,
                        Exception = lastException,
                    };
                }
            }
        }
    }
}