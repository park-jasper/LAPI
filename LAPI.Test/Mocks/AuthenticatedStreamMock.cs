using System.IO;
using System.Net.Security;

namespace LAPI.Test.Mocks
{
    public class AuthenticatedStreamMock : AuthenticatedStream
    {
        public AuthenticatedStreamMock(
            Stream innerStream,
            bool leaveInnerStreamOpen,
            bool isServer)
            : base(innerStream, leaveInnerStreamOpen)
        {
            this.IsServer = isServer;
        }

        public override void Flush()
        {
            this.InnerStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.InnerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.InnerStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.InnerStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.InnerStream.Write(buffer, offset, count);
        }

        public override bool CanRead => this.InnerStream.CanRead;
        public override bool CanSeek => this.InnerStream.CanSeek;
        public override bool CanWrite => this.InnerStream.CanWrite;
        public override long Length => this.InnerStream.Length;

        public override long Position
        {
            get => this.InnerStream.Position;
            set => this.InnerStream.Position = value;
        }
        public override bool IsAuthenticated => false;
        public override bool IsMutuallyAuthenticated => false;
        public override bool IsEncrypted => false;
        public override bool IsSigned => false;
        public override bool IsServer { get; }
    }
}