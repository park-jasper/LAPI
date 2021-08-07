using System.Security.Cryptography.X509Certificates;
using LAPI.Domain;
using LAPI.Domain.Builder;

namespace LAPI.Providers.Ssl
{
    public static class LapiBuilderExtensions
    {
        public static TLapiBuilder WithSslEncryption<TLapiBuilder>(this TLapiBuilder builder, X509Certificate2 clientCertificate)
            where TLapiBuilder : LapiBuilder
        {
            return builder.WithCustomAuthenticatedConnectionFactory(() => new SslConnectionFactory(clientCertificate));
        }
    }
}