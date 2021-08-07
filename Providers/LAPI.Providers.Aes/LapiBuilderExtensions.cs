using System;
using LAPI.Domain.Builder;
using LAPI.Domain.Model.Cryptography;

namespace LAPI.Providers.Aes
{
    public static class LapiBuilderExtensions
    {
        public static TLapiBuilder WithAesOtpCryptography<TLapiBuilder>(
            this TLapiBuilder builder,
            Func<SymmetricKey> getCurrentOtpKey)
            where TLapiBuilder : LapiBuilder
        {
            builder.WithCustomOtpServiceFactory(() => new AesCryptographicServiceFactory(getCurrentOtpKey));
            return builder;
        }
    }
}
