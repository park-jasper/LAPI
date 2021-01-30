using FluentAssertions;

namespace LAPI.Test.Extensions.IStreamExtensions.Then
{
    public class TheResultIsSuccessful : IThen<ICommunicationResultTestBase>
    {
        public void Then(ICommunicationResultTestBase tbase)
        {
            tbase.Result.Successful.Should().BeTrue();
        }
    }
}