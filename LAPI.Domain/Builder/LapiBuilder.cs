using System;
using System.Collections.Immutable;
using LAPI.Abstractions;
using LAPI.Abstractions.Cryptography;

namespace LAPI.Domain.Builder
{
    public abstract class LapiBuilder
    {
        protected Func<IAuthenticatedConnectionFactory> CreateAuthenticatedConnectionFactory;
        protected Func<IOtpServiceFactory> CreateOtpServiceFactory;
        protected Func<Guid> GetGuid;
        protected Func<ImmutableArray<byte>> GetPresharedKey;

        protected LapiBuilder()
        {
        }

        internal void SetAuthenticatedConnectionFactory(
            Func<IAuthenticatedConnectionFactory> createAuthenticatedConnectionFactory)
        {
            this.CreateAuthenticatedConnectionFactory = createAuthenticatedConnectionFactory;
        }

        internal void SetOtpServiceFactory(
            Func<IOtpServiceFactory> createOtpServiceFactory)
        {
            this.CreateOtpServiceFactory = createOtpServiceFactory;
        }

        internal void SetGuid(Func<Guid> getGuid)
        {
            this.GetGuid = getGuid;
        }

        internal void SetPresharedKey(Func<ImmutableArray<byte>> getPresharedKey)
        {
            this.GetPresharedKey = getPresharedKey;
        }
    }

    public static class LapiBuilderMethods
    {
        public static TLapiBuilder WithCustomAuthenticatedConnectionFactory<TLapiBuilder>(
            this TLapiBuilder builder,
            Func<IAuthenticatedConnectionFactory> createAuthenticatedConnectionFactory)
            where TLapiBuilder : LapiBuilder
        {
            builder.SetAuthenticatedConnectionFactory(createAuthenticatedConnectionFactory);
            return builder;
        }

        public static TLapiBuilder WithCustomOtpServiceFactory<TLapiBuilder>(
            this TLapiBuilder builder,
            Func<IOtpServiceFactory> createOtpServiceFactory)
            where TLapiBuilder : LapiBuilder
        {
            builder.SetOtpServiceFactory(createOtpServiceFactory);
            return builder;
        }

        public static TLapiBuilder WithGuid<TLapiBuilder>(
            this TLapiBuilder builder,
            Func<Guid> getGuid)
            where TLapiBuilder : LapiBuilder
        {
            builder.SetGuid(getGuid);
            return builder;
        }

        public static TLapiBuilder WithPresharedKey<TLapiBuilder>(
            this TLapiBuilder builder,
            Func<ImmutableArray<byte>> getPresharedKey)
            where TLapiBuilder : LapiBuilder
        {
            builder.SetPresharedKey(getPresharedKey);
            return builder;
        }
    }
}