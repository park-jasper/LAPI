using LAPI.Test.Extensions.IStreamExtensions.Given;
using LAPI.Test.Extensions.IStreamExtensions.Then;
using LAPI.Test.Extensions.IStreamExtensions.When;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LAPI.Test.Extensions.IStreamExtensions
{
    [TestClass]
    public class StreamExtensionTests : CommunicationResultTestBase
    {
        [TestMethod]
        public void MalfunctioningStream_ReadSafely()
        {
            Given<AClientStreamThatThrowsAnException>();
            When<TheClientWritesSafely, byte[]>(new byte[] { 1, 2, 3, 4 });
            Then<TheResultIsNotSuccessful>();
        }
    }

    [TestClass]
    public class IStreamBytesArrayTests : CommunicationResultTestBase<byte[]>
    {
        [TestMethod]
        public void MalfunctioningStream_WriteSafely()
        {
            Given<AClientStreamThatThrowsAnException>();
            When<TheClientReadsSafely, int>(4);
            Then<TheResultIsNotSuccessful>();
        }

        [TestMethod]
        public void WorkingStreams_ReadWriteTest()
        {
            Given<AClientStreamAndAServerStream>();
            When<DataIsSentAndReceived, byte[]>(new byte[] { 1, 2, 3, 4 });
            Then<TheResultIsSuccessful>();
            Then<TheReceivedDataIs, byte[]>(new byte[] { 1, 2, 3, 4 });
        }
    }
}