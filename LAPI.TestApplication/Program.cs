using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Cryptography;
using LAPI.Domain;
using LAPI.Domain.Builder;
using LAPI.Domain.Extensions;
using LAPI.Domain.Model.Cryptography;
using LAPI.ExteriorProviders.LocalNetwork;
using LAPI.Providers.Aes;
using LAPI.Providers.Ssl;

namespace LAPI.TestApplication
{
    public class Program
    {
        public static readonly Guid TestGuid = new Guid("ca9fc5a0-86d6-45d4-9133-714a512e19d9");
        public static readonly Guid OtherTestGuid = new Guid("08f281d2-9a7e-479e-9c37-25e8a97093ef");
        public const string Password = "SomeTestPasswordJustForTestingNothingToSeeHere";
        public const int Port = 24512;
        public static async Task Main(string[] args)
        {
            //await FirstTest();
            await SecondTest();
        }

        private static async Task FirstTest()
        {
            //Console.WriteLine("Attempting to read from server");

            //byte[] buf = new byte[4];
            //stream.Result.ReadAsync(buf, 0, 4).Wait();
            //Console.WriteLine($"Received: {string.Join(", ", buf)}");


            //var registerResult = await Lapi.RegisterWithServer(IPAddress.Loopback, presharedKey, OtherTestGuid, TestGuid, clientCert, serverCert, otp, Port);
            //Console.WriteLine("Registering was " + (registerResult.Successful ? "" : "not ") + "successful");
            //registerResult.Result.Close();

            //var guidsResult = await Lapi.DiscoverServersOnLocalNetwork(presharedKey);
            //Console.WriteLine($"Discovery was " + (guidsResult.Successful ? "" : "not ") + "successful");
            //if (guidsResult.Successful)
            //{
            //    Console.WriteLine($"Found servers on network: {string.Join(", ", guidsResult.Result)}");
            //}
        }

        private static async Task SecondTest()
        {
            var presharedKey = Enumerable.Range(1, 20).Select(x => (byte) x).ToImmutableArray();
            var serverCert = X509CertificateService.GetOwnCertificate(TestGuid, Password);
            var clientCert = X509CertificateService.GetOwnCertificate(OtherTestGuid, Password);
            var otpBuffer = Enumerable.Range(1, 32).Select(x => (byte) x).ToArray();
            var otpKey = SymmetricKey.FromBuffer(otpBuffer);
            var otp = new AesCryptographicService(SymmetricKey.FromBuffer(otpBuffer));

            var server = new LapiServerBuilder()
                .WithGuid(() => TestGuid)
                .WithPresharedKey(() => presharedKey)
                .WithTcpServer(IPAddress.Loopback, Port)
                .WithSslEncryption(serverCert)
                .WithAesOtpCryptography(() => otpKey)
                .BuildServer();

            var client = new LapiClientBuilder()
                .WithGuid(() => OtherTestGuid)
                .WithPresharedKey(() => presharedKey)
                .WithSslEncryption(clientCert)
                .WithAesOtpCryptography(() => otpKey)
                .BuildClient();

         
            var serverControl = server.RunServer(
                serverCert,
                ClientConnected,
                (_, __) => { },
                _ => clientCert);
            serverControl.OnError += error => Console.WriteLine($"{error.ErrorType}: {error.Message}");

            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(IPAddress.Loopback, Port);

            var result = await client.ConnectToServerAsync(
                tcpClient.GetStream(),
                TestGuid,
                serverCert);

            Console.WriteLine($"Successful connected: {result.Successful}");
            var stream = result.Result;
            var payload = await stream.ReadSafelyAsync(4, CancellationToken.None);

            Console.WriteLine($"Receive successful: {payload.Successful}");
            if (payload.Successful)
            {
                Console.WriteLine($"Received {string.Join(", ", payload.Result)}");
            }

            stream.Close();
            stream.Dispose();
            serverControl.StopServer();
        }

        private static void ClientConnected(Guid guid, AuthenticatedStream str)
        {
            Console.WriteLine("Client Connected");
            var payload = new byte[] { 1, 2, 3, 4 };
            str.WriteAsync(payload, 0, 4);
            str.Close();
            str.Dispose();
        }
    }
}
