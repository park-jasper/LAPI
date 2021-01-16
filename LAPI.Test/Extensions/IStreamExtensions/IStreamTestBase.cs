using System.Threading;
using LAPI.Contracts;

namespace LAPI.Test.Extensions.IStreamExtensions
{
    public abstract class IStreamTestBase : TestSpecsAsync
    {
        protected IStream Server;
        protected IStream Client;

        protected static CancellationToken Token = CancellationToken.None;
    }
}