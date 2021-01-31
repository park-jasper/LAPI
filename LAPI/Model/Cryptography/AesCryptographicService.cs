using System;
using System.Linq;
using System.Security.Cryptography;

namespace LAPI.Model
{
    public class AesCryptographicService : ICryptographicService
    {
        private readonly SymmetricAlgorithm _aes;
        private readonly byte[] _key;
        private readonly RNGCryptoServiceProvider _random;
        public const int BitsPerByte = 8;
        public const int BlockSize = 128 / BitsPerByte;
        public const int IvSize = 128 / BitsPerByte;
        public const int KeySize = 256 / BitsPerByte;
        public AesCryptographicService(SymmetricKey key)
        {
            if (key.Key.Length != KeySize)
            {
                throw new ArgumentException($"Key does not have right size of {KeySize} bytes");
            }
            _aes = Aes.Create();
            _aes.Mode = CipherMode.CBC;
            _key = key.Key.ToArray();
            _random = new RNGCryptoServiceProvider();
        }
        public byte[] Encrypt(byte[] content)
        {
            var iv = new byte[IvSize];
            _random.GetBytes(iv);
            var encryptor = _aes.CreateEncryptor(_key, iv);
            var paddedContent = new byte[content.Length + BlockSize];
            _random.GetBytes(paddedContent, 0, BlockSize);
            Array.Copy(content, 0, paddedContent, BlockSize, content.Length);
            var cipher = encryptor.TransformFinalBlock(paddedContent, 0, paddedContent.Length);
            return cipher;
        }

        public byte[] Decrypt(byte[] cipher)
        {
            var decryptor = _aes.CreateDecryptor(_key, _aes.IV);
            var paddedResult = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
            var result = new byte[paddedResult.Length - BlockSize];
            Array.Copy(paddedResult, BlockSize, result, 0, result.Length);
            return result;
        }

        public bool CanEncrypt => true;
        public bool CanDecrypt => true;
    }
}