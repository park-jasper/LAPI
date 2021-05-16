using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LAPI.Test.Mocks
{
    public class StreamMock : Stream
    {
        private StreamMock _partner;
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

        private static byte[] Copy(byte[] buffer, int count)
        {
            int minLength = Math.Min(buffer.Length, count);
            var result = new byte[minLength];
            Array.Copy(buffer, result, minLength);
            return result;
        }

        private StreamMock()
        {

        }

        public static void Create(out Stream serverStream, out Stream clientStream)
        {
            var server = new StreamMock();
            var client = new StreamMock();
            Link(server, client);
            serverStream = server;
            clientStream = client;

        }
        private static void Link(StreamMock left, StreamMock right)
        {
            left._partner = right;
            right._partner = left;
        }

        public void Dispose()
        {

        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var copy = Copy(buffer, count);
            _partner.PutBytes(copy, count);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
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

                    return Task.FromResult(minLength).ToApm(callback, state);
                }
                else
                {
                    _currentTaskSource = new TaskCompletionSource<int>();
                    _currentRequestBufferQueue.Enqueue(buffer);
                    return _currentTaskSource.Task.ToApm(callback, state);
                }
            }
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            if (asyncResult is Task<int> task)
            {
                return task.Result;
            }
            else
            {
                throw new ArgumentException("not the right async result");
            }
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => 0;

        public override long Position
        {
            get => 0;
            set { }
        }
    }
}