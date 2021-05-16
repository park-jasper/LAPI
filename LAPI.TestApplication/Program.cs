using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using LAPI.Cryptography;
using LAPI.Domain.Model.Cryptography;
using LAPI.Providers;

namespace LAPI.TestApplication
{
    public class Program
    {
        public static readonly Guid TestGuid = new Guid("ca9fc5a0-86d6-45d4-9133-714a512e19d9");
        public static readonly Guid OtherTestGuid = new Guid("08f281d2-9a7e-479e-9c37-25e8a97093ef");
        public const string Password = "SomeTestPasswordJustForTestingNothingToSeeHere";
        public const int Port = 24512;
        public static void Main(string[] args)
        {
            var server = new TcpServer(IPAddress.Loopback, Port);
            var presharedKey = Enumerable.Range(1, 20).Select(x => (byte) x).ToArray();
            var serverCert = X509CertificateService.GetOwnCertificate(TestGuid, Password);
            var clientCert = X509CertificateService.GetOwnCertificate(OtherTestGuid, Password);
            var otpBuffer = Enumerable.Range(1, 32).Select(x => (byte) x).ToArray();
            var otp = new AesCryptographicService(SymmetricKey.FromBuffer(otpBuffer));

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
            //var stream = Lapi.ConnectToServer(
            //    IPAddress.Loopback, 
            //    presharedKey, 
            //    OtherTestGuid, 
            //    TestGuid,
            //    clientCert, 
            //    serverCert, 
            //    Port).Result;

            //Console.WriteLine($"Successful connected: {stream.Successful}");
            //Console.WriteLine("Attempting to read from server");

            //byte[] buf = new byte[4];
            //stream.Result.ReadAsync(buf, 0, 4).Wait();
            //Console.WriteLine($"Received: {string.Join(", ", buf)}");

            var registerResult = Lapi.RegisterWithServer(IPAddress.Loopback, presharedKey, OtherTestGuid, TestGuid, clientCert, serverCert, otp, Port).Result;
            Console.WriteLine("Registering was " + (registerResult.Successful ? "" : "not ") + "successful");
            registerResult.Result.Close();

            Console.ReadLine();
            serverControl.StopServer();
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
