using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using FluentAssertions;

namespace LAPI.Test.Communication.EncryptedStream
{
    [TestClass]
    public class ReceiveTests : StreamTestBase
    {

        [TestMethod]
        public async Task TheServerCanReceiveTheBytes()
        {
            await Given(Streams)

                .When(TheClientSendsBytes(1, 2, 3, 4))
                .And(TheServerReadsBytesSafely(4))

                .Then(TheReadIsSuccessful)
                .And(TheResultIs(1, 2, 3, 4));
        }

        [TestMethod]
        public async Task Then_TheServerCanReceiveTheBytesInPortions()
        {
            await Given(Streams)

                .When(TheClientSendsBytes(1, 2, 3, 4, 5, 6, 7, 8))
                .And(TheServerReadsBytesSafely(4))

                .Then(TheReadIsSuccessful)
                .And(TheResultIs(1, 2, 3, 4));


            await When(TheServerReadsBytesSafely(4))

                .Then(TheReadIsSuccessful)
                .And(TheResultIs(5, 6, 7, 8));
        }
    }
}