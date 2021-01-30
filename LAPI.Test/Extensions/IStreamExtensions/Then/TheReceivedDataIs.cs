using FluentAssertions;

namespace LAPI.Test.Extensions.IStreamExtensions.Then
{
    public class TheReceivedDataIs : IThen<CommunicationResultTestBase<byte[]>, byte[]>
    {
        public void Then(CommunicationResultTestBase<byte[]> tbase, byte[] parameter)
        {
            tbase.Result.Result.Should().BeEquivalentTo(parameter);
        }
    }
}