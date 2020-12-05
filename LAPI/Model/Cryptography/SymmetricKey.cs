using System;
using System.Collections.Immutable;
using System.Security.Cryptography;

namespace LAPI.Model
{
    public class SymmetricKey
    {
        private static readonly Lazy<RandomNumberGenerator> RandomLazy = new Lazy<RandomNumberGenerator>(() => new RNGCryptoServiceProvider());
        private static RandomNumberGenerator Random => RandomLazy.Value;
        public ImmutableArray<byte> Key { get; }

        private SymmetricKey(ImmutableArray<byte> key)
        {
            Key = key;
        }

        public static SymmetricKey FromBuffer(byte[] key)
        {
            return new SymmetricKey(key.ToImmutableArray());
        }

        public static SymmetricKey GenerateNewKey(int length)
        {
            var keyBuffer = new byte[length];
            Random.GetBytes(keyBuffer);
            return FromBuffer(keyBuffer);
        }
    }
}