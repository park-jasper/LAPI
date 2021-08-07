using System;
using LAPI.Abstractions;
using LAPI.Abstractions.Cryptography;
using LAPI.Domain.Model.Cryptography;

namespace LAPI.Providers.Aes
{
    public class AesCryptographicServiceFactory : IOtpServiceFactory
    {
        private readonly Func<SymmetricKey> getCurrentOtpKey;

        public AesCryptographicServiceFactory(Func<SymmetricKey> getCurrentOtpKey)
        {
            this.getCurrentOtpKey = getCurrentOtpKey;
        }

        public ICryptographicService GetCurrentOtp()
        {
            return new AesCryptographicService(this.getCurrentOtpKey());
        }
    }
}