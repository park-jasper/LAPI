using System.Collections.Immutable;

namespace LAPI.Model
{
    public class RsaPublicKey
    {
        public ImmutableArray<byte> Modulus { get; }
        public ImmutableArray<byte> PublicKeyExponent { get; }

        public RsaPublicKey(byte[] modulus, byte[] publicKeyExponent)
        {
            Modulus = modulus.ToImmutableArray();
            PublicKeyExponent = publicKeyExponent.ToImmutableArray();
        }
    }
}