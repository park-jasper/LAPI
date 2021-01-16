using System.Threading.Tasks;
using FluentAssertions;
using LAPI.Extensions;
using LAPI.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LAPI.Test.Extensions.IStreamExtensions.MalfunctioningStreams
{
    [TestClass]
    public class When_TheClientWritesSafely : Given_AClientStreamThatThrowsAnException
    {
        private CommunicationResult WriteResult;
        public override async Task When()
        {
            await base.When();
            WriteResult = await Client.WriteSafelyAsync(Token, new byte[] { 1, 2, 3, 4 });
        }

        [TestMethod]
        public void Then_TheWriteWasNotSuccessful()
        {
            WriteResult.Successful.Should().BeFalse();
        }
    }
}