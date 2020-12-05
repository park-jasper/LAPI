using System.Threading;
using System.Threading.Tasks;

namespace LAPI.Contracts
{
    public interface IStream
    {
        Task<int> ReadAsync(byte[] buffer, int count, CancellationToken token);
        Task WriteAsync(byte[] buffer, int count, CancellationToken token);
    }
}