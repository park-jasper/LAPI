using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using FluentAssertions;
using LAPI.Extensions;

namespace LAPI.Test.Communication.EncryptedStream
{
    [TestClass]
    public class When_TheClientSendsBytes : Given_EncryptedStreams
    {
        private byte[] _buffer = { 1, 2, 3, 4, 5, 6, 7, 8 };
        public override async Task When()
        {
            await base.When();
            await Client.WriteAsync(_buffer, 8, Token);
        }

        [TestMethod]
        public async Task Then_TheServerCanReceiveTheBytes()
        {
            var result = await Server.ReadSafelyAsync(8, Token);
            result.Successful.Should().BeTrue();
            var buffer = result.Result;
            buffer.Length.Should().Be(8);
            buffer.StartsWith(_buffer).Should().BeTrue();
        }

        [TestMethod]
        public async Task Then_TheServerCanReceiveTheBytesInPortions()
        {
            var firstResult = await Server.ReadSafelyAsync(4, Token);
            firstResult.Successful.Should().BeTrue();
            var firstBuffer = firstResult.Result;
            firstBuffer.Length.Should().Be(4);
            for (int i = 0; i < 4; i += 1)
            {
                firstBuffer[i].Should().Be(_buffer[i]);
            }

            var secondResult = await Server.ReadSafelyAsync(4, Token);
            secondResult.Successful.Should().BeTrue();
            var secondBuffer = secondResult.Result;
            secondBuffer.Length.Should().Be(4);
            for (int i = 0; i < 4; i += 1)
            {
                secondBuffer[i].Should().Be(_buffer[i + 4]);
            }
        }
    }
}