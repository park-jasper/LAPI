using System.Threading;
using LAPI.Contracts;

namespace LAPI.Test.Extensions.IStreamExtensions
{
    public abstract class IStreamExtensionsBase : TestSpecsAsync
    {
        protected IStream Server;
        protected IStream Client;

        protected static CancellationToken Token = CancellationToken.None;
    }
}