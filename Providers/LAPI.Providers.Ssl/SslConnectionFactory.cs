using System;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Abstractions;
using LAPI.Abstractions.Cryptography;
using LAPI.Abstractions.Result;
using LAPI.Domain.Extensions;

namespace LAPI.Providers.Ssl
{
    public class SslConnectionFactory : IAuthenticatedConnectionFactory
    {
        private readonly X509Certificate2 certificate;

        public SslConnectionFactory(X509Certificate2 certificate)
        {
            this.certificate = certificate;
        }

        public Task<AuthenticationResult> AuthenticateAsServerAsync(Stream clientStream, CancellationToken cancellationToken)
        {
            return AuthenticateAsync(
                clientStream,
                cancellationToken,
                (sslStream, token) => sslStream
                    .AuthenticateAsServerAsync(this.certificate, true, SslProtocols.None, true)
                    .WithCancellationToken(token));
        }

        public Task<AuthenticationResult> AuthenticateAsClientAsync(Stream serverStream, Guid assumedServerGuid, CancellationToken cancellationToken)
        {
            return AuthenticateAsync(
                serverStream,
                cancellationToken,
                (sslStream, token) => sslStream
                    .AuthenticateAsClientAsync(
                        assumedServerGuid.ToString(),
                        new X509CertificateCollection { this.certificate },
                        SslProtocols.None, 
                        true)
                    .WithCancellationToken(token));
        }

        private static async Task<AuthenticationResult> AuthenticateAsync(Stream clientStream, CancellationToken cancellationToken, Func<SslStream, CancellationToken, Task> authenticate)
        {
            var sslStream = new SslStream(clientStream, false, IsValidRemoteParty);
            try
            {
                await authenticate(sslStream, cancellationToken);
            }
            catch (AuthenticationException authExc)
            {
                return new AuthenticationResult.AuthenticationFailed(authExc);
            }
            catch (NotSupportedException notSupportedExc)
            {
                return new AuthenticationResult.NotSupported(notSupportedExc);
            }
            catch (TaskCanceledException tcExc)
            {
                return new AuthenticationResult.TaskCanceled();
            }

            return new AuthenticationResult.Authenticated(sslStream);
        }

        private static bool IsValidRemoteParty(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors errors)
        {
            if (errors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable) ||
                errors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch))
            {
                return false;
            }

            return true; //TODO
        }
    }
}
