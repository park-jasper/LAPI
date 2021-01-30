using LAPI.Extensions;

namespace LAPI.Test.Extensions.IStreamExtensions.When
{
    public class TheClientReceivesAByteSafelyAsync : IWhen<IClientResultTestBase<byte>>
    {
        public void When(IClientResultTestBase<byte> tbase)
        {
            tbase.Result = tbase.Client.ReceiveByteSafelyAsync(tbase.Token).Result;
        }
    }
}