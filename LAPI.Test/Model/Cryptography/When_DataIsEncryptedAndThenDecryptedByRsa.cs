using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LAPI.Test.Model.Cryptography
{
    [TestClass]
    public class When_DataIsEncryptedAndThenDecryptedByRsa : Given_AnRsaCryptographicServiceWithAValidKeyPair
    {
        private byte[] _content;
        private byte[] _cipher;
        private byte[] _decryptedContent;
        public override void When()
        {
            base.When();
            _content = TestData.TestSymmetricKey.Key.ToArray();
            _cipher = Rsa.Encrypt(_content);
            _decryptedContent = Rsa.Decrypt(_cipher);
        }

        [TestMethod]
        public void Then_TheDecryptedContentShouldNotBeEmpty()
        {
            _decryptedContent.Should().NotBeEmpty();
        }

        [TestMethod]
        public void Then_TheOriginalContentAndTheDecryptedContentContainTheSameValues()
        {
            _decryptedContent.Should().BeEquivalentTo(_content);
        }
    }
}