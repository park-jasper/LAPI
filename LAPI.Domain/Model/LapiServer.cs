using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Abstractions;
using LAPI.Abstractions.Cryptography;
using LAPI.Domain.Communication;

namespace LAPI.Domain.Model
{
    public class LapiServer
    {
        private readonly ImmutableArray<byte> presharedKey;
        private readonly IAuthenticatedConnectionFactory authenticatedConnectionFactory;
        private readonly IOtpServiceFactory otpServiceFactory;
        private readonly IServer serverImplementation;
        private readonly Guid ownGuid;

        public LapiServer(
            Guid ownGuid,
            ImmutableArray<byte> presharedKey,
            IAuthenticatedConnectionFactory authenticatedConnectionFactory,
            IOtpServiceFactory otpServiceFactory,
            IServer serverImplementation)
        {
            this.presharedKey = presharedKey;
            this.authenticatedConnectionFactory = authenticatedConnectionFactory;
            this.otpServiceFactory = otpServiceFactory;
            this.serverImplementation = serverImplementation;
            this.ownGuid = ownGuid;
        }

        public ServerControl RunServer(
            X509Certificate2 serverCertificate,
            Action<Guid, AuthenticatedStream> onClientConnected,
            Action<Guid, X509Certificate> onClientRegistered,
            Func<Guid, X509Certificate> getClientPublicKey,
            int udpDiscoveryPort = Facts.Discovery.UdpDiscoveryDefaultPort)
        {
            var tokenSource = new CancellationTokenSource();
            var control = new ServerControl(this.serverImplementation, tokenSource);
            Task.Run(
                () => this.Listen(
                    serverCertificate,
                    onClientConnected,
                    onClientRegistered,
                    getClientPublicKey,
                    control.ReportError,
                    tokenSource.Token),
                tokenSource.Token);
            Task.Run(() => ListenForDiscovery(udpDiscoveryPort, tokenSource.Token), tokenSource.Token);
            return control;
        }

        private async Task Listen(
            X509Certificate2 serverCertificate,
            Action<Guid, AuthenticatedStream> onClientConnected,
            Action<Guid, X509Certificate> onClientRegistered,
            Func<Guid, X509Certificate> getClientPublicKey,
            Action<InitializationError> onError,
            CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var clientStream = await this.serverImplementation.AcceptClientAsync();
                    Task.Run(
                        async () =>
                        {
                            var result = await Initialization.HandleInitializationOfClient(
                                clientStream,
                                this.presharedKey.ToArray(),
                                this.ownGuid,
                                this.authenticatedConnectionFactory,
                                this.otpServiceFactory.GetCurrentOtp(),
                                onClientConnected,
                                onClientRegistered,
                                getClientPublicKey,
                                cancellationToken);
                            if (!result.Successful)
                            {
                                onError(result.Error);
                            }
                        },
                        cancellationToken);
                }
            }
            catch (Exception exc)
            {
                //TODO exception handling
                Console.WriteLine(exc);
                throw;
            }
        }

        private async Task ListenForDiscovery(
            int udpDiscoveryPort,
            CancellationToken cancellationToken)
        {
            var discoveryServer = new UdpClient(udpDiscoveryPort)
            {
                EnableBroadcast = true,
            };

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var request = await discoveryServer.ReceiveAsync();

                    Task SendAnswerAsync(byte[] datagram, int length)
                    {
                        var endPoint = request.RemoteEndPoint;
                        var responseClient = new UdpClient(endPoint.Port, endPoint.AddressFamily);
                        return responseClient.SendAsync(datagram, length, endPoint);
                    }

                    await Initialization.HandleServerDiscoveryMessage(
                        request.Buffer,
                        SendAnswerAsync,
                        this.presharedKey.ToArray(),
                        this.ownGuid,
                        cancellationToken);
                }
            }
            catch (SocketException sExc)
            {

            }
            finally
            {
                discoveryServer.Close();
            }
        }
    }
}