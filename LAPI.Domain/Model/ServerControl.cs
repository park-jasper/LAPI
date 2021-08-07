using System;
using System.Threading;
using LAPI.Abstractions;
using LAPI.Domain.Communication;
using LAPI.Domain.Contracts;

namespace LAPI.Domain.Model
{
    public class ServerControl
    {
        private readonly IServer _server;
        private readonly CancellationTokenSource _tokenSource;

        public event Action<InitializationError> OnError;

        public ServerControl(IServer server, CancellationTokenSource tokenSource)
        {
            _server = server;
            _tokenSource = tokenSource;
        }

        public void ReportError(InitializationError error)
        {
            OnError?.Invoke(error);
        }

        public void StopServer()
        {
            _tokenSource.Cancel();
        }
    }
}