using System;
using CSharpToolbox.UnitTesting;
using FluentAssertions;
using LAPI.Cryptography;
using LAPI.Domain.Contracts.Cryptography;
using LAPI.Domain.Model.Cryptography;

namespace LAPI.Test.Model.Cryptography
{
    public class CryptographyTestBase : GivenWhenThen
    {
        protected ICryptographicService CryptographicService;
        protected byte[] CipherText;
        protected byte[] PlainText;

        protected void AnAesCryptographicServiceWithValidKey()
        {
            CryptographicService = new AesCryptographicService(TestData.TestSymmetricKey);
        }

        protected Action DataIsEncrypted(params byte[] plaintext)
        {
            return () => CipherText = CryptographicService.Encrypt(plaintext);
        }

        protected void TheCipherTextIsDecrypted()
        {
            PlainText = CryptographicService.Decrypt(CipherText);
        }

        protected void TheDecryptedContentIsNotEmpty()
        {
            PlainText.Should().NotBeEmpty();
        }
        protected Action TheDecryptedContentIs(params byte[] plaintext)
        {
            return () => PlainText.Should().BeEquivalentTo(plaintext);
        }
        protected void TheCryptographicServiceCanEncrypt()
        {
            CryptographicService.CanEncrypt.Should().BeTrue();
        }
        protected void TheCryptographicServiceCanDecrypt()
        {
            CryptographicService.CanDecrypt.Should().BeTrue();
        }
    }
}