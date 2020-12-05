using FluentAssertions;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LAPI.Test.Model.Cryptography
{
    [TestClass]
    public class When_DataIsEncryptedAndThenDecryptedByAes : Given_AnAesCryptographicServiceWithValidKey
    {
        private static readonly byte[] Content = Enumerable.Range(1, 20).Select(x => (byte) x).ToArray();
        private byte[] _cipher;
        private byte[] _decryptedContent;
        public override void When()
        {
            base.When();
            _cipher = Aes.Encrypt(Content);
            _decryptedContent = Aes.Decrypt(_cipher);
        }

        [TestMethod]
        public void Then_TheDecryptedContentShouldNotBeEmpty()
        {
            _decryptedContent.Should().NotBeEmpty();
        }

        [TestMethod]
        public void Then_TheOriginalContentAndTheDecryptedContentContainTheSameValues()
        {
            _decryptedContent.Should().BeEquivalentTo(Content);
        }
    }
}