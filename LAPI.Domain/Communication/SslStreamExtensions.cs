using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Domain.Extensions;

namespace LAPI.Domain.Communication
{
    public static class SslStreamExtensions
    {
        public static async Task AuthenticateAsServerAsync(this SslStream sslStream, X509Certificate serverCertificate, CancellationToken token)
        {
            await sslStream
                .AuthenticateAsServerAsync(serverCertificate, true, SslProtocols.None, true)
                .WithCancellationToken(token);
        }
        public static async Task AuthenticateAsClientAsync(this SslStream sslStream, Guid host, X509Certificate clientCertificate, CancellationToken token)
        {
            await sslStream
                .AuthenticateAsClientAsync(host.ToString(), new X509CertificateCollection { clientCertificate}, SslProtocols.None, true)
                .WithCancellationToken(token);
        }
    }
}