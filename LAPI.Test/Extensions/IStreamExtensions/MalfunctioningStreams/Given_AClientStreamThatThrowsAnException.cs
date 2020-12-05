using System.Threading;
using System.Threading.Tasks;
using LAPI.Contracts;

namespace LAPI.Test.Extensions.IStreamExtensions.MalfunctioningStreams
{
    public class Given_AClientStreamThatThrowsAnException : IStreamExtensionsBase
    {
        public override async Task Given()
        {
            await base.Given();
            Client = new StreamMock();
        }

        private class StreamMock : IStream
        {
            public Task<int> ReadAsync(byte[] buffer, int count, CancellationToken token)
            {
                throw new System.NotImplementedException();
            }

            public Task WriteAsync(byte[] buffer, int count, CancellationToken token)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}