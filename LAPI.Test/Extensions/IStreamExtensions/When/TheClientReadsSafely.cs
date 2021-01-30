using LAPI.Extensions;

namespace LAPI.Test.Extensions.IStreamExtensions.When
{
    public class TheClientReadsSafely : IWhen<IClientResultTestBase<byte[]>, int>
    {
        public void When(IClientResultTestBase<byte[]> tbase, int length)
        {
            tbase.Result = tbase.Client.ReadSafelyAsync(length, tbase.Token).Result;
        }
    }
}