using System;
using System.Threading;
using System.Threading.Tasks;

namespace LAPI.Contracts
{
    public interface IStream : IDisposable
    {
        Task<int> ReadAsync(byte[] buffer, int count, CancellationToken token);
        Task WriteAsync(byte[] buffer, int count, CancellationToken token);
    }
}