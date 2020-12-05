using System.Threading.Tasks;

namespace LAPI.Contracts
{
    public interface IServer
    {
        void Start();
        Task<IStream> AcceptClientAsync();
    }
}