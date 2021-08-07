using System;
using LAPI.Abstractions;
using LAPI.Domain.Model;

namespace LAPI.Domain.Builder
{
    public class LapiClientBuilder : LapiBuilder
    {
        public LapiClientBuilder()
        {
        }

        public LapiClient BuildClient()
        {
            return new LapiClient(
                this.GetPresharedKey(),
                this.CreateAuthenticatedConnectionFactory(),
                this.CreateOtpServiceFactory());
        }
    }
}