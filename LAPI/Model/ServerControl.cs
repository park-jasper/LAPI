using System.Threading;
using LAPI.Contracts;

namespace LAPI.Model
{
    public class ServerControl
    {
        private readonly IServer _server;
        private readonly CancellationTokenSource _tokenSource;

        public ServerControl(IServer server, CancellationTokenSource tokenSource)
        {
            _server = server;
            _tokenSource = tokenSource;
        }

        public void StopServer()
        {
            _tokenSource.Cancel();
        }
    }
}