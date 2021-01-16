using LAPI.Test.Extensions.IStreamExtensions;
using System.Threading.Tasks;
using LAPI.Model;
using LAPI.Test.Mocks;

namespace LAPI.Test.Communication.EncryptedStream
{
    public class Given_EncryptedStreams : IStreamTestBase
    {
        protected ICryptographicService Crypto;
        public override async Task Given()
        {
            await base.Given();
            Crypto = new ICryptographicServiceMock();
            IStreamMock.Create(out var server, out var client);
            Server = new LAPI.Communication.EncryptedStream(Crypto, server);
            Client = new LAPI.Communication.EncryptedStream(Crypto, client);
        }
    }
}