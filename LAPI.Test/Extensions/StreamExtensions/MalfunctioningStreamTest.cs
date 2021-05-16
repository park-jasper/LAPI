using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LAPI.Test.Extensions.StreamExtensions
{
    [TestClass]
    public class MalfunctioningStreamTest : ByteArrayResultBase
    {
        [TestMethod]
        public async Task ReadSafely()
        {
            await Given(AClientStreamThatThrowsAnException)
                .When(TheClientReadsBytesSafely(4))
                .Then(TheResultIsNotSuccessful);
        }

        [TestMethod]
        public async Task WriteSafely()
        {
            await Given(AClientStreamThatThrowsAnException)
                .When(TheClientSendsBytesSafely(1, 2, 3, 4))
                .Then(TheResultIsNotSuccessful);
        }
    }
}