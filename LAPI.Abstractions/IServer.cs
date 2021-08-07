using System.IO;
using System.Threading.Tasks;

namespace LAPI.Abstractions
{
    public interface IServer
    {
        Task<Stream> AcceptClientAsync();
    }
}