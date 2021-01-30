using LAPI.Extensions;

namespace LAPI.Test.Extensions.IStreamExtensions.When
{
    public class TheClientWritesSafely : IWhen<CommunicationResultTestBase, byte[]>
    {
        public void When(CommunicationResultTestBase tbase, byte[] content)
        {
            tbase.Result = tbase.Client.WriteSafelyAsync(tbase.Token, content).Result;
        }
    }
}