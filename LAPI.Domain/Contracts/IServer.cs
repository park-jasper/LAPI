using System.IO;
using System.Threading.Tasks;

namespace LAPI.Domain.Contracts
{
    public interface IServer
    {
        void Start();
        Task<Stream> AcceptClientAsync();
    }
}