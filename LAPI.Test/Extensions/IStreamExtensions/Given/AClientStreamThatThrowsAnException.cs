using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LAPI.Contracts;

namespace LAPI.Test.Extensions.IStreamExtensions.Given
{
    public class AClientStreamThatThrowsAnException : IGiven<IClientTestBase>
    {
        public void Given(IClientTestBase tbase)
        {
            tbase.Client = new StreamMock();
        }

        private class StreamMock : Stream
        {
            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token)
            {
                throw new NotImplementedException();
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }

            public override bool CanRead => throw new NotImplementedException();
            public override bool CanSeek => throw new NotImplementedException();
            public override bool CanWrite => throw new NotImplementedException();
            public override long Length => throw new NotImplementedException();

            public override long Position
            {
                get => throw new NotImplementedException(); 
                set => throw new NotImplementedException();
            }
        }
    }
}