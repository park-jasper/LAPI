using LAPI.Abstractions.Cryptography;
using LAPI.Domain.Contracts.Cryptography;
using LAPI.Domain.Model.Cryptography;

namespace LAPI.Test.Mocks
{
    public class CryptographicServiceMock : ICryptographicService
    {
        public byte[] Encrypt(byte[] content)
        {
            return content;
        }

        public byte[] Decrypt(byte[] cipher)
        {
            return cipher;
        }

        public bool CanEncrypt => true;
        public bool CanDecrypt => true;
    }
}