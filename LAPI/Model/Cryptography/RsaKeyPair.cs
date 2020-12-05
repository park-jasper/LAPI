using System;
using System.Collections.Immutable;
using System.Linq;

namespace LAPI.Model.Cryptography
{
    public class RsaKeyPair
    {
        public ImmutableArray<byte> Modulus => PrivateKey.Modulus;
        public RsaPrivateKey PrivateKey { get; }
        public RsaPublicKey PublicKey { get; }

        public RsaKeyPair(RsaPrivateKey privateKey, RsaPublicKey publicKey)
        {
            if (privateKey.Modulus.Length != publicKey.Modulus.Length ||
                !Enumerable.SequenceEqual(privateKey.Modulus, publicKey.Modulus))
            {
                throw new ArgumentException("Modulus of private and public key needs to be the same");
            }
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }
    }
}