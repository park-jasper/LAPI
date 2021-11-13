using System;
using LAPI.Abstractions;
using LAPI.Domain.Model;

namespace LAPI.Domain.Builder
{
    public class LapiServerBuilder : LapiBuilder
    {
        private Func<IServer> createServerImplementation;

        public LapiServerBuilder()
        {
        }

        public LapiServerBuilder WithCustomServerImplementation(Func<IServer> createServerImplementation)
        {
            this.createServerImplementation = createServerImplementation;
            return this;
        }

        public LapiServer BuildServer()
        {
            return new LapiServer(
                this.GetGuid(),
                this.GetPresharedKey(),
                this.CreateAuthenticatedConnectionFactory(),
                this.CreateOtpServiceFactory(),
                this.createServerImplementation());
        }
    }
}