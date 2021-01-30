using FluentAssertions;

namespace LAPI.Test.Extensions.IStreamExtensions.Then
{
    public class TheResultIsNotSuccessful : IThen<ICommunicationResultTestBase>
    {
        public void Then(ICommunicationResultTestBase tbase)
        {
            tbase.Result.Successful.Should().BeFalse();
        }
    }
}