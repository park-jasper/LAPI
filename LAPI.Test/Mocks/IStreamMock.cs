using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Contracts;

namespace LAPI.Test.Mocks
{
    public class IStreamMock : IStream
    {
        private IStreamMock _partner;
        private readonly Deque<byte[]> _currentBufferQueue = new Deque<byte[]>();
        private readonly Deque<byte[]> _currentRequestBufferQueue = new Deque<byte[]>();
        private TaskCompletionSource<int> _currentTaskSource { get; set; }

        private readonly object _completionLock = new object();

        private void PutBytes(byte[] buffer, int count)
        {
            lock (_completionLock)
            {
                if (_currentTaskSource != null)
                {
                    var requestBuffer = _currentRequestBufferQueue.Dequeue();
                    var minLength = Math.Min(Math.Min(count, buffer.Length), requestBuffer.Length);
                    Array.Copy(buffer, requestBuffer, minLength);
                    _currentTaskSource.SetResult(minLength);
                    _currentTaskSource = null;
                }
                else
                {
                    _currentBufferQueue.Enqueue(buffer);
                }
            }
        }
        public Task<int> ReadAsync(byte[] buffer, int count, CancellationToken token)
        {
            lock (_completionLock)
            {
                if (_currentBufferQueue.Count != 0)
                {
                    var availableBuffer = _currentBufferQueue.Dequeue();
                    var minLength = Math.Min(count, availableBuffer.Length);
                    Array.Copy(availableBuffer, buffer, minLength);
                    if (minLength < availableBuffer.Length)
                    {
                        var reinsert = new byte[availableBuffer.Length - minLength];
                        Array.Copy(availableBuffer, minLength, reinsert, 0, reinsert.Length);
                        _currentBufferQueue.PushFront(reinsert);
                    }
                    return Task.FromResult(minLength);
                }
                else
                {
                    _currentTaskSource = new TaskCompletionSource<int>();
                    _currentRequestBufferQueue.Enqueue(buffer);
                    return _currentTaskSource.Task;
                }
            }
        }

        public Task WriteAsync(byte[] buffer, int count, CancellationToken token)
        {
            var copy = Copy(buffer, count);
            _partner.PutBytes(copy, count);
            return Task.CompletedTask;
        }

        private static byte[] Copy(byte[] buffer, int count)
        {
            int minLength = Math.Min(buffer.Length, count);
            var result = new byte[minLength];
            Array.Copy(buffer, result, minLength);
            return result;
        }

        private IStreamMock()
        {

        }

        public static void Create(out IStream serverStream, out IStream clientStream)
        {
            var server = new IStreamMock();
            var client = new IStreamMock();
            Link(server, client);
            serverStream = server;
            clientStream = client;

        }
        private static void Link(IStreamMock left, IStreamMock right)
        {
            left._partner = right;
            right._partner = left;
        }

        public void Dispose()
        {

        }
    }
}