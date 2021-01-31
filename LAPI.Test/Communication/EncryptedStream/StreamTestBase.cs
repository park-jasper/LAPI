using System.IO;
using System.Threading;
using LAPI.Contracts;

namespace LAPI.Test.Communication.EncryptedStream
{
    public abstract class StreamTestBase : TestSpecsAsync
    {
        protected Stream Client { get; set; }
        protected Stream Server { get; set; }
        protected CancellationToken Token => CancellationToken.None;
    }
}