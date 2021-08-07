using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LAPI.Cryptography;
using LAPI.Domain;
using LAPI.Domain.Builder;
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
            var server = new TcpServer(IPAddress.Loopback, Port);
            var presharedKey = Enumerable.Range(1, 20).Select(x => (byte) x).ToArray();
            var serverCert = X509CertificateService.GetOwnCertificate(TestGuid, Password);
            var clientCert = X509CertificateService.GetOwnCertificate(OtherTestGuid, Password);
            var otpBuffer = Enumerable.Range(1, 32).Select(x => (byte) x).ToArray();
            var otp = new AesCryptographicService(SymmetricKey.FromBuffer(otpBuffer));

            var reducedServerCert = new X509Certificate2(serverCert.Export(X509ContentType.Cert));
            var reducedClientCert = new X509Certificate2(clientCert.Export(X509ContentType.Cert));

            var serverControl = Lapi.RunServer(
                server,
                presharedKey,
                TestGuid,
                serverCert,
                () => otp,
                ClientConnected,
                (_, __) => { },
                x =>
                {
                    if (x.Equals(OtherTestGuid))
                    {
                        return clientCert;
                    }

                    return null;
                });
            serverControl.OnError += error => Console.WriteLine($"{error.ErrorType}: {error.Message}");

            var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, Port);

            var stream = await Lapi.ConnectToServer(
                client.GetStream(),
                presharedKey,
                OtherTestGuid,
                TestGuid,
                clientCert,
                serverCert);

            Console.WriteLine($"Successful connected: {stream.Successful}");
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

            Console.ReadLine();
            serverControl.StopServer();
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
        }

        private static void ClientConnected(Guid guid, AuthenticatedStream str)
        {
            Console.WriteLine("Client Connected");
            var payload = new byte[] { 1, 2, 3, 4 };
            str.WriteAsync(payload, 0, 4);
            str.Close();
        }
    }
}
