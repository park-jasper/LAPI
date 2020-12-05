using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using FluentAssertions;
using LAPI.Extensions;
using LAPI.Model;

namespace LAPI.Test.Extensions.IStreamExtensions.MalfunctioningStreams
{
    [TestClass]
    public class When_TheClientReadsSafely : Given_AClientStreamThatThrowsAnException
    {
        private CommunicationResult<byte[]> ReadResult;
        public override async Task When()
        {
            await base.When();
            ReadResult = await Client.ReadSafelyAsync(4, Token);
        }

        [TestMethod]
        public void Then_TheReadResultWasNotSuccessful()
        {
            ReadResult.Successful.Should().BeFalse();
        }
    }
}