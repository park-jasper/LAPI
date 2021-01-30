using System.Threading;
using LAPI.Contracts;

namespace LAPI.Test.Communication.EncryptedStream
{
    public abstract class StreamTestBase : TestSpecsAsync
    {
        protected IStream Client { get; set; }
        protected IStream Server { get; set; }
        protected CancellationToken Token => CancellationToken.None;
    }
}