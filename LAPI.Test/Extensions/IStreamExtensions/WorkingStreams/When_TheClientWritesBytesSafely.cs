using System.Threading.Tasks;
using FluentAssertions;
using LAPI.Extensions;
using LAPI.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LAPI.Test.Extensions.IStreamExtensions.WorkingStreams
{
    [TestClass]
    public class When_TheClientWritesBytesSafely : Given_AClientStreamAndAServerStream
    {
        private CommunicationResult WriteResult;
        private CommunicationResult<byte[]> ReadResult;

        public override async Task When()
        {
            await base.When();
            await DoParallel(
                async () => WriteResult = await Client.WriteSafelyAsync(new byte[] { 1, 2, 3, 4 }, Token),
                async () => ReadResult = await Server.ReadSafelyAsync(4, Token)
            );
        }

        [TestMethod]
        public void Then_TheWriteResultWasSuccessful()
        {
            WriteResult.Successful.Should().BeTrue();
        }

        [TestMethod]
        public void Then_TheReadResultWasSuccessful()
        {
            ReadResult.Successful.Should().BeTrue();
        }

        [TestMethod]
        public void Then_TheReadResultHasTheRightBytes()
        {
            ReadResult.Result.Should().BeEquivalentTo(1, 2, 3, 4);
        }
    }
}