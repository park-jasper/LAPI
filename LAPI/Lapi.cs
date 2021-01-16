using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Communication;
using LAPI.Contracts;
using LAPI.Model;
using LAPI.Model.Cryptography;

namespace LAPI
{
    public class Lapi
    {
        public static ClientControl RegisterWithServer(IStream clientStream, Guid serverGuid, SymmetricKey serverOtp)
        {
            throw new NotImplementedException();
        }

        public static ClientControl ConnectToServer(IStream clientStream, Guid serverGuid,
            ICryptographicService serverEncryptor, ICryptographicService clientDecryptor)
        {
            throw new NotImplementedException();
        }

        public const int DefaultDiscoveryPort = 11000;
        public static ServerControl RunServer(
            IServer server,
            byte[] presharedKey,
            Guid serverGuid,
            Func<ICryptographicService> getOtp,
            IAsymmetricCryptographicService asymmetric,
            ISymmetricCryptographicService symmetric,
            Action<IStream> onClientConnected,
            Action<Guid, RsaPublicKey> onClientRegistered,
            Func<Guid, RsaPublicKey> getClientPublicKey,
            int discoveryPort = DefaultDiscoveryPort)
        {
            var tokenSource = new CancellationTokenSource();
            Task.Run(
                () => Listen(
                    server, 
                    presharedKey,
                    serverGuid,
                    getOtp,
                    asymmetric,
                    symmetric,
                    onClientConnected,
                    onClientRegistered,
                    getClientPublicKey, 
                    tokenSource.Token), 
                tokenSource.Token);
            Task.Run(() => ListenForDiscovery(presharedKey, serverGuid, discoveryPort, tokenSource.Token), tokenSource.Token);
            return new ServerControl(server, tokenSource);
        }

        private static async Task Listen(
            IServer server,
            byte[] presharedKey,
            Guid serverGuid,
            Func<ICryptographicService> getOtp,
            IAsymmetricCryptographicService asymmetric,
            ISymmetricCryptographicService symmetric,
            Action<IStream> onClientConnected,
            Action<Guid, RsaPublicKey> onClientRegistered,
            Func<Guid, RsaPublicKey> getClientPublicKey,
            CancellationToken token)
        {
            server.Start();
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var clientStream = await server.AcceptClientAsync();
#pragma warning disable 4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Task.Run(
                        () => Initialization.HandleInitializationOfClient(
                            clientStream, 
                            presharedKey, 
                            serverGuid, 
                            getOtp(), 
                            asymmetric, 
                            symmetric, 
                            onClientConnected, 
                            onClientRegistered, 
                            getClientPublicKey, 
                            token), 
                        token);
#pragma warning restore 4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
            catch (Exception e)
            {
                //TODO exception handling
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task ListenForDiscovery(
            byte[] presharedKey,
            Guid serverGuid,
            int discoveryPort,
            CancellationToken token)
        {
            var discoveryServer = new UdpClient(discoveryPort)
            {
                EnableBroadcast = true,
            };

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var request = await discoveryServer.ReceiveAsync();
                    await Initialization.HandleServerDiscoveryMessage(request.Buffer, request.RemoteEndPoint, presharedKey, serverGuid, token);
                }
            }
            catch (SocketException sexc)
            {

            }
            finally
            {
                discoveryServer.Close();
            }
        }

        // ServerDiscoveryResult as return?
        public static async Task<CommunicationResult<IEnumerable<Guid>>> DiscoverServersOnLocalNetwork(
            byte[] presharedKey,
            int timeoutInMilliseconds = 200,
            int discoveryPort = DefaultDiscoveryPort)
        {
            var client = new UdpClient(discoveryPort, AddressFamily.InterNetwork)
            {
                EnableBroadcast = true,
            };
            var broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, discoveryPort);
            await client.SendAsync(presharedKey, presharedKey.Length, broadcastEndpoint);

            client.EnableBroadcast = false;
            client.Client.ReceiveTimeout = timeoutInMilliseconds;
            var timeoutSource = new CancellationTokenSource();
            timeoutSource.CancelAfter(TimeSpan.FromMilliseconds(timeoutInMilliseconds));

            var guids = new List<Guid>();
            while (!timeoutSource.Token.IsCancellationRequested)
            {
                try
                {
                    var result = await client.ReceiveAsync();
                    if (result.Buffer.Length == 16)
                    {
                        var guid = new Guid(result.Buffer);
                        guids.Add(guid);
                    }
                }
                catch (SocketException sexc)
                {

                }
                catch (Exception exc)
                {
                    return new CommunicationResult<IEnumerable<Guid>>
                    {
                        Successful = false,
                        Exception = exc,
                    };
                }
            }
            return CommunicationResult.FromResult(guids.AsEnumerable());
        }
    }
}
