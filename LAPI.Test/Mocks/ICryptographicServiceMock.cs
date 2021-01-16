using LAPI.Model;

namespace LAPI.Test.Mocks
{
    public class ICryptographicServiceMock : ICryptographicService
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