using System.Collections.Immutable;

namespace LAPI.Model
{
    public class RsaPrivateKey
    {
        public ImmutableArray<byte> Modulus { get; }
        public ImmutableArray<byte> PrivateKeyExponent { get; }

        public RsaPrivateKey(byte[] modulus, byte[] privateKeyExponent)
        {
            Modulus = modulus.ToImmutableArray();
            PrivateKeyExponent = privateKeyExponent.ToImmutableArray();
        }
    }
}