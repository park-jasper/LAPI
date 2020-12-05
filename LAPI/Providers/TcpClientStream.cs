using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Contracts;

namespace LAPI.Providers
{
    public class TcpClientStream : IStream, IDisposable
    {
        private readonly TcpClient _client;
        private readonly BufferedStream _stream;

        public TcpClientStream(TcpClient tcpClient)
        {
            if (!tcpClient.Connected)
            {
                throw new ArgumentException(
                    $"Call TcpClient.ConnectAsync(IPAddress, int port) before creating a ClientStream");
            }
            _client = tcpClient;
            _stream = new BufferedStream(tcpClient.GetStream());
        }

        public Task<int> ReadAsync(byte[] buffer, int count, CancellationToken token)
        {
            return _stream.ReadAsync(buffer, 0, count, token);
        }

        public Task WriteAsync(byte[] buffer, int count, CancellationToken token)
        {
            return _stream.WriteAsync(buffer, 0, count, token);
        }

        public void Dispose()
        {
            _client?.Close();
            _stream?.Dispose();
        }
    }
}