using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LAPI.Test.Model.Cryptography
{
    [TestClass]
    public class AesTests : CryptographyTestBase
    {
        [TestMethod]
        public void AesCanEncrypt()
        {
            Given(AnAesCryptographicServiceWithValidKey);
            Then(TheCryptographicServiceCanEncrypt);
        }

        [TestMethod]
        public void AesCanDecrypt()
        {
            Given(AnAesCryptographicServiceWithValidKey);
            Then(TheCryptographicServiceCanDecrypt);
        }

        [TestMethod]
        public void EncryptDecryptTest()
        {
            Given(AnAesCryptographicServiceWithValidKey);
            When(DataIsEncrypted(1, 2, 3, 4, 5, 6, 7, 8))
                .And(TheCipherTextIsDecrypted);
            Then(TheDecryptedContentIs(1, 2, 3, 4, 5, 6, 7, 8));
        }
    }
}