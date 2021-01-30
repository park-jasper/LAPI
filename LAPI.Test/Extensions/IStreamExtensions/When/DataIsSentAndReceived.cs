using System.Threading.Tasks;
using LAPI.Extensions;

namespace LAPI.Test.Extensions.IStreamExtensions.When
{
    public class DataIsSentAndReceived : IWhen<IStreamResultTestBase<byte[]>, byte[]>
    {
        public void When(IStreamResultTestBase<byte[]> tbase, byte[] parameter)
        {
            Task.Run(async () =>
            {
                var write = tbase.Client.WriteSafelyAsync(tbase.Token, parameter);
                var read = tbase.Server.ReadSafelyAsync(parameter.Length, tbase.Token);
                await Task.WhenAll(write, read);
                tbase.Result = await read;
            }).Wait();
        }
    }
}