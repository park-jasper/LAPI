using System.Threading.Tasks;
using FluentAssertions;
using LAPI.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LAPI.Test.Extensions.IStreamExtensions.WorkingStreams
{
    [TestClass]
    public class When_TheClientWritesBytes : Given_AClientStreamAndAServerStream
    {
        private byte[] Result;
        public override async Task When()
        {
            await base.When();

            Result = new byte[4];
            await DoParallel(
                () => Client.WriteAsync(new byte[] { 1, 2, 3, 4 }, Token),
                () => Server.ReadAsync(Result, Token)
            );
        }

        [TestMethod]
        public void Then_TheReceivedBytesMatch()
        {
            Result.Should().BeEquivalentTo(1, 2, 3, 4);
        }
    }
}